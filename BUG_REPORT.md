# Bug Report — Hero of Eternia

> **Audit Date:** 2026-07-25  
> **Scope:** Bug hunt across all systems from Prompts 0–20

---

## Bug Summary

| Severity | Count | Status |
|:--------:|:-----:|:------:|
| Critical | 0 | ✅ None found |
| High | 0 | ✅ None found |
| Medium | 3 | ⚠️ Documented |
| Low | 5 | ℹ️ Documented |
| **Total** | **8** | |

---

## Medium Issues

### M1: UIManager Window Reference in Headless Mode
- **File:** `Scripts/UI/UIManager.cs` (line 88)
- **Issue:** `Engine.GetMainLoop() as Window` may return null in headless test mode
- **Impact:** UIManager initialization could fail in test environments
- **Fix:** Add null check and fallback to new Window()

### M2: Screens Hardcoded to 1920x1080
- **File:** `Scripts/UI/Screens/ScreenRegistry.cs` (all screens)
- **Issue:** All 20 screen types use hardcoded 1920x1080 sizes
- **Impact:** Not responsive on mobile devices despite ResponsiveLayout existing
- **Fix:** Integrate screens with ResponsiveLayout for dynamic sizing

### M3: Color Blind Filter Not Implemented
- **File:** `Scripts/UI/Accessibility/AccessibilityManager.cs` (lines 229-245)
- **Issue:** Color blind mode switch cases are empty (no shader applied)
- **Impact:** Color blind accessibility feature is non-functional
- **Fix:** Implement custom shader for color blindness simulation

---

## Low Issues

### L1: UIInputHandler Vibration Commented Out
- **File:** `Scripts/UI/Input/UIInputHandler.cs` (line 381)
- **Issue:** `Input.vibrate_handheld(durationMs)` is commented out
- **Impact:** Haptic feedback not functional on mobile
- **Fix:** Uncomment and verify API compatibility

### L2: SaveLoadScreen TODO
- **File:** `Scripts/UI/Screens/ScreenRegistry.cs` (line 662)
- **Issue:** `// TODO: Integrate with SaveManager` comment
- **Impact:** Save/Load screen not connected to actual save system
- **Fix:** Wire SaveLoadScreen to SaveManager

### L3: Screens Use Placeholder Data
- **File:** `Scripts/UI/Screens/ScreenRegistry.cs` (all gameplay screens)
- **Issue:** Inventory, Equipment, Character, etc. use hardcoded placeholder data
- **Impact:** Not connected to real game data systems
- **Fix:** Connect screens to ItemDatabase, EquipmentManager, PlayerData, etc.

### L4: MiniMapWidget Is Placeholder
- **File:** `Scripts/UI/HUD/HUDController.cs` (lines 648-673)
- **Issue:** MiniMapWidget shows "MINI-MAP" text placeholder
- **Impact:** No functional minimap
- **Fix:** Implement actual minimap rendering

### L5: FPSDebugWidget Not Fully Implemented
- **File:** `Scripts/UI/HUD/HUDController.cs`
- **Issue:** FPSDebugWidget exists but not fully wired in HUDController
- **Impact:** FPS debug display may not work
- **Fix:** Complete FPSDebugWidget implementation

---

## Previously Fixed Issues (from earlier audits)

| Bug | Prompt | Status |
|-----|:------:|:------:|
| EventBus race condition | P4 | ✅ Fixed |
| ServiceLocator OCP violation | P3 | ✅ Fixed |
| SaveManager hardcoded password | P3 | ✅ Fixed |
| DeviceDetector hardcoded RAM | P3 | ✅ Fixed |
| SettingsManager dead API params | P3 | ✅ Fixed |
| ConfigManager missing templates | P3 | ✅ Fixed |
| OS.GetPowerPercentLeft() removed API | P3 | ✅ Fixed |
| SaveManager.SavesCount path | P3 | ✅ Fixed |
| PATH issue (critical) | P10 | ✅ Fixed |
| Duplicate AI pipeline docs | P5 | ✅ Fixed |

---

## Bug Trend

```
Prompt 3:  7 bugs found, 7 fixed
Prompt 4:  1 bug found, 1 fixed
Prompt 5:  1 bug found, 1 fixed
Prompt 10: 1 bug found, 1 fixed
Prompt 20: 8 issues found, 0 fixed (documented)
```

---

## Final Bug Score: 8/10

No critical or high-severity bugs found. 3 medium issues and 5 low issues documented for future resolution.

---

*End of Bug Report*