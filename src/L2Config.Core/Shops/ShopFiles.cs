using System.Globalization;
using System.Text;

namespace L2Config.Core.Shops;

/// <summary>One datapack file a shop needs, and where it goes under `game\data`.</summary>
public sealed record ShopFile(string RelativePath, string Content);

/// <summary>
/// Turns a shop into the files a world reads at start-up. Everything here is a <b>new</b> file in a folder the server
/// already loads from: nothing the release ships is edited, which is what lets an L2Everdream update leave the shop
/// alone (the launcher only captures and restores files it shipped, and it deletes nothing).
/// <list type="bullet">
/// <item>`stats\npcs\custom\…` — the shopkeeper. Loaded when `CustomNpcData` is on, and read recursively, so a folder
/// of our own can never collide with a file a future release adds.</item>
/// <item>`html\merchant\&lt;npc id&gt;.htm` — what clicking the shopkeeper shows. The server builds this path itself and
/// falls back to a blank "nothing to say" window if the file is not exactly there, so the name matters.</item>
/// <item>`buylists\custom\&lt;id&gt;.xml` and `multisell\custom\&lt;id&gt;.xml` — the windows. Both folders take numbered
/// files only, and need `CustomBuyListLoad` / `CustomMultisellLoad` on.</item>
/// <item>`spawns\…` — where the shopkeepers stand. The whole tree is read recursively and every file carries its own
/// `enabled`, which is the shop's on/off switch.</item>
/// </list>
/// </summary>
public static class ShopFiles
{
	/// <summary>Our own folder, so nothing we write can ever share a name with something L2Everdream ships.</summary>
	public const string Folder = "L2EverdreamConfig";

	/// <summary>Buy and barter lists are numbered from here; the ranges are clear in every release so far.</summary>
	public const int FirstBuyListId = 6000001;

	public const int FirstMultisellId = 4000001;

	public static IReadOnlyList<ShopFile> Build(ShopDefinition shop)
	{
		var files = new List<ShopFile>
		{
			new($"stats/npcs/custom/{Folder}/gm-shop.xml", Shopkeeper(shop)),
			new($"spawns/{Folder}/gm-shops.xml", Spawns(shop)),
		};
		files.AddRange(Pages(shop));
		files.Add(new($"html/merchant/{shop.NpcId}.htm", MainPage(shop)));
		return files;
	}

	/// <summary>Every path a shop owns, whether or not it is switched on — what a remove has to delete.</summary>
	public static IReadOnlyList<string> PathsOf(ShopDefinition shop) => Build(shop).Select(f => f.RelativePath).ToList();

	// ---------------------------------------------------------------- the shopkeeper

	private static string Shopkeeper(ShopDefinition shop) => $"""
		<?xml version="1.0" encoding="UTF-8"?>
		<list xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="../../../../xsd/npcs.xsd">
			<!-- Written by L2Everdream Config. Safe to delete: nothing else refers to it. -->
			<npc id="{shop.NpcId}" displayId="{shop.DisplayId}" name="{Escape(shop.Name)}" usingServerSideName="true" title="{Escape(shop.Title)}" usingServerSideTitle="true" type="Merchant">
				<status attackable="false" />
				<collision>
					<radius normal="8" />
					<height normal="23" />
				</collision>
			</npc>
		</list>

		""";

	private static string Spawns(ShopDefinition shop)
	{
		var text = new StringBuilder();
		text.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
		text.AppendLine($"""<list enabled="{(shop.Enabled ? "true" : "false")}" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="../../xsd/spawns.xsd">""");
		text.AppendLine("\t<!-- Written by L2Everdream Config. enabled=\"false\" above takes every shopkeeper away. -->");
		text.AppendLine($"\t<spawn name=\"{Folder}Shops\">");
		foreach (var place in shop.Towns)
		{
			text.AppendLine(
				$"\t\t<npc id=\"{shop.NpcId}\" x=\"{place.X}\" y=\"{place.Y}\" z=\"{place.Z}\" heading=\"{place.Heading}\" respawnDelay=\"60\" /> <!-- {Escape(place.Beside.Where)} -->");
		}
		text.AppendLine("\t</spawn>");
		text.AppendLine("</list>");
		return text.ToString();
	}

	// ---------------------------------------------------------------- the windows

	private static IEnumerable<ShopFile> Pages(ShopDefinition shop)
	{
		var buy = FirstBuyListId;
		var barter = FirstMultisellId;
		foreach (var page in shop.UsedPages)
		{
			yield return page.Kind == ShopPageKind.Buy
				? new ShopFile($"buylists/custom/{buy++}.xml", BuyList(shop, page))
				: new ShopFile($"multisell/custom/{barter++}.xml", Multisell(shop, page));
		}
	}

	/// <summary>The list id each page opens, in the order the pages are written.</summary>
	public static IReadOnlyList<(ShopPage Page, int ListId)> PageIds(ShopDefinition shop)
	{
		var buy = FirstBuyListId;
		var barter = FirstMultisellId;
		return shop.UsedPages
			.Select(page => (page, page.Kind == ShopPageKind.Buy ? buy++ : barter++))
			.ToList();
	}

	private static string BuyList(ShopDefinition shop, ShopPage page)
	{
		var text = new StringBuilder();
		text.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
		text.AppendLine("""<list xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="../../xsd/buylist.xsd">""");
		text.AppendLine($"\t<npcs><npc>{shop.NpcId}</npc></npcs>");
		foreach (var item in page.Items)
		{
			text.AppendLine($"\t<item id=\"{item.ItemId}\" price=\"{Price(item.Price)}\" />");
		}
		text.AppendLine("</list>");
		return text.ToString();
	}

	private static string Multisell(ShopDefinition shop, ShopPage page)
	{
		var text = new StringBuilder();
		text.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
		text.AppendLine("""<list maintainEnchantment="false" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="../../xsd/multisell.xsd">""");
		text.AppendLine($"\t<npcs><npc>{shop.NpcId}</npc></npcs>");
		foreach (var bundle in page.Bundles)
		{
			text.AppendLine($"\t<item> <!-- {Escape(bundle.Name)} -->");
			text.AppendLine($"\t\t<ingredient count=\"{Math.Max(1, Price(bundle.Price))}\" id=\"57\" />");
			foreach (var piece in bundle.Items)
			{
				text.AppendLine($"\t\t<production count=\"{Math.Max(1, piece.Count)}\" id=\"{piece.ItemId}\" />");
			}
			text.AppendLine("\t</item>");
		}
		text.AppendLine("</list>");
		return text.ToString();
	}

	// ---------------------------------------------------------------- what the shopkeeper says

	private static string MainPage(ShopDefinition shop)
	{
		var text = new StringBuilder();
		text.Append($"<html><title>{Escape(shop.Title)}</title><body><center><br>");
		text.Append($"<font color=\"LEVEL\">{Escape(shop.Title)}</font><br>");
		text.Append("<img src=\"L2UI.SquareGray\" width=\"270\" height=\"1\"><br>");
		text.Append("<table width=\"280\">");

		var pages = PageIds(shop);
		for (var i = 0; i < pages.Count; i += 2)
		{
			text.Append("<tr>");
			text.Append(Cell(pages[i]));
			text.Append(i + 1 < pages.Count ? Cell(pages[i + 1]) : "<td></td>");
			text.Append("</tr>");
		}
		text.Append("</table>");
		text.Append("<br><img src=\"L2UI.SquareGray\" width=\"270\" height=\"1\"><br>");
		text.Append($"<button value=\"Sell\" action=\"bypass -h npc_%objectId%_Sell\" width=\"85\" height=\"26\" back=\"L2UI_ch3.BigButton2_over\" fore=\"L2UI_ch3.BigButton2\">");
		text.Append("</center></body></html>");
		return text.ToString();

		static string Cell((ShopPage Page, int ListId) page)
		{
			var bypass = page.Page.Kind == ShopPageKind.Buy
				? $"npc_%objectId%_Buy {page.ListId}"
				: $"npc_%objectId%_multisell {page.ListId}";
			return "<td align=\"center\">"
				+ $"<button value=\"{Escape(page.Page.Title)}\" action=\"bypass -h {bypass}\" "
				+ "width=\"130\" height=\"26\" back=\"L2UI_ch3.BigButton2_over\" fore=\"L2UI_ch3.BigButton2\">"
				+ "</td>";
		}
	}

	/// <summary>The server keeps a buy price in an int, so a shop cannot ask for more than that however the item is valued.</summary>
	private static int Price(long price) => (int)Math.Clamp(price, 0, int.MaxValue);

	private static string Escape(string text) => text
		.Replace("&", "&amp;", StringComparison.Ordinal)
		.Replace("<", "&lt;", StringComparison.Ordinal)
		.Replace(">", "&gt;", StringComparison.Ordinal)
		.Replace("\"", "&quot;", StringComparison.Ordinal);
}
