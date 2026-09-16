# Items & Enchanting settings

Enchanting, augmenting, crafting and items on the ground.

**50 settings** in the **Items & Enchanting** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Enchanting

### Items that can never be enchanted (EnchantBlackList)

`Player.ini › EnchantBlackList` · list

Comma-separated item IDs.

- **Default:** `7816,7817,7818,7819,7820,7821,7822,7823,7824,7825,7826,7827,7828,7829,7830,7831,13293,13294,13296`
- **Allowed:** numbers separated by commas, e.g. `57,4037`

### Stop enchanting at the maximum instead of breaking rules (DisableOverEnchanting)

`Player.ini › DisableOverEnchanting` · on/off

If enabled, enchanting over the maximum enchant limit will not be possible; instead, a warning message will be sent to the player. You can set custom maxEnchant values at EnchantItemData.xml

- **Default:** On
- **Allowed:** On or Off
- **Works with:** [Punish players found with over-enchanted items](Settings-Items-Enchanting#punish-players-found-with-over-enchanted-items-overenchantprotection) — With over-enchanting blocked, the protection only catches items that were already over the limit.

### Punish players found with over-enchanted items (OverEnchantProtection)

`Player.ini › OverEnchantProtection` · on/off

Over-enchant protection. If player is found with an over-enchanted item, he will be punished and the item will disappear.

- **Default:** On
- **Allowed:** On or Off
- **Works with:** [Stop enchanting at the maximum instead of breaking rules](Settings-Items-Enchanting#stop-enchanting-at-the-maximum-instead-of-breaking-rules-disableoverenchanting)
- **Controls:** [Punishment for over-enchanted items](Settings-Items-Enchanting#punishment-for-over-enchanted-items-overenchantpunishment) — it only works while this is On

### Punishment for over-enchanted items (OverEnchantPunishment)

`Player.ini › OverEnchantPunishment` · choice

The punishment for over-enchanting. NONE - Disabled (items will still be deleted) BROADCAST - broadcast warning to GMs only KICK - kick player KICKBAN - kick and ban player JAIL - jail player

- **Default:** `JAIL` (jail player)
- **Allowed:** one of `NONE` (Disabled (items will still be deleted)), `BROADCAST` (broadcast warning to GMs only), `KICK` (kick player), `KICKBAN` (kick and ban player), `JAIL` (jail player)
- **Needs:** [Punish players found with over-enchanted items](Settings-Items-Enchanting#punish-players-found-with-over-enchanted-items-overenchantprotection) to be On — Punishment only applies while over-enchant protection is on.

## Augmentation

### Skill chance: no-grade life stone (AugmentationNGSkillChance)

`Player.ini › AugmentationNGSkillChance` · number · %

Augmenting These control the chance to get a skill in the augmentation process.

- **Default:** `15`
- **Allowed:** 0 – 100 %

### Skill chance: mid-grade life stone (AugmentationMidSkillChance)

`Player.ini › AugmentationMidSkillChance` · number · %

- **Default:** `30`
- **Allowed:** 0 – 100 %

### Skill chance: high-grade life stone (AugmentationHighSkillChance)

`Player.ini › AugmentationHighSkillChance` · number · %

- **Default:** `45`
- **Allowed:** 0 – 100 %

### Skill chance: top-grade life stone (AugmentationTopSkillChance)

`Player.ini › AugmentationTopSkillChance` · number · %

- **Default:** `60`
- **Allowed:** 0 – 100 %

### Chance of a base stat bonus (AugmentationBaseStatChance)

`Player.ini › AugmentationBaseStatChance` · number · %

This controls the chance to get a base stat modifier in the augmentation process. This has no dependency on the grade of Life Stone.

- **Default:** `1`
- **Allowed:** 0 – 100 %
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be Off — Glow and skill chances are ignored while retail-like augmentation is on.

### Glow chance: no-grade life stone (AugmentationNGGlowChance)

`Player.ini › AugmentationNGGlowChance` · number · %

These control the chance to get a glow effect in the augmentation process. No/Mid Grade Life Stone can not have glow effect if you do not get a skill or base stat modifier.

- **Default:** `0`
- **Allowed:** 0 – 100 %
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be Off — Glow and skill chances are ignored while retail-like augmentation is on.

### Glow chance: mid-grade life stone (AugmentationMidGlowChance)

`Player.ini › AugmentationMidGlowChance` · number · %

- **Default:** `40`
- **Allowed:** 0 – 100 %
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be Off — Glow and skill chances are ignored while retail-like augmentation is on.

### Glow chance: high-grade life stone (AugmentationHighGlowChance)

`Player.ini › AugmentationHighGlowChance` · number · %

- **Default:** `70`
- **Allowed:** 0 – 100 %
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be Off — Glow and skill chances are ignored while retail-like augmentation is on.

### Glow chance: top-grade life stone (AugmentationTopGlowChance)

`Player.ini › AugmentationTopGlowChance` · number · %

- **Default:** `100`
- **Allowed:** 0 – 100 %
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be Off — Glow and skill chances are ignored while retail-like augmentation is on.

### Retail-like augmentation colors (RetailLikeAugmentation)

`Player.ini › RetailLikeAugmentation` · on/off

This will enable retail like weapon augmentation, but then you cant change weapon glow, base stat chance, because it wouldnt be retail like again.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Chance of a base stat bonus](Settings-Items-Enchanting#chance-of-a-base-stat-bonus-augmentationbasestatchance) — it only works while this is Off
- **Controls:** [Glow chance: no-grade life stone](Settings-Items-Enchanting#glow-chance-no-grade-life-stone-augmentationngglowchance) — it only works while this is Off
- **Controls:** [Glow chance: mid-grade life stone](Settings-Items-Enchanting#glow-chance-mid-grade-life-stone-augmentationmidglowchance) — it only works while this is Off
- **Controls:** [Glow chance: high-grade life stone](Settings-Items-Enchanting#glow-chance-high-grade-life-stone-augmentationhighglowchance) — it only works while this is Off
- **Controls:** [Glow chance: top-grade life stone](Settings-Items-Enchanting#glow-chance-top-grade-life-stone-augmentationtopglowchance) — it only works while this is Off
- **Controls:** [Color odds: no-grade life stone](Settings-Items-Enchanting#color-odds-no-grade-life-stone-retaillikeaugmentationnogradechance) — it only works while this is On
- **Controls:** [Color odds: mid-grade life stone](Settings-Items-Enchanting#color-odds-mid-grade-life-stone-retaillikeaugmentationmidgradechance) — it only works while this is On
- **Controls:** [Color odds: high-grade life stone](Settings-Items-Enchanting#color-odds-high-grade-life-stone-retaillikeaugmentationhighgradechance) — it only works while this is On
- **Controls:** [Color odds: top-grade life stone](Settings-Items-Enchanting#color-odds-top-grade-life-stone-retaillikeaugmentationtopgradechance) — it only works while this is On

### Color odds: no-grade life stone (RetailLikeAugmentationNoGradeChance)

`Player.ini › RetailLikeAugmentationNoGradeChance` · list

Yellow, blue, purple, red — must add up to 100.

- **Default:** `55,35,7,3`
- **Allowed:** four percentages that add up to 100
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be On — Color odds are only used with retail-like augmentation.

### Color odds: mid-grade life stone (RetailLikeAugmentationMidGradeChance)

`Player.ini › RetailLikeAugmentationMidGradeChance` · list

Yellow, blue, purple, red — must add up to 100.

- **Default:** `55,35,7,3`
- **Allowed:** four percentages that add up to 100
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be On — Color odds are only used with retail-like augmentation.

### Color odds: high-grade life stone (RetailLikeAugmentationHighGradeChance)

`Player.ini › RetailLikeAugmentationHighGradeChance` · list

Yellow, blue, purple, red — must add up to 100.

- **Default:** `55,35,7,3`
- **Allowed:** four percentages that add up to 100
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be On — Color odds are only used with retail-like augmentation.

### Color odds: top-grade life stone (RetailLikeAugmentationTopGradeChance)

`Player.ini › RetailLikeAugmentationTopGradeChance` · list

Yellow, blue, purple, red — must add up to 100.

- **Default:** `55,35,7,3`
- **Allowed:** four percentages that add up to 100
- **Needs:** [Retail-like augmentation colors](Settings-Items-Enchanting#retail-like-augmentation-colors-retaillikeaugmentation) to be On — Color odds are only used with retail-like augmentation.

### Items that can never be augmented (AugmentationBlackList)

`Player.ini › AugmentationBlackList` · list

Comma-separated item IDs.

- **Default:** `6656,6657,6658,6659,6660,6661,6662,8191`
- **Allowed:** numbers separated by commas, e.g. `57,4037`

### Allow augmenting PvP items (AltAllowAugmentPvPItems)

`Player.ini › AltAllowAugmentPvPItems` · on/off

Allows alternative augmentation of PvP items.

- **Default:** Off
- **Allowed:** On or Off

### Allow trading augmented items (AltAllowAugmentTrade)

`Player.ini › AltAllowAugmentTrade` · on/off

Enable Trade/Drop/Sell for augmented items.

- **Default:** Off
- **Allowed:** On or Off

### Allow destroying augmented items (AltAllowAugmentDestroy)

`Player.ini › AltAllowAugmentDestroy` · on/off

Enable Destroy/Crystalize for augmented items.

- **Default:** On
- **Allowed:** On or Off

## Soul crystals

### Soul crystal leveling chance (SoulCrystalChanceMultiplier)

`Player.ini › SoulCrystalChanceMultiplier` · number · times

Soul Crystal Multiplier for retail soul crystal leveling changes.

- **Default:** `1`
- **Allowed:** 0 or more times

## Crafting

### Allow crafting (CraftingEnabled)

`Player.ini › CraftingEnabled` · on/off

Option to enable or disable crafting.

- **Default:** On
- **Allowed:** On or Off

### Dwarven recipe book size (DwarfRecipeLimit)

`Player.ini › DwarfRecipeLimit` · number

Limits for recipes

- **Default:** `50`
- **Allowed:** 0 – 2147483647

### Common recipe book size (CommonRecipeLimit)

`Player.ini › CommonRecipeLimit` · number

- **Default:** `50`
- **Allowed:** 0 – 2147483647

### Crafting takes time and gives experience (AltGameCreation)

`Player.ini › AltGameCreation` · on/off

Alternative crafting rules. If enabled, the following will be true: Crafting takes time. Players get EXP/SP for crafting.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Crafting time](Settings-Items-Enchanting#crafting-time-altgamecreationspeed) — it only works while this is On
- **Controls:** [Crafting experience](Settings-Items-Enchanting#crafting-experience-altgamecreationxprate) — it only works while this is On
- **Controls:** [Crafting skill points](Settings-Items-Enchanting#crafting-skill-points-altgamecreationsprate) — it only works while this is On

### Crafting time (AltGameCreationSpeed)

`Player.ini › AltGameCreationSpeed` · number · times

Crafting Time multiplier. The higher the number, the more time the crafting process takes. XP/SP reward increases with time.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Crafting takes time and gives experience](Settings-Items-Enchanting#crafting-takes-time-and-gives-experience-altgamecreation) to be On — Crafting time only applies when crafting takes time.

### Crafting experience (AltGameCreationXpRate)

`Player.ini › AltGameCreationXpRate` · number · times

Additional crafting XP/SP rate multiplier.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Crafting takes time and gives experience](Settings-Items-Enchanting#crafting-takes-time-and-gives-experience-altgamecreation) to be On — Crafting experience only applies when crafting takes time.

### Crafting skill points (AltGameCreationSpRate)

`Player.ini › AltGameCreationSpRate` · number · times

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Crafting takes time and gives experience](Settings-Items-Enchanting#crafting-takes-time-and-gives-experience-altgamecreation) to be On — Crafting skill points only apply when crafting takes time.

### Blacksmiths use up recipes (AltBlacksmithUseRecipes)

`Player.ini › AltBlacksmithUseRecipes` · on/off

If set to False, blacksmiths don't take recipes from players inventory when crafting.

- **Default:** On
- **Allowed:** On or Off

### Remember private workshop lists after logout (StoreRecipeShopList)

`Player.ini › StoreRecipeShopList` · on/off

Store/Restore Dwarven Manufacture list Keep manufacture shoplist after relog

- **Default:** Off
- **Allowed:** On or Off

## Dropped items

### Players can drop items on the ground (AllowDiscardItem)

`General.ini › AllowDiscardItem` · on/off

Items on ground management. Allow players to drop items on the ground.

- **Default:** On
- **Allowed:** On or Off

### Remove dropped items after (AutoDestroyDroppedItemAfter)

`General.ini › AutoDestroyDroppedItemAfter` · number · seconds · *added by L2Everdream*

0 = never.

- **Default:** `600`
- **Allowed:** 0 – 2147483647 seconds
- **Controls:** [Also remove items dropped by players](Settings-Items-Enchanting#also-remove-items-dropped-by-players-destroyplayerdroppeditem) — it only works while this is `>0`

### Remove dropped herbs after (AutoDestroyHerbTime)

`General.ini › AutoDestroyHerbTime` · number · seconds

Time in seconds after which dropped herb will be auto-destroyed

- **Default:** `60`
- **Allowed:** 0 – 2147483647 seconds

### Items never removed from the ground (ListOfProtectedItems)

`General.ini › ListOfProtectedItems` · list

Comma-separated item IDs.

- **Default:** `0`
- **Allowed:** numbers separated by commas, e.g. `57,4037`

### Also remove items dropped by players (DestroyPlayerDroppedItem)

`General.ini › DestroyPlayerDroppedItem` · on/off

Also delete from world misc. items dropped by players (all except equip-able items). Works only if AutoDestroyDroppedItemAfter is greater than 0.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Remove dropped items after](Settings-Items-Enchanting#remove-dropped-items-after-autodestroydroppeditemafter) to be `>0` — Dropped items are never removed while the removal time is 0.
- **Controls:** [Also remove equipment dropped by players](Settings-Items-Enchanting#also-remove-equipment-dropped-by-players-destroyequipableitem) — it only works while this is On

### Also remove equipment dropped by players (DestroyEquipableItem)

`General.ini › DestroyEquipableItem` · on/off

Destroy dropped equippable items (armor, weapon, jewelry). Works only if DestroyPlayerDroppedItem = True

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Also remove items dropped by players](Settings-Items-Enchanting#also-remove-items-dropped-by-players-destroyplayerdroppeditem) to be On — Player-dropped items are not being removed.

### Players can destroy any item (DestroyAllItems)

`General.ini › DestroyAllItems` · on/off

Make all items destroyable. If enabled players can destroy all items!!!

- **Default:** Off
- **Allowed:** On or Off

### Keep items on the ground through restarts (SaveDroppedItem)

`General.ini › SaveDroppedItem` · on/off

Save dropped items into the database for restoring after restart.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Clear saved ground items after loading them](Settings-Items-Enchanting#clear-saved-ground-items-after-loading-them-emptydroppeditemtableafterload) — it only works while this is On
- **Controls:** [Save ground items every](Settings-Items-Enchanting#save-ground-items-every-savedroppediteminterval) — it only works while this is On
- **Controls:** [Delete all saved ground items on next start](Settings-Items-Enchanting#delete-all-saved-ground-items-on-next-start-cleardroppeditemtable) — it only works while this is Off

### Clear saved ground items after loading them (EmptyDroppedItemTableAfterLoad)

`General.ini › EmptyDroppedItemTableAfterLoad` · on/off

Enable/Disable the emptying of the stored dropped items table after items are loaded into memory (safety setting). If the server crashed before saving items, on next start old items will be restored and players may already have picked up some of them so this will prevent duplicates.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Keep items on the ground through restarts](Settings-Items-Enchanting#keep-items-on-the-ground-through-restarts-savedroppeditem) to be On — Ground items are not being kept through restarts.

### Save ground items every (SaveDroppedItemInterval)

`General.ini › SaveDroppedItemInterval` · number · minutes

Time interval in minutes to save in DB items on ground. Disabled = 0. If SaveDroppedItemInterval is disabled, items will be saved into the database only at server shutdown.

- **Default:** `60`
- **Allowed:** 0 – 2147483647 minutes
- **Needs:** [Keep items on the ground through restarts](Settings-Items-Enchanting#keep-items-on-the-ground-through-restarts-savedroppeditem) to be On — Ground items are not being kept through restarts.

### Delete all saved ground items on next start (ClearDroppedItemTable)

`General.ini › ClearDroppedItemTable` · on/off

Delete all saved items from the database on next restart? Works only if SaveDroppedItem = False.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Keep items on the ground through restarts](Settings-Items-Enchanting#keep-items-on-the-ground-through-restarts-savedroppeditem) to be Off — Only works while ground items are not kept through restarts.

### Allow dropping several non-stackable items at once (MultipleItemDrop)

`General.ini › MultipleItemDrop` · on/off

Allow creating multiple non-stackable items at one time?

- **Default:** On
- **Allowed:** On or Off

## Transmogrification

### Enable transmogrification (TransmogEnabled)

`Custom/Transmog.ini › TransmogEnabled` · on/off

Enable/Disable Transmogrification System.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Share appearances across the account](Settings-Items-Enchanting#share-appearances-across-the-account-transmogshareaccount) — it only works while this is On
- **Controls:** [Price to apply an appearance](Settings-Items-Enchanting#price-to-apply-an-appearance-transmogapplycost) — it only works while this is On
- **Controls:** [Price to remove an appearance](Settings-Items-Enchanting#price-to-remove-an-appearance-transmogremovecost) — it only works while this is On
- **Controls:** [Items that can't be used as appearances](Settings-Items-Enchanting#items-that-can-t-be-used-as-appearances-transmogbanneditemids) — it only works while this is On

### Share appearances across the account (TransmogShareAccount)

`Custom/Transmog.ini › TransmogShareAccount` · on/off

Make transmog sharable in the same account.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable transmogrification](Settings-Items-Enchanting#enable-transmogrification-transmogenabled) to be On

### Price to apply an appearance (TransmogApplyCost)

`Custom/Transmog.ini › TransmogApplyCost` · number · adena

Transmog apply cost in Adena.

- **Default:** `0`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Enable transmogrification](Settings-Items-Enchanting#enable-transmogrification-transmogenabled) to be On

### Price to remove an appearance (TransmogRemoveCost)

`Custom/Transmog.ini › TransmogRemoveCost` · number · adena

Transmog remove cost in Adena.

- **Default:** `0`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Enable transmogrification](Settings-Items-Enchanting#enable-transmogrification-transmogenabled) to be On

### Items that can't be used as appearances (TransmogBannedItemIds)

`Custom/Transmog.ini › TransmogBannedItemIds` · list

List of items that cannot be added to transmog, separated by commas.

- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Enable transmogrification](Settings-Items-Enchanting#enable-transmogrification-transmogenabled) to be On

## Other item rules

### Penalty for wearing gear above your grade (ExpertisePenalty)

`Player.ini › ExpertisePenalty` · on/off

Expertise penalty If disabled, player will not receive penalty for equip higher grade items

- **Default:** On
- **Allowed:** On or Off
