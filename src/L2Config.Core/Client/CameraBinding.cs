using System.Globalization;

namespace L2Config.Core.Client;

/// <summary>
/// The right mouse button's binding in the client's `user.ini`, which is where how far the camera zooms out really
/// lives.
/// <para>
/// `[Engine.LineagePlayerController] MaxZoomingDist` looks like the setting for it, but the client writes `user.ini`
/// back out when it closes and puts that value to 250 again, so editing it does nothing that lasts. What does last is a
/// command on the binding: the client runs `RightMouse` on every right-click, so `set Engine.LineagePlayerController
/// MaxZoomingDist 65535` there re-applies the limit all through the session. That is what the community "zoom fix" does,
/// and it is why this is offered as a switch rather than a number in a file.
/// </para>
/// <para>
/// The stock binding also carries `FixedDefaultCamera OnRelease MaxPressedTime=200.0` — tapping right-click snaps the
/// camera back behind you. The zoom fix drops it, which surprises people, so it is kept as a switch of its own. Anything
/// else someone has put on this binding is left alone and written back in the order it was found.
/// </para>
/// </summary>
public sealed record CameraBinding(bool ZoomsRightOut, int MaxDistance, int MinDistance, bool SnapsBack)
{
    public const string Section = "Engine.Input";
    public const string Key = "RightMouse";

    /// <summary>What the client ships: as far out as the camera goes without the fix.</summary>
    public const int StockMaxDistance = 250;

    /// <summary>What the community fix uses — far enough that nothing in the game reaches it.</summary>
    public const int WideMaxDistance = 65535;

    public const int StockMinDistance = -200;
    public const int LowestMaxDistance = 250;
    public const int HighestMaxDistance = 65535;

    private const string Rotate = "CameraRotationModeOn";
    private const string RotateOff = "CameraRotationModeOff";
    private const string SnapBack = "FixedDefaultCamera OnRelease MaxPressedTime=200.0";
    private const string Controller = "Engine.LineagePlayerController";

    /// <summary>The binding as the client ships it.</summary>
    public static CameraBinding Stock { get; } = new(false, StockMaxDistance, StockMinDistance, true);

    /// <summary>Anything on the binding this app does not recognise, kept so a player's own commands survive.</summary>
    public IReadOnlyList<string> Extras { get; init; } = [];

    /// <summary>Reads the switches out of a `RightMouse=` value. A missing or empty line counts as the stock binding.</summary>
    public static CameraBinding Read(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Stock;
        }

        var zoomsOut = false;
        var snapsBack = false;
        var max = StockMaxDistance;
        var min = StockMinDistance;
        var extras = new List<string>();

        foreach (var part in value.Split('|'))
        {
            var command = part.Trim();
            if (command.Length == 0)
            {
                continue;
            }
            if (command.Equals(Rotate, StringComparison.OrdinalIgnoreCase)
                || command.Equals(RotateOff, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            if (command.StartsWith("FixedDefaultCamera", StringComparison.OrdinalIgnoreCase))
            {
                snapsBack = true;
                continue;
            }
            if (Distance(command, "MaxZoomingDist") is { } maximum)
            {
                zoomsOut = true;
                max = maximum;
                continue;
            }
            if (Distance(command, "MinZoomingDist") is { } minimum)
            {
                min = minimum;
                continue;
            }
            extras.Add(command);
        }
        return new CameraBinding(zoomsOut, max, min, snapsBack) { Extras = extras };
    }

    /// <summary>The number out of `set Engine.LineagePlayerController MaxZoomingDist 65535`, or null for anything else.</summary>
    private static int? Distance(string command, string setting)
    {
        var words = command.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return words is ["set", var controller, var name, var number]
            && controller.Equals(Controller, StringComparison.OrdinalIgnoreCase)
            && name.Equals(setting, StringComparison.OrdinalIgnoreCase)
            && int.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : null;
    }

    /// <summary>The `RightMouse=` value these switches make, with the rotate commands the button exists for first.</summary>
    public string ToValue()
    {
        var parts = new List<string> { Rotate, RotateOff };
        if (SnapsBack)
        {
            parts.Add(SnapBack);
        }
        if (ZoomsRightOut)
        {
            var max = Math.Clamp(MaxDistance, LowestMaxDistance, HighestMaxDistance);
            parts.Add($"set {Controller} MaxZoomingDist {max.ToString(CultureInfo.InvariantCulture)}");
            parts.Add($"set {Controller} MinZoomingDist {MinDistance.ToString(CultureInfo.InvariantCulture)}");
        }
        parts.AddRange(Extras);
        return string.Join(" | ", parts);
    }
}

/// <summary>
/// The three states of the right-click binding the app offers, as short words a settings file can hold. The binding
/// itself is a line of commands, which is no use as a value in a list of choices, so the editor stores one of these and
/// <see cref="L2Config.Core.Storage.ClientIniFile"/> turns it into the real line (keeping anything else on the button).
/// </summary>
public static class CameraChoices
{
	/// <summary>The binding as the client ships it: tap right-click snaps the camera back, the camera stops at 250.</summary>
	public const string Shipped = "shipped";

	/// <summary>The camera zooms right out, and tapping right-click still snaps it back.</summary>
	public const string ZoomOut = "zoom-out";

	/// <summary>What the community zoom fix writes: zooms right out, and no snap-back.</summary>
	public const string ZoomOutNoSnap = "zoom-out-no-snap";

	/// <summary>Something else is bound to right-click. Shown so a player's own binding is never called invalid.</summary>
	public const string Custom = "custom";

	/// <summary>Which of the three a binding already is, or <see cref="Custom"/> for anything else.</summary>
	public static string Of(CameraBinding binding) => (binding.ZoomsRightOut, binding.SnapsBack) switch
	{
		(false, true) => Shipped,
		(true, true) => ZoomOut,
		(true, false) => ZoomOutNoSnap,
		_ => Custom,
	};

	/// <summary>The binding a choice makes of the one already there. <see cref="Custom"/> changes nothing.</summary>
	public static CameraBinding Apply(string? choice, CameraBinding current) => choice switch
	{
		Shipped => current with { ZoomsRightOut = false, SnapsBack = true },
		ZoomOut => current with { ZoomsRightOut = true, SnapsBack = true, MaxDistance = Widest(current) },
		ZoomOutNoSnap => current with { ZoomsRightOut = true, SnapsBack = false, MaxDistance = Widest(current) },
		_ => current,
	};

	/// <summary>A distance already chosen is kept; otherwise the one the community fix uses.</summary>
	private static int Widest(CameraBinding current) =>
		current.ZoomsRightOut && current.MaxDistance > CameraBinding.StockMaxDistance
			? current.MaxDistance
			: CameraBinding.WideMaxDistance;
}
