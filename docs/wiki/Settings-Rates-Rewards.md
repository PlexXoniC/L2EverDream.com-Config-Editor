# Rates & Rewards settings

How fast characters level and how much monsters and quests give.

**50 settings** in the **Rates & Rewards** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

> **Setting these by hand is easy to get wrong.** The [Rates](Rates-and-Drops) tab turns one number into all of them and splits it between drop chance and drop amount so none of it is wasted, and the [Drops](Rates-and-Drops#the-drops-tab) tab shows what your world really gives, monster by monster.

## Experience & skill points

### Experience (XP) rate (RateXp)

`Rates.ini › RateXp` · number · times

How fast characters gain experience from monsters. 1 = retail.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Extra experience for being in a party](Settings-Rates-Rewards#extra-experience-for-being-in-a-party-ratepartyxp)
- **Works with:** [Karma lost per death or kill](Settings-PvP-Karma#karma-lost-per-death-or-kill-ratekarmalost)

### Skill point (SP) rate (RateSp)

`Rates.ini › RateSp` · number · times

How fast characters gain skill points from monsters. 1 = retail.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Extra skill points for being in a party](Settings-Rates-Rewards#extra-skill-points-for-being-in-a-party-ratepartysp)

### Extra experience for being in a party (RatePartyXp)

`Rates.ini › RatePartyXp` · number · times

Multiplies only the size bonus a party gets, not the experience itself. The experience rate does that. At 1 a party still earns the normal party bonus.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Experience (XP) rate](Settings-Rates-Rewards#experience-xp-rate-ratexp) — The experience rate multiplies the experience itself; this one only multiplies the extra a party gets for its size.

### Extra skill points for being in a party (RatePartySp)

`Rates.ini › RatePartySp` · number · times

Multiplies only the size bonus a party gets, not the skill points themselves. The skill point rate does that.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Skill point (SP) rate](Settings-Rates-Rewards#skill-point-sp-rate-ratesp) — The skill point rate multiplies the skill points themselves; this one only multiplies the extra a party gets for its size.

### No experience from monsters more than this many levels below you (MonsterExpMaxLevelDifference)

`Rates.ini › MonsterExpMaxLevelDifference` · number · levels

The maximum monster level difference for rewarding experience.

- **Default:** `11`
- **Allowed:** 0 – 2147483647 levels

## Quest rewards

### Quest item drop amount (QuestItemDropAmountMultiplier)

`Rates.ini › QuestItemDropAmountMultiplier` · number · times

Quest item drop amount multiplier.

- **Default:** `1`
- **Allowed:** 0 or more times

### Quest reward experience (RateQuestRewardXP)

`Rates.ini › RateQuestRewardXP` · number · times

Exp/SP reward multipliers

- **Default:** `1`
- **Allowed:** 0 or more times

### Quest reward skill points (RateQuestRewardSP)

`Rates.ini › RateQuestRewardSP` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times

### Quest reward adena (RateQuestRewardAdena)

`Rates.ini › RateQuestRewardAdena` · number · times

Adena reward multiplier

- **Default:** `1`
- **Allowed:** 0 or more times

### Use separate quest reward multipliers per item type (UseQuestRewardMultipliers)

`Rates.ini › UseQuestRewardMultipliers` · on/off

Use additional item multipliers?

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Quest reward potions](Settings-Rates-Rewards#quest-reward-potions-ratequestrewardpotion) — it only works while this is On
- **Controls:** [Quest reward scrolls](Settings-Rates-Rewards#quest-reward-scrolls-ratequestrewardscroll) — it only works while this is On
- **Controls:** [Quest reward recipes](Settings-Rates-Rewards#quest-reward-recipes-ratequestrewardrecipe) — it only works while this is On
- **Controls:** [Quest reward materials](Settings-Rates-Rewards#quest-reward-materials-ratequestrewardmaterial) — it only works while this is On

### Quest reward items (general) (RateQuestReward)

`Rates.ini › RateQuestReward` · number · times

Default reward multiplier When UseRewardMultipliers=False - default multiplier is used for any reward When UseRewardMultipliers=True  - default multiplier is used for all items not affected by additional multipliers

- **Default:** `1`
- **Allowed:** 0 or more times

### Quest reward potions (RateQuestRewardPotion)

`Rates.ini › RateQuestRewardPotion` · number · times

Additional quest-reward multipliers based on item type

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Use separate quest reward multipliers per item type](Settings-Rates-Rewards#use-separate-quest-reward-multipliers-per-item-type-usequestrewardmultipliers) to be On — Per-type quest multipliers are only used when they are switched on; otherwise the general quest reward rate applies.

### Quest reward scrolls (RateQuestRewardScroll)

`Rates.ini › RateQuestRewardScroll` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Use separate quest reward multipliers per item type](Settings-Rates-Rewards#use-separate-quest-reward-multipliers-per-item-type-usequestrewardmultipliers) to be On — Per-type quest multipliers are only used when they are switched on; otherwise the general quest reward rate applies.

### Quest reward recipes (RateQuestRewardRecipe)

`Rates.ini › RateQuestRewardRecipe` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Use separate quest reward multipliers per item type](Settings-Rates-Rewards#use-separate-quest-reward-multipliers-per-item-type-usequestrewardmultipliers) to be On — Per-type quest multipliers are only used when they are switched on; otherwise the general quest reward rate applies.

### Quest reward materials (RateQuestRewardMaterial)

`Rates.ini › RateQuestRewardMaterial` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Use separate quest reward multipliers per item type](Settings-Rates-Rewards#use-separate-quest-reward-multipliers-per-item-type-usequestrewardmultipliers) to be On — Per-type quest multipliers are only used when they are switched on; otherwise the general quest reward rate applies.

## Monster drops

### Rewards from opening boxes and extractable items (RateExtractable)

`Rates.ini › RateExtractable` · number · times

Modify the rate of reward of all extractable items and skills.

- **Default:** `1`
- **Allowed:** 0 or more times

### Drop amount (how much per drop) (DeathDropAmountMultiplier)

`Rates.ini › DeathDropAmountMultiplier` · number · times

Multiplies the size of each drop. This keeps working however high you set it, so it is the safe way to raise rates.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Most different items a normal monster can drop at once](Settings-Rates-Rewards#most-different-items-a-normal-monster-can-drop-at-once-dropmaxoccurrencesnormal) — Past the limit on different items, bigger amounts are what raised rates still give you.
- **Works with:** [Drop chance (how often something drops)](Settings-Rates-Rewards#drop-chance-how-often-something-drops-deathdropchancemultiplier)
- **Works with:** [Drop amount for specific items (adena is item 57)](Settings-Rates-Rewards#drop-amount-for-specific-items-adena-is-item-57-dropamountmultiplierbyitemid)

### Spoil amount (SpoilDropAmountMultiplier)

`Rates.ini › SpoilDropAmountMultiplier` · number · times

Multiplies the amount of items rewarded from monsters when a Spoil skill is used.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Spoil chance](Settings-Rates-Rewards#spoil-chance-spoildropchancemultiplier)

### Herb drop amount (HerbDropAmountMultiplier)

`Rates.ini › HerbDropAmountMultiplier` · number · times

Multiplies the amount of items rewarded from monsters when they die.

- **Default:** `1`
- **Allowed:** 0 or more times

### Raid boss drop amount (RaidDropAmountMultiplier)

`Rates.ini › RaidDropAmountMultiplier` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times

### Drop chance (how often something drops) (DeathDropChanceMultiplier)

`Rates.ini › DeathDropChanceMultiplier` · number · times

Multiplies how often a drop happens. Each drop is rolled once, so anything above a 100% chance is wasted: past that point only the drop amount still helps. The Rates tab works this out for you.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Drop amount (how much per drop)](Settings-Rates-Rewards#drop-amount-how-much-per-drop-deathdropamountmultiplier) — Chance and amount multiply together, so 5 and 5 is 25 times the drops — but chance stops helping once a drop is certain, while amount never does.
- **Works with:** [Most different items a normal monster can drop at once](Settings-Rates-Rewards#most-different-items-a-normal-monster-can-drop-at-once-dropmaxoccurrencesnormal) — However often drops happen, one kill still gives at most this many different items.
- **Works with:** [Drop chance for specific items (adena is item 57)](Settings-Rates-Rewards#drop-chance-for-specific-items-adena-is-item-57-dropchancemultiplierbyitemid)

### Spoil chance (SpoilDropChanceMultiplier)

`Rates.ini › SpoilDropChanceMultiplier` · number · times

Multiplies the chance of items that can be rewarded from monsters when a Spoil skill is used.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Works with:** [Spoil amount](Settings-Rates-Rewards#spoil-amount-spoildropamountmultiplier) — Chance and amount multiply: 5 × 5 means about 25 times the spoils.

### Herb drop chance (HerbDropChanceMultiplier)

`Rates.ini › HerbDropChanceMultiplier` · number · times

Multiplies the chance of items that can be rewarded from monsters when they die.

- **Default:** `1`
- **Allowed:** 0 or more times

### Raid boss drop chance (RaidDropChanceMultiplier)

`Rates.ini › RaidDropChanceMultiplier` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times

### Drop amount for specific items (adena is item 57) (DropAmountMultiplierByItemId)

`Rates.ini › DropAmountMultiplierByItemId` · list

Format: itemId,multiplier;itemId,multiplier. Items listed here use this instead of the general drop amount, not as well as it.

- **Default:** `57,1`
- **Allowed:** `id,value` pairs separated by semicolons, e.g. `57,2;4037,1.5`
- **Works with:** [Drop amount (how much per drop)](Settings-Rates-Rewards#drop-amount-how-much-per-drop-deathdropamountmultiplier) — Items listed here (adena is 57) use their own amount instead of the general drop amount, so adena needs raising here too.

### Drop chance for specific items (adena is item 57) (DropChanceMultiplierByItemId)

`Rates.ini › DropChanceMultiplierByItemId` · list

Format: itemId,multiplier;itemId,multiplier. Items listed here use this instead of the general drop chance, not as well as it.

- **Allowed:** `id,value` pairs separated by semicolons, e.g. `57,2;4037,1.5`
- **Works with:** [Drop chance (how often something drops)](Settings-Rates-Rewards#drop-chance-how-often-something-drops-deathdropchancemultiplier) — Items listed here use their own chance instead of the general drop chance.

### Most different items a normal monster can drop at once (DropMaxOccurrencesNormal)

`Rates.ini › DropMaxOccurrencesNormal` · number · %

A kill stops here however high the rates go, so raising the drop amount gives bigger piles rather than more kinds of item.

- **Default:** `2`
- **Allowed:** 0 – 100 %
- **Works with:** [Drop amount (how much per drop)](Settings-Rates-Rewards#drop-amount-how-much-per-drop-deathdropamountmultiplier)
- **Works with:** [Drop chance (how often something drops)](Settings-Rates-Rewards#drop-chance-how-often-something-drops-deathdropchancemultiplier)

### Most different items a raid boss can drop at once (DropMaxOccurrencesRaidboss)

`Rates.ini › DropMaxOccurrencesRaidboss` · number

- **Default:** `7`
- **Allowed:** 0 – 2147483647

## Drops vs. level difference

### Adena starts dropping less when you are this many levels above the monster (DropAdenaMinLevelDifference)

`Rates.ini › DropAdenaMinLevelDifference` · number · levels

The min and max level difference used for level gap calculation this is only for how many levels higher the player is than the monster

- **Default:** `8`
- **Allowed:** 0 – 2147483647 levels

### Adena drop reaches its lowest at this many levels above the monster (DropAdenaMaxLevelDifference)

`Rates.ini › DropAdenaMaxLevelDifference` · number · levels

- **Default:** `15`
- **Allowed:** 0 – 2147483647 levels

### Lowest adena drop chance when far above the monster (DropAdenaMinLevelGapChance)

`Rates.ini › DropAdenaMinLevelGapChance` · number · %

This is the minimum level gap chance meaning for 10 that the monster will have 10% chance to allow dropping the item if level difference is bigger than DropAdenaMaxLevelDifference Note: This value is scalling from 100 to the specified value for DropAdenaMinLevelDifference to DropAdenaMaxLevelDifference limits

- **Default:** `10.0`
- **Allowed:** 0 – 100 %

### Items start dropping less when you are this many levels above the monster (DropItemMinLevelDifference)

`Rates.ini › DropItemMinLevelDifference` · number · levels

The min and max level difference used for level gap calculation this is only for how many levels higher the player is than the monster

- **Default:** `5`
- **Allowed:** 0 – 2147483647 levels

### Item drop reaches its lowest at this many levels above the monster (DropItemMaxLevelDifference)

`Rates.ini › DropItemMaxLevelDifference` · number · levels

- **Default:** `10`
- **Allowed:** 0 – 2147483647 levels

### Lowest item drop chance when far above the monster (DropItemMinLevelGapChance)

`Rates.ini › DropItemMinLevelGapChance` · number · %

This is the minimum level gap chance meaning for 10 that the monster will have 10% chance to allow dropping the item if level difference is bigger than DropAdenaMaxLevelDifference Note: This value is scalling from 100 to the specified value for DropAdenaMinLevelDifference to DropAdenaMaxLevelDifference limits

- **Default:** `10.0`
- **Allowed:** 0 – 100 %

### Event items stop dropping beyond this level difference (EventItemMaxLevelDifference)

`Rates.ini › EventItemMaxLevelDifference` · number · levels

Allow event items drop within custom level range between character and monster.

- **Default:** `9`
- **Allowed:** 0 – 2147483647 levels

## Vitality bonus

### Vitality level 1: XP multiplier (RateVitalityLevel1)

`Rates.ini › RateVitalityLevel1` · number · times

The following configures the XP multiplier of each vitality level. Basically, you have 5 levels, the first one being 0. Official rates are: Level 1: 150% Level 2: 200% Level 3: 250% Level 4: 300% Take care setting these values according to your server rates, as the can lead to huge differences! Example with a server rate 15x and a level 4 vitality = 3. => final server rate = 45 (15x3)!

- **Default:** `1.5`
- **Allowed:** 0 or more times
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — Vitality rates do nothing while the vitality system is off.

### Vitality level 2: XP multiplier (RateVitalityLevel2)

`Rates.ini › RateVitalityLevel2` · number · times

- **Default:** `2`
- **Allowed:** 0 or more times
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — Vitality rates do nothing while the vitality system is off.

### Vitality level 3: XP multiplier (RateVitalityLevel3)

`Rates.ini › RateVitalityLevel3` · number · times

- **Default:** `2.5`
- **Allowed:** 0 or more times
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — Vitality rates do nothing while the vitality system is off.

### Vitality level 4: XP multiplier (RateVitalityLevel4)

`Rates.ini › RateVitalityLevel4` · number · times

- **Default:** `3.0`
- **Allowed:** 0 or more times
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — Vitality rates do nothing while the vitality system is off.

### Vitality gained (RateVitalityGain)

`Rates.ini › RateVitalityGain` · number · times

These options are to be used if you want to increase the vitality gain/lost for each mob you kills Default values are 1.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — Vitality rates do nothing while the vitality system is off.

### Vitality used up per kill (RateVitalityLost)

`Rates.ini › RateVitalityLost` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — Vitality rates do nothing while the vitality system is off.

### Vitality recovery speed in towns (RateRecoveryPeaceZone)

`Rates.ini › RateRecoveryPeaceZone` · number · times

This defines how many times faster do the players regain their vitality when in peace zones

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable the vitality system](Settings-Characters#enable-the-vitality-system-enablevitality) to be On — Vitality rates do nothing while the vitality system is off.

### Vitality recovery speed while offline (RateRecoveryOnReconnect)

`Rates.ini › RateRecoveryOnReconnect` · number · times

This defines how many times faster do the players regain their vitality when offline Note that you need to turn on "RecoverVitalityOnReconnect" to have this option effective

- **Default:** `4.0`
- **Allowed:** 0 or more times
- **Needs:** [Recover vitality while offline](Settings-Characters#recover-vitality-while-offline-recovervitalityonreconnect) to be On — Offline recovery speed only matters while offline recovery is on.

## Pets

### Pet experience rate (PetXpRate)

`Rates.ini › PetXpRate` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times

### Pet food consumption (PetFoodRate)

`Rates.ini › PetFoodRate` · number · times

- **Default:** `1`
- **Allowed:** 0 – 2147483647 times

### Sin Eater experience rate (SinEaterXpRate)

`Rates.ini › SinEaterXpRate` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times

## Extra raid boss drops

### Give extra drops from raid bosses (BossDropEnable)

`Rates.ini › BossDropEnable` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Extra drops: lowest raid boss level](Settings-Rates-Rewards#extra-drops-lowest-raid-boss-level-bossdropminlevel) — it only works while this is On
- **Controls:** [Extra drops: highest raid boss level](Settings-Rates-Rewards#extra-drops-highest-raid-boss-level-bossdropmaxlevel) — it only works while this is On
- **Controls:** [Extra drops: item list](Settings-Rates-Rewards#extra-drops-item-list-bossdroplist) — it only works while this is On

### Extra drops: lowest raid boss level (BossDropMinLevel)

`Rates.ini › BossDropMinLevel` · number

- **Default:** `40`
- **Allowed:** 0 – 2147483647
- **Needs:** [Give extra drops from raid bosses](Settings-Rates-Rewards#give-extra-drops-from-raid-bosses-bossdropenable) to be On — Extra raid boss drops are off.

### Extra drops: highest raid boss level (BossDropMaxLevel)

`Rates.ini › BossDropMaxLevel` · number

- **Default:** `999`
- **Allowed:** 0 – 2147483647
- **Needs:** [Give extra drops from raid bosses](Settings-Rates-Rewards#give-extra-drops-from-raid-bosses-bossdropenable) to be On — Extra raid boss drops are off.

### Extra drops: item list (BossDropList)

`Rates.ini › BossDropList` · list

Format: itemId,minAmount,maxAmount,chance;itemId,...

- **Default:** `Gold Einhasad, min: 1x, max: 2x, 100% chance of drop`
- **Allowed:** `itemId,min,max,chance` groups separated by semicolons
- **Needs:** [Give extra drops from raid bosses](Settings-Rates-Rewards#give-extra-drops-from-raid-bosses-bossdropenable) to be On — Extra raid boss drops are off.
