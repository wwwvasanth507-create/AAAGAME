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
    Phase 10: World Systems Audit (P9-Audit)           :done,   p9b, after p9a,  1d
    Phase 11: Combat Architecture (P10)                :done,   p10a, after p9b,  2d
    Phase 12: Gameplay Expansion (P11)                 :done,   p10b, after p10a, 2d
    Phase 13: Boss & Elite Encounters (P12)            :done,   p10c, after p10b, 2d
    section Ability & Progression
    Phase 14: Ability System (P13)                   :done,   p13, after p10c, 1d
    section Rendering
    Phase 15: 3D Shaders & Presets (P14-P30)         :active, p14, after p13,  10d
    section Mechanics
    Phase 16: Gameplay & Combat (P31-P70)           :        p15, after p14,  20d
    section Content
    Phase 17: Dungeons, AI & World (P71-P110)       :        p16, after p15,  20d
    section Polish
    Phase 18: UI/UX & Audio (P111-P130)             :        p17, after p16,  10d
    Phase 19: Security & QA Release (P131-P150)     :        p18, after p17,  10d
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

### ✅ Phase 9: NPC Architecture (Completed — Prompt 9)
*   **Features:**
    *   `NpcDefinition` — NpcData and 15 NpcTypeEnum variants.
    *   `NpcStateMachine` — 12-state FSM, conditional transition scoring.
    *   `NpcScheduler` — Time-driven periods and override stack.
    *   `Social Systems` — Scoped reputation and relationship floats.
    *   `DialogueFramework` — Localization-key conditional scoring.
    *   `NpcSpawner` — Seed-based placement rules.
    *   `Save Integration` — SaveProfile V6 serialization.
    *   `Test suite` — Expanded from 33 to 42 tests.

### ✅ Phase 10: Combat Architecture (Completed — Prompt 10)
*   **Features:**
    *   `CombatManager` — Attacks, cooldowns, projectiles, and events coordination.
    *   `WeaponDefinition` — Stats, range, speeds, visual/audio hooks for 12 weapons.
    *   `TargetingSystem` — Locking modes and LoS math queries.
    *   `HitDetection` — Headless bounding sphere/box collision checks.
    *   `DamageSystem` — Element multipliers, resistances, and critical hits.
    *   `StatusEffects` — Buff/de-buff ticks, stack limits, duration refreshes.
    *   `ProjectileSystem` — Simulation pool, Homings, and impact callbacks.
    *   `Player FSM Extension` — 8 combat states (Attack, Block, Parry, Stun, Knockdown).
    *   `Save Integration` — SaveProfile V7 serialization.
    *   `Test suite` — Expanded from 42 to 52 tests.

### ✅ Phase 11: Gameplay Expansion (Completed — Prompt 11)
*   **Features:**
    *   `Enemy Spawning` — Data-driven EnemyDefinition/EnemyDatabase registry, 5 default configurations.
    *   `Enemy AI` — Headless 8-state FSM controller node managing movement, attack triggers and deaths.
    *   `Spawner` — Wave-based spawner featuring golden-ratio scatter placement.
    *   `Player Abilities` — 4-slot executor service supporting cooldowns, resource usage and 5 default skills.
    *   `Game Loop` — Managed timer session, XP progression thresholds, pauses, and autosaves on wave clears.
    *   `Save Integration` — SaveProfile V8 version promoting level, XP, kill tracking.
    *   `Test suite` — Expanded from 52 to 62 unit/integration tests running headlessly.

### ✅ Phase 12: Boss Framework, Elite Enemies & Encounter System (Completed — Prompt 12)
*   **Features:**
    *   `Boss Database` — Data-driven configurations with BossClass templates, loot tables, and music profiles.
    *   `Elite Enemies` — Flags-based combinable modifiers (Fortified, Swift, Fireborn, etc.) scaling HP/damage/speed and resistances.
    *   `Phase System` — Multi-stage combat phases transitioning on HP thresholds or enrage timers.
    *   `Special Attacks` — Melee combos, AoE spells, summon hooks, charge runs, and hazards.
    *   `Encounter Manager` — Warmup, active, victory, defeat states coordination with boundary cylinder checks and arena resets.
    *   `Save Integration` — SaveProfile V9 persisting completions, defeat IDs, and claims with backward compatibility.
    *   `Test suite` — Expanded from 62 to 72 unit/integration tests passing headlessly.

### ✅ Phase 13: Ability System, Skill Framework & Player Progression (Completed — Prompt 13)
*   **Features:**
    *   `Ability Database` — Data-driven JSON with 10 abilities, each containing 40+ fields (Unique ID, Internal Name, Display Name, Description, Category, Ability Type, Target Type, Element, Animation/Audio/VFX References, Icon, Cooldown, Cast Time, Range, AoE, Resource Costs, Unlock Requirements, Upgrade Path Hook, Localization Key, Version, DLC Fields)
    *   `Ability Categories` — 11 extensible categories (Melee, Magic, Ranged, Movement, Support, Healing, Defensive, Summoning, Passive, Ultimate, Utility) with CategoryManager supporting runtime registration
    *   `Ability Manager` — Full execution framework with activation pipeline, cancellation, interruptions, cooldowns, charges, resource consumption, target validation, animation/VFX/SFX hooks, global cooldown, network-ready event-driven architecture
    *   `Resource Framework` — 7 configurable resource types (Health, Mana, Stamina, Energy, Focus, Rage, Spirit) with configurable pools, regen rates, no hardcoded assumptions
    *   `Player Progression` — Level 1-100 with XP curves, stat growth hooks, prestige system (10 levels), future seasonal/prestige support
    *   `Ability Loadouts` — 6 configurable loadouts with primary/secondary/passive/ultimate/quick slots, save/load persistence
    *   `Ability Effects` — 12 reusable effect types (Damage, Healing, Shield, Teleport, Buff, Debuff, Summon, ProjectileSpawn, AreaCreation, Movement, EnvironmentalInteraction, Custom) with stacking, duration, tick-based processing
    *   `Save Integration` — SaveProfile V10 with unlocked abilities, ability levels, loadout data, ability manager state, progression data, version migration
    *   `Test suite` — Expanded from 72 to 170 unit/integration tests (98 new tests across 16 test areas)
    *   `Documentation` — ABILITY_SYSTEM.md, PROJECT_MEMORY.md, ARCHITECTURE.md updated

### ✅ Phase 14: Equipment, Gear Progression & Attribute Calculation System (Completed — Prompt 14)
*   **Features:**
    *   `Attribute Calculation Engine` — Centralized deterministic engine with 10 modifier layers (Base, Equipment, Ability, Buff, Debuff, Environment, Difficulty, Guild, Mount, Pet), dirty-flag caching, event-driven recalculations
    *   `Expanded Attributes` — 40+ attribute types covering core vitals, combat stats, elemental resistances (Fire, Ice, Lightning, Poison, Holy, Shadow), status resistances (Stun, Freeze, Burn, Bleed, Silence, Knockback), special hooks (LifeSteal, ManaRegen, ExperienceBonus, GoldBonus)
    *   `Item Modifier System` — Reusable modifier definitions with 5 stacking rules (Additive, HighestOnly, LowestOnly, Multiplicative, Override), 22 default modifier presets, category-based indexing
    *   `Enchantment Framework` — 23 enchantments across 10 elements (Fire, Ice, Lightning, Poison, Holy, Shadow, Wind, Earth, Water, Custom) with level 1-10 scaling, target type filtering (Weapon/Armor/Accessory/Any)
    *   `Durability System` — Per-item durability with max/current tracking, damage sources (Attack, Block, Hit, Ability), break state with repair hooks, visual damage event hooks, save/load persistence
    *   `Gear Set System` — 4 default sets (Iron Warrior, Flame Mage, Shadow Assassin, Guardian) with tiered bonuses, piece counting, activation/deactivation events, visual override hooks
    *   `Item Quality System` — 8 quality grades (Broken, Poor, Normal, Fine, Superior, Masterwork, Legendary, Divine) with configurable stat multipliers (0.25x-5.00x), upgrade success bonuses, max upgrade level caps, VFX hooks
    *   `Upgrade Framework` — 10 upgrade levels with progressive success rates (95%→25%), failure downgrade chance, critical failure destroy chance (0%→20%), stat multiplier increases per level, cost hooks for future blacksmith integration
    *   `Save Integration` — SaveProfile V11 with EquipmentSaveData (durability, upgrades, enchantments, quality, modifiers, active sets), V10→V11 migration
    *   `Test suite` — 17 new tests covering all systems (attribute engine, modifiers, enchantments, durability, gear sets, quality, upgrades, save/load, version migration, stress testing)
    *   `Documentation` — ATTRIBUTE_ENGINE.md, EQUIPMENT_PROGRESSION.md, MODIFIER_SYSTEM.md, ENCHANTMENT_SYSTEM.md, DURABILITY_SYSTEM.md

### 🔲 Phase 15: Crafting, Gathering, Professions & Economy (Next — Prompt 15)
*   **Planned Features:** Crafting system with recipes, gathering/harvesting mechanics, profession system, merchant economy, item enhancement through crafting, resource management.

### 🔲 Phase 16: UI/UX & Audio (Prompts 71–110)
*   **Planned Features:** Glassmorphic HUD overlay screens, settings, volume sliders.

### 🔲 Phase 17: Security & QA Release (Prompts 111–150)
*   **Planned Features:** OBX profiling, Google Play Store signed release APK.