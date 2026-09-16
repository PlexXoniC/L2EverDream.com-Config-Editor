# Monsters & Bosses settings

Monster behaviour, champions, stat multipliers and boss spawns.

**127 settings** in the **Monsters & Bosses** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## NPCs

### NPC idle animation: shortest delay (MinNpcAnimation)

`General.ini › MinNpcAnimation` · number · seconds

Minimum and maximum variables in seconds for NPC animation delay. You must keep MinNpcAnimation lower or equal to MaxNpcAnimation. Set values to 0 for disabling random animations.

- **Default:** `5`
- **Allowed:** 0 – 2147483647 seconds
- **Works with:** [NPC idle animation: longest delay](Settings-Monsters-Bosses#npc-idle-animation-longest-delay-maxnpcanimation) — The shortest delay must not be longer than the longest.

### NPC idle animation: longest delay (MaxNpcAnimation)

`General.ini › MaxNpcAnimation` · number · seconds

- **Default:** `60`
- **Allowed:** 0 – 2147483647 seconds
- **Works with:** [NPC idle animation: shortest delay](Settings-Monsters-Bosses#npc-idle-animation-shortest-delay-minnpcanimation)

### Monster idle animation: shortest delay (MinMonsterAnimation)

`General.ini › MinMonsterAnimation` · number · seconds

- **Default:** `5`
- **Allowed:** 0 – 2147483647 seconds
- **Works with:** [Monster idle animation: longest delay](Settings-Monsters-Bosses#monster-idle-animation-longest-delay-maxmonsteranimation) — The shortest delay must not be longer than the longest.

### Monster idle animation: longest delay (MaxMonsterAnimation)

`General.ini › MaxMonsterAnimation` · number · seconds

- **Default:** `60`
- **Allowed:** 0 – 2147483647 seconds
- **Works with:** [Monster idle animation: shortest delay](Settings-Monsters-Bosses#monster-idle-animation-shortest-delay-minmonsteranimation)

### Announce where Mammon's merchants appear (AnnounceMammonSpawn)

`NPC.ini › AnnounceMammonSpawn` · on/off

Global announcements will be made indicating Blacksmith/Merchant of Mammon Spawning points.

- **Default:** Off
- **Allowed:** On or Off

### Aggressive monsters attack in peace zones (AltMobAgroInPeaceZone)

`NPC.ini › AltMobAgroInPeaceZone` · on/off

True - Mobs can be aggressive while in peace zones. False - Mobs can NOT be aggressive while in peace zones.

- **Default:** On
- **Allowed:** On or Off

### NPCs can be attacked (AltAttackableNpcs)

`NPC.ini › AltAttackableNpcs` · on/off

Defines whether NPCs are attackable by default

- **Default:** On
- **Allowed:** On or Off

### Show NPC details on Shift+click (AltGameViewNpc)

`NPC.ini › AltGameViewNpc` · on/off · *added by L2Everdream*

Allows non-GM players to view NPC stats via shift-click

- **Default:** Off
- **Allowed:** On or Off

### Show monster levels (ShowNpcLevel)

`NPC.ini › ShowNpcLevel` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Mark aggressive monsters (ShowNpcAggression)

`NPC.ini › ShowNpcAggression` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Monsters stay near players they killed (AttackablesCampPlayerCorpses)

`NPC.ini › AttackablesCampPlayerCorpses` · on/off

Attackables do not leave player corpses.

- **Default:** Off
- **Allowed:** On or Off

### Show clan crests on NPCs without the quest (ShowCrestWithoutQuest)

`NPC.ini › ShowCrestWithoutQuest` · on/off

Show clan, alliance crests for territory NPCs without quests

- **Default:** Off
- **Allowed:** On or Off

### NPC weapons glow with random enchant effects (EnableRandomEnchantEffect)

`NPC.ini › EnableRandomEnchantEffect` · on/off

Custom random EnchantEffect All npcs with weapons get random weapon enchanted value Enchantment is only visual, range is 4-21

- **Default:** Off
- **Allowed:** On or Off

## Monsters

### Corpse decay check interval (DecayTimeTask)

`NPC.ini › DecayTimeTask` · number · ms

Decay Time Task (don't set it too low!) (in milliseconds):

- **Default:** `5000`
- **Allowed:** 0 – 2147483647 ms

### Monster corpses stay for (DefaultCorpseTime)

`NPC.ini › DefaultCorpseTime` · number · seconds

This is the default corpse time (in seconds).

- **Default:** `7`
- **Allowed:** 0 – 2147483647 seconds

### Spoiled corpses stay extra (SpoiledCorpseExtendTime)

`NPC.ini › SpoiledCorpseExtendTime` · number · seconds

This is the time that will be added to spoiled corpse time (in seconds).

- **Default:** `10`
- **Allowed:** 0 – 2147483647 seconds

### Corpse skills can be used until decay minus (CorpseConsumeSkillAllowedTimeBeforeDecay)

`NPC.ini › CorpseConsumeSkillAllowedTimeBeforeDecay` · number · ms

The time allowed to use a corpse consume skill before the corpse decays.

- **Default:** `2000`
- **Allowed:** 0 – 2147483647 ms

### Monsters chase players for up to (MaxAggroRange)

`NPC.ini › MaxAggroRange` · number

Maximum distance mobs can get aggro. Related post at https://l2jserver.com/forum/viewtopic.php?f=128&t=31588 L2jMobius: 450

- **Default:** `450`
- **Allowed:** 0 – 2147483647

### Monsters wander up to (MaxDriftRange)

`NPC.ini › MaxDriftRange` · number

Maximum distance monsters can randomly move from spawn.

- **Default:** `300`
- **Allowed:** 0 – 2147483647

### Monsters give up when pulled too far (AggroDistanceCheckEnabled)

`NPC.ini › AggroDistanceCheckEnabled` · on/off · *added by L2Everdream*

Enable monster aggro distance check. When enabled monsters will lose aggro if pulled far away from spawn.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** […give-up distance](Settings-Monsters-Bosses#give-up-distance-aggrodistancecheckrange) — it only works while this is On
- **Controls:** […also applies to raid bosses](Settings-Monsters-Bosses#also-applies-to-raid-bosses-aggrodistancecheckraids) — it only works while this is On
- **Controls:** […also applies inside instances](Settings-Monsters-Bosses#also-applies-inside-instances-aggrodistancecheckinstances) — it only works while this is On
- **Controls:** […monsters heal fully when they give up](Settings-Monsters-Bosses#monsters-heal-fully-when-they-give-up-aggrodistancecheckrestorelife) — it only works while this is On

### …give-up distance (AggroDistanceCheckRange)

`NPC.ini › AggroDistanceCheckRange` · number

Maximum distance monsters can be pulled away from spawn. Overridden by Spawn chaseRange parameter.

- **Default:** `1500`
- **Allowed:** 0 – 2147483647
- **Needs:** [Monsters give up when pulled too far](Settings-Monsters-Bosses#monsters-give-up-when-pulled-too-far-aggrodistancecheckenabled) to be On — Monsters never give up while the distance check is off.

### …also applies to raid bosses (AggroDistanceCheckRaids)

`NPC.ini › AggroDistanceCheckRaids` · on/off

Use maximum aggro distance check for raids. Grandbosses are excluded.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Monsters give up when pulled too far](Settings-Monsters-Bosses#monsters-give-up-when-pulled-too-far-aggrodistancecheckenabled) to be On — Monsters never give up while the distance check is off.
- **Controls:** […raid boss give-up distance](Settings-Monsters-Bosses#raid-boss-give-up-distance-aggrodistancecheckraidrange) — it only works while this is On

### …raid boss give-up distance (AggroDistanceCheckRaidRange)

`NPC.ini › AggroDistanceCheckRaidRange` · number

Maximum distance raids can be pulled away from spawn. Overridden by Spawn chaseRange parameter.

- **Default:** `3000`
- **Allowed:** 0 – 2147483647
- **Needs:** […also applies to raid bosses](Settings-Monsters-Bosses#also-applies-to-raid-bosses-aggrodistancecheckraids) to be On — Raid bosses are not included in the distance check.

### …also applies inside instances (AggroDistanceCheckInstances)

`NPC.ini › AggroDistanceCheckInstances` · on/off

Use maximum aggro distance check in instances.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Monsters give up when pulled too far](Settings-Monsters-Bosses#monsters-give-up-when-pulled-too-far-aggrodistancecheckenabled) to be On — Monsters never give up while the distance check is off.

### …monsters heal fully when they give up (AggroDistanceCheckRestoreLife)

`NPC.ini › AggroDistanceCheckRestoreLife` · on/off

Restore monster HP and MP when aggro is reset by distance.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Monsters give up when pulled too far](Settings-Monsters-Bosses#monsters-give-up-when-pulled-too-far-aggrodistancecheckenabled) to be On — Monsters never give up while the distance check is off.

## Champion monsters

### Enable champion monsters (ChampionEnable)

`Custom/ChampionMonsters.ini › ChampionEnable` · on/off

Enable/Disable Champion Mob System.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Champions can be passive monsters](Settings-Monsters-Bosses#champions-can-be-passive-monsters-championpassive) — it only works while this is On
- **Controls:** [Chance a monster is a champion](Settings-Monsters-Bosses#chance-a-monster-is-a-champion-championfrequency) — it only works while this is On
- **Controls:** [Champion title](Settings-Monsters-Bosses#champion-title-championtitle) — it only works while this is On
- **Controls:** [Champions glow](Settings-Monsters-Bosses#champions-glow-championaura) — it only works while this is On
- **Controls:** [Lowest champion level](Settings-Monsters-Bosses#lowest-champion-level-championminlevel) — it only works while this is On
- **Controls:** [Highest champion level](Settings-Monsters-Bosses#highest-champion-level-championmaxlevel) — it only works while this is On
- **Controls:** [Champion HP](Settings-Monsters-Bosses#champion-hp-championhp) — it only works while this is On
- **Controls:** [Champion HP regeneration](Settings-Monsters-Bosses#champion-hp-regeneration-championhpregen) — it only works while this is On
- **Controls:** [Champion experience and SP](Settings-Monsters-Bosses#champion-experience-and-sp-championrewardsexpsp) — it only works while this is On
- **Controls:** [Champion drop chance](Settings-Monsters-Bosses#champion-drop-chance-championrewardschance) — it only works while this is On
- **Controls:** [Champion drop amount](Settings-Monsters-Bosses#champion-drop-amount-championrewardsamount) — it only works while this is On
- **Controls:** [Champion adena chance](Settings-Monsters-Bosses#champion-adena-chance-championadenasrewardschance) — it only works while this is On
- **Controls:** [Champion adena amount](Settings-Monsters-Bosses#champion-adena-amount-championadenasrewardsamount) — it only works while this is On
- **Controls:** [Champion attack](Settings-Monsters-Bosses#champion-attack-championatk) — it only works while this is On
- **Controls:** [Champion attack speed](Settings-Monsters-Bosses#champion-attack-speed-championspdatk) — it only works while this is On
- **Controls:** [Champion bonus items](Settings-Monsters-Bosses#champion-bonus-items-championrewarditems) — it only works while this is On
- **Controls:** [Bonus item chance when the player is lower level](Settings-Monsters-Bosses#bonus-item-chance-when-the-player-is-lower-level-championrewardlowerlvlitemchance) — it only works while this is On
- **Controls:** [Bonus item chance when the player is higher level](Settings-Monsters-Bosses#bonus-item-chance-when-the-player-is-higher-level-championrewardhigherlvlitemchance) — it only works while this is On
- **Controls:** [Champions inside instances](Settings-Monsters-Bosses#champions-inside-instances-championenableininstances) — it only works while this is On

### Champions can be passive monsters (ChampionPassive)

`Custom/ChampionMonsters.ini › ChampionPassive` · on/off

Force Champion mobs to be passive? To leave champion mobs to default/Aggressive, set to False. To set all champion mobs to Passive, set True.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Chance a monster is a champion (ChampionFrequency)

`Custom/ChampionMonsters.ini › ChampionFrequency` · number · %

% chance for a mob to became champion (0 to disable).

- **Default:** `0`
- **Allowed:** 0 – 100 %
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion title (ChampionTitle)

`Custom/ChampionMonsters.ini › ChampionTitle` · text

Title of all Champion Mobs.

- **Default:** `Champion`
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champions glow (ChampionAura)

`Custom/ChampionMonsters.ini › ChampionAura` · on/off

Red aura for Champion Mobs.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Lowest champion level (ChampionMinLevel)

`Custom/ChampionMonsters.ini › ChampionMinLevel` · number

Min and max levels allowed for a mob to be a Champion mob.

- **Default:** `20`
- **Allowed:** 1 – 99
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Highest champion level (ChampionMaxLevel)

`Custom/ChampionMonsters.ini › ChampionMaxLevel` · number

- **Default:** `60`
- **Allowed:** 1 – 99
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion HP (ChampionHp)

`Custom/ChampionMonsters.ini › ChampionHp` · number · times

Hp multiplier for Champion mobs.

- **Default:** `7`
- **Allowed:** 0 – 2147483647 times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion HP regeneration (ChampionHpRegen)

`Custom/ChampionMonsters.ini › ChampionHpRegen` · number · times

Hp Regen Multiplier for Champion mobs.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion experience and SP (ChampionRewardsExpSp)

`Custom/ChampionMonsters.ini › ChampionRewardsExpSp` · number · times

Exp/Sp rewards multiplier for Champion mobs.

- **Default:** `8.0`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion drop chance (ChampionRewardsChance)

`Custom/ChampionMonsters.ini › ChampionRewardsChance` · number · times

Standard rewards chance multiplier for Champion mobs.

- **Default:** `8.0`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion drop amount (ChampionRewardsAmount)

`Custom/ChampionMonsters.ini › ChampionRewardsAmount` · number · times

Standard rewards amount multiplier for Champion mobs.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion adena chance (ChampionAdenasRewardsChance)

`Custom/ChampionMonsters.ini › ChampionAdenasRewardsChance` · number · times

Adena & Seal Stone rewards chance multiplier for Champion mobs.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion adena amount (ChampionAdenasRewardsAmount)

`Custom/ChampionMonsters.ini › ChampionAdenasRewardsAmount` · number · times

Adena & Seal Stone rewards amount multiplier for Champion mobs.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion attack (ChampionAtk)

`Custom/ChampionMonsters.ini › ChampionAtk` · number · times

P. Attack and M. Attack bonus for Champion mobs.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion attack speed (ChampionSpdAtk)

`Custom/ChampionMonsters.ini › ChampionSpdAtk` · number · times

Physical/Magical Attack Speed bonus for Champion mobs.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion bonus items (ChampionRewardItems)

`Custom/ChampionMonsters.ini › ChampionRewardItems` · list

Specified id and amount of reward a player will receive if they are awarded the item. Separated by ; Example: 6393,1;57,5000

- **Default:** `4356,10`
- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Bonus item chance when the player is lower level (ChampionRewardLowerLvlItemChance)

`Custom/ChampionMonsters.ini › ChampionRewardLowerLvlItemChance` · number · %

% Chance to obtain a specified reward item from a lower level Champion mob.

- **Default:** `0`
- **Allowed:** 0 – 100 %
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Bonus item chance when the player is higher level (ChampionRewardHigherLvlItemChance)

`Custom/ChampionMonsters.ini › ChampionRewardHigherLvlItemChance` · number · %

% Chance to obtain a specified reward item from a higher level Champion mob.

- **Default:** `0`
- **Allowed:** 0 – 100 %
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

### Champion kills use vitality (ChampionEnableVitality)

`Custom/ChampionMonsters.ini › ChampionEnableVitality` · on/off

Do you want to enable the vitality calculation when killing champion mobs? Be aware that it can lead to huge unbalance on your server, your rate for that mob would then be "mobXP x serverRate x vitalityRate x championXpRate Works only if EnableVitality = True

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — The vitality system is off.

### Champions inside instances (ChampionEnableInInstances)

`Custom/ChampionMonsters.ini › ChampionEnableInInstances` · on/off

Enable spawning of the champions in instances Default = False

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable champion monsters](Settings-Monsters-Bosses#enable-champion-monsters-championenable) to be On

## NPC stat multipliers

### Enable NPC stat multipliers (EnableNpcStatMultipliers)

`Custom/NpcStatMultipliers.ini › EnableNpcStatMultipliers` · on/off

Enable/Disable NPC stat multipliers.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Monsters: HP multiplier](Settings-Monsters-Bosses#monsters-hp-multiplier-monsterhp) — it only works while this is On
- **Controls:** [Monsters: MP multiplier](Settings-Monsters-Bosses#monsters-mp-multiplier-monstermp) — it only works while this is On
- **Controls:** [Monsters: P. Atk. multiplier](Settings-Monsters-Bosses#monsters-p-atk-multiplier-monsterpatk) — it only works while this is On
- **Controls:** [Monsters: M. Atk. multiplier](Settings-Monsters-Bosses#monsters-m-atk-multiplier-monstermatk) — it only works while this is On
- **Controls:** [Monsters: P. Def. multiplier](Settings-Monsters-Bosses#monsters-p-def-multiplier-monsterpdef) — it only works while this is On
- **Controls:** [Monsters: M. Def. multiplier](Settings-Monsters-Bosses#monsters-m-def-multiplier-monstermdef) — it only works while this is On
- **Controls:** [Monsters: aggro range multiplier](Settings-Monsters-Bosses#monsters-aggro-range-multiplier-monsteraggrorange) — it only works while this is On
- **Controls:** [Monsters: help-call range multiplier](Settings-Monsters-Bosses#monsters-help-call-range-multiplier-monsterclanhelprange) — it only works while this is On
- **Controls:** [Raid bosses: HP multiplier](Settings-Monsters-Bosses#raid-bosses-hp-multiplier-raidbosshp) — it only works while this is On
- **Controls:** [Raid bosses: MP multiplier](Settings-Monsters-Bosses#raid-bosses-mp-multiplier-raidbossmp) — it only works while this is On
- **Controls:** [Raid bosses: P. Atk. multiplier](Settings-Monsters-Bosses#raid-bosses-p-atk-multiplier-raidbosspatk) — it only works while this is On
- **Controls:** [Raid bosses: M. Atk. multiplier](Settings-Monsters-Bosses#raid-bosses-m-atk-multiplier-raidbossmatk) — it only works while this is On
- **Controls:** [Raid bosses: P. Def. multiplier](Settings-Monsters-Bosses#raid-bosses-p-def-multiplier-raidbosspdef) — it only works while this is On
- **Controls:** [Raid bosses: M. Def. multiplier](Settings-Monsters-Bosses#raid-bosses-m-def-multiplier-raidbossmdef) — it only works while this is On
- **Controls:** [Raid bosses: aggro range multiplier](Settings-Monsters-Bosses#raid-bosses-aggro-range-multiplier-raidbossaggrorange) — it only works while this is On
- **Controls:** [Raid bosses: help-call range multiplier](Settings-Monsters-Bosses#raid-bosses-help-call-range-multiplier-raidbossclanhelprange) — it only works while this is On
- **Controls:** [Guards: HP multiplier](Settings-Monsters-Bosses#guards-hp-multiplier-guardhp) — it only works while this is On
- **Controls:** [Guards: MP multiplier](Settings-Monsters-Bosses#guards-mp-multiplier-guardmp) — it only works while this is On
- **Controls:** [Guards: P. Atk. multiplier](Settings-Monsters-Bosses#guards-p-atk-multiplier-guardpatk) — it only works while this is On
- **Controls:** [Guards: M. Atk. multiplier](Settings-Monsters-Bosses#guards-m-atk-multiplier-guardmatk) — it only works while this is On
- **Controls:** [Guards: P. Def. multiplier](Settings-Monsters-Bosses#guards-p-def-multiplier-guardpdef) — it only works while this is On
- **Controls:** [Guards: M. Def. multiplier](Settings-Monsters-Bosses#guards-m-def-multiplier-guardmdef) — it only works while this is On
- **Controls:** [Guards: aggro range multiplier](Settings-Monsters-Bosses#guards-aggro-range-multiplier-guardaggrorange) — it only works while this is On
- **Controls:** [Guards: help-call range multiplier](Settings-Monsters-Bosses#guards-help-call-range-multiplier-guardclanhelprange) — it only works while this is On
- **Controls:** [Siege defenders: HP multiplier](Settings-Monsters-Bosses#siege-defenders-hp-multiplier-defenderhp) — it only works while this is On
- **Controls:** [Siege defenders: MP multiplier](Settings-Monsters-Bosses#siege-defenders-mp-multiplier-defendermp) — it only works while this is On
- **Controls:** [Siege defenders: P. Atk. multiplier](Settings-Monsters-Bosses#siege-defenders-p-atk-multiplier-defenderpatk) — it only works while this is On
- **Controls:** [Siege defenders: M. Atk. multiplier](Settings-Monsters-Bosses#siege-defenders-m-atk-multiplier-defendermatk) — it only works while this is On
- **Controls:** [Siege defenders: P. Def. multiplier](Settings-Monsters-Bosses#siege-defenders-p-def-multiplier-defenderpdef) — it only works while this is On
- **Controls:** [Siege defenders: M. Def. multiplier](Settings-Monsters-Bosses#siege-defenders-m-def-multiplier-defendermdef) — it only works while this is On
- **Controls:** [Siege defenders: aggro range multiplier](Settings-Monsters-Bosses#siege-defenders-aggro-range-multiplier-defenderaggrorange) — it only works while this is On
- **Controls:** [Siege defenders: help-call range multiplier](Settings-Monsters-Bosses#siege-defenders-help-call-range-multiplier-defenderclanhelprange) — it only works while this is On

### Monsters: HP multiplier (MonsterHP)

`Custom/NpcStatMultipliers.ini › MonsterHP` · number

Monsters

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Monsters: MP multiplier (MonsterMP)

`Custom/NpcStatMultipliers.ini › MonsterMP` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Monsters: P. Atk. multiplier (MonsterPAtk)

`Custom/NpcStatMultipliers.ini › MonsterPAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Monsters: M. Atk. multiplier (MonsterMAtk)

`Custom/NpcStatMultipliers.ini › MonsterMAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Monsters: P. Def. multiplier (MonsterPDef)

`Custom/NpcStatMultipliers.ini › MonsterPDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Monsters: M. Def. multiplier (MonsterMDef)

`Custom/NpcStatMultipliers.ini › MonsterMDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Monsters: aggro range multiplier (MonsterAggroRange)

`Custom/NpcStatMultipliers.ini › MonsterAggroRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Monsters: help-call range multiplier (MonsterClanHelpRange)

`Custom/NpcStatMultipliers.ini › MonsterClanHelpRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: HP multiplier (RaidbossHP)

`Custom/NpcStatMultipliers.ini › RaidbossHP` · number

Raidbosses

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: MP multiplier (RaidbossMP)

`Custom/NpcStatMultipliers.ini › RaidbossMP` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: P. Atk. multiplier (RaidbossPAtk)

`Custom/NpcStatMultipliers.ini › RaidbossPAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: M. Atk. multiplier (RaidbossMAtk)

`Custom/NpcStatMultipliers.ini › RaidbossMAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: P. Def. multiplier (RaidbossPDef)

`Custom/NpcStatMultipliers.ini › RaidbossPDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: M. Def. multiplier (RaidbossMDef)

`Custom/NpcStatMultipliers.ini › RaidbossMDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: aggro range multiplier (RaidbossAggroRange)

`Custom/NpcStatMultipliers.ini › RaidbossAggroRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Raid bosses: help-call range multiplier (RaidbossClanHelpRange)

`Custom/NpcStatMultipliers.ini › RaidbossClanHelpRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: HP multiplier (GuardHP)

`Custom/NpcStatMultipliers.ini › GuardHP` · number

Guards

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: MP multiplier (GuardMP)

`Custom/NpcStatMultipliers.ini › GuardMP` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: P. Atk. multiplier (GuardPAtk)

`Custom/NpcStatMultipliers.ini › GuardPAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: M. Atk. multiplier (GuardMAtk)

`Custom/NpcStatMultipliers.ini › GuardMAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: P. Def. multiplier (GuardPDef)

`Custom/NpcStatMultipliers.ini › GuardPDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: M. Def. multiplier (GuardMDef)

`Custom/NpcStatMultipliers.ini › GuardMDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: aggro range multiplier (GuardAggroRange)

`Custom/NpcStatMultipliers.ini › GuardAggroRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Guards: help-call range multiplier (GuardClanHelpRange)

`Custom/NpcStatMultipliers.ini › GuardClanHelpRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: HP multiplier (DefenderHP)

`Custom/NpcStatMultipliers.ini › DefenderHP` · number

Defenders

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: MP multiplier (DefenderMP)

`Custom/NpcStatMultipliers.ini › DefenderMP` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: P. Atk. multiplier (DefenderPAtk)

`Custom/NpcStatMultipliers.ini › DefenderPAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: M. Atk. multiplier (DefenderMAtk)

`Custom/NpcStatMultipliers.ini › DefenderMAtk` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: P. Def. multiplier (DefenderPDef)

`Custom/NpcStatMultipliers.ini › DefenderPDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: M. Def. multiplier (DefenderMDef)

`Custom/NpcStatMultipliers.ini › DefenderMDef` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: aggro range multiplier (DefenderAggroRange)

`Custom/NpcStatMultipliers.ini › DefenderAggroRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

### Siege defenders: help-call range multiplier (DefenderClanHelpRange)

`Custom/NpcStatMultipliers.ini › DefenderClanHelpRange` · number

- **Default:** `1`
- **Allowed:** 0 or more
- **Needs:** [Enable NPC stat multipliers](Settings-Monsters-Bosses#enable-npc-stat-multipliers-enablenpcstatmultipliers) to be On

## Raid bosses

### Raid boss HP regeneration (RaidHpRegenMultiplier)

`NPC.ini › RaidHpRegenMultiplier` · number · %

Percent of HP and MP regeneration for raid bosses. Example: Setting HP to 10 will cause raid boss HP to regenerate 90% slower than normal.

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Raid boss MP regeneration (RaidMpRegenMultiplier)

`NPC.ini › RaidMpRegenMultiplier` · number · %

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Raid boss P. Def. (RaidPDefenceMultiplier)

`NPC.ini › RaidPDefenceMultiplier` · number · %

Percent of physical and magical defense for raid bosses. Example: A setting of 10 will cause defense to be 90% lower than normal, while 110 will cause defense to be 10% higher than normal.

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Raid boss M. Def. (RaidMDefenceMultiplier)

`NPC.ini › RaidMDefenceMultiplier` · number · %

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Raid boss P. Atk. (RaidPAttackMultiplier)

`NPC.ini › RaidPAttackMultiplier` · number · %

Percent of physical and magical attack for raid bosses. Example: A setting of 10 will cause attack to be 90% lower than normal, while 110 will cause attack to be 10% higher than normal.

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Raid boss M. Atk. (RaidMAttackMultiplier)

`NPC.ini › RaidMAttackMultiplier` · number · %

- **Default:** `100.0`
- **Allowed:** 0 or more %

### Raid boss shortest respawn (RaidMinRespawnMultiplier)

`NPC.ini › RaidMinRespawnMultiplier` · number · times

Configure Minimum and Maximum time multiplier between raid boss re-spawn. By default 12Hours*1.0 for Minimum Time and 24Hours*1.0 for Maximum Time. Example: Setting RaidMaxRespawnMultiplier to 2 will make the time between re-spawn 24 hours to 48 hours.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Raid boss longest respawn](Settings-Monsters-Bosses#raid-boss-longest-respawn-raidmaxrespawnmultiplier) — Raid respawn falls between these two; keep the shortest below the longest.

### Raid boss longest respawn (RaidMaxRespawnMultiplier)

`NPC.ini › RaidMaxRespawnMultiplier` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Raid boss shortest respawn](Settings-Monsters-Bosses#raid-boss-shortest-respawn-raidminrespawnmultiplier)

### Raid minions respawn after (RaidMinionRespawnTime)

`NPC.ini › RaidMinionRespawnTime` · number · ms

Configure the interval at which raid boss minions will re-spawn. This time is in milliseconds, 1 minute is 60000 milliseconds.

- **Default:** `300000`
- **Allowed:** 0 – 2147483647 ms

### Custom minion respawn times (CustomMinionsRespawnTime)

`NPC.ini › CustomMinionsRespawnTime` · list

Format: npcId,seconds;npcId,seconds

- **Allowed:** `id,value` pairs separated by semicolons, e.g. `57,2;4037,1.5`

### Remove minions when their boss dies (ForceDeleteMinions)

`NPC.ini › ForceDeleteMinions` · on/off

Force delete spawned minions upon master death. By default minions are deleted only for raidbosses.

- **Default:** Off
- **Allowed:** On or Off

### Turn off the raid curse (Raid Fear/Silence on high-level players) (DisableRaidCurse)

`NPC.ini › DisableRaidCurse` · on/off

Disable Raid Curse if raid more than 8 levels lower.

- **Default:** Off
- **Allowed:** On or Off

### Raid bosses switch targets after (RaidChaosTime)

`NPC.ini › RaidChaosTime` · number · seconds

Configure the interval at which raid bosses and minions wont reconsider their target This time is in seconds, 1 minute is 60 seconds.

- **Default:** `10`
- **Allowed:** 0 – 2147483647 seconds

### Grand bosses switch targets after (GrandChaosTime)

`NPC.ini › GrandChaosTime` · number · seconds

- **Default:** `10`
- **Allowed:** 0 – 2147483647 seconds

### Minions switch targets after (MinionChaosTime)

`NPC.ini › MinionChaosTime` · number · seconds

- **Default:** `10`
- **Allowed:** 0 – 2147483647 seconds

## Grand bosses

### Antharas: time before the fight starts (AntharasWaitTime)

`GrandBoss.ini › AntharasWaitTime` · number

Delay of appearance time of Antharas. Value is minute. Range 3-60

- **Default:** `30`
- **Allowed:** 0 – 2147483647

### Antharas: respawn interval (IntervalOfAntharasSpawn)

`GrandBoss.ini › IntervalOfAntharasSpawn` · number · hours

Interval time of Antharas. Value is hour. Range 1-480

- **Default:** `264`
- **Allowed:** 0 – 2147483647 hours

### Antharas: random extra respawn time (RandomOfAntharasSpawn)

`GrandBoss.ini › RandomOfAntharasSpawn` · number

Random interval. Range 1-192

- **Default:** `72`
- **Allowed:** 0 – 2147483647

### Antharas: greets heroes who enter (AntharasRecognizeHero)

`GrandBoss.ini › AntharasRecognizeHero` · on/off

Does Antharas recognizes presence of a Hero player.

- **Default:** On
- **Allowed:** On or Off

### Valakas: time before the fight starts (ValakasWaitTime)

`GrandBoss.ini › ValakasWaitTime` · number

Delay of appearance time of Valakas. Value is minute. Range 3-60

- **Default:** `30`
- **Allowed:** 0 – 2147483647

### Valakas: respawn interval (IntervalOfValakasSpawn)

`GrandBoss.ini › IntervalOfValakasSpawn` · number · hours

Interval time of Valakas. Value is hour. Range 1-480

- **Default:** `264`
- **Allowed:** 0 – 2147483647 hours

### Valakas: random extra respawn time (RandomOfValakasSpawn)

`GrandBoss.ini › RandomOfValakasSpawn` · number

Random interval. Range 1-192

- **Default:** `72`
- **Allowed:** 0 – 2147483647

### Valakas: greets heroes who enter (ValakasRecognizeHero)

`GrandBoss.ini › ValakasRecognizeHero` · on/off

Does Valakas recognizes presence of a Hero player.

- **Default:** On
- **Allowed:** On or Off

### Baium: respawn interval (IntervalOfBaiumSpawn)

`GrandBoss.ini › IntervalOfBaiumSpawn` · number · hours

Interval time of Baium. Value is hour. Range 1-480

- **Default:** `168`
- **Allowed:** 0 – 2147483647 hours

### Baium: random extra respawn time (RandomOfBaiumSpawn)

`GrandBoss.ini › RandomOfBaiumSpawn` · number

Random interval. Range 1-192

- **Default:** `48`
- **Allowed:** 0 – 2147483647

### Baium: greets heroes who enter (BaiumRecognizeHero)

`GrandBoss.ini › BaiumRecognizeHero` · on/off

Does Baium recognizes presence of a Hero player.

- **Default:** On
- **Allowed:** On or Off

### Core: respawn interval (IntervalOfCoreSpawn)

`GrandBoss.ini › IntervalOfCoreSpawn` · number · hours

Interval time of Core. Value is hour. Range 1-480

- **Default:** `60`
- **Allowed:** 0 – 2147483647 hours

### Core: random extra respawn time (RandomOfCoreSpawn)

`GrandBoss.ini › RandomOfCoreSpawn` · number

Random interval. Range 1-192

- **Default:** `24`
- **Allowed:** 0 – 2147483647

### Orfen: respawn interval (IntervalOfOrfenSpawn)

`GrandBoss.ini › IntervalOfOrfenSpawn` · number · hours

Interval time of Orfen. Value is hour. Range 1-480

- **Default:** `48`
- **Allowed:** 0 – 2147483647 hours

### Orfen: random extra respawn time (RandomOfOrfenSpawn)

`GrandBoss.ini › RandomOfOrfenSpawn` · number

Random interval. Range 1-192

- **Default:** `20`
- **Allowed:** 0 – 2147483647

### Queen ant: respawn interval (IntervalOfQueenAntSpawn)

`GrandBoss.ini › IntervalOfQueenAntSpawn` · number · hours

Interval time of QueenAnt. Value is hour. Range 1-480

- **Default:** `36`
- **Allowed:** 0 – 2147483647 hours

### Queen ant: random extra respawn time (RandomOfQueenAntSpawn)

`GrandBoss.ini › RandomOfQueenAntSpawn` · number

Random interval. Range 1-192

- **Default:** `17`
- **Allowed:** 0 – 2147483647

### Zaken: respawn interval (IntervalOfZakenSpawn)

`GrandBoss.ini › IntervalOfZakenSpawn` · number · hours

Interval time of Zaken. Value is hour. Range 1-480

- **Default:** `36`
- **Allowed:** 0 – 2147483647 hours

### Zaken: random extra respawn time (RandomOfZakenSpawn)

`GrandBoss.ini › RandomOfZakenSpawn` · number

Random interval. Range 1-192

- **Default:** `17`
- **Allowed:** 0 – 2147483647

### Frintezza: respawn interval (IntervalOfFrintezzaSpawn)

`GrandBoss.ini › IntervalOfFrintezzaSpawn` · number · hours

Interval time of Frintezza. Value is hour. Range 1-480

- **Default:** `48`
- **Allowed:** 0 – 2147483647 hours

### Frintezza: random extra respawn time (RandomOfFrintezzaSpawn)

`GrandBoss.ini › RandomOfFrintezzaSpawn` · number

Random interval. Range 1-192

- **Default:** `8`
- **Allowed:** 0 – 2147483647

## Boss announcements

### Announce raid boss spawns (RaidBossSpawnAnnouncements)

`Custom/BossAnnouncements.ini › RaidBossSpawnAnnouncements` · on/off

Enable RaidBoss spawn announcements.

- **Default:** Off
- **Allowed:** On or Off

### Announce raid boss kills (RaidBossDefeatAnnouncements)

`Custom/BossAnnouncements.ini › RaidBossDefeatAnnouncements` · on/off

Enable RaidBoss defeat announcements.

- **Default:** Off
- **Allowed:** On or Off

### Also announce raid bosses in instances (RaidBossInstanceAnnouncements)

`Custom/BossAnnouncements.ini › RaidBossInstanceAnnouncements` · on/off

Enable RaidBoss announcements in instances.

- **Default:** Off
- **Allowed:** On or Off

### Announce grand boss spawns (GrandBossSpawnAnnouncements)

`Custom/BossAnnouncements.ini › GrandBossSpawnAnnouncements` · on/off

Enable GrandBoss spawn announcements.

- **Default:** Off
- **Allowed:** On or Off

### Announce grand boss kills (GrandBossDefeatAnnouncements)

`Custom/BossAnnouncements.ini › GrandBossDefeatAnnouncements` · on/off

Enable GrandBoss defeat announcements.

- **Default:** Off
- **Allowed:** On or Off

### Also announce grand bosses in instances (GrandBossInstanceAnnouncements)

`Custom/BossAnnouncements.ini › GrandBossInstanceAnnouncements` · on/off

Enable GrandBoss announcements in instances.

- **Default:** Off
- **Allowed:** On or Off

### Bosses never announced when they spawn (RaidbossExcludedFromSpawnAnnouncements)

`Custom/BossAnnouncements.ini › RaidbossExcludedFromSpawnAnnouncements` · list

Exclude certain Raid Bosses or Grand Bosses from spawn announcements. Add the NPC IDs of bosses to exclude, separated by commas. Leave empty to include all.


### Bosses never announced when killed (RaidbossExcludedFromDefeatAnnouncements)

`Custom/BossAnnouncements.ini › RaidbossExcludedFromDefeatAnnouncements` · list

Exclude certain Raid Bosses or Grand Bosses from defeat announcements. Add the NPC IDs of bosses to exclude, separated by commas. Leave empty to include all.


## Random spawns

### Monsters spawn at random spots nearby (EnableRandomMonsterSpawns)

`Custom/RandomSpawns.ini › EnableRandomMonsterSpawns` · on/off

Enable random monster spawns.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Random spawn distance](Settings-Monsters-Bosses#random-spawn-distance-maxspawnmobrange) — it only works while this is On
- **Controls:** [Monsters that always use their exact spot](Settings-Monsters-Bosses#monsters-that-always-use-their-exact-spot-mobsspawnnotrandom) — it only works while this is On

### Random spawn distance (MaxSpawnMobRange)

`Custom/RandomSpawns.ini › MaxSpawnMobRange` · number

Max range for X and Y coords.

- **Default:** `150`
- **Allowed:** 0 – 2147483647
- **Needs:** [Monsters spawn at random spots nearby](Settings-Monsters-Bosses#monsters-spawn-at-random-spots-nearby-enablerandommonsterspawns) to be On

### Monsters that always use their exact spot (MobsSpawnNotRandom)

`Custom/RandomSpawns.ini › MobsSpawnNotRandom` · list

Examples: No random spawns for Kasha's Eye, Pagan Guards, Sel Mahums, Four Sepulchers MobsSpawnNotRandom = 18812,18813,18814

- **Default:** `18812,18813,18814,22138`
- **Needs:** [Monsters spawn at random spots nearby](Settings-Monsters-Bosses#monsters-spawn-at-random-spots-nearby-enablerandommonsterspawns) to be On

## Guards

### Guards attack aggressive monsters (GuardAttackAggroMob)

`NPC.ini › GuardAttackAggroMob` · on/off

True - Allows guards to attack aggressive mobs within range.

- **Default:** Off
- **Allowed:** On or Off
