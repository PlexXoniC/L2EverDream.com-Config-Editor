# Privacy and safety

## It never connects to the internet

No update checks, no analytics, no telemetry, and no fonts or images loaded from the web. Its only network connections are to **your own
computer** (`127.0.0.1`):

- a quick check whether your world's game port is open, so it can tell you whether the world is running
- the world's database, for the Characters tab, using the address in your own server's `Database.ini`

It creates no account and asks for no password. The database login it uses is the one already in your server's config. It also checks
whether `L2.exe` is running, by looking at the process list.

## What it will and won't do

| It will | It won't |
|---|---|
| Edit the server and client **on this PC** | Connect to, or change, the public L2Everdream servers |
| Check whether your world is running and whether Lineage 2 is open, to warn you | Start, stop or restart your world, its database or the game |
| Save server settings while the world runs (they apply on the next start) | Save client settings while Lineage 2 is running |
| Change adena and items of logged-out player characters | Touch simulated players, or change characters who are logged in |
| Queue new items for the server to deliver | Write new item rows into the database while the server runs |
| Back up everything before every change, and take full backups when you ask | Delete your backups |
| Refuse values the server would reject, with a reason | Let an invalid value reach a config file |
| Show values the launcher owns | Change them (the launcher would overwrite them anyway) |
| Restore backups | Restore files while the world runs, or client files while the game is open |

## Where it writes

Only to:

- the config files of the server and client folders **you** chose, when you click **Save changes** or restore
- the world's database, when you confirm a character change or restore
- `%LOCALAPPDATA%\L2EverdreamConfig` (preferences and change backups)
- the full backup folder **you** chose

Nothing goes to the registry, and nothing is installed.

## Secrets

The client's saved automatic-login password (`l2.ini`) is shown in a password box. In full backup comparisons, password values are hidden.
Backups are plain copies of your files, so they contain whatever those files contain. Keep your backup folders private.

## Open source

The full source is on [GitHub](https://github.com/PlexXoniC/L2EverDream.com-Config-Editor) under the MIT licence. Release downloads list
SHA-256 fingerprints so you can check a download is the published one.
