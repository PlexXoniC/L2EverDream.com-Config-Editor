# Olympiad & Heroes settings

The Grand Olympiad schedule, rules and rewards.

**30 settings** in the **Olympiad & Heroes** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Olympiad

### Enable the Olympiad (OlympiadEnabled)

`Olympiad.ini › OlympiadEnabled` · on/off

Enable Olympiad.

- **Default:** On
- **Allowed:** On or Off

### Daily start hour (OlympiadStartTime)

`Olympiad.ini › OlympiadStartTime` · number · hour

Olympiad Start Time in Military hours Default 6pm (18)

- **Default:** `18`
- **Allowed:** 0 – 23 hour

### Daily start minute (OlympiadMin)

`Olympiad.ini › OlympiadMin` · number · minute

Olympiad Start Time for Min's, Default 00 so at the start of the hour.

- **Default:** `0`
- **Allowed:** 0 – 59 minute

### Competition period length (OlympiadCPeriod)

`Olympiad.ini › OlympiadCPeriod` · number · ms

Olympiad Competition Period, Default 6 hours. (If set different, should be increment by 10mins)

- **Default:** `21600000`
- **Allowed:** 0 – 9223372036854775807 ms

### Match length (OlympiadBattle)

`Olympiad.ini › OlympiadBattle` · number · ms

Olympiad Battle Period, Default 6 minutes.

- **Default:** `360000`
- **Allowed:** 0 – 9223372036854775807 ms

### Weekly point bonus every (OlympiadWPeriod)

`Olympiad.ini › OlympiadWPeriod` · number · ms

Olympiad Weekly Period, Default 1 week Used for adding points to nobles

- **Default:** `604800000`
- **Allowed:** 0 – 9223372036854775807 ms

### Validation period length (OlympiadVPeriod)

`Olympiad.ini › OlympiadVPeriod` · number · ms

Olympiad Validation Period, Default 24 Hours.

- **Default:** `86400000`
- **Allowed:** 0 – 9223372036854775807 ms

### Starting Olympiad points (OlympiadStartPoints)

`Olympiad.ini › OlympiadStartPoints` · number

Points for reaching Noblesse for the first time

- **Default:** `18`
- **Allowed:** 0 – 2147483647

### Points added each week (OlympiadWeeklyPoints)

`Olympiad.ini › OlympiadWeeklyPoints` · number

Points every week

- **Default:** `3`
- **Allowed:** 0 – 2147483647

### Players needed for class matches (OlympiadClassedParticipants)

`Olympiad.ini › OlympiadClassedParticipants` · number

Required number of participants for Classed and Non-Classed matches, Default 5 & 9

- **Default:** `5`
- **Allowed:** 0 – 2147483647

### Players needed for open matches (OlympiadNonClassedParticipants)

`Olympiad.ini › OlympiadNonClassedParticipants` · number

- **Default:** `9`
- **Allowed:** 0 – 2147483647

### Registration count shown to players (OlympiadRegistrationDisplayNumber)

`Olympiad.ini › OlympiadRegistrationDisplayNumber` · number

Displaying for registered participants, messages in old style (digits) or new style (phrases). Set 0 for displaying digits instead of text phrases (old style). Set 100 for displaying "Fewer than ..." or "More than ...".

- **Default:** `0`
- **Allowed:** 0 – 2147483647

### Match reward item ID (OlympiadBattleRewItem)

`Olympiad.ini › OlympiadBattleRewItem` · number

ItemId used for reward battle winner for class and non-class games.

- **Default:** `6651`
- **Allowed:** 0 – 2147483647

### Class match reward amount (OlympiadClassedRewItemCount)

`Olympiad.ini › OlympiadClassedRewItemCount` · number

- **Default:** `50`
- **Allowed:** 0 – 2147483647

### Open match reward amount (OlympiadNonClassedRewItemCount)

`Olympiad.ini › OlympiadNonClassedRewItemCount` · number

- **Default:** `30`
- **Allowed:** 0 – 2147483647

### Point exchange reward item ID (OlympiadCompRewItem)

`Olympiad.ini › OlympiadCompRewItem` · number

ItemId used for exchanging points for Noblesse Gate Passes.

- **Default:** `6651`
- **Allowed:** 0 – 2147483647

### Reward items per Olympiad point (OlympiadGPPerPoint)

`Olympiad.ini › OlympiadGPPerPoint` · number

Rate to exchange points to reward item.

- **Default:** `1000`
- **Allowed:** 0 – 2147483647

### Bonus points for heroes (OlympiadHeroPoints)

`Olympiad.ini › OlympiadHeroPoints` · number

Noblesse points awarded to Heroes.

- **Default:** `100`
- **Allowed:** 0 – 2147483647

### Most points a player can gain (OlympiadMaxPoints)

`Olympiad.ini › OlympiadMaxPoints` · number

Maximum points that player can gain/lose on a match.

- **Default:** `10`
- **Allowed:** 0 – 2147483647

### Show monthly winners in the ranking (OlympiadShowMonthlyWinners)

`Olympiad.ini › OlympiadShowMonthlyWinners` · on/off

Hero tables show last month's winners or current status.

- **Default:** On
- **Allowed:** On or Off

### Announce matches (OlympiadAnnounceGames)

`Olympiad.ini › OlympiadAnnounceGames` · on/off

Olympiad Managers announce each start of fight.

- **Default:** On
- **Allowed:** On or Off

### Items not allowed in the Olympiad (OlympiadRestrictedItems)

`Olympiad.ini › OlympiadRestrictedItems` · list

Restrict specified items in Olympiad. ItemID's need to be separated with a comma (ex. 1,200,350) Equipped items will be moved to inventory during port.

- **Default:** (empty)
- **Allowed:** numbers separated by commas, e.g. `57,4037`

### Block Blessed Spiritshots in the Olympiad (OlympiadDisableBlessedSpiritShots)

`Olympiad.ini › OlympiadDisableBlessedSpiritShots` · on/off

Disable Blessed SpiritShots. Interlude Default: True

- **Default:** On
- **Allowed:** On or Off

### Enchant level limit in the Olympiad (OlympiadEnchantLimit)

`Olympiad.ini › OlympiadEnchantLimit` · number

-1 = no limit.

- **Default:** `-1`
- **Allowed:** -2147483648 – 2147483647

### Log Olympiad fights (OlympiadLogFights)

`Olympiad.ini › OlympiadLogFights` · on/off

Log all Olympiad fights and outcome to olympiad.csv file.

- **Default:** Off
- **Allowed:** On or Off

### Wait before a match starts (OlympiadWaitTime)

`Olympiad.ini › OlympiadWaitTime` · number · seconds

Time to wait before teleported to arena. Possible choices are: 120, 60, 30, 15, or 5. If any other number is entered, the time will be set to 120 seconds.

- **Default:** `120`
- **Allowed:** 0 – 2147483647 seconds

## Custom period

### Use a custom Olympiad period (OlympiadUseCustomPeriodSettings)

`Olympiad.ini › OlympiadUseCustomPeriodSettings` · on/off

Custom Olympiad period settings Example for Olympiad every 2 weeks: OlympiadUseCustomPeriodSettings = True OlympiadPeriod = WEEK OlympiadPeriodMultiplier = 2 Enable/disable custom period settings.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Custom period unit](Settings-Olympiad-Heroes#custom-period-unit-olympiadperiod) — it only works while this is On
- **Controls:** [Custom period length](Settings-Olympiad-Heroes#custom-period-length-olympiadperiodmultiplier) — it only works while this is On
- **Controls:** [Competition days](Settings-Olympiad-Heroes#competition-days-olympiadcompetitiondays) — it only works while this is On

### Custom period unit (OlympiadPeriod)

`Olympiad.ini › OlympiadPeriod` · choice

Change the type of delay between two Olympiads. Available values: MONTH, WEEK, DAY

- **Default:** `MONTH` (Month)
- **Allowed:** one of `MONTH` (Month), `WEEK` (Week), `DAY` (Day)
- **Needs:** [Use a custom Olympiad period](Settings-Olympiad-Heroes#use-a-custom-olympiad-period-olympiadusecustomperiodsettings) to be On — The custom period is off.

### Custom period length (OlympiadPeriodMultiplier)

`Olympiad.ini › OlympiadPeriodMultiplier` · number

Change the Olympiad frequency. The value is a multiplier of period type, i.e. if type is MONTH and multiplier is 2, then Olympiad will occur every 2 months. Note! If type = DAY, multiplier must be >= 7!

- **Default:** `1`
- **Allowed:** 0 – 2147483647
- **Needs:** [Use a custom Olympiad period](Settings-Olympiad-Heroes#use-a-custom-olympiad-period-olympiadusecustomperiodsettings) to be On — The custom period is off.

### Competition days (OlympiadCompetitionDays)

`Olympiad.ini › OlympiadCompetitionDays` · list

1 = Sunday … 7 = Saturday.

- **Default:** `1,2,3,4,5,6,7`
- **Allowed:** day numbers 1–7 separated by commas (1 = Sunday)
- **Needs:** [Use a custom Olympiad period](Settings-Olympiad-Heroes#use-a-custom-olympiad-period-olympiadusecustomperiodsettings) to be On — The custom period is off.
