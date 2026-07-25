# RENDERING SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 23)

## System Overview
`RenderingOptimizationManager.cs` provides LOD distance culling, shadow resolution limits, GPU instancing toggles, and dynamic render scaling.

## Quality Scaling

| Feature | Low Preset (Mobile) | Medium Preset | High Preset |
|---|---|---|---|
| Shadow Quality | Off | Low | High |
| Max Draw Distance | 80m | 120m | 200m |
| GPU Instancing | Enabled | Enabled | Enabled |
| Render Scale | 0.8x | 0.9x | 1.0x |
