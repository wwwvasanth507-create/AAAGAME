# Procedural World Architecture — Hero of Eternia

This manual defines the structural division, coordinates grids, and elements databases of the procedural world.

---

## 1. World Hierarchy

The world is divided into distinct, hierarchical layers to optimize loading scopes and balance data sizes:

```
World (Normal, Underground, DLC Dimensions)
  └── Regions (Discovered states, e.g. "EterniaFields")
        └── Biomes (Data-driven profiles, Forest/Desert/Snow)
              └── Chunks (32x32 units streaming blocks)
                    └── Cells (Sub-chunk placement nodes)
```

### Partitioning Elements
- **Regions:** Large geographical territories loaded dynamically. Player entry marks the region discovered in `SaveProfile`.
- **Chunks:** 32x32 unit square chunks. Terrain loading, static object spawning, and resource loading operate at chunk boundaries.
- **Cells:** Smaller grids within chunks to cache coordinate references for rocks, trees, and spawners.

---

## 2. Deterministic Seeds (64-bit)

Procedural generation uses 64-bit (`ulong`) seeds to produce identical coordinates across devices:
- **FNV-1a 64-bit Hashing:** Converts text entries (e.g. `"AncientLakes"`) to ulong hashes.
- **Deterministic RNG:** Seeds Godot's `RandomNumberGenerator` for each chunk based on:
  `chunkSeed = ActiveSeed ^ (ulong)coords.X ^ ((ulong)coords.Y << 32);`
  This guarantees that vegetation, rocks, and ore nodes spawn at the exact same offsets for a given seed.

---

## 3. World Database Structure

All environment items are defined in `world_database.json` via `ConfigManager` to allow adding new types without rebuilding.
- **Vegetation / Rocks / Ores:** Mapped as `WorldElementRecord` keys (e.g. `tree_oak`, `ore_iron`), referencing their 3D GLB mesh paths and spawn weights.
