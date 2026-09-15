# Client settings

The **Client** tab edits your Lineage 2 client's settings: **81 settings** from two files in the client's `system` folder.

![The Client tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/client.png)

| Category | Groups |
|---|---|
| Graphics | Display · Detail & effects · Draw distance · Window |
| Sound | Volume |
| Gameplay & Interface | Names shown · Chat & messages · Camera & controls · Interface · Language & font |
| Connection & Login | Server · Automatic login |

The section list, search, filters and setting cards work exactly as on the [Server tab](Server-Settings).

## The two files

| File | Settings | Format |
|---|---|---|
| `Option.ini` | 58: resolution, full screen, texture and model detail, effects, gamma, sound volumes, names and chat shown, camera | Plain text |
| `l2.ini` | 23: server address and port, automatic login, windowed and full-screen sizes, aspect ratio, name display, draw distances, engine cache, language and fonts | **Encrypted** by the client |

`l2.ini` is encrypted. The program decodes it, changes only the values you edited and encodes it again. Before writing, it decodes its
own result and compares it with what it meant to write. If they don't match, nothing is saved, so the game always gets a file it can
read.

## Close the game before saving

Lineage 2 writes its settings back to these files when it closes, which would undo your changes. While Lineage 2 is running:

- the folder bar says the game is open
- **Save changes** refuses client changes and asks you to close the game first (server changes can still be saved)

## Good to know

- **Resolution** only applies in full screen; the cards show this with *Depends on* lines.
- Some detail options (texture detail, model detail, draw distance steps) are shown as the raw numbers the file uses, because their exact
  in-game labels haven't been confirmed. If unsure, change the option in game and compare.
- **Server address** (`ServerAddr`) is locked: the launcher sets it on every start, depending on whether you play locally, online or with
  a friend.
- The client ships with `IsL2AutoLogOn=Ture` (a typo in the file). Anything other than `true` counts as off.
- Your automatic-login password, if the client has one saved, is shown in a password box.
