# How settings affect each other

Many settings do nothing unless another setting is on, and some switch a whole feature on or off. Setting cards show this directly, and
the lines update as you edit (before you save).

![Relation lines on the Your World settings](https://raw.githubusercontent.com/PlexXoniC/L2EverDream.com-Config-Editor/main/docs/images/server.png)

| Line | Colour | Meaning |
|---|---|---|
| **DEPENDS ON** | violet | This setting needs another one, and that one is set so it works, e.g. *Works because "Start in full screen" is On.* |
| **HAS NO EFFECT RIGHT NOW** | amber | This setting needs another one that is currently set so it doesn't work, e.g. vitality rates while the vitality system is off. |
| **CONTROLS** | violet | This is a switch for other settings: *"Offline shops" only has an effect while this is On.* |
| **WORKS WITH** | violet | A related setting you'll probably want to look at together, e.g. the experience rate and the party experience rate. |

Every line has **Show →**, which jumps to the other setting (clearing filters and switching tabs if needed).

## Where the links come from

- A hand-written list of **359** relationships between settings, each with a plain note where it helps, e.g. *Above 100% only works while
  "Allow more shops than shipped" is on; the launcher caps it at 100% otherwise.*
- Rules the program works out from the config files themselves, such as feature switches in the `Custom` config files that the
  feature's other settings depend on.

A relationship never stops you saving. *Has no effect right now* is a warning, not an error: you might be setting things up before
switching the feature on.
