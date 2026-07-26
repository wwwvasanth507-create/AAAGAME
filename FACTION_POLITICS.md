# HERO OF ETERNIA — FACTION POLITICS SYSTEM DOCUMENTATION

---

## 1. Overview & Mechanics

The Faction Politics Engine in Act II governs the power struggle between the three major regional powers of Eternia:

1. **The Iron Vanguard (`faction_iron_vanguard`)**: Military order focused on law, fortification, and defense against Void incursions.
2. **The Silver Syndicate (`faction_silver_syndicate`)**: Wealthy merchant guild controlling trade routes, harbors, and exotic commerce.
3. **The Sylvan Circle (`faction_sylvan_circle`)**: Druidic herbalists and elemental alchemists preserving ancient forests and sacred grottos.

---

## 2. Dynamic Influence & Alliances

- **Influence Scores**: Dynamic 0 to 100 scale per faction per region.
- **Territory Disputes**: Conceding or liberating outposts dynamically adjusts regional faction dominance.
- **Alliance Pathing**: Players can form official alliances with factions upon reaching influence thresholds, unlocking faction-exclusive merchants, crafting recipes, and companion bonuses.

---

## 3. AI Asset Production Report

### A. Faction Crest Icons
1. **Iron Vanguard Crest (`icon_faction_iron_vanguard`)**
   - **Resolution**: 512x512 PNG transparent.
   - **AI Prompt**: `"Game UI faction crest icon of a heavy silver shield with crossed steel longswords on a dark crimson field, clean sharp vector graphic"`
   - **Folder Location**: `res://Assets/UI/Icons/icon_faction_iron_vanguard.png`

---

## 4. Code & API Reference

Managed by `FactionPoliticsManager.cs` implementing `IInitializable` registered with `ServiceLocator`. Verified by `Act2SystemTests`.
