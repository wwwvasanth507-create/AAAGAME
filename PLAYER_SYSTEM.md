# Player Character System - Hero of Eternia

This document details the player character architecture, model customizer pipeline, attribute modifier math, and player effects framework.

---

## 1. High-Level Player Architecture

The player character is structured around a centralized modular blueprint:

```
PlayerRoot (CharacterBody3D)
  ├── PlayerModelController (Swappable slots, LOD settings)
  ├── PlayerInteractionDetector (Collision Area3D, tap/hold/auto processing)
  ├── PlayerAnimationController (AnimationPlayer & AnimationTree state blend wrapper)
  ├── PlayerEffectsController (Visual overlays & status emitter)
  └── PlayerAudioController (Surface raycast footstep manager)
```

- **PlayerRoot**: Directs standard physics ticks, ticks stats regeneration, and maps outer APIs.
- **PlayerData**: Extends basic vitals and primary attributes to map into the new dynamic attributes set.

---

## 2. Dynamic Model Customizer Pipeline

The `PlayerModelController` manages 11 swappable mesh slots to support male/female rigs and cosmetics:
- **Slots:** `Hair`, `Face`, `Eyes`, `Body`, `Hands`, `Feet`, `Armor`, `Helmet`, `Cape`, `Weapon`, `Accessory`.
- **Level of Detail (LOD) Rules:**
  - **LOD0 (High):** Full meshes, active casting shadows.
  - **LOD1 (Medium):** Standard meshes, casting shadows.
  - **LOD2 (Low):** Hidden detail slots (`Eyes`, `Accessory`), disabled shadow-casting recursive flags for performance on 2GB RAM Android devices.
- **Customization Tinting:** Supports color tint overrides (`SetPartColor`) and material overrides (`SetPartMaterial`) for character customization.

---

## 3. Data-Driven Attribute Math

Attributes are defined dynamically in `player_attributes_config.json` via `ConfigManager` to allow balancing without modifying code.

### Recalculation Formula
Modifiers are processed under the standard RPG formula:
$$\text{CurrentValue} = (\text{BaseValue} + \sum \text{FlatModifiers}) \times (1 + \sum \text{PercentAddModifiers}) \times \prod (1 + \text{PercentMultModifiers})$$

- **Caching:** Calculations are only performed on modification changes (`_isDirty` pattern) to optimize CPU time.
- **Sources:** `Equipment`, `Potion`, `Buff`, `Debuff`, `Skill`, `Temporary`, `Permanent`.
- **Duration Updates:** Handled automatically in `PlayerRoot._PhysicsProcess`.

---

## 4. Status Effects Framework

The `PlayerEffectsController` provides a decoupled status visual overlay system:
- **Effect Types:** `Glow`, `Fire`, `Ice`, `Poison`, `Electric`, `Wind`, `Water`, `Dark`, `Light`, `Aura`, `Healing`, `Shield`.
- **Automatic Lifecycle:** Timing out is done on the physics process. Expired effect nodes are cleaned via `QueueFree()`.
