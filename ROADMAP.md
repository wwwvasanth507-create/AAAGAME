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

### ✅ Phase 15: Gathering & Crafting (Completed — Prompt 15)
*   **Features:** Resource Database (27 resources), Profession System (14 professions), Gathering Manager, Recipe Database (20 recipes), Crafting Manager (instant/queue/batch), Workstation Framework (16 workstations), Resource Regeneration, Save V12 integration.

### ✅ Phase 16: Settlement Simulation (Completed — Prompt 16)
*   **Features:** Settlement Database (6 settlements, 15 types), Building Database (25+ buildings), NPC Schedule Expansion (6 profession schedules, weather/festival/emergency overrides), World Event Framework (8 event templates), Settlement Manager (orchestrator), Public Services (20 service types), Save V14 integration, 40 tests.

### ✅ Phase 19: Quest & Dialogue Framework (Completed — Prompt 19)
*   **Features:**
    *   **QuestDatabase** — Data-driven quest registry with 18 categories, O(1) lookups, indexed by category/giver/faction. Thread-safe. JSON loadable. Supports thousands of quests.
    *   **QuestDefinition** — Complete data model with 40+ fields including QuestId, InternalName, DisplayName, Description, Category, RecommendedLevel, QuestGiver, RequiredFaction, RequiredReputation, Prerequisites, Branches, Objectives, Rewards, FailureConditions, TimeLimits, Repeatability, LocalizationKeys, DLC fields, co-op support.
    *   **ObjectiveManager** — 16 objective types (TalkToNpc, ReachLocation, DefeatEnemy, DefeatBoss, CollectItem, CraftItem, GatherResource, DeliverItem, Interact, EscortNpc, Survive, UseAbility, VisitSettlement, ExploreArea, TriggerEvent, Custom). Unlimited objective chains. Prerequisite-based activation. Branching on complete/fail. Optional objectives.
    *   **QuestManager** — Full lifecycle management (Accept, Complete, Fail, Abandon, Retry). Prerequisite evaluation. Repeatability/schedule checks. Time limit tracking. Reward distribution. Quest history. Full save/load.
    *   **QuestBranch** — Branching quest paths with conditions, objectives, and transition hooks.
    *   **DialogueDatabase** — Data-driven conversation registry. O(1) lookups. NPC-indexed. JSON loadable. Thread-safe.
    *   **DialogueManager** — Branching dialogue execution engine. Conversation flow. Choice selection with condition filtering. Quest hooks. Flag setting. Decision recording. Merchant/service hooks. Cinematic hooks. Loop prevention. Full save/load.
    *   **NarrativeManager** — Central narrative state tracker. Global/regional flags. World/NPC/dialogue variables. Player decisions. Story chapters. Condition evaluation engine. Full save/load.
    *   **JournalManager** — Quest journal (active/completed/failed). Lore entries. Dialogue log. Discovery log. Future bestiary/codex hooks. Full save/load.
    *   **Save Integration** — SaveProfile V15 with QuestData, JournalData, NarrativeData, DialogueData.
    *   **Data Files** — Settings/quest_database.json (3 example quests). Settings/dialogue_database.json (1 example conversation).
    *   **Tests** — 55 tests covering all systems + stress + edge cases.
    *   **Documentation** — QUEST_SYSTEM.md, DIALOGUE_SYSTEM.md, OBJECTIVE_SYSTEM.md, JOURNAL_SYSTEM.md, NARRATIVE_SYSTEM.md.

### ✅ Phase 17: Social Simulation Framework (Completed — Prompt 18)
*   **Features:** Faction Database (9 factions, 16 types), Reputation Manager (5 scopes, 8 tiers), Reputation Modifier Registry (25 modifiers), Crime Manager (7 crime types, 5 severities, witness detection, bounties), Guard AI System (12 states, 4 alert levels, reinforcement calling), Diplomacy Framework (7 relations, allies/enemies queries), NPC Reaction System (10-factor evaluation), SocialManager orchestrator with full save/load, 98 tests.

### ✅ Phase 20: UI/UX Framework (Completed — Prompt 20)
*   **Features:**
    *   **UIManager** — Complete rewrite with screen lifecycle, navigation stack (max depth 20), modal dialog system, 10-layer management, focus management, Tween-based transition animations, input routing, UI state persistence (UIPreferences), and plugin system (IUIPlugin). Registered in ServiceLocator as IInitializable.
    *   **Screen Framework** — 20 reusable screen types via ScreenRegistry — MainMenu, PauseMenu, Settings (tabs: Audio/Graphics/Controls/Accessibility), Inventory (grid + detail panel), Equipment (10 slots), Character (stats display), Abilities (ability list), QuestJournal (list + detail), Map (full-screen placeholder), Crafting (recipe list), Trading (merchant + player inventories), Dialogue (speaker + text + choices), Notifications (history), Loading (progress + tips), GameOver (retry/load/quit), SaveLoad (10 slots), Bestiary, Codex, Achievements, and DLCPlaceholder. All support lazy loading via OnLazyLoad().
    *   **HUD System** — Modular HUDController with 14 independently enabled/disabled widgets — Health, Mana, Stamina, Experience (progress bars + labels), Compass (N/NE/E/etc. direction), MiniMap (hook), QuestTracker (add/remove/clear), AbilityBar (6 slots), InteractionPrompt (show/hide), BuffDebuff (add/clear), StatusEffect (add/clear), TargetInfo (name/level/HP), BossHealth (show/update/hide with percentage), FPSDebug (FPS + memory, 0.5s update, dev-only). All widgets implement IAccessibleWidget for text scale and high contrast. EventBus-driven updates.
    *   **Notification System** — NotificationManager with priority queue (Low/Normal/High/Critical), max 5 visible, max 50 queued, color-coded priority styling, fade-out animation, history tracking, convenience methods (QuestUpdated, LevelUp, ItemAcquired, AchievementUnlocked, CraftComplete, SystemMessage, Warning, Error), handler system (INotificationHandler), and full persistence.
    *   **Input Integration** — UIInputHandler with touch, mouse, keyboard, and gamepad (future) support. Gesture hooks for long press (0.5s), double tap (0.3s interval), drag & drop (10px threshold), pinch, and swipe. Input rebinding framework with 8 default actions (accept/cancel/pause/inventory/character/journal/map/abilities). IGestureHandler interface for extensible gesture processing.
    *   **Responsive Layout** — ResponsiveLayout with 4 device categories (Phone ≤480dp, SmallTablet ≤768dp, LargeTablet ≤1024dp, Desktop >1024dp), DPI-aware scaling (160 base DPI), safe areas (status bar + nav bar), orientation detection with events, foldable device hooks, and per-category layout presets (grid columns, sidebar, bottom nav, padding, font scale).
    *   **Accessibility** — AccessibilityManager with adjustable text scale (0.5x–2.0x), high contrast mode, color-blind friendly hooks (Protanopia/Deuteranopia/Tritanopia shader hooks), subtitle framework (show/hide, auto-fade, adjustable size), reduced motion mode (70% shorter tweens), screen reader labels (Accessible + TooltipText), haptic feedback toggle (Light/Medium/Heavy), future voice navigation hooks, and full settings persistence via SettingsManager.
    *   **Tests** — UISystemTests with 100+ test cases covering UIManager, all 20 screen types, all 14 HUD widgets, notification system, responsive layout, accessibility, input, save/load persistence, and stress tests.
    *   **Documentation** — UI_SYSTEM.md, HUD_SYSTEM.md, NOTIFICATION_SYSTEM.md, ACCESSIBILITY.md, RESPONSIVE_LAYOUT.md created.

### 🔲 Phase 21: Story Content & Data Integration (Prompts 21–30)
*   **Planned Features:** Screen-to-data integration (connect UI to game systems), responsive screen layouts, SaveLoadScreen integration, color blind shader, minimap implementation, main storyline quests, side quest content, dialogue writing, player housing, kingdom politics.

### 🔲 Phase 22: UI/UX & Audio (Prompts 31–70)
*   **Planned Features:** Glassmorphic HUD overlay screens, settings, volume sliders.

### 🔲 Phase 23: Security & QA Release (Prompts 71–150)
*   **Planned Features:** OBX profiling, Google Play Store signed release APK.
