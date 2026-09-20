using System.Text;

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
	private const uint PackageTag = 0x9E2A83C1;
	private readonly string _systemTextures;
	private readonly string _textures;
	private readonly Dictionary<string, Package?> _packages = new(StringComparer.OrdinalIgnoreCase);
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
			image = Load(packageName)?.Read(textureName);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or IndexOutOfRangeException or ArgumentOutOfRangeException)
		{
			image = null;
		}
		_cache[iconName] = image;
		return image;
	}

	private Package? Load(string packageName)
	{
		if (_packages.TryGetValue(packageName, out var cached))
		{
			return cached;
		}
		Package? package = null;
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
				package = Package.Open(path);
				break;
			}
		}
		_packages[packageName] = package;
		return package;
	}

	// ------------------------------------------------------------------------------------------ the package

	private sealed class Package
	{
		private readonly byte[] _data;
		private readonly string[] _names;
		private readonly Dictionary<string, (int Offset, int Size)> _exports = new(StringComparer.OrdinalIgnoreCase);

		private Package(byte[] data, string[] names)
		{
			_data = data;
			_names = names;
		}

		public static Package? Open(string path)
		{
			var raw = File.ReadAllBytes(path);
			if (raw.Length < 32)
			{
				return null;
			}
			var data = Decrypt(raw, Path.GetFileName(path));
			var r = new Cursor(data);
			if (r.U32() != PackageTag)
			{
				return null;
			}
			r.U16();
			r.U16();
			r.U32();
			int nameCount = (int)r.U32(), nameOffset = (int)r.U32();
			int exportCount = (int)r.U32(), exportOffset = (int)r.U32();

			var names = new string[nameCount];
			r.Position = nameOffset;
			for (var i = 0; i < nameCount; i++)
			{
				names[i] = r.Name();
				r.U32();
			}

			var package = new Package(data, names);
			r.Position = exportOffset;
			for (var i = 0; i < exportCount; i++)
			{
				r.Index();
				r.Index();
				r.U32();
				var name = r.Index();
				r.U32();
				var size = r.Index();
				var offset = size > 0 ? r.Index() : 0;
				if (size > 0 && name >= 0 && name < names.Length)
				{
					package._exports.TryAdd(names[name], (offset, size));
				}
			}
			return package;
		}

		/// <summary>"Lineage2Ver121": every byte after the header is XORed with a key made from the file name.</summary>
		private static byte[] Decrypt(byte[] raw, string fileName)
		{
			const int headerBytes = 28;
			var header = Encoding.Unicode.GetString(raw, 0, Math.Min(headerBytes, raw.Length));
			if (!header.StartsWith("Lineage2Ver", StringComparison.Ordinal))
			{
				return raw;
			}
			byte key = 0;
			foreach (var c in fileName.ToLowerInvariant())
			{
				key = unchecked((byte)(key + c));
			}
			var body = new byte[raw.Length - headerBytes];
			for (var i = 0; i < body.Length; i++)
			{
				body[i] = (byte)(raw[headerBytes + i] ^ key);
			}
			return body;
		}

		public IconImage? Read(string textureName)
		{
			if (!_exports.TryGetValue(textureName, out var export))
			{
				return null;
			}
			var r = new Cursor(_data) { Position = export.Offset };
			int width = 0, height = 0, format = -1;

			// Properties, until the "None" name.
			while (true)
			{
				var nameIndex = r.Index();
				if (nameIndex < 0 || nameIndex >= _names.Length)
				{
					return null;
				}
				var name = _names[nameIndex];
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

			r.Position += 4; // four bytes the client writes before the mipmaps
			var mips = r.Index();
			if (mips <= 0)
			{
				return null;
			}
			var endOfData = r.I32();
			var length = r.Index();
			if (length <= 0 || r.Position + length > _data.Length || endOfData != r.Position + length)
			{
				return null;
			}
			var pixels = _data.AsSpan(r.Position, length);
			return format switch
			{
				3 => new IconImage(width, height, Dxt.Decode(pixels, width, height, dxt3: false)),
				7 => new IconImage(width, height, Dxt.Decode(pixels, width, height, dxt3: true)),
				5 => new IconImage(width, height, pixels.ToArray()),
				_ => null,
			};
		}
	}

	/// <summary>Unreal's little-endian reader, including its variable-length "compact index".</summary>
	private sealed class Cursor(byte[] data)
	{
		public int Position { get; set; }

		public byte U8() => data[Position++];

		public ushort U16()
		{
			var value = BitConverter.ToUInt16(data, Position);
			Position += 2;
			return value;
		}

		public uint U32()
		{
			var value = BitConverter.ToUInt32(data, Position);
			Position += 4;
			return value;
		}

		public int I32()
		{
			var value = BitConverter.ToInt32(data, Position);
			Position += 4;
			return value;
		}

		public int Index()
		{
			var first = U8();
			var negative = (first & 0x80) != 0;
			var value = first & 0x3F;
			if ((first & 0x40) != 0)
			{
				var shift = 6;
				for (var i = 0; i < 4; i++)
				{
					var b = U8();
					value |= (b & 0x7F) << shift;
					shift += 7;
					if ((b & 0x80) == 0)
					{
						break;
					}
				}
			}
			return negative ? -value : value;
		}

		public string Name()
		{
			var length = Index();
			if (length <= 0 || Position + length > data.Length)
			{
				return "";
			}
			var text = Encoding.Latin1.GetString(data, Position, length).TrimEnd('\0');
			Position += length;
			return text;
		}
	}

	/// <summary>DXT1 and DXT3 to BGRA.</summary>
	private static class Dxt
	{
		public static byte[] Decode(ReadOnlySpan<byte> source, int width, int height, bool dxt3)
		{
			var result = new byte[width * height * 4];
			var blockSize = dxt3 ? 16 : 8;
			var index = 0;
			for (var blockY = 0; blockY < (height + 3) / 4; blockY++)
			{
				for (var blockX = 0; blockX < (width + 3) / 4; blockX++, index += blockSize)
				{
					if (index + blockSize > source.Length)
					{
						return result;
					}
					var block = source.Slice(index, blockSize);
					var alpha = dxt3 ? block[..8] : default;
					var colours = dxt3 ? block[8..] : block;
					DecodeBlock(colours, alpha, dxt3, result, width, height, blockX * 4, blockY * 4);
				}
			}
			return result;
		}

		private static void DecodeBlock(ReadOnlySpan<byte> colours, ReadOnlySpan<byte> alpha, bool dxt3, byte[] result, int width, int height, int x0, int y0)
		{
			var c0 = (ushort)(colours[0] | (colours[1] << 8));
			var c1 = (ushort)(colours[2] | (colours[3] << 8));
			Span<int> r = stackalloc int[4], g = stackalloc int[4], b = stackalloc int[4], a = stackalloc int[4];
			Unpack(c0, out r[0], out g[0], out b[0]);
			Unpack(c1, out r[1], out g[1], out b[1]);
			a[0] = a[1] = a[2] = a[3] = 255;
			if (dxt3 || c0 > c1)
			{
				r[2] = (2 * r[0] + r[1]) / 3; g[2] = (2 * g[0] + g[1]) / 3; b[2] = (2 * b[0] + b[1]) / 3;
				r[3] = (r[0] + 2 * r[1]) / 3; g[3] = (g[0] + 2 * g[1]) / 3; b[3] = (b[0] + 2 * b[1]) / 3;
			}
			else
			{
				r[2] = (r[0] + r[1]) / 2; g[2] = (g[0] + g[1]) / 2; b[2] = (b[0] + b[1]) / 2;
				r[3] = g[3] = b[3] = 0;
				a[3] = 0; // DXT1's transparent colour
			}

			var bits = colours[4] | (colours[5] << 8) | (colours[6] << 16) | (colours[7] << 24);
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
					var code = (bits >> (2 * (4 * y + x))) & 0x03;
					var offset = ((py * width) + px) * 4;
					result[offset] = (byte)b[code];
					result[offset + 1] = (byte)g[code];
					result[offset + 2] = (byte)r[code];
					if (dxt3)
					{
						var nibble = alpha[(4 * y + x) / 2];
						var value = ((4 * y + x) % 2 == 0 ? nibble & 0x0F : nibble >> 4) * 17;
						result[offset + 3] = (byte)value;
					}
					else
					{
						result[offset + 3] = (byte)a[code];
					}
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
