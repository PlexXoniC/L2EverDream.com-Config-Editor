# GM shop

The app's **GM Shop** tab writes this shop into a world by itself — this folder is for the other way round: *Save a copy
of the files…* on that tab puts the same files in a folder of your choosing, and this explains what they are and how to
put them in a world by hand.

Nothing is kept here as ready-made XML on purpose. The files are built from **your** world: the items it knows about, the
prices it puts on them, the armour sets it defines, and where its own spawn files put each gatekeeper. A copy checked in
here would be a snapshot of one machine's world and would go stale with the next L2Everdream release.

## What a copy contains

Everything goes under `game\data` in the world's install folder, keeping the same layout:

| File | What it is |
|---|---|
| `stats\npcs\custom\L2EverdreamConfig\gm-shop.xml` | The shopkeeper: npc id 60000, a `Merchant`, wearing the look of an npc your client already draws |
| `html\merchant\60000.htm` | What clicking the shopkeeper shows — one button per page, and Sell |
| `buylists\custom\6000001.xml` … | The buy windows: an item and a price per line |
| `multisell\custom\4000001.xml` … | The exchange windows, which can hand over a whole armour set for one price |
| `spawns\L2EverdreamConfig\gm-shops.xml` | Where the shopkeepers stand. `enabled="false"` on the first line takes every one of them away |

Copy the folders over `%LOCALAPPDATA%\L2Everdream\game\data`, then restart the world from the launcher: all of this is
read at start-up.

## What makes it safe

- **Every file is a new one.** None of the files L2Everdream ships is edited. The launcher's update only captures and
  restores files that are in its own manifest, and it deletes nothing, so files like these simply survive an update.
- **The folder names are ours.** `stats\npcs\custom` is read recursively, so a folder of our own can never collide with a
  file a future release adds. The buy and exchange lists use a numbered block (6000001+, 4000001+) that no release uses.
- **Three settings have to be on**, all of them `True` as L2Everdream ships: `CustomNpcData`, `CustomBuyListLoad` and
  `CustomMultisellLoad` in `General.ini`. The tab says so if one of them is off.

## Taking it out again

Delete the five kinds of file above and restart the world. The tab's **Take it back out** does the same thing, backing
each file up first. Nothing else in the world is touched either way.

## Not the same as `..\server-scripts`

Those are Java that a world compiles and runs, and the app never touches them. This shop is data — XML and one HTML
page — and the app does write it, from the GM Shop tab.
