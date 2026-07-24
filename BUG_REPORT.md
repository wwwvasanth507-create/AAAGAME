# Bug Report — Hero of Eternia (v0.2.0)

This report logs the debugging history and resolutions during the validation of the foundational architecture.

---

## 1. Solved Issues

### Bug 1: GameManager State Guard Mismatch on Initialize
*   **Symptom:** `GameManager.Initialize()` failed to transition out of `GameState.Boot` state during testing, returning "FAIL: GameManager start state. Got Boot".
*   **Cause:** `CurrentState` was initialized to `GameState.Boot`. When transitioning to `GameState.Boot` during `Initialize()`, the guard `if (CurrentState == newState) return;` evaluated to true and blocked the boot transition loop.
*   **Resolution:** Added a `None` state to the `GameState` enum and set the initial state to `GameState.None`.

### Bug 2: Editor Settings Ignore on Minor Version Changes
*   **Symptom:** Headless export failed reporting "A valid Java SDK path is required in Editor Settings".
*   **Cause:** Godot `4.3` ignores `editor_settings-4.tres` and instead reads settings from `editor_settings-4.3.tres`.
*   **Resolution:** Modified the script to write path variables to the minor version settings file `editor_settings-4.3.tres`.

### Bug 3: Assembly Space Mismatches
*   **Symptom:** Mono assemblies printed "res://...cs is a C# file but no solution file exists" warning traces.
*   **Cause:** The target project assembly was named "Hero of Eternia" (with spaces) in `project.godot` while the solution file was `HeroOfEternia.sln` (without spaces).
*   **Resolution:** Changed the assembly name to `HeroOfEternia` in `project.godot`.

---

## 2. Remaining Notes
*   None. There are currently zero open compiler, build, run-time, or packaging issues.
