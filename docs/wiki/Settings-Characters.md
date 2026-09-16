# Characters settings

Creating characters, levels, stats, limits and everyday rules.

**118 settings** in the **Characters** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## New characters

### Every new character on an account counts as a newbie (AltNewCharAlwaysIsNewbie)

`Player.ini › AltNewCharAlwaysIsNewbie` · on/off · *added by L2Everdream*

All new characters of the same account are newbies, not only first one.

- **Default:** Off
- **Allowed:** On or Off

### Starting adena (StartingAdena)

`Player.ini › StartingAdena` · number · adena

This is the amount of Adena that a new character starts their character with.

- **Default:** `0`
- **Allowed:** 0 – 2147483647 adena

### Starting level (StartingLevel)

`Player.ini › StartingLevel` · number

This is the starting level of the new character.

- **Default:** `1`
- **Allowed:** 1 – 80
- **Works with:** [Maximum character level](Settings-Characters#maximum-character-level-maximumplayerlevel) — Must not be above the maximum character level.

### Starting skill points (StartingSP)

`Player.ini › StartingSP` · number

This is the amount of SP that a new character starts their character with.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Humans can be created (AllowHuman)

`Custom/AllowedPlayerRaces.ini › AllowHuman` · on/off

Allowing specific races to be created.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Elves can be created](Settings-Characters#elves-can-be-created-allowelf) — it only works while this is On
- **Controls:** [Dark Elves can be created](Settings-Characters#dark-elves-can-be-created-allowdarkelf) — it only works while this is On
- **Controls:** [Orcs can be created](Settings-Characters#orcs-can-be-created-alloworc) — it only works while this is On
- **Controls:** [Dwarves can be created](Settings-Characters#dwarves-can-be-created-allowdwarf) — it only works while this is On

### Elves can be created (AllowElf)

`Custom/AllowedPlayerRaces.ini › AllowElf` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Humans can be created](Settings-Characters#humans-can-be-created-allowhuman) to be On

### Dark Elves can be created (AllowDarkElf)

`Custom/AllowedPlayerRaces.ini › AllowDarkElf` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Humans can be created](Settings-Characters#humans-can-be-created-allowhuman) to be On

### Orcs can be created (AllowOrc)

`Custom/AllowedPlayerRaces.ini › AllowOrc` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Humans can be created](Settings-Characters#humans-can-be-created-allowhuman) to be On

### Dwarves can be created (AllowDwarf)

`Custom/AllowedPlayerRaces.ini › AllowDwarf` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Humans can be created](Settings-Characters#humans-can-be-created-allowhuman) to be On

### Start new characters at a custom location (CustomStartingLocation)

`Custom/StartingLocation.ini › CustomStartingLocation` · on/off

Enable custom starting location.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Custom start: X](Settings-Characters#custom-start-x-customstartinglocx) — it only works while this is On
- **Controls:** [Custom start: Y](Settings-Characters#custom-start-y-customstartinglocy) — it only works while this is On
- **Controls:** [Custom start: Z](Settings-Characters#custom-start-z-customstartinglocz) — it only works while this is On

### Custom start: X (CustomStartingLocX)

`Custom/StartingLocation.ini › CustomStartingLocX` · number

Coords for custom starting location

- **Default:** `50821`
- **Allowed:** 0 – 2147483647
- **Needs:** [Start new characters at a custom location](Settings-Characters#start-new-characters-at-a-custom-location-customstartinglocation) to be On — The custom starting location is off.

### Custom start: Y (CustomStartingLocY)

`Custom/StartingLocation.ini › CustomStartingLocY` · number

- **Default:** `186527`
- **Allowed:** 0 – 2147483647
- **Needs:** [Start new characters at a custom location](Settings-Characters#start-new-characters-at-a-custom-location-customstartinglocation) to be On — The custom starting location is off.

### Custom start: Z (CustomStartingLocZ)

`Custom/StartingLocation.ini › CustomStartingLocZ` · number

- **Default:** `-3625`
- **Allowed:** -2147483648 – 2147483647
- **Needs:** [Start new characters at a custom location](Settings-Characters#start-new-characters-at-a-custom-location-customstartinglocation) to be On — The custom starting location is off.

### Give new characters a title (EnableStartingTitle)

`Custom/StartingTitle.ini › EnableStartingTitle` · on/off

Enable starting title.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Starting title](Settings-Characters#starting-title-startingtitle) — it only works while this is On

### Starting title (StartingTitle)

`Custom/StartingTitle.ini › StartingTitle` · text

Starting title for new players.

- **Default:** `Newbie`
- **Needs:** [Give new characters a title](Settings-Characters#give-new-characters-a-title-enablestartingtitle) to be On

## Names & character slots

### Allowed character names (pattern) (CnameTemplate)

`Server.ini › CnameTemplate` · text

A regular expression. .* allows anything.

- **Default:** `.*`
- **Allowed:** a regular expression
- **Works with:** [Allowed pet names (pattern)](Settings-Characters#allowed-pet-names-pattern-petnametemplate)
- **Works with:** [Allowed clan names (pattern)](Settings-Characters#allowed-clan-names-pattern-clannametemplate)

### Allowed pet names (pattern) (PetNameTemplate)

`Server.ini › PetNameTemplate` · text

This setting restricts names players can give to their pets. See CnameTemplate for details

- **Default:** `.*`
- **Allowed:** a regular expression
- **Works with:** [Allowed character names (pattern)](Settings-Characters#allowed-character-names-pattern-cnametemplate) — Uses the same kind of pattern as character names.

### Allowed clan names (pattern) (ClanNameTemplate)

`Server.ini › ClanNameTemplate` · text

This setting restricts clan/subpledge names players can set. See CnameTemplate for details

- **Default:** `.*`
- **Allowed:** a regular expression
- **Works with:** [Allowed character names (pattern)](Settings-Characters#allowed-character-names-pattern-cnametemplate) — Uses the same kind of pattern as character names.

### Characters per account (CharMaxNumber)

`Server.ini › CharMaxNumber` · number

The client shows at most 7.

- **Default:** `7`
- **Allowed:** 1 – 7

## Levels & death

### Lose levels when experience drops below the level (Delevel)

`Player.ini › Delevel` · on/off

This option, if enabled, will force a character to de-level if the characters' experience is below their level after losing experience on death. If this is set to False, the character will not de-level even if their Experience is below their level after death.

- **Default:** On
- **Allowed:** On or Off

### Lower skill levels after losing a level (DecreaseSkillOnDelevel)

`Player.ini › DecreaseSkillOnDelevel` · on/off

This option enable check for all player skills for skill level. If player level is lower than skill learn level - 9, skill level is decreased to next possible level. If there is no possible level, skill is removed from player.

- **Default:** On
- **Allowed:** On or Off

### Chance of Death Penalty debuff when killed by a monster (DeathPenaltyChance)

`Player.ini › DeathPenaltyChance` · number · %

Chance of receiving the Death Penalty debuff when killed by a mob.

- **Default:** `20`
- **Allowed:** 0 – 100 %

### CP restored on revival (RespawnRestoreCP)

`Player.ini › RespawnRestoreCP` · number · %

Percent of HP, MP, and CP which is restored on character revival. Use 0 to disable restore

- **Default:** `0`
- **Allowed:** 0 – 100 %

### HP restored on revival (RespawnRestoreHP)

`Player.ini › RespawnRestoreHP` · number · %

- **Default:** `65.0`
- **Allowed:** 0 – 100 %

### MP restored on revival (RespawnRestoreMP)

`Player.ini › RespawnRestoreMP` · number · %

- **Default:** `0`
- **Allowed:** 0 – 100 %

### HP regeneration speed (HpRegenMultiplier)

`Player.ini › HpRegenMultiplier` · number · %

Percent of HP, MP, and CP regeneration for players. Example: Setting HP to 10 will cause player HP to regenerate 90% slower than normal.

- **Default:** `100.0`
- **Allowed:** 0 or more %

### MP regeneration speed (MpRegenMultiplier)

`Player.ini › MpRegenMultiplier` · number · %

- **Default:** `100.0`
- **Allowed:** 0 or more %

### CP regeneration speed (CpRegenMultiplier)

`Player.ini › CpRegenMultiplier` · number · %

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Most skill points a character can hold (MaxSp)

`Player.ini › MaxSp` · number

Maximum amount of SP a character can posses. Current retail limit is max integer number, use -1 to set it to unlimited.

- **Default:** `50000000000`
- **Allowed:** -1 – 2147483647

### Maximum character level (MaximumPlayerLevel)

`Player.ini › MaximumPlayerLevel` · number

Cannot be higher than the experience table allows (80 in Interlude).

- **Default:** `80`
- **Allowed:** 1 – 80
- **Works with:** [Maximum subclass level](Settings-Characters#maximum-subclass-level-maxsubclasslevel) — Subclasses cannot go above this level either.
- **Works with:** [Starting level](Settings-Characters#starting-level-startinglevel)

### Can't move for this long after talking to an NPC (NpcTalkBlockingTime)

`Player.ini › NpcTalkBlockingTime` · number · seconds

Npc talk blockage. When a player talks to a NPC, he must wait some secs before being able to walk again. In seconds Set to 0 to disable it

- **Default:** `0`
- **Allowed:** 0 – 2147483647 seconds

### Protection after entering the world (PlayerSpawnProtection)

`Player.ini › PlayerSpawnProtection` · number · seconds

After a player spawns, this is the time the player is protected. This time is in seconds, leave it at 0 if you want this feature disabled. Retail (Since GE): 600 (10 minutes)

- **Default:** `0`
- **Allowed:** 0 – 2147483647 seconds

### Disconnect players who stay dead for an hour (DisconnectAfterDeath)

`Player.ini › DisconnectAfterDeath` · on/off

Disconnect player after being dead for 1 hour.

- **Default:** On
- **Allowed:** On or Off

## Classes & subclasses

### Subclasses without doing the quests (AltSubClassWithoutQuests)

`Player.ini › AltSubClassWithoutQuests` · on/off

Allow player to sub-class without checking for unique quest items.

- **Default:** Off
- **Allowed:** On or Off

### Add or change subclass at any village master (AltSubclassEverywhere)

`Player.ini › AltSubclassEverywhere` · on/off

Allow player to add/change subclass at all village master

- **Default:** Off
- **Allowed:** On or Off

### Price to remove transfer skills (FeeDeleteTransferSkills)

`Player.ini › FeeDeleteTransferSkills` · number · adena

Fee to remove Transfer skills.

- **Default:** `10000000`
- **Allowed:** 0 – 2147483647 adena

### Number of subclasses allowed (MaxSubclass)

`Player.ini › MaxSubclass` · number

Maximum number of allowed subclasses for every player.

- **Default:** `3`
- **Allowed:** 0 – 3

### Level a new subclass starts at (BaseSubclassLevel)

`Player.ini › BaseSubclassLevel` · number

Starting level for subclasses.

- **Default:** `40`
- **Allowed:** 1 – 80
- **Works with:** [Maximum subclass level](Settings-Characters#maximum-subclass-level-maxsubclasslevel) — New subclasses start at this level; it must be below the maximum subclass level.

### Maximum subclass level (MaxSubclassLevel)

`Player.ini › MaxSubclassLevel` · number

Maximum subclass level.

- **Default:** `80`
- **Allowed:** 1 – 80
- **Works with:** [Maximum character level](Settings-Characters#maximum-character-level-maximumplayerlevel)
- **Works with:** [Level a new subclass starts at](Settings-Characters#level-a-new-subclass-starts-at-basesubclasslevel)

## Stat limits

### Carrying capacity (AltWeightLimit)

`Player.ini › AltWeightLimit` · number · times

Weight limit multiplier. Example: Setting this to 5 will give players 5x the normal weight limit.

- **Default:** `1`
- **Allowed:** 0 or more times

### Extra running speed for everyone (RunSpeedBoost)

`Player.ini › RunSpeedBoost` · number

Run speed modifier. Example: Setting this to 5 will give players +5 to their running speed.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Highest total experience bonus (MaxExpBonus)

`Player.ini › MaxExpBonus` · number · times

Maximum Exp Bonus. from vitality + nevit's hunting bonus, and etc..

- **Default:** `3.5`
- **Allowed:** 0 or more times

### Highest total skill point bonus (MaxSpBonus)

`Player.ini › MaxSpBonus` · number · times

Maximum Sp Bonus. from vitality + nevit's hunting bonus, and etc..

- **Default:** `3.5`
- **Allowed:** 0 or more times

### Highest running speed (MaxRunSpeed)

`Player.ini › MaxRunSpeed` · number

Maximum character running speed.

- **Default:** `250`
- **Allowed:** 0 – 2147483647

### Highest P. Atk. (MaxPAtk)

`Player.ini › MaxPAtk` · number

Maximum character Physical Attack.

- **Default:** `999999`
- **Allowed:** 0 – 2147483647

### Highest M. Atk. (MaxMAtk)

`Player.ini › MaxMAtk` · number

Maximum character Magic Attack.

- **Default:** `999999`
- **Allowed:** 0 – 2147483647

### Highest physical critical rate (MaxPCritRate)

`Player.ini › MaxPCritRate` · number

10 = 1%.

- **Default:** `500`
- **Allowed:** 0 – 10000

### Highest magic critical rate (MaxMCritRate)

`Player.ini › MaxMCritRate` · number

10 = 1%.

- **Default:** `200`
- **Allowed:** 0 – 10000

### Highest attack speed (MaxPAtkSpeed)

`Player.ini › MaxPAtkSpeed` · number

Maximum character Attack Speed.

- **Default:** `1500`
- **Allowed:** 0 – 2147483647

### Highest casting speed (MaxMAtkSpeed)

`Player.ini › MaxMAtkSpeed` · number

Maximum character Cast Speed.

- **Default:** `1999`
- **Allowed:** 0 – 2147483647

### Highest evasion (MaxEvasion)

`Player.ini › MaxEvasion` · number

Maximum character Evasion.

- **Default:** `250`
- **Allowed:** 0 – 2147483647

### Lowest debuff success chance (MinAbnormalStateSuccessRate)

`Player.ini › MinAbnormalStateSuccessRate` · number · %

Minimum and Maximum Abnormal State Success Rate. This affect all skills/effects chances, except in skills where minChance or maxChance parameters are defined.

- **Default:** `10`
- **Allowed:** 0 – 100 %

### Highest debuff success chance (MaxAbnormalStateSuccessRate)

`Player.ini › MaxAbnormalStateSuccessRate` · number · %

- **Default:** `90`
- **Allowed:** 0 – 100 %

## Inventory & storage

### Allow warehouses (AllowWarehouse)

`General.ini › AllowWarehouse` · on/off

If you are experiencing problems with Warehouse transactions, feel free to disable them here.

- **Default:** On
- **Allowed:** On or Off

### Private sell store slots (dwarves) (MaxPvtStoreSellSlotsDwarf)

`Player.ini › MaxPvtStoreSellSlotsDwarf` · number

Maximum number of allowed slots for Private Stores Sell. Other means all the other races aside from Dwarf.

- **Default:** `4`
- **Allowed:** 0 – 2147483647

### Private sell store slots (other races) (MaxPvtStoreSellSlotsOther)

`Player.ini › MaxPvtStoreSellSlotsOther` · number

- **Default:** `3`
- **Allowed:** 0 – 2147483647

### Private buy store slots (dwarves) (MaxPvtStoreBuySlotsDwarf)

`Player.ini › MaxPvtStoreBuySlotsDwarf` · number

Maximum number of allowed slots for Private Stores Buy. Other means all the other races aside from Dwarf.

- **Default:** `5`
- **Allowed:** 0 – 2147483647

### Private buy store slots (other races) (MaxPvtStoreBuySlotsOther)

`Player.ini › MaxPvtStoreBuySlotsOther` · number

- **Default:** `4`
- **Allowed:** 0 – 2147483647

### Inventory slots (other races) (MaximumSlotsForNoDwarf)

`Player.ini › MaximumSlotsForNoDwarf` · number

This will control the inventory space limit (NOT WEIGHT LIMIT).

- **Default:** `80`
- **Allowed:** 0 – 2147483647
- **Works with:** [Inventory slots (dwarves)](Settings-Characters#inventory-slots-dwarves-maximumslotsfordwarf)
- **Works with:** [Inventory slots (Game Masters)](Settings-Characters#inventory-slots-game-masters-maximumslotsforgmplayer)

### Inventory slots (dwarves) (MaximumSlotsForDwarf)

`Player.ini › MaximumSlotsForDwarf` · number

- **Default:** `100`
- **Allowed:** 0 – 2147483647
- **Works with:** [Inventory slots (other races)](Settings-Characters#inventory-slots-other-races-maximumslotsfornodwarf) — Dwarves use this limit instead; the Characters tab uses it when adding items to dwarves.

### Inventory slots (Game Masters) (MaximumSlotsForGMPlayer)

`Player.ini › MaximumSlotsForGMPlayer` · number

- **Default:** `250`
- **Allowed:** 0 – 2147483647
- **Works with:** [Inventory slots (other races)](Settings-Characters#inventory-slots-other-races-maximumslotsfornodwarf) — Game Masters use this limit instead; the Characters tab uses it when adding items to them.

### Warehouse slots (dwarves) (MaximumWarehouseSlotsForDwarf)

`Player.ini › MaximumWarehouseSlotsForDwarf` · number

Must stay below 300 or the client crashes.

- **Default:** `120`
- **Allowed:** 1 – 299

### Warehouse slots (other races) (MaximumWarehouseSlotsForNoDwarf)

`Player.ini › MaximumWarehouseSlotsForNoDwarf` · number

Must stay below 300 or the client crashes.

- **Default:** `100`
- **Allowed:** 1 – 299

### Clan warehouse slots (MaximumWarehouseSlotsForClan)

`Player.ini › MaximumWarehouseSlotsForClan` · number

Must stay below 300 or the client crashes.

- **Default:** `150`
- **Allowed:** 1 – 299

### Freight slots (MaximumFreightSlots)

`Player.ini › MaximumFreightSlots` · number

Freight Maximum items that can be placed in Freight

- **Default:** `200`
- **Allowed:** 0 – 2147483647

### Freight price per item (FreightPrice)

`Player.ini › FreightPrice` · number · adena

The price for each item that's deposited

- **Default:** `1000`
- **Allowed:** 0 – 2147483647 adena

### Sorting buttons in the clan warehouse (EnableWarehouseSortingClan)

`Custom/WarehouseSorting.ini › EnableWarehouseSortingClan` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Sorting buttons in the private warehouse](Settings-Characters#sorting-buttons-in-the-private-warehouse-enablewarehousesortingprivate) — it only works while this is On

### Sorting buttons in the private warehouse (EnableWarehouseSortingPrivate)

`Custom/WarehouseSorting.ini › EnableWarehouseSortingPrivate` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Sorting buttons in the clan warehouse](Settings-Characters#sorting-buttons-in-the-clan-warehouse-enablewarehousesortingclan) to be On

## Looting

### Pick up herbs automatically (AutoLootHerbs)

`Player.ini › AutoLootHerbs` · on/off

Enable herbs auto pickup.

- **Default:** Off
- **Allowed:** On or Off

### Pick up monster drops automatically (AutoLoot)

`Player.ini › AutoLoot` · on/off

This option, when set to True, will enable automatically picking up items. If set False it will force the player to pickup dropped items from mobs. This excludes herbs mentioned above and items from Raid/GrandBosses with minions.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Drop to the ground when the inventory is full](Settings-Characters#drop-to-the-ground-when-the-inventory-is-full-autolootslotlimit) — it only works while this is On
- **Works with:** [Always auto-pick these items](Settings-Characters#always-auto-pick-these-items-autolootitemids)

### Pick up raid boss drops automatically (AutoLootRaids)

`Player.ini › AutoLootRaids` · on/off

This option, when set to True, will enable automatically picking up items from Raid/GrandBosses with minions. If set False it will force the player to pickup dropped items from bosses. This excludes herbs mentioned above and items from mobs.

- **Default:** Off
- **Allowed:** On or Off

### Drop to the ground when the inventory is full (AutoLootSlotLimit)

`Player.ini › AutoLootSlotLimit` · on/off

Prevent auto loot when inventory slot limit is reached. The items will be dropped to the ground instead.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Pick up monster drops automatically](Settings-Characters#pick-up-monster-drops-automatically-autoloot) to be On — Only matters while drops are picked up automatically.

### Raid loot priority time for command channels (RaidLootRightsInterval)

`Player.ini › RaidLootRightsInterval` · number · seconds

Delay for raid drop items loot privilege Require Command Channel , check next option Value is in seconds

- **Default:** `900`
- **Allowed:** 0 – 2147483647 seconds
- **Works with:** [Command channel size needed for raid loot priority](Settings-Characters#command-channel-size-needed-for-raid-loot-priority-raidlootrightsccsize)

### Command channel size needed for raid loot priority (RaidLootRightsCCSize)

`Player.ini › RaidLootRightsCCSize` · number

Minimum size of Command Channel for apply raid loot privilege

- **Default:** `45`
- **Allowed:** 0 – 2147483647
- **Works with:** [Raid loot priority time for command channels](Settings-Characters#raid-loot-priority-time-for-command-channels-raidlootrightsinterval) — A command channel this big gets raid loot priority for the time set there.

### Always auto-pick these items (AutoLootItemIds)

`Player.ini › AutoLootItemIds` · list

Comma-separated item IDs. 0 = none.

- **Default:** `0`
- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Works with:** [Pick up monster drops automatically](Settings-Characters#pick-up-monster-drops-automatically-autoloot) — Items listed here are always picked up automatically, even when auto loot is off.

## Parties

### Party reward range (AltPartyRange)

`Player.ini › AltPartyRange` · number

Party members farther than this from the kill get no share.

- **Default:** `1500`
- **Allowed:** 0 – 2147483647

### When the leader leaves, pass leadership instead of disbanding (AltLeavePartyLeader)

`Player.ini › AltLeavePartyLeader` · on/off · *added by L2Everdream*

If true, when party leader leaves party, next member in party will be the leader. If false the party be will dispersed.

- **Default:** Off
- **Allowed:** On or Off

### Party experience sharing rule (PartyXpCutoffMethod)

`Player.ini › PartyXpCutoffMethod` · choice

PARTY XP DISTRIBUTION With "auto method" member is cut from Exp/SP distribution when his share is lower than party bonus acquired for him (30% for 2 member party). In that case he will not receive any Exp/SP from party and is not counted for party bonus. If you don't want to have a cutoff point for party members' XP distribution, set the first option to "none". Available Options: highfive, auto, level, percentage, none

- **Default:** `LEVEL`
- **Allowed:** one of `highfive`, `auto`, `level`, `percentage`, `none`
- **Controls:** [Sharing rule "percentage": cutoff](Settings-Characters#sharing-rule-percentage-cutoff-partyxpcutoffpercent) — it only works while this is `percentage`
- **Controls:** [Sharing rule "level": level gap cutoff](Settings-Characters#sharing-rule-level-level-gap-cutoff-partyxpcutofflevel) — it only works while this is `level`
- **Controls:** [Sharing rule "highfive": level gap ranges](Settings-Characters#sharing-rule-highfive-level-gap-ranges-partyxpcutoffgaps) — it only works while this is `highfive`
- **Controls:** [Sharing rule "highfive": XP percent per range](Settings-Characters#sharing-rule-highfive-xp-percent-per-range-partyxpcutoffgappercent) — it only works while this is `highfive`

### Sharing rule "percentage": cutoff (PartyXpCutoffPercent)

`Player.ini › PartyXpCutoffPercent` · number · %

This option takes effect when "percentage" method is chosen. Don't use high values for this!

- **Default:** `3.0`
- **Allowed:** 0 – 100 %
- **Needs:** [Party experience sharing rule](Settings-Characters#party-experience-sharing-rule-partyxpcutoffmethod) to be `percentage` — Only used when the sharing rule is "percentage".

### Sharing rule "level": level gap cutoff (PartyXpCutoffLevel)

`Player.ini › PartyXpCutoffLevel` · number · levels

This option takes effect when "level" method is chosen. Don't use low values for this!

- **Default:** `20`
- **Allowed:** 0 – 2147483647 levels
- **Needs:** [Party experience sharing rule](Settings-Characters#party-experience-sharing-rule-partyxpcutoffmethod) to be `level` — Only used when the sharing rule is "level".

### Sharing rule "highfive": level gap ranges (PartyXpCutoffGaps)

`Player.ini › PartyXpCutoffGaps` · list

This option takes effect when "highfive" method is chosen. Each pair of numbers represent a level range. If the gap is between the first pair, there is no penalty. If the gap is between the second pair, the lowest party member will gain only 30% of the XP that others receive. If the gap is between the last pair, the lowest party member will not receive any XP.

- **Default:** `0,9;10,14;15,99`
- **Allowed:** `from,to` level ranges separated by semicolons, e.g. `0,9;10,14`
- **Needs:** [Party experience sharing rule](Settings-Characters#party-experience-sharing-rule-partyxpcutoffmethod) to be `highfive` — Only used when the sharing rule is "highfive".

### Sharing rule "highfive": XP percent per range (PartyXpCutoffGapPercent)

`Player.ini › PartyXpCutoffGapPercent` · list

This option takes effect when "highfive" method is chosen. Each number represent the XP percent gain at that level gap. For the first gap, the lowest party member will gain 100% XP as there is no penalty. For the second gap, the lowest party member will gain only 30% of the XP that others receive. For the last gap, the lowest party member will not receive any XP.

- **Default:** `100;30;0`
- **Allowed:** numbers separated by semicolons, e.g. `100;30;0`
- **Needs:** [Party experience sharing rule](Settings-Characters#party-experience-sharing-rule-partyxpcutoffmethod) to be `highfive` — Only used when the sharing rule is "highfive".

## Travel & teleports

### /unstuck casting time (UnstuckInterval)

`Player.ini › UnstuckInterval` · number · seconds

This is the time in seconds that it will take for the player command "/unstuck" to activate.

- **Default:** `300`
- **Allowed:** 0 – 2147483647 seconds

### Force a stuck teleport to finish after (TeleportWatchdogTimeout)

`Player.ini › TeleportWatchdogTimeout` · number · seconds

Teleport Watchdog Timeout (seconds) Player forced to appear if remain in teleported state longer than timeout Does not set too low, recommended value 60s. This time is in seconds, leave it at 0 if you want this feature disabled.

- **Default:** `0`
- **Allowed:** 0 – 2147483647 seconds

### Protection after teleporting (PlayerTeleportProtection)

`Player.ini › PlayerTeleportProtection` · number · seconds

After a player teleports, this is the time the player is protected. This time is in seconds, leave it at 0 if you want this feature disabled.

- **Default:** `0`
- **Allowed:** 0 – 2147483647 seconds

### Respawn at random spots in town (RandomRespawnInTownEnabled)

`Player.ini › RandomRespawnInTownEnabled` · on/off

If enabled, players respawn in town on different locations defined in zone.xml for given town. If disabled the first spawn location from zone.xml is used.

- **Default:** On
- **Allowed:** On or Off

### Scatter teleport arrival points slightly (OffsetOnTeleportEnabled)

`Player.ini › OffsetOnTeleportEnabled` · on/off

This will allow a random offset from the base teleport location coordinates based on a maximum offset.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Teleport scatter distance](Settings-Characters#teleport-scatter-distance-maxoffsetonteleport) — it only works while this is On

### Teleport scatter distance (MaxOffsetOnTeleport)

`Player.ini › MaxOffsetOnTeleport` · number

Maximum offset for base teleport location when OffsetOnTeleportEnabled is enabled.

- **Default:** `50`
- **Allowed:** 0 – 2147483647
- **Needs:** [Scatter teleport arrival points slightly](Settings-Characters#scatter-teleport-arrival-points-slightly-offsetonteleportenabled) to be On — Teleport scatter is off.

### Allow teleporting into a siege (TeleportWhileSiegeInProgress)

`Player.ini › TeleportWhileSiegeInProgress` · on/off

Enable teleporting while siege in progress.

- **Default:** On
- **Allowed:** On or Off

### Teleports are free up to level (MaxFreeTeleportLevel)

`Player.ini › MaxFreeTeleportLevel` · number · *added by L2Everdream*

0 = teleports always cost adena.

- **Default:** `40`
- **Allowed:** 0 – 2147483647
- **Works with:** [Gatekeeper teleport price](Settings-Characters#gatekeeper-teleport-price-rateteleportfee) — Above this level, teleports cost the gatekeeper price times the teleport price rate.
- **Works with:** [Gatekeeper teleport price](Settings-Characters#gatekeeper-teleport-price-rateteleportfee)

### Gatekeeper teleport price (RateTeleportFee)

`Rates.ini › RateTeleportFee` · number · times · *added by L2Everdream*

Scales the adena price of normal town gatekeeper teleports. 1 = retail prices, 0.5 = half price.

- **Default:** `1`
- **Allowed:** 0.01 – 100 times
- **Works with:** [Teleports are free up to level](Settings-Characters#teleports-are-free-up-to-level-maxfreeteleportlevel) — Characters at or below the free teleport level pay nothing, whatever the teleport price is.
- **Works with:** [Teleports are free up to level](Settings-Characters#teleports-are-free-up-to-level-maxfreeteleportlevel)

## Vitality system

### Enable the vitality system (EnableVitality)

`Player.ini › EnableVitality` · on/off

Enables vitality system

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Recover vitality while offline](Settings-Characters#recover-vitality-while-offline-recovervitalityonreconnect) — it only works while this is On
- **Controls:** [Vitality points for new characters](Settings-Characters#vitality-points-for-new-characters-startingvitalitypoints) — it only works while this is On
- **Controls:** [Raid boss kills use vitality](Settings-Characters#raid-boss-kills-use-vitality-raidbossusevitality) — it only works while this is On
- **Controls:** [Vitality level 1: XP multiplier](Settings-Rates-Rewards#vitality-level-1-xp-multiplier-ratevitalitylevel1) — it only works while this is On
- **Controls:** [Vitality level 2: XP multiplier](Settings-Rates-Rewards#vitality-level-2-xp-multiplier-ratevitalitylevel2) — it only works while this is On
- **Controls:** [Vitality level 3: XP multiplier](Settings-Rates-Rewards#vitality-level-3-xp-multiplier-ratevitalitylevel3) — it only works while this is On
- **Controls:** [Vitality level 4: XP multiplier](Settings-Rates-Rewards#vitality-level-4-xp-multiplier-ratevitalitylevel4) — it only works while this is On
- **Controls:** [Vitality gained](Settings-Rates-Rewards#vitality-gained-ratevitalitygain) — it only works while this is On
- **Controls:** [Vitality used up per kill](Settings-Rates-Rewards#vitality-used-up-per-kill-ratevitalitylost) — it only works while this is On
- **Controls:** [Vitality recovery speed in towns](Settings-Rates-Rewards#vitality-recovery-speed-in-towns-raterecoverypeacezone) — it only works while this is On
- **Controls:** [Champion kills use vitality](Settings-Monsters-Bosses#champion-kills-use-vitality-championenablevitality) — it only works while this is On

### Recover vitality while offline (RecoverVitalityOnReconnect)

`Player.ini › RecoverVitalityOnReconnect` · on/off

Do you want players to recover their vitality when they reconnect? This is calculated with the time they've been offline Actual Time - Last Time Online / 1000 x rate recovery on reconnect Works only if EnableVitality = True

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — The vitality system is off.
- **Controls:** [Vitality recovery speed while offline](Settings-Rates-Rewards#vitality-recovery-speed-while-offline-raterecoveryonreconnect) — it only works while this is On

### Vitality points for new characters (StartingVitalityPoints)

`Player.ini › StartingVitalityPoints` · number

Option to set a lower vitality at character creation. Vitality needs to be enabled, and startingpoints needs to be lower than max-vitality points.

- **Default:** `20000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — The vitality system is off.

### Raid boss kills use vitality (RaidbossUseVitality)

`Player.ini › RaidbossUseVitality` · on/off

Calculate vitality bonus for raidboss kills.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — The vitality system is off.

## Pets & summons

### Allow the wyvern upgrade NPC (AllowWyvernUpgrader)

`NPC.ini › AllowWyvernUpgrader` · on/off

This option enables or disables the Wyvern manager located in every castle to train Wyverns and Striders from Hatchlings.

- **Default:** Off
- **Allowed:** On or Off

### Pet inventory slots (MaximumSlotsForPet)

`NPC.ini › MaximumSlotsForPet` · number

This will control the inventory space limit for pets (NOT WEIGHT LIMIT).

- **Default:** `12`
- **Allowed:** 0 – 2147483647

### Pet HP regeneration (PetHpRegenMultiplier)

`NPC.ini › PetHpRegenMultiplier` · number · %

HP/MP Regen Multiplier for Pets

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Pet MP regeneration (PetMpRegenMultiplier)

`NPC.ini › PetMpRegenMultiplier` · number · %

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Pets and summons keep buffs after logging out (SummonStoreSkillCooltime)

`Player.ini › SummonStoreSkillCooltime` · on/off

This option is to enable or disable the storage of buffs/debuffs among other effects on pets/invocations

- **Default:** On
- **Allowed:** On or Off

### Re-summon servitors on login (RestoreServitorOnReconnect)

`Player.ini › RestoreServitorOnReconnect` · on/off

Servitor summons on login if player had it summoned before logout

- **Default:** On
- **Allowed:** On or Off

### Re-summon pets on login (RestorePetOnReconnect)

`Player.ini › RestorePetOnReconnect` · on/off

Pet summons on login if player had it summoned before logout

- **Default:** On
- **Allowed:** On or Off

## Other player rules

### Falling damage (EnableFallingDamage)

`General.ini › EnableFallingDamage` · on/off

Allow characters to receive damage from falling.

- **Default:** On
- **Allowed:** On or Off

### Allow monster races (AllowRace)

`General.ini › AllowRace` · on/off

- **Default:** On
- **Allowed:** On or Off

### Allow swimming (AllowWater)

`General.ini › AllowWater` · on/off

- **Default:** On
- **Allowed:** On or Off

### Allow fishing (AllowFishing)

`General.ini › AllowFishing` · on/off

- **Default:** On
- **Allowed:** On or Off

### Allow boats (AllowBoat)

`General.ini › AllowBoat` · on/off

- **Default:** On
- **Allowed:** On or Off

### Boat announcement range (BoatBroadcastRadius)

`General.ini › BoatBroadcastRadius` · number

Boat broadcast radius. If players getting annoyed by boat shouts then radius can be decreased.

- **Default:** `20000`
- **Allowed:** 0 – 2147483647

### Allow parties inside the same event (AllowPartyInSameEvent)

`General.ini › AllowPartyInSameEvent` · on/off

If false, always block party on event. If true, allows party if both are in the same event.

- **Default:** On
- **Allowed:** On or Off

### Allow moving with the keyboard (KeyboardMovement)

`Player.ini › KeyboardMovement` · on/off

Enable keyboard movement.

- **Default:** On
- **Allowed:** On or Off

### Allow petitions to GMs (PetitioningAllowed)

`Player.ini › PetitioningAllowed` · on/off

This option is to enable or disable the use of in game petitions. The MaxPetitionsPerPlayer is the amount of petitions a player can make. The MaximumPendingPetitions is the total amount of petitions in the server. Logically, MaximumPendingPetitions must be higher then MaxPetitionsPerPlayer.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Petitions per player](Settings-Characters#petitions-per-player-maxpetitionsperplayer) — it only works while this is On
- **Controls:** [Petitions waiting in total](Settings-Characters#petitions-waiting-in-total-maxpetitionspending) — it only works while this is On

### Petitions per player (MaxPetitionsPerPlayer)

`Player.ini › MaxPetitionsPerPlayer` · number

- **Default:** `5`
- **Allowed:** 0 – 2147483647
- **Needs:** [Allow petitions to GMs](Settings-Characters#allow-petitions-to-gms-petitioningallowed) to be On — Petitions are turned off.
- **Works with:** [Petitions waiting in total](Settings-Characters#petitions-waiting-in-total-maxpetitionspending)

### Petitions waiting in total (MaxPetitionsPending)

`Player.ini › MaxPetitionsPending` · number

- **Default:** `25`
- **Allowed:** 0 – 2147483647
- **Needs:** [Allow petitions to GMs](Settings-Characters#allow-petitions-to-gms-petitioningallowed) to be On — Petitions are turned off.
- **Works with:** [Petitions per player](Settings-Characters#petitions-per-player-maxpetitionsperplayer) — The total waiting must be higher than the number one player may send.

### Recommend the same player more than once a day (AltRecommend)

`Player.ini › AltRecommend` · on/off

Allow character to recommend same person more then once per day.

- **Default:** Off
- **Allowed:** On or Off

### Character deletion waiting time (DeleteCharAfterDays)

`Player.ini › DeleteCharAfterDays` · number · days

0 = characters cannot be deleted.

- **Default:** `7`
- **Allowed:** 0 – 2147483647 days

### Turn off the newbie tutorial (DisableTutorial)

`Player.ini › DisableTutorial` · on/off

Disable tutorial on new player enter into Game Please remember its sometimes important to novice players

- **Default:** Off
- **Allowed:** On or Off

### Words not allowed in character names (ForbiddenNames)

`Player.ini › ForbiddenNames` · list

Comma-separated.

- **Allowed:** words separated by commas

### Messaging someone lets them reply while you are in silence mode (SilenceModeExclude)

`Player.ini › SilenceModeExclude` · on/off

If enabled, when character in silence (block PMs) mode sends a PM to a character, silence mode no longer blocks this character, allowing both characters send each other PMs even with enabled silence mode. The exclude list is cleared each time the character goes into silence mode.

- **Default:** Off
- **Allowed:** On or Off
