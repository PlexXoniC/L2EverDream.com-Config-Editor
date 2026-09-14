using System.Text.Json;
using System.Text.Json.Serialization;

namespace L2Config.Core.Backups;

public sealed class BackupManifest
{
	public const string FileName = "manifest.json";

	public static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
	};

	public int Version { get; init; } = 2;
	public DateTime Created { get; init; }
	public BackupKind Kind { get; init; }
	public string Title { get; init; } = "";
	public List<BackupEntry> Entries { get; init; } = [];
	public List<BackupChange> Changes { get; init; } = [];
}

public enum BackupKind
{
	Settings,
	Characters,
	BeforeRestore,
	Legacy,
}

public enum BackupEntryKind
{
	File,
	DatabaseRows,
}

public sealed class BackupEntry
{
	public BackupEntryKind Kind { get; init; }

	/// <summary>game-config, game-player-copy, login-config, login-player-copy, client, world-profile, database.</summary>
	public string Role { get; init; } = "";

	/// <summary>File name (Rates.ini, Custom/AutoPlay.ini) or a description of the database change.</summary>
	public string Name { get; init; } = "";

	/// <summary>The file inside the backup folder.</summary>
	public string File { get; init; } = "";

	public string? OriginalPath { get; init; }
	public string? Sha256 { get; init; }
	public string? Database { get; init; }
	public string? Table { get; init; }
	public long? CharId { get; init; }
	public string? CharName { get; init; }
}

public sealed class BackupChange
{
	public string What { get; init; } = "";
	public string? From { get; init; }
	public string? To { get; init; }
}

/// <summary>Database rows as they were before a change, and what the change did to them.</summary>
public sealed class DbRowsBackup
{
	public string Database { get; init; } = "";
	public string Table { get; init; } = "";

	/// <summary>
	/// update/delete: <see cref="Rows"/> hold the rows as they were before. insert: <see cref="Rows"/> hold the rows
	/// this app added (restoring removes them if they are still there).
	/// </summary>
	public DbOperation Operation { get; init; }

	public long? CharId { get; init; }
	public string? CharName { get; init; }

	/// <summary>Column → value (as text; null for SQL NULL).</summary>
	public List<Dictionary<string, string?>> Rows { get; init; } = [];
}

public enum DbOperation
{
	Update,
	Delete,
	Insert,
}
