# HERO OF ETERNIA — SIEGE SYSTEM FRAMEWORK

---

## 1. Overview & Battle Stages

The **Siege System Framework** (`SiegeEncounterManager.cs`) orchestrates multi-phase siege encounters:

- **Stage 1: Preparation (`Preparation`)**: Barricade setup, ally positioning, and siege weapon loading.
- **Stage 2: Wall Defense (`WallDefense`)**: Defensive wave battle repelling shadow forces from gate ramps.
- **Stage 3: Counter-Assault (`BreachCounterAssault`)**: Offensive charge breaking enemy lines and siege towers.
- **Stage 4: Victory Sequence (`VictorySequence`)**: Cinematic victory fanfare, barrier sealing, and loot distribution.

---

## 2. AI Asset Production Report

### A. 3D Siege Prop Prompts
1. **Siege Engine Ram (`prop_siege_engine_ram`)**
   - **Asset Name**: `SiegeRam.glb`
   - **Purpose**: Destructible enemy siege weapon prop.
   - **Technical Specifications**: 2,200 Polygons (LOD0), 1,100 (LOD1), 400 (LOD2).
   - **AI Prompt**: `"3D model of a heavy dark timber siege battering ram with spiked iron plating and wheels, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Props/SiegeRam.glb`

---

## 3. Code Reference

Managed by `SiegeEncounterManager.cs` and verified by `Chapter7SystemTests.cs`.
