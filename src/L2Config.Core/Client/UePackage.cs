using System.Text;

namespace L2Config.Core.Client;

/// <summary>
/// A Lineage 2 Unreal package (<c>.ukx</c> animation/mesh packages, <c>.utx</c> textures), read without loading the
/// whole file: the mesh packages are hundreds of megabytes, so only the header, the name table, the export table and
/// the one object being asked for are decrypted.
/// <para>
/// The container is the same one <see cref="IconLibrary"/> reads: "Lineage2Ver111" XORs every byte with 0xAC,
/// "Lineage2Ver121" with a key made from the file name, and inside is an ordinary UE2 package.
/// </para>
/// </summary>
public sealed class UePackage : IDisposable
{
	private const uint PackageTag = 0x9E2A83C1;
	private const int HeaderBytes = 28;

	private readonly FileStream _file;
	private readonly byte _key;
	private readonly int _start;
	private readonly Dictionary<string, (int Offset, int Size)> _exports = new(StringComparer.OrdinalIgnoreCase);
	private readonly List<string> _exportOrder = [];
	private readonly List<string> _importOrder = [];

	private UePackage(FileStream file, byte key, int start)
	{
		_file = file;
		_key = key;
		_start = start;
	}

	public IReadOnlyCollection<string> ExportNames => _exports.Keys;

	/// <summary>The package's name table, which its objects refer to by index.</summary>
	public IReadOnlyList<string> Names { get; private set; } = [];

	public static UePackage? Open(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		FileStream? file = null;
		try
		{
			file = File.OpenRead(path);
			var head = new byte[HeaderBytes];
			if (file.Read(head) != HeaderBytes)
			{
				file.Dispose();
				return null;
			}
			var header = Encoding.Unicode.GetString(head);
			byte key;
			int start;
			if (header.StartsWith("Lineage2Ver111", StringComparison.Ordinal))
			{
				(key, start) = (0xAC, HeaderBytes);
			}
			else if (header.StartsWith("Lineage2Ver", StringComparison.Ordinal))
			{
				byte k = 0;
				foreach (var c in Path.GetFileName(path).ToLowerInvariant())
				{
					k = unchecked((byte)(k + c));
				}
				(key, start) = (k, HeaderBytes);
			}
			else
			{
				(key, start) = (0, 0);
			}

			var package = new UePackage(file, key, start);
			return package.ReadTables() ? package : Close(package);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
		{
			file?.Dispose();
			return null;
		}
	}

	private static UePackage? Close(UePackage package)
	{
		package.Dispose();
		return null;
	}

	private bool ReadTables()
	{
		var head = new Cursor(Read(0, 64));
		if (head.U32() != PackageTag)
		{
			return false;
		}
		head.U16();          // version
		head.U16();          // licensee version
		head.U32();          // package flags
		int nameCount = (int)head.U32(), nameOffset = (int)head.U32();
		int exportCount = (int)head.U32(), exportOffset = (int)head.U32();
		int importCount = (int)head.U32(), importOffset = (int)head.U32();
		if (nameCount <= 0 || exportCount <= 0)
		{
			return false;
		}

		// The name table sits right after the header while the import and export tables are at the far end of the
		// file, so read the two regions separately rather than the hundreds of megabytes between them.
		if (nameOffset < 0 || exportOffset < 0 || nameOffset >= _file.Length || exportOffset >= _file.Length)
		{
			return false;
		}
		var names = new string[nameCount];
		Names = names;
		var nameTable = new Cursor(Read(nameOffset, Region(nameOffset, exportOffset, importOffset)));
		for (var i = 0; i < nameCount; i++)
		{
			if (nameTable.Position + 5 > nameTable.Data.Length)
			{
				return false;
			}
			names[i] = nameTable.Name();
			nameTable.U32();    // flags
		}

		// Imports, so that an object reference into another package can be named.
		if (importCount > 0 && importOffset > 0 && importOffset < _file.Length)
		{
			var importTable = new Cursor(Read(importOffset, Region(importOffset, nameOffset, exportOffset)));
			for (var i = 0; i < importCount; i++)
			{
				if (importTable.Position + 8 > importTable.Data.Length)
				{
					break;
				}
				importTable.Index();  // class package
				importTable.Index();  // class name
				importTable.I32();    // package
				var objectName = importTable.Index();
				_importOrder.Add(objectName >= 0 && objectName < names.Length ? names[objectName] : "");
			}
		}

		var exportTable = new Cursor(Read(exportOffset, Region(exportOffset, nameOffset, importOffset)));
		for (var i = 0; i < exportCount; i++)
		{
			if (exportTable.Position + 12 > exportTable.Data.Length)
			{
				break;
			}
			exportTable.Index();  // class
			exportTable.Index();  // super
			exportTable.U32();    // package
			var name = exportTable.Index();
			exportTable.U32();    // flags
			var size = exportTable.Index();
			var offset = size > 0 ? exportTable.Index() : 0;
			var objectName = name >= 0 && name < names.Length ? names[name] : "";
			_exportOrder.Add(objectName);
			// A package can hold several objects under one name — a stub beside the real thing — so keep the biggest.
			if (size > 0 && objectName.Length > 0
				&& (!_exports.TryGetValue(objectName, out var existing) || size > existing.Size))
			{
				_exports[objectName] = (offset, size);
			}
		}
		return _exports.Count > 0;
	}

	/// <summary>How much to read for a table: up to the next table that follows it, or the end of the file, capped.</summary>
	private int Region(int from, params int[] others)
	{
		var end = _file.Length - _start;
		foreach (var other in others)
		{
			if (other > from && other < end)
			{
				end = other;
			}
		}
		return (int)Math.Min(end - from, 64L << 20);
	}

	/// <summary>The bytes of one object, or null when this package does not hold it.</summary>
	public byte[]? Export(string name) =>
		_exports.TryGetValue(name, out var e) ? Read(e.Offset, e.Size) : null;

	/// <summary>
	/// The name an object reference points at. Unreal counts exports from 1, imports from -1, and 0 means nothing.
	/// </summary>
	public string? ObjectName(int reference) => reference switch
	{
		> 0 when reference <= _exportOrder.Count => _exportOrder[reference - 1],
		< 0 when -reference <= _importOrder.Count => _importOrder[-reference - 1],
		_ => null,
	};

	/// <summary>Where an object starts in the package, which some objects refer to from inside themselves.</summary>
	public int OffsetOf(string name) => _exports.TryGetValue(name, out var e) ? e.Offset : 0;

	private byte[] Read(int offset, int count)
	{
		var buffer = new byte[count];
		_file.Position = _start + offset;
		var read = _file.ReadAtLeast(buffer, count, throwOnEndOfStream: false);
		if (read < count)
		{
			Array.Resize(ref buffer, read);
		}
		if (_key != 0)
		{
			for (var i = 0; i < buffer.Length; i++)
			{
				buffer[i] ^= _key;
			}
		}
		return buffer;
	}

	public void Dispose() => _file.Dispose();

	/// <summary>Reads the UE2 primitives out of a decrypted block.</summary>
	internal sealed class Cursor(byte[] data)
	{
		public int Position { get; set; }

		public byte[] Data => data;

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

		public float F32()
		{
			var value = BitConverter.ToSingle(data, Position);
			Position += 4;
			return value;
		}

		public void Skip(int count) => Position += count;

		/// <summary>UE2's compact index: a sign bit, then 6 + 7 × n value bits.</summary>
		public int Index()
		{
			int b = U8();
			var negative = (b & 0x80) != 0;
			var value = b & 0x3F;
			if ((b & 0x40) != 0)
			{
				var shift = 6;
				while (true)
				{
					int c = U8();
					value |= (c & 0x7F) << shift;
					shift += 7;
					if ((c & 0x80) == 0)
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
			var text = Encoding.Latin1.GetString(data, Position, length - 1);
			Position += length;
			return text;
		}
	}
}
