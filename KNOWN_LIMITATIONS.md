# Known Limitations - Hero of Eternia

This document lists the known technical bottlenecks and architectural constraints of *Hero of Eternia* for the target Android platforms.

---

## 1. Engine Backend Compatibility
*   **GLES 3.0 (Compatibility Mode):** To support Android 8.0+ devices with a 2GB RAM minimum, we use Godot's Compatibility renderer (GLES 3.0) instead of Vulkan Forward+. This limits advanced post-processing effects (such as high-end volumetric fog and SSAO).

---

## 2. Platform Limitations
*   **Android Heap/GC Limits:** Low-end ARM64 devices with 2GB RAM can experience Garbage Collection spikes. We address this constraint by using strict object pooling (avoiding `new` allocations in physics loops).
*   **Storage Space Limits:** Native Android APK packaging must remain compact. We aim for a target size under 50MB, restricting high-resolution texture maps (clamped to 1024×1024 / 2048×2048 ETC2 format).

---

## 3. Graphics & Shaders Constraints
*   **Dynamic Lighting Caps:** Mobile GPUs have restricted fragment processor registers. We limit dynamic lights in a scene (typically 1 directional light and a maximum of 3–4 local point lights simultaneously).
*   **Shader Variants Complexity:** Complex fragment calculations can trigger thermal throttling on mobile devices. Shaders must be kept instruction-light, avoiding nested loops.

---

## 4. Phase 3 Known Limitations

### 4.1 Battery Monitoring
*   `OS.GetPowerPercentLeft()` was removed in Godot 4.x. Battery percentage in PerformanceMonitor displays "N/A" until a community plugin (e.g., `godot-power-vitals`) is integrated in a future phase.

### 4.2 Physical RAM Detection
*   Godot 4.x does not expose total physical RAM via a public C# API. `DeviceDetector.SystemRamMb` uses a conservative heuristic (`static_memory_bytes × 8`) from `Performance.GetMonitor(MemoryStatic)`. This underestimates RAM on high-end devices. A future phase may integrate a platform-specific JNI/NDK call on Android to query `ActivityManager.MemoryInfo`.

### 4.3 Device-Bound Save Files
*   Save files are encrypted with a key derived from `OS.GetUniqueId()`. This is intentional for local security, but **save files cannot be restored to a different device** without a server-side re-encryption step. Cloud sync must handle this in a future online phase.

### 4.4 Screen Resolution in Headless Mode
*   `DisplayServer.WindowGetSize()` returns `(0, 0)` in headless (`--headless`) mode. DeviceDetector logs `Res=0x0` during automated test runs — this is expected and not a runtime bug.

### 4.5 ServiceLocator & Thread Safety During Init
*   `ServiceLocator.Get<T>()` acquires a lock and then calls `IInitializable.Initialize()` inside the lock. If `Initialize()` itself calls `ServiceLocator.Get<T2>()` (circular dependency), it will deadlock. **Rule:** Never call `ServiceLocator.Get<T>()` inside an `Initialize()` method — use constructor injection or post-init hooks instead.
