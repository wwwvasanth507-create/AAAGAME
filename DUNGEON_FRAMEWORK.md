# DUNGEON FRAMEWORK SPECIFICATION — HERO OF ETERNIA (PROMPT 24)

## System Overview
`DungeonFramework.cs` provides reusable subterranean and interior dungeon architecture supporting room graphs, enemy spawn anchors, puzzle triggers, and boss chamber placement.

## Architecture
* **Room Graphs**: Node-based connectivity graph connecting entrances, corridors, puzzle rooms, and boss chambers.
* **Dungeon Types**: `Crypt`, `Mine`, `Cavern`, `Ruins`, `Fortress`, `Rift`.
* **Completion Tracking**: Persists cleared dungeon IDs in `SaveProfile` Version 19.
