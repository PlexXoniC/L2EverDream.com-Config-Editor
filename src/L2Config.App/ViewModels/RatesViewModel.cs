using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Client;
using L2Config.Core.Rates;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>
/// The Rates tab: pick how much faster your world should be than retail, see exactly what that does to a real monster,
/// and apply it. The values land in the Server tab as unsaved changes, so they are saved, backed up and undone like any
/// other setting.
/// </summary>
public sealed class RatesViewModel : ObservableObject
{
	private readonly Func<IReadOnlyList<SettingViewModel>> _serverSettings;
	private readonly Func<L2Locations?> _locations;
	private readonly Func<Task<MonsterCatalog>> _loadMonsters;
	private readonly Action<string> _showSetting;
	private readonly Action _applied;
	private MonsterCatalog? _monsters;
	private IconLibrary? _icons;
	private MonsterRowViewModel? _selected;
	private double _rate = 1;
	private double _delivery = 0.5;
	private string _rateText = "1";
	private string _searchText = "";
	private string? _status;
	private bool _isLoading = true;

	public RatesViewModel(Func<IReadOnlyList<SettingViewModel>> serverSettings, Func<L2Locations?> locations,
		Func<Task<MonsterCatalog>> loadMonsters, Action<string> showSetting, Action applied)
	{
		_serverSettings = serverSettings;
		_locations = locations;
		_loadMonsters = loadMonsters;
		_showSetting = showSetting;
		_applied = applied;
		PresetCommand = new RelayCommand(p => Rate = double.TryParse(p as string, NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : Rate);
		ApplyCommand = new RelayCommand(Apply, () => HasServer);
		ShowSettingCommand = new RelayCommand(p => { if (p is string id) showSetting(id); });
		RefreshCommand = new RelayCommand(() => _ = LoadAsync());
	}

	public string Title => "Rates";

	public string Intro =>
		"Pick how much faster than retail your world should be. One number sets experience, skill points, drops, spoil, " +
		"quest rewards and adena together, and the preview shows exactly what it does to a real monster — including the " +
		"parts of Lineage 2 that quietly waste a rate if you set it the obvious way.";

	public bool HasServer => _locations() is { HasServer: true };

	public bool IsLoading
	{
		get => _isLoading;
		private set => Set(ref _isLoading, value);
	}

	// ------------------------------------------------------------------ the rate

	public double Rate
	{
		get => _rate;
		set
		{
			var clamped = Math.Clamp(Math.Round(value, 2), RateOptions.MinRate, RateOptions.MaxRate);
			if (Set(ref _rate, clamped))
			{
				_rateText = RateText(clamped);
				OnPropertyChanged(nameof(RateInput));
				RaisePlan();
			}
		}
	}

	/// <summary>What the user types in the "custom" box; bad text is left alone until it makes sense.</summary>
	public string RateInput
	{
		get => _rateText;
		set
		{
			if (!Set(ref _rateText, value ?? ""))
			{
				return;
			}
			if (double.TryParse(_rateText.Trim().TrimEnd('x', 'X', '×'), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
			{
				Rate = parsed;
			}
			OnPropertyChanged(nameof(RateProblem));
		}
	}

	public string? RateProblem =>
		double.TryParse(_rateText.Trim().TrimEnd('x', 'X', '×'), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
			? parsed is >= RateOptions.MinRate and <= RateOptions.MaxRate ? null : $"Rates go from {RateOptions.MinRate}x to {RateOptions.MaxRate}x."
			: "Type a number, for example 5.";

	/// <summary>0 = drops come more often, 1 = drops come in bigger stacks.</summary>
	public double Delivery
	{
		get => _delivery;
		set
		{
			if (Set(ref _delivery, Math.Clamp(value, 0, 1)))
			{
				RaisePlan();
			}
		}
	}

	public RateOptions Options => new(Rate, Delivery);
	public RatePlan Plan => RatePlan.Build(Options);

	public IReadOnlyList<double> Presets => RateOptions.Presets;

	public string DeliveryText => Delivery switch
	{
		<= 0.01 => $"Drops come more often: chance ×{RatePlan.Text(Options.ChanceMultiplier)}, amount ×{RatePlan.Text(Options.AmountMultiplier)}.",
		>= 0.99 => $"Drops come in bigger stacks: chance unchanged, amount ×{RatePlan.Text(Options.AmountMultiplier)}.",
		_ => $"Chance ×{RatePlan.Text(Options.ChanceMultiplier)}, amount ×{RatePlan.Text(Options.AmountMultiplier)} — together {RatePlan.Text(Rate)}×.",
	};

	public string SummaryText => Rate <= 1
		? "Retail rates: everything exactly as Lineage 2 shipped it."
		: $"{RatePlan.Text(Rate)}× experience, skill points, drops, spoil, adena and quest rewards. " +
		  $"Raid boss drops move less, at {RatePlan.Text(Options.RaidRate)}×.";

	/// <summary>What the world is set to right now, read from the files.</summary>
	public string CurrentText
	{
		get
		{
			if (!HasServer)
			{
				return "Choose your server folder in the Server tab first.";
			}
			var xp = RatePlan.RateOf(Find("Rates.ini", "RateXp")?.Value);
			var amount = RatePlan.RateOf(Find("Rates.ini", "DeathDropAmountMultiplier")?.Value);
			var chance = RatePlan.RateOf(Find("Rates.ini", "DeathDropChanceMultiplier")?.Value);
			if (xp is null)
			{
				return "Your world's current rates could not be read.";
			}
			var drops = (amount ?? 1) * (chance ?? 1);
			return $"Right now your world runs at {RatePlan.Text(xp.Value)}× experience and {RatePlan.Text(Math.Round(drops, 2))}× drops.";
		}
	}

	public ObservableCollection<RateChangeViewModel> Changes { get; } = [];
	public IReadOnlyList<string> Untouched => Plan.Untouched;

	// ------------------------------------------------------------------ the preview

	public ObservableCollection<MonsterRowViewModel> Monsters { get; } = [];

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (Set(ref _searchText, value ?? ""))
			{
				FilterMonsters();
			}
		}
	}

	public MonsterRowViewModel? Selected
	{
		get => _selected;
		set
		{
			if (Set(ref _selected, value))
			{
				RaisePreview();
			}
		}
	}

	public MonsterPreview? Preview => _selected is null || _monsters is null
		? null
		: DropPreview.For(_selected.Monster, _monsters, Options, MaxDifferentItems(_selected.Monster));

	public ObservableCollection<DropRowViewModel> DropRows { get; } = [];

	public string? PreviewHeader => Preview is { } p
		? $"{p.Monster.Name}  ·  level {p.Monster.Level}{(p.Monster.IsRaid ? "  ·  raid boss" : "")}"
		: null;

	public string? ExperienceText => Preview is { } p
		? $"{p.ExpBefore:N0} → {p.ExpAfter:N0} XP   ·   {p.SpBefore:N0} → {p.SpAfter:N0} SP"
		: null;

	public string? AdenaText => Preview is { AdenaBefore: > 0 } p
		? $"{p.AdenaBefore:N0} → {p.AdenaAfter:N0} adena per kill on average"
		: null;

	public string? LimitText => Preview is { } p && p.HitsItemLimit
		? $"This monster now has {p.GroupsAtFullChance} drop groups that always fire, but the server only gives {p.MaxDifferentItems} " +
		  "different items per kill, so some of that chance is wasted. Raising the amount instead of the chance avoids this."
		: null;

	public string? Status
	{
		get => _status;
		private set => Set(ref _status, value);
	}

	public RelayCommand PresetCommand { get; }
	public RelayCommand ApplyCommand { get; }
	public RelayCommand ShowSettingCommand { get; }
	public RelayCommand RefreshCommand { get; }

	public string ApplyText => $"Apply {RatePlan.Text(Rate)}× to my world";

	public async Task LoadAsync()
	{
		if (!HasServer)
		{
			IsLoading = false;
			RaisePlan();
			return;
		}
		IsLoading = true;
		try
		{
			_monsters = await _loadMonsters();
			_icons = IconLibrary.ForClient(_locations()?.ClientSystemDir);
			FilterMonsters();
			Selected ??= Monsters.FirstOrDefault();
			Rate = RatePlan.RateOf(Find("Rates.ini", "RateXp")?.Value) ?? 1;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Xml.XmlException)
		{
			Status = "The monster list could not be read: " + ex.Message;
		}
		finally
		{
			IsLoading = false;
			RaisePlan();
		}
	}

	private int MaxDifferentItems(Monster monster) =>
		int.TryParse(Find("Rates.ini", monster.IsRaid ? "DropMaxOccurrencesRaidboss" : "DropMaxOccurrencesNormal")?.Value, out var n) && n > 0
			? n
			: monster.IsRaid ? 7 : 2;

	private SettingViewModel? Find(string file, string key) =>
		_serverSettings().FirstOrDefault(s => s.Definition.File == file && s.Definition.Key == key);

	private void FilterMonsters()
	{
		Monsters.Clear();
		if (_monsters is null)
		{
			return;
		}
		var words = _searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var matches = _monsters.All
			.Where(m => m.HasDrops && m.Exp > 0)
			.Where(m => words.All(w => m.Name.Contains(w, StringComparison.OrdinalIgnoreCase) || m.Id.ToString() == w))
			.Take(400);
		foreach (var monster in matches)
		{
			Monsters.Add(new MonsterRowViewModel(monster));
		}
		OnPropertyChanged(nameof(MonsterCountText));
	}

	public string MonsterCountText => Monsters.Count switch
	{
		0 => "No monster matches.",
		1 => "1 monster",
		var n => $"{n} monsters{(n >= 400 ? " (type to narrow it down)" : "")}",
	};

	private void Apply()
	{
		var applied = 0;
		var skipped = new List<string>();
		foreach (var change in Plan.Changes)
		{
			var setting = Find(change.File, change.Key);
			if (setting is null || !setting.IsEditable)
			{
				skipped.Add(change.Key);
				continue;
			}
			setting.Value = change.NewValue;
			applied++;
		}
		_applied();
		Status = applied == 0
			? "Nothing was changed: those settings could not be found in your files."
			: $"{applied} settings set for a {RatePlan.Text(Rate)}× world. Press Save changes to write them, then restart your world." +
			  (skipped.Count > 0 ? $" {skipped.Count} could not be set: {string.Join(", ", skipped)}." : "");
		OnPropertyChanged(nameof(CurrentText));
	}

	private void RaisePlan()
	{
		Changes.Clear();
		foreach (var change in Plan.Changes)
		{
			Changes.Add(new RateChangeViewModel(change, Find(change.File, change.Key), ShowSettingCommand));
		}
		OnPropertyChanged(nameof(Options));
		OnPropertyChanged(nameof(Plan));
		OnPropertyChanged(nameof(SummaryText));
		OnPropertyChanged(nameof(DeliveryText));
		OnPropertyChanged(nameof(ApplyText));
		OnPropertyChanged(nameof(Untouched));
		OnPropertyChanged(nameof(CurrentText));
		OnPropertyChanged(nameof(RateProblem));
		RaisePreview();
	}

	private void RaisePreview()
	{
		DropRows.Clear();
		if (Preview is { } preview)
		{
			foreach (var row in preview.Drops.Concat(preview.Spoil).OrderByDescending(r => r.PerKillAfter))
			{
				DropRows.Add(new DropRowViewModel(row, _icons, _monsters));
			}
		}
		OnPropertyChanged(nameof(Preview));
		OnPropertyChanged(nameof(PreviewHeader));
		OnPropertyChanged(nameof(ExperienceText));
		OnPropertyChanged(nameof(AdenaText));
		OnPropertyChanged(nameof(LimitText));
		OnPropertyChanged(nameof(HasPreview));
	}

	public bool HasPreview => DropRows.Count > 0;

	private static string RateText(double value) => RatePlan.Text(value);
}

public sealed class MonsterRowViewModel(Monster monster)
{
	public Monster Monster { get; } = monster;
	public string Name => Monster.Name;
	public string Details => $"level {Monster.Level}  ·  {Monster.Exp:N0} XP{(Monster.IsRaid ? "  ·  raid boss" : "")}";
}

public sealed class RateChangeViewModel(RateChange change, SettingViewModel? setting, RelayCommand showSetting)
{
	public string Location => change.Location;
	public string Name => setting?.Name ?? change.Key;
	public string NewValue => change.NewValue;
	public string OldValue => setting?.Value is { Length: > 0 } value ? value : "—";
	public string Why => change.Why;
	public bool Changes => setting is not null && setting.Value != change.NewValue;
	public string? SettingId => setting?.Definition.Id;
	public bool CanShow => SettingId is not null;
	public RelayCommand ShowCommand { get; } = showSetting;
}

public sealed class DropRowViewModel
{
	public DropRowViewModel(DropRow row, IconLibrary? icons, MonsterCatalog? catalog)
	{
		Row = row;
		Name = row.ItemName;
		Image = ItemIcon.For(icons, catalog?.IconName(row.ItemId));
		ChanceText = $"{Format(row.ChanceBefore)}% → {Format(row.ChanceAfter)}%";
		AmountText = row.AmountBefore == row.AmountAfter ? Format(row.AmountBefore) : $"{Format(row.AmountBefore)} → {Format(row.AmountAfter)}";
		PerKillText = $"{Format(row.PerKillBefore)} → {Format(row.PerKillAfter)}";
		RealText = row.Unchanged ? "unchanged" : $"×{row.RealMultiplier:0.##}";
		Note = row.IsHerb ? "Herb — left as it is"
			: row.ChanceIsFull ? "Always drops now; more chance would be wasted"
			: row.IsAdena ? "Adena uses its own rate" : null;
	}

	public DropRow Row { get; }
	public string Name { get; }
	/// <summary>The item's icon from the player's own game client, when it can be read.</summary>
	public System.Windows.Media.ImageSource? Image { get; }

	public bool HasImage => Image is not null;
	public string ChanceText { get; }
	public string AmountText { get; }
	public string PerKillText { get; }
	public string RealText { get; }
	public string? Note { get; }
	public bool IsSpoil => Row.IsSpoil;
	public string KindWord => Row.IsSpoil ? "Spoil" : "Drop";

	private static string Format(double value) => value switch
	{
		0 => "0",
		< 0.01 => value.ToString("0.####", CultureInfo.InvariantCulture),
		< 1 => value.ToString("0.##", CultureInfo.InvariantCulture),
		< 1000 => value.ToString("0.#", CultureInfo.InvariantCulture),
		_ => value.ToString("N0", CultureInfo.InvariantCulture),
	};
}
