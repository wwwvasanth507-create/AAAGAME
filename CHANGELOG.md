# Changelog - Hero of Eternia

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.2.0] - 2026-07-24

### Added
*   **Godot Project Initialization**: Configured `project.godot`, `export_presets.cfg`, `HeroOfEternia.csproj`, and `HeroOfEternia.sln` solution files.
*   **Scene Structure Templates**: Built Boot, Splash, MainMenu, Loading, Settings, Credits, and TestEnvironment scenes (.tscn).
*   **Core Manager Implementations (C#)**: Fully drafted EventBus, thread-safe Logger, GameManager, SceneManager, SaveManager, SettingsManager, AudioManager, LocalizationManager, InputManager, ResourceManager, UIManager, and PerformanceManager classes.
*   **Logging System**: Implemented conditional compilation logs (`[Conditional("DEBUG")]`) that strip debug logging in release builds.
*   **Configuration Profiles**: Created JSON graphics, audio, controls, language, performance, and developer templates.
*   **Headless Automated Test Harness**: Created `TestRunner.cs` validating manager operations, state switches, EventBus subscriptions, and MD5 file integrity signatures.
*   **Signed APK Packaging**: Generated debug/release keystores, built C# code cleanly (0 warnings/errors), and manually signed/verified `Build/HeroOfEternia.apk` using `apksigner`.

### Fixed
*   **GameManager Initialize Guard**: Fixed transition blocks during Boot by adding a default `None` state.
*   **Headless Editor Settings Resolution**: Updated JDK/SDK path properties inside Roaming Godot minor version settings (`editor_settings-4.3.tres`).
*   **Assembly Mismatch**: Resolved Mono solution search errors by matching assembly names exactly to solutions.

## [0.1.0] - 2026-07-24

### Added
*   **Project Foundation & Architectural Vision**: Selected **Godot 4.x (C#)** as the core engine.
*   **Folder Structure Blueprint**: Formulated directory trees mapping Assets, Prefabs, Scripts, and documentation.
*   **Decoupled Script Design**: Documented manager script classes (GameManager, SaveManager, InputManager, etc.) coordinated by an EventBus.
*   **Local Save Strategy**: Outlined SQLite save slot patterns with version check headers to support progress migrations.
*   **Scalable Graphics Quality Layout**: Formulated hardware presets (Low, Medium, High, Ultra) adjusting draw parameters.
*   **Security Strategy**: Outlined save validation signatures and obfuscation layers.
*   **Workspace Agent Bindings**: Configured [.agents/AGENTS.md](file:///c:/AAA/.agents/AGENTS.md) to permanently enforce AI-first asset production.
