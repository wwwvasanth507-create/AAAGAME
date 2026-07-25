# Status Effects Documentation

**Version:** 1.0.0
**Phase:** Prompt 10 / 150
**Status:** Production Ready

---

## Overview

The Status Effect System provides a fully data-driven framework for applying, stacking, ticking, and expiring temporary status modifications (buffs and debuffs). All effects are configured in a configuration file (`status_effects_config.json`), allowing designers to balance durations, modifiers, and ticks without touching C# code.

---

## Built-In Status Effects

The framework implements 10 reusable combat effects:

| Effect | Damage/Modifier | Custom Tick Type | Standard Behavior |
|--------|-----------------|------------------|-------------------|
| **Burn** | 4.0 dmg / sec | Fire | Drains health over time |
| **Freeze** | Movement locked | - | Disables all FSM movement inputs |
| **Shock** | 2.0 dmg / 0.5s | Lightning | Fast stun interrupts |
| **Poison** | 3.0 dmg / sec | Poison | Long duration, stacks up to 3 |
| **Bleed** | 5.0 dmg / sec | Physical | Physical dot, stacks up to 5 |
| **Slow** | -50% Speed | - | Binds movement properties |
| **Stun** | Disables FSM | - | Complete target lock, stops inputs |
| **Silence** | Skill lock | - | Bypasses spellcast commands |
| **Knockback**| Forced vector | - | Displaces character coordinates |
| **Regen** | -5.0 dmg / sec | Healing | Restores health over time |

---

## Configuration Schema (status_effects_config.json)

Status effects are defined as JSON arrays and loaded into the `StatusEffectSystem` at startup:

```json
[
  {
    "Type": 0,
    "Duration": 5.0,
    "TickInterval": 1.0,
    "TickDamage": 4.0,
    "TickDamageType": 1,
    "StackLimit": 1,
    "VfxHookKey": "vfx_burn"
  },
  {
    "Type": 3,
    "Duration": 8.0,
    "TickInterval": 1.0,
    "TickDamage": 3.0,
    "TickDamageType": 4,
    "StackLimit": 3,
    "VfxHookKey": "vfx_poison"
  }
]
```

---

## Stacking & Refreshes

- **Stack Limit**: Bounded by `StackLimit` defined in the configuration.
- **Duration Refresh**: If an effect is applied but the stack count is already at the `StackLimit` threshold, the remaining duration of the active stack resets to its maximum duration instead of adding a new stack.
- **Tick Ticks**: Tick interval timers operate independently per stack.
- **Removal**: All effects expire automatically when duration hits `0f` or can be cleaned up instantly using `RemoveAll(entityId)`.
