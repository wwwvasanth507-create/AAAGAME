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
    Phase 4: Player Controller & Input (P4)         :active, p4,  after p3,   2d
    section Database & World
    Phase 5: Offline Storage & Database (P5-P10)    :        p5,  after p4,   5d
    section Rendering
    Phase 6: 3D Shaders & Presets (P11-P30)         :        p6,  after p5,   10d
    section Mechanics
    Phase 7: Gameplay & Combat (P31-P70)            :        p7,  after p6,   20d
    section Content
    Phase 8: Dungeons, AI & World (P71-P110)        :        p8,  after p7,   20d
    section Polish
    Phase 9: UI/UX & Audio (P111-P130)              :        p9,  after p8,   10d
    Phase 10: Security & QA Release (P131-P150)     :        p10, after p9,   10d
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

### 🔲 Phase 4: Player Controller & Touch Input (Next — Prompt 4)
*   **Target:** Prompt 4.
*   **Planned Features:** Virtual joystick (left), action buttons (right), gesture recognition (swipe, pinch), InputManager integration, player node with CharacterBody3D, camera follow rig.

### 🔲 Phase 5: Offline Database & Local Storage (Prompts 5–10)
*   **Planned Features:** SQLite schema for items, quests, world states. Data access layer (DAL). Migration runner.

### 🔲 Phase 6: 3D Rendering & Shaders (Prompts 11–30)
*   **Planned Features:** PBR materials, normal/AO maps, dynamic shadows, LOD system, scalable preset application from ConfigManager data.

### 🔲 Phase 7: Gameplay Systems & Combat (Prompts 31–70)
*   **Planned Features:** CharacterStats, health/mana/stamina, combat state machine, hitbox detection, damage math, skill tree foundation.

### 🔲 Phase 8: Dungeons, World & AI (Prompts 71–110)
*   **Planned Features:** Procedural dungeon seeds, FSM enemy AI (Patrol → Alert → Chase → Attack), NPC dialogue, world time/weather.

### 🔲 Phase 9: UI/UX & Audio (Prompts 111–130)
*   **Planned Features:** Glassmorphic HUD overlays, dialogue boxes, inventory UI, settings sliders, multi-bus audio engine, BGM loops.

### 🔲 Phase 10: Security, Optimization & QA Release (Prompts 131–150)
*   **Planned Features:** Full security audit, APK compression, GC profiling, obfuscation, Google Play store packaging, production APK.
