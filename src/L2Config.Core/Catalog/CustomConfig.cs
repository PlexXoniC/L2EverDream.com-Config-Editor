using System.Text.Json;

namespace L2Config.Core.Catalog;

/// <summary>How the L2Everdream release's config differs from stock L2J Mobius (catalog/custom-config.json).</summary>
public sealed class CustomConfigReport
{
	public int Version { get; init; }
	public StockSource Stock { get; init; } = new();
	public IReadOnlyList<CustomChange> Changes { get; init; } = [];
	public IReadOnlyList<CustomFile> Files { get; init; } = [];

	public static CustomConfigReport Load(string path)
	{
		using var stream = File.OpenRead(path);
		return Load(stream);
	}

	public static CustomConfigReport Load(Stream stream) =>
		JsonSerializer.Deserialize<CustomConfigReport>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
			?? throw new InvalidDataException("The Custom Config report is empty.");
}

public sealed class StockSource
{
	public string Project { get; init; } = "";
	public string Path { get; init; } = "";
	public string Commit { get; init; } = "";
	public string Committed { get; init; } = "";
}

public sealed class CustomChange
{
	public required string Target { get; init; }
	public required string File { get; init; }
	public required string Key { get; init; }

	/// <summary>added (new setting), value (changed value), note (same value, documented), removed.</summary>
	public required string Kind { get; init; }

	public required string Name { get; init; }
	public string? SettingId { get; init; }
	public string? Category { get; init; }
	public string? StockValue { get; init; }
	public string? ShippedValue { get; init; }
	public string Note { get; init; } = "";
	public bool LauncherManaged { get; init; }
}

public sealed class CustomFile
{
	public required string Target { get; init; }
	public required string File { get; init; }
	public required string Kind { get; init; }
	public string Summary { get; init; } = "";
}
