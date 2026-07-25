# Attribute Calculation Engine — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 14)

## 1. Architecture

The `AttributeCalculationEngine` is a centralized, deterministic, cache-friendly pipeline for computing final attribute values from multiple modifier sources (base, equipment, abilities, buffs, debuffs, environment, difficulty, guild, mount, pet).

### Calculation Pipeline

```
GetValue(AttributeType)
  └─ Check cache (dirty flag)
       ├─ Clean → return cached value
       └─ Dirty → Recalculate
                    ├─ Process each layer in order
                    │   ├─ Base
                    │   ├─ Equipment
                    │   ├─ Ability
                    │   ├─ Buff
                    │   ├─ Debuff
                    │   ├─ Environment
                    │   ├─ Difficulty
                    │   ├─ Guild (future)
                    │   ├─ Mount (future)
                    │   └─ Pet (future)
                    ├─ Standard RPG Formula: (Base + Flat) * (1 + PercentAdd) * Product(1 + PercentMult)
                    ├─ Apply clamping rules per attribute type
                    ├─ Update cache
                    └─ Fire recalculated event
```

### RPG Formula

```
CurrentValue = (BaseValue + ΣFlatModifiers) × (1 + ΣPercentAddModifiers) × Π(1 + PercentMultModifiers)
```

## 2. Attributes

The `AttributeType` enum has been expanded to support:

| Category | Attributes |
|----------|------------|
| Core Vitals | Health, Mana, Energy, Stamina |
| Core Stats | Strength, Vitality, Magic, Dexterity, Luck |
| Combat Stats | Attack, MagicAttack, Defense, MagicDefense, Speed, CriticalRate, CriticalDamage, AttackSpeed, CastingSpeed, MovementSpeed |
| Defensive | BlockChance, DodgeChance |
| Elemental Resistances | Fire, Ice, Lightning, Poison, Holy, Shadow |
| Status Resistances | Stun, Freeze, Burn, Bleed, Silence, Knockback |
| Special Hooks | LifeSteal, ManaRegen, HealthRegen, ExperienceBonus, GoldBonus |
| Custom | Extensible via Custom |

## 3. Performance

- Dictionary lookups: O(1) per attribute
- Dirty flag caching: recalculations only on changes
- Global dirty flag: batch invalidation
- AggressiveInlining on hot paths
- No allocations in calculation hot path