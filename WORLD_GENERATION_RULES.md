# WORLD GENERATION RULES SPECIFICATION — HERO OF ETERNIA (PROMPT 24)

## System Overview
`WorldGenerationRules.cs` validates procedural POI placement using slope angle limits, elevation filtering, road/water proximity, settlement distance, and seed-reproducible pseudo-random values.

## Placement Constraints
* **Max Slope Angle**: Rejects POI placement on steep cliffs (>25 degrees).
* **Settlement Buffer**: Enforces minimum clearance distance (150m) from existing towns and settlements.
* **Seed Reproducibility**: Uses deterministic seed hashing (`WorldSeed`, `chunkX`, `chunkZ`) for identical world generation results across sessions.
