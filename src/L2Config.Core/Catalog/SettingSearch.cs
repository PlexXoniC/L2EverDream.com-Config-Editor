namespace L2Config.Core.Catalog;

/// <summary>
/// Matches a search box query against a setting. Every word must appear somewhere in the friendly name,
/// the real key, the file, the group or the description, so "xp party" finds RatePartyXp and
/// "Rates.ini" lists the whole file.
/// </summary>
public static class SettingSearch
{
	public static string[] Tokenize(string? query) =>
		string.IsNullOrWhiteSpace(query)
			? []
			: query.Split((char[])[' ', '\t', ',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

	public static bool Matches(SettingDefinition setting, string groupName, IReadOnlyList<string> tokens)
	{
		foreach (var token in tokens)
		{
			if (!Contains(setting.Name, token)
				&& !Contains(setting.Key, token)
				&& !Contains(setting.File, token)
				&& !Contains(groupName, token)
				&& !Contains(setting.Description, token)
				&& !Contains(setting.Section, token))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Higher is better: exact key, then name/key hits, then description-only hits. Ties go to the shorter key,
	/// which is usually the general setting rather than one of its many specific variants.
	/// </summary>
	public static int Rank(SettingDefinition setting, IReadOnlyList<string> tokens)
	{
		var score = 0;
		foreach (var token in tokens)
		{
			if (string.Equals(setting.Key, token, StringComparison.OrdinalIgnoreCase))
			{
				score += 100;
			}
			else if (Contains(setting.Name, token) || Contains(setting.Key, token))
			{
				score += 10;
			}
			else
			{
				score += 1;
			}
		}
		return score * 1000 - Math.Min(setting.Key.Length, 999);
	}

	private static bool Contains(string? haystack, string needle) =>
		haystack is not null && haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
}
