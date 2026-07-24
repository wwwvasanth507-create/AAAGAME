# Test Plan - Hero of Eternia

This test plan outlines the verification strategy for *Hero of Eternia* to guarantee runtime stability, performance, and hardware compatibility.

---

## 1. Automated Testing Strategy
We utilize a natively integrated C# test harness (`TestRunner.cs`) executed headlessly inside Godot:
*   **Unit Tests:**
    *   *Save System:* Verify slot reads/writes, AES-256 decryption, backup recovery, and MD5/SHA-256 checksum validations.
    *   *Input Clamping:* Test virtual joystick coordinate limits.
    *   *Localization:* Verify dictionary lookups.
    *   *Config Systems:* Test JSON configurations loading.
*   **Integration Tests:**
    *   *EventBus Binds:* Test communication loops and generic publish-subscribe payloads.
    *   *Game State Transitions:* Verify flows (None -> Boot -> Menu -> Playing) without initialization blocks.
    *   *Settings Persistence:* Adjust options, trigger autosaves, reload manager, and verify persistence.
    *   *Device Checks:* Test CPU/GPU specifications query and default presets assignments.

---

## 2. Headless Test Execution Command
To run all tests programmatically:
```powershell
$env:DOTNET_ROOT = "c:\AAA\.dotnet"; $env:PATH = "c:\AAA\.dotnet;" + $env:PATH; & ".godot/Godot_v4.3-stable_mono_win64/Godot_v4.3-stable_mono_win64_console.exe" --headless --run-tests
```

---

## 3. Stress and Performance Benchmarks
*   **Dynamic Viewport Scaling:** Accumulate process loops with delta delays to verify that the PerformanceManager drops rendering scaling dynamically when FPS is low.
*   **Corruption Recovery:** Manually write corrupt byte streams into slots and verify that recovery from `.bak` backup files is triggered automatically without app crashes.
