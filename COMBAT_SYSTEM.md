# Combat System Documentation

**Version:** 1.0.0
**Phase:** Prompt 10 / 150
**Status:** Production Ready

---

## Overview

The Combat System provides a modular, data-driven, and event-driven core for orchestrating melee and ranged attacks in *Hero of Eternia*. It handles attacker registries, cooldown management, attack execution, status effects, and simulation ticking without hardcoded gameplay mechanics or direct visual bindings, enabling full headless simulation.

---

## Architecture

```
Scripts/Combat/
├── CombatDefinitions.cs     ← DamageType, WeaponType, TargetMode, CombatEvent structures
├── WeaponDefinition.cs      ← WeaponData record & WeaponDatabase configuration lookup
├── TargetingSystem.cs       ← Target registry, nearest search, and lock management
├── HitDetection.cs          ← Sphere/AABB melee sweeps and projectile point collision math
├── DamageSystem.cs          ← Resistance profiling, elemental modifiers, and critical hits calculations
├── StatusEffectSystem.cs    ← Temporary Buffs/De-buffs ticking and application
├── ProjectileSystem.cs      ← Gravity-fed arrow and bolt physical simulations
└── CombatManager.cs         ← Service locator orchestration and combat event channel
```

---

## Combat Event Channel (CombatEvent)

Every interaction within the combat loop broadcasts a `CombatEvent` to ensure loose coupling between gameplay logic, visual animations, audio hooks, and UI displays:

```csharp
public class CombatEvent
{
    public CombatEventType Type      { get; set; }
    public string          ActorId   { get; set; }
    public string          TargetId  { get; set; }
    public float           Value     { get; set; } // e.g. damage amount, remaining stacks
    public string          MetaTag   { get; set; } // e.g. weapon id, effect name
    public double          Timestamp { get; set; }
}
```

### Event Types

- `AttackStarted`: Attack action initiated.
- `HitLanded`: Hit validation succeeded on a target.
- `HitMissed`: Swing did not collide with any valid targets.
- `DamageDealt` / `DamageReceived`: Quantified damage applied.
- `StatusApplied` / `StatusExpired`: Status effect lifecycle changes.
- `ProjectileFired` / `ProjectileImpact`: Physical projectiles lifecycle.
- `EntityDied` / `EntityRevived`: Health thresholds reached.
- `BlockSucceeded` / `ParrySucceeded`: Shield defensive mitigations.

---

## Modular Subsystems

### 1. Attack Cooldowns

Attacks are bounded by weapon-specific speeds (`1f / AttackSpeed`). Cooldowns are tracked in `CombatManager` on a per-attacker, per-weapon basis:

```csharp
// CombatManager handles automatically:
if (!CheckCooldown(attackerId, weaponId)) return;
StartCooldown(attackerId, weaponId, 1f / weapon.AttackSpeed);
```

### 2. Audio & Visual Hooks

Audio and visual details are completely data-driven. The `WeaponData` configuration contains string keys referring to game assets:
- `AudioHookKey`: Swing swooshes, arrow releases, magic casts.
- `VfxHookKey`: Sword sweeps, trail particles, impact explosions.

Rendering and audio manager layers subscribe to the `OnCombatEvent` channel and play the assets matching these keys dynamically.

---

## Integration with Player State Machine

The player character state machine includes 8 dedicated states:
1. `Attack`: Light melee sweep. Slows movement.
2. `HeavyAttack`: Heavy melee sweep. Long recovery, high damage.
3. `Casting`: Spellcasting state. Restricts movement during execution.
4. `Blocking`: Active blocking, reduces move speed, drains stamina slowly.
5. `Parrying`: Short parry timing window. Transitions to block on release.
6. `HitReaction`: Light recoil animation when taking damage.
7. `Knockdown`: Flattened onto ground.
8. `Recovery`: Rising back to feet before restoring normal input states.
