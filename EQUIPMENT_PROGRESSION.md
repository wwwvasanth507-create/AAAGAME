# Equipment Progression Framework — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 14)

## 1. Overview

The equipment progression framework provides a complete system for gear, stat calculation, modifiers, durability, upgrades, enchantments, quality grades, and gear sets. All systems are data-driven, modular, and designed for extensibility.

## 2. Systems

| System | File | Description |
|--------|------|-------------|
| Attribute Calculation Engine | `Scripts/Equipment/Attributes/AttributeCalculationEngine.cs` | Centralized deterministic stat calculation with 10 modifier layers |
| Item Modifier System | `Scripts/Equipment/Modifiers/ItemModifierSystem.cs` | Reusable modifier definitions with configurable stacking rules |
| Enchantment Framework | `Scripts/Equipment/Enchantments/EnchantmentFramework.cs` | Elemental enchantments with level scaling |
| Durability System | `Scripts/Equipment/Durability/DurabilitySystem.cs` | Equipment wear, break state, repair hooks |
| Gear Set System | `Scripts/Equipment/Sets/GearSetSystem.cs` | Set identification, piece counting, bonus hooks |
| Item Quality System | `Scripts/Equipment/Quality/ItemQualitySystem.cs` | 8 quality grades with stat multipliers |
| Upgrade Framework | `Scripts/Equipment/Upgrade/UpgradeFramework.cs` | Upgrade levels with success/failure/destroy rules |
| Save Integration | `Scripts/Equipment/Save/EquipmentSaveData.cs` | Save V11 with full equipment persistence |

## 3. Data Flow

```
Item Equipped
  └─ EquipmentManager
       ├─ Apply modifiers to AttributeCalculationEngine
       ├─ Register durability component
       ├─ Check gear set completion
       └─ Apply quality multiplier
            └─ AttributeCalculationEngine recalculates
                 └─ Events fired for UI updates
```

## 4. Extension Points

- New attributes: Add to `AttributeType` enum
- New modifiers: Register via `ItemModifierSystem.RegisterModifier()`
- New enchantments: Register via `EnchantmentFramework.RegisterEnchantment()`
- New gear sets: Register via `GearSetManager.RegisterSet()`
- New quality grades: Register via `ItemQualitySystem.RegisterQuality()`
- New upgrade rules: Add to `UpgradeState.DefaultUpgradeRules`