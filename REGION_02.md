# HERO OF ETERNIA — REGION 02: EASTERN RIDGELINE & MIRKWOOD SWAMPS

---

## 1. Regional Overview

Region 02 introduces two massive interconnected biomes establishing the Act II wilderness:

1. **Eastern Ridgeline (`region_eastern_ridgeline`)**:
   - **Recommended Level**: 19–24
   - **Environment**: High-altitude mountain passes, ancient stone watchtowers, pine forests, and rocky cliffs.
   - **Hazards**: High-altitude howling winds (stamina drain modifier) and crumbling cliff edges.

2. **Mirkwood Swamps (`region_mirkwood_swamps`)**:
   - **Recommended Level**: 21–27
   - **Environment**: Deep murky wetlands, glowing bioluminescent flora, sunken ruins, and fog-covered grottos.
   - **Hazards**: Poisonous swamp water (slow poison damage tick) and dense visual fog.

---

## 2. AI Asset Production Report

### A. Environment & Texture Prompts
1. **Mirkwood Swamp Water Material (`material_swamp_water`)**
   - **Resolution**: 2048x2048 PNG (Albedo, Normal map, Roughness).
   - **AI Prompt**: `"Tileable game texture of murky greenish brown swamp water with floating duckweed algae and subtle green bioluminescent ripples, high detail normal map"`
   - **Folder Location**: `res://Assets/Textures/Environment/SwampWater.png`

---

## 3. Streaming & Performance

Both sub-regions integrate into the infinite chunk streaming pipeline (`ChunkManager.cs`), capping active memory under 450 MB RAM on mobile devices.
