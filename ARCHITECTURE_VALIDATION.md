# Architecture Validation Report — Hero of Eternia (v0.2.0)

This document validates the core structural interfaces, manager decoupled systems, event flows, and local storage formats.

---

## 1. Modular Services & DI Container
All 12 core system modules are mapped to independent C# namespaces. Communication limits:
*   Managers must not call methods on other managers.
*   Resolving manager objects is handled during Boot via a Service Locator context:
    `var saveManager = ServiceLocator.Get<SaveManager>();`

---

## 2. EventBus Pub-Sub System
All cross-layer notifications (e.g. settings updates, gameplay triggers, state transitions) execute asynchronously via the delegate-based EventBus:
*   *Publisher:* `EventBus.Publish(new SettingsUpdatedEvent(volume));`
*   *Subscriber:* `EventBus.Subscribe<SettingsUpdatedEvent>(OnSettingsChanged);`
This ensures changes in one module do not break compilations of dependent packages.

---

## 3. Storage & Migration Architecture
*   **Tamper Protection:** Save slot files (`slot_*.sav`) are appended with an MD5 hash signature calculated from unique hardware and profile attributes. If bytes are altered offline, loading is aborted.
*   **Version Migrations:** Slots check a 32-bit schema header version. If the load detects older slot formats, the SaveManager triggers SQL/binary schema updates automatically.

---

## 4. Offline / Online Dual Topology
The game is completely standalone:
*   All gameplay, saves, configs, and states run 100% offline.
*   Optional online sync services (cloud saves, leaderboard profiles) are mapped through interfaces. No gameplay systems depend on active socket connections.
