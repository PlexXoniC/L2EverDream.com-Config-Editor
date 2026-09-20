namespace L2Config.Core.Client;

/// <summary>An icon decoded from the game client: 32-bit BGRA pixels, top row first.</summary>
public sealed record IconImage(int Width, int Height, byte[] Bgra);

/// <summary>
/// Reads item icons out of the Lineage 2 client the user already has (systextures\Icon.utx and friends). Nothing is
/// copied or redistributed: the picture is decoded from the player's own installed client, in memory, when it is shown.
/// <para>
/// The packages are Unreal Engine 2 packages wrapped in the client's "Lineage2Ver121" obfuscation (every byte after the
/// 28-byte header is XORed with a key made from the file name). Inside, each icon is a Texture object whose properties
/// give its size and format, followed by its mipmaps. Icons are DXT1 or DXT3 compressed.
/// </para>
/// </summary>
public sealed class IconLibrary
{
	private readonly string _systemTextures;
	private readonly string _textures;
	private readonly Dictionary<string, UePackage?> _packages = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, IconImage?> _cache = new(StringComparer.OrdinalIgnoreCase);

	private IconLibrary(string systemTextures, string textures)
	{
		_systemTextures = systemTextures;
		_textures = textures;
	}

	/// <summary>Null when the client folder has no texture packages (so the app simply shows no icons).</summary>
	public static IconLibrary? ForClient(string? clientSystemDir)
	{
		if (clientSystemDir is null)
		{
			return null;
		}
		var root = Path.GetDirectoryName(Path.GetFullPath(clientSystemDir.TrimEnd('\\', '/')));
		if (root is null)
		{
			return null;
		}
		var systemTextures = Path.Combine(root, "systextures");
		var textures = Path.Combine(root, "textures");
		return Directory.Exists(systemTextures) || Directory.Exists(textures) ? new IconLibrary(systemTextures, textures) : null;
	}

	/// <summary>Looks up an icon by its datapack name, e.g. "icon.weapon_long_sword_i00". Null when it cannot be read.</summary>
	public IconImage? Find(string? iconName)
	{
		if (string.IsNullOrWhiteSpace(iconName))
		{
			return null;
		}
		if (_cache.TryGetValue(iconName, out var cached))
		{
			return cached;
		}
		IconImage? image = null;
		try
		{
			var dot = iconName.IndexOf('.');
			var packageName = dot > 0 ? iconName[..dot] : "Icon";
			var textureName = dot > 0 ? iconName[(dot + 1)..] : iconName;
			image = Load(packageName) is { } package && package.Export(textureName) is { } export
				? UeTexture.Read(export, package.Names, package.OffsetOf(textureName))
				: null;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or IndexOutOfRangeException or ArgumentOutOfRangeException)
		{
			image = null;
		}
		_cache[iconName] = image;
		return image;
	}

	private UePackage? Load(string packageName)
	{
		if (_packages.TryGetValue(packageName, out var cached))
		{
			return cached;
		}
		UePackage? package = null;
		foreach (var folder in new[] { _systemTextures, _textures })
		{
			var path = Path.Combine(folder, packageName + ".utx");
			if (!File.Exists(path) && Directory.Exists(folder))
			{
				path = Directory.EnumerateFiles(folder, "*.utx")
					.FirstOrDefault(f => string.Equals(Path.GetFileNameWithoutExtension(f), packageName, StringComparison.OrdinalIgnoreCase)) ?? path;
			}
			if (File.Exists(path))
			{
				package = UePackage.Open(path);
				break;
			}
		}
		_packages[packageName] = package;
		return package;
	}
}
