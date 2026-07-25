# Project Memory — Hero of Eternia

> Central knowledge base for all development decisions, architecture rules, and project state.  
> Last Updated: 2026-07-25 (Phase 15)

---

## Project Identity

| Field | Value |
|-------|-------|
| **Title** | Hero of Eternia |
| **Engine** | Godot 4.3 (Mono/C#) |
| **Target Platform** | Android (primary), PC (secondary) |
| **Genre** | 3D Action RPG |
| **Version** | 0.15.0 |
| **Assembly** | HeroOfEternia |

---

## Foundation Audit Status

| Audit | Score | Date |
|-------|-------|------|
| Prompts 0–4 Audit | 8.4/10 | 2026-07-24 |
| Prompts 0–5 Audit | 9.1/10 | 2026-07-24 |
| Prompts 0–6 Audit | 9.4/10 | 2026-07-24 |
| Prompts 0–7 Audit | 9.5/10 | 2026-07-25 |
| Prompts 0–8 Audit | 9.7/10 | 2026-07-25 |
| Prompts 0–9 Audit | 10.0/10 | 2026-07-25 |
| **Prompts 0–10 Audit** | **10.0/10** | **2026-07-25** |
| **Phase 13 (Ability System)** | **Complete** | **2026-07-25** |
| **Phase 14 (Equipment/Progression)** | **Complete** | **2026-07-25** |
| **Phase 15 (Gathering & Crafting)** | **Complete** | **2026-07-25** |

### Phase 13 Status (Complete)
- Ability Database: ✅ 10 abilities with full data fields
- Ability Categories: ✅ 11 categories with extensible CategoryManager
- Ability Manager: ✅ Full execution framework
- Resource Framework: ✅ 7 resource types
- Player Progression: ✅ Level 1-100, XP curves, prestige
- Ability Loadouts: ✅ 6 configurable loadouts
- Ability Effects: ✅ 12 effect types
- Save Integration: ✅ Save V10
- Tests: ✅ 98 tests
- Documentation: ✅ Complete

### Phase 14 Status (Complete)
- Attribute Calculation Engine: ✅ Centralized deterministic engine with 10 modifier layers
- Expanded Attributes: ✅ 40+ attribute types
- Item Modifier System: ✅ 5 stacking rules, 22 default presets
- Enchantment Framework: ✅ 23 enchantments across 10 elements
- Durability System: ✅ Per-item durability with damage sources
- Gear Set System: ✅ 4 default sets with tiered bonuses
- Item Quality System: ✅ 8 quality grades (Broken→Divine)
- Upgrade Framework: ✅ 10 upgrade levels
- Save Integration: ✅ Save V11
- Tests: ✅ 17 tests
- Documentation: ✅ Complete

### Phase 15 Status (Complete)
- Resource Database: ✅ 27 resources with full data fields, biome/category/tool indices
- Resource Types: ✅ 15 categories with subcategories (Trees, Ore, Stone, Herbs, Crystals, etc.)
- Profession System: ✅ 14 professions with XP curves, level 1-100, unlocks, bonuses
- Gathering System: ✅ Tool validation, node health, critical gather, bonus yield, depletion, respawn
- Recipe Database: ✅ 20 recipes with profession/level/ingredient/workstation requirements
- Crafting Manager: ✅ Instant craft, timed queue, batch craft, cancellation, pause/resume
- Workstation Framework: ✅ 16 workstation definitions with tiered bonuses
- Resource Regeneration: ✅ Biome modifiers, seasonal modifiers, in-season bonuses
- Save Integration: ✅ Save V12 with profession states, node states, known recipes, craft queue
- Tests: ✅ 20 tests covering all systems with stress testing
- Documentation: ✅ RESOURCE_SYSTEM.md, CRAFTING_SYSTEM.md, PROFESSION_SYSTEM.md, WORKSTATION_SYSTEM.md
- Build: ✅ 0 warnings, 0 errors
- Ready for Prompt 16: **YES**

---

## Permanent Engineering Rules (Post-Audit)

### Rule 1: NO MOCK PRODUCTION SYSTEMS
Temporary mock implementations are allowed only during prototype phases. Before any release milestone:
- Replace mocked systems with real implementations
- Remove fake async behavior
- Remove placeholder service responses
- Validate runtime behavior

**Priority systems**: SceneManager, AudioManager, ResourceLoader, Save systems, Asset systems, Input systems

### Rule 2: MANAGER STANDARDIZATION
Every major manager/service must implement:
- `IInitializable` interface
- Initialization state tracking
- Shutdown handling
- Error reporting
- Unit tests
- Documentation

**Required lifecycle**: Created → Initialized → Active → Shutdown

### Rule 3: EVENT SYSTEM RULES
EventBus must always maintain:
- Thread-safe subscription
- Thread-safe publishing
- Listener cleanup
- Duplicate prevention
- Error isolation
- Performance monitoring

No system may directly depend on unsafe event access.

### Rule 4: AI ASSET PIPELINE STANDARD
All assets must include:
- Asset ID
- Version number
- Category
- Source prompt
- Generation metadata
- Optimization status
- Validation status

**Required asset flow**: Concept → AI Generation → Review → Optimization → Integration → Validation

### Rule 5: TESTING REQUIREMENT
Every new major system requires:
- Unit test
- Integration test
- Documentation entry
- Performance consideration
- Failure handling

No manager is considered complete without tests.

### Rule 6: OFFLINE-FIRST RULE
Hero of Eternia remains offline playable by default. No critical gameplay dependency on servers, accounts, internet connection, or online services. Online features must remain optional layers.

### Rule 7: ANDROID PERFORMANCE RULE
Every feature must consider: Memory budget, CPU cost, GPU cost, Battery impact, Storage usage, Loading time. Target: Stable mobile RPG performance across supported devices.

### Rule 8: DOCUMENTATION RULE
Every phase must update: PROJECT_MEMORY.md, ROADMAP.md, CHANGELOG.md. Every major system must have: Technical documentation, Usage documentation, Validation documentation.

---

## Architecture Overview

### Core Pattern
```
Godot Lifecycle → ServiceLocator (DI) → Manager Init → EventBus (Pub-Sub)
```

### Key Systems
| System | Status | Notes |
|--------|--------|-------|
| ServiceLocator | ✅ Complete | Thread-safe DI with IInitializable |
| EventBus | ✅ Complete | Thread-safe (lock-protected) |
| GameManager | ✅ Complete | 6-state lifecycle machine |
| SaveManager | ✅ Complete | AES-256 + SHA-256 + backups, Save V12 |
| SettingsManager | ✅ Complete | Auto-persisted JSON |
| ConfigManager | ✅ Complete | Hot-reload, templates |
| DeviceDetector | ✅ Complete | Hardware heuristics |
| PerformanceManager | ✅ Complete | Dynamic resolution scaling |
| PerformanceMonitor | ✅ Complete | Dev telemetry overlay |
| ErrorSystem | ✅ Complete | Crash logging |
| Logger | ✅ Complete | Thread-safe, dual-routing |
| LocalizationManager | ✅ Complete | 14-key English, hot-swap |
| SceneManager | ✅ Complete | Real ResourceLoader async loading |
| AudioManager | ✅ Complete | Node-based pooling and AudioServer routing |
| UIManager | ✅ Complete | Stack layer controller & HUD event listener |
| ResourceManager | ✅ Complete | Real caching and preloading |
| ItemDatabase | ✅ Complete | Data-driven JSON loading, fast lookups |
| LootTable | ✅ Complete | Dynamic loot drop table roller |
| ItemEffectsFramework | ✅ Complete | Consumables healing & buff resolvers |
| WorldSeed | ✅ Complete | Deterministic FNV-1a hashing & hex parsing |
| WorldDatabase | ✅ Complete | Data-driven biomes & element specifications |
| ChunkManager | ✅ Complete | Asynchronous chunk streaming, distance buffers |
| ResourceSpawner | ✅ Complete | Spawning chance, biomes, and slope limits |
| WorldTimeSystem | ✅ Complete | Day/night cycle stage intervals |
| WeatherManager | ✅ Complete | Transitions climate profile offsets |
| TerrainGenerator | ✅ Complete | Layered simplex & ridged noise height computations |
| NavigationFoundation | ✅ Complete | Walkable cell grid slope calculations |
| VegetationSystem | ✅ Complete | Dynamic plant density scaling based on presets |
| WorldPopulationManager | ✅ Complete | Data-only landmark coordinate layout planner |
| WorldValidator | ✅ Complete | Scans chunks to detect floating meshes and overlaps |

### Phase 15 Systems
| System | Status | Notes |
|--------|--------|-------|
| ResourceDatabase | ✅ Complete | 27 resources, 15 categories, indexed lookups |
| ProfessionManager | ✅ Complete | 14 professions, XP curves, level 1-100, unlocks |
| GatheringManager | ✅ Complete | Tool validation, node health, critical/bonus yield |
| RecipeDatabase | ✅ Complete | 20 recipes, profession/level/workstation indexed |
| CraftingManager | ✅ Complete | Instant/queued/batch craft, cancellation, pause |
| WorkstationManager | ✅ Complete | 16 workstation definitions with tiered bonuses |
| ResourceRegeneration | ✅ Complete | Biome/season modifiers, respawn timing |

### Gameplay Expansion & UI
| System | Status | Notes |
|--------|--------|-------|
| BootController | ✅ Complete | Initializes all services and manages boot flow |
| MainMenuController | ✅ Complete | Handles wired main menu button transitions |
| HUD Controller | ✅ Complete | EventBus-driven CanvasLayer |
| EnemyDefinition | ✅ Complete | Multi-dimensional stat blocks |
| EnemyDatabase | ✅ Complete | Configurable registry |
| EnemyStateMachine | ✅ Complete | Headless 8-state AI FSM |
| EnemyController | ✅ Complete | CharacterBody3D node |
| EnemySpawner | ✅ Complete | Wave-based spawner |
| AbilityManager | ✅ Complete | Full execution framework |
| CategoryManager | ✅ Complete | 11 default categories |
| EffectsManager | ✅ Complete | 12 effect types |
| LoadoutManager | ✅ Complete | 6 configurable loadouts |
| ResourceManager | ✅ Complete | 7 resource types |
| PlayerProgression | ✅ Complete | Level 1-100, XP curves, prestige |
| GameLoop | ✅ Complete | Session timer, XP leveling, waves |

### NPC Systems
| System | Status | Notes |
|--------|--------|-------|
| NpcDefinition | ✅ Complete | NpcData, 15 types, NpcSaveState |
| NpcStateMachine | ✅ Complete | 12-state FSM |
| NpcScheduler | ✅ Complete | Time-fraction schedule blocks |
| RelationshipSystem | ✅ Complete | Friendship/Trust/Respect/Fear |
| ReputationSystem | ✅ Complete | Event-driven scoped reputation |
| DialogueFramework | ✅ Complete | Localization-key resolver |
| NpcSpawner | ✅ Complete | Deterministic placements |
| NpcNavigationAgent | ✅ Complete | Cell-validated movement |
| NpcManager | ✅ Complete | Central service, 0.5s throttled tick |

### Combat Systems
| System | Status | Notes |
|--------|--------|-------|
| CombatManager | ✅ Complete | Orchestrates attacks, targeting, projectiles |
| WeaponDefinition | ✅ Complete | WeaponData + WeaponDatabase |
| TargetingSystem | ✅ Complete | SoftLock, HardLock, LoS |
| HitDetection | ✅ Complete | Sphere/AABB melee sweeps |
| DamageSystem | ✅ Complete | Physical/Elemental/True calculations |
| StatusEffectSystem | ✅ Complete | Buff/de-buff registry |
| ProjectileSystem | ✅ Complete | Headless physics, Homings |

### Boss & Encounter Systems
| System | Status | Notes |
|--------|--------|-------|
| BossDefinition | ✅ Complete | BossData model |
| BossDatabase | ✅ Complete | Central registry |
| BossPhaseSystem | ✅ Complete | HP threshold transitions |
| EliteSystem | ✅ Complete | Data-driven flags |
| ArenaFramework | ✅ Complete | Entry/exit markers, SafeZones |
| EncounterManager | ✅ Complete | Battle state coordination |
| RewardFramework | ✅ Complete | Secure reward claims |

### Player Systems
| System | Status | Notes |
|--------|--------|-------|
| PlayerRoot | ✅ Complete | CharacterBody3D with modules |
| PlayerMovement | ✅ Complete | Ground/Air/Jump/Surface detection |
| PlayerStateMachine | ✅ Complete | 24 states (SOLID FSM) |
| PlayerData | ✅ Complete | Stats, vitals, XP, stamina |
| InputHandler | ✅ Complete | InputFrame snapshot |
| TouchControls | ✅ Complete | Virtual joystick + gestures |
| CameraController | ✅ Complete | Spring-arm, shake, lock-on |
| PlayerAnimationController | ✅ Complete | AnimationTree + fallback blending |
| PlayerAudioController | ✅ Complete | Footstep relay |
| PlayerModelController | ✅ Complete | Dynamic slot swapper, LOD |
| PlayerInteractionDetector | ✅ Complete | Area3D overlaps, Tap/Hold/Auto |
| PlayerAttributeSet | ✅ Complete | Capped caching formula |
| PlayerEffectsController | ✅ Complete | Status VFX overlays |
| InventorySlot | ✅ Complete | Count, lock/favorite, custom stats |
| InventoryContainer | ✅ Complete | Merges, splits, filters, sorting |
| EquipmentManager | ✅ Complete | 12 slots, attribute modifier hooks |
| PlayerSettings | ✅ Complete | Auto-persistent |

---

## Save System Architecture

### Data Flow
```
SaveProfile (JSON) → AES-256 Encrypt → SHA-256 Checksum → File (.sav + .bak)
```

### Security
- Device-bound key (AppSalt + OS.GetUniqueId())
- PBKDF2 key derivation (1000 iterations)
- SHA-256 integrity verification on load
- Automatic backup recovery on corruption

### Schema Migration
- SaveVersion field enables forward-compatible migrations (current: V12)
- MigrateProfile() hook prepared for version transitions
- JsonExtensionData preserves unknown fields

### Save V12 (Phase 15)
- All Save V11 content
- ProfessionStates (14 professions with level, XP, unlocks)
- ResourceNodeStates (depletion, respawn timers per node)
- KnownRecipeIds (unlocked recipe list)
- CraftQueueItems (active queue for resume)
- V11→V12 migration initializes empty collections

### Save V11 (Phase 14)
- All Save V10 content
- EquipmentSaveData with durability, upgrades, enchantments, quality, modifiers, sets

### Save V10 (Phase 13)
- Unlocked ability IDs, ability levels, loadout data
- Ability manager runtime state, progression data

---

## AI Asset Pipeline

### Asset Flow
```
Concept → AI Generation → Review → Optimization → Integration → Validation
```

### Required Metadata
- Asset ID, Version, Category, Source prompt
- Generation metadata, Optimization status, Validation status

### Format Standards
| Type | Format | Spec |
|------|--------|------|
| 3D Models | glTF 2.0 (.glb) | Hero <3K tris, Enemy <2.5K, Props <800 |
| Textures | PBR (Metallic/Roughness/Normal/AO) | 2048/1024/512px, ETC2/ASTC |
| Audio | WAV | SFX prompts in AUDIO_SPEC.md |
| UI | PNG | 2048x2048 max |

---

## Testing Status

### Current Test Suite (190 tests)
| Test | Type | Status |
|------|------|--------|
| ... (170 previous tests) | ... | ✅ |
| P15-1 Resource Database Load | Unit | ✅ |
| P15-2 Resource Database Lookups | Unit | ✅ |
| P15-3 Profession System Init | Unit | ✅ |
| P15-4 Profession Leveling | Unit | ✅ |
| P15-5 Profession XP Requirements | Unit | ✅ |
| P15-6 Gathering Validation | Unit | ✅ |
| P15-7 Gathering Execution | Unit | ✅ |
| P15-8 Recipe Database Load | Unit | ✅ |
| P15-9 Recipe Database Lookups | Unit | ✅ |
| P15-10 Crafting Validation | Unit | ✅ |
| P15-11 Crafting Instant | Unit | ✅ |
| P15-12 Crafting Queue | Unit | ✅ |
| P15-13 Crafting Batch | Unit | ✅ |
| P15-14 Workstation Definitions | Unit | ✅ |
| P15-15 Workstation Bonuses | Unit | ✅ |
| P15-16 Resource Regeneration | Unit | ✅ |
| P15-17 Save Integration | Unit | ✅ |
| P15-18 Stress Resource Lookups | Unit | ✅ |
| P15-19 Stress Recipe Lookups | Unit | ✅ |

### Coverage Gaps
- GameManager state transitions
- CameraController
- TouchControls
- PlayerMovement physics
- EventBus edge cases
- Logger failsafe behavior

---

## Known Limitations

1. UIManager handles basic screen stacks — HUD is fully integrated; custom panel bindings will expand in Phase 16
2. Boss, Elite AI, and quest frameworks are complete; specific quest-lines and story encounters will be added in Phase 16
3. No final visual/audio assets imported yet — README prompt specifications exist for all 10 categories
4. Ability system is framework-only — no skill trees, class restrictions, or balance implemented yet
5. Gathering and crafting systems are framework-only — no UI, merchant integration, or quest integration yet

---

## Next Steps (Prompt 16)

1. **Merchant & Economy System** — Build NPC merchants, buy/sell mechanics, gold economy, and item pricing.
2. **Quest Crafting Integration** — Quest-specific recipes, gathering objectives, and profession-based quest rewards.
3. **Player Housing & Building** — Building placement, furniture crafting, and player-owned property system.

---

## World System Rules

1. All procedural generation must be deterministic.
2. World changes must be saved through versioned serialization.
3. Chunk systems must never block the main gameplay thread.
4. Every biome must be data-driven.
5. Environmental systems must support future gameplay integration.
6. Procedural assets must use AI asset metadata tracking.
7. World streaming must maintain Android memory limits.
8. New world systems require save migration testing.