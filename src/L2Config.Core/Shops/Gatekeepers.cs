using System.Xml.Linq;

namespace L2Config.Core.Shops;

/// <summary>A gatekeeper standing somewhere in the world, read from the world's own spawn files.</summary>
public sealed record Gatekeeper(string Town, int NpcId, string Name, int X, int Y, int Z, int Heading)
{
	/// <summary>One of the places people mean by "every town", so it is ticked by default.</summary>
	public bool IsMainTown => Gatekeepers.Towns.ContainsKey(NpcId);

	public string Where => $"{Town} — {Name}";
}

/// <summary>
/// Finds every gatekeeper in a world: the npc templates typed `Teleporter`, then wherever the spawn files put them.
/// Nothing is hard-coded about the positions — they come from the player's own datapack, so they stay right when
/// L2Everdream moves someone.
/// </summary>
public static class Gatekeepers
{
	/// <summary>
	/// The gatekeeper people think of as their town's, by npc id. The names are here because a few of the spawn files
	/// are named after map squares rather than towns.
	/// </summary>
	public static IReadOnlyDictionary<int, string> Towns { get; } = new Dictionary<int, string>
	{
		[30006] = "Talking Island",
		[30320] = "Gludin Village",
		[30256] = "Gludio",
		[30059] = "Dion",
		[30878] = "Floran",
		[30080] = "Giran",
		[30177] = "Oren",
		[30162] = "Ivory Tower",
		[30233] = "Hunters Village",
		[30899] = "Heine",
		[30848] = "Aden",
		[31275] = "Goddard",
		[31320] = "Rune",
		[31964] = "Schuttgart",
		[30146] = "Elven Village",
		[30134] = "Dark Elf Village",
		[30540] = "Dwarven Village",
		[30576] = "Orc Village",
	};

	public static IReadOnlyList<Gatekeeper> ReadFromServer(string serverRoot) => Read(
		Path.Combine(serverRoot, "game", "data", "stats", "npcs"),
		Path.Combine(serverRoot, "game", "data", "spawns"));

	public static IReadOnlyList<Gatekeeper> Read(string npcFolder, string spawnFolder)
	{
		var names = TeleporterNames(npcFolder);
		var found = new List<Gatekeeper>();
		if (names.Count == 0 || !Directory.Exists(spawnFolder))
		{
			return found;
		}

		foreach (var file in Directory.EnumerateFiles(spawnFolder, "*.xml", SearchOption.AllDirectories).Order())
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
			foreach (var npc in document.Descendants("npc"))
			{
				if (!int.TryParse(npc.Attribute("id")?.Value, out var id)
					|| !names.TryGetValue(id, out var name)
					|| Coordinate(npc, "x") is not { } x
					|| Coordinate(npc, "y") is not { } y
					|| Coordinate(npc, "z") is not { } z)
				{
					continue;
				}
				found.Add(new Gatekeeper(
					Towns.GetValueOrDefault(id) ?? TownFromFileName(file),
					id, name, x, y, z, Coordinate(npc, "heading") ?? 0));
			}
		}
		// A gatekeeper spawned twice (Dion's tower pairs) would only confuse the list: keep the first of each.
		return found.GroupBy(g => g.NpcId).Select(g => g.First()).OrderBy(g => !g.IsMainTown).ThenBy(g => g.Town).ToList();
	}

	private static int? Coordinate(XElement npc, string attribute) =>
		int.TryParse(npc.Attribute(attribute)?.Value, out var value) ? value : null;

	private static Dictionary<int, string> TeleporterNames(string npcFolder)
	{
		var names = new Dictionary<int, string>();
		if (!Directory.Exists(npcFolder))
		{
			return names;
		}
		foreach (var file in Directory.EnumerateFiles(npcFolder, "*.xml", SearchOption.AllDirectories))
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
			foreach (var npc in document.Descendants("npc"))
			{
				if (npc.Attribute("type")?.Value == "Teleporter"
					&& int.TryParse(npc.Attribute("id")?.Value, out var id)
					&& npc.Attribute("name")?.Value is { Length: > 0 } name)
				{
					names[id] = name;
				}
			}
		}
		return names;
	}

	/// <summary>`Giran\GiranNPCs.xml` → "Giran", `TalkingIslandNPCs.xml` → "Talking Island".</summary>
	private static string TownFromFileName(string file)
	{
		var name = Path.GetFileNameWithoutExtension(file);
		if (name.EndsWith("NPCs", StringComparison.OrdinalIgnoreCase))
		{
			name = name[..^4];
		}
		var words = new System.Text.StringBuilder();
		foreach (var character in name)
		{
			if (char.IsUpper(character) && words.Length > 0 && words[^1] != ' ')
			{
				words.Append(' ');
			}
			words.Append(character == '_' ? ' ' : character);
		}
		return words.ToString().Trim() is { Length: > 0 } town ? town : "Elsewhere";
	}
}
