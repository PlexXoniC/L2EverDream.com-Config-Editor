using System.Collections.ObjectModel;
using System.Globalization;
using L2Config.App.Infrastructure;
using L2Config.Core.Rates;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>
/// The Rates tab: pick how much faster your world should be than retail, see every setting that takes to get there, and
/// apply it. The values land in the Server tab as unsaved changes, so they are saved, backed up and undone like any
/// other setting. What a rate does to real monsters is the Drops tab's job, and it follows the rate picked here.
/// </summary>
public sealed class RatesViewModel : ObservableObject
{
	private readonly Func<IReadOnlyList<SettingViewModel>> _serverSettings;
	private readonly Func<L2Locations?> _locations;
	private readonly Action _applied;
	private double _rate = 1;
	private double _delivery = 0.5;
	private string _rateText = "1";
	private string? _status;

	public RatesViewModel(Func<IReadOnlyList<SettingViewModel>> serverSettings, Func<L2Locations?> locations,
		Action<string> showSetting, Action applied, Action showDrops)
	{
		_serverSettings = serverSettings;
		_locations = locations;
		_applied = applied;
		PresetCommand = new RelayCommand(p => Rate = double.TryParse(p as string, NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : Rate);
		ApplyCommand = new RelayCommand(Apply, () => HasServer);
		ShowSettingCommand = new RelayCommand(p => { if (p is string id) showSetting(id); });
		ShowDropsCommand = new RelayCommand(showDrops);
	}

	/// <summary>Raised whenever the rate changes, so the Drops tab can follow it.</summary>
	public event Action? RatesChanged;

	public string Title => "Rates";

	public string Intro =>
		"Pick how much faster than retail your world should be. One number sets experience, skill points, drops, spoil, " +
		"quest rewards and adena together — split between how often drops happen and how big they are, so that none of it " +
		"is wasted. The Drops tab shows what it does to any monster you like.";

	public bool HasServer => _locations() is { HasServer: true };

	// ------------------------------------------------------------------ the rate

	public double Rate
	{
		get => _rate;
		set
		{
			var clamped = Math.Clamp(Math.Round(value, 2), RateOptions.MinRate, RateOptions.MaxRate);
			if (Set(ref _rate, clamped))
			{
				_rateText = RatePlan.Text(clamped);
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
			var now = RateSettings.Read(key => Find("Rates.ini", key)?.Value);
			return Find("Rates.ini", "RateXp") is null
				? "Your world's current rates could not be read."
				: $"Right now your world runs at {RatePlan.Text(now.Xp)}× experience and {RatePlan.Text(now.DropsOverall)}× drops.";
		}
	}

	public ObservableCollection<RateChangeViewModel> Changes { get; } = [];

	/// <summary>The same list in two columns, so each one stacks tightly instead of lining up in a grid.</summary>
	public ObservableCollection<RateChangeViewModel> ChangesLeft { get; } = [];

	public ObservableCollection<RateChangeViewModel> ChangesRight { get; } = [];
	public IReadOnlyList<string> Untouched => Plan.Untouched;

	public string? Status
	{
		get => _status;
		private set => Set(ref _status, value);
	}

	public RelayCommand PresetCommand { get; }
	public RelayCommand ApplyCommand { get; }
	public RelayCommand ShowSettingCommand { get; }
	public RelayCommand ShowDropsCommand { get; }

	public string ApplyText => $"Apply {RatePlan.Text(Rate)}× to my world";

	public string DropsLinkText => $"See what {RatePlan.Text(Rate)}× does to a monster →";

	/// <summary>Reads the world's own rate in, so the tab opens on what the world already is.</summary>
	public void Reset()
	{
		Rate = RatePlan.RateOf(Find("Rates.ini", "RateXp")?.Value) ?? 1;
		RaisePlan();
	}

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
		RaisePlan();
	}

	private SettingViewModel? Find(string file, string key) =>
		_serverSettings().FirstOrDefault(s => s.Definition.File == file && s.Definition.Key == key);

	private void RaisePlan()
	{
		Changes.Clear();
		ChangesLeft.Clear();
		ChangesRight.Clear();
		foreach (var change in Plan.Changes)
		{
			Changes.Add(new RateChangeViewModel(change, Find(change.File, change.Key), ShowSettingCommand));
		}
		var half = (Changes.Count + 1) / 2;
		for (var i = 0; i < Changes.Count; i++)
		{
			(i < half ? ChangesLeft : ChangesRight).Add(Changes[i]);
		}
		foreach (var name in new[]
		{
			nameof(Options), nameof(Plan), nameof(SummaryText), nameof(DeliveryText), nameof(ApplyText), nameof(Untouched),
			nameof(CurrentText), nameof(RateProblem), nameof(DropsLinkText), nameof(HasServer),
		})
		{
			OnPropertyChanged(name);
		}
		RatesChanged?.Invoke();
	}
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
