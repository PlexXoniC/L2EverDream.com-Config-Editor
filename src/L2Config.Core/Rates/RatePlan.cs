using System.Globalization;

namespace L2Config.Core.Rates;

/// <summary>
/// What "I want a 5x world" means in real settings.
/// <para>
/// The server rolls each drop group once: <c>random(0–100) &lt; groupChance × chanceMultiplier</c>, and then gives
/// <c>random(min,max) × amountMultiplier</c> of one item in the group. So chance stops helping once a group reaches 100%,
/// while amount keeps scaling. A plan therefore splits the rate between the two: <see cref="Delivery"/> 0 puts as much as
/// possible into chance ("drops more often", capped so nothing is wasted), 1 puts it all into amount ("bigger stacks").
/// Multiplied together they always come to the rate the player asked for.
/// </para>
/// </summary>
public sealed record RateOptions(double Rate, double Delivery = 0.5)
{
	/// <summary>Chance is never multiplied by more than this: beyond it, common drops sit at 100% and the rest is wasted.</summary>
	public const double ChanceCap = 2.0;

	public const double MinRate = 0.1;
	public const double MaxRate = 100;

	/// <summary>Raid bosses move less than normal monsters, so a 20x world is not a 20x epic drop world.</summary>
	public double RaidRate => Math.Round(1 + ((Rate - 1) / 3), 2);

	public double ChanceMultiplier => Math.Round(1 + ((Math.Min(Rate, ChanceCap) - 1) * (1 - Delivery)), 2);

	public double AmountMultiplier => Math.Round(Rate / ChanceMultiplier, 3);

	public double RaidChanceMultiplier => Math.Round(1 + ((Math.Min(RaidRate, ChanceCap) - 1) * (1 - Delivery)), 2);

	public double RaidAmountMultiplier => Math.Round(RaidRate / RaidChanceMultiplier, 3);

	public static IReadOnlyList<double> Presets => [1, 3, 5, 15, 20];
}

/// <summary>One setting a plan changes.</summary>
public sealed record RateChange(string File, string Key, string NewValue, string Why)
{
	public string Location => $"{File} › {Key}";
}

/// <summary>The settings a rate plan writes, and the ones it deliberately leaves alone.</summary>
public sealed record RatePlan(RateOptions Options, IReadOnlyList<RateChange> Changes, IReadOnlyList<string> Untouched)
{
	public const int AdenaItemId = 57;

	/// <summary>Builds the plan. Nothing is written here: the app shows this, then saves it like any other change.</summary>
	public static RatePlan Build(RateOptions options)
	{
		var rate = Text(options.Rate);
		var chance = Text(options.ChanceMultiplier);
		var amount = Text(options.AmountMultiplier);
		var raidChance = Text(options.RaidChanceMultiplier);
		var raidAmount = Text(options.RaidAmountMultiplier);

		var changes = new List<RateChange>
		{
			new("Rates.ini", "RateXp", rate, "Experience from monsters."),
			new("Rates.ini", "RateSp", rate, "Skill points from monsters."),
			new("Rates.ini", "DeathDropChanceMultiplier", chance, "How often monsters drop something."),
			new("Rates.ini", "DeathDropAmountMultiplier", amount, "How much they drop each time."),
			new("Rates.ini", "DropAmountMultiplierByItemId", $"{AdenaItemId},{amount}",
				"Adena is set on its own here, because a per-item rate replaces the general one instead of adding to it. " +
				"L2Everdream ships this as 57,1, which is why raising the drop amount alone never changed adena."),
			new("Rates.ini", "SpoilDropChanceMultiplier", chance, "How often spoiling gives something."),
			new("Rates.ini", "SpoilDropAmountMultiplier", amount, "How much spoiling gives."),
			new("Rates.ini", "RaidDropChanceMultiplier", raidChance, $"Raid boss drops move less than normal drops ({Text(options.RaidRate)}x)."),
			new("Rates.ini", "RaidDropAmountMultiplier", raidAmount, "Raid boss drop amount."),
			new("Rates.ini", "RateQuestRewardXP", rate, "Experience from quests."),
			new("Rates.ini", "RateQuestRewardSP", rate, "Skill points from quests."),
			new("Rates.ini", "RateQuestRewardAdena", rate, "Adena from quests."),
			new("Rates.ini", "RateQuestReward", rate, "Items from quests."),
			new("Rates.ini", "QuestItemDropAmountMultiplier", rate, "Quest items monsters drop."),
			new("Rates.ini", "PetXpRate", rate, "Experience for pets, so they keep up with you."),
		};

		var untouched = new List<string>
		{
			"Herb drops — at a high rate herbs would bury you; change them yourself in Rates & Rewards if you want.",
			"Vitality — it already multiplies experience on top of this.",
			"Premium bonuses — they are meant to be a bonus over your world's rate.",
			"The extra for being in a party — it multiplies the party bonus, not the experience itself, so it is left alone.",
			"Level-difference penalties, and the most-items-per-monster limit — those shape drops, and the preview shows their effect.",
			"Manor, fishing, lottery and other side rewards.",
		};
		return new RatePlan(options, changes, untouched);
	}

	/// <summary>Reads a plan back from the values in the files, so the app can show which preset a world is on.</summary>
	public static double? RateOf(string? rateXp)
	{
		return double.TryParse(rateXp?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && value > 0 ? value : null;
	}

	public static string Text(double value) =>
		value == Math.Floor(value) && Math.Abs(value) < 1e9
			? ((long)value).ToString(CultureInfo.InvariantCulture)
			: value.ToString("0.###", CultureInfo.InvariantCulture);
}
