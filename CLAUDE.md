# CLAUDE.md — working on L2Everdream Config

Read this first, then `PROJECT.md` (the full project guide) and, before touching save behaviour or file formats,
`research/L2EVERDREAM-KNOWLEDGE.md`. Everything needed to continue lives in this repository (plus the git-ignored
`CLAUDE.local.md`); per-folder assistant memory from earlier sessions does not carry over — these files replace it.

## What this is

A standalone Windows desktop app (C# / WPF, .NET 10) that edits the settings of a **locally hosted** L2Everdream world
(L2J Mobius CT_0_Interlude, protocol 746, plus L2Everdream's simulated-player layer) and the **local** Lineage 2 client,
for people who are not programmers. Visual design follows https://l2everdream.com and the official L2Everdream launcher.
The project owner knows the user is building it.

## Rules from the user (do not change without asking)

1. **Local only.** The world and client on this PC. Never the public/official L2Everdream servers. Public-server
   hardening is irrelevant (`AutoCreateAccounts=True` is correct for a local world).
2. **Editor only.** Never start, stop or control the server, database or client — that is the launcher's job. Read-only
   checks (is port listening, is `L2.exe` running) are fine for notices.
3. **Every config it can.** Server ini (game + login), `world-profile.json`, client `Option.ini` and encrypted `l2.ini`;
   XML configs and `user.ini` are next. `-Dl2sp.*` JVM flags are out of scope (the launcher regenerates them each start).
4. **Non-programmer UX.** Friendly names linked to the real `File › [Section] › Key`; grouping by type of setting, not by
   file; search across friendly and real names; a category → group section list on the left of the main window.
5. **Separate tabs:** Server, Client, **Rates**, a read-only **Custom Config** tab (how the release differs from stock L2J Mobius),
   **Characters** and **Backups**.
   - **Characters** edits player characters in the running world's database: adena, plus an inventory editor that changes,
     removes and adds items from the full item list. Existing rows are written only while the character is offline (the
     character row is locked and `online = 0` re-checked inside the writing transaction). Sims (account `$sim`) are never
     listed. **No item rows are ever inserted while the server runs** (it allocates object IDs in memory): new items are
     queued in `custom_mail` and the server's CustomMailManager delivers them when the character is online — this must
     be explained plainly in the UI, including the live state of `CustomMailManagerEnabled` and that it needs a world
     restart. The inventory limit is enforced exactly as the server counts it (`InventoryRules.cs`), including queued
     deliveries, and stacks are capped (adena by `MaxAdena`).
   - **Backups**: every settings save, character change and restore first backs up what it replaces — files, and database
     rows as they were — into one flat folder per backup (`yyyyMMdd-HHmmss-title\role__file` + `manifest.json`).
     Restores: files never while the world is running (client files never while `L2.exe` runs); database rows only for
     offline characters, while the world runs (its database only runs with it). A removed item is re-created with its
     original ID only if the world has not restarted since the backup. The user chose these rules.
   - **Full backups** (Backups tab › Full backups): one button copies every settings file — install config, launcher
     protected copies and `.shipped-baseline`, `worlds\*.json`, `world-release.json`, client `system\*.ini` — into a
     folder the user chooses (**no default**; never inside the install folder, which updates replace). Comparing a full
     backup with now is **by setting value, not by file** (updates rewrite comments), shows backup / now / shipped, and
     restores **ticked settings one value at a time** (both server copies; XML/text files whole). Same restore rules as
     above, launcher-managed values never restored, and a normal "before restore" backup is taken first.
   - **Rates**: one number (presets 1/3/5/15/20 or typed) written across every experience and drop setting, because
     Mobius's separate chance and amount multipliers confuse players. The rate is split so that `chance x amount = rate`
     while the chance never goes past the point where it is wasted (a drop group is rolled once, so chance saturates at
     100% and only amount keeps scaling); adena needs its own entry in `DropAmountMultiplierByItemId`. A monster preview
     shows before/after per kill with item icons read from the player's own client. Herbs, vitality, premium, the party
     bonus, level-gap penalties, manor and fishing are deliberately left alone.
   - **Explain how settings affect each other.** Setting cards show "depends on / has no effect right now / controls /
     works with" lines from `catalog/setting-relations.tsv` plus derived rules, evaluated live.
6. **The user chooses both folders.** No default or auto-detected server/client paths.
7. **Valid values only.** Every setting has a type, range and/or format; editors refuse bad input; Save refuses invalid
   values with a plain-language reason. The test `EveryCurrentValueIsWithinItsLimits` must keep passing.
8. **No personal names anywhere** — not the owner, not config-comment authors, not anyone — in the app, catalog, docs,
   commit messages or replies. Config comments contain attributions; `scrub_names` in `catalog/build_catalog.py`
   removes them, and editor descriptions use stock Mobius wording. Say "the owner" at most.
9. **Standalone, never an installer.** Ship one self-contained single-file exe; data files (catalog) are embedded
   resources, not side files.
10. **Keep `PROJECT.md` current.** Any change to scope, rules, files, commands, catalog numbers or UI updates the matching
    section and adds a changelog row in the same change. When the user wants to test, give the commands below.
11. Commit only when asked. Commit messages end with the Co-Authored-By trailer given by the session.

## Commands (run from the repository root)

```bash
dotnet build L2EverdreamConfig.slnx -c Debug
```
```bash
dotnet run --project src/L2Config.App
```
```bash
dotnet test tests/L2Config.Core.Tests
```
```bash
dotnet publish src/L2Config.App -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/standalone
```
```bash
dotnet publish src/L2Config.App -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish/needs-dotnet
```
```bash
python catalog/build_catalog.py
```
```bash
python catalog/build_custom_config.py "%LOCALAPPDATA%\L2Everdream"
```
```bash
python catalog/build_wiki.py
```

Regenerate the catalog after editing `catalog/*.tsv` or the generators, then rebuild (the JSON is embedded).
`build_custom_config.py` compares against the install's current files, so on a machine where settings were edited it
picks those edits up as "release changes" — review its output before committing a regenerated `custom-config.json`.

Database tests: `WorldDatabaseIntegrationTests` run only when `L2CONFIG_TEST_DB` points at a **throwaway** database with
the Mobius `characters`, `items` and `custom_mail` tables (never a world's database).
Visual checks without clicking: `L2EverdreamConfig.exe --snapshot out.png --server <dir> --client <dir> --tab server|client|custom ...`
(see PROJECT.md §2). Snapshot mode never saves.

## This machine

**Never commit machine-specific paths** (user folders, the client location, this repository's location) — not in files,
docs, tests or commit messages. They live in two git-ignored files at the repository root:

- `CLAUDE.local.md` — this PC's server and client folders, the access the user granted, tooling notes. Loaded
  automatically alongside this file. If it is missing, ask the user where the server and client are.
- `test-paths.local.json` — `{ "server": "...", "client": "..." }` for the local-install tests
  (or set `L2CONFIG_TEST_SERVER` / `L2CONFIG_TEST_CLIENT`). Tests only read those folders and write to temp copies.

The L2Everdream launcher installs to `%LOCALAPPDATA%\L2Everdream` with player data in `%LOCALAPPDATA%\L2Everdream-data`;
that is the product's standard location, not a personal path. Saving from the app writes the real files, with backups
under `%LOCALAPPDATA%\L2EverdreamConfig\backups`.

## Key facts that are easy to get wrong

- The launcher rewrites on every start: both `Database.ini` URLs, `LoginPort` (game + login `Server.ini`), login
  `AutoCreateAccounts`, `ClassMaster.xml`, client `l2.ini` `ServerAddr`. These are read-only in the app.
- The launcher keeps protected player copies in `L2Everdream-data\db\config\{game,login}` with `.shipped-baseline`
  copies and merges them over the install on start/update. The app writes both the install copy and the player copy.
- Client `l2.ini`/`user.ini` are `Lineage2Ver413` (RSA blocks + zlib + CRC32 tail); `Localization.ini`/`TTFontInfo.ini`
  are `Lineage2Ver111` (XOR 0xAC); `Option.ini` is plain. `Core/Ini/L2IniCodec.cs` reproduces the files byte-for-byte.
- The client ships `IsL2AutoLogOn=Ture` (typo); toggles treat anything but "true" as off.
- Rate semantics, read from the shipped `GameServer.jar`: experience is `template exp x RateXp`; `RatePartyXp` multiplies
  **only** the party-size bonus, not the experience. Drop chance and amount each follow an **else-if** chain (by item id,
  herb, raid, death), so a per-item rate **replaces** the general one — the shipped `DropAmountMultiplierByItemId = 57,1`
  means adena ignores the general drop amount. Each drop group is rolled once, so chance above 100% is wasted, and
  `DropMaxOccurrencesNormal`/`Raidboss` cap how many different items one kill can give.
- Item icons come from the player's own client (`systextures\Icon.utx`, `Lineage2Ver121` XOR by filename, then a UE2
  package with DXT1/DXT3 textures) - nothing is bundled. Monsters have no 2D artwork in the client, only 3D meshes.
- `SkillDurationList` (edited on its own page, max 12 h per the user) is keyed by skill id for every caster, never affects
  toggles, and is **added** to enchanted "+Time" levels 100–139 (see the knowledge base). Skills come from the datapack
  (`Skills/SkillCatalog.cs`), not the catalog.
- l2jmobius.org is behind a Cloudflare bot check — do not try to get past it; use the GitLab API
  (`gitlab.com/MobiusDevelopment/L2J_Mobius`) for upstream source. Stock configs for Custom Config are in
  `research/upstream-mobius/59097d4d05f0/`.
