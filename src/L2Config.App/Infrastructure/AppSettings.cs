using System.IO;
using System.Text.Json;

namespace L2Config.App.Infrastructure;

/// <summary>The config manager's own preferences (the folders the user picked). Never the game's settings.</summary>
public sealed class AppSettings
{
	private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };

	public string? ServerFolder { get; set; }
	public string? ClientFolder { get; set; }
	public bool ShowAdvanced { get; set; }
	public string LastTab { get; set; } = "server";

	/// <summary>Developer snapshot runs use throwaway settings that are never written.</summary>
	[System.Text.Json.Serialization.JsonIgnore]
	public bool Transient { get; init; }

	public static string Folder => Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "L2EverdreamConfig");

	private static string FilePath => Path.Combine(Folder, "settings.json");

	public static AppSettings Load()
	{
		try
		{
			return File.Exists(FilePath)
				? JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings()
				: new AppSettings();
		}
		catch (JsonException)
		{
			return new AppSettings();
		}
	}

	public void Save()
	{
		if (Transient)
		{
			return;
		}
		Directory.CreateDirectory(Folder);
		File.WriteAllText(FilePath, JsonSerializer.Serialize(this, Json));
	}
}
