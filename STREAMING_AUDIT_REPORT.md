# Chunk Streaming Audit Report — Hero of Eternia (v0.7.0)

This report logs the technical audit of the asynchronous chunk loading, thread-safety queues, and mobile memory performance.

---

## 1. Asynchronous Task Loaders

To prevent main gameplay thread lag on Android, chunk generation is processed on background worker threads:
- **Task Spawns:** `ChunkManager.UpdatePlayerPosition` uses `Task.Run()` to process chunk loading.
- **Concurrent Maps:** Storing chunks in `ConcurrentDictionary<string, Chunk>` protects lookup calls from cross-thread exceptions.
- **Loading Queue Lock:** A `HashSet` loading queue protected via locks prevents duplicate task allocations for a single chunk coordinate.

---

## 2. Load and Unload Boundaries

Coordinates calculations use a double-radius buffer layout to prevent loading thrashing during player borders crossing:
- **Load Distance:** Chunks within radius 2 are loaded.
- **Unload Distance:** Chunks are only unloaded outside radius 3 (Distance + 1 buffer).
- **Stress traversal:** Repeated traversal and player position shifts (tested headlessly) process cleanly without duplicates or memory spikes.

---

## 3. Object Pooling Hooks

The manager exposes two events:
- `OnChunkLoaded(Chunk chunk)`: Visual systems capture chunk nodes coordinates to retrieve meshes from a pool.
- `OnChunkUnloaded(Chunk chunk)`: Triggers reclaiming active meshes, returning them to the pool without memory allocation overhead.
