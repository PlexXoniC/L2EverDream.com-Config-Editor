# Convenience Features settings

Optional helpers: auto-play, auto-potions, free mounts and service NPCs.

**82 settings** in the **Convenience Features** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Auto play

### Enable auto play (EnableAutoPlay)

`Custom/AutoPlay.ini › EnableAutoPlay` · on/off

Enable player .play voiced command.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Auto play uses potions](Settings-Convenience-Features#auto-play-uses-potions-enableautopotion) — it only works while this is On
- **Controls:** [Auto play uses skills](Settings-Convenience-Features#auto-play-uses-skills-enableautoskill) — it only works while this is On
- **Controls:** [Auto play uses items](Settings-Convenience-Features#auto-play-uses-items-enableautoitem) — it only works while this is On
- **Controls:** [Resume auto play after logging back in](Settings-Convenience-Features#resume-auto-play-after-logging-back-in-resumeautoplay) — it only works while this is On
- **Controls:** [Auto play assists the party leader](Settings-Convenience-Features#auto-play-assists-the-party-leader-assistleader) — it only works while this is On
- **Controls:** [Auto play short range](Settings-Convenience-Features#auto-play-short-range-shortrange) — it only works while this is On
- **Controls:** [Auto play long range](Settings-Convenience-Features#auto-play-long-range-longrange) — it only works while this is On
- **Controls:** [Auto play is premium-only](Settings-Convenience-Features#auto-play-is-premium-only-autoplaypremium) — it only works while this is On
- **Controls:** [Skills auto play never uses](Settings-Convenience-Features#skills-auto-play-never-uses-disabledskillids) — it only works while this is On
- **Controls:** [Items auto play never uses](Settings-Convenience-Features#items-auto-play-never-uses-disableditemids) — it only works while this is On
- **Controls:** [Items auto play never picks up](Settings-Convenience-Features#items-auto-play-never-picks-up-ignoredautopickitems) — it only works while this is On
- **Controls:** [Auto play login message](Settings-Convenience-Features#auto-play-login-message-autoplayloginmessage) — it only works while this is On

### Auto play uses potions (EnableAutoPotion)

`Custom/AutoPlay.ini › EnableAutoPotion` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Auto play uses skills (EnableAutoSkill)

`Custom/AutoPlay.ini › EnableAutoSkill` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Auto play uses items (EnableAutoItem)

`Custom/AutoPlay.ini › EnableAutoItem` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Resume auto play after logging back in (ResumeAutoPlay)

`Custom/AutoPlay.ini › ResumeAutoPlay` · on/off

Resume auto play upon enter game.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Auto play assists the party leader (AssistLeader)

`Custom/AutoPlay.ini › AssistLeader` · on/off

Assist party leader. When in party, target what the leader is targeting.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Auto play short range (ShortRange)

`Custom/AutoPlay.ini › ShortRange` · number

Range Targeting.

- **Default:** `600`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Auto play long range (LongRange)

`Custom/AutoPlay.ini › LongRange` · number

- **Default:** `1400`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Auto play is premium-only (AutoPlayPremium)

`Custom/AutoPlay.ini › AutoPlayPremium` · on/off

Enable .play command only for premium players. Premium System must be enabled.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Skills auto play never uses (DisabledSkillIds)

`Custom/AutoPlay.ini › DisabledSkillIds` · list

List of skills that cannot be added to auto play selection menu, separated by commas.

- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Items auto play never uses (DisabledItemIds)

`Custom/AutoPlay.ini › DisabledItemIds` · list

List of items that cannot be added to auto play selection menu, separated by commas.

- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Items auto play never picks up (IgnoredAutoPickItems)

`Custom/AutoPlay.ini › IgnoredAutoPickItems` · list

Items ignored from auto play pickup. Cursed Weapons: 8190, 8689

- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

### Auto play login message (AutoPlayLoginMessage)

`Custom/AutoPlay.ini › AutoPlayLoginMessage` · text

Message send to players on login. Leave empty to disable. Example: You can use .play to automate farming.

- **Needs:** [Enable auto play](Settings-Convenience-Features#enable-auto-play-enableautoplay) to be On

## Auto potions

### Enable auto potions (.apon) (AutoPotionsEnabled)

`Custom/AutoPotions.ini › AutoPotionsEnabled` · on/off

Enable auto potion commands.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Allow auto potions in the Olympiad](Settings-Convenience-Features#allow-auto-potions-in-the-olympiad-autopotionsinolympiad) — it only works while this is On
- **Controls:** [Level needed for auto potions](Settings-Convenience-Features#level-needed-for-auto-potions-autopotionminimumlevel) — it only works while this is On
- **Controls:** [Auto CP potions](Settings-Convenience-Features#auto-cp-potions-autocpenabled) — it only works while this is On
- **Controls:** [Auto HP potions](Settings-Convenience-Features#auto-hp-potions-autohpenabled) — it only works while this is On
- **Controls:** [Auto MP potions](Settings-Convenience-Features#auto-mp-potions-autompenabled) — it only works while this is On

### Allow auto potions in the Olympiad (AutoPotionsInOlympiad)

`Custom/AutoPotions.ini › AutoPotionsInOlympiad` · on/off

Use auto potions in Olympiad.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable auto potions (.apon)](Settings-Convenience-Features#enable-auto-potions-apon-autopotionsenabled) to be On

### Level needed for auto potions (AutoPotionMinimumLevel)

`Custom/AutoPotions.ini › AutoPotionMinimumLevel` · number

Minimum player level to use the commands.

- **Default:** `1`
- **Allowed:** 1 – 80
- **Needs:** [Enable auto potions (.apon)](Settings-Convenience-Features#enable-auto-potions-apon-autopotionsenabled) to be On

### Auto CP potions (AutoCpEnabled)

`Custom/AutoPotions.ini › AutoCpEnabled` · on/off

Enable auto CP potions.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable auto potions (.apon)](Settings-Convenience-Features#enable-auto-potions-apon-autopotionsenabled) to be On
- **Controls:** [Use CP potion below](Settings-Convenience-Features#use-cp-potion-below-autocppercentage) — it only works while this is On
- **Controls:** [CP potion item IDs](Settings-Convenience-Features#cp-potion-item-ids-autocpitemids) — it only works while this is On

### Use CP potion below (AutoCpPercentage)

`Custom/AutoPotions.ini › AutoCpPercentage` · number · %

Percentage that CP potions will be used.

- **Default:** `70`
- **Allowed:** 0 – 100 %
- **Needs:** [Auto CP potions](Settings-Convenience-Features#auto-cp-potions-autocpenabled) to be On — Auto CP potions are off.

### CP potion item IDs (AutoCpItemIds)

`Custom/AutoPotions.ini › AutoCpItemIds` · list

Auto CP item ids. Order by use priority.

- **Default:** `0`
- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Auto CP potions](Settings-Convenience-Features#auto-cp-potions-autocpenabled) to be On — Auto CP potions are off.

### Auto HP potions (AutoHpEnabled)

`Custom/AutoPotions.ini › AutoHpEnabled` · on/off

Enable auto HP potions.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable auto potions (.apon)](Settings-Convenience-Features#enable-auto-potions-apon-autopotionsenabled) to be On
- **Controls:** [Use HP potion below](Settings-Convenience-Features#use-hp-potion-below-autohppercentage) — it only works while this is On
- **Controls:** [HP potion item IDs](Settings-Convenience-Features#hp-potion-item-ids-autohpitemids) — it only works while this is On

### Use HP potion below (AutoHpPercentage)

`Custom/AutoPotions.ini › AutoHpPercentage` · number · %

Percentage that HP potions will be used.

- **Default:** `70`
- **Allowed:** 0 – 100 %
- **Needs:** [Auto HP potions](Settings-Convenience-Features#auto-hp-potions-autohpenabled) to be On — Auto HP potions are off.

### HP potion item IDs (AutoHpItemIds)

`Custom/AutoPotions.ini › AutoHpItemIds` · list

Auto HP item ids. Order by use priority.

- **Default:** `0`
- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Auto HP potions](Settings-Convenience-Features#auto-hp-potions-autohpenabled) to be On — Auto HP potions are off.

### Auto MP potions (AutoMpEnabled)

`Custom/AutoPotions.ini › AutoMpEnabled` · on/off

Enable auto MP potions.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable auto potions (.apon)](Settings-Convenience-Features#enable-auto-potions-apon-autopotionsenabled) to be On
- **Controls:** [Use MP potion below](Settings-Convenience-Features#use-mp-potion-below-automppercentage) — it only works while this is On
- **Controls:** [MP potion item IDs](Settings-Convenience-Features#mp-potion-item-ids-autompitemids) — it only works while this is On

### Use MP potion below (AutoMpPercentage)

`Custom/AutoPotions.ini › AutoMpPercentage` · number · %

Percentage that MP potions will be used.

- **Default:** `70`
- **Allowed:** 0 – 100 %
- **Needs:** [Auto MP potions](Settings-Convenience-Features#auto-mp-potions-autompenabled) to be On — Auto MP potions are off.

### MP potion item IDs (AutoMpItemIds)

`Custom/AutoPotions.ini › AutoMpItemIds` · text

Auto MP item ids. Order by use priority.

- **Default:** `0`
- **Allowed:** numbers separated by commas, e.g. `57,4037`
- **Needs:** [Auto MP potions](Settings-Convenience-Features#auto-mp-potions-autompenabled) to be On — Auto MP potions are off.

## Offline auto play

### Enable offline auto play (.offlineplay) (EnableOfflinePlayCommand)

`Custom/OfflinePlay.ini › EnableOfflinePlayCommand` · on/off

Enable .offlineplay command for logging out and continue auto play.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Resume offline auto play after a restart](Settings-Convenience-Features#resume-offline-auto-play-after-a-restart-restoreautoplayoffliners) — it only works while this is On
- **Controls:** [Offline auto play is premium-only](Settings-Convenience-Features#offline-auto-play-is-premium-only-offlineplaypremium) — it only works while this is On
- **Controls:** [Log offline characters out when they die](Settings-Convenience-Features#log-offline-characters-out-when-they-die-offlineplaylogoutondeath) — it only works while this is On
- **Controls:** [Stop offline play when the account logs in](Settings-Convenience-Features#stop-offline-play-when-the-account-logs-in-offlineplaydisconnectsameaccount) — it only works while this is On
- **Controls:** [Offline auto play login message](Settings-Convenience-Features#offline-auto-play-login-message-offlineplayloginmessage) — it only works while this is On
- **Controls:** [Color the names of offline players](Settings-Convenience-Features#color-the-names-of-offline-players-offlineplaysetnamecolor) — it only works while this is On
- **Controls:** [Visual effect on offline players](Settings-Convenience-Features#visual-effect-on-offline-players-offlineplayabnormaleffect) — it only works while this is On

### Resume offline auto play after a restart (RestoreAutoPlayOffliners)

`Custom/OfflinePlay.ini › RestoreAutoPlayOffliners` · on/off

Store offline play players to database. This allows offline players to be restored after server restart.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable offline auto play (.offlineplay)](Settings-Convenience-Features#enable-offline-auto-play-offlineplay-enableofflineplaycommand) to be On

### Offline auto play is premium-only (OfflinePlayPremium)

`Custom/OfflinePlay.ini › OfflinePlayPremium` · on/off

Enable .offlineplay command only for premium players. Premium System must be enabled.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable offline auto play (.offlineplay)](Settings-Convenience-Features#enable-offline-auto-play-offlineplay-enableofflineplaycommand) to be On

### Log offline characters out when they die (OfflinePlayLogoutOnDeath)

`Custom/OfflinePlay.ini › OfflinePlayLogoutOnDeath` · on/off

Logout player on death.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable offline auto play (.offlineplay)](Settings-Convenience-Features#enable-offline-auto-play-offlineplay-enableofflineplaycommand) to be On

### Stop offline play when the account logs in (OfflinePlayDisconnectSameAccount)

`Custom/OfflinePlay.ini › OfflinePlayDisconnectSameAccount` · on/off

Disconnect when player from same account logins to the game.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable offline auto play (.offlineplay)](Settings-Convenience-Features#enable-offline-auto-play-offlineplay-enableofflineplaycommand) to be On

### Offline auto play login message (OfflinePlayLoginMessage)

`Custom/OfflinePlay.ini › OfflinePlayLoginMessage` · text

Message send to players on login. Leave empty to disable. Example: You can use .offlineplay to logout and continue auto play.

- **Needs:** [Enable offline auto play (.offlineplay)](Settings-Convenience-Features#enable-offline-auto-play-offlineplay-enableofflineplaycommand) to be On

### Color the names of offline players (OfflinePlaySetNameColor)

`Custom/OfflinePlay.ini › OfflinePlaySetNameColor` · on/off

If set to True, name color will be changed then entering offline play mode.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable offline auto play (.offlineplay)](Settings-Convenience-Features#enable-offline-auto-play-offlineplay-enableofflineplaycommand) to be On
- **Controls:** [Offline player name color](Settings-Convenience-Features#offline-player-name-color-offlineplaynamecolor) — it only works while this is On

### Offline player name color (OfflinePlayNameColor)

`Custom/OfflinePlay.ini › OfflinePlayNameColor` · text

Hex color, e.g. 808080.

- **Default:** `808080`
- **Allowed:** a 6-digit colour code, e.g. `00FF00`
- **Needs:** [Color the names of offline players](Settings-Convenience-Features#color-the-names-of-offline-players-offlineplaysetnamecolor) to be On — Name coloring for offline players is off.

### Visual effect on offline players (OfflinePlayAbnormalEffect)

`Custom/OfflinePlay.ini › OfflinePlayAbnormalEffect` · list

Abnormal effect for offline play. Can use multiple enums separated by commas to choose random effect. Leave empty to disable. Example: SLEEP

- **Needs:** [Enable offline auto play (.offlineplay)](Settings-Convenience-Features#enable-offline-auto-play-offlineplay-enableofflineplaycommand) to be On

## Premium & PC Café points

### Enable premium accounts (EnablePremiumSystem)

`Custom/PremiumSystem.ini › EnablePremiumSystem` · on/off

Enable premium system.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Enable PC Café points](Settings-Convenience-Features#enable-pc-caf-points-pccafeenabled) — it only works while this is On
- **Controls:** [Retail-like point amounts](Settings-Convenience-Features#retail-like-point-amounts-acquisitionpointsretaillikepoints) — it only works while this is On
- **Controls:** [Points gained](Settings-Convenience-Features#points-gained-acquisitionpointsrate) — it only works while this is On
- **Controls:** [Random point amounts](Settings-Convenience-Features#random-point-amounts-acquisitionpointsrandom) — it only works while this is On
- **Controls:** [Chance to double points](Settings-Convenience-Features#chance-to-double-points-doublingacquisitionpoints) — it only works while this is On
- **Controls:** [Points for low-experience kills](Settings-Convenience-Features#points-for-low-experience-kills-rewardlowexpkills) — it only works while this is On
- **Controls:** [Premium bonus: xp rate](Settings-Convenience-Features#premium-bonus-xp-rate-premiumratexp) — it only works while this is On
- **Controls:** [Premium bonus: sp rate](Settings-Convenience-Features#premium-bonus-sp-rate-premiumratesp) — it only works while this is On
- **Controls:** [Premium bonus: drop chance rate](Settings-Convenience-Features#premium-bonus-drop-chance-rate-premiumratedropchance) — it only works while this is On
- **Controls:** [Premium bonus: drop amount rate](Settings-Convenience-Features#premium-bonus-drop-amount-rate-premiumratedropamount) — it only works while this is On
- **Controls:** [Premium bonus: spoil chance rate](Settings-Convenience-Features#premium-bonus-spoil-chance-rate-premiumratespoilchance) — it only works while this is On
- **Controls:** [Premium bonus: spoil amount rate](Settings-Convenience-Features#premium-bonus-spoil-amount-rate-premiumratespoilamount) — it only works while this is On
- **Controls:** [Premium bonus: quest xp rate](Settings-Convenience-Features#premium-bonus-quest-xp-rate-premiumratequestxp) — it only works while this is On
- **Controls:** [Premium bonus: quest sp rate](Settings-Convenience-Features#premium-bonus-quest-sp-rate-premiumratequestsp) — it only works while this is On
- **Controls:** [Premium bonus: drop chance by item id rate](Settings-Convenience-Features#premium-bonus-drop-chance-by-item-id-rate-premiumratedropchancebyitemid) — it only works while this is On
- **Controls:** [Premium bonus: drop amount by item id rate](Settings-Convenience-Features#premium-bonus-drop-amount-by-item-id-rate-premiumratedropamountbyitemid) — it only works while this is On

### Enable PC Café points (PcCafeEnabled)

`Custom/PremiumSystem.ini › PcCafeEnabled` · on/off

The client also needs UsePCBangPoint=true in l2.ini.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On
- **Controls:** [Retail-like PC Café points](Settings-Convenience-Features#retail-like-pc-caf-points-pccaferetaillike) — it only works while this is On
- **Controls:** [PC Café points every](Settings-Convenience-Features#pc-caf-points-every-pccaferewardtime) — it only works while this is On
- **Controls:** [PC Café points for premium only](Settings-Convenience-Features#pc-caf-points-for-premium-only-pccafeonlypremium) — it only works while this is On
- **Controls:** [Most PC Café points](Settings-Convenience-Features#most-pc-caf-points-maxpccafepoints) — it only works while this is On

### Retail-like PC Café points (PcCafeRetailLike)

`Custom/PremiumSystem.ini › PcCafeRetailLike` · on/off

Retail accuisition of PC points(10 PA points are gained for each 5 minutes of being online)

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable PC Café points](Settings-Convenience-Features#enable-pc-caf-points-pccafeenabled) to be On — PC Café points are off.

### PC Café points every (PcCafeRewardTime)

`Custom/PremiumSystem.ini › PcCafeRewardTime` · number · ms

PC Cafe point reward time.

- **Default:** `300000`
- **Allowed:** 0 – 2147483647 ms
- **Needs:** [Enable PC Café points](Settings-Convenience-Features#enable-pc-caf-points-pccafeenabled) to be On — PC Café points are off.

### PC Café points for premium only (PcCafeOnlyPremium)

`Custom/PremiumSystem.ini › PcCafeOnlyPremium` · on/off

Allow only players with a Premium account.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable PC Café points](Settings-Convenience-Features#enable-pc-caf-points-pccafeenabled) to be On — PC Café points are off.

### Most PC Café points (MaxPcCafePoints)

`Custom/PremiumSystem.ini › MaxPcCafePoints` · number

Max points that player may have. Limited by int limit.

- **Default:** `200000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable PC Café points](Settings-Convenience-Features#enable-pc-caf-points-pccafeenabled) to be On — PC Café points are off.

### Retail-like point amounts (AcquisitionPointsRetailLikePoints)

`Custom/PremiumSystem.ini › AcquisitionPointsRetailLikePoints` · number

PC Ban points acquisition if it's retail like

- **Default:** `10`
- **Allowed:** 0 – 2147483647
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Points gained (AcquisitionPointsRate)

`Custom/PremiumSystem.ini › AcquisitionPointsRate` · number · times

PC Bang point rate if it's not retail like. Acquisition formula equals (exp * 0.0001 * AcquisitionPointsRate) e.g. with 1.0 it's 10000 exp = 1 PC Bang point 2.0 - 10000 exp = 2 PC Bang points 0.5 - 5000  exp = 1 PC Bang point

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Random point amounts (AcquisitionPointsRandom)

`Custom/PremiumSystem.ini › AcquisitionPointsRandom` · on/off

Use random points rewarding. If enabled points will be random from points/2 to points.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Chance to double points (DoublingAcquisitionPoints)

`Custom/PremiumSystem.ini › DoublingAcquisitionPoints` · on/off

Creates a chance to aquire double points.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On
- **Controls:** [Double points chance](Settings-Convenience-Features#double-points-chance-doublingacquisitionpointschance) — it only works while this is On

### Double points chance (DoublingAcquisitionPointsChance)

`Custom/PremiumSystem.ini › DoublingAcquisitionPointsChance` · number · %

Double points chance. Used when DoublingAcquisitionPoints is enabled. Default 1 (%)

- **Default:** `1`
- **Allowed:** 0 – 100 %
- **Needs:** [Chance to double points](Settings-Convenience-Features#chance-to-double-points-doublingacquisitionpoints) to be On — Doubling points is off.

### Points for low-experience kills (RewardLowExpKills)

`Custom/PremiumSystem.ini › RewardLowExpKills` · on/off

Reward low exp kills Acquire points if player gains exp and aquire formula equals 0.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On
- **Controls:** [Points chance for low-experience kills](Settings-Convenience-Features#points-chance-for-low-experience-kills-rewardlowexpkillschance) — it only works while this is On

### Points chance for low-experience kills (RewardLowExpKillsChance)

`Custom/PremiumSystem.ini › RewardLowExpKillsChance` · number · %

Chance for low exp kills Used when RewardLowExpKills is enabled. Default 50 (%)

- **Default:** `50`
- **Allowed:** 0 – 100 %
- **Needs:** [Points for low-experience kills](Settings-Convenience-Features#points-for-low-experience-kills-rewardlowexpkills) to be On — Points for low-experience kills are off.

### Premium bonus: xp rate (PremiumRateXp)

`Custom/PremiumSystem.ini › PremiumRateXp` · number · times

Xp rate for premium players.

- **Default:** `2`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: sp rate (PremiumRateSp)

`Custom/PremiumSystem.ini › PremiumRateSp` · number · times

Sp rate for premium players.

- **Default:** `2`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: drop chance rate (PremiumRateDropChance)

`Custom/PremiumSystem.ini › PremiumRateDropChance` · number · times

Drop chance for premium players.

- **Default:** `2`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: drop amount rate (PremiumRateDropAmount)

`Custom/PremiumSystem.ini › PremiumRateDropAmount` · number · times

Drop amount for premium players.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: spoil chance rate (PremiumRateSpoilChance)

`Custom/PremiumSystem.ini › PremiumRateSpoilChance` · number · times

Spoil chance for premium players.

- **Default:** `2`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: spoil amount rate (PremiumRateSpoilAmount)

`Custom/PremiumSystem.ini › PremiumRateSpoilAmount` · number · times

Spoil amount for premium players.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: quest xp rate (PremiumRateQuestXp)

`Custom/PremiumSystem.ini › PremiumRateQuestXp` · number · times

Quest Xp rate for premium players.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: quest sp rate (PremiumRateQuestSp)

`Custom/PremiumSystem.ini › PremiumRateQuestSp` · number · times

Quest Sp rate for premium players.

- **Default:** `1`
- **Allowed:** 0 or more times
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: drop chance by item id rate (PremiumRateDropChanceByItemId)

`Custom/PremiumSystem.ini › PremiumRateDropChanceByItemId` · list

Caution: Raid bosses and herbs are not affected by premium rates, but specific items can be affected by rates bellow. List of items affected by custom drop rate by id, used now for Adena rate too. Usage: itemId1,multiplier1;itemId2,multiplier2;...

- **Allowed:** `id,value` pairs separated by semicolons, e.g. `57,2;4037,1.5`
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

### Premium bonus: drop amount by item id rate (PremiumRateDropAmountByItemId)

`Custom/PremiumSystem.ini › PremiumRateDropAmountByItemId` · list

- **Allowed:** `id,value` pairs separated by semicolons, e.g. `57,2;4037,1.5`
- **Needs:** [Enable premium accounts](Settings-Convenience-Features#enable-premium-accounts-enablepremiumsystem) to be On

## Service NPCs

### Delevel NPC: enabled (Enabled)

`Custom/DelevelManager.ini › Enabled` · on/off

Enable Delevel Manager NPC.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Delevel NPC: NPC ID](Settings-Convenience-Features#delevel-npc-npc-id-npcid) — it only works while this is On
- **Controls:** [Delevel NPC: price item ID](Settings-Convenience-Features#delevel-npc-price-item-id-requireditemid) — it only works while this is On
- **Controls:** [Delevel NPC: price amount](Settings-Convenience-Features#delevel-npc-price-amount-requireditemcount) — it only works while this is On
- **Controls:** [Delevel NPC: lowest level](Settings-Convenience-Features#delevel-npc-lowest-level-mimimumdelevel) — it only works while this is On

### Delevel NPC: NPC ID (NpcId)

`Custom/DelevelManager.ini › NpcId` · number

Delevel Manager NPC id.

- **Default:** `1002000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Delevel NPC: enabled](Settings-Convenience-Features#delevel-npc-enabled-enabled) to be On

### Delevel NPC: price item ID (RequiredItemId)

`Custom/DelevelManager.ini › RequiredItemId` · number

Required item id.

- **Default:** `4356`
- **Allowed:** 0 – 2147483647
- **Needs:** [Delevel NPC: enabled](Settings-Convenience-Features#delevel-npc-enabled-enabled) to be On

### Delevel NPC: price amount (RequiredItemCount)

`Custom/DelevelManager.ini › RequiredItemCount` · number

Required item count.

- **Default:** `2`
- **Allowed:** 0 – 2147483647
- **Needs:** [Delevel NPC: enabled](Settings-Convenience-Features#delevel-npc-enabled-enabled) to be On

### Delevel NPC: lowest level (MimimumDelevel)

`Custom/DelevelManager.ini › MimimumDelevel` · number

Mimimum level you can reach.

- **Default:** `20`
- **Allowed:** 0 – 2147483647
- **Needs:** [Delevel NPC: enabled](Settings-Convenience-Features#delevel-npc-enabled-enabled) to be On

### Noblesse NPC: enabled (Enabled)

`Custom/NoblessMaster.ini › Enabled` · on/off

Enable or disable the Nobless Master NPC.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Noblesse NPC: NPC ID](Settings-Convenience-Features#noblesse-npc-npc-id-npcid) — it only works while this is On
- **Controls:** [Noblesse NPC: level required](Settings-Convenience-Features#noblesse-npc-level-required-levelrequirement) — it only works while this is On
- **Controls:** [Noblesse NPC: price item ID](Settings-Convenience-Features#noblesse-npc-price-item-id-itemid) — it only works while this is On
- **Controls:** [Noblesse NPC: price amount](Settings-Convenience-Features#noblesse-npc-price-amount-itemcount) — it only works while this is On
- **Controls:** [Noblesse NPC: also give the Noblesse Tiara](Settings-Convenience-Features#noblesse-npc-also-give-the-noblesse-tiara-rewardtiara) — it only works while this is On

### Noblesse NPC: NPC ID (NpcId)

`Custom/NoblessMaster.ini › NpcId` · number

The ID of the Nobless Master NPC.

- **Default:** `1003000`
- **Allowed:** 0 – 2147483647
- **Needs:** [Noblesse NPC: enabled](Settings-Convenience-Features#noblesse-npc-enabled-enabled) to be On

### Noblesse NPC: level required (LevelRequirement)

`Custom/NoblessMaster.ini › LevelRequirement` · number

Level requirement to become Nobless. Note: If you modify this, ensure the corresponding HTMLs are updated.

- **Default:** `80`
- **Allowed:** 0 – 2147483647
- **Needs:** [Noblesse NPC: enabled](Settings-Convenience-Features#noblesse-npc-enabled-enabled) to be On

### Noblesse NPC: price item ID (ItemId)

`Custom/NoblessMaster.ini › ItemId` · number

The item ID required to become Nobless (e.g., Adena).

- **Default:** `57`
- **Allowed:** 0 – 2147483647
- **Needs:** [Noblesse NPC: enabled](Settings-Convenience-Features#noblesse-npc-enabled-enabled) to be On

### Noblesse NPC: price amount (ItemCount)

`Custom/NoblessMaster.ini › ItemCount` · number

The quantity of the required item. Set to 0 to disable the item requirement.

- **Default:** `0`
- **Allowed:** 0 – 2147483647
- **Needs:** [Noblesse NPC: enabled](Settings-Convenience-Features#noblesse-npc-enabled-enabled) to be On

### Noblesse NPC: also give the Noblesse Tiara (RewardTiara)

`Custom/NoblessMaster.ini › RewardTiara` · on/off

Whether to reward the Nobless Tiara upon becoming Nobless.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Noblesse NPC: enabled](Settings-Convenience-Features#noblesse-npc-enabled-enabled) to be On

## Free mounts

### Free strider for everyone (EnableFreeStrider)

`Custom/FreeMounts.ini › EnableFreeStrider` · on/off

Enable free strider mount.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Free wyvern for everyone](Settings-Convenience-Features#free-wyvern-for-everyone-enablefreewyvern) — it only works while this is On

### Free wyvern for everyone (EnableFreeWyvern)

`Custom/FreeMounts.ini › EnableFreeWyvern` · on/off

Enable free wyvern mount.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Free strider for everyone](Settings-Convenience-Features#free-strider-for-everyone-enablefreestrider) to be On

## Mobius fake players

### Enable Mobius fake players (EnableFakePlayers)

`Custom/FakePlayers.ini › EnableFakePlayers` · on/off

These are Mobius's built-in fake players, separate from L2Everdream's sims.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Fake players chat](Settings-Convenience-Features#fake-players-chat-fakeplayerchat) — it only works while this is On
- **Controls:** [Fake players use soulshots](Settings-Convenience-Features#fake-players-use-soulshots-fakeplayeruseshots) — it only works while this is On
- **Controls:** [Killing fake players counts as PvP](Settings-Convenience-Features#killing-fake-players-counts-as-pvp-fakeplayerkillsrewardpvp) — it only works while this is On
- **Controls:** [Killing unflagged fake players gives karma](Settings-Convenience-Features#killing-unflagged-fake-players-gives-karma-fakeplayerunflaggedkillskarma) — it only works while this is On
- **Controls:** [Fake players can be attacked without Ctrl](Settings-Convenience-Features#fake-players-can-be-attacked-without-ctrl-fakeplayerautoattackable) — it only works while this is On
- **Controls:** [Fake players attack monsters](Settings-Convenience-Features#fake-players-attack-monsters-fakeplayeraggromonsters) — it only works while this is On
- **Controls:** [Fake players attack players](Settings-Convenience-Features#fake-players-attack-players-fakeplayeraggroplayers) — it only works while this is On
- **Controls:** [Fake players attack each other](Settings-Convenience-Features#fake-players-attack-each-other-fakeplayeraggrofpc) — it only works while this is On
- **Controls:** [Fake players drop items](Settings-Convenience-Features#fake-players-drop-items-fakeplayercandropitems) — it only works while this is On
- **Controls:** [Fake players pick up items](Settings-Convenience-Features#fake-players-pick-up-items-fakeplayercanpickup) — it only works while this is On

### Fake players chat (FakePlayerChat)

`Custom/FakePlayers.ini › FakePlayerChat` · on/off

Enable chatting with fake players.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Fake players use soulshots (FakePlayerUseShots)

`Custom/FakePlayers.ini › FakePlayerUseShots` · on/off

Enable shots usage for fake players.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Killing fake players counts as PvP (FakePlayerKillsRewardPvP)

`Custom/FakePlayers.ini › FakePlayerKillsRewardPvP` · on/off

Reward PvP kills by killing fake players.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Killing unflagged fake players gives karma (FakePlayerUnflaggedKillsKarma)

`Custom/FakePlayers.ini › FakePlayerUnflaggedKillsKarma` · on/off

Fake player kills apply karma rules.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Fake players can be attacked without Ctrl (FakePlayerAutoAttackable)

`Custom/FakePlayers.ini › FakePlayerAutoAttackable` · on/off

Fake players can be attacked without PvP flagging.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Fake players attack monsters (FakePlayerAggroMonsters)

`Custom/FakePlayers.ini › FakePlayerAggroMonsters` · on/off

Aggressive AI fake players attack nearby monsters.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Fake players attack players (FakePlayerAggroPlayers)

`Custom/FakePlayers.ini › FakePlayerAggroPlayers` · on/off

Aggressive AI fake players attack nearby players.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Fake players attack each other (FakePlayerAggroFPC)

`Custom/FakePlayers.ini › FakePlayerAggroFPC` · on/off

Aggressive AI fake players attack nearby fake players.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Fake players drop items (FakePlayerCanDropItems)

`Custom/FakePlayers.ini › FakePlayerCanDropItems` · on/off

Fake players can drop items when killing monsters.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

### Fake players pick up items (FakePlayerCanPickup)

`Custom/FakePlayers.ini › FakePlayerCanPickup` · on/off

Fake players can pickup dropped items.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Enable Mobius fake players](Settings-Convenience-Features#enable-mobius-fake-players-enablefakeplayers) to be On

## Account

### Allow .changepassword (AllowChangePassword)

`Custom/PasswordChange.ini › AllowChangePassword` · on/off

Enables .changepassword voiced command which allows the players to change their account's password ingame.

- **Default:** Off
- **Allowed:** On or Off
