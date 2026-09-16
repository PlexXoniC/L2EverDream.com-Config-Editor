# Skills & Combat settings

Skills, buffs, damage and class balance.

**80 settings** in the **Skills & Combat** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Skills & learning

### Skill enchant cost (NormalEnchantCostMultipiler)

`General.ini › NormalEnchantCostMultipiler` · number · times

Enchant Skill Details Settings

- **Default:** `1`
- **Allowed:** 0 – 2147483647 times

### Safe skill enchant cost (SafeEnchantCostMultipiler)

`General.ini › SafeEnchantCostMultipiler` · number · times

- **Default:** `5`
- **Allowed:** 0 – 2147483647 times

### Use custom skill durations (EnableModifySkillDuration)

`Player.ini › EnableModifySkillDuration` · on/off

Turns on the list of skills with their own duration below. Takes effect the next time the world starts.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Custom skill durations](Settings-Skills-Combat#custom-skill-durations-skilldurationlist) — it only works while this is On

### Custom skill durations (SkillDurationList)

`Player.ini › SkillDurationList` · skill list

Choose skills and how long their effect lasts, up to 12 hours. The server uses the new duration for everyone who casts the skill (you, simulated players and monsters). Takes effect the next time the world starts.

- **Allowed:** `skillId,seconds` pairs (edited on its own page)
- **Needs:** [Use custom skill durations](Settings-Skills-Combat#use-custom-skill-durations-enablemodifyskillduration) to be On — This list is ignored until custom skill durations are switched on.

### Use custom skill cooldowns (EnableModifySkillReuse)

`Player.ini › EnableModifySkillReuse` · on/off

When this is enabled it will read the "SkillReuseList" option.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Custom skill cooldowns](Settings-Skills-Combat#custom-skill-cooldowns-skillreuselist) — it only works while this is On

### Custom skill cooldowns (SkillReuseList)

`Player.ini › SkillReuseList` · list

Format: skillId,milliseconds;skillId,milliseconds.

- **Allowed:** `id,value` pairs separated by semicolons, e.g. `57,2;4037,1.5`
- **Needs:** [Use custom skill cooldowns](Settings-Skills-Combat#use-custom-skill-cooldowns-enablemodifyskillreuse) to be On — This list is ignored until custom skill cooldowns are switched on.

### Learn class skills automatically (AutoLearnSkills)

`Player.ini › AutoLearnSkills` · on/off

If it's true all class skills will be delivered upon level up and login.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Also auto-learn skills that normally need a book](Settings-Skills-Combat#also-auto-learn-skills-that-normally-need-a-book-autolearnskillswithoutitems) — it only works while this is On
- **Controls:** [Also auto-learn Forgotten Scroll skills](Settings-Skills-Combat#also-auto-learn-forgotten-scroll-skills-autolearnforgottenscrollskills) — it only works while this is On

### Also auto-learn skills that normally need a book (AutoLearnSkillsWithoutItems)

`Player.ini › AutoLearnSkillsWithoutItems` · on/off

Auto learn skills that need items to be learned. Forgotten Scroll skills have their own configuration.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Learn class skills automatically](Settings-Skills-Combat#learn-class-skills-automatically-autolearnskills) to be On — Only applies while skills are learned automatically.

### Also auto-learn Forgotten Scroll skills (AutoLearnForgottenScrollSkills)

`Player.ini › AutoLearnForgottenScrollSkills` · on/off

If it's true skills from forgotten scrolls will be delivered upon level up and login, require AutoLearnSkills.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Learn class skills automatically](Settings-Skills-Combat#learn-class-skills-automatically-autolearnskills) to be On — Only applies while skills are learned automatically.

### Clan skills require Life Crystals (LifeCrystalNeeded)

`Player.ini › LifeCrystalNeeded` · on/off

Class, Sub-class and skill learning options Require life crystal needed to learn clan skills.

- **Default:** On
- **Allowed:** On or Off

### Skill enchanting requires Giant's Codex books (EnchantSkillSpBookNeeded)

`Player.ini › EnchantSkillSpBookNeeded` · on/off

Require book needed to enchant skills.

- **Default:** On
- **Allowed:** On or Off

### Divine Inspiration requires its book (DivineInspirationSpBookNeeded)

`Player.ini › DivineInspirationSpBookNeeded` · on/off

Require spell book needed to learn Divine Inspiration.

- **Default:** On
- **Allowed:** On or Off

### Any class can learn any skill (at extra SP cost) (AltGameSkillLearn)

`Player.ini › AltGameSkillLearn` · on/off

When enabled, the following will be true: All classes can learn all skills. Skills of another class costs x2 SP to learn. Skills of another race costs x2 SP to learn. Skills of fighters/mages costs x3 SP to learn.

- **Default:** Off
- **Allowed:** On or Off

### Triggered PvP skills never hit unflagged players (AltValidateTriggerSkills)

`Player.ini › AltValidateTriggerSkills` · on/off

Enables alternative validation of triggering skills. When enabled pvp skills will not be casted on non flagged player. Sadly its non-retail

- **Default:** Off
- **Allowed:** On or Off

## Buffs & effects

### Resistances shorten debuffs (DebuffDurationUsesResists)

`General.ini › DebuffDurationUsesResists` · on/off

Affect debuff time by the character resistances. If the option is false, the debuff time will be the one set in the skill xml.

- **Default:** Off
- **Allowed:** On or Off

### Show buff messages in chat on login (ShowEffectMessagesOnLogin)

`Player.ini › ShowEffectMessagesOnLogin` · on/off

Show chat effect messages on login.

- **Default:** Off
- **Allowed:** On or Off

### Buff slots (MaxBuffAmount)

`Player.ini › MaxBuffAmount` · number

Divine Inspiration adds up to 4 more.

- **Default:** `20`
- **Allowed:** 1 – 40

### Song and dance slots (MaxDanceAmount)

`Player.ini › MaxDanceAmount` · number

- **Default:** `12`
- **Allowed:** 0 – 40

### Let players cancel songs/dances with Alt+click (DanceCancelBuff)

`Player.ini › DanceCancelBuff` · on/off

Allow players to cancel dances/songs via Alt+click on buff icon

- **Default:** Off
- **Allowed:** On or Off

### Songs and dances cost extra MP when stacked (DanceConsumeAdditionalMP)

`Player.ini › DanceConsumeAdditionalMP` · on/off

This option enables/disables additional MP consume for dances and songs.

- **Default:** On
- **Allowed:** On or Off

### Keep songs and dances after logging out (AltStoreDances)

`Player.ini › AltStoreDances` · on/off

Allow players to have all dances/songs stored when logout.

- **Default:** Off
- **Allowed:** On or Off

### Keep toggle skills after logging out (AltStoreToggles)

`Player.ini › AltStoreToggles` · on/off

Allow players to have toggle skills stored when logout.

- **Default:** Off
- **Allowed:** On or Off

### Learn Divine Inspiration automatically (AutoLearnDivineInspiration)

`Player.ini › AutoLearnDivineInspiration` · on/off

This option allows a player to automatically learn Divine Inspiration. This is not included in AutoLearnSkills above.

- **Default:** Off
- **Allowed:** On or Off

### Protection from monsters after standing up from Fake Death (PlayerFakeDeathUpProtection)

`Player.ini › PlayerFakeDeathUpProtection` · number · seconds

Protection from aggressive mobs after getting up from fake death. The value is specified in seconds.

- **Default:** `0`
- **Allowed:** 0 – 2147483647 seconds

### Keep buffs and cooldowns after logging out (StoreSkillCooltime)

`Player.ini › StoreSkillCooltime` · on/off

This option is to enable or disable the storage of buffs/debuffs among other effects.

- **Default:** On
- **Allowed:** On or Off

### Keep buffs and cooldowns when switching subclass (SubclassStoreSkillCooltime)

`Player.ini › SubclassStoreSkillCooltime` · on/off

This option is to enable or disable the storage of buffs/debuffs among other effects during a subclass change

- **Default:** Off
- **Allowed:** On or Off

### Over-time effect tick length (EffectTickRatio)

`Player.ini › EffectTickRatio` · number · ms

This is the value ticks are multiplied with to result in interval per tick in milliseconds. Note: Editing this will not affect how much the over-time effects heals since heal scales with that value too.

- **Default:** `666`
- **Allowed:** 0 – 9223372036854775807 ms

### Fake Death removes you from monsters' target (FakeDeathUntarget)

`Player.ini › FakeDeathUntarget` · on/off

Untarget player when uses fake death.

- **Default:** Off
- **Allowed:** On or Off

### Taking damage ends Fake Death (FakeDeathDamageStand)

`Player.ini › FakeDeathDamageStand` · on/off

Stand when fake death is active and taking damage.

- **Default:** On
- **Allowed:** On or Off

## Damage

### Getting hit interrupts (AltGameCancelByHit)

`Player.ini › AltGameCancelByHit` · choice

This is to allow a character to be canceled during bow use, skill use, or both. Available Options: bow, cast, all

- **Default:** `Cast`
- **Allowed:** one of `bow`, `cast`, `all`

### Spells can miss (MagicFailures)

`Player.ini › MagicFailures` · on/off

This option, if enabled, will allow magic to fail, and if disabled magic damage will always succeed with a 100% chance.

- **Default:** On
- **Allowed:** On or Off

### Alternative shield block formula (AltShieldBlocks)

`Player.ini › AltShieldBlocks` · on/off

These are alternative rules for shields. If True and they block: The damage is powerAtk-shieldDef, If False and they block: The damage is powerAtk / (shieldDef + powerDef)

- **Default:** Off
- **Allowed:** On or Off

### Perfect shield block chance (AltPerfectShieldBlockRate)

`Player.ini › AltPerfectShieldBlockRate` · number · %

This is the percentage for perfect shield block rate.

- **Default:** `10`
- **Allowed:** 0 – 100 %

### Debuff success uses the skill's magic level (CalculateMagicSuccessBySkillMagicLevel)

`Player.ini › CalculateMagicSuccessBySkillMagicLevel` · on/off

Calculate magic success by target level and skill magic level (when available). Otherwise target level and (alternatively) attacker level is used.

- **Default:** On
- **Allowed:** On or Off

### Bow damage depends on distance (DistanceBowDamage)

`Player.ini › DistanceBowDamage` · on/off

Distance damage calculation for bow. Bows Ranged Damage Formula. Full hit range is 500 which is the base bow range and the 60% of this is 800.

- **Default:** Off
- **Allowed:** On or Off

### Random damage variation on normal attacks (RandomizeAutoAttackDamage)

`Player.ini › RandomizeAutoAttackDamage` · on/off

Damage Randomization Applies the weapon's random multiplier to basic physical attacks.

- **Default:** On
- **Allowed:** On or Off

### Random damage variation on physical skills (RandomizePhysicalSkillDamage)

`Player.ini › RandomizePhysicalSkillDamage` · on/off

Applies the random weapon multiplier to physical skills (including blows/backstabs).

- **Default:** On
- **Allowed:** On or Off

### Random damage variation on magic skills (RandomizeMagicalSkillDamage)

`Player.ini › RandomizeMagicalSkillDamage` · on/off

Applies the random weapon multiplier to magic skills.

- **Default:** On
- **Allowed:** On or Off

## Class balance multipliers

### PvE: magical skill damage by class (PveMagicalSkillDamageMultipliers)

`Custom/ClassBalance.ini › PveMagicalSkillDamageMultipliers` · text · *Advanced*


### PvP: magical skill damage by class (PvpMagicalSkillDamageMultipliers)

`Custom/ClassBalance.ini › PvpMagicalSkillDamageMultipliers` · text · *Advanced*


### PvE: magical skill defence by class (PveMagicalSkillDefenceMultipliers)

`Custom/ClassBalance.ini › PveMagicalSkillDefenceMultipliers` · text · *Advanced*


### PvP: magical skill defence by class (PvpMagicalSkillDefenceMultipliers)

`Custom/ClassBalance.ini › PvpMagicalSkillDefenceMultipliers` · text · *Advanced*


### PvE: magical skill critical chance by class (PveMagicalSkillCriticalChanceMultipliers)

`Custom/ClassBalance.ini › PveMagicalSkillCriticalChanceMultipliers` · text · *Advanced*


### PvP: magical skill critical chance by class (PvpMagicalSkillCriticalChanceMultipliers)

`Custom/ClassBalance.ini › PvpMagicalSkillCriticalChanceMultipliers` · text · *Advanced*


### PvE: magical skill critical damage by class (PveMagicalSkillCriticalDamageMultipliers)

`Custom/ClassBalance.ini › PveMagicalSkillCriticalDamageMultipliers` · text · *Advanced*


### PvP: magical skill critical damage by class (PvpMagicalSkillCriticalDamageMultipliers)

`Custom/ClassBalance.ini › PvpMagicalSkillCriticalDamageMultipliers` · text · *Advanced*


### PvE: physical skill damage by class (PvePhysicalSkillDamageMultipliers)

`Custom/ClassBalance.ini › PvePhysicalSkillDamageMultipliers` · text · *Advanced*


### PvP: physical skill damage by class (PvpPhysicalSkillDamageMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalSkillDamageMultipliers` · text · *Advanced*


### PvE: physical skill defence by class (PvePhysicalSkillDefenceMultipliers)

`Custom/ClassBalance.ini › PvePhysicalSkillDefenceMultipliers` · text · *Advanced*


### PvP: physical skill defence by class (PvpPhysicalSkillDefenceMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalSkillDefenceMultipliers` · text · *Advanced*


### PvE: physical skill critical chance by class (PvePhysicalSkillCriticalChanceMultipliers)

`Custom/ClassBalance.ini › PvePhysicalSkillCriticalChanceMultipliers` · text · *Advanced*


### PvP: physical skill critical chance by class (PvpPhysicalSkillCriticalChanceMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalSkillCriticalChanceMultipliers` · text · *Advanced*


### PvE: physical skill critical damage by class (PvePhysicalSkillCriticalDamageMultipliers)

`Custom/ClassBalance.ini › PvePhysicalSkillCriticalDamageMultipliers` · text · *Advanced*


### PvP: physical skill critical damage by class (PvpPhysicalSkillCriticalDamageMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalSkillCriticalDamageMultipliers` · text · *Advanced*


### PvE: physical attack damage by class (PvePhysicalAttackDamageMultipliers)

`Custom/ClassBalance.ini › PvePhysicalAttackDamageMultipliers` · text · *Advanced*


### PvP: physical attack damage by class (PvpPhysicalAttackDamageMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalAttackDamageMultipliers` · text · *Advanced*


### PvE: physical attack defence by class (PvePhysicalAttackDefenceMultipliers)

`Custom/ClassBalance.ini › PvePhysicalAttackDefenceMultipliers` · text · *Advanced*


### PvP: physical attack defence by class (PvpPhysicalAttackDefenceMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalAttackDefenceMultipliers` · text · *Advanced*


### PvE: physical attack critical chance by class (PvePhysicalAttackCriticalChanceMultipliers)

`Custom/ClassBalance.ini › PvePhysicalAttackCriticalChanceMultipliers` · text · *Advanced*


### PvP: physical attack critical chance by class (PvpPhysicalAttackCriticalChanceMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalAttackCriticalChanceMultipliers` · text · *Advanced*


### PvE: physical attack critical damage by class (PvePhysicalAttackCriticalDamageMultipliers)

`Custom/ClassBalance.ini › PvePhysicalAttackCriticalDamageMultipliers` · text · *Advanced*


### PvP: physical attack critical damage by class (PvpPhysicalAttackCriticalDamageMultipliers)

`Custom/ClassBalance.ini › PvpPhysicalAttackCriticalDamageMultipliers` · text · *Advanced*


### PvE: blow skill damage by class (PveBlowSkillDamageMultipliers)

`Custom/ClassBalance.ini › PveBlowSkillDamageMultipliers` · text · *Advanced*


### PvP: blow skill damage by class (PvpBlowSkillDamageMultipliers)

`Custom/ClassBalance.ini › PvpBlowSkillDamageMultipliers` · text · *Advanced*


### PvE: blow skill defence by class (PveBlowSkillDefenceMultipliers)

`Custom/ClassBalance.ini › PveBlowSkillDefenceMultipliers` · text · *Advanced*


### PvP: blow skill defence by class (PvpBlowSkillDefenceMultipliers)

`Custom/ClassBalance.ini › PvpBlowSkillDefenceMultipliers` · text · *Advanced*


### PvE: energy skill damage by class (PveEnergySkillDamageMultipliers)

`Custom/ClassBalance.ini › PveEnergySkillDamageMultipliers` · text · *Advanced*


### PvP: energy skill damage by class (PvpEnergySkillDamageMultipliers)

`Custom/ClassBalance.ini › PvpEnergySkillDamageMultipliers` · text · *Advanced*


### PvE: energy skill defence by class (PveEnergySkillDefenceMultipliers)

`Custom/ClassBalance.ini › PveEnergySkillDefenceMultipliers` · text · *Advanced*


### PvP: energy skill defence by class (PvpEnergySkillDefenceMultipliers)

`Custom/ClassBalance.ini › PvpEnergySkillDefenceMultipliers` · text · *Advanced*


### Player healing skill multipliers (PlayerHealingSkillMultipliers)

`Custom/ClassBalance.ini › PlayerHealingSkillMultipliers` · text · *Advanced*


### Skill mastery chance multipliers (SkillMasteryChanceMultipliers)

`Custom/ClassBalance.ini › SkillMasteryChanceMultipliers` · text · *Advanced*


### Skill reuse multipliers (SkillReuseMultipliers)

`Custom/ClassBalance.ini › SkillReuseMultipliers` · text · *Advanced*


### EXP amount multipliers (ExpAmountMultipliers)

`Custom/ClassBalance.ini › ExpAmountMultipliers` · text · *Advanced*


### SP amount multipliers (SpAmountMultipliers)

`Custom/ClassBalance.ini › SpAmountMultipliers` · text · *Advanced*


## Cancelled buff return

### Return cancelled buffs (CancelReturn)

`Custom/CancelReturn.ini › CancelReturn` · on/off

Global toggle for the buff return feature. When False, no buffs will be restored under any circumstances.

- **Default:** Off
- **Allowed:** On or Off

### …when cancelled by monsters (ReturnMonster)

`Custom/CancelReturn.ini › ReturnMonster` · on/off

Restore buff if the original caster was a Monster or Raid Boss.

- **Default:** On
- **Allowed:** On or Off

### …when cancelled by players (ReturnPlayer)

`Custom/CancelReturn.ini › ReturnPlayer` · on/off

Restore buff if the original caster was a Player.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** […when cancelled in the Olympiad](Settings-Skills-Combat#when-cancelled-in-the-olympiad-returnplayerolys) — it only works while this is On

### …when cancelled in the Olympiad (ReturnPlayerOlys)

`Custom/CancelReturn.ini › ReturnPlayerOlys` · on/off

Restore buffs for Players who are currently participating in the Olympiad. This setting only takes effect when `ReturnPlayer = True`.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** […when cancelled by players](Settings-Skills-Combat#when-cancelled-by-players-returnplayer) to be On — Olympiad returns only apply when returning buffs cancelled by players.

### Buffs come back after (TimeToReturn)

`Custom/CancelReturn.ini › TimeToReturn` · number · seconds

Duration (in seconds) before a cancelled buff is automatically restored. Applies only when conditions for returning a buff are satisfied.

- **Default:** `10`
- **Allowed:** 0 – 2147483647 seconds
