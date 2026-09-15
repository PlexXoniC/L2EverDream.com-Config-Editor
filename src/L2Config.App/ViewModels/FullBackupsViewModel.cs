using System.Collections.ObjectModel;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Backups;
using L2Config.Core.Catalog;
using L2Config.Core.Characters;
using L2Config.Core.FullBackups;
using L2Config.Core.Storage;

namespace L2Config.App.ViewModels;

/// <summary>
/// Full backups: a copy of every settings file (server, launcher copies and baselines, world, client), taken with one button — for
/// example right before updating L2Everdream — and a checklist comparison that puts chosen settings back.
/// </summary>
public sealed class FullBackupsViewModel : ObservableObject
{
	private readonly SettingsCatalog _catalog;
	private readonly AppSettings _settings;
	private readonly Func<L2Locations?> _locations;
	private readonly Func<Task<RestoreConditions>> _conditions;
	private readonly IDialogs _dialogs;
	private readonly Action _afterRestore;
	private ComparisonViewModel? _comparison;
	private string? _status;
	private bool _statusIsError;
	private bool _isBusy;

	public FullBackupsViewModel(SettingsCatalog catalog, AppSettings settings, Func<L2Locations?> locations,
		Func<Task<RestoreConditions>> conditions, IDialogs dialogs, Action afterRestore)
	{
		_catalog = catalog;
		_settings = settings;
		_locations = locations;
		_conditions = conditions;
		_dialogs = dialogs;
		_afterRestore = afterRestore;
		BackUpCommand = new RelayCommand(() => _ = BackUpAsync(), () => !IsBusy);
		ChooseFolderCommand = new RelayCommand(() => ChooseFolder(), () => !IsBusy);
		OpenFolderCommand = new RelayCommand(() => dialogs.OpenFolder(Folder!), () => HasFolder && Directory.Exists(Folder));
		RefreshCommand = new RelayCommand(Refresh);
	}

	public string Intro =>
		"A full backup copies every settings file at once: the server's game and login config, the launcher's protected copies and what it " +
		"ships, your world settings, and the game client's settings. Take one right before updating L2Everdream, then compare it with " +
		"your files afterwards to see exactly what the update changed and tick the settings you want back.";

	public string? Folder => _settings.FullBackupFolder;
	public bool HasFolder => Folder is not null;
	public string FolderText => Folder ?? "No folder chosen yet — you will be asked the first time you back up.";

	public string? FolderProblem
	{
		get
		{
			if (Folder is null)
			{
				return null;
			}
			if (_locations() is { } locations && FullBackupLibrary.LocationProblem(Folder, locations) is { } problem)
			{
				return problem;
			}
			return Directory.Exists(Folder) ? null : "This folder no longer exists. It will be created again when you back up.";
		}
	}

	public string InstalledText
	{
		get
		{
			var locations = _locations();
			return locations is { HasServer: true } ? $"Installed now: {ReleaseInfo.Read(locations.ServerRoot).Text}" : "Choose the server folder in the Server tab to include server settings.";
		}
	}

	public ObservableCollection<FullBackupRowViewModel> Rows { get; } = [];
	public string Summary => Rows.Count switch { 0 => "No full backups in this folder yet.", 1 => "1 full backup", var n => $"{n} full backups" };

	public ComparisonViewModel? Comparison
	{
		get => _comparison;
		private set
		{
			if (Set(ref _comparison, value))
			{
				OnPropertyChanged(nameof(IsListVisible));
			}
		}
	}

	public bool IsListVisible => Comparison is null;

	public string? Status
	{
		get => _status;
		private set => Set(ref _status, value);
	}

	public bool StatusIsError
	{
		get => _statusIsError;
		private set => Set(ref _statusIsError, value);
	}

	public bool IsBusy
	{
		get => _isBusy;
		private set => Set(ref _isBusy, value);
	}

	public RelayCommand BackUpCommand { get; }
	public RelayCommand ChooseFolderCommand { get; }
	public RelayCommand OpenFolderCommand { get; }
	public RelayCommand RefreshCommand { get; }

	public void Refresh()
	{
		Rows.Clear();
		var installed = _locations() is { HasServer: true } l ? ReleaseInfo.Read(l.ServerRoot) : null;
		foreach (var backup in FullBackupLibrary.List(Folder))
		{
			Rows.Add(new FullBackupRowViewModel(backup, installed, this, _dialogs));
		}
		OnPropertyChanged(nameof(Summary));
		OnPropertyChanged(nameof(FolderText));
		OnPropertyChanged(nameof(HasFolder));
		OnPropertyChanged(nameof(FolderProblem));
		OnPropertyChanged(nameof(InstalledText));
	}

	private bool ChooseFolder()
	{
		var picked = _dialogs.PickFolder("Choose where full backups are kept", Folder);
		if (picked is null)
		{
			return false;
		}
		if (_locations() is { } locations && FullBackupLibrary.LocationProblem(picked, locations) is { } problem)
		{
			_dialogs.Notice("Choose another folder", $"{picked}\n\n{problem}");
			return false;
		}
		_settings.FullBackupFolder = picked;
		_settings.Save();
		Comparison = null;
		Refresh();
		return true;
	}

	private async Task BackUpAsync()
	{
		if (!HasFolder && !ChooseFolder())
		{
			return;
		}
		var locations = _locations();
		if (locations is null || (!locations.HasServer && !locations.HasClient))
		{
			_dialogs.Notice("Choose your folders first", "Choose the server folder in the Server tab and the client folder in the Client tab, then back up.");
			return;
		}
		if (FullBackupLibrary.LocationProblem(Folder!, locations) is { } problem)
		{
			_dialogs.Notice("Choose another folder", problem);
			return;
		}

		IsBusy = true;
		Status = "Backing up…";
		StatusIsError = false;
		try
		{
			var backup = await Task.Run(() =>
			{
				Directory.CreateDirectory(Folder!);
				return FullBackupLibrary.Create(Folder!, locations, DateTime.Now);
			});
			var size = backup.Manifest.Files.Sum(f => f.Size);
			Status = $"Backed up {backup.Manifest.Files.Count} files ({FormatSize(size)}) — {backup.Manifest.VersionText}." +
				(locations.HasClient ? "" : " The client folder is not chosen, so client settings were not included.") +
				(locations.HasServer ? "" : " The server folder is not chosen, so server and world settings were not included.");
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
		{
			Status = "Nothing was backed up: " + ex.Message;
			StatusIsError = true;
		}
		finally
		{
			IsBusy = false;
		}
		Refresh();
	}

	internal async Task CompareAsync(FullBackupRowViewModel row)
	{
		var locations = _locations();
		if (locations is null)
		{
			return;
		}
		IsBusy = true;
		Status = "Comparing…";
		StatusIsError = false;
		try
		{
			var comparison = await Task.Run(() => FullBackupComparer.Compare(row.Backup, locations, _catalog));
			Comparison = new ComparisonViewModel(comparison, _catalog, locations, _conditions, _dialogs, this);
			await Comparison.RefreshConditionsAsync();
			Status = null;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
		{
			Status = "Could not compare: " + ex.Message;
			StatusIsError = true;
		}
		finally
		{
			IsBusy = false;
		}
	}

	/// <summary>Opens the comparison for the backup in <paramref name="folderName"/>, or the newest one (snapshot mode).</summary>
	internal async Task CompareByNameAsync(string folderName)
	{
		Refresh();
		var row = Rows.FirstOrDefault(r => r.Backup.FolderName == folderName) ?? Rows.FirstOrDefault();
		if (row is not null)
		{
			await CompareAsync(row);
		}
	}

	internal void CloseComparison()
	{
		Comparison = null;
		Refresh();
	}

	internal void AfterRestore() => _afterRestore();

	internal static string FormatSize(long bytes) =>
		bytes >= 1024 * 1024 ? $"{bytes / 1024.0 / 1024.0:0.0} MB" : $"{Math.Max(1, bytes / 1024)} KB";
}

public sealed class FullBackupRowViewModel
{
	public FullBackupRowViewModel(StoredFullBackup backup, ReleaseInfo? installed, FullBackupsViewModel owner, IDialogs dialogs)
	{
		Backup = backup;
		var m = backup.Manifest;
		Title = m.Title;
		When = m.Created.ToString("dddd d MMMM yyyy, HH:mm:ss");
		VersionText = m.VersionText;
		var parts = new List<string>();
		if (m.Files.Any(f => f.Role.EndsWith("-config", StringComparison.Ordinal)))
		{
			parts.Add("server");
		}
		if (m.Files.Any(f => f.Role == FileRoles.World))
		{
			parts.Add("world");
		}
		if (m.Files.Any(f => f.Role == FileRoles.Client))
		{
			parts.Add("client");
		}
		Summary = $"{m.Files.Count} files · {FullBackupsViewModel.FormatSize(m.Files.Sum(f => f.Size))} · {string.Join(", ", parts)} settings";
		UpdatedNotice = installed?.LauncherVersion is { } now && m.LauncherVersion is { } then && (now != then || installed.EngineVersion != m.EngineVersion)
			? $"L2Everdream has been updated since this backup: {m.LauncherVersion} → {now}. Compare to see what the update changed."
			: null;
		CompareCommand = new RelayCommand(() => _ = owner.CompareAsync(this), () => !owner.IsBusy);
		OpenFolderCommand = new RelayCommand(() => dialogs.OpenFolder(backup.Folder));
	}

	public StoredFullBackup Backup { get; }
	public string Title { get; }
	public string When { get; }
	public string VersionText { get; }
	public string Summary { get; }
	public string FolderName => Backup.FolderName;
	public string? UpdatedNotice { get; }
	public RelayCommand CompareCommand { get; }
	public RelayCommand OpenFolderCommand { get; }
}

/// <summary>The checklist of every setting that differs between a full backup and the files now.</summary>
public sealed class ComparisonViewModel : ObservableObject
{
	private readonly BackupComparison _comparison;
	private readonly L2Locations _locations;
	private readonly Func<Task<RestoreConditions>> _conditions;
	private readonly IDialogs _dialogs;
	private readonly FullBackupsViewModel _owner;
	private readonly List<DifferenceRowViewModel> _settings;
	private readonly List<FileDifferenceRowViewModel> _files;
	private string _filter = "changed";
	private string _searchText = "";
	private string? _blockedText;
	private bool _worldRunning;
	private bool _clientRunning;
	private bool _isBusy;

	public ComparisonViewModel(BackupComparison comparison, SettingsCatalog catalog, L2Locations locations,
		Func<Task<RestoreConditions>> conditions, IDialogs dialogs, FullBackupsViewModel owner)
	{
		_comparison = comparison;
		_locations = locations;
		_conditions = conditions;
		_dialogs = dialogs;
		_owner = owner;

		var categoryOrder = catalog.Categories.Select((c, i) => (c.Id, i)).ToDictionary(x => x.Id, x => x.i);
		var categoryNames = catalog.Categories.ToDictionary(c => c.Id, c => c.Name);
		var groupNames = catalog.Categories.SelectMany(c => c.Groups).ToDictionary(g => g.Id, g => g.Name);
		_settings = comparison.Settings
			.Select(s => new DifferenceRowViewModel(s, categoryNames, groupNames, this))
			.OrderBy(r => r.Difference.Target.Area)
			.ThenBy(r => r.Difference.Definition is { } d && categoryOrder.TryGetValue(d.Category, out var i) ? i : int.MaxValue)
			.ThenBy(r => r.Breadcrumb, StringComparer.OrdinalIgnoreCase)
			.ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();
		_files = comparison.Files
			.Where(f => f.Kind != FileDifferenceKind.CommentsOnly)
			.Select(f => new FileDifferenceRowViewModel(f, this))
			.ToList();
		CommentOnlyFiles = comparison.Files.Where(f => f.Kind == FileDifferenceKind.CommentsOnly).Select(f => f.FileLabel).ToList();

		FilterCommand = new RelayCommand(p => Filter = p as string ?? "changed");
		SelectAllCommand = new RelayCommand(() => SetAll(true));
		ClearCommand = new RelayCommand(() => SetAll(false), () => SelectedCount > 0);
		RestoreCommand = new RelayCommand(() => _ = RestoreAsync(), () => SelectedCount > 0 && !_isBusy);
		BackCommand = new RelayCommand(owner.CloseComparison);
		ApplyFilter();
	}

	public string Title => $"Compared with the full backup of {_comparison.Backup.Manifest.Created:dddd d MMMM yyyy, HH:mm}";

	public string ReleaseText => _comparison.ReleaseChanged
		? $"L2Everdream was updated since this backup: {Version(_comparison.Backup.Manifest.LauncherVersion, _comparison.Backup.Manifest.EngineVersion)}  →  {Version(_comparison.CurrentRelease.LauncherVersion, _comparison.CurrentRelease.EngineVersion)} now."
		: $"Same L2Everdream version as when the backup was taken: {Version(_comparison.CurrentRelease.LauncherVersion, _comparison.CurrentRelease.EngineVersion)}.";

	private static string Version(string? launcher, string? engine) =>
		launcher is null ? "unknown version" : engine is null ? launcher : $"{launcher} (engine {engine})";

	public bool ReleaseChanged => _comparison.ReleaseChanged;

	public string SummaryText
	{
		get
		{
			var changed = _settings.Count(r => r.IsChangedFilter) + _files.Count(f => f.Difference.Kind != FileDifferenceKind.NewSinceBackup);
			var parts = new List<string>
			{
				changed switch { 0 => "No setting has a different value than in the backup", 1 => "1 setting differs from the backup", var n => $"{n} settings differ from the backup" },
			};
			if (ShippedCount > 0)
			{
				parts.Add($"{ShippedCount} shipped default{(ShippedCount == 1 ? "" : "s")} changed");
			}
			if (NewCount > 0)
			{
				parts.Add($"{NewCount} new since the backup");
			}
			return string.Join("  ·  ", parts) + ".";
		}
	}

	public IReadOnlyList<string> Warnings => _comparison.Warnings;
	public bool HasWarnings => _comparison.Warnings.Count > 0;

	public IReadOnlyList<string> CommentOnlyFiles { get; }

	public string? CommentOnlyText => CommentOnlyFiles.Count == 0 ? null
		: $"{CommentOnlyFiles.Count} file{(CommentOnlyFiles.Count == 1 ? "" : "s")} changed only in comments or blank lines — no setting in them is different, so there is nothing to restore: "
		  + string.Join(", ", CommentOnlyFiles.Take(8).Select(Path.GetFileName)) + (CommentOnlyFiles.Count > 8 ? $" and {CommentOnlyFiles.Count - 8} more." : ".");

	public int ChangedCount => _settings.Count(r => r.IsChangedFilter) + _files.Count(f => f.Difference.Kind != FileDifferenceKind.NewSinceBackup);
	public int ShippedCount => _settings.Count(r => r.Difference.Kind == DifferenceKind.ShippedDefaultChanged);
	public int NewCount => _settings.Count(r => r.Difference.Kind == DifferenceKind.NewSinceBackup) + _files.Count(f => f.Difference.Kind == FileDifferenceKind.NewSinceBackup);
	public int AllCount => _settings.Count + _files.Count;

	public string Filter
	{
		get => _filter;
		set
		{
			if (Set(ref _filter, value))
			{
				OnPropertyChanged(nameof(IsChangedFilter));
				OnPropertyChanged(nameof(IsShippedFilter));
				OnPropertyChanged(nameof(IsNewFilter));
				OnPropertyChanged(nameof(IsAllFilter));
				ApplyFilter();
			}
		}
	}

	public bool IsChangedFilter => _filter == "changed";
	public bool IsShippedFilter => _filter == "shipped";
	public bool IsNewFilter => _filter == "new";
	public bool IsAllFilter => _filter == "all";

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (Set(ref _searchText, value ?? ""))
			{
				ApplyFilter();
			}
		}
	}

	/// <summary>Setting rows, file rows and headings, in display order.</summary>
	public ObservableCollection<object> Rows { get; } = [];
	public bool HasRows => Rows.Count > 0;

	public string EmptyText => Filter == "changed"
		? "Nothing to restore: every setting has the same value as in the backup."
		: "Nothing matches.";

	public int SelectedCount => _settings.Count(r => r.IsSelected) + _files.Count(f => f.IsSelected);
	public string RestoreText => SelectedCount switch { 0 => "Restore selected", 1 => "Restore 1 setting", var n => $"Restore {n} settings" };

	public string? BlockedText
	{
		get => _blockedText;
		private set => Set(ref _blockedText, value);
	}

	public ObservableCollection<RestoreLine> Results { get; } = [];
	public bool HasResults => Results.Count > 0;

	public RelayCommand FilterCommand { get; }
	public RelayCommand SelectAllCommand { get; }
	public RelayCommand ClearCommand { get; }
	public RelayCommand RestoreCommand { get; }
	public RelayCommand BackCommand { get; }

	internal void SelectionChanged()
	{
		OnPropertyChanged(nameof(SelectedCount));
		OnPropertyChanged(nameof(RestoreText));
	}

	public async Task RefreshConditionsAsync()
	{
		var conditions = await _conditions();
		_worldRunning = conditions.WorldRunning;
		_clientRunning = conditions.ClientRunning;
		var blocked = new List<string>();
		if (_worldRunning && _settings.Concat<object>(_files).Any(r => AreaOf(r) != SettingArea.Client))
		{
			blocked.Add("Your world is running: server and world settings can only be restored after you stop it from the launcher.");
		}
		if (_clientRunning && _settings.Any(r => r.Difference.Target.Area == SettingArea.Client))
		{
			blocked.Add("Lineage 2 is open: close the game before restoring client settings — it rewrites them when it exits.");
		}
		BlockedText = blocked.Count == 0 ? null : string.Join(" ", blocked);
	}

	private static SettingArea AreaOf(object row) => row switch
	{
		DifferenceRowViewModel s => s.Difference.Target.Area,
		FileDifferenceRowViewModel f => f.Difference.Target.Area,
		_ => SettingArea.Server,
	};

	private void SetAll(bool selected)
	{
		foreach (var row in Rows)
		{
			switch (row)
			{
				case DifferenceRowViewModel s when s.CanRestore:
					s.IsSelected = selected;
					break;
				case FileDifferenceRowViewModel f when f.CanRestore:
					f.IsSelected = selected;
					break;
			}
		}
		if (!selected)
		{
			foreach (var s in _settings)
			{
				s.IsSelected = false;
			}
			foreach (var f in _files)
			{
				f.IsSelected = false;
			}
		}
	}

	private void ApplyFilter()
	{
		Rows.Clear();
		var words = _searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		bool Matches(string haystack) => words.All(w => haystack.Contains(w, StringComparison.OrdinalIgnoreCase));

		string? lastHeading = null;
		foreach (var row in _settings)
		{
			var include = _filter switch
			{
				"shipped" => row.Difference.Kind == DifferenceKind.ShippedDefaultChanged,
				"new" => row.Difference.Kind == DifferenceKind.NewSinceBackup,
				"all" => true,
				_ => row.IsChangedFilter,
			};
			if (!include || !Matches(row.SearchText))
			{
				continue;
			}
			if (row.Heading != lastHeading)
			{
				Rows.Add(new ComparisonHeading(row.Heading));
				lastHeading = row.Heading;
			}
			Rows.Add(row);
		}

		var files = _files.Where(f => (_filter switch
		{
			"new" => f.Difference.Kind == FileDifferenceKind.NewSinceBackup,
			"shipped" => false,
			"all" => true,
			_ => f.Difference.Kind != FileDifferenceKind.NewSinceBackup,
		}) && Matches(f.Difference.FileLabel)).ToList();
		if (files.Count > 0)
		{
			Rows.Add(new ComparisonHeading("Whole files (XML and text)"));
			foreach (var f in files)
			{
				Rows.Add(f);
			}
		}
		OnPropertyChanged(nameof(HasRows));
		OnPropertyChanged(nameof(EmptyText));
	}

	private async Task RestoreAsync()
	{
		await RefreshConditionsAsync();
		var settings = _settings.Where(s => s.IsSelected).Select(s => s.Difference).ToList();
		var files = _files.Where(f => f.IsSelected).Select(f => f.Difference).ToList();
		var count = settings.Count + files.Count;
		var plan = new List<string>();
		void Plan(SettingArea area, string what)
		{
			var n = settings.Count(s => s.Target.Area == area) + files.Count(f => f.Target.Area == area);
			if (n == 0)
			{
				return;
			}
			var blocked = area == SettingArea.Client ? _clientRunning : _worldRunning;
			plan.Add(blocked
				? $"• {n} {what} setting(s): will NOT be restored — " + (area == SettingArea.Client ? "Lineage 2 is open." : "your world is running. Stop it from the launcher first.")
				: $"• {n} {what} setting(s) put back to the value in the backup.");
		}
		Plan(SettingArea.Server, "server");
		Plan(SettingArea.World, "world");
		Plan(SettingArea.Client, "client");
		if (!_dialogs.Confirm("Restore settings",
				$"{Title}\n\n{string.Join("\n", plan)}\n\nOnly the ticked values are written — comments and settings added since the backup stay as they are. " +
				"Everything that gets replaced is backed up first (Change backups), so this can be undone.",
				count == 1 ? "Restore 1 setting" : $"Restore {count} settings"))
		{
			return;
		}

		_isBusy = true;
		try
		{
			var report = await Task.Run(() => FullBackupRestorer.Restore(_comparison, settings, files, _locations, _worldRunning, _clientRunning, BackupSession.DefaultRoot));
			Results.Clear();
			foreach (var line in report.Lines)
			{
				Results.Add(line);
			}
			Results.Insert(0, report.Refused == 0
				? RestoreLine.Success($"{report.Restored} restored." + (report.BeforeRestoreFolder is null ? "" : " What was replaced is in a new change backup."))
				: RestoreLine.Fail($"{report.Restored} restored, {report.Refused} not restored." + (report.BeforeRestoreFolder is null ? "" : " What was replaced is in a new change backup.")));
			OnPropertyChanged(nameof(HasResults));
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
		{
			Results.Clear();
			Results.Add(RestoreLine.Fail("Restore stopped: " + ex.Message));
			OnPropertyChanged(nameof(HasResults));
		}
		finally
		{
			_isBusy = false;
		}

		var results = Results.ToList();
		_owner.AfterRestore();
		if (_owner.Rows.FirstOrDefault(r => r.Backup.Folder == _comparison.Backup.Folder) is { } row)
		{
			await _owner.CompareAsync(row);
		}
		if (_owner.Comparison is { } fresh && fresh != this)
		{
			foreach (var line in results)
			{
				fresh.Results.Add(line);
			}
			fresh.OnPropertyChanged(nameof(HasResults));
		}
	}
}

public sealed record ComparisonHeading(string Title);

public sealed class DifferenceRowViewModel : ObservableObject
{
	private readonly ComparisonViewModel _owner;
	private bool _isSelected;

	public DifferenceRowViewModel(SettingDifference difference, IReadOnlyDictionary<string, string> categories,
		IReadOnlyDictionary<string, string> groups, ComparisonViewModel owner)
	{
		Difference = difference;
		_owner = owner;
		var d = difference.Definition;
		var area = difference.Target.Area switch { SettingArea.Client => "Client", SettingArea.World => "Your world", _ => "Server" };
		Name = d?.Name ?? difference.Key;
		Heading = d is not null && categories.TryGetValue(d.Category, out var category)
			? $"{area}  ·  {category}"
			: $"{area}  ·  Other settings in {Path.GetFileName(difference.FileLabel)}";
		Breadcrumb = d is not null && groups.TryGetValue(d.Group, out var group) ? group : "Not in the editor's list";
		Location = difference.Section is null
			? $"{difference.FileLabel}  ›  {difference.Key}"
			: $"{difference.FileLabel}  ›  [{difference.Section}]  ›  {difference.Key}";
		KindWord = difference.Kind switch
		{
			DifferenceKind.ValueChanged => "Changed since the backup",
			DifferenceKind.MissingNow => "No longer in the file",
			DifferenceKind.NewSinceBackup => "New since the backup",
			_ => "Shipped default changed",
		};
		BackupText = Show(difference.BackupValue, difference.IsSecret);
		NowText = Show(difference.CurrentValue, difference.IsSecret);
		ShippedText = difference.ShippedBefore is null && difference.ShippedNow is null ? null
			: difference.ShippedBefore == difference.ShippedNow ? Show(difference.ShippedNow, difference.IsSecret)
			: $"{Show(difference.ShippedBefore, difference.IsSecret)}  →  {Show(difference.ShippedNow, difference.IsSecret)}";
		SearchText = $"{Name} {difference.Key} {difference.FileLabel} {Heading} {Breadcrumb}";
	}

	public SettingDifference Difference { get; }
	public string Name { get; }
	public string Heading { get; }
	public string Breadcrumb { get; }
	public string Location { get; }
	public string KindWord { get; }
	public string BackupText { get; }
	public string NowText { get; }
	public string? ShippedText { get; }
	public string SearchText { get; }
	public bool CanRestore => Difference.CanRestore;
	public string? Reason => Difference.NotRestorableReason;
	public bool IsChangedFilter => Difference.Kind is DifferenceKind.ValueChanged or DifferenceKind.MissingNow;
	public bool IsShippedChange => Difference.Kind == DifferenceKind.ShippedDefaultChanged;

	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			if (Set(ref _isSelected, value && CanRestore))
			{
				_owner.SelectionChanged();
			}
		}
	}

	private static string Show(string? value, bool secret) =>
		value is null ? "(not set)" : secret ? "••••••" : value.Length == 0 ? "(empty)" : value;
}

public sealed class FileDifferenceRowViewModel : ObservableObject
{
	private readonly ComparisonViewModel _owner;
	private bool _isSelected;

	public FileDifferenceRowViewModel(FileDifference difference, ComparisonViewModel owner)
	{
		Difference = difference;
		_owner = owner;
		KindWord = difference.Kind switch
		{
			FileDifferenceKind.ContentChanged => "Content changed",
			FileDifferenceKind.MissingNow => "File no longer exists",
			FileDifferenceKind.NewSinceBackup => "New since the backup",
			_ => "Comments only",
		};
		Lines = difference.Lines.Take(40).Select(l => (l.Added ? "+  now:     " : "−  backup:  ") + l.Text).ToList();
		MoreLines = difference.Lines.Count > 40 ? $"and {difference.Lines.Count - 40} more changed lines" : null;
	}

	public FileDifference Difference { get; }
	public string KindWord { get; }
	public IReadOnlyList<string> Lines { get; }
	public string? MoreLines { get; }
	public bool CanRestore => Difference.CanRestore && Difference.Kind is FileDifferenceKind.ContentChanged or FileDifferenceKind.MissingNow;
	public string? Reason => Difference.NotRestorableReason ?? (CanRestore ? "Restoring puts the whole file back as it was in the backup." : null);

	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			if (Set(ref _isSelected, value && CanRestore))
			{
				_owner.SelectionChanged();
			}
		}
	}
}
