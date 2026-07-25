# WORLD STATE SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 26)

## System Overview
`WorldStateManager.cs` provides a reversible state engine managing story flags, regional flags, global flags, settlement states, NPC availability, enemy variants, and weather overrides.

## Key Mechanics
* **Flag Mutation**: `SetFlag(key, value)` sets state flags and pushes a `StateChangeEvent` onto an undo stack.
* **Reversibility**: `RevertLastStateChange()` pops the last state change and restores previous values, allowing branching choices or temporary storyline overrides.
* **Persistence**: Active world state flags are saved in `SaveProfile` Version 21.
