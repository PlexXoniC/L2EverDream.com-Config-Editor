# Protection & Anti-cheat settings

Flood protection, captcha, dual-boxing and login security.

**112 settings** in the **Protection & Anti-cheat** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Flood protection

### Using items: minimum time between uses (FloodProtectorUseItemInterval)

`FloodProtector.ini › FloodProtectorUseItemInterval` · number · minutes · *Advanced*

UseItem - item usage flooding Item usage interval Disabled to match retail, if you want to enable this protection change the value to 4 for example.

- **Default:** `0`
- **Allowed:** 0 or more minutes

### Using items: log flooding attempts (FloodProtectorUseItemLogFlooding)

`FloodProtector.ini › FloodProtectorUseItemLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Using items: attempts before punishment (FloodProtectorUseItemPunishmentLimit)

`FloodProtector.ini › FloodProtectorUseItemPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Using items: punishment (FloodProtectorUseItemPunishmentType)

`FloodProtector.ini › FloodProtectorUseItemPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Using items: punishment length (FloodProtectorUseItemPunishmentTime)

`FloodProtector.ini › FloodProtectorUseItemPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Rolling dice: minimum time between uses (FloodProtectorRollDiceInterval)

`FloodProtector.ini › FloodProtectorRollDiceInterval` · number · *Advanced*

RollDice - rolling dice flooding

- **Allowed:** 0 or more

### Rolling dice: log flooding attempts (FloodProtectorRollDiceLogFlooding)

`FloodProtector.ini › FloodProtectorRollDiceLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Rolling dice: attempts before punishment (FloodProtectorRollDicePunishmentLimit)

`FloodProtector.ini › FloodProtectorRollDicePunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Rolling dice: punishment (FloodProtectorRollDicePunishmentType)

`FloodProtector.ini › FloodProtectorRollDicePunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Rolling dice: punishment length (FloodProtectorRollDicePunishmentTime)

`FloodProtector.ini › FloodProtectorRollDicePunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Summoning pets: minimum time between uses (FloodProtectorItemPetSummonInterval)

`FloodProtector.ini › FloodProtectorItemPetSummonInterval` · number · *Advanced*

ItemPetSummon - item summoning and pet mounting flooding

- **Allowed:** 0 or more

### Summoning pets: log flooding attempts (FloodProtectorItemPetSummonLogFlooding)

`FloodProtector.ini › FloodProtectorItemPetSummonLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Summoning pets: attempts before punishment (FloodProtectorItemPetSummonPunishmentLimit)

`FloodProtector.ini › FloodProtectorItemPetSummonPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Summoning pets: punishment (FloodProtectorItemPetSummonPunishmentType)

`FloodProtector.ini › FloodProtectorItemPetSummonPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Summoning pets: punishment length (FloodProtectorItemPetSummonPunishmentTime)

`FloodProtector.ini › FloodProtectorItemPetSummonPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Hero voice chat: minimum time between uses (FloodProtectorHeroVoiceInterval)

`FloodProtector.ini › FloodProtectorHeroVoiceInterval` · number · *Advanced*

HeroVoice - hero voice flooding

- **Allowed:** 0 or more

### Hero voice chat: log flooding attempts (FloodProtectorHeroVoiceLogFlooding)

`FloodProtector.ini › FloodProtectorHeroVoiceLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Hero voice chat: attempts before punishment (FloodProtectorHeroVoicePunishmentLimit)

`FloodProtector.ini › FloodProtectorHeroVoicePunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Hero voice chat: punishment (FloodProtectorHeroVoicePunishmentType)

`FloodProtector.ini › FloodProtectorHeroVoicePunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Hero voice chat: punishment length (FloodProtectorHeroVoicePunishmentTime)

`FloodProtector.ini › FloodProtectorHeroVoicePunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Global chat: minimum time between uses (FloodProtectorGlobalChatInterval)

`FloodProtector.ini › FloodProtectorGlobalChatInterval` · number · *Advanced*

GlobalChat - global chat flooding

- **Allowed:** 0 or more

### Global chat: log flooding attempts (FloodProtectorGlobalChatLogFlooding)

`FloodProtector.ini › FloodProtectorGlobalChatLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Global chat: attempts before punishment (FloodProtectorGlobalChatPunishmentLimit)

`FloodProtector.ini › FloodProtectorGlobalChatPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Global chat: punishment (FloodProtectorGlobalChatPunishmentType)

`FloodProtector.ini › FloodProtectorGlobalChatPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Global chat: punishment length (FloodProtectorGlobalChatPunishmentTime)

`FloodProtector.ini › FloodProtectorGlobalChatPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Changing subclass: minimum time between uses (FloodProtectorSubclassInterval)

`FloodProtector.ini › FloodProtectorSubclassInterval` · number · *Advanced*

Subclass - subclass flooding

- **Allowed:** 0 or more

### Changing subclass: log flooding attempts (FloodProtectorSubclassLogFlooding)

`FloodProtector.ini › FloodProtectorSubclassLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Changing subclass: attempts before punishment (FloodProtectorSubclassPunishmentLimit)

`FloodProtector.ini › FloodProtectorSubclassPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Changing subclass: punishment (FloodProtectorSubclassPunishmentType)

`FloodProtector.ini › FloodProtectorSubclassPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Changing subclass: punishment length (FloodProtectorSubclassPunishmentTime)

`FloodProtector.ini › FloodProtectorSubclassPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Dropping items: minimum time between uses (FloodProtectorDropItemInterval)

`FloodProtector.ini › FloodProtectorDropItemInterval` · number · *Advanced*

DropItem - drop item flooding

- **Allowed:** 0 or more

### Dropping items: log flooding attempts (FloodProtectorDropItemLogFlooding)

`FloodProtector.ini › FloodProtectorDropItemLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Dropping items: attempts before punishment (FloodProtectorDropItemPunishmentLimit)

`FloodProtector.ini › FloodProtectorDropItemPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Dropping items: punishment (FloodProtectorDropItemPunishmentType)

`FloodProtector.ini › FloodProtectorDropItemPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Dropping items: punishment length (FloodProtectorDropItemPunishmentTime)

`FloodProtector.ini › FloodProtectorDropItemPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Enchanting: minimum time between uses (FloodProtectorEnchantItemInterval)

`FloodProtector.ini › FloodProtectorEnchantItemInterval` · number · *Advanced*

EnchantItem - flooding

- **Allowed:** 0 or more

### Enchanting: log flooding attempts (FloodProtectorEnchantItemLogFlooding)

`FloodProtector.ini › FloodProtectorEnchantItemLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Enchanting: attempts before punishment (FloodProtectorEnchantItemPunishmentLimit)

`FloodProtector.ini › FloodProtectorEnchantItemPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Enchanting: punishment (FloodProtectorEnchantItemPunishmentType)

`FloodProtector.ini › FloodProtectorEnchantItemPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Enchanting: punishment length (FloodProtectorEnchantItemPunishmentTime)

`FloodProtector.ini › FloodProtectorEnchantItemPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### NPC dialog clicks: minimum time between uses (FloodProtectorServerBypassInterval)

`FloodProtector.ini › FloodProtectorServerBypassInterval` · number · *Advanced*

ServerBypass - server bypass flooding

- **Allowed:** 0 or more

### NPC dialog clicks: log flooding attempts (FloodProtectorServerBypassLogFlooding)

`FloodProtector.ini › FloodProtectorServerBypassLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### NPC dialog clicks: attempts before punishment (FloodProtectorServerBypassPunishmentLimit)

`FloodProtector.ini › FloodProtectorServerBypassPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### NPC dialog clicks: punishment (FloodProtectorServerBypassPunishmentType)

`FloodProtector.ini › FloodProtectorServerBypassPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### NPC dialog clicks: punishment length (FloodProtectorServerBypassPunishmentTime)

`FloodProtector.ini › FloodProtectorServerBypassPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Multisell exchanges: minimum time between uses (FloodProtectorMultiSellInterval)

`FloodProtector.ini › FloodProtectorMultiSellInterval` · number · *Advanced*

ServerBypass - multisell list request flooding

- **Allowed:** 0 or more

### Multisell exchanges: log flooding attempts (FloodProtectorMultiSellLogFlooding)

`FloodProtector.ini › FloodProtectorMultiSellLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Multisell exchanges: attempts before punishment (FloodProtectorMultiSellPunishmentLimit)

`FloodProtector.ini › FloodProtectorMultiSellPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Multisell exchanges: punishment (FloodProtectorMultiSellPunishmentType)

`FloodProtector.ini › FloodProtectorMultiSellPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Multisell exchanges: punishment length (FloodProtectorMultiSellPunishmentTime)

`FloodProtector.ini › FloodProtectorMultiSellPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Trades: minimum time between uses (FloodProtectorTransactionInterval)

`FloodProtector.ini › FloodProtectorTransactionInterval` · number · *Advanced*

All kind of other transactions - to/from pet, private store, warehouse, destroy

- **Allowed:** 0 or more

### Trades: log flooding attempts (FloodProtectorTransactionLogFlooding)

`FloodProtector.ini › FloodProtectorTransactionLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Trades: attempts before punishment (FloodProtectorTransactionPunishmentLimit)

`FloodProtector.ini › FloodProtectorTransactionPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Trades: punishment (FloodProtectorTransactionPunishmentType)

`FloodProtector.ini › FloodProtectorTransactionPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Trades: punishment length (FloodProtectorTransactionPunishmentTime)

`FloodProtector.ini › FloodProtectorTransactionPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Private workshop: minimum time between uses (FloodProtectorManufactureInterval)

`FloodProtector.ini › FloodProtectorManufactureInterval` · number · *Advanced*

Manufacture

- **Allowed:** 0 or more

### Private workshop: log flooding attempts (FloodProtectorManufactureLogFlooding)

`FloodProtector.ini › FloodProtectorManufactureLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Private workshop: attempts before punishment (FloodProtectorManufacturePunishmentLimit)

`FloodProtector.ini › FloodProtectorManufacturePunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Private workshop: punishment (FloodProtectorManufacturePunishmentType)

`FloodProtector.ini › FloodProtectorManufacturePunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Private workshop: punishment length (FloodProtectorManufacturePunishmentTime)

`FloodProtector.ini › FloodProtectorManufacturePunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Sending mail: minimum time between uses (FloodProtectorSendMailInterval)

`FloodProtector.ini › FloodProtectorSendMailInterval` · number · *Advanced*

SendMail - sending mail interval, 10s on retail

- **Allowed:** 0 or more

### Sending mail: log flooding attempts (FloodProtectorSendMailLogFlooding)

`FloodProtector.ini › FloodProtectorSendMailLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Sending mail: attempts before punishment (FloodProtectorSendMailPunishmentLimit)

`FloodProtector.ini › FloodProtectorSendMailPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Sending mail: punishment (FloodProtectorSendMailPunishmentType)

`FloodProtector.ini › FloodProtectorSendMailPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Sending mail: punishment length (FloodProtectorSendMailPunishmentTime)

`FloodProtector.ini › FloodProtectorSendMailPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Character selection: minimum time between uses (FloodProtectorCharacterSelectInterval)

`FloodProtector.ini › FloodProtectorCharacterSelectInterval` · number · *Advanced*

CharacterSelect - attempts to load character

- **Allowed:** 0 or more

### Character selection: log flooding attempts (FloodProtectorCharacterSelectLogFlooding)

`FloodProtector.ini › FloodProtectorCharacterSelectLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Character selection: attempts before punishment (FloodProtectorCharacterSelectPunishmentLimit)

`FloodProtector.ini › FloodProtectorCharacterSelectPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Character selection: punishment (FloodProtectorCharacterSelectPunishmentType)

`FloodProtector.ini › FloodProtectorCharacterSelectPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Character selection: punishment length (FloodProtectorCharacterSelectPunishmentTime)

`FloodProtector.ini › FloodProtectorCharacterSelectPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Item auction bids: minimum time between uses (FloodProtectorItemAuctionInterval)

`FloodProtector.ini › FloodProtectorItemAuctionInterval` · number · *Advanced*

Item Auction - Request for refresh

- **Allowed:** 0 or more

### Item auction bids: log flooding attempts (FloodProtectorItemAuctionLogFlooding)

`FloodProtector.ini › FloodProtectorItemAuctionLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Item auction bids: attempts before punishment (FloodProtectorItemAuctionPunishmentLimit)

`FloodProtector.ini › FloodProtectorItemAuctionPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Item auction bids: punishment (FloodProtectorItemAuctionPunishmentType)

`FloodProtector.ini › FloodProtectorItemAuctionPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Item auction bids: punishment length (FloodProtectorItemAuctionPunishmentTime)

`FloodProtector.ini › FloodProtectorItemAuctionPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

### Player actions: minimum time between uses (FloodProtectorPlayerActionInterval)

`FloodProtector.ini › FloodProtectorPlayerActionInterval` · number · *Advanced*

Player Action - Next Target, Attack, etc

- **Allowed:** 0 or more

### Player actions: log flooding attempts (FloodProtectorPlayerActionLogFlooding)

`FloodProtector.ini › FloodProtectorPlayerActionLogFlooding` · on/off · *Advanced*

- **Allowed:** On or Off

### Player actions: attempts before punishment (FloodProtectorPlayerActionPunishmentLimit)

`FloodProtector.ini › FloodProtectorPlayerActionPunishmentLimit` · number · *Advanced*

- **Allowed:** 0 or more

### Player actions: punishment (FloodProtectorPlayerActionPunishmentType)

`FloodProtector.ini › FloodProtectorPlayerActionPunishmentType` · choice · *Advanced*

- **Allowed:** one of `none` (No punishment), `kick` (Kick), `ban` (Ban), `jail` (Jail)

### Player actions: punishment length (FloodProtectorPlayerActionPunishmentTime)

`FloodProtector.ini › FloodProtectorPlayerActionPunishmentTime` · number · *Advanced*

- **Allowed:** 0 or more

## Captcha

### Ask farmers to solve a captcha (EnableCaptcha)

`Custom/Captcha.ini › EnableCaptcha` · on/off

Enable bot prevention system.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Monster kills before a captcha](Settings-Protection-Anti-cheat#monster-kills-before-a-captcha-killcounter) — it only works while this is On
- **Controls:** [Random extra kills before a captcha](Settings-Protection-Anti-cheat#random-extra-kills-before-a-captcha-killcounterrandomization) — it only works while this is On
- **Controls:** [Reset the kill count over time](Settings-Protection-Anti-cheat#reset-the-kill-count-over-time-killcounterreset) — it only works while this is On
- **Controls:** [Reset the kill count after](Settings-Protection-Anti-cheat#reset-the-kill-count-after-killcounterresettime) — it only works while this is On
- **Controls:** [Time to answer the captcha](Settings-Protection-Anti-cheat#time-to-answer-the-captcha-validationtime) — it only works while this is On
- **Controls:** [Captcha attempts](Settings-Protection-Anti-cheat#captcha-attempts-captchaattempts) — it only works while this is On
- **Controls:** [Captcha: punishment for failing](Settings-Protection-Anti-cheat#captcha-punishment-for-failing-punishment) — it only works while this is On
- **Controls:** [Captcha: jail time](Settings-Protection-Anti-cheat#captcha-jail-time-jailtime) — it only works while this is On
- **Controls:** [Captcha: double jail time for repeat offenders](Settings-Protection-Anti-cheat#captcha-double-jail-time-for-repeat-offenders-doublejailtime) — it only works while this is On

### Monster kills before a captcha (KillCounter)

`Custom/Captcha.ini › KillCounter` · number

How many monsters have to be killed to run validation task?

- **Default:** `100`
- **Allowed:** 0 – 2147483647
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Random extra kills before a captcha (KillCounterRandomization)

`Custom/Captcha.ini › KillCounterRandomization` · number

Specify range of randomly taken values summed with main counter.

- **Default:** `50`
- **Allowed:** 0 – 2147483647
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Reset the kill count over time (KillCounterReset)

`Custom/Captcha.ini › KillCounterReset` · on/off

Reset the kills counter due to inactivity.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Reset the kill count after (KillCounterResetTime)

`Custom/Captcha.ini › KillCounterResetTime` · number · minutes

Reset the kills counter time after the last mob killed. (in minutes)

- **Default:** `20`
- **Allowed:** 0 – 2147483647 minutes
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Time to answer the captcha (ValidationTime)

`Custom/Captcha.ini › ValidationTime` · number · seconds

How long validation window awaits an answer. (in seconds)

- **Default:** `60`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Captcha attempts (CaptchaAttempts)

`Custom/Captcha.ini › CaptchaAttempts` · number

Number of retries the player has if they type incorrect code. 1 = disable.

- **Default:** `2`
- **Allowed:** 0 – 2147483647
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Captcha: punishment for failing (Punishment)

`Custom/Captcha.ini › Punishment` · choice

Punishments: 0 = move character to the closest village. 1 = kick characters from the server. 2 = put character to jail. 3 = ban character from the server.

- **Default:** `0` (move character to the closest village.)
- **Allowed:** one of `0` (move character to the closest village.), `1` (kick characters from the server.), `2` (put character to jail.), `3` (ban character from the server.)
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Captcha: jail time (JailTime)

`Custom/Captcha.ini › JailTime` · number · minutes

How long character were suppose to stay in jail? (in minutes) 0 = forever in jail.

- **Default:** `2`
- **Allowed:** 0 – 2147483647 minutes
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

### Captcha: double jail time for repeat offenders (DoubleJailTime)

`Custom/Captcha.ini › DoubleJailTime` · on/off

Double the time on each entry in jail. After 24h the time will return from the initial time.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Ask farmers to solve a captcha](Settings-Protection-Anti-cheat#ask-farmers-to-solve-a-captcha-enablecaptcha) to be On

## Dual-box limits

### Characters online per IP (DualboxCheckMaxPlayersPerIP)

`Custom/DualboxCheck.ini › DualboxCheckMaxPlayersPerIP` · number

0 = unlimited.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Olympiad players per IP (DualboxCheckMaxOlympiadParticipantsPerIP)

`Custom/DualboxCheck.ini › DualboxCheckMaxOlympiadParticipantsPerIP` · number

Maximum number of players per IP address allowed to participate in olympiad.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Event players per IP (DualboxCheckMaxL2EventParticipantsPerIP)

`Custom/DualboxCheck.ini › DualboxCheckMaxL2EventParticipantsPerIP` · number

Maximum number of players per IP address allowed to participate in events using L2J Event Engine (//event).

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Offline auto play characters per IP (DualboxCheckMaxOfflinePlayPerIP)

`Custom/DualboxCheck.ini › DualboxCheckMaxOfflinePlayPerIP` · number

Maximum number of players per IP address allowed to offlineplay.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Offline auto play characters per IP (premium) (DualboxCheckMaxOfflinePlayPremiumPerIP)

`Custom/DualboxCheck.ini › DualboxCheckMaxOfflinePlayPremiumPerIP` · number

Maximum number of premium players per IP address allowed to offlineplay.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Offline traders count toward the limit (DualboxCountOfflineTraders)

`Custom/DualboxCheck.ini › DualboxCountOfflineTraders` · on/off

Count offline traders as dualbox.

- **Default:** Off
- **Allowed:** On or Off

### IPs without limits (DualboxCheckWhitelist)

`Custom/DualboxCheck.ini › DualboxCheckWhitelist` · list

Whitelist of the addresses for dualbox checks. Format: Address1,Number1;Address2,Number2... Network address can be number (127.0.0.1) or symbolic (localhost) formats. Additional connection number added to the global limits for this address. For example, if number of TvT event participants per IP address set to the 1 (no dualbox) and whitelist contains "l2jmobius.org,2" then number of allowed participants from l2jmobius.org will be 1+2=3. Use 0 or negative value for unlimited number of connections.

- **Default:** `127.0.0.1,0`
- **Allowed:** IP addresses separated by commas

## Hardware ID

### Read hardware ID from the client (EnableHardwareInfo)

`Server.ini › EnableHardwareInfo` · on/off

Needs a modified client.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Kick clients without a hardware ID](Settings-Protection-Anti-cheat#kick-clients-without-a-hardware-id-kickmissinghwid) — it only works while this is On
- **Controls:** [Characters per computer](Settings-Protection-Anti-cheat#characters-per-computer-maxplayersperhwid) — it only works while this is On

### Kick clients without a hardware ID (KickMissingHWID)

`Server.ini › KickMissingHWID` · on/off

Players without hardware information are kicked from the game. Automatically set to True when MaxPlayersPerHWID > 0.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Read hardware ID from the client](Settings-Protection-Anti-cheat#read-hardware-id-from-the-client-enablehardwareinfo) to be On — Hardware IDs are not being read.

### Characters per computer (MaxPlayersPerHWID)

`Server.ini › MaxPlayersPerHWID` · number

0 = unlimited.

- **Default:** `0`
- **Allowed:** 0 – 100
- **Needs:** [Read hardware ID from the client](Settings-Protection-Anti-cheat#read-hardware-id-from-the-client-enablehardwareinfo) to be On — Hardware IDs are not being read.

## Login security

### Wrong passwords before an IP is blocked (LoginTryBeforeBan)

`Server.ini › LoginTryBeforeBan` · number

How many times you can provide an invalid account/pass before the IP gets banned.

- **Default:** `5`
- **Allowed:** 0 or more

### Block wrong-password IPs for (LoginBlockAfterBan)

`Server.ini › LoginBlockAfterBan` · number · seconds

Time you won't be able to login back again after LoginTryBeforeBan tries to login.

- **Default:** `900 (15 minutes)`
- **Allowed:** 0 or more seconds

### Accept new game servers (AcceptNewGameServer)

`Server.ini › AcceptNewGameServer` · on/off

If set to True any GameServer can register on your login's free slots

- **Default:** On
- **Allowed:** On or Off

### Login flood protection (EnableFloodProtection)

`Server.ini › EnableFloodProtection` · on/off

Flood Protection. All values are in milliseconds.

- **Default:** On
- **Allowed:** On or Off

### Fast connections allowed (FastConnectionLimit)

`Server.ini › FastConnectionLimit` · number

- **Default:** `15`
- **Allowed:** 0 or more

### Normal connection time (NormalConnectionTime)

`Server.ini › NormalConnectionTime` · number · ms

- **Default:** `700`
- **Allowed:** 0 or more ms

### Fast connection time (FastConnectionTime)

`Server.ini › FastConnectionTime` · number · ms

- **Default:** `350`
- **Allowed:** 0 or more ms

### Connections per IP (MaxConnectionPerIP)

`Server.ini › MaxConnectionPerIP` · number

- **Default:** `50`
- **Allowed:** 0 or more

## Bot protection

### Check players for skills they should not have (SkillCheckEnable)

`General.ini › SkillCheckEnable` · on/off

Check players for non-allowed skills

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Remove skills players should not have](Settings-Protection-Anti-cheat#remove-skills-players-should-not-have-skillcheckremove) — it only works while this is On
- **Controls:** [Also check Game Masters' skills](Settings-Protection-Anti-cheat#also-check-game-masters-skills-skillcheckgm) — it only works while this is On

### Remove skills players should not have (SkillCheckRemove)

`General.ini › SkillCheckRemove` · on/off

If true, remove invalid skills from player and database. Report only, if false.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Check players for skills they should not have](Settings-Protection-Anti-cheat#check-players-for-skills-they-should-not-have-skillcheckenable) to be On — Skill checking is off.

### Also check Game Masters' skills (SkillCheckGM)

`General.ini › SkillCheckGM` · on/off

Check also GM characters (only if SkillCheckEnable = True)

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Check players for skills they should not have](Settings-Protection-Anti-cheat#check-players-for-skills-they-should-not-have-skillcheckenable) to be On — Skill checking is off.

### Detect L2Walker bots (L2WalkerProtection)

`Custom/WalkerBotProtection.ini › L2WalkerProtection` · on/off

Basic protection against L2Walker.

- **Default:** Off
- **Allowed:** On or Off
