# Known Limitations - Hero of Eternia

This document lists the known technical bottlenecks and architectural constraints of *Hero of Eternia* for the target Android platforms.

---

## 1. Engine Backend Compatibility
*   **GLES 3.0 (Compatibility Mode):** To support Android 8.0+ devices with a 2GB RAM minimum, we use Godot's Compatibility renderer (GLES 3.0) instead of Vulkan Forward+. This limits advanced post-processing effects (such as high-end volumetric fog and SSAO).

---

## 2. Platform Limitations
*   **Android Heap/GC Limits:** Low-end ARM64 devices with 2GB RAM can experience Garbage Collection spikes. We address this constraint by using strict object pooling (avoiding `new` allocations in physics loops).
*   **Storage Space Limits:** Native Android APK packaging must remain compact. We aim for a target size under 50MB, restricting high-resolution texture maps (clamped to 1024x1024 / 2048x2048 ETC2 format).

---

## 3. Graphics & Shaders Constraints
*   **Dynamic Lighting Caps:** Mobile GPUs have restricted fragment processor registers. We limit dynamic lights in a scene (typically 1 directional light and a maximum of 3-4 local point lights simultaneously).
*   **Shader Variants Complexity:** Complex fragment calculations can trigger thermal throttling on mobile devices. Shaders must be kept instruction-light, avoiding nested loops.
