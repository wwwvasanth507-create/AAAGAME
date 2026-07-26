# HERO OF ETERNIA — TEMPLE PUZZLE SYSTEM FRAMEWORK

---

## 1. Scope & Puzzle Categories

The **Temple Puzzle Sequence** framework (`TemplePuzzleSequence.cs`) evaluates layered environmental puzzles:

- **Sun Rune Dial (`SunRuneDial`)**: Rotate concentric marble dials to align sun, dawn, and noon runes.
- **Light Prism Reflection (`LightPrismReflect`)**: Rotate crystal prisms to reflect beam of light onto sanctuary door gems.
- **Water Level Controls (`WaterLevelValve`)**: Drain and flood lower temple channels to reveal submerged staircases.
- **Weight Plate Sequences (`WeightPlateSequence`)**: Step on balance plates in numerical order to release iron portcullises.

---

## 2. AI Asset Production Report

### A. 3D Puzzle Props
1. **Light Reflection Crystal Prism (`prop_light_prism_crystal`)**
   - **Asset Name**: `LightPrismCrystal.glb`
   - **Purpose**: Moveable light reflection puzzle prop.
   - **Technical Specifications**: 1,200 Polygons (LOD0), 600 (LOD1).
   - **AI Prompt**: `"3D model asset of a glowing clear quartz crystal mounted on carved brass pedestal ring, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Props/LightPrismCrystal.glb`

---

## 3. Code Reference

Managed by `TemplePuzzleSequence.cs` and verified by `Chapter10SystemTests`.
