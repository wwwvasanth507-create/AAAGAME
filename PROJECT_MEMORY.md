# Project Memory - Hero of Eternia (v0.1.0)

## Completed Features
- **Project Vision & Strategy**: Finalized offline-first 3D Action RPG design for Android 8+ (ARM64) devices.
- **Engine Decision**: Formally selected **Godot 4.x (C#)** over Unity. Justified choice based on APK size (~25MB), fast command-line compiler loops, and zero licensing constraints.
- **Architecture Design**: Laid out decoupled C# manager classes coordinated by an event bus, dependency injection patterns, and finite state machines.
- **Rules Configuration**: Enabled permanent workspace instructions in [.agents/AGENTS.md](file:///c:/AAA/.agents/AGENTS.md) to enforce AI-first asset production.

---

## Current Architecture
- **Central EventBus**: Decouples game controllers from UI overlays.
- **System Managers**: GameManager, SaveManager, InputManager, AudioManager, SettingsManager, UIManager, CombatManager, inventory, quest, and resource managers.
- **Local Storage Model**: Binary-serialized structures or SQLite db slots with version header checks.

---

## Folder Structure
We will adopt the following structure starting in Phase 2:
```
c:\AAA\
├── .agents/ (Workspace custom rules)
├── Assets/
│   ├── Animations/
│   ├── Audio/ (Music, SFX)
│   ├── Models/ (Characters, Environment, Items)
│   ├── Textures/ (PBR maps, UI icons)
│   └── Fonts/
├── Prefabs/ (Saved scene node layouts)
├── Scenes/ (Main level scenes)
├── Scripts/ (C# logic files)
│   ├── Core/ (Managers, EventBus, DI)
│   ├── Entities/ (Player, Enemy, NPC)
│   ├── UI/ (HUD, Menu controls)
│   └── Database/ (SQLite, Save slots)
├── Shaders/ (Custom materials)
├── Settings/ (Godot config presets)
└── Documentation/
```

---

## Assets Created
- Phase 1 focuses on design and documentation; no graphics meshes or textures are generated in this phase.

---

## Scripts Created
- Phase 1 compiles structural schemas and documents manager operations; active scripting begins in Phase 2.

---

## Known Bugs
- None. Project compiles and unit test verification suite passes.

---

## Technical Debt
- **Native Android Migration**: The project currently has native Android files in `app/`. In Prompt 2, as we initialize the Godot project, these native Kotlin files will be cleaned up, archived, or refactored into the Godot structure.

---

## Future Improvements
- Implement automated GLES3 profile overrides based on Android GPU checks.
- Add support for game-controller maps.

---

## Coding Conventions
- Namespace naming follows PascalCase (e.g., `HeroOfEternia.Core`).
- File classes match their names.
- Public APIs use standard C# docstrings.
