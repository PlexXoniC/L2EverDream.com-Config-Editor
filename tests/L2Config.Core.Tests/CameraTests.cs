using L2Config.Core.Client;

namespace L2Config.Core.Tests;

public class CameraBindingTests
{
	// The two bindings that matter: what the client ships, and what the community zoom fix writes.
	private const string Stock = "CameraRotationModeOn | CameraRotationModeOff | FixedDefaultCamera OnRelease MaxPressedTime=200.0 ";
	private const string ZoomFix = "CameraRotationModeOn | CameraRotationModeOff | set Engine.LineagePlayerController MaxZoomingDist 65535 | set Engine.LineagePlayerController MinZoomingDist -200";

	[Fact]
	public void TheShippedBindingSnapsBackAndDoesNotZoomOut()
	{
		var binding = CameraBinding.Read(Stock);

		Assert.False(binding.ZoomsRightOut);
		Assert.True(binding.SnapsBack);
		Assert.Equal(CameraBinding.StockMaxDistance, binding.MaxDistance);
		Assert.Empty(binding.Extras);
	}

	[Fact]
	public void TheCommunityZoomFixIsReadBackAsItsSwitches()
	{
		var binding = CameraBinding.Read(ZoomFix);

		Assert.True(binding.ZoomsRightOut);
		Assert.False(binding.SnapsBack);   // the fix drops the tap-to-snap-back, which is why that is its own switch
		Assert.Equal(65535, binding.MaxDistance);
		Assert.Equal(-200, binding.MinDistance);
	}

	[Fact]
	public void TurningTheSwitchOnWritesWhatTheZoomFixWrites()
	{
		var value = CameraBinding.Stock with { ZoomsRightOut = true, MaxDistance = 65535, SnapsBack = false };

		Assert.Equal(ZoomFix, value.ToValue());
	}

	[Fact]
	public void TurningItOffAgainLeavesTheShippedBinding()
	{
		var off = CameraBinding.Read(ZoomFix) with { ZoomsRightOut = false, SnapsBack = true };

		// The binding is the shipped one again, even though the distance that was picked is still remembered
		// for the next time the switch goes on.
		Assert.Equal(CameraBinding.Stock.ToValue(), off.ToValue());
		Assert.Equal(65535, off.MaxDistance);
		Assert.DoesNotContain("MaxZoomingDist", off.ToValue(), StringComparison.Ordinal);
	}

	[Fact]
	public void SomebodyElsesCommandOnTheButtonSurvives()
	{
		var mine = CameraBinding.Read("CameraRotationModeOn | CameraRotationModeOff | ShowMyThing | set Engine.LineagePlayerController MaxZoomingDist 900");

		Assert.True(mine.ZoomsRightOut);
		Assert.Equal(900, mine.MaxDistance);
		Assert.Equal("ShowMyThing", Assert.Single(mine.Extras));
		Assert.Contains("ShowMyThing", (mine with { ZoomsRightOut = false }).ToValue(), StringComparison.Ordinal);
	}

	[Fact]
	public void AMissingBindingCountsAsTheShippedOne()
	{
		Assert.Equal(CameraBinding.Stock, CameraBinding.Read(null));
		Assert.Equal(CameraBinding.Stock, CameraBinding.Read("   "));
	}

	[Fact]
	public void TheDistanceStaysWithinWhatTheClientAccepts()
	{
		var tooFar = CameraBinding.Stock with { ZoomsRightOut = true, MaxDistance = 999_999 };
		Assert.Contains($"MaxZoomingDist {CameraBinding.HighestMaxDistance}", tooFar.ToValue(), StringComparison.Ordinal);

		var tooNear = CameraBinding.Stock with { ZoomsRightOut = true, MaxDistance = 1 };
		Assert.Contains($"MaxZoomingDist {CameraBinding.LowestMaxDistance}", tooNear.ToValue(), StringComparison.Ordinal);
	}
}
