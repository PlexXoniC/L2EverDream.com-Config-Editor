using System.Text.Json;

namespace L2Config.Core.Shops;

/// <summary>What the player changed about one town's shopkeeper.</summary>
public sealed class TownChoice
{
	public bool Enabled { get; set; }
	public int OffsetX { get; set; }
	public int OffsetY { get; set; }
}

/// <summary>What the player changed about one page: prices of their own, and anything they took off it.</summary>
public sealed class PageChoice
{
	public Dictionary<int, long> Prices { get; set; } = [];
	public List<int> Removed { get; set; } = [];
}

/// <summary>
/// The player's own decisions about the shop — not the shop itself. Everything else (which items exist, what they are
/// worth, where each gatekeeper stands) is read from the world every time, so a world update moves the shopkeepers with
/// their gatekeepers and brings in new items instead of freezing whatever was true when the shop was first set up.
/// </summary>
public sealed class ShopChoices
{
	private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

	public int NpcId { get; set; } = ShopDefinition.DefaultNpcId;
	public int DisplayId { get; set; } = ShopDefinition.DefaultDisplayId;
	public string Name { get; set; } = "Shop Keeper";
	public string Title { get; set; } = "General Shop";
	public bool Enabled { get; set; } = true;

	/// <summary>Keyed by the gatekeeper's npc id, so it still matches after the world moves someone.</summary>
	public Dictionary<int, TownChoice> Towns { get; set; } = [];

	/// <summary>Keyed by the page's title.</summary>
	public Dictionary<string, PageChoice> Pages { get; set; } = [];

	public static string FileName => "gm-shop.json";

	public static ShopChoices Load(string folder)
	{
		var path = Path.Combine(folder, FileName);
		try
		{
			return File.Exists(path)
				? JsonSerializer.Deserialize<ShopChoices>(File.ReadAllText(path)) ?? new ShopChoices()
				: new ShopChoices();
		}
		catch (JsonException)
		{
			return new ShopChoices();
		}
	}

	public void Save(string folder)
	{
		Directory.CreateDirectory(folder);
		File.WriteAllText(Path.Combine(folder, FileName), JsonSerializer.Serialize(this, Json));
	}

	/// <summary>The shop these choices make of the world's own defaults.</summary>
	public ShopDefinition ApplyTo(ShopDefinition defaults) => defaults with
	{
		NpcId = NpcId,
		DisplayId = DisplayId,
		Name = string.IsNullOrWhiteSpace(Name) ? defaults.Name : Name.Trim(),
		Title = string.IsNullOrWhiteSpace(Title) ? defaults.Title : Title.Trim(),
		Enabled = Enabled,
		Pages = defaults.Pages.Select(Page).ToList(),
		Placements = defaults.Placements.Select(Town).ToList(),
	};

	private ShopPage Page(ShopPage page)
	{
		if (!Pages.TryGetValue(page.Title, out var choice))
		{
			return page;
		}
		var removed = choice.Removed.ToHashSet();
		return page with
		{
			Items = page.Items
				.Where(i => !removed.Contains(i.ItemId))
				.Select(i => choice.Prices.TryGetValue(i.ItemId, out var price) ? i with { Price = price } : i)
				.ToList(),
			Bundles = page.Bundles
				.Where(b => b.Items.Count > 0 && !removed.Contains(b.Items[0].ItemId))
				.Select(b => choice.Prices.TryGetValue(b.Items[0].ItemId, out var price) ? b with { Price = price } : b)
				.ToList(),
		};
	}

	private ShopPlacement Town(ShopPlacement placement) =>
		Towns.TryGetValue(placement.Beside.NpcId, out var choice)
			? placement with { Enabled = choice.Enabled, OffsetX = choice.OffsetX, OffsetY = choice.OffsetY }
			: placement;

	/// <summary>Remembers one town's tick and nudge.</summary>
	public void Remember(ShopPlacement placement) =>
		Towns[placement.Beside.NpcId] = new TownChoice
		{
			Enabled = placement.Enabled,
			OffsetX = placement.OffsetX,
			OffsetY = placement.OffsetY,
		};

	/// <summary>Remembers a price the player typed, or forgets it again when it matches what the item is worth.</summary>
	public void RememberPrice(string page, int itemId, long price, long original)
	{
		var choice = Pages.TryGetValue(page, out var existing) ? existing : Pages[page] = new PageChoice();
		if (price == original)
		{
			choice.Prices.Remove(itemId);
		}
		else
		{
			choice.Prices[itemId] = price;
		}
	}

	public void RememberRemoved(string page, int itemId, bool removed)
	{
		var choice = Pages.TryGetValue(page, out var existing) ? existing : Pages[page] = new PageChoice();
		if (removed)
		{
			if (!choice.Removed.Contains(itemId))
			{
				choice.Removed.Add(itemId);
			}
		}
		else
		{
			choice.Removed.Remove(itemId);
		}
	}
}
