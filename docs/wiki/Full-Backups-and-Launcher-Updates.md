# Full backups and launcher updates

When the L2Everdream launcher updates, it replaces the whole install folder and rewrites every config file. It tries hard to keep your
changes, but it warns that hand-edited files are "not yet guaranteed to survive every update". **Full backups** let you check what an
update did, and put back anything you want.

![Comparing a full backup with the files after an update](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/full-backup-compare.png)

## The routine

1. **Before updating:** open **Backups › Full backups** and click **Back up everything**.
2. **Update** in the launcher. Don't start the world or the game yet if you want the cleanest comparison.
3. **After updating:** open the backup and click **Compare with now**.
4. **Tick** any settings you want back and click **Restore**.
5. Start your world.

## Choosing where full backups go

There is no default. The first time you click **Back up everything**, you choose a folder; it's remembered, and **Change folder…**
picks another.

- Choose a folder **outside** the L2Everdream install folder (`%LOCALAPPDATA%\L2Everdream`). The install folder is refused, because
  updates replace it and would delete the backups meant to protect you.
- The client's `system` folder is refused too.
- Good choices: a *L2Everdream backups* folder in Documents, on another drive, or in a cloud-synced folder.

## What a full backup contains

About 190 files and under 1 MB, taken in a couple of seconds:

| Part | From |
|---|---|
| Game and login server config | `L2Everdream\game\config\**`, `L2Everdream\login\config\**` (all `.ini`, `.xml`, `.txt` files) |
| Your protected copies | `L2Everdream-data\db\config\game` and `\login`: the launcher's copies of your settings |
| What L2Everdream ships | the launcher's `.shipped-baseline` copies |
| World settings | `L2Everdream-data\worlds\*.json` |
| Version stamp | `L2Everdream\world-release.json` (launcher and engine versions) |
| Client settings | the client's `system\*.ini` |

It does **not** contain your world's database or the game's program files; the launcher backs those up itself when it updates.

Each backup is one folder, e.g. `20260915-052721-full-backup-0-5-19`, with the files named `role__file` and a `full-backup.json`
manifest (versions, where each file came from, SHA-256 fingerprints). It is written under a temporary name and only appears when complete.

## The list

Each full backup shows its date, the **L2Everdream version** it was taken on, how many files and how big, and which parts it covers. If
L2Everdream has been updated since, it says so: *L2Everdream has been updated since this backup: 0.5.19 → 0.5.20.*

## Comparing with now

The comparison goes **setting by setting, not file by file**. Updates often rewrite files without changing any value (0.5.20 removed
almost every comment from the config files), and a file comparison would bury the real changes.

The header says whether L2Everdream was updated since the backup, and sums up the result.

| Filter | Shows |
|---|---|
| **Different from the backup** (default) | settings whose value is different now, or that are no longer in the file |
| **Shipped defaults changed** | your value is the same, but what L2Everdream ships for it changed (for your information) |
| **New since the backup** | settings that weren't in the backup, usually added by the update |
| **Everything** | all of the above |

Each row shows the friendly name (or the real key for settings the editor doesn't list), where it lives, and three values:
**IN THE BACKUP**, **NOW** and **L2EVERDREAM SHIPS** (before → now when that changed). Search works on names, keys and files.

- Files where **only comments or blank lines** changed are summed up in one line: nothing to restore.
- **XML and text files** whose content changed appear under *Whole files* with the changed lines.
- If a backup copy was changed since it was taken, a warning says so and that copy isn't used.

## Restoring chosen settings

Tick the settings you want back (**Tick all shown** ticks every restorable row currently shown), then click **Restore N settings**. You
see a plan and confirm.

- **Only the ticked values are written.** Comments, new settings from the update, and everything else in the file stay as they are.
- Server settings are written to **both** the install and the launcher's protected copy, so the launcher keeps them.
- Whole XML/text files are put back as they were in the backup.
- **Not while running:** server and world settings aren't restored while your world runs; client settings aren't restored while
  Lineage 2 is open. The panel tells you.
- **Can't be ticked:** values the launcher sets itself (database address, login port, `ClassMaster.xml`…), settings that weren't in the
  backup, shipped-default-only changes, and values the editor wouldn't accept. Each says why.
- What gets replaced is saved first as a normal **Before restore** change backup, so the restore can be undone.

Each ticked setting then shows its result, and the comparison refreshes.

## What the 0.5.20 update did (example)

A real comparison of 0.5.19 (engine 1.0.48) against 0.5.20 (engine 1.0.82), taken on a world with many customised settings:

- **No setting value was lost or changed.** The launcher kept every customised value.
- **One shipped default changed:** *Give GMs the special GM skills* now ships On.
- **Comments were stripped** from almost every server config file (`Player.ini` went from 880 lines to 360). The launcher's protected
  copies kept theirs. The comparison shows 42 files as "comments only".
- Both `Database.ini` connection addresses were reset to the stock value until the next world start, when the launcher rewrites them.
- The client wasn't touched.

## Good to know

- Full backups are only taken when you click the button. The program can't tell that an update is waiting (only the launcher checks).
- The launcher's own update messages such as *"we also changed X in this update and yours was kept"* appear on every start. They list
  settings where your value differs from what ships, not what the update changed. The comparison tells you what actually changed.
