# Events & Activities settings

Seven Signs, Dimensional Rift, lottery, fishing, weddings and instances.

**69 settings** in the **Events & Activities** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Seven Signs & Festival of Darkness

### Catacombs and necropolises always open (AltOpenCatacumbs)

`Feature.ini › AltOpenCatacumbs` · on/off

Seven Signs Determine any player can use Necropoils and catacoms independing seal status or period.

- **Default:** Off
- **Allowed:** On or Off

### Necropolises always open (AltOpenNecropolis)

`Feature.ini › AltOpenNecropolis` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Only castle-owning clans can join Seven Signs (AltRequireClanCastle)

`Feature.ini › AltRequireClanCastle` · on/off

Determines whether castle ownership is determined by clan or by alliance. Default is by alliance, as on official servers.

- **Default:** Off
- **Allowed:** On or Off

### Joining Dawn requires a castle (AltCastleForDawn)

`Feature.ini › AltCastleForDawn` · on/off

Dawn: True - Players not owning castle need pay participation fee False - Anyone can join Dawn

- **Default:** On
- **Allowed:** On or Off

### Joining Dusk requires no castle (AltCastleForDusk)

`Feature.ini › AltCastleForDusk` · on/off

Dusk: True - Players owning castle can not join Dusk side False - Anyone can join Dusk

- **Default:** On
- **Allowed:** On or Off

### Festival of Darkness: minimum party (AltFestivalMinPlayer)

`Feature.ini › AltFestivalMinPlayer` · number

Minimum Players for participate in SevenSigns Festival.

- **Default:** `5`
- **Allowed:** 0 – 2147483647

### Most seal stones a player can contribute (AltMaxPlayerContrib)

`Feature.ini › AltMaxPlayerContrib` · number

Maximum seal stone contribution per player during festival.

- **Default:** `1000000`
- **Allowed:** 0 – 2147483647

### Festival: signup opens after (AltFestivalManagerStart)

`Feature.ini › AltFestivalManagerStart` · number · ms

Festival Manager Start time (set in milliseconds; 1000 = 1 second).

- **Default:** `120000`
- **Allowed:** 0 – 9223372036854775807 ms

### Festival length (AltFestivalLength)

`Feature.ini › AltFestivalLength` · number · ms

Festival Length (set in milliseconds; 1000 = 1 second).

- **Default:** `1080000`
- **Allowed:** 0 – 9223372036854775807 ms

### Festival cycle length (AltFestivalCycleLength)

`Feature.ini › AltFestivalCycleLength` · number · ms

Festival Cycle Length.

- **Default:** `2280000`
- **Allowed:** 0 – 9223372036854775807 ms

### Festival: first monsters after (AltFestivalFirstSpawn)

`Feature.ini › AltFestivalFirstSpawn` · number · ms

At what point the first festival spawn occurs.

- **Default:** `120000`
- **Allowed:** 0 – 9223372036854775807 ms

### Festival: first swarm after (AltFestivalFirstSwarm)

`Feature.ini › AltFestivalFirstSwarm` · number · ms

At what Point the first festival swarm occurs.

- **Default:** `300000`
- **Allowed:** 0 – 9223372036854775807 ms

### Festival: second monsters after (AltFestivalSecondSpawn)

`Feature.ini › AltFestivalSecondSpawn` · number · ms

At what Point the second festival spawn occurs.

- **Default:** `540000`
- **Allowed:** 0 – 9223372036854775807 ms

### Festival: second swarm after (AltFestivalSecondSwarm)

`Feature.ini › AltFestivalSecondSwarm` · number · ms

At what Point the second festival spawn occurs.

- **Default:** `720000`
- **Allowed:** 0 – 9223372036854775807 ms

### Festival: chests appear after (AltFestivalChestSpawn)

`Feature.ini › AltFestivalChestSpawn` · number · ms

At what point the chests spawn in.

- **Default:** `900000`
- **Allowed:** 0 – 9223372036854775807 ms

### Dawn castle gate P. Def. (AltDawnGatesPdefMult)

`Feature.ini › AltDawnGatesPdefMult` · number · times

This multipliers are used to change P.Def/M.Def of castle gates/walls while Seal of Strife is controlled by one of the sides.

- **Default:** `1.1`
- **Allowed:** 0 or more times

### Dusk castle gate P. Def. (AltDuskGatesPdefMult)

`Feature.ini › AltDuskGatesPdefMult` · number · times

- **Default:** `0.8`
- **Allowed:** 0 or more times

### Dawn castle gate M. Def. (AltDawnGatesMdefMult)

`Feature.ini › AltDawnGatesMdefMult` · number · times

- **Default:** `1.1`
- **Allowed:** 0 or more times

### Dusk castle gate M. Def. (AltDuskGatesMdefMult)

`Feature.ini › AltDuskGatesMdefMult` · number · times

- **Default:** `0.8`
- **Allowed:** 0 or more times

### Retail Seven Signs restrictions (StrictSevenSigns)

`Feature.ini › StrictSevenSigns` · on/off

If this = true only cabal period owners can use merchant and blacksmith of mammon. If false any player can use.

- **Default:** On
- **Allowed:** On or Off

### Save Seven Signs data less often (AltSevenSignsLazyUpdate)

`Feature.ini › AltSevenSignsLazyUpdate` · on/off

Save SevenSigns status only each 30 minutes and after period change. Player info saved only during periodic data store (set by CharacterDataStoreInterval) and logout. If False then save info and status immediately after changes.

- **Default:** On
- **Allowed:** On or Off

### Lord of Dawn tickets available (SevenSignsDawnTicketQuantity)

`Feature.ini › SevenSignsDawnTicketQuantity` · number

Total count of available tickets.

- **Default:** `300`
- **Allowed:** 0 – 2147483647

### Lord of Dawn ticket price (SevenSignsDawnTicketPrice)

`Feature.ini › SevenSignsDawnTicketPrice` · number · adena

Price of each ticket.

- **Default:** `1000`
- **Allowed:** 0 – 2147483647 adena

### Lord of Dawn tickets per bundle (SevenSignsDawnTicketBundle)

`Feature.ini › SevenSignsDawnTicketBundle` · number

Tickets bundle (exchanged in amounts of).

- **Default:** `10`
- **Allowed:** 0 – 2147483647

### Manor agreement item ID (SevenSignsManorsAgreementId)

`Feature.ini › SevenSignsManorsAgreementId` · number

Ticket item Id.

- **Default:** `6388`
- **Allowed:** 0 – 2147483647

### Fee to join Dawn (SevenSignsJoinDawnFee)

`Feature.ini › SevenSignsJoinDawnFee` · number · adena

Fee for joining Dawn

- **Default:** `50000`
- **Allowed:** 0 – 2147483647 adena

## Dimensional Rift

### Smallest party allowed in the rift (RiftMinPartySize)

`General.ini › RiftMinPartySize` · number

Minimum party size to enter rift. Min = 2, Max = 9. If while inside the rift, the party becomes smaller, all members will be teleported back.

- **Default:** `5`
- **Allowed:** 2 – 9

### Room jumps before being sent back (MaxRiftJumps)

`General.ini › MaxRiftJumps` · number

Number of maximum jumps between rooms allowed, after this time party will be teleported back

- **Default:** `4`
- **Allowed:** 1 – 9

### Monsters appear after entering a room (RiftSpawnDelay)

`General.ini › RiftSpawnDelay` · number · ms

Time in ms the party has to wait until the mobs spawn when entering a room. C4 retail: 10s

- **Default:** `10000`
- **Allowed:** 0 – 2147483647 ms

### Automatic jump: shortest wait (AutoJumpsDelayMin)

`General.ini › AutoJumpsDelayMin` · number · seconds

Time between automatic jumps in seconds

- **Default:** `480`
- **Allowed:** 0 – 2147483647 seconds

### Automatic jump: longest wait (AutoJumpsDelayMax)

`General.ini › AutoJumpsDelayMax` · number · seconds

- **Default:** `600`
- **Allowed:** 0 – 2147483647 seconds

### Extra time in the boss room (BossRoomTimeMultiply)

`General.ini › BossRoomTimeMultiply` · number · times

Time Multiplier for stay in the boss room

- **Default:** `1.5`
- **Allowed:** 0 or more times

### Entry cost: Recruit rift (RecruitCost)

`General.ini › RecruitCost` · number

Dimensional fragments per party member.

- **Default:** `18`
- **Allowed:** 0 – 2147483647

### Entry cost: Soldier rift (SoldierCost)

`General.ini › SoldierCost` · number

- **Default:** `21`
- **Allowed:** 0 – 2147483647

### Entry cost: Officer rift (OfficerCost)

`General.ini › OfficerCost` · number

- **Default:** `24`
- **Allowed:** 0 – 2147483647

### Entry cost: Captain rift (CaptainCost)

`General.ini › CaptainCost` · number

- **Default:** `27`
- **Allowed:** 0 – 2147483647

### Entry cost: Commander rift (CommanderCost)

`General.ini › CommanderCost` · number

- **Default:** `30`
- **Allowed:** 0 – 2147483647

### Entry cost: Hero rift (HeroCost)

`General.ini › HeroCost` · number

- **Default:** `33`
- **Allowed:** 0 – 2147483647

## Lottery

### Enable the lottery (AllowLottery)

`General.ini › AllowLottery` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Starting jackpot](Settings-Events-Activities#starting-jackpot-altlotteryprize) — it only works while this is On
- **Controls:** [Ticket price](Settings-Events-Activities#ticket-price-altlotteryticketprice) — it only works while this is On

### Starting jackpot (AltLotteryPrize)

`General.ini › AltLotteryPrize` · number · adena

Initial Lottery prize.

- **Default:** `50000`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Enable the lottery](Settings-Events-Activities#enable-the-lottery-allowlottery) to be On — The lottery is off.

### Ticket price (AltLotteryTicketPrice)

`General.ini › AltLotteryTicketPrice` · number · adena

Lottery Ticket Price

- **Default:** `2000`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Enable the lottery](Settings-Events-Activities#enable-the-lottery-allowlottery) to be On — The lottery is off.

### Share of jackpot for 5 numbers (AltLottery5NumberRate)

`General.ini › AltLottery5NumberRate` · number · times

0.6 = 60%.

- **Default:** `0.6`
- **Allowed:** 0 – 1 times

### Share of jackpot for 4 numbers (AltLottery4NumberRate)

`General.ini › AltLottery4NumberRate` · number · times

What part of jackpot amount should receive characters who pick 4 wining numbers

- **Default:** `0.2`
- **Allowed:** 0 – 1 times

### Share of jackpot for 3 numbers (AltLottery3NumberRate)

`General.ini › AltLottery3NumberRate` · number · times

What part of jackpot amount should receive characters who pick 3 wining numbers

- **Default:** `0.2`
- **Allowed:** 0 – 1 times

### Prize for 2 or fewer numbers (AltLottery2and1NumberPrize)

`General.ini › AltLottery2and1NumberPrize` · number · adena

How much Adena receive characters who pick two or less of the winning number

- **Default:** `200`
- **Allowed:** 0 – 2147483647 adena

## Fishing championship

### Enable the fishing championship (AltFishChampionshipEnabled)

`General.ini › AltFishChampionshipEnabled` · on/off

Enable or disable the Fishing Tournament system

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Prize item ID](Settings-Events-Activities#prize-item-id-altfishchampionshiprewarditemid) — it only works while this is On

### Prize item ID (AltFishChampionshipRewardItemId)

`General.ini › AltFishChampionshipRewardItemId` · number

57 = adena.

- **Default:** `57`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable the fishing championship](Settings-Events-Activities#enable-the-fishing-championship-altfishchampionshipenabled) to be On — The fishing championship is off.

### 1st place prize (AltFishChampionshipReward1)

`General.ini › AltFishChampionshipReward1` · number

Item count used as reward (for the 5 first winners)

- **Default:** `800000`
- **Allowed:** 0 – 2147483647

### 2nd place prize (AltFishChampionshipReward2)

`General.ini › AltFishChampionshipReward2` · number

- **Default:** `500000`
- **Allowed:** 0 – 2147483647

### 3rd place prize (AltFishChampionshipReward3)

`General.ini › AltFishChampionshipReward3` · number

- **Default:** `300000`
- **Allowed:** 0 – 2147483647

### 4th place prize (AltFishChampionshipReward4)

`General.ini › AltFishChampionshipReward4` · number

- **Default:** `200000`
- **Allowed:** 0 – 2147483647

### 5th place prize (AltFishChampionshipReward5)

`General.ini › AltFishChampionshipReward5` · number

- **Default:** `100000`
- **Allowed:** 0 – 2147483647

## Item auction

### Enable the item auction (AltItemAuctionEnabled)

`General.ini › AltItemAuctionEnabled` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Delete finished auctions after](Settings-Events-Activities#delete-finished-auctions-after-altitemauctionexpiredafter) — it only works while this is On
- **Controls:** [Extend an auction on each new bid by](Settings-Events-Activities#extend-an-auction-on-each-new-bid-by-altitemauctiontimeextendsonbid) — it only works while this is On

### Delete finished auctions after (AltItemAuctionExpiredAfter)

`General.ini › AltItemAuctionExpiredAfter` · number · days

Number of days before auction cleared from database with all bids.

- **Default:** `14`
- **Allowed:** 0 – 2147483647 days
- **Needs:** [Enable the item auction](Settings-Events-Activities#enable-the-item-auction-altitemauctionenabled) to be On — The item auction is off.

### Extend an auction on each new bid by (AltItemAuctionTimeExtendsOnBid)

`General.ini › AltItemAuctionTimeExtendsOnBid` · number · seconds

Auction extends to specified amount of seconds if one or more new bids added. By default auction extends only two times, by 5 and 3 minutes, this custom value used after it. Values higher than 60s is not recommended.

- **Default:** `0`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Enable the item auction](Settings-Events-Activities#enable-the-item-auction-altitemauctionenabled) to be On — The item auction is off.

## Weddings

### Enable weddings (AllowWedding)

`Custom/Wedding.ini › AllowWedding` · on/off

Wedding System Wedding Manager ID: 50007 First part - "Engagement" 1) Target the player that you want to make a couple with. 2) Use the voice command ".engage nameofyourpartner" then press enter. 3) If the target player has you on listed as a friend (ie. you are in each other's friends list) a popup will appear with an engagement request along with a system message that you want to be engaged with him/her. 4) If the target player accepts the engagement invitation, you will be engaged. Second part - "Marriage" 1) Once two players are engaged, they can speak to Andromeda, the Wedding Priest in the Hot Springs Guild House (Goddard Area). (You may need Formal Wear and Adena to pay wedding fees!) 2) Each player needs to speak to the NPC and make the request to be married. 3) Once done, fireworks will display and the two players will be married. Afterwards you can use the voice command ".gotolove nameofyourpartner" to teleport to your partner if you're married (there may also be a fee which can be specified below) If you want to cancel your Engagement/Marriage, use the voice command ".divorce nameofyourpartner". If you're married you have to pay a specified % of your adena to your partner. If a player attempts to become engaged to another player while married they may suffer a penalty if it's enabled below. Enable/Disable Wedding System

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Wedding price](Settings-Events-Activities#wedding-price-weddingprice) — it only works while this is On
- **Controls:** [Punish cheating spouses](Settings-Events-Activities#punish-cheating-spouses-weddingpunishinfidelity) — it only works while this is On
- **Controls:** [Spouses can teleport to each other](Settings-Events-Activities#spouses-can-teleport-to-each-other-weddingteleport) — it only works while this is On
- **Controls:** [Allow same-sex weddings](Settings-Events-Activities#allow-same-sex-weddings-weddingallowsamesex) — it only works while this is On
- **Controls:** [Formal wear required](Settings-Events-Activities#formal-wear-required-weddingformalwear) — it only works while this is On
- **Controls:** [Divorce costs this share of the adena](Settings-Events-Activities#divorce-costs-this-share-of-the-adena-weddingdivorcecosts) — it only works while this is On

### Wedding price (WeddingPrice)

`Custom/Wedding.ini › WeddingPrice` · number · adena

Amount of Adena required to get married

- **Default:** `250000000`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Enable weddings](Settings-Events-Activities#enable-weddings-allowwedding) to be On

### Punish cheating spouses (WeddingPunishInfidelity)

`Custom/Wedding.ini › WeddingPunishInfidelity` · on/off

Enable/Disable punishing of players who attempt to be engaged to other players while married.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable weddings](Settings-Events-Activities#enable-weddings-allowwedding) to be On

### Spouses can teleport to each other (WeddingTeleport)

`Custom/Wedding.ini › WeddingTeleport` · on/off

Enable/Disable teleport function for married couples.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable weddings](Settings-Events-Activities#enable-weddings-allowwedding) to be On
- **Controls:** [Spouse teleport price](Settings-Events-Activities#spouse-teleport-price-weddingteleportprice) — it only works while this is On
- **Controls:** [Spouse teleport casting time](Settings-Events-Activities#spouse-teleport-casting-time-weddingteleportduration) — it only works while this is On

### Spouse teleport price (WeddingTeleportPrice)

`Custom/Wedding.ini › WeddingTeleportPrice` · number · adena

Amount of Adena required to teleport to spouse.

- **Default:** `50000`
- **Allowed:** 0 – 2147483647 adena
- **Needs:** [Spouses can teleport to each other](Settings-Events-Activities#spouses-can-teleport-to-each-other-weddingteleport) to be On — Spouse teleport is off.

### Spouse teleport casting time (WeddingTeleportDuration)

`Custom/Wedding.ini › WeddingTeleportDuration` · number · seconds

Time before character is teleported after using the skill.

- **Default:** `60`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Spouses can teleport to each other](Settings-Events-Activities#spouses-can-teleport-to-each-other-weddingteleport) to be On — Spouse teleport is off.

### Allow same-sex weddings (WeddingAllowSameSex)

`Custom/Wedding.ini › WeddingAllowSameSex` · on/off

Enable/Disable same sex marriages.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable weddings](Settings-Events-Activities#enable-weddings-allowwedding) to be On

### Formal wear required (WeddingFormalWear)

`Custom/Wedding.ini › WeddingFormalWear` · on/off

Require players to wear formal wear to be married?

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable weddings](Settings-Events-Activities#enable-weddings-allowwedding) to be On

### Divorce costs this share of the adena (WeddingDivorceCosts)

`Custom/Wedding.ini › WeddingDivorceCosts` · number · %

Amount of Adena (%) a player must pay to the other to get divorced.

- **Default:** `20`
- **Allowed:** 0 – 100 %
- **Needs:** [Enable weddings](Settings-Events-Activities#enable-weddings-allowwedding) to be On

## Instances

### Return players to their instance on login (RestorePlayerInstance)

`General.ini › RestorePlayerInstance` · on/off

Instances Restores the player to their previous instance (ie. an instanced area/dungeon) on EnterWorld.

- **Default:** Off
- **Allowed:** On or Off

### Summon friends into instances (AllowSummonInInstance)

`General.ini › AllowSummonInInstance` · on/off

Set whether summon skills can be used to summon players inside an instance. When enabled individual instances can have summoning disabled in instance xml's.

- **Default:** Off
- **Allowed:** On or Off

### Remove dead players from an instance after (EjectDeadPlayerTime)

`General.ini › EjectDeadPlayerTime` · number · seconds

When a player dies, is removed from instance after a fixed period of time. Time in seconds.

- **Default:** `60`
- **Allowed:** 0 – 2147483647 seconds

### Close a finished instance after (DefaultFinishTime)

`General.ini › DefaultFinishTime` · number · seconds

When is instance finished, is set time to destruction currency instance. Time in seconds.

- **Default:** `300`
- **Allowed:** 0 – 2147483647 seconds

## Cursed weapons

### Enable cursed weapons (Zariche, Akamanah) (AllowCursedWeapons)

`General.ini › AllowCursedWeapons` · on/off · *added by L2Everdream*

The two cursed swords that can drop from monsters and make their holder very powerful.

- **Default:** On
- **Allowed:** On or Off
