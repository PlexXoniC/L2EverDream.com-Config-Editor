using System.Globalization;
using System.IO.Compression;
using System.Numerics;
using System.Text;

namespace L2Config.Core.Ini;

/// <summary>
/// Reads and writes Lineage 2 client ini files.
/// <list type="bullet">
/// <item>Lineage2Ver413 (l2.ini, user.ini): UTF-16LE header, 128-byte RSA blocks holding a length-prefixed
/// zlib stream, and a 20-byte tail carrying CRC32(header + body). Same keys as the L2Everdream launcher's
/// L2IniCrypt, verified byte-for-byte against the shipped client.</item>
/// <item>Lineage2Ver111 (Localization.ini, TTFontInfo.ini): header, then every byte XOR 0xAC.</item>
/// <item>No header: plain text (Option.ini, WindowsInfo.ini).</item>
/// </list>
/// Text is decoded as Latin-1 so every byte survives a read/write round trip.
/// </summary>
public static class L2IniCodec
{
	private const string HeaderPrefix = "Lineage2Ver";
	private const int HeaderSize = 28;
	private const int TailSize = 20;
	private const int BlockSize = 128;
	private const int BlockBody = 124;

	private static readonly BigInteger Modulus = ParseHex(
		"75b4d6de5c016544068a1acf125869f43d2e09fc55b8b1e289556daf9b8757635593446288b3653da1ce91c87bb1a5c1" +
		"8f16323495c55d7d72c0890a83f69bfd1fd9434eb1c02f3e4679edfa43309319070129c267c85604d87bb65bae205de3" +
		"707af1d2108881abb567c3b3d069ae67c3a4c6a3aa93d26413d4c66094ae2039");

	private static readonly BigInteger PublicExponent = new(0x1D);

	private static readonly BigInteger PrivateExponent = ParseHex(
		"30b4c2d798d47086145c75063c8e841e719776e400291d7838d3e6c4405b504c6a07f8fca27f32b86643d2649d1d5f12" +
		"4cdd0bf272f0909dd7352fe10a77b34d831043d9ae541f8263c6fe3d1c14c2f04e43a7253a6dda9a8c1562cbd493c1b6" +
		"31a1957618ad5dfe5ca28553f746e2fc6f2db816c7db223ec91e955081c1de65");

	public static L2IniFormat DetectFormat(ReadOnlySpan<byte> data)
	{
		if (data.Length < HeaderSize)
		{
			return L2IniFormat.Plain;
		}
		var header = Encoding.Unicode.GetString(data[..HeaderSize]);
		return header switch
		{
			"Lineage2Ver413" => L2IniFormat.Ver413,
			"Lineage2Ver111" => L2IniFormat.Ver111,
			_ when header.StartsWith(HeaderPrefix, StringComparison.Ordinal) => L2IniFormat.Unsupported,
			_ => L2IniFormat.Plain,
		};
	}

	public static string Decode(ReadOnlySpan<byte> data, out L2IniFormat format)
	{
		format = DetectFormat(data);
		return format switch
		{
			L2IniFormat.Plain => Encoding.Latin1.GetString(data),
			L2IniFormat.Ver111 => DecodeXor(data[HeaderSize..]),
			L2IniFormat.Ver413 => Decode413(data),
			_ => throw new NotSupportedException("This client file uses an encryption version the config manager does not support."),
		};
	}

	public static byte[] Encode(string text, L2IniFormat format) => format switch
	{
		L2IniFormat.Plain => Encoding.Latin1.GetBytes(text),
		L2IniFormat.Ver111 => [.. Encoding.Unicode.GetBytes("Lineage2Ver111"), .. Encoding.Latin1.GetBytes(text).Select(b => (byte)(b ^ 0xAC))],
		L2IniFormat.Ver413 => Encode413(Encoding.Latin1.GetBytes(text)),
		_ => throw new NotSupportedException("Cannot write this client file format."),
	};

	private static string DecodeXor(ReadOnlySpan<byte> body)
	{
		var bytes = body.ToArray();
		for (var i = 0; i < bytes.Length; i++)
		{
			bytes[i] ^= 0xAC;
		}
		return Encoding.Latin1.GetString(bytes);
	}

	private static string Decode413(ReadOnlySpan<byte> data)
	{
		var body = data[HeaderSize..^TailSize];
		if (body.Length % BlockSize != 0)
		{
			throw new InvalidDataException("The encrypted body is not a whole number of RSA blocks.");
		}

		using var payload = new MemoryStream();
		for (var offset = 0; offset < body.Length; offset += BlockSize)
		{
			var block = RsaTransform(body.Slice(offset, BlockSize), PublicExponent);
			int size = block[3];
			if (size > BlockBody)
			{
				throw new InvalidDataException("A decrypted block reports an impossible size.");
			}
			var pad = -size & 3;
			payload.Write(block, BlockSize - size - pad, size);
		}

		var bytes = payload.ToArray();
		var expected = BitConverter.ToInt32(bytes, 0);
		using var inflater = new ZLibStream(new MemoryStream(bytes, 4, bytes.Length - 4), CompressionMode.Decompress);
		using var raw = new MemoryStream();
		inflater.CopyTo(raw);
		if (raw.Length != expected)
		{
			throw new InvalidDataException($"Decompressed {raw.Length} bytes but the file says {expected}.");
		}
		return Encoding.Latin1.GetString(raw.ToArray());
	}

	private static byte[] Encode413(byte[] raw)
	{
		using var payload = new MemoryStream();
		payload.Write(BitConverter.GetBytes(raw.Length));
		using (var deflater = new ZLibStream(payload, CompressionLevel.Optimal, leaveOpen: true))
		{
			deflater.Write(raw);
		}
		var compressed = payload.ToArray();

		var header = Encoding.Unicode.GetBytes("Lineage2Ver413");
		using var file = new MemoryStream();
		file.Write(header);
		for (var offset = 0; offset < compressed.Length; offset += BlockBody)
		{
			var size = Math.Min(BlockBody, compressed.Length - offset);
			var pad = -size & 3;
			var block = new byte[BlockSize];
			block[3] = (byte)size;
			Buffer.BlockCopy(compressed, offset, block, BlockSize - size - pad, size);
			file.Write(RsaTransform(block, PrivateExponent));
		}

		var crc = Crc32(file.ToArray());
		file.Write(new byte[12]);
		file.Write(BitConverter.GetBytes(crc));
		file.Write(new byte[4]);
		return file.ToArray();
	}

	private static byte[] RsaTransform(ReadOnlySpan<byte> block, BigInteger exponent)
	{
		var input = new BigInteger(block, isUnsigned: true, isBigEndian: true);
		var output = BigInteger.ModPow(input, exponent, Modulus).ToByteArray(isUnsigned: true, isBigEndian: true);
		var result = new byte[BlockSize];
		Buffer.BlockCopy(output, 0, result, BlockSize - output.Length, output.Length);
		return result;
	}

	/// <summary>Standard CRC-32 (IEEE, reflected, as zlib computes it).</summary>
	internal static uint Crc32(ReadOnlySpan<byte> data)
	{
		var crc = 0xFFFFFFFFu;
		foreach (var b in data)
		{
			crc ^= b;
			for (var bit = 0; bit < 8; bit++)
			{
				crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320u : crc >> 1;
			}
		}
		return ~crc;
	}

	private static BigInteger ParseHex(string hex) =>
		BigInteger.Parse("0" + hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
}

public enum L2IniFormat
{
	Plain,
	Ver111,
	Ver413,
	Unsupported,
}
