# Troubleshooting and FAQ

## Messages you might see

**"That isn't a server folder"**
The folder you picked has no `game\config\Server.ini`. Choose the folder that contains the `game` and `login` folders. With the official
launcher that's `%LOCALAPPDATA%\L2Everdream`; paste it into the folder picker's address bar.

**"That isn't a client folder"**
The folder has no `system\l2.ini`. Choose your Lineage 2 folder or its `system` folder.

**"This folder no longer contains game\config\Server.ini."**
The folder was moved or the install was removed. Click **Change folder…**.

**"Check this value"** when saving
One of your edits isn't allowed. You're taken to the setting, and the card explains the allowed values. Nothing was saved.

**"Close the game first"**
Lineage 2 is running. It rewrites its settings when it closes, which would undo your client changes. Close the game and save again.
Server changes aren't affected.

**"Nothing was saved"**
Something stopped the save, such as a file that couldn't be written (another program holding it open, or a folder that needs
administrator rights). The message says which file and why. Nothing was half-written. The settings are reloaded from disk, which clears the unsaved edits, so
fix the cause and make the changes again.

**"Could not reach the world's database…"** on the Characters tab
The database only runs while your world runs. Start the world from the launcher, then click **↻ Refresh**.

**"Log this character out to change its adena or items."**
The character is online. The server keeps a logged-in character's items in memory and would overwrite the change.

**"Not enough room: … slots would be needed and the limit is …"**
The item doesn't fit in the inventory limit. Remove items, or raise the inventory slots setting in *Server › Characters › Inventory &
storage*.

**"The item's count changed since the inventory was loaded. Refresh and try again."**
Something changed the item in the meantime (for example the character logged in and out). Click **↻ Refresh**.

**"Stop the world from the launcher to restore these files."**
File restores never run while the world is running. Stop it, then restore.

**"That folder is inside the L2Everdream install folder…"** when choosing a full backup folder
Updates replace the install folder, so backups there would be deleted by the very update they protect against. Choose another folder.

## Questions

**Will this mess up my world?**
It's built to make that very hard. It refuses invalid values, never writes new item rows while the server runs, only edits logged-out
characters, keeps inventories under their limit, and backs up everything before any change. If you don't like a change, restore the backup.

**I saved, but nothing changed in game.**
Server settings apply the next time you **start your world from the launcher**, and client settings the next time you **start Lineage 2**.

**I added an item, but the character didn't get it.**
Look at the top of the inventory screen: while **Deliver items queued from the database** is off, a warning there says so and counts the
deliveries that will never arrive. Turn it **On**, **Save changes**, **restart the world from the launcher**, then log the character **in**.
The server checks the queue every 30 seconds as shipped and puts the items straight into the inventory, with a line in chat — Interlude has
no mail window, so there is nothing to open. The item stays under *Waiting for the server to deliver* until it arrives.

That setting is off as L2Everdream ships, and the launcher does not keep a protected copy of its file, so check it again after an update.

**Can I make buffs last longer?**
Yes. See [Custom skill durations](Custom-Skill-Durations).

**Why can't I change the login port, the database address or the server address?**
The launcher rewrites them every time it starts, so any change would be overwritten. They're shown locked with the reason.

**Some of my values were different from the defaults before I ever used this.**
L2Everdream ships its own tuned values. The [Custom Config](Custom-Config) tab shows which ones differ from stock L2J Mobius, and
**Changed only** shows everything that differs from its default.

**Why does a setting say "Has no effect right now"?**
It depends on another setting that is currently off. Click **Show →** to jump to it. See [How settings affect each other](How-Settings-Affect-Each-Other).

**The launcher says an update is available. What should I do?**
Take a full backup first: **Backups › Full backups › Back up everything**, then update and compare. See
[Full backups and launcher updates](Full-Backups-and-Launcher-Updates).

**Do my changes survive a launcher update?**
The program writes both the install copy and the launcher's protected copy, which is what the launcher keeps through updates. In the
0.5.20 update no customised value was lost, and the 0.5.23 update (world 1.0.146) kept them too. A full backup lets you check after every
update.

**I edited a config file by hand and a setting doesn't show its value.**
The program reads files the way the server does, one `Key = Value` per line. A file whose lines end inconsistently (a mix of Windows and
Unix line endings, which some editors produce) can hide settings. Re-save the file with consistent line endings.

**Can I edit the XML configs, `user.ini` key bindings, warehouses or skills learned?**
Not yet. XML configs are backed up and compared in full backups but not edited, and the Characters tab covers adena and inventory items.

**Does it work with other Lineage 2 servers?**
It's made for L2Everdream's local world: L2J Mobius CT 0 Interlude (protocol 746) with L2Everdream's additions. Other L2J Mobius
Interlude setups may partly work, but they aren't supported.

**Mac or Linux?**
No, it's a Windows program, like L2Everdream.

**Is it free?**
Yes. Free, open source (MIT), no adverts, no accounts.

**Where do I report a problem?**
[Open an issue](https://github.com/PlexXoniC/L2EverDream.com-Config-Editor/issues). Please don't send L2Everdream or L2J Mobius support
requests about this program.
