# Bug Report — Hero of Eternia (v0.4.0)

> Bug tracking and issue resolution status.  
> Audit Date: 2026-07-24

---

## Bug Summary

| Severity | Count |
|----------|-------|
| Critical | 0 |
| High | 0 |
| Medium | 2 |
| Low | 4 |
| Info | 2 |
| **Total** | **8** |

---

## Bug Details

### B3 — SceneManager Async Loading is Mocked
| Field | Value |
|-------|-------|
| **ID** | B3 |
| **Severity** | MEDIUM |
| **Location** | Scripts/Core/SceneManager.cs (lines 32-41) |
| **Description** | `SimulateAsyncLoad()` is a mock that reports fake progress (0.0 → 0.5 → 1.0) without actually loading any resources. Real gameplay requires Godot `ResourceLoader.LoadThreadedRequest()`. |
| **Impact** | Blocks real gameplay scene transitions. Loading screen shows fake progress. |
| **Fix** | Replace with `ResourceLoader.LoadThreadedRequest()` + `LoadThreadedGetStatus()` for async loading |
| **Status** | CONFIRMED — Fix deferred to Prompt 5 |
| **Priority** | HIGH (fix early in Prompt 5) |

### B4 — AudioManager Has No Audio Playback
| Field | Value |
|-------|-------|
| **ID** | B4 |
| **Severity** | MEDIUM |
| **Location** | Scripts/Core/AudioManager.cs (all methods) |
| **Description** | All AudioManager methods (`PlayMusic`, `PlaySfx`, `StopMusic`) are logging-only stubs. No actual `AudioStreamPlayer3D` references exist. |
| **Impact** | No sound in game. Audio feedback for gameplay (footsteps, combat, UI) is silent. |
| **Fix** | Add `AudioStreamPlayer` nodes, implement actual stream playback |
| **Status** | CONFIRMED — Fix deferred to Prompt 5 |
| **Priority** | HIGH (fix early in Prompt 5) |

### B2 — EventBus Dictionary Lacks Thread-Safety
| Field | Value |
|-------|-------|
| **ID** | B2 |
| **Severity** | LOW |
| **Location** | Scripts/Core/EventBus.cs (line 11, _eventListeners dictionary) |
| **Description** | Static Dictionary accessed from Subscribe/Unsubscribe/Publish without lock protection. Concurrent access could cause race conditions. |
| **Impact** | Potential `Collection was modified` exception if Subscribe/Unsubscribe called during Publish iteration. |
| **Fix** | ✅ **RESOLVED in v0.4.0** — Added `lock` around all dictionary access |
| **Status** | ✅ FIXED |

### B1 — TestRunner Uses null! for PlayerRoot in ForceTransition
| Field | Value |
|-------|-------|
| **ID** | B1 |
| **Severity** | LOW |
| **Location** | Scripts/Core/TestRunner.cs (line 299) |
| **Description** | `fsm.ForceTransition(null!, PlayerStateId.Dead)` passes null-forgiving operator for PlayerRoot. If any state handler dereferences the player reference, a NullReferenceException will occur. |
| **Impact** | Test fragility — works currently but could break if state handlers are enhanced to access player. |
| **Fix** | Create a minimal PlayerRoot instance for the test, or make ForceTransition nullable-safe |
| **Status** | CONFIRMED — Deferred |

### B5 — GameManager Boot Transition is Synchronous
| Field | Value |
|-------|-------|
| **ID** | B5 |
| **Severity** | LOW |
| **Location** | Scripts/Core/GameManager.cs (lines 60-65) |
| **Description** | `HandleBoot()` immediately transitions to MainMenu without verifying that all services have completed initialization. Could cause race conditions if services are still initializing. |
| **Impact** | Potential startup race condition if services take time to initialize. |
| **Fix** | Add initialization check before transitioning from Boot |
| **Status** | CONFIRMED — Deferred |

### B6 — Android Package Name Mismatch
| Field | Value |
|-------|-------|
| **ID** | B6 |
| **Severity** | INFO |
| **Location** | app/src/main/AndroidManifest.xml |
| **Description** | Android package name is `com.antigravity.voidodyssey` which doesn't match the game title "Hero of Eternia". |
| **Impact** | Cosmetic — does not affect functionality. May cause confusion in Play Store listing. |
| **Fix** | Change package to `com.heroofeternia.app` or similar |
| **Status** | CONFIRMED — Low priority |

### B7 — Redundant AI Pipeline Documentation
| Field | Value |
|-------|-------|
| **ID** | B7 |
| **Severity** | INFO |
| **Location** | Root directory |
| **Description** | Both `AI_PIPELINE_REPORT.md` and `AI_ASSET_PIPELINE_REPORT.md` exist with overlapping content. Creates confusion about which is canonical. |
| **Impact** | Documentation maintenance overhead. Risk of inconsistencies. |
| **Fix** | Merge into single canonical `AI_ASSET_PIPELINE_REPORT.md` |
| **Status** | CONFIRMED — Low priority |

### B8 — UIManager and ResourceManager Not Reviewed
| Field | Value |
|-------|-------|
| **ID** | B8 |
| **Severity** | LOW |
| **Location** | Scripts/Core/UIManager.cs, Scripts/Core/ResourceManager.cs |
| **Description** | These managers are referenced in the project structure but their implementations were not reviewed during this audit. They may be incomplete stubs. |
| **Impact** | Unknown — may need implementation when UI systems are built |
| **Fix** | Review and complete implementations as needed |
| **Status** | NOT REVIEWED |

---

## Fixed Bugs

| ID | Description | Fixed In | Fix |
|----|-------------|----------|-----|
| B2 | EventBus thread-safety | v0.4.0 | Added lock around _eventListeners dictionary |

---

## Known Limitations (from KNOWN_LIMITATIONS.md)

1. **SceneManager** uses mocked async loading — not real Godot ResourceLoader
2. **AudioManager** methods are logging stubs — no actual audio playback
3. **UIManager** and **ResourceManager** are incomplete implementations
4. **Gameplay systems** (Combat, Inventory, Quests, World) are not yet implemented (planned for Prompt 5+)
5. **No actual AI-generated assets** have been imported yet — only placeholder README files exist in asset folders

---

## Bug Resolution Statistics

| Metric | Value |
|--------|-------|
| Total Bugs Found | 8 |
| Fixed | 1 (12.5%) |
| Confirmed (Deferred) | 5 (62.5%) |
| Not Reviewed | 1 (12.5%) |
| Info | 2 (25%) |
| Critical/High | 0 |