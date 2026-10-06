using L2Config.Core.Backups;
using L2Config.Core.Shops;

namespace L2Config.Core.Tests;

public class GatekeeperTests
{
	private static (string Npcs, string Spawns) SmallWorld()
	{
		var root = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		var npcs = Path.Combine(root, "npcs");
		var spawns = Path.Combine(root, "spawns", "Giran");
		Directory.CreateDirectory(npcs);
		Directory.CreateDirectory(spawns);
		File.WriteAllText(Path.Combine(npcs, "30000-30099.xml"), """
			<list>
				<npc id="30080" level="70" type="Teleporter" name="Clarissa" />
				<npc id="30001" level="70" type="Merchant" name="Lector" />
				<npc id="31211" level="70" type="Teleporter" name="Race Track Guide" />
			</list>
			""");
		File.WriteAllText(Path.Combine(spawns, "GiranNPCs.xml"), """
			<list enabled="true">
				<spawn name="GiranNPCs">
					<npc id="30080" x="83396" y="147904" z="-3404" heading="16384" respawnDelay="60" />
					<npc id="30001" x="83000" y="147000" z="-3400" heading="0" respawnDelay="60" />
					<npc id="31211" x="12661" y="181687" z="-3560" heading="32768" respawnDelay="60" />
				</spawn>
			</list>
			""");
		return (npcs, Path.Combine(root, "spawns"));
	}

	[Fact]
	public void GatekeepersComeFromTheWorldsOwnSpawnFiles()
	{
		var (npcs, spawns) = SmallWorld();
		var found = Gatekeepers.Read(npcs, spawns);

		var clarissa = Assert.Single(found, g => g.NpcId == 30080);
		Assert.Equal(("Giran", "Clarissa", 83396, 147904), (clarissa.Town, clarissa.Name, clarissa.X, clarissa.Y));
		Assert.Equal(-3404, clarissa.Z);
		Assert.True(clarissa.IsMainTown);

		// Merchants are not gatekeepers, and a gatekeeper outside a town is found but not ticked by default.
		Assert.DoesNotContain(found, g => g.NpcId == 30001);
		Assert.False(Assert.Single(found, g => g.NpcId == 31211).IsMainTown);
	}

	[LocalInstallFact]
	public void EveryTownInTheRealWorldHasItsGatekeeper()
	{
		var found = Gatekeepers.ReadFromServer(TestPaths.RealLocations.ServerRoot!);
		var towns = found.Where(g => g.IsMainTown).ToList();

		Assert.Equal(Gatekeepers.Towns.Count, towns.Count);
		Assert.All(towns, g => Assert.NotEqual(0, g.X));
		var giran = Assert.Single(towns, g => g.Town == "Giran");
		Assert.Equal((30080, "Clarissa", 83396, 147904, -3404), (giran.NpcId, giran.Name, giran.X, giran.Y, giran.Z));
	}
}

public class ShopFileTests
{
	private static ShopDefinition Shop(bool enabled = true, int pages = 2) => new(
		60000, 30001, "Shop Keeper", "General Shop", enabled,
		Enumerable.Range(1, pages)
			.Select(i => ShopPage.Buying($"Weapons {i}", [new ShopItem(1000 + i, 500 * i)]))
			.Append(ShopPage.Bartering("Armour sets", [new ShopBundle("Doom set", 8_000_000, [new ShopItem(2381, 0), new ShopItem(2417, 0)])]))
			.ToList(),
		[
			new ShopPlacement(new Gatekeeper("Giran", 30080, "Clarissa", 83396, 147904, -3404, 16384), Enabled: true),
			new ShopPlacement(new Gatekeeper("Aden", 30848, "Elisa", 146737, 25807, -2013, 0), Enabled: false),
		]);

	[Fact]
	public void AShopIsWrittenAsFilesTheServerAlreadyLooksIn()
	{
		var files = ShopFiles.Build(Shop()).ToDictionary(f => f.RelativePath, f => f.Content);

		Assert.Contains("stats/npcs/custom/L2EverdreamConfig/gm-shop.xml", files.Keys);
		Assert.Contains("html/merchant/60000.htm", files.Keys);
		Assert.Contains("buylists/custom/6000001.xml", files.Keys);
		Assert.Contains("multisell/custom/4000001.xml", files.Keys);
		Assert.All(files.Keys, path => Assert.DoesNotContain("..", path));

		// The shopkeeper has to be a Merchant or the server refuses the Buy and Sell bypasses.
		Assert.Contains("type=\"Merchant\"", files["stats/npcs/custom/L2EverdreamConfig/gm-shop.xml"]);
		// Each window is bound to the shopkeeper, or the server refuses to open it.
		Assert.Contains("<npcs><npc>60000</npc></npcs>", files["buylists/custom/6000001.xml"]);
		Assert.Contains("<npcs><npc>60000</npc></npcs>", files["multisell/custom/4000001.xml"]);
	}

	[Fact]
	public void OnlyTheTickedTownsGetAShopkeeperAndTheFileCarriesTheOnOffSwitch()
	{
		var on = ShopFiles.Build(Shop()).Single(f => f.RelativePath.StartsWith("spawns", StringComparison.Ordinal)).Content;
		Assert.Contains("enabled=\"true\"", on);
		Assert.Single(on.Split("<npc id=\"60000\"").Skip(1));            // Giran only; Aden is unticked
		Assert.Contains("Giran — Clarissa", on);

		var off = ShopFiles.Build(Shop(enabled: false)).Single(f => f.RelativePath.StartsWith("spawns", StringComparison.Ordinal)).Content;
		Assert.Contains("enabled=\"false\"", off);
	}

	[Fact]
	public void TheShopkeeperStandsBesideTheGatekeeperNotOnTopOfIt()
	{
		var place = Shop().Placements[0];
		Assert.NotEqual((place.Beside.X, place.Beside.Y), (place.X, place.Y));
		var distance = Math.Sqrt(Math.Pow(place.X - place.Beside.X, 2) + Math.Pow(place.Y - place.Beside.Y, 2));
		Assert.Equal(ShopPlacement.SideStep, distance, 0);
		Assert.Equal(place.Beside.Z, place.Z);
		Assert.Equal(place.Beside.Heading, place.Heading);
	}

	[Fact]
	public void EveryPageHasAButtonThatOpensItsOwnWindow()
	{
		var shop = Shop();
		var html = ShopFiles.Build(shop).Single(f => f.RelativePath.EndsWith(".htm", StringComparison.Ordinal)).Content;

		foreach (var (page, listId) in ShopFiles.PageIds(shop))
		{
			var bypass = page.Kind == ShopPageKind.Buy ? $"_Buy {listId}" : $"_multisell {listId}";
			Assert.Contains(bypass, html);
			Assert.Contains(page.Title, html);
		}
		Assert.Contains("npc_%objectId%_Sell", html);
	}

	[Fact]
	public void InstallingThenRemovingLeavesTheWorldAsItWas()
	{
		var root = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		var data = ShopInstaller.DataFolder(root);
		Directory.CreateDirectory(Path.Combine(data, "buylists", "custom"));
		var theirs = Path.Combine(data, "buylists", "custom", "999.xml");
		File.WriteAllText(theirs, "someone else's list");
		var backups = Path.Combine(root, "backups");

		Assert.Equal(ShopState.NotInstalled, ShopInstaller.Look(root, Shop()).State);

		ShopInstaller.Install(root, Shop(), new BackupSession(backups, DateTime.Now, BackupKind.Settings, "shop on"));
		Assert.Equal(ShopState.Installed, ShopInstaller.Look(root, Shop()).State);
		Assert.True(ShopInstaller.Look(root, Shop()).SpawnsEnabled);

		// A shop with fewer pages takes its own leftovers with it, and never anyone else's file.
		ShopInstaller.Install(root, Shop(pages: 1), new BackupSession(backups, DateTime.Now, BackupKind.Settings, "fewer pages"));
		Assert.False(File.Exists(Path.Combine(data, "buylists", "custom", "6000002.xml")));
		Assert.Equal(ShopState.Installed, ShopInstaller.Look(root, Shop(pages: 1)).State);

		ShopInstaller.Remove(root, Shop(pages: 1), new BackupSession(backups, DateTime.Now, BackupKind.Settings, "shop off"));
		Assert.Equal(ShopState.NotInstalled, ShopInstaller.Look(root, Shop(pages: 1)).State);
		Assert.Equal("someone else's list", File.ReadAllText(theirs));
		Assert.False(Directory.Exists(Path.Combine(data, "spawns", ShopFiles.Folder)));
	}

	[Fact]
	public void TheSettingsTheShopDependsOnAreReported()
	{
		var root = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		var status = ShopInstaller.Look(root, Shop(), (_, key) => key == "CustomNpcData" ? "True" : "False");

		Assert.Equal(2, status.Problems.Count);
		Assert.Contains(status.Problems, p => p.Contains("CustomBuyListLoad", StringComparison.Ordinal));
		Assert.False(status.Works);
	}
}
