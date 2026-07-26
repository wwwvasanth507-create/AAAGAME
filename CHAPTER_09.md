# HERO OF ETERNIA — CHAPTER 09: CORRUPTED FORTRESS & ANTAGONIST FACTION

---

## 1. Overview & Chapter Scope

Chapter 9 advances Act III into the heart of Malakor's military power. Players infiltrate the **Fortress of Obsidian Shadows**, disrupt legion supply lines, liberate captive allied scouts from the Prison Catacombs, and confront **General Vaelis the Unforgiving**.

---

## 2. Quest Chain Structure

- `q_chapter9_fortress_recon`: Scout outer battlements and identify searchlight towers.
- `q_chapter9_prison_sabotage`: Rescue captive scouts and sabotage dark steel armory caches.
- `q_chapter9_command_assault`: Defeat General Vaelis in the War Arena and uncover Malakor's invasion maps.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 9.

### A. 3D Model Generation Prompts
1. **General Vaelis the Unforgiving (`enemy_boss_general_vaelis`)**
   - **Asset Name**: `GeneralVaelis.glb`
   - **Purpose**: Chapter 9 Fortress Commander boss model.
   - **Style & Art Direction**: Imposing dark warlord in heavy obsidian plate armor, wearing a spiked horned helm, carrying a glowing void tower shield and dark steel greatsword.
   - **Technical Specifications**: 4,600 Polygons (LOD0), 2,300 (LOD1), 750 (LOD2). Bounding size: 2.3m x 2.9m x 1.9m.
   - **AI Prompt**: `"3D character model of a dark knight commander, obsidian armor, horned spiked helmet, void tower shield, dark greatsword, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/GeneralVaelis.glb`

---

## 4. Save Integration (Save V36)

Fortress clearance, General Vaelis defeat, alert levels, and supply disruption flags persist under `SaveVersion = 36` in `Chapter9SaveData.cs`.
