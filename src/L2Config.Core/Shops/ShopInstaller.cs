using System.Text.RegularExpressions;
using L2Config.Core.Backups;

namespace L2Config.Core.Shops;

public enum ShopState
{
	/// <summary>Nothing of ours is in the world.</summary>
	NotInstalled,

	/// <summary>Every file is there and says what this shop says.</summary>
	Installed,

	/// <summary>Some of it is there, or what is there is not what this shop would write (an update, or a hand edit).</summary>
	Different,
}

/// <summary>What the world holds right now, and anything that would stop the shop working.</summary>
public sealed record ShopStatus(ShopState State, bool SpawnsEnabled, IReadOnlyList<string> Problems)
{
	public bool Works => State == ShopState.Installed && Problems.Count == 0;
}

/// <summary>
/// Puts a shop into a world and takes it out again. Everything it writes is a file of its own, so an L2Everdream update
/// leaves it alone; everything it replaces or deletes is backed up first, the same as any other change the app makes.
/// The world reads all of this when it starts, so a change needs the world restarted from the launcher.
/// </summary>
public static class ShopInstaller
{
	/// <summary>The settings the server needs on before it will read any of these folders.</summary>
	public static IReadOnlyList<(string File, string Key, string Why)> Needs { get; } =
	[
		("General.ini", "CustomNpcData", "the shopkeeper is a custom NPC"),
		("General.ini", "CustomBuyListLoad", "the shop's buy windows are custom lists"),
		("General.ini", "CustomMultisellLoad", "the shop's exchange windows are custom lists"),
	];

	public static string DataFolder(string serverRoot) => Path.Combine(serverRoot, "game", "data");

	/// <summary>What is in the world now, compared with what this shop would write.</summary>
	public static ShopStatus Look(string serverRoot, ShopDefinition shop, Func<string, string, string?>? setting = null)
	{
		var data = DataFolder(serverRoot);
		var problems = new List<string>();
		if (setting is not null)
		{
			foreach (var (file, key, why) in Needs)
			{
				if (!string.Equals(setting(file, key)?.Trim(), "True", StringComparison.OrdinalIgnoreCase))
				{
					problems.Add($"{key} is switched off, and {why}.");
				}
			}
		}

		var wanted = ShopFiles.Build(shop);
		var present = wanted.Count(f => File.Exists(Path.Combine(data, f.RelativePath.Replace('/', Path.DirectorySeparatorChar))));
		if (present == 0 && Ours(data, shop).Count == 0)
		{
			return new ShopStatus(ShopState.NotInstalled, shop.Enabled, problems);
		}

		var same = wanted.All(f => Reads(data, f.RelativePath) == f.Content) && Ours(data, shop).Count == wanted.Count;
		var enabled = Reads(data, $"spawns/{ShopFiles.Folder}/gm-shops.xml")?.Contains("enabled=\"true\"", StringComparison.Ordinal) ?? false;
		return new ShopStatus(same ? ShopState.Installed : ShopState.Different, enabled, problems);
	}

	/// <summary>Writes the shop. Anything it replaces goes into the backup first.</summary>
	public static void Install(string serverRoot, ShopDefinition shop, BackupSession backup)
	{
		var data = DataFolder(serverRoot);
		var wanted = ShopFiles.Build(shop);
		var keep = wanted.Select(f => f.RelativePath).ToHashSet(StringComparer.OrdinalIgnoreCase);

		// A shop with fewer pages than last time leaves numbered lists behind, so anything of ours that is no longer
		// wanted goes first.
		foreach (var stale in Ours(data, shop).Where(path => !keep.Contains(path)))
		{
			Delete(data, stale, backup);
		}

		foreach (var file in wanted)
		{
			var path = Path.Combine(data, file.RelativePath.Replace('/', Path.DirectorySeparatorChar));
			if (File.Exists(path))
			{
				if (Reads(data, file.RelativePath) == file.Content)
				{
					continue;
				}
				backup.PreserveFile(path, "shop", file.RelativePath);
			}
			Directory.CreateDirectory(Path.GetDirectoryName(path)!);
			File.WriteAllText(path, file.Content);
		}
		backup.NoteChange($"GM shop in {shop.Towns.Count()} town(s)", null, shop.Enabled ? "on" : "written but switched off");
	}

	/// <summary>Takes the shop out again: our files are backed up, then deleted, and our own folders tidied away.</summary>
	public static void Remove(string serverRoot, ShopDefinition shop, BackupSession backup)
	{
		var data = DataFolder(serverRoot);
		foreach (var path in Ours(data, shop))
		{
			Delete(data, path, backup);
		}
		foreach (var folder in new[] { $"stats/npcs/custom/{ShopFiles.Folder}", $"spawns/{ShopFiles.Folder}" })
		{
			var full = Path.Combine(data, folder.Replace('/', Path.DirectorySeparatorChar));
			if (Directory.Exists(full) && Directory.GetFileSystemEntries(full).Length == 0)
			{
				Directory.Delete(full);
			}
		}
		backup.NoteChange("GM shop", "installed", "removed");
	}

	/// <summary>
	/// Every file in the world that belongs to this shop: the ones it would write now, plus any numbered list left over
	/// from a shop with more pages. Nothing outside these paths is ever touched.
	/// </summary>
	private static List<string> Ours(string data, ShopDefinition shop)
	{
		var paths = ShopFiles.Build(shop)
			.Select(f => f.RelativePath)
			.Where(p => File.Exists(Path.Combine(data, p.Replace('/', Path.DirectorySeparatorChar))))
			.ToList();

		paths.AddRange(Numbered(data, "buylists/custom", ShopFiles.FirstBuyListId));
		paths.AddRange(Numbered(data, "multisell/custom", ShopFiles.FirstMultisellId));
		foreach (var page in new[] { $"html/merchant/{shop.NpcId}.htm" })
		{
			if (File.Exists(Path.Combine(data, page.Replace('/', Path.DirectorySeparatorChar))) && !paths.Contains(page))
			{
				paths.Add(page);
			}
		}
		return paths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
	}

	/// <summary>The numbered files in one of the server's custom folders that fall in the block this app writes.</summary>
	private static IEnumerable<string> Numbered(string data, string folder, int first)
	{
		var full = Path.Combine(data, folder.Replace('/', Path.DirectorySeparatorChar));
		if (!Directory.Exists(full))
		{
			yield break;
		}
		foreach (var file in Directory.EnumerateFiles(full, "*.xml"))
		{
			var name = Path.GetFileNameWithoutExtension(file);
			if (Regex.IsMatch(name, @"^\d+$") && int.TryParse(name, out var id) && id >= first && id < first + 100)
			{
				yield return $"{folder}/{Path.GetFileName(file)}";
			}
		}
	}

	private static void Delete(string data, string relative, BackupSession backup)
	{
		var path = Path.Combine(data, relative.Replace('/', Path.DirectorySeparatorChar));
		if (!File.Exists(path))
		{
			return;
		}
		backup.PreserveFile(path, "shop", relative);
		File.Delete(path);
	}

	private static string? Reads(string data, string relative)
	{
		var path = Path.Combine(data, relative.Replace('/', Path.DirectorySeparatorChar));
		return File.Exists(path) ? File.ReadAllText(path) : null;
	}
}
