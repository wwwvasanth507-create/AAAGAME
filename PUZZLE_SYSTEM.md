# PUZZLE SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 25)

## System Overview
`PuzzleManager.cs` provides a data-driven reusable puzzle state engine managing pressure plates, levers, switches, rune activations, light reflection, weight sensors, and multi-stage puzzles.

## Supported Puzzle Mechanisms
* **PressurePlate**: Triggered by player weight or movable physics props.
* **Lever & Switch**: Toggleable mechanical components.
* **RuneActivation**: Sequential or pattern-matched rune lighting.
* **LightReflection**: Rotating mirrors to guide light beams to receptors.
* **MultiStage**: Puzzles requiring consecutive solved steps before unlocking.
