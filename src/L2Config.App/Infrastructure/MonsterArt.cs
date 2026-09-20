using System.Windows.Media;
using System.Windows.Media.Imaging;
using L2Config.Core.Client;

namespace L2Config.App.Infrastructure;

/// <summary>
/// Draws a monster from the player's own game client and hands WPF the picture. Drawing reads a model out of a very
/// large package file, so it happens off the interface thread; the result is frozen and cached, and a client with no
/// model for a monster simply has no picture.
/// </summary>
public static class MonsterArt
{
	private static readonly Dictionary<int, IReadOnlyList<ImageSource>?> Cache = [];
	private static readonly Lock Gate = new();

	/// <summary>Every step of the monster turning on the spot, ready for WPF. Null when there is nothing to draw.</summary>
	public static async Task<IReadOnlyList<ImageSource>?> TurnAsync(MonsterArtLibrary? art, int npcId, int size = 208)
	{
		if (art is null)
		{
			return null;
		}
		lock (Gate)
		{
			if (Cache.TryGetValue(npcId, out var cached))
			{
				return cached;
			}
		}

		var drawn = await Task.Run(() => art.Turn(npcId, size));
		IReadOnlyList<ImageSource>? frames = null;
		if (drawn is not null)
		{
			var images = new List<ImageSource>(drawn.Count);
			foreach (var frame in drawn)
			{
				var bitmap = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgra32, null, frame.Bgra, frame.Width * 4);
				bitmap.Freeze();
				images.Add(bitmap);
			}
			frames = images;
		}
		lock (Gate)
		{
			Cache[npcId] = frames;
		}
		return frames;
	}

	/// <summary>Forgotten when the client folder changes, so monsters are drawn from the new client.</summary>
	public static void Clear()
	{
		lock (Gate)
		{
			Cache.Clear();
		}
	}
}
