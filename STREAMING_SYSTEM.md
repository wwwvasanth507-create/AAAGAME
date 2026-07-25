# Chunk Streaming & Mobile Performance — Hero of Eternia

This manual defines the chunk loading sequences, asynchronous threads execution, and object pooling hooks.

---

## 1. Asynchronous Chunk Load Sequence

To avoid frame rate blocks on Android mobile, loading and unloading execute on separate threads via C# Task libraries:

```
UpdatePlayerPosition()
  └── Calculates Active Chunk Coordinate (X, Z)
        ├── Chunks within StreamingDistance (radius: 2) -> Run Task.Run()
        │     └── LoadChunkAsync() -> deterministic RNG rolls -> Emit OnChunkLoaded
        └── Chunks outside Distance + 1 -> Unload -> Emit OnChunkUnloaded
```

- **Distance Buffer:** Chunks are only unloaded outside `Distance + 1` to act as a buffer and prevent loading/unloading thrashing when players cross borders.
- **Thread Safety:** Dictionary manipulations utilize `ConcurrentDictionary` and lock objects.

---

## 2. Android Mobile Optimizations

1. **Object Pooling Hooks:** The `OnChunkLoaded` and `OnChunkUnloaded` events allow rendering managers to reclaim meshes (trees, rocks) from pools instead of allocating memory at runtime.
2. **Deterministic PRNG:** Random number generation uses ulong seed computations. Same coordinates roll regardless of device platform (Android or PC).
3. **Save Game Compaction:** Only modified node coordinates are persisted to slots, keeping disk footprint for 10,000 modified slots under 16 KB.
