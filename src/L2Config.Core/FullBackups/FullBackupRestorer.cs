using L2Config.Core.Backups;
using L2Config.Core.Characters;
using L2Config.Core.Storage;

namespace L2Config.Core.FullBackups;

/// <summary>
/// Puts chosen settings from a full backup back, one value at a time, so comments and settings a newer version added are kept.
/// Server and world settings are never restored while the world is running, client settings never while Lineage 2 is open.
/// Everything replaced is first backed up into the normal backups folder, so a restore can be undone from the Backups tab.
/// </summary>
public static class FullBackupRestorer
{
	public static RestoreReport Restore(
		BackupComparison comparison,
		IReadOnlyCollection<SettingDifference> settings,
		IReadOnlyCollection<FileDifference> files,
		L2Locations locations,
		bool worldRunning,
		bool clientRunning,
		string changeBackupsRoot)
	{
		var backup = comparison.Backup;
		var before = new BackupSession(changeBackupsRoot, DateTime.Now, BackupKind.BeforeRestore,
			$"Before restoring settings from the full backup of {backup.Manifest.Created:d MMM yyyy HH:mm}");
		var lines = new List<RestoreLine>();

		string? Blocked(SettingArea area) => area == SettingArea.Client
			? clientRunning ? "close Lineage 2 first — the client rewrites its settings when it exits." : null
			: worldRunning ? "stop the world from the L2Everdream launcher first. Settings are never restored while the world is running." : null;

		foreach (var group in settings.GroupBy(s => s.Target))
		{
			var target = group.Key;
			var chosen = group.ToList();
			string Label(SettingDifference s) => s.Definition?.Name ?? $"{s.FileLabel} › {(s.Section is null ? "" : $"[{s.Section}] › ")}{s.Key}";

			if (Blocked(target.Area) is { } reason)
			{
				lines.AddRange(chosen.Select(s => RestoreLine.Fail($"{Label(s)}: {reason}")));
				continue;
			}
			var refused = chosen.Where(s => !s.CanRestore || s.BackupValue is null).ToList();
			lines.AddRange(refused.Select(s => RestoreLine.Fail($"{Label(s)}: {s.NotRestorableReason ?? "nothing to put back."}")));
			var toWrite = chosen.Except(refused).ToList();
			if (toWrite.Count == 0)
			{
				continue;
			}
			if (!SourcesIntact(backup, target))
			{
				lines.AddRange(toWrite.Select(s => RestoreLine.Fail($"{Label(s)}: the backup copy has been changed since it was taken, so it was not used.")));
				continue;
			}

			var file = OpenFile(target, locations);
			file.Load();
			if (!file.IsLoaded)
			{
				lines.AddRange(toWrite.Select(s => RestoreLine.Fail($"{Label(s)}: {file.LoadError}")));
				continue;
			}
			try
			{
				foreach (var s in toWrite)
				{
					before.NoteChange($"{Label(s)} ({s.FileLabel})", file.Get(s.Section, s.Key), s.BackupValue);
					file.Set(s.Section, s.Key, s.BackupValue!);
				}
				file.Save(before);
				lines.AddRange(toWrite.Select(s => RestoreLine.Success($"{Label(s)}: restored to {(s.IsSecret ? "the backup value" : Quote(s.BackupValue!))}.")));
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException)
			{
				lines.AddRange(toWrite.Select(s => RestoreLine.Fail($"{Label(s)}: not restored — {ex.Message}")));
			}
		}

		foreach (var diff in files)
		{
			lines.Add(RestoreWholeFile(backup, diff, locations, Blocked(diff.Target.Area), before));
		}
		return new RestoreReport(lines, before.IsEmpty ? null : before.Folder);
	}

	private static ConfigFile OpenFile(RestoreTarget target, L2Locations locations) => target.FileKind switch
	{
		RestoreTarget.ServerIni => new ServerIniFile(target.Server!, target.Name,
			Path.Combine(target.Server == "game" ? locations.GameConfigDir : locations.LoginConfigDir, target.Name),
			Path.Combine(target.Server == "game" ? locations.PlayerGameConfigDir : locations.PlayerLoginConfigDir, target.Name)),
		RestoreTarget.ClientIni => new ClientIniFile($"client/system/{target.Name}", Path.Combine(locations.ClientSystemDir!, target.Name)),
		RestoreTarget.WorldProfile => new WorldProfileFile("worlds/world-profile.json", locations.WorldProfilePath),
		_ => throw new InvalidOperationException($"{target.Name} can't be restored setting by setting."),
	};

	private static bool SourcesIntact(StoredFullBackup backup, RestoreTarget target)
	{
		IEnumerable<FullBackupFile?> sources = target.FileKind switch
		{
			RestoreTarget.ServerIni or RestoreTarget.ServerFile =>
				[backup.Find(FileRoles.Config(target.Server!), target.Name), backup.Find(FileRoles.PlayerCopy(target.Server!), target.Name)],
			RestoreTarget.ClientIni => [backup.Find(FileRoles.Client, target.Name)],
			_ => [backup.Find(FileRoles.World, target.Name)],
		};
		var present = sources.OfType<FullBackupFile>().ToList();
		return present.Count > 0 && present.All(backup.IsIntact);
	}

	private static RestoreLine RestoreWholeFile(StoredFullBackup backup, FileDifference diff, L2Locations locations, string? blocked, BackupSession before)
	{
		var target = diff.Target;
		if (!diff.CanRestore || diff.Kind is FileDifferenceKind.CommentsOnly or FileDifferenceKind.NewSinceBackup || target.FileKind != RestoreTarget.ServerFile)
		{
			return RestoreLine.Fail($"{diff.FileLabel}: {diff.NotRestorableReason ?? "this file is restored setting by setting, not as a whole."}");
		}
		if (blocked is not null)
		{
			return RestoreLine.Fail($"{diff.FileLabel}: {blocked}");
		}
		if (!SourcesIntact(backup, target))
		{
			return RestoreLine.Fail($"{diff.FileLabel}: the backup copy has been changed since it was taken, so it was not used.");
		}

		var server = target.Server!;
		var install = backup.Find(FileRoles.Config(server), target.Name);
		var copy = backup.Find(FileRoles.PlayerCopy(server), target.Name);
		var installPath = Path.Combine(server == "game" ? locations.GameConfigDir : locations.LoginConfigDir, target.Name);
		var copyPath = Path.Combine(server == "game" ? locations.PlayerGameConfigDir : locations.PlayerLoginConfigDir, target.Name);

		// Both copies get the same content (the protected copy wins, as it does when the launcher starts the world).
		var source = copy ?? install!;
		var written = 0;
		foreach (var (path, role) in new[] { (installPath, FileRoles.Config(server)), (copyPath, FileRoles.PlayerCopy(server)) })
		{
			if (role == FileRoles.PlayerCopy(server) && copy is null && !File.Exists(path))
			{
				continue; // the launcher does not protect this file; do not create a protected copy it never had
			}
			if (!Directory.Exists(Path.GetDirectoryName(path)))
			{
				continue;
			}
			before.PreserveFile(path, role, target.Name);
			var temp = path + ".l2config.tmp";
			File.Copy(backup.PathOf(source), temp, overwrite: true);
			File.Move(temp, path, overwrite: true);
			written++;
		}
		return written > 0
			? RestoreLine.Success($"{diff.FileLabel}: restored.")
			: RestoreLine.Fail($"{diff.FileLabel}: its folder no longer exists.");
	}

	private static string Quote(string value) => value.Length == 0 ? "(empty)" : value.Length > 60 ? $"\"{value[..57]}…\"" : $"\"{value}\"";
}
