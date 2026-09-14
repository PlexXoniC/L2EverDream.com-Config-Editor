using System.Text.Json;
using L2Config.Core.Characters;

namespace L2Config.Core.Backups;

/// <summary>Lists backups and restores them.</summary>
public sealed class BackupLibrary(string root)
{
	public string Root => root;

	/// <summary>Newest first. Folders from the first version of the app (nested full paths, no manifest) are read as legacy backups.</summary>
	public IReadOnlyList<StoredBackup> List()
	{
		if (!Directory.Exists(root))
		{
			return [];
		}
		var result = new List<StoredBackup>();
		foreach (var folder in Directory.EnumerateDirectories(root))
		{
			var manifestPath = Path.Combine(folder, BackupManifest.FileName);
			if (File.Exists(manifestPath))
			{
				try
				{
					var manifest = JsonSerializer.Deserialize<BackupManifest>(File.ReadAllText(manifestPath), BackupManifest.JsonOptions);
					if (manifest is not null)
					{
						result.Add(new StoredBackup(folder, manifest));
					}
				}
				catch (JsonException)
				{
					// A damaged manifest is skipped rather than failing the whole list.
				}
			}
			else if (ReadLegacy(folder) is { } legacy)
			{
				result.Add(legacy);
			}
		}
		return result.OrderByDescending(b => b.Manifest.Created).ToList();
	}

	public static DbRowsBackup ReadRows(StoredBackup backup, BackupEntry entry) =>
		JsonSerializer.Deserialize<DbRowsBackup>(File.ReadAllText(Path.Combine(backup.Folder, entry.File)), BackupManifest.JsonOptions)
		?? throw new InvalidDataException($"{entry.File} is empty.");

	/// <summary>
	/// Restores every entry it safely can. Files are never restored while the world is running (or, for client files, while
	/// Lineage 2 is open). Database rows are restored only for offline characters while the world runs (the database only
	/// runs with the world). The current state of everything restored is backed up first.
	/// </summary>
	public async Task<RestoreReport> RestoreAsync(StoredBackup backup, RestoreContext context, CancellationToken cancellation = default)
	{
		var before = new BackupSession(root, DateTime.Now, BackupKind.BeforeRestore, $"Before restoring {backup.Manifest.Title}");
		var lines = new List<RestoreLine>();

		foreach (var entry in backup.Manifest.Entries)
		{
			if (entry.Kind == BackupEntryKind.File)
			{
				lines.Add(RestoreFile(backup, entry, context, before));
			}
		}

		var dbEntries = backup.Manifest.Entries.Where(e => e.Kind == BackupEntryKind.DatabaseRows).ToList();
		if (dbEntries.Count > 0)
		{
			if (context.Database is null)
			{
				lines.Add(RestoreLine.Fail("Database changes were not restored: choose the server folder first."));
			}
			else
			{
				foreach (var entry in dbEntries)
				{
					try
					{
						lines.AddRange(await context.Database.RestoreRowsAsync(ReadRows(backup, entry), backup.Manifest.Created, context.Limits, context.Items, before, cancellation));
					}
					catch (WorldDatabaseException ex)
					{
						lines.Add(RestoreLine.Fail($"{entry.Name}: {ex.Message}"));
					}
				}
			}
		}
		return new RestoreReport(lines, before.IsEmpty ? null : before.Folder);
	}

	private static RestoreLine RestoreFile(StoredBackup backup, BackupEntry entry, RestoreContext context, BackupSession before)
	{
		var label = entry.Role is { Length: > 0 } and not "file" ? $"{entry.Name} ({entry.Role})" : entry.Name;
		var source = Path.Combine(backup.Folder, entry.File);
		var target = entry.OriginalPath;
		if (target is null || !File.Exists(source))
		{
			return RestoreLine.Fail($"{label}: the backup copy is missing.");
		}
		if (entry.Sha256 is { } expected && !string.Equals(BackupSession.Hash(source), expected, StringComparison.OrdinalIgnoreCase))
		{
			return RestoreLine.Fail($"{label}: the backup copy has been changed since it was taken, so it was not used.");
		}
		var isClient = entry.Role == "client" || IsUnder(target, context.ClientSystemDir);
		if (isClient ? context.ClientRunning : context.WorldRunning)
		{
			return RestoreLine.Fail(isClient
				? $"{label}: close Lineage 2 first — the client rewrites its settings when it exits."
				: $"{label}: stop the world from the L2Everdream launcher first. Files are never restored while the world is running.");
		}
		if (!Directory.Exists(Path.GetDirectoryName(target)))
		{
			return RestoreLine.Fail($"{label}: its folder no longer exists ({Path.GetDirectoryName(target)}).");
		}
		before.PreserveFile(target, entry.Role is { Length: > 0 } ? entry.Role : "file", entry.Name);
		var temp = target + ".l2config.tmp";
		File.Copy(source, temp, overwrite: true);
		File.Move(temp, target, overwrite: true);
		return RestoreLine.Success($"{label}: restored.");
	}

	private static bool IsUnder(string path, string? folder) =>
		folder is not null && Path.GetFullPath(path).StartsWith(Path.GetFullPath(folder).TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase);

	private static string LegacyRole(string path) =>
		path.Contains(@"\L2Everdream-data\db\config\game\", StringComparison.OrdinalIgnoreCase) ? "game-player-copy"
		: path.Contains(@"\L2Everdream-data\db\config\login\", StringComparison.OrdinalIgnoreCase) ? "login-player-copy"
		: path.Contains(@"\game\config\", StringComparison.OrdinalIgnoreCase) ? "game-config"
		: path.Contains(@"\login\config\", StringComparison.OrdinalIgnoreCase) ? "login-config"
		: path.EndsWith("world-profile.json", StringComparison.OrdinalIgnoreCase) ? "world-profile"
		: path.Contains(@"\system\", StringComparison.OrdinalIgnoreCase) ? "client"
		: "file";

	/// <summary>The first app version stored C:\a\b\file.ini as &lt;backup&gt;\C\a\b\file.ini with no manifest.</summary>
	private static StoredBackup? ReadLegacy(string folder)
	{
		var name = Path.GetFileName(folder);
		if (!DateTime.TryParseExact(name, "yyyyMMdd-HHmmss", null, System.Globalization.DateTimeStyles.None, out var created))
		{
			return null;
		}
		var entries = new List<BackupEntry>();
		foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
		{
			var relative = Path.GetRelativePath(folder, file);
			var parts = relative.Split(Path.DirectorySeparatorChar);
			if (parts.Length < 2 || parts[0].Length != 1)
			{
				continue;
			}
			var originalPath = parts[0] + ":\\" + string.Join('\\', parts[1..]);
			entries.Add(new BackupEntry
			{
				Kind = BackupEntryKind.File,
				Role = LegacyRole(originalPath),
				Name = Path.GetFileName(file),
				File = relative,
				OriginalPath = originalPath,
			});
		}
		return entries.Count == 0 ? null : new StoredBackup(folder, new BackupManifest
		{
			Version = 1, Created = created, Kind = BackupKind.Legacy, Title = "Saved settings (older backup format)", Entries = entries,
		});
	}
}

public sealed record StoredBackup(string Folder, BackupManifest Manifest);

public sealed record RestoreContext(bool WorldRunning, bool ClientRunning, string? ClientSystemDir, WorldDatabase? Database, InventoryLimits Limits, ItemCatalog Items);

public sealed record RestoreReport(IReadOnlyList<RestoreLine> Lines, string? BeforeRestoreFolder)
{
	public int Restored => Lines.Count(l => l.Ok);
	public int Refused => Lines.Count(l => !l.Ok);
}
