using System.Collections.ObjectModel;
using L2Config.App.Infrastructure;
using L2Config.Core.Catalog;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>The read-only Custom Config tab: what L2Everdream changed compared with stock L2J Mobius.</summary>
public sealed class CustomConfigViewModel : ObservableObject
{
	private readonly CustomConfigReport _report;
	private readonly List<CustomChangeViewModel> _all;
	private string _filter = "all";

	public CustomConfigViewModel(CustomConfigReport report, Action<string> showSetting)
	{
		_report = report;
		_all = report.Changes.Select(c => new CustomChangeViewModel(c, showSetting)).ToList();
		Files = report.Files.Where(f => f.Kind != "settings").ToList();
		FilterCommand = new RelayCommand(p => Filter = p as string ?? "all");
		ApplyFilter();
	}

	public string Title => "Custom Config";

	public string Intro =>
		$"Every setting where the L2Everdream release differs from stock L2J Mobius Interlude ({_report.Stock.Path.Replace("/dist", "")}, " +
		$"commit {_report.Stock.Commit} from {_report.Stock.Committed}). This page only shows the differences — change values in the Server tab.";

	public ObservableCollection<CustomChangeViewModel> Items { get; } = [];
	public IReadOnlyList<CustomFile> Files { get; }
	public bool HasFiles => Files.Count > 0;

	public int AddedCount => _all.Count(c => c.Change.Kind == "added");
	public int ChangedCount => _all.Count(c => c.Change.Kind == "value");
	public int NotedCount => _all.Count(c => c.Change.Kind is "note" or "removed");
	public int AllCount => _all.Count;

	public RelayCommand FilterCommand { get; }

	public string Filter
	{
		get => _filter;
		set
		{
			if (Set(ref _filter, value))
			{
				ApplyFilter();
			}
		}
	}

	/// <summary>Shows what is in the user's files now, next to what shipped.</summary>
	public void UpdateCurrentValues(SettingsStore? store, SettingsCatalog catalog, bool hasServer)
	{
		var byId = catalog.Settings.ToDictionary(s => s.Id);
		foreach (var item in _all)
		{
			item.CurrentValue = hasServer && store is not null && item.Change.SettingId is { } id && byId.TryGetValue(id, out var def)
				? store.GetValue(def)
				: null;
			item.HasServer = hasServer;
		}
	}

	private void ApplyFilter()
	{
		Items.Clear();
		foreach (var item in _all.Where(c => _filter switch
		{
			"added" => c.Change.Kind == "added",
			"changed" => c.Change.Kind == "value",
			"noted" => c.Change.Kind is "note" or "removed",
			_ => true,
		}))
		{
			Items.Add(item);
		}
	}
}

public sealed class CustomChangeViewModel : ObservableObject
{
	private string? _currentValue;
	private bool _hasServer;

	public CustomChangeViewModel(CustomChange change, Action<string> showSetting)
	{
		Change = change;
		ShowCommand = new RelayCommand(() => showSetting(change.SettingId!), () => change.SettingId is not null);
	}

	public CustomChange Change { get; }
	public string Name => Change.Name;
	public string Location => $"{(Change.Target == SettingTargets.ServerLogin ? "login" : "game")}/config/{Change.File}  ›  {Change.Key}";
	public string Note => Change.Note;

	public string KindWord => Change.Kind switch
	{
		"added" => "New setting",
		"value" => "Changed value",
		"removed" => "Removed",
		_ => "Documented",
	};

	public bool IsAdded => Change.Kind == "added";
	public string StockText => Change.StockValue ?? "not in stock Mobius";
	public string ShippedText => Change.ShippedValue ?? "removed";

	public string? CurrentValue
	{
		get => _currentValue;
		set
		{
			if (Set(ref _currentValue, value))
			{
				OnPropertyChanged(nameof(CurrentText));
				OnPropertyChanged(nameof(DiffersFromShipped));
			}
		}
	}

	public bool HasServer
	{
		get => _hasServer;
		set
		{
			if (Set(ref _hasServer, value))
			{
				OnPropertyChanged(nameof(CurrentText));
			}
		}
	}

	public string CurrentText => !HasServer ? "choose the server folder" : CurrentValue ?? "not in your file";

	/// <summary>True when the user's own file no longer matches what L2Everdream ships.</summary>
	public bool DiffersFromShipped => HasServer && CurrentValue is not null && Change.ShippedValue is not null
		&& !string.Equals(CurrentValue.Trim(), Change.ShippedValue.Trim(), StringComparison.OrdinalIgnoreCase);

	public RelayCommand ShowCommand { get; }
}
