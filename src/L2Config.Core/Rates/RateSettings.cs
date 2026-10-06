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
	double HerbChance,
	double HerbAmount,
	double SpoilChance,
	double SpoilAmount,
	double RaidChance,
	double RaidAmount)
{
	/// <summary>
	/// `DropChanceMultiplierByItemId` exactly as the file holds it (`57,15;6656,1`). Every item listed here uses its own
	/// chance instead of the general one. Kept as the list rather than a map so two sets of rates still compare as values;
	/// the lists are a handful of entries, and only one monster is previewed at a time.
	/// </summary>
	public string ChanceByItemIdList { get; init; } = "";

	/// <summary>`DropAmountMultiplierByItemId`, the same way. L2Everdream lists the epic boss jewels here at 1.</summary>
	public string AmountByItemIdList { get; init; } = "";

	/// <summary>Lineage 2 as it shipped: the numbers a drop table site lists.</summary>
	public static RateSettings Retail { get; } = new(1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

	public bool IsRetail => Equals(Retail);

	/// <summary>What a rate plan would put in the files, for a world whose own by-item-id lists are not to hand.</summary>
	public static RateSettings FromPlan(RateOptions options) => RatePlan.Build(options).Result;

	/// <summary>
	/// What a world is set to right now. <paramref name="value"/> is asked for `Rates.ini` keys and may return null for
	/// anything missing, which counts as 1 (or, for the by-item-id lists, as "nothing listed").
	/// </summary>
	public static RateSettings Read(Func<string, string?> value)
	{
		double Number(string key, double fallback = 1) =>
			double.TryParse(value(key)?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) && parsed >= 0
				? parsed
				: fallback;

		return new RateSettings(
			Number("RateXp"), Number("RateSp"),
			Number("DeathDropChanceMultiplier"), Number("DeathDropAmountMultiplier"),
			Number("HerbDropChanceMultiplier"), Number("HerbDropAmountMultiplier"),
			Number("SpoilDropChanceMultiplier"), Number("SpoilDropAmountMultiplier"),
			Number("RaidDropChanceMultiplier"), Number("RaidDropAmountMultiplier"))
		{
			ChanceByItemIdList = value("DropChanceMultiplierByItemId")?.Trim() ?? "",
			AmountByItemIdList = value("DropAmountMultiplierByItemId")?.Trim() ?? "",
		};
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

	/// <summary>Adena's own chance, or the general one when it is not listed by id.</summary>
	public double AdenaChance => ByItemId(ChanceByItemIdList, RatePlan.AdenaItemId) ?? DropChance;

	public double AdenaAmount => ByItemId(AmountByItemIdList, RatePlan.AdenaItemId) ?? DropAmount;

	/// <summary>
	/// The pair that applies to one item of one monster, after the server's else-if chain. Spoiling has its own two
	/// multipliers and never looks at the by-item-id lists; everything else takes its own rate when it is listed, and
	/// chance and amount are resolved separately (the epic jewels have an amount of their own but no chance).
	/// </summary>
	public (double Chance, double Amount) For(int itemId, bool isHerb, bool isSpoil, bool isRaid)
	{
		if (isSpoil)
		{
			return (SpoilChance, SpoilAmount);
		}
		var (chance, amount) = isHerb ? (HerbChance, HerbAmount)
			: isRaid ? (RaidChance, RaidAmount)
			: (DropChance, DropAmount);
		return (ByItemId(ChanceByItemIdList, itemId) ?? chance, ByItemId(AmountByItemIdList, itemId) ?? amount);
	}

	/// <summary>How many times retail this world's ordinary drops are overall, for a one-line summary.</summary>
	public double DropsOverall => Math.Round(DropChance * DropAmount, 2);
}
