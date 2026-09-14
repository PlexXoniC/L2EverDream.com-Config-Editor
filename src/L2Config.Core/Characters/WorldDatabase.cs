using System.Text.RegularExpressions;
using L2Config.Core.Ini;
using L2Config.Core.Storage;
using MySqlConnector;

namespace L2Config.Core.Characters;

/// <summary>
/// Reads player characters and edits their inventory adena in the running world's database.
/// <para>
/// The game server keeps an online character's inventory in memory and writes it back over the database, so edits are
/// only made while the character is offline — and that check is part of the same statement that writes, so a character
/// logging in at the wrong moment simply makes the edit fail. Simulated players (account <c>$sim</c>) are never listed:
/// the sim layer holds them in memory. New item rows are never created, because item object IDs are allocated by the
/// running server and an outside insert could collide with one it has already handed out.
/// </para>
/// </summary>
public sealed partial class WorldDatabase(string connectionString)
{
	public const int AdenaItemId = 57;
	public const string SimAccount = "$sim";

	/// <summary>Builds the connection from the world's game\config\Database.ini (the file the launcher points at the world's database).</summary>
	public static WorldDatabase FromServerFolder(L2Locations locations)
	{
		var path = Path.Combine(locations.GameConfigDir, "Database.ini");
		if (!File.Exists(path))
		{
			throw new WorldDatabaseException($"Not found: game\\config\\Database.ini in the server folder.");
		}
		var ini = IniDocument.Parse(File.ReadAllText(path));
		return new WorldDatabase(BuildConnectionString(ini.Get(null, "URL") ?? "", ini.Get(null, "Login") ?? "root", ini.Get(null, "Password") ?? ""));
	}

	/// <summary>Turns Mobius's JDBC URL (jdbc:mysql://host[:port]/database?...) into a MySqlConnector connection string.</summary>
	public static string BuildConnectionString(string jdbcUrl, string user, string password)
	{
		var match = JdbcUrl().Match(jdbcUrl.Trim());
		if (!match.Success)
		{
			throw new WorldDatabaseException("Database.ini does not contain a database URL this app understands.");
		}
		var builder = new MySqlConnectionStringBuilder
		{
			Server = match.Groups["host"].Value,
			Port = match.Groups["port"].Success ? uint.Parse(match.Groups["port"].Value) : 3306,
			Database = match.Groups["db"].Value,
			UserID = user,
			Password = password,
			ConnectionTimeout = 3,
			DefaultCommandTimeout = 15,
			Pooling = false,
		};
		return builder.ConnectionString;
	}

	public async Task<IReadOnlyList<PlayerCharacter>> ListPlayerCharactersAsync(CancellationToken cancellation = default)
	{
		await using var connection = await OpenAsync(cancellation);
		await using var command = new MySqlCommand(
			"""
			SELECT c.charId, c.char_name, c.account_name, c.level, c.online,
			       (SELECT i.object_id FROM items i
			         WHERE i.owner_id = c.charId AND i.item_id = @adena AND i.loc = 'INVENTORY'
			         ORDER BY i.count DESC LIMIT 1) AS adena_object,
			       (SELECT i.count FROM items i
			         WHERE i.owner_id = c.charId AND i.item_id = @adena AND i.loc = 'INVENTORY'
			         ORDER BY i.count DESC LIMIT 1) AS adena
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
				CharId: Convert.ToInt64(reader["charId"]),
				Name: reader.GetString("char_name"),
				Account: reader.GetString("account_name"),
				Level: reader.IsDBNull(reader.GetOrdinal("level")) ? 0 : Convert.ToInt32(reader["level"]),
				OnlineState: reader.IsDBNull(reader.GetOrdinal("online")) ? 0 : Convert.ToInt32(reader["online"]),
				AdenaObjectId: reader.IsDBNull(reader.GetOrdinal("adena_object")) ? null : Convert.ToInt64(reader["adena_object"]),
				Adena: reader.IsDBNull(reader.GetOrdinal("adena")) ? null : Convert.ToInt64(reader["adena"])));
		}
		return result;
	}

	/// <summary>
	/// Sets the character's inventory adena to <paramref name="newCount"/> (0 removes the adena). Succeeds only if the
	/// character is offline and the amount is still <paramref name="expectedCount"/>, all checked by the writing statement.
	/// </summary>
	public async Task<AdenaEditResult> SetAdenaAsync(PlayerCharacter character, long expectedCount, long newCount, CancellationToken cancellation = default)
	{
		if (character.AdenaObjectId is not { } objectId)
		{
			return AdenaEditResult.NoAdenaRow;
		}
		await using var connection = await OpenAsync(cancellation);
		var sql = newCount == 0
			? """
			  DELETE i FROM items i JOIN characters c ON c.charId = i.owner_id
			  WHERE i.object_id = @object AND i.owner_id = @char AND i.item_id = @adena AND i.loc = 'INVENTORY'
			    AND i.count = @expected AND c.online = 0 AND c.account_name <> @sim
			  """
			: """
			  UPDATE items i JOIN characters c ON c.charId = i.owner_id SET i.count = @count
			  WHERE i.object_id = @object AND i.owner_id = @char AND i.item_id = @adena AND i.loc = 'INVENTORY'
			    AND i.count = @expected AND c.online = 0 AND c.account_name <> @sim
			  """;
		await using var command = new MySqlCommand(sql, connection);
		command.Parameters.AddWithValue("@count", newCount);
		command.Parameters.AddWithValue("@object", objectId);
		command.Parameters.AddWithValue("@char", character.CharId);
		command.Parameters.AddWithValue("@adena", AdenaItemId);
		command.Parameters.AddWithValue("@expected", expectedCount);
		command.Parameters.AddWithValue("@sim", SimAccount);
		if (await command.ExecuteNonQueryAsync(cancellation) == 1)
		{
			return AdenaEditResult.Saved;
		}

		// Nothing changed: work out why, so the user gets a real reason.
		await using var check = new MySqlCommand(
			"""
			SELECT c.online, (SELECT i.count FROM items i WHERE i.object_id = @object AND i.owner_id = @char) AS count
			FROM characters c WHERE c.charId = @char
			""", connection);
		check.Parameters.AddWithValue("@object", objectId);
		check.Parameters.AddWithValue("@char", character.CharId);
		await using var reader = await check.ExecuteReaderAsync(cancellation);
		if (!await reader.ReadAsync(cancellation))
		{
			return AdenaEditResult.CharacterGone;
		}
		if (!reader.IsDBNull(0) && Convert.ToInt32(reader[0]) != 0)
		{
			return AdenaEditResult.CharacterOnline;
		}
		return reader.IsDBNull(1) ? AdenaEditResult.NoAdenaRow : AdenaEditResult.AmountChanged;
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

	[GeneratedRegex(@"^jdbc:(?:mysql|mariadb)://(?<host>[^/:?]+)(?::(?<port>\d+))?/(?<db>[^/?]+)", RegexOptions.IgnoreCase)]
	private static partial Regex JdbcUrl();
}

public sealed record PlayerCharacter(long CharId, string Name, string Account, int Level, int OnlineState, long? AdenaObjectId, long? Adena)
{
	/// <summary>0 offline, 1 online, 2 online without a client (offline shop).</summary>
	public bool IsOnline => OnlineState != 0;
}

public enum AdenaEditResult
{
	Saved,
	CharacterOnline,
	AmountChanged,
	NoAdenaRow,
	CharacterGone,
}

public sealed class WorldDatabaseException(string message, Exception? inner = null) : Exception(message, inner);
