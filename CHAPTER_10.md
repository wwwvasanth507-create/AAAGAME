# HERO OF ETERNIA — CHAPTER 10: ANCIENT TEMPLE COMPLEX & ACT III FINALE

---

## 1. Overview & Chapter Scope

Chapter 10 concludes **Act III** of Hero of Eternia. Shifting the focus from military conflict to ancient history, players unseal the **Temple of the Eternal Sun**, solve light reflection and water control puzzles, uncover the true origin of Arch-Sorcerer Malakor's power, and witness the major campaign turning point preparing for Act IV.

---

## 2. Quest Chain Structure

- `q_chapter10_temple_discovery`: Unseal the Portal of Astral Light.
- `q_chapter10_puzzle_sanctum`: Solve the Water Prism Reflection Array and Weight Plate Sequence.
- `q_chapter10_astral_revelation`: Reach the Core Astral Vault and discover the Golden Codex plate revealing Malakor's origin.
- `q_act3_conclusion`: Conclude Act III, report to Archivist Orion, and unlock late-game temple recipes.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 10.

### A. 3D Model Generation Prompts
1. **Astral Guardian Guardian Construct (`enemy_boss_astral_guardian`)**
   - **Asset Name**: `AstralGuardianConstruct.glb`
   - **Purpose**: Chapter 10 Temple Vault Guardian boss model.
   - **Style & Art Direction**: Ancient white marble and brass automaton with glowing golden sun runes along its chest plate and double sun blades.
   - **Technical Specifications**: 4,400 Polygons (LOD0), 2,200 (LOD1), 700 (LOD2). Bounding size: 2.5m x 3.2m x 2.0m.
   - **AI Prompt**: `"3D character model of an ancient white marble and brass guardian automaton, glowing gold sun runes, double sun blades, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/AstralGuardianConstruct.glb`

---

## 4. Save Integration (Save V37)

Act III finale progress, temple chamber clearances, solved puzzles, unlocked codex entries, and revelation flags persist under `SaveVersion = 37` in `Chapter10SaveData.cs`.
