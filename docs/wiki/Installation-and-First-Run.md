# Installation and first run

L2Everdream Config has no installer. It is one program file that runs from any folder.

## 1. Download

Get the latest version from the [Releases page](https://github.com/PlexXoniC/L2EverDream.com-Config-Editor/releases/latest). There are
two downloads with the same program inside:

| Download | Size | Choose it when |
|---|---|---|
| **standalone** | about 54 MB | You just want it to work. Nothing else to install. **Most people want this one.** |
| **needs-dotnet** | about 1 MB | You already have (or don't mind installing) the [.NET 10 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0). |

Each zip contains `L2EverdreamConfig.exe` and `LICENSE.txt`. The release notes list a SHA-256 fingerprint for each zip. To check yours,
run this in PowerShell or Command Prompt and compare the result:

```
certutil -hashfile L2EverdreamConfig-<version>-win-x64-standalone.zip SHA256
```

## 2. Unzip and run

Unzip anywhere you like, such as your Desktop or a Tools folder, and run **`L2EverdreamConfig.exe`**.

**"Windows protected your PC"**: Windows shows this blue box for any program without a paid code-signing certificate. Click
**More info**, then **Run anyway**. You only need to do this once per downloaded copy.

The program needs Windows 10 or 11, 64-bit. It does not need administrator rights.

## 3. Choose your folders

The program never guesses where your game is. The first time, each tab asks for its folder, with one gold button:

| Tab | Choose | The right folder contains |
|---|---|---|
| **Server** | Your L2Everdream install. With the official launcher this is `%LOCALAPPDATA%\L2Everdream`. Paste that into the folder picker's address bar and press Enter. | `game\config\Server.ini` |
| **Client** | Your Lineage 2 Interlude folder, or its `system` folder | `system\l2.ini` |

If you pick the wrong folder, the program tells you what it expected to find. Your choices are remembered, and **Change folder…** in
the folder bar at the top of each tab picks another one.

The launcher keeps your personal copies of the settings in `%LOCALAPPDATA%\L2Everdream-data`, beside the install folder. The program
finds that folder by itself, so you don't choose it.

## 4. Your first change

1. Pick a category in the list on the left, or type in the search box, for example `party xp`.
2. Change the value on the card. The card turns gold and says **Unsaved**, and the save bar at the bottom counts your changes.
3. Click **Save changes**.
4. Server changes take effect the next time you start your world from the launcher. Client changes take effect the next time you
   start Lineage 2.

See [Server settings](Server-Settings) for everything the cards and filters do.

## Updating the program

Download the new version and replace the old `L2EverdreamConfig.exe`. Your folder choices, preferences and backups are kept. They live
in `%LOCALAPPDATA%\L2EverdreamConfig`, not next to the exe.

## Removing the program

Delete `L2EverdreamConfig.exe`. If you also want to remove its preferences and change backups, delete
`%LOCALAPPDATA%\L2EverdreamConfig`. Full backups are wherever you chose to keep them. Your world, the launcher and the game are not
affected either way.
