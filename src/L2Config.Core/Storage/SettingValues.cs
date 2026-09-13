using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using L2Config.Core.Catalog;

namespace L2Config.Core.Storage;

/// <summary>Comparing, validating and formatting values the way the game server will read them.</summary>
public static partial class SettingValues
{
	public static bool IsTrue(string? value) => string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);

	public static bool AreEqual(SettingDefinition setting, string? a, string? b)
	{
		if (a is null || b is null)
		{
			return a == b;
		}
		if (setting.ValueType == SettingValueType.Bool || setting.Editor == SettingEditor.Toggle)
		{
			return IsTrue(a) == IsTrue(b);
		}
		if (setting.ValueType is SettingValueType.Int or SettingValueType.Decimal
			&& TryParseNumber(a, out var x) && TryParseNumber(b, out var y))
		{
			return Math.Abs(x - y) < 1e-9;
		}
		return string.Equals(a.Trim(), b.Trim(), StringComparison.Ordinal);
	}

	/// <summary>A plain-language problem with the value, or null when the server will accept it.</summary>
	public static string? Validate(SettingDefinition setting, string value)
	{
		if (value.Contains('\n') || value.Contains('\r'))
		{
			return "Must be on one line.";
		}
		if (setting.Editor == SettingEditor.Toggle)
		{
			// The switch only ever writes true/false; anything else already in a file (the client ships "Ture") reads as off.
			return null;
		}
		if (setting.Editor == SettingEditor.Choice && setting.Options is { Count: > 0 } options
			&& !options.Any(o => string.Equals(OptionValue(o), value.Trim(), StringComparison.OrdinalIgnoreCase)))
		{
			return "Pick one of the listed options.";
		}
		switch (setting.ValueType)
		{
			case SettingValueType.Int:
				if (!long.TryParse(value.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var whole))
				{
					return value.Trim().Length == 0 ? "Enter a number." : "Enter a whole number (no decimals).";
				}
				return CheckRange(setting, whole);
			case SettingValueType.Decimal:
				if (!TryParseNumber(value, out var number))
				{
					return "Enter a number (use a dot for decimals, e.g. 1.5).";
				}
				return CheckRange(setting, number);
		}
		return setting.Format is null ? null : ValidateFormat(setting.Format, value.Trim());
	}

	/// <summary>The value part of a catalog option ("0=Peace all the time" → "0").</summary>
	public static string OptionValue(string option)
	{
		var eq = option.IndexOf('=');
		return (eq > 0 ? option[..eq] : option).Trim();
	}

	/// <summary>Checks list and text shapes the Mobius config readers expect.</summary>
	public static string? ValidateFormat(string format, string value)
	{
		switch (format)
		{
			case "int-list":
				return value.Length == 0 || IntListComma().IsMatch(value) ? null : "Enter numbers separated by commas, e.g. 57,4037.";
			case "int-list;":
				return value.Length == 0 || IntListSemicolon().IsMatch(value) ? null : "Enter numbers separated by semicolons, e.g. 100;30;0.";
			case "pair-list":
				return value.Length == 0 || PairList().IsMatch(value) ? null : "Use id,value pairs separated by semicolons, e.g. 57,2;4037,1.5";
			case "boss-drop-list":
				return value.Length == 0 || BossDropList().IsMatch(value) ? null : "Use itemId,min,max,chance separated by semicolons, e.g. 4356,1,2,100;";
			case "range-list":
				return RangeList().IsMatch(value) ? null : "Use from,to level ranges separated by semicolons, e.g. 0,9;10,14;15,99.";
			case "percent-split-4":
			{
				var parts = value.Split(',');
				if (parts.Length != 4 || !parts.All(p => int.TryParse(p.Trim(), out var n) && n is >= 0 and <= 100))
				{
					return "Enter four percentages separated by commas, e.g. 55,35,7,3.";
				}
				return parts.Sum(p => int.Parse(p.Trim())) == 100 ? null : "The four percentages must add up to 100.";
			}
			case "time-list":
				return TimeList().IsMatch(value) ? null : "Enter times as HH:MM separated by commas, e.g. 08:00, 20:00.";
			case "weekday-list":
				return WeekdayList().IsMatch(value) ? null : "Enter day numbers 1–7 separated by commas (1 = Sunday).";
			case "hex-color":
				return HexColor().IsMatch(value) ? null : "Enter a 6-digit color code, e.g. 00FF00.";
			case "coordinates":
				return Coordinates().IsMatch(value) ? null : "Enter x,y,z coordinates, e.g. -84318,244579,-3730.";
			case "buffer-list":
				return BufferList().IsMatch(value) ? null : "Use sizexcount pairs separated by semicolons, e.g. 100x8;128x8.";
			case "word-list":
				return value.Length == 0 || WordListComma().IsMatch(value) ? null : "Enter words separated by commas.";
			case "word-list;":
				return value.Length == 0 || WordListSemicolon().IsMatch(value) ? null : "Enter words separated by semicolons.";
			case "ip-list":
				return value.Length == 0 || value.Split(',').All(p => IPAddress.TryParse(p.Trim(), out _)) ? null : "Enter IP addresses separated by commas.";
			case "host":
				return value is "*" || IPAddress.TryParse(value, out _) || HostName().IsMatch(value) ? null : "Enter an IP address such as 127.0.0.1.";
			case "regex":
				try
				{
					_ = new Regex(value);
					return null;
				}
				catch (ArgumentException)
				{
					return "This is not a valid name pattern (regular expression).";
				}
			default:
				return null;
		}
	}

	/// <summary>Formats a toggle in the same style ("True", "true", "TRUE") the file already uses.</summary>
	public static string FormatBool(bool value, string? existing)
	{
		var word = value ? "true" : "false";
		if (existing is { Length: > 0 } && char.IsUpper(existing[0]))
		{
			return existing.Length > 1 && char.IsUpper(existing[1]) ? word.ToUpperInvariant() : char.ToUpperInvariant(word[0]) + word[1..];
		}
		return existing is null ? char.ToUpperInvariant(word[0]) + word[1..] : word;
	}

	public static bool TryParseNumber(string value, out double number) =>
		double.TryParse(value.Trim().TrimEnd('.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);

	/// <summary>"Between 0 and 100", "At least 1", or null when unbounded.</summary>
	public static string? DescribeRange(SettingDefinition setting)
	{
		var min = setting.Min is { } lo && lo > -1e15 ? Format(lo) : null;
		var max = setting.Max is { } hi && hi < 1e15 && hi != int.MaxValue ? Format(hi) : null;
		return (min, max) switch
		{
			(not null, not null) => $"{min} – {max}",
			(not null, null) => $"{min} or more",
			(null, not null) => $"up to {max}",
			_ => null,
		};
	}

	private static string Format(double n) => n.ToString("0.##", CultureInfo.InvariantCulture);

	private static string? CheckRange(SettingDefinition setting, double value)
	{
		if (setting.Min is { } min && value < min)
		{
			return $"Must be at least {Format(min)}.";
		}
		if (setting.Max is { } max && value > max)
		{
			return $"Must be at most {Format(max)}.";
		}
		return null;
	}

	[GeneratedRegex(@"^\s*\d+(\s*,\s*\d+)*\s*,?\s*$")]
	private static partial Regex IntListComma();

	[GeneratedRegex(@"^\s*\d+(\s*;\s*\d+)*\s*;?\s*$")]
	private static partial Regex IntListSemicolon();

	[GeneratedRegex(@"^(\d+,\d+(\.\d+)?;)*\d+,\d+(\.\d+)?;?$")]
	private static partial Regex PairList();

	[GeneratedRegex(@"^(\d+,\d+,\d+,\d+(\.\d+)?;)*(\d+,\d+,\d+,\d+(\.\d+)?;?)$")]
	private static partial Regex BossDropList();

	[GeneratedRegex(@"^\d+,\d+(;\d+,\d+)*;?$")]
	private static partial Regex RangeList();

	[GeneratedRegex(@"^([01]?\d|2[0-3]):[0-5]\d(\s*,\s*([01]?\d|2[0-3]):[0-5]\d)*$")]
	private static partial Regex TimeList();

	[GeneratedRegex(@"^[1-7](\s*,\s*[1-7])*$")]
	private static partial Regex WeekdayList();

	[GeneratedRegex(@"^[0-9A-Fa-f]{6}$")]
	private static partial Regex HexColor();

	[GeneratedRegex(@"^-?\d+,-?\d+,-?\d+(,-?\d+)?$")]
	private static partial Regex Coordinates();

	[GeneratedRegex(@"^\d+x\d+(;\d+x\d+)*;?$")]
	private static partial Regex BufferList();

	[GeneratedRegex(@"^[^,;]+(\s*,\s*[^,;]+)*$")]
	private static partial Regex WordListComma();

	[GeneratedRegex(@"^[^,;]+(\s*;\s*[^,;]+)*;?$")]
	private static partial Regex WordListSemicolon();

	[GeneratedRegex(@"^[A-Za-z0-9.-]+$")]
	private static partial Regex HostName();
}
