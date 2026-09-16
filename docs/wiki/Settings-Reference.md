# Settings reference

Every setting the program can change: **1408** in **20** categories, generated from the same
catalog the program itself uses, so the names, defaults and limits here are exactly what you see in the app. The value
your own world uses may differ: the app shows it, and marks it *Changed* when it is not the default.

Looking for one setting? The fastest way is the search box in the app, which matches friendly names, real setting
names, file names and descriptions. In this wiki, use your browser's find (Ctrl+F) on a category page, or the wiki
search box at the top of the page.

## Server tab

| Category | Settings | What it covers |
|---|---|---|
| [Your World](Settings-Your-World) | 5 | The L2Everdream world profile the launcher starts with. |
| [Rates & Rewards](Settings-Rates-Rewards) | 50 | How fast characters level and how much monsters and quests give. |
| [Characters](Settings-Characters) | 118 | Creating characters, levels, stats, limits and everyday rules. |
| [Skills & Combat](Settings-Skills-Combat) | 80 | Skills, buffs, damage and class balance. |
| [Items & Enchanting](Settings-Items-Enchanting) | 50 | Enchanting, augmenting, crafting and items on the ground. |
| [Economy & Trade](Settings-Economy-Trade) | 46 | Shops, private stores, offline trading, manor and money. |
| [PvP & Karma](Settings-PvP-Karma) | 83 | Player killing, karma, fame and PvP rewards. |
| [Clans & Sieges](Settings-Clans-Sieges) | 223 | Clans, alliances, castles, fortresses, clan halls and sieges. |
| [Olympiad & Heroes](Settings-Olympiad-Heroes) | 30 | The Grand Olympiad schedule, rules and rewards. |
| [Events & Activities](Settings-Events-Activities) | 69 | Seven Signs, Dimensional Rift, lottery, fishing, weddings and instances. |
| [Monsters & Bosses](Settings-Monsters-Bosses) | 127 | Monster behaviour, champions, stat multipliers and boss spawns. |
| [Chat & Community](Settings-Chat-Community) | 41 | Chat channels, filters, Community Board, languages and messages. |
| [Convenience Features](Settings-Convenience-Features) | 82 | Optional helpers: auto-play, auto-potions, free mounts and service NPCs. |
| [GM & Administration](Settings-GM-Administration) | 33 | Game Master powers, punishments and logging. |
| [Protection & Anti-cheat](Settings-Protection-Anti-cheat) | 112 | Flood protection, captcha, dual-boxing and login security. |
| [Server & Performance](Settings-Server-Performance) | 178 | Technical server settings. Most players never need these. |

## Client tab

| Category | Settings | What it covers |
|---|---|---|
| [Graphics](Settings-Graphics) | 40 | Resolution, detail, draw distance and effects in the game client. |
| [Sound](Settings-Sound) | 5 | Volume levels in the game client. |
| [Gameplay & Interface](Settings-Gameplay-Interface) | 30 | Names above heads, chat, camera and interface options. |
| [Connection & Login](Settings-Connection-Login) | 6 | Where the client connects and automatic login. |

## How to read these pages

Each setting looks like this:

> ### Experience (XP) rate (RateXp)
> `Rates.ini › RateXp` · number · times
>
> How fast characters gain experience from monsters. 1 = retail.
>
> - **Default:** `1`
> - **Allowed:** 0 or more times
> - **Works with:** Party experience rate

| Line | Meaning |
|---|---|
| The heading | The friendly name in the app, and the real setting name in brackets |
| The grey line | The file and key it is saved to, the kind of editor, its unit, and whether it is *Advanced* (hidden until you turn the Advanced filter on) or *added by L2Everdream* |
| **Default** | What the setting is worth when nothing sets it |
| **Allowed** | The values the program accepts. Anything else is refused when you save |
| **🔒 Set by the launcher** | The launcher rewrites this on every start, so the app shows it locked |
| **Needs** | This setting only does something while another setting has a certain value |
| **Controls** | Other settings that only work while this one is set a certain way |
| **Works with** | A related setting worth looking at together |

Server settings take effect the next time you start your world from the launcher; client settings the next time you
start Lineage 2. See [Server settings](Server-Settings) and [Client settings](Client-Settings) for how to find,
change and save them.
