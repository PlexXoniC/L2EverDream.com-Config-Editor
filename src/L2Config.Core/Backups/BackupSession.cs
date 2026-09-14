using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace L2Config.Core.Backups;

/// <summary>
/// One backup, taken before a save, a character edit or a restore. Everything is written flat into one folder:
/// <code>
/// backups\20260914-101500-saved-3-settings\
///   manifest.json                      what is in here, where each file came from, what changed
///   game-config__Rates.ini             a file, named role__file
///   game-player-copy__Rates.ini
///   client__l2.ini
///   db-01__items.json                  database rows as they were before the change
/// </code>
/// The manifest is rewritten after every addition, so a crash part-way through still leaves a readable backup.
/// </summary>
public sealed class BackupSession
{
	private readonly BackupManifest _manifest;
	private readonly HashSet<string> _preservedPaths = new(StringComparer.OrdinalIgnoreCase);
	private int _dbCount;

	public BackupSession(string backupsRoot, DateTime now, BackupKind kind, string title)
	{
		var baseName = $"{now:yyyyMMdd-HHmmss}-{Slug(title)}";
		var folder = Path.Combine(backupsRoot, baseName);
		for (var n = 2; Directory.Exists(folder); n++)
		{
			folder = Path.Combine(backupsRoot, $"{baseName}-{n}");
		}
		Folder = folder;
		_manifest = new BackupManifest { Created = now, Kind = kind, Title = title };
	}

	public string Folder { get; }
	public bool IsEmpty => _manifest.Entries.Count == 0 && _manifest.Changes.Count == 0;
	public BackupManifest Manifest => _manifest;

	public static string DefaultRoot => Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "L2EverdreamConfig", "backups");

	/// <summary>Copies a file once, before its first write. <paramref name="role"/> says which copy it is (game-config, client, …).</summary>
	public void PreserveFile(string path, string role, string displayName)
	{
		var full = Path.GetFullPath(path);
		if (!File.Exists(full) || !_preservedPaths.Add(full))
		{
			return;
		}
		var fileName = UniqueName($"{role}__{displayName.Replace('/', '_').Replace('\\', '_')}");
		Directory.CreateDirectory(Folder);
		File.Copy(full, Path.Combine(Folder, fileName));
		_manifest.Entries.Add(new BackupEntry
		{
			Kind = BackupEntryKind.File,
			Role = role,
			Name = displayName,
			File = fileName,
			OriginalPath = full,
			Sha256 = Hash(Path.Combine(Folder, fileName)),
		});
		Flush();
	}

	/// <summary>Stores database rows exactly as they were before a change.</summary>
	public BackupEntry RecordRows(DbRowsBackup rows, string description)
	{
		var fileName = UniqueName($"db-{++_dbCount:00}__{rows.Table}.json");
		Directory.CreateDirectory(Folder);
		File.WriteAllText(Path.Combine(Folder, fileName), JsonSerializer.Serialize(rows, BackupManifest.JsonOptions));
		var entry = new BackupEntry
		{
			Kind = BackupEntryKind.DatabaseRows,
			Role = "database",
			Name = description,
			File = fileName,
			Database = rows.Database,
			Table = rows.Table,
			CharId = rows.CharId,
			CharName = rows.CharName,
		};
		_manifest.Entries.Add(entry);
		Flush();
		return entry;
	}

	/// <summary>Removes a database entry whose change was rolled back.</summary>
	public void Forget(BackupEntry entry)
	{
		if (_manifest.Entries.Remove(entry))
		{
			File.Delete(Path.Combine(Folder, entry.File));
			Flush();
		}
	}

	/// <summary>A human-readable line for the manifest: what changed, from what, to what.</summary>
	public void NoteChange(string what, string? from, string? to)
	{
		_manifest.Changes.Add(new BackupChange { What = what, From = from, To = to });
		Flush();
	}

	private void Flush()
	{
		Directory.CreateDirectory(Folder);
		File.WriteAllText(Path.Combine(Folder, BackupManifest.FileName), JsonSerializer.Serialize(_manifest, BackupManifest.JsonOptions));
	}

	private string UniqueName(string name)
	{
		var candidate = name;
		var stem = Path.GetFileNameWithoutExtension(name);
		var ext = Path.GetExtension(name);
		for (var n = 2; File.Exists(Path.Combine(Folder, candidate)); n++)
		{
			candidate = $"{stem}-{n}{ext}";
		}
		return candidate;
	}

	internal static string Hash(string path)
	{
		using var stream = File.OpenRead(path);
		return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
	}

	private static string Slug(string title)
	{
		var sb = new StringBuilder();
		foreach (var c in title.ToLowerInvariant())
		{
			if (char.IsAsciiLetterOrDigit(c))
			{
				sb.Append(c);
			}
			else if (sb.Length > 0 && sb[^1] != '-')
			{
				sb.Append('-');
			}
			if (sb.Length >= 40)
			{
				break;
			}
		}
		return sb.ToString().Trim('-') is { Length: > 0 } slug ? slug : "backup";
	}
}
