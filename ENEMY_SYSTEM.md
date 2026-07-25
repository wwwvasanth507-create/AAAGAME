# Enemy System — Hero of Eternia

**Version:** 0.11.0  
**Phase:** Prompt 11 / 150 — Gameplay Expansion  
**Status:** ✅ Production Ready

---

## Overview

The Enemy System provides a fully data-driven, headless-safe enemy framework.
All logic runs without Godot scene dependencies except `EnemyController` (the runtime Godot node).

---

## Architecture

```
enemy_database.json
        │
        ▼
   EnemyDatabase ──► EnemyDefinition (wraps EnemyData)
                              │
                              ├── GetScaledData(waveIndex)
                              └── GetDamageMultiplier(element)

   EnemyController (CharacterBody3D)
        │
        ├── EnemyStateMachine ──► EnemyContext ──► EnemyState
        ├── TakeDamage(amount)
        ├── SetTarget(node)
        └── EventBus ──► EnemyDiedEvent / EnemyHitEvent / EnemyAttackedPlayerEvent

   EnemySpawner (Node3D)
        │
        ├── WaveCompositions[]
        ├── SpawnEnemy(id, pos, wave)
        └── EventBus ──► WaveStartedEvent / WaveCompleteEvent / AllWavesCompleteEvent
```

---

## EnemyDefinition

**File:** `Scripts/Enemies/EnemyDefinition.cs`

### EnemyData fields

| Field | Type | Description |
|-------|------|-------------|
| `EnemyId` | `string` | Unique identifier key |
| `DisplayName` | `string` | UI display name |
| `Species` | `string` | Goblin, Undead, Beast, etc. |
| `MaxHp` | `float` | Base maximum HP |
| `MoveSpeed` | `float` | Movement speed (m/s) |
| `AttackDamage` | `float` | Base attack damage |
| `AttackRange` | `float` | Melee attack reach (m) |
| `AggroRange` | `float` | Detection radius (m) |
| `AttackCooldown` | `float` | Time between attacks (s) |
| `Defense` | `float` | Flat damage reduction |
| `XpReward` | `int` | XP given on death |
| `LootTableId` | `string` | Loot table reference key |
| `Behaviour` | `EnemyBehaviour` | AI profile |
| `Element` | `EnemyElement` | Elemental affinity |
| `Resistances` | `Dict<string,float>` | Element → multiplier (<1 = resist) |
| `Weaknesses` | `Dict<string,float>` | Element → multiplier (>1 = weak) |
| `HpScaleFactor` | `float` | Per-wave HP multiplier |
| `DamageScaleFactor` | `float` | Per-wave damage multiplier |

### Wave Scaling Formula

```
ScaledHp     = MaxHp × HpScaleFactor × (1 + (waveIndex−1) × 0.15)
ScaledDamage = Damage × DamageScaleFactor × (1 + (waveIndex−1) × 0.15)
```

---

## EnemyDatabase

**File:** `Scripts/Enemies/EnemyDatabase.cs`  
**Config:** `Settings/enemy_database.json`

Loads JSON at startup. Falls back to 5 embedded defaults.

### Default Enemy Roster

| ID | Name | HP | Speed | Damage | Behaviour |
|----|------|----|-------|--------|-----------|
| `goblin_grunt` | Goblin Grunt | 40 | 4.5 | 6 | Aggressive |
| `skeleton_warrior` | Skeleton Warrior | 70 | 3.0 | 12 | Patrol |
| `forest_wolf` | Forest Wolf | 55 | 6.5 | 10 | Aggressive |
| `stone_golem` | Stone Golem | 200 | 1.8 | 30 | Guard |
| `dark_mage` | Dark Mage | 45 | 2.5 | 18 | Aggressive (Ranged) |

### Elemental Matchups

| Enemy | Weak To | Resists |
|-------|---------|---------|
| Goblin Grunt | Fire ×1.5 | Poison ×0.5 |
| Skeleton Warrior | Holy ×2.0, Fire ×1.25 | Poison ×0, Ice ×0.5 |
| Forest Wolf | Fire ×1.5 | — |
| Stone Golem | Lightning ×1.5 | Fire ×0.5, Ice ×0.5, Poison ×0 |
| Dark Mage | Holy ×2.0 | Shadow ×0 |

---

## EnemyStateMachine

**File:** `Scripts/Enemies/EnemyStateMachine.cs`

### States

| State | Entry Condition | Action |
|-------|----------------|--------|
| `Idle` | No target, not patrol | Stand still |
| `Patrol` | Patrol behaviour, no target in range | Walk waypoints |
| `Alert` | Target lost from sight | Hold position briefly |
| `Chase` | Target in aggro range with LoS | Move toward target |
| `Attack` | Target in attack range + cooldown ready | Execute attack |
| `Stagger` | `ForceState(Stagger)` called | Stunned for 0.6s |
| `Retreat` | HP < 20% (Patrol behaviour only) | Move away from target |
| `Dead` | HP ≤ 0 | Terminal — QueueFree |

### Transition Table

```
Idle ──► Chase (target in range + LoS)
Patrol ──► Chase (target spotted)
Chase ──► Attack (in range + cooldown ready)
Chase ──► Alert (LoS lost)
Alert ──► Chase (regain LoS) | Idle (timeout)
Attack ──► Chase (target moved out of range)
Any ──► Stagger (ForceState)
Any ──► Dead (HP ≤ 0)
Patrol behaviour ──► Retreat (HP < 20%)
```

---

## EnemyController

**File:** `Scripts/Enemies/EnemyController.cs`  
**Extends:** `CharacterBody3D`

### Exported Properties

| Property | Default | Description |
|----------|---------|-------------|
| `EnemyId` | `"goblin_grunt"` | Enemy type to instantiate |
| `WaveIndex` | `1` | Difficulty wave index for stat scaling |

### Events Published

| Event | Payload | When |
|-------|---------|------|
| `EnemyHitEvent` | `(EnemyId, DamageDealt, RemainingHp)` | On `TakeDamage()` |
| `EnemyDiedEvent` | `(EnemyId, DisplayName, XpReward, Position)` | On death |
| `EnemyAttackedPlayerEvent` | `(EnemyId, Damage)` | On attack execution |

---

## EnemySpawner

**File:** `Scripts/Enemies/EnemySpawner.cs`  
**Extends:** `Node3D`

### Exported Properties

| Property | Default | Description |
|----------|---------|-------------|
| `MaxActiveEnemies` | `8` | Android-safe enemy cap |
| `TotalWaves` | `5` | Number of waves |
| `WaveCooldown` | `5.0s` | Pause between waves |
| `AutoStart` | `true` | Start waves on `_Ready` |

### Default Wave Compositions

| Wave | Enemies |
|------|---------|
| 1 | 3× Goblin Grunt |
| 2 | 2× Goblin + 1× Forest Wolf |
| 3 | 2× Forest Wolf + 1× Skeleton Warrior |
| 4 | 2× Skeleton Warrior + 1× Dark Mage |
| 5 (Boss) | 1× Stone Golem + 2× Goblin Grunt |

### Spawn Position Scatter

Enemies are placed around spawn point markers using a golden-ratio angular scatter to prevent overlapping:
```
angle = i × π × 0.618
pos   = spawnPoint + (cos(angle) × 1.5, 0, sin(angle) × 1.5)
```

### Events Published

| Event | When |
|-------|------|
| `WaveStartedEvent(wave, count)` | Wave begins |
| `WaveCompleteEvent(wave, remaining)` | All wave enemies dead |
| `AllWavesCompleteEvent(totalWaves)` | All 5 waves cleared |

---

## AI Asset Production — Enemy System

### 3D Model Specifications

#### 1. Goblin Grunt

| Property | Value |
|----------|-------|
| **Asset Name** | `goblin_grunt` |
| **Poly Budget** | 1,800 tris |
| **Height** | 1.2m |
| **Textures** | 1024×1024 Diffuse, Normal, Roughness (ETC2) |
| **Animations** | Idle, Walk, Attack, Hit, Death |
| **Folder** | `Assets/Characters/Enemies/` |

**AI Generation Prompt:**
> A stylized fantasy goblin character for a mobile RPG. Short stocky build (1.2m), green warty skin, yellow eyes, torn leather armour, carrying a small rusty dagger. Low-poly game-ready character, T-pose, neutral expression. Dark fantasy art style. 2048×2048 reference sheet.

---

#### 2. Skeleton Warrior

**AI Prompt:**
> A stylized fantasy skeleton warrior character for a mobile RPG. Tall undead humanoid with visible bones, wearing ancient dented iron armour and carrying a rusted sword and dented shield. Dark fantasy art style. T-pose for rigging. 2048×2048 reference sheet. Polygon budget 2,000 tris.

---

#### 3. Forest Wolf

**AI Prompt:**
> A large stylized fantasy grey wolf for a mobile RPG. Muscular build, fierce amber eyes, thick grey fur with dark markings, snarling expression. T-pose suitable for rigging. Low-poly mobile game character. 2048×2048 reference sheet. 1,500 tris.

---

#### 4. Stone Golem

**AI Prompt:**
> A massive stylized stone golem boss-type enemy for a mobile RPG. 3m tall, heavily armoured in ancient stone plates, glowing orange crystal core in chest, four claw-like stone hands, imposing silhouette. T-pose. 2048×2048 reference. 2,500 tris.

---

#### 5. Dark Mage

**AI Prompt:**
> A stylized dark sorcerer enemy for a mobile RPG. Tall robed humanoid in torn black robes with glowing purple rune embroidery, hood up, holding a dark crystal staff, glowing purple eyes beneath hood. T-pose. 2048×2048 reference. 2,000 tris.

---

### Audio SFX Specifications

| Hook Key | Description | AI Prompt |
|----------|-------------|-----------|
| `sfx_goblin_aggro` | Goblin spot player | High-pitched aggressive goblin snarl, 0.4s |
| `sfx_goblin_attack` | Goblin melee swing | Short sharp knife slash with goblin grunt, 0.3s |
| `sfx_goblin_death` | Goblin death | Pained goblin squeal fading out, 0.5s |
| `sfx_skeleton_rattle` | Skeleton aggro | Bone rattling then shield clash, 0.6s |
| `sfx_skeleton_swing` | Skeleton sword | Heavy iron sword swing whoosh, 0.4s |
| `sfx_skeleton_crumble` | Skeleton death | Bones clattering and crumbling to ground, 0.8s |
| `sfx_wolf_howl` | Wolf aggro | Threatening wolf growl building to short howl, 0.8s |
| `sfx_wolf_bite` | Wolf attack | Fast snapping jaw bite impact, 0.25s |
| `sfx_wolf_death` | Wolf death | Pained wolf whimper fading, 0.6s |
| `sfx_golem_awaken` | Golem aggro | Deep stone grinding and cracking, 1.0s |
| `sfx_golem_slam` | Golem attack | Massive ground-shaking stone slam impact, 0.7s |
| `sfx_golem_shatter` | Golem death | Huge stone explosion and rubble cascade, 1.2s |
| `sfx_mage_cackle` | Dark Mage aggro | Sinister echoing cackle, 0.7s |
| `sfx_mage_cast` | Dark Mage attack | Eerie dark magic charge and release, 0.6s |
| `sfx_mage_death` | Dark Mage death | Pained shriek fading with dark energy dispersal, 0.9s |
