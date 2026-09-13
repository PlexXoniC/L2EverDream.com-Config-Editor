using System.Text.Json;

namespace L2Config.Core.Catalog;

public static class CatalogLoader
{
	private static readonly JsonSerializerOptions Options = new()
	{
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
	};

	public static SettingsCatalog Load(string path)
	{
		using var stream = File.OpenRead(path);
		return Load(stream);
	}

	public static SettingsCatalog Load(Stream stream) =>
		JsonSerializer.Deserialize<SettingsCatalog>(stream, Options)
			?? throw new InvalidDataException("The settings catalog is empty.");
}
