# Project Memory — Hero of Eternia

> Central knowledge base for all development decisions, architecture rules, and project state.  
> Last Updated: 2026-07-25

---

## Project Identity

| Field | Value |
|-------|-------|
| **Title** | Hero of Eternia |
| **Engine** | Godot 4.3 (Mono/C#) |
| **Target Platform** | Android (primary), PC (secondary) |
| **Genre** | 3D Action RPG |
| **Version** | 0.8.0 |
| **Assembly** | HeroOfEternia |

---

## Foundation Audit Status

| Status | Score |
|--------|-------|
| ✅ **APPROVED** | **8.4 / 10** |

### Audit Verdict
- Prompts 0–4: Successfully completed
- Ready for Prompt 5: **CONDITIONALLY YES**
- Recommended Prompt 5 focus: Real SceneManager, Real AudioManager, Expand testing, AI asset version tracking

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
| SaveManager | ✅ Complete | AES-256 + SHA-256 + backups |
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
| UIManager | ⚠️ Incomplete | Needs layout screens |
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

### Player Systems
| System | Status | Notes |
|--------|--------|-------|
| PlayerRoot | ✅ Complete | CharacterBody3D with modules |
| PlayerMovement | ✅ Complete | Ground/Air/Jump/Surface detection |
| PlayerStateMachine | ✅ Complete | 24 states (SOLID FSM) |
| PlayerData | ✅ Complete | Stats, vitals, XP, stamina (bridges to attributes) |
| InputHandler | ✅ Complete | InputFrame snapshot |
| TouchControls | ✅ Complete | Virtual joystick + gestures |
| CameraController | ✅ Complete | Spring-arm, shake, lock-on |
| PlayerAnimationController | ✅ Complete | AnimationTree + fallback blending |
| PlayerAudioController | ✅ Complete | Footstep relay |
| PlayerModelController | ✅ Complete | Dynamic slot swapper, LOD, customize |
| PlayerInteractionDetector | ✅ Complete | Area3D overlaps, Tap/Hold/Auto modes |
| PlayerAttributeSet | ✅ Complete | Capped caching formula, modifiers |
| PlayerEffectsController | ✅ Complete | Status VFX overlays |
| InventorySlot | ✅ Complete | Count, lock/favorite status, custom stats |
| InventoryContainer | ✅ Complete | Merges, splits, filters, favorite sorting |
| EquipmentManager | ✅ Complete | 12 slots, dynamic attribute set modifier hooks |
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
- SaveVersion field enables forward-compatible migrations
- MigrateProfile() hook prepared for version transitions
- JsonExtensionData preserves unknown fields

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

### Current Test Suite (33 tests)
| Test | Type | Status |
|------|------|--------|
| ServiceLocator DI & Boot | Integration | ✅ |
| SettingsManager Persistence | Integration | ✅ |
| ConfigManager Hot-Reload | Integration | ✅ |
| DeviceDetector Query | Integration | ✅ |
| SaveManager AES + Backup | Integration | ✅ |
| InputActionMap Registration | Integration | ✅ |
| PlayerData Stats/Stamina/XP | Unit | ✅ |
| PlayerStateMachine Transitions | Unit | ✅ |
| PlayerSettings Persistence | Integration | ✅ |
| PlayerModel Slot Swap & LOD | Unit | ✅ |
| Attributes & Modifiers | Unit | ✅ |
| Interaction Detector Closest | Unit | ✅ |
| Player VFX Status Effects | Unit | ✅ |
| Save V2 Slot Write/Load & Migration | Integration | ✅ |
| Item Database Configuration Loads | Unit | ✅ |
| Stacks Merging & Splitting | Unit | ✅ |
| Inventory Multi-Sort & Filtering | Unit | ✅ |
| Equipment Assignment Modifiers | Unit | ✅ |
| Save V3 Slot Serialization | Integration | ✅ |
| Loot Table Roll Resolutions | Unit | ✅ |
| Consumable Item Effect Resolvers | Unit | ✅ |
| WorldSeed Text Hashing Determinism | Unit | ✅ |
| Deterministic Float PRNG Rolls | Unit | ✅ |
| Biomes Loader & Database Fallbacks | Unit | ✅ |
| Time Cycles Stages Switches | Unit | ✅ |
| Chunk Async Streaming & Modifying | Unit | ✅ |
| Save V4 Serialization & Migration | Integration | ✅ |
| Layered Terrain Heights | Unit | ✅ |
| Navigation Walkable Grids | Unit | ✅ |
| Vegetation preset densities | Unit | ✅ |
| Landmarks Populator | Unit | ✅ |
| World Validator checks | Unit | ✅ |
| Save V5 Serialization & Migration | Integration | ✅ |

### Coverage Gaps
- GameManager state transitions
- CameraController
- TouchControls
- PlayerMovement physics
- EventBus edge cases
- Logger failsafe behavior

---

## Known Limitations

1. UIManager is an incomplete stack tracker — needs actual layout screen control bindings
2. Gameplay systems (Combat, Enemy, World) not yet implemented
3. No actual AI-generated assets imported yet — only placeholder README files

---

## Next Steps (Prompt 8)

1. **Offline Database & SQLite Storage** — Establish SQLite tables for items, quests, and world configuration, linking back to the Save System.
2. **3D Shaders & Presets** — Develop vertex and outline shading systems.
3. **UIManager & HUD Overlays** — Interface overlays and inventory HUD screen menus.
4. **Gameplay & Combat Systems** — Begin Combat triggers and weapon hitboxes.

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