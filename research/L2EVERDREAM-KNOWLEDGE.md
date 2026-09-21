# L2Everdream — knowledge base for the config manager

> **Scope of the config manager: the LOCAL world (Single Player mode on this PC) and the LOCAL client only.**
> The public/official servers (Online mode, `server-directory.json`) and Friend mode are out of scope; they are
> described below only because the launcher shares code paths with them.

Surveyed 2026-09-13 from the local installs (launcher 0.5.19, world/engine 1.0.48), l2everdream.com, and the
upstream L2J Mobius repo. Everything here was read off disk or disassembled from the shipped jars (`javap`),
not guessed; places where something is inferred say so.

Seed data generated from this survey lives in [`seed-data/`](seed-data/):

| File | What it is |
|---|---|
| `server-config-schema.json` | All 1,322 ini keys (game 1,257 + login 65): file, section, current value, documented default, description comment, L2SP-annotation flag, **Java type + Java default** (from bytecode, 1,071 keys) |
| `l2sp-jvm-properties.json` | All 119 `-Dl2sp.*` system properties, the classes that read them, defaults where statically resolvable (45) |
| `l2ini_decrypt_reference.py` | Working decoder for client `Lineage2Ver413` (RSA+zlib) and `Lineage2Ver111` (XOR 0xAC) inis — port to C# |
| `launcher.css` | The launcher's complete design system (palette, every component) |
| `launcher-messages.properties` | Launcher UI copy (EN; the jar also ships es, pt_BR, ru) |

---

## 1. What L2Everdream is

- **L2Everdream** (internal codename **L2SP**, also "L2ED"/"L2PSP" in comments) = a Lineage 2 **Interlude C6**
  world with up to **7,500 simulated players ("sims")** that hunt, party, buff, run offline shops and chat.
  An economy simulation (18 regions, production graph, regional ledgers) feeds the shops.
- Server engine: **L2J Mobius `L2J_Mobius_CT_0_Interlude`** (not the older `C6_Interlude` branch), protocol
  **746**. Upstream: https://gitlab.com/MobiusDevelopment/L2J_Mobius. The shipped `GameServer.jar` build is dated
  2026-07-22, `LoginServer.jar` 2026-06-29 — so it lags upstream and carries L2SP patches
  (e.g. `RateTeleportFee`, `AutoDestroyDroppedItemAfter` notes, `patches/0016-player45-free-class-change.patch`).
- Sim layer: `l2sp-core.jar` (pure logic: combat, economy, nav/geo, placement, shops, chat, QA) and
  `l2sp-shell.jar` (Mobius integration: `EconomyConfig`, sim layers, observer HTTP). Both on GameServer's
  `Class-Path`.
- Two modes: **Single Player / Local** (launcher boots a real server on this PC) and **Online** (public world —
  `server-directory.json`: login `40.160.86.29:2106`, game `158.69.55.170:7777`), plus **Friend** (connect by IP).
- Public worlds run **1x**; offline worlds are meant to be tunable.

## 2. Install layout

### `%LOCALAPPDATA%\L2Everdream` — install root (replaced by every update; MSI major upgrade deletes it)

| Path | Contents |
|---|---|
| `L2Everdream.exe` | jpackage launcher (JavaFX); `app\L2Everdream.cfg` → main class `org.l2sp.launcher.gui.LauncherMain` |
| `app\` | launcher jar + deps (JavaFX 25, mariaDB4j 3.3.1 w/ MariaDB 10.11.5, spring-core) |
| `runtime\` | Temurin **JDK 25.0.4.1** (full JDK — `javap`, `jcmd`, `jfr` available) |
| `libs\` | `GameServer.jar`, `LoginServer.jar`, `l2sp-core.jar`, `l2sp-shell.jar`, HikariCP 7, mysql-connector-j 9.5 |
| `game\config\` | 28 Mobius config files + `Custom\` (43 ini). Working dir for GameServer |
| `game\data\` | datapack (XML stats/spawns/html/scripts ~ 80 MB), `geodata\` (754 MB, generated per client), `l2sp\` (114 MB sim data: catalogs, placement, nav zones, `economy.db`) |
| `login\config\` | Database, Interface, Network, Server, Threads .ini |
| `db\base\` | MariaDB binaries (unpacked on demand by the launcher) |
| `launch-world.ps1` | **the one boot path** (see §4) |
| `launcher-jvm-flags.txt` / `launcher-mariadb-args.txt` | arg files the launcher writes before each boot, one arg per line |
| `world-release.json` | release pin: engine version, prod SHA, jar SHA-256s |
| `tools\make-gm.ps1` | set a character's access level (100 Master / 70 Admin / 0 player) — deliberately not a button |
| `tools\Backup-L2Everdream.ps1` | fail-closed backup of player-owned files (exclusion-list based) |

### `%LOCALAPPDATA%\L2Everdream-data` — player data (survives updates)

| Path | Contents |
|---|---|
| `db\data\` | MariaDB datadir. Databases: `l2ed_login` (shared accounts, 4 tables), `l2ed_<world>_<sha8>` per world (96 tables) |
| `db\config\game\`, `db\config\login\` | **player copies** of every config + `.shipped-baseline\<file>` and `<file>.stamp` (`version=0.5.19+1.0.48`, `effective=<sha256>`) |
| `worlds\world-profile.json` | `{name, chronicle:"CT_0_Interlude", simsPercent, shopsPercent, shopsOverArmed, freeClassChange}` |
| `worlds\client-selection.json` | `{chronicle:"Interlude C6 (protocol 746)", exePath}` |
| `worlds\play-choice.json` | `{target: LOCAL/OFFICIAL/FRIEND, friendIp}` |
| `datapack\shipped-manifest.txt` | SHA manifest of shipped datapack; `overrides\`/`unclaimed\` capture player datapack edits |
| `logs\launcher.log` | launcher log — very informative about what it rewrites |
| `your-files\` | backups, `shipped-baseline.zip`, update reports |

## 3. Config ownership rules (critical for a config manager)

The launcher treats config as a three-way merge (`org.l2sp.launcher.config.PlayerConfigs`, `ConfigMerge`,
`patch.TextMerge`, `IniDocument` regex `^([A-Za-z_][A-Za-z0-9_]*)( *= *)(.*)$`):

1. **Player copy** in `L2Everdream-data\db\config\{game,login}` is authoritative for player edits.
2. **Baseline** in `.shipped-baseline\` records what was shipped, so an update can tell "you changed it" from "we changed it".
3. On Start / update, player changes are applied onto `game\config` ("Rates.ini: kept your 5 change(s)"). Edits made
   directly in the install folder are also detected and adopted (`: kept the change you made in the install folder`).
4. Tracked extensions: `.ini .xml .txt .properties .cfg` (+ many more for datapack overrides, ≤4 MB).

**Launcher-managed files — rewritten on every Start, never player-owned:**

| File | What the launcher writes |
|---|---|
| `game\config\Database.ini`, `login\config\Database.ini` | `URL = jdbc:mysql://localhost:<port>/<db>` (port 33061, or next free in 33061–33125) |
| `login\config\Server.ini` + `game\config\Server.ini` | `LoginPort` (9014 or next free, both ends kept equal); `AutoCreateAccounts = True` for self-host |
| `game\config\ClassMaster.xml` | one of two embedded docs (`runtime/ClassMaster.free.xml` / `.vanilla.xml`) per the "Free class change" toggle |
| `launcher-jvm-flags.txt` | hardcoded base flags + slider flags (below) |
| client `system\l2.ini` | `ServerAddr=` (127.0.0.1 local / official host / friend IP), decrypted and re-encrypted |

➡ A manager should **edit the player copies** (and mirror into `game\config` when the world is stopped), never
Database.ini/ClassMaster.xml/LoginPort, and should show which keys the launcher will override.

### What a launcher update does (observed 0.5.19+1.0.48 → 0.5.20+1.0.82, 2026-09-15)

Measured with SHA-256 fingerprints of the install, `L2Everdream-data` and the whole client before and after:

- The MSI replaces the install in place. 81 install files changed: jars, launcher, `launch-world.ps1`, tools, and every
  `game\config` / `login\config` file. Before replacing, the launcher's update guard copies the install to
  `your-files\backup-<stamp>\install-root\` (skipping runtime, app, geodata, `datapack.zip`, logs) and zips the player's
  edited config files (`mine/` + `base/`) to `your-files\rescue-<version>-<ms>.zip`; `last-update-report.json` records
  captured / merged / keptYours / conflicted / lost counts.
- **The new release strips almost all comments from the install config files** (`Player.ini` 880 → 360 lines, `Rates.ini`
  236 → 113; XML comment blocks removed). The launcher's protected copies and `.shipped-baseline` keep their comments, so
  install and protected copy now differ textually but not in values.
- **No setting value changed** for the player: every key in every `.ini` had the same value after the update. One shipped
  default moved in `.shipped-baseline` (`General.ini GMGiveSpecialSkills` False → True). Both `Database.ini` URLs were back
  to the stock `jdbc:mysql://localhost/l2jmobiusinterlude` until the next Start rewrites them.
- The launcher log's "we also changed X in this update and yours was kept" lines are printed on every start and list keys
  where the player's value differs from what ships — they are **not** a list of what the update changed.
- New `game\datapack.zip` (19 MB); `game\data` itself was untouched. Of 1,827 files that differ between the zip and
  `game\data`, 1,613 are comment/whitespace-only (attribution and ticket references scrubbed) and the rest are sim data
  (`l2sp\zones`, economy, catalogs); item and skill XML content is identical.
- The client was not touched. `world-profile.json` unchanged. Separately, "Launch without update" re-pointed client
  `l2.ini ServerAddr` at the public server (it follows the play mode).

Consequences for this app: compare settings by value (the full-backup comparer does), never assume comment text is
stable, and do not regenerate catalog descriptions from a post-0.5.20 install (use `seed-data/` and upstream Mobius).

### Checked after launcher 0.5.23 / world 1.0.146 (2026-09-21)

The update rewrote every `game\config` and `login\config` file but kept the player's values, including
`Custom/CustomMailManager.ini CustomMailManagerEnabled = True`, which has no protected copy. What the app depends on was re-checked:

- `python catalog/build_catalog.py` against the updated install produced a **byte-identical** `catalog.json`: no setting added,
  removed or reshaped.
- The drop calculation in `GameServer.jar` (`NpcTemplate`) is **bytecode-identical** to the 0.5.19 copy in the pre-update
  snapshot, and still reads the same `RatesConfig` fields, so the Rates and Drops arithmetic holds. `CustomMailManager` runs the same
  `SELECT * FROM custom_mail` / `DELETE … WHERE date=? AND receiver=?`.
- All tests pass against the updated install and client; the Characters tab reads the updated database. Only runtime client files
  changed (`l2.ini` from the launcher, `Option.ini` and the shader cache from the game).

## 4. Boot sequence

`LauncherApp.onStart` → patch DB/port/classmaster files → write arg files → spawn console running
`launch-world.ps1 -GameDir -LoginDir -JavaExe -JvmFlagsFile -MariaDbExe -MariaDbArgsFile`.

The script: prints release stamp → preflight (Java, jar) → **DB target check** (exit 3 unpatched stock URL,
4 contradiction with embedded port, 5 unreachable; `-ExternalDatabase` override; `-PreflightOnly` dry run) →
starts `mariadbd.exe` (`--datadir --port=33061 --bind-address=127.0.0.1`) → LoginServer
(`java -server -Dfile.encoding=UTF-8 -jar ../libs/LoginServer.jar`, checks ports 9014/2106, names the owning
process if busy) → GameServer (`<flags> -jar ../libs/GameServer.jar`) → waits for `GameserverPort` 7777 (180 s)
→ reads pulse `http://127.0.0.1:8129/economy/pulse` → hides its console → when GameServer exits, kills login
then DB. Logs: `{game,login}\log\stdout.log` / `stderr.log` rotated per boot; Mobius logs `log\java0.log.0`.
Graceful shutdown is from inside Mobius (GM shutdown); the launcher's Stop is a reverse-order hard stop.

Exit codes: 0 ok · 1 world didn't come up · 2 preflight · 3 DB unpatched · 4 DB contradiction · 5 DB unreachable.

**JVM flags actually shipped** (`launcher-jvm-flags.txt`): `-server -Dfile.encoding=UTF-8 -XX:+UseZGC
-Dl2sp.world.activesims=true -Dl2sp.economy.demandpricing=true -Xmx16g -XX:SoftMaxHeapSize=8g
-Dl2sp.region.host.slider=<sims> -Dl2sp.world.shopfraction=<shops>`. Only the last two come from UI sliders
(`MeterMapping`: SIMS → host.slider, min 25%, no over-range; SHOPS → shopfraction 0.5 at 100%, up to 200% when
"Allow more than we ship" is armed). The launcher regenerates this file every Start, so **any other `-Dl2sp.*`
tuning cannot persist through the official launcher** — a manager wanting those must boot via
`launch-world.ps1` with its own `-JvmFlagsFile` (the script is explicitly designed for that).

Memory model (`MachineFit`): RSS ≈ SoftMaxHeap + ~1 GB non-heap; 2 GB host reserve; min bootable heap 2 GB;
shipped heap target 8 GB. Measured on a dev world: 7,788 bodies ≈ 9.7 GB RSS, ~8.9/16 cores.

## 5. Ports

| Port | Owner | Configured in |
|---|---|---|
| 2106 | LoginServer ← clients | login `Server.ini` `LoginserverPort` |
| 9014 | LoginServer ← GameServer | login+game `Server.ini` `LoginPort` (launcher-managed) |
| 7777 | GameServer ← clients | game `Server.ini` `GameserverPort` |
| 33061 | embedded MariaDB (localhost) | `launcher-mariadb-args.txt`, both `Database.ini` |
| 8129 | L2SP debug observer HTTP (`/economy/pulse`, `/qa/tick`, `/qa/fields`) | `-Dl2sp.debug.observer.port` |

## 6. Server config — Mobius CT_0_Interlude

Files (loader class): `Server.ini` (ServerConfig), `Rates.ini` (RatesConfig), `General.ini`, `Player.ini`,
`NPC.ini`, `Feature.ini`, `PVP.ini`, `Olympiad.ini`, `GrandBoss.ini`, `Siege.ini` (read by SiegeManager, keys
built from castle names), `ConquerableHallSiege.ini`, `FloodProtector.ini` (keys built as prefix+suffix),
`GeoEngine.ini`, `IdManager.ini`, `Development.ini`, `Network.ini`, `Threads.ini`, `Interface.ini`,
`Database.ini`, and 43 `Custom\*.ini` (AutoPlay, ChampionMonsters, OfflineTrade/Play, PremiumSystem,
SchemeBuffer, FactionSystem, FakePlayers, Wedding, Transmog, …). XML configs: `AccessLevels.xml`,
`AdminCommands.xml`, `ClassMaster.xml`, `DynamicExpRates.xml`, `Scripts.xml`, `SiegeSchedule.xml`,
`default-ipconfig.xml`; plus `chatfilter.txt`, `hexid.txt`.

Format: `Key = Value`, `#` comments, sections are `# ----` / `# Title` / `# ----` banners, per-key
`# Default: x` lines (sometimes `# Retail: x`), lists use `;` and `,` (e.g. `itemId,mult;itemId,mult`),
`\` line continuation. Types via `ConfigReader.getBoolean/Byte/Int/Long/Float/Double/String`. The comment
format is inconsistent enough that section detection needs heuristics (some descriptions start with a banner).

**`Player.ini SkillDurationList`** (read by `PlayerConfig`: split on `;`, then `,` into id → seconds; bad entries are logged
and skipped, a repeated id keeps the last value). Applied in the `Skill(StatSet)` constructor when `EnableModifySkillDuration` is
on, the id is listed and the skill is not a toggle (`operateType T`): levels below 100 and above 140 get the listed seconds as
`abnormalTime`; levels 100–139 (the "+Time" enchant route) get the listed seconds **added** to their enchanted `abnormalTime`.
It is keyed by skill id, so it changes the skill for every caster (players, sims, NPCs, monsters). In the 1.0.82 datapack 983
skills have an `abnormalTime` (none of them toggles): 347 in player skill trees (`stats\players\skillTrees`), all 85
`SchemeBufferSkills.xml` buffs, 443 flagged `isDebuff`; songs/dances are `abnormalType SONG_*` / `DANCE_*`. Durations are
seconds; `abnormalTime` may be a `#table` per level and enchant routes use `<enchantN name="abnormalTime">`. The app caps
listed durations at 12 hours.

Notable L2Everdream deviations from Mobius defaults (current install):

| Setting | Value | Why (from comments) |
|---|---|---|
| Rates `RateXp/Sp/PartyXp/PartySp/DropManor` | **15** on this machine (player edit; ships 1) | launcher log: "kept your 5 change(s)" |
| Rates `RateTeleportFee` | 0.5 | L2SP-added key, launch-week discount (ECON-75) |
| Player `MaxFreeTeleportLevel` | 0 | nothing free; retail Interlude charged all levels |
| Player `AltLeavePartyLeader` | True | leader leaving must not disband sim parties (PLAYER-17) |
| Player `AltNewCharAlwaysIsNewbie` | True | dev convenience; public profile expects False |
| General `AutoDestroyDroppedItemAfter` | 3600 | drops stay an hour (ECON-163) |
| General `AllowCursedWeapons` | False | until the mechanic is implemented (COMBAT-560) |
| General `Custom*Load` | True | custom NPC/teleport/skills/items/multisell/buylists |
| Server `PacketEncryption` | False | perf |
| NPC `MaxAggroRange` 450, `AggroDistanceCheckEnabled` True, `AltGameViewNpc` True | | |
| Network `BufferPool.AutoExpandCapacity` False, `BufferPool.Huge.Size` 4, `DropPacketThreshold` 2500 | | sized for thousands of sims |
| Login `AutoCreateAccounts` | True | correct for a local world (launcher enforces it via `SelfHostLoginPatch`); only a public login server needs False — out of scope here |

## 7. L2SP JVM properties (`-Dl2sp.*`)

119 total (see seed JSON). The meaningful tuning set lives in `org.l2sp.shell.EconomyConfig`:

- **World**: `world.activesims`(true) `world.pop`(7500) `world.shopfraction`(0.5) `world.multitown`(true)
  `world.level.center`(band) `world.level.spread`(16) `world.town.floor`(25) `region.host.slider`(1.0)
  `region.embodiment.radius`(9000) `region.zone.floor`(2) `region.sim.spawn`(true) `field.migration`(true)
  `sim.count`(380 default via resolver) `sim.invulnerable`(false) `sevensigns.sim`(true)
- **Economy**: `economy.demandpricing`(false; launcher sets true) `economy.vendorsink`(1.0) `economy.maturity`("genesis")
  `economy.craftershare`(25%) `economy.spellbookresale` `shotsupply` `crystalsupply` `passingtrade`(all true)
  `world.demand.leasescans`(0) `world.demand.idlefraction`(0.3) `shop.lightbody`(true)
- **Autoscale**: `autoscale.enabled`(true) `autoscale.costperplayer`(3) `autoscale.floorfraction`(0.5)
- **Chat**: `chat.enabled`(true) `chat.scan.ms`(4000) `chat.say.permin`(12) `chat.trade.permin`(7)
  `chat.shout.permin`(4) `chat.greet.permin`(5) `chat.shout.cityband`(55) `chat.speaker.cooldown`(5)
- **Party/combat**: `combat.derivedcomp` `combat.rolepool` `party.escort.pickup`(true) `party.escort.trashdelete`
  `party.escort.caststuckrepair`(false) `party.invite.maxAbove/maxBelow` `combat.poolSize/poolTickMs`
  `reflex.poolSize/tickMs`
- **Player features**: `player.commands`(true) `player.autohunt` `player.raidcam`
- **Debug**: `debug.observer`, `debug.observer.port`(8129), `dev.nameplate`(true)
- **Data dirs** (`*.dir`, `nav.dir`, `placement.*`) and **launcher-only** (`launcher.heapgb`, `launcher.lang`,
  `launcher.profiledir`, `launcher.manifestUrl`, …).

The launcher also carries a `KnobRegistry` (PLAYER tier: `world-sims`, `offline-shops`; DEV tier: 13 compiled
constants such as `party-size-bias` 1.5, `comfortable-margin` 1.4, `heal-threshold` 0.75, `seating-scale-a/s`
0.55) — these DEV knobs are **source constants, not runtime-settable**; useful as documentation only.

## 8. Database

Embedded MariaDB 10.11.5, root / no password, localhost only. Schema = Mobius `db_installer` DDL vendored in the
launcher jar (`db/index.txt`, login first). Login DB: `accounts` (login, password hash, accessLevel, lastIP…),
`account_data`, `accounts_ipauth`, `gameservers`. World DB: 96 tables — `characters` (charId, char_name,
level, exp, sp, x/y/z, accesslevel, classid, online…), `items`, `character_skills`, `clan_data`, `castle`,
`olympiad_*`, `grandboss_data`, `seven_signs*`, `punishments`, `announcements`, `global_variables`, etc.
World DB name = `l2ed_` + slug + `_` + first 8 hex of SHA-256 (`WorldDatabaseName`). The DB only runs while
the world runs (make-gm.ps1 reads the port from game `Database.ini`).

## 9. Client (Interlude C6 install, 6.1 GB, 1,460 files)

Interlude C6 (protocol 746). The launcher identifies chronicles by marker files (`ChronicleProfile`: Interlude
requires `l2.exe core.u engine.u interface.xdat …`, forbids `hairaccessarygrp.dat`, `l2cefsubprocess.exe`, …;
also knows Classic and Gracia/High Five). Client needs **admin elevation** (launch error 740 → relaunch via
elevated PowerShell).

`system\` ini files:

| File | Encoding | Contents |
|---|---|---|
| `l2.ini` | Lineage2Ver413 | `[URL] ServerAddr, Port=7777`, `[FontSet]`, `[ClippingRange]`, `[AutoLogOn]` (L2ID/L2Passwd — treat as secret), `[Engine.Engine]`, `[WinDrv.WindowsClient]` resolutions |
| `user.ini` | Lineage2Ver413 | keybinds / input aliases |
| `Option.ini` | plain | `[Video]` resolution, texture/model detail, gamma, AA, fullscreen; `[Game]` name display, chat, loot mode; `[Audio]` volumes; `[ClippingRange]` |
| `WindowsInfo.ini` | plain | UI window positions |
| `chatfilter.ini`, `s_info.ini` | plain | chat tab filters, misc |
| `Localization.ini`, `TTFontInfo.ini` | Lineage2Ver111 (XOR 0xAC) | language set, fonts |
| `Lineage2us.ini` | no header — unknown/binary | not decoded |

`L2IniCrypt` (launcher) implements 413 both ways: RSA-1024 blocks of 128 bytes (124-byte body), public exponent
0x1D, private exponent in the jar, zlib payload with 4-byte LE length prefix, 20-byte tail with CRC32 at offset 12.

## 10. Design system (for WPF)

**Launcher (`launcher.css`, "the one theme", v12)** — no OS titlebar (own ✕/– chrome), dark twilight gradient.

| Token | Hex | Use |
|---|---|---|
| twilight | `#1b1636` | background top |
| deep | `#12101f` | background bottom, inputs |
| wisp | `#7c6fb0` | secondary text, idle dots |
| lantern | `#e8a24c` | THE one lit action, warnings/offline (amber, never red) |
| ember | `#f6c67a` | primary gradient top, accents |
| starlight | `#f4f1fa` | primary text |
| verdance | `#8fb7a6` | online/running/done |
| arcane | `#9d8cf0` | "yours / selected / adjustable" |
| label lavender | `#a99cd4`, `#8478b4`, `#9d90c8`, `#b3a7d6` | kickers, captions |
| on-lantern ink | `#2a1c07` | text on gold buttons |
| hair | `rgba(124,111,176,.28)` | hairline borders |
| card | `rgba(42,35,80,.34)` | card fill |
| wordmark | L2 `#e8a24c` · Ever `#f5f6ff` · dream gradient `#8f93f5→#8188ff` | Georgia bold |

Components: primary button = vertical gradient ember→lantern, radius 10–14, bold dark ink; ghost button =
transparent + hairline, hover border `rgba(232,162,76,.45)`; cards radius 13–16 with violet hairline
`rgba(157,140,240,.42)` and gradient `rgba(42,35,80,.55)→rgba(26,22,54,.6)`; overlays `rgba(18,15,32,.984)`
over backdrop `rgba(6,5,12,.62)`; flyout options with 2px arcane left marker when selected; pill badges radius
999; sliders = 5px track `rgba(124,111,176,.28)` + 16px arcane thumb with glow; meters show value chip green at
recommended / amber above / arcane below; lock + checkbox 26×26 radius 8 (ember = official/sealed, arcane =
tuned). Type: Georgia (display/titles), system UI for body, Consolas for logs/stream. Rules written in the CSS:
**one lit (gold) action per screen**, status never color-only (always a word), no effects on text-bearing
containers (shadow on a sibling behind), amber warns but doesn't block.

**Website (Astro, `/_astro/index.*.css`)** — `--void #0b0a18` body, `--deep #100e22`, `--field #12102a`,
`--surface #1a1740`, `--twilight #1b1636`, `--starlight #f5f6ff`, `--body #c9ccea`, `--greyple #9096b8`,
`--arcane #9d8cf0`, `--ember #f6c67a`, `--lantern #f0b67a`, `--verdance #93c0ac`, `--label-ink #a18a71`,
`--blurple #5865f2` (Discord), `--line #8894f726`, `--radius 16px`, wrap 1360px. Fonts: serif "Cormorant
Garamond"/Georgia, sans system-ui/Segoe UI/Inter, mono "JetBrains Mono"/Consolas. Nav: Server Features, Wiki,
Dream Log, Download, Account (amber pill).

## 11. Gameplay concepts worth surfacing in the UI

Player commands (voice commands): `.help .dashboard .buffme .invite .inviteme .dismiss .party .role .assist
.simrole .smartroam .nearby .crystallize`. Sims tick on a shared 700 ms beat (median pass ~233 ms). Invite band
±9 levels. Shops' signs are ≤29 chars. Wiki: https://l2everdream.com/wiki.html.

## 12. Related projects

- L2J Mobius (upstream, all chronicles): https://gitlab.com/MobiusDevelopment/L2J_Mobius — forum/wiki at
  l2jmobius.org (Cloudflare-protected; not scraped).
- elberacasa/l2j-interlude-toolkit — Python headless client, geodata scanner, web dashboard for Mobius Interlude.
- l2encdec / L2 ini crypt tools (lineage of the 413/111 formats implemented by `L2IniCrypt`).

## 13. Open questions / not yet verified

- Exact `WorldSettings.hostSlider()` math: shipped flags show `host.slider=0.5` at simsPercent 100 while
  `MeterMapping.SIMS.atFull` is 1.0 — likely a scale applied in `hostSlider()`; verify before writing that flag.
- Whether GameServer supports hot config reload (Mobius has `//reload config` admin commands for some files) —
  would allow "apply without restart".
- `Lineage2us.ini` format.
- Live DB contents (world was stopped during the survey).
