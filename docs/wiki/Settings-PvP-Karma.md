# PvP & Karma settings

Player killing, karma, fame and PvP rewards.

**83 settings** in the **PvP & Karma** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Peace zones

### Town PvP rules (PeaceZoneMode)

`General.ini › PeaceZoneMode` · choice

Peace Zone Modes: 0 = Peace All the Time 1 = PVP During Siege for siege participants 2 = PVP All the Time

- **Default:** `0` (Peace All the Time)
- **Allowed:** one of `0` (Peace All the Time), `1` (PVP During Siege for siege participants), `2` (PVP All the Time)

## Karma rules

### Players with karma can be killed in towns (AltKarmaPlayerCanBeKilledInPeaceZone)

`Player.ini › AltKarmaPlayerCanBeKilledInPeaceZone` · on/off

Karma player can be killed in Peace zone.

- **Default:** Off
- **Allowed:** On or Off

### Players with karma can use gatekeepers (AltKarmaPlayerCanUseGK)

`Player.ini › AltKarmaPlayerCanUseGK` · on/off

Karma player can use GateKeeper.

- **Default:** Off
- **Allowed:** On or Off
- **Works with:** [Players with karma can use escape and recall](Settings-PvP-Karma#players-with-karma-can-use-escape-and-recall-altkarmaplayercanteleport)

### Players with karma can use escape and recall (AltKarmaPlayerCanTeleport)

`Player.ini › AltKarmaPlayerCanTeleport` · on/off

Karma player can use escape and recall skills.

- **Default:** On
- **Allowed:** On or Off
- **Works with:** [Players with karma can use gatekeepers](Settings-PvP-Karma#players-with-karma-can-use-gatekeepers-altkarmaplayercanusegk) — Escape and recall are separate from gatekeepers.

### Players with karma can use shops (AltKarmaPlayerCanShop)

`Player.ini › AltKarmaPlayerCanShop` · on/off

Karma player can shop.

- **Default:** On
- **Allowed:** On or Off

### Players with karma can trade (AltKarmaPlayerCanTrade)

`Player.ini › AltKarmaPlayerCanTrade` · on/off

Karma player can trade.

- **Default:** On
- **Allowed:** On or Off

### Players with karma can use the warehouse (AltKarmaPlayerCanUseWareHouse)

`Player.ini › AltKarmaPlayerCanUseWareHouse` · on/off

Karma player can use warehouse.

- **Default:** On
- **Allowed:** On or Off

### Karma lost per death or kill (RateKarmaLost)

`Rates.ini › RateKarmaLost` · number · times

Karma decreasing rate Note: -1 means RateXp so it means it will use retail rate for decreasing karma upon death or receiving exp by farming mobs.

- **Default:** `-1.0`
- **Works with:** [Experience (XP) rate](Settings-Rates-Rewards#experience-xp-rate-ratexp) — -1 means "use the experience rate".

### Experience lost while carrying karma (RateKarmaExpLost)

`Rates.ini › RateKarmaExpLost` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times

## Item loss on death

### GMs can drop equipment on death (CanGMDropEquipment)

`PVP.ini › CanGMDropEquipment` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Pet items that never drop (ListOfPetItems)

`PVP.ini › ListOfPetItems` · list

Warning: Make sure the lists do NOT CONTAIN trailing spaces or spaces between the numbers! List of pet items we cannot drop.

- **Default:** `2375,3500,3501,3502,4422,4423,4424,4425,6648,6649,6650,9882`
- **Allowed:** numbers separated by commas, e.g. `57,4037`

### Items that never drop on death (ListOfNonDroppableItems)

`PVP.ini › ListOfNonDroppableItems` · list

Lists of items which should NEVER be dropped (note, Adena will never be dropped) whether on this list or not

- **Default:** `57,1147,425,1146,461,10,2368,7,6,2370,2369,6842,6611,6612,6613,6614,6615,6616,6617,6618,6619,6620,6621,7694,8181,5575,7694`
- **Allowed:** numbers separated by commas, e.g. `57,4037`

### Player kills needed before items can drop (MinimumPKRequiredToDrop)

`PVP.ini › MinimumPKRequiredToDrop` · number

- **Default:** `5`
- **Allowed:** 0 – 2147483647
- **Works with:** [Chance a normal player drops something on death](Settings-PvP-Karma#chance-a-normal-player-drops-something-on-death-playerratedrop)
- **Works with:** [Chance a player with karma drops something on death](Settings-PvP-Karma#chance-a-player-with-karma-drops-something-on-death-karmaratedrop)

### Most items a normal player can lose on death (PlayerDropLimit)

`Rates.ini › PlayerDropLimit` · number

- **Default:** `3`
- **Allowed:** 0 – 2147483647

### Chance a normal player drops something on death (PlayerRateDrop)

`Rates.ini › PlayerRateDrop` · number · %

in %

- **Default:** `5`
- **Allowed:** 0 – 100 %
- **Works with:** [Player kills needed before items can drop](Settings-PvP-Karma#player-kills-needed-before-items-can-drop-minimumpkrequiredtodrop) — Items only drop once a player has at least this many player kills.

### …of which is an inventory item (PlayerRateDropItem)

`Rates.ini › PlayerRateDropItem` · number · %

in %

- **Default:** `70`
- **Allowed:** 0 – 100 %

### …of which is worn equipment (PlayerRateDropEquip)

`Rates.ini › PlayerRateDropEquip` · number · %

in %

- **Default:** `25`
- **Allowed:** 0 – 100 %

### …of which is the weapon (PlayerRateDropEquipWeapon)

`Rates.ini › PlayerRateDropEquipWeapon` · number · %

in %

- **Default:** `5`
- **Allowed:** 0 – 100 %

### Most items a player with karma can lose on death (KarmaDropLimit)

`Rates.ini › KarmaDropLimit` · number

- **Default:** `10`
- **Allowed:** 0 – 2147483647

### Chance a player with karma drops something on death (KarmaRateDrop)

`Rates.ini › KarmaRateDrop` · number · %

- **Default:** `70`
- **Allowed:** 0 – 100 %
- **Works with:** [Player kills needed before items can drop](Settings-PvP-Karma#player-kills-needed-before-items-can-drop-minimumpkrequiredtodrop) — Items only drop once a player has at least this many player kills.

### …of which is an inventory item (karma) (KarmaRateDropItem)

`Rates.ini › KarmaRateDropItem` · number · %

- **Default:** `50`
- **Allowed:** 0 – 100 %

### …of which is worn equipment (karma) (KarmaRateDropEquip)

`Rates.ini › KarmaRateDropEquip` · number · %

- **Default:** `40`
- **Allowed:** 0 – 100 %

### …of which is the weapon (karma) (KarmaRateDropEquipWeapon)

`Rates.ini › KarmaRateDropEquipWeapon` · number · %

- **Default:** `10`
- **Allowed:** 0 – 100 %

## Anti-feed

### Block feeding kills (AntiFeedEnable)

`PVP.ini › AntiFeedEnable` · on/off

This option will enable antifeed for pvp/pk/clanrep points.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Kills between the same computer don't count](Settings-PvP-Karma#kills-between-the-same-computer-don-t-count-antifeeddualbox) — it only works while this is On
- **Controls:** [Kills of disconnected players don't count](Settings-PvP-Karma#kills-of-disconnected-players-don-t-count-antifeeddisconnectedasdualbox) — it only works while this is On
- **Controls:** [Killing the same player again counts after](Settings-PvP-Karma#killing-the-same-player-again-counts-after-antifeedinterval) — it only works while this is On

### Kills between the same computer don't count (AntiFeedDualbox)

`PVP.ini › AntiFeedDualbox` · on/off

If set to True, kills from dualbox will not increase pvp/pk points and clan reputation will not be transferred.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Block feeding kills](Settings-PvP-Karma#block-feeding-kills-antifeedenable) to be On — Anti-feed is off.

### Kills of disconnected players don't count (AntiFeedDisconnectedAsDualbox)

`PVP.ini › AntiFeedDisconnectedAsDualbox` · on/off

If set to True, server will count disconnected (unable to determine ip address) as dualbox.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Block feeding kills](Settings-PvP-Karma#block-feeding-kills-antifeedenable) to be On — Anti-feed is off.

### Killing the same player again counts after (AntiFeedInterval)

`PVP.ini › AntiFeedInterval` · number · seconds

If character died faster than timeout - pvp/pk points for killer will not increase and clan reputation will not be transferred. Setting to 0 will disable this feature.

- **Default:** `120`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Block feeding kills](Settings-PvP-Karma#block-feeding-kills-antifeedenable) to be On — Anti-feed is off.

## Fame

### Enable fame (EnableFameSystem)

`Player.ini › EnableFameSystem` · on/off

Enable Fame system.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Most fame a player can have](Settings-PvP-Karma#most-fame-a-player-can-have-maxpersonalfamepoints) — it only works while this is On
- **Controls:** [Fortress siege: fame every](Settings-PvP-Karma#fortress-siege-fame-every-fortresszonefametaskfrequency) — it only works while this is On
- **Controls:** [Fortress siege: fame gained](Settings-PvP-Karma#fortress-siege-fame-gained-fortresszonefameaquirepoints) — it only works while this is On
- **Controls:** [Castle siege: fame every](Settings-PvP-Karma#castle-siege-fame-every-castlezonefametaskfrequency) — it only works while this is On
- **Controls:** [Castle siege: fame gained](Settings-PvP-Karma#castle-siege-fame-gained-castlezonefameaquirepoints) — it only works while this is On
- **Controls:** [Dead players still earn fame](Settings-PvP-Karma#dead-players-still-earn-fame-famefordeadplayers) — it only works while this is On

### Most fame a player can have (MaxPersonalFamePoints)

`Player.ini › MaxPersonalFamePoints` · number

The maximum number of Fame points a player can have

- **Default:** `100000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable fame](Settings-PvP-Karma#enable-fame-enablefamesystem) to be On — The fame system is off.

### Fortress siege: fame every (FortressZoneFameTaskFrequency)

`Player.ini › FortressZoneFameTaskFrequency` · number · seconds

How frequently the player gets Fame points while in a Fortress Siege zone

- **Default:** `300`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Enable fame](Settings-PvP-Karma#enable-fame-enablefamesystem) to be On — The fame system is off.

### Fortress siege: fame gained (FortressZoneFameAquirePoints)

`Player.ini › FortressZoneFameAquirePoints` · number

How much Fame aquired while in a Fortress Siege Zone

- **Default:** `31`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable fame](Settings-PvP-Karma#enable-fame-enablefamesystem) to be On — The fame system is off.

### Castle siege: fame every (CastleZoneFameTaskFrequency)

`Player.ini › CastleZoneFameTaskFrequency` · number · seconds

How frequently the player gets Fame points while in a Castle Siege zone

- **Default:** `300`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Enable fame](Settings-PvP-Karma#enable-fame-enablefamesystem) to be On — The fame system is off.

### Castle siege: fame gained (CastleZoneFameAquirePoints)

`Player.ini › CastleZoneFameAquirePoints` · number

How much Fame acquired while in a Castle Siege Zone

- **Default:** `125`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable fame](Settings-PvP-Karma#enable-fame-enablefamesystem) to be On — The fame system is off.

### Dead players still earn fame (FameForDeadPlayers)

`Player.ini › FameForDeadPlayers` · on/off

Dead players can receive fame.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable fame](Settings-PvP-Karma#enable-fame-enablefamesystem) to be On — The fame system is off.

## PvP rewards & announcements

### Enable the "find PvP" teleport (EnableFindPvP)

`Custom/FindPvP.ini › EnableFindPvP` · on/off

Enable FindPvP bypass.

- **Default:** Off
- **Allowed:** On or Off

### Announce player kills (AnnouncePkPvP)

`Custom/PvpAnnounce.ini › AnnouncePkPvP` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Use a normal message instead of a screen message (AnnouncePkPvPNormalMessage)

`Custom/PvpAnnounce.ini › AnnouncePkPvPNormalMessage` · on/off

Announce this as normal system message

- **Default:** On
- **Allowed:** On or Off

### PK announcement text (AnnouncePkMsg)

`Custom/PvpAnnounce.ini › AnnouncePkMsg` · text

$killer and $target are replaced with names.

- **Default:** `$killer has slaughtered $target`

### PvP announcement text (AnnouncePvpMsg)

`Custom/PvpAnnounce.ini › AnnouncePvpMsg` · text

$killer and $target are replaced with names.

- **Default:** `$killer has defeated $target`

### Reward PvP kills with an item (RewardPvpItem)

`Custom/PvpRewardItem.ini › RewardPvpItem` · on/off

Reward item on PvP

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [PvP reward item ID](Settings-PvP-Karma#pvp-reward-item-id-rewardpvpitemid) — it only works while this is On
- **Controls:** [PvP reward amount](Settings-PvP-Karma#pvp-reward-amount-rewardpvpitemamount) — it only works while this is On
- **Controls:** [Tell the player about the PvP reward](Settings-PvP-Karma#tell-the-player-about-the-pvp-reward-rewardpvpitemmessage) — it only works while this is On

### PvP reward item ID (RewardPvpItemId)

`Custom/PvpRewardItem.ini › RewardPvpItemId` · number

Reward PvP item ID

- **Default:** `57`
- **Allowed:** 0 – 2147483647
- **Needs:** [Reward PvP kills with an item](Settings-PvP-Karma#reward-pvp-kills-with-an-item-rewardpvpitem) to be On — PvP rewards are off.

### PvP reward amount (RewardPvpItemAmount)

`Custom/PvpRewardItem.ini › RewardPvpItemAmount` · number

Reward PvP item Amount

- **Default:** `1000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Reward PvP kills with an item](Settings-PvP-Karma#reward-pvp-kills-with-an-item-rewardpvpitem) to be On — PvP rewards are off.

### Tell the player about the PvP reward (RewardPvpItemMessage)

`Custom/PvpRewardItem.ini › RewardPvpItemMessage` · on/off

Show PvP item reward message?

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Reward PvP kills with an item](Settings-PvP-Karma#reward-pvp-kills-with-an-item-rewardpvpitem) to be On — PvP rewards are off.

### Reward PK kills with an item (RewardPkItem)

`Custom/PvpRewardItem.ini › RewardPkItem` · on/off

Reward item on PK

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [PK reward item ID](Settings-PvP-Karma#pk-reward-item-id-rewardpkitemid) — it only works while this is On
- **Controls:** [PK reward amount](Settings-PvP-Karma#pk-reward-amount-rewardpkitemamount) — it only works while this is On
- **Controls:** [Tell the player about the PK reward](Settings-PvP-Karma#tell-the-player-about-the-pk-reward-rewardpkitemmessage) — it only works while this is On

### PK reward item ID (RewardPkItemId)

`Custom/PvpRewardItem.ini › RewardPkItemId` · number

Reward PK item ID

- **Default:** `57`
- **Allowed:** 0 – 2147483647
- **Needs:** [Reward PK kills with an item](Settings-PvP-Karma#reward-pk-kills-with-an-item-rewardpkitem) to be On — PK rewards are off.

### PK reward amount (RewardPkItemAmount)

`Custom/PvpRewardItem.ini › RewardPkItemAmount` · number

Reward PK item Amount

- **Default:** `500`
- **Allowed:** 0 – 2147483647
- **Needs:** [Reward PK kills with an item](Settings-PvP-Karma#reward-pk-kills-with-an-item-rewardpkitem) to be On — PK rewards are off.

### Tell the player about the PK reward (RewardPkItemMessage)

`Custom/PvpRewardItem.ini › RewardPkItemMessage` · on/off

Show PK item reward message?

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Reward PK kills with an item](Settings-PvP-Karma#reward-pk-kills-with-an-item-rewardpkitem) to be On — PK rewards are off.

### No rewards inside instances (DisableRewardsInInstances)

`Custom/PvpRewardItem.ini › DisableRewardsInInstances` · on/off

Disable rewards in instances.

- **Default:** On
- **Allowed:** On or Off

### No rewards inside PvP zones (DisableRewardsInPvpZones)

`Custom/PvpRewardItem.ini › DisableRewardsInPvpZones` · on/off

Disable rewards in PvP zones.

- **Default:** On
- **Allowed:** On or Off

## PvP title colors

### Color titles by PvP count (EnablePvPColorSystem)

`Custom/PvpTitleColor.ini › EnablePvPColorSystem` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [PvP amount 1](Settings-PvP-Karma#pvp-amount-1-pvpamount1) — it only works while this is On
- **Controls:** [Color for amount 1](Settings-PvP-Karma#color-for-amount-1-colorforamount1) — it only works while this is On
- **Controls:** [PvP title for amount 1](Settings-PvP-Karma#pvp-title-for-amount-1-pvptitleforamount1) — it only works while this is On
- **Controls:** [PvP amount 2](Settings-PvP-Karma#pvp-amount-2-pvpamount2) — it only works while this is On
- **Controls:** [Color for amount 2](Settings-PvP-Karma#color-for-amount-2-colorforamount2) — it only works while this is On
- **Controls:** [PvP title for amount 2](Settings-PvP-Karma#pvp-title-for-amount-2-pvptitleforamount2) — it only works while this is On
- **Controls:** [PvP amount 3](Settings-PvP-Karma#pvp-amount-3-pvpamount3) — it only works while this is On
- **Controls:** [Color for amount 3](Settings-PvP-Karma#color-for-amount-3-colorforamount3) — it only works while this is On
- **Controls:** [PvP title for amount 3](Settings-PvP-Karma#pvp-title-for-amount-3-pvptitleforamount3) — it only works while this is On
- **Controls:** [PvP amount 4](Settings-PvP-Karma#pvp-amount-4-pvpamount4) — it only works while this is On
- **Controls:** [Color for amount 4](Settings-PvP-Karma#color-for-amount-4-colorforamount4) — it only works while this is On
- **Controls:** [PvP title for amount 4](Settings-PvP-Karma#pvp-title-for-amount-4-pvptitleforamount4) — it only works while this is On
- **Controls:** [PvP amount 5](Settings-PvP-Karma#pvp-amount-5-pvpamount5) — it only works while this is On
- **Controls:** [Color for amount 5](Settings-PvP-Karma#color-for-amount-5-colorforamount5) — it only works while this is On
- **Controls:** [PvP title for amount 5](Settings-PvP-Karma#pvp-title-for-amount-5-pvptitleforamount5) — it only works while this is On

### PvP amount 1 (PvpAmount1)

`Custom/PvpTitleColor.ini › PvpAmount1` · number

Rank 1

- **Default:** `500`
- **Allowed:** 0 – 2147483647
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### Color for amount 1 (ColorForAmount1)

`Custom/PvpTitleColor.ini › ColorForAmount1` · text

- **Default:** `00FF00`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP title for amount 1 (PvPTitleForAmount1)

`Custom/PvpTitleColor.ini › PvPTitleForAmount1` · text

- **Default:** `Title`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP amount 2 (PvpAmount2)

`Custom/PvpTitleColor.ini › PvpAmount2` · number

Rank 2

- **Default:** `1000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### Color for amount 2 (ColorForAmount2)

`Custom/PvpTitleColor.ini › ColorForAmount2` · text

- **Default:** `00FF00`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP title for amount 2 (PvPTitleForAmount2)

`Custom/PvpTitleColor.ini › PvPTitleForAmount2` · text

- **Default:** `Title`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP amount 3 (PvpAmount3)

`Custom/PvpTitleColor.ini › PvpAmount3` · number

Rank 3

- **Default:** `1500`
- **Allowed:** 0 – 2147483647
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### Color for amount 3 (ColorForAmount3)

`Custom/PvpTitleColor.ini › ColorForAmount3` · text

- **Default:** `00FF00`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP title for amount 3 (PvPTitleForAmount3)

`Custom/PvpTitleColor.ini › PvPTitleForAmount3` · text

- **Default:** `Title`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP amount 4 (PvpAmount4)

`Custom/PvpTitleColor.ini › PvpAmount4` · number

Rank 4

- **Default:** `2500`
- **Allowed:** 0 – 2147483647
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### Color for amount 4 (ColorForAmount4)

`Custom/PvpTitleColor.ini › ColorForAmount4` · text

- **Default:** `00FF00`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP title for amount 4 (PvPTitleForAmount4)

`Custom/PvpTitleColor.ini › PvPTitleForAmount4` · text

- **Default:** `Title`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP amount 5 (PvpAmount5)

`Custom/PvpTitleColor.ini › PvpAmount5` · number

Rank 5

- **Default:** `5000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### Color for amount 5 (ColorForAmount5)

`Custom/PvpTitleColor.ini › ColorForAmount5` · text

- **Default:** `00FF00`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

### PvP title for amount 5 (PvPTitleForAmount5)

`Custom/PvpTitleColor.ini › PvPTitleForAmount5` · text

- **Default:** `Title`
- **Needs:** [Color titles by PvP count](Settings-PvP-Karma#color-titles-by-pvp-count-enablepvpcolorsystem) to be On

## Faction system (Good vs Evil)

### Enable Good vs Evil factions (EnableFactionSystem)

`Custom/FactionSystem.ini › EnableFactionSystem` · on/off

Enable faction system.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Faction: starting location](Settings-PvP-Karma#faction-starting-location-startinglocation) — it only works while this is On
- **Controls:** [Faction: manager NPC location](Settings-PvP-Karma#faction-manager-npc-location-managerspawnlocation) — it only works while this is On
- **Controls:** [Good faction base](Settings-PvP-Karma#good-faction-base-goodbaselocation) — it only works while this is On
- **Controls:** [Evil faction base](Settings-PvP-Karma#evil-faction-base-evilbaselocation) — it only works while this is On
- **Controls:** [Good faction name](Settings-PvP-Karma#good-faction-name-goodteamname) — it only works while this is On
- **Controls:** [Evil faction name](Settings-PvP-Karma#evil-faction-name-evilteamname) — it only works while this is On
- **Controls:** [Good faction name color](Settings-PvP-Karma#good-faction-name-color-goodnamecolor) — it only works while this is On
- **Controls:** [Evil faction name color](Settings-PvP-Karma#evil-faction-name-color-evilnamecolor) — it only works while this is On
- **Controls:** [Faction guards](Settings-PvP-Karma#faction-guards-enablefactionguards) — it only works while this is On
- **Controls:** [Respawn at the faction base](Settings-PvP-Karma#respawn-at-the-faction-base-respawnatfactionbase) — it only works while this is On
- **Controls:** [Faction members become Noblesse](Settings-PvP-Karma#faction-members-become-noblesse-factionautonobless) — it only works while this is On
- **Controls:** [Faction chat](Settings-PvP-Karma#faction-chat-enablefactionchat) — it only works while this is On
- **Controls:** [Keep factions balanced](Settings-PvP-Karma#keep-factions-balanced-balanceonlineplayers) — it only works while this is On
- **Controls:** [Largest allowed faction difference](Settings-PvP-Karma#largest-allowed-faction-difference-balanceplayerexceedlimit) — it only works while this is On

### Faction: starting location (StartingLocation)

`Custom/FactionSystem.ini › StartingLocation` · list

Starting location for all players.

- **Default:** `85332,16199,-1252`
- **Allowed:** `x,y,z` coordinates
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Faction: manager NPC location (ManagerSpawnLocation)

`Custom/FactionSystem.ini › ManagerSpawnLocation` · list

Spawn location for faction manager NPC.

- **Default:** `85712,15974,-1260,26808`
- **Allowed:** `x,y,z` coordinates
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Good faction base (GoodBaseLocation)

`Custom/FactionSystem.ini › GoodBaseLocation` · list

Good base location.

- **Default:** `45306,48878,-3058`
- **Allowed:** `x,y,z` coordinates
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Evil faction base (EvilBaseLocation)

`Custom/FactionSystem.ini › EvilBaseLocation` · list

Evil base location.

- **Default:** `-44037,-113283,-237`
- **Allowed:** `x,y,z` coordinates
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Good faction name (GoodTeamName)

`Custom/FactionSystem.ini › GoodTeamName` · text

Good team name.

- **Default:** `Good`
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Evil faction name (EvilTeamName)

`Custom/FactionSystem.ini › EvilTeamName` · text

Evil team name.

- **Default:** `Evil`
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Good faction name color (GoodNameColor)

`Custom/FactionSystem.ini › GoodNameColor` · text

Good name color.

- **Default:** `00FF00`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Evil faction name color (EvilNameColor)

`Custom/FactionSystem.ini › EvilNameColor` · text

Evil name color.

- **Default:** `0000FF`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Faction guards (EnableFactionGuards)

`Custom/FactionSystem.ini › EnableFactionGuards` · on/off

Enable faction guards. The NPC template must have faction as clan.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Respawn at the faction base (RespawnAtFactionBase)

`Custom/FactionSystem.ini › RespawnAtFactionBase` · on/off

Upon death, respawn at faction base.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Faction members become Noblesse (FactionAutoNobless)

`Custom/FactionSystem.ini › FactionAutoNobless` · on/off

Upon selecting faction, players become nobless.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Faction chat (EnableFactionChat)

`Custom/FactionSystem.ini › EnableFactionChat` · on/off

Disallow chat between factions.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Keep factions balanced (BalanceOnlinePlayers)

`Custom/FactionSystem.ini › BalanceOnlinePlayers` · on/off

Prohibit login when faction has more online players.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

### Largest allowed faction difference (BalancePlayerExceedLimit)

`Custom/FactionSystem.ini › BalancePlayerExceedLimit` · number

Online player exceed limit (used by setting above).

- **Default:** `20`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable Good vs Evil factions](Settings-PvP-Karma#enable-good-vs-evil-factions-enablefactionsystem) to be On

## Other

### Player kills also count as PvP kills (AwardPKKillPVPPoint)

`PVP.ini › AwardPKKillPVPPoint` · on/off

Should we award a pvp point for killing a player with karma?

- **Default:** Off
- **Allowed:** On or Off

### Purple (PvP) flag lasts after attacking (PvPVsNormalTime)

`PVP.ini › PvPVsNormalTime` · number · ms

How much time one stays in PvP mode after hitting an innocent (in ms)

- **Default:** `120000`
- **Allowed:** 0 – 2147483647 ms

### Purple flag lasts after fighting a flagged player (PvPVsPvPTime)

`PVP.ini › PvPVsPvPTime` · number · ms

Length one stays in PvP mode after hitting a purple player (in ms)

- **Default:** `60000`
- **Allowed:** 0 – 2147483647 ms
