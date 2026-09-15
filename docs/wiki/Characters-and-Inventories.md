# Characters and inventories

The **Characters** tab changes the adena and items of **player characters** in your world. The simulated players that populate the
world are never listed.

![The Characters tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/characters.png)

## Before you start

- **Start your world from the launcher.** The world's database only runs while the world does. Without it, the tab says *Could not reach
  the world's database…*; start the world, then click **↻ Refresh**.
- **Log the character out** to change anything. A logged-in character's inventory lives in the server's memory, and the server would
  overwrite any change when the character logs out.

## The character list

Each character shows its level, account, **inventory slots used** of its limit, and its adena. Search by character or account name.

- **Adena:** type the new amount and click **Apply**. The allowed range comes from your world's **Maximum adena** setting
  (`Player.ini › MaxAdena`); 0 removes the adena.
- **Inventory…** opens the inventory editor.

Every change is backed up first (see [Backups and restore](Backups-and-Restore)).

## The inventory editor

![An inventory with a waiting delivery and the Add items panel](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/inventory.png)

The header shows slots used, slots used once waiting deliveries arrive, and which setting the limit comes from.

- **In the inventory:** everything the character carries, with equipped items marked. Stacks such as adena, arrows or potions have a
  count box and **Set count**. Every item has **Remove**, which asks first.
- **Waiting for the server to deliver:** items you've added that haven't arrived yet, each with **Cancel delivery**.

## Adding items: read this

The running server gives every item a unique ID from its own memory. If a program wrote a new item straight into the database while the
server runs, the server could later give that same ID to another item. So **this program never writes new items into the database.**
Instead, new items are **queued for the server to deliver**:

1. Search the full item list (over 9,000 items, including L2Everdream's custom ones) by name or ID and pick one.
2. Enter an amount, and for weapons and armour an enchant level.
3. A line tells you exactly what will happen and how many inventory slots it needs. If it can't be done, it says why instead.
4. Click the gold button. The item goes onto the delivery queue and appears under *Waiting for the server to deliver*.
5. The next time the character is **logged in**, the server adds the item at its next check. That's every 30 seconds as shipped
   (**Check for new mail every**, `Custom/CustomMailManager.ini › DatabaseQueryDelay`).

**Delivery must be switched on.** The server only delivers queued items while **Deliver items queued from the database**
(`Custom/CustomMailManager.ini › CustomMailManagerEnabled`) is **On**, and it reads that setting when the world starts. It's **off** as
L2Everdream ships. The inventory screen tells you its current state and has **Open that setting →**. Turn it on, save, then restart your
world before logging in.

**Adding to a stack you already have is instant.** If the character already carries that stackable item (adena, arrows, potions…), the
program adds to the existing stack straight away. No new slot and no delivery needed.

## Protection against broken characters

- **Inventory limit, counted exactly as the server counts it:** equipped and carried items plus waiting deliveries. A stackable item only
  needs a new slot if the character doesn't carry it yet; weapons and armour always take one slot each. The limit is:
  - Game Masters (access levels marked as GM in `AccessLevels.xml`): **Game Master inventory slots** (`MaximumSlotsForGMPlayer`)
  - Dwarves: **Dwarf inventory slots** (`MaximumSlotsForDwarf`)
  - everyone else: **inventory slots** (`MaximumSlotsForNoDwarf`)

  Skills that expand the inventory can add a little in game, so the program's limit is slightly cautious. If an addition doesn't fit,
  you get *Not enough room: … Remove items, or raise the inventory limit in the Server tab.*
- **Stack limits:** one stack can't exceed 2,147,483,647, and adena can't exceed your world's maximum adena.
- **No race with logging in:** every change locks the character's row and checks it is still logged out in the same step. If the item
  changed since you opened the inventory, you're asked to refresh and try again.

## Undoing a character change

Every adena change, item change or removal, and queued or cancelled delivery is recorded in a change backup, with the database rows as
they were. Restore it from the **Backups** tab while your world is running and the character is logged out. A removed item comes back
with its original identity only if the world hasn't been restarted since. See [Backups and restore](Backups-and-Restore).
