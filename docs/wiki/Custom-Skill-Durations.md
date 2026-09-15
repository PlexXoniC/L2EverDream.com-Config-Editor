# Custom skill durations

Make buffs, songs and dances (or any skill with a timed effect) last longer or shorter than normal, without editing the datapack.

![The skill durations page](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/skill-durations.png)

## Turning it on

1. Open **Server › Skills & Combat › Skills & learning**, or search for `skill duration`.
2. Switch on **Use custom skill durations** (`Player.ini › EnableModifySkillDuration`).
3. On the **Custom skill durations** card (`Player.ini › SkillDurationList`), click **Edit skill durations…**.

While the switch is off, the card says so and has **Show →** to take you to the switch. The page opens inside the Server tab;
**← Back to settings** returns to the list.

## The page

The page lists every skill that has a timed effect in your datapack: **983** in L2Everdream.

| Filter | Shows |
|---|---|
| **Player and buffer buffs** (default) | Buffs, songs and dances that players learn or get from the buffer NPC (231) |
| **Songs and dances** | Bard and dancer skills (27) |
| **Debuffs** | Skills that harm their target, cast by players or monsters (443) |
| **NPC and monster skills** | Skills no player learns and the buffer doesn't give (632) |
| **Custom duration** | Only skills you've given a duration |
| **All** | Everything |

The search box finds skills by name, or by exact ID.

Each row shows:

- the skill's **name**, a **Buff**, **Song / dance** or **Debuff** badge, its **ID** and who uses it (Players, Buffer NPC, or NPC and monster skill)
- its **normal duration**. When it differs by skill level you see the range, e.g. `30 s – 45 s by level`, and for skills that can be
  enchanted on the "+Time" route, the enchanted maximum, e.g. `+Time enchant: up to 40 min`
- a box for **your duration**. Leave it empty (it says *normal*) to keep the normal duration. **↺ Normal** empties it again.

## Typing a duration

Any of these work, from 1 second up to **12 hours**:

| You type | Means |
|---|---|
| `90` or `90s` | 90 seconds |
| `20m` or `20 min` | 20 minutes |
| `1h`, `2 hours` | 1 hour, 2 hours |
| `1h 30m` | 1 hour 30 minutes |
| `20:00` | 20 minutes |
| `1:30:00` | 1 hour 30 minutes |

As you type, the row confirms what it understood (`= 1 h 30 min`). If it can't read the text, or the duration is outside 1 second –
12 hours, the row says so in amber and the skill keeps its previous value.

## Set many at once

The panel on the right applies one choice to **every skill currently shown**. Use the filters and search to choose which skills:

- **2× normal** / **3× normal**: twice or three times each skill's normal duration (its longest level), at most 12 hours
- **1 hour** / **2 hours**
- a duration you type, then **Set all shown**
- **↺ Put all shown back to normal**: removes them from the list

You're asked to confirm first, with the number of skills affected.

**Example: all player buffs last an hour.** Keep the *Player and buffer buffs* filter, click **1 hour**, confirm, then **Save changes**.

## Saving and when it applies

Your durations are stored in the *Custom skill durations* setting. Like every other setting they're written when you click **Save
changes**, with a backup first, and **↶ Undo** on the card or **Discard** puts the list back. The server reads the list when it starts,
so **restart your world** for new durations to apply.

## How the server uses the list

Worth knowing before you set long durations:

- **It's per skill, for everyone.** A skill's new duration applies to every caster: you, the simulated players who buff you, NPC buffers
  and monsters. Longer debuffs also last longer when a monster casts them on you.
- **Enchanted "+Time" levels add your duration.** For those levels, the server adds the seconds you set to the enchanted duration instead
  of replacing it. For example, Shield set to 1 hour lasts 1 hour at normal levels and up to 40 min + 1 hour when "+Time" enchanted. The
  page reminds you on each affected row.
- **Toggles aren't affected.** Toggle skills have no duration.
- **Only real skills count.** If the list contains skill IDs your datapack doesn't have, the page names them and offers **Remove them
  from the list**. Entries that can't be read at all (from a hand-edited file) are listed too, and are left out the next time you
  change a duration on the page.

## In the file

The list is saved in `Player.ini` as `SkillDurationList = skillId,seconds;skillId,seconds`, sorted by skill ID, for example:

```
EnableModifySkillDuration = True
SkillDurationList = 1040,3600;1062,3600;264,600
```
