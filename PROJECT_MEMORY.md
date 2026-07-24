# Project Memory - Hero of Eternia (v0.4.0)

## Completed Features

### Phase 1 — Project Foundation ✅
- **Project Vision & Strategy**: Finalized offline-first 3D Action RPG design for Android 8+ (ARM64).
- **Engine Selection**: Formally selected **Godot 4.3 (C#)**.
- **Coding Standards**: Established SOLID principles, naming conventions, and modular architecture rules.
- **Workspace Agent Bindings**: Configured `.agents/AGENTS.md` to permanently enforce AI-first asset production.

### Phase 2 — Project Creation ✅
- **Project Setup**: Created `project.godot`, `export_presets.cfg`, `HeroOfEternia.csproj`, `HeroOfEternia.sln`.
- **Folder Structure**: All Asset subdirectories, Scenes, Scripts/Core, Settings, Build, Shaders, Prefabs, Tests, Editor folders.
- **Scene Structure**: Boot, Splash, MainMenu, Loading, Settings, Credits, TestEnvironment scene files.
- **Core Managers (initial)**: EventBus, Logger, GameManager, SceneManager, SaveManager (basic), SettingsManager (basic), AudioManager, LocalizationManager, InputManager, ResourceManager, UIManager, PerformanceManager.
- **Configuration Files**: JSON profiles for graphics (4 presets), audio, controls, language, performance, developer.
- **Build Pipeline**: Signed APK compiling from portable .NET 8 + Godot Mono 4.3. Keystores generated and verified with `apksigner`.

### Phase 3 — Core Framework & Local Save System ✅
- **ServiceLocator (DI Container)**: Thread-safe service registration/resolution with per-service startup timing. Uses `IInitializable` interface — fully Open/Closed compliant. No type-coupling inside the locator.
- **SaveManager (AES-256 Encrypted)**: Full save pipeline: JSON serialization → AES-256 encryption (device-unique key via PBKDF2) → SHA-256 checksum appended. Backup `.bak` files auto-created on every write. Corrupted saves auto-recovered from backup. Slot preview metadata cache. Schema version migration hooks. `JsonExtensionData` for unlimited future DLC fields.
- **SettingsManager**: Audio (master/music/sfx), graphics preset, language, touch controls (deadzone/sensitivity), accessibility (large fonts, colorblind mode), autosave, developer console toggle. All changes saved instantly to disk. Factory reset support.
- **ConfigManager**: JSON config loading with in-memory caching and hot-reload (`HotReloadAll()`). Auto-generates templates for: physics, camera, gameplay, performance, localization, debug.
- **DeviceDetector**: Queries OS, CPU, GPU, display resolution, refresh rate, free storage. RAM queried via Godot Performance API. Maps hardware to LOW/MEDIUM/HIGH preset automatically. Manual override supported.
- **PerformanceManager**: Exponential-moving-average FPS tracker. Auto-adjusts dynamic resolution scale between 0.5×–1.0× based on 80%/95% FPS thresholds.
- **PerformanceMonitor (Overlay)**: Godot Label node rendering live FPS, frame time, static memory, and draw call counts. Dev-only toggle via SettingsManager.
- **ErrorSystem**: AppDomain unhandled exception listener. Writes timestamped crash logs to `crash_log.txt`. Reports fatal errors and asset misses.
- **Logger**: Thread-safe, level-tagged (Info/Warning/Error/Critical). Info+Warning stripped in Release via `[Conditional("DEBUG")]`. Routes to Godot Output panel via `GD.Print/PushWarning/PushError` with Console fallback for headless environments.
- **LocalizationManager**: Implements IInitializable. Full English base string table (14 keys). Runtime `ChangeLanguage()` hot-swap support.
- **TestRunner**: 5-test headless suite — ServiceLocator DI boot, SettingsManager persistence+reset, ConfigManager template generation+hot-reload, DeviceDetector hardware query, SaveManager AES encrypt/decrypt/backup/corruption-recovery.

---

## Current Architecture
- **ServiceLocator**: Central DI registry. All managers implement `IInitializable` for self-declared startup.
- **EventBus**: Generic publish/subscribe with copy-on-iterate safety.
- **SaveProfile**: Unified local data model — Stats, Inventory, Quests, WorldState, Statistics, `JsonExtensionData` (future DLC).
- **AES Encryption**: Device-bound key from `AppSalt + OS.GetUniqueId()`. Saves are non-transferable between devices.
- **Dynamic Resolution**: PerformanceManager auto-scales viewport based on sustained FPS.

---

## Folder Structure
```
c:\AAA\
├── .agents/               (Workspace AI-first rules)
├── Assets/
│   ├── Animations/
│   ├── Audio/
│   ├── Bosses/
│   ├── Characters/
│   ├── Enemies/
│   ├── Environment/
│   ├── Fonts/
│   ├── Items/
│   ├── Materials/
│   └── UI/
├── Build/                 (debug.keystore, release.keystore, HeroOfEternia.apk)
├── Editor/
├── Prefabs/
├── Scenes/               (Boot, Splash, MainMenu, Loading, Settings, Credits, TestEnvironment)
├── Scripts/
│   └── Core/
│       ├── ConfigManager.cs      [Phase 3 NEW]
│       ├── DeviceDetector.cs     [Phase 3 NEW]
│       ├── ErrorSystem.cs        [Phase 3 NEW]
│       ├── PerformanceMonitor.cs [Phase 3 NEW]
│       ├── ServiceLocator.cs     [Phase 3 NEW]
│       ├── AudioManager.cs
│       ├── EventBus.cs
│       ├── GameManager.cs        [Phase 3 UPDATED — IInitializable]
│       ├── InputManager.cs
│       ├── LocalizationManager.cs [Phase 3 UPDATED — IInitializable, full string table]
│       ├── Logger.cs             [Phase 3 UPDATED — GD.Print routing]
│       ├── PerformanceManager.cs  [Phase 3 UPDATED — IInitializable]
│       ├── ResourceManager.cs
│       ├── SaveManager.cs        [Phase 3 UPDATED — AES-256, SHA-256, backup, migration]
│       ├── SceneManager.cs
│       ├── SettingsManager.cs    [Phase 3 UPDATED — full settings surface]
│       ├── TestRunner.cs         [Phase 3 UPDATED — 5-test suite]
│       └── UIManager.cs
├── Settings/             (6 JSON config files)
├── Shaders/
├── Tests/
└── Documentation/
```

---

## Build Status
- **C# Compilation**: ✅ 0 errors, 0 warnings
- **Android APK**: ✅ 22.7 MB, signed with release.keystore, verified by apksigner
- **Headless Tests**: ✅ EXIT_CODE=0 — ALL FRAMEWORK TESTS PASSED

---

## Known Issues / Limitations
- `DeviceDetector.SystemRamMb` uses a conservative estimate from static memory × 8 rather than true total RAM (Godot 4.x does not expose total physical RAM via a public C# API).
- `PerformanceMonitor` battery field shows "N/A" — Godot removed `OS.GetPowerPercentLeft()` in v4.x.
- Save files are device-bound. Intentional for security, but cloud sync will require a server-side re-encryption step in future phases.

---

### Phase 4 — Input System, Camera & Player Foundation ✅
- **InputActionMap**: 27 named actions, default keyboard/mouse/gamepad bindings, runtime rebinding, disk persistence (`input_bindings.json`).
- **InputHandler**: Unified `InputFrame` snapshot from all device types. Touch axes override on Android.
- **TouchControls**: Dynamic joystick, 6 action buttons, gestures (double-tap=roll, swipe-up=jump, long-press=interact). Left-handed mode. Tablet scaling.
- **CameraController**: Spring-arm third-person, smooth follow, pitch/yaw clamp, configurable zoom, dynamic FOV sprint boost (+8°), trauma-based shake, soft lock-on orbit, FreeCam/PhotoMode.
- **PlayerData**: Vitals (HP/MP/Stamina + regen), primary stats (STR/DEF/MAG/SPD/LCK + 4 more), movement params, stamina costs, `CustomStats` dict for future DLC.
- **PlayerStateMachine**: OCP-compliant — 12 states registered via `Register()`. `OnStateChanged` event for UI binding.
- **Player States (12)**: Idle, Walk, Run, Sprint (stamina drain), Jump, Fall, Land (camera shake), Roll, Swim/Climb (stubs), Dead, Frozen. All null-safe for headless testing.
- **PlayerMovement**: Camera-relative ground movement, smooth yaw rotation, reduced air control, `ApplyJump`, `DetectSurface` downward raycast.
- **PlayerAnimationController**: AnimationPlayer wrapper with named constants, per-state blend times, footstep event relay.
- **PlayerAudioController**: 3 spatial AudioStreamPlayer3D channels, 7-surface × 2-clip footstep routing, tempo scaling per speed, safe no-op when audio not imported.
- **PlayerSettings**: Per-player camera/movement/touch settings, auto-persist to `player_settings.json`.
- **PlayerRoot**: `CharacterBody3D` wiring all modules, physics driver, vitals regen, `Kill/Freeze/TakeDamage` public API, `EventBus` typed events.
- **Player.tscn**: Prefab — CapsuleShape, Model+AnimationPlayer, 3 module nodes, 3 raycasts, interaction point, weapon holder, FootDust particles, CameraPivot, mount/pet points.
- **PlayerEvents.cs**: Typed EventBus structs (PlayerDiedEvent, PlayerStateChangedEvent, PlayerDamagedEvent, PlayerLevelUpEvent).
- **Tests**: 9/9 headless tests pass (InputActionMap, PlayerData vitals/XP/stamina, FSM Idle→Dead, PlayerSettings persist).

---

## Next Phase
**Prompt 5** — SQLite offline database: items, quests, world state, schema migration runner.
