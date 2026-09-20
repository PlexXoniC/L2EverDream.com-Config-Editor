using L2Config.Core.Rates;

namespace L2Config.Core.Tests;

public class RatesTests
{
	private static MonsterCatalog SmallCatalog()
	{
		var root = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		var npcs = Path.Combine(root, "npcs");
		var items = Path.Combine(root, "items");
		Directory.CreateDirectory(npcs);
		Directory.CreateDirectory(items);
		File.WriteAllText(Path.Combine(npcs, "20000-20099.xml"), """
			<list>
				<npc id="20066" level="25" type="Monster" name="Ol Mahum Captain">
					<acquire exp="1052" sp="53" />
					<stats><vitals hp="460" mp="259" /></stats>
					<dropLists>
						<drop>
							<group chance="70"><item id="57" min="171" max="322" chance="100" /></group>
							<group chance="14.4938">
								<item id="1868" min="1" max="1" chance="76.6611" />
								<item id="1873" min="1" max="1" chance="23.3389" />
							</group>
							<group chance="42"><item id="8600" min="1" max="1" chance="100" /></group>
						</drop>
						<spoil><item id="1865" min="1" max="2" chance="37.1" /></spoil>
					</dropLists>
				</npc>
				<npc id="29001" level="79" type="RaidBoss" name="Queen Ant">
					<acquire exp="1000000" sp="100000" />
					<dropLists><drop><group chance="100"><item id="6660" min="1" max="1" chance="100" /></group></drop></dropLists>
				</npc>
			</list>
			""");
		File.WriteAllText(Path.Combine(items, "items.xml"), """
			<list>
				<item id="57" type="EtcItem" name="Adena"><set name="is_stackable" val="true" /></item>
				<item id="1868" type="EtcItem" name="Thread" />
				<item id="1873" type="EtcItem" name="Silver Nugget" />
				<item id="1865" type="EtcItem" name="Varnish" />
				<item id="8600" type="EtcItem" name="Herb of Life"><set name="ex_immediate_effect" val="true" /></item>
				<item id="6660" type="EtcItem" name="Ring of Queen Ant" />
			</list>
			""");
		return MonsterCatalog.Load(npcs, items);
	}

	[Fact]
	public void MonstersAndTheirDropsAreReadFromTheDatapack()
	{
		var catalog = SmallCatalog();
		var mob = catalog.Find(20066)!;

		Assert.Equal(("Ol Mahum Captain", 25, 1052d, false), (mob.Name, mob.Level, mob.Exp, mob.IsRaid));
		Assert.Equal(3, mob.Drops.Count);
		Assert.Equal(70, mob.Drops[0].Chance);
		Assert.Equal((57, 171L, 322L), (mob.Drops[0].Items[0].ItemId, mob.Drops[0].Items[0].Min, mob.Drops[0].Items[0].Max));
		Assert.Equal("Varnish", catalog.ItemName(mob.Spoil[0].Items[0].ItemId));
		Assert.Contains(8600, catalog.Herbs);
		Assert.True(catalog.Find(29001)!.IsRaid);
	}

	[Theory]
	[InlineData(5, 0.5)]
	[InlineData(15, 0)]
	[InlineData(20, 1)]
	[InlineData(3, 0.25)]
	public void ChanceAndAmountAlwaysMultiplyUpToTheRateAsked(double rate, double delivery)
	{
		var options = new RateOptions(rate, delivery);
		Assert.Equal(rate, options.ChanceMultiplier * options.AmountMultiplier, 2);
		Assert.InRange(options.ChanceMultiplier, 1, RateOptions.ChanceCap);
	}

	[Fact]
	public void DeliveryChoosesBetweenMoreOftenAndBiggerStacks()
	{
		Assert.Equal((2.0, 2.5), (new RateOptions(5, 0).ChanceMultiplier, new RateOptions(5, 0).AmountMultiplier));
		Assert.Equal((1.0, 5.0), (new RateOptions(5, 1).ChanceMultiplier, new RateOptions(5, 1).AmountMultiplier));
		Assert.Equal((1.0, 1.0), (new RateOptions(1, 0).ChanceMultiplier, new RateOptions(1, 0).AmountMultiplier));
	}

	[Fact]
	public void ThePlanWritesAdenaOnItsOwnBecauseAPerItemRateReplacesTheGeneralOne()
	{
		var plan = RatePlan.Build(new RateOptions(5, 1));
		var byId = Assert.Single(plan.Changes, c => c.Key == "DropAmountMultiplierByItemId");

		Assert.Equal("57,5", byId.NewValue);
		Assert.Equal("5", Assert.Single(plan.Changes, c => c.Key == "RateXp").NewValue);
		Assert.Equal("1", Assert.Single(plan.Changes, c => c.Key == "DeathDropChanceMultiplier").NewValue);
		Assert.Equal("5", Assert.Single(plan.Changes, c => c.Key == "DeathDropAmountMultiplier").NewValue);
		Assert.DoesNotContain(plan.Changes, c => c.Key.StartsWith("Herb", StringComparison.Ordinal));
		Assert.DoesNotContain(plan.Changes, c => c.Key == "RatePartyXp");
		Assert.Equal("2.33", Assert.Single(plan.Changes, c => c.Key == "RaidDropAmountMultiplier").NewValue); // raids move less
	}

	[Fact]
	public void ThePreviewShowsWhatAPlanReallyDoesToAMonster()
	{
		var catalog = SmallCatalog();
		var mob = catalog.Find(20066)!;

		// "More often" at 5x: chance x2 (70% -> 100%, wasted above), amount x2.5.
		var often = DropPreview.For(mob, catalog, new RateOptions(5, 0));
		var adena = Assert.Single(often.Drops, d => d.IsAdena);
		Assert.Equal(100, adena.ChanceAfter, 1);
		Assert.True(adena.ChanceIsFull);
		Assert.Equal(246.5 * 0.7, adena.PerKillBefore, 1);
		Assert.Equal(246.5 * 2.5, adena.PerKillAfter, 1); // 100% of a 2.5x stack
		Assert.Equal(3.57, adena.RealMultiplier, 2); // not 5x: the chance half was wasted

		// "Bigger stacks" at 5x: chance untouched, amount x5 — exactly 5x more adena.
		var stacks = DropPreview.For(mob, catalog, new RateOptions(5, 1));
		var adenaStacks = Assert.Single(stacks.Drops, d => d.IsAdena);
		Assert.Equal(70, adenaStacks.ChanceAfter, 1);
		Assert.Equal(5, adenaStacks.RealMultiplier, 2);
		Assert.Equal(5260, stacks.ExpAfter, 0);
	}

	[Fact]
	public void HerbsAreLeftAloneAndRaidsUseTheirOwnRates()
	{
		var catalog = SmallCatalog();
		var herb = Assert.Single(DropPreview.For(catalog.Find(20066)!, catalog, new RateOptions(20, 0.5)).Drops, d => d.IsHerb);
		Assert.True(herb.Unchanged);
		Assert.Equal(42, herb.ChanceAfter, 1);

		var raid = DropPreview.For(catalog.Find(29001)!, catalog, new RateOptions(20, 1));
		var ring = Assert.Single(raid.Drops);
		Assert.Equal(7.33, ring.RealMultiplier, 2); // 1 + (20-1)/3, not 20
		Assert.Equal(20_000_000, raid.ExpAfter, 0); // experience still follows the world rate
	}

	[Fact]
	public void TheItemLimitIsPointedOutWhenMoreGroupsAlwaysDropThanTheServerGives()
	{
		var catalog = SmallCatalog();
		var preview = DropPreview.For(catalog.Find(20066)!, catalog, new RateOptions(20, 0), maxDifferentItems: 2);
		Assert.Equal(1, preview.GroupsAtFullChance); // only the adena group reaches 100%
		Assert.False(preview.HitsItemLimit);

		var tight = DropPreview.For(catalog.Find(20066)!, catalog, new RateOptions(20, 0), maxDifferentItems: 0);
		Assert.True(tight.HitsItemLimit);
	}

	[LocalInstallFact]
	public void TheRealDatapackReadsAndPreviews()
	{
		var catalog = MonsterCatalog.LoadFromServer(TestPaths.RealLocations.ServerRoot!);
		Assert.InRange(catalog.Count, 1000, 10000);

		var captain = catalog.All.First(m => m.Name == "Ol Mahum Captain");
		Assert.Equal(25, captain.Level);
		Assert.Equal(1052, captain.Exp);

		var preview = DropPreview.For(captain, catalog, new RateOptions(5, 1));
		Assert.Equal(5, preview.ExpAfter / preview.ExpBefore, 2);
		Assert.Equal(5, preview.AdenaAfter / preview.AdenaBefore, 2);
		Assert.Contains(preview.Drops, d => d.IsHerb && d.Unchanged);
	}
}

public class IconTests
{
	[LocalInstallFact]
	public void IconsAreReadFromTheInstalledClient()
	{
		var icons = L2Config.Core.Client.IconLibrary.ForClient(TestPaths.RealLocations.ClientSystemDir);
		Assert.NotNull(icons);

		var sword = icons!.Find("icon.weapon_long_sword_i00");
		Assert.NotNull(sword);
		Assert.Equal((32, 32), (sword!.Width, sword.Height));
		Assert.Equal(32 * 32 * 4, sword.Bgra.Length);
		Assert.Contains(sword.Bgra, b => b != 0); // not a blank image

		Assert.Same(sword, icons.Find("icon.weapon_long_sword_i00")); // cached
		Assert.Null(icons.Find("icon.not_a_real_icon"));
		Assert.Null(icons.Find(null));
	}

	[Fact]
	public void AClientFolderWithoutTexturesSimplyHasNoIcons()
	{
		var empty = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"), "system");
		Directory.CreateDirectory(empty);
		Assert.Null(L2Config.Core.Client.IconLibrary.ForClient(empty));
		Assert.Null(L2Config.Core.Client.IconLibrary.ForClient(null));
	}
}
