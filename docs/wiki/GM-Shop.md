# GM shop

A shopkeeper standing beside the gatekeeper in every town, selling the things your own world knows about at the prices
your own world puts on them. Nothing is downloaded and nothing is invented: the items, the prices, the armour sets and
the place each shopkeeper stands all come out of your world's own files.

This is the one thing the config manager **adds** to a world rather than just changing a setting in it — so it is careful
about how, and it can take every bit of it back out again.

## Setting one up

1. Open the **GM Shop** tab.
2. Give the shopkeeper a name and a title, and pick which npc it looks like. Any npc your game client can already draw
   will do; `30001` is Lector, the merchant who stands in Giran.
3. Tick the towns you want one in. All 18 main towns are ticked to start with, and every other gatekeeper in the world is
   listed underneath in case you want one somewhere unusual.
4. Look through what it sells, and change anything you like (below).
5. **Write it to my world**, then **restart your world from the launcher**. Your world reads all of this when it starts,
   so nothing appears until it does.

## What it sells

One button per page, the way a merchant's window works in game:

| Page | What is on it |
|---|---|
| Weapons D to S | Every graded weapon your world knows about |
| Armour D to S | Every graded piece of armour |
| Jewellery D to S | Necklaces, earrings and rings |
| Supplies | Soulshots, spiritshots, potions and a Scroll of Escape |
| Armour sets | Each complete set, handed over in one purchase |

Every line shows the item's icon — read from your own game client — its price, and a tick. Untick anything you do not
want sold, and type over any price. A price you change is remembered, and the line says what the item is normally worth
so you can see how far you have moved it.

**Epic boss jewels are left off on purpose.** The Rings of Baium and Queen Ant and the rest are the rarest things in the
game, and L2Everdream deliberately keeps them out of reach of a fast world's drop rates. Nothing stops you adding them
back, but they start off the list.

## Where the shopkeepers stand

Each one is put a short step to the side of its town's gatekeeper, facing the same way. If one ends up in a wall, on a
step or inside the gatekeeper's own clickbox, nudge it east/west and north/south with the two boxes on that town's row,
write it again and restart. There is no way around walking over and looking — nobody can tell from the files alone.

## Switching it off

Untick **Stand in the towns below** and write again: the files stay where they are and no shopkeeper is spawned. Restart
your world and they are gone.

**Take it back out** deletes every file the app wrote for the shop. Each one is backed up first, the same as any other
change, and nothing else in your world is touched.

## Why an update will not break it

L2Everdream updates replace the files a release ships. Everything the shop uses is a **new** file, in the folders the
server keeps for custom content, so an update has nothing to replace and nothing to delete — it simply leaves them where
they are. The tab will tell you if it ever finds something different from what you set.

Three settings have to be on for the server to read those folders at all, and all three are on as L2Everdream ships:
`CustomNpcData`, `CustomBuyListLoad` and `CustomMultisellLoad`, all in `General.ini`. If one of them is off, the tab says
so rather than letting you write a shop that will never appear.

## A word about your economy

A shop that sells top-grade gear for adena changes what your world is for: drops and spoil stop mattering much, because
anything can be bought. That may be exactly what you want on a world you run for yourself and a few friends. If it is
not, a shop that only sells supplies and low grades is a couple of unticks away.

## If the tab says it cannot find your world

Everything the shop is made of comes out of your world's own game data, so the tab is only as good as the folder you pointed the app
at. If that folder is not really an L2Everdream install — the wrong one picked, or an install that did not finish — you will see:

> This folder does not look like an L2Everdream world, so there is nothing to build a shop from. Check it on the Server tab.

with a line for each part it could not read:

| Folder | What the shop needs it for |
|---|---|
| `game\data\stats\items` | The items a shop would sell, and what they are worth |
| `game\data\stats\npcs` | The gatekeepers a shopkeeper stands beside |
| `game\data\spawns` | The place each gatekeeper stands in |
| `game\data\stats\armorsets` | The armour sets a shop sells whole |

A normal install has all four, so there is nothing to copy in or set up — go to the **Server** tab and check the folder. With the
official launcher it is `%LOCALAPPDATA%\L2Everdream`, the folder that contains `game` and `login`. Until it is a world, **Write it to my
world** stays switched off, so a half-built shop can never be written.

## Saving a copy

**Save a copy of the files…** writes the same files into a folder you choose, laid out the way they sit under
`game\data`. Useful for keeping a copy, for looking at what is actually written, or for putting the shop into a world by
hand.
