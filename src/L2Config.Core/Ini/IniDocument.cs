using System.Text.RegularExpressions;

namespace L2Config.Core.Ini;

/// <summary>
/// An INI file edited line by line, so everything not being changed (comments, blank lines, spacing,
/// key order, line endings) is written back exactly as it was read.
/// Handles both Mobius's section-less "Key = Value" files and the client's "[Section]" files.
/// </summary>
public sealed partial class IniDocument
{
	private readonly List<string> _lines;
	private readonly string _newline;
	private readonly bool _endsWithNewline;

	private IniDocument(List<string> lines, string newline, bool endsWithNewline)
	{
		_lines = lines;
		_newline = newline;
		_endsWithNewline = endsWithNewline;
	}

	public static IniDocument Parse(string text)
	{
		var newline = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
		var endsWithNewline = text.EndsWith('\n');
		var body = endsWithNewline ? text[..^(text.EndsWith("\r\n", StringComparison.Ordinal) ? 2 : 1)] : text;
		var lines = body.Length == 0 ? new List<string>() : body.Split(newline).ToList();
		return new IniDocument(lines, newline, endsWithNewline);
	}

	public string Text => string.Join(_newline, _lines) + (_endsWithNewline ? _newline : "");

	/// <summary>The value of <paramref name="key"/>, or null when absent. Pass null for section-less files.</summary>
	public string? Get(string? section, string key)
	{
		var index = FindKey(section, key);
		return index < 0 ? null : EntryRegex().Match(_lines[index]).Groups["value"].Value.TrimEnd();
	}

	/// <summary>All keys and values, first occurrence wins (matching how the game server reads them).</summary>
	public IReadOnlyDictionary<(string? Section, string Key), string> Values()
	{
		var result = new Dictionary<(string?, string), string>();
		string? section = null;
		foreach (var line in _lines)
		{
			var header = SectionRegex().Match(line);
			if (header.Success)
			{
				section = header.Groups["name"].Value;
				continue;
			}
			var entry = EntryRegex().Match(line);
			if (entry.Success)
			{
				result.TryAdd((section, entry.Groups["key"].Value), entry.Groups["value"].Value.TrimEnd());
			}
		}
		return result;
	}

	/// <summary>
	/// Sets a value, keeping the line's own spacing around '='. A missing key is appended to the end of its
	/// section (or the file); a missing section is appended to the file.
	/// </summary>
	public void Set(string? section, string key, string value)
	{
		if (value.Contains('\n') || value.Contains('\r'))
		{
			throw new ArgumentException("An INI value cannot contain a line break.", nameof(value));
		}

		var index = FindKey(section, key);
		if (index >= 0)
		{
			var match = EntryRegex().Match(_lines[index]);
			_lines[index] = match.Groups["lead"].Value + match.Groups["key"].Value + match.Groups["sep"].Value + value;
			return;
		}

		var spacing = GuessSeparator();
		var newLine = key + spacing + value;
		if (section is null)
		{
			_lines.Add(newLine);
			return;
		}

		var (start, end) = FindSection(section);
		if (start < 0)
		{
			if (_lines.Count > 0 && _lines[^1].Trim().Length > 0)
			{
				_lines.Add("");
			}
			_lines.Add($"[{section}]");
			_lines.Add(newLine);
			return;
		}

		// Insert after the last non-blank line of the section so blank separators stay where they were.
		var insertAt = end;
		while (insertAt > start + 1 && _lines[insertAt - 1].Trim().Length == 0)
		{
			insertAt--;
		}
		_lines.Insert(insertAt, newLine);
	}

	private int FindKey(string? section, string key)
	{
		int from = 0, to = _lines.Count;
		if (section is not null)
		{
			(from, to) = FindSection(section);
			if (from < 0)
			{
				return -1;
			}
			from++;
		}
		for (var i = from; i < to; i++)
		{
			var entry = EntryRegex().Match(_lines[i]);
			if (entry.Success && string.Equals(entry.Groups["key"].Value, key, StringComparison.OrdinalIgnoreCase))
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>Index of the section header and the index one past its last line, or (-1, -1).</summary>
	private (int Start, int End) FindSection(string section)
	{
		for (var i = 0; i < _lines.Count; i++)
		{
			var header = SectionRegex().Match(_lines[i]);
			if (!header.Success || !string.Equals(header.Groups["name"].Value, section, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			var end = i + 1;
			while (end < _lines.Count && !SectionRegex().IsMatch(_lines[end]))
			{
				end++;
			}
			return (i, end);
		}
		return (-1, -1);
	}

	private string GuessSeparator()
	{
		foreach (var line in _lines)
		{
			var entry = EntryRegex().Match(line);
			if (entry.Success)
			{
				return entry.Groups["sep"].Value;
			}
		}
		return " = ";
	}

	[GeneratedRegex(@"^(?<lead>\s*)(?<key>[A-Za-z0-9_.\[\]]+)(?<sep>\s*=\s*)(?<value>.*)$")]
	private static partial Regex EntryRegex();

	[GeneratedRegex(@"^\s*\[(?<name>[^\]]+)\]\s*$")]
	private static partial Regex SectionRegex();
}
