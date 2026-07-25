# Bug Report — Hero of Eternia (v0.8.0)

This report tracks and documents issue resolution status across all systems.

---

## 1. Bug Resolution Matrix

| Bug ID | Severity | Location | Description | Status |
|---|---|---|---|---|
| **B1** | Low | `TestRunner.cs` | Passes null for PlayerRoot in transitions | ✅ FIXED (Used safe mocks) |
| **B2** | Low | `EventBus.cs` | Dictionary operations lacked thread-safety | ✅ FIXED (Added locks) |
| **B3** | Medium | `SceneManager.cs` | Async loading was mocked / simulated | ✅ FIXED (Real ResourceLoader) |
| **B4** | Medium | `AudioManager.cs` | Audio playback was a logging stub | ✅ FIXED (Real AudioServer & pools) |
| **B5** | Low | `GameManager.cs` | Boot transition was synchronous | ✅ RESOLVED (Init verification) |
| **B6** | Info | Android Manifest | Package name mismatch (`voidodyssey`) | ✅ RESOLVED (Manifest update) |
| **B7** | Info | Root Docs | Redundant AI Pipeline documentation | ✅ RESOLVED (Merged manifests) |
| **B8** | Low | `ResourceManager.cs`| ResourceManager cache was mocked | ✅ FIXED (Real resource loads) |
| **P6-E1** | Low | `InventoryContainer.cs`| Unused local variable `ordered` warning | ✅ FIXED (Removed variable) |
| **P6-E2** | Low | `EquipmentManager.cs` | Missing `using Godot;` import for Material loader | ✅ FIXED (Added import) |
| **P6-E3** | Medium| `EquipmentManager.cs` | RemoveModifier expecting string ID, got StatModifier | ✅ FIXED (Passed `modifier.Id`) |
| **P7-E1** | Low | `TestRunner.cs` | Missing `using HeroOfEternia.World;` imports block | ✅ FIXED (Added import statement) |
| **P8-E1** | Medium| `TerrainGenerator.cs`| Capitalization and enum qualifying compiler errors | ✅ FIXED (Corrected FastNoiseLite API) |

---

## 2. Fixed Issue Details (Phase 7 & 8)

### P7-E1 — Missing World Namespace in TestRunner
- **Issue:** TestRunner compiler errors: `WorldSeed` and `ChunkManager` could not be resolved because the namespace import was missing.
- **Fix:** Added `using HeroOfEternia.World;` and restored Standard libraries.

### P8-E1 — FastNoiseLite API Mismatches
- **Issue:** `TerrainGenerator.cs` utilized `GetNoise2d` (lowercase d) and unqualified `FastNoiseLite.NoiseTypeEnum.OpenSimplex2` enums, causing C# build errors in Godot 4.3.
- **Fix:** Refactored calls to `GetNoise2D`, qualified `NoiseTypeEnum.Simplex`, and corrected `FractalTypeEnum.Ridged`.