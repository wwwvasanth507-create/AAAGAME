# Biome & Resource Spawn System — Hero of Eternia

This manual defines the biome configuration profiles, weather states, and resource spawning structures.

---

## 1. Biome Profiles

Biomes are fully data-driven. The `WorldDatabase` service loads definitions from `biomes.json` containing the initial types:

- **Grassland & Forest:** Temperate regions with high tree densities.
- **Desert:** High temperature, low humidity, sandstorms weather.
- **Snow:** Low temperature, snow/blizzard weather profiles.
- **Swamp & Jungle:** Warm, humid zones with dense vegetation.
- **Volcano:** High temperature, ash falls, lava-aligned material configs.
- **Crystal Caverns & Ancient Ruins:** Specialty underground / POI structures.

---

## 2. Weather Profiles

The `WeatherManager` controls transitioning ambient environments:

| Weather Type | Temperature Modifier | Wind Strength | Visual Effect Hook | Ambient Sound |
|---|---|---|---|---|
| **Clear** | `0.0` | `0.0` | `Vfx_Clear` | `Ambient_Clear` |
| **Rain** | `-0.05` | `0.3` | `Vfx_Rain` | `Ambient_Rain` |
| **Storm** | `-0.1` | `0.8` | `Vfx_Storm` | `Ambient_Storm` |
| **Snow** | `-0.3` | `0.2` | `Vfx_Snow` | `Ambient_Snow` |
| **Blizzard** | `-0.5` | `0.9` | `Vfx_Blizzard` | `Ambient_Blizzard` |
| **Sandstorm** | `+0.1` | `0.7` | `Vfx_Sandstorm` | `Ambient_Sand` |

---

## 3. Resource Spawner Rules

Resources (Iron, Copper, Gold, Stone, Herbs) use `ResourceSpawnRule` files:
- **Biomes Limits:** Rules define which biomes are eligible (e.g. `ore_iron` spawns only in Mountains and Crystal Caverns).
- **Slope Verification:** Spawn queries evaluate terrain slope angles. Items do not spawn on cliff edges exceeding 30 degrees.
- **Deterministic Rolls:** Spawn locations are generated from chunk seed offsets. If the node ID exists in the chunk's `ModifiedNodeIds` hash (indicating it has been harvested/mined), it is bypassed during rendering.
