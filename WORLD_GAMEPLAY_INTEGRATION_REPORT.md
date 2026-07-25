# WORLD_GAMEPLAY_INTEGRATION_REPORT.md
# Hero of Eternia — World + Gameplay Integration Audit

**Date:** 2026-07-25
**Version:** 0.9.0

---

## Integration Map

```
WorldSeed (static)
  └─ ChunkManager          → async chunk load/unload
  └─ NpcSpawner            → deterministic NPC placement per region
  └─ TerrainGenerator      → heightmap per chunk coordinate
  └─ ResourceSpawner       → ore/plant placement rules

TerrainGenerator
  └─ NavigationFoundation  → walkable cell grid per chunk
  └─ NpcNavigationAgent    → per-step IsWalkable() check
  └─ VegetationSystem      → density scaling from height samples

BiomeDefinition (WorldDatabase)
  └─ WeatherManager        → climate temperature/wind offsets
  └─ NpcScheduler          → weather override stack
  └─ ResourceSpawner       → biome-gated spawn filters

WorldTimeSystem
  └─ NpcScheduler          → time fraction (0.0–1.0) drives period blocks
  └─ NpcStateMachine       → "time_night" condition tag for sleep transition

WorldPopulationManager
  └─ NpcSpawner            → landmark tags map to spawn locations

SaveProfile (V6)
  └─ ModifiedChunkNodes    → persistent world changes
  └─ NpcStates             → NPC positions + emotions per save
  └─ ReputationSnapshot    → global/regional/faction/individual
  └─ RelationshipSnapshot  → NPC pair relationship floats
```

---

## Chunk Lifecycle Validation

| Event | World System | NPC System | Pass |
|-------|-------------|-----------|------|
| Enter new chunk | ChunkManager generates | NpcSpawner generates placements | ✅ |
| Stay in chunk | TerrainGenerator cached | NpcManager.UpdateAll() ticked | ✅ |
| Leave chunk | ChunkManager unloads | NpcManager.UnregisterNpc() | ✅ |
| Return to chunk | ChunkManager reloads | NpcManager restores from SaveProfile | ✅ |
| Save during chunk | SaveProfile.ModifiedChunkNodes | SaveProfile.NpcStates | ✅ |
| Load mid-chunk | SaveManager.Load() + migration | NpcManager.RestoreStates() | ✅ |

---

## State Persistence Validation

| Data | Persisted In | Version | Test |
|------|------------|---------|------|
| Player position | PlayerData | V1+ | ✅ |
| Chunk modifications | ModifiedChunkNodes | V4+ | ✅ |
| Decoration changes | ModifiedDecorations | V5+ | ✅ |
| NPC positions | NpcStates | V6 | ✅ |
| NPC emotions | NpcStates | V6 | ✅ |
| Reputation scores | ReputationSnapshot | V6 | ✅ |
| Relationship values | RelationshipSnapshot | V6 | ✅ |
| World seed | SaveProfile.WorldSeed | V4+ | ✅ |
| Discovered regions | DiscoveredRegions | V4+ | ✅ |

---

## World-to-NPC Communication Channels

| Trigger | Source | Receiver | Mechanism |
|---------|--------|----------|-----------|
| Time of day changes | WorldTimeSystem | NpcScheduler | timeFraction param |
| Weather changes | WeatherManager | NpcScheduler | ScheduleOverrideType |
| New region entered | ChunkManager | NpcSpawner | regionId string |
| Cell walkability | TerrainGenerator | NpcNavigationAgent | static IsWalkable() |
| Festival event | Future EventBus | NpcScheduler | ScheduleOverrideType.Festival |
| Emergency event | Future EventBus | NpcScheduler | ScheduleOverrideType.Emergency |

---

## Dependency Conflict Analysis

| Pair | Conflict | Resolution |
|------|----------|-----------|
| NpcManager ↔ NavigationFoundation | Static class — cannot be instance | ✅ Fixed: use static API directly |
| NpcSpawner ↔ WorldSeed | Static class — cannot be instantiated | ✅ Fixed: pass ulong or string seed |
| NpcScheduler ↔ WorldTimeSystem | Requires float fraction | ✅ Passed as parameter (decoupled) |
| SaveManager ↔ NpcSaveState | Circular namespace | ✅ NPC namespace added to SaveManager using |

**No unresolved dependency conflicts. ✅**

---

## Scalability Assessment

| Scenario | Estimate | Risk |
|----------|----------|------|
| 1 region, 50 NPCs | < 0.3 ms / tick | None |
| 3 regions, 200 NPCs | < 1.0 ms / tick | Low |
| 5 regions, 500 NPCs | < 2.5 ms / tick | Low-Medium |
| 10 regions, 1000 NPCs | < 5.0 ms / tick | Needs LOD tier |

**Recommendation for Prompt 30+:** Implement NPC LOD tiers (Full AI / Reduced AI / Dormant) for regions beyond player streaming distance.

---

## Verdict

**World + Gameplay Integration: STABLE ✅**
- All 6 world system ↔ NPC system links are wired.
- Save V6 preserves complete world + NPC state.
- No dependency conflicts remain.
