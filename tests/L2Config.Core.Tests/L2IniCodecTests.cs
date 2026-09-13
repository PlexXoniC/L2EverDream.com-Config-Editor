using System.Text;
using L2Config.Core.Ini;

namespace L2Config.Core.Tests;

public class L2IniCodecTests
{
	[Fact]
	public void Ver413RoundTripsAndCarriesAValidTail()
	{
		const string text = "[URL]\r\nServerAddr=127.0.0.1\r\nPort=7777\r\n" + "[Padding]\r\nLine=abcdefghijklmnopqrstuvwxyz0123456789\r\n";
		var bytes = L2IniCodec.Encode(text, L2IniFormat.Ver413);

		Assert.Equal(L2IniFormat.Ver413, L2IniCodec.DetectFormat(bytes));
		Assert.Equal(text, L2IniCodec.Decode(bytes, out var format));
		Assert.Equal(L2IniFormat.Ver413, format);

		var crc = BitConverter.ToUInt32(bytes, bytes.Length - 8);
		Assert.Equal(L2IniCodec.Crc32(bytes.AsSpan(0, bytes.Length - 20)), crc);
		Assert.All(bytes[^20..^8], b => Assert.Equal(0, b));
	}

	[Fact]
	public void Ver111RoundTrips()
	{
		const string text = "[LanguageSet]\r\nLanguage=1\r\n";
		var bytes = L2IniCodec.Encode(text, L2IniFormat.Ver111);
		Assert.Equal(text, L2IniCodec.Decode(bytes, out var format));
		Assert.Equal(L2IniFormat.Ver111, format);
	}

	[Fact]
	public void PlainFilesPassThroughByteForByte()
	{
		var bytes = new byte[] { (byte)'[', (byte)'A', (byte)']', 0xE9, (byte)'\r', (byte)'\n' };
		Assert.Equal(bytes, L2IniCodec.Encode(L2IniCodec.Decode(bytes, out var format), format));
	}

	[Fact]
	public void Crc32MatchesTheStandardCheckValue()
	{
		Assert.Equal(0xCBF43926u, L2IniCodec.Crc32(Encoding.ASCII.GetBytes("123456789")));
	}

	[LocalInstallFact]
	public void DecodesTheRealClientL2Ini()
	{
		var path = Path.Combine(TestPaths.RealLocations.ClientSystemDir!, "l2.ini");
		var bytes = File.ReadAllBytes(path);
		var text = L2IniCodec.Decode(bytes, out var format);

		Assert.Equal(L2IniFormat.Ver413, format);
		Assert.Contains("ServerAddr=", text);
		Assert.Equal(text, L2IniCodec.Decode(L2IniCodec.Encode(text, format), out _));
	}
}
