# Damage System Documentation

**Version:** 1.0.0
**Phase:** Prompt 10 / 150
**Status:** Production Ready

---

## Overview

The Damage System processes all attack instances through resistance profiles, critical strike rates, elemental multipliers, and true damage bypass calculations. It generates immutable results for entity health adjustments and status effects.

---

## Damage Types

The framework supports 9 core damage types:

1. **Physical**: Reduced by base armor ratings (Physical resistance).
2. **Fire**: Ignite effects. Elemental fire multiplier.
3. **Ice**: Chill and freezing effects.
4. **Lightning**: Instant arc damage and shock.
5. **Poison**: Damage over time (DOT) stacks.
6. **Holy**: Smite damage, healing affinities.
7. **Shadow**: Life drain, shadow decay.
8. **True Damage**: Ignores all resistances.
9. **Healing**: Negative damage (restores health instead of draining).

---

## Resistance Profile

Every entity possesses a `ResistanceProfile` detailing float mitigation values for each damage type:
- `0.0` = No resistance (full damage taken).
- `0.5` = 50% resistance (half damage taken).
- `1.0` = 100% immunity (zero damage taken).
- `-0.5` = Vulnerable (takes 50% extra damage).

```csharp
var rp = new ResistanceProfile();
rp.Set(DamageType.Fire, 0.40f);     // 40% fire resistance
rp.Set(DamageType.Holy, -0.20f);    // 20% vulnerability to Holy
```

---

## Damage Processing Pipeline

```
DamageInstance (BaseDamage, Type, CritChance)
  ├── 1. Critical Roll (BaseDamage × CritMultiplier if successful)
  ├── 2. Is Healing? ── Yes ──> Return negative damage value (restore)
  ├── 3. Is True Damage? ── Yes ──> Return damage value directly (bypasses step 4 & 5)
  ├── 4. Apply Elemental Multipliers (e.g. Fire deals 1.25x against vulnerable targets)
  └── 5. Apply Resistance Deduction (Damage × (1.0 - TargetResistance))
```

### Damage Processing API

```csharp
// Attacker initiates damage instance
var dmg = new DamageInstance
{
    AttackerId = "player_hero",
    TargetId = "enemy_orc",
    BaseDamage = 20f,
    Type = DamageType.Lightning,
    CritChance = 0.15f
};

// CombatManager applies to target using its profile
float finalDamage = DamageSystem.ProcessDamage(dmg, targetResistanceProfile, new Random());
```
