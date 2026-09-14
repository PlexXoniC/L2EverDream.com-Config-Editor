namespace L2Config.Core.Characters;

/// <summary>
/// The inventory size limit exactly as the game server computes it (Player.getInventoryLimit /
/// PlayerInventory.validateCapacity): every item row in the inventory, equipped or not, takes one slot; a stackable item
/// only needs a slot when the character does not already carry it. Skills that expand the inventory add to the limit,
/// so the base limit used here is the safe lower bound.
/// </summary>
public sealed record InventoryLimits(int NoDwarf, int Dwarf, int GameMaster, IReadOnlySet<int> GameMasterAccessLevels)
{
	public const int DwarfRace = 4;

	/// <summary>Largest count the client and the server's item code accept for one stack.</summary>
	public const long MaxStackCount = int.MaxValue;

	public int For(PlayerCharacter character) =>
		GameMasterAccessLevels.Contains(character.AccessLevel) ? GameMaster
		: character.Race == DwarfRace ? Dwarf
		: NoDwarf;

	public string DescribeFor(PlayerCharacter character) =>
		GameMasterAccessLevels.Contains(character.AccessLevel) ? "Game Master inventory slots (Player.ini › MaximumSlotsForGMPlayer)"
		: character.Race == DwarfRace ? "Dwarf inventory slots (Player.ini › MaximumSlotsForDwarf)"
		: "inventory slots (Player.ini › MaximumSlotsForNoDwarf)";
}

public static class InventorySlots
{
	/// <summary>Slots the character's inventory will use once every pending delivery has arrived.</summary>
	public static int UsedIncludingPending(IReadOnlyCollection<InventoryItem> inventory, IEnumerable<DeliveryItem> pending, ItemCatalog catalog)
	{
		var carried = inventory.Select(i => i.ItemId).ToHashSet();
		var used = inventory.Count;
		foreach (var delivery in pending)
		{
			used += SlotsFor(delivery.ItemId, delivery.Count, carried, catalog);
			carried.Add(delivery.ItemId);
		}
		return used;
	}

	/// <summary>Slots needed to add <paramref name="count"/> of an item, given what the character carries (or will).</summary>
	public static int SlotsFor(int itemId, long count, ISet<int> carriedItemIds, ItemCatalog catalog)
	{
		var template = catalog.Find(itemId);
		if (template?.Stackable == true)
		{
			return carriedItemIds.Contains(itemId) ? 0 : 1;
		}
		// Unknown items are treated as non-stackable: the cautious answer.
		return (int)Math.Min(count, int.MaxValue);
	}
}
