# Test Plan - Hero of Eternia

This test plan outlines the verification strategy for *Hero of Eternia* to guarantee runtime stability, performance, and hardware compatibility.

---

## 1. Automated Testing Strategy
We will execute unit tests within Godot using a testing library (such as GUT or standard NUnit integrations):
*   **Unit Tests:**
    *   *Save System:* Verify slot reads/writes, integrity signatures, and database version migration scripts.
    *   *Movement Vector Math:* Test steering yaw/pitch angles and boundary offsets.
    *   *Inventory & Item Spawning:* Verify slot transfers, equipment status changes, and weight limits.
*   **Integration Tests:**
    *   *EventBus Binds:* Test communication loops between CombatManager, UIManager, and AudioManager.
    *   *Game State Machine Transitions:* Verify transition flows (Menu -> Loading -> Active -> Pause -> Main Menu).

---

## 2. Manual Verification Checklist
*   **Interface Testing:**
    *   Select graphics preset: Verify detail settings changes (resolution, particles) update the UI description.
    *   Load save slots: Check that selecting slots updates metadata correctly.
*   **Gestures and Inputs:**
    *   Steer virtual joysticks: Verify touch movement vectors trigger expected steering changes.
    *   Adjust slider throttle: Verify speed values match slider percentages.

---

## 3. Stress and Performance Benchmarks
*   **Entity Count Limit:** Spawn 50+ moving asteroids / monsters. Ensure frame rate does not drop below 30 FPS on minimum spec hardware.
*   **Memory Leak Test:** Run continuous scene loads/unloads for 20 minutes, monitoring memory footprint limits via profiling tools.

---

## 4. Platform Compatibility
*   **Android Testing:** Deploy APKs on Android 8+ WSA (Windows Subsystem for Android) or physical devices.
*   **Aspect Ratio Testing:** Test layouts on 16:9, 18:9, and 21:9 screen sizes to verify layout responsive anchors.
