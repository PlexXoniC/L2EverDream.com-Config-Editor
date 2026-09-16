# Chat & Community settings

Chat channels, filters, Community Board, languages and messages.

**41 settings** in the **Chat & Community** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Chat

### Shout chat (!) (GlobalChat)

`General.ini › GlobalChat` · choice

Global Chat. Available Options: ON, OFF, GM, GLOBAL

- **Default:** `ON`
- **Allowed:** one of `ON`, `OFF`, `GM`, `GLOBAL`

### Trade chat (+) (TradeChat)

`General.ini › TradeChat` · choice

Trade Chat. Available Options: ON, OFF, GM, GLOBAL

- **Default:** `ON`
- **Allowed:** one of `ON`, `OFF`, `GM`, `GLOBAL`

### Level needed to chat (MinimumChatLevel)

`General.ini › MinimumChatLevel` · number

0 = no limit.

- **Default:** `20`
- **Allowed:** 0 – 80

### Filter bad words in chat (UseChatFilter)

`General.ini › UseChatFilter` · on/off

Enable chat filter Default = False

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Replace filtered words with](Settings-Chat-Community#replace-filtered-words-with-chatfilterchars) — it only works while this is On

### Replace filtered words with (ChatFilterChars)

`General.ini › ChatFilterChars` · text

Replace filter words with following chars

- **Default:** `^_^`
- **Needs:** [Filter bad words in chat](Settings-Chat-Community#filter-bad-words-in-chat-usechatfilter) to be On — The chat filter is off.

### Channels blocked by a chat ban (BanChatChannels)

`General.ini › BanChatChannels` · list

Separate channel names with ;

- **Default:** `GENERAL;SHOUT;WORLD;TRADE;HERO_VOICE`
- **Allowed:** words separated by semicolons

### Enable GM chat moderation commands (ChatAdmin)

`Custom/ChatModeration.ini › ChatAdmin` · on/off

This option will enable using of the voice commands .banchat and .unbanchat for players with corresponding access level (default: 100). Check AdminCommands.xml for details.

- **Default:** On
- **Allowed:** On or Off

## Community Board

### Enable the Community Board (Alt+B) (EnableCommunityBoard)

`General.ini › EnableCommunityBoard` · on/off

Enable the Community Board.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Community Board start page](Settings-Chat-Community#community-board-start-page-bbsdefault) — it only works while this is On

### Community Board start page (BBSDefault)

`General.ini › BBSDefault` · text

Default Community Board page.

- **Default:** `_bbshome`
- **Needs:** [Enable the Community Board (Alt+B)](Settings-Chat-Community#enable-the-community-board-alt-b-enablecommunityboard) to be On — The Community Board is off.

### Use the custom Community Board (CustomCommunityBoard)

`Custom/CommunityBoard.ini › CustomCommunityBoard` · on/off

Enable Custom Community Board.

- **Default:** Off
- **Allowed:** On or Off

### Community Board currency item ID (CommunityCurrencyId)

`Custom/CommunityBoard.ini › CommunityCurrencyId` · number

57 = adena.

- **Default:** `57`
- **Allowed:** 0 – 2147483647

### Community Board shops (CommunityEnableMultisells)

`Custom/CommunityBoard.ini › CommunityEnableMultisells` · on/off

Enable Multisells.

- **Default:** On
- **Allowed:** On or Off

### Community Board teleports (CommunityEnableTeleports)

`Custom/CommunityBoard.ini › CommunityEnableTeleports` · on/off

Enable Teleports.

- **Default:** On
- **Allowed:** On or Off

### Community Board buffs (CommunityEnableBuffs)

`Custom/CommunityBoard.ini › CommunityEnableBuffs` · on/off

Enable Buffs.

- **Default:** On
- **Allowed:** On or Off

### Community Board healing (CommunityEnableHeal)

`Custom/CommunityBoard.ini › CommunityEnableHeal` · on/off

Enable Heal.

- **Default:** On
- **Allowed:** On or Off

### Community Board delevel (CommunityEnableDelevel)

`Custom/CommunityBoard.ini › CommunityEnableDelevel` · on/off

Enable delevel.

- **Default:** Off
- **Allowed:** On or Off

### Teleport price (CommunityTeleportPrice)

`Custom/CommunityBoard.ini › CommunityTeleportPrice` · number

Price for Teleports.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Buff price (CommunityBuffPrice)

`Custom/CommunityBoard.ini › CommunityBuffPrice` · number

Price for Buffs.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Heal price (CommunityHealPrice)

`Custom/CommunityBoard.ini › CommunityHealPrice` · number

Price for Heal.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Delevel price (CommunityDelevelPrice)

`Custom/CommunityBoard.ini › CommunityDelevelPrice` · number

Price for Delevel.

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Only usable in peace zones (CommunityBoardPeaceOnly)

`Custom/CommunityBoard.ini › CommunityBoardPeaceOnly` · on/off

Enable Custom Community Board only in peace zone.

- **Default:** Off
- **Allowed:** On or Off

### Not usable while in combat (CommunityCombatDisabled)

`Custom/CommunityBoard.ini › CommunityCombatDisabled` · on/off

Disable Community Board while in combat.

- **Default:** On
- **Allowed:** On or Off

### Not usable with karma (CommunityKarmaDisabled)

`Custom/CommunityBoard.ini › CommunityKarmaDisabled` · on/off

Disable Community Board while player has Karma.

- **Default:** On
- **Allowed:** On or Off

### Show cast animations for board buffs (CommunityCastAnimations)

`Custom/CommunityBoard.ini › CommunityCastAnimations` · on/off

Cast animations of each buff.

- **Default:** Off
- **Allowed:** On or Off

### Sell premium on the Community Board (CommunityPremiumSystem)

`Custom/CommunityBoard.ini › CommunityPremiumSystem` · on/off

Enable buying premium from community board. Premium System must be enabled.

- **Default:** Off
- **Allowed:** On or Off

### Premium price item ID (CommunityPremiumBuyCoinId)

`Custom/CommunityBoard.ini › CommunityPremiumBuyCoinId` · number

ItemID for buying premium in community board. Check data/html/CommunityBoard/Custom/premium/main.html

- **Default:** `57`
- **Allowed:** 0 – 2147483647

### Premium price per day (CommunityPremiumPricePerDay)

`Custom/CommunityBoard.ini › CommunityPremiumPricePerDay` · number

Amount of coins needed for each premium day bought.

- **Default:** `1000000`
- **Allowed:** 0 – 2147483647

### Buffs offered on the board (CommunityAvailableBuffs)

`Custom/CommunityBoard.ini › CommunityAvailableBuffs` · list

List of available buffs to avoid exploits. Usage: SkillId1,SkillId2...


### Teleports offered on the board (CommunityTeleportList)

`Custom/CommunityBoard.ini › CommunityTeleportList` · list

List of available teleports to avoid exploits. Usage: TeleportName1,X1,Y1,Z1;TeleportName2,X2,Y2,Z2... the "\" indicates a new line


## Languages

### Multiple languages for NPC dialogs (MultiLangEnable)

`Custom/MultilingualSupport.ini › MultiLangEnable` · on/off

Enable or disable multilingual support.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Default language](Settings-Chat-Community#default-language-multilangdefault) — it only works while this is On
- **Controls:** [Available languages](Settings-Chat-Community#available-languages-multilangallowed) — it only works while this is On
- **Controls:** [Let players pick a language with .lang](Settings-Chat-Community#let-players-pick-a-language-with-lang-multilangvoicecommand) — it only works while this is On

### Default language (MultiLangDefault)

`Custom/MultilingualSupport.ini › MultiLangDefault` · text

Default language, if not defined.

- **Default:** `en`
- **Needs:** [Multiple languages for NPC dialogs](Settings-Chat-Community#multiple-languages-for-npc-dialogs-multilangenable) to be On

### Available languages (MultiLangAllowed)

`Custom/MultilingualSupport.ini › MultiLangAllowed` · list

List of allowed languages, semicolon separated.

- **Default:** `en;el`
- **Allowed:** words separated by semicolons
- **Needs:** [Multiple languages for NPC dialogs](Settings-Chat-Community#multiple-languages-for-npc-dialogs-multilangenable) to be On

### Let players pick a language with .lang (MultiLangVoiceCommand)

`Custom/MultilingualSupport.ini › MultiLangVoiceCommand` · on/off

Enable or disable voice command .lang for changing languages on the fly.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Multiple languages for NPC dialogs](Settings-Chat-Community#multiple-languages-for-npc-dialogs-multilangenable) to be On

## Welcome & info messages

### Show server news on login (ShowServerNews)

`General.ini › ShowServerNews` · on/off

Show "data/html/servnews.htm" when a character enters world.

- **Default:** Off
- **Allowed:** On or Off

### Enable the .online command (EnableOnlineCommand)

`Custom/OnlineInfo.ini › EnableOnlineCommand` · on/off

Enable player .online voiced command. Shows how many players are online.

- **Default:** Off
- **Allowed:** On or Off

### Show a welcome message on screen (ScreenWelcomeMessageEnable)

`Custom/ScreenWelcomeMessage.ini › ScreenWelcomeMessageEnable` · on/off

Show screen welcome message on character login

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Welcome message text](Settings-Chat-Community#welcome-message-text-screenwelcomemessagetext) — it only works while this is On
- **Controls:** [Welcome message stays for](Settings-Chat-Community#welcome-message-stays-for-screenwelcomemessagetime) — it only works while this is On

### Welcome message text (ScreenWelcomeMessageText)

`Custom/ScreenWelcomeMessage.ini › ScreenWelcomeMessageText` · text

Screen welcome message text to show on character login if enabled ('#' for a new line, but message can have max 2 lines)

- **Default:** `Welcome to our server!`
- **Needs:** [Show a welcome message on screen](Settings-Chat-Community#show-a-welcome-message-on-screen-screenwelcomemessageenable) to be On

### Welcome message stays for (ScreenWelcomeMessageTime)

`Custom/ScreenWelcomeMessage.ini › ScreenWelcomeMessageTime` · number · seconds

Show screen welcome message for x seconds when character log in to game if enabled

- **Default:** `10`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Show a welcome message on screen](Settings-Chat-Community#show-a-welcome-message-on-screen-screenwelcomemessageenable) to be On

### Show server time on login (DisplayServerTime)

`Custom/ServerTime.ini › DisplayServerTime` · on/off

This option will enable displaying of the local server time for /time command.

- **Default:** Off
- **Allowed:** On or Off

## Mail

### Deliver items queued from the database (CustomMailManagerEnabled)

`Custom/CustomMailManager.ini › CustomMailManagerEnabled` · on/off

Needed for adding new items in the Characters tab: the server checks for queued items every few seconds and adds them to characters who are online. Takes effect the next time the world starts.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Check for new mail every](Settings-Chat-Community#check-for-new-mail-every-databasequerydelay) — it only works while this is On

### Check for new mail every (DatabaseQueryDelay)

`Custom/CustomMailManager.ini › DatabaseQueryDelay` · number · seconds

Database query delay in seconds.

- **Default:** `30`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Deliver items queued from the database](Settings-Chat-Community#deliver-items-queued-from-the-database-custommailmanagerenabled) to be On
