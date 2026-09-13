using L2Config.Core.Catalog;

namespace L2Config.Core.Storage;

/// <summary>Loads every file the catalog refers to and saves changes back with backups.</summary>
public sealed class SettingsStore
{
	private readonly Dictionary<(string Target, string File), ConfigFile> _files = [];

	public SettingsStore(SettingsCatalog catalog, L2Locations locations)
	{
		Catalog = catalog;
		Locations = locations;
		foreach (var setting in catalog.Settings)
		{
			var id = (setting.Target, setting.File);
			if (!_files.ContainsKey(id))
			{
				_files[id] = CreateFile(setting.Target, setting.File);
			}
		}
	}

	public SettingsCatalog Catalog { get; }
	public L2Locations Locations { get; }
	public IEnumerable<ConfigFile> Files => _files.Values;

	public void Load()
	{
		foreach (var file in _files.Values)
		{
			file.Load();
		}
	}

	public ConfigFile FileFor(SettingDefinition setting) => _files[(setting.Target, setting.File)];

	/// <summary>The value on disk, or null when the file or key is missing.</summary>
	public string? GetValue(SettingDefinition setting)
	{
		var file = FileFor(setting);
		return file.IsLoaded ? file.Get(setting.Section, setting.Key) : null;
	}

	/// <summary>Writes the changed values. Files are backed up before their first write.</summary>
	public SaveResult Save(IReadOnlyCollection<(SettingDefinition Setting, string Value)> changes, string backupsRoot)
	{
		// Check everything before touching anything, so a bad value never leaves half a save behind.
		foreach (var (setting, value) in changes)
		{
			if (setting.IsManaged)
			{
				throw new InvalidOperationException($"{setting.Name} is managed by the L2Everdream launcher.");
			}
			if (SettingValues.Validate(setting, value) is { } problem)
			{
				throw new ArgumentException($"{setting.Name}: {problem}");
			}
			if (!FileFor(setting).IsLoaded)
			{
				throw new InvalidOperationException(FileFor(setting).LoadError);
			}
		}

		var backup = new BackupSession(backupsRoot, DateTime.Now);
		var touched = new List<ConfigFile>();
		foreach (var (setting, value) in changes)
		{
			var file = FileFor(setting);
			file.Set(setting.Section, setting.Key, value);
			if (!touched.Contains(file))
			{
				touched.Add(file);
			}
		}
		foreach (var file in touched)
		{
			file.Save(backup);
		}
		return new SaveResult(touched.SelectMany(f => f.Paths).ToList(), backup.PreservedFiles.Count > 0 ? backup.Folder : null);
	}

	private ConfigFile CreateFile(string target, string file) => target switch
	{
		SettingTargets.ServerGame => new ServerIniFile(
			$"game/config/{file}",
			Path.Combine(Locations.GameConfigDir, file),
			Path.Combine(Locations.PlayerGameConfigDir, file)),
		SettingTargets.ServerLogin => new ServerIniFile(
			$"login/config/{file}",
			Path.Combine(Locations.LoginConfigDir, file),
			Path.Combine(Locations.PlayerLoginConfigDir, file)),
		SettingTargets.ClientOption or SettingTargets.ClientL2Ini => new ClientIniFile(
			$"client/system/{file}",
			Path.Combine(Locations.ClientSystemDir ?? "", file)),
		SettingTargets.WorldProfile => new WorldProfileFile("worlds/world-profile.json", Locations.WorldProfilePath),
		_ => throw new InvalidDataException($"Unknown setting target '{target}'."),
	};
}

public sealed record SaveResult(IReadOnlyList<string> WrittenFiles, string? BackupFolder);
