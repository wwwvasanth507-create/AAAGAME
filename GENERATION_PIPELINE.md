# Procedural Generation Pipeline — Hero of Eternia

This manual defines the generation sequence, quality scaling, and verification audits.

---

## 1. Generation Lifecycle Sequence

```
1. Receive Active Seed (ulong)
     │
     ├── 2. Preload Biome Definitions & Static database
     │
     └── 3. Update Player Location Coordinates
           │
           ├── 4. Generate Chunks (Task.Run background thread)
           │     ├── A. Layered Noise Height queries (TerrainGenerator)
           │     ├── B. Biome mapping (Grassland, Volcano, Desert, etc.)
           │     ├── C. Spawner rules verification (slope and water filters)
           │     ├── D. Graphics density scaling (VegetationSystem)
           │     └── E. Modified nodes history check (mine offsets)
           │
           └── 5. Run Quality Scanner (WorldValidator)
                 └── Floating object scans & overlaps checks
```

---

## 2. Graphics Density Scaling

Vegetation and decoration counts adapt to user performance presets:

| Preset Level | Density Scaling | Nodes Target (Base: 100) |
|---|---|---|
| **Low** | `25%` | 25 |
| **Medium** | `60%` | 60 |
| **High** | `100%` | 100 |
| **Ultra** | `150%` | 150 |

---

## 3. Automated Validation Reporting

The `WorldValidator` service audits loaded regions, appending warning nodes if:
- Placed entity `LocalY` deviates from `TerrainY` by more than 0.5 units (floating objects).
- Placed entity overlaps another entity's bounding box.
- Entity is placed in an incompatible biome (e.g. Oak tree in Ocean).
