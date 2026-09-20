namespace L2Config.Core.Client;

/// <summary>
/// One texture out of an Unreal package: the object's property list gives its size and format, then come the mipmaps,
/// the first of which is the full-size picture. Lineage 2 uses DXT1 and DXT3 for almost everything, with a few plain
/// 32-bit pictures.
/// </summary>
public static class UeTexture
{
	/// <param name="baseOffset">Where this object starts in its package: the mipmap header points at the package,
	/// not at the object, so the two have to be compared in the same terms.</param>
	public static IconImage? Read(byte[] data, IReadOnlyList<string> names, int baseOffset = 0)
	{
		var r = new UePackage.Cursor(data);
		int width = 0, height = 0, format = -1;

		// Properties, until the "None" name.
		while (true)
		{
			var nameIndex = r.Index();
			if (nameIndex < 0 || nameIndex >= names.Count)
			{
				return null;
			}
			var name = names[nameIndex];
			if (string.Equals(name, "None", StringComparison.OrdinalIgnoreCase))
			{
				break;
			}
			var info = r.U8();
			var type = info & 0x0F;
			var sizeBits = (info >> 4) & 0x07;
			if (type == 10)
			{
				r.Index();
			}
			var size = sizeBits switch
			{
				0 => 1, 1 => 2, 2 => 4, 3 => 12, 4 => 16,
				5 => r.U8(),
				6 => r.U16(),
				_ => (int)r.U32(),
			};
			if ((info & 0x80) != 0 && type != 3)
			{
				r.Index();
			}
			var start = r.Position;
			var value = type switch { 1 => r.U8(), 2 => r.I32(), _ => 0 };
			r.Position = start + (type == 3 ? 0 : size);
			switch (name)
			{
				case "USize": width = value; break;
				case "VSize": height = value; break;
				case "Format": format = value; break;
			}
		}

		if (width <= 0 || height <= 0)
		{
			return null;
		}

		r.Skip(4);                      // four bytes the client writes before the mipmaps
		var mips = r.Index();
		if (mips <= 0)
		{
			return null;
		}
		var endOfData = r.I32();
		var length = r.Index();
		if (length <= 0 || r.Position + length > data.Length || endOfData != baseOffset + r.Position + length)
		{
			return null;
		}
		var pixels = data.AsSpan(r.Position, length);
		return format switch
		{
			3 => new IconImage(width, height, Dxt.Decode(pixels, width, height, dxt3: false)),
			7 => new IconImage(width, height, Dxt.Decode(pixels, width, height, dxt3: true)),
			5 => new IconImage(width, height, pixels.ToArray()),
			_ => null,
		};
	}

	/// <summary>
	/// The object a shader paints with. Lineage 2 names a monster's skin after a Shader, whose "Diffuse" property
	/// points at the picture itself, so a name that does not read as a texture is worth following once.
	/// </summary>
	public static int? Diffuse(byte[] data, IReadOnlyList<string> names)
	{
		var r = new UePackage.Cursor(data);
		try
		{
			while (r.Position < data.Length)
			{
				var nameIndex = r.Index();
				if (nameIndex < 0 || nameIndex >= names.Count || string.Equals(names[nameIndex], "None", StringComparison.OrdinalIgnoreCase))
				{
					return null;
				}
				var property = names[nameIndex];
				var info = r.U8();
				var type = info & 0x0F;
				var sizeBits = (info >> 4) & 0x07;
				if (type == 10)
				{
					r.Index();
				}
				var size = sizeBits switch
				{
					0 => 1, 1 => 2, 2 => 4, 3 => 12, 4 => 16,
					5 => r.U8(),
					6 => r.U16(),
					_ => (int)r.U32(),
				};
				if ((info & 0x80) != 0 && type != 3)
				{
					r.Index();
				}
				var start = r.Position;
				// Type 5 is an object reference, which is what "Diffuse" holds.
				if (type == 5 && string.Equals(property, "Diffuse", StringComparison.OrdinalIgnoreCase))
				{
					return r.Index();
				}
				r.Position = start + (type == 3 ? 0 : size);
			}
		}
		catch (IndexOutOfRangeException)
		{
			return null;
		}
		return null;
	}

	/// <summary>DXT1 and DXT3: 4×4 blocks of two colours to blend between, DXT3 with four bits of alpha per pixel.</summary>
	internal static class Dxt
	{
		public static byte[] Decode(ReadOnlySpan<byte> source, int width, int height, bool dxt3)
		{
			var result = new byte[width * height * 4];
			var blockSize = dxt3 ? 16 : 8;
			var at = 0;
			for (var y = 0; y < height; y += 4)
			{
				for (var x = 0; x < width; x += 4)
				{
					if (at + blockSize > source.Length)
					{
						return result;
					}
					var block = source.Slice(at, blockSize);
					DecodeBlock(dxt3 ? block[8..] : block, dxt3 ? block[..8] : default, dxt3, result, width, height, x, y);
					at += blockSize;
				}
			}
			return result;
		}

		private static void DecodeBlock(ReadOnlySpan<byte> colours, ReadOnlySpan<byte> alpha, bool dxt3, byte[] result,
			int width, int height, int x0, int y0)
		{
			var c0 = (ushort)(colours[0] | (colours[1] << 8));
			var c1 = (ushort)(colours[2] | (colours[3] << 8));
			Span<int> red = stackalloc int[4];
			Span<int> green = stackalloc int[4];
			Span<int> blue = stackalloc int[4];
			Unpack(c0, out red[0], out green[0], out blue[0]);
			Unpack(c1, out red[1], out green[1], out blue[1]);
			if (c0 > c1 || dxt3)
			{
				red[2] = (2 * red[0] + red[1]) / 3;
				green[2] = (2 * green[0] + green[1]) / 3;
				blue[2] = (2 * blue[0] + blue[1]) / 3;
				red[3] = (red[0] + 2 * red[1]) / 3;
				green[3] = (green[0] + 2 * green[1]) / 3;
				blue[3] = (blue[0] + 2 * blue[1]) / 3;
			}
			else
			{
				red[2] = (red[0] + red[1]) / 2;
				green[2] = (green[0] + green[1]) / 2;
				blue[2] = (blue[0] + blue[1]) / 2;
				red[3] = green[3] = blue[3] = 0;
			}

			var bits = (uint)(colours[4] | (colours[5] << 8) | (colours[6] << 16) | (colours[7] << 24));
			for (var y = 0; y < 4; y++)
			{
				for (var x = 0; x < 4; x++)
				{
					var px = x0 + x;
					var py = y0 + y;
					if (px >= width || py >= height)
					{
						continue;
					}
					var code = (int)((bits >> (2 * (4 * y + x))) & 3);
					var at = (py * width + px) * 4;
					result[at] = (byte)blue[code];
					result[at + 1] = (byte)green[code];
					result[at + 2] = (byte)red[code];
					result[at + 3] = dxt3
						? (byte)(((alpha[(4 * y + x) / 2] >> (4 * ((4 * y + x) % 2))) & 0x0F) * 17)
						: code == 3 && c0 <= c1 ? (byte)0 : (byte)255;
				}
			}
		}

		private static void Unpack(ushort colour, out int r, out int g, out int b)
		{
			r = ((colour >> 11) & 0x1F) * 255 / 31;
			g = ((colour >> 5) & 0x3F) * 255 / 63;
			b = (colour & 0x1F) * 255 / 31;
		}
	}
}
