using System.Globalization;

namespace L2Config.Core.Rates;

/// <summary>
/// The drop and experience multipliers that are actually in force — read from a world's own files, taken from a rate
/// plan, or retail (everything at 1). The server resolves each drop through an else-if chain (by item id, herb, raid,
/// then the general death rates), so a per-item rate <b>replaces</b> the general one; that resolution is done here once,
/// and <see cref="DropPreview"/> just picks the pair it needs.
/// </summary>
public sealed record RateSettings(
	double Xp,
	double Sp,
	double DropChance,
	double DropAmount,
	double AdenaChance,
	double AdenaAmount,
	double HerbChance,
	double HerbAmount,
	double SpoilChance,
	double SpoilAmount,
	double RaidChance,
	double RaidAmount)
{
	/// <summary>Lineage 2 as it shipped: the numbers a drop table site lists.</summary>
	public static RateSettings Retail { get; } = new(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

	public bool IsRetail => Equals(Retail);

	/// <summary>What a rate plan would put in the files.</summary>
	public static RateSettings FromPlan(RateOptions options) => new(
		options.Rate, options.Rate,
		options.ChanceMultiplier, options.AmountMultiplier,
		// The plan writes adena's amount by item id and leaves its chance on the general rate.
		options.ChanceMultiplier, options.AmountMultiplier,
		1, 1,
		options.ChanceMultiplier, options.AmountMultiplier,
		options.RaidChanceMultiplier, options.RaidAmountMultiplier);

	/// <summary>
	/// What a world is set to right now. <paramref name="value"/> is asked for `Rates.ini` keys and may return null for
	/// anything missing, which counts as 1 (or, for the by-item-id lists, as "not listed").
	/// </summary>
	public static RateSettings Read(Func<string, string?> value)
	{
		double Number(string key, double fallback = 1) =>
			double.TryParse(value(key)?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) && parsed >= 0
				? parsed
				: fallback;

		var dropChance = Number("DeathDropChanceMultiplier");
		var dropAmount = Number("DeathDropAmountMultiplier");
		return new RateSettings(
			Number("RateXp"), Number("RateSp"),
			dropChance, dropAmount,
			ByItemId(value("DropChanceMultiplierByItemId"), RatePlan.AdenaItemId) ?? dropChance,
			ByItemId(value("DropAmountMultiplierByItemId"), RatePlan.AdenaItemId) ?? dropAmount,
			Number("HerbDropChanceMultiplier"), Number("HerbDropAmountMultiplier"),
			Number("SpoilDropChanceMultiplier"), Number("SpoilDropAmountMultiplier"),
			Number("RaidDropChanceMultiplier"), Number("RaidDropAmountMultiplier"));
	}

	/// <summary>One item's multiplier out of a `57,1;6656,2` list, or null when the item is not listed.</summary>
	public static double? ByItemId(string? list, int itemId)
	{
		foreach (var entry in (list ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
		{
			var parts = entry.Split(',', StringSplitOptions.TrimEntries);
			if (parts.Length == 2
				&& int.TryParse(parts[0], out var id) && id == itemId
				&& double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var multiplier)
				&& multiplier >= 0)
			{
				return multiplier;
			}
		}
		return null;
	}

	/// <summary>The pair that applies to one item of one monster, after the server's else-if chain.</summary>
	public (double Chance, double Amount) For(bool isHerb, bool isAdena, bool isSpoil, bool isRaid) =>
		isAdena ? (AdenaChance, AdenaAmount)
		: isSpoil ? (SpoilChance, SpoilAmount)
		: isHerb ? (HerbChance, HerbAmount)
		: isRaid ? (RaidChance, RaidAmount)
		: (DropChance, DropAmount);

	/// <summary>How many times retail this world's ordinary drops are overall, for a one-line summary.</summary>
	public double DropsOverall => Math.Round(DropChance * DropAmount, 2);
}
