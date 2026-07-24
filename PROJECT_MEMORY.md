# Project Memory - Hero of Eternia (v0.2.0)

## Completed Features
- **Project Vision & Strategy**: Finalized offline-first 3D Action RPG design for Android 8+ (ARM64) devices.
- **Engine Selection**: Formally selected and configured **Godot 4.3 (C#)**.
- **Project Setup (Phase 2)**: Created `project.godot`, `export_presets.cfg`, `HeroOfEternia.csproj`, and `HeroOfEternia.sln` matching assembly names.
- **Folder Structure**: Created all Assets subdirectories, Scenes, Scripts/Core, Settings, Build, Shaders, Prefabs, Tests, and Editor folders.
- **Scene Structure**: Built Boots, Splash, MainMenu, Loading, Settings, Credits, and TestEnvironment scene files (.tscn).
- **Core Framework Managers (C#)**: Fully drafted EventBus, Logger, GameManager, SceneManager, SaveManager, SettingsManager, AudioManager, LocalizationManager, InputManager, ResourceManager, UIManager, and PerformanceManager classes.
- **Logging Subsystem**: Thread-safe Logger supporting Info, Warning, Error, and Critical tags, using conditional compilation (`[Conditional("DEBUG")]`) to strip logs in production builds.
- **Configuration Files**: Created JSON configuration profiles for graphics, audio, controls, language, performance, and developer tools.
- **Successful Compilation & Android Build**:
  - Portable .NET 8.0 SDK and Godot Mono 4.3 tools configured headlessly in the workspace.
  - Successfully compiled the C# solution (0 warnings, 0 errors).
  - Exported the release Android APK to `Build/HeroOfEternia.apk` (22.7MB base size).
  - Generated `debug.keystore` and `release.keystore` using keytool.
  - Signed and verified the exported APK using the Android SDK `apksigner`.

---

## Current Architecture
- **Central EventBus**: Decouples game controllers from UI overlays using action delegate publish/subscribe loops.
- **System Managers**: Fully decoupled modular classes instantiated and resolved via the Service Locator pattern.
- **Local Storage Model**: Binary-serialized save slot files (`slot_*.sav`) employing MD5 integrity signatures and database version migration schemas.
- **Dynamic Resolution Scale**: Managed by the PerformanceManager, which tracks frame times and adjusts rendering viewport scaling automatically.

---

## Folder Structure
```
c:\AAA\
├── .agents/ (Workspace custom rules)
├── Assets/
│   ├── Animations/
│   ├── Audio/ (Music, SFX)
│   ├── Characters/
│   ├── Enemies/
│   ├── Bosses/
│   ├── Environment/
│   ├── Items/
│   ├── Materials/
│   ├── UI/
│   └── Fonts/
├── Prefabs/ (Saved scene node layouts)
├── Scenes/ (Main level scenes and boot templates)
├── Scripts/ (C# logic files)
│   ├── Core/ (Managers, EventBus, Logger)
│   ├── Entities/
│   ├── UI/
│   └── Database/
├── Shaders/ (Custom materials)
├── Settings/ (JSON configuration profiles)
├── Documentation/
├── Tests/ (Automated GUT unit tests)
├── Editor/ (Editor inspectors)
└── Build/ (Keystores and target APK output packages)
```

---

## Assets Created
- Phase 2 focuses on project creation, folders, manager architectures, and configuration settings files. No visual sprites, textures, or 3D models were created in this phase.

---

## Scripts Created
- `Scripts/Core/Logger.cs`
- `Scripts/Core/EventBus.cs`
- `Scripts/Core/GameManager.cs`
- `Scripts/Core/SceneManager.cs`
- `Scripts/Core/SaveManager.cs`
- `Scripts/Core/SettingsManager.cs`
- `Scripts/Core/AudioManager.cs`
- `Scripts/Core/LocalizationManager.cs`
- `Scripts/Core/InputManager.cs`
- `Scripts/Core/ResourceManager.cs`
- `Scripts/Core/UIManager.cs`
- `Scripts/Core/PerformanceManager.cs`

---

## Known Bugs
- None. C# codebase compiles successfully with 0 warnings/errors, and the Android APK is successfully exported, signed, and verified.

---

## Technical Debt
- **Archiving Old Native Code**: The native Kotlin Android files from Phase 1 remain in the root directory under `/app`. These files will be archived or deleted in the subsequent phase.

---

## Future Improvements
- Implement automated GLES3 profile overrides based on Android GPU checks.
- Add support for game-controller maps.
- Implement the GUT (Godot Unit Testing) C# wrappers.

---

## Coding Conventions
- Namespace naming follows PascalCase (e.g., `HeroOfEternia.Core`).
- File classes match their names.
- Public APIs use standard C# docstrings.
