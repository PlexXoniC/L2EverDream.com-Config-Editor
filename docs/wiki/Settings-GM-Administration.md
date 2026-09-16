# GM & Administration settings

Game Master powers, punishments and logging.

**33 settings** in the **GM & Administration** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Game Masters

### Every new character is an administrator (EverybodyHasAdminRights)

`General.ini › EverybodyHasAdminRights` · on/off · *Advanced*

Do not turn this on unless you really mean it.

- **Default:** Off
- **Allowed:** On or Off

### Only Game Masters can log in (ServerGMOnly)

`General.ini › ServerGMOnly` · on/off

If True, only accounts with GM access can enter the server.

- **Default:** Off
- **Allowed:** On or Off

### GMs glow with the hero aura (GMHeroAura)

`General.ini › GMHeroAura` · on/off

Enable GMs to have the glowing aura of a Hero character on login. GMs can do "///hero" on themselves and get this aura voluntarily. It's advised to keep this off due to graphic lag.

- **Default:** Off
- **Allowed:** On or Off

### GMs log in hidden in builder mode (GMStartupBuilderHide)

`General.ini › GMStartupBuilderHide` · on/off

Whether GM logins in builder hide mode by default.

- **Default:** Off
- **Allowed:** On or Off

### GMs log in invulnerable (GMStartupInvulnerable)

`General.ini › GMStartupInvulnerable` · on/off

Auto set invulnerable status to a GM on login.

- **Default:** Off
- **Allowed:** On or Off

### GMs log in invisible (GMStartupInvisible)

`General.ini › GMStartupInvisible` · on/off

Auto set invisible status to a GM on login.

- **Default:** Off
- **Allowed:** On or Off

### GMs log in blocking private messages (GMStartupSilence)

`General.ini › GMStartupSilence` · on/off

Auto block private messages to a GM on login.

- **Default:** Off
- **Allowed:** On or Off

### GMs appear in /gmlist on login (GMStartupAutoList)

`General.ini › GMStartupAutoList` · on/off

Auto list GMs in GM list (/gmlist) on login.

- **Default:** Off
- **Allowed:** On or Off

### GMs log in without weight penalty (GMStartupDietMode)

`General.ini › GMStartupDietMode` · on/off

Auto set diet mode on to a GM on login (affects your weight penalty).

- **Default:** Off
- **Allowed:** On or Off

### Item restrictions apply to GMs (GMItemRestriction)

`General.ini › GMItemRestriction` · on/off

Item restrictions apply to GMs as well? (True = restricted usage)

- **Default:** On
- **Allowed:** On or Off

### Skill restrictions apply to GMs (GMSkillRestriction)

`General.ini › GMSkillRestriction` · on/off

Skill restrictions apply to GMs as well? (True = restricted usage)

- **Default:** On
- **Allowed:** On or Off

### GMs can trade or drop untradable items (GMTradeRestrictedItems)

`General.ini › GMTradeRestrictedItems` · on/off

Allow GMs to drop/trade non-tradable and quest(drop only) items

- **Default:** Off
- **Allowed:** On or Off

### GMs can restart or exit while fighting (GMRestartFighting)

`General.ini › GMRestartFighting` · on/off

Allow GMs to restart/exit while is fighting stance

- **Default:** On
- **Allowed:** On or Off

### Show the GM's name after announcements (GMShowAnnouncerName)

`General.ini › GMShowAnnouncerName` · on/off

Show the GM's name behind an announcement made by him example: "Announce: hi (HanWik)"

- **Default:** Off
- **Allowed:** On or Off

### Show the GM's name before critical announcements (GMShowCritAnnouncerName)

`General.ini › GMShowCritAnnouncerName` · on/off

Show the GM's name before an announcement made by him example: "Nyaran: hi"

- **Default:** Off
- **Allowed:** On or Off

### Give GMs the special GM skills (GMGiveSpecialSkills)

`General.ini › GMGiveSpecialSkills` · on/off

Give special skills for every GM 7029,7041-7064,7088-7096,23238-23249 (Master's Blessing)

- **Default:** Off
- **Allowed:** On or Off

### Give GMs the special aura skills (GMGiveSpecialAuraSkills)

`General.ini › GMGiveSpecialAuraSkills` · on/off

Give special aura skills for every GM 7029,23238-23249,23253-23296 (Master's Blessing)

- **Default:** Off
- **Allowed:** On or Off

### Show HTML file paths to GMs (GMDebugHtmlPaths)

`General.ini › GMDebugHtmlPaths` · on/off

Debug html paths for GM characters.

- **Default:** On
- **Allowed:** On or Off

### Use the old Super Haste for //gmspeed (UseSuperHasteAsGMSpeed)

`General.ini › UseSuperHasteAsGMSpeed` · on/off

In case you are not satisfied with the retail-like implementation of //gmspeed", with this config you can rollback it to the old custom L2J version of the GM Speed.

- **Default:** Off
- **Allowed:** On or Off

## Punishments & jail

### Punishment for illegal actions (DefaultPunish)

`General.ini › DefaultPunish` · choice

Player punishment for illegal actions: BROADCAST - broadcast warning to GMs only KICK - kick player (default) KICKBAN - kick and ban player JAIL - jail player

- **Default:** `KICK` (kick player (default))
- **Allowed:** one of `BROADCAST` (broadcast warning to GMs only), `KICK` (kick player (default)), `KICKBAN` (kick and ban player), `JAIL` (jail player)
- **Works with:** [Punishment length](Settings-GM-Administration#punishment-length-defaultpunishparam)
- **Works with:** [Block chat in jail](Settings-GM-Administration#block-chat-in-jail-jaildisablechat)

### Punishment length (DefaultPunishParam)

`General.ini › DefaultPunishParam` · number

0 = permanent.

- **Default:** `0`
- **Allowed:** 0 – 9223372036854775807
- **Works with:** [Punishment for illegal actions](Settings-GM-Administration#punishment-for-illegal-actions-defaultpunish) — How long the chosen punishment lasts.

### Punish buying items for zero adena (OnlyGMItemsFree)

`General.ini › OnlyGMItemsFree` · on/off

Apply default punish if player buy items for zero Adena.

- **Default:** On
- **Allowed:** On or Off

### Jail is a PvP zone (JailIsPvp)

`General.ini › JailIsPvp` · on/off

Jail is a PvP zone.

- **Default:** Off
- **Allowed:** On or Off

### Block chat in jail (JailDisableChat)

`General.ini › JailDisableChat` · on/off

Disable all chat in jail (except normal one)

- **Default:** On
- **Allowed:** On or Off
- **Works with:** [Punishment for illegal actions](Settings-GM-Administration#punishment-for-illegal-actions-defaultpunish) — Applies to anyone jailed, including by the punishment for illegal actions.

### Block trading in jail (JailDisableTransaction)

`General.ini › JailDisableTransaction` · on/off

Disable all transaction in jail Trade/Store/Drop

- **Default:** Off
- **Allowed:** On or Off

## Logging & audits

### Log all chat (LogChat)

`General.ini › LogChat` · on/off

Enable logging of player chat messages. Set to True if you need a record of all chat interactions.

- **Default:** Off
- **Allowed:** On or Off

### Log item movement (LogItems)

`General.ini › LogItems` · on/off

Enable logging of item transactions (e.g., pickups, trades, sales). This setting can be useful for tracking item movement but may lead to extensive logging on busy servers.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Item log: only adena and equipment](Settings-GM-Administration#item-log-only-adena-and-equipment-logitemssmalllog) — it only works while this is On
- **Controls:** [Item log: only the listed items](Settings-GM-Administration#item-log-only-the-listed-items-logitemsidsonly) — it only works while this is On

### Item log: only adena and equipment (LogItemsSmallLog)

`General.ini › LogItemsSmallLog` · on/off

If LogItems is enabled, set this to True to only log important items, specifically Adena (in-game currency) and equippable items. This helps to reduce log volume by excluding common, low-value items.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Log item movement](Settings-GM-Administration#log-item-movement-logitems) to be On — Item logging is off.

### Item log: only the listed items (LogItemsIdsOnly)

`General.ini › LogItemsIdsOnly` · on/off

If LogItems is enabled, set this to True to log only specific item IDs rather than all items. This is helpful if you only need logs for certain items, such as rare or high-value items.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Log item movement](Settings-GM-Administration#log-item-movement-logitems) to be On — Item logging is off.
- **Controls:** [Item log: item IDs to log](Settings-GM-Administration#item-log-item-ids-to-log-logitemsidslist) — it only works while this is On

### Item log: item IDs to log (LogItemsIdsList)

`General.ini › LogItemsIdsList` · list

Specifies the item IDs to log when LogItemsIdsOnly is enabled. Enter item IDs separated by commas to track specific items, for example, rare items or in-game currency.

- **Default:** `0`
- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Item log: only the listed items](Settings-GM-Administration#item-log-only-the-listed-items-logitemsidsonly) to be On — The ID list is only used when logging only listed items.

### Log item enchanting (LogItemEnchants)

`General.ini › LogItemEnchants` · on/off

Enable logging for all actions involving item enchantments, such as success or failure of upgrades. This can help with tracking suspicious behavior but may create large log files if enchantment is frequently used.

- **Default:** Off
- **Allowed:** On or Off

### Log skill enchanting (LogSkillEnchants)

`General.ini › LogSkillEnchants` · on/off

Enable logging for all actions related to skill enchantments, including upgrades and modifications. Useful for monitoring skill progression but can lead to extensive logging on active servers.

- **Default:** Off
- **Allowed:** On or Off

### Log GM actions (GMAudit)

`General.ini › GMAudit` · on/off

Enable audit logging for actions performed by Game Masters (GMs). This helps in tracking GM activities to ensure administrative actions are recorded for accountability.

- **Default:** Off
- **Allowed:** On or Off
