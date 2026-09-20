using System.Xml.Linq;

namespace L2Config.Core.Characters;

/// <summary>
/// How far the world itself enchants items. The database column holds far more than the game ever gives out, so this
/// is what the editor compares against before warning that a level could not be reached by playing.
/// </summary>
public static class EnchantRules
{
	/// <summary>
	/// The highest enchant the world's own enchanting reaches, from `game\data\EnchantItemData.xml`. Null when the
	/// file cannot be read, in which case the editor simply does not mention a limit.
	/// </summary>
	public static int? MaxInGame(string? serverRoot)
	{
		if (serverRoot is null)
		{
			return null;
		}
		var path = Path.Combine(serverRoot, "game", "data", "EnchantItemData.xml");
		if (!File.Exists(path))
		{
			return null;
		}
		try
		{
			var levels = XDocument.Load(path).Descendants()
				.Select(e => e.Attribute("maxEnchant")?.Value)
				.Where(v => v is not null)
				.Select(v => int.TryParse(v, out var n) ? n : 0)
				.Where(n => n > 0)
				.ToList();
			return levels.Count == 0 ? null : levels.Max();
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Xml.XmlException)
		{
			return null;
		}
	}
}
