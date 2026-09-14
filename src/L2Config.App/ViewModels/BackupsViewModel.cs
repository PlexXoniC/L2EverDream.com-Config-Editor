using System.Collections.ObjectModel;
using System.IO;
using L2Config.App.Infrastructure;
using L2Config.Core.Backups;
using L2Config.Core.Characters;

namespace L2Config.App.ViewModels;

/// <summary>What restoring needs to know about the world right now.</summary>
public sealed record RestoreConditions(bool WorldRunning, bool ClientRunning, string? ClientSystemDir, ServerFacts? Server);

/// <summary>The Backups tab: every backup taken before a save, a character edit or a restore, and restoring them.</summary>
public sealed class BackupsViewModel : ObservableObject
{
	private readonly BackupLibrary _library = new(BackupSession.DefaultRoot);
	private readonly Func<Task<RestoreConditions>> _conditions;
	private readonly CharactersViewModel _characters;
	private readonly IDialogs _dialogs;
	private readonly Action _afterRestore;
	private string? _status;

	public BackupsViewModel(Func<Task<RestoreConditions>> conditions, CharactersViewModel characters, IDialogs dialogs, Action afterRestore)
	{
		_conditions = conditions;
		_characters = characters;
		_dialogs = dialogs;
		_afterRestore = afterRestore;
		RefreshCommand = new RelayCommand(() => _ = RefreshAsync());
		OpenRootCommand = new RelayCommand(() => dialogs.OpenFolder(BackupSession.DefaultRoot));
	}

	public string Title => "Backups";

	public string Intro =>
		"A backup is taken automatically before every save, every character change and every restore: the files as they were, and " +
		"the database rows as they were. Restoring never happens while it could be overwritten — files are only restored while the " +
		"world is stopped (and client files while Lineage 2 is closed); database changes are only restored for characters that are " +
		"logged out, while the world is running (its database only runs with it).";

	public ObservableCollection<BackupRowViewModel> Rows { get; } = [];
	public RelayCommand RefreshCommand { get; }
	public RelayCommand OpenRootCommand { get; }

	public string? Status
	{
		get => _status;
		private set => Set(ref _status, value);
	}

	public string Summary => Rows.Count switch { 0 => "No backups yet.", 1 => "1 backup", var n => $"{n} backups" };

	public async Task RefreshAsync()
	{
		var conditions = await _conditions();
		Rows.Clear();
		foreach (var backup in _library.List())
		{
			Rows.Add(new BackupRowViewModel(backup, conditions, this, _dialogs));
		}
		OnPropertyChanged(nameof(Summary));
	}

	internal async Task RestoreAsync(BackupRowViewModel row)
	{
		var conditions = await _conditions();
		var files = row.Backup.Manifest.Entries.Count(e => e.Kind == BackupEntryKind.File);
		var rows = row.Backup.Manifest.Entries.Count(e => e.Kind == BackupEntryKind.DatabaseRows);
		var plan = new List<string>();
		if (files > 0)
		{
			plan.Add(conditions.WorldRunning
				? $"• {files} file(s): will NOT be restored — the world is running. Stop it from the launcher first."
				: $"• {files} file(s) put back exactly as they were (client files only if Lineage 2 is closed).");
		}
		if (rows > 0)
		{
			plan.Add(!conditions.WorldRunning
				? $"• {rows} database change(s): will NOT be restored — the database only runs while the world is running."
				: $"• {rows} database change(s) put back, only for characters that are logged out.");
		}
		if (!_dialogs.Confirm("Restore backup",
				$"{row.Title}\n{row.When}\n\n{string.Join("\n", plan)}\n\nEverything that gets replaced is backed up first, so this restore can be undone too.",
				"Restore"))
		{
			return;
		}

		ItemCatalog items;
		WorldDatabase? database = null;
		InventoryLimits limits;
		if (conditions.Server is { } server)
		{
			items = rows > 0 ? await _characters.ItemsAsync(server.Locations.ServerRoot!) : ItemCatalog.LoadFromFolder("");
			database = WorldDatabase.FromServerFolder(server.Locations);
			limits = server.Limits;
		}
		else
		{
			items = ItemCatalog.LoadFromFolder("");
			limits = new InventoryLimits(0, 0, 0, new HashSet<int>());
		}

		try
		{
			var report = await _library.RestoreAsync(row.Backup,
				new RestoreContext(conditions.WorldRunning, conditions.ClientRunning, conditions.ClientSystemDir, database, limits, items));
			row.ShowResult(report);
			Status = $"{report.Restored} restored, {report.Refused} not restored." + (report.BeforeRestoreFolder is null ? "" : " What was replaced is in a new backup.");
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
		{
			Status = "Restore stopped: " + ex.Message;
		}
		_afterRestore();
		var results = row.Results.ToList();
		await RefreshAsync();
		Rows.FirstOrDefault(r => r.Backup.Folder == row.Backup.Folder)?.ShowLines(results);
	}
}

public sealed class BackupRowViewModel : ObservableObject
{
	public BackupRowViewModel(StoredBackup backup, RestoreConditions conditions, BackupsViewModel owner, IDialogs dialogs)
	{
		Backup = backup;
		var m = backup.Manifest;
		Title = m.Title;
		When = m.Created.ToString("dddd d MMMM yyyy, HH:mm:ss");
		KindWord = m.Kind switch
		{
			BackupKind.Settings => "Settings",
			BackupKind.Characters => "Characters",
			BackupKind.BeforeRestore => "Before restore",
			_ => "Older format",
		};
		Contents = m.Entries.Select(e => e.Kind == BackupEntryKind.File
			? $"File  {e.Name}" + (e.Role is { Length: > 0 } and not "file" ? $"  ({RoleText(e.Role)})" : "")
			: $"Database  {e.Name}").ToList();
		Changes = m.Changes.Take(12).Select(c => $"{c.What}:  {c.From ?? "—"}  →  {c.To ?? "—"}").ToList();
		MoreChanges = m.Changes.Count > 12 ? $"and {m.Changes.Count - 12} more" : null;

		var hasFiles = m.Entries.Any(e => e.Kind == BackupEntryKind.File);
		var hasRows = m.Entries.Any(e => e.Kind == BackupEntryKind.DatabaseRows);
		BlockedReason = hasFiles && conditions.WorldRunning && !hasRows ? "Stop the world from the launcher to restore these files."
			: hasRows && !conditions.WorldRunning && !hasFiles ? "Start the world to restore database changes (logged-out characters only)."
			: hasRows && conditions.Server is null ? "Choose the server folder first."
			: null;
		RestoreCommand = new RelayCommand(() => _ = owner.RestoreAsync(this), () => BlockedReason is null);
		OpenFolderCommand = new RelayCommand(() => dialogs.OpenFolder(backup.Folder));
	}

	public StoredBackup Backup { get; }
	public string Title { get; }
	public string When { get; }
	public string KindWord { get; }
	public IReadOnlyList<string> Contents { get; }
	public IReadOnlyList<string> Changes { get; }
	public string? MoreChanges { get; }
	public string FolderName => Path.GetFileName(Backup.Folder);
	public string? BlockedReason { get; }
	public RelayCommand RestoreCommand { get; }
	public RelayCommand OpenFolderCommand { get; }
	public ObservableCollection<RestoreLine> Results { get; } = [];

	public void ShowResult(RestoreReport report) => ShowLines(report.Lines);

	public void ShowLines(IEnumerable<RestoreLine> lines)
	{
		Results.Clear();
		foreach (var line in lines)
		{
			Results.Add(line);
		}
	}

	private static string RoleText(string role) => role switch
	{
		"game-config" => "game server",
		"game-player-copy" => "launcher's protected copy",
		"login-config" => "login server",
		"login-player-copy" => "launcher's protected copy",
		"client" => "game client",
		"world-profile" => "launcher world",
		_ => role,
	};
}
