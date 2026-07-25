# Development Roadmap - Hero of Eternia

This roadmap details the progression milestones across the 150-prompt development lifecycle of *Hero of Eternia*.

---

## Milestone Stages

```mermaid
gantt
    title Hero of Eternia Development Schedule
    dateFormat  YYYY-MM-DD
    section Core Infrastructure
    Phase 1: Project Foundation (P1)                :done,   p1,  2026-07-24, 1d
    Phase 2: Project Creation & Audit (P2)          :done,   p2,  2026-07-24, 1d
    Phase 3: Core Framework & Save System (P3)      :done,   p3,  2026-07-24, 1d
    Phase 4: Player Controller & Input (P4)         :done,   p4,  after p3,   2d
    Phase 5: Player Character Framework (P5)        :done,   p5,  after p4,   2d
    Phase 6: Item Ecosystem (P6)                    :done,   p6,  after p5,   2d
    Phase 7: Procedural World & Streaming (P7)      :done,   p7,  after p6,   2d
    Phase 8: Procedural Terrain & Navigation (P8)    :done,   p8,  after p7,   2d
    section Database & World
    Phase 9: NPC Architecture (P9)                    :done,   p9a, after p8,   2d
    Phase 10: World Systems Audit (P9-Audit)           :active, p9b, after p9a,  1d
    section Rendering
    Phase 10: 3D Shaders & Presets (P12-P30)         :        p10, after p9,   10d
    section Mechanics
    Phase 11: Gameplay & Combat (P31-P70)           :        p11, after p10,  20d
    section Content
    Phase 12: Dungeons, AI & World (P71-P110)       :        p12, after p11,  20d
    section Polish
    Phase 13: UI/UX & Audio (P111-P130)             :        p13, after p12,  10d
    Phase 14: Security & QA Release (P131-P150)     :        p14, after p13,  10d
```

---

## Detailed Phases

### ✅ Phase 1: Project Foundation (Completed — Prompt 1)
*   **Features:** Engine selection (Godot 4.3 C#), folder structure blueprint, manager design, coding standards, AI-first asset pipeline rules, SOLID architecture specification.

### ✅ Phase 2: Project Creation & Audit (Completed — Prompts 2 + Audit)
*   **Features:** `project.godot`, `export_presets.cfg`, `.csproj`/`.sln` creation. Scene templates (Boot → MainMenu → Loading → Settings → Credits). Core manager stubs. Signed APK pipeline verified. Headless test harness established.

### ✅ Phase 3: Core Framework & Local Save System (Completed — Prompt 3 + Audit)
*   **Features:**
    *   `ServiceLocator` — thread-safe DI container with `IInitializable` interface (OCP-compliant).
    *   `SaveManager` — AES-256 + SHA-256 + PBKDF2 device-bound key. Backup/recovery. Schema migration.
    *   `SettingsManager` — full settings surface: audio, graphics, accessibility, controls, gameplay, dev toggle. Auto-persist.
    *   `ConfigManager` — JSON hot-reload. Templates for all 6 config categories.
    *   `DeviceDetector` — hardware query → preset mapping (LOW/MEDIUM/HIGH).
    *   `PerformanceManager` — EMA FPS tracking, dynamic resolution scale.
    *   `PerformanceMonitor` — dev-only Godot overlay node.
    *   `ErrorSystem` — global exception listener, crash log writer.
    *   `Logger` — dual Godot/Console output routing.
    *   `LocalizationManager` — IInitializable, 14-key base string table, `ChangeLanguage()` hot-swap.
    *   `TestRunner` — 5-test headless suite, EXIT_CODE=0 ✅.
*   **Audit Fixes Applied:** OCP violation in ServiceLocator, hardcoded AES password, hardcoded RAM, dead API parameters, missing config templates, Logger routing.
*   **Build:** 0 errors, 0 warnings. APK 22.7 MB signed & verified.

### ✅ Phase 4: Player Controller & Input (Completed — Prompt 4)
*   **Features:** Virtual joystick (left), action buttons (right), gesture recognition (swipe, pinch), InputManager integration, player node with CharacterBody3D, camera follow rig. FSM with 12 states, movement physics, settings, audio footstep relays.

### ✅ Phase 5: Player Character Framework (Completed — Prompt 5)
*   **Features:**
    *   `PlayerModelController` — swappable mesh slots (Hair, Armor, Weapon, Helmet, etc.), LOD shadow optimizations, material overrides.
    *   `PlayerAnimationController` — state extensions (24 states), AnimationTree blend tree & layer filters, root motion toggle.
    *   `Universal Interaction` — IInteractable interface, Area3D detection sweep, Tap/Hold/Auto modes, neon cyan shader highlights.
    *   `Stats & Attributes` — expandable modifier system (Flat, PercentAdd, PercentMult), cached dirty flags, JSON data-driven base configurations.
    *   `Effects Controller` — timing and visualization overlays (Shield, Aura, Glow, etc.).
    *   `Save Integration` — Equipped parts, base stats, and active effects written to SaveProfile V2 with backwards-compatible migration.
    *   `Test Suite` — Expanded to 14 automated tests covering all Phase 5 modules.

### ✅ Phase 6: Item Ecosystem (Completed — Prompt 6)
*   **Features:**
    *   `ItemDatabase` — data-driven config schema with extensible string categories, dynamic DLC catch-all properties.
    *   `Rarity System` — common to divine tiers with color configurations, drop weights, and VFX/SFX hooks.
    *   `InventoryContainer` — split, merge stack calculations, search filters, lock/favorites, multi-criteria sorting.
    *   `EquipmentManager` — 12 slots, applies stat modifiers directly to player attribute sets, links visual meshes.
    *   `LootTable` — roll probabilities resolving item quantities.
    *   `ItemEffects` — consumable item hook resolvers (healing, mana, buffs).
    *   `Save Integration` — SaveProfile V3 with version 2-to-3 backward-compatible migration.
    *   `Test suite` — Expanded from 14 to 21 unit/integration tests passing 100% success.

### ✅ Phase 7: Procedural World & Streaming (Completed — Prompt 7)
*   **Features:**
    *   `WorldSeed` — deterministic 64-bit seed systems parsing manual alphanumeric string seeds using FNV-1a.
    *   `WorldDatabase` — loads biome elevation parameters and element paths.
    *   `ChunkManager` — asynchronous streaming load bounds via thread pools.
    *   `ResourceSpawner` — spawn chance constraints, tilt angle filters.
    *   `WorldTimeSystem` — cycle stage checks (Sunrise, Day, Sunset, Night) and seasonal shift triggers.
    *   `WeatherManager` — profile values for blizzards, sandstorms, and ash falls.
    *   `Save Integration` — SaveProfile V4 serialization.
    *   `Test suite` — Expanded to 27 unit/integration tests running headlessly.

### ✅ Phase 8: Procedural Terrain & Navigation (Completed — Prompt 8)
*   **Features:**
    *   `Layered Noise Terrain` — 3 overlapping FastNoiseLite layers (continent simplex, ridged mountain, valley carving) representing height queries.
    *   `NavigationFoundation` — walkable cell grids slope checking and water restrictions avoiding graphics Mesh baking overhead.
    *   `VegetationSystem` — scales environment counts and spawn probabilities based on Low, Medium, and High presets.
    *   `WorldPopulationManager` — distributes ruins, shrines, and central arena landmarks on flat plateaus.
    *   `WorldValidator` — quality sweeps checking Y offsets floating objects and resource overlaps.
    *   `Save Integration` — SaveProfile V5 serialization.
    *   `Test suite` — Expanded from 27 to 33 unit/integration tests running headlessly.

### 🔲 Phase 9: Offline Database & Local Storage (Next — Prompts 9–11)
*   **Planned Features:** SQLite schema for items, quests, world states. Data access layer (DAL). Migration runner.

### 🔲 Phase 10: 3D Rendering & Shaders (Prompts 12–30)
*   **Planned Features:** PBR materials, normal/AO maps, dynamic shadows, LOD system, outline highlights.

### 🔲 Phase 11: Gameplay & Combat (Prompts 31–70)
*   **Planned Features:** Combat states, weapons hitboxes, dynamic attributes multipliers, skill trees.

### 🔲 Phase 12: Dungeons, World & AI (Prompts 71–110)
*   **Planned Features:** Seeded generation, FSM enemy AI, dialogues.

### 🔲 Phase 13: UI/UX & Audio (Prompts 111–130)
*   **Planned Features:** Glassmorphic HUD overlay screens, settings, volume sliders.

### 🔲 Phase 14: Security, Optimization & QA Release (Prompts 131–150)
*   **Planned Features:** OBX profiling, Google Play Store signed release APK.
