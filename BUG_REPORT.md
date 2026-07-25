# Bug Report — Hero of Eternia (v0.5.0)

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

---

## 2. Fixed Issue Details

### B3 — SceneManager Async Loading Mocked
- **Issue:** `SimulateAsyncLoad()` simulated resource packages.
- **Fix:** Refactored `SceneManager` to inherit from `Godot.Node`, added dynamically to the active scene root, and integrated with `ResourceLoader.LoadThreadedRequest` and `LoadThreadedGetStatus`. Transitions are executed using `ChangeSceneToPacked`.

### B4 — AudioManager Playback Stubbed
- **Issue:** All playback methods logged messages without generating sound.
- **Fix:** Refactored `AudioManager` to manage `AudioStreamPlayer` and `AudioStreamPlayer3D` pools, sync volume levels to Godot buses, and loop/fade tracks with Tweens.

### B8 — ResourceManager Cache Mocked
- **Issue:** Preload cache instantiated generic `new object()` instances.
- **Fix:** Integrated real `ResourceLoader.Load<Resource>()` caching and added typed asset retrievals (`GetAsset<T>`).

### B7 — Duplicate AI Documentation
- **Issue:** Both `AI_PIPELINE_REPORT.md` and `AI_ASSET_PIPELINE_REPORT.md` existed.
- **Fix:** Merged content into `AI_PIPELINE_REPORT.md` and configured `AI_ASSET_PIPELINE_REPORT.md` to act as a redirect warning.