using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace L2Config.Core.Skills;

public enum SkillKind
{
	Buff,
	SongOrDance,
	Debuff,
}

/// <summary>A skill whose effect has a duration — the skills Player.ini's SkillDurationList can change.</summary>
public sealed record TimedSkill(
	int Id,
	string Name,
	SkillKind Kind,
	IReadOnlyList<int> BaseSeconds,
	int? EnchantedMaxSeconds,
	bool LearnedByPlayers,
	bool FromBuffer,
	bool IsCustom)
{
	public int MinSeconds => BaseSeconds.Min();
	public int MaxSeconds => BaseSeconds.Max();
	public bool VariesByLevel => MinSeconds != MaxSeconds;

	/// <summary>Only NPCs, monsters or items use it (not in any player skill tree, not given by the buffer).</summary>
	public bool NpcOnly => !LearnedByPlayers && !FromBuffer;
}

/// <summary>
/// Every skill with a duration, read from the datapack (game\data\stats\skills, custom skills last so they override), with who uses
/// it (player skill trees, the scheme buffer) and its "+Time" enchant durations.
/// </summary>
/// <remarks>
/// How the server applies SkillDurationList (Mobius Skill constructor, CT_0_Interlude): when EnableModifySkillDuration is on and the
/// skill id is listed and the skill is not a toggle, abnormalTime becomes the listed seconds for levels below 100 and above 140, and
/// the listed seconds are ADDED to the enchanted duration for levels 100–139. Toggles have no abnormalTime in this datapack.
/// </remarks>
public sealed partial class SkillCatalog
{
	/// <summary>The longest duration the editor allows (12 hours).</summary>
	public const int MaxSeconds = 12 * 60 * 60;

	private readonly Dictionary<int, TimedSkill> _byId;

	private SkillCatalog(Dictionary<int, TimedSkill> skills)
	{
		_byId = skills;
		All = skills.Values.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ThenBy(s => s.Id).ToList();
	}

	public IReadOnlyList<TimedSkill> All { get; }
	public int Count => _byId.Count;

	public TimedSkill? Find(int id) => _byId.GetValueOrDefault(id);

	public static SkillCatalog LoadFromServer(string serverRoot)
	{
		var data = Path.Combine(serverRoot, "game", "data");
		return Load(Path.Combine(data, "stats", "skills"), Path.Combine(data, "stats", "players", "skillTrees"), Path.Combine(data, "SchemeBufferSkills.xml"));
	}

	public static SkillCatalog Load(string skillsFolder, string skillTreesFolder, string schemeBufferFile)
	{
		var learned = new HashSet<int>();
		if (Directory.Exists(skillTreesFolder))
		{
			foreach (var file in Directory.EnumerateFiles(skillTreesFolder, "*.xml", SearchOption.AllDirectories))
			{
				foreach (Match m in SkillTreeId().Matches(File.ReadAllText(file)))
				{
					learned.Add(int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture));
				}
			}
		}
		var buffer = new HashSet<int>();
		if (File.Exists(schemeBufferFile))
		{
			foreach (var buff in XDocument.Load(schemeBufferFile).Descendants("buff"))
			{
				if (int.TryParse((string?)buff.Attribute("id"), out var id))
				{
					buffer.Add(id);
				}
			}
		}

		var skills = new Dictionary<int, TimedSkill>();
		if (Directory.Exists(skillsFolder))
		{
			// Stock files first, then subfolders (custom), so later definitions override earlier ones as on the server.
			var files = Directory.EnumerateFiles(skillsFolder, "*.xml", SearchOption.TopDirectoryOnly).Order(StringComparer.OrdinalIgnoreCase)
				.Concat(Directory.EnumerateDirectories(skillsFolder).Order(StringComparer.OrdinalIgnoreCase)
					.SelectMany(d => Directory.EnumerateFiles(d, "*.xml", SearchOption.AllDirectories).Order(StringComparer.OrdinalIgnoreCase)));
			foreach (var file in files)
			{
				var isCustom = !string.Equals(Path.GetDirectoryName(file), skillsFolder.TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase);
				foreach (var element in XDocument.Load(file).Descendants("skill"))
				{
					if (!int.TryParse((string?)element.Attribute("id"), out var id))
					{
						continue;
					}
					if (Read(element, id, learned.Contains(id), buffer.Contains(id), isCustom) is { } skill)
					{
						skills[id] = skill;
					}
					else
					{
						skills.Remove(id);
					}
				}
			}
		}
		return new SkillCatalog(skills);
	}

	private static TimedSkill? Read(XElement skill, int id, bool learned, bool buffer, bool isCustom)
	{
		if (string.Equals(skill.Element("operateType")?.Value.Trim(), "T", StringComparison.OrdinalIgnoreCase))
		{
			return null; // toggles are never changed by the list
		}
		var tables = skill.Elements("table")
			.Where(t => t.Attribute("name") is not null)
			.GroupBy(t => (string)t.Attribute("name")!)
			.ToDictionary(g => g.Key, g => g.First().Value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
		var levels = int.TryParse((string?)skill.Attribute("levels"), out var n) && n > 0 ? n : 1;

		var baseSeconds = Values(skill.Element("abnormalTime")?.Value, tables);
		if (baseSeconds.Count == 0 || baseSeconds.All(s => s <= 0))
		{
			return null;
		}
		if (baseSeconds.Count > levels)
		{
			baseSeconds = baseSeconds.Take(levels).ToList();
		}
		var enchanted = skill.Elements()
			.Where(e => e.Name.LocalName.StartsWith("enchant", StringComparison.Ordinal) && (string?)e.Attribute("name") == "abnormalTime")
			.SelectMany(e => Values(e.Value, tables))
			.ToList();

		var abnormalType = skill.Element("abnormalType")?.Value.Trim() ?? "";
		var isDebuff = skill.Element("isDebuff")?.Value.Trim() is "1" or "true";
		var kind = isDebuff ? SkillKind.Debuff
			: abnormalType.StartsWith("SONG_", StringComparison.OrdinalIgnoreCase) || abnormalType.StartsWith("DANCE_", StringComparison.OrdinalIgnoreCase) ? SkillKind.SongOrDance
			: SkillKind.Buff;
		var name = (string?)skill.Attribute("name") is { Length: > 0 } s ? s : $"Skill {id}";
		return new TimedSkill(id, name, kind, baseSeconds.Where(v => v > 0).DefaultIfEmpty(baseSeconds.Max()).ToList(),
			enchanted.Count > 0 ? enchanted.Max() : null, learned, buffer, isCustom);
	}

	private static List<int> Values(string? raw, IReadOnlyDictionary<string, string[]> tables)
	{
		raw = raw?.Trim();
		if (string.IsNullOrEmpty(raw))
		{
			return [];
		}
		var parts = raw.StartsWith('#') ? tables.GetValueOrDefault(raw) ?? [] : [raw];
		return parts.Select(p => double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? (int)Math.Round(d) : -1)
			.Where(v => v >= 0)
			.ToList();
	}

	[GeneratedRegex("skillId=\"(\\d+)\"")]
	private static partial Regex SkillTreeId();
}

/// <summary>Player.ini SkillDurationList: "skillId,seconds;skillId,seconds", read and written the way the server reads it.</summary>
public static class SkillDurationList
{
	/// <summary>The listed durations (the last entry wins for a repeated id, as on the server) and a problem for each entry it can't read.</summary>
	public static (SortedDictionary<int, int> Durations, IReadOnlyList<string> Problems) Parse(string? value)
	{
		var durations = new SortedDictionary<int, int>();
		var problems = new List<string>();
		foreach (var raw in (value ?? "").Split(';'))
		{
			var entry = raw.Trim();
			if (entry.Length == 0)
			{
				continue;
			}
			var parts = entry.Split(',');
			if (parts.Length != 2
				|| !int.TryParse(parts[0].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var id)
				|| !int.TryParse(parts[1].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
			{
				problems.Add($"“{entry}” is not a skill id and a number of seconds (for example 1040,3600).");
				continue;
			}
			if (id <= 0)
			{
				problems.Add($"“{entry}”: the skill id must be above 0.");
				continue;
			}
			if (seconds is < 1 or > SkillCatalog.MaxSeconds)
			{
				problems.Add($"Skill {id}: {seconds:N0} seconds is outside 1 second – 12 hours.");
			}
			durations[id] = seconds;
		}
		return (durations, problems);
	}

	/// <summary>Sorted by skill id, no trailing separator; empty when nothing is listed.</summary>
	public static string Format(IEnumerable<KeyValuePair<int, int>> durations) =>
		string.Join(";", durations.OrderBy(d => d.Key).Select(d => $"{d.Key.ToString(CultureInfo.InvariantCulture)},{d.Value.ToString(CultureInfo.InvariantCulture)}"));

	/// <summary>"20 min", "1 h 30 min", "45 s".</summary>
	public static string Describe(int seconds)
	{
		if (seconds < 60)
		{
			return $"{seconds} s";
		}
		var h = seconds / 3600;
		var m = seconds % 3600 / 60;
		var s = seconds % 60;
		var parts = new List<string>();
		if (h > 0)
		{
			parts.Add($"{h} h");
		}
		if (m > 0)
		{
			parts.Add($"{m} min");
		}
		if (s > 0)
		{
			parts.Add($"{s} s");
		}
		return string.Join(" ", parts);
	}

	/// <summary>Reads "90", "90s", "20m", "20 min", "1h 30m", "1:30:00", "20:00"; null when it can't.</summary>
	public static int? ParseDuration(string? text)
	{
		text = text?.Trim().ToLowerInvariant();
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var plain))
		{
			return plain;
		}
		var clock = Regex.Match(text, @"^(?:(\d+):)?(\d{1,2}):(\d{2})$");
		if (clock.Success)
		{
			var hours = clock.Groups[1].Success ? int.Parse(clock.Groups[1].Value, CultureInfo.InvariantCulture) : 0;
			var first = int.Parse(clock.Groups[2].Value, CultureInfo.InvariantCulture);
			var second = int.Parse(clock.Groups[3].Value, CultureInfo.InvariantCulture);
			return clock.Groups[1].Success ? hours * 3600 + first * 60 + second : first * 60 + second;
		}
		// Every non-space character must belong to an "<amount><unit>" part, so "20 minutes please" is refused.
		var matches = Regex.Matches(text, @"(\d{1,6})\s*(h|hr|hrs|hour|hours|m|min|mins|minute|minutes|s|sec|secs|second|seconds)\b");
		static int Letters(string s) => s.Count(c => !char.IsWhiteSpace(c));
		if (matches.Count == 0 || matches.Sum(m => Letters(m.Value)) != Letters(text))
		{
			return null;
		}
		var total = matches.Sum(m =>
		{
			var amount = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
			return m.Groups[2].Value[0] switch { 'h' => amount * 3600, 'm' => amount * 60, _ => amount };
		});
		return total <= int.MaxValue ? (int)total : null;
	}
}
