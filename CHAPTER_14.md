# HERO OF ETERNIA — CHAPTER 14: FINAL BOSS ENCOUNTER & MULTI-PHASE FINALE

---

## 1. Overview & Chapter Scope

Chapter 14 delivers the gameplay climax of Hero of Eternia — the 4-phase final boss battle against **Arch-Sorcerer Malakor, Void Avatar** (12,000 Total HP across 4 distinct combat phases). Featuring adaptive AI, anti-exploit distance safeguards, dynamic arena terrain shifts, phase transition triggers, and Save Profile Version 41, this chapter intentionally pauses prior to the narrative epilogue.

---

## 2. Quest Chain Structure

- `q_chapter14_entering_throne_room`: Pass through the threshold and enter Malakor's Throne Room.
- `q_chapter14_malakor_phase1_defeat`: Defeat High Warden Malakor and strip his corrupted solar armor.
- `q_chapter14_malakor_phase2_defeat`: Survive gravity distortions and shatter Malakor's Void Avatar form.
- `q_chapter14_malakor_final_defeat`: Deliver the final strike to the Unbound Void Core.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 14.

### A. 3D Model Generation Prompts
1. **Arch-Sorcerer Malakor, Void Avatar (`boss_malakor_void_avatar`)**
   - **Asset Name**: `ArchSorcererMalakorVoidAvatar.glb`
   - **Purpose**: Final Boss 3D Character Model (4 Phases).
   - **Style & Art Direction**: Colossal dark paladin in shattered obsidian plate with floating void crystal crown, glowing purple eyes, and radiant sunfire broadsword.
   - **Technical Specifications**: 6,200 Polygons (LOD0), 3,100 (LOD1), 1,100 (LOD2). Bounding size: 3.2m x 4.5m x 2.4m.
   - **AI Prompt**: `"3D character model of a towering dark sorcerer armor with glowing purple void energy seams and a floating crystal crown, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/ArchSorcererMalakorVoidAvatar.glb`

---

## 4. Save Integration (Save V41)

Final Boss phase completion, Malakor defeat flag, highest phase reached, acquired boss trophies, and Chapter 14 quest states persist under `SaveVersion = 41` in `Chapter14SaveData.cs`.
