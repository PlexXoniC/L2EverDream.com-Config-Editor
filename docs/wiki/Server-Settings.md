# Server settings

The **Server** tab holds every setting of your local world: **1,257 game server settings**, **65 login server settings** and the
launcher's **5 world settings** (from `world-profile.json`, such as how many simulated players fill your world).

![Rates & Rewards with an unsaved change](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/rates.png)

## Categories

Settings are grouped by what they do, not by which file they happen to live in:

| Category | Groups |
|---|---|
| Your World | Population · World rules |
| Rates & Rewards | Experience & skill points · Quest rewards · Monster drops · Drops vs. level difference · Vitality bonus · Pets · Extra raid boss drops |
| Characters | New characters · Names & character slots · Levels & death · Classes & subclasses · Stat limits · Inventory & storage · Looting · Parties · Travel & teleports · Vitality system · Pets & summons · Other player rules |
| Skills & Combat | Skills & learning (including [custom skill durations](Custom-Skill-Durations)) · Buffs & effects · Damage · Class balance multipliers · Cancelled buff return |
| Items & Enchanting | Enchanting · Augmentation · Soul crystals · Crafting · Dropped items · Transmogrification · Other item rules |
| Economy & Trade | NPC shops & multisell · Private stores · Offline trade & craft · Selling buffs · Manor · Banking · Scheme buffer · Other |
| PvP & Karma | Peace zones · Karma rules · Item loss on death · Anti-feed · Fame · PvP rewards & announcements · PvP title colors · Faction system · Other |
| Clans & Sieges | Clans & alliances · Clan reputation points · Castle sieges · Castle tower spawns · Castle, fortress and clan hall functions & fees · Contestable clan halls · Mounts during sieges |
| Olympiad & Heroes | Olympiad · Custom period |
| Events & Activities | Seven Signs & Festival of Darkness · Dimensional Rift · Lottery · Fishing championship · Item auction · Weddings · Instances · Cursed weapons |
| Monsters & Bosses | NPCs · Monsters · Champion monsters · NPC stat multipliers · Raid bosses · Grand bosses · Boss announcements · Random spawns · Guards |
| Chat & Community | Chat · Community Board · Languages · Welcome & info messages · Mail (including queued item delivery) |
| Convenience Features | Auto play · Auto potions · Offline auto play · Premium & PC Café points · Service NPCs · Free mounts · Mobius fake players · Account |
| GM & Administration | Game Masters · Punishments & jail · Logging & audits |
| Protection & Anti-cheat | Flood protection · Captcha · Dual-box limits · Hardware ID · Login security · Bot protection |
| Server & Performance | Network & ports · General · Automatic restarts · Saving & memory · Geodata & pathfinding · Threads · Network buffers · Object IDs · Database · Custom data · Development & debugging · Login server · Server console window |

Click a group in the section list to jump to it.

## Finding a setting

- **Section list (left):** every category with a count. Click one to open it; its groups appear underneath. Click a group to jump
  straight to it.
- **Search:** matches the friendly name, the real setting name, the file name and the description, and every word must match.
  `party xp`, `RateXp` and `Rates.ini` all work. While you search, the section list shows only matching categories and groups, and ✕
  clears the search.

  ![Search results](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/search.png)

- **Changed only:** shows settings that differ from their default, plus anything you've edited and not saved yet.
- **Advanced:** also shows 359 technical settings (buffer sizes, thread pools, protocol details) that most people never need. They're
  hidden by default.

## A setting card

| Part | What it tells you |
|---|---|
| **Name and description** | What the setting does, in plain words |
| **Unsaved** badge (gold) | You've edited it and not saved yet |
| **Changed** badge | The value in your file differs from the setting's default |
| **🔒 Set by the launcher** | The launcher rewrites this value on every start, so it can't be changed here (the reason is shown) |
| **Advanced** badge | A technical setting |
| **Relation lines** | How other settings affect this one; see [How settings affect each other](How-Settings-Affect-Each-Other) |
| **Real-name chip** | The exact file and key it is saved to, e.g. `Rates.ini › RateXp` |
| **Default** and **Allowed** | The setting's default and the range of values it accepts |
| **The editor** | An on/off switch, a number box, a slider, a dropdown, a text or list box, or a password box. *Custom skill durations* opens its own page |
| **↶ Undo** | Puts back the value that is in the file |
| **↺ Reset to default** | Sets the default value (you still need to save) |

## Only valid values

- Number boxes refuse letters.
- Values outside the allowed range are marked in amber with the reason, and **Save changes** refuses them.
- Lists are checked too: item ID lists, `id,value` pairs, coordinates, times of day, days of the week, colours, IP addresses, host
  names, percentage splits that must add up to 100, and skill durations.
- The limits are never stricter than what L2Everdream actually ships. An automated test fails if any value in a real install would be
  rejected.

## Saving

The save bar at the bottom shows how many changes are waiting.

- **Save changes** writes every edited setting at once. If any value is invalid, nothing is saved and you're taken to the setting.
- **Discard** throws away all unsaved edits.
- Before writing, the program backs up every file it touches (see [Backups and restore](Backups-and-Restore)).
- Only the values you changed are rewritten. Comments, spacing, order and line endings stay as they were, and each file is written in
  one step so a crash can't leave half a file.
- Server settings are written to **both** the install and the launcher's protected copy. That way the launcher keeps them on its next
  start and through updates (see [Where your files are](Where-Your-Files-Are)).
- Closing the program with unsaved changes asks first.

## When changes take effect

The server reads its settings when it starts. **Server and world settings take effect the next time you start your world from the
launcher.** You can save while the world is running; the folder bar reminds you that the changes apply on the next start.

## Values the launcher owns

The launcher rewrites these on every start, so they're shown locked:

- the database connection in both `Database.ini` files
- `LoginPort` in both `Server.ini` files, and `AutoCreateAccounts` for the login server
- `ClassMaster.xml`, which the launcher writes from its "Free class change" option (that option itself is in *Your World › World rules*)
- the client's `ServerAddr` in `l2.ini`, which follows the launcher's play mode
