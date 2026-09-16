# Server & Performance settings

Technical server settings. Most players never need these.

**178 settings** in the **Server & Performance** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Network & ports

### Login server address (LoginHost)

`Server.ini › LoginHost` · text · *Advanced*

Where's the Login server this gameserver should connect to Warning: Please don't change default IPs here if you don't know what are you doing! Warning: External/Internal IPs are now inside "ipconfig.xml" file.

- **Default:** `127.0.0.1`
- **Allowed:** an IP address or host name

### Port for talking to the login server (LoginPort)

`Server.ini › LoginPort` · number · *Advanced*

TCP port the login server listen to for gameserver connection requests

- **Default:** `9013`
- **Allowed:** 1 – 65535
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (it picks a free port).

### Game server listens on (GameserverHostname)

`Server.ini › GameserverHostname` · text · *Advanced*

0.0.0.0 = every network adapter.

- **Default:** `0.0.0.0`
- **Allowed:** an IP address or host name

### Game server port (GameserverPort)

`Server.ini › GameserverPort` · number · *Advanced*

The port the client connects to (7777).

- **Default:** `7777`
- **Allowed:** 1 – 65535

### Encrypt network traffic (PacketEncryption)

`Server.ini › PacketEncryption` · on/off · *Advanced*

Packet encryption. By default packets sent or received are encrypted using the Blowfish algorithm. Disabling this reduces the resources needed to process any packets transfered, also broadcasted packets do not need to be re-encrypted for each client sent.

- **Default:** Off
- **Allowed:** On or Off

### Login server listens on (LoginserverHostname)

`Server.ini › LoginserverHostname` · text · *Advanced*

Bind ip of the LoginServer, use 0.0.0.0 to bind on all available IPs Warning: Please don't change default IPs here if you don't know what are you doing! Warning: External/Internal IPs are now inside "ipconfig.xml" file.

- **Default:** `0.0.0.0`
- **Allowed:** an IP address or host name

### Login server port for players (LoginserverPort)

`Server.ini › LoginserverPort` · number · *Advanced*

The client connects here (2106).

- **Default:** `2106`
- **Allowed:** 1 – 65535

### Address game servers connect to (LoginHostname)

`Server.ini › LoginHostname` · list · *Advanced*

The address on which login will listen for GameServers, use * to bind on all available IPs Warning: Please don't change default IPs here if you don't know what are you doing! Warning: External/Internal IPs are now inside "ipconfig.xml" file.

- **Default:** `127.0.0.1`
- **Allowed:** an IP address or host name

### Port game servers connect to (LoginPort)

`Server.ini › LoginPort` · number · *Advanced*

The port on which login will listen for GameServers

- **Default:** `9014`
- **Allowed:** 1 – 65535
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (it picks a free port).

## General

### Server ID to request (RequestServerID)

`Server.ini › RequestServerID` · number · *Advanced*

1 = Bartz.

- **Default:** `0`
- **Allowed:** 1 – 127

### Accept a different server ID if taken (AcceptAlternateID)

`Server.ini › AcceptAlternateID` · on/off · *Advanced*

True = The Login Server will give an other ID to the server if the requested ID is already reserved.

- **Default:** On
- **Allowed:** On or Off

### Game data folder (DatapackRoot)

`Server.ini › DatapackRoot` · text · *Advanced*

Datapack root directory. Defaults to current directory from which the server is started unless the below line is uncommented. Warning: If the specified path is invalid, it will lead to multiple errors!

- **Default:** `.`

### Scripts folder (ScriptRoot)

`Server.ini › ScriptRoot` · text · *Advanced*

Scripts root directory.

- **Default:** `./data/scripts`

### Most players online at once (MaximumOnlineUsers)

`Server.ini › MaximumOnlineUsers` · number · *Advanced*

Define how many players are allowed to play simultaneously on your server.

- **Default:** `100`
- **Allowed:** 0 – 2147483647

### Allowed client protocol versions (AllowedProtocolRevisions)

`Server.ini › AllowedProtocolRevisions` · text · *Advanced*

Interlude is 746. Changing this breaks the client connection.

- **Default:** `746`
- **Allowed:** numbers separated by semicolons, e.g. `100;30;0`

### Server type label on the server list (ServerListType)

`Server.ini › ServerListType` · choice · *Advanced*

Displays server type next to the server name on character selection. Accepted Values: Normal, Relax, Test, Broad, Restricted, Event, Free, World, New, Classic

- **Default:** `Free`
- **Allowed:** one of `Normal`, `Relax`, `Test`, `Broad`, `Restricted`, `Event`, `Free`, `World`, `New`, `Classic`

### Age rating on the server list (ServerListAge)

`Server.ini › ServerListAge` · choice · *Advanced*

Displays server minimum age to the server name on character selection. Accepted values: 0, 15, 18

- **Default:** `0`
- **Allowed:** one of `0`, `15`, `18`

### Show [ ] around the server name (ServerListBrackets)

`Server.ini › ServerListBrackets` · on/off · *Advanced*

Setting for serverList Displays [] in front of server name on character selection

- **Default:** Off
- **Allowed:** On or Off

## Automatic restarts

### Watch for server freezes (DeadlockWatcher)

`Server.ini › DeadlockWatcher` · on/off

Deadlock Watcher (separate thread for detecting deadlocks) For improved crash logs and automatic restart in deadlock case if enabled. Check interval is in seconds.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Check for freezes every](Settings-Server-Performance#check-for-freezes-every-deadlockcheckinterval) — it only works while this is On
- **Controls:** [Restart the server after a freeze](Settings-Server-Performance#restart-the-server-after-a-freeze-restartondeadlock) — it only works while this is On

### Check for freezes every (DeadlockCheckInterval)

`Server.ini › DeadlockCheckInterval` · number · seconds

- **Default:** `20`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Watch for server freezes](Settings-Server-Performance#watch-for-server-freezes-deadlockwatcher) to be On — The freeze watcher is off.

### Restart the server after a freeze (RestartOnDeadlock)

`Server.ini › RestartOnDeadlock` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Watch for server freezes](Settings-Server-Performance#watch-for-server-freezes-deadlockwatcher) to be On — The freeze watcher is off.

### Restart when the computer is overloaded (PrecautionaryRestartEnabled)

`Server.ini › PrecautionaryRestartEnabled` · on/off

Enable server restart when CPU or memory usage is too high.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Overload check: CPU](Settings-Server-Performance#overload-check-cpu-precautionaryrestartcpu) — it only works while this is On
- **Controls:** [Overload check: memory](Settings-Server-Performance#overload-check-memory-precautionaryrestartmemory) — it only works while this is On
- **Controls:** [Don't restart during sieges, Olympiad or raids](Settings-Server-Performance#don-t-restart-during-sieges-olympiad-or-raids-precautionaryrestartchecks) — it only works while this is On
- **Controls:** [Overloaded above](Settings-Server-Performance#overloaded-above-precautionaryrestartpercentage) — it only works while this is On
- **Controls:** [Overload check every](Settings-Server-Performance#overload-check-every-precautionaryrestartdelay) — it only works while this is On

### Overload check: CPU (PrecautionaryRestartCpu)

`Server.ini › PrecautionaryRestartCpu` · on/off

Enable monitoring system CPU usage.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Restart when the computer is overloaded](Settings-Server-Performance#restart-when-the-computer-is-overloaded-precautionaryrestartenabled) to be On — Overload restarts are off.

### Overload check: memory (PrecautionaryRestartMemory)

`Server.ini › PrecautionaryRestartMemory` · on/off

Enable monitoring process memory usage.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Restart when the computer is overloaded](Settings-Server-Performance#restart-when-the-computer-is-overloaded-precautionaryrestartenabled) to be On — Overload restarts are off.

### Don't restart during sieges, Olympiad or raids (PrecautionaryRestartChecks)

`Server.ini › PrecautionaryRestartChecks` · on/off

Check if sieges are in progress or players are in olympiad, events, instances or have targeted raidbosses.

- **Default:** On
- **Allowed:** On or Off
- **Needs:** [Restart when the computer is overloaded](Settings-Server-Performance#restart-when-the-computer-is-overloaded-precautionaryrestartenabled) to be On — Overload restarts are off.

### Overloaded above (PrecautionaryRestartPercentage)

`Server.ini › PrecautionaryRestartPercentage` · number · %

Percentage of used resources.

- **Default:** `95`
- **Allowed:** 0 – 100 %
- **Needs:** [Restart when the computer is overloaded](Settings-Server-Performance#restart-when-the-computer-is-overloaded-precautionaryrestartenabled) to be On — Overload restarts are off.

### Overload check every (PrecautionaryRestartDelay)

`Server.ini › PrecautionaryRestartDelay` · number · seconds

Delay in seconds between each check.

- **Default:** `60`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Restart when the computer is overloaded](Settings-Server-Performance#restart-when-the-computer-is-overloaded-precautionaryrestartenabled) to be On — Overload restarts are off.

### Restart on a schedule (ServerRestartScheduleEnabled)

`Server.ini › ServerRestartScheduleEnabled` · on/off

Enable scheduled server restart.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Tell players about the next restart when they log in](Settings-Server-Performance#tell-players-about-the-next-restart-when-they-log-in-serverrestartschedulemessage) — it only works while this is On
- **Controls:** [Restart countdown](Settings-Server-Performance#restart-countdown-serverrestartschedulecountdown) — it only works while this is On
- **Controls:** [Restart times](Settings-Server-Performance#restart-times-serverrestartschedule) — it only works while this is On
- **Controls:** [Restart days](Settings-Server-Performance#restart-days-serverrestartdays) — it only works while this is On

### Tell players about the next restart when they log in (ServerRestartScheduleMessage)

`Server.ini › ServerRestartScheduleMessage` · on/off

Send a message when player enters the game.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Restart on a schedule](Settings-Server-Performance#restart-on-a-schedule-serverrestartscheduleenabled) to be On — Scheduled restarts are off.

### Restart countdown (ServerRestartScheduleCountdown)

`Server.ini › ServerRestartScheduleCountdown` · number · seconds

Restart time countdown (in seconds).

- **Default:** `600`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Restart on a schedule](Settings-Server-Performance#restart-on-a-schedule-serverrestartscheduleenabled) to be On — Scheduled restarts are off.

### Restart times (ServerRestartSchedule)

`Server.ini › ServerRestartSchedule` · list

Example: 12:00, 00:00

- **Default:** `08:00`
- **Allowed:** times as `HH:MM` separated by commas
- **Needs:** [Restart on a schedule](Settings-Server-Performance#restart-on-a-schedule-serverrestartscheduleenabled) to be On — Scheduled restarts are off.

### Restart days (ServerRestartDays)

`Server.ini › ServerRestartDays` · list

1 = Sunday … 7 = Saturday, comma-separated.

- **Default:** `4 (WEDNESDAY)`
- **Allowed:** day numbers 1–7 separated by commas (1 = Sunday)
- **Needs:** [Restart on a schedule](Settings-Server-Performance#restart-on-a-schedule-serverrestartscheduleenabled) to be On — Scheduled restarts are off.

## Saving & memory

### Save characters every (CharacterDataStoreInterval)

`General.ini › CharacterDataStoreInterval` · number · minutes · *Advanced*

0 = only on logout.

- **Default:** `15`
- **Allowed:** 0 – 2147483647 minutes

### Save items only with the character (LazyItemsUpdate)

`General.ini › LazyItemsUpdate` · on/off · *Advanced*

Faster, but items can be lost if the server crashes.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Always save items when saving the character](Settings-Server-Performance#always-save-items-when-saving-the-character-updateitemsoncharstore) — it only works while this is On

### Always save items when saving the character (UpdateItemsOnCharStore)

`General.ini › UpdateItemsOnCharStore` · on/off · *Advanced*

When enabled, this forces (even if using lazy item updates) the items owned by the character to be updated into DB when saving its character.

- **Default:** Off
- **Allowed:** On or Off
- **Needs:** [Save items only with the character](Settings-Server-Performance#save-items-only-with-the-character-lazyitemsupdate) to be On — Only matters when items are saved lazily.

### Delete broken quest data from characters (AutoDeleteInvalidQuestData)

`General.ini › AutoDeleteInvalidQuestData` · on/off · *Advanced*

Delete invalid quest from players.

- **Default:** Off
- **Allowed:** On or Off

### Load all NPC dialogs at startup (HtmCache)

`General.ini › HtmCache` · on/off · *Advanced*

Enable/Disable html caching. True = Load all html's into cache on server startup. False = Load html's into cache only on first time html is requested. Recommended for live servers: True Recommended for development: False

- **Default:** On
- **Allowed:** On or Off

### Warn about non-English characters in NPC dialogs (CheckHtmlEncoding)

`General.ini › CheckHtmlEncoding` · on/off · *Advanced*

Check if html files contain non ASCII characters. Default = True

- **Default:** On
- **Allowed:** On or Off

### Keep every world region active (GridsAlwaysOn)

`General.ini › GridsAlwaysOn` · on/off · *Advanced*

Uses much more CPU. Not recommended.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Wake nearby regions after](Settings-Server-Performance#wake-nearby-regions-after-gridneighborturnontime) — it only works while this is Off
- **Controls:** [Sleep empty regions after](Settings-Server-Performance#sleep-empty-regions-after-gridneighborturnofftime) — it only works while this is Off

### Wake nearby regions after (GridNeighborTurnOnTime)

`General.ini › GridNeighborTurnOnTime` · number · seconds · *Advanced*

- **Default:** `1`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Keep every world region active](Settings-Server-Performance#keep-every-world-region-active-gridsalwayson) to be Off — Regions never sleep while they are all kept active.

### Sleep empty regions after (GridNeighborTurnOffTime)

`General.ini › GridNeighborTurnOffTime` · number · seconds · *Advanced*

- **Default:** `90`
- **Allowed:** 0 – 2147483647 seconds
- **Needs:** [Keep every world region active](Settings-Server-Performance#keep-every-world-region-active-gridsalwayson) to be Off — Regions never sleep while they are all kept active.

## Geodata & pathfinding

### Path finding (PathFinding)

`GeoEngine.ini › PathFinding` · choice · *Advanced*

Pathfinding options: 0 = Disabled 1 = Enabled using path node files. 2 = Enabled using geodata cells at runtime (recommended).

- **Default:** `0` (Disabled)
- **Allowed:** one of `0` (Disabled), `1` (Enabled using path node files.), `2` (Enabled using geodata cells at runtime (recommended).)
- **Controls:** [Path find buffers](Settings-Server-Performance#path-find-buffers-pathfindbuffers) — it only works while this is `>0`

### Geo data path (GeoDataPath)

`GeoEngine.ini › GeoDataPath` · text · *Advanced*

Geodata file directory.

- **Default:** `geodata`

### Pathnode path (PathnodePath)

`GeoEngine.ini › PathnodePath` · text · *Advanced*

Pathnode file directory.

- **Default:** `pathnode`

### Geo edit path (GeoEditPath)

`GeoEngine.ini › GeoEditPath` · text · *Advanced*

Geodata editor save directory. Can set to ./data/geodata/ to replace existing files on save.

- **Default:** `saves`

### Path find buffers (PathFindBuffers)

`GeoEngine.ini › PathFindBuffers` · list · *Advanced* · *added by L2Everdream*

Pathfinding array buffers configuration.

- **Default:** `100x6;128x6;192x6;256x4;320x4;384x4;500x2`
- **Allowed:** `size x count` pairs separated by semicolons, e.g. `100x8;128x8`
- **Needs:** [Path finding](Settings-Server-Performance#path-finding-pathfinding) to be `>0` — Pathfinding is off.

### Low weight (LowWeight)

`GeoEngine.ini › LowWeight` · number · *Advanced*

Weight for nodes without obstacles far from walls.

- **Default:** `0.5`
- **Allowed:** 0 or more

### Medium weight (MediumWeight)

`GeoEngine.ini › MediumWeight` · number · *Advanced*

Weight for nodes near walls.

- **Default:** `2`
- **Allowed:** 0 or more

### High weight (HighWeight)

`GeoEngine.ini › HighWeight` · number · *Advanced*

Weight for nodes with obstacles.

- **Default:** `3.0`
- **Allowed:** 0 or more

### Advanced diagonal strategy (AdvancedDiagonalStrategy)

`GeoEngine.ini › AdvancedDiagonalStrategy` · on/off · *Advanced*

Angle paths will be more "smart", but in cost of higher CPU utilization.

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Diagonal weight](Settings-Server-Performance#diagonal-weight-diagonalweight) — it only works while this is On

### Avoid obstructed path nodes (AvoidObstructedPathNodes)

`GeoEngine.ini › AvoidObstructedPathNodes` · on/off · *Advanced*

Avoid pathing to nodes that are obstructed at any direction.

- **Default:** On
- **Allowed:** On or Off

### Diagonal weight (DiagonalWeight)

`GeoEngine.ini › DiagonalWeight` · number · *Advanced*

Weight for diagonal movement. Used only with AdvancedDiagonalStrategy = True

- **Default:** `0.707`
- **Allowed:** 0 or more
- **Needs:** [Advanced diagonal strategy](Settings-Server-Performance#advanced-diagonal-strategy-advanceddiagonalstrategy) to be On — Only used with the advanced diagonal strategy.

### Maximum postfilter passes (MaxPostfilterPasses)

`GeoEngine.ini › MaxPostfilterPasses` · number · *Advanced*

Maximum number of LOS postfilter passes, 0 will disable postfilter.

- **Default:** `3`
- **Allowed:** 0 – 2147483647

## Threads

### Shutdown wait time (ShutdownWaitTime)

`Network.ini › ShutdownWaitTime` · number · seconds · *Advanced*

Shutdown Wait Time Defines the time in seconds the server waits for send/receive packets to be finalized during shutdown.

- **Default:** `5 seconds`
- **Allowed:** 0 or more seconds

### Drop packets (DropPackets)

`Network.ini › DropPackets` · on/off · *Advanced*

Define if packet dropping is enabled.

- **Default:** On
- **Allowed:** On or Off

### Drop packet threshold (DropPacketThreshold)

`Network.ini › DropPacketThreshold` · number · *Advanced*

Packet Dropping Threshold Defines the threshold to drop disposable packets. Higher values allow more packets to be queued before dropping starts, potentially reducing packet loss at the cost of higher memory usage.

- **Default:** `250`
- **Allowed:** 0 or more

### Thread pool size (ThreadPoolSize)

`Network.ini › ThreadPoolSize` · number · *Advanced*

Defines the core pool size of threads in each packet execution thread pool. This determines the number of threads that will be prestarted and available to handle tasks immediately. If set to 0 the default size is set to the number of available processor cores. A higher number can handle more tasks concurrently, while a lower number conserves resources.

- **Default:** `0 (auto-configured as the number of available processor cores * 4)`
- **Allowed:** 0 or more

### Thread priority (ThreadPriority)

`Network.ini › ThreadPriority` · number · *Advanced*

Defines the priority level of threads within the packet execution thread pool. This value determines the importance of these threads relative to other processes on the system. Values typically range from 1 (lowest priority) to 10 (highest priority).

- **Default:** `5 (medium priority level suitable for general task handling)`
- **Allowed:** 1 – 10

### Scheduled thread pool size (ScheduledThreadPoolSize)

`Threads.ini › ScheduledThreadPoolSize` · number · *Advanced*

Defines the number of threads in the scheduled thread pool. If set to -1, this will be determined by available processors multiplied by 4. You can specify a positive integer to manually set the pool size. Additionally, a high priority pool is created, sized at one quarter of the scheduled pool. Note that higher values can improve task handling under heavy load but may increase CPU and memory usage.

- **Default:** `-1`
- **Allowed:** -2147483648 – 2147483647

### Instant thread pool size (InstantThreadPoolSize)

`Threads.ini › InstantThreadPoolSize` · number · *Advanced*

Defines the number of threads in the instant thread pool. If set to -1, this will be determined by available processors multiplied by 2. You can specify a positive integer to manually set the pool size. Note that higher values can improve task handling under heavy load but may increase CPU and memory usage.

- **Default:** `-1`
- **Allowed:** -2147483648 – 2147483647

### Threads for loading (ThreadsForLoading)

`Threads.ini › ThreadsForLoading` · on/off · *Advanced*

Use threads to decrease startup time.

- **Default:** Off
- **Allowed:** On or Off

## Network buffers

### Buffer segment size (BufferSegmentSize)

`Network.ini › BufferSegmentSize` · number · *Advanced*

Buffer Segment Size Size of segments for dynamic buffers, which are used to increase buffer sizes as needed.

- **Default:** `64`
- **Allowed:** 0 or more

### Buffer pool auto expand capacity (BufferPool.AutoExpandCapacity)

`Network.ini › BufferPool.AutoExpandCapacity` · on/off · *Advanced* · *added by L2Everdream*

Allow the buffer pool to dynamically increase its size when it reaches maximum capacity.

- **Default:** On
- **Allowed:** On or Off

### Buffer pool init factor (BufferPool.InitFactor)

`Network.ini › BufferPool.InitFactor` · number · *Advanced*

Initial Factor Multiplier for pre-initializing buffer pools. A higher factor means more buffers are pre-created.

- **Default:** `0 (no pre-initialization)`
- **Allowed:** 0 or more

### Empty buffers: how many (BufferPool.Empty.Size)

`Network.ini › BufferPool.Empty.Size` · number · *Advanced*

Empty Buffer Pool Size and buffer size for empty size buffers. Used for empty sized network packets.

- **Default:** `100 buffers of 2 bytes each.`
- **Allowed:** 0 or more

### Empty buffers: buffer size (BufferPool.Empty.BufferSize)

`Network.ini › BufferPool.Empty.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Minimum buffers: how many (BufferPool.Minimum.Size)

`Network.ini › BufferPool.Minimum.Size` · number · *Advanced*

Minimum Buffer Pool Size and buffer size for minimum size buffers. Used for minimum sized network packets.

- **Default:** `100 buffers of 64 bytes each.`
- **Allowed:** 0 or more

### Minimum buffers: buffer size (BufferPool.Minimum.BufferSize)

`Network.ini › BufferPool.Minimum.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Very small buffers: how many (BufferPool.VerySmall.Size)

`Network.ini › BufferPool.VerySmall.Size` · number · *Advanced*

Very Small Buffer Pool Size and buffer size for very small buffers. Used for very small network packets.

- **Default:** `75 buffers of 128 bytes each.`
- **Allowed:** 0 or more

### Very small buffers: buffer size (BufferPool.VerySmall.BufferSize)

`Network.ini › BufferPool.VerySmall.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Small buffers: how many (BufferPool.Small.Size)

`Network.ini › BufferPool.Small.Size` · number · *Advanced*

Small Buffer Pool Size and buffer size for small buffers. Used for small network packets.

- **Default:** `75 buffers of 256 bytes each.`
- **Allowed:** 0 or more

### Small buffers: buffer size (BufferPool.Small.BufferSize)

`Network.ini › BufferPool.Small.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Large small buffers: how many (BufferPool.LargeSmall.Size)

`Network.ini › BufferPool.LargeSmall.Size` · number · *Advanced*

Large Small Buffer Pool Size and buffer size for larger small buffers. Used for larger small network packets.

- **Default:** `75 buffers of 512 bytes each.`
- **Allowed:** 0 or more

### Large small buffers: buffer size (BufferPool.LargeSmall.BufferSize)

`Network.ini › BufferPool.LargeSmall.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Small medium buffers: how many (BufferPool.SmallMedium.Size)

`Network.ini › BufferPool.SmallMedium.Size` · number · *Advanced*

Small Medium Buffer Pool Size and buffer size for small medium buffers. Used for smaller medium network packets.

- **Default:** `50 buffers of 1024 bytes each.`
- **Allowed:** 0 or more

### Small medium buffers: buffer size (BufferPool.SmallMedium.BufferSize)

`Network.ini › BufferPool.SmallMedium.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Medium buffers: how many (BufferPool.Medium.Size)

`Network.ini › BufferPool.Medium.Size` · number · *Advanced*

Medium Buffer Pool Size and buffer size for medium buffers. Used for medium network packets.

- **Default:** `50 buffers of 2048 bytes each.`
- **Allowed:** 0 or more

### Medium buffers: buffer size (BufferPool.Medium.BufferSize)

`Network.ini › BufferPool.Medium.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Large medium buffers: how many (BufferPool.LargeMedium.Size)

`Network.ini › BufferPool.LargeMedium.Size` · number · *Advanced*

Large Medium Buffer Pool Size and buffer size for larger medium buffers. Used for larger medium network packets.

- **Default:** `50 buffers of 4096 bytes each.`
- **Allowed:** 0 or more

### Large medium buffers: buffer size (BufferPool.LargeMedium.BufferSize)

`Network.ini › BufferPool.LargeMedium.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Small large buffers: how many (BufferPool.SmallLarge.Size)

`Network.ini › BufferPool.SmallLarge.Size` · number · *Advanced*

Small Large Buffer Pool Size and buffer size for small large buffers. Used for smaller large network packets.

- **Default:** `25 buffers of 8192 bytes each.`
- **Allowed:** 0 or more

### Small large buffers: buffer size (BufferPool.SmallLarge.BufferSize)

`Network.ini › BufferPool.SmallLarge.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Large buffers: how many (BufferPool.Large.Size)

`Network.ini › BufferPool.Large.Size` · number · *Advanced*

Large Buffer Pool Size and buffer size for large buffers. Used for large network packets.

- **Default:** `25 buffers of 16384 bytes.`
- **Allowed:** 0 or more

### Large buffers: buffer size (BufferPool.Large.BufferSize)

`Network.ini › BufferPool.Large.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Very large buffers: how many (BufferPool.VeryLarge.Size)

`Network.ini › BufferPool.VeryLarge.Size` · number · *Advanced*

Very Large Buffer Pool Size and buffer size for very large buffers. Used for very large network packets.

- **Default:** `25 buffers of 24576 bytes each.`
- **Allowed:** 0 or more

### Very large buffers: buffer size (BufferPool.VeryLarge.BufferSize)

`Network.ini › BufferPool.VeryLarge.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Maximum buffers: how many (BufferPool.Maximum.Size)

`Network.ini › BufferPool.Maximum.Size` · number · *Advanced*

Maximum Buffer Pool Size and buffer size for maximum size buffers. Used for maximum sized network packets.

- **Default:** `10 buffers of 32768 bytes.`
- **Allowed:** 0 or more

### Maximum buffers: buffer size (BufferPool.Maximum.BufferSize)

`Network.ini › BufferPool.Maximum.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Security buffer pool: how many (BufferPool.Huge.Size)

`Network.ini › BufferPool.Huge.Size` · number · *Advanced* · *added by L2Everdream*

Part of a security protection added by L2Everdream. Leave it as shipped.

- **Allowed:** 0 or more

### Security buffer pool: buffer size (BufferPool.Huge.BufferSize)

`Network.ini › BufferPool.Huge.BufferSize` · number · *Advanced*

Part of a security protection added by L2Everdream. Leave it as shipped.

- **Allowed:** 0 or more

## Object IDs

### Database clean up (DatabaseCleanUp)

`IdManager.ini › DatabaseCleanUp` · on/off · *Advanced*

Enables database cleanup during server startup to remove obsolete or invalid data. A larger database may result in a slower startup if cleanup is enabled. To speed up startup, set this to 'False'; however, regular cleanup is recommended for optimal performance.

- **Default:** On
- **Allowed:** On or Off

### First object ID (FirstObjectId)

`IdManager.ini › FirstObjectId` · number · *Advanced*

Specifies the starting object ID in the range of allocatable IDs. This value, along with LastObjectId, defines the entire range of unique IDs that can be allocated. Note: FirstObjectId should be less than LastObjectId.

- **Default:** `268435456`
- **Allowed:** 0 – 2147483647

### Last object ID (LastObjectId)

`IdManager.ini › LastObjectId` · number · *Advanced*

Specifies the ending object ID in the range of allocatable IDs. The maximum value (2,147,483,647 or 0x7FFFFFFF) ensures the ID range fits within standard 32-bit integer limits.

- **Default:** `2147483647`
- **Allowed:** 0 – 2147483647

### Initial capacity (InitialCapacity)

`IdManager.ini › InitialCapacity` · number · *Advanced*

Defines the initial capacity of the BitSet in terms of ID count. This setting specifies how many IDs the BitSet can initially track and may influence memory usage.

- **Default:** `100000`
- **Allowed:** 0 – 2147483647

### Resize threshold (ResizeThreshold)

`IdManager.ini › ResizeThreshold` · number · % · *Advanced*

Sets the utilization threshold for triggering dynamic resizing of the BitSet. When the percentage of used IDs meets or exceeds this value, the BitSet expands to accommodate more IDs. The value must be between 0 and 1 (e.g., 0.9 represents 90% usage).

- **Default:** `0.9`
- **Allowed:** 0 – 100 %

### Resize multiplier (ResizeMultiplier)

`IdManager.ini › ResizeMultiplier` · number · times · *Advanced*

Sets the growth factor for resizing the BitSet when utilization reaches the ResizeThreshold. This multiplier determines how much the BitSet expands, as a factor of its current size. For example, a value of 1.1 expands the BitSet by 10% each time resizing occurs.

- **Default:** `1.1`
- **Allowed:** 0 or more times

## Database

### Driver (Driver)

`Database.ini › Driver` · text · *Advanced*

Specify the JDBC driver class for your database.

- **Default:** `com.mysql.cj.jdbc.Driver`
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Url (URL)

`Database.ini › URL` · list · *Advanced*

Database URL

- **Default:** `jdbc:mysql://localhost/l2jmobius`
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Login (Login)

`Database.ini › Login` · text · *Advanced*

Database user info. Default is "root" but it's not recommended.

- **Default:** `root`
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Password (Password)

`Database.ini › Password` · text · *Advanced*

Database user password, leave empty for no password.

- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Database connections (MaximumDatabaseConnections)

`Database.ini › MaximumDatabaseConnections` · number · *Advanced*

Maximum number of database connections to maintain in the pool.

- **Default:** `10`
- **Allowed:** 0 – 2147483647
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Test database connections (TestDatabaseConnections)

`Database.ini › TestDatabaseConnections` · on/off · *Advanced*

Determine whether database connections should be tested for availability.

- **Default:** Off
- **Allowed:** On or Off
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Back up the database on shutdown (BackupDatabase)

`Database.ini › BackupDatabase` · on/off · *Advanced*

Automatic Database Backup Settings Generate database backups when server restarts or shuts down.

- **Default:** Off
- **Allowed:** On or Off
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### MySQL tools folder (MySqlBinLocation)

`Database.ini › MySqlBinLocation` · text · *Advanced*

Path to MySQL bin folder. Only necessary on Windows.

- **Default:** `C:/xampp/mysql/bin/`
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Backup folder (BackupPath)

`Database.ini › BackupPath` · text · *Advanced*

Path where MySQL backups are stored.

- **Default:** `../backup/`
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Keep backups for (BackupDays)

`Database.ini › BackupDays` · number · days · *Advanced*

Maximum number of days that backups will be kept. Old files in backup folder will be deleted. Set to 0 to disable.

- **Default:** `30`
- **Allowed:** 0 – 2147483647 days
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the world's own database).

### Driver (Driver)

`Database.ini › Driver` · text · *Advanced*

Specify the JDBC driver class for your database.

- **Default:** `com.mysql.cj.jdbc.Driver`
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Url (URL)

`Database.ini › URL` · list · *Advanced*

Database URL

- **Default:** `jdbc:mysql://localhost/l2jmobiusinterlude?useUnicode=true&characterEncoding=utf-8&allowPublicKeyRetrieval=true&useSSL=false&connectTimeout=10000&interactiveClient=true&sessionVariables=wait_timeout=600,interactive_timeout=600&autoReconnect=true`
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Login (Login)

`Database.ini › Login` · text · *Advanced*

Database user info. Default is "root" but it's not recommended.

- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Password (Password)

`Database.ini › Password` · text · *Advanced*

Database user password, leave empty for no password.

- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Maximum database connections (MaximumDatabaseConnections)

`Database.ini › MaximumDatabaseConnections` · number · *Advanced*

Maximum number of database connections to maintain in the pool.

- **Default:** `5`
- **Allowed:** 0 or more
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Test database connections (TestDatabaseConnections)

`Database.ini › TestDatabaseConnections` · on/off · *Advanced*

Determine whether database connections should be tested for availability.

- **Default:** Off
- **Allowed:** On or Off
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Backup database (BackupDatabase)

`Database.ini › BackupDatabase` · on/off · *Advanced*

Automatic Database Backup Settings Generate database backups when server restarts or shuts down.

- **Allowed:** On or Off
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### MySQL bin location (MySqlBinLocation)

`Database.ini › MySqlBinLocation` · text · *Advanced*

Path to MySQL bin folder. Only necessary on Windows.

- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Backup path (BackupPath)

`Database.ini › BackupPath` · text · *Advanced*

Path where MySQL backups are stored.

- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

### Backup days (BackupDays)

`Database.ini › BackupDays` · number · days · *Advanced*

Maximum number of days that backups will be kept. Old files in backup folder will be deleted. Set to 0 to disable.

- **Allowed:** 0 or more days
- **🔒 Set by the launcher:** Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).

## Custom data

### Load custom NPCs (CustomNpcData)

`General.ini › CustomNpcData` · on/off · *Advanced*

- **Default:** Off
- **Allowed:** On or Off

### Load custom teleports (CustomTeleportTable)

`General.ini › CustomTeleportTable` · on/off · *Advanced*

- **Default:** Off
- **Allowed:** On or Off

### Load custom skills (CustomSkillsLoad)

`General.ini › CustomSkillsLoad` · on/off · *Advanced*

- **Default:** Off
- **Allowed:** On or Off

### Load custom items (CustomItemsLoad)

`General.ini › CustomItemsLoad` · on/off · *Advanced*

- **Default:** Off
- **Allowed:** On or Off

### Load custom multisells (CustomMultisellLoad)

`General.ini › CustomMultisellLoad` · on/off · *Advanced*

- **Default:** Off
- **Allowed:** On or Off

### Load custom NPC shop lists (CustomBuyListLoad)

`General.ini › CustomBuyListLoad` · on/off · *Advanced*

- **Default:** Off
- **Allowed:** On or Off

## Development & debugging

### Log server load times (LogServerLoadTimes)

`Development.ini › LogServerLoadTimes` · on/off · *Advanced*

Display the time taken to load each server initialization section during startup. When enabled, logs will show completion time for Database, ThreadPool, Skills, Items, etc. Useful for identifying slow startup phases and performance bottlenecks.

- **Default:** Off
- **Allowed:** On or Off

### HTML action cache debug (HtmlActionCacheDebug)

`Development.ini › HtmlActionCacheDebug` · on/off · *Advanced*

Html action cache debugging.

- **Default:** Off
- **Allowed:** On or Off

### No quests (NoQuests)

`Development.ini › NoQuests` · on/off · *Advanced*

Don't load quests.

- **Default:** Off
- **Allowed:** On or Off

### No spawns (NoSpawns)

`Development.ini › NoSpawns` · on/off · *Advanced*

Don't load spawntable.

- **Default:** Off
- **Allowed:** On or Off

### Show quest load in logs (ShowQuestLoadInLogs)

`Development.ini › ShowQuestLoadInLogs` · on/off · *Advanced*

Show quests while loading them.

- **Default:** Off
- **Allowed:** On or Off

### Show script load in logs (ShowScriptLoadInLogs)

`Development.ini › ShowScriptLoadInLogs` · on/off · *Advanced*

Show scripts while loading them.

- **Default:** Off
- **Allowed:** On or Off

### Debug client packets (DebugClientPackets)

`Development.ini › DebugClientPackets` · on/off · *Advanced*

Debug client packets.

- **Default:** Off
- **Allowed:** On or Off

### Debug ex client packets (DebugExClientPackets)

`Development.ini › DebugExClientPackets` · on/off · *Advanced*

Debug ex-client packets.

- **Default:** Off
- **Allowed:** On or Off

### Debug server packets (DebugServerPackets)

`Development.ini › DebugServerPackets` · on/off · *Advanced*

Debug server packets.

- **Default:** Off
- **Allowed:** On or Off

### Debug unknown packets (DebugUnknownPackets)

`Development.ini › DebugUnknownPackets` · on/off · *Advanced*

Debug unknown packets.

- **Default:** On
- **Allowed:** On or Off

### Excluded packet list (ExcludedPacketList)

`Development.ini › ExcludedPacketList` · list · *Advanced*

Excluded packet list. Packet names that are excluded from debugging, separated by commas.

- **Allowed:** words separated by commas

## Login server

### Show the login server window (EnableGUI)

`Interface.ini › EnableGUI` · on/off · *Advanced*

Enable L2jMobius GUI when OS supports it. Provides access to admin commands without the need to be online.

- **Default:** On
- **Allowed:** On or Off

### Dark login server window (DarkTheme)

`Interface.ini › DarkTheme` · on/off · *Advanced*

Dark theme. Use a dark version of the Nimbus theme.

- **Default:** On
- **Allowed:** On or Off

### Shutdown wait time (ShutdownWaitTime)

`Network.ini › ShutdownWaitTime` · number · seconds · *Advanced*

Shutdown Wait Time Defines the time in seconds the server waits for send/receive packets to be finalized during shutdown.

- **Default:** `5 seconds`
- **Allowed:** 0 or more seconds

### Drop packets (DropPackets)

`Network.ini › DropPackets` · on/off · *Advanced*

Define if packet dropping is enabled.

- **Default:** Off
- **Allowed:** On or Off

### Drop packet threshold (DropPacketThreshold)

`Network.ini › DropPacketThreshold` · number · *Advanced*

Packet Dropping Threshold Defines the threshold to drop disposable packets. Higher values allow more packets to be queued before dropping starts, potentially reducing packet loss at the cost of higher memory usage.

- **Default:** `250`
- **Allowed:** 0 or more

### Thread pool size (ThreadPoolSize)

`Network.ini › ThreadPoolSize` · number · *Advanced*

Defines the core pool size of threads in each packet execution thread pool. This determines the number of threads that will be prestarted and available to handle tasks immediately. If set to 0 the default size is set to the number of available processor cores. A higher number can handle more tasks concurrently, while a lower number conserves resources.

- **Default:** `0 (auto-configured as the number of available processor cores * 4)`
- **Allowed:** 0 or more

### Thread priority (ThreadPriority)

`Network.ini › ThreadPriority` · number · *Advanced*

Defines the priority level of threads within the packet execution thread pool. This value determines the importance of these threads relative to other processes on the system. Values typically range from 1 (lowest priority) to 10 (highest priority).

- **Default:** `5 (medium priority level suitable for general task handling)`
- **Allowed:** 1 – 10

### Buffer segment size (BufferSegmentSize)

`Network.ini › BufferSegmentSize` · number · *Advanced*

Buffer Segment Size Size of segments for dynamic buffers, which are used to increase buffer sizes as needed.

- **Default:** `64`
- **Allowed:** 0 or more

### Buffer pool auto expand capacity (BufferPool.AutoExpandCapacity)

`Network.ini › BufferPool.AutoExpandCapacity` · on/off · *Advanced* · *added by L2Everdream*

Allow the buffer pool to dynamically increase its size when it reaches maximum capacity.

- **Default:** On
- **Allowed:** On or Off

### Buffer pool init factor (BufferPool.InitFactor)

`Network.ini › BufferPool.InitFactor` · number · *Advanced*

Initial Factor Multiplier for pre-initializing buffer pools. A higher factor means more buffers are pre-created.

- **Default:** `0 (no pre-initialization)`
- **Allowed:** 0 or more

### Empty buffers: how many (BufferPool.Empty.Size)

`Network.ini › BufferPool.Empty.Size` · number · *Advanced*

Empty Buffer Pool Size and buffer size for empty size buffers. Used for empty sized network packets.

- **Default:** `100 buffers of 2 bytes each.`
- **Allowed:** 0 or more

### Empty buffers: buffer size (BufferPool.Empty.BufferSize)

`Network.ini › BufferPool.Empty.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Minimum buffers: how many (BufferPool.Minimum.Size)

`Network.ini › BufferPool.Minimum.Size` · number · *Advanced*

Minimum Buffer Pool Size and buffer size for minimum size buffers. Used for minimum sized network packets.

- **Default:** `100 buffers of 64 bytes each.`
- **Allowed:** 0 or more

### Minimum buffers: buffer size (BufferPool.Minimum.BufferSize)

`Network.ini › BufferPool.Minimum.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Very small buffers: how many (BufferPool.VerySmall.Size)

`Network.ini › BufferPool.VerySmall.Size` · number · *Advanced*

Very Small Buffer Pool Size and buffer size for very small buffers. Used for very small network packets.

- **Default:** `75 buffers of 128 bytes each.`
- **Allowed:** 0 or more

### Very small buffers: buffer size (BufferPool.VerySmall.BufferSize)

`Network.ini › BufferPool.VerySmall.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Small buffers: how many (BufferPool.Small.Size)

`Network.ini › BufferPool.Small.Size` · number · *Advanced*

Small Buffer Pool Size and buffer size for small buffers. Used for small network packets.

- **Default:** `75 buffers of 256 bytes each.`
- **Allowed:** 0 or more

### Small buffers: buffer size (BufferPool.Small.BufferSize)

`Network.ini › BufferPool.Small.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Large small buffers: how many (BufferPool.LargeSmall.Size)

`Network.ini › BufferPool.LargeSmall.Size` · number · *Advanced*

Large Small Buffer Pool Size and buffer size for larger small buffers. Used for larger small network packets.

- **Default:** `75 buffers of 512 bytes each.`
- **Allowed:** 0 or more

### Large small buffers: buffer size (BufferPool.LargeSmall.BufferSize)

`Network.ini › BufferPool.LargeSmall.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Small medium buffers: how many (BufferPool.SmallMedium.Size)

`Network.ini › BufferPool.SmallMedium.Size` · number · *Advanced*

Small Medium Buffer Pool Size and buffer size for small medium buffers. Used for smaller medium network packets.

- **Default:** `50 buffers of 1024 bytes each.`
- **Allowed:** 0 or more

### Small medium buffers: buffer size (BufferPool.SmallMedium.BufferSize)

`Network.ini › BufferPool.SmallMedium.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Medium buffers: how many (BufferPool.Medium.Size)

`Network.ini › BufferPool.Medium.Size` · number · *Advanced*

Medium Buffer Pool Size and buffer size for medium buffers. Used for medium network packets.

- **Default:** `50 buffers of 2048 bytes each.`
- **Allowed:** 0 or more

### Medium buffers: buffer size (BufferPool.Medium.BufferSize)

`Network.ini › BufferPool.Medium.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Large medium buffers: how many (BufferPool.LargeMedium.Size)

`Network.ini › BufferPool.LargeMedium.Size` · number · *Advanced*

Large Medium Buffer Pool Size and buffer size for larger medium buffers. Used for larger medium network packets.

- **Default:** `50 buffers of 4096 bytes each.`
- **Allowed:** 0 or more

### Large medium buffers: buffer size (BufferPool.LargeMedium.BufferSize)

`Network.ini › BufferPool.LargeMedium.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Small large buffers: how many (BufferPool.SmallLarge.Size)

`Network.ini › BufferPool.SmallLarge.Size` · number · *Advanced*

Small Large Buffer Pool Size and buffer size for small large buffers. Used for smaller large network packets.

- **Default:** `25 buffers of 8192 bytes each.`
- **Allowed:** 0 or more

### Small large buffers: buffer size (BufferPool.SmallLarge.BufferSize)

`Network.ini › BufferPool.SmallLarge.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Large buffers: how many (BufferPool.Large.Size)

`Network.ini › BufferPool.Large.Size` · number · *Advanced*

Large Buffer Pool Size and buffer size for large buffers. Used for large network packets.

- **Default:** `25 buffers of 16384 bytes.`
- **Allowed:** 0 or more

### Large buffers: buffer size (BufferPool.Large.BufferSize)

`Network.ini › BufferPool.Large.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Very large buffers: how many (BufferPool.VeryLarge.Size)

`Network.ini › BufferPool.VeryLarge.Size` · number · *Advanced*

Very Large Buffer Pool Size and buffer size for very large buffers. Used for very large network packets.

- **Default:** `25 buffers of 24576 bytes each.`
- **Allowed:** 0 or more

### Very large buffers: buffer size (BufferPool.VeryLarge.BufferSize)

`Network.ini › BufferPool.VeryLarge.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Maximum buffers: how many (BufferPool.Maximum.Size)

`Network.ini › BufferPool.Maximum.Size` · number · *Advanced*

Maximum Buffer Pool Size and buffer size for maximum size buffers. Used for maximum sized network packets.

- **Default:** `10 buffers of 32768 bytes.`
- **Allowed:** 0 or more

### Maximum buffers: buffer size (BufferPool.Maximum.BufferSize)

`Network.ini › BufferPool.Maximum.BufferSize` · number · *Advanced*

- **Allowed:** 0 or more

### Security buffer pool: how many (BufferPool.Huge.Size)

`Network.ini › BufferPool.Huge.Size` · number · *Advanced* · *added by L2Everdream*

Part of a security protection added by L2Everdream. Leave it as shipped.

- **Allowed:** 0 or more

### Security buffer pool: buffer size (BufferPool.Huge.BufferSize)

`Network.ini › BufferPool.Huge.BufferSize` · number · *Advanced*

Part of a security protection added by L2Everdream. Leave it as shipped.

- **Allowed:** 0 or more

### Show the license screen after login (ShowLicence)

`Server.ini › ShowLicence` · on/off · *Advanced*

If False, the license (after the login) will not be shown.

- **Default:** On
- **Allowed:** On or Off

### Create an account automatically on first login (AutoCreateAccounts)

`Server.ini › AutoCreateAccounts` · on/off · *Advanced* · *added by L2Everdream*

Any new account name and password typed at the login screen becomes a new account.

- **Default:** On
- **Allowed:** On or Off
- **🔒 Set by the launcher:** The launcher keeps this on for a self-hosted world so you never need to register an account.

### Data folder (DatapackRoot)

`Server.ini › DatapackRoot` · text · *Advanced*

Datapack root directory. Defaults to current directory from which the server is started.


### Restart the login server on a schedule (LoginRestartSchedule)

`Server.ini › LoginRestartSchedule` · on/off · *Advanced*

Enable disable scheduled login restart.

- **Default:** Off
- **Allowed:** On or Off

### Restart the login server every (LoginRestartTime)

`Server.ini › LoginRestartTime` · number · hours · *Advanced*

Time in hours.

- **Default:** `24`
- **Allowed:** 0 or more hours

### Scheduled thread pool size (ScheduledThreadPoolSize)

`Threads.ini › ScheduledThreadPoolSize` · number · *Advanced*

Defines the number of threads in the scheduled thread pool. If set to -1, this will be determined by available processors multiplied by 4. You can specify a positive integer to manually set the pool size. Note that higher values can improve task handling under heavy load but may increase CPU and memory usage.


### Instant thread pool size (InstantThreadPoolSize)

`Threads.ini › InstantThreadPoolSize` · number · *Advanced*

Defines the number of threads in the instant thread pool. If set to -1, this will be determined by available processors multiplied by 2. You can specify a positive integer to manually set the pool size. Note that higher values can improve task handling under heavy load but may increase CPU and memory usage.


## Server console window

### Show the server window (EnableGUI)

`Interface.ini › EnableGUI` · on/off · *Advanced*

Enable L2jMobius GUI when OS supports it. Provides access to admin commands without the need to be online.

- **Default:** On
- **Allowed:** On or Off

### Dark server window (DarkTheme)

`Interface.ini › DarkTheme` · on/off · *Advanced*

Dark theme. Use a dark version of the Nimbus theme.

- **Default:** On
- **Allowed:** On or Off
