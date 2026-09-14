"""Builds catalog.json — the friendly layer over every config value the manager can edit.

Inputs
  research/seed-data/server-config-schema.json   every server ini key (generated from the install)
  catalog/friendly-names.tsv                      hand-curated names/units/notes (wins over the humanizer)
  catalog/client-settings.tsv                     hand-written client + world-profile entries

Output
  catalog/catalog.json                            loaded by the app (copied to its output folder)

Re-run after an engine update:  python catalog/build_catalog.py
"""
import csv
import json
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
SCHEMA = os.path.join(ROOT, "research", "seed-data", "server-config-schema.json")

# ---------------------------------------------------------------------------------------------
# Taxonomy: categories (left-nav top level) and their groups (left-nav second level).
# Grouped by WHAT A SETTING DOES, never by which file it lives in.
# ---------------------------------------------------------------------------------------------
CATEGORIES = [
    # id, name, glyph, scope, description, [(groupId, groupName)]
    ("world", "Your World", "✦", "server", "The L2Everdream world profile the launcher starts with.", [
        ("world.population", "Population"), ("world.rules", "World rules")]),
    ("rates", "Rates & Rewards", "◈", "server", "How fast characters level and how much monsters and quests give.", [
        ("rates.xp", "Experience & skill points"), ("rates.quest", "Quest rewards"), ("rates.drops", "Monster drops"),
        ("rates.dropgap", "Drops vs. level difference"), ("rates.vitality", "Vitality bonus"),
        ("rates.pets", "Pets"), ("rates.bossbonus", "Extra raid boss drops")]),
    ("characters", "Characters", "◉", "server", "Creating characters, levels, stats, limits and everyday rules.", [
        ("characters.new", "New characters"), ("characters.names", "Names & character slots"),
        ("characters.progress", "Levels & death"), ("characters.class", "Classes & subclasses"),
        ("characters.limits", "Stat limits"), ("characters.inventory", "Inventory & storage"),
        ("characters.loot", "Looting"), ("characters.party", "Parties"), ("characters.travel", "Travel & teleports"),
        ("characters.vitality", "Vitality system"), ("characters.summons", "Pets & summons"),
        ("characters.misc", "Other player rules")]),
    ("combat", "Skills & Combat", "⚔", "server", "Skills, buffs, damage and class balance.", [
        ("combat.skills", "Skills & learning"), ("combat.buffs", "Buffs & effects"), ("combat.damage", "Damage"),
        ("combat.balance", "Class balance multipliers"), ("combat.cancel", "Cancelled buff return")]),
    ("items", "Items & Enchanting", "◆", "server", "Enchanting, augmenting, crafting and items on the ground.", [
        ("items.enchant", "Enchanting"), ("items.augment", "Augmentation"), ("items.soulcrystal", "Soul crystals"),
        ("items.craft", "Crafting"), ("items.ground", "Dropped items"), ("items.transmog", "Transmogrification"),
        ("items.misc", "Other item rules")]),
    ("economy", "Economy & Trade", "⚖", "server", "Shops, private stores, offline trading, manor and money.", [
        ("economy.shops", "NPC shops & multisell"), ("economy.stores", "Private stores"),
        ("economy.offline", "Offline trade & craft"), ("economy.sellbuffs", "Selling buffs"),
        ("economy.manor", "Manor"), ("economy.banking", "Banking"), ("economy.buffer", "Scheme buffer"),
        ("economy.misc", "Other")]),
    ("pvp", "PvP & Karma", "☠", "server", "Player killing, karma, fame and PvP rewards.", [
        ("pvp.zones", "Peace zones"), ("pvp.karma", "Karma rules"), ("pvp.drops", "Item loss on death"),
        ("pvp.antifeed", "Anti-feed"), ("pvp.fame", "Fame"), ("pvp.rewards", "PvP rewards & announcements"),
        ("pvp.colors", "PvP title colors"), ("pvp.faction", "Faction system (Good vs Evil)"), ("pvp.misc", "Other")]),
    ("clans", "Clans & Sieges", "⚑", "server", "Clans, alliances, castles, fortresses, clan halls and sieges.", [
        ("clans.clan", "Clans & alliances"), ("clans.reputation", "Clan reputation points"),
        ("clans.siege", "Castle sieges"), ("clans.towers", "Castle tower spawns"), ("clans.castle", "Castle functions & fees"),
        ("clans.fort", "Fortress functions & fees"), ("clans.hall", "Clan hall functions & fees"),
        ("clans.hallsiege", "Contestable clan halls"), ("clans.mounts", "Mounts during sieges")]),
    ("olympiad", "Olympiad & Heroes", "♛", "server", "The Grand Olympiad schedule, rules and rewards.", [
        ("olympiad.general", "Olympiad"), ("olympiad.period", "Custom period")]),
    ("events", "Events & Activities", "✧", "server", "Seven Signs, Dimensional Rift, lottery, fishing, weddings and instances.", [
        ("events.sevensigns", "Seven Signs & Festival of Darkness"), ("events.rift", "Dimensional Rift"),
        ("events.lottery", "Lottery"), ("events.fishing", "Fishing championship"), ("events.auction", "Item auction"),
        ("events.wedding", "Weddings"), ("events.instances", "Instances"), ("events.cursed", "Cursed weapons")]),
    ("monsters", "Monsters & Bosses", "☬", "server", "Monster behaviour, champions, stat multipliers and boss spawns.", [
        ("monsters.general", "NPCs"), ("monsters.monsters", "Monsters"), ("monsters.champions", "Champion monsters"),
        ("monsters.stats", "NPC stat multipliers"), ("monsters.raids", "Raid bosses"), ("monsters.grand", "Grand bosses"),
        ("monsters.announce", "Boss announcements"), ("monsters.spawns", "Random spawns"), ("monsters.guards", "Guards")]),
    ("social", "Chat & Community", "✉", "server", "Chat channels, filters, Community Board, languages and messages.", [
        ("social.chat", "Chat"), ("social.board", "Community Board"), ("social.language", "Languages"),
        ("social.messages", "Welcome & info messages"), ("social.mail", "Mail")]),
    ("features", "Convenience Features", "✚", "server", "Optional helpers: auto-play, auto-potions, free mounts and service NPCs.", [
        ("features.autoplay", "Auto play"), ("features.autopotions", "Auto potions"), ("features.offlineplay", "Offline auto play"),
        ("features.premium", "Premium & PC Café points"), ("features.npcs", "Service NPCs"), ("features.mounts", "Free mounts"),
        ("features.fakeplayers", "Mobius fake players"), ("features.account", "Account")]),
    ("admin", "GM & Administration", "✪", "server", "Game Master powers, punishments and logging.", [
        ("admin.gm", "Game Masters"), ("admin.punish", "Punishments & jail"), ("admin.logging", "Logging & audits")]),
    ("protection", "Protection & Anti-cheat", "⛨", "server", "Flood protection, captcha, dual-boxing and login security.", [
        ("protection.flood", "Flood protection"), ("protection.captcha", "Captcha"), ("protection.dualbox", "Dual-box limits"),
        ("protection.hwid", "Hardware ID"), ("protection.login", "Login security"), ("protection.bots", "Bot protection")]),
    ("server", "Server & Performance", "⚙", "server", "Technical server settings. Most players never need these.", [
        ("server.network", "Network & ports"), ("server.general", "General"), ("server.restart", "Automatic restarts"),
        ("server.saving", "Saving & memory"), ("server.geodata", "Geodata & pathfinding"), ("server.threads", "Threads"),
        ("server.buffers", "Network buffers"), ("server.ids", "Object IDs"), ("server.database", "Database"),
        ("server.datapack", "Custom data"), ("server.dev", "Development & debugging"), ("server.login", "Login server"),
        ("server.interface", "Server console window")]),
    ("clientgfx", "Graphics", "▣", "client", "Resolution, detail, draw distance and effects in the game client.", [
        ("clientgfx.display", "Display"), ("clientgfx.detail", "Detail & effects"), ("clientgfx.distance", "Draw distance"),
        ("clientgfx.window", "Window")]),
    ("clientaudio", "Sound", "♪", "client", "Volume levels in the game client.", [("clientaudio.volume", "Volume")]),
    ("clientgame", "Gameplay & Interface", "⌘", "client", "Names above heads, chat, camera and interface options.", [
        ("clientgame.names", "Names shown"), ("clientgame.chat", "Chat & messages"), ("clientgame.camera", "Camera & controls"),
        ("clientgame.interface", "Interface"), ("clientgame.language", "Language & font")]),
    ("clientconn", "Connection & Login", "⇄", "client", "Where the client connects and automatic login.", [
        ("clientconn.server", "Server"), ("clientconn.autologin", "Automatic login")]),
]
GROUP_IDS = {g for c in CATEGORIES for g, _ in c[5]}

# ---------------------------------------------------------------------------------------------
# Routing: first matching rule wins. (file regex, section regex, key regex) -> group id
# ---------------------------------------------------------------------------------------------
ROUTES = [
    # ---- Rates.ini
    (r"Rates", r"Standard", r"RateKarma", "pvp.karma"),
    (r"Rates", r"Standard", r"RateSiegeGuardsPrice", "clans.siege"),
    (r"Rates", r"Standard", r"RateExtractable", "rates.drops"),
    (r"Rates", r"Standard", r"RateTeleportFee", "characters.travel"),
    (r"Rates", r"Standard", r"RateDropManor", "economy.manor"),
    (r"Rates", r"Standard", r".", "rates.xp"),
    (r"Rates", r"Quest", r"MonsterExpMaxLevelDifference", "rates.xp"),
    (r"Rates", r"Quest", r".", "rates.quest"),
    (r"Rates", r"Item Drop Rates", r".", "rates.drops"),
    (r"Rates", r"Level Difference", r".", "rates.dropgap"),
    (r"Rates", r"Vitality", r".", "rates.vitality"),
    (r"Rates", r"Player Drops", r".", "pvp.drops"),
    (r"Rates", r"Pets", r".", "rates.pets"),
    (r"Rates", r"raid bosses", r".", "rates.bossbonus"),
    # ---- Player.ini
    (r"Player", r"Statistics", r"Delevel|DecreaseSkill|DeathPenalty|Respawn", "characters.progress"),
    (r"Player", r"Statistics", r"Regen", "characters.progress"),
    (r"Player", r"Statistics", r"Weight|RunSpeed", "characters.limits"),
    (r"Player", r"Skills & Effects", r"Buff|Dance|Store|Effect|Toggle|FakeDeath|Divine", "combat.buffs"),
    (r"Player", r"Skills & Effects", r"Shield|Bow|Magic|CancelByHit", "combat.damage"),
    (r"Player", r"Skills & Effects", r".", "combat.skills"),
    (r"Player", r"Class, Sub-class", r"SubClass|Subclass|TransferSkills", "characters.class"),
    (r"Player", r"Class, Sub-class", r".", "combat.skills"),
    (r"Player", r"Summons", r".", "characters.summons"),
    (r"Player", r"Vitality", r".", "characters.vitality"),
    (r"Player", r"Limits", r"Slots|Freight|Warehouse", "characters.inventory"),
    (r"Player", r"Limits", r"PvtStore", "economy.stores"),
    (r"Player", r"Limits", r"Subclass", "characters.class"),
    (r"Player", r"Limits", r"MaximumPlayerLevel|MaxSp$|NpcTalk", "characters.progress"),
    (r"Player", r"Limits", r".", "characters.limits"),
    (r"Player", r"Enchanting", r".", "items.enchant"),
    (r"Player", r"Augmenting", r".", "items.augment"),
    (r"Player", r"Soul Crystal", r".", "items.soulcrystal"),
    (r"Player", r"Karma", r".", "pvp.karma"),
    (r"Player", r"Fame", r".", "pvp.fame"),
    (r"Player", r"Crafting", r".", "items.craft"),
    (r"Player", r"Clan", r"Newbie", "characters.new"),
    (r"Player", r"Clan", r".", "clans.clan"),
    (r"Player", r"Party", r".", "characters.party"),
    (r"Player", r"Initial", r".", "characters.new"),
    (r"Player", r"Other", r"AutoLoot|RaidLoot", "characters.loot"),
    (r"Player", r"Other", r"Teleport|Unstuck|Respawn|Offset", "characters.travel"),
    (r"Player", r"Other", r"PartyXp", "characters.party"),
    (r"Player", r"Other", r"MaxAdena", "economy.misc"),
    (r"Player", r"Other", r"Petition|Recommend|DeleteChar|ForbiddenNames|Silence|Tutorial|Keyboard", "characters.misc"),
    (r"Player", r"Other", r"SpawnProtection|DisconnectAfterDeath", "characters.progress"),
    (r"Player", r"Other", r"ExpertisePenalty", "items.misc"),
    (r"Player", r"Other", r"TriggerSkills", "combat.skills"),
    (r"Player", r"Other", r".", "characters.misc"),
    (r"Player", r"Damage", r".", "combat.damage"),
    # ---- General.ini
    (r"General", r"Administrator", r".", "admin.gm"),
    (r"General", r"Server Security", r"SkillCheck", "protection.bots"),
    (r"General", r"Server Security", r".", "admin.logging"),
    (r"General", r"Optimization", r"Drop|Destroy|Discard|ProtectedItems|Herb", "items.ground"),
    (r"General", r"Optimization", r"CorrectPrices|Multisell", "economy.shops"),
    (r"General", r"Optimization", r"Animation", "monsters.general"),
    (r"General", r"Optimization", r"QuestData", "server.saving"),
    (r"General", r"Optimization", r".", "server.saving"),
    (r"General", r"Falling", r".", "characters.misc"),
    (r"General", r"Skills & Effects", r".", "combat.buffs"),
    (r"General", r"Features", r"PeaceZone", "pvp.zones"),
    (r"General", r"Features", r"Chat", "social.chat"),
    (r"General", r"Features", r"Warehouse", "characters.inventory"),
    (r"General", r"Features", r"Refund|Wear", "economy.shops"),
    (r"General", r"Features", r".", "economy.misc"),
    (r"General", r"Instances", r".", "events.instances"),
    (r"General", r"Misc", r"CursedWeapons", "events.cursed"),
    (r"General", r"Misc", r"Community|BBS", "social.board"),
    (r"General", r"Misc", r"ServerNews", "social.messages"),
    (r"General", r"Misc", r"Chat", "social.chat"),
    (r"General", r"Misc", r".", "characters.misc"),
    (r"General", r"Manor", r".", "economy.manor"),
    (r"General", r"Lottery", r".", "events.lottery"),
    (r"General", r"Fishing", r".", "events.fishing"),
    (r"General", r"Auction", r".", "events.auction"),
    (r"General", r"Rift", r".", "events.rift"),
    (r"General", r"Punishment", r"Enchant", "combat.skills"),
    (r"General", r"Punishment", r".", "admin.punish"),
    (r"General", r"Custom Components", r".", "server.datapack"),
    # ---- Feature.ini
    (r"Feature", r"^Castle", r"SiegeHourList", "clans.siege"),
    (r"Feature", r"^Castle", r".", "clans.castle"),
    (r"Feature", r"Clan Hall", r".", "clans.hall"),
    (r"Feature", r"Fortress", r".", "clans.fort"),
    (r"Feature", r"Seven Signs", r".", "events.sevensigns"),
    (r"Feature", r"Reputation", r".", "clans.reputation"),
    (r"Feature", r"Mount", r".", "clans.mounts"),
    # ---- other game files
    (r"^Siege", r"Tower", r".", "clans.towers"),
    (r"^Siege", r".", r".", "clans.siege"),
    (r"ConquerableHallSiege", r".", r".", "clans.hallsiege"),
    (r"Olympiad", r"Custom", r".", "olympiad.period"),
    (r"Olympiad", r".", r".", "olympiad.general"),
    (r"PVP", r"Drop", r".", "pvp.drops"),
    (r"PVP", r"AntiFeed", r".", "pvp.antifeed"),
    (r"PVP", r".", r".", "pvp.misc"),
    (r"GrandBoss", r".", r".", "monsters.grand"),
    (r"^NPC", r"General", r".", "monsters.general"),
    (r"^NPC", r"Monsters", r".", "monsters.monsters"),
    (r"^NPC", r"Guards", r".", "monsters.guards"),
    (r"^NPC", r"Pets", r".", "characters.summons"),
    (r"^NPC", r"Raid", r".", "monsters.raids"),
    (r"FloodProtector", r".", r".", "protection.flood"),
    (r"GeoEngine", r".", r".", "server.geodata"),
    (r"IdManager", r".", r".", "server.ids"),
    (r"Development", r".", r".", "server.dev"),
    (r"Database", r".", r".", "server.database"),
    (r"Interface", r".", r".", "server.interface"),
    (r"Threads", r".", r".", "server.threads"),
    (r"Network", r"Buffer", r".", "server.buffers"),
    (r"Network", r".", r".", "server.threads"),
    (r"^Server", r"Networking", r".", "server.network"),
    (r"^Server", r"Misc Server", r".", "server.general"),
    (r"^Server", r"Deadlock|Restart", r".", "server.restart"),
    (r"^Server", r"HWID", r".", "protection.hwid"),
    (r"^Server", r"Misc Player", r".", "characters.names"),
    # ---- Custom/
    (r"AllowedPlayerRaces|StartingLocation|StartingTitle", r".", r".", "characters.new"),
    (r"AutoPlay", r".", r".", "features.autoplay"),
    (r"AutoPotions", r".", r".", "features.autopotions"),
    (r"Banking", r".", r".", "economy.banking"),
    (r"BossAnnouncements", r".", r".", "monsters.announce"),
    (r"CancelReturn", r".", r".", "combat.cancel"),
    (r"Captcha", r".", r".", "protection.captcha"),
    (r"ChampionMonsters", r".", r".", "monsters.champions"),
    (r"ChatModeration", r".", r".", "social.chat"),
    (r"ClassBalance", r".", r".", "combat.balance"),
    (r"CommunityBoard", r".", r".", "social.board"),
    (r"CustomMailManager", r".", r".", "social.mail"),
    (r"DelevelManager|NoblessMaster", r".", r".", "features.npcs"),
    (r"DualboxCheck", r".", r".", "protection.dualbox"),
    (r"FactionSystem", r".", r".", "pvp.faction"),
    (r"FakePlayers", r".", r".", "features.fakeplayers"),
    (r"FindPvP|PvpAnnounce|PvpRewardItem", r".", r".", "pvp.rewards"),
    (r"FreeMounts", r".", r".", "features.mounts"),
    (r"MerchantZeroSellPrice", r".", r".", "economy.shops"),
    (r"MultilingualSupport", r".", r".", "social.language"),
    (r"NpcStatMultipliers", r".", r".", "monsters.stats"),
    (r"OfflinePlay", r".", r".", "features.offlineplay"),
    (r"OfflineTrade", r".", r".", "economy.offline"),
    (r"OnlineInfo|ScreenWelcomeMessage|ServerTime", r".", r".", "social.messages"),
    (r"PasswordChange", r".", r".", "features.account"),
    (r"PremiumSystem", r".", r".", "features.premium"),
    (r"PrivateStoreRange", r".", r".", "economy.stores"),
    (r"PvpTitleColor", r".", r".", "pvp.colors"),
    (r"RandomSpawns", r".", r".", "monsters.spawns"),
    (r"SchemeBuffer", r".", r".", "economy.buffer"),
    (r"SellBuffs", r".", r".", "economy.sellbuffs"),
    (r"Transmog", r".", r".", "items.transmog"),
    (r"WalkerBotProtection", r".", r".", "protection.bots"),
    (r"WarehouseSorting", r".", r".", "characters.inventory"),
    (r"Wedding", r".", r".", "events.wedding"),
]
LOGIN_ROUTES = [
    (r"Server", r"Security", r".", "protection.login"),
    (r"Server", r"Networking", r".", "server.network"),
    (r"Database", r".", r".", "server.database"),
    (r".", r".", r".", "server.login"),
]

# Values the owner's launcher rewrites every time it starts the world. Shown read-only, with the reason.
MANAGED = {
    ("game", "Database.ini", "URL"): "Written by the L2Everdream launcher each time the world starts (points at the world's own database).",
    ("login", "Database.ini", "URL"): "Written by the L2Everdream launcher each time the world starts (points at the shared accounts database).",
    ("game", "Server.ini", "LoginPort"): "Written by the L2Everdream launcher each time the world starts (it picks a free port).",
    ("login", "Server.ini", "LoginPort"): "Written by the L2Everdream launcher each time the world starts (it picks a free port).",
    ("login", "Server.ini", "AutoCreateAccounts"): "The launcher keeps this on for a self-hosted world so you never need to register an account.",
}
# Keys whose name means nothing without the feature they belong to (the same key appears in several files).
GENERIC_KEYS = {"Enabled", "NpcId", "ItemId", "ItemCount", "RequiredItemId", "RequiredItemCount", "LevelRequirement"}

# Settings that can break the world if changed casually. Hidden until "Show advanced" is on.
ADVANCED_GROUPS = {"server.network", "server.general", "server.saving", "server.geodata", "server.threads", "server.buffers",
                   "server.ids", "server.database", "server.datapack", "server.dev", "server.login", "server.interface",
                   "protection.flood", "combat.balance", "clans.towers"}
ADVANCED_KEYS = {"AllowedProtocolRevisions", "DatapackRoot", "ScriptRoot", "RequestServerID", "AcceptAlternateID",
                 "EverybodyHasAdminRights"}

ACRONYMS = {
    "xp": "XP", "sp": "SP", "hp": "HP", "mp": "MP", "cp": "CP", "pvp": "PvP", "pk": "PK", "gm": "GM", "gms": "GMs",
    "npc": "NPC", "npcs": "NPCs", "id": "ID", "ids": "IDs", "ip": "IP", "hwid": "HWID", "html": "HTML", "gui": "GUI",
    "db": "database", "gk": "gatekeeper", "wh": "warehouse", "lvl": "level", "pvt": "private", "cc": "command channel",
    "atk": "Atk.", "def": "Def.", "p": "P.", "m": "M.", "ng": "no-grade", "mid": "mid-grade", "bbs": "Community Board",
    "sql": "SQL", "cpu": "CPU", "dp": "datapack", "ms": "ms", "l2": "L2", "ssq": "Seven Signs", "ai": "AI",
    "min": "minimum", "max": "maximum", "alt": "", "custom": "custom", "hero": "hero", "mysql": "MySQL", "oly": "Olympiad",
    "olys": "Olympiad", "exp": "EXP", "pc": "PC", "tp": "teleport", "hw": "hardware", "geodata": "geodata",
}


WORD_FIXES = [
    (r"\bP\. (?!Atk|Def)", "P "), (r"\bM\. (?!Atk|Def)", "M "), (r"\bpdef\b", "P. Def."), (r"\bmdef\b", "M. Def."),
    (r"\bware house\b", "warehouse"), (r"\braidboss\b", "raid boss"), (r"\bgameserver\b", "game server"),
    (r"\bsub class\b", "subclass"), (r"\bL2 event\b", "event"), (r"\bmy SQL\b", "MySQL"), (r"\bdualbox\b", "dual-box"),
    (r"\bnum\b", "number"), (r"\bchar\b", "character"), (r"\bmsg\b", "message"), (r"\bregen\b", "regeneration"),
    (r"\bmult\b", "multiplier"), (r"\bmultipiler\b", "multiplier"), (r"\bmultipler\b", "multiplier"),
    (r"\bmimimum\b", "minimum"), (r"\bcontrib\b", "contribution"), (r"\baquire\b", "acquire"), (r"\bleaved\b", "leaving"),
    (r"\bcatacumbs\b", "catacombs"), (r"\bspd\b", "speed"), (r"\bfpc\b", "fake players"), (r"\blang\b", "language"),
    (r"\brew\b", "reward"), (r"\bcomp\b", "competition"), (r"\bgp\b", "GP"), (r"\bcname\b", "character name"),
    (r"\bhtm\b", "HTML"), (r"\bolympiad\b", "Olympiad"), (r"\bseven signs\b", "Seven Signs"), (r"\badenas?\b", "adena"),
    (r"\bpve\b", "PvE"), (r"\bcancel return\b", "return cancelled buffs"), (r"\bcp\b", "CP"),
]


def humanize(key):
    key = re.sub(r"PvP|Pvp", "Pvp", key)
    key = re.sub(r"PvE|Pve", "Pve", key)
    parts = re.findall(r"[A-Z]+(?=[A-Z][a-z]|\d|\b)|[A-Z]?[a-z]+|[A-Z]+|\d+", key.replace(".", " ").replace("_", " "))
    words = []
    for p in parts:
        low = p.lower()
        if low in ACRONYMS:
            if ACRONYMS[low]:
                words.append(ACRONYMS[low])
        else:
            words.append(low)
    text = " ".join(words).strip()
    for pat, rep in WORD_FIXES:
        text = re.sub(pat, rep, text, flags=re.I)
    return (text[:1].upper() + text[1:]) if text else key


# ---------------------------------------------------------------------------------------------
# Pattern names for large families of near-identical keys (checked before the humanizer).
# ---------------------------------------------------------------------------------------------
FLOOD_ACTIONS = {
    "UseItem": "Using items", "RollDice": "Rolling dice", "ItemPetSummon": "Summoning pets", "HeroVoice": "Hero voice chat",
    "GlobalChat": "Global chat", "Subclass": "Changing subclass", "DropItem": "Dropping items", "EnchantItem": "Enchanting",
    "ServerBypass": "NPC dialog clicks", "MultiSell": "Multisell exchanges", "Transaction": "Trades", "Manufacture": "Private workshop",
    "SendMail": "Sending mail", "CharacterSelect": "Character selection", "ItemAuction": "Item auction bids", "PlayerAction": "Player actions",
}
FLOOD_SUFFIX = {
    "Interval": "minimum time between uses", "LogFlooding": "log flooding attempts", "PunishmentLimit": "attempts before punishment",
    "PunishmentType": "punishment", "PunishmentTime": "punishment length",
}
FUNCTIONS = {
    "Teleport": "Teleport", "Support": "Buff support", "MpRegeneration": "MP regeneration", "HpRegeneration": "HP regeneration",
    "ExpRegeneration": "EXP recovery", "ItemCreation": "Item creation", "Curtain": "Curtains", "FrontPlatform": "Front platform",
}
PLACE = {"Castle": "Castle", "ClanHall": "Clan hall", "Fortress": "Fortress"}


def ordinal_rank(n):
    return f"#{n}"


def pattern_name(key):
    m = re.fullmatch(r"FloodProtector(\w+?)(Interval|LogFlooding|PunishmentLimit|PunishmentType|PunishmentTime)", key)
    if m and m.group(1) in FLOOD_ACTIONS:
        return f"{FLOOD_ACTIONS[m.group(1)]}: {FLOOD_SUFFIX[m.group(2)]}"
    m = re.fullmatch(r"(Castle|ClanHall|Fortress)(\w+?)(?:Function)?Fee(Ratio|Lvl(\d+))", key)
    if m and m.group(2) in FUNCTIONS:
        what = f"{PLACE[m.group(1)]} {FUNCTIONS[m.group(2)].lower()}"
        return f"{what}: fee payment period" if m.group(3) == "Ratio" else f"{what}: fee at level {m.group(4)}"
    m = re.fullmatch(r"(Outer|Inner)?(Door|Wall|Trap)UpgradePriceLvl(\d)", key)
    if m:
        part = (m.group(1) + " door") if m.group(1) else m.group(2).lower()
        return f"Castle {part.lower()} upgrade price (level {m.group(3)})"
    m = re.fullmatch(r"(Gludio|Giran|Dion|Oren|Aden|Innadril|Goddard|Rune|Schuttgart)(FlameTower|ControlTower)(\d)", key)
    if m:
        return f"{m.group(1)}: {'flame' if m.group(2) == 'FlameTower' else 'control'} tower {m.group(3)} location"
    m = re.fullmatch(r"(Gludio|Giran|Dion|Oren|Aden|Innadril|Goddard|Rune|Schuttgart)MaxMercenaries", key)
    if m:
        return f"{m.group(1)}: maximum mercenaries"
    m = re.fullmatch(r"(\d+)(?:st|nd|rd|th)RaidRankingPoints", key)
    if m:
        return f"Raid ranking {ordinal_rank(m.group(1))}: clan reputation reward"
    m = re.fullmatch(r"UpTo(\d+)thRaidRankingPoints", key)
    if m:
        lo = {"50": "11", "100": "51"}[m.group(1)]
        return f"Raid ranking #{lo}–{m.group(1)}: clan reputation reward"
    m = re.fullmatch(r"ClanLevel(\d+)(Cost|Requirement)", key)
    if m:
        return f"Clan level {m.group(1)}: {'reputation cost' if m.group(2) == 'Cost' else 'members required'}"
    m = re.fullmatch(r"IntervalOf(\w+)Spawn", key)
    if m:
        return f"{humanize(m.group(1))}: respawn interval"
    m = re.fullmatch(r"RandomOf(\w+)Spawn", key)
    if m:
        return f"{humanize(m.group(1))}: random extra respawn time"
    m = re.fullmatch(r"(Antharas|Valakas)WaitTime", key)
    if m:
        return f"{m.group(1)}: time before the fight starts"
    m = re.fullmatch(r"(Antharas|Valakas|Baium)RecognizeHero", key)
    if m:
        return f"{m.group(1)}: greets heroes who enter"
    m = re.fullmatch(r"BufferPool\.(\w+)\.(Size|BufferSize)", key)
    if m:
        return f"{humanize(m.group(1))} buffers: {'how many' if m.group(2) == 'Size' else 'buffer size'}"
    m = re.fullmatch(r"(Pve|Pvp)(\w+)Multipliers", key)
    if m:
        return f"{'PvE' if m.group(1) == 'Pve' else 'PvP'}: {humanize(m.group(2)).lower()} by class"
    m = re.fullmatch(r"(Monster|Raidboss|Guard|Defender)(HP|MP|PAtk|MAtk|PDef|MDef|AggroRange|ClanHelpRange)", key)
    if m:
        stat = {"HP": "HP", "MP": "MP", "PAtk": "P. Atk.", "MAtk": "M. Atk.", "PDef": "P. Def.", "MDef": "M. Def.",
                "AggroRange": "aggro range", "ClanHelpRange": "help-call range"}[m.group(2)]
        who = {"Monster": "Monsters", "Raidboss": "Raid bosses", "Guard": "Guards", "Defender": "Siege defenders"}[m.group(1)]
        return f"{who}: {stat} multiplier"
    m = re.fullmatch(r"PremiumRate(\w+)", key)
    if m:
        return f"Premium bonus: {humanize(m.group(1)).lower()} rate"
    m = re.fullmatch(r"RateVitalityLevel(\d)", key)
    if m:
        return f"Vitality level {m.group(1)}: XP multiplier"
    return None


# Nobody is named anywhere in the manager. Config comments credit or quote people in a few recurring shapes;
# those shapes are removed without the generator itself having to list anyone.
ATTRIBUTION_PARENS = re.compile(r"\s*\([^()]*(?:\d{4}-\d{2}-\d{2}|\b[a-z0-9_]+ (?:hit|saw|found|reported) )[^()]*\)")
ATTRIBUTION_LINE = re.compile(
    r"\b[A-Z][a-z]+(?:'s)?\s*:\s*[\"“]"                                   # Name: "quote"
    r"|\b[A-Z][a-z]+ (?:asked|ruled|reported|said|wants|requested|spotted|found|took|chose)\b"  # Name asked ...
    r"|[—-]\s*[a-z0-9_]{3,},\s*(?:live|creator)"                            # — handle, live
    r"|\bAT [A-Z]+'S REQUEST\b|\b[A-Z][a-z]+'s (?:request|call|ruling)\b",
)


def scrub_names(text):
    """Removes attributions, quotes and bylines so no person is named."""
    text = ATTRIBUTION_PARENS.sub("", text or "")
    return "\n".join(l for l in text.splitlines() if not ATTRIBUTION_LINE.search(l))


def clean_description(text):
    text = scrub_names(text)
    text = re.sub(r"<[^>]+>", "", text or "")
    text = re.sub(r"^WARNING:\s*", "Warning: ", text, flags=re.M)
    lines = [l.strip() for l in text.splitlines()]
    lines = [l for l in lines if l and not re.match(r"^(Notes?:)$", l) and not CUSTOM_NOISE.search(l)]
    return "\n".join(lines).strip()


# Internal dev-ticket chatter in L2SP comments is not player-facing.
CUSTOM_NOISE = re.compile(r"\[\[[A-Z]+-\d+\]\]|🔴|⚠️|scripts/|docs/|\.java|\.py\b|Expectation row|drift checker|patch\b", re.I)

UNIT_PATTERNS = [
    (re.compile(r"\bmillisecond|\bin ms\b|\(ms\)|1000 = 1 second", re.I), "ms"),
    (re.compile(r"\bin seconds\b|\bseconds\b|\(sec", re.I), "seconds"),
    (re.compile(r"\bin minutes\b|\bminutes\b", re.I), "minutes"),
    (re.compile(r"\bin hours\b|\bhours?\b", re.I), "hours"),
    (re.compile(r"\bdays\b", re.I), "days"),
    (re.compile(r"percent|%", re.I), "%"),
]


def infer_editor(entry, description):
    value = entry["value"]
    jt = (entry.get("javaType") or "").lower()
    desc_all = entry.get("description") or ""
    opts = None
    m = re.search(r"(?:Available Options|Accepted Values)\s*:\s*(.+)", desc_all, re.I)
    if m:
        opts = [o.strip() for o in re.split(r",", m.group(1)) if o.strip()]
    if not opts:
        pairs = re.findall(r"^\s*(\d+)\s*=\s*(.+)$", desc_all, re.M)
        if len(pairs) >= 2 and re.fullmatch(r"-?\d+", value):
            opts = [f"{k}={v.strip()}" for k, v in pairs]
    if not opts:
        pairs = re.findall(r"^\s*([A-Z][A-Z_]{2,})\s+-\s+(.+)$", desc_all, re.M)
        if len(pairs) >= 2 and re.fullmatch(r"[A-Z_]+", value):
            opts = [f"{k}={v.strip()}" for k, v in pairs]
    if jt == "boolean" or value.lower() in ("true", "false"):
        return "toggle", "bool", None
    if opts:
        return "choice", "string", opts
    if jt in ("int", "long", "byte") or (not jt and re.fullmatch(r"-?\d+", value)):
        return "number", "int", None
    if jt in ("float", "double") or (not jt and re.fullmatch(r"-?\d+\.\d*|-?\d*\.\d+", value)):
        return "number", "decimal", None
    if re.search(r"[;,]", value) or re.search(r"list|separated|format", desc_all, re.I):
        return "list", "string", None
    return "text", "string", None


def infer_unit(key, description, value_type):
    if value_type not in ("int", "decimal"):
        return None
    if re.search(r"Rate|Multiplier|Multipler|Ratio", key) and value_type == "decimal":
        return "times"
    if re.search(r"Chance|Percent|Percentage", key):
        return "%"
    for pat, unit in UNIT_PATTERNS:
        if unit == "%" and re.search(r"10\s*=\s*1\s*%", description or ""):
            continue
        if pat.search(description or ""):
            return unit
    return None


# ---------------------------------------------------------------------------------------------
# Valid values. Every setting gets the tightest limits we can justify from its Java type, its unit,
# its name and the shape of the value the release ships. The app refuses anything outside them.
# Formats are checked in L2Config.Core SettingValues.ValidateFormat.
# ---------------------------------------------------------------------------------------------
JAVA_RANGES = {"Byte": (-128, 127), "Int": (-2147483648, 2147483647), "Long": (-9223372036854775808, 9223372036854775807)}

KNOWN_LIMITS = {
    # key: (min, max)
    "CharMaxNumber": (1, 7), "MaximumPlayerLevel": (1, 80), "StartingLevel": (1, 80), "BaseSubclassLevel": (1, 80),
    "MaxSubclassLevel": (1, 80), "MaxSubclass": (0, 3), "RiftMinPartySize": (2, 9), "MaxRiftJumps": (1, 9),
    "MaximumWarehouseSlotsForDwarf": (1, 299), "MaximumWarehouseSlotsForNoDwarf": (1, 299), "MaximumWarehouseSlotsForClan": (1, 299),
    "MultisellAmountLimit": (1, 999999), "RateTeleportFee": (0.01, 100), "ServerListAge": (0, 18),
    "MaxBuffAmount": (1, 40), "MaxDanceAmount": (0, 40), "AltMaxNumOfClansInAlly": (1, 20), "MaxAdena": (-1, 2147483647),
    "MaxSp": (-1, 2147483647), "GameserverPort": (1, 65535), "LoginPort": (1, 65535), "LoginserverPort": (1, 65535),
    "OlympiadStartTime": (0, 23), "OlympiadMin": (0, 59), "AltManorRefreshTime": (0, 23), "AltManorRefreshMin": (0, 59),
    "AltManorApproveTime": (0, 23), "AltManorApproveMin": (0, 59), "MinimumChatLevel": (0, 80), "PeaceZoneMode": (0, 2),
    "AltLottery5NumberRate": (0, 1), "AltLottery4NumberRate": (0, 1), "AltLottery3NumberRate": (0, 1),
    "ThreadPriority": (1, 10), "MaxPlayersPerHWID": (0, 100), "RequestServerID": (1, 127), "SiegeClanMinLevel": (0, 11),
    "MinClanLevel": (0, 11), "AutoPotionMinimumLevel": (1, 80), "ChampionMinLevel": (1, 99), "ChampionMaxLevel": (1, 99),
    "MaxPCritRate": (0, 10000), "MaxMCritRate": (0, 10000),
}

KNOWN_FORMATS = {
    "ServerRestartSchedule": "time-list", "ServerRestartDays": "weekday-list", "OlympiadCompetitionDays": "weekday-list",
    "CnameTemplate": "regex", "PetNameTemplate": "regex", "ClanNameTemplate": "regex",
    "RetailLikeAugmentationNoGradeChance": "percent-split-4", "RetailLikeAugmentationMidGradeChance": "percent-split-4",
    "RetailLikeAugmentationHighGradeChance": "percent-split-4", "RetailLikeAugmentationTopGradeChance": "percent-split-4",
    "PartyXpCutoffGaps": "range-list", "PartyXpCutoffGapPercent": "int-list;", "AllowedProtocolRevisions": "int-list;",
    "PathFindBuffers": "buffer-list", "BossDropList": "boss-drop-list", "SiegeHourList": "int-list",
    "LoginHost": "host", "GameserverHostname": "host", "LoginserverHostname": "host", "LoginHostname": "host",
    "BanChatChannels": "word-list;", "MultiLangAllowed": "word-list;", "ForbiddenNames": "word-list",
    "ExcludedPacketList": "word-list", "DualboxCheckWhitelist": "ip-list",
}

# Keys whose value picks from a fixed set the comments describe only loosely.
KNOWN_CHOICES = {
    r"FloodProtector\w+PunishmentType": ["none=No punishment", "kick=Kick", "ban=Ban", "jail=Jail"],
    r"OlympiadPeriod": ["MONTH=Month", "WEEK=Week", "DAY=Day"],
    r"ThreadPriority": None,
}


def infer_limits(entry, key, desc_all, editor, value_type, unit, options):
    """Returns (min, max, format, options) for a server setting."""
    value = entry["value"]
    java = entry.get("javaType")
    lo = hi = fmt = None
    for pattern, choices in KNOWN_CHOICES.items():
        if choices and re.fullmatch(pattern, key):
            return None, None, None, choices
    if value_type in ("int", "decimal"):
        lo, hi = JAVA_RANGES.get(java, (None, None))
        stock_default = entry.get("javaDefault") or entry.get("documentedDefault") or ""
        negative_ok = (value.startswith("-") or str(stock_default).startswith("-")
                       or re.search(r"(^|\s)-1\b|negative|unlimited", desc_all or "", re.I))
        if not negative_ok:
            lo = 0
        if unit == "%" and not re.search(r"Multiplier|Regen|Raid[PM]", key):
            hi = 100
        if re.search(r"Port$", key):
            lo, hi = 1, 65535
        if unit == "hour":
            lo, hi = 0, 23
        if unit == "minute":
            lo, hi = 0, 59
        if key in KNOWN_LIMITS:
            lo, hi = KNOWN_LIMITS[key]
        if value_type == "decimal" and java not in ("Float", "Double"):
            pass
        return lo, hi, None, options
    if editor in ("list", "text"):
        if key in KNOWN_FORMATS:
            return None, None, KNOWN_FORMATS[key], options
        if re.search(r"Color", key) and re.fullmatch(r"[0-9A-Fa-f]{6}", value or ""):
            return None, None, "hex-color", options
        if re.fullmatch(r"-?\d+,-?\d+,-?\d+(,-?\d+)?", value or "") and re.search(r"Location|Tower|Loc$", key):
            return None, None, "coordinates", options
        if re.fullmatch(r"(\d+,\d+(\.\d+)?;)*(\d+,\d+(\.\d+)?;?)?", value or "") and re.search(r"ByItemId|DurationList|ReuseList|RespawnTime", key):
            return None, None, "pair-list", options
        if re.fullmatch(r"\s*\d+(\s*,\s*\d+)*\s*", value or "") or (value == "" and re.search(r"Ids?$|IdsList|List$|Items$|BlackList", key)):
            if re.search(r"Ids?$|IdsList|List$|Items$|BlackList|ItemIds|Protected", key):
                return None, None, "int-list", options
    return None, None, None, options


TARGET_ALIASES = {"game": "server-game", "login": "server-login", "option": "client-option", "l2ini": "client-l2ini", "world": "world-profile"}
ONLY_IF = re.compile(r"(?:works only|only (?:works|takes effect|used)?|used only|effective only|only)\s*(?:if|when|with)\s*`?([A-Z][A-Za-z0-9]+)`?\s*(?:=|is)\s*`?(true|false|enabled)`?", re.I)
ENABLE_LIKE = re.compile(r"^(Enable|Allow)[A-Z]|(Enable|Enabled)$")


def add_relations(settings, schema):
    """Adds "relations": how other settings change what this one does (curated first, then derived)."""
    by_key = {(s["target"], s["file"], s["key"]): s for s in settings}
    by_target_key = {}
    for st in settings:
        by_target_key.setdefault((st["target"], st["key"]), []).append(st)
    for st in settings:
        st["relations"] = []

    def resolve(ref):
        alias, file, key = ref.split(":", 2)
        return by_key.get((TARGET_ALIASES[alias], file, key))

    def add(source, other, kind, value, note):
        if source is other or any(r["id"] == other["id"] and r["kind"] == kind for r in source["relations"]):
            return
        source["relations"].append({"id": other["id"], "kind": kind, "value": value, "note": note or None})

    bad = []
    for row in load_tsv("setting-relations.tsv"):
        source, other = resolve(row["setting"]), resolve(row["other"])
        if not source or not other:
            bad.append(row["setting"] + " -> " + row["other"])
            continue
        add(source, other, row["kind"], row["value"] or None, row["note"])
    if bad:
        print("setting-relations.tsv rows that match no setting:", *bad, sep="\n  ")

    # "Works only if X = True" in the stock or release comments.
    for e in schema:
        source = by_key.get(("server-" + e["server"], e["file"], e["key"]))
        if not source:
            continue
        for match in ONLY_IF.finditer(e["description"] or ""):
            candidates = by_target_key.get((source["target"], match.group(1)), [])
            if len(candidates) == 1 and candidates[0]["editor"] == "toggle":
                add(source, candidates[0], "requires", "false" if match.group(2).lower() == "false" else "true", None)

    # Custom feature files: the first on/off switch named Enable…/…Enabled/Allow… turns the whole feature on.
    files = {}
    for st in settings:
        if st["target"] == "server-game" and st["file"].startswith("Custom/"):
            files.setdefault(st["file"], []).append(st)
    for file_settings in files.values():
        master = next((st for st in file_settings if st["editor"] == "toggle" and ENABLE_LIKE.search(st["key"])), None)
        if master is None or master is not file_settings[0]:
            continue
        for st in file_settings[1:]:
            if not any(r["kind"] == "requires" for r in st["relations"]):
                add(st, master, "requires", "true", None)


def load_tsv(name):
    path = os.path.join(HERE, name)
    if not os.path.exists(path):
        return []
    rows = []
    with open(path, encoding="utf-8", newline="") as fh:
        lines = [l for l in fh if l.strip() and not l.lstrip().startswith("#")]
    for r in csv.DictReader(lines, delimiter="|", quoting=csv.QUOTE_NONE):
        rows.append({k.strip(): (v or "").strip() for k, v in r.items() if k})
    return rows


def route(server, file, section, key):
    table = ROUTES if server == "game" else LOGIN_ROUTES
    fbase = file.split("/")[-1].rsplit(".", 1)[0]
    for fr, sr, kr, gid in table:
        if re.search(fr, fbase) and (sr == "." or re.search(sr, section or "", re.I)) and re.search(kr, key):
            return gid
    return None


def stock_comments():
    """Stock L2J Mobius comments per (server, file, key), used instead of the release's internal notes."""
    import glob
    roots = sorted(glob.glob(os.path.join(ROOT, "research", "upstream-mobius", "*")))
    result = {}
    if not roots:
        return result
    for server in ("game", "login"):
        base = os.path.join(roots[-1], server, "config")
        for path in glob.glob(os.path.join(base, "**", "*.ini"), recursive=True):
            rel = os.path.relpath(path, base).replace("\\", "/")
            comments = []
            for line in open(path, encoding="utf-8", errors="replace").read().splitlines():
                s = line.strip()
                if s.startswith("#"):
                    text = s.lstrip("#").strip()
                    if text and not re.fullmatch(r"-{3,}", text) and not re.match(r"(?:Default|Retail)\s*:", text, re.I):
                        comments.append(text)
                elif "=" in s:
                    result[(server, rel, s.split("=", 1)[0].strip())] = "\n".join(comments)
                    comments = []
                elif not s:
                    comments = []
    return result


def main():
    schema = json.load(open(SCHEMA, encoding="utf-8"))
    stock = stock_comments()
    names = {(r["target"], r["file"], r["key"]): r for r in load_tsv("friendly-names.tsv")}
    settings, unrouted = [], []
    for e in schema:
        gid = route(e["server"], e["file"], e["section"], e["key"])
        if not gid:
            unrouted.append(f'{e["server"]}/{e["file"]} [{e["section"]}] {e["key"]}')
            continue
        target = "server-" + e["server"]
        # Descriptions use the stock L2J Mobius wording whenever the setting exists upstream, so the release's
        # internal notes never leak into the editor; the reasons for L2Everdream's changes live in Custom Config.
        raw = stock.get((e["server"], e["file"], e["key"]), e["description"])
        desc = clean_description(raw)
        editor, vtype, options = infer_editor(e, desc)
        over = names.get((target, e["file"], e["key"]), {})
        unit = over.get("unit") or infer_unit(e["key"], e["description"], vtype)
        lo, hi, fmt, options = infer_limits(e, e["key"], e["description"], editor, vtype, unit, options)
        if options and editor != "toggle":
            editor = "choice"
        name = over.get("name") or pattern_name(e["key"]) or humanize(e["key"])
        if e["key"] in GENERIC_KEYS and not over.get("name"):
            name = f'{e["file"].split("/")[-1][:-4]}: {name[:1].lower() + name[1:]}'
        settings.append({
            "id": f'{target}:{e["file"]}:{e["key"]}',
            "target": target,
            "file": e["file"],
            "key": e["key"],
            "category": gid.split(".")[0],
            "group": gid,
            "name": name,
            "description": over.get("description") or desc,
            "editor": over.get("editor") or editor,
            "valueType": vtype,
            "options": options,
            "unit": unit,
            "min": lo,
            "max": hi,
            "format": fmt,
            "default": re.sub(r"^(-?\d+(?:\.\d*)?)[fdlFDL]$", r"\1", e["javaDefault"]) if e["javaDefault"] not in (None, "None") else e["documentedDefault"],
            "shippedValue": e["value"],
            "advanced": gid in ADVANCED_GROUPS or e["key"] in ADVANCED_KEYS,
            "managedReason": MANAGED.get((e["server"], e["file"], e["key"])) or
                             (MANAGED.get((e["server"], e["file"], "URL")) if e["file"] == "Database.ini" else None),
            "l2everdream": e["l2spAnnotated"],
        })
    for r in load_tsv("client-settings.tsv"):
        settings.append({
            "id": f'{r["target"]}:{r["file"]}:{r["section"]}:{r["key"]}',
            "target": r["target"], "file": r["file"], "section": r["section"], "key": r["key"],
            "category": r["group"].split(".")[0], "group": r["group"], "name": r["name"],
            "description": r["description"], "editor": r["editor"], "valueType": r["valueType"],
            "options": [o.strip() for o in r["options"].split(";") if o.strip()] or None, "unit": r["unit"] or None,
            "min": float(r["min"]) if r["min"] else None, "max": float(r["max"]) if r["max"] else None, "format": None,
            "default": r["default"] or None, "shippedValue": None, "advanced": r["advanced"] == "1",
            "managedReason": r["managedReason"] or None, "secret": r.get("secret") == "1", "l2everdream": False,
        })
    add_relations(settings, schema)
    known = {(s["target"], s["file"], s["key"]) for s in settings}
    stale = [k for k in names if k not in known]
    if stale:
        print("friendly-names.tsv rows that match no setting (typo, or removed from the engine):")
        for t, f, k in stale:
            print(f"  {t}|{f}|{k}")
    missing = [s["group"] for s in settings if s["group"] not in GROUP_IDS]
    if missing or unrouted:
        print("unknown groups:", sorted(set(missing)))
        print("unrouted:", *unrouted, sep="\n  ")
        sys.exit(1)
    used = {s["group"] for s in settings}
    categories = [{
        "id": cid, "name": name, "glyph": glyph, "scope": scope, "description": desc,
        "groups": [{"id": g, "name": gn} for g, gn in groups if g in used],
    } for cid, name, glyph, scope, desc, groups in CATEGORIES]
    categories = [c for c in categories if c["groups"]]
    out = {"version": 1, "categories": categories, "settings": settings}
    with open(os.path.join(HERE, "catalog.json"), "w", encoding="utf-8") as fh:
        json.dump(out, fh, indent=1, ensure_ascii=False)
    curated = sum(1 for s in settings if (s["target"], s["file"], s["key"]) in names or s["target"].startswith(("client", "world")))
    print(f"{len(settings)} settings in {len(categories)} categories; curated names: {curated}")
    if "--dump-names" in sys.argv:
        for s in settings:
            print(f'{s["group"]:24} {s["file"]:28} {s["key"]:44} {s["name"]}')


if __name__ == "__main__":
    main()
