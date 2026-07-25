# Ability System — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 13)

---

## 1. Architecture Overview

The ability system is a data-driven, modular framework that supports unlimited abilities, categories, effects, and progression hooks. It is designed to be extensible without code changes for designers.

```
AbilityDatabase (JSON-driven)
  └── AbilityDefinition (runtime wrapper)
        └── AbilityData (pure data record)

AbilityManager (execution framework)
  ├── AbilityState (per-ability runtime state)
  ├── AbilityResult (execution result)
  └── AbilityExecutionConfig (behavior settings)

CategoryManager (extensible categories)
  └── CategoryDefinition (per-category data)

EffectsManager (status effects)
  ├── AbilityEffect (definition)
  └── ActiveEffect (runtime instance)

LoadoutManager (ability loadouts)
  └── AbilityLoadout (slot configuration)

ResourceManager (resource pools)
  └── ResourcePool (per-resource state)

PlayerProgression (level/XP/prestige)
  └── ProgressionSaveData (persistence)
```

---

## 2. Ability Database

### 2.1 Data Fields

Every ability in `Settings/ability_database.json` contains:

| Field | Type | Description |
|-------|------|-------------|
| `abilityId` | string | Unique identifier |
| `internalName` | string | Internal name for code references |
| `displayName` | string | Player-facing name |
| `description` | string | Player-facing description |
| `category` | string | Category ID (Melee, Magic, etc.) |
| `abilityType` | string | Active, Passive, Toggle, Triggered, Ultimate |
| `targetType` | string | self, singleEnemy, aoe, projectile, directional |
| `damageType` | string | none, physical, fire, ice, lightning, poison, holy, shadow |
| `element` | string | Element affinity (None, Physical, Fire, etc.) |
| `animationReference` | string | Animation resource path |
| `audioReference` | string | Audio resource path |
| `visualEffectReference` | string | VFX resource path |
| `cooldownSec` | float | Cooldown in seconds |
| `castTime` | float | Cast time in seconds (0 = instant) |
| `manaCost` | float | Mana cost |
| `staminaCost` | float | Stamina cost |
| `energyCost` | float | Energy cost |
| `focusCost` | float | Focus cost |
| `rageCost` | float | Rage cost |
| `spiritCost` | float | Spirit cost |
| `healthCost` | float | Health cost |
| `baseDamage` | float | Base damage value |
| `baseHealing` | float | Base healing value |
| `shieldAmount` | float | Shield amount |
| `aoeRadius` | float | Area of effect radius |
| `duration` | float | Effect duration |
| `range` | float | Ability range |
| `maxCharges` | int | Maximum charges (1 = no charges) |
| `chargeRechargeSec` | float | Time to recharge one charge |
| `levelRequired` | int | Minimum player level |
| `unlockRequirement` | string | Quest or condition requirement |
| `upgradePathHook` | string | Reference to upgrade path data |
| `vfxCastKey` | string | Cast VFX key |
| `vfxHitKey` | string | Hit VFX key |
| `vfxChannelKey` | string | Channel VFX key |
| `sfxCastKey` | string | Cast SFX key |
| `sfxHitKey` | string | Hit SFX key |
| `sfxChannelKey` | string | Channel SFX key |
| `iconPath` | string | UI icon resource path |
| `localizationKey` | string | Localization string key |
| `version` | int | Data version for migration |
| `dlcId` | string | DLC ownership requirement |
| `tags` | string[] | Search/filter tags |

### 2.2 Current Abilities

| ID | Category | Type | Cooldown | Resource Cost |
|----|----------|------|----------|---------------|
| `power_strike` | Melee | Active | 6s | 25 Stamina |
| `dodge_roll` | Movement | Active | 3s (2 charges) | 15 Stamina |
| `arrow_rain` | Ranged | Active | 12s (1s cast) | 20 Mana |
| `barrier` | Defensive | Active | 18s (0.5s cast) | 35 Mana |
| `fireball` | Magic | Active | 8s (1.5s cast) | 30 Mana |
| `healing_light` | Healing | Active | 15s (2s cast) | 40 Mana |
| `summon_spirit_wolf` | Summoning | Active | 45s (2s cast) | 50 Mana |
| `power_aura` | Passive | Passive | 0s | None |
| `blink` | Movement | Active | 8s | 25 Mana |
| `ultimate_judgment` | Ultimate | Ultimate | 120s (3s cast) | 100 Mana |

---

## 3. Ability Manager

### 3.1 Execution Flow

```
ActivateAbility(abilityId, targetContext)
  │
  ├─ 1. Validate ability exists in database
  ├─ 2. Validate ability is registered
  ├─ 3. Check global cooldown
  ├─ 4. Check per-ability cooldown
  ├─ 5. Check charges available
  ├─ 6. Check not already casting
  ├─ 7. Check level requirement
  ├─ 8. Validate resources (mana, stamina, etc.)
  ├─ 9. Validate target
  ├─ 10. Consume resources
  ├─ 11. Consume charge
  ├─ 12. Start cast (if cast time > 0)
  ├─ 13. Start cooldown
  ├─ 14. Apply global cooldown
  ├─ 15. Fire activation event
  ├─ 16. Trigger animation
  ├─ 17. Trigger VFX/SFX
  └─ 18. Complete (if instant) or wait for cast completion
```

### 3.2 Supported Operations

- **Activation**: Full validation pipeline before execution
- **Cancellation**: Player-initiated cast cancel
- **Interruption**: External interruption (stun, knockback, etc.)
- **Cooldowns**: Per-ability and global cooldown tracking
- **Charges**: Multi-charge abilities with recharge
- **Resource Consumption**: Mana, Stamina, Energy, Focus, Rage, Spirit, Health
- **Target Validation**: Self, Single Enemy, AoE, Projectile, Directional
- **Animation Triggers**: Hook system for animation controller
- **Visual Effect Hooks**: Cast, hit, and channel VFX
- **Audio Hooks**: Cast, hit, and channel SFX

### 3.3 Events

| Event | Description |
|-------|-------------|
| `AbilityActivatedEvent` | Fired when ability execution begins |
| `AbilityCompletedEvent` | Fired when ability effects are applied |
| `AbilityInterruptedEvent` | Fired when casting is interrupted |
| `AbilityCooldownCompleteEvent` | Fired when cooldown expires |
| `AbilityFailedEvent` | Fired when execution fails |
| `AbilityChargesChangedEvent` | Fired when charges change |
| `AbilityCastStartedEvent` | Fired when cast begins |
| `AbilityResourceConsumedEvent` | Fired when resources are spent |

---

## 4. Ability Categories

### 4.1 Default Categories

| Category | Sort Order | Default Unlock | Tags |
|----------|------------|----------------|------|
| Melee | 1 | Yes | physical, close-range |
| Magic | 2 | Yes | magical, ranged |
| Ranged | 3 | Yes | physical, ranged |
| Movement | 4 | Yes | mobility |
| Support | 5 | Yes | utility, buff |
| Healing | 6 | Yes | healing, restoration |
| Defensive | 7 | Yes | defense, mitigation |
| Summoning | 8 | Yes | summon, pet |
| Passive | 9 | Yes | passive, bonus |
| Ultimate | 10 | No (level 50) | ultimate, powerful |
| Utility | 11 | Yes | utility, misc |

### 4.2 Extensibility

New categories can be added at runtime via `CategoryManager.Register()` without code changes. Each category has an ID, display name, description, icon, sort order, unlock condition, and tags.

---

## 5. Resource Framework

### 5.1 Supported Resources

| Resource | Default Max | Regen/sec | Color |
|----------|-------------|-----------|-------|
| Health | 500 | 2 | #FF4444 |
| Mana | 100 | 5 | #00BFFF |
| Stamina | 100 | 15 | #32CD32 |
| Energy | 100 | 10 | #FFD700 |
| Focus | 100 | 8 | #FF69B4 |
| Rage | 100 | 0 (combat gain) | #FF4500 |
| Spirit | 100 | 0 (combat gain) | #9370DB |

### 5.2 Features

- Configurable max values and regen rates
- No hardcoded assumptions about which resources exist
- Event-driven change notifications
- Support for custom resources via `ResourceConfig`

---

## 6. Player Progression

### 6.1 Level System

- Max Level: 100
- Base XP Requirement: 100
- XP Growth Factor: 1.15 (15% more XP per level)
- Prestige Levels: 10 (resets to level 1 with permanent bonuses)

### 6.2 Stat Growth

| Stat | Per Level |
|------|-----------|
| Health | +20 |
| Mana | +10 |
| Stamina | +5 |
| Attack | +3 |
| Defense | +2 |
| Magic | +3 |

### 6.3 Prestige Bonuses

- Damage Multiplier: +5% per prestige level
- Health Multiplier: +3% per prestige level

---

## 7. Ability Loadouts

### 7.1 Slot Configuration

Each loadout supports:
- 4 Primary ability slots
- 4 Secondary ability slots
- 4 Passive slots
- 1 Ultimate slot
- 4 Quick-access slots

### 7.2 Features

- Up to 6 configurable loadouts
- Save/load persistence
- Slot assignment and management
- Loadout switching at runtime

---

## 8. Ability Effects

### 8.1 Effect Types

| Type | Description |
|------|-------------|
| Damage | Direct damage application |
| Healing | Health restoration |
| Shield | Damage absorption |
| Buff | Positive stat modification |
| Debuff | Negative stat modification |
| Teleport | Positional movement |
| Summon | Entity spawning |
| ProjectileSpawn | Projectile creation |
| AreaCreation | Persistent area effects |
| Movement | Movement modification |
| EnvironmentalInteraction | World interaction hooks |
| Custom | Extensible custom effects |

### 8.2 Features

- Stacking with configurable max stacks
- Duration tracking and expiration
- Tick-based effects (damage over time, healing over time)
- Event-driven lifecycle

---

## 9. Save Integration

### 9.1 Persisted Data (Save V10)

- Unlocked ability IDs
- Ability levels
- Current loadout configuration
- Active loadout index
- Ability manager runtime state (cooldowns, charges, cast progress)
- Progression data (level, XP, prestige)

### 9.2 Version Migration

Save V10 migration handles:
- Promoting `LearnedAbilities` to `UnlockedAbilityIds`
- Initializing empty `AbilityLevels` dictionary
- Creating default `LoadoutData` list
- Creating `ProgressionSaveData` from existing level/XP

---

## 10. Performance Considerations

### 10.1 Optimizations

- **Object pooling**: AbilityState objects are created once per ability
- **Dictionary lookups**: O(1) ability state access
- **Event-driven**: No polling for cooldown completion
- **Lazy registration**: Abilities registered on-demand
- **GC-friendly**: Minimal allocations in hot paths

### 10.2 Memory Usage

- Each AbilityState: ~64 bytes
- Each ActiveEffect: ~48 bytes
- AbilityManager overhead: ~2KB for 100 abilities
- EffectsManager overhead: scales with active effects

### 10.3 Android Optimization

- Object pooling for frequently created/destroyed objects
- Reduced dictionary allocations via capacity hints
- Event batching for UI updates
- Minimal string allocations in hot paths

---

## 11. Testing

### 11.1 Test Coverage

| Test Area | Tests |
|-----------|-------|
| Ability Definition | 12 |
| Ability Category | 7 |
| Ability State | 12 |
| Ability Activation | 6 |
| Cooldowns | 5 |
| Resource Consumption | 6 |
| Target Validation | 2 |
| Cancellation | 4 |
| Interruption | 3 |
| Charges | 4 |
| Global Cooldown | 3 |
| Progression | 10 |
| Loadout | 8 |
| Save/Load | 5 |
| Effects Manager | 7 |
| Stress | 4 |

### 11.2 Running Tests

Tests are in `Tests/AbilitySystemTests.cs` and can be run via `TestRunner`.

---

## 12. Future Expansion

### 12.1 Planned Features

- Skill trees with branching paths
- Class-specific ability restrictions
- Ability synergy system
- Crafting interaction (ability-enhancing items)
- Advanced combat mechanics (combo abilities)
- Long-term character progression (paragon levels)

### 12.2 DLC Support

The `DlcId` field on ability data allows marking abilities as DLC-gated. The system checks DLC ownership before allowing registration/activation.