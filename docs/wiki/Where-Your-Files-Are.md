# Where your files are

## What each tab reads and writes

| Settings | Read from | Written to |
|---|---|---|
| Game server `.ini` | your protected copy `L2Everdream-data\db\config\game\<file>` if it exists, otherwise `L2Everdream\game\config\<file>` | **both** the install copy and the protected copy (when it exists) |
| Login server `.ini` | `L2Everdream-data\db\config\login\<file>`, otherwise `L2Everdream\login\config\<file>` | **both** |
| World settings | `L2Everdream-data\worlds\world-profile.json` | the same file (unknown fields and value types are kept) |
| Client `Option.ini` | `<client>\system\Option.ini` | the same file |
| Client `l2.ini` (encrypted) | decoded in memory | re-encoded, checked by decoding again, then written |
| Characters | the world's database, using the address in `L2Everdream\game\config\Database.ini` | the database (never new item rows) and the delivery queue |

With the official launcher, `L2Everdream` is `%LOCALAPPDATA%\L2Everdream` and `L2Everdream-data` is `%LOCALAPPDATA%\L2Everdream-data`.

## Why two copies of the server settings

The launcher protects your settings from updates by keeping its own **player copy** of each config file in `L2Everdream-data\db\config`,
plus a `.shipped-baseline` copy of what L2Everdream shipped. On every start and update it merges your copy back over the install, so it
can tell "you changed this" from "we changed this".

Writing **both** copies means:

- the change is used on the next start, and
- the launcher sees it as *your* change and keeps it through updates.

## Values the launcher owns

The launcher rewrites these on every start, so they're shown locked and never restored:

| Value | Written by the launcher from |
|---|---|
| Database address (`URL`) in `game\config\Database.ini` and `login\config\Database.ini` | the embedded database it starts |
| `LoginPort` in the game and login `Server.ini` | a free port it picks |
| Login `AutoCreateAccounts` | always True for a world on your PC |
| `game\config\ClassMaster.xml` | its **Free class change** option |
| Client `l2.ini` `ServerAddr` | the play mode: your PC, the public server or a friend |

## The program's own files

Everything the program keeps for itself is in `%LOCALAPPDATA%\L2EverdreamConfig`:

| Path | What |
|---|---|
| `settings.json` | your folder choices, last tab, Advanced filter, full backup folder |
| `backups\` | automatic change backups |

Full backups go to the folder you choose. Nothing is written to the registry.

## Writing safely

- Only the values you change are rewritten. Comments, blank lines, spacing, order, line endings and the file's encoding stay as they
  were. A setting missing from a file is added at the end of its section.
- Each file is written to a temporary file first and then swapped in, so a crash or power cut can't leave a half-written file.
- Every file is backed up before its first write.
