namespace L2Config.Core.Shops;

/// <summary>
/// The parts of a world's datapack a shop is built out of. Every L2Everdream install has all of them, so this only ever
/// has something to say when the server folder is not really a world — the wrong folder picked, or an install that did
/// not finish. Without it the tab would simply show no towns and nothing to sell, and never say why.
/// </summary>
public static class ShopSources
{
	private static readonly (string Folder, string What)[] Parts =
	[
		(@"stats\items", "the items a shop would sell"),
		(@"stats\npcs", "the gatekeepers a shopkeeper stands beside"),
		(@"spawns", "the place each gatekeeper stands in"),
		(@"stats\armorsets", "the armour sets a shop sells whole"),
	];

	/// <summary>A line for each part of the datapack this world is missing, in plain language. Empty when all is well.</summary>
	public static IReadOnlyList<string> Missing(string? serverRoot)
	{
		if (string.IsNullOrWhiteSpace(serverRoot))
		{
			return [];
		}
		var data = Path.Combine(serverRoot, "game", "data");
		var missing = new List<string>();
		foreach (var (folder, what) in Parts)
		{
			var path = Path.Combine(data, folder.Replace('\\', Path.DirectorySeparatorChar));
			if (!Directory.Exists(path) || !Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories).Any())
			{
				missing.Add($@"game\data\{folder} is missing or empty — that is {what}.");
			}
		}
		return missing;
	}
}
