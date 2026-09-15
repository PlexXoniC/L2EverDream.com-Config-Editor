using L2Config.Core.Backups;
using L2Config.Core.Catalog;
using L2Config.Core.FullBackups;
using L2Config.Core.Storage;

namespace L2Config.Core.Tests;

public class FullBackupTests
{
	private static readonly SettingsCatalog Catalog = CatalogLoader.Load(TestPaths.CatalogPath);

	/// <summary>A small fake L2Everdream install, player data folder and client, laid out like the real ones.</summary>
	private sealed class World
	{
		public World()
		{
			Root = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
			Locations = new L2Locations(Path.Combine(Root, "L2Everdream"), Path.Combine(Root, "client", "system"));
			Write(Locations.GameConfigDir, "Server.ini", "GameserverPort = 7777\n");
			Write(Locations.GameConfigDir, "Rates.ini", "# Experience\nRateXp = 1\nRateSp = 1\n");
			Write(Locations.PlayerGameConfigDir, "Rates.ini", "# Experience\nRateXp = 1\nRateSp = 1\n");
			Write(Path.Combine(Locations.PlayerGameConfigDir, ".shipped-baseline"), "Rates.ini", "RateXp = 1\nRateSp = 1\n");
			Write(Locations.GameConfigDir, "Custom/Wedding.ini", "# Wedding system\n# long explanation\nAllowWedding = False\n");
			Write(Locations.GameConfigDir, "Database.ini", "URL = jdbc:mysql://localhost:33061/world\n");
			Write(Locations.GameConfigDir, "AccessLevels.xml", "<list>\n\t<!-- master -->\n\t<access level=\"100\" />\n</list>\n");
			Write(Locations.GameConfigDir, "ClassMaster.xml", "<list free=\"true\" />\n");
			Write(Locations.LoginConfigDir, "Server.ini", "LoginserverPort = 2106\n");
			Write(Path.GetDirectoryName(Locations.WorldProfilePath)!, "world-profile.json", "{\n  \"name\": \"My World\",\n  \"simsPercent\": 100\n}\n");
			Write(Locations.ServerRoot!, "world-release.json", "{ \"launcherVersion\": \"0.5.19\", \"engineVersion\": \"1.0.48\" }");
			Write(Locations.ClientSystemDir!, "l2.ini", "[URL]\nServerAddr=127.0.0.1\n");
			Write(Locations.ClientSystemDir!, "Option.ini", "[Video]\nGamePlayViewportX=1024\nGamePlayViewportY=768\n");
		}

		public string Root { get; }
		public L2Locations Locations { get; }
		public string Backups => Path.Combine(Root, "full-backups");
		public string ChangeBackups => Path.Combine(Root, "change-backups");

		public static void Write(string folder, string name, string text)
		{
			var path = Path.Combine(folder, name);
			Directory.CreateDirectory(Path.GetDirectoryName(path)!);
			File.WriteAllText(path, text);
		}

		public string Read(string folder, string name) => File.ReadAllText(Path.Combine(folder, name));
	}

	[Fact]
	public void BackupCopiesEverySettingsFileFlatWithVersionsAndFingerprints()
	{
		var world = new World();
		var backup = FullBackupLibrary.Create(world.Backups, world.Locations, new DateTime(2026, 9, 15, 5, 27, 0));

		Assert.Equal("20260915-052700-full-backup-0-5-19", backup.FolderName);
		Assert.Equal("0.5.19", backup.Manifest.LauncherVersion);
		Assert.Empty(Directory.EnumerateDirectories(backup.Folder));
		Assert.Contains(backup.Manifest.Files, f => f.Role == FileRoles.GameConfig && f.Name == "Custom/Wedding.ini" && f.File == "game-config__Custom_Wedding.ini");
		Assert.Contains(backup.Manifest.Files, f => f.Role == FileRoles.GamePlayerCopy && f.Name == "Rates.ini");
		Assert.Contains(backup.Manifest.Files, f => f.Role == FileRoles.GameBaseline && f.Name == "Rates.ini");
		Assert.DoesNotContain(backup.Manifest.Files, f => f.Role == FileRoles.GamePlayerCopy && f.Name.Contains("shipped-baseline"));
		Assert.Contains(backup.Manifest.Files, f => f.Role == FileRoles.World && f.Name == "world-profile.json");
		Assert.Contains(backup.Manifest.Files, f => f.Role == FileRoles.Client && f.Name == "Option.ini");
		Assert.All(backup.Manifest.Files, f => Assert.True(backup.IsIntact(f)));

		var listed = Assert.Single(FullBackupLibrary.List(world.Backups));
		Assert.Equal(backup.Manifest.Files.Count, listed.Manifest.Files.Count);
	}

	[Fact]
	public void BackupsAreRefusedInsideTheInstallFolderThatUpdatesReplace()
	{
		var world = new World();
		Assert.NotNull(FullBackupLibrary.LocationProblem(Path.Combine(world.Locations.ServerRoot!, "my-backups"), world.Locations));
		Assert.Null(FullBackupLibrary.LocationProblem(world.Backups, world.Locations));
		Assert.Throws<InvalidOperationException>(() => FullBackupLibrary.Create(Path.Combine(world.Locations.ServerRoot!, "b"), world.Locations, DateTime.Now));
		Assert.Empty(FullBackupLibrary.List(Path.Combine(world.Locations.ServerRoot!, "b")));
	}

	[Fact]
	public void AnUpdateThatOnlyRewritesCommentsChangesNoSetting()
	{
		var world = new World();
		var backup = FullBackupLibrary.Create(world.Backups, world.Locations, DateTime.Now.AddHours(-1));
		World.Write(world.Locations.GameConfigDir, "Custom/Wedding.ini", "AllowWedding = False\n");
		World.Write(world.Locations.GameConfigDir, "AccessLevels.xml", "<list>\n\t<access level=\"100\" />\n</list>\n");

		var comparison = FullBackupComparer.Compare(backup, world.Locations, Catalog);

		Assert.Empty(comparison.Settings);
		Assert.Equal(2, comparison.Files.Count);
		Assert.All(comparison.Files, f => Assert.True(f.Kind == FileDifferenceKind.CommentsOnly, $"{f.FileLabel}: {f.Kind} {string.Join(" | ", f.Lines.Select(l => (l.Added ? "+" : "-") + l.Text))}"));
		Assert.All(comparison.Files, f => Assert.False(f.CanRestore));
	}

	[Fact]
	public void ComparisonSeparatesChangedValuesShippedDefaultsNewAndManagedSettings()
	{
		var world = new World();
		var backup = FullBackupLibrary.Create(world.Backups, world.Locations, DateTime.Now.AddHours(-1));

		// What an update might do: change a value, change a shipped default, add a key, rewrite a launcher-managed value.
		World.Write(world.Locations.GameConfigDir, "Rates.ini", "RateXp = 1\nRateSp = 3\nRateNew = 2\n");
		World.Write(world.Locations.PlayerGameConfigDir, "Rates.ini", "# Experience\nRateXp = 1\nRateSp = 3\nRateNew = 2\n");
		World.Write(Path.Combine(world.Locations.PlayerGameConfigDir, ".shipped-baseline"), "Rates.ini", "RateXp = 2\nRateSp = 1\n");
		World.Write(world.Locations.GameConfigDir, "Database.ini", "URL = jdbc:mysql://localhost/stock\n");
		World.Write(world.Locations.GameConfigDir, "ClassMaster.xml", "<list free=\"false\" />\n");
		World.Write(world.Locations.GameConfigDir, "world-release.json", "{}");
		File.WriteAllText(Path.Combine(world.Locations.ServerRoot!, "world-release.json"), "{ \"launcherVersion\": \"0.5.20\", \"engineVersion\": \"1.0.82\" }");
		World.Write(Path.GetDirectoryName(world.Locations.WorldProfilePath)!, "world-profile.json", "{\n  \"name\": \"My World\",\n  \"simsPercent\": 50\n}\n");
		World.Write(world.Locations.ClientSystemDir!, "Option.ini", "[Video]\nGamePlayViewportX=1920\nGamePlayViewportY=768\n");

		var comparison = FullBackupComparer.Compare(backup, world.Locations, Catalog);
		SettingDifference Find(string key) => Assert.Single(comparison.Settings, s => s.Key == key);

		Assert.True(comparison.ReleaseChanged);
		Assert.Equal("0.5.20", comparison.CurrentRelease.LauncherVersion);

		var sp = Find("RateSp");
		Assert.Equal(DifferenceKind.ValueChanged, sp.Kind);
		Assert.Equal(("1", "3"), (sp.BackupValue, sp.CurrentValue));
		Assert.NotNull(sp.Definition);
		Assert.True(sp.CanRestore);

		var xp = Find("RateXp");
		Assert.Equal(DifferenceKind.ShippedDefaultChanged, xp.Kind);
		Assert.Equal(("1", "2"), (xp.ShippedBefore, xp.ShippedNow));
		Assert.False(xp.CanRestore);

		Assert.Equal(DifferenceKind.NewSinceBackup, Find("RateNew").Kind);
		Assert.False(Find("RateNew").CanRestore);

		var url = Find("URL");
		Assert.Equal(DifferenceKind.ValueChanged, url.Kind);
		Assert.False(url.CanRestore); // the launcher rewrites it on every start

		var classMaster = Assert.Single(comparison.Files, f => f.FileLabel == "game/config/ClassMaster.xml");
		Assert.Equal(FileDifferenceKind.ContentChanged, classMaster.Kind);
		Assert.False(classMaster.CanRestore);

		var sims = Find("simsPercent");
		Assert.Equal(("100", "50"), (sims.BackupValue, sims.CurrentValue));
		Assert.Equal(SettingArea.World, sims.Target.Area);

		var width = Find("GamePlayViewportX");
		Assert.Equal("Video", width.Section);
		Assert.Equal(SettingArea.Client, width.Target.Area);
		Assert.Equal(("1024", "1920"), (width.BackupValue, width.CurrentValue));
	}

	[Fact]
	public void RestorePutsBackChosenValuesInBothCopiesKeepingCommentsAndNewSettings()
	{
		var world = new World();
		var backup = FullBackupLibrary.Create(world.Backups, world.Locations, DateTime.Now.AddHours(-1));
		World.Write(world.Locations.GameConfigDir, "Rates.ini", "RateXp = 5\nRateSp = 3\nRateNew = 2\n");
		World.Write(world.Locations.PlayerGameConfigDir, "Rates.ini", "# Experience (new comment)\nRateXp = 5\nRateSp = 3\nRateNew = 2\n");
		World.Write(Path.GetDirectoryName(world.Locations.WorldProfilePath)!, "world-profile.json", "{\n  \"name\": \"My World\",\n  \"simsPercent\": 50\n}\n");
		World.Write(world.Locations.ClientSystemDir!, "Option.ini", "[Video]\nGamePlayViewportX=1920\nGamePlayViewportY=768\n");

		var comparison = FullBackupComparer.Compare(backup, world.Locations, Catalog);
		var chosen = comparison.Settings.Where(s => s.Key is "RateSp" or "simsPercent" or "GamePlayViewportX").ToList();
		var report = FullBackupRestorer.Restore(comparison, chosen, [], world.Locations, worldRunning: false, clientRunning: false, world.ChangeBackups);

		Assert.Equal(3, report.Restored);
		Assert.Equal("RateXp = 5\nRateSp = 1\nRateNew = 2\n", world.Read(world.Locations.GameConfigDir, "Rates.ini"));
		Assert.Equal("# Experience (new comment)\nRateXp = 5\nRateSp = 1\nRateNew = 2\n", world.Read(world.Locations.PlayerGameConfigDir, "Rates.ini"));
		Assert.Contains("\"simsPercent\": 100", File.ReadAllText(world.Locations.WorldProfilePath));
		Assert.Contains("GamePlayViewportX=1024", world.Read(world.Locations.ClientSystemDir!, "Option.ini"));

		// What was replaced is in a normal "before restore" backup, so the restore can be undone.
		var undo = Assert.Single(new BackupLibrary(world.ChangeBackups).List());
		Assert.Equal(BackupKind.BeforeRestore, undo.Manifest.Kind);
		Assert.Contains(undo.Manifest.Entries, e => e.Role == "game-player-copy" && e.Name == "Rates.ini");
		Assert.Contains(undo.Manifest.Changes, c => c.From == "3" && c.To == "1");
	}

	[Fact]
	public void NothingIsRestoredWhileTheWorldOrTheClientRuns()
	{
		var world = new World();
		var backup = FullBackupLibrary.Create(world.Backups, world.Locations, DateTime.Now.AddHours(-1));
		World.Write(world.Locations.GameConfigDir, "Rates.ini", "RateXp = 5\nRateSp = 1\n");
		World.Write(world.Locations.PlayerGameConfigDir, "Rates.ini", "RateXp = 5\nRateSp = 1\n");
		World.Write(world.Locations.ClientSystemDir!, "Option.ini", "[Video]\nGamePlayViewportX=1920\nGamePlayViewportY=768\n");

		var comparison = FullBackupComparer.Compare(backup, world.Locations, Catalog);
		var report = FullBackupRestorer.Restore(comparison, comparison.Settings, [], world.Locations, worldRunning: true, clientRunning: true, world.ChangeBackups);

		Assert.Equal(0, report.Restored);
		Assert.Equal(2, report.Refused);
		Assert.Contains("RateXp = 5", world.Read(world.Locations.PlayerGameConfigDir, "Rates.ini"));
		Assert.Contains("GamePlayViewportX=1920", world.Read(world.Locations.ClientSystemDir!, "Option.ini"));
		Assert.Null(report.BeforeRestoreFolder);
	}

	[Fact]
	public void AChangedBackupCopyIsNeverUsed()
	{
		var world = new World();
		var backup = FullBackupLibrary.Create(world.Backups, world.Locations, DateTime.Now.AddHours(-1));
		World.Write(world.Locations.PlayerGameConfigDir, "Rates.ini", "RateXp = 5\nRateSp = 1\n");
		World.Write(world.Locations.GameConfigDir, "Rates.ini", "RateXp = 5\nRateSp = 1\n");
		var comparison = FullBackupComparer.Compare(backup, world.Locations, Catalog);
		var xp = Assert.Single(comparison.Settings, s => s.Key == "RateXp");

		File.WriteAllText(Path.Combine(backup.Folder, "game-player-copy__Rates.ini"), "RateXp = 999\n");
		var report = FullBackupRestorer.Restore(comparison, [xp], [], world.Locations, false, false, world.ChangeBackups);

		Assert.Equal(1, report.Refused);
		Assert.Contains("RateXp = 5", world.Read(world.Locations.PlayerGameConfigDir, "Rates.ini"));
		Assert.NotEmpty(FullBackupComparer.Compare(backup, world.Locations, Catalog).Warnings);
	}

	[Fact]
	public void AChangedXmlFileIsRestoredWholeToBothCopies()
	{
		var world = new World();
		World.Write(world.Locations.PlayerGameConfigDir, "AccessLevels.xml", "<list>\n\t<access level=\"100\" />\n</list>\n");
		var backup = FullBackupLibrary.Create(world.Backups, world.Locations, DateTime.Now.AddHours(-1));
		World.Write(world.Locations.GameConfigDir, "AccessLevels.xml", "<list>\n\t<access level=\"70\" />\n</list>\n");
		World.Write(world.Locations.PlayerGameConfigDir, "AccessLevels.xml", "<list>\n\t<access level=\"70\" />\n</list>\n");

		var comparison = FullBackupComparer.Compare(backup, world.Locations, Catalog);
		var file = Assert.Single(comparison.Files);
		Assert.Equal(FileDifferenceKind.ContentChanged, file.Kind);
		Assert.Contains(file.Lines, l => !l.Added && l.Text.Contains("100"));
		Assert.Contains(file.Lines, l => l.Added && l.Text.Contains("70"));

		var report = FullBackupRestorer.Restore(comparison, [], [file], world.Locations, false, false, world.ChangeBackups);
		Assert.Equal(1, report.Restored);
		Assert.Contains("level=\"100\"", world.Read(world.Locations.GameConfigDir, "AccessLevels.xml"));
		Assert.Contains("level=\"100\"", world.Read(world.Locations.PlayerGameConfigDir, "AccessLevels.xml"));
	}
}
