# Gameplay & Interface settings

Names above heads, chat, camera and interface options.

**30 settings** in the **Gameplay & Interface** category of the Client tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Names shown

### Show my name (MyName)

`Option.ini › [Game] › MyName` · on/off

- **Default:** On
- **Allowed:** On or Off

### Show NPC names (NPCName)

`Option.ini › [Game] › NPCName` · on/off

- **Default:** On
- **Allowed:** On or Off

### Show other players' names (OtherPCName)

`Option.ini › [Game] › OtherPCName` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Works with:** [Name display distance](Settings-Gameplay-Interface#name-display-distance-dist) — Names only show within the name display distance.

### Show party members' names (PartyMemberName)

`Option.ini › [Game] › PartyMemberName` · on/off

- **Default:** On
- **Allowed:** On or Off

### Show clan members' names (PledgeMemberName)

`Option.ini › [Game] › PledgeMemberName` · on/off

- **Default:** On
- **Allowed:** On or Off

### Show clan and alliance names (GroupName)

`Option.ini › [Game] › GroupName` · on/off

- **Default:** On
- **Allowed:** On or Off

### Show names above characters (engine) (Name)

`l2.ini › [CharacterDisplay] › Name` · on/off · *Advanced*

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Name display distance](Settings-Gameplay-Interface#name-display-distance-dist) — it only works while this is On

### Name display distance (Dist)

`l2.ini › [CharacterDisplay] › Dist` · number

- **Default:** `1000`
- **Allowed:** 0 – 10000
- **Needs:** [Show names above characters (engine)](Settings-Gameplay-Interface#show-names-above-characters-engine-name) to be On — Names above characters are hidden.
- **Works with:** [Show other players' names](Settings-Gameplay-Interface#show-other-players-names-otherpcname)

## Chat & messages

### Press Enter to start typing in chat (EnterChatting)

`Option.ini › [Game] › EnterChatting` · on/off

- **Default:** On
- **Allowed:** On or Off

### Classic chat window (OldChatting)

`Option.ini › [Game] › OldChatting` · on/off

- **Default:** On
- **Allowed:** On or Off

### Separate system message window (SystemMsgWnd)

`Option.ini › [Game] › SystemMsgWnd` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** […show damage messages there](Settings-Gameplay-Interface#show-damage-messages-there-systemmsgwnddamage) — it only works while this is On
- **Controls:** […show item use messages there](Settings-Gameplay-Interface#show-item-use-messages-there-systemmsgwndexpendableitem) — it only works while this is On

### …show damage messages there (SystemMsgWndDamage)

`Option.ini › [Game] › SystemMsgWndDamage` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Separate system message window](Settings-Gameplay-Interface#separate-system-message-window-systemmsgwnd) to be On — There is no separate system message window.

### …show item use messages there (SystemMsgWndExpendableItem)

`Option.ini › [Game] › SystemMsgWndExpendableItem` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Separate system message window](Settings-Gameplay-Interface#separate-system-message-window-systemmsgwnd) to be On — There is no separate system message window.

### Show game tips (ShowGameTipMsg)

`Option.ini › [Game] › ShowGameTipMsg` · on/off

- **Default:** Off
- **Allowed:** On or Off

## Camera & controls

### Arrow keys turn the camera (ArrowMode)

`Option.ini › [Game] › ArrowMode` · on/off

- **Default:** On
- **Allowed:** On or Off

### Camera follows your character (AutoTrackingPawn)

`Option.ini › [Game] › AutoTrackingPawn` · on/off

- **Default:** On
- **Allowed:** On or Off

### Native mouse movement (IsNative)

`Option.ini › [Game] › IsNative` · on/off · *Advanced*

- **Default:** On
- **Allowed:** On or Off

## Interface

### Show zone names when entering an area (ShowZoneTitle)

`Option.ini › [Game] › ShowZoneTitle` · on/off

- **Default:** On
- **Allowed:** On or Off

### Hide items on the ground (HideDropItem)

`Option.ini › [Game] › HideDropItem` · on/off

- **Default:** Off
- **Allowed:** On or Off

### See-through interface windows (TransparencyMode)

`Option.ini › [Game] › TransparencyMode` · on/off

- **Default:** On
- **Allowed:** On or Off

### Refuse duel requests (IsRejectingDuel)

`Option.ini › [Game] › IsRejectingDuel` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Default party loot rule (PartyLooting)

`Option.ini › [Game] › PartyLooting` · choice

- **Default:** `0` (Finders keepers)
- **Allowed:** one of `0` (Finders keepers), `1` (Random), `2` (Random including spoil), `3` (By turn), `4` (By turn including spoil)

### Screenshot format (ScreenShotQuality)

`Option.ini › [Game] › ScreenShotQuality` · choice · *Advanced*

- **Default:** `0` (0)
- **Allowed:** one of `0` (0), `1` (1), `2` (2)

### Lock the shortcut bar (IsLockShortcutWnd)

`Option.ini › [Game] › IsLockShortcutWnd` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Show second shortcut bar (Is1ExpandShortcutWnd)

`Option.ini › [Game] › Is1ExpandShortcutWnd` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Show third shortcut bar (Is2ExpandShortcutWnd)

`Option.ini › [Game] › Is2ExpandShortcutWnd` · on/off

- **Default:** Off
- **Allowed:** On or Off

### Vertical shortcut bar (IsShortcutWndVertical)

`Option.ini › [Game] › IsShortcutWndVertical` · on/off

- **Default:** Off
- **Allowed:** On or Off

## Language & font

### Client language (Language)

`l2.ini › [LanguageSet] › Language` · number · *Advanced*

0 = Korean, 1 = English, 2 = Japanese.

- **Default:** `0`
- **Allowed:** 0 – 10

### Interface font package (Font)

`l2.ini › [FontSet] › Font` · text · *Advanced*


### Interface font glyph file (Glyph)

`l2.ini › [FontSet] › Glyph` · text · *Advanced*
