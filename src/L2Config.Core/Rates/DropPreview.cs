namespace L2Config.Core.Rates;

/// <summary>One item of a monster's drop list, under two sets of rates.</summary>
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

	/// <summary>The same either way (herbs, or anything neither set of rates touches).</summary>
	public bool Unchanged => Math.Abs(PerKillAfter - PerKillBefore) < 1e-9;
}

/// <summary>What one monster gives under two sets of rates: experience, drops and spoil, with the server's own arithmetic.</summary>
public sealed record MonsterPreview(
	Monster Monster,
	RateSettings Before,
	RateSettings After,
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

	/// <summary>Nothing moves between the two sets of rates (they are the same, or nothing this monster gives is affected).</summary>
	public bool IsSameBothWays => Math.Abs(ExpAfter - ExpBefore) < 1e-9 && Drops.Concat(Spoil).All(d => d.Unchanged);
}

/// <summary>
/// Works out what a set of rates means for a single monster, exactly the way the game server does it:
/// each drop group rolls once against <c>groupChance × chanceMultiplier</c>, then one item in the group is chosen by
/// weight and its count is <c>random(min,max) × amountMultiplier</c>. Per-item multipliers (adena) replace the general
/// ones rather than adding to them, and herbs, spoil and raid bosses have their own.
/// </summary>
public static class DropPreview
{
	/// <summary>Retail → what a rate plan would give.</summary>
	public static MonsterPreview For(Monster monster, MonsterCatalog catalog, RateOptions options, int maxDifferentItems = 2) =>
		Between(monster, catalog, RateSettings.Retail, RateSettings.FromPlan(options), maxDifferentItems);

	/// <summary>Any two sets of rates: retail → this world now, this world now → a planned rate, and so on.</summary>
	public static MonsterPreview Between(Monster monster, MonsterCatalog catalog, RateSettings before, RateSettings after,
		int maxDifferentItems = 2)
	{
		var drops = Rows(monster, monster.Drops, catalog, before, after, isSpoil: false).ToList();
		var spoil = Rows(monster, monster.Spoil, catalog, before, after, isSpoil: true).ToList();
		var adenaBefore = drops.Where(d => d.IsAdena).Sum(d => d.PerKillBefore);
		var adenaAfter = drops.Where(d => d.IsAdena).Sum(d => d.PerKillAfter);
		var full = monster.Drops.Count(g => GroupChance(g, monster, catalog, after) >= 100);

		return new MonsterPreview(
			monster, before, after,
			monster.Exp * before.Xp, monster.Exp * after.Xp,
			monster.Sp * before.Sp, monster.Sp * after.Sp,
			drops, spoil, adenaBefore, adenaAfter, full, maxDifferentItems);
	}

	private static double GroupChance(DropGroup group, Monster monster, MonsterCatalog catalog, RateSettings rates)
	{
		var first = group.Items.Count > 0 ? group.Items[0] : null;
		var (chance, _) = rates.For(
			isHerb: first is not null && group.Items.All(i => catalog.Herbs.Contains(i.ItemId)),
			isAdena: first?.ItemId == RatePlan.AdenaItemId,
			isSpoil: false,
			isRaid: monster.IsRaid);
		return Math.Min(100, group.Chance * chance);
	}

	private static IEnumerable<DropRow> Rows(Monster monster, IReadOnlyList<DropGroup> groups, MonsterCatalog catalog,
		RateSettings before, RateSettings after, bool isSpoil)
	{
		foreach (var group in groups)
		{
			var weights = group.Items.Sum(i => i.Weight);
			foreach (var item in group.Items)
			{
				var isHerb = catalog.Herbs.Contains(item.ItemId);
				var isAdena = item.ItemId == RatePlan.AdenaItemId;
				var (chanceBefore, amountBefore) = before.For(isHerb, isAdena, isSpoil, monster.IsRaid);
				var (chanceAfter, amountAfter) = after.For(isHerb, isAdena, isSpoil, monster.IsRaid);

				var share = weights <= 0 ? 1 : item.Weight / weights;
				var average = (item.Min + item.Max) / 2.0;

				yield return new DropRow(
					item.ItemId, catalog.ItemName(item.ItemId),
					Math.Min(100, group.Chance * chanceBefore) * share,
					Math.Min(100, group.Chance * chanceAfter) * share,
					average * amountBefore, average * amountAfter,
					isHerb, isAdena, isSpoil);
			}
		}
	}
}
