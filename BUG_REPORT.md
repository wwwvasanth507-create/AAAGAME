# Bug Report — Hero of Eternia (v0.6.0)

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

---

## 2. Fixed Issue Details (Phase 6)

### P6-E1 — Unused Variable Warning
- **Issue:** Compiler warning CS0168 declared `IOrderedEnumerable<System.Dynamic.ExpandoObject> ordered` but never used.
- **Fix:** Deleted the declaration to keep the build warning-free.

### P6-E2 — Missing Namespace Import in EquipmentManager
- **Issue:** `GD.Load<Material>` and `Material` references caused compiler errors because `using Godot;` was not imported.
- **Fix:** Added `using Godot;` to the top of `EquipmentManager.cs`.

### P6-E3 — RemoveModifier Parameter Mismatch
- **Issue:** `EquipmentManager.cs` passed the `StatModifier` instance directly to `RemoveModifier`, which expects a `string modifierId`.
- **Fix:** Refactored the call to `player.Data.Attributes.RemoveModifier(pair.Item1, pair.Item2.Id)`.