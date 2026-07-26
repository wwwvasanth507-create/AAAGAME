# HERO OF ETERNIA — GUILD SYSTEM DESIGN & MECHANICS

---

## 1. Overview & Guild Progression

The Guild System Engine (`GuildSystemManager.cs`) provides reusable long-term progression across multiple imperial guilds:

1. **The Crown Adventurers' Guild (`guild_adventurers`)**: Beast hunting, ruin exploration, and bounty contracts.
2. **The Arcane Circle of Mages (`guild_arcane_circle`)**: Spellcraft research, crystal alchemy, and relic analysis.
3. **Consortium of Iron Artisans (`guild_iron_artisans`)**: Master blacksmithing, mithril forging, and war supplies.

---

## 2. Ranks & Reputation Thresholds

| Guild Rank | Required Reputation Points | Rank Perks |
| :--- | :---: | :--- |
| **Recruit** | 0 | Access to basic guild bounty board & guild shop. |
| **Journeyman** | 200 | 10% discount at guild vendors, tier 2 guild quests. |
| **Master** | 500 | Guild master insignia item, tier 3 guild quests, respec discount. |
| **Grandmaster** | 1000 | Grandmaster title, exclusive guild abilities & recipe unlocks. |

---

## 3. AI Asset Production Report

### A. Guild Crest UI Icons
1. **Adventurers' Guild Crest (`icon_guild_adventurers`)**
   - **Resolution**: 512x512 PNG transparent.
   - **AI Prompt**: `"Game UI guild crest icon of a golden dragon head on a blue shield background, clean vector artwork, transparent background"`
   - **Folder Location**: `res://Assets/UI/Icons/icon_guild_adventurers.png`

---

## 4. Code & API Reference

Managed by `GuildSystemManager.cs` (`IInitializable` registered with `ServiceLocator`). Verified by `Chapter6SystemTests`.
