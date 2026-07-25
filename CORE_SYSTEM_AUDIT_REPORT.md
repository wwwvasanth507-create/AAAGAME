# Core Architecture Audit Report — Hero of Eternia (v0.6.0)

This report documents the architectural audit, initialization loops, dependency boundaries, and decoupling rules.

---

## 1. Manager Initialization & Lifecycle Flow

Managers initialize in a deterministic, linear sequence managed on startup to prevent boot dependency race conditions:

```
TestRunner / Boot Scene
  ├── ConfigManager.Initialize() ── (Populates JSON templates)
  ├── SettingsManager.Initialize() ── (Loads volume and sensitivity values)
  ├── LocalizationManager.Initialize() ── (Loads base tables)
  ├── PerformanceManager.Initialize() ── (Sets initial scaling bounds)
  ├── AudioManager.Initialize() ── (Spawns 16-node player pools)
  ├── SceneManager.Initialize() ── (Pre-fetches Boot target)
  └── UIManager.Initialize() ── (Sets screen layers)
```

- **generic ServiceLocator (DI):** All core classes implement `IInitializable`. circular coupling is prevented because dependencies are resolved dynamically via lazy lookup loops during `Initialize()` rather than constructor injection.
- **Node-safe Shutdown:** Managers registered as nodes (SceneManager, AudioManager) cleanup automatically during SceneTree exits.

---

## 2. EventBus Thread-Safety

`EventBus.cs` decouples modules by routing events globally. To prevent thread conflicts under concurrent loading conditions (e.g. background resource loads updating progression states), the listeners dictionary uses lock boundaries:

```csharp
private readonly Dictionary<Type, List<Delegate>> _eventListeners = new();
private readonly object _lock = new object();

public void Subscribe<T>(Action<T> listener)
{
    lock (_lock)
    {
        // Add to listeners array safely
    }
}
```

---

## 3. Dependency Boundary Analysis

To ensure long-term scalability and support future online extensions:
1. **Low-Coupling Module Design:** The Player, Items, Core, and UI namespaces compile independently. For example, `InventoryContainer.cs` has zero references to player models or scene meshes.
2. **Open/Closed ServiceLocator:** Registering custom services requires no changes to the locator code. DLC nodes or online network adapters can register as new service plugins dynamically on boot.
3. **No Unsafe Global States:** Managers do not contain static instance singletons (`public static Manager Instance`). This prevents illegal cross-thread mutations and keeps testing completely isolated.
