# Changelog - Hero of Eternia

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.4.0] - 2026-07-24

### Added
*   **Foundation Audit & Validation**: Complete technical audit covering Prompts 0–4. See FOUNDATION_AUDIT_REPORT.md for full details.
*   **PERFORMANCE_REPORT.md**: Dedicated performance analysis document covering Android optimization, memory, battery, and scalability.
*   **EventBus Thread-Safety**: Added `lock` around `_eventListeners` dictionary to prevent race conditions under concurrent access.

### Changed
*   **FOUNDATION_AUDIT_REPORT.md**: Updated with comprehensive audit covering all 12 tasks (global rules, project structure, architecture, code quality, AI pipeline, data systems, offline/online, Android performance, testing, bug hunt, documentation).
*   **PROMPT_0_4_VALIDATION.md**: Updated with detailed scoring and final decision for Prompt 5 readiness.

### Fixed
*   **EventBus Race Condition**: Dictionary access in Subscribe/Unsubscribe/Publish now protected by a lock, preventing potential concurrent modification exceptions.
*   **Documentation Gap**: Added missing PERFORMANCE_REPORT.md for Android performance analysis.

### Security
*   EventBus thread-safety ensures no data corruption under concurrent event publish/subscribe scenarios.

---

## [0.3.0] - 2026-07-24

### Added
*   **ServiceLocator** (`Scripts/Core/ServiceLocator.cs`): Thread-safe DI container with lazy initialization and per-service startup time logging. Implements `IInitializable` interface pattern — fully Open/Closed compliant.
*   **IInitializable Interface**: Allows any manager to self-declare startup logic without the ServiceLocator needing concrete type knowledge.
*   **SaveManager — AES-256 + SHA-256** (`Scripts/Core/SaveManager.cs`): Complete save pipeline — JSON → AES-256 (PBKDF2 device-unique key) → SHA-256 checksum appended. `.bak` backup on every write, automatic corruption recovery, slot preview metadata, and schema version migration hooks.
*   **SettingsManager — Full Surface** (`Scripts/Core/SettingsManager.cs`): Audio levels (master/music/sfx), graphics presets, language, touch deadzone/sensitivity, accessibility flags (large fonts, colorblind mode), autosave trigger, developer console toggle. All options auto-persisted to `user_settings.json`.
*   **ConfigManager** (`Scripts/Core/ConfigManager.cs`): JSON config loading with in-memory caching and `HotReloadAll()`. Templates auto-generated for: physics, camera, gameplay, performance, localization, debug.
*   **DeviceDetector** (`Scripts/Core/DeviceDetector.cs`): Queries OS/CPU/GPU/resolution/refresh rate/storage. RAM estimated via Godot Performance API. Maps hardware heuristics to LOW/MEDIUM/HIGH quality preset.
*   **PerformanceManager — Dynamic Resolution** (`Scripts/Core/PerformanceManager.cs`): EMA-filtered FPS tracker auto-adjusting resolution scale between 0.5×–1.0×.
*   **PerformanceMonitor Overlay** (`Scripts/Core/PerformanceMonitor.cs`): Godot Label node showing live FPS, frame time, static memory, draw calls. Dev-only via SettingsManager flag.
*   **ErrorSystem** (`Scripts/Core/ErrorSystem.cs`): AppDomain unhandled exception listener. Timestamped crash log writer to `crash_log.txt`. Asset-miss reporting.
*   **TestRunner — Phase 3 Suite** (`Scripts/Core/TestRunner.cs`): 5 headless tests — ServiceLocator boot, SettingsManager persistence+reset, ConfigManager hot-reload, DeviceDetector query, SaveManager AES+backup+recovery.

### Changed
*   **Logger** (`Scripts/Core/Logger.cs`): Now routes to Godot Output panel (`GD.Print`, `GD.PushWarning`, `GD.PushError`) with Console fallback for headless environments. Added `using Godot;`.
*   **LocalizationManager** (`Scripts/Core/LocalizationManager.cs`): Implements `IInitializable`. Extended base English string table to 14 keys. Added `ChangeLanguage()` hot-swap.
*   **GameManager** (`Scripts/Core/GameManager.cs`): Implements `IInitializable`.
*   **PerformanceManager** (`Scripts/Core/PerformanceManager.cs`): Implements `IInitializable` via no-arg `Initialize()` wrapper.

### Fixed
*   **ServiceLocator OCP Violation**: Removed brittle `if (service is GameManager)` type-switch; replaced with `IInitializable` interface dispatch.
*   **SaveManager Hardcoded Password**: Replaced plaintext `DefaultPassword` const with a PBKDF2-derived device-unique key (`AppSalt + OS.GetUniqueId()`).
*   **DeviceDetector Hardcoded RAM**: Replaced hardcoded `SystemRamMb = 4096` with a runtime query via Godot Performance API with graceful fallback.
*   **SettingsManager Dead API**: Removed 4 unused parameters from `LoadSettings()` that were silently ignored.
*   **ConfigManager Missing Templates**: Added gameplay, performance, and localization config templates (previously fell through to empty `{}`).
*   **OS.GetPowerPercentLeft()**: Removed removed Godot 4.x battery API call from PerformanceMonitor (fixed compile error).
*   **SaveManager.SavesCount**: Fixed incorrect property path (`SavesCount` → `StatsData.SavesCount`).

### Security
*   Save encryption key is now device-bound via `OS.GetUniqueId()`. Save files cannot be trivially transferred between devices.
*   AES-256 + SHA-256 checksum prevents both tampering and undetected corruption.

---

## [0.2.0] - 2026-07-24

### Added
*   **Godot Project Initialization**: Configured `project.godot`, `export_presets.cfg`, `HeroOfEternia.csproj`, and `HeroOfEternia.sln`.
*   **Scene Structure Templates**: Boot, Splash, MainMenu, Loading, Settings, Credits, TestEnvironment scenes.
*   **Core Manager Implementations (C#)**: EventBus, thread-safe Logger, GameManager, SceneManager, SaveManager, SettingsManager, AudioManager, LocalizationManager, InputManager, ResourceManager, UIManager, PerformanceManager.
*   **Configuration Profiles**: JSON graphics, audio, controls, language, performance, developer templates.
*   **Headless Automated Test Harness**: TestRunner.cs validating manager operations, state switches, EventBus subscriptions, and file integrity.
*   **Signed APK Packaging**: Generated debug/release keystores, built C# (0 warnings/errors), signed and verified `Build/HeroOfEternia.apk`.

### Fixed
*   **GameManager Initialize Guard**: Fixed transition blocks during Boot by adding a default `None` state.
*   **Headless Editor Settings Resolution**: Updated JDK/SDK paths inside Godot's `editor_settings-4.3.tres`.
*   **Assembly Mismatch**: Resolved Mono solution search errors by matching assembly names exactly.

---

## [0.1.0] - 2026-07-24

### Added
*   **Project Foundation & Vision**: Selected Godot 4.x (C#) as core engine.
*   **Folder Structure Blueprint**: Formulated directory trees for Assets, Prefabs, Scripts, Documentation.
*   **Decoupled Script Design**: Documented manager classes coordinated by an EventBus.
*   **Local Save Strategy**: Outlined save slot patterns with version check headers for migration support.
*   **Scalable Graphics Quality Layout**: Formulated hardware presets (Low, Medium, High, Ultra).
*   **Security Strategy**: Outlined save validation signatures and obfuscation layers.
*   **Workspace Agent Bindings**: Configured `.agents/AGENTS.md` to permanently enforce AI-first asset production.
