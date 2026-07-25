# Profession System — Hero of Eternia

> **Version:** 1.0.0  
> **Phase:** Prompt 15 / 150  
> **Status:** Production Ready

---

## 1. Overview

The Profession System provides 14 fully data-driven professions with XP curves, level progression, unlock tracking, and stat bonuses. Each profession is independently tracked and can be leveled from 1 to 100.

---

## 2. Architecture

```
Scripts/Gathering/
└── ProfessionSystem.cs  ← ProfessionData, ProfessionManager, XP curves
```

---

## 3. Professions (14)

| Profession | Curve | Base XP | Growth | Bonus Types |
|------------|-------|---------|--------|-------------|
| Woodcutting | Moderate | 100 | 1.15x | gather_speed, yield_bonus, critical_chance |
| Mining | Moderate | 120 | 1.15x | gather_speed, yield_bonus, critical_chance |
| Fishing | Linear | 80 | 1.10x | gather_speed, yield_bonus, rare_chance |
| Cooking | Linear | 60 | 1.12x | craft_speed, quality_bonus |
| Blacksmithing | Steep | 150 | 1.18x | craft_speed, quality_bonus, stat_bonus |
| Alchemy | Steep | 130 | 1.16x | craft_speed, potency_bonus, duration_bonus |
| Tailoring | Moderate | 100 | 1.14x | craft_speed, quality_bonus, armor_bonus |
| Leatherworking | Moderate | 110 | 1.14x | craft_speed, quality_bonus, durability_bonus |
| Carpentry | Moderate | 90 | 1.13x | craft_speed, durability_bonus, quality_bonus |
| Engineering | Exponential | 200 | 1.20x | craft_speed, damage_bonus, durability_bonus |
| Jewelry | Exponential | 180 | 1.19x | craft_speed, quality_bonus, stat_bonus |
| Enchanting | Exponential | 250 | 1.22x | enchant_power, quality_bonus, success_rate |
| Farming | Linear | 70 | 1.10x | gather_speed, yield_bonus, growth_speed |
| Animal Care | Linear | 80 | 1.10x | gather_speed, yield_bonus, taming_chance |

---

## 4. XP System

### 4.1 Formula

```
XP for Level N = BaseXP × Growth^(N-1)
Total XP to Level N = Σ(BaseXP × Growth^(i-1)) for i = 1 to N-1
```

### 4.2 Curves

- **Linear** (1.10x): Fast early levels, slower endgame
- **Moderate** (1.15x): Balanced progression
- **Steep** (1.18x): Slow early, rewarding mastery
- **Exponential** (1.20x+): Very slow progression, prestigious

### 4.3 Level-Up Flow

```
AddExperience(amount)
  ├── Apply XP bonus modifier
  ├── Add to current XP
  ├── While XP >= XP for next level:
  │     ├── Subtract XP cost
  │     ├── Increment level
  │     ├── Check unlocks at this level
  │     └── Fire level-up event
  └── Return levels gained
```

---

## 5. Unlocks

Each profession has 10 unlock thresholds (levels 5, 10, 15, 20, 25, 30, 40, 50, 75, 100).
Format: `"level:unlock_id"` — extensible without code changes.

Examples:
- "5:unlock_iron_axe" (Woodcutting level 5)
- "20:unlock_mythril_recipes" (Blacksmithing level 20)
- "50:unlock_legendary_gems" (Jewelry level 50)
- "100:unlock_woodcutting_mastery" (Woodcutting capstone)

---

## 6. Bonuses

Bonuses are key-value float pairs that affect gameplay:
- `gather_speed`: Multiplier for gather time
- `yield_bonus`: Additional yield per gather
- `critical_chance`: Chance for double yield
- `craft_speed`: Multiplier for craft time
- `quality_bonus`: Additional quality tier chance
- `xp_bonus`: Experience gain multiplier

---

## 7. Save Integration

Professions are saved in Save V12 as `List<ProfessionSaveState>`:
```json
{
  "Type": "Blacksmithing",
  "Level": 15,
  "Experience": 450,
  "IsUnlocked": true,
  "Specialization": "",
  "Achievements": []
}
```

---

## 8. Extensibility

Designers can add new professions by:
1. Adding a new `ProfessionType` enum value
2. Adding a `ProfessionData` entry in `InitializeDefaultProfessions()`
3. Adding recipes that reference the new profession

No code changes required for adding recipes or unlocks.