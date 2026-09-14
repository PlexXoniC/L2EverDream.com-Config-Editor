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
		Assert.Equal("localhost", cs.Server);
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
}
