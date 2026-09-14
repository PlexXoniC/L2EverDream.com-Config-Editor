using L2Config.Core.Backups;
using L2Config.Core.Characters;

namespace L2Config.Core.Tests;

public class BackupTests
{
	private static string TempDir()
	{
		var dir = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		return dir;
	}

	private static RestoreContext Context(bool worldRunning = false, bool clientRunning = false, string? clientDir = null) =>
		new(worldRunning, clientRunning, clientDir, null, new InventoryLimits(80, 100, 250, new HashSet<int>()), ItemCatalog.LoadFromFolder(""));

	[Fact]
	public void FilesAreStoredFlatWithReadableNamesAndAManifest()
	{
		var work = TempDir();
		var original = Path.Combine(work, "deep", "nested", "config", "Rates.ini");
		Directory.CreateDirectory(Path.GetDirectoryName(original)!);
		File.WriteAllText(original, "RateXp = 1\n");

		var session = new BackupSession(Path.Combine(work, "backups"), new DateTime(2026, 9, 14, 10, 15, 0), BackupKind.Settings, "Saved Experience (XP) rate");
		session.PreserveFile(original, "game-config", "Rates.ini");
		session.PreserveFile(original, "game-config", "Rates.ini"); // second call for the same file is ignored
		session.PreserveFile(Path.Combine(work, "missing.ini"), "game-config", "missing.ini");
		session.NoteChange("Experience (XP) rate", "1", "15");

		Assert.Equal("20260914-101500-saved-experience-xp-rate", Path.GetFileName(session.Folder));
		Assert.Equal(["game-config__Rates.ini", "manifest.json"], Directory.EnumerateFileSystemEntries(session.Folder).Select(p => Path.GetFileName(p)!).Order().ToArray());

		var stored = Assert.Single(new BackupLibrary(Path.Combine(work, "backups")).List());
		var entry = Assert.Single(stored.Manifest.Entries);
		Assert.Equal(Path.GetFullPath(original), entry.OriginalPath);
		Assert.Equal("1", Assert.Single(stored.Manifest.Changes).From);
	}

	[Fact]
	public void CustomFolderFilesDoNotCreateSubfolders()
	{
		var work = TempDir();
		var original = Path.Combine(work, "AutoPlay.ini");
		File.WriteAllText(original, "EnableAutoPlay = False\n");
		var session = new BackupSession(work, DateTime.Now, BackupKind.Settings, "x");
		session.PreserveFile(original, "game-player-copy", "Custom/AutoPlay.ini");
		Assert.True(File.Exists(Path.Combine(session.Folder, "game-player-copy__Custom_AutoPlay.ini")));
	}

	[Fact]
	public async Task FilesAreNotRestoredWhileTheWorldRunsAndAreRestoredWhenStopped()
	{
		var work = TempDir();
		var original = Path.Combine(work, "Rates.ini");
		File.WriteAllText(original, "RateXp = 1\n");
		var root = Path.Combine(work, "backups");
		new BackupSession(root, DateTime.Now.AddMinutes(-5), BackupKind.Settings, "before").PreserveFile(original, "game-config", "Rates.ini");
		File.WriteAllText(original, "RateXp = 15\n");

		var library = new BackupLibrary(root);
		var backup = library.List().Single();

		var refused = await library.RestoreAsync(backup, Context(worldRunning: true));
		Assert.Equal(0, refused.Restored);
		Assert.Equal("RateXp = 15\n", File.ReadAllText(original));

		var restored = await library.RestoreAsync(backup, Context());
		Assert.Equal(1, restored.Restored);
		Assert.Equal("RateXp = 1\n", File.ReadAllText(original));
		Assert.NotNull(restored.BeforeRestoreFolder);
		Assert.Equal("RateXp = 15\n", File.ReadAllText(Path.Combine(restored.BeforeRestoreFolder!, "game-config__Rates.ini")));
	}

	[Fact]
	public async Task ClientFilesWaitForTheClientToClose()
	{
		var work = TempDir();
		var original = Path.Combine(work, "Option.ini");
		File.WriteAllText(original, "[Audio]\nMusicVolume=1\n");
		var root = Path.Combine(work, "backups");
		new BackupSession(root, DateTime.Now, BackupKind.Settings, "x").PreserveFile(original, "client", "Option.ini");
		var library = new BackupLibrary(root);

		var report = await library.RestoreAsync(library.List().Single(), Context(clientRunning: true, clientDir: work));
		Assert.Equal(0, report.Restored);
		Assert.Contains("close Lineage 2", report.Lines.Single().Message);
	}

	[Fact]
	public async Task TamperedBackupCopiesAreNotUsed()
	{
		var work = TempDir();
		var original = Path.Combine(work, "Rates.ini");
		File.WriteAllText(original, "RateXp = 1\n");
		var root = Path.Combine(work, "backups");
		var session = new BackupSession(root, DateTime.Now, BackupKind.Settings, "x");
		session.PreserveFile(original, "game-config", "Rates.ini");
		File.WriteAllText(Path.Combine(session.Folder, "game-config__Rates.ini"), "RateXp = 999\n");
		var library = new BackupLibrary(root);

		var report = await library.RestoreAsync(library.List().Single(), Context());
		Assert.Equal(0, report.Restored);
		Assert.Equal("RateXp = 1\n", File.ReadAllText(original));
	}

	[Fact]
	public void OlderNestedBackupsAreStillListed()
	{
		var root = TempDir();
		var legacy = Path.Combine(root, "20260913-140700", "C", "Users", "someone", "game", "config", "Rates.ini");
		Directory.CreateDirectory(Path.GetDirectoryName(legacy)!);
		File.WriteAllText(legacy, "RateXp = 1\n");

		var backup = Assert.Single(new BackupLibrary(root).List());
		Assert.Equal(BackupKind.Legacy, backup.Manifest.Kind);
		Assert.Equal(@"C:\Users\someone\game\config\Rates.ini", Assert.Single(backup.Manifest.Entries).OriginalPath);
	}
}

public class InventoryTests
{
	private static ItemCatalog Catalog()
	{
		var dir = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		File.WriteAllText(Path.Combine(dir, "00000-00099.xml"), """
			<?xml version="1.0" encoding="UTF-8"?>
			<list>
				<item id="1" type="Weapon" name="Short Sword"><set name="is_stackable" val="true" /><set name="crystal_type" val="D" /></item>
				<item id="57" type="EtcItem" name="Adena"><set name="is_stackable" val="true" /></item>
				<item id="1060" type="EtcItem" name="Lesser Healing Potion"><set name="is_stackable" val="true" /></item>
			</list>
			""");
		return ItemCatalog.LoadFromFolder(dir);
	}

	[Fact]
	public void ReadsItemsAndNeverTreatsWeaponsAsStackable()
	{
		var catalog = Catalog();
		Assert.Equal(3, catalog.Count);
		Assert.False(catalog.Find(1)!.Stackable);
		Assert.True(catalog.Find(57)!.Stackable);
	}

	[Fact]
	public void SlotsCountLikeTheServer()
	{
		var catalog = Catalog();
		var inventory = new List<InventoryItem> { new(10, 57, 100, 0, false), new(11, 1, 1, 0, true) };
		var carried = inventory.Select(i => i.ItemId).ToHashSet();

		Assert.Equal(0, InventorySlots.SlotsFor(57, 5000, carried, catalog));   // more adena: same stack
		Assert.Equal(1, InventorySlots.SlotsFor(1060, 50, carried, catalog));   // new stackable: one slot
		Assert.Equal(3, InventorySlots.SlotsFor(1, 3, carried, catalog));       // three swords: three slots
		Assert.Equal(4, InventorySlots.SlotsFor(99999, 4, carried, catalog));   // unknown item: cautious

		var pending = new[] { new DeliveryItem(1060, 10, 0), new DeliveryItem(1060, 5, 0), new DeliveryItem(1, 2, 0) };
		Assert.Equal(2 + 1 + 2, InventorySlots.UsedIncludingPending(inventory, pending, catalog));
	}

	[Fact]
	public void LimitDependsOnRaceAndGameMasterLevel()
	{
		var limits = new InventoryLimits(80, 100, 250, new HashSet<int> { 70, 100 });
		PlayerCharacter Char(int race, int access) => new(1, "x", "a", 1, 0, null, null, race, access);
		Assert.Equal(80, limits.For(Char(0, 0)));
		Assert.Equal(100, limits.For(Char(InventoryLimits.DwarfRace, 0)));
		Assert.Equal(250, limits.For(Char(0, 100)));
	}

	[Fact]
	public void ParsesTheServersDeliveryFormat()
	{
		var items = PendingDelivery.ParseItems("57 1000;6 1 7; 1060 20 ");
		Assert.Equal([new DeliveryItem(57, 1000, 0), new DeliveryItem(6, 1, 7), new DeliveryItem(1060, 20, 0)], items);
	}

	[LocalInstallFact]
	public void ReadsTheRealItemList()
	{
		var catalog = ItemCatalog.LoadFromServer(TestPaths.RealLocations.ServerRoot!);
		Assert.True(catalog.Count > 8000);
		Assert.Equal("Adena", catalog.Find(57)!.Name);
		Assert.True(catalog.Find(57)!.Stackable);
	}
}
