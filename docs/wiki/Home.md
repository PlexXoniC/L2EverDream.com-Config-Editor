# L2Everdream Config

**Change every setting of your own L2Everdream world and your Lineage 2 client, without opening a single config file.**

L2Everdream Config is a free Windows program for people who run [L2Everdream](https://l2everdream.com) on their own PC. It gathers the
world's 1,322 server settings, the launcher's world settings and the client's settings into one window. Every setting has a plain-English
name, is grouped by what it does, and is limited to values the server accepts.

![The Server tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/server.png)

> **For your local world only.** It edits the world and the client **on your own computer**. It cannot touch the public L2Everdream
> servers, and it never starts, stops or controls your world, its database or the game. That stays the launcher's job.
>
> This is an independent tool. It is not an official L2Everdream product.

## Start here

1. [Installation and first run](Installation-and-First-Run): download, run, choose your folders.
2. [Server settings](Server-Settings): find, change and save a setting.
3. Before you update L2Everdream, read [Full backups and launcher updates](Full-Backups-and-Launcher-Updates).

## Guides

| Page | What it covers |
|---|---|
| [Installation and first run](Installation-and-First-Run) | Which download to pick, Windows SmartScreen, choosing the server and client folders, updating the program |
| [Server settings](Server-Settings) | The section list, search, filters, setting cards, valid values, saving, when changes take effect |
| [Custom skill durations](Custom-Skill-Durations) | Making buffs (or any skill) last longer or shorter, one by one or many at once |
| [Client settings](Client-Settings) | Graphics, sound, interface and connection settings in `Option.ini` and the encrypted `l2.ini` |
| [How settings affect each other](How-Settings-Affect-Each-Other) | The *Depends on*, *Has no effect right now*, *Controls* and *Works with* lines |
| [Custom Config](Custom-Config) | How L2Everdream's shipped settings differ from stock L2J Mobius |
| [Characters and inventories](Characters-and-Inventories) | Adena, changing and removing items, adding items through server delivery, inventory limits |
| [Backups and restore](Backups-and-Restore) | The automatic backup taken before every change, and putting it back |
| [Full backups and launcher updates](Full-Backups-and-Launcher-Updates) | Backing up everything before an update, comparing afterwards, restoring chosen settings |
| [Where your files are](Where-Your-Files-Are) | Which files each setting is saved to, the launcher's protected copies, values the launcher owns |
| [Troubleshooting and FAQ](Troubleshooting-and-FAQ) | Common messages and what to do about them |
| [Privacy and safety](Privacy-and-Safety) | What the program will and won't do, and what it connects to |
| [For developers](For-Developers) | Building, testing, the settings catalog, snapshot mode, releases |

## The safety rules in one place

- **Nothing is written until you click Save changes** (or confirm a character change or restore).
- **Everything is backed up first.** Every save, character change and restore copies what it replaces into a backup you can put back.
- **Only valid values.** Every setting has a type and range, and Save refuses anything the server would reject, with a reason.
- **Never against a running game.** Client settings aren't saved while Lineage 2 is open. Backups of files aren't restored while your
  world runs. Character items are only changed while that character is logged out.
- **The launcher stays in charge.** The few values the launcher rewrites on every start are shown locked, with the reason.
