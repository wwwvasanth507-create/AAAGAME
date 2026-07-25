# Project Memory — Hero of Eternia

> Central knowledge base for all development decisions, architecture rules, and project state.  
> Last Updated: 2026-07-25 (Phase 21 — Complete Audio Framework)

---

## Project Identity

| Field | Value |
|-------|-------|
| **Title** | Hero of Eternia |
| **Engine** | Godot 4.3 (Mono/C#) |
| **Target Platform** | Android (primary), PC (secondary) |
| **Genre** | 3D Action RPG |
| **Version** | 0.21.0 |
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
| **Phase 16 (Settlement Simulation)** | **Complete** | **2026-07-25** |
| **Phase 17 (Social Simulation)** | **Complete** | **2026-07-25** |
| **Phase 19 (Quest & Dialogue Framework)** | **Complete** | **2026-07-25** |
| **Phase 20 (UI/UX Framework)** | **Complete** | **2026-07-25** |

### Phase 20 Status (Complete) — UI/UX Framework
- **UIManager**: Complete rewrite with screen lifecycle, navigation stack (max depth 20), modal dialog system, 10-layer management, focus management, Tween-based transition animations, input routing, UI state persistence (UIPreferences), and plugin system (IUIPlugin). Registered in ServiceLocator as IInitializable.
- **Screen Framework**: 20 reusable screen types via ScreenRegistry — MainMenu, PauseMenu, Settings (tabs: Audio/Graphics/Controls/Accessibility), Inventory (grid + detail panel), Equipment (10 slots), Character (stats display), Abilities (ability list), QuestJournal (list + detail), Map (full-screen placeholder), Crafting (recipe list), Trading (merchant + player inventories), Dialogue (speaker + text + choices), Notifications (history), Loading (progress + tips), GameOver (retry/load/quit), SaveLoad (10 slots), Bestiary, Codex, Achievements, and DLCPlaceholder. All support lazy loading via OnLazyLoad().
- **HUD System**: Modular HUDController with 14 independently enabled/disabled widgets — Health, Mana, Stamina, Experience (progress bars + labels), Compass (N/NE/E/etc. direction), MiniMap (hook), QuestTracker (add/remove/clear), AbilityBar (6 slots), InteractionPrompt (show/hide), BuffDebuff (add/clear), StatusEffect (add/clear), TargetInfo (name/level/HP), BossHealth (show/update/hide with percentage), FPSDebug (FPS + memory, 0.5s update, dev-only). All widgets implement IAccessibleWidget for text scale and high contrast. EventBus-driven updates.
- **Notification System**: NotificationManager with priority queue (Low/Normal/High/Critical), max 5 visible, max 50 queued, color-coded priority styling, fade-out animation, history tracking, convenience methods (QuestUpdated, LevelUp, ItemAcquired, AchievementUnlocked, CraftComplete, SystemMessage, Warning, Error), handler system (INotificationHandler), and full persistence.
- **Input Integration**: UIInputHandler with touch, mouse, keyboard, and gamepad (future) support. Gesture hooks for long press (0.5s), double tap (0.3s interval), drag & drop (10px threshold), pinch, and swipe. Input rebinding framework with 8 default actions (accept/cancel/pause/inventory/character/journal/map/abilities). IGestureHandler interface for extensible gesture processing.
- **Responsive Layout**: ResponsiveLayout with 4 device categories (Phone ≤480dp, SmallTablet ≤768dp, LargeTablet ≤1024dp, Desktop >1024dp), DPI-aware scaling (160 base DPI), safe areas (status bar + nav bar), orientation detection with events, foldable device hooks, and per-category layout presets (grid columns, sidebar, bottom nav, padding, font scale).
- **Accessibility**: AccessibilityManager with adjustable text scale (0.5x–2.0x), high contrast mode, color-blind friendly hooks (Protanopia/Deuteranopia/Tritanopia shader hooks), subtitle framework (show/hide, auto-fade, adjustable size), reduced motion mode (70% shorter tweens), screen reader labels (Accessible + TooltipText), haptic feedback toggle (Light/Medium/Heavy), future voice navigation hooks, and full settings persistence via SettingsManager.
- **Tests**: UISystemTests with 100+ test cases covering UIManager (initialization, screen registration, navigation, stack depth, modals, layers, focus, transitions, back button, plugins), all 20 screen types, all 14 HUD widgets, notification system (queue, priority, duration, convenience methods, handlers, clear, history, stress test 1000), responsive layout (presets, safe areas, orientation, elements, foldables), accessibility (text scale, high contrast, color blind, subtitles, reduced motion, screen reader, haptics), input (action registration/rebinding, gesture handlers), save/load persistence, and stress tests (50 rapid navigations, 10 concurrent modals).
- **Documentation**: UI_SYSTEM.md, HUD_SYSTEM.md, NOTIFICATION_SYSTEM.md, ACCESSIBILITY.md, RESPONSIVE_LAYOUT.md created. PROJECT_MEMORY.md, ARCHITECTURE.md, ROADMAP.md, CHANGELOG.md updated.

### Phase 19 Status (Complete) — Quest & Dialogue Framework
- **QuestDatabase**: Data-driven quest registry with 18 quest categories, O(1) lookups, indexed by category/giver/faction. Thread-safe. JSON loadable. Supports thousands of quests.
- **QuestDefinition**: Complete data model with QuestId, InternalName, DisplayName, Description, Category, RecommendedLevel, QuestGiver, RequiredFaction, RequiredReputation, Prerequisites, Branches, Objectives, Rewards, FailureConditions, TimeLimits, Repeatability, LocalizationKeys, DLC fields, co-op support.
- **ObjectiveManager**: 16 objective types (TalkToNpc, ReachLocation, DefeatEnemy, DefeatBoss, CollectItem, CraftItem, GatherResource, DeliverItem, Interact, EscortNpc, Survive, UseAbility, VisitSettlement, ExploreArea, TriggerEvent, Custom). Unlimited objective chains. Prerequisite-based activation. Branching on complete/fail. Optional objectives. Float/count progression.
- **QuestManager**: Full lifecycle management (Accept, Complete, Fail, Abandon, Retry). Prerequisite evaluation. Repeatability/schedule checks. Time limit tracking. Survival objective updates. Reward distribution. Quest history tracking. Full save/load.
- **QuestBranch**: Branching quest paths with conditions, objectives, and transition hooks. Supports complex narrative trees.
- **DialogueDatabase**: Data-driven conversation registry. O(1) dialogue/conversation lookups. NPC-indexed conversations. JSON loadable. Thread-safe.
- **DialogueManager**: Branching dialogue execution engine. Conversation flow (start/advance/end). Choice selection with condition filtering. Quest hooks (accept/advance/complete/fail). Flag setting. Decision recording. Merchant/service hooks. Cinematic hooks. Loop prevention (max depth + visited set). Full save/load.
- **DialogueEntry**: Complete data model with SpeakerId, SpeakerType, TextKey, AudioKey, EmotionHook, AnimationHook, CameraHook, VfxHook, Conditions, Choices, QuestHooks, NextDialogueId, IsEndOfConversation, nested conversation support.
- **DialogueChoice**: Choice data model with Conditions, NextDialogueId, SetFlag, RecordDecision, QuestHook, Rewards, MerchantHook, ServiceHook, CinematicHook.
- **NarrativeManager**: Central narrative state tracker. Global/regional flags. World/NPC/dialogue variables. Player decisions. Story chapter tracking. Condition evaluation engine (flag, variable, quest, chapter, decision, npc, region). Full save/load.
- **JournalManager**: Quest journal (active/completed/failed). Lore entry system. Dialogue log. Discovery log. Future bestiary/codex hooks. Full save/load.
- **Save Integration**: SaveProfile V15 with QuestData, JournalData, NarrativeData, DialogueData. Version 15 migration.
- **Data Files**: Settings/quest_database.json (3 example quests with branching). Settings/dialogue_database.json (1 example conversation with 6 dialogue entries, branching choices, quest hooks).
- **Tests**: 55 tests covering QuestDatabase (8), QuestManager (8), ObjectiveManager (7), NarrativeManager (8), JournalManager (6), DialogueDatabase (4), DialogueManager (5), Stress tests (4), Edge cases (5).
- **Documentation**: QUEST_SYSTEM.md, DIALOGUE_SYSTEM.md, OBJECTIVE_SYSTEM.md, JOURNAL_SYSTEM.md, NARRATIVE_SYSTEM.md created. PROJECT_MEMORY.md, ARCHITECTURE.md, ROADMAP.md, CHANGELOG.md updated.

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

### Phase 16 Status (Complete)
- Settlement Database: ✅ 6 settlements with full data fields, indexed lookups
- Settlement Types: ✅ 15 type definitions (Camp→Capital + special types)
- Building Database: ✅ 25+ building definitions with 14 categories
- NPC Schedules: ✅ 6 per-profession schedules with weather/festival/emergency adaptation
- World Event Framework: ✅ 8 event templates with lifecycle, cooldowns, severity scaling
- Settlement Manager: ✅ Central orchestrator with load/unload, NPC spawning, daily updates
- Public Services: ✅ 20 service definitions integrated with buildings
- Save Integration: ✅ Save V14 with settlement states, building states, world events
- Tests: ✅ 40 tests covering all systems with stress testing
- Documentation: ✅ SETTLEMENT_SYSTEM.md
- Build: ✅ 0 warnings, 0 errors

### Phase 17 Status (Complete) — Social Simulation Framework
- Faction Database: ✅ 9 default factions with full data fields, 16 FactionType definitions, 9 Alignment types. JSON data-driven with runtime registration.
- Faction Lookups: ✅ By ID, type, region. Lightweight FactionReference for UI. Thread-safe. Event-driven change notifications.
- Reputation Manager: ✅ 5 scopes (Global, Region, Faction, Settlement, Individual). Configurable tier thresholds (8 default tiers: Hated→Legendary). Bulk operations. Thread-safe. Event-driven.
- Reputation Modifier Registry: ✅ 25 data-driven modifiers across 8 categories (help, attack, trade, combat, donation, crime, dialogue, faction_event). Runtime registration. Per-faction/settlement overrides.
- Crime Manager: ✅ 7 crime types, 5 severity levels. Witness detection with distance/scaling. Bounty management (per-faction + global). Crime expiration. Full save/load.
- Guard AI System: ✅ 12 guard states (Patrol→ReturnToPatrol). Configurable per-guard parameters. Settlement alert levels. Reinforcement calling. Crime-triggered investigation. 0.25s throttled updates.
- Diplomacy Framework: ✅ 7 diplomatic relations (Alliance→Ceasefire). Default initialization from faction data. Allies/enemies queries. Diplomatic reputation modifiers.
- NPC Reaction System: ✅ 10 factor evaluation (reputation, crime, faction, time, security, occupation, personality, world events, weather, player level). Disposition scoring -100 to +100. Attack/flee/trade/report thresholds.
- SocialManager Orchestrator: ✅ Central service integrating all subsystems. Cross-system event wiring (crime→guard, diplomacy→faction). Apply reputation modifiers by ID. Full save/load with SocialSaveData V1.
- Performance: ✅ Throttled guard AI (0.25s tick). O(1) dictionary lookups. Thread-safe locks. Stress tested with 100 guards, 50+ factions.
- Documentation: ✅ FACTION_SYSTEM.md, REPUTATION_SYSTEM.md (updated), CRIME_SYSTEM.md, GUARD_SYSTEM.md, DIPLOMACY_SYSTEM.md
- Tests: ✅ 98 tests covering all systems + stress testing + edge cases
- Build: ✅ No code compilation errors

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
| SaveManager | ✅ Complete | AES-256 + SHA-256 + backups, Save V15 |
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

### Phase 19 Systems (Quest & Dialogue)
| System | Status | Notes |
|--------|--------|-------|
| QuestDatabase | ✅ Complete | Data-driven quest registry, O(1) lookups, 18 categories |
| QuestManager | ✅ Complete | Full lifecycle, branching, rewards, save/load |
| ObjectiveManager | ✅ Complete | 16 objective types, chains, prerequisites, branching |
| NarrativeManager | ✅ Complete | Flags, variables, decisions, chapters, condition eval |
| JournalManager | ✅ Complete | Quest journal, lore, dialogue log, discoveries |
| DialogueDatabase | ✅ Complete | Data-driven conversation registry, O(1) lookups |
| DialogueManager | ✅ Complete | Branching dialogue, conditions, quest hooks, loop prevention |

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
- SaveVersion field enables forward-compatible migrations (current: V15)
- MigrateProfile() hook prepared for version transitions
- JsonExtensionData preserves unknown fields

### Save V15 (Phase 19)
- All Save V14 content
- QuestSaveData (active instances, completion records, history)
- JournalSaveData (active/completed/failed entries, lore, dialogue log, discoveries)
- NarrativeSaveData (global/regional flags, world/npc variables, player decisions, story chapters)
- DialogueManagerSaveData (active conversation state, depth tracking, visited dialogues)

### Save V14 (Phase 16)
- All Save V13 content
- SettlementSaveData (settlement states, building states, world events)

### Save V13 (Phase 15 Economy)
- All Save V12 content
- EconomySaveData (economy system state)

### Save V12 (Phase 15)
- All Save V11 content
- ProfessionStates (14 professions with level, XP, unlocks)
- ResourceNodeStates (depletion, respawn timers per node)
- KnownRecipeIds (unlocked recipe list)
- CraftQueueItems (active queue for resume)

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

### Current Test Suite (245 tests)
| Test | Type | Status |
|------|------|--------|
| ... (190 previous tests) | ... | ✅ |
| P19-1 QuestDB Empty Init | Unit | ✅ |
| P19-2 QuestDB Register Single | Unit | ✅ |
| P19-3 QuestDB Register Multiple | Unit | ✅ |
| P19-4 QuestDB Get By Category | Unit | ✅ |
| P19-5 QuestDB Get By Giver | Unit | ✅ |
| P19-6 QuestDB Search | Unit | ✅ |
| P19-7 QuestDB Clear | Unit | ✅ |
| P19-8 QuestDB Stress Lookup | Unit | ✅ |
| P19-9 QuestMgr Accept | Unit | ✅ |
| P19-10 QuestMgr Complete | Unit | ✅ |
| P19-11 QuestMgr Fail | Unit | ✅ |
| P19-12 QuestMgr Abandon | Unit | ✅ |
| P19-13 QuestMgr Retry | Unit | ✅ |
| P19-14 QuestMgr Active Quests | Unit | ✅ |
| P19-15 QuestMgr History | Unit | ✅ |
| P19-16 QuestMgr Save/Load | Unit | ✅ |
| P19-17 ObjMgr Init | Unit | ✅ |
| P19-18 ObjMgr Advance | Unit | ✅ |
| P19-19 ObjMgr Complete | Unit | ✅ |
| P19-20 ObjMgr Fail | Unit | ✅ |
| P19-21 ObjMgr Branching | Unit | ✅ |
| P19-22 ObjMgr Optional | Unit | ✅ |
| P19-23 ObjMgr Prerequisite Chain | Unit | ✅ |
| P19-24 NarrMgr Global Flags | Unit | ✅ |
| P19-25 NarrMgr Regional Flags | Unit | ✅ |
| P19-26 NarrMgr World Variables | Unit | ✅ |
| P19-27 NarrMgr NPC Variables | Unit | ✅ |
| P19-28 NarrMgr Decisions | Unit | ✅ |
| P19-29 NarrMgr Condition Eval | Unit | ✅ |
| P19-30 NarrMgr Story Chapters | Unit | ✅ |
| P19-31 NarrMgr Save/Load | Unit | ✅ |
| P19-32 JourMgr Add Quest | Unit | ✅ |
| P19-33 JourMgr Complete Quest | Unit | ✅ |
| P19-34 JourMgr Lore | Unit | ✅ |
| P19-35 JourMgr Dialogue Log | Unit | ✅ |
| P19-36 JourMgr Discoveries | Unit | ✅ |
| P19-37 JourMgr Save/Load | Unit | ✅ |
| P19-38 DlgDB Register | Unit | ✅ |
| P19-39 DlgDB Get By NPC | Unit | ✅ |
| P19-40 DlgDB Starting Dialogue | Unit | ✅ |
| P19-41 DlgDB Stress | Unit | ✅ |
| P19-42 DlgMgr Start | Unit | ✅ |
| P19-43 DlgMgr Choice | Unit | ✅ |
| P19-44 DlgMgr Conditional | Unit | ✅ |
| P19-45 DlgMgr End | Unit | ✅ |
| P19-46 DlgMgr Loop Prevention | Unit | ✅ |
| P19-47 Stress 1000 Quests | Stress | ✅ |
| P19-48 Stress 1000 Dialogues | Stress | ✅ |
| P19-49 Stress Concurrent | Stress | ✅ |
| P19-50 Stress Memory | Stress | ✅ |
| P19-51 Edge Empty DB | Edge | ✅ |
| P19-52 Edge Duplicate | Edge | ✅ |
| P19-53 Edge Invalid Accept | Edge | ✅ |
| P19-54 Edge Max Depth | Edge | ✅ |
| P19-55 Edge Serialization | Edge | ✅ |

### Coverage Gaps
- GameManager state transitions
- CameraController
- TouchControls
- PlayerMovement physics
- EventBus edge cases
- Logger failsafe behavior

---

## Known Limitations

1. UIManager handles basic screen stacks — HUD is fully integrated; custom panel bindings will expand in future phases.
2. Quest and dialogue frameworks are complete; specific quest-lines, story content, and dialogue writing will be added in Prompt 20.
3. No final visual/audio assets imported yet — README prompt specifications exist for all 10 categories.
4. Ability system is framework-only — no skill trees, class restrictions, or balance implemented yet.
5. Gathering and crafting systems are framework-only — no UI, merchant integration, or quest integration yet.

---

## Next Steps (Prompt 21+)

1. **Screen-to-Data Integration** — Connect all 20 UI screens to real game data systems (Inventory→ItemDatabase, Equipment→EquipmentManager, etc.)
2. **Responsive Screen Layouts** — Integrate screens with ResponsiveLayout for dynamic sizing instead of hardcoded 1920x1080
3. **SaveLoadScreen Integration** — Wire SaveLoadScreen to SaveManager
4. **Color Blind Shader** — Implement actual color blindness simulation shader
5. **MiniMap Implementation** — Replace placeholder with actual minimap rendering
6. **Main Storyline Implementation** — Create the main story quests using the quest framework
7. **Side Quest Content** — Populate the world with side quests, faction quests, and daily quests
8. **Dialogue Writing** — Write dialogue content for NPCs using the dialogue framework
9. **Player Housing** — Building placement, furniture crafting, and player-owned property system
10. **Kingdom Politics** — Faction relationships, territory control, and governance mechanics

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