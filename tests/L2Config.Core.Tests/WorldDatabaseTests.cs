using L2Config.Core.Characters;
using MySqlConnector;

namespace L2Config.Core.Tests;

public class WorldDatabaseTests
{
	[Fact]
	public void ReadsTheLaunchersJdbcUrl()
	{
		var cs = new MySqlConnectionStringBuilder(WorldDatabase.BuildConnectionString(
			"jdbc:mysql://localhost:33061/l2ed_world?useUnicode=true&characterEncoding=utf-8", "root", ""));
		Assert.Equal("127.0.0.1", cs.Server);
		Assert.Equal(33061u, cs.Port);
		Assert.Equal("l2ed_world", cs.Database);
		Assert.Equal("root", cs.UserID);
	}

	[Fact]
	public void StockUrlWithoutPortUsesTheMySqlDefault()
	{
		var cs = new MySqlConnectionStringBuilder(WorldDatabase.BuildConnectionString("jdbc:mysql://localhost/l2jmobiusinterlude", "root", ""));
		Assert.Equal(3306u, cs.Port);
	}

	[Fact]
	public void RejectsSomethingThatIsNotAJdbcUrl()
	{
		Assert.Throws<WorldDatabaseException>(() => WorldDatabase.BuildConnectionString("localhost", "root", ""));
	}

	/// <summary>Read-only: runs only while the local world (and so its database) is running.</summary>
	[LocalInstallFact]
	public async Task ListsPlayerCharactersButNeverSims()
	{
		IReadOnlyList<PlayerCharacter> characters;
		try
		{
			characters = await WorldDatabase.FromServerFolder(TestPaths.RealLocations).ListPlayerCharactersAsync();
		}
		catch (WorldDatabaseException)
		{
			return; // world not running
		}
		Assert.DoesNotContain(characters, c => c.Account == WorldDatabase.SimAccount);
		Assert.All(characters.Where(c => c.Adena is not null), c => Assert.NotNull(c.AdenaObjectId));
	}

	/// <summary>Read-only: every player character's inventory and pending deliveries can be read while the world runs.</summary>
	[LocalInstallFact]
	public async Task ReadsInventoriesAndDeliveries()
	{
		var database = WorldDatabase.FromServerFolder(TestPaths.RealLocations);
		IReadOnlyList<PlayerCharacter> characters;
		try
		{
			characters = await database.ListPlayerCharactersAsync();
		}
		catch (WorldDatabaseException)
		{
			return; // world not running
		}
		foreach (var character in characters)
		{
			var inventory = await database.ListInventoryAsync(character.CharId);
			Assert.Equal(character.SlotsUsed, inventory.Count);
			await database.ListDeliveriesAsync(character.CharId);
		}
		Assert.True((await database.WorldStartedAsync()) < DateTime.Now);
	}
}
