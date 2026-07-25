# Targeting System Documentation

**Version:** 1.0.0
**Phase:** Prompt 10 / 150
**Status:** Production Ready

---

## Overview

The Targeting System manages entity target selection, range verification, field-of-view validations, and target locking. It uses lightweight, headless-safe data structures (`CombatTarget`) to represent target colliders, ensuring gameplay evaluation runs completely independently of Godot Node hierarchies.

---

## Targeting Modes

| Mode | Key Behavior |
|------|--------------|
| **Free Target** | Attacks sweep in the player's forward direction. No active lock. |
| **Soft Lock** | The system dynamically finds the closest valid target inside the player's field of view. |
| **Hard Lock** | Camera focuses permanently on the locked target. Ignores distance fluctuations. |
| **Nearest Target** | Quick query for the absolute closest enemy entity. |
| **Manual Selection** | Player manually clicks or selects a target ID. |

---

## Targeting APIs

```csharp
var targeting = new TargetingSystem();

// 1. Register targetable entity
targeting.RegisterTarget(new CombatTarget 
{
    TargetId = "enemy_skeleton_01",
    FactionId = "enemy",
    WorldX = 12f,
    WorldY = 0f,
    WorldZ = -5f,
    ColliderRadius = 0.6f,
    Priority = 2, // Preferred focus priority
    IsAlive = true
});

// 2. Perform lock search
var target = targeting.SoftLock(originX, originY, originZ, maxRange: 15f, excludeFaction: "player");

// 3. Switch target index (cycles candidates sorted by priority)
var next = targeting.SwitchTarget(originX, originY, originZ, maxRange: 15f, excludeFaction: "player");
```

---

## Line-of-Sight (LoS) & Range Validation

### 1. Range Validation
Calculates the absolute distance between the attacker coordinate and the target:

$$\text{Distance} = \sqrt{(x_2 - x_1)^2 + (y_2 - y_1)^2 + (z_2 - z_1)^2}$$

If the target's distance exceeds the weapon's range, validation fails and combat events are not processed.

### 2. Line of Sight (Headless Approximation)
Validates that the target lies within the player's forward view angle (FOV cone) using vector math:

```csharp
float dx = target.WorldX - playerX;
float dz = target.WorldZ - playerZ;
float len = MathF.Sqrt(dx * dx + dz * dz);
float dot = (dx / len) * fwdX + (dz / len) * fwdZ;

// Fails if target lies behind the player or outside the FOV cone angle
bool visible = dot >= MathF.Cos(fovAngleDegrees * MathF.PI / 360f);
```

---

## Future Multiplayer Ready

All targets are tracked via string identifier keys (`TargetId`) rather than raw memory pointers. This allows network serialization layers to cleanly pass target references between client and server systems.
