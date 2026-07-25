# SHADER SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 23)

## System Overview
`ShaderManager.cs` provides unified control over material parameters across character, terrain, vegetation, water, sky, and transparent shaders.

## Key Features
* **Dissolve Hooks**: Controls `dissolve_amount` (0.0 to 1.0) for enemy death disintegration and item spawning.
* **Highlight Hooks**: Sets `highlight_color` and `highlight_intensity` for interactive loot objects and target selection.
* **Seasonal Variants**: Applies global seasonal tint parameters (`Winter`, `Autumn`, `Spring`) across vegetation and terrain materials.
