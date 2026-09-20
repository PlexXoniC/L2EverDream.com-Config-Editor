# Rates and drops

**One number for how fast your world is.** Pick 3×, 5×, 15×, 20× or type your own, and the Rates tab works out every
experience and drop setting behind it — including the two that quietly waste a rate if you set them the obvious way.

Two tabs work as a pair:

- **Rates** — pick the number and apply it. It writes the settings.
- **Drops** — pick a monster and see what it really gives. It never writes anything.

![The Rates tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/rates.png)

## The short version

Lineage 2 multiplies drops in **two** separate places, and they do not behave the same way:

- **Drop chance** — how often a drop happens. The server rolls each drop **once**. Once that roll is certain (100%), more
  chance does nothing at all. Everything above it is thrown away.
- **Drop amount** — how big each drop is. This never stops working.

So `DeathDropChanceMultiplier = 15` does **not** give you fifteen times the drops. Common drops hit 100% at around 1.5×
and stay there; only the rarest items keep gaining. That is the calculus players have been trying to do in their heads.

The Rates tab does it for you: it keeps the chance just high enough to be useful, puts the rest into the amount, and the
two multiplied together come to the rate you asked for.

## Using the Rates tab

1. Open the **Rates** tab.
2. Pick a preset — **Retail 1×**, **3×**, **5×**, **15×**, **20×** — or type any rate from 0.1 to 100.
3. Optionally move the **how should the extra arrive?** slider:
   - **more often** — as much as usefully possible goes into the chance, so drops appear on more kills.
   - **bigger stacks** — the chance stays at retail and everything goes into the amount, so the same kills give more.
   - Anywhere in between splits it. Whatever you pick, chance × amount is always your rate.
4. Click **Apply N× to my world** — or first follow **See what N× does to a monster →** into the Drops tab.

**Apply** does not save. It fills the values into the **Server** tab as unsaved changes, so you can look them over (the
save bar shows the count, and **Show →** next to each line jumps to its card). Click **Save changes** as usual — and as
usual, the old values are backed up first. The world has to be restarted for them to take effect.

## The Drops tab

Every monster in your world, searchable by name or id. Pick one and you get:

- **Experience and skill points** per kill, and **adena per kill on average**.
- **The monster itself**, drawn from the model in your own game client, painted with its own skins and turning slowly on
  the spot. Lineage 2 has no pictures of monsters, only 3D models, so one is drawn when you pick it; a monster your
  client has no model for shows its level and kind instead.
- One row per drop, with the item's **icon read from your own game client** (nothing is bundled with this program — if you
  have not chosen your client folder yet, the rows simply have no pictures).
- **How often it drops in words**: "about 1 in 6 kills", or "every kill".
- **Chance** before → after, **amount** before → after, and **per kill on average** with the multiplier you are really
  getting — which is the interesting number.
- **Spoil** items are marked; they use the spoil rates.
- **Herbs** are marked as having their own rate: they are deliberately not multiplied (see below).
- A note when a drop is **already certain**, because that is where extra chance would be wasted.
- A note when a monster has more drops than one kill can give, since a kill is capped at 2 different items (7 for raid
  bosses) however high the rates go.
- Drop groups holding the same item added into one row: a grand boss can have seven that all give adena.

![The Drops tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/drops.png)

### Which two sets of rates you are comparing

| Pill | Shows |
|---|---|
| **Retail → your world** | What a drop table site lists, against what your world gives **right now**. This is the one to check when a player asks "is this really 15×?" |
| **N× planned** | Your world now, against the rate picked on the Rates tab. What would change if you applied it. |
| **Retail → planned** | Retail against that planned rate, ignoring what the world is set to today. |

The planned rate is whatever the Rates tab is set to, so the two tabs move together — **Change the rate →** jumps back.
Nothing on the Drops tab writes anything.

### A real example

The Ol Mahum Captain at **5×**, balanced, from retail:

| | Before | After | Really |
|---|---|---|---|
| Adena | 70% chance, 246.5 average | 100% chance, 821.6 | **×4.76** — the chance ran out of room |
| Varnish (spoil) | 37.1%, 1 | 55.7%, 3.3 | ×5 |
| Herb of Life | 23.1% | 23.1% | unchanged |

Adena reaches ×4.76 rather than ×5 because its chance could only climb from 70% to 100%. That gap grows with the rate and
with the slider: at **15×, more often** the same adena comes out at **×10.71**, while at **15×, bigger stacks** it is the
full ×15, in fewer but larger piles. Neither is wrong — but the preview is where you see which one you are choosing,
instead of guessing.

**Retail → your world** is where this really pays off. A world set to `DeathDropChanceMultiplier = 15`,
`DeathDropAmountMultiplier = 3` and adena pinned at 1 by `DropAmountMultiplierByItemId` looks like a 45× world — and the
Drops tab shows its adena arriving at **×1.43** while a spoil recipe lands at ×63. That is not a bug in the server; it is
what those three settings say. Seeing it is the point.

## What it writes

All of these live in `Rates.ini`, and all of them appear in the **What this writes** list with their old → new values:

| Setting | Gets |
|---|---|
| `RateXp`, `RateSp` | Your rate |
| `DeathDropChanceMultiplier` | The chance part |
| `DeathDropAmountMultiplier` | The amount part |
| `DropAmountMultiplierByItemId` | `57,<amount>` — adena, set on its own (see below) |
| `SpoilDropChanceMultiplier`, `SpoilDropAmountMultiplier` | The same split, for spoiling |
| `RaidDropChanceMultiplier`, `RaidDropAmountMultiplier` | A gentler raid rate (see below) |
| `RateQuestRewardXP`, `RateQuestRewardSP`, `RateQuestRewardAdena`, `RateQuestReward`, `QuestItemDropAmountMultiplier` | Your rate |
| `PetXpRate` | Your rate, so pets keep up with you |

**Adena needs its own line.** A per-item rate **replaces** the general one rather than adding to it, and L2Everdream ships
`DropAmountMultiplierByItemId = 57,1` — item 57 is adena. That single line is why raising the drop amount on its own has
never changed the adena players get. The tab keeps that entry in step with your rate.

**Raid bosses move less.** A raid rate of `1 + (rate - 1) / 3` means a 20× world is not a 20× epic-drop world: at 20× the
raid rate is 7.33×. Change it yourself afterwards if you disagree.

## What it deliberately leaves alone

| Left alone | Why |
|---|---|
| **Herb drops** | At a high rate herbs bury you in pickups. Change them in **Rates & Rewards** if you want to. |
| **Vitality** | It already multiplies experience on top of your rate. |
| **Premium bonuses** | They are meant to be a bonus over your world's rate, whatever that is. |
| **The extra for being in a party** | `RatePartyXp` multiplies only the **bonus a party gets for its size**, not the experience — a common misreading of Mobius's ini. |
| **Level-difference penalties and the items-per-kill limit** | They shape drops rather than scale them, and the preview already shows their effect. |
| **Manor, fishing, lottery and other side rewards** | Not part of "how fast is my world". |

Nothing stops you editing any of these by hand in the **Server** tab afterwards — the Rates tab is a starting point, not a
lock.

## Questions

**Does this match l2hub?** l2hub.info lists retail (1×) values, which is what the *before* column shows in the
**Retail → your world** and **Retail → planned** views. If a row's *after* matches l2hub, your world is at retail for that
drop.

**My world says 50× experience and 45× drops — where does that come from?** The line under the slider, and the
**Retail → your world** pill on the Drops tab, read your world's current `RateXp` and drop multipliers, whatever set them.
Applying a rate replaces them.

**Why is the chance never multiplied by more than 2?** Past that, ordinary drops are already certain and the extra is
thrown away. The rate is not lost: it goes into the amount instead, where it still does something.

**Where does the picture of the monster come from?** Your own game client. It has no pictures of monsters — only 3D
models — so the program reads the model the client would use for that npc and draws it. Nothing is bundled with the
program and nothing is downloaded, so choosing your client folder is what makes the pictures (and the item icons) appear.
Monsters stand still and turn on the spot — they do not walk or attack, though the client's animations are in the same
files, so that could follow.

**Do I have to restart the world?** Yes — rate settings are read when the world starts.

## See also

- [Server settings](Server-Settings) — finding, changing and saving any setting by hand
- [Rates & Rewards reference](Settings-Rates-Rewards) — every rate setting, one by one
- [Backups and restore](Backups-and-Restore) — putting the old rates back
