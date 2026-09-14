using System.Xml;

namespace L2Config.Core.Characters;

/// <summary>Every item the server knows, read from the datapack's game\data\stats\items XML (including custom items).</summary>
public sealed class ItemCatalog
{
	private readonly Dictionary<int, ItemTemplate> _items;

	private ItemCatalog(Dictionary<int, ItemTemplate> items)
	{
		_items = items;
		All = items.Values.OrderBy(i => i.Id).ToList();
	}

	public IReadOnlyList<ItemTemplate> All { get; }
	public int Count => _items.Count;

	public ItemTemplate? Find(int id) => _items.GetValueOrDefault(id);

	public static ItemCatalog LoadFromServer(string serverRoot) =>
		LoadFromFolder(Path.Combine(serverRoot, "game", "data", "stats", "items"));

	public static ItemCatalog LoadFromFolder(string folder)
	{
		var items = new Dictionary<int, ItemTemplate>();
		if (!Directory.Exists(folder))
		{
			return new ItemCatalog(items);
		}
		// Custom items (a subfolder) are read last so they override stock definitions, as the server does.
		var files = Directory.EnumerateFiles(folder, "*.xml", SearchOption.TopDirectoryOnly).Order()
			.Concat(Directory.EnumerateDirectories(folder).Order().SelectMany(d => Directory.EnumerateFiles(d, "*.xml", SearchOption.AllDirectories).Order()));
		var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, IgnoreComments = true, IgnoreWhitespace = true };
		foreach (var file in files)
		{
			using var reader = XmlReader.Create(file, settings);
			ItemBuilder? current = null;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "item")
				{
					if (int.TryParse(reader.GetAttribute("id"), out var id))
					{
						current = new ItemBuilder(id, reader.GetAttribute("name") ?? $"Item {id}", reader.GetAttribute("type") ?? "EtcItem");
					}
					if (reader.IsEmptyElement && current is not null)
					{
						items[current.Id] = current.Build();
						current = null;
					}
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.Name == "set" && current is not null && reader.Depth == 2)
				{
					current.Set(reader.GetAttribute("name"), reader.GetAttribute("val"));
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "item" && current is not null)
				{
					items[current.Id] = current.Build();
					current = null;
				}
			}
		}
		return new ItemCatalog(items);
	}

	private sealed class ItemBuilder(int id, string name, string type)
	{
		private bool _stackable;
		private bool _quest;
		private string? _grade;
		private int _weight;
		private string? _kind;

		public int Id => id;

		public void Set(string? key, string? value)
		{
			switch (key)
			{
				case "is_stackable":
					_stackable = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
					break;
				case "is_questitem":
					_quest = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
					break;
				case "crystal_type":
					_grade = value;
					break;
				case "weight":
					int.TryParse(value, out _weight);
					break;
				case "etcitem_type" or "weapon_type" or "bodypart":
					_kind ??= value;
					break;
			}
		}

		// Weapons and armor are never stackable in Interlude, whatever the XML says.
		public ItemTemplate Build() => new(id, name, type, type == "EtcItem" && _stackable, _quest, _grade, _weight, _kind);
	}
}

public sealed record ItemTemplate(int Id, string Name, string Type, bool Stackable, bool Quest, string? Grade, int Weight, string? Kind)
{
	public string Description
	{
		get
		{
			var parts = new List<string> { Type == "EtcItem" ? "Item" : Type };
			if (Kind is not null)
			{
				parts.Add(Kind.ToLowerInvariant() switch
				{
					"rear;lear" => "earring",
					"rfinger;lfinger" => "ring",
					"lrhand" => "two-handed",
					"rhand" => "one-handed",
					"lhand" => "shield",
					"onepiece" => "full body",
					"hair" or "hairall" => "hair accessory",
					var k => k.Replace('_', ' '),
				});
			}
			if (Grade is not null)
			{
				parts.Add($"{Grade}-grade");
			}
			if (Stackable)
			{
				parts.Add("stackable");
			}
			if (Quest)
			{
				parts.Add("quest item");
			}
			return string.Join(" · ", parts);
		}
	}
}
