# Layered Noise Terrain System — Hero of Eternia

This manual defines the terrain generation passes, mathematical formulations, and reproducibility guarantees.

---

## 1. Noise Passes & Layering Formulas

Terrain elevation Y is calculated dynamically using three overlapping C# `FastNoiseLite` passes:

$$\text{Elevation}(x, z) = \text{BaseContinental}(x, z) + \text{MountainRanges}(x, z) - \text{ValleyCarving}(x, z)$$

### Low Frequency Base (Continental)
- **Algorithm:** Simplex Noise.
- **Frequency:** `0.005f` (smooth rolling hills).
- **Scale:** Multiplied by `12.0f` to scale limits from `-12` to `+12` units.

### High Frequency Ridges (Mountain peaks)
- **Algorithm:** Simplex Noise.
- **Frequency:** `0.015f`.
- **Fractal:** Ridged Fractal type.
- **Masking:** Multiplied by a base noise mask to limit high peaks to specific continental regions.

### Valleys & Rivers (Carving)
- **Algorithm:** Simplex Noise.
- **Frequency:** `0.01f`.
- **Carving:** Subtracts elevations along narrow threshold bands to simulate rivers and drainage trenches.

---

## 2. Flat Plateaus Capping

To facilitate village placements, flat plateau overrides cap Y values:

$$\text{if } 25.0 < Y < 28.0 \implies Y = 25.0$$

This creates flat-topped mesas suitable for building placement.
