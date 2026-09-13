using L2Config.Core.Catalog;
using L2Config.Core.Ini;
using L2Config.Core.Storage;

namespace L2Config.Core.Tests;

public class SettingsStoreTests
{
	private static readonly SettingsCatalog Catalog = CatalogLoader.Load(TestPaths.CatalogPath);

	private static SettingDefinition Setting(string target, string key) =>
		Catalog.Settings.Single(s => s.Target == target && s.Key == key && (s.File != "Database.ini"));

	[Fact]
	public void CatalogIsConsistent()
	{
		var groups = Catalog.Categories.SelectMany(c => c.Groups).Select(g => g.Id).ToHashSet();
		Assert.All(Catalog.Settings, s => Assert.Contains(s.Group, groups));
		Assert.Equal(Catalog.Settings.Count, Catalog.Settings.Select(s => s.Id).Distinct().Count());
		Assert.All(Catalog.Settings.Where(s => s.Editor == SettingEditor.Choice), s => Assert.NotEmpty(s.Options!));
	}

	[LocalInstallFact]
	public void EveryCatalogedSettingIsFoundInTheLocalFiles()
	{
		var store = new SettingsStore(Catalog, TestPaths.RealLocations);
		store.Load();

		var missing = Catalog.Settings
			.Where(s => s.Target is SettingTargets.ServerGame or SettingTargets.ServerLogin or SettingTargets.WorldProfile)
			.Where(s => store.GetValue(s) is null)
			.Select(s => s.Id)
			.ToList();
		Assert.Empty(missing);
	}

	/// <summary>The limits must never reject what the shipped release and this PC's files already contain.</summary>
	[LocalInstallFact]
	public void EveryCurrentValueIsWithinItsLimits()
	{
		var store = new SettingsStore(Catalog, TestPaths.RealLocations);
		store.Load();

		var rejected = Catalog.Settings
			.Where(s => !s.IsManaged)
			.Select(s => (Setting: s, Value: store.GetValue(s)))
			.Where(x => x.Value is not null && SettingValues.Validate(x.Setting, x.Value) is not null)
			.Select(x => $"{x.Setting.Id} = '{x.Value}': {SettingValues.Validate(x.Setting, x.Value!)}")
			.ToList();
		Assert.Empty(rejected);
	}

	[Theory]
	[InlineData("int-list", "57,4037", true)]
	[InlineData("int-list", "57, abc", false)]
	[InlineData("pair-list", "57,1;6656,1.5;", true)]
	[InlineData("pair-list", "57;1", false)]
	[InlineData("boss-drop-list", "4356,1,2,100;", true)]
	[InlineData("time-list", "08:00, 23:59", true)]
	[InlineData("time-list", "24:00", false)]
	[InlineData("weekday-list", "1,7", true)]
	[InlineData("weekday-list", "0", false)]
	[InlineData("hex-color", "00FF00", true)]
	[InlineData("hex-color", "green", false)]
	[InlineData("percent-split-4", "55,35,7,3", true)]
	[InlineData("percent-split-4", "50,35,7,3", false)]
	[InlineData("regex", "[A-Z][a-z]*", true)]
	[InlineData("regex", "[A-Z", false)]
	[InlineData("coordinates", "-84318,244579,-3730", true)]
	public void FormatsAcceptOnlyWhatTheServerReads(string format, string value, bool valid)
	{
		Assert.Equal(valid, SettingValues.ValidateFormat(format, value) is null);
	}

	[Fact]
	public void NumbersAreHeldToTheirRangeAndType()
	{
		var charSlots = Catalog.Settings.Single(s => s.Key == "CharMaxNumber");
		Assert.Null(SettingValues.Validate(charSlots, "7"));
		Assert.NotNull(SettingValues.Validate(charSlots, "8"));
		Assert.NotNull(SettingValues.Validate(charSlots, "3.5"));

		var chance = Catalog.Settings.Single(s => s.Key == "DeathPenaltyChance");
		Assert.NotNull(SettingValues.Validate(chance, "101"));
		Assert.NotNull(SettingValues.Validate(chance, "-1"));

		var peace = Catalog.Settings.Single(s => s.Key == "PeaceZoneMode");
		Assert.Null(SettingValues.Validate(peace, "2"));
		Assert.NotNull(SettingValues.Validate(peace, "3"));
	}

	[LocalInstallFact]
	public void SavingWritesInstallAndPlayerCopyAndBacksUpFirst()
	{
		var locations = TestPaths.CopyRealFilesToTemp();
		var rates = Path.Combine(locations.GameConfigDir, "Rates.ini");
		var playerRates = Path.Combine(locations.PlayerGameConfigDir, "Rates.ini");
		var before = File.ReadAllText(rates);

		var store = new SettingsStore(Catalog, locations);
		store.Load();
		var rateXp = Setting(SettingTargets.ServerGame, "RateXp");
		var backups = Path.Combine(locations.DataRoot, "backups");
		var result = store.Save([(rateXp, "7")], backups);

		var after = File.ReadAllText(rates);
		Assert.Equal("7", IniDocument.Parse(after).Get(null, "RateXp"));
		Assert.Equal("7", IniDocument.Parse(File.ReadAllText(playerRates)).Get(null, "RateXp"));
		Assert.Equal(before.Split('\n').Length, after.Split('\n').Length);
		Assert.Single(DiffLines(before, after));
		Assert.NotNull(result.BackupFolder);
		Assert.Equal(2, Directory.EnumerateFiles(result.BackupFolder!, "Rates.ini", SearchOption.AllDirectories).Count());
	}

	[LocalInstallFact]
	public void SavingClientSettingsKeepsTheEncryptedFileReadable()
	{
		var locations = TestPaths.CopyRealFilesToTemp();
		var store = new SettingsStore(Catalog, locations);
		store.Load();

		var windowed = Setting(SettingTargets.ClientL2Ini, "WindowedViewportX");
		var music = Setting(SettingTargets.ClientOption, "MusicVolume");
		store.Save([(windowed, "1280"), (music, "0.250000")], Path.Combine(locations.DataRoot, "backups"));

		var reloaded = new SettingsStore(Catalog, locations);
		reloaded.Load();
		Assert.Equal("1280", reloaded.GetValue(windowed));
		Assert.Equal("0.250000", reloaded.GetValue(music));
		Assert.Equal(L2IniFormat.Ver413, L2IniCodec.DetectFormat(File.ReadAllBytes(Path.Combine(locations.ClientSystemDir!, "l2.ini"))));
	}

	[LocalInstallFact]
	public void ManagedAndInvalidValuesAreRefusedBeforeAnythingIsWritten()
	{
		var locations = TestPaths.CopyRealFilesToTemp();
		var store = new SettingsStore(Catalog, locations);
		store.Load();
		var rates = Path.Combine(locations.GameConfigDir, "Rates.ini");
		var before = File.ReadAllText(rates);

		var loginPort = Catalog.Settings.First(s => s.Key == "LoginPort" && s.IsManaged);
		Assert.Throws<InvalidOperationException>(() => store.Save([(Setting(SettingTargets.ServerGame, "RateXp"), "2"), (loginPort, "1")], Path.GetTempPath()));
		Assert.Throws<ArgumentException>(() => store.Save([(Setting(SettingTargets.ServerGame, "RateXp"), "fast")], Path.GetTempPath()));
		Assert.Equal(before, File.ReadAllText(rates));
	}

	[LocalInstallFact]
	public void WorldProfileKeepsTypesAndUnknownFields()
	{
		var locations = TestPaths.CopyRealFilesToTemp();
		var store = new SettingsStore(Catalog, locations);
		store.Load();
		store.Save([(Setting(SettingTargets.WorldProfile, "simsPercent"), "50"), (Setting(SettingTargets.WorldProfile, "freeClassChange"), "false")], Path.Combine(locations.DataRoot, "b"));

		var json = File.ReadAllText(locations.WorldProfilePath);
		Assert.Contains("\"simsPercent\": 50", json);
		Assert.Contains("\"freeClassChange\": false", json);
		Assert.Contains("\"chronicle\"", json);
	}

	private static IEnumerable<string> DiffLines(string a, string b) =>
		a.Split('\n').Zip(b.Split('\n')).Where(p => p.First != p.Second).Select(p => p.Second);
}

public class SettingSearchTests
{
	private static readonly SettingsCatalog Catalog = CatalogLoader.Load(TestPaths.CatalogPath);

	[Theory]
	[InlineData("party xp", "RatePartyXp")]
	[InlineData("RateXp", "RateXp")]
	[InlineData("auto loot", "AutoLoot")]
	[InlineData("music", "MusicVolume")]
	public void FindsSettingsByFriendlyOrRealName(string query, string expectedKey)
	{
		var tokens = SettingSearch.Tokenize(query);
		var groups = Catalog.Categories.SelectMany(c => c.Groups).ToDictionary(g => g.Id, g => g.Name);
		var hits = Catalog.Settings.Where(s => SettingSearch.Matches(s, groups[s.Group], tokens))
			.OrderByDescending(s => SettingSearch.Rank(s, tokens))
			.Select(s => s.Key)
			.ToList();
		Assert.Contains(expectedKey, hits.Take(5));
	}
}
