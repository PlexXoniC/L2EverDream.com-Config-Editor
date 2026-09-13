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
| **Custom Config is read-only** | The tab shows how L2Everdream differs from stock L2J Mobius; it never writes. |
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

**Standalone release** (the Release configuration is preset to win-x64, self-contained, single file, compressed):

```bash
dotnet publish src/L2Config.App -c Release -o publish
```

Output is one file, `publish\L2EverdreamConfig.exe` (~60 MB). Copy it anywhere and run it; nothing else is needed.
Verified by running it alone from an empty folder.

**Developer snapshot mode** (renders the window to a PNG and exits; uses throwaway preferences and never saves):

```bash
L2EverdreamConfig.exe --snapshot out.png --server "<server folder>" --client "<client folder>" --tab server --category rates --search "party xp" --edit RateXp=3 --advanced --size 1280x820
```

`--tab` is `server`, `client` or `custom`; `--group <group id>` scrolls to a group; `--edit key=value` shows an
unsaved edit in memory.

---

## 3. Repository layout

```
L2EverdreamConfig.slnx
CLAUDE.md                        rules from the user + working notes (read first in a new session)
CLAUDE.local.md                  git-ignored: this PC's server/client folders and tooling notes
test-paths.local.json            git-ignored: server/client folders used by the local-install tests
PROJECT.md                       ← this file
catalog/                         the friendly layer (data + generators)
  build_catalog.py               → catalog.json   (names, groups, editors, limits, descriptions)
  build_custom_config.py         → custom-config.json (differences from stock L2J Mobius)
  friendly-names.tsv             hand-curated names/units/descriptions for server settings
  client-settings.tsv            hand-written client + world-profile settings
  custom-config-notes.tsv        neutral summaries for each Custom Config difference
  catalog.json, custom-config.json   generated; embedded into the exe at build time
research/
  L2EVERDREAM-KNOWLEDGE.md       how L2Everdream, the launcher, Mobius and the client work
  seed-data/                     schema extracted from the install (config keys + Java types/defaults), launcher.css, ini decoder reference
  upstream-mobius/<commit>/      stock L2J_Mobius_CT_0_Interlude config files (from GitLab) used for Custom Config
src/L2Config.Core/               no UI: catalog model, INI editing, client ini codec, stores, validation, search
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
| `Storage/BackupSession.cs` | Copies each file once before its first write to `%LOCALAPPDATA%\L2EverdreamConfig\backups\yyyyMMdd-HHmmss\…`. |
| `Storage/RuntimeStatus.cs` | Read-only: is the world listening on its game port, is `L2.exe` running. |

### App (`src/L2Config.App`)

| File | Responsibility |
|---|---|
| `App.xaml.cs` | Loads the embedded `catalog.json` + `custom-config.json`, builds the main view model; snapshot mode. |
| `MainWindow.xaml` | Chromeless window: wordmark, **Server / Client / Custom Config** tabs, folder bar, left section list with search and filters, virtualized settings list, save bar. All editor, row and page templates. |
| `ViewModels/MainViewModel.cs` | Tabs, folder choice and validation, reload, save/discard, runtime notices, "Show in editor" jump. |
| `ViewModels/TabViewModel.cs` | One Server or Client tab: categories → groups, counts, search, "Changed only", advanced filter, rows. |
| `ViewModels/SettingViewModel.cs` | One setting: value, dirty/changed state (as words), validation error, range text, undo, reset to default. |
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
- Every save first backs up each file it touches (see `BackupSession`). "Open backups" is in the save bar.

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

Editors: 750 number, 420 on/off, 98 list, 92 text, 40 choice, 7 slider, 1 secret. 359 are marked **Advanced**
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
  `word-list`, `word-list;`, `ip-list`, `host`, `regex`.
- Guard rail: the test `EveryCurrentValueIsWithinItsLimits` fails if any value in the real install or client would be
  rejected, so a limit can never be stricter than what actually ships.

### Regenerating

```bash
python catalog/build_catalog.py
```
```bash
python catalog/build_custom_config.py "%LOCALAPPDATA%\L2Everdream"
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

## 7. What the UI does

- **Tabs**: Server, Client, Custom Config. The tab pill shows a count of unsaved changes.
- **Folder bar** per tab with *Change folder…*; with no folder chosen the page is a single card with one gold button.
- **Left section list**: categories with counts; the selected category expands to its groups; clicking a group
  scrolls to it. While searching, only matching categories/groups are listed.
- **Search**: friendly names, real keys, file names and descriptions ("RateXp", "party xp", "Rates.ini" all work).
- **Filters**: *Changed only* (differs from default or edited), *Advanced*.
- **Setting card**: name, state word (*Unsaved* / *Changed*), *Set by the launcher* lock, *Advanced* tag, description,
  real-name chip, default, allowed range, the editor, error text, *Undo* and *Reset to default*.
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
| 2026-09-13 | No local paths in the repository or its history: this PC's folders moved to git-ignored `CLAUDE.local.md` and `test-paths.local.json`. |
