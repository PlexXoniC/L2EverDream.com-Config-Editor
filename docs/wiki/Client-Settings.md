# Client settings

The **Client** tab edits your Lineage 2 client's settings: **81 settings** from two files in the client's `system` folder.

![The Client tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/client.png)

| Category | Groups |
|---|---|
| [Graphics](Settings-Graphics) | Display · Detail & effects · Draw distance · Window |
| [Sound](Settings-Sound) | Volume |
| [Gameplay & Interface](Settings-Gameplay-Interface) | Names shown · Chat & messages · Camera & controls · Interface · Language & font |
| [Connection & Login](Settings-Connection-Login) | Server · Automatic login |

The section list, search, filters and setting cards work exactly as on the [Server tab](Server-Settings).

## The three files

| File | Settings | Format |
|---|---|---|
| `Option.ini` | 58: resolution, full screen, texture and model detail, effects, gamma, sound volumes, names and chat shown, camera | Plain text |
| `l2.ini` | 23: server address and port, automatic login, windowed and full-screen sizes, aspect ratio, name display, draw distances, engine cache, language and fonts | **Encrypted** by the client |
| `user.ini` | 1: how far the camera zooms out (see below). The rest of the file is your key bindings, which are not edited here | **Encrypted** by the client |

`l2.ini` and `user.ini` are encrypted. The program decodes a file, changes only the values you edited and encodes it again. Before
writing, it decodes its own result and compares it with what it meant to write. If they don't match, nothing is saved, so the game
always gets a file it can read.

## How far the camera zooms out

This one is not where you would expect, and it is worth knowing why.

`user.ini` has a setting called `MaxZoomingDist`, and it looks like the answer. It is not: the client writes `user.ini` back out
when it closes and puts that number to 250 again, so changing it never survives. What does survive is a command on the **right
mouse button**, because the client runs that binding on every right-click and it re-applies the limit all session. That is what the
community "zoom fix" does, and it is why the setting here is a choice on the binding rather than a number:

| Choice | What right-click does |
|---|---|
| As the game ships it | Turns the camera, and a tap snaps it back behind you. The camera stops at 250 |
| Let the camera zoom right out | The same, and the zoom limit is lifted |
| Zoom right out, and no snap-back on a tap | The community fix exactly: the limit is lifted and the tap-to-snap-back is gone |

The usual fix drops that snap-back without saying so, which catches people out, so here it is a choice. If you have bound something
of your own to right-click it is read, kept and written back, and the setting shows "Your own binding" rather than calling it wrong.

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
