using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Client;
using L2Config.Core.Rates;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>Which two sets of rates the Drops tab puts side by side.</summary>
public enum DropComparison
{
	/// <summary>Retail (what a drop table site lists) against the world as it is set now.</summary>
	RetailToNow,

	/// <summary>The world as it is set now against the rate picked on the Rates tab.</summary>
	NowToPlanned,

	/// <summary>Retail against the rate picked on the Rates tab.</summary>
	RetailToPlanned,
}

/// <summary>
/// The Drops tab: pick any monster in the world and see what it really gives — experience, every drop and spoil item,
/// how often and how much. The rates it compares come from the Rates tab, so the two work as a pair: choose a rate
/// there, see what it does to real monsters here.
/// </summary>
public sealed class DropsViewModel : ObservableObject
{
	private readonly RatesViewModel _rates;
	private readonly Func<IReadOnlyList<SettingViewModel>> _serverSettings;
	private readonly Func<L2Locations?> _locations;
	private readonly Func<Task<MonsterCatalog>> _loadMonsters;
	private MonsterCatalog? _monsters;
	private IconLibrary? _icons;
	private MonsterArtLibrary? _art;
	private IReadOnlyList<System.Windows.Media.ImageSource>? _turn;
	private System.Windows.Threading.DispatcherTimer? _turning;
	private int _frame;
	private int _artFor;
	private MonsterRowViewModel? _selected;
	private DropComparison _comparison = DropComparison.RetailToNow;
	private string _searchText = "";
	private string? _status;
	private bool _isLoading = true;

	public DropsViewModel(RatesViewModel rates, Func<IReadOnlyList<SettingViewModel>> serverSettings, Func<L2Locations?> locations,
		Func<Task<MonsterCatalog>> loadMonsters, Action showRates)
	{
		_rates = rates;
		_serverSettings = serverSettings;
		_locations = locations;
		_loadMonsters = loadMonsters;
		_rates.RatesChanged += RatesChanged;
		ComparisonCommand = new RelayCommand(p => Comparison = Enum.TryParse<DropComparison>(p as string, out var parsed) ? parsed : Comparison);
		ShowRatesCommand = new RelayCommand(showRates);
	}

	public string Title => "Drops";

	public string Intro =>
		"Every monster in your world, with what it really gives: experience, how often each item drops, how much, and the " +
		"average per kill. Retail is what a drop table site lists; your world is whatever your rates are set to now.";

	public bool HasServer => _locations() is { HasServer: true };

	public bool IsLoading
	{
		get => _isLoading;
		private set => Set(ref _isLoading, value);
	}

	// ------------------------------------------------------------------ which rates are compared

	public DropComparison Comparison
	{
		get => _comparison;
		set
		{
			if (Set(ref _comparison, value))
			{
				RaisePreview();
			}
		}
	}

	public bool IsRetailToNow => Comparison == DropComparison.RetailToNow;
	public bool IsNowToPlanned => Comparison == DropComparison.NowToPlanned;
	public bool IsRetailToPlanned => Comparison == DropComparison.RetailToPlanned;

	/// <summary>The world as its settings stand, including changes that are not saved yet.</summary>
	public RateSettings Now => RateSettings.Read(key => Find("Rates.ini", key)?.Value);

	public RateSettings Planned => RateSettings.FromPlan(_rates.Options);

	public RateSettings Before => Comparison == DropComparison.NowToPlanned ? Now : RateSettings.Retail;

	public RateSettings After => Comparison == DropComparison.RetailToNow ? Now : Planned;

	public string BeforeLabel => Comparison == DropComparison.NowToPlanned ? "your world" : "retail";

	public string AfterLabel => Comparison == DropComparison.RetailToNow ? "your world" : $"{RatePlan.Text(_rates.Rate)}x planned";

	public string PlannedPillText => $"{RatePlan.Text(_rates.Rate)}× planned";

	public string ComparisonText => Comparison switch
	{
		DropComparison.RetailToNow =>
			$"Retail → your world as it is set now: {RatePlan.Text(Now.Xp)}× experience, {RatePlan.Text(Now.DropsOverall)}× drops.",
		DropComparison.NowToPlanned =>
			$"Your world now → the {RatePlan.Text(_rates.Rate)}× picked on the Rates tab. Nothing is written until you apply it there.",
		_ => $"Retail → the {RatePlan.Text(_rates.Rate)}× picked on the Rates tab.",
	};

	/// <summary>Shown when both sides are identical, so an unchanged list has a reason next to it.</summary>
	public string? SameText => Preview is { IsSameBothWays: true }
		? Comparison == DropComparison.RetailToNow
			? "Your world gives this monster exactly what retail does."
			: "The rate on the Rates tab comes to what this world already gives, so nothing moves."
		: null;

	// ------------------------------------------------------------------ the monsters

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
		: DropPreview.Between(_selected.Monster, _monsters, Before, After, MaxDifferentItems(_selected.Monster));

	public ObservableCollection<DropRowViewModel> DropRows { get; } = [];

	public bool HasPreview => DropRows.Count > 0;

	public string? MonsterName => Preview?.Monster.Name;

	public string? MonsterKind => Preview is { } p ? p.Monster.IsRaid ? "Raid boss" : "Monster" : null;

	public string? MonsterLevel => Preview is { } p ? p.Monster.Level.ToString(CultureInfo.InvariantCulture) : null;

	public string? MonsterHealth => Preview is { Monster.Hp: > 0 } p ? $"{p.Monster.Hp:N0} HP" : null;

	/// <summary>
	/// The monster itself, drawn from the model in the player's own game client. The client has no pictures of
	/// monsters, only 3D models, so one is drawn on demand; until it is ready (or when this client has no model for
	/// this monster) the frame shows what is known about it instead.
	/// </summary>
	public System.Windows.Media.ImageSource? MonsterArt => _turn is { Count: > 0 } ? _turn[_frame % _turn.Count] : null;

	public bool HasMonsterArt => _turn is { Count: > 0 };

	/// <summary>True while a monster is being drawn, so a screenshot can wait for it.</summary>
	public bool IsDrawing { get; private set; }

	public string? ExperienceText => Preview is { } p
		? $"{p.ExpBefore:N0} → {p.ExpAfter:N0} XP   ·   {p.SpBefore:N0} → {p.SpAfter:N0} SP"
		: null;

	public string? AdenaText => Preview is { AdenaBefore: > 0 } p
		? $"{p.AdenaBefore:N0} → {p.AdenaAfter:N0} adena per kill on average"
		: null;

	public string? LimitText => Preview is { } p && p.HitsItemLimit
		? $"{p.GroupsAtFullChance} of this monster's drop groups always fire, but the server only gives {p.MaxDifferentItems} " +
		  "different items per kill, so some of that chance is wasted. Raising the amount instead of the chance avoids this."
		: null;

	public string? Status
	{
		get => _status;
		private set => Set(ref _status, value);
	}

	public RelayCommand ComparisonCommand { get; }
	public RelayCommand ShowRatesCommand { get; }

	public async Task LoadAsync()
	{
		if (!HasServer)
		{
			IsLoading = false;
			RaisePreview();
			return;
		}
		IsLoading = true;
		try
		{
			_monsters = await _loadMonsters();
			var clientSystem = _locations()?.ClientSystemDir;
			_icons = IconLibrary.ForClient(clientSystem);
			_art ??= await Task.Run(() => MonsterArtLibrary.ForClient(clientSystem));
			_rates.Reset();                      // so "planned" starts at the rate this world already runs
			FilterMonsters();
			Selected ??= Monsters.FirstOrDefault();
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Xml.XmlException)
		{
			Status = "The monster list could not be read: " + ex.Message;
		}
		finally
		{
			IsLoading = false;
			RaisePreview();
		}
	}

	/// <summary>The Rates tab changed its rate or applied one, so the planned and "now" columns move with it.</summary>
	public void RatesChanged()
	{
		OnPropertyChanged(nameof(PlannedPillText));
		RaisePreview();
	}

	/// <summary>Draws the selected monster in the background; a later selection wins over an earlier one.</summary>
	private async Task DrawSelectedAsync()
	{
		var monster = _selected?.Monster;
		if (_art is null || monster is null || monster.Id == _artFor)
		{
			return;
		}
		_artFor = monster.Id;
		_turn = null;
		_frame = 0;
		Turning(false);
		IsDrawing = true;
		OnPropertyChanged(nameof(MonsterArt));
		OnPropertyChanged(nameof(HasMonsterArt));

		var frames = await Infrastructure.MonsterArt.TurnAsync(_art, monster.Id);
		if (_artFor != monster.Id)
		{
			return;                              // the user moved on while it was being drawn
		}
		_turn = frames;
		_frame = 0;
		IsDrawing = false;
		OnPropertyChanged(nameof(MonsterArt));
		OnPropertyChanged(nameof(HasMonsterArt));
		Turning(frames is { Count: > 1 });
	}

	/// <summary>Stops the turn on the frame that faces the viewer — what a screenshot should catch.</summary>
	public void ShowFacingFrame()
	{
		Turning(false);
		_frame = 0;
		OnPropertyChanged(nameof(MonsterArt));
	}

	/// <summary>The monster turns on the spot while you look at it; a still monster costs nothing to show.</summary>
	private void Turning(bool on)
	{
		if (!on)
		{
			_turning?.Stop();
			return;
		}
		_turning ??= new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Background)
		{
			Interval = TimeSpan.FromMilliseconds(80),
		};
		_turning.Tick -= Turn;
		_turning.Tick += Turn;
		_turning.Start();
	}

	private void Turn(object? sender, EventArgs e)
	{
		if (_turn is not { Count: > 1 })
		{
			_turning?.Stop();
			return;
		}
		_frame = (_frame + 1) % _turn.Count;
		OnPropertyChanged(nameof(MonsterArt));
	}

	private int MaxDifferentItems(Monster monster) =>
		int.TryParse(Find("Rates.ini", monster.IsRaid ? "DropMaxOccurrencesRaidboss" : "DropMaxOccurrencesNormal")?.Value, out var n) && n > 0
			? n
			: monster.IsRaid ? 7 : 2;

	private SettingViewModel? Find(string file, string key) =>
		_serverSettings().FirstOrDefault(s => s.Definition.File == file && s.Definition.Key == key);

	private void FilterMonsters()
	{
		var keep = Selected?.Monster.Id;
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
		// Keep looking at the same monster while the list is narrowed, as long as it is still in it.
		if (keep is { } id && Monsters.FirstOrDefault(m => m.Monster.Id == id) is { } still)
		{
			_selected = still;
			OnPropertyChanged(nameof(Selected));
		}
	}

	/// <summary>
	/// Several drop groups holding one item, as a single row: the chance becomes the chance of getting it at all, and
	/// the amount is what it gives on the kills where it drops, so the average per kill still adds up.
	/// </summary>
	private static DropRow Combine(IReadOnlyList<DropRow> rows)
	{
		double MissAll(Func<DropRow, double> chance) => rows.Aggregate(1.0, (miss, r) => miss * (1 - Math.Clamp(chance(r), 0, 100) / 100));
		var chanceBefore = (1 - MissAll(r => r.ChanceBefore)) * 100;
		var chanceAfter = (1 - MissAll(r => r.ChanceAfter)) * 100;
		var perKillBefore = rows.Sum(r => r.PerKillBefore);
		var perKillAfter = rows.Sum(r => r.PerKillAfter);
		var first = rows[0];
		return first with
		{
			ChanceBefore = chanceBefore,
			ChanceAfter = chanceAfter,
			AmountBefore = chanceBefore <= 0 ? 0 : perKillBefore / (chanceBefore / 100),
			AmountAfter = chanceAfter <= 0 ? 0 : perKillAfter / (chanceAfter / 100),
		};
	}

	public string MonsterCountText => Monsters.Count switch
	{
		0 => "No monster matches.",
		1 => "1 monster",
		var n => $"{n} monsters{(n >= 400 ? " (type to narrow it down)" : "")}",
	};

	private void RaisePreview()
	{
		DropRows.Clear();
		if (Preview is { } preview)
		{
			// A monster can give the same item from several drop groups (Antharas has five that hold adena); each is
			// rolled on its own, so they are added up into one row rather than listed over and over.
			var groups = preview.Drops.Concat(preview.Spoil)
				.GroupBy(r => (r.ItemId, r.IsSpoil))
				.Select(g => (Row: g.Count() == 1 ? g.First() : Combine(g.ToList()), Groups: g.Count()))
				.OrderByDescending(r => r.Row.PerKillAfter);
			foreach (var (row, count) in groups)
			{
				DropRows.Add(new DropRowViewModel(row, _icons, _monsters, count));
			}
		}
		_ = DrawSelectedAsync();
		foreach (var name in new[]
		{
			nameof(Preview), nameof(HasPreview), nameof(MonsterName), nameof(MonsterKind), nameof(MonsterLevel),
			nameof(MonsterHealth), nameof(MonsterArt), nameof(HasMonsterArt), nameof(ExperienceText), nameof(AdenaText),
			nameof(LimitText), nameof(ComparisonText), nameof(SameText), nameof(BeforeLabel), nameof(AfterLabel),
			nameof(IsRetailToNow), nameof(IsNowToPlanned), nameof(IsRetailToPlanned),
		})
		{
			OnPropertyChanged(name);
		}
	}
}

public sealed class MonsterRowViewModel(Monster monster)
{
	public Monster Monster { get; } = monster;
	public string Name => Monster.Name;
	public string Details => $"level {Monster.Level}  ·  {Monster.Exp:N0} XP{(Monster.IsRaid ? "  ·  raid boss" : "")}";
	public bool IsRaid => Monster.IsRaid;
}

public sealed class DropRowViewModel
{
	public DropRowViewModel(DropRow row, IconLibrary? icons, MonsterCatalog? catalog, int groups = 1)
	{
		Row = row;
		Groups = groups;
		Name = row.ItemName;
		Image = ItemIcon.For(icons, catalog?.IconName(row.ItemId));
		ChanceText = $"{Format(row.ChanceBefore)}% → {Format(row.ChanceAfter)}%";
		AmountText = row.AmountBefore == row.AmountAfter ? Format(row.AmountBefore) : $"{Format(row.AmountBefore)} → {Format(row.AmountAfter)}";
		PerKillText = $"{Format(row.PerKillBefore)} → {Format(row.PerKillAfter)}";
		RealText = row.Unchanged ? "unchanged" : $"×{row.RealMultiplier:0.##}";
		OddsText = row.ChanceAfter >= 100 ? "every kill"
			: row.ChanceAfter <= 0 ? "never"
			: $"about 1 in {Format(Math.Round(100 / row.ChanceAfter))} kills";
		Note = groups > 1 ? $"{groups} drop groups added up"
			: row.IsHerb ? "Herb — its own rate"
			: row.ChanceIsFull ? "Always drops; more chance would be wasted"
			: row.IsAdena ? "Adena uses its own rate" : null;
	}

	public DropRow Row { get; }
	public string Name { get; }

	/// <summary>How many of the monster's drop groups this row adds up.</summary>
	public int Groups { get; }

	/// <summary>The item's icon from the player's own game client, when it can be read.</summary>
	public System.Windows.Media.ImageSource? Image { get; }

	public bool HasImage => Image is not null;
	public string ChanceText { get; }
	public string AmountText { get; }
	public string PerKillText { get; }
	public string RealText { get; }

	/// <summary>How often it drops, the way a player counts it.</summary>
	public string OddsText { get; }
	public string? Note { get; }
	public bool IsSpoil => Row.IsSpoil;
	public string KindWord => Row.IsSpoil ? "Spoil" : "Drop";

	/// <summary>Short enough to read at a glance: a boss gives billions, a rare drop a hundredth of an item.</summary>
	private static string Format(double value) => value switch
	{
		0 => "0",
		< 0.01 => value.ToString("0.####", CultureInfo.InvariantCulture),
		< 1 => value.ToString("0.##", CultureInfo.InvariantCulture),
		< 1000 => value.ToString("0.#", CultureInfo.InvariantCulture),
		< 100_000 => value.ToString("N0", CultureInfo.InvariantCulture),
		< 1_000_000 => (value / 1_000).ToString("0.#", CultureInfo.InvariantCulture) + "K",
		< 1_000_000_000 => (value / 1_000_000).ToString("0.##", CultureInfo.InvariantCulture) + "M",
		_ => (value / 1_000_000_000).ToString("0.##", CultureInfo.InvariantCulture) + "B",
	};
}
