# HERO OF ETERNIA — CHAPTER 07: ACT II FINALE & REGIONAL SIEGE

---

## 1. Overview & Chapter Scope

Chapter 7 concludes **Act II** of Hero of Eternia. A regional void rift breach destabilizes the Eastern Ridgeline, bringing a massive shadow army to the gates of Valenhold and Eternia Prime. Players lead the defense, destroy siege engines, conduct a counter-assault into the Shadow Crucible, and defeat **Malakor's Harbinger: Shadow Lord Emissary**.

---

## 2. Quest Chain Structure

- `q_chapter7_crisis_call`: Mobilize allied faction forces (Vanguard, Syndicate, Sylvan).
- `q_chapter7_siege_defense`: Defend outer gate barricades against 3 enemy shadow waves.
- `q_chapter7_final_assault`: Lead the rampart counter-assault and defeat Shadow Lord Malakor Emissary.
- `q_act2_conclusion`: Witness the aftermath, report to High King Roderick, and set the stage for Act III.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 7.

### A. 3D Model Generation Prompts
1. **Shadow Lord Malakor Emissary — Act II Finale Boss (`enemy_boss_malakor_emissary`)**
   - **Asset Name**: `MalakorEmissary.glb`
   - **Purpose**: Act II Finale Boss 3D model.
   - **Style & Art Direction**: Towering ethereal void demon clad in cracked obsidian plate armor with a glowing purple void core and double-bladed dark scythe.
   - **Technical Specifications**: 5,000 Polygons (LOD0), 2,500 (LOD1), 850 (LOD2). Bounding size: 2.8m x 3.5m x 2.2m.
   - **AI Prompt**: `"3D character model of a towering shadow void demon warlord, obsidian armor, glowing purple void energy, double-bladed dark scythe, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/MalakorEmissary.glb`

---

## 4. Save Integration (Save V34)

Act II finale progress, siege stages, boss completion, and world aftermath flags persist under `SaveVersion = 34` in `Chapter7SaveData.cs`.
