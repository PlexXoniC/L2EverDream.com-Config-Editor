# Backups and restore

**Everything is backed up before it is changed.** You don't have to do anything: before every settings save, every adena or item
change, every delivery queued or cancelled, and every restore, the program copies what it is about to replace.

The **Backups** tab has two views. **Change backups** (this page) are the automatic ones. **Full backups** copy everything at once, for
launcher updates; see [Full backups and launcher updates](Full-Backups-and-Launcher-Updates).

![The Backups tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/backups.png)

## Where they are

`%LOCALAPPDATA%\L2EverdreamConfig\backups`, one folder per backup, named by date, time and what it was:

```
20260914-101500-saved-3-settings\
  manifest.json                 what's inside, where each file came from, its SHA-256 fingerprint, what changed from → to
  game-config__Rates.ini        the game server's copy, as it was
  game-player-copy__Rates.ini   the launcher's protected copy, as it was
  client__l2.ini
  db-01__items.json             database rows as they were (readable JSON)
```

Files are named `role__file`, so there are never deep folders. **Open backups folder** opens it in Explorer.

| Kind badge | Taken before |
|---|---|
| **Settings** | Save changes |
| **Characters** | an adena change, a count change or removal, a queued or cancelled delivery |
| **Before restore** | every restore, so a restore can be undone |
| **Older format** | backups made by the very first version of the program, which can still be restored |

Each backup lists what changed (for example `Experience (XP) rate (Rates.ini › RateXp): 1 → 3`) and what it contains.

## Restoring

Click **Restore…** on a backup. First you see a plan saying what will and won't be restored right now, and why. After confirming, each
item shows its result.

Restores follow strict rules so they never clash with a running game:

| Backup of | Restored only when |
|---|---|
| Server and world settings | your **world is stopped** |
| Client settings | **Lineage 2 is closed** |
| Character changes | your **world is running** (its database only runs with it) and the character is **logged out** |

- Each file's SHA-256 fingerprint is checked first. A backup copy that has been changed since it was taken is not used.
- Database rows from a different world's database are refused.
- **Updated rows** (adena, counts) are put back as they were.
- **Removed items** are re-created with their original identity only if the world hasn't been restarted since the backup, because the
  server may have given that identity to something else. Otherwise the result says so.
- **Queued deliveries** are withdrawn if they haven't been delivered yet. **Cancelled deliveries** are queued again if they still fit
  in the inventory.
- Everything the restore replaces goes into a new **Before restore** backup, so you can undo the restore the same way.

If a restore can't happen right now, the card says why, e.g. *Stop the world from the launcher to restore these files.*

## Deleting backups

The program never deletes backups. They're small, but you can delete old folders from Explorer whenever you like.
