# Your World settings

The L2Everdream world profile the launcher starts with.

**5 settings** in the **Your World** category of the Server tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Population

### World sims (simsPercent)

`world-profile.json › simsPercent` · slider · %

How many simulated players fill your world, compared with the full shipped world. 100% is the world as intended; lower it if your computer struggles.

- **Default:** `100`
- **Allowed:** 25 – 100 %

### Offline shops (shopsPercent)

`world-profile.json › shopsPercent` · slider · %

How many sim-run offline shops appear in towns. Above 100% needs "Allow more shops than shipped".

- **Default:** `100`
- **Allowed:** 0 – 200 %
- **Needs:** [Allow more shops than shipped](Settings-Your-World#allow-more-shops-than-shipped-shopsoverarmed) to be On — Above 100% only works while "Allow more shops than shipped" is on; the launcher caps it at 100% otherwise.

### Allow more shops than shipped (shopsOverArmed)

`world-profile.json › shopsOverArmed` · on/off

Lets the offline shops setting go up to 200%. More shops use more memory.

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Offline shops](Settings-Your-World#offline-shops-shopspercent) — it only works while this is On

## World rules

### World name (name)

`world-profile.json › name` · text

The name of your saved world. The launcher derives the world's database from this name, so it can only be changed in the launcher.

- **🔒 Set by the launcher:** Renaming the world would point it at a different, empty database. Rename it in the launcher instead.

### Free class change (freeClassChange)

`world-profile.json › freeClassChange` · on/off

On: class changes at level 20, 40 and 76 are free from the Class Masters. Off: the retail class-change quests are required. The launcher writes the matching ClassMaster.xml when the world starts.

- **Default:** On
- **Allowed:** On or Off
