# Connection & Login settings

Where the client connects and automatic login.

**6 settings** in the **Connection & Login** category of the Client tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Server

### Server address (ServerAddr)

`l2.ini › [URL] › ServerAddr` · text

The login server the client connects to. 127.0.0.1 is your own world.

- **🔒 Set by the launcher:** Written by the L2Everdream launcher every time you press Play (your world, the official world, or a friend's IP).

### Game port (Port)

`l2.ini › [URL] › Port` · number · *Advanced*

- **Default:** `7777`
- **Allowed:** 1 – 65535

## Automatic login

### Log in automatically (IsL2AutoLogOn)

`l2.ini › [AutoLogOn] › IsL2AutoLogOn` · on/off

Skip the login screen using the account below. Stored in plain text inside l2.ini.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Account name](Settings-Connection-Login#account-name-l2id) — it only works while this is On
- **Controls:** [Account password](Settings-Connection-Login#account-password-l2passwd) — it only works while this is On
- **Controls:** [Character slot to enter](Settings-Connection-Login#character-slot-to-enter-l2slot) — it only works while this is On

### Account name (L2ID)

`l2.ini › [AutoLogOn] › L2ID` · text

- **Needs:** [Log in automatically](Settings-Connection-Login#log-in-automatically-isl2autologon) to be On — Automatic login is off.

### Account password (L2Passwd)

`l2.ini › [AutoLogOn] › L2Passwd` · password

Hidden here, but stored in plain text inside l2.ini.

- **Needs:** [Log in automatically](Settings-Connection-Login#log-in-automatically-isl2autologon) to be On — Automatic login is off.

### Character slot to enter (L2Slot)

`l2.ini › [AutoLogOn] › L2Slot` · number

- **Default:** `0`
- **Allowed:** 0 – 7
- **Needs:** [Log in automatically](Settings-Connection-Login#log-in-automatically-isl2autologon) to be On — Automatic login is off.
