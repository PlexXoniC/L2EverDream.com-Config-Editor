using L2Config.Core.Ini;

namespace L2Config.Core.Tests;

public class IniDocumentTests
{
	private const string Mobius =
		"# Experience multiplier\r\nRateXp = 15\r\n# Skill points multiplier\r\nRateSp=15\r\n\r\nBufferPool.Huge.Size = 4\r\n";

	[Fact]
	public void UnchangedDocumentRoundTripsExactly()
	{
		Assert.Equal(Mobius, IniDocument.Parse(Mobius).Text);
	}

	[Fact]
	public void SetChangesOnlyTheValueAndKeepsSpacing()
	{
		var doc = IniDocument.Parse(Mobius);
		doc.Set(null, "RateXp", "1");
		doc.Set(null, "RateSp", "2");

		Assert.Equal(Mobius.Replace("RateXp = 15", "RateXp = 1").Replace("RateSp=15", "RateSp=2"), doc.Text);
	}

	[Fact]
	public void GetIsCaseInsensitiveAndHandlesDottedKeys()
	{
		var doc = IniDocument.Parse(Mobius);
		Assert.Equal("15", doc.Get(null, "ratexp"));
		Assert.Equal("4", doc.Get(null, "BufferPool.Huge.Size"));
		Assert.Null(doc.Get(null, "Missing"));
	}

	[Fact]
	public void CommentedOutKeysAreIgnored()
	{
		var doc = IniDocument.Parse("#RateXp = 99\nRateXp = 3\n");
		Assert.Equal("3", doc.Get(null, "RateXp"));
	}

	[Fact]
	public void SectionedValuesAreScopedToTheirSection()
	{
		var doc = IniDocument.Parse("[Video]\nGamma=0.8\n\n[ClippingRange]\nTerrain=4.0\n[Other]\nTerrain=9\n");
		doc.Set("ClippingRange", "Terrain", "6.0");
		doc.Set("Video", "PostProc", "1");

		Assert.Equal("[Video]\nGamma=0.8\nPostProc=1\n\n[ClippingRange]\nTerrain=6.0\n[Other]\nTerrain=9\n", doc.Text);
	}

	[Fact]
	public void MissingSectionIsAppended()
	{
		var doc = IniDocument.Parse("[Video]\nGamma=0.8\n");
		doc.Set("Audio", "MusicVolume", "0.5");
		Assert.Equal("[Video]\nGamma=0.8\n\n[Audio]\nMusicVolume=0.5\n", doc.Text);
	}

	[Fact]
	public void LineBreaksInValuesAreRejected()
	{
		var doc = IniDocument.Parse("A = 1\n");
		Assert.Throws<ArgumentException>(() => doc.Set(null, "A", "1\nB = 2"));
	}
}
