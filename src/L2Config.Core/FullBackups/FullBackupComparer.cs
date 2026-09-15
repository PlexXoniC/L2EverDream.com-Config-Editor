using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using L2Config.Core.Catalog;
using L2Config.Core.Ini;
using L2Config.Core.Storage;

namespace L2Config.Core.FullBackups;

public enum SettingArea
{
	Server,
	World,
	Client,
}

public enum DifferenceKind
{
	/// <summary>The value is different now from the backup.</summary>
	ValueChanged,

	/// <summary>The setting was in the backup but is no longer in the file.</summary>
	MissingNow,

	/// <summary>The setting is in the file now but was not in the backup (usually added by an update).</summary>
	NewSinceBackup,

	/// <summary>Your value is the same, but what L2Everdream ships for it changed.</summary>
	ShippedDefaultChanged,
}

/// <summary>Which file a restore writes and how.</summary>
public sealed record RestoreTarget(SettingArea Area, string FileKind, string? Server, string Name)
{
	public const string ServerIni = "server-ini";
	public const string ServerFile = "server-file";
	public const string ClientIni = "client-ini";
	public const string WorldProfile = "world-profile";
}

/// <summary>One setting whose value, presence or shipped default differs between a full backup and the files on disk now.</summary>
public sealed record SettingDifference(
	RestoreTarget Target,
	string FileLabel,
	string? Section,
	string Key,
	SettingDefinition? Definition,
	string? BackupValue,
	string? CurrentValue,
	string? ShippedBefore,
	string? ShippedNow,
	DifferenceKind Kind,
	string? NotRestorableReason)
{
	public bool CanRestore => NotRestorableReason is null;
	public bool IsSecret => Definition?.Secret == true || Key.Contains("passw", StringComparison.OrdinalIgnoreCase);
}

public enum FileDifferenceKind
{
	ContentChanged,
	CommentsOnly,
	MissingNow,
	NewSinceBackup,
}

/// <summary>A whole file (XML, text, an ini that can't be read) that differs.</summary>
public sealed record FileDifference(
	RestoreTarget Target,
	string FileLabel,
	FileDifferenceKind Kind,
	IReadOnlyList<DiffLine> Lines,
	string? NotRestorableReason)
{
	public bool CanRestore => NotRestorableReason is null;
}

public sealed record DiffLine(bool Added, string Text);

public sealed record BackupComparison(
	StoredFullBackup Backup,
	ReleaseInfo CurrentRelease,
	IReadOnlyList<SettingDifference> Settings,
	IReadOnlyList<FileDifference> Files,
	IReadOnlyList<string> Warnings)
{
	public bool ReleaseChanged => Backup.Manifest.LauncherVersion != CurrentRelease.LauncherVersion || Backup.Manifest.EngineVersion != CurrentRelease.EngineVersion;
}

/// <summary>
/// Compares a full backup with the files on disk now, setting by setting. Values are compared, not files: an update that only
/// rewrites comments shows as "comments only" and never as a setting change.
/// </summary>
public static partial class FullBackupComparer
{
	/// <summary>Files the launcher writes on every start; restoring them would be undone at once.</summary>
	private static readonly HashSet<string> LauncherWrittenFiles = new(StringComparer.OrdinalIgnoreCase) { "ClassMaster.xml" };

	public static BackupComparison Compare(StoredFullBackup backup, L2Locations locations, SettingsCatalog catalog)
	{
		var settings = new List<SettingDifference>();
		var files = new List<FileDifference>();
		var warnings = new List<string>();
		var lookup = new CatalogLookup(catalog);

		string? ReadBackup(FullBackupFile? file)
		{
			if (file is null)
			{
				return null;
			}
			if (!backup.IsIntact(file))
			{
				warnings.Add($"{file.Name} ({file.Role}): the backup copy has been changed or is missing since it was taken, so it was not used.");
				return null;
			}
			return ReadText(backup.PathOf(file));
		}

		var hasServerBackup = backup.Manifest.Files.Any(f => f.Role.EndsWith("-config", StringComparison.Ordinal));
		if (hasServerBackup && locations.HasServer)
		{
			foreach (var server in new[] { "game", "login" })
			{
				CompareServer(server, backup, locations, lookup, ReadBackup, settings, files);
			}
			CompareWorldProfile(backup, locations, lookup, ReadBackup, settings);
		}
		else if (hasServerBackup)
		{
			warnings.Add("The server folder is not chosen, so server and world settings were not compared.");
		}

		var hasClientBackup = backup.Manifest.Files.Any(f => f.Role == FileRoles.Client);
		if (hasClientBackup && locations.HasClient)
		{
			CompareClient(backup, locations, lookup, settings, files, warnings);
		}
		else if (hasClientBackup)
		{
			warnings.Add("The client folder is not chosen, so client settings were not compared.");
		}

		return new BackupComparison(backup, ReleaseInfo.Read(locations.HasServer ? locations.ServerRoot : null), settings, files, warnings);
	}

	private static void CompareServer(string server, StoredFullBackup backup, L2Locations locations, CatalogLookup lookup,
		Func<FullBackupFile?, string?> readBackup, List<SettingDifference> settings, List<FileDifference> files)
	{
		var installDir = server == "game" ? locations.GameConfigDir : locations.LoginConfigDir;
		var copyDir = server == "game" ? locations.PlayerGameConfigDir : locations.PlayerLoginConfigDir;
		var baselineDir = Path.Combine(copyDir, ".shipped-baseline");
		var target = server == "game" ? SettingTargets.ServerGame : SettingTargets.ServerLogin;

		var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var f in backup.Manifest.Files.Where(f => f.Role == FileRoles.Config(server) || f.Role == FileRoles.PlayerCopy(server)))
		{
			names.Add(f.Name);
		}
		foreach (var dir in new[] { installDir, copyDir })
		{
			if (Directory.Exists(dir))
			{
				foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
				{
					var relative = Path.GetRelativePath(dir, file).Replace('\\', '/');
					if (!relative.StartsWith(".shipped-baseline/", StringComparison.OrdinalIgnoreCase) && !file.EndsWith(".l2config.tmp", StringComparison.OrdinalIgnoreCase))
					{
						names.Add(relative);
					}
				}
			}
		}

		foreach (var name in names)
		{
			var label = $"{server}/config/{name}";
			// The launcher's protected copy is what survives and is applied on start; the install copy is the fallback.
			var backupText = readBackup(backup.Find(FileRoles.PlayerCopy(server), name)) ?? readBackup(backup.Find(FileRoles.Config(server), name));
			var currentPath = File.Exists(Path.Combine(copyDir, name)) ? Path.Combine(copyDir, name) : Path.Combine(installDir, name);
			var currentText = File.Exists(currentPath) ? ReadText(currentPath) : null;

			if (name.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
			{
				var shippedBefore = readBackup(backup.Find(FileRoles.Baseline(server), name));
				var shippedNowPath = Path.Combine(baselineDir, name);
				var shippedNow = File.Exists(shippedNowPath) ? ReadText(shippedNowPath) : null;
				if (backupText == currentText && shippedBefore == shippedNow)
				{
					continue;
				}
				var restore = new RestoreTarget(SettingArea.Server, RestoreTarget.ServerIni, server, name);
				var before = settings.Count;
				AddIniDifferences(settings, restore, label, target, name, lookup, backupText, currentText, shippedBefore, shippedNow);
				if (settings.Count == before && backupText is not null && currentText is not null && backupText != currentText)
				{
					files.Add(new FileDifference(restore, label, FileDifferenceKind.CommentsOnly, [], "Only comments or blank lines changed — no setting is different."));
				}
				continue;
			}

			if (backupText == currentText)
			{
				continue;
			}
			var fileTarget = new RestoreTarget(SettingArea.Server, RestoreTarget.ServerFile, server, name);
			var launcherWritten = LauncherWrittenFiles.Contains(Path.GetFileName(name))
				? "The L2Everdream launcher writes this file every time the world starts (from its \"Free class change\" option), so restoring it would be undone."
				: null;
			if (backupText is null)
			{
				files.Add(new FileDifference(fileTarget, label, FileDifferenceKind.NewSinceBackup, [], "This file was not in the backup."));
			}
			else if (currentText is null)
			{
				files.Add(new FileDifference(fileTarget, label, FileDifferenceKind.MissingNow, [], launcherWritten));
			}
			else if (Normalize(name, backupText).SequenceEqual(Normalize(name, currentText)))
			{
				files.Add(new FileDifference(fileTarget, label, FileDifferenceKind.CommentsOnly, [], "Only comments or blank lines changed — nothing to restore."));
			}
			else
			{
				files.Add(new FileDifference(fileTarget, label, FileDifferenceKind.ContentChanged,
					DiffLines(Normalize(name, backupText), Normalize(name, currentText)), launcherWritten));
			}
		}
	}

	private static void AddIniDifferences(List<SettingDifference> settings, RestoreTarget restore, string label, string target, string file,
		CatalogLookup lookup, string? backupText, string? currentText, string? shippedBeforeText, string? shippedNowText)
	{
		var backup = IniValues(backupText);
		var current = IniValues(currentText);
		var shippedBefore = IniValues(shippedBeforeText);
		var shippedNow = IniValues(shippedNowText);
		var keys = backup.Keys.Concat(current.Keys).Concat(shippedBefore.Keys).Concat(shippedNow.Keys)
			.Distinct(IniKeyComparer.Instance).ToList();

		foreach (var id in keys)
		{
			var b = backup.GetValueOrDefault(id);
			var c = current.GetValueOrDefault(id);
			var sb = shippedBefore.GetValueOrDefault(id);
			var sn = shippedNow.GetValueOrDefault(id);
			if (b == c && (sb == sn || b is null))
			{
				continue;
			}
			var definition = lookup.Find(target, file, id.Section, id.Key);
			var kind = b == c ? DifferenceKind.ShippedDefaultChanged
				: b is null ? DifferenceKind.NewSinceBackup
				: c is null ? DifferenceKind.MissingNow
				: DifferenceKind.ValueChanged;
			settings.Add(new SettingDifference(restore, label, id.Section, id.Key, definition, b, c, sb, sn, kind,
				NotRestorable(kind, definition, b, currentText is null)));
		}
	}

	private static string? NotRestorable(DifferenceKind kind, SettingDefinition? definition, string? backupValue, bool fileMissing)
	{
		if (kind == DifferenceKind.ShippedDefaultChanged)
		{
			return "Your value is the same as in the backup; only what L2Everdream ships for it changed.";
		}
		if (kind == DifferenceKind.NewSinceBackup)
		{
			return "This setting was not in the backup, so there is nothing to put back.";
		}
		if (fileMissing)
		{
			return "The file this setting lives in no longer exists.";
		}
		if (definition?.IsManaged == true)
		{
			return definition.ManagedReason;
		}
		if (definition is not null && backupValue is not null && SettingValues.Validate(definition, backupValue) is { } problem)
		{
			return $"The backup value is not allowed: {problem}";
		}
		return null;
	}

	private static void CompareWorldProfile(StoredFullBackup backup, L2Locations locations, CatalogLookup lookup,
		Func<FullBackupFile?, string?> readBackup, List<SettingDifference> settings)
	{
		var backupText = readBackup(backup.Find(FileRoles.World, "world-profile.json"));
		var currentText = File.Exists(locations.WorldProfilePath) ? ReadText(locations.WorldProfilePath) : null;
		if (backupText == currentText)
		{
			return;
		}
		var b = JsonValues(backupText);
		var c = JsonValues(currentText);
		var restore = new RestoreTarget(SettingArea.World, RestoreTarget.WorldProfile, null, "world-profile.json");
		foreach (var key in b.Keys.Concat(c.Keys).Distinct())
		{
			var bv = b.GetValueOrDefault(key);
			var cv = c.GetValueOrDefault(key);
			if (bv == cv)
			{
				continue;
			}
			var definition = lookup.Find(SettingTargets.WorldProfile, "world-profile.json", null, key);
			var kind = bv is null ? DifferenceKind.NewSinceBackup : cv is null ? DifferenceKind.MissingNow : DifferenceKind.ValueChanged;
			settings.Add(new SettingDifference(restore, "worlds/world-profile.json", null, key, definition, bv, cv, null, null, kind,
				NotRestorable(kind, definition, bv, currentText is null)));
		}
	}

	private static void CompareClient(StoredFullBackup backup, L2Locations locations, CatalogLookup lookup,
		List<SettingDifference> settings, List<FileDifference> files, List<string> warnings)
	{
		var system = locations.ClientSystemDir!;
		var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var f in backup.Manifest.Files.Where(f => f.Role == FileRoles.Client))
		{
			names.Add(f.Name);
		}
		foreach (var file in Directory.EnumerateFiles(system, "*.ini"))
		{
			names.Add(Path.GetFileName(file));
		}

		foreach (var name in names)
		{
			var entry = backup.Find(FileRoles.Client, name);
			if (entry is not null && !backup.IsIntact(entry))
			{
				warnings.Add($"{name} (client): the backup copy has been changed or is missing since it was taken, so it was not used.");
				continue;
			}
			var backupBytes = entry is null ? null : File.ReadAllBytes(backup.PathOf(entry));
			var currentPath = Path.Combine(system, name);
			var currentBytes = File.Exists(currentPath) ? File.ReadAllBytes(currentPath) : null;
			if (backupBytes is not null && currentBytes is not null && backupBytes.AsSpan().SequenceEqual(currentBytes))
			{
				continue;
			}
			var label = $"client/system/{name}";
			var restore = new RestoreTarget(SettingArea.Client, RestoreTarget.ClientIni, null, name);
			if (backupBytes is null)
			{
				files.Add(new FileDifference(restore, label, FileDifferenceKind.NewSinceBackup, [], "This file was not in the backup."));
				continue;
			}
			if (currentBytes is null)
			{
				files.Add(new FileDifference(restore, label, FileDifferenceKind.MissingNow, [], "The game no longer has this file; start Lineage 2 once so it creates it, then compare again."));
				continue;
			}
			string backupText, currentText;
			try
			{
				backupText = L2IniCodec.Decode(backupBytes, out _);
				currentText = L2IniCodec.Decode(currentBytes, out _);
			}
			catch (Exception ex) when (ex is InvalidDataException or NotSupportedException)
			{
				files.Add(new FileDifference(restore, label, FileDifferenceKind.ContentChanged, [], "This file's format can't be read, so its settings can't be compared one by one."));
				continue;
			}
			var target = string.Equals(name, "l2.ini", StringComparison.OrdinalIgnoreCase) ? SettingTargets.ClientL2Ini : SettingTargets.ClientOption;
			var before = settings.Count;
			AddIniDifferences(settings, restore, label, target, name, lookup, backupText, currentText, null, null);
			if (settings.Count == before)
			{
				files.Add(new FileDifference(restore, label, FileDifferenceKind.CommentsOnly, [], "Only the file's layout changed — no setting is different."));
			}
		}
	}

	// ------------------------------------------------------------------------------------------ helpers

	internal static string ReadText(string path)
	{
		var bytes = File.ReadAllBytes(path);
		var body = bytes.AsSpan();
		if (body.Length >= 3 && body[0] == 0xEF && body[1] == 0xBB && body[2] == 0xBF)
		{
			body = body[3..];
		}
		try
		{
			return new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(body);
		}
		catch (DecoderFallbackException)
		{
			return Encoding.Latin1.GetString(body);
		}
	}

	private static Dictionary<IniKey, string> IniValues(string? text)
	{
		var result = new Dictionary<IniKey, string>(IniKeyComparer.Instance);
		if (text is null)
		{
			return result;
		}
		foreach (var ((section, key), value) in IniDocument.Parse(text).Values())
		{
			result.TryAdd(new IniKey(section, key), value);
		}
		return result;
	}

	private static Dictionary<string, string> JsonValues(string? text)
	{
		var result = new Dictionary<string, string>(StringComparer.Ordinal);
		if (text is null)
		{
			return result;
		}
		try
		{
			if (JsonNode.Parse(text) is JsonObject root)
			{
				foreach (var (key, node) in root)
				{
					result[key] = node switch
					{
						null => "null",
						JsonValue v when v.TryGetValue<bool>(out var flag) => flag ? "true" : "false",
						_ => node.ToJsonString(),
					};
				}
			}
		}
		catch (JsonException)
		{
		}
		return result;
	}

	/// <summary>The text with comments and blank lines removed and each line trimmed, for deciding whether anything real changed.</summary>
	private static IReadOnlyList<string> Normalize(string name, string text)
	{
		if (name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
		{
			text = XmlComment().Replace(text, "");
		}
		var hashComments = !name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);
		return text.Split('\n')
			.Select(l => l.Trim())
			.Where(l => l.Length > 0 && !(hashComments && l.StartsWith('#')))
			.ToList();
	}

	/// <summary>A minimal line diff (longest common subsequence), capped so huge files can't stall the app.</summary>
	private static IReadOnlyList<DiffLine> DiffLines(IReadOnlyList<string> before, IReadOnlyList<string> after)
	{
		const int cap = 4000;
		if (before.Count > cap || after.Count > cap)
		{
			return [new DiffLine(false, $"({before.Count:N0} lines before, {after.Count:N0} now — too large to show line by line)")];
		}
		var lcs = new int[before.Count + 1, after.Count + 1];
		for (var i = before.Count - 1; i >= 0; i--)
		{
			for (var j = after.Count - 1; j >= 0; j--)
			{
				lcs[i, j] = before[i] == after[j] ? lcs[i + 1, j + 1] + 1 : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
			}
		}
		var lines = new List<DiffLine>();
		int a = 0, b = 0;
		while (a < before.Count || b < after.Count)
		{
			if (a < before.Count && b < after.Count && before[a] == after[b])
			{
				a++;
				b++;
			}
			else if (b < after.Count && (a == before.Count || lcs[a, b + 1] >= lcs[a + 1, b]))
			{
				lines.Add(new DiffLine(true, after[b++]));
			}
			else
			{
				lines.Add(new DiffLine(false, before[a++]));
			}
		}
		return lines;
	}

	[GeneratedRegex("<!--.*?-->", RegexOptions.Singleline)]
	private static partial Regex XmlComment();

	private readonly record struct IniKey(string? Section, string Key);

	private sealed class IniKeyComparer : IEqualityComparer<IniKey>
	{
		public static readonly IniKeyComparer Instance = new();

		public bool Equals(IniKey x, IniKey y) =>
			string.Equals(x.Section, y.Section, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Key, y.Key, StringComparison.OrdinalIgnoreCase);

		public int GetHashCode(IniKey k) =>
			HashCode.Combine(k.Section?.ToUpperInvariant(), k.Key.ToUpperInvariant());
	}

	private sealed class CatalogLookup(SettingsCatalog catalog)
	{
		private readonly Dictionary<string, SettingDefinition> _byLocation = catalog.Settings
			.GroupBy(s => Id(s.Target, s.File, s.Section, s.Key))
			.ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

		public SettingDefinition? Find(string target, string file, string? section, string key) =>
			_byLocation.GetValueOrDefault(Id(target, file, section, key));

		private static string Id(string target, string file, string? section, string key) => $"{target}|{file}|{section}|{key}";
	}
}
