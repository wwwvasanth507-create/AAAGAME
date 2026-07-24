# Hero of Eternia

An offline-first, production-quality 3D Action RPG built natively for Android 8+ using the **Godot 4.3 (C#)** engine.

---

## 1. Project Vision
*Hero of Eternia* is designed from zero as a performance-optimized mobile role-playing game. Players explore dungeons, interact with NPC factions, level up via custom skill trees, collect/craft loot, and combat boss creatures in real-time.

---

## 2. Technology Stack
*   **Game Engine:** Godot 4.3 (Mono/C# edition)
*   **Target Platforms:** Android 8.0+ (API 26+)
*   **CPU Architectures:** ARM64-v8a (ARM64)
*   **Rendering API:** GLES 3.0 Compatibility (Scalable from Low to Ultra presets)
*   **Code Language:** C# (conforming to SOLID, design patterns, and clean code standards)
*   **Local Storage:** Extensible JSON profiles encrypted using AES-256 with SHA-256 checksum integrity checks.

---

## 3. High-Level Folder Structure
*   `Assets/` - Shared raw and optimized assets (meshes, textures, sound audio, animations).
*   `Prefabs/` - Reusable node subtrees.
*   `Scenes/` - Main stages (boot, menus, loadings, dungeons).
*   `Scripts/` - C# code classes, managers, and data entities.
*   `Shaders/` - Customized PBR material shaders.
*   `Settings/` - JSON configuration profiles.
*   `Build/` - Signing keystores and compiled APK binaries.

---

## 4. Setup and Development Instructions

### Prerequisite Dependencies:
The project uses a portable sandboxed build pipeline:
*   .NET 8.0 SDK (stored in `.dotnet/`)
*   Godot Mono 4.3 editor executable (stored in `.godot/`)

### Compilation:
To compile the C# codebase:
```powershell
$env:DOTNET_ROOT = "c:\AAA\.dotnet"; $env:PATH = "c:\AAA\.dotnet;" + $env:PATH; dotnet build
```

### Headless Verification Tests:
To run the automated C# testing harness:
```powershell
$env:DOTNET_ROOT = "c:\AAA\.dotnet"; $env:PATH = "c:\AAA\.dotnet;" + $env:PATH; & ".godot/Godot_v4.3-stable_mono_win64/Godot_v4.3-stable_mono_win64_console.exe" --headless --run-tests
```

### Android APK Export:
To export the Android package:
```powershell
$env:DOTNET_ROOT = "c:\AAA\.dotnet"; $env:PATH = "c:\AAA\.dotnet;" + $env:PATH; & ".godot/Godot_v4.3-stable_mono_win64/Godot_v4.3-stable_mono_win64_console.exe" --headless --export-release "Android" Build/HeroOfEternia.apk
```
The APK is signed using `apksigner` with the release keystore under `Build/release.keystore`.
