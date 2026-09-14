using L2Config.Core.Backups;
using L2Config.Core.Characters;
using MySqlConnector;

namespace L2Config.Core.Tests;

/// <summary>
/// Runs the real SQL against a THROWAWAY database that has the Mobius characters, items and custom_mail tables.
/// Never point this at a world's database. Set L2CONFIG_TEST_DB to a connection string, e.g.
/// "Server=127.0.0.1;Port=33999;User ID=root;Database=l2test". Skipped when it is not set.
/// </summary>
public class WorldDatabaseIntegrationTests
{
	private static readonly string? ConnectionString = Environment.GetEnvironmentVariable("L2CONFIG_TEST_DB");

	private const long CharId = 268500001;

	private static async Task<(WorldDatabase Db, BackupSession Backup, ItemCatalog Items, InventoryLimits Limits)> SetUpAsync(int online = 0, int slots = 5)
	{
		await using (var connection = new MySqlConnection(ConnectionString))
		{
			await connection.OpenAsync();
			var sql = $"""
				DELETE FROM items WHERE owner_id = {CharId};
				DELETE FROM custom_mail WHERE receiver = {CharId};
				DELETE FROM characters WHERE charId = {CharId};
				INSERT INTO characters (account_name, charId, char_name, level, online, race, accesslevel, createDate) VALUES ('tester', {CharId}, 'Tester', 40, {online}, 0, 0, '2026-01-01');
				INSERT INTO items (owner_id, object_id, item_id, count, enchant_level, loc, loc_data) VALUES
				  ({CharId}, 268600001, 57, 1000, 0, 'INVENTORY', 0),
				  ({CharId}, 268600002, 1, 1, 3, 'PAPERDOLL', 7);
				""";
			await using var command = new MySqlCommand(sql, connection);
			await command.ExecuteNonQueryAsync();
		}
		var itemsDir = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(itemsDir);
		File.WriteAllText(Path.Combine(itemsDir, "items.xml"), """
			<list>
				<item id="1" type="Weapon" name="Short Sword" />
				<item id="57" type="EtcItem" name="Adena"><set name="is_stackable" val="true" /></item>
				<item id="1060" type="EtcItem" name="Lesser Healing Potion"><set name="is_stackable" val="true" /></item>
			</list>
			""");
		var backup = new BackupSession(Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N")), DateTime.Now, BackupKind.Characters, "test");
		return (new WorldDatabase(ConnectionString!), backup, ItemCatalog.LoadFromFolder(itemsDir), new InventoryLimits(slots, slots, slots, new HashSet<int>()));
	}

	private static async Task<PlayerCharacter> TesterAsync(WorldDatabase db) =>
		(await db.ListPlayerCharactersAsync()).Single(c => c.CharId == CharId);

	[Fact]
	public async Task ChangesAdenaAndBacksUpTheRowFirst()
	{
		if (ConnectionString is null) return;
		var (db, backup, _, _) = await SetUpAsync();
		var tester = await TesterAsync(db);
		Assert.Equal(1000, tester.Adena);
		Assert.Equal(2, tester.SlotsUsed);

		var adena = (await db.ListInventoryAsync(CharId)).Single(i => i.ItemId == 57);
		var outcome = await db.SetItemCountAsync(tester, adena, 5000, "Adena", backup);
		Assert.True(outcome.Ok, outcome.Message);
		Assert.Equal(5000, (await TesterAsync(db)).Adena);

		var stored = new BackupLibrary(Path.GetDirectoryName(backup.Folder)!).List().Single();
		var rows = BackupLibrary.ReadRows(stored, stored.Manifest.Entries.Single());
		Assert.Equal("1000", rows.Rows.Single()["count"]);

		// Stale expected count is refused.
		var stale = await db.SetItemCountAsync(tester, adena, 1, "Adena", backup);
		Assert.False(stale.Ok);
	}

	[Fact]
	public async Task NothingChangesWhileTheCharacterIsOnline()
	{
		if (ConnectionString is null) return;
		var (db, backup, items, limits) = await SetUpAsync(online: 1);
		var tester = await TesterAsync(db);
		var adena = (await db.ListInventoryAsync(CharId)).Single(i => i.ItemId == 57);

		Assert.False((await db.SetItemCountAsync(tester, adena, 1, "Adena", backup)).Ok);
		Assert.False((await db.QueueDeliveryAsync(tester, items.Find(1060)!, 5, 0, limits, items, 2_000_000_000, backup)).Ok);
		Assert.Equal(1000, (await TesterAsync(db)).Adena);
		Assert.True(backup.IsEmpty);
	}

	[Fact]
	public async Task DeliveriesRespectTheInventoryLimitIncludingWhatIsAlreadyQueued()
	{
		if (ConnectionString is null) return;
		var (db, backup, items, limits) = await SetUpAsync(slots: 5);
		var tester = await TesterAsync(db);

		Assert.True((await db.QueueDeliveryAsync(tester, items.Find(1060)!, 50, 0, limits, items, 2_000_000_000, backup)).Ok); // 3 slots
		Assert.True((await db.QueueDeliveryAsync(tester, items.Find(1060)!, 50, 0, limits, items, 2_000_000_000, backup)).Ok); // same stack: still 3
		Assert.True((await db.QueueDeliveryAsync(tester, items.Find(1)!, 2, 4, limits, items, 2_000_000_000, backup)).Ok);      // 5 slots
		var full = await db.QueueDeliveryAsync(tester, items.Find(1)!, 1, 0, limits, items, 2_000_000_000, backup);             // would be 6
		Assert.False(full.Ok);
		Assert.Contains("Not enough room", full.Message);

		var tooMuchAdena = await db.QueueDeliveryAsync(tester, items.Find(57)!, 2_000, 0, limits, items, 2_500, backup);
		Assert.False(tooMuchAdena.Ok);

		var pending = await db.ListDeliveriesAsync(CharId);
		Assert.Equal(3, pending.Count);
		Assert.Equal(3, pending.Select(p => p.Date).Distinct().Count());
		Assert.Contains(pending, p => p.ItemsText == "1 2 4");

		Assert.True((await db.CancelDeliveryAsync(tester, pending[0], backup)).Ok);
		Assert.Equal(2, (await db.ListDeliveriesAsync(CharId)).Count);
	}

	[Fact]
	public async Task RestoringPutsRowsBackForOfflineCharacters()
	{
		if (ConnectionString is null) return;
		var (db, backup, items, limits) = await SetUpAsync();
		var root = Path.GetDirectoryName(backup.Folder)!;
		var tester = await TesterAsync(db);
		var sword = (await db.ListInventoryAsync(CharId)).Single(i => i.ItemId == 1);

		Assert.True((await db.SetItemCountAsync(tester, sword, 0, "Short Sword", backup)).Ok);
		Assert.True((await db.QueueDeliveryAsync(tester, items.Find(1060)!, 10, 0, limits, items, 2_000_000_000, backup)).Ok);
		Assert.DoesNotContain(await db.ListInventoryAsync(CharId), i => i.ItemId == 1);

		var library = new BackupLibrary(root);
		var stored = library.List().Single(b => b.Manifest.Kind == BackupKind.Characters);
		var report = await library.RestoreAsync(stored, new RestoreContext(true, false, null, db, limits, items));

		Assert.True(report.Refused == 0, string.Join("\n", report.Lines.Select(l => l.Message)));
		var restored = (await db.ListInventoryAsync(CharId)).Single(i => i.ItemId == 1);
		Assert.Equal(268600002, restored.ObjectId);
		Assert.Equal(3, restored.Enchant);
		Assert.True(restored.Equipped);
		Assert.Empty(await db.ListDeliveriesAsync(CharId));
	}
}
