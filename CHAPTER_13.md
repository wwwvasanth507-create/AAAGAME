# HERO OF ETERNIA — CHAPTER 13: FINAL DUNGEON, ENDGAME STRONGHOLD & PRE-FINAL ENCOUNTERS

---

## 1. Overview & Chapter Scope

Chapter 13 constructs the **entire final dungeon experience** — **The Citadel of Obsidian Void** — across 8 interconnected sectors, featuring checkpoint waypoints, shortcut iron gates, pre-final mini-boss encounters, and ending intentionally at the threshold of Arch-Sorcerer Malakor's Throne Room in the Pre-Final Antechamber.

---

## 2. Quest Chain Structure

- `q_chapter13_breaching_citadel`: Lead the Grand Alliance assault team through the Outer Breach and capture the Fortified Gatehouse.
- `q_chapter13_machine_core_sabotage`: Overload the void shield generators in the Machine Core.
- `q_chapter13_gatekeeper_confrontation`: Defeat Archon of the Sunless Void and High Commander Vaelis Remnant on the Grand Promenade.
- `q_chapter13_pre_final_antechamber_reached`: Unseal the final sanctuary doors and stand ready in the Pre-Final Antechamber.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 13.

### A. 3D Model Generation Prompts
1. **Archon of the Sunless Void (`encounter_archon_sunless_void`)**
   - **Asset Name**: `ArchonSunlessVoid.glb`
   - **Purpose**: Pre-final Citadel mini-boss 3D model in Sector 6 (Subterranean Portal Vault).
   - **Style & Art Direction**: Floating levitating sorcerer wrapped in dark eclipse robes with four floating void mirror blades and glowing purple eyes.
   - **Technical Specifications**: 4,600 Polygons (LOD0), 2,300 (LOD1), 750 (LOD2). Bounding size: 2.2m x 3.0m x 1.8m.
   - **AI Prompt**: `"3D character model of a dark levitating void sorcerer wearing dark eclipse robes with four floating purple void mirror blades, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/ArchonSunlessVoid.glb`

---

## 4. Save Integration (Save V40)

Citadel sector clearances, active checkpoint IDs, unlocked shortcuts, defeated pre-final mini-bosses, and Chapter 13 quest states persist under `SaveVersion = 40` in `Chapter13SaveData.cs`.
