using System.Globalization;
using System.Xml.Linq;

namespace L2Config.Core.Rates;

/// <summary>One item inside a drop group. <see cref="Weight"/> is its share of the group (the group's own chance decides whether it drops at all).</summary>
public sealed record DropItem(int ItemId, long Min, long Max, double Weight);

/// <summary>A drop group: it fires with <see cref="Chance"/> per cent, and then one item inside it is chosen by weight.</summary>
public sealed record DropGroup(double Chance, IReadOnlyList<DropItem> Items);

/// <summary>A monster (or raid boss) from the datapack, with what it gives when it dies.</summary>
public sealed record Monster(
	int Id,
	string Name,
	int Level,
	string Type,
	double Exp,
	double Sp,
	double Hp,
	IReadOnlyList<DropGroup> Drops,
	IReadOnlyList<DropGroup> Spoil)
{
	/// <summary>Raid and grand bosses use the raid drop multipliers instead of the normal ones.</summary>
	public bool IsRaid => Type.Contains("RaidBoss", StringComparison.OrdinalIgnoreCase) || Type.Contains("GrandBoss", StringComparison.OrdinalIgnoreCase);

	public bool HasDrops => Drops.Count > 0 || Spoil.Count > 0;
}

/// <summary>
/// Every monster in the datapack (game\data\stats\npcs), with its experience and drop lists, and which items are herbs
/// (herbs have their own multipliers). Read once per server folder; nothing here touches the running world.
/// </summary>
public sealed class MonsterCatalog
{
	private readonly Dictionary<int, Monster> _byId;

	private MonsterCatalog(Dictionary<int, Monster> monsters, IReadOnlySet<int> herbs, IReadOnlyDictionary<int, string> itemNames,
		IReadOnlyDictionary<int, string> itemIcons)
	{
		_byId = monsters;
		Herbs = herbs;
		ItemNames = itemNames;
		ItemIcons = itemIcons;
		All = monsters.Values.OrderBy(m => m.Level).ThenBy(m => m.Name, StringComparer.OrdinalIgnoreCase).ToList();
	}

	public IReadOnlyList<Monster> All { get; }

	/// <summary>Item ids with ex_immediate_effect: herbs, which the server multiplies with the herb rates.</summary>
	public IReadOnlySet<int> Herbs { get; }

	public IReadOnlyDictionary<int, string> ItemNames { get; }

	/// <summary>Item id → the icon the client draws for it, e.g. "icon.weapon_long_sword_i00". Not every item has one.</summary>
	public IReadOnlyDictionary<int, string> ItemIcons { get; }

	public int Count => _byId.Count;

	public Monster? Find(int id) => _byId.GetValueOrDefault(id);

	public string ItemName(int id) => ItemNames.GetValueOrDefault(id) ?? $"Item {id}";

	public string? IconName(int id) => ItemIcons.GetValueOrDefault(id);

	public static MonsterCatalog LoadFromServer(string serverRoot)
	{
		var data = Path.Combine(serverRoot, "game", "data");
		return Load(Path.Combine(data, "stats", "npcs"), Path.Combine(data, "stats", "items"));
	}

	public static MonsterCatalog Load(string npcFolder, string itemFolder)
	{
		var herbs = new HashSet<int>();
		var itemNames = new Dictionary<int, string>();
		var itemIcons = new Dictionary<int, string>();
		if (Directory.Exists(itemFolder))
		{
			foreach (var file in Directory.EnumerateFiles(itemFolder, "*.xml", SearchOption.AllDirectories))
			{
				foreach (var item in XDocument.Load(file).Descendants("item"))
				{
					if (!int.TryParse((string?)item.Attribute("id"), out var id))
					{
						continue;
					}
					itemNames[id] = (string?)item.Attribute("name") ?? $"Item {id}";
					if (item.Elements("set").FirstOrDefault(s => (string?)s.Attribute("name") == "icon") is { } icon
						&& (string?)icon.Attribute("val") is { Length: > 0 } iconName)
					{
						itemIcons[id] = iconName;
					}
					if (item.Elements("set").Any(s => (string?)s.Attribute("name") == "ex_immediate_effect"
						&& string.Equals((string?)s.Attribute("val"), "true", StringComparison.OrdinalIgnoreCase)))
					{
						herbs.Add(id);
					}
				}
			}
		}

		var monsters = new Dictionary<int, Monster>();
		if (Directory.Exists(npcFolder))
		{
			foreach (var file in Directory.EnumerateFiles(npcFolder, "*.xml", SearchOption.AllDirectories).Order(StringComparer.OrdinalIgnoreCase))
			{
				foreach (var npc in XDocument.Load(file).Descendants("npc"))
				{
					if (Read(npc) is { } monster)
					{
						monsters[monster.Id] = monster;
					}
				}
			}
		}
		return new MonsterCatalog(monsters, herbs, itemNames, itemIcons);
	}

	private static Monster? Read(XElement npc)
	{
		if (!int.TryParse((string?)npc.Attribute("id"), out var id))
		{
			return null;
		}
		var type = (string?)npc.Attribute("type") ?? "";
		var acquire = npc.Element("acquire");
		var vitals = npc.Descendants("vitals").FirstOrDefault();
		var lists = npc.Element("dropLists");
		return new Monster(
			id,
			(string?)npc.Attribute("name") is { Length: > 0 } name ? name : $"NPC {id}",
			Number(npc.Attribute("level")) is var level && level > 0 ? (int)level : 0,
			type,
			Number(acquire?.Attribute("exp")),
			Number(acquire?.Attribute("sp")),
			Number(vitals?.Attribute("hp")),
			Groups(lists?.Element("drop")),
			Groups(lists?.Element("spoil")));
	}

	/// <summary>Reads &lt;group&gt; entries; items written straight under the list are read as a group of their own.</summary>
	private static IReadOnlyList<DropGroup> Groups(XElement? list)
	{
		if (list is null)
		{
			return [];
		}
		var groups = new List<DropGroup>();
		foreach (var element in list.Elements())
		{
			if (element.Name.LocalName == "group")
			{
				var items = element.Elements("item").Select(Item).Where(i => i is not null).Select(i => i!).ToList();
				if (items.Count > 0)
				{
					groups.Add(new DropGroup(Number(element.Attribute("chance")), items));
				}
			}
			else if (element.Name.LocalName == "item" && Item(element) is { } single)
			{
				groups.Add(new DropGroup(single.Weight, [single with { Weight = 100 }]));
			}
		}
		return groups;
	}

	private static DropItem? Item(XElement item) =>
		int.TryParse((string?)item.Attribute("id"), out var id)
			? new DropItem(id, (long)Number(item.Attribute("min"), 1), (long)Number(item.Attribute("max"), 1), Number(item.Attribute("chance")))
			: null;

	private static double Number(XAttribute? attribute, double fallback = 0) =>
		attribute is not null && double.TryParse(attribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : fallback;
}
