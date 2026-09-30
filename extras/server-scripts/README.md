# Server scripts

Datapack scripts for a **local** L2Everdream world. They are **not part of the config manager** — the app never installs,
edits or runs them, and it keeps its rule of editing settings only. They live here so they survive an L2Everdream update,
which replaces the install folder and takes anything added to it with it.

Each folder goes under `%LOCALAPPDATA%\L2Everdream\game\data\scripts\custom\`. The world compiles the scripts when it
starts, so the world has to be restarted from the launcher before a change is picked up, and removing a script means
deleting its folder and restarting again.

## Checking a script before restarting a world

A script that does not compile is skipped with an error in the log. It is quicker to find out first, with the JDK and the
jars the server itself uses:

```bash
"%LOCALAPPDATA%\L2Everdream\runtime\bin\javac.exe" -nowarn -cp "%LOCALAPPDATA%\L2Everdream\libs\*" -d "%TEMP%\script-check" "extras\server-scripts\Vanity\VanityCommands.java"
```

## Vanity — `.onfire` and `.bighead`

Two novelty chat commands. Each is a toggle: while it is on, every player, NPC and monster within about 3,000 units of you
shows that effect, and a task every three seconds catches whatever walks into range and restores whatever leaves. Turning
it off, logging out or dying puts everything back.

- `.onfire` — everyone nearby appears to be burning (`DOT_FIRE`).
- `.bighead` — everyone nearby gets a comically large head (`BIG_HEAD`).

Worth knowing:

- These are the game's own visual effects and change nothing about combat, drops or stats.
- They travel in the packets that describe each creature, so **everyone nearby sees them**, not only the person who typed
  the command. There is no way to show them to one player alone from a script; that would need the server to build those
  packets per viewer.
- The script only ever removes an effect it applied itself, so a monster that is genuinely burning from a skill keeps
  its own effect.
- No Game Master access is needed. Chat commands starting with `.` reach registered handlers through the datapack's
  `handlers/chat/channels/ChatGeneral.java`, and the script registers itself the way `custom/SellBuff` does, so no
  existing file is modified.
