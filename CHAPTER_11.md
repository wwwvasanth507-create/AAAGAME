# HERO OF ETERNIA — CHAPTER 11: ENDGAME REGION & LEGENDARY PROGRESSION

---

## 1. Overview & Chapter Scope

Chapter 11 initiates **Act IV** of Hero of Eternia. Shifting the focus to endgame preparation, players cross into **The Astral Divide**, establish a forward base, unlock Tier 5 Legendary Crafting, battle the Apex Crystal Behemoth, and breach the perimeter of the Obsidian Citadel Gate.

---

## 2. Quest Chain Structure

- `q_chapter11_astral_divide_entry`: Establish forward camp in The Crystal Wasteland.
- `q_chapter11_legendary_research`: Unlock Tier 5 Legendary Crafting at the Sun Spire Altar.
- `q_chapter11_elite_trial`: Defeat the Apex Crystal Behemoth roaming the Crystal Wasteland.
- `q_chapter11_astral_champion_confrontation`: Secure the perimeter of the Obsidian Citadel Gate.

---

## 3. AI Asset Production Report

### A. 3D Model Generation Prompts
1. **Apex Crystal Behemoth (`enemy_crystal_behemoth`)**
   - **Asset Name**: `ApexCrystalBehemoth.glb`
   - **Purpose**: World Mini-Boss 3D model in The Crystal Wasteland.
   - **Style & Art Direction**: Massive quadrupedal beast composed of glowing jagged purple starlight quartz crystals with a core void heart.
   - **Technical Specifications**: 4,800 Polygons (LOD0), 2,400 (LOD1), 800 (LOD2). Bounding size: 4.5m x 3.8m x 3.2m.
   - **AI Prompt**: `"3D character model of a giant crystal behemoth creature made of sharp glowing purple quartz spikes, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/ApexCrystalBehemoth.glb`

---

## 4. Save Integration (Save V38)

Act IV progress, unlocked legendary recipes, discovered Astral Divide sub-zones, cleared elite encounters, and legendary materials persist under `SaveVersion = 38` in `Chapter11SaveData.cs`.
