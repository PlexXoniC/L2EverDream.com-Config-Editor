using System.Text.Json;
using System.Text.Json.Serialization;
using L2Config.Core.Backups;
using L2Config.Core.Storage;

namespace L2Config.Core.FullBackups;

/// <summary>
/// A copy of every settings file at one moment, taken on request (typically right before a launcher update) so the settings can be
/// compared with what is on disk later and chosen settings put back. One flat folder per backup:
/// <code>
/// 20260915-052700-full-backup-0-5-19\
///   full-backup.json                 manifest: versions, where every file came from, SHA-256
///   game-config__Rates.ini           install copy        game-player-copy__Rates.ini   launcher's protected copy
///   game-baseline__Rates.ini         what was shipped    client__l2.ini   world__world-profile.json   release__world-release.json
/// </code>
/// The folder is written under a temporary name and renamed when complete, so a half-written backup is never listed.
/// </summary>
public sealed class FullBackupManifest
{
	public const string FileName = "full-backup.json";

	public static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	public int Version { get; init; } = 1;
	public DateTime Created { get; init; }
	public string Title { get; init; } = "";

	/// <summary>From the install's world-release.json when the backup was taken.</summary>
	public string? LauncherVersion { get; init; }

	public string? EngineVersion { get; init; }
	public string? ServerRoot { get; init; }
	public string? ClientSystemDir { get; init; }
	public List<FullBackupFile> Files { get; init; } = [];

	[JsonIgnore]
	public string VersionText => LauncherVersion is null ? "unknown version" : $"L2Everdream {LauncherVersion}" + (EngineVersion is null ? "" : $" (engine {EngineVersion})");
}

public sealed class FullBackupFile
{
	/// <summary>See <see cref="FileRoles"/>.</summary>
	public string Role { get; init; } = "";

	/// <summary>Path relative to the role's folder, with forward slashes: Rates.ini, Custom/AutoPlay.ini, l2.ini.</summary>
	public string Name { get; init; } = "";

	/// <summary>The file inside the backup folder.</summary>
	public string File { get; init; } = "";

	public string OriginalPath { get; init; } = "";
	public string Sha256 { get; init; } = "";
	public long Size { get; init; }
}

public static class FileRoles
{
	public const string GameConfig = "game-config";
	public const string GamePlayerCopy = "game-player-copy";
	public const string GameBaseline = "game-baseline";
	public const string LoginConfig = "login-config";
	public const string LoginPlayerCopy = "login-player-copy";
	public const string LoginBaseline = "login-baseline";
	public const string Client = "client";
	public const string World = "world";
	public const string Release = "release";

	public static string Config(string server) => $"{server}-config";
	public static string PlayerCopy(string server) => $"{server}-player-copy";
	public static string Baseline(string server) => $"{server}-baseline";
}

public sealed record StoredFullBackup(string Folder, FullBackupManifest Manifest)
{
	public string FolderName => Path.GetFileName(Folder);

	public FullBackupFile? Find(string role, string name) =>
		Manifest.Files.FirstOrDefault(f => f.Role == role && string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));

	public string PathOf(FullBackupFile file) => Path.Combine(Folder, file.File);

	/// <summary>True when the stored copy still has the fingerprint recorded when it was taken.</summary>
	public bool IsIntact(FullBackupFile file) =>
		System.IO.File.Exists(PathOf(file)) && string.Equals(BackupSession.Hash(PathOf(file)), file.Sha256, StringComparison.OrdinalIgnoreCase);
}

/// <summary>The installed L2Everdream release, from world-release.json.</summary>
public sealed record ReleaseInfo(string? LauncherVersion, string? EngineVersion)
{
	public string Text => LauncherVersion is null ? "unknown version" : $"L2Everdream {LauncherVersion}" + (EngineVersion is null ? "" : $" (engine {EngineVersion})");

	public static ReleaseInfo Read(string? serverRoot) => ReadFile(serverRoot is null ? null : Path.Combine(serverRoot, "world-release.json"));

	public static ReleaseInfo ReadFile(string? path)
	{
		if (path is null || !File.Exists(path))
		{
			return new ReleaseInfo(null, null);
		}
		try
		{
			using var doc = JsonDocument.Parse(File.ReadAllText(path));
			string? Get(string name) => doc.RootElement.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
			return new ReleaseInfo(Get("launcherVersion"), Get("engineVersion"));
		}
		catch (JsonException)
		{
			return new ReleaseInfo(null, null);
		}
	}
}

public static class FullBackupLibrary
{
	/// <summary>Why <paramref name="folder"/> can't hold full backups, or null when it can.</summary>
	public static string? LocationProblem(string folder, L2Locations locations)
	{
		if (locations.ServerRoot is { } server && IsSameOrUnder(folder, server))
		{
			return "That folder is inside the L2Everdream install folder, which every launcher update replaces — the backups would be deleted by the very update they are meant to protect against. Choose a folder outside it.";
		}
		if (locations.ClientSystemDir is { } client && IsSameOrUnder(folder, client))
		{
			return "That folder is inside the game client's system folder. Choose a folder outside the game.";
		}
		return null;
	}

	/// <summary>Newest first. Folders without a readable manifest (for example one still being written) are skipped.</summary>
	public static IReadOnlyList<StoredFullBackup> List(string? root)
	{
		if (root is null || !Directory.Exists(root))
		{
			return [];
		}
		var result = new List<StoredFullBackup>();
		foreach (var folder in Directory.EnumerateDirectories(root))
		{
			var manifestPath = Path.Combine(folder, FullBackupManifest.FileName);
			if (!File.Exists(manifestPath))
			{
				continue;
			}
			try
			{
				if (JsonSerializer.Deserialize<FullBackupManifest>(File.ReadAllText(manifestPath), FullBackupManifest.JsonOptions) is { } manifest)
				{
					result.Add(new StoredFullBackup(folder, manifest));
				}
			}
			catch (JsonException)
			{
				// A damaged manifest is skipped rather than failing the whole list.
			}
		}
		return result.OrderByDescending(b => b.Manifest.Created).ToList();
	}

	/// <summary>
	/// Copies every settings file: the server's game and login config folders, the launcher's protected copies and shipped baselines,
	/// the launcher's world files, the release stamp, and the client's system\*.ini.
	/// </summary>
	public static StoredFullBackup Create(string root, L2Locations locations, DateTime now)
	{
		if (LocationProblem(root, locations) is { } problem)
		{
			throw new InvalidOperationException(problem);
		}
		var release = ReleaseInfo.Read(locations.HasServer ? locations.ServerRoot : null);
		var title = "Full backup" + (release.LauncherVersion is null ? "" : $" of {release.LauncherVersion}");
		var baseName = $"{now:yyyyMMdd-HHmmss}-full-backup" + (release.LauncherVersion is null ? "" : "-" + release.LauncherVersion.Replace('.', '-'));
		var final = Path.Combine(root, baseName);
		for (var n = 2; Directory.Exists(final); n++)
		{
			final = Path.Combine(root, $"{baseName}-{n}");
		}
		var partial = final + ".partial";
		Directory.CreateDirectory(partial);

		var files = new List<FullBackupFile>();
		var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { FullBackupManifest.FileName };
		void Add(string role, string name, string source)
		{
			var stored = $"{role}__{name.Replace('/', '_')}";
			for (var n = 2; !used.Add(stored); n++)
			{
				stored = $"{role}__{Path.GetFileNameWithoutExtension(name.Replace('/', '_'))}-{n}{Path.GetExtension(name)}";
			}
			var target = Path.Combine(partial, stored);
			File.Copy(source, target);
			files.Add(new FullBackupFile
			{
				Role = role, Name = name, File = stored, OriginalPath = Path.GetFullPath(source),
				Sha256 = BackupSession.Hash(target), Size = new FileInfo(target).Length,
			});
		}
		void AddFolder(string role, string folder, bool skipBaseline)
		{
			if (!Directory.Exists(folder))
			{
				return;
			}
			foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).Order(StringComparer.OrdinalIgnoreCase))
			{
				var relative = Path.GetRelativePath(folder, file).Replace('\\', '/');
				if (file.EndsWith(".l2config.tmp", StringComparison.OrdinalIgnoreCase)
					|| (skipBaseline && relative.StartsWith(".shipped-baseline/", StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}
				Add(role, relative, file);
			}
		}

		try
		{
			if (locations.HasServer)
			{
				AddFolder(FileRoles.GameConfig, locations.GameConfigDir, skipBaseline: false);
				AddFolder(FileRoles.LoginConfig, locations.LoginConfigDir, skipBaseline: false);
				AddFolder(FileRoles.GamePlayerCopy, locations.PlayerGameConfigDir, skipBaseline: true);
				AddFolder(FileRoles.LoginPlayerCopy, locations.PlayerLoginConfigDir, skipBaseline: true);
				AddFolder(FileRoles.GameBaseline, Path.Combine(locations.PlayerGameConfigDir, ".shipped-baseline"), skipBaseline: false);
				AddFolder(FileRoles.LoginBaseline, Path.Combine(locations.PlayerLoginConfigDir, ".shipped-baseline"), skipBaseline: false);
				var worlds = Path.GetDirectoryName(locations.WorldProfilePath)!;
				if (Directory.Exists(worlds))
				{
					foreach (var file in Directory.EnumerateFiles(worlds, "*.json").Order(StringComparer.OrdinalIgnoreCase))
					{
						Add(FileRoles.World, Path.GetFileName(file), file);
					}
				}
				var releaseFile = Path.Combine(locations.ServerRoot!, "world-release.json");
				if (File.Exists(releaseFile))
				{
					Add(FileRoles.Release, "world-release.json", releaseFile);
				}
			}
			if (locations.HasClient)
			{
				foreach (var file in Directory.EnumerateFiles(locations.ClientSystemDir!, "*.ini").Order(StringComparer.OrdinalIgnoreCase))
				{
					Add(FileRoles.Client, Path.GetFileName(file), file);
				}
			}
			if (files.Count == 0)
			{
				throw new InvalidOperationException("There is nothing to back up yet: choose the server and client folders first.");
			}

			var manifest = new FullBackupManifest
			{
				Created = now, Title = title, LauncherVersion = release.LauncherVersion, EngineVersion = release.EngineVersion,
				ServerRoot = locations.HasServer ? Path.GetFullPath(locations.ServerRoot!) : null,
				ClientSystemDir = locations.HasClient ? Path.GetFullPath(locations.ClientSystemDir!) : null,
				Files = files,
			};
			File.WriteAllText(Path.Combine(partial, FullBackupManifest.FileName), JsonSerializer.Serialize(manifest, FullBackupManifest.JsonOptions));
			Directory.Move(partial, final);
			return new StoredFullBackup(final, manifest);
		}
		catch
		{
			TryDelete(partial);
			throw;
		}
	}

	private static void TryDelete(string partialFolder)
	{
		try
		{
			// Only the half-written copy this call created (it always ends in .partial).
			if (partialFolder.EndsWith(".partial", StringComparison.Ordinal) && Directory.Exists(partialFolder))
			{
				Directory.Delete(partialFolder, recursive: true);
			}
		}
		catch (IOException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}
	}

	internal static bool IsSameOrUnder(string path, string folder)
	{
		var p = Path.GetFullPath(path).TrimEnd('\\', '/');
		var f = Path.GetFullPath(folder).TrimEnd('\\', '/');
		return string.Equals(p, f, StringComparison.OrdinalIgnoreCase)
			|| p.StartsWith(f + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
	}
}
