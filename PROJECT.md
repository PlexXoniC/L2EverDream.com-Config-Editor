# L2Everdream Config — project guide

A standalone Windows desktop app (C# / WPF, one self-contained `.exe`, no installer) for changing the settings of a **locally hosted** L2Everdream world and the
**local** Lineage 2 client, written for people who are not programmers. It follows the look of
[l2everdream.com](https://l2everdream.com) and the official L2Everdream launcher.

> Start with `CLAUDE.md` (rules and working notes for assistant sessions).
>
> **Keep this file current.** Any change to scope, rules, structure, commands, the catalog or the UI updates the
> matching section here and adds a line to the [changelog](#changelog).

---

## 1. Scope and ground rules

| Rule | What it means in practice |
|---|---|
| **Local only** | Edits the world on this PC and the client on this PC. Never the public L2Everdream servers. |
| **Editor only** | Never starts or stops the server, database or client — that is the launcher's job. It only reads a port and the process list to warn that changes apply on the next start. |
| **Every config it can** | Server `.ini` (game + login), the launcher's `world-profile.json`, client `Option.ini` and the encrypted `l2.ini`. XML configs are listed (not yet edited). |
| **User picks both folders** | No default or auto-detected paths. The server folder must contain `game\config\Server.ini`; the client folder must contain `system\l2.ini` (the client folder or its `system` folder both work). Choices are remembered. |
| **Valid values only** | Every setting has a type, range and/or format; editors refuse bad input and Save refuses out-of-range values with a plain-language reason. |
| **Friendly first, real name always visible** | Each setting shows a plain-English name, and a chip with the real `File › [Section] › Key` it is saved to. Search matches both. |
| **No personal names** | Nobody (owner, comment authors, anyone) is named in the app, catalog, docs or messages. The catalog generator strips attributions from config comments. |
| **One rate for the whole world** | The Rates tab turns a single number (3×, 5×, 15×, 20× or typed) into every experience and drop setting it needs, splitting it between drop *chance* and drop *amount* so that chance never climbs past the point where it is wasted. It fills in the Server tab; the normal save bar saves it. |
| **Drops are their own tab** | The Drops tab is the monster browser the rate is judged by: pick any monster and see experience, every drop and spoil item, how often, how much and the average per kill, with the item icons from the player's own client. It compares any two sets of rates — retail (what a drop table site lists), the world as it is set now, and the rate planned on the Rates tab — so it shows what a world really gives, not only what a change would do. |
| **Custom Config is read-only** | The tab shows how L2Everdream differs from stock L2J Mobius; it never writes. |
| **Characters edit the live world carefully** | The Characters tab changes adena, inventory items and item enchant levels of player characters in the running world's database. Existing rows change only while the character is **offline** (character row locked, `online = 0` re-checked in the same transaction). **Item rows are never inserted while the server runs** — new items are queued in `custom_mail` and the server delivers them when the character is online (needs `CustomMailManagerEnabled`, explained in the UI). The inventory limit is enforced as the server counts it, including queued deliveries. Sims are hidden. |
| **Settings explain each other** | Cards show what a setting depends on (and whether it currently has any effect), what it controls and what it works with. |
| **Everything is backed up, restores never race the world** | Every save, character change and restore first backs up what it replaces (files and database rows). Files are restored only while the world is stopped (client files only while Lineage 2 is closed); database rows only for offline characters while the world runs. |
| **Full backups before updates** | One button copies every settings file (server, launcher copies and shipped baselines, world, client) into a folder the user chooses — no default, never inside the install folder that updates replace. A full backup is compared with now **setting by setting** and ticked settings are put back one value at a time, with the same restore rules. |
| **Standalone, no installer** | Ships as a single self-contained `L2EverdreamConfig.exe`: no .NET install, no side files, runs from any folder. The catalog is embedded. Its only own files are preferences and backups under `%LOCALAPPDATA%\L2EverdreamConfig`. |

---

## 2. Build, run, test

Requirements: Windows 10/11, .NET SDK 10 (`dotnet --list-sdks`). Python 3.11+ only to regenerate the catalog.
All commands run from the repository root; nothing depends on where the repository folder lives.

```bash
dotnet build L2EverdreamConfig.slnx -c Debug
```
```bash
dotnet run --project src/L2Config.App
```
```bash
dotnet test tests/L2Config.Core.Tests
```

- The Debug build is `src\L2Config.App\bin\Debug\net10.0-windows\L2EverdreamConfig.exe` (needs the .NET 10 runtime).
- Tests that need a real install and client read their folders from the git-ignored `test-paths.local.json` at the
  repository root (`{ "server": "…\L2Everdream", "client": "…\l2" }`) or from `L2CONFIG_TEST_SERVER` /
  `L2CONFIG_TEST_CLIENT`. They **only read** those folders and write to temporary copies; without them those tests are skipped.
- No machine-specific paths are committed. This PC's paths live in the git-ignored `CLAUDE.local.md` and `test-paths.local.json`.

**Release builds.** Both are one `L2EverdreamConfig.exe` with the catalog embedded and no side files. The version is `<Version>` in
`src/L2Config.App/L2Config.App.csproj`, currently 1.2.0.

```bash
dotnet publish src/L2Config.App -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/standalone
```
```bash
dotnet publish src/L2Config.App -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish/needs-dotnet
```

| Variant | Exe | Zip | Needs |
|---|---|---|---|
| standalone | ~59 MB | ~54 MB | nothing |
| needs-dotnet | ~2.4 MB | ~0.7 MB | .NET 10 Desktop Runtime (x64) |

- Release defaults to self-contained, so `dotnet publish src/L2Config.App -c Release -o publish` still gives the standalone exe.
  Compression is enabled only when self-contained, because the SDK refuses it otherwise.
- GitHub release assets are named `L2EverdreamConfig-<version>-win-x64-standalone.zip` and `…-needs-dotnet.zip`. Each zip contains
  the exe and `LICENSE.txt`. The release notes list SHA-256 hashes (`certutil -hashfile <zip> SHA256`).
- Both variants were verified by running the exe alone from an empty folder.
- `README.md` is the user-facing GitHub page. Keep its numbers (settings, tests, sizes) in step with this file.
- **Wiki:** `docs/wiki/` is the source of the GitHub wiki: 15 hand-written guide pages, 20 generated settings-reference pages
  (`Settings-<Category>.md`) plus `Settings-Reference.md` from `python catalog/build_wiki.py`, and `_Sidebar.md` / `_Footer.md`.
  Regenerate the reference after changing the catalog; never hand-edit the generated pages. Pages link to each other by
  page name and to images on the main branch (`raw.githubusercontent.com/…/main/docs/images/`). Keep it in step with the app and the README.
  To publish: create the first wiki page once on GitHub (Wiki › Create the first page), then clone
  `https://github.com/PlexXoniC/L2EverDream.com-Config-Editor.wiki.git`, copy `docs/wiki/*.md` into it, commit and push.

**README screenshots** (`docs/images/*.png`) must show no real folders, account or character names. They are made in snapshot mode
(1280x820 at 125 % scaling). `rates.png` and `drops.png` show no folder bar and no characters, so they come from a real install
(`--tab rates --rate 5`, `--tab drops --monster Antharas`). Everything else comes from a demo world, which keeps folder paths neutral
and needs no black bars (a real install with the path covered by a solid black bar also works for a screen whose only private detail is
the folder bar):

1. Copy an install's `game\config`, `login\config`, `game\data\stats\items`, `game\data\EnchantItemData.xml`, `world-release.json`,
   the data folder's `db\config` and `worlds\world-profile.json`, and a client's `system\*.ini`, into a neutral tree, e.g.
   `D:\Games\L2Everdream`, `D:\Games\L2Everdream-data` and `D:\Games\Lineage II` (a temporary `subst` drive works; remove it
   afterwards). For skill durations also copy `game\data\stats\skills`, `game\data\stats\players\skillTrees` and
   `game\data\SchemeBufferSkills.xml`.
2. Start a throwaway MariaDB: `db\base\bin\mariadb-install-db.exe --datadir=<temp>`, then the install's
   `db\base\bin\mariadbd.exe --no-defaults --datadir=<temp> --port=33999 --skip-grant-tables`. The install ships no game SQL, so take
   the `characters`, `items` and `custom_mail` definitions from a world database with `SHOW CREATE TABLE` (structure only, no rows) and
   fill them with made-up characters and items. Point both of the copy's `Database.ini` files at it, turn `CustomMailManagerEnabled`
   on in the copy, and give the copy's `GameserverPort` (install and protected copy) an unused port, so a real world running on this
   PC does not put "Your world is running" into the folder bar.
3. Make demo backups into a temporary folder with the Core API (`SettingsStore.Save`, `WorldDatabase.SetItemCountAsync`,
   `SetItemEnchantAsync`, `QueueDeliveryAsync`), and a demo full backup with `FullBackupLibrary.Create` followed by a few settings
   edited in the demo, as an update would. Render each tab with `--server`, `--client`, `--backups` and `--full-backups` pointing at
   the demo (`skill-durations.png`: `--tab server --skill-durations --edit "SkillDurationList=1085,3600;1087,3600;1354,5400"`;
   `full-backup-compare.png`: `--tab backups --compare latest --filter all`).
4. Stop the database, remove the `subst` drive and delete the demo tree.

**Developer snapshot mode** (renders the window to a PNG and exits; uses throwaway preferences and never saves):

```bash
L2EverdreamConfig.exe --snapshot out.png --server "<server folder>" --client "<client folder>" --tab server --category rates --search "party xp" --edit RateXp=3 --advanced --size 1280x820
```

`--backups <dir>` lists backups from another folder instead of `%LOCALAPPDATA%\L2EverdreamConfig\backups`; `--backups-mode full` opens Full backups, `--full-backups <dir>` sets their folder, `--compare <backup folder|latest>` opens a comparison (with `--filter changed|shipped|new|all` and `--search`; nothing is restored); `--tab rates` opens the Rates tab and `--tab drops` the Drops tab (with `--rate <number>`, `--delivery 0|1`, `--monster <name>` and `--view retail-now|now-planned|retail-planned`); `--skill-durations` opens the skill durations page (with `--filter players|songs|debuffs|npc|changed|all` and `--search`); `--inventory <character>` opens that character's inventory (with `--item-search`, `--item <id>`, `--amount` to fill the add panel; nothing is applied); `--tab` is `server`, `client`, `rates`, `drops`, `custom`, `characters` or `backups`; `--group <group id>` scrolls to a group; `--edit key=value` shows an
unsaved edit in memory.

---

## 3. Repository layout

```
L2EverdreamConfig.slnx
CLAUDE.md                        rules from the user + working notes (read first in a new session)
CLAUDE.local.md                  git-ignored: this PC's server/client folders and tooling notes
test-paths.local.json            git-ignored: server/client folders used by the local-install tests
PROJECT.md                       ← this file
README.md                        user-facing GitHub page (download, features, FAQ, developer notes)
docs/images/                     README and wiki screenshots (see §2 for how they are made)
docs/wiki/                       GitHub wiki source: one .md per page, _Sidebar.md, _Footer.md (see §2)
catalog/                         the friendly layer (data + generators)
  build_catalog.py               → catalog.json   (names, groups, editors, limits, descriptions)
  build_custom_config.py         → custom-config.json (differences from stock L2J Mobius)
  friendly-names.tsv             hand-curated names/units/descriptions for server settings
  client-settings.tsv            hand-written client + world-profile settings
  custom-config-notes.tsv        neutral summaries for each Custom Config difference
  setting-relations.tsv          how settings affect each other (requires / affects), with plain notes
  catalog.json, custom-config.json   generated; embedded into the exe at build time
research/
  L2EVERDREAM-KNOWLEDGE.md       how L2Everdream, the launcher, Mobius and the client work
  seed-data/                     schema extracted from the install (config keys + Java types/defaults), launcher.css, ini decoder reference
  upstream-mobius/<commit>/      stock L2J_Mobius_CT_0_Interlude config files (from GitLab) used for Custom Config
src/L2Config.Core/               no UI: catalog model, INI editing, client ini codec, stores, validation, search, world database (MySqlConnector)
src/L2Config.App/                WPF app (MVVM, no third-party packages)
tests/L2Config.Core.Tests/       xUnit
```

### Core (`src/L2Config.Core`)

| File | Responsibility |
|---|---|
| `Catalog/CatalogModels.cs` | `SettingsCatalog`, `CategoryDefinition`, `GroupDefinition`, `SettingDefinition` (target, file, section, key, name, editor, value type, options, unit, min/max, format, default, advanced, managed reason, secret). |
| `Catalog/SettingSearch.cs` | Search: every word must match name, key, file, section, group or description; ranking prefers exact key, then name/key hits, then shorter keys. |
| `Catalog/CustomConfig.cs` | Model for `custom-config.json`. |
| `Ini/IniDocument.cs` | Line-preserving INI editor: comments, spacing, order, line endings and BOM survive; missing keys/sections are appended. |
| `Ini/L2IniCodec.cs` | Client ini formats: `Lineage2Ver413` (RSA-1024 blocks + zlib + CRC32 tail), `Lineage2Ver111` (XOR 0xAC), plain. Verified byte-for-byte against the shipped client. |
| `Storage/L2Locations.cs` | The two chosen folders and every path derived from them; folder validation. |
| `Storage/ConfigFiles.cs` | `ServerIniFile` (install copy + launcher player copy), `ClientIniFile` (re-verifies encoding before writing), `WorldProfileFile` (keeps JSON types and unknown fields). Atomic writes. |
| `Storage/SettingsStore.cs` | Loads every file the catalog uses; validates **all** changes before writing any; backs up, then saves. |
| `Storage/SettingValues.cs` | Equality (numbers/booleans), range and format validation, bool formatting in the file's own style, range text. |
| `Backups/BackupSession.cs` | One backup: flat folder `backups\yyyyMMdd-HHmmss-title\` with `role__file` copies (e.g. `game-config__Rates.ini`, `game-player-copy__Rates.ini`, `client__l2.ini`), `db-NN__table.json` row snapshots and a `manifest.json` (original paths, SHA-256, what changed from → to). |
| `Backups/BackupLibrary.cs` | Lists backups (also the older nested-folder format) and restores them: verifies SHA-256, refuses files while the world or client runs, backs up the current state first, restores database rows through `WorldDatabase`. |
| `FullBackups/FullBackup.cs` | Full backups: manifest (versions from `world-release.json`, every file's role, original path, SHA-256), `FullBackupLibrary.Create` (flat `role__file` copies written under `.partial` and renamed when complete), `List`, `LocationProblem` (refuses the install and client folders). |
| `FullBackups/FullBackupComparer.cs` | Compares a full backup with the files now by **setting value**: server ini (protected copy preferred, shipped baseline before/now), world-profile fields, client ini (decoded); XML/text files with comments and blank lines ignored plus a line diff. Kinds: changed, no longer in the file, new since the backup, shipped default changed, comments only. Marks what can't be restored and why (launcher-managed, not allowed any more, not in the backup, launcher-written file). |
| `FullBackups/FullBackupRestorer.cs` | Restores ticked settings one value at a time through the normal file classes (both server copies, client encoding re-verified), XML/text files whole; refuses while the world / Lineage 2 runs, re-checks each backup copy's SHA-256, backs up what it replaces first. |
| `Storage/RuntimeStatus.cs` | Read-only: is the world listening on its game port, is `L2.exe` running. |
| `Characters/WorldDatabase.cs` | The world's database (from `game\config\Database.ini`, MySqlConnector): characters, inventory, queued deliveries; set/remove item counts, set enchant levels and queue/cancel deliveries inside transactions that lock the character and back up rows first; restore rows (offline only; re-create a removed item with its original ID only if the world has not restarted since the backup). |
| `Characters/EnchantRules.cs` | How far the world's own enchanting goes, from `game\data\EnchantItemData.xml`, so the editor can say when a level is one the game itself never gives out. |
| `Characters/ItemCatalog.cs`, `Characters/InventoryRules.cs` | Every item from `game\data\stats\items` (name, type, stackable, grade); the server's inventory limit (race, Game Master access levels from `AccessLevels.xml`) and slot counting incl. pending deliveries. |
| `Rates/MonsterCatalog.cs` | Every monster from `game\data\stats\npcs` (level, type, experience, skill points, HP, drop and spoil groups) with item names and icon names from `game\data\stats\items`; herbs recognised by `ex_immediate_effect`. |
| `Rates/RateSettings.cs` | The multipliers actually in force — read from a world's files, built from a rate plan, or retail (all 1) — with the server's else-if chain (by item id, herb, raid, then the general rates) resolved once, so adena's own entry replaces the general one rather than adding to it. |
| `Rates/RatePlan.cs` | Turns one rate into the settings that produce it: `RateOptions` (rate, and how much of it goes to chance rather than amount) computes the chance multiplier — never past the point where chance is wasted — and the amount multiplier that makes up the rest, plus a gentler raid rate; `RatePlan.Build` lists the 16 values it writes (with adena's own amount, because `DropAmountMultiplierByItemId` replaces the general one) and what it deliberately leaves alone. |
| `Rates/DropPreview.cs` | What one monster gives under **any two** sets of rates (retail → now, now → planned, retail → planned), exactly as the server works it out: a single roll per drop group (so chance stops at 100%), amounts multiplied, herbs and the limit on different items per kill honoured, raid multipliers for raid and grand bosses. |
| `Client/UePackage.cs` | A Lineage 2 Unreal package read without loading it: the mesh packages are 100-200 MB, so only the header, the name table, the export table and the one object asked for are decrypted (XOR 0xAC, or the file-name key). |
| `Client/NpcGrp.cs` | The client's `npcgrp.dat` (encrypted like `l2.ini`): for every npc id, the model and skin textures the client draws it with. |
| `Client/MonsterMesh.cs` | A Lineage 2 skeletal mesh out of a `.ukx` package. The stock UE2 point, wedge and face arrays are empty in this game: the geometry is in the LOD models, as 52-byte wedges (position, a normal of length 512, texture coordinates, bones and weights) plus a triangle index buffer. Those positions are the reference pose, so a still needs no skinning; the smallest LOD that still looks right is used. |
| `Client/MeshRenderer.cs` | Draws a model into a small picture: orthographic, depth-buffered, painted from the model's own skins, one light, no graphics card — so it looks the same everywhere, including in snapshot mode. `Turntable` draws a whole turn, and `FacingYaw` is the angle that shows a Lineage 2 model three-quarters on (they look along +Y). |
| `Client/MonsterArtLibrary.cs` | Ties those together: npc id → model and skins → a turn of pictures, with the packages kept open and the frames cached. A skin named by the client is usually a Shader, so its `Diffuse` texture is followed. |
| `Client/UeTexture.cs` | One texture out of a package: the property list gives size and format, then the mipmaps (DXT1, DXT3 or plain). Shared by the item icons and the monster skins. |
| `Client/IconLibrary.cs` | Reads item icons out of the player's own client: decrypts `systextures\Icon.utx` (`Lineage2Ver121`, XOR by filename), walks the Unreal package's name/export tables and decodes the DXT1/DXT3 textures. Nothing is bundled with the app. |
| `Skills/SkillCatalog.cs` | Every skill with a duration from `game\data\stats\skills` (custom last), whether players learn it (`stats\players\skillTrees`) or the buffer gives it (`SchemeBufferSkills.xml`), kind (buff, song/dance, debuff), durations by level and "+Time" enchant maximum; `SkillDurationList` parse/format, plain-word durations (`90s`, `1h 30m`, `1:30:00`). |

### App (`src/L2Config.App`)

| File | Responsibility |
|---|---|
| `App.xaml.cs` | Loads the embedded `catalog.json` + `custom-config.json`, builds the main view model; snapshot mode. |
| `MainWindow.xaml` | Chromeless window: "L2Everdreamconfig" wordmark ("config" styled like "L2"), **Server / Client / Rates / Drops / Custom Config / Characters / Backups** tabs (the strip wraps onto a second line when the window is too narrow for them), folder bar, left section list with search and filters, virtualized settings list, save bar. All editor, row and page templates. |
| `ViewModels/MainViewModel.cs` | Tabs, folder choice and validation, reload, save/discard, runtime notices, "Show in editor" jump. |
| `ViewModels/TabViewModel.cs` | One Server or Client tab: categories → groups, counts, search, "Changed only", advanced filter, rows. |
| `ViewModels/SettingViewModel.cs` | One setting: value, dirty/changed state (as words), validation error, range text, undo, reset to default. |
| `ViewModels/CharactersViewModel.cs`, `ViewModels/InventoryViewModel.cs` | Characters list with adena editor; inventory editor: items with Set count / Remove, waiting deliveries with Cancel, item search over the full list, amount/enchant, live slot and stack checks, the plain delivery explanation. |
| `ViewModels/BackupsViewModel.cs`, `ViewModels/RelationViewModel.cs` | Backups tab (Change backups: list, what changed, restore with a plan and per-item results); live "depends on / has no effect right now / controls / works with" lines on setting cards. |
| `ViewModels/FullBackupsViewModel.cs` | Backups tab › Full backups: folder choice, *Back up everything*, list with "updated since this backup", and the comparison checklist (filters, search, tick, *Restore selected*, results). |
| `ViewModels/SkillDurationsViewModel.cs` | The skill durations page (opened from the "Custom skill durations" card while "Use custom skill durations" is on): filters, search, a duration box per skill with its normal and enchanted duration, "Set many at once" (2×, 3×, 1 h, 2 h, typed, back to normal), unknown ids and unreadable entries. Edits go into the setting value, so the normal save bar saves them. |
| `ViewModels/RatesViewModel.cs` | The Rates tab: preset pills and a typed rate, the *how it arrives* slider, and the settings it will write in two columns (old → new); *Apply* fills them into the Server tab as unsaved changes. Raises `RatesChanged`, which the Drops tab follows. |
| `ViewModels/DropsViewModel.cs` | The Drops tab: the monster search and list, which two sets of rates are compared, the monster card (level, kind, HP, experience, adena per kill, and the frame a rendered monster would go in) and a row per drop with its icon, chance, amount, average per kill, real multiplier and how often it drops in words. |
| `Infrastructure/ItemIcon.cs` | Caches icons decoded from the client as frozen WPF images; cleared when the client folder changes. |
| `Infrastructure/MonsterArt.cs` | Draws a monster's whole turn off the interface thread and caches the frozen frames; cleared when the client folder changes. |
| `ViewModels/CustomConfigViewModel.cs` | Read-only differences with stock / shipped / current values and filters. |
| `Theme/Colors.xaml`, `Theme/Controls.xaml` | The design system (see §8). |
| `Infrastructure/*` | `ObservableObject`, `RelayCommand`, app preferences, converters, editor template selector, numeric input filter, password binding. |
| `DialogService.cs` | Folder picker and themed dialogs. |

---

## 4. How saving works (and why)

| Target | Files read | Files written |
|---|---|---|
| Server game/login `.ini` | Player copy `…\L2Everdream-data\db\config\{game,login}\<file>` if present, else install `…\L2Everdream\{game,login}\config\<file>` | **Both** the install copy and the player copy (when it exists) |
| `world-profile.json` | `…\L2Everdream-data\worlds\world-profile.json` | same |
| Client `Option.ini` (plain) | `<client>\system\Option.ini` | same |
| Client `l2.ini` (encrypted) | decoded in memory | re-encoded, decoded again and compared before it is written |

- The L2Everdream launcher keeps protected **player copies** of 27 game and 5 login config files and merges them over
  the install on every start and update. Writing both copies keeps them in agreement, so edits survive updates.
  (`Data root` is the `L2Everdream-data` folder beside the chosen server folder.)
- **Launcher-managed values are read-only** in the app, with the reason shown: both `Database.ini` URLs, `LoginPort`
  (game and login `Server.ini`), login `AutoCreateAccounts`, the world name, and client `ServerAddr`. The launcher rewrites
  these on every start. `ClassMaster.xml` is likewise written by the launcher from the "Free class change" setting.
- Saving is refused while `L2.exe` is running if there are client changes (the client rewrites its settings on exit).
- Server changes save while the world runs, with a notice that they apply on the next start.
- Every save first backs up each file it touches, into one flat backup folder with a manifest (see §6a). "Backups and restore" in the save bar opens the Backups tab.

---

## 5. The catalog

`catalog/catalog.json` is the single source of what the UI shows. **Current contents: 1,408 settings** in
20 categories (16 server, 4 client).

| Target | Settings |
|---|---|
| server-game | 1,257 |
| server-login | 65 |
| client-option (`Option.ini`) | 58 |
| client-l2ini (`l2.ini`) | 23 |
| world-profile | 5 |

Editors: 750 number, 420 on/off, 97 list, 92 text, 40 choice, 7 slider, 1 secret, 1 skill-durations page. 359 are marked **Advanced**
(hidden until the Advanced filter is on), 25 are launcher-managed.

### Categories (grouped by what a setting does, not by file)

Server: Your World · Rates & Rewards · Characters · Skills & Combat · Items & Enchanting · Economy & Trade ·
PvP & Karma · Clans & Sieges · Olympiad & Heroes · Events & Activities · Monsters & Bosses · Chat & Community ·
Convenience Features · GM & Administration · Protection & Anti-cheat · Server & Performance.
Client: Graphics · Sound · Gameplay & Interface · Connection & Login.

Each category has groups (e.g. Rates & Rewards → Experience & skill points, Quest rewards, Monster drops, …). The
routing rules live in `ROUTES` in `build_catalog.py`.

### Names and descriptions

1. `friendly-names.tsv` (hand-curated, ~930 rows) wins.
2. Pattern names for large families (flood protection, clan hall/castle/fortress fees, siege towers, raid rankings,
   grand bosses, buffer pools, class balance, NPC stat multipliers, premium rates).
3. A humanizer for the rest (splits the key, expands abbreviations).

Descriptions use the **stock L2J Mobius comment** whenever the setting exists upstream, so the release's internal
notes never appear in the editor; otherwise the file's own comment, cleaned (HTML, ticket references and personal
attributions removed).

### Valid values

- Numbers: Java type range (Byte/Int/Long) → never negative unless the file or stock default allows it → `%` values
  0–100 → ports 1–65535 → hours 0–23 / minutes 0–59 → explicit limits in `KNOWN_LIMITS` (e.g. characters per account
  1–7, warehouse slots ≤ 299, max level 1–80). 754 of 769 numeric settings have a minimum, 496 a maximum.
- Choices: from the comments ("Available Options", `0 = …` lists, `NAME - …` lists) and `KNOWN_CHOICES`.
- Formats (`SettingValues.ValidateFormat`): `int-list`, `int-list;`, `pair-list`, `boss-drop-list`, `range-list`,
  `percent-split-4` (must total 100), `time-list`, `weekday-list`, `hex-color`, `coordinates`, `buffer-list`,
  `word-list`, `word-list;`, `ip-list`, `host`, `regex`, `skill-duration-list` (skill id, 1 s–12 h).
- Guard rail: the test `EveryCurrentValueIsWithinItsLimits` fails if any value in the real install or client would be
  rejected, so a limit can never be stricter than what actually ships.

### Regenerating

```bash
python catalog/build_catalog.py
```
```bash
python catalog/build_custom_config.py "%LOCALAPPDATA%\L2Everdream"
```
```bash
python catalog/build_wiki.py
```

`build_catalog.py` warns about `friendly-names.tsv` rows that match no setting; `build_custom_config.py` warns about
differences without a summary in `custom-config-notes.tsv`. Rebuild or republish the app afterwards (the JSON is embedded at build).
After an engine update, first refresh `research/seed-data/server-config-schema.json` (see the knowledge base).

---

## 6. Custom Config tab

Compares the configs L2Everdream **ships** (the launcher's `.shipped-baseline` copies, or the install for files the
launcher does not baseline) with stock `L2J_Mobius_CT_0_Interlude/dist` at commit `59097d4d05f0` (2026-06-27), the
last upstream change to those files before the shipped server build (2026-07-22).

Current result: **22 setting differences** — 5 new settings that needed code changes (gatekeeper teleport price and
the network security buffer pools), 16 changed values (rates, free teleports, party leader hand-off, dropped-item
lifetime, cursed weapons, monster leash, Shift+click NPC info, pathfinding buffers, buffer pool growth, login window,
newbie status), 1 documented setting — plus `ClassMaster.xml` as a changed file.

Each card shows the friendly name, the real file/key, a neutral summary, **stock → shipped → your file now** (flagged
when you changed it) and **Show in editor**, which opens that setting in the Server tab.

---

## 6a. Backups and restore

Location: `%LOCALAPPDATA%\L2EverdreamConfig\backups\`. One folder per backup, never nested:

```
20260914-101500-saved-3-settings\
  manifest.json                 kind, title, created, entries (original path, SHA-256), changes (what: from -> to)
  game-config__Rates.ini        the game server's copy
  game-player-copy__Rates.ini   the launcher's protected copy
  client__l2.ini
  db-01__items.json             database rows as they were, and the operation (update / delete / insert)
```

| Kind | Taken before | Restore |
|---|---|---|
| Settings | Save changes | Files, only while the world is stopped; client files only while Lineage 2 is closed |
| Characters | Every adena/item change, delivery queued or cancelled | Rows, only for offline characters while the world runs. Updated rows are put back; removed items are re-created with their original ID only if the world has not restarted since; queued deliveries are removed if not yet delivered; cancelled deliveries are re-queued if they still fit |
| Before restore | Every restore | Same rules, so a restore can be undone |
| Older format | (first app version) | Files, same rules |

Every restore verifies the backup copy's SHA-256 and refuses database rows from a different world's database.

Database behaviour is covered by `WorldDatabaseIntegrationTests`, which run only with `L2CONFIG_TEST_DB` pointing at a throwaway database.

### Full backups (before updating L2Everdream)

Launcher updates replace the whole install folder and rewrite every config file (0.5.20 stripped nearly all comments but
changed no values — see the knowledge base). A full backup is the safety net for that, separate from the automatic
change backups above.

- **Where:** a folder the user chooses the first time (*Change folder…* later); remembered in the app's preferences.
  There is no default. The install folder and the client's system folder are refused.
- **What:** `game\config\**`, `login\config\**`, the launcher's protected copies and `.shipped-baseline\**` for game and
  login, `L2Everdream-data\worlds\*.json`, `world-release.json`, client `system\*.ini`. About 190 files / 0.7 MB, under
  two seconds. One flat folder `yyyyMMdd-HHmmss-full-backup-<launcher version>\` with `role__file` copies and
  `full-backup.json`.
- **Compare with now:** per setting — *in the backup*, *now* and *L2Everdream ships* (baseline before → now). Filters:
  *Different from the backup* (default), *Shipped defaults changed*, *New since the backup*, *Everything*; search.
  Files whose settings are all equal but whose text changed are summarised as "comments only". The header says whether
  L2Everdream was updated since the backup (release versions differ).
- **Restore selected:** only ticked values are written, so comments and settings added by an update stay. Refused while
  the world runs (server/world) or Lineage 2 is open (client); launcher-managed values, settings not in the backup,
  shipped-default-only changes and `ClassMaster.xml` can't be ticked. A "before restore" change backup is taken first.
- Verified against the real 0.5.19 → 0.5.20 update: 1 shipped-default change (`GMGiveSpecialSkills`), 42 comment-only
  files, nothing to restore; comparison 0.13 s.

---

## 7. What the UI does

- **Tabs**: Server, Client, Rates, Drops, Custom Config, Characters, Backups. The Server and Client tab pills show a count of unsaved changes.
- **Setting cards** also show how other settings affect them: *Depends on* (green when met), *Has no effect right now* (amber, e.g. vitality rates while the vitality system is off), *Controls* and *Works with*, each with a *Show →* jump.
- **Rates**: one number for the whole world. Preset pills (1×, 3×, 5×, 15×, 20×) or a typed rate from 0.1 to 100; a
  *how it arrives* slider between "drops happen more often" and "drops come in bigger piles"; a plain summary of what the
  rate means and what the world runs at now; *What this writes*, all 16 settings in two columns with their old → new values,
  a one-line reason and a *Show →* jump; and what is deliberately left alone (herbs, vitality, premium, the party bonus,
  level-gap penalties, manor and fishing). *Apply* fills the values into the Server tab as unsaved changes, saved by the
  normal save bar. *See what N× does to a monster →* opens the Drops tab.
- **Drops**: every monster in the world, searchable by name or id. Three comparisons — *Retail → your world*,
  *N× planned* (your world → the rate picked on the Rates tab) and *Retail → planned* — with a line saying exactly what is
  being compared and a link back to the Rates tab. The monster card shows the monster itself — drawn from the model in the player's own
  client, painted with its own skins and turning slowly on the spot — with level, kind, HP, experience and skill points
  before → after and adena per kill. Until a picture is ready
  (or when the client has no model for that monster) the frame shows its level and kind instead. Each drop is a row: the item's icon
  read from the player's own client, how often it drops in words ("about 1 in 6 kills", "every kill"), chance and amount
  before → after, the average per kill and the multiplier really achieved, with *Spoil* marked, herbs marked as having
  their own rate, and a note when a drop is already certain so extra chance would be wasted. Drop groups that hold the
  same item are added into one row (a grand boss can have seven that hold adena), and big numbers are shortened (107M).
- **Characters**: each player character with level, account, inventory slots used of the limit, adena editor (offline only) and *Inventory…*.
- **Inventory**: items in the inventory (equipped marked) with *Set count* for stacks, *Set enchant* for weapons, armour and jewellery (with a note when the level is past the world's own ceiling) and *Remove* (confirmed); a warning across the top of the screen, with *Open that setting →*, whenever item delivery is off — including how many queued items will never arrive; *Waiting for the server to deliver* with *Cancel delivery*; *Add items*: search every item by name or ID, amount, enchant for weapons/armor, a line saying exactly what will happen and how many slots it needs, and the reason when it can't. Adding to a stack the character already carries changes that stack immediately; anything else is queued for the server, with a highlighted explanation and the live state of the delivery setting.
- **Backups**: *Change backups* / *Full backups* switch. Change backups: every backup newest first with what changed (from → to), what it contains, *Restore…* (with a plan of what will and won't be restored right now) and *Open folder*; results per item after a restore.
- **Full backups**: folder card (*Change folder…*, *Open folder*, the gold *Back up everything*, installed version), list of full backups with version, size and "updated since this backup", *Compare with now* → checklist of setting cards (tick box, name, kind badge, group, real-name chip, backup / now / ships values, reason when it can't be restored), filter pills with counts, search, a "comments only" summary, and the *Put settings back* panel (tick all shown, clear, *Restore N settings*, per-item results).
- **Folder bar** per tab with *Change folder…*; with no folder chosen the page is a single card with one gold button.
- **Left section list**: categories with counts; the selected category expands to its groups; clicking a group
  scrolls to it. While searching, only matching categories/groups are listed.
- **Search**: friendly names, real keys, file names and descriptions ("RateXp", "party xp", "Rates.ini" all work).
- **Filters**: *Changed only* (differs from default or edited), *Advanced*.
- **Setting card**: name, state word (*Unsaved* / *Changed*), *Set by the launcher* lock, *Advanced* tag, description,
  real-name chip, default, allowed range, the editor, error text, *Undo* and *Reset to default*.
- **Skill durations page**: "Custom skill durations" (Player.ini `SkillDurationList`) opens a page in the Server tab listing all 983 skills with a duration — filters *Player and buffer buffs* (default), *Songs and dances*, *Debuffs*, *NPC and monster skills*, *Custom duration*, *All* — each with its normal duration (by level, +Time enchant maximum) and a box for your duration (empty = normal, 1 s–12 h). *Set many at once* applies 2×/3× normal, 1 h, 2 h or a typed duration to every skill shown (with a confirmation). The page explains that durations apply to everyone casting the skill (players, sims, monsters), that +Time enchanted levels add the listed seconds, and that the world must restart.
- **Editors**: on/off switch (also says On/Off), number box that refuses letters, slider + number, dropdown,
  text/list box, password box.
- **Save bar**: status, backups link, unsaved count, *Discard*, *Save changes* (the one gold action).
- Closing with unsaved changes asks first.

---

## 8. Design system

Ported from the launcher's `launcher.css` (copied to `research/seed-data/launcher.css`) and the site's CSS variables.
All colours live in `Theme/Colors.xaml`.

| Token | Value | Use |
|---|---|---|
| Twilight / Deep | `#1B1636` / `#12101F` | window ground gradient |
| Starlight | `#F4F1FA` | primary text |
| Wisp / labels | `#7C6FB0`, `#A99CD4`, `#9D90C8`, `#8478B4` | secondary text, kickers, captions |
| Lantern / Ember | `#E8A24C` / `#F6C67A` | the one gold action, unsaved state, warnings (amber, never red) |
| Arcane | `#9D8CF0` | selected / adjustable / "yours" |
| Verdance | `#8FB7A6` | success, saved |
| Hairlines | violet at 14–42 % alpha | card and field borders |
| Fonts | Georgia (display), Segoe UI (body), Consolas (real names, values) | |

Rules carried over: one lit gold action per screen; state is always a word as well as a colour; no effects on
text-bearing containers; warnings are amber and do not block unless a value is invalid.

---

## 9. Knowledge base

`research/L2EVERDREAM-KNOWLEDGE.md` documents the installs, the launcher's boot sequence and file ownership, ports,
embedded MariaDB, L2SP JVM properties, the client ini formats and the design tokens. Read it before changing save
behaviour or adding a new file type.

---

## 10. Known limitations and next steps

- XML configs (`AccessLevels.xml`, `AdminCommands.xml`, `DynamicExpRates.xml`, `SiegeSchedule.xml`, `Scripts.xml`,
  `default-ipconfig.xml`, `chatfilter.txt`) are not editable yet.
- Client `user.ini` (key bindings) and `WindowsInfo.ini` are not in the catalog.
- Some client option numbers (texture/model detail, draw distance steps) are shown as raw values because their exact
  in-game labels are not verified.
- About 480 server settings still use pattern or humanized names; improve them in `friendly-names.tsv`.
- `-Dl2sp.*` launch settings are out of scope: the launcher regenerates its flags file on every start.
- Characters: only inventory items (no warehouse, skills, stats yet). Deliveries need `CustomMailManagerEnabled` on and the world restarted after turning it on; the base inventory limit ignores in-game inventory-expansion skills (so it is slightly conservative).
- `build_custom_config.py` reads the install's current files, so regenerating on a machine with edited settings counts those edits as release changes.
- Full backups are taken with the button only; the app cannot know an update is pending (the launcher checks online).
- `IniDocument` splits lines on the file's own newline style; a hand-edited file with mixed CRLF/LF line endings can hide
  keys from both the editor and the comparison.
- Drops: the preview reads the datapack, so it describes a freshly started world — it does not know about a boss already killed or a player's own bonuses (premium, vitality, level gap). Monsters are drawn in their reference pose and turned on the spot; they do not walk or attack, though the client's animation tracks are in the same packages.
- No app icon yet. (There will be no installer — the app is a standalone exe by design.)

---

## Changelog

| Date | Change |
|---|---|
| 2026-09-13 | Research: installs, launcher, Mobius, client formats, site design → knowledge base and seed data. |
| 2026-09-13 | Catalog generator with categories, friendly names, editors; client and world-profile settings. |
| 2026-09-13 | Core library: line-preserving INI editing, client ini codec, stores with dual-copy saves and backups; tests. |
| 2026-09-13 | WPF app: Server/Client tabs, user-chosen folders, section list, search, filters, editors, save bar. |
| 2026-09-13 | Valid-value limits and formats for every setting, enforced in editors and on save; limit guard test. |
| 2026-09-13 | Custom Config tab (read-only differences from stock L2J Mobius); personal names removed everywhere. |
| 2026-09-13 | PROJECT.md created. |
| 2026-09-13 | Standalone: catalog embedded in the exe; Release publishes one self-contained single-file exe; no installer. |
| 2026-09-13 | Repository made location-independent for a move: relative commands, CLAUDE.md with all rules and context. |
| 2026-09-14 | Characters tab: edit inventory adena of offline player characters in the running world's database (MySqlConnector). |
| 2026-09-14 | Inventory editor (change/remove items offline; add items via server delivery with inventory-limit checks), Backups tab with restore (files only while stopped, rows only for offline characters), row-level database backups, flat backup folders with manifest, setting relations on cards. |
| 2026-09-14 | Version 1.0.0: GitHub `README.md`; two release variants (standalone and needs-dotnet single-file exes), zipped with SHA-256 hashes and release notes. README screenshots from a demo world; snapshot mode `--backups <dir>`. |
| 2026-09-15 | Wiki settings reference generated from the catalog (`catalog/build_wiki.py`, 20 category pages + index, every setting with default, allowed values and relations); README and release notes trimmed to point at the wiki. |
| 2026-09-15 | Version 1.1.0 (full backups, custom skill durations); release notes in `publish/release/release-notes-v1.1.0.md`. |
| 2026-09-15 | GitHub wiki source in `docs/wiki` (13 pages: install, server and client settings, skill durations, relations, Custom Config, characters, backups, full backups and updates, file locations, troubleshooting, privacy, developers). |
| 2026-09-15 | Skill durations page for `SkillDurationList` (all skills with a duration, normal/enchanted durations, filters, bulk set, 12-hour limit); settings renamed "Use custom skill durations" / "Custom skill durations". README screenshot with the folder path blacked out. |
| 2026-09-15 | README screenshot of the real 0.5.19 → 0.5.20 full-backup comparison. |
| 2026-09-15 | Full backups: *Back up everything* to a user-chosen folder, compare with now setting by setting (backup / now / shipped), restore ticked settings; checked against the real 0.5.19 → 0.5.20 update (findings in the knowledge base). |
| 2026-09-13 | No local paths in the repository or its history: this PC's folders moved to git-ignored `CLAUDE.local.md` and `test-paths.local.json`. |
| 2026-09-20 | Rates tab: one rate for the whole world, split between drop chance and amount, with a before/after preview of a real monster using item icons read from the player's own client; clearer names for the drop chance/amount and party bonus settings. |
| 2026-09-20 | Split the rate picker and the monster browser into Rates and Drops tabs; the Drops tab compares any two sets of rates, including what the world gives right now. |
| 2026-09-20 | Drops tab: draws the monster itself, read from the model in the player's own game client (npcgrp.dat → the .ukx package → a software render); drop groups holding the same item are added up. |
| 2026-09-20 | Monsters face the viewer, are painted with their own skins (a skin name is a Shader, so its Diffuse texture is followed) and turn on the spot; the package reader and texture decoder are now shared with the item icons. |
| 2026-09-20 | Inventory: change an item's enchant level (offline, backed up, with the world's own ceiling noted), and a warning across the screen while item delivery is switched off. |
| 2026-09-21 | Checked against launcher 0.5.23 / world 1.0.146: catalog byte-identical, drop code bytecode-identical, all tests pass (notes in the knowledge base). Fixed a stray "Spoil" badge on the inventory item search results (never released). Every screenshot re-taken, the character screens from a demo world. Version 1.2.0. |
