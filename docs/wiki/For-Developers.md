# For developers

L2Everdream Config is C# / WPF on .NET 10. The only third-party runtime package is
[MySqlConnector](https://mysqlconnector.net/) (MIT). Tests use [xUnit](https://xunit.net/).

## Build, test, run

Requires Windows and the [.NET 10 SDK](https://dotnet.microsoft.com/download). Python 3.11+ is only needed to regenerate the settings
catalog.

```
git clone https://github.com/PlexXoniC/L2EverDream.com-Config-Editor.git
cd L2EverDream.com-Config-Editor
dotnet build L2EverdreamConfig.slnx
dotnet test tests/L2Config.Core.Tests
dotnet run --project src/L2Config.App
```

## Projects

| Project | What it is |
|---|---|
| `src/L2Config.Core` | No UI. Catalog model and search, line-preserving INI editor, client ini codec, settings store with validation and dual-copy saves, change backups and restore, full backups (create, compare by value, restore), world database access, item and skill catalogs, inventory rules |
| `src/L2Config.App` | The WPF application (MVVM, no UI libraries), styled after the L2Everdream site and launcher |
| `tests/L2Config.Core.Tests` | xUnit tests; some need a local install (see below) |
| `catalog/` | Hand-curated names and relations (`*.tsv`), the Python generators, and the generated `catalog.json` and `custom-config.json` |
| `research/` | How L2Everdream, its launcher, L2J Mobius and the client work (`L2EVERDREAM-KNOWLEDGE.md`), extracted schema data, stock Mobius configs |
| `docs/images/` | README and wiki screenshots |
| `docs/wiki/` | The source of this wiki |

`PROJECT.md` in the repository is the full project guide: rules, architecture, save and restore behaviour, catalog rules, design tokens,
known limitations and the changelog.

## Release builds

Both produce a single `L2EverdreamConfig.exe` with the catalog embedded and no side files:

```
dotnet publish src/L2Config.App -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/standalone

dotnet publish src/L2Config.App -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish/needs-dotnet
```

Release process:

1. Set `<Version>` in `src/L2Config.App/L2Config.App.csproj`, commit and push. Build after the final commit, because the exe's version
   records the commit.
2. Run both publish commands. Zip each exe with `LICENSE.txt` as `L2EverdreamConfig-<version>-win-x64-standalone.zip` and
   `…-needs-dotnet.zip`.
3. Get the SHA-256 hashes (`certutil -hashfile <zip> SHA256`) and put them in the release notes.
4. Create the GitHub release with tag `v<version>`, paste the notes, attach both zips.

## The settings catalog

Everything the Server and Client tabs show comes from `catalog/catalog.json`: 1,408 settings in 20 categories, each with name,
description, editor, type, limits, format, default and relations.

```
python catalog/build_catalog.py
python catalog/build_custom_config.py "%LOCALAPPDATA%\L2Everdream"
```

- `friendly-names.tsv`: hand-curated names, units and descriptions. Large families get pattern names; the rest are humanised from the key.
- `client-settings.tsv`: client and world-profile settings. `setting-relations.tsv`: depends-on and works-with links.
  `custom-config-notes.tsv`: Custom Config summaries.
- Descriptions use stock L2J Mobius comments wherever a setting exists upstream. Attributions in config comments are stripped. Names of
  people never appear anywhere.
- `build_catalog.py` reads the committed seed data, not the live install. `build_custom_config.py` compares against an install's
  *current* files, so on an install with edited settings those edits show up as release changes. Review its output before committing.
- Rebuild the app after regenerating; the JSON is embedded.

## Local-install and database tests

- Tests marked `LocalInstallFact` need a real install and client. Put `{ "server": "…", "client": "…" }` in a git-ignored
  `test-paths.local.json` at the repository root, or set `L2CONFIG_TEST_SERVER` / `L2CONFIG_TEST_CLIENT`. They only read those folders
  and write to temporary copies; without them they're skipped.
- `WorldDatabaseIntegrationTests` run only when `L2CONFIG_TEST_DB` is a connection string for a **throwaway** MariaDB/MySQL database
  with the Mobius `characters`, `items` and `custom_mail` tables. Never point it at a real world.
- Never commit machine-specific paths: not in files, tests, docs or commit messages.

## Snapshot mode

Renders the window to a PNG and exits, with throwaway preferences, and never saves:

```
L2EverdreamConfig.exe --snapshot out.png --server <dir> --client <dir> --tab server|client|custom|characters|backups
  [--category id] [--group id] [--search text] [--edit Key=value] [--advanced] [--size 1280x820]
  [--inventory <character> --item-search text --item <id> --amount n]
  [--backups <dir>] [--backups-mode full] [--full-backups <dir> --compare <backup folder|latest> --filter changed|shipped|new|all]
  [--skill-durations --filter players|songs|debuffs|npc|changed|all]
```

Screenshots for the README and wiki must not show real folder paths, account or character names. Screens that show neither can come from
a real install. When the folder bar is the only private detail, cover the path with a solid black bar. Screens with characters come
from a demo world with made-up characters (the procedure is in `PROJECT.md`).

## How key things work

- **Client ini formats:** `l2.ini` and `user.ini` are `Lineage2Ver413` (RSA-1024 blocks, zlib, CRC32 tail). `Localization.ini` and
  `TTFontInfo.ini` are `Lineage2Ver111` (XOR `0xAC`). `Option.ini` is plain. The codec reproduces shipped files byte for byte.
- **Dual-copy saves:** server settings go to the install and the launcher's protected copy in `L2Everdream-data\db\config`.
- **Items:** never inserted while the server runs (it allocates object IDs in memory). New items go into `custom_mail`, which the server's
  CustomMailManager delivers. Existing rows change only inside a transaction that locks the character row and re-checks `online = 0`.
- **Full backups** compare by value: server ini (protected copy preferred, shipped baseline before/now), world-profile fields, decoded
  client ini, and XML/text with comments ignored. Restores write single values through the same file classes as saving.
- **`SkillDurationList`** is keyed by skill ID for every caster, skips toggles, and for enchanted "+Time" levels 100–139 adds the listed
  seconds instead of replacing the duration. The skill list is read from the datapack.

## Updating this wiki

The wiki's source is `docs/wiki/` in the main repository. Page file names are wiki page names (`Server-Settings.md` → *Server
Settings*); `_Sidebar.md` and `_Footer.md` appear on every page. Images link to `docs/images/` on the main branch. To publish:

1. The first time only, create one page on GitHub (**Wiki › Create the first page** › **Save page**) so the wiki repository exists.
2. Clone it next to the main repository, copy the pages over, and push:

```
git clone https://github.com/PlexXoniC/L2EverDream.com-Config-Editor.wiki.git
copy L2EverDream.com-Config-Editor\docs\wiki\*.md L2EverDream.com-Config-Editor.wiki\
cd L2EverDream.com-Config-Editor.wiki
git add -A
git commit -m "Update wiki"
git push
```
