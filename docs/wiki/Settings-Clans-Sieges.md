# Clans & Sieges settings

Clans, alliances, castles, fortresses, clan halls and sieges.

**223 settings** in the **Clans & Sieges** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Clans & alliances

### Clan leader changes happen instantly (AltClanLeaderInstantActivation)

`Player.ini › AltClanLeaderInstantActivation` · on/off

When enabled all clan leader requests will be performed instantly.

- **Default:** Off
- **Allowed:** On or Off

### Wait before joining another clan (DaysBeforeJoinAClan)

`Player.ini › DaysBeforeJoinAClan` · number · days

Number of days you have to wait before joining another clan.

- **Default:** `1`
- **Allowed:** 0 – 2147483647 days

### Wait before creating a new clan (DaysBeforeCreateAClan)

`Player.ini › DaysBeforeCreateAClan` · number · days

Number of days you have to wait before creating a new clan.

- **Default:** `10`
- **Allowed:** 0 – 2147483647 days

### Time to dissolve a clan (DaysToPassToDissolveAClan)

`Player.ini › DaysToPassToDissolveAClan` · number · days

Number of days it takes to dissolve a clan.

- **Default:** `7`
- **Allowed:** 0 – 2147483647 days

### Wait before joining an alliance after leaving one (DaysBeforeJoinAllyWhenLeaved)

`Player.ini › DaysBeforeJoinAllyWhenLeaved` · number · days

Number of days before joining a new alliance when clan voluntarily leave an alliance.

- **Default:** `1`
- **Allowed:** 0 – 2147483647 days

### Wait before joining an alliance after being dismissed (DaysBeforeJoinAllyWhenDismissed)

`Player.ini › DaysBeforeJoinAllyWhenDismissed` · number · days

Number of days before joining a new alliance when clan was dismissed from an alliance.

- **Default:** `1`
- **Allowed:** 0 – 2147483647 days

### Wait before an alliance accepts a new clan after dismissing one (DaysBeforeAcceptNewClanWhenDismissed)

`Player.ini › DaysBeforeAcceptNewClanWhenDismissed` · number · days

Number of days before accepting a new clan for alliance when clan was dismissed from an alliance.

- **Default:** `1`
- **Allowed:** 0 – 2147483647 days

### Wait before creating an alliance after dissolving one (DaysBeforeCreateNewAllyWhenDissolved)

`Player.ini › DaysBeforeCreateNewAllyWhenDissolved` · number · days

Number of days before creating a new alliance after dissolving an old alliance.

- **Default:** `1`
- **Allowed:** 0 – 2147483647 days

### Clans per alliance (AltMaxNumOfClansInAlly)

`Player.ini › AltMaxNumOfClansInAlly` · number

Maximum number of clans in alliance.

- **Default:** `3`
- **Allowed:** 1 – 20

### Clan members can take items from the clan warehouse (AltMembersCanWithdrawFromClanWH)

`Player.ini › AltMembersCanWithdrawFromClanWH` · on/off

Allow clan members to withdraw from the clan warehouse.

- **Default:** Off
- **Allowed:** On or Off

### Remove castle circlets when the castle is lost (RemoveCastleCirclets)

`Player.ini › RemoveCastleCirclets` · on/off

Remove castle circlets after a clan loses their castle or a player leaves a clan.

- **Default:** On
- **Allowed:** On or Off

### Clan members needed to declare war (AltClanMembersForWar)

`Player.ini › AltClanMembersForWar` · number

Number of members needed to request a clan war.

- **Default:** `15`
- **Allowed:** 0 – 2147483647

## Clan reputation points

### Reputation for taking a fortress (TakeFortPoints)

`Feature.ini › TakeFortPoints` · number

Clan Reputation Points Reputation score gained by taking Fortress.

- **Default:** `200`
- **Allowed:** 0 – 2147483647

### Reputation for taking a castle (TakeCastlePoints)

`Feature.ini › TakeCastlePoints` · number

Reputation score gained by taking Castle.

- **Default:** `1500`
- **Allowed:** 0 – 2147483647

### Reputation for defending a castle (CastleDefendedPoints)

`Feature.ini › CastleDefendedPoints` · number

Reputation score gained by defended Castle.

- **Default:** `750`
- **Allowed:** 0 – 2147483647

### Reputation for winning the Festival of Darkness (FestivalOfDarknessWin)

`Feature.ini › FestivalOfDarknessWin` · number

Reputation score gained per clan members of festival winning party.

- **Default:** `200`
- **Allowed:** 0 – 2147483647

### Reputation when a member becomes a hero (HeroPoints)

`Feature.ini › HeroPoints` · number

Reputation score gained for per hero clan members.

- **Default:** `1000`
- **Allowed:** 0 – 2147483647

### Reputation for an academy graduate (lowest) (CompleteAcademyMinPoints)

`Feature.ini › CompleteAcademyMinPoints` · number

Minimum Reputation score gained after completing 2nd class transfer under Academy.

- **Default:** `190`
- **Allowed:** 0 – 2147483647

### Reputation for an academy graduate (highest) (CompleteAcademyMaxPoints)

`Feature.ini › CompleteAcademyMaxPoints` · number

Maximum Reputation score gained after completing 2nd class transfer under Academy.

- **Default:** `650`
- **Allowed:** 0 – 2147483647

### Reputation for destroying a ballista (KillBallistaPoints)

`Feature.ini › KillBallistaPoints` · number

Reputation score gained per killed ballista.

- **Default:** `30`
- **Allowed:** 0 – 2147483647

### Reputation per Blood Alliance (BloodAlliancePoints)

`Feature.ini › BloodAlliancePoints` · number

Reputation score gained for one Blood Alliance.

- **Default:** `500`
- **Allowed:** 0 – 2147483647

### Reputation per Blood Oath (BloodOathPoints)

`Feature.ini › BloodOathPoints` · number

Reputation score gained for 10 Blood Oaths.

- **Default:** `200`
- **Allowed:** 0 – 2147483647

### Reputation per Knight's Epaulette (KnightsEpaulettePoints)

`Feature.ini › KnightsEpaulettePoints` · number

Reputation score gained for 100 Knight's Epaulettes.

- **Default:** `20`
- **Allowed:** 0 – 2147483647

### Raid ranking #1: clan reputation reward (1stRaidRankingPoints)

`Feature.ini › 1stRaidRankingPoints` · number

Reputation score gained per clan member listed as top raid killers.

- **Allowed:** 0 or more

### Raid ranking #2: clan reputation reward (2ndRaidRankingPoints)

`Feature.ini › 2ndRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #3: clan reputation reward (3rdRaidRankingPoints)

`Feature.ini › 3rdRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #4: clan reputation reward (4thRaidRankingPoints)

`Feature.ini › 4thRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #5: clan reputation reward (5thRaidRankingPoints)

`Feature.ini › 5thRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #6: clan reputation reward (6thRaidRankingPoints)

`Feature.ini › 6thRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #7: clan reputation reward (7thRaidRankingPoints)

`Feature.ini › 7thRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #8: clan reputation reward (8thRaidRankingPoints)

`Feature.ini › 8thRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #9: clan reputation reward (9thRaidRankingPoints)

`Feature.ini › 9thRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #10: clan reputation reward (10thRaidRankingPoints)

`Feature.ini › 10thRaidRankingPoints` · number

- **Allowed:** 0 or more

### Raid ranking #11–50: clan reputation reward (UpTo50thRaidRankingPoints)

`Feature.ini › UpTo50thRaidRankingPoints` · number

- **Default:** `25`
- **Allowed:** 0 – 2147483647

### Raid ranking #51–100: clan reputation reward (UpTo100thRaidRankingPoints)

`Feature.ini › UpTo100thRaidRankingPoints` · number

- **Default:** `12`
- **Allowed:** 0 – 2147483647

### Reputation per clan war kill (ReputationScorePerKill)

`Feature.ini › ReputationScorePerKill` · number

Reputation score gained/reduced per kill during a clan war or siege war.

- **Default:** `1`
- **Allowed:** 0 – 2147483647

### Reputation lost when a fortress falls (LooseFortPoints)

`Feature.ini › LooseFortPoints` · number

Reputation score reduced by loosing Fortress in battle.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Reputation lost when a castle falls (LooseCastlePoints)

`Feature.ini › LooseCastlePoints` · number

Reputation score reduced by loosing Castle in battle.

- **Default:** `3000`
- **Allowed:** 0 – 2147483647

### Reputation cost: create Royal Guard (CreateRoyalGuardCost)

`Feature.ini › CreateRoyalGuardCost` · number

Reputation score reduced by creating Royal Guard.

- **Default:** `5000`
- **Allowed:** 0 – 2147483647

### Reputation cost: create Order of Knights (CreateKnightUnitCost)

`Feature.ini › CreateKnightUnitCost` · number

Reputation score reduced by creating Knight Unit.

- **Default:** `10000`
- **Allowed:** 0 – 2147483647

### Reputation cost: expand Order of Knights (ReinforceKnightUnitCost)

`Feature.ini › ReinforceKnightUnitCost` · number

Reputation score reduced by reinforcing Knight Unit (if clan level is 9 or more).

- **Default:** `5000`
- **Allowed:** 0 – 2147483647

### Clan level 6: reputation cost (ClanLevel6Cost)

`Feature.ini › ClanLevel6Cost` · number

Reputation score reduced by increasing clan level.

- **Default:** `5000`
- **Allowed:** 0 – 2147483647

### Clan level 7: reputation cost (ClanLevel7Cost)

`Feature.ini › ClanLevel7Cost` · number

- **Default:** `10000`
- **Allowed:** 0 – 2147483647

### Clan level 8: reputation cost (ClanLevel8Cost)

`Feature.ini › ClanLevel8Cost` · number

- **Default:** `20000`
- **Allowed:** 0 – 2147483647

### Clan level 9: reputation cost (ClanLevel9Cost)

`Feature.ini › ClanLevel9Cost` · number

- **Default:** `40000`
- **Allowed:** 0 – 2147483647

### Clan level 10: reputation cost (ClanLevel10Cost)

`Feature.ini › ClanLevel10Cost` · number

- **Default:** `40000`
- **Allowed:** 0 – 2147483647

### Clan level 6: members required (ClanLevel6Requirement)

`Feature.ini › ClanLevel6Requirement` · number

Number of clan members needed to increase clan level.

- **Default:** `30`
- **Allowed:** 0 – 2147483647

### Clan level 7: members required (ClanLevel7Requirement)

`Feature.ini › ClanLevel7Requirement` · number

- **Default:** `50`
- **Allowed:** 0 – 2147483647

### Clan level 8: members required (ClanLevel8Requirement)

`Feature.ini › ClanLevel8Requirement` · number

- **Default:** `80`
- **Allowed:** 0 – 2147483647

### Clan level 9: members required (ClanLevel9Requirement)

`Feature.ini › ClanLevel9Requirement` · number

- **Default:** `120`
- **Allowed:** 0 – 2147483647

### Clan level 10: members required (ClanLevel10Requirement)

`Feature.ini › ClanLevel10Requirement` · number

- **Default:** `140`
- **Allowed:** 0 – 2147483647

## Castle sieges

### Hours a siege may start (SiegeHourList)

`Feature.ini › SiegeHourList` · list

Comma-separated hours.

- **Default:** `16,20`
- **Allowed:** numbers separated by commas, e.g. `57,4037`

### Siege guard hiring price (RateSiegeGuardsPrice)

`Rates.ini › RateSiegeGuardsPrice` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times

### Weeks between castle sieges (SiegeCycle)

`Siege.ini › SiegeCycle` · number · weeks

Siege cycle (in weeks)

- **Default:** `2`
- **Allowed:** 0 or more weeks

### Siege length (SiegeLength)

`Siege.ini › SiegeLength` · number · minutes

Length of siege before the count down (in minutes).

- **Default:** `120`
- **Allowed:** 0 or more minutes

### Headquarters flags per clan (MaxFlags)

`Siege.ini › MaxFlags` · number

Maximum number of flags per clan.

- **Default:** `1`
- **Allowed:** 0 or more

### Clan level needed to join a siege (SiegeClanMinLevel)

`Siege.ini › SiegeClanMinLevel` · number

Minimum level to register.

- **Default:** `4`
- **Allowed:** 0 – 11

### Most attacking clans (AttackerMaxClans)

`Siege.ini › AttackerMaxClans` · number

Max number of clans that can register on each side.

- **Default:** `500`
- **Allowed:** 0 or more

### Most defending clans (DefenderMaxClans)

`Siege.ini › DefenderMaxClans` · number

- **Default:** `500`
- **Allowed:** 0 or more

### Attacker respawn delay (AttackerRespawn)

`Siege.ini › AttackerRespawn` · number · ms

Respawn times (in milliseconds).

- **Default:** `0`
- **Allowed:** 0 or more ms

### Blood Alliance reward for a successful defense (BloodAllianceReward)

`Siege.ini › BloodAllianceReward` · number

Reward successful siege defense with blood alliance in clan warehouse

- **Default:** `1`
- **Allowed:** 0 or more

## Castle tower spawns

### Gludio: flame tower 1 location (GludioFlameTower1)

`Siege.ini › GludioFlameTower1` · list · *Advanced*

Gludio


### Gludio: flame tower 2 location (GludioFlameTower2)

`Siege.ini › GludioFlameTower2` · list · *Advanced*


### Gludio: control tower 1 location (GludioControlTower1)

`Siege.ini › GludioControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Gludio: control tower 2 location (GludioControlTower2)

`Siege.ini › GludioControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Gludio: control tower 3 location (GludioControlTower3)

`Siege.ini › GludioControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Gludio: maximum mercenaries (GludioMaxMercenaries)

`Siege.ini › GludioMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Giran: flame tower 1 location (GiranFlameTower1)

`Siege.ini › GiranFlameTower1` · list · *Advanced*

Giran


### Giran: flame tower 2 location (GiranFlameTower2)

`Siege.ini › GiranFlameTower2` · list · *Advanced*


### Giran: control tower 1 location (GiranControlTower1)

`Siege.ini › GiranControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Giran: control tower 2 location (GiranControlTower2)

`Siege.ini › GiranControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Giran: control tower 3 location (GiranControlTower3)

`Siege.ini › GiranControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Giran: maximum mercenaries (GiranMaxMercenaries)

`Siege.ini › GiranMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Dion: flame tower 1 location (DionFlameTower1)

`Siege.ini › DionFlameTower1` · list · *Advanced*

Dion


### Dion: flame tower 2 location (DionFlameTower2)

`Siege.ini › DionFlameTower2` · list · *Advanced*


### Dion: control tower 1 location (DionControlTower1)

`Siege.ini › DionControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Dion: control tower 2 location (DionControlTower2)

`Siege.ini › DionControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Dion: control tower 3 location (DionControlTower3)

`Siege.ini › DionControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Dion: maximum mercenaries (DionMaxMercenaries)

`Siege.ini › DionMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Oren: flame tower 1 location (OrenFlameTower1)

`Siege.ini › OrenFlameTower1` · list · *Advanced*

Oren


### Oren: flame tower 2 location (OrenFlameTower2)

`Siege.ini › OrenFlameTower2` · list · *Advanced*


### Oren: control tower 1 location (OrenControlTower1)

`Siege.ini › OrenControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Oren: control tower 2 location (OrenControlTower2)

`Siege.ini › OrenControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Oren: control tower 3 location (OrenControlTower3)

`Siege.ini › OrenControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Oren: maximum mercenaries (OrenMaxMercenaries)

`Siege.ini › OrenMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Aden: flame tower 1 location (AdenFlameTower1)

`Siege.ini › AdenFlameTower1` · list · *Advanced*

Aden


### Aden: flame tower 2 location (AdenFlameTower2)

`Siege.ini › AdenFlameTower2` · list · *Advanced*


### Aden: control tower 1 location (AdenControlTower1)

`Siege.ini › AdenControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Aden: control tower 2 location (AdenControlTower2)

`Siege.ini › AdenControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Aden: control tower 3 location (AdenControlTower3)

`Siege.ini › AdenControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Aden: maximum mercenaries (AdenMaxMercenaries)

`Siege.ini › AdenMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Innadril: flame tower 1 location (InnadrilFlameTower1)

`Siege.ini › InnadrilFlameTower1` · list · *Advanced*

Innadril


### Innadril: flame tower 2 location (InnadrilFlameTower2)

`Siege.ini › InnadrilFlameTower2` · list · *Advanced*


### Innadril: control tower 1 location (InnadrilControlTower1)

`Siege.ini › InnadrilControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Innadril: control tower 2 location (InnadrilControlTower2)

`Siege.ini › InnadrilControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Innadril: control tower 3 location (InnadrilControlTower3)

`Siege.ini › InnadrilControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Innadril: maximum mercenaries (InnadrilMaxMercenaries)

`Siege.ini › InnadrilMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Goddard: flame tower 1 location (GoddardFlameTower1)

`Siege.ini › GoddardFlameTower1` · list · *Advanced*

Goddard


### Goddard: flame tower 2 location (GoddardFlameTower2)

`Siege.ini › GoddardFlameTower2` · list · *Advanced*


### Goddard: control tower 1 location (GoddardControlTower1)

`Siege.ini › GoddardControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Goddard: control tower 2 location (GoddardControlTower2)

`Siege.ini › GoddardControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Goddard: control tower 3 location (GoddardControlTower3)

`Siege.ini › GoddardControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Goddard: maximum mercenaries (GoddardMaxMercenaries)

`Siege.ini › GoddardMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Rune: flame tower 1 location (RuneFlameTower1)

`Siege.ini › RuneFlameTower1` · list · *Advanced*

Rune


### Rune: flame tower 2 location (RuneFlameTower2)

`Siege.ini › RuneFlameTower2` · list · *Advanced*


### Rune: control tower 1 location (RuneControlTower1)

`Siege.ini › RuneControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Rune: control tower 2 location (RuneControlTower2)

`Siege.ini › RuneControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Rune: control tower 3 location (RuneControlTower3)

`Siege.ini › RuneControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Rune: maximum mercenaries (RuneMaxMercenaries)

`Siege.ini › RuneMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

### Schuttgart: flame tower 1 location (SchuttgartFlameTower1)

`Siege.ini › SchuttgartFlameTower1` · list · *Advanced*

Schuttgart


### Schuttgart: flame tower 2 location (SchuttgartFlameTower2)

`Siege.ini › SchuttgartFlameTower2` · list · *Advanced*


### Schuttgart: control tower 1 location (SchuttgartControlTower1)

`Siege.ini › SchuttgartControlTower1` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Schuttgart: control tower 2 location (SchuttgartControlTower2)

`Siege.ini › SchuttgartControlTower2` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Schuttgart: control tower 3 location (SchuttgartControlTower3)

`Siege.ini › SchuttgartControlTower3` · list · *Advanced*

- **Allowed:** `x,y,z` coordinates

### Schuttgart: maximum mercenaries (SchuttgartMaxMercenaries)

`Siege.ini › SchuttgartMaxMercenaries` · number · *Advanced*

- **Allowed:** 0 or more

## Castle functions & fees

### Castle teleport: fee payment period (CastleTeleportFunctionFeeRatio)

`Feature.ini › CastleTeleportFunctionFeeRatio` · number · days

Teleport Function price Price = 7 days

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807 days

### Castle teleport: fee at level 1 (CastleTeleportFunctionFeeLvl1)

`Feature.ini › CastleTeleportFunctionFeeLvl1` · number

- **Default:** `1000`
- **Allowed:** 0 – 2147483647

### Castle teleport: fee at level 2 (CastleTeleportFunctionFeeLvl2)

`Feature.ini › CastleTeleportFunctionFeeLvl2` · number

- **Default:** `10000`
- **Allowed:** 0 – 2147483647

### Castle buff support: fee payment period (CastleSupportFunctionFeeRatio)

`Feature.ini › CastleSupportFunctionFeeRatio` · number

Support magic buff price Price = 7 day

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807

### Castle buff support: fee at level 1 (CastleSupportFeeLvl1)

`Feature.ini › CastleSupportFeeLvl1` · number

- **Default:** `49000`
- **Allowed:** 0 – 2147483647

### Castle buff support: fee at level 2 (CastleSupportFeeLvl2)

`Feature.ini › CastleSupportFeeLvl2` · number

- **Default:** `120000`
- **Allowed:** 0 – 2147483647

### Castle mp regeneration: fee payment period (CastleMpRegenerationFunctionFeeRatio)

`Feature.ini › CastleMpRegenerationFunctionFeeRatio` · number

MP Regeneration price Price = 7 day

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807

### Castle mp regeneration: fee at level 1 (CastleMpRegenerationFeeLvl1)

`Feature.ini › CastleMpRegenerationFeeLvl1` · number

- **Default:** `45000`
- **Allowed:** 0 – 2147483647

### Castle mp regeneration: fee at level 2 (CastleMpRegenerationFeeLvl2)

`Feature.ini › CastleMpRegenerationFeeLvl2` · number

- **Default:** `65000`
- **Allowed:** 0 – 2147483647

### Castle hp regeneration: fee payment period (CastleHpRegenerationFunctionFeeRatio)

`Feature.ini › CastleHpRegenerationFunctionFeeRatio` · number

Hp Regeneration price Price = 7 day

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807

### Castle hp regeneration: fee at level 1 (CastleHpRegenerationFeeLvl1)

`Feature.ini › CastleHpRegenerationFeeLvl1` · number

- **Default:** `12000`
- **Allowed:** 0 – 2147483647

### Castle hp regeneration: fee at level 2 (CastleHpRegenerationFeeLvl2)

`Feature.ini › CastleHpRegenerationFeeLvl2` · number

- **Default:** `20000`
- **Allowed:** 0 – 2147483647

### Castle exp recovery: fee payment period (CastleExpRegenerationFunctionFeeRatio)

`Feature.ini › CastleExpRegenerationFunctionFeeRatio` · number

Exp Regeneration price Price = 7 day

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807

### Castle exp recovery: fee at level 1 (CastleExpRegenerationFeeLvl1)

`Feature.ini › CastleExpRegenerationFeeLvl1` · number

- **Default:** `63000`
- **Allowed:** 0 – 2147483647

### Castle exp recovery: fee at level 2 (CastleExpRegenerationFeeLvl2)

`Feature.ini › CastleExpRegenerationFeeLvl2` · number

- **Default:** `70000`
- **Allowed:** 0 – 2147483647

### Castle outer door upgrade price (level 2) (OuterDoorUpgradePriceLvl2)

`Feature.ini › OuterDoorUpgradePriceLvl2` · number

Outer Door upgrade price

- **Default:** `3000000`
- **Allowed:** 0 – 2147483647

### Castle outer door upgrade price (level 3) (OuterDoorUpgradePriceLvl3)

`Feature.ini › OuterDoorUpgradePriceLvl3` · number

- **Default:** `4000000`
- **Allowed:** 0 – 2147483647

### Castle outer door upgrade price (level 5) (OuterDoorUpgradePriceLvl5)

`Feature.ini › OuterDoorUpgradePriceLvl5` · number

- **Default:** `5000000`
- **Allowed:** 0 – 2147483647

### Castle inner door upgrade price (level 2) (InnerDoorUpgradePriceLvl2)

`Feature.ini › InnerDoorUpgradePriceLvl2` · number

Inner Door upgrade price

- **Default:** `750000`
- **Allowed:** 0 – 2147483647

### Castle inner door upgrade price (level 3) (InnerDoorUpgradePriceLvl3)

`Feature.ini › InnerDoorUpgradePriceLvl3` · number

- **Default:** `900000`
- **Allowed:** 0 – 2147483647

### Castle inner door upgrade price (level 5) (InnerDoorUpgradePriceLvl5)

`Feature.ini › InnerDoorUpgradePriceLvl5` · number

- **Default:** `1000000`
- **Allowed:** 0 – 2147483647

### Castle wall upgrade price (level 2) (WallUpgradePriceLvl2)

`Feature.ini › WallUpgradePriceLvl2` · number

Wall upgrade price

- **Default:** `1600000`
- **Allowed:** 0 – 2147483647

### Castle wall upgrade price (level 3) (WallUpgradePriceLvl3)

`Feature.ini › WallUpgradePriceLvl3` · number

- **Default:** `1800000`
- **Allowed:** 0 – 2147483647

### Castle wall upgrade price (level 5) (WallUpgradePriceLvl5)

`Feature.ini › WallUpgradePriceLvl5` · number

- **Default:** `2000000`
- **Allowed:** 0 – 2147483647

### Castle trap upgrade price (level 1) (TrapUpgradePriceLvl1)

`Feature.ini › TrapUpgradePriceLvl1` · number

Trap upgrade price

- **Default:** `3000000`
- **Allowed:** 0 – 2147483647

### Castle trap upgrade price (level 2) (TrapUpgradePriceLvl2)

`Feature.ini › TrapUpgradePriceLvl2` · number

- **Default:** `4000000`
- **Allowed:** 0 – 2147483647

### Castle trap upgrade price (level 3) (TrapUpgradePriceLvl3)

`Feature.ini › TrapUpgradePriceLvl3` · number

- **Default:** `5000000`
- **Allowed:** 0 – 2147483647

### Castle trap upgrade price (level 4) (TrapUpgradePriceLvl4)

`Feature.ini › TrapUpgradePriceLvl4` · number

- **Default:** `6000000`
- **Allowed:** 0 – 2147483647

## Fortress functions & fees

### Fortress teleport: fee payment period (FortressTeleportFunctionFeeRatio)

`Feature.ini › FortressTeleportFunctionFeeRatio` · number · days

Teleport Function price Price = 7 days

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807 days

### Fortress teleport: fee at level 1 (FortressTeleportFunctionFeeLvl1)

`Feature.ini › FortressTeleportFunctionFeeLvl1` · number

- **Default:** `1000`
- **Allowed:** 0 – 2147483647

### Fortress teleport: fee at level 2 (FortressTeleportFunctionFeeLvl2)

`Feature.ini › FortressTeleportFunctionFeeLvl2` · number

- **Default:** `10000`
- **Allowed:** 0 – 2147483647

### Fortress buff support: fee payment period (FortressSupportFunctionFeeRatio)

`Feature.ini › FortressSupportFunctionFeeRatio` · number

Support magic buff price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Fortress buff support: fee at level 1 (FortressSupportFeeLvl1)

`Feature.ini › FortressSupportFeeLvl1` · number

- **Default:** `7000`
- **Allowed:** 0 – 2147483647

### Fortress buff support: fee at level 2 (FortressSupportFeeLvl2)

`Feature.ini › FortressSupportFeeLvl2` · number

- **Default:** `17000`
- **Allowed:** 0 – 2147483647

### Fortress mp regeneration: fee payment period (FortressMpRegenerationFunctionFeeRatio)

`Feature.ini › FortressMpRegenerationFunctionFeeRatio` · number

MP Regeneration price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Fortress mp regeneration: fee at level 1 (FortressMpRegenerationFeeLvl1)

`Feature.ini › FortressMpRegenerationFeeLvl1` · number

- **Default:** `6500`
- **Allowed:** 0 – 2147483647

### Fortress mp regeneration: fee at level 2 (FortressMpRegenerationFeeLvl2)

`Feature.ini › FortressMpRegenerationFeeLvl2` · number

- **Default:** `9300`
- **Allowed:** 0 – 2147483647

### Fortress hp regeneration: fee payment period (FortressHpRegenerationFunctionFeeRatio)

`Feature.ini › FortressHpRegenerationFunctionFeeRatio` · number

Hp Regeneration price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Fortress hp regeneration: fee at level 1 (FortressHpRegenerationFeeLvl1)

`Feature.ini › FortressHpRegenerationFeeLvl1` · number

- **Default:** `2000`
- **Allowed:** 0 – 2147483647

### Fortress hp regeneration: fee at level 2 (FortressHpRegenerationFeeLvl2)

`Feature.ini › FortressHpRegenerationFeeLvl2` · number

- **Default:** `3500`
- **Allowed:** 0 – 2147483647

### Fortress exp recovery: fee payment period (FortressExpRegenerationFunctionFeeRatio)

`Feature.ini › FortressExpRegenerationFunctionFeeRatio` · number

Exp Regeneration price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Fortress exp recovery: fee at level 1 (FortressExpRegenerationFeeLvl1)

`Feature.ini › FortressExpRegenerationFeeLvl1` · number

- **Default:** `9000`
- **Allowed:** 0 – 2147483647

### Fortress exp recovery: fee at level 2 (FortressExpRegenerationFeeLvl2)

`Feature.ini › FortressExpRegenerationFeeLvl2` · number

- **Default:** `10000`
- **Allowed:** 0 – 2147483647

### Fortress reward interval (FortressPeriodicUpdateFrequency)

`Feature.ini › FortressPeriodicUpdateFrequency` · number · minutes

This is the time frequently when Fort owner gets Blood Oath, supply level raised and Fort fee is payed Default 360 mins

- **Default:** `360`
- **Allowed:** 0 – 2147483647 minutes

### Blood Oaths per fortress reward (FortressBloodOathCount)

`Feature.ini › FortressBloodOathCount` · number

The number of Blood Oath which given to the Fort owner clan when Fort Updater runs

- **Default:** `1`
- **Allowed:** 0 – 2147483647

### Fortress supply level cap (FortressMaxSupplyLevel)

`Feature.ini › FortressMaxSupplyLevel` · number

The maximum Fort supply level Max lvl what you can define here is 21!

- **Default:** `6`
- **Allowed:** 0 – 2147483647

### Fortress tax paid to the castle (FortressFeeForCastle)

`Feature.ini › FortressFeeForCastle` · number · adena

Fort fee which payed to the Castle

- **Default:** `25000`
- **Allowed:** 0 – 2147483647 adena

### Longest a clan can hold a fortress (FortressMaximumOwnTime)

`Feature.ini › FortressMaximumOwnTime` · number · hours

The maximum time while a clan can own a fortress Deafault: 168 hours

- **Default:** `168`
- **Allowed:** 0 – 2147483647 hours

## Clan hall functions & fees

### Clan hall teleport: fee payment period (ClanHallTeleportFunctionFeeRatio)

`Feature.ini › ClanHallTeleportFunctionFeeRatio` · number · days

Teleport Function price Price = 7 days

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807 days

### Clan hall teleport: fee at level 1 (ClanHallTeleportFunctionFeeLvl1)

`Feature.ini › ClanHallTeleportFunctionFeeLvl1` · number

- **Default:** `7000`
- **Allowed:** 0 – 2147483647

### Clan hall teleport: fee at level 2 (ClanHallTeleportFunctionFeeLvl2)

`Feature.ini › ClanHallTeleportFunctionFeeLvl2` · number

- **Default:** `14000`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee payment period (ClanHallSupportFunctionFeeRatio)

`Feature.ini › ClanHallSupportFunctionFeeRatio` · number

Support magic buff price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Clan hall buff support: fee at level 1 (ClanHallSupportFeeLvl1)

`Feature.ini › ClanHallSupportFeeLvl1` · number

- **Default:** `2500`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee at level 2 (ClanHallSupportFeeLvl2)

`Feature.ini › ClanHallSupportFeeLvl2` · number

- **Default:** `5000`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee at level 3 (ClanHallSupportFeeLvl3)

`Feature.ini › ClanHallSupportFeeLvl3` · number

- **Default:** `7000`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee at level 4 (ClanHallSupportFeeLvl4)

`Feature.ini › ClanHallSupportFeeLvl4` · number

- **Default:** `11000`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee at level 5 (ClanHallSupportFeeLvl5)

`Feature.ini › ClanHallSupportFeeLvl5` · number

- **Default:** `21000`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee at level 6 (ClanHallSupportFeeLvl6)

`Feature.ini › ClanHallSupportFeeLvl6` · number

- **Default:** `36000`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee at level 7 (ClanHallSupportFeeLvl7)

`Feature.ini › ClanHallSupportFeeLvl7` · number

- **Default:** `37000`
- **Allowed:** 0 – 2147483647

### Clan hall buff support: fee at level 8 (ClanHallSupportFeeLvl8)

`Feature.ini › ClanHallSupportFeeLvl8` · number

- **Default:** `52000`
- **Allowed:** 0 – 2147483647

### Clan hall mp regeneration: fee payment period (ClanHallMpRegenerationFunctionFeeRatio)

`Feature.ini › ClanHallMpRegenerationFunctionFeeRatio` · number

MP Regeneration price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Clan hall mp regeneration: fee at level 1 (ClanHallMpRegenerationFeeLvl1)

`Feature.ini › ClanHallMpRegenerationFeeLvl1` · number

- **Default:** `2000`
- **Allowed:** 0 – 2147483647

### Clan hall mp regeneration: fee at level 2 (ClanHallMpRegenerationFeeLvl2)

`Feature.ini › ClanHallMpRegenerationFeeLvl2` · number

- **Default:** `3750`
- **Allowed:** 0 – 2147483647

### Clan hall mp regeneration: fee at level 3 (ClanHallMpRegenerationFeeLvl3)

`Feature.ini › ClanHallMpRegenerationFeeLvl3` · number

- **Default:** `6500`
- **Allowed:** 0 – 2147483647

### Clan hall mp regeneration: fee at level 4 (ClanHallMpRegenerationFeeLvl4)

`Feature.ini › ClanHallMpRegenerationFeeLvl4` · number

- **Default:** `13750`
- **Allowed:** 0 – 2147483647

### Clan hall mp regeneration: fee at level 5 (ClanHallMpRegenerationFeeLvl5)

`Feature.ini › ClanHallMpRegenerationFeeLvl5` · number

- **Default:** `20000`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee payment period (ClanHallHpRegenerationFunctionFeeRatio)

`Feature.ini › ClanHallHpRegenerationFunctionFeeRatio` · number

Hp Regeneration price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Clan hall hp regeneration: fee at level 1 (ClanHallHpRegenerationFeeLvl1)

`Feature.ini › ClanHallHpRegenerationFeeLvl1` · number

- **Default:** `700`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 2 (ClanHallHpRegenerationFeeLvl2)

`Feature.ini › ClanHallHpRegenerationFeeLvl2` · number

- **Default:** `800`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 3 (ClanHallHpRegenerationFeeLvl3)

`Feature.ini › ClanHallHpRegenerationFeeLvl3` · number

- **Default:** `1000`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 4 (ClanHallHpRegenerationFeeLvl4)

`Feature.ini › ClanHallHpRegenerationFeeLvl4` · number

- **Default:** `1166`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 5 (ClanHallHpRegenerationFeeLvl5)

`Feature.ini › ClanHallHpRegenerationFeeLvl5` · number

- **Default:** `1500`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 6 (ClanHallHpRegenerationFeeLvl6)

`Feature.ini › ClanHallHpRegenerationFeeLvl6` · number

- **Default:** `1750`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 7 (ClanHallHpRegenerationFeeLvl7)

`Feature.ini › ClanHallHpRegenerationFeeLvl7` · number

- **Default:** `2000`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 8 (ClanHallHpRegenerationFeeLvl8)

`Feature.ini › ClanHallHpRegenerationFeeLvl8` · number

- **Default:** `2250`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 9 (ClanHallHpRegenerationFeeLvl9)

`Feature.ini › ClanHallHpRegenerationFeeLvl9` · number

- **Default:** `2500`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 10 (ClanHallHpRegenerationFeeLvl10)

`Feature.ini › ClanHallHpRegenerationFeeLvl10` · number

- **Default:** `3250`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 11 (ClanHallHpRegenerationFeeLvl11)

`Feature.ini › ClanHallHpRegenerationFeeLvl11` · number

- **Default:** `3270`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 12 (ClanHallHpRegenerationFeeLvl12)

`Feature.ini › ClanHallHpRegenerationFeeLvl12` · number

- **Default:** `4250`
- **Allowed:** 0 – 2147483647

### Clan hall hp regeneration: fee at level 13 (ClanHallHpRegenerationFeeLvl13)

`Feature.ini › ClanHallHpRegenerationFeeLvl13` · number

- **Default:** `5166`
- **Allowed:** 0 – 2147483647

### Clan hall exp recovery: fee payment period (ClanHallExpRegenerationFunctionFeeRatio)

`Feature.ini › ClanHallExpRegenerationFunctionFeeRatio` · number

Exp Regeneration price Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Clan hall exp recovery: fee at level 1 (ClanHallExpRegenerationFeeLvl1)

`Feature.ini › ClanHallExpRegenerationFeeLvl1` · number

- **Default:** `3000`
- **Allowed:** 0 – 2147483647

### Clan hall exp recovery: fee at level 2 (ClanHallExpRegenerationFeeLvl2)

`Feature.ini › ClanHallExpRegenerationFeeLvl2` · number

- **Default:** `6000`
- **Allowed:** 0 – 2147483647

### Clan hall exp recovery: fee at level 3 (ClanHallExpRegenerationFeeLvl3)

`Feature.ini › ClanHallExpRegenerationFeeLvl3` · number

- **Default:** `9000`
- **Allowed:** 0 – 2147483647

### Clan hall exp recovery: fee at level 4 (ClanHallExpRegenerationFeeLvl4)

`Feature.ini › ClanHallExpRegenerationFeeLvl4` · number

- **Default:** `15000`
- **Allowed:** 0 – 2147483647

### Clan hall exp recovery: fee at level 5 (ClanHallExpRegenerationFeeLvl5)

`Feature.ini › ClanHallExpRegenerationFeeLvl5` · number

- **Default:** `21000`
- **Allowed:** 0 – 2147483647

### Clan hall exp recovery: fee at level 6 (ClanHallExpRegenerationFeeLvl6)

`Feature.ini › ClanHallExpRegenerationFeeLvl6` · number

- **Default:** `23330`
- **Allowed:** 0 – 2147483647

### Clan hall exp recovery: fee at level 7 (ClanHallExpRegenerationFeeLvl7)

`Feature.ini › ClanHallExpRegenerationFeeLvl7` · number

- **Default:** `30000`
- **Allowed:** 0 – 2147483647

### Clan hall item creation: fee payment period (ClanHallItemCreationFunctionFeeRatio)

`Feature.ini › ClanHallItemCreationFunctionFeeRatio` · number

Creation item function Price = 1 day

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807

### Clan hall item creation: fee at level 1 (ClanHallItemCreationFunctionFeeLvl1)

`Feature.ini › ClanHallItemCreationFunctionFeeLvl1` · number

- **Default:** `30000`
- **Allowed:** 0 – 2147483647

### Clan hall item creation: fee at level 2 (ClanHallItemCreationFunctionFeeLvl2)

`Feature.ini › ClanHallItemCreationFunctionFeeLvl2` · number

- **Default:** `70000`
- **Allowed:** 0 – 2147483647

### Clan hall item creation: fee at level 3 (ClanHallItemCreationFunctionFeeLvl3)

`Feature.ini › ClanHallItemCreationFunctionFeeLvl3` · number

- **Default:** `140000`
- **Allowed:** 0 – 2147483647

### Clan hall curtains: fee payment period (ClanHallCurtainFunctionFeeRatio)

`Feature.ini › ClanHallCurtainFunctionFeeRatio` · number · days

Curtains Decoration Price = 7 days

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807 days

### Clan hall curtains: fee at level 1 (ClanHallCurtainFunctionFeeLvl1)

`Feature.ini › ClanHallCurtainFunctionFeeLvl1` · number

- **Default:** `2000`
- **Allowed:** 0 – 2147483647

### Clan hall curtains: fee at level 2 (ClanHallCurtainFunctionFeeLvl2)

`Feature.ini › ClanHallCurtainFunctionFeeLvl2` · number

- **Default:** `2500`
- **Allowed:** 0 – 2147483647

### Clan hall front platform: fee payment period (ClanHallFrontPlatformFunctionFeeRatio)

`Feature.ini › ClanHallFrontPlatformFunctionFeeRatio` · number · days

Fixtures Decoration Price = 3 days

- **Default:** `259200000`
- **Allowed:** 0 – 9223372036854775807 days

### Clan hall front platform: fee at level 1 (ClanHallFrontPlatformFunctionFeeLvl1)

`Feature.ini › ClanHallFrontPlatformFunctionFeeLvl1` · number

- **Default:** `1300`
- **Allowed:** 0 – 2147483647

### Clan hall front platform: fee at level 2 (ClanHallFrontPlatformFunctionFeeLvl2)

`Feature.ini › ClanHallFrontPlatformFunctionFeeLvl2` · number

- **Default:** `4000`
- **Allowed:** 0 – 2147483647

### Clan hall buffs cost no MP (AltClanHallMpBuffFree)

`Feature.ini › AltClanHallMpBuffFree` · on/off

If true Clan Hall buff cost 0 mp.

- **Default:** Off
- **Allowed:** On or Off

## Contestable clan halls

### Clan level needed to register (MinClanLevel)

`ConquerableHallSiege.ini › MinClanLevel` · number

Min level that each clan needs to register for the siege

- **Default:** `4`
- **Allowed:** 0 – 11

### Most attacking clans (MaxAttackers)

`ConquerableHallSiege.ini › MaxAttackers` · number

Max number of clans allowed to register for the battle

- **Default:** `500`
- **Allowed:** 0 – 2147483647

### Flags per clan (MaxFlagsPerClan)

`ConquerableHallSiege.ini › MaxFlagsPerClan` · number

Max numbers of flags that each clan is allowed to put

- **Default:** `1`
- **Allowed:** 0 – 2147483647

### Participants earn fame (EnableFame)

`ConquerableHallSiege.ini › EnableFame` · on/off

Enable the fame reward

- **Default:** Off
- **Allowed:** On or Off

### Fame gained (FameAmount)

`ConquerableHallSiege.ini › FameAmount` · number

Fame amount

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Fame every (FameFrequency)

`ConquerableHallSiege.ini › FameFrequency` · number · seconds

Fame Frequency

- **Default:** `0`
- **Allowed:** 0 – 2147483647 seconds

## Mounts during sieges

### Wyverns can be ridden anywhere (AllowRideWyvernAlways)

`Feature.ini › AllowRideWyvernAlways` · on/off

Allow riding wyvern ignoring 7 Signs status. This will allow Castle Lords to ride wyvern even when Dusk has won Seal of Strife.

- **Default:** Off
- **Allowed:** On or Off

### Wyverns can be ridden during sieges (AllowRideWyvernDuringSiege)

`Feature.ini › AllowRideWyvernDuringSiege` · on/off

Allow riding wyvern during Castle/Fort Siege. All players are restricted from riding wyverns during a siege. However, the castle lord (clan leader who owns the castle being sieged) is still allowed to ride a wyvern during the siege. This matches official behavior.

- **Default:** On
- **Allowed:** On or Off

### Mounts can be ridden during sieges (AllowRideMountsDuringSiege)

`Feature.ini › AllowRideMountsDuringSiege` · on/off

Allow riding mounts (wyvern excluded) during Castle/Fort Siege.

- **Default:** Off
- **Allowed:** On or Off
