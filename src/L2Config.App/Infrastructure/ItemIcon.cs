using System.Windows.Media;
using System.Windows.Media.Imaging;
using L2Config.Core.Client;

namespace L2Config.App.Infrastructure;

/// <summary>
/// Turns an item icon decoded from the player's own game client into something WPF can draw. Images are cached and
/// frozen, so the same icon is decoded once however many rows show it.
/// </summary>
public static class ItemIcon
{
	private static readonly Dictionary<string, ImageSource?> Cache = new(StringComparer.OrdinalIgnoreCase);

	public static ImageSource? For(IconLibrary? icons, string? iconName)
	{
		if (icons is null || string.IsNullOrWhiteSpace(iconName))
		{
			return null;
		}
		if (Cache.TryGetValue(iconName, out var cached))
		{
			return cached;
		}
		ImageSource? image = null;
		if (icons.Find(iconName) is { } decoded)
		{
			var bitmap = BitmapSource.Create(decoded.Width, decoded.Height, 96, 96, PixelFormats.Bgra32, null, decoded.Bgra, decoded.Width * 4);
			bitmap.Freeze();
			image = bitmap;
		}
		Cache[iconName] = image;
		return image;
	}

	/// <summary>Forgotten when the client folder changes, so icons are read from the new client.</summary>
	public static void Clear() => Cache.Clear();
}
