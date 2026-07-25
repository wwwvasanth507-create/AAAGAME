# World Audit Report — Hero of Eternia (v0.7.0)

This report details the technical audit of the procedural generation seeds, biome configurations, and world database.

---

## 1. Procedural Seed Determinism

Deterministic world generation is verified using ulong seeds parsed via `WorldSeed.cs`:
- **Text Hashing:** String entries (e.g. `"GrassFields"`) are hashed to ulong via FNV-1a.
- **Validation:** Validation regex prevents illegal directory characters, ensuring seed share strings remain safe.
- **RNG Reproducibility:** Godot's `RandomNumberGenerator` is initialized with the chunk seed. Multiple evaluations on coordinate `(0,0)` yield identical float coordinates (verified by `TestRunner.cs`).

---

## 2. Biome Data Configurations

Biomes are parsed from `biomes.json` and loaded into the `WorldDatabase` service cache on startup.

### Extensible Profiles
Adding a new biome (e.g., `FloatingIslands`, `NetherCaverns`) only requires appending a record block in the JSON file with:
- Humidity and temperature float indices.
- Elevation limits (Min/Max elevations).
- Visual environment hooks (Sky profile, Lighting tint hex).
- Sound tracks keys.

C# classes use `BiomeType` enum as the key lookup, but support raw string fallback matching, ensuring DLC biomes populate without rebuilds.
