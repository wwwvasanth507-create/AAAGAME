# Equipment System Architecture - Hero of Eternia

This document details player equipment slots, visual mesh overlays, and attribute modifier integration.

---

## 1. Equipment Slots

The player character has 12 equipment slots managed by the `EquipmentManager` class:

| Slot Type | Extensible Category Match | Target Mesh Part Category |
|---|---|---|
| **Helmet** | `helmet` | `PartCategory.Helmet` |
| **Chest** | `armor`, `chest` | `PartCategory.Armor` |
| **Legs** | `legs`, `armor` | `PartCategory.Body` |
| **Boots** | `boots`, `feet` | `PartCategory.Feet` |
| **Gloves** | `gloves`, `hands` | `PartCategory.Hands` |
| **MainWeapon**| `weapon`, `tool` | `PartCategory.Weapon` |
| **OffHand** | `shield`, `offhand` | `PartCategory.Cape` (Visual Fallback) |
| **Ring1** | `ring`, `accessory` | None (Stat modifier only) |
| **Ring2** | `ring`, `accessory` | None (Stat modifier only) |
| **Necklace** | `necklace`, `accessory`| None (Stat modifier only) |
| **Pet** | `pet` | None |
| **Mount** | `mount`, `mounttoken` | None |

---

## 2. Dynamic Attribute Recalculation

Equipping items dynamically adjusts player attributes by interfacing with the `PlayerAttributeSet` from Prompt 5:

```
EquipItem(MainWeapon, IronSword)
  ├── Resolve StatModifiers in IronSword -> Flat +2 Strength
  ├── Register StatModifier in PlayerAttributeSet -> "Equip_MainWeapon_Strength"
  └── Set Attributes Dirty Flag -> CurrentValue recalculation triggered on next query
```

- **Modifier Binding:** Modifiers are registered with the slot type prefixed in their ID (e.g. `Equip_MainWeapon_Strength`).
- **Clean Unequip:** When items are unequipped, the modifiers list is retrieved by key and removed from the attribute set, returning the player stats to their base values.
- **Safety Checks:** All slot mappings are checked against item category restrictions before equip triggers are processed.

---

## 3. Visual Mesh Integration

The `EquipmentManager` interfaces with the `PlayerModelController` attached to the player node:
- **Part Swapping:** Upon equip, it passes the item's `ModelPath` to the model controller's slot resolver to update meshes dynamically.
- **Material Override:** If `MaterialPath` is configured, it loads and applies the material override (e.g., crystal tints) recursively.
- **Dismount Visual Reset:** Unequipping an item resets the slot back to its default placeholder geometry.
