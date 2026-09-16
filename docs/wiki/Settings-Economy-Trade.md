# Economy & Trade settings

Shops, private stores, offline trading, manor and money.

**46 settings** in the **Economy & Trade** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## NPC shops & multisell

### Fix shop prices that are below the sell price (CorrectPrices)

`General.ini › CorrectPrices` · on/off

Correct buylist and multisell prices when lower than sell price.

- **Default:** On
- **Allowed:** On or Off

### Most items per multisell exchange (MultisellAmountLimit)

`General.ini › MultisellAmountLimit` · number

Item limit on multisell transaction. Max client allowed 999999

- **Default:** `10000`
- **Allowed:** 1 – 999999

### Allow selling back to NPC shops (refund) (AllowRefund)

`General.ini › AllowRefund` · on/off

- **Default:** On
- **Allowed:** On or Off

### Allow trying on gear in shops (AllowWear)

`General.ini › AllowWear` · on/off

If True player can try on weapon and armor in shop.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Try-on lasts](Settings-Economy-Trade#try-on-lasts-weardelay) — it only works while this is On
- **Controls:** [Try-on price](Settings-Economy-Trade#try-on-price-wearprice) — it only works while this is On

### Try-on lasts (WearDelay)

`General.ini › WearDelay` · number · seconds

- **Default:** `5`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Allow trying on gear in shops](Settings-Economy-Trade#allow-trying-on-gear-in-shops-allowwear) to be On — Trying on gear is off.

### Try-on price (WearPrice)

`General.ini › WearPrice` · number · adena

Adena cost to try on an item.

- **Default:** `10`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Allow trying on gear in shops](Settings-Economy-Trade#allow-trying-on-gear-in-shops-allowwear) to be On — Trying on gear is off.

### NPC shops pay nothing for items (MerchantZeroSellPrice)

`Custom/MerchantZeroSellPrice.ini › MerchantZeroSellPrice` · on/off

All items sold to merchants reward no Adena.

- **Default:** Off
- **Allowed:** On or Off

## Private stores

### Private stores must be this far from other stores (ShopMinRangeFromPlayer)

`Custom/PrivateStoreRange.ini › ShopMinRangeFromPlayer` · number

Minimum distance from player / npc to open a new private store.

- **Default:** `50`
- **Allowed:** 0 – 2147483647

### Private stores must be this far from NPCs (ShopMinRangeFromNpc)

`Custom/PrivateStoreRange.ini › ShopMinRangeFromNpc` · number

- **Default:** `100`
- **Allowed:** 0 – 2147483647

## Offline trade & craft

### Private stores stay open after logout (OfflineTradeEnable)

`Custom/OfflineTrade.ini › OfflineTradeEnable` · on/off

Option to enable or disable offline trade feature.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Private workshops stay open after logout](Settings-Economy-Trade#private-workshops-stay-open-after-logout-offlinecraftenable) — it only works while this is On
- **Controls:** [Only in peace zones](Settings-Economy-Trade#only-in-peace-zones-offlinemodeinpeacezone) — it only works while this is On
- **Controls:** [Offline traders can't be hurt](Settings-Economy-Trade#offline-traders-can-t-be-hurt-offlinemodenodamage) — it only works while this is On
- **Controls:** [Color the names of offline traders](Settings-Economy-Trade#color-the-names-of-offline-traders-offlinesetnamecolor) — it only works while this is On
- **Controls:** [Offline traders earn fame](Settings-Economy-Trade#offline-traders-earn-fame-offlinefame) — it only works while this is On
- **Controls:** [Reopen offline stores after a restart](Settings-Economy-Trade#reopen-offline-stores-after-a-restart-restoreoffliners) — it only works while this is On
- **Controls:** [Close offline stores after](Settings-Economy-Trade#close-offline-stores-after-offlinemaxdays) — it only works while this is On
- **Controls:** [Log out traders who sold everything](Settings-Economy-Trade#log-out-traders-who-sold-everything-offlinedisconnectfinished) — it only works while this is On
- **Controls:** [Close offline stores when the account logs in](Settings-Economy-Trade#close-offline-stores-when-the-account-logs-in-offlinedisconnectsameaccount) — it only works while this is On
- **Controls:** [Save offline stores immediately](Settings-Economy-Trade#save-offline-stores-immediately-storeofflinetradeinrealtime) — it only works while this is On
- **Controls:** [Enable the .offline command](Settings-Economy-Trade#enable-the-offline-command-enableofflinecommand) — it only works while this is On
- **Controls:** [Visual effect on offline traders](Settings-Economy-Trade#visual-effect-on-offline-traders-offlineabnormaleffect) — it only works while this is On

### Private workshops stay open after logout (OfflineCraftEnable)

`Custom/OfflineTrade.ini › OfflineCraftEnable` · on/off

Option to enable or disable offline craft feature.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Only in peace zones (OfflineModeInPeaceZone)

`Custom/OfflineTrade.ini › OfflineModeInPeaceZone` · on/off

If set to True, off-line shops will be possible only peace zones.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Offline traders can't be hurt (OfflineModeNoDamage)

`Custom/OfflineTrade.ini › OfflineModeNoDamage` · on/off

If set to True, players in off-line shop mode wont take any damage, thus they cannot be killed.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Color the names of offline traders (OfflineSetNameColor)

`Custom/OfflineTrade.ini › OfflineSetNameColor` · on/off

If set to True, name color will be changed then entering offline mode.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On
- **Controls:** [Offline trader name color](Settings-Economy-Trade#offline-trader-name-color-offlinenamecolor) — it only works while this is On

### Offline trader name color (OfflineNameColor)

`Custom/OfflineTrade.ini › OfflineNameColor` · text

Hex color, e.g. 808080.

- **Default:** `808080`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Color the names of offline traders](Settings-Economy-Trade#color-the-names-of-offline-traders-offlinesetnamecolor) to be On — Name coloring for offline traders is off.

### Offline traders earn fame (OfflineFame)

`Custom/OfflineTrade.ini › OfflineFame` · on/off

Allow fame for characters in offline mode.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Reopen offline stores after a restart (RestoreOffliners)

`Custom/OfflineTrade.ini › RestoreOffliners` · on/off

Restore offline traders after restart/shutdown.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Close offline stores after (OfflineMaxDays)

`Custom/OfflineTrade.ini › OfflineMaxDays` · number · days

0 = never.

- **Default:** `10`
- **Allowed:** 0 – 2147483647 days
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Log out traders who sold everything (OfflineDisconnectFinished)

`Custom/OfflineTrade.ini › OfflineDisconnectFinished` · on/off

Disconnect shop after finished selling, buying.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Close offline stores when the account logs in (OfflineDisconnectSameAccount)

`Custom/OfflineTrade.ini › OfflineDisconnectSameAccount` · on/off

Disconnect shop when character from same account logins to the game.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Save offline stores immediately (StoreOfflineTradeInRealtime)

`Custom/OfflineTrade.ini › StoreOfflineTradeInRealtime` · on/off

Store offline trader transactions in realtime. Uses more datatabase resources, but helps if server shuts down unexpectedly.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Enable the .offline command (EnableOfflineCommand)

`Custom/OfflineTrade.ini › EnableOfflineCommand` · on/off

Enable .offline command for logging out.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

### Visual effect on offline traders (OfflineAbnormalEffect)

`Custom/OfflineTrade.ini › OfflineAbnormalEffect` · list

Abnormal effect for offline traders. Can use multiple enums separated by commas to choose random effect. Leave empty to disable. Example: SLEEP

- **Needs:** [Private stores stay open after logout](Settings-Economy-Trade#private-stores-stay-open-after-logout-offlinetradeenable) to be On

## Selling buffs

### Players can sell their buffs (.sellbuffs) (SellBuffEnable)

`Custom/SellBuffs.ini › SellBuffEnable` · on/off

Enable/Disable selling buffs

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [MP cost of sold buffs](Settings-Economy-Trade#mp-cost-of-sold-buffs-mpcostmultipler) — it only works while this is On
- **Controls:** [Payment item ID](Settings-Economy-Trade#payment-item-id-paymentid) — it only works while this is On
- **Controls:** [Lowest buff price](Settings-Economy-Trade#lowest-buff-price-minimumprice) — it only works while this is On
- **Controls:** [Highest buff price](Settings-Economy-Trade#highest-buff-price-maximumprice) — it only works while this is On
- **Controls:** [Most buffs in one shop](Settings-Economy-Trade#most-buffs-in-one-shop-maxbuffs) — it only works while this is On

### MP cost of sold buffs (MpCostMultipler)

`Custom/SellBuffs.ini › MpCostMultipler` · number · times

Multipler for mana cost of buffs

- **Default:** `1`
- **Allowed:** 0 – 2147483647 times
- **Needs:** [Players can sell their buffs (.sellbuffs)](Settings-Economy-Trade#players-can-sell-their-buffs-sellbuffs-sellbuffenable) to be On

### Payment item ID (PaymentID)

`Custom/SellBuffs.ini › PaymentID` · number

57 = adena.

- **Default:** `57`
- **Allowed:** 0 – 2147483647
- **Needs:** [Players can sell their buffs (.sellbuffs)](Settings-Economy-Trade#players-can-sell-their-buffs-sellbuffs-sellbuffenable) to be On

### Lowest buff price (MinimumPrice)

`Custom/SellBuffs.ini › MinimumPrice` · number

Minimum price of every buff

- **Default:** `100000`
- **Allowed:** 0 – 9223372036854775807
- **Needs:** [Players can sell their buffs (.sellbuffs)](Settings-Economy-Trade#players-can-sell-their-buffs-sellbuffs-sellbuffenable) to be On

### Highest buff price (MaximumPrice)

`Custom/SellBuffs.ini › MaximumPrice` · number

Maximum price of every buff

- **Default:** `100000000`
- **Allowed:** 0 – 9223372036854775807
- **Needs:** [Players can sell their buffs (.sellbuffs)](Settings-Economy-Trade#players-can-sell-their-buffs-sellbuffs-sellbuffenable) to be On

### Most buffs in one shop (MaxBuffs)

`Custom/SellBuffs.ini › MaxBuffs` · number

Maximum count of buffs in sell list

- **Default:** `15`
- **Allowed:** 0 – 2147483647
- **Needs:** [Players can sell their buffs (.sellbuffs)](Settings-Economy-Trade#players-can-sell-their-buffs-sellbuffs-sellbuffenable) to be On

## Manor

### Enable the manor (AllowManor)

`General.ini › AllowManor` · on/off

- **Default:** On
- **Allowed:** On or Off

### Manor refresh hour (AltManorRefreshTime)

`General.ini › AltManorRefreshTime` · number · hour

Manor refresh time in military hours.

- **Default:** `20`
- **Allowed:** 0 – 23 hour

### Manor refresh minute (AltManorRefreshMin)

`General.ini › AltManorRefreshMin` · number · minute

Manor refresh time (minutes).

- **Default:** `0`
- **Allowed:** 0 – 59 minute

### Manor approval hour (AltManorApproveTime)

`General.ini › AltManorApproveTime` · number · hour

Manor period approve time in military hours.

- **Default:** `4`
- **Allowed:** 0 – 23 hour

### Manor approval minute (AltManorApproveMin)

`General.ini › AltManorApproveMin` · number · minute

Manor period approve time (minutes).

- **Default:** `30`
- **Allowed:** 0 – 59 minute

### Manor maintenance length (AltManorMaintenanceMin)

`General.ini › AltManorMaintenanceMin` · number · minutes

Manor maintenance time (minutes).

- **Default:** `6`
- **Allowed:** 0 – 2147483647 minutes

### Save the manor after every action (AltManorSaveAllActions)

`General.ini › AltManorSaveAllActions` · on/off

Manor Save Type. True = Save data into the database after every action

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Save the manor every](Settings-Economy-Trade#save-the-manor-every-altmanorsaveperiodrate) — it only works while this is Off

### Save the manor every (AltManorSavePeriodRate)

`General.ini › AltManorSavePeriodRate` · number · hours

Manor Save Period (used only if AltManorSaveAllActions = False)

- **Default:** `2`
- **Allowed:** 0 – 2147483647 hours
- **Needs:** [Save the manor after every action](Settings-Economy-Trade#save-the-manor-after-every-action-altmanorsaveallactions) to be Off — Not used while the manor saves after every action.

### Manor crop harvest rate (RateDropManor)

`Rates.ini › RateDropManor` · number · times

- **Default:** `1`
- **Allowed:** 0 – 2147483647 times

## Banking

### Enable banking (.deposit / .withdraw) (BankingEnabled)

`Custom/Banking.ini › BankingEnabled` · on/off

Enable/Disable Banking System

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Gold bars per exchange](Settings-Economy-Trade#gold-bars-per-exchange-bankinggoldbarcount) — it only works while this is On
- **Controls:** [Adena per exchange](Settings-Economy-Trade#adena-per-exchange-bankingadenacount) — it only works while this is On

### Gold bars per exchange (BankingGoldbarCount)

`Custom/Banking.ini › BankingGoldbarCount` · number

Amount of Goldbars a player gets when they use the ".deposit" command. Also the same amount they will lose with ".withdraw".

- **Default:** `1`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable banking (.deposit / .withdraw)](Settings-Economy-Trade#enable-banking-deposit-withdraw-bankingenabled) to be On

### Adena per exchange (BankingAdenaCount)

`Custom/Banking.ini › BankingAdenaCount` · number · adena

Amount of Adena a player gets when they use the ".withdraw" command. Also the same amount they will lose with ".deposit".

- **Default:** `500000000`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Enable banking (.deposit / .withdraw)](Settings-Economy-Trade#enable-banking-deposit-withdraw-bankingenabled) to be On

## Scheme buffer

### Buff schemes per character (BufferMaxSchemesPerChar)

`Custom/SchemeBuffer.ini › BufferMaxSchemesPerChar` · number

Maximum number of available schemes per player.

- **Default:** `4`
- **Allowed:** 0 – 2147483647

### Buffer price item ID (BufferItemId)

`Custom/SchemeBuffer.ini › BufferItemId` · number

57 = adena.

- **Default:** `57`
- **Allowed:** 0 – 2147483647

### Price per buff (BufferStaticCostPerBuff)

`Custom/SchemeBuffer.ini › BufferStaticCostPerBuff` · number

-1 = use each buff's own price.

- **Default:** `-1`
- **Allowed:** -2147483648 – 2147483647

## Other

### No bonus adena for turning in 10+ items to village quests (AltVillagesRepQuestReward)

`General.ini › AltVillagesRepQuestReward` · on/off

Disable additional adena rewards for starter villages repeatable quests based on turning in items True = additional reward for 10+ items not given on quest turn in, False = get additional reward for 10+ items on quest turn in ATTENTION: enabling this option greatly decrease adena income capabilities on low levels.

- **Default:** Off
- **Allowed:** On or Off

### Most adena a character can hold (MaxAdena)

`Player.ini › MaxAdena` · number · adena

Characters can never hold more than this, however much drops. The Characters tab also uses it as the adena limit.

- **Default:** `2000000000`
- **Allowed:** -1 – 2147483647 adena
