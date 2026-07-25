# World System Report — Hero of Eternia

**Version:** 0.12.0  
**Audit Date:** 2026-07-25  

---

## 1. Procedural World Pipeline

```
   Manual Seed (string) ──► FNV-1a Hash (64-bit ulong)
                                │
                                ▼
                       Terrain heights query
       Ridged Mountain + Simplex Continent + Valley Noise
                                │
                                ▼
     Chunk Grid (16x16 nodes) ──► Walkable cell slope query
                                │
                                ▼
         Resource spawner weights + Landmark plateaus placement
                                │
                                ▼
                  Asynchronous Chunk Streaming
```

---

## 2. Component Specifications

### 2.1 Seed Consistency
- Hashing parses alphanumeric seeds deterministically. The same seed produces identical terrain heights, landmark plateaus, and spawned resources.

### 2.2 Biome Distribution
- Configured in Settings to map temperature/moisture variables into regional biomes (Forest, Desert, Tundra, Mountains, Plains).

### 2.3 Async Chunk Streaming
- `ChunkManager` handles loading/unloading in task threads.
- Radius buffers prevent loading thrashing during circular navigation paths.

### 2.4 World Environment & Time
- `WorldTimeSystem` rotates solar coordinates across 4 intervals (Sunrise, Day, Sunset, Night) and triggers seasonal transitions.
- `WeatherManager` controls rain, wind, fog, blizzards, and sandstorms using profile offsets.

---

## 3. World Persistence

State changes (broken rocks, opened chests, cut trees) serialize under:
- `ModifiedChunkNodes` dict: Maps chunk coordinate keys to lists of removed node IDs.
- `ModifiedDecorations` dict: Tracks vegetation state changes.
Saves recover to exact states upon reloading slots.
