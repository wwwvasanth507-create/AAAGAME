# Workstation System — Hero of Eternia

> **Version:** 1.0.0  
> **Phase:** Prompt 15 / 150  
> **Status:** Production Ready

---

## 1. Overview

The Workstation System provides 16 data-driven workstation definitions with tiered bonuses. Workstations are required for specific crafting recipes and provide speed, quality, success rate, experience, and cost reduction bonuses.

---

## 2. Architecture

```
Scripts/Crafting/
└── WorkstationFramework.cs  ← WorkstationDefinition, WorkstationState, WorkstationManager
```

---

## 3. Workstation Definitions (16)

| ID | Type | Tier | Professions | Speed | Quality | Success | XP | Cost |
|----|------|------|-------------|-------|---------|---------|----|------|
| ws_campfire | Campfire | 1 | Cooking | 0.8x | 0% | 0% | 1.0x | 0% |
| ws_forge | Forge | 1 | Blacksmithing | 1.0x | 0% | 0% | 1.0x | 0% |
| ws_anvil | Anvil | 1 | Blacksmithing | 1.0x | 5% | 0% | 1.0x | 0% |
| ws_workbench | Workbench | 1 | Carpentry, Engineering | 1.0x | 0% | 0% | 1.0x | 0% |
| ws_alchemy_table | AlchemyTable | 1 | Alchemy | 1.0x | 5% | 0% | 1.0x | 0% |
| ws_cooking_pot | CookingPot | 1 | Cooking | 1.2x | 0% | 0% | 1.0x | 0% |
| ws_tailor_bench | TailorBench | 1 | Tailoring | 1.0x | 5% | 0% | 1.0x | 0% |
| ws_enchanting_table | EnchantingTable | 1 | Enchanting | 1.0x | 10% | 5% | 1.2x | 0% |
| ws_jewelry_station | JewelryStation | 1 | Jewelry | 1.0x | 10% | 0% | 1.0x | 0% |
| ws_smelter | Smelter | 2 | Blacksmithing, Mining | 1.5x | 0% | 0% | 1.1x | 10% |
| ws_grinder | Grinder | 2 | Blacksmithing, Carpentry | 1.3x | 5% | 0% | 1.0x | 0% |
| ws_loom | Loom | 2 | Tailoring | 1.2x | 5% | 0% | 1.0x | 0% |
| ws_tanning_rack | TanningRack | 1 | Leatherworking | 1.0x | 0% | 0% | 1.0x | 0% |
| ws_sawmill | Sawmill | 2 | Carpentry, Woodcutting | 1.5x | 0% | 0% | 1.1x | 10% |
| ws_advanced_forge | AdvancedForge | 4 | Blacksmithing | 2.0x | 20% | 10% | 1.5x | 20% |
| ws_arcane_altar | ArcaneAltar | 5 | Enchanting, Alchemy | 2.5x | 30% | 15% | 2.0x | 25% |

---

## 4. Workstation Bonuses

### 4.1 Craft Speed Multiplier
- Reduces craft time: `effectiveTime = baseTime / speedMultiplier`
- Range: 0.8x (Campfire) to 2.5x (Arcane Altar)

### 4.2 Quality Bonus
- Adds to recipe quality roll
- Range: 0% to 30%

### 4.3 Success Rate Bonus
- Adds to recipe success chance
- Range: 0% to 15%

### 4.4 Experience Bonus
- Multiplies XP gained
- Range: 1.0x to 2.0x

### 4.5 Cost Reduction
- Reduces ingredient requirements
- Range: 0% to 25%

---

## 5. Workstation States

Runtime states track:
- Position in world
- Active/inactive status
- Durability
- Owner
- Player-placed flag

---

## 6. Extensibility

New workstations can be added by:
1. Adding a `WorkstationDefinition` entry
2. Adding supported professions
3. Setting tier and bonuses

No code changes required for adding new workstations.