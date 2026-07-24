# Development Roadmap - Hero of Eternia

This roadmap details the progression milestones across the 150-prompt development lifecycle of *Hero of Eternia*.

---

## Milestone Stages

```mermaid
gantt
    title Hero of Eternia Development Schedule
    dateFormat  YYYY-MM-DD
    section Core Infrastructure
    Phase 1: Project Foundation (P1)           :active, p1, 2026-07-24, 2026-07-25
    Phase 2: Engine Initialization (P2-P10)     :after p1, p2, 5d
    section Core Database
    Phase 3: Offline Storage & Room (P11-P40)   :after p2, p3, 15d
    section Rendering
    Phase 4: 3D Shaders & Presets (P41-P70)     :after p3, p4, 15d
    section Mechanics
    Phase 5: Gameplay & Controls (P71-P100)     :after p4, p5, 15d
    section Content
    Phase 6: Dungeons, AI & World (P101-P130)   :after p5, p6, 15d
    section Polish
    Phase 7: UI/UX & Audio (P131-P140)          :after p6, p7, 5d
    Phase 8: Security & QA Audits (P141-P150)   :after p7, p8, 5d
```

---

## Detailed Phases

### Phase 1: Project Foundation (Current)
*   **Target:** Prompt 1.
*   **Features:** Choose Godot 4.x (C#), design folder structures, define manager responsibilities, establish standards, and structure database storage.

### Phase 2: Engine Initialization & Properties
*   **Target:** Prompts 2–10.
*   **Features:** Set up Godot directory project files, compile base empty packages, verify Android export templates, and configure target SDK platforms.

### Phase 3: Offline Database & Save Profiles
*   **Target:** Prompts 11–40.
*   **Features:** Program local SQLite data schemas. Implement profile slots, player stats, items inventory, world seed caches, and database version migrations.

### Phase 4: 3D Shaders & Presets
*   **Target:** Prompts 41–70.
*   **Features:** Code PBR shaders, normal maps, dynamic shadows, and hardware-scaled presets (Low to Ultra).

### Phase 5: Gameplay, Steering & Physics
*   **Target:** Prompts 71–100.
*   **Features:** Develop player controllers, multi-touch virtual joysticks, and object-pooled projectiles/asteroids.

### Phase 6: Dungeons, Monsters & AI
*   **Target:** Prompts 101–130.
*   **Features:** Code Finite State Machine AI (patrol, alert, chase), dungeon generation seeds, and NPC dialogue boxes.

### Phase 7: UI/UX & Audio
*   **Target:** Prompts 131–140.
*   **Features:** Build glassmorphic overlays, settings sliders, and multi-bus sound engines.

### Phase 8: Optimization, Security & QA Release
*   **Target:** Prompts 141–150.
*   **Features:** Conduct security code audits, APK size compression, GC sweeps, and export the production release APK.
