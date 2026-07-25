# Persistent World Object Audit — Hero of Eternia (v0.8.0)

This report logs the technical audit of chunk modifications history, harvested states, and save recovery persistence.

---

## 1. Object State Persistence

To ensure player modifications (harvested trees, mined ores, collected herbs) persist, chunk loading references modification tables:
- **Modified Lists:** The `Chunk` structural data-model includes `ModifiedNodeIds` and `ModifiedDecorations` hash sets.
- **Save mapping:** Changes map to `SaveProfile.ModifiedChunkNodes` and `ModifiedDecorations` lists using chunk key indices (e.g. `"3_5"`).
- **Offline Integrity:** Disk write overhead scales only with modified coordinates, keeping overall data structures light.

---

## 2. Walkable Re-sweeps & Restoration

Upon reloading a save slot:
- **Modified Checks:** Generation loops query the loaded profiles. Matching nodes are skipped.
- **Navigation Restoration:** Mined boulders or chopped trees clear their grid cells, flagging cells walkable dynamically.
- **Stress traversal:** Moving between coordinate chunks and back maintains states cleanly.
