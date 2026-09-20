namespace L2Config.Core.Rates;

/// <summary>One item of a monster's drop list, before and after a rate plan.</summary>
public sealed record DropRow(
	int ItemId,
	string ItemName,
	double ChanceBefore,
	double ChanceAfter,
	double AmountBefore,
	double AmountAfter,
	bool IsHerb,
	bool IsAdena,
	bool IsSpoil)
{
	/// <summary>Average number of this item per kill (chance × average amount), the number that actually matters.</summary>
	public double PerKillBefore => ChanceBefore / 100 * AmountBefore;

	public double PerKillAfter => ChanceAfter / 100 * AmountAfter;

	/// <summary>How much better this item really got. 1 = no change.</summary>
	public double RealMultiplier => PerKillBefore <= 0 ? 0 : PerKillAfter / PerKillBefore;

	/// <summary>The group already drops every time, so extra chance is wasted on it.</summary>
	public bool ChanceIsFull => ChanceAfter >= 100;

	/// <summary>Left as it is by the plan (herbs).</summary>
	public bool Unchanged => Math.Abs(PerKillAfter - PerKillBefore) < 1e-9;
}

/// <summary>What a rate plan does to one monster: experience, drops and spoil, with the server's own arithmetic.</summary>
public sealed record MonsterPreview(
	Monster Monster,
	RateOptions Options,
	double ExpBefore,
	double ExpAfter,
	double SpBefore,
	double SpAfter,
	IReadOnlyList<DropRow> Drops,
	IReadOnlyList<DropRow> Spoil,
	double AdenaBefore,
	double AdenaAfter,
	int GroupsAtFullChance,
	int MaxDifferentItems)
{
	public bool HasDrops => Drops.Count > 0 || Spoil.Count > 0;

	/// <summary>The monster drops fewer different items than its list suggests, because the server caps it.</summary>
	public bool HitsItemLimit => GroupsAtFullChance > MaxDifferentItems;
}

/// <summary>
/// Works out what a rate plan means for a single monster, exactly the way the game server does it:
/// each drop group rolls once against <c>groupChance × chanceMultiplier</c>, then one item in the group is chosen by
/// weight and its count is <c>random(min,max) × amountMultiplier</c>. Per-item multipliers (adena) replace the general
/// ones rather than adding to them, and herbs use their own rates, which a plan leaves alone.
/// </summary>
public static class DropPreview
{
	public static MonsterPreview For(Monster monster, MonsterCatalog catalog, RateOptions options, int maxDifferentItems = 2)
	{
		var chance = monster.IsRaid ? options.RaidChanceMultiplier : options.ChanceMultiplier;
		var amount = monster.IsRaid ? options.RaidAmountMultiplier : options.AmountMultiplier;

		var drops = Rows(monster.Drops, catalog, chance, amount, options, isSpoil: false).ToList();
		var spoil = Rows(monster.Spoil, catalog, options.ChanceMultiplier, options.AmountMultiplier, options, isSpoil: true).ToList();
		var adenaBefore = drops.Where(d => d.IsAdena).Sum(d => d.PerKillBefore);
		var adenaAfter = drops.Where(d => d.IsAdena).Sum(d => d.PerKillAfter);
		var full = monster.Drops.Count(g => Math.Min(100, g.Chance * (catalog.Herbs.Count > 0 && g.Items.All(i => catalog.Herbs.Contains(i.ItemId)) ? 1 : chance)) >= 100);

		return new MonsterPreview(
			monster, options,
			monster.Exp, monster.Exp * options.Rate,
			monster.Sp, monster.Sp * options.Rate,
			drops, spoil, adenaBefore, adenaAfter, full, maxDifferentItems);
	}

	private static IEnumerable<DropRow> Rows(IReadOnlyList<DropGroup> groups, MonsterCatalog catalog, double chanceMultiplier,
		double amountMultiplier, RateOptions options, bool isSpoil)
	{
		foreach (var group in groups)
		{
			var weights = group.Items.Sum(i => i.Weight);
			foreach (var item in group.Items)
			{
				var isHerb = catalog.Herbs.Contains(item.ItemId);
				var isAdena = item.ItemId == RatePlan.AdenaItemId;

				// Herbs keep their own (untouched) multipliers; adena uses the per-item amount rate the plan writes.
				var chance = isHerb ? 1 : chanceMultiplier;
				var amount = isHerb ? 1 : isAdena ? options.AmountMultiplier : amountMultiplier;

				var share = weights <= 0 ? 1 : item.Weight / weights;
				var chanceBefore = Math.Min(100, group.Chance) * share;
				var chanceAfter = Math.Min(100, group.Chance * chance) * share;
				var average = (item.Min + item.Max) / 2.0;

				yield return new DropRow(
					item.ItemId, catalog.ItemName(item.ItemId),
					chanceBefore, chanceAfter,
					average, average * amount,
					isHerb, isAdena, isSpoil);
			}
		}
	}
}
