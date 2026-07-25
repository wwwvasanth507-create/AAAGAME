# EXPLORATION SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 24)

## System Overview
`ExplorationManager.cs` tracks discovered POIs, landmarks, fog-of-war map reveal states, region completion stats, and achievement triggers.

## Key Features
* **Area Discovery**: Triggers `OnLocationDiscovered` events and awards exploration XP when entering a new POI radius.
* **Map Reveal Hooks**: Unveils local map fog-of-war chunks around discovered points.
* **Persistence**: Discovered location IDs are persisted in `SaveProfile` Version 19 via `WorldContentSaveData`.
