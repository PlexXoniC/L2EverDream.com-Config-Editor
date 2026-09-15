using L2Config.Core.Catalog;
using L2Config.Core.Skills;
using L2Config.Core.Storage;

namespace L2Config.Core.Tests;

public class SkillDurationTests
{
	[Fact]
	public void TheListIsReadLikeTheServerReadsItAndWrittenSortedById()
	{
		var (durations, problems) = SkillDurationList.Parse(" 1062,3600;1040,1800; ;1040,2400;");
		Assert.Empty(problems);
		Assert.Equal([1040, 1062], durations.Keys);
		Assert.Equal(2400, durations[1040]); // the last entry for an id wins, as in the server's map
		Assert.Equal("1040,2400;1062,3600", SkillDurationList.Format(durations));
		Assert.Equal("", SkillDurationList.Format(new SortedDictionary<int, int>()));
	}

	[Theory]
	[InlineData("1040")]
	[InlineData("1040,abc")]
	[InlineData("1040,20,5")]
	[InlineData("0,60")]
	[InlineData("1040,0")]
	[InlineData("1040,43201")]
	public void BadEntriesAreRefusedWithAReason(string value)
	{
		Assert.NotEmpty(SkillDurationList.Parse(value).Problems);
		var setting = new SettingDefinition
		{
			Id = "x", Target = SettingTargets.ServerGame, File = "Player.ini", Key = "SkillDurationList", Category = "c", Group = "g", Name = "n",
			Editor = SettingEditor.SkillDurations, Format = "skill-duration-list",
		};
		Assert.NotNull(SettingValues.Validate(setting, value));
		Assert.Null(SettingValues.Validate(setting, "1040,43200;264,600"));
		Assert.Null(SettingValues.Validate(setting, ""));
	}

	[Theory]
	[InlineData("90", 90)]
	[InlineData("90s", 90)]
	[InlineData("20m", 1200)]
	[InlineData("20 min", 1200)]
	[InlineData("1h 30m", 5400)]
	[InlineData("2 hours", 7200)]
	[InlineData("20:00", 1200)]
	[InlineData("1:30:00", 5400)]
	[InlineData("twenty", null)]
	[InlineData("20 minutes please", null)]
	[InlineData("", null)]
	public void DurationsCanBeTypedInPlainWords(string text, int? seconds)
	{
		Assert.Equal(seconds, SkillDurationList.ParseDuration(text));
	}

	[Theory]
	[InlineData(45, "45 s")]
	[InlineData(1200, "20 min")]
	[InlineData(5400, "1 h 30 min")]
	[InlineData(43200, "12 h")]
	public void DurationsAreDescribedInPlainWords(int seconds, string text)
	{
		Assert.Equal(text, SkillDurationList.Describe(seconds));
	}

	[Fact]
	public void SkillsWithADurationAreReadWithWhoUsesThem()
	{
		var root = Path.Combine(Path.GetTempPath(), "l2config-tests", Guid.NewGuid().ToString("N"));
		var skills = Path.Combine(root, "skills");
		Directory.CreateDirectory(Path.Combine(skills, "custom"));
		File.WriteAllText(Path.Combine(skills, "01000-01099.xml"), """
			<list>
				<skill id="1040" levels="3" name="Shield">
					<table name="#ench1AbnormalTimes">1240 1280 2400</table>
					<operateType>A2</operateType>
					<abnormalTime>1200</abnormalTime>
					<abnormalType>PD_UP</abnormalType>
					<enchant1 name="abnormalTime">#ench1AbnormalTimes</enchant1>
				</skill>
				<skill id="264" levels="1" name="Song of Earth">
					<operateType>A2</operateType><abnormalTime>120</abnormalTime><abnormalType>SONG_OF_EARTH</abnormalType>
				</skill>
				<skill id="1069" levels="2" name="Sleep">
					<table name="#times">30 45</table>
					<operateType>A2</operateType><abnormalTime>#times</abnormalTime><isDebuff>1</isDebuff>
				</skill>
				<skill id="1001" levels="1" name="Toggle"><operateType>T</operateType><abnormalTime>60</abnormalTime></skill>
				<skill id="1002" levels="1" name="Instant"><operateType>A1</operateType></skill>
				<skill id="4001" levels="1" name="Monster Haste"><operateType>A2</operateType><abnormalTime>60</abnormalTime></skill>
			</list>
			""");
		File.WriteAllText(Path.Combine(skills, "custom", "custom.xml"), """
			<list><skill id="4001" levels="1" name="Monster Haste (custom)"><operateType>A2</operateType><abnormalTime>90</abnormalTime></skill></list>
			""");
		var trees = Path.Combine(root, "trees");
		Directory.CreateDirectory(trees);
		File.WriteAllText(Path.Combine(trees, "classSkillTree.xml"), """<list><skillTree><skill skillName="Shield" skillId="1040" skillLevel="1" /><skill skillName="Sleep" skillId="1069" skillLevel="1" /></skillTree></list>""");
		var buffer = Path.Combine(root, "SchemeBufferSkills.xml");
		File.WriteAllText(buffer, """<list><category type="Buffs"><buff id="264" level="1" price="0" /></category></list>""");

		var catalog = SkillCatalog.Load(skills, trees, buffer);

		Assert.Equal(4, catalog.Count);
		Assert.Null(catalog.Find(1001)); // toggles are never changed by the list
		Assert.Null(catalog.Find(1002)); // no duration
		var shield = catalog.Find(1040)!;
		Assert.Equal((SkillKind.Buff, 1200, 2400, true, false), (shield.Kind, shield.MinSeconds, shield.EnchantedMaxSeconds, shield.LearnedByPlayers, shield.FromBuffer));
		var song = catalog.Find(264)!;
		Assert.Equal((SkillKind.SongOrDance, true), (song.Kind, song.FromBuffer));
		var sleep = catalog.Find(1069)!;
		Assert.Equal((SkillKind.Debuff, 30, 45, true), (sleep.Kind, sleep.MinSeconds, sleep.MaxSeconds, sleep.VariesByLevel));
		var monster = catalog.Find(4001)!;
		Assert.Equal(("Monster Haste (custom)", 90, true, true), (monster.Name, monster.MinSeconds, monster.NpcOnly, monster.IsCustom));
	}

	[LocalInstallFact]
	public void TheRealDatapackHasTheSkillsTheEditorPromises()
	{
		var catalog = SkillCatalog.LoadFromServer(TestPaths.RealLocations.ServerRoot!);

		Assert.InRange(catalog.Count, 900, 1100);
		var shield = catalog.Find(1040)!;
		Assert.Equal(("Shield", SkillKind.Buff, 1200, true, true), (shield.Name, shield.Kind, shield.MinSeconds, shield.LearnedByPlayers, shield.FromBuffer));
		Assert.Equal(2400, shield.EnchantedMaxSeconds);
		Assert.Equal(SkillKind.SongOrDance, catalog.Find(264)!.Kind);
		Assert.All(catalog.All, s => Assert.True(s.MinSeconds > 0));
		Assert.Contains(catalog.All, s => s.NpcOnly && s.Kind == SkillKind.Debuff);
	}
}
