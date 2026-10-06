using System.Xml.Linq;
using L2Config.Core.Characters;

namespace L2Config.Core.Shops;

/// <summary>How a page hands things over.</summary>
public enum ShopPageKind
{
	/// <summary>The ordinary merchant Buy window, one item at a time, each with its own price.</summary>
	Buy,

	/// <summary>The exchange window, which can hand over several items at once — a whole armour set for one price.</summary>
	Barter,
}

/// <summary>One thing a shop sells, at a price in adena.</summary>
public sealed record ShopItem(int ItemId, long Price, int Count = 1);

/// <summary>Several items sold together for one price. Only a barter page can do this.</summary>
public sealed record ShopBundle(string Name, long Price, IReadOnlyList<ShopItem> Items);

/// <summary>One page of the shop: a button on the shopkeeper's first screen, and the window it opens.</summary>
public sealed record ShopPage(
	string Title,
	ShopPageKind Kind,
	IReadOnlyList<ShopItem> Items,
	IReadOnlyList<ShopBundle> Bundles)
{
	public static ShopPage Buying(string title, IEnumerable<ShopItem> items) =>
		new(title, ShopPageKind.Buy, items.ToList(), []);

	public static ShopPage Bartering(string title, IEnumerable<ShopBundle> bundles) =>
		new(title, ShopPageKind.Barter, [], bundles.ToList());

	public int Count => Kind == ShopPageKind.Buy ? Items.Count : Bundles.Count;

	public bool IsEmpty => Count == 0;
}

/// <summary>Where one shopkeeper stands: beside a gatekeeper, nudged by however much the player chose.</summary>
public sealed record ShopPlacement(Gatekeeper Beside, bool Enabled, int OffsetX = 0, int OffsetY = 0)
{
	/// <summary>How far to the gatekeeper's side a shopkeeper is put when nothing has been nudged.</summary>
	public const int SideStep = 80;

	public int X => Beside.X + OffsetX + (int)Math.Round(-Math.Sin(Angle) * SideStep);

	public int Y => Beside.Y + OffsetY + (int)Math.Round(Math.Cos(Angle) * SideStep);

	public int Z => Beside.Z;

	/// <summary>Facing the way the gatekeeper faces, so the two stand together rather than nose to nose.</summary>
	public int Heading => Beside.Heading;

	private double Angle => Beside.Heading / 65536.0 * 2 * Math.PI;
}

/// <summary>A whole shop: who the shopkeeper is, what the shop sells, and which towns it stands in.</summary>
public sealed record ShopDefinition(
	int NpcId,
	int DisplayId,
	string Name,
	string Title,
	bool Enabled,
	IReadOnlyList<ShopPage> Pages,
	IReadOnlyList<ShopPlacement> Placements)
{
	/// <summary>Free in every L2Everdream release so far, and clear of the 80000–89999 range the server keeps for sims.</summary>
	public const int DefaultNpcId = 60000;

	/// <summary>Lector, a merchant who already stands in Giran: a shopkeeper the client can draw without any change to it.</summary>
	public const int DefaultDisplayId = 30001;

	public IEnumerable<ShopPage> UsedPages => Pages.Where(p => !p.IsEmpty);

	public IEnumerable<ShopPlacement> Towns => Placements.Where(p => p.Enabled);
}

/// <summary>The shop the app offers before anyone changes anything: the world's own items, at the world's own prices.</summary>
public static class ShopDefaults
{
	private static readonly string[] JewelParts = ["rear;lear", "rfinger;lfinger", "neck"];

	private static readonly string[] Grades = ["D", "C", "B", "A", "S"];

	/// <summary>
	/// The epic boss jewels. L2Everdream pins these at 1 in `DropAmountMultiplierByItemId` so a fast world cannot hand
	/// them out in stacks, so a shop does not offer them either unless the player puts them back in.
	/// </summary>
	public static IReadOnlySet<int> EpicJewels { get; } = new HashSet<int> { 6656, 6657, 6658, 6659, 6660, 6661, 6662, 8191 };

	/// <summary>Shots, potions and a Scroll of Escape — the things people actually run out of.</summary>
	private static readonly int[] Supplies =
	[
		1835, 1463, 1464, 1465, 1466, 1467,
		2509, 2510, 2511, 2512, 2513, 2514,
		3947, 3948, 3949, 3950, 3951, 3952,
		1060, 1061, 1539, 728, 1374, 736,
	];

	public static ShopDefinition Build(ItemCatalog items, IReadOnlyList<ArmorSet> sets, IEnumerable<Gatekeeper> gatekeepers)
	{
		var pages = new List<ShopPage>();
		foreach (var grade in Grades)
		{
			pages.Add(ShopPage.Buying($"Weapons {grade}", Sellable(items, "Weapon", grade, jewel: false)));
		}
		foreach (var grade in Grades)
		{
			pages.Add(ShopPage.Buying($"Armour {grade}", Sellable(items, "Armor", grade, jewel: false)));
		}
		foreach (var grade in Grades)
		{
			pages.Add(ShopPage.Buying($"Jewellery {grade}", Sellable(items, "Armor", grade, jewel: true)));
		}
		pages.Add(ShopPage.Buying("Supplies", Supplies
			.Select(items.Find)
			.OfType<ItemTemplate>()
			.Where(i => i.Price > 0)
			.Select(i => new ShopItem(i.Id, i.Price))));
		pages.Add(ShopPage.Bartering("Armour sets", Bundles(items, sets)));

		return new ShopDefinition(
			ShopDefinition.DefaultNpcId, ShopDefinition.DefaultDisplayId,
			"Shop Keeper", "General Shop", Enabled: true,
			pages,
			gatekeepers.Select(g => new ShopPlacement(g, g.IsMainTown)).ToList());
	}

	private static IEnumerable<ShopItem> Sellable(ItemCatalog items, string type, string grade, bool jewel) => items.All
		.Where(i => i.Type == type
			&& string.Equals(i.Grade, grade, StringComparison.OrdinalIgnoreCase)
			&& !i.Quest
			&& i.Price > 0
			&& HasAName(i.Name)
			&& !EpicJewels.Contains(i.Id)
			&& JewelParts.Contains(i.Kind) == jewel)
		.OrderBy(i => i.Price)
		.Select(i => new ShopItem(i.Id, i.Price));

	/// <summary>
	/// The datapack carries a few placeholders named "0" or left blank. They are real items the server would happily
	/// sell, but nobody wants them in a shop.
	/// </summary>
	private static bool HasAName(string name) => name.Any(char.IsLetter);

	/// <summary>Each armour set as one purchase, priced at what its pieces cost one by one.</summary>
	private static IEnumerable<ShopBundle> Bundles(ItemCatalog items, IReadOnlyList<ArmorSet> sets)
	{
		foreach (var set in sets)
		{
			var pieces = set.Pieces.Select(items.Find).OfType<ItemTemplate>().Where(i => i.Price > 0).ToList();
			if (pieces.Count < 2 || items.Find(set.Pieces[0]) is not { } chest)
			{
				continue;
			}
			yield return new ShopBundle($"{chest.Name} set", pieces.Sum(p => p.Price), pieces.Select(p => new ShopItem(p.Id, 0)).ToList());
		}
	}
}

/// <summary>An armour set from `game\data\stats\armorsets`: the pieces that have to be worn together.</summary>
public sealed record ArmorSet(int Id, IReadOnlyList<int> Pieces);

public static class ArmorSets
{
	private static readonly string[] Parts = ["chest", "legs", "head", "gloves", "feet", "shield"];

	public static IReadOnlyList<ArmorSet> ReadFromServer(string serverRoot) =>
		Read(Path.Combine(serverRoot, "game", "data", "stats", "armorsets"));

	public static IReadOnlyList<ArmorSet> Read(string folder)
	{
		var sets = new List<ArmorSet>();
		if (!Directory.Exists(folder))
		{
			return sets;
		}
		foreach (var file in Directory.EnumerateFiles(folder, "*.xml", SearchOption.AllDirectories).Order())
		{
			XDocument document;
			try
			{
				document = XDocument.Load(file);
			}
			catch (System.Xml.XmlException)
			{
				continue;
			}
			foreach (var set in document.Descendants("set"))
			{
				var pieces = Parts
					.Select(part => set.Element(part)?.Attribute("id")?.Value)
					.Select(value => int.TryParse(value, out var id) ? id : 0)
					.Where(id => id > 0)
					.ToList();
				if (pieces.Count > 0 && int.TryParse(set.Attribute("id")?.Value, out var setId))
				{
					sets.Add(new ArmorSet(setId, pieces));
				}
			}
		}
		return sets;
	}
}
