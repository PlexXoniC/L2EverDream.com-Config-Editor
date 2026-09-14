using System.Globalization;
using System.Text.RegularExpressions;
using L2Config.Core.Backups;
using L2Config.Core.Ini;
using L2Config.Core.Storage;
using MySqlConnector;

namespace L2Config.Core.Characters;

/// <summary>
/// Reads and edits player characters' inventories in the running world's database.
/// <list type="bullet">
/// <item>The game server keeps an online character's inventory in memory and writes it back, so rows are changed only
/// while the character is offline. Each change locks the character row and re-checks <c>online = 0</c> inside the same
/// transaction, so a login at the wrong moment waits for (or defeats) the change instead of racing it.</item>
/// <item>The server hands out item object IDs from memory, so this class never invents one. New items are queued in
/// <c>custom_mail</c>; the server's CustomMailManager adds them itself when the character is next online.</item>
/// <item>Every change first stores the affected rows in a <see cref="BackupSession"/>.</item>
/// <item>Simulated players (account <c>$sim</c>) are never listed or changed.</item>
/// </list>
/// </summary>
public sealed partial class WorldDatabase(string connectionString)
{
	public const int AdenaItemId = 57;
	public const string SimAccount = "$sim";
	public const string DeliverySubject = "L2Everdream Config";
	private const string InventoryLocations = "('INVENTORY','PAPERDOLL')";

	public static WorldDatabase FromServerFolder(L2Locations locations)
	{
		var path = Path.Combine(locations.GameConfigDir, "Database.ini");
		if (!File.Exists(path))
		{
			throw new WorldDatabaseException("Not found: game\\config\\Database.ini in the server folder.");
		}
		var ini = IniDocument.Parse(File.ReadAllText(path));
		return new WorldDatabase(BuildConnectionString(ini.Get(null, "URL") ?? "", ini.Get(null, "Login") ?? "root", ini.Get(null, "Password") ?? ""));
	}

	public static string BuildConnectionString(string jdbcUrl, string user, string password)
	{
		var match = JdbcUrl().Match(jdbcUrl.Trim());
		if (!match.Success)
		{
			throw new WorldDatabaseException("Database.ini does not contain a database URL this app understands.");
		}
		// The launcher's database listens on 127.0.0.1 only. "localhost" makes Windows try IPv6 (::1) first, and a refused
		// connection there costs about two seconds every time a connection is opened.
		var host = match.Groups["host"].Value;
		return new MySqlConnectionStringBuilder
		{
			Server = string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ? "127.0.0.1" : host,
			Port = match.Groups["port"].Success ? uint.Parse(match.Groups["port"].Value, CultureInfo.InvariantCulture) : 3306,
			Database = match.Groups["db"].Value,
			UserID = user,
			Password = password,
			ConnectionTimeout = 3,
			DefaultCommandTimeout = 15,
			Pooling = true,
			ConnectionIdleTimeout = 30,
		}.ConnectionString;
	}

	public string DatabaseName => new MySqlConnectionStringBuilder(connectionString).Database;

	// ------------------------------------------------------------------------------------------ reading

	public async Task<IReadOnlyList<PlayerCharacter>> ListPlayerCharactersAsync(CancellationToken cancellation = default)
	{
		await using var connection = await OpenAsync(cancellation);
		await using var command = new MySqlCommand(
			$"""
			SELECT c.charId, c.char_name, c.account_name, c.level, c.online, c.race, c.accesslevel,
			       (SELECT i.object_id FROM items i WHERE i.owner_id = c.charId AND i.item_id = @adena AND i.loc = 'INVENTORY'
			         ORDER BY i.count DESC LIMIT 1) AS adena_object,
			       (SELECT i.count FROM items i WHERE i.owner_id = c.charId AND i.item_id = @adena AND i.loc = 'INVENTORY'
			         ORDER BY i.count DESC LIMIT 1) AS adena,
			       (SELECT COUNT(*) FROM items i WHERE i.owner_id = c.charId AND i.loc IN {InventoryLocations}) AS slots
			FROM characters c
			WHERE c.account_name IS NOT NULL AND c.account_name <> @sim
			ORDER BY c.account_name, c.char_name
			""", connection);
		command.Parameters.AddWithValue("@adena", AdenaItemId);
		command.Parameters.AddWithValue("@sim", SimAccount);

		var result = new List<PlayerCharacter>();
		await using var reader = await command.ExecuteReaderAsync(cancellation);
		while (await reader.ReadAsync(cancellation))
		{
			result.Add(new PlayerCharacter(
				CharId: Convert.ToInt64(reader["charId"], CultureInfo.InvariantCulture),
				Name: reader.GetString("char_name"),
				Account: reader.GetString("account_name"),
				Level: IntOr(reader, "level", 0),
				OnlineState: IntOr(reader, "online", 0),
				AdenaObjectId: LongOrNull(reader, "adena_object"),
				Adena: LongOrNull(reader, "adena"),
				Race: IntOr(reader, "race", 0),
				AccessLevel: IntOr(reader, "accesslevel", 0),
				SlotsUsed: IntOr(reader, "slots", 0)));
		}
		return result;
	}

	public async Task<IReadOnlyList<InventoryItem>> ListInventoryAsync(long charId, CancellationToken cancellation = default)
	{
		await using var connection = await OpenAsync(cancellation);
		await using var command = new MySqlCommand(
			$"SELECT object_id, item_id, count, enchant_level, loc FROM items WHERE owner_id = @char AND loc IN {InventoryLocations} ORDER BY loc DESC, item_id",
			connection);
		command.Parameters.AddWithValue("@char", charId);
		var result = new List<InventoryItem>();
		await using var reader = await command.ExecuteReaderAsync(cancellation);
		while (await reader.ReadAsync(cancellation))
		{
			result.Add(new InventoryItem(
				ObjectId: Convert.ToInt64(reader["object_id"], CultureInfo.InvariantCulture),
				ItemId: IntOr(reader, "item_id", 0),
				Count: LongOrNull(reader, "count") ?? 0,
				Enchant: IntOr(reader, "enchant_level", 0),
				Equipped: string.Equals(reader.GetString("loc"), "PAPERDOLL", StringComparison.Ordinal)));
		}
		return result;
	}

	/// <summary>Items queued for the server to add (rows in custom_mail for this character).</summary>
	public async Task<IReadOnlyList<PendingDelivery>> ListDeliveriesAsync(long charId, CancellationToken cancellation = default)
	{
		await using var connection = await OpenAsync(cancellation);
		return await ReadDeliveriesAsync(connection, null, charId, forUpdate: false, cancellation);
	}

	// ------------------------------------------------------------------------------------------ editing

	/// <summary>Sets an item stack's count (0 removes the item). Only while the character is offline and the count is unchanged.</summary>
	public async Task<EditOutcome> SetItemCountAsync(PlayerCharacter character, InventoryItem item, long newCount, string itemName, BackupSession backup,
		CancellationToken cancellation = default)
	{
		if (newCount < 0 || newCount > InventoryLimits.MaxStackCount)
		{
			return EditOutcome.Refused($"The count must be between 0 and {InventoryLimits.MaxStackCount:N0}.");
		}
		await using var connection = await OpenAsync(cancellation);
		await using var tx = await connection.BeginTransactionAsync(cancellation);

		if (await LockOfflineCharacterAsync(connection, tx, character.CharId, cancellation) is { } refusal)
		{
			return refusal;
		}
		var rows = await ReadRowsAsync(connection, tx,
			$"SELECT * FROM items WHERE object_id = @object AND owner_id = @char AND loc IN {InventoryLocations} FOR UPDATE",
			[("@object", item.ObjectId), ("@char", character.CharId)], cancellation);
		if (rows.Count == 0)
		{
			return EditOutcome.Refused("The item is no longer in the character's inventory. Refresh and try again.");
		}
		var current = long.Parse(rows[0]["count"] ?? "0", CultureInfo.InvariantCulture);
		if (current != item.Count)
		{
			return EditOutcome.Refused("The item's count changed since the inventory was loaded. Refresh and try again.");
		}

		var entry = backup.RecordRows(new DbRowsBackup
		{
			Database = DatabaseName,
			Table = "items",
			Operation = newCount == 0 ? DbOperation.Delete : DbOperation.Update,
			CharId = character.CharId,
			CharName = character.Name,
			Rows = rows,
		}, newCount == 0 ? $"{character.Name}: removed {itemName}" : $"{character.Name}: {itemName} count {current:N0} → {newCount:N0}");

		await using var command = new MySqlCommand(
			newCount == 0
				? "DELETE FROM items WHERE object_id = @object AND owner_id = @char AND count = @expected"
				: "UPDATE items SET count = @count WHERE object_id = @object AND owner_id = @char AND count = @expected",
			connection, tx);
		command.Parameters.AddWithValue("@count", newCount);
		command.Parameters.AddWithValue("@object", item.ObjectId);
		command.Parameters.AddWithValue("@char", character.CharId);
		command.Parameters.AddWithValue("@expected", current);
		if (await command.ExecuteNonQueryAsync(cancellation) != 1)
		{
			await tx.RollbackAsync(cancellation);
			backup.Forget(entry);
			return EditOutcome.Refused("The item changed while saving. Nothing was changed.");
		}
		await tx.CommitAsync(cancellation);
		backup.NoteChange($"{character.Name} › {itemName}", current.ToString("N0", CultureInfo.InvariantCulture),
			newCount == 0 ? "removed" : newCount.ToString("N0", CultureInfo.InvariantCulture));
		return EditOutcome.Done(newCount == 0 ? $"Removed {itemName}." : $"{itemName} is now {newCount:N0}.");
	}

	/// <summary>
	/// Queues new items for the server to add the next time the character is online. Refused if, once every pending
	/// delivery has arrived, the inventory would exceed its limit, or a stack would exceed its maximum.
	/// </summary>
	public async Task<EditOutcome> QueueDeliveryAsync(PlayerCharacter character, ItemTemplate item, long count, int enchant, InventoryLimits limits,
		ItemCatalog catalog, long maxAdena, BackupSession backup, CancellationToken cancellation = default)
	{
		if (count < 1 || count > InventoryLimits.MaxStackCount)
		{
			return EditOutcome.Refused($"The amount must be between 1 and {InventoryLimits.MaxStackCount:N0}.");
		}
		if (item.Stackable && enchant != 0)
		{
			return EditOutcome.Refused("Stackable items cannot be enchanted.");
		}
		await using var connection = await OpenAsync(cancellation);
		await using var tx = await connection.BeginTransactionAsync(cancellation);

		if (await LockOfflineCharacterAsync(connection, tx, character.CharId, cancellation) is { } refusal)
		{
			return refusal;
		}
		var inventory = await ReadInventoryForCheckAsync(connection, tx, character.CharId, cancellation);
		var pending = await ReadDeliveriesAsync(connection, tx, character.CharId, forUpdate: true, cancellation);
		var pendingItems = pending.SelectMany(p => p.Items).ToList();

		var limit = limits.For(character);
		var used = InventorySlots.UsedIncludingPending(inventory, pendingItems, catalog);
		var carried = inventory.Select(i => i.ItemId).Concat(pendingItems.Select(p => p.ItemId)).ToHashSet();
		var needed = InventorySlots.SlotsFor(item.Id, count, carried, catalog);
		if (used + needed > limit)
		{
			return EditOutcome.Refused(
				$"Not enough room: the inventory would need {used + needed:N0} of {limit:N0} slots ({used:N0} used or already on the way, " +
				$"{needed:N0} more for this). Remove items, or raise the {limits.DescribeFor(character)} in the Server tab.");
		}
		if (item.Stackable)
		{
			var total = inventory.Where(i => i.ItemId == item.Id).Sum(i => i.Count) + pendingItems.Where(p => p.ItemId == item.Id).Sum(p => p.Count) + count;
			var cap = item.Id == AdenaItemId ? maxAdena : InventoryLimits.MaxStackCount;
			if (total > cap)
			{
				return EditOutcome.Refused(item.Id == AdenaItemId
					? $"That would make {total:N0} adena, above the server's maximum of {cap:N0} (Player.ini › MaxAdena)."
					: $"That would make a stack of {total:N0}, above the maximum of {cap:N0}.");
			}
		}

		var date = await NextFreeDeliveryTimeAsync(connection, tx, character.CharId, cancellation);
		var itemsText = enchant > 0 ? $"{item.Id} {count} {enchant}" : $"{item.Id} {count}";
		var row = new Dictionary<string, string?>
		{
			["date"] = date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
			["receiver"] = character.CharId.ToString(CultureInfo.InvariantCulture),
			["subject"] = DeliverySubject,
			["message"] = "Items added from L2Everdream Config.",
			["items"] = itemsText,
		};
		var description = $"{character.Name}: deliver {count:N0} × {item.Name}{(enchant > 0 ? $" +{enchant}" : "")}";
		backup.RecordRows(new DbRowsBackup
		{
			Database = DatabaseName, Table = "custom_mail", Operation = DbOperation.Insert,
			CharId = character.CharId, CharName = character.Name, Rows = [row],
		}, description);

		await using var insert = new MySqlCommand(
			"INSERT INTO custom_mail (date, receiver, subject, message, items) VALUES (@date, @receiver, @subject, @message, @items)", connection, tx);
		foreach (var (key, value) in row)
		{
			insert.Parameters.AddWithValue("@" + key, value);
		}
		await insert.ExecuteNonQueryAsync(cancellation);
		await tx.CommitAsync(cancellation);
		backup.NoteChange($"{character.Name} › deliveries", null, $"{count:N0} × {item.Name}{(enchant > 0 ? $" +{enchant}" : "")}");
		return EditOutcome.Done($"Queued {count:N0} × {item.Name}. The server adds it the next time {character.Name} is online.");
	}

	/// <summary>Removes a delivery that has not arrived yet.</summary>
	public async Task<EditOutcome> CancelDeliveryAsync(PlayerCharacter character, PendingDelivery delivery, BackupSession backup, CancellationToken cancellation = default)
	{
		await using var connection = await OpenAsync(cancellation);
		await using var tx = await connection.BeginTransactionAsync(cancellation);
		var rows = await ReadRowsAsync(connection, tx,
			"SELECT * FROM custom_mail WHERE receiver = @receiver AND date = @date AND items = @items LIMIT 1 FOR UPDATE",
			[("@receiver", character.CharId), ("@date", delivery.Date), ("@items", delivery.ItemsText)], cancellation);
		if (rows.Count == 0)
		{
			return EditOutcome.Refused("That delivery is no longer waiting — the server has probably already added it. Refresh to see the inventory.");
		}
		var entry = backup.RecordRows(new DbRowsBackup
		{
			Database = DatabaseName, Table = "custom_mail", Operation = DbOperation.Delete,
			CharId = character.CharId, CharName = character.Name, Rows = rows,
		}, $"{character.Name}: cancelled delivery {delivery.ItemsText}");
		await using var delete = new MySqlCommand("DELETE FROM custom_mail WHERE receiver = @receiver AND date = @date AND items = @items LIMIT 1", connection, tx);
		delete.Parameters.AddWithValue("@receiver", character.CharId);
		delete.Parameters.AddWithValue("@date", delivery.Date);
		delete.Parameters.AddWithValue("@items", delivery.ItemsText);
		if (await delete.ExecuteNonQueryAsync(cancellation) != 1)
		{
			await tx.RollbackAsync(cancellation);
			backup.Forget(entry);
			return EditOutcome.Refused("The delivery changed while cancelling. Refresh and try again.");
		}
		await tx.CommitAsync(cancellation);
		backup.NoteChange($"{character.Name} › deliveries", delivery.ItemsText, "cancelled");
		return EditOutcome.Done("Delivery cancelled.");
	}

	// ------------------------------------------------------------------------------------------ restoring

	/// <summary>When the database (and so the world) last started.</summary>
	public async Task<DateTime> WorldStartedAsync(CancellationToken cancellation = default)
	{
		await using var connection = await OpenAsync(cancellation);
		await using var command = new MySqlCommand("SELECT NOW() - INTERVAL VARIABLE_VALUE SECOND FROM information_schema.GLOBAL_STATUS WHERE VARIABLE_NAME = 'UPTIME'", connection);
		return Convert.ToDateTime(await command.ExecuteScalarAsync(cancellation), CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Puts backed-up rows back. Only for characters that are offline. A removed item is re-created with its original
	/// object ID only if the world has not restarted since the backup (the server still holds that ID as used);
	/// otherwise it is refused, because the server may since have given that ID to another item.
	/// </summary>
	public async Task<IReadOnlyList<RestoreLine>> RestoreRowsAsync(DbRowsBackup rows, DateTime backupCreated, InventoryLimits limits, ItemCatalog catalog,
		BackupSession before, CancellationToken cancellation = default)
	{
		if (!string.Equals(rows.Database, DatabaseName, StringComparison.OrdinalIgnoreCase))
		{
			return [RestoreLine.Fail($"This backup is from another world's database ({rows.Database}); the chosen server uses {DatabaseName}.")];
		}
		var worldStarted = await WorldStartedAsync(cancellation);
		var lines = new List<RestoreLine>();
		foreach (var row in rows.Rows)
		{
			await using var connection = await OpenAsync(cancellation);
			await using var tx = await connection.BeginTransactionAsync(cancellation);
			var line = (rows.Table, rows.Operation) switch
			{
				("items", DbOperation.Update or DbOperation.Delete) => await RestoreItemRowAsync(connection, tx, rows, row, worldStarted, backupCreated, before, cancellation),
				("custom_mail", DbOperation.Insert) => await UndoDeliveryAsync(connection, tx, rows, row, before, cancellation),
				("custom_mail", DbOperation.Delete) => await RequeueDeliveryAsync(connection, tx, rows, row, limits, catalog, before, cancellation),
				_ => RestoreLine.Fail($"Restoring {rows.Operation} on {rows.Table} is not supported."),
			};
			if (line.Ok)
			{
				await tx.CommitAsync(cancellation);
			}
			else
			{
				await tx.RollbackAsync(cancellation);
			}
			lines.Add(line);
		}
		return lines;
	}

	private async Task<RestoreLine> RestoreItemRowAsync(MySqlConnection connection, MySqlTransaction tx, DbRowsBackup rows, Dictionary<string, string?> row,
		DateTime worldStarted, DateTime backupCreated, BackupSession before, CancellationToken cancellation)
	{
		if (!long.TryParse(row.GetValueOrDefault("object_id"), out var objectId) || !long.TryParse(row.GetValueOrDefault("owner_id"), out var ownerId))
		{
			return RestoreLine.Fail("A backed-up item row has no object or owner ID.");
		}
		var who = rows.CharName ?? $"character {ownerId}";
		if (await LockOfflineCharacterAsync(connection, tx, ownerId, cancellation) is { } refusal)
		{
			return RestoreLine.Fail($"{who}: {refusal.Message}");
		}
		var columns = await TableColumnsAsync(connection, tx, "items", cancellation);
		var usable = row.Keys.Where(columns.Contains).ToList();
		var current = await ReadRowsAsync(connection, tx, "SELECT * FROM items WHERE object_id = @object FOR UPDATE", [("@object", objectId)], cancellation);

		if (current.Count > 0)
		{
			if (current[0].GetValueOrDefault("owner_id") != ownerId.ToString(CultureInfo.InvariantCulture))
			{
				return RestoreLine.Fail($"{who}: item {objectId} now belongs to someone else, so it was not changed.");
			}
			before.RecordRows(new DbRowsBackup { Database = DatabaseName, Table = "items", Operation = DbOperation.Update, CharId = ownerId, CharName = rows.CharName, Rows = current },
				$"{who}: item {objectId} before restore");
			await using var update = new MySqlCommand(
				$"UPDATE items SET {string.Join(", ", usable.Select(c => $"`{c}` = @{c}"))} WHERE object_id = @key", connection, tx);
			AddParameters(update, row, usable);
			update.Parameters.AddWithValue("@key", objectId);
			await update.ExecuteNonQueryAsync(cancellation);
			return RestoreLine.Success($"{who}: item {objectId} restored (count {row.GetValueOrDefault("count")}).");
		}

		if (worldStarted > backupCreated)
		{
			return RestoreLine.Fail($"{who}: item {objectId} was removed and the world has restarted since this backup, so the server may have reused its ID. " +
				"Add the item again from the inventory editor instead.");
		}
		before.RecordRows(new DbRowsBackup { Database = DatabaseName, Table = "items", Operation = DbOperation.Insert, CharId = ownerId, CharName = rows.CharName, Rows = [row] },
			$"{who}: item {objectId} re-created by restore");
		await using var insert = new MySqlCommand(
			$"INSERT INTO items ({string.Join(", ", usable.Select(c => $"`{c}`"))}) VALUES ({string.Join(", ", usable.Select(c => "@" + c))})", connection, tx);
		AddParameters(insert, row, usable);
		await insert.ExecuteNonQueryAsync(cancellation);
		return RestoreLine.Success($"{who}: item {objectId} put back (count {row.GetValueOrDefault("count")}).");
	}

	private async Task<RestoreLine> UndoDeliveryAsync(MySqlConnection connection, MySqlTransaction tx, DbRowsBackup rows, Dictionary<string, string?> row,
		BackupSession before, CancellationToken cancellation)
	{
		var who = rows.CharName ?? "character";
		var current = await ReadRowsAsync(connection, tx, "SELECT * FROM custom_mail WHERE receiver = @receiver AND date = @date AND items = @items LIMIT 1 FOR UPDATE",
			[("@receiver", row.GetValueOrDefault("receiver")), ("@date", row.GetValueOrDefault("date")), ("@items", row.GetValueOrDefault("items"))], cancellation);
		if (current.Count == 0)
		{
			return RestoreLine.Fail($"{who}: the delivery {row.GetValueOrDefault("items")} already arrived, so there is nothing to undo here. Remove the items in the inventory editor if needed.");
		}
		before.RecordRows(new DbRowsBackup { Database = DatabaseName, Table = "custom_mail", Operation = DbOperation.Delete, CharId = rows.CharId, CharName = rows.CharName, Rows = current },
			$"{who}: pending delivery before restore");
		await using var delete = new MySqlCommand("DELETE FROM custom_mail WHERE receiver = @receiver AND date = @date AND items = @items LIMIT 1", connection, tx);
		delete.Parameters.AddWithValue("@receiver", row.GetValueOrDefault("receiver"));
		delete.Parameters.AddWithValue("@date", row.GetValueOrDefault("date"));
		delete.Parameters.AddWithValue("@items", row.GetValueOrDefault("items"));
		await delete.ExecuteNonQueryAsync(cancellation);
		return RestoreLine.Success($"{who}: pending delivery {row.GetValueOrDefault("items")} removed.");
	}

	private async Task<RestoreLine> RequeueDeliveryAsync(MySqlConnection connection, MySqlTransaction tx, DbRowsBackup rows, Dictionary<string, string?> row,
		InventoryLimits limits, ItemCatalog catalog, BackupSession before, CancellationToken cancellation)
	{
		var who = rows.CharName ?? "character";
		if (!long.TryParse(row.GetValueOrDefault("receiver"), out var charId))
		{
			return RestoreLine.Fail("A backed-up delivery has no receiver.");
		}
		var character = (await ReadRowsAsync(connection, tx, "SELECT race, accesslevel, online FROM characters WHERE charId = @char FOR UPDATE", [("@char", charId)], cancellation)).FirstOrDefault();
		if (character is null)
		{
			return RestoreLine.Fail($"{who}: the character no longer exists.");
		}
		if (character.GetValueOrDefault("online") is not ("0" or null))
		{
			return RestoreLine.Fail($"{who}: log the character out first so the inventory can be checked exactly.");
		}
		var inventory = await ReadInventoryForCheckAsync(connection, tx, charId, cancellation);
		var pending = (await ReadDeliveriesAsync(connection, tx, charId, forUpdate: true, cancellation)).SelectMany(p => p.Items).ToList();
		var wanted = PendingDelivery.ParseItems(row.GetValueOrDefault("items") ?? "");
		var probe = new PlayerCharacter(charId, who, "", 0, 0, null, null, IntOr(character, "race"), IntOr(character, "accesslevel"), inventory.Count);
		var used = InventorySlots.UsedIncludingPending(inventory, pending.Concat(wanted), catalog);
		if (used > limits.For(probe))
		{
			return RestoreLine.Fail($"{who}: not enough inventory room to re-queue {row.GetValueOrDefault("items")} ({used:N0} of {limits.For(probe):N0} slots).");
		}
		before.RecordRows(new DbRowsBackup { Database = DatabaseName, Table = "custom_mail", Operation = DbOperation.Insert, CharId = charId, CharName = rows.CharName, Rows = [row] },
			$"{who}: delivery re-queued by restore");
		await using var insert = new MySqlCommand("INSERT INTO custom_mail (date, receiver, subject, message, items) VALUES (@date, @receiver, @subject, @message, @items)", connection, tx);
		foreach (var key in new[] { "date", "receiver", "subject", "message", "items" })
		{
			insert.Parameters.AddWithValue("@" + key, row.GetValueOrDefault(key) ?? "");
		}
		await insert.ExecuteNonQueryAsync(cancellation);
		return RestoreLine.Success($"{who}: delivery {row.GetValueOrDefault("items")} queued again.");
	}

	// ------------------------------------------------------------------------------------------ helpers

	/// <summary>Locks the character row; returns a refusal when it is online, a sim, or gone.</summary>
	private static async Task<EditOutcome?> LockOfflineCharacterAsync(MySqlConnection connection, MySqlTransaction tx, long charId, CancellationToken cancellation)
	{
		var rows = await ReadRowsAsync(connection, tx, "SELECT online, account_name FROM characters WHERE charId = @char FOR UPDATE", [("@char", charId)], cancellation);
		if (rows.Count == 0)
		{
			return EditOutcome.Refused("The character no longer exists.");
		}
		if (rows[0].GetValueOrDefault("account_name") == SimAccount)
		{
			return EditOutcome.Refused("Simulated players cannot be edited.");
		}
		return rows[0].GetValueOrDefault("online") is "0" or null ? null : EditOutcome.Refused("The character is logged in. Log it out and try again.");
	}

	private static async Task<List<InventoryItem>> ReadInventoryForCheckAsync(MySqlConnection connection, MySqlTransaction tx, long charId, CancellationToken cancellation)
	{
		var rows = await ReadRowsAsync(connection, tx, $"SELECT object_id, item_id, count, enchant_level, loc FROM items WHERE owner_id = @char AND loc IN {InventoryLocations} FOR UPDATE",
			[("@char", charId)], cancellation);
		return rows.Select(r => new InventoryItem(long.Parse(r["object_id"]!, CultureInfo.InvariantCulture), IntOr(r, "item_id"),
			long.TryParse(r.GetValueOrDefault("count"), out var c) ? c : 0, IntOr(r, "enchant_level"), r.GetValueOrDefault("loc") == "PAPERDOLL")).ToList();
	}

	private static async Task<IReadOnlyList<PendingDelivery>> ReadDeliveriesAsync(MySqlConnection connection, MySqlTransaction? tx, long charId, bool forUpdate, CancellationToken cancellation)
	{
		var rows = await ReadRowsAsync(connection, tx,
			$"SELECT DATE_FORMAT(date, '%Y-%m-%d %H:%i:%s') AS date, subject, items FROM custom_mail WHERE receiver = @char ORDER BY date{(forUpdate ? " FOR UPDATE" : "")}",
			[("@char", charId)], cancellation);
		return rows.Select(r => new PendingDelivery(r["date"]!, r.GetValueOrDefault("subject") ?? "", r.GetValueOrDefault("items") ?? "")).ToList();
	}

	private static async Task<DateTime> NextFreeDeliveryTimeAsync(MySqlConnection connection, MySqlTransaction tx, long charId, CancellationToken cancellation)
	{
		// The server deletes delivered rows by (date, receiver), so each queued row gets its own second.
		var now = DateTime.Now;
		var candidate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
		var taken = (await ReadDeliveriesAsync(connection, tx, charId, forUpdate: true, cancellation)).Select(d => d.Date).ToHashSet();
		while (taken.Contains(candidate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)))
		{
			candidate = candidate.AddSeconds(1);
		}
		return candidate;
	}

	private static async Task<HashSet<string>> TableColumnsAsync(MySqlConnection connection, MySqlTransaction tx, string table, CancellationToken cancellation)
	{
		var rows = await ReadRowsAsync(connection, tx, "SELECT COLUMN_NAME AS c FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t", [("@t", table)], cancellation);
		return rows.Select(r => r["c"]!).Where(c => SafeColumn().IsMatch(c)).ToHashSet(StringComparer.Ordinal);
	}

	private static async Task<List<Dictionary<string, string?>>> ReadRowsAsync(MySqlConnection connection, MySqlTransaction? tx, string sql,
		IEnumerable<(string Name, object? Value)> parameters, CancellationToken cancellation)
	{
		await using var command = new MySqlCommand(sql, connection, tx);
		foreach (var (name, value) in parameters)
		{
			command.Parameters.AddWithValue(name, value);
		}
		var rows = new List<Dictionary<string, string?>>();
		await using var reader = await command.ExecuteReaderAsync(cancellation);
		while (await reader.ReadAsync(cancellation))
		{
			var row = new Dictionary<string, string?>(StringComparer.Ordinal);
			for (var i = 0; i < reader.FieldCount; i++)
			{
				row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i) switch
				{
					DateTime d => d.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
					IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
					var v => v.ToString(),
				};
			}
			rows.Add(row);
		}
		return rows;
	}

	private static void AddParameters(MySqlCommand command, Dictionary<string, string?> row, IEnumerable<string> columns)
	{
		foreach (var column in columns)
		{
			command.Parameters.AddWithValue("@" + column, (object?)row[column] ?? DBNull.Value);
		}
	}

	private async Task<MySqlConnection> OpenAsync(CancellationToken cancellation)
	{
		var connection = new MySqlConnection(connectionString);
		try
		{
			await connection.OpenAsync(cancellation);
			return connection;
		}
		catch (MySqlException ex)
		{
			await connection.DisposeAsync();
			throw new WorldDatabaseException(
				"Could not reach the world's database. It only runs while your world is running — start the world from the L2Everdream launcher, then refresh.",
				ex);
		}
	}

	private static int IntOr(MySqlDataReader reader, string column, int fallback)
	{
		var i = reader.GetOrdinal(column);
		return reader.IsDBNull(i) ? fallback : Convert.ToInt32(reader.GetValue(i), CultureInfo.InvariantCulture);
	}

	private static long? LongOrNull(MySqlDataReader reader, string column)
	{
		var i = reader.GetOrdinal(column);
		return reader.IsDBNull(i) ? null : Convert.ToInt64(reader.GetValue(i), CultureInfo.InvariantCulture);
	}

	private static int IntOr(Dictionary<string, string?> row, string column) =>
		int.TryParse(row.GetValueOrDefault(column), out var v) ? v : 0;

	[GeneratedRegex(@"^jdbc:(?:mysql|mariadb)://(?<host>[^/:?]+)(?::(?<port>\d+))?/(?<db>[^/?]+)", RegexOptions.IgnoreCase)]
	private static partial Regex JdbcUrl();

	[GeneratedRegex(@"^[A-Za-z0-9_]+$")]
	private static partial Regex SafeColumn();
}

public sealed record PlayerCharacter(long CharId, string Name, string Account, int Level, int OnlineState, long? AdenaObjectId, long? Adena,
	int Race = 0, int AccessLevel = 0, int SlotsUsed = 0)
{
	/// <summary>0 offline, 1 online, 2 online without a client (offline shop).</summary>
	public bool IsOnline => OnlineState != 0;
}

public sealed record InventoryItem(long ObjectId, int ItemId, long Count, int Enchant, bool Equipped);

public sealed record DeliveryItem(int ItemId, long Count, int Enchant);

public sealed record PendingDelivery(string Date, string Subject, string ItemsText)
{
	public IReadOnlyList<DeliveryItem> Items => ParseItems(ItemsText);

	/// <summary>The server's format: "itemId count [enchant];itemId count;…".</summary>
	public static IReadOnlyList<DeliveryItem> ParseItems(string text)
	{
		var result = new List<DeliveryItem>();
		foreach (var part in text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			var bits = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (bits.Length >= 1 && int.TryParse(bits[0], out var id))
			{
				var count = bits.Length >= 2 && long.TryParse(bits[1], out var c) ? c : 1;
				var enchant = bits.Length >= 3 && int.TryParse(bits[2], out var e) ? e : 0;
				result.Add(new DeliveryItem(id, count, enchant));
			}
		}
		return result;
	}
}

public sealed record EditOutcome(bool Ok, string Message)
{
	public static EditOutcome Done(string message) => new(true, message);
	public static EditOutcome Refused(string message) => new(false, message);
}

public sealed record RestoreLine(bool Ok, string Message)
{
	public static RestoreLine Success(string message) => new(true, message);
	public static RestoreLine Fail(string message) => new(false, message);
}

public sealed class WorldDatabaseException(string message, Exception? inner = null) : Exception(message, inner);
