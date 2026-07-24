# Architecture Health Report — Hero of Eternia (v0.4.0)

This report details the architectural health of the *Hero of Eternia* codebase as of Phase 4.

---

## 1. Core Systems & Coupling

The project employs a hybrid decoupled service architecture utilizing a central **ServiceLocator** (for persistent managers/singletons) and a typed generic **EventBus** (for cross-layer pub-sub events).

```
+--------------------------------------------------------------+
|                     ServiceLocator                           |
|  - Registration of IInitializable managers                  |
|  - Resolves dependencies dynamically & logs timing           |
+--------------------------------------------------------------+
         |                      |                      |
         v                      v                      v
+------------------+   +------------------+   +------------------+
|   SaveManager    |   |  SettingsManager |   |  ConfigManager   |
+------------------+   +------------------+   +------------------+
         |                      |                      |
         +----------------------+----------------------+
                                |
                                v
             [ Decoupled via EventBus notifications ]
```

### Key Service Modules
1. **GameManager**: Standard state transitions (Boot, MainMenu, Playing, Paused, GameOver).
2. **InputHandler**: Polls hardware devices, combines gamepad/keyboard/mouse, accepts virtual touch input injections from `TouchControls`, and provides a unified frame-by-frame read-only snapshot (`InputFrame`).
3. **CameraController**: Positioned under player camera pivot, provides spring-arm collision avoidance, custom distance, sprint FOV boosts, and camera shake.
4. **PlayerRoot**: Encapsulates character state. Composed of modular sub-controllers (`PlayerMovement`, `PlayerAnimationController`, `PlayerAudioController`, `PlayerData`, `PlayerSettings`, and `PlayerStateMachine`).

---

## 2. Dependency Matrix & Coupling Check

| Module | Depends On | Coupling Risk | Mitigation |
|---|---|---|---|
| `ServiceLocator` | `IInitializable` (interface) | **Low** | Extensible to any future manager type. |
| `PlayerRoot` | `InputHandler`, `CameraController` | **Low** | Soft references dynamically resolved at `_Ready()`. |
| `PlayerStateMachine` | `IPlayerState` (interface) | **None** | States registered at runtime. Closed to modification. |
| `SaveManager` | `Godot.OS` | **Medium** | Safe fallback constants used when native OS ID queries fail (e.g. headless test runs). |
| `EventBus` | Generic System Types | **None** | Completely decoupled. Publishers and subscribers do not know about each other. |

---

## 3. Data Flow Diagrams

### 3.1 Input System Data Flow
```
[Keyboard / Mouse] ----+
                       |
[Xbox / PS Gamepad] ---+---> [InputHandler.cs] ---> [InputFrame (Snapshot)] ---> [PlayerStates / Movement]
                       |          ^
[TouchControls UI] ----+----------+
(Virtual Joystick / Buttons)
```

### 3.2 Save / Load Sequence
```
[Save Action] ---> [SaveProfile (JSON String)]
                      |
                      v
             [Encrypt via AES-256] (using PBKDF2 device-bound key)
                      |
                      v
             [Generate SHA-256 Checksum]
                      |
                      v
             [Write to disk slot_X.sav] + [Create slot_X.sav.bak]
```

---

## 4. Save Architecture & Scalability

- **DLC / Mod Expansion:** The `SaveProfile` contains a dictionary annotated with `[JsonExtensionData]`. This allows future patches or DLC to load older save files without breaking or discarding unrecognized properties.
- **Migration Logic:** Serialized slots contain a header specifying `SaveVersion`. On load, if the file version is less than the runtime database version, the `MigrateProfile` routine updates stats and maps properties automatically.
- **Tamper Protection:** File validation verifies the SHA-256 signature of the encrypted payload before loading. If the checksum does not match, the manager automatically attempts recovery from the `.bak` file.

---

## 5. Technical Debt & Risks

1. **Synchronous Disk I/O:** Saving profiles and user options executes synchronously on the calling thread. While saves are small (under 5KB), writing to local storage on low-end Android eMMC devices can cause brief frame drops.
   * *Mitigation:* In Phase 5 (Database implementation), write operations will be refactored into async task operations using `System.Threading.Tasks.Task.Run()`.
2. **RAM Estimation Heuristic:** `DeviceDetector` estimates physical RAM using Godot's static memory profile. This is a conservative heuristic since Godot lacks native total physical RAM APIs.
   * *Mitigation:* Future Android custom native JNI wrappers will query the system activity memory info block directly.
3. **Deadlock Risk in ServiceLocator:** Service registration occurs under a lock. If a manager's `Initialize()` method synchronously resolves another manager that isn't fully initialized, a thread deadlock could occur.
   * *Mitigation:* Strict development guidelines rule out invoking `ServiceLocator.Get<T>()` inside constructor or initialization phases.
