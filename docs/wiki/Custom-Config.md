# Custom Config

L2Everdream is built on [L2J Mobius](https://gitlab.com/MobiusDevelopment/L2J_Mobius) (CT 0 Interlude). The read-only **Custom Config**
tab shows how the configuration L2Everdream **ships** differs from stock L2J Mobius. It's handy for seeing which of your world's
defaults are L2Everdream's own choices.

![The Custom Config tab](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/custom-config.png)

## What's listed

| Filter | Differences |
|---|---|
| **All differences** | Everything below |
| **New settings** | Settings stock Mobius doesn't have, added with code changes (for example the gatekeeper teleport price and the network security buffer pools) |
| **Changed values** | Stock settings L2Everdream ships with a different value (rates, free teleports, party leader hand-off, dropped item lifetime, cursed weapons, monster leash, Shift+click NPC info, pathfinding and network buffers, login window, newbie status) |
| **Documented** | Settings whose value is unchanged but which the release specifically documents (for example automatic account creation, which suits a world on your own PC) |

Whole files that differ, such as `ClassMaster.xml`, are listed at the end.

## Each difference shows

- the friendly name, the real file and key, and a short neutral summary of the change
- **STOCK L2J MOBIUS → L2EVERDREAM SHIPS → YOUR FILE NOW**, with *(you changed this)* when your value differs from what ships
- **Show in editor**, which opens that setting in the Server tab so you can change it

The tab never writes anything. The comparison is against stock Mobius at the last upstream change to those files before the shipped
server build.
