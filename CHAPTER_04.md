# HERO OF ETERNIA — CHAPTER 04 QUESTLINE & DESIGN DOCUMENT

---

## 1. Chapter Overview

Chapter 4 initiates Act II narrative progression following the aftermath of Act I. The player travels into the Eastern Ridgeline to investigate political unrest and uncover Void Cult incursions threatening Valenhold.

---

## 2. Quest Chain Structure

| Quest ID | Quest Name | Primary Objective | Rewards |
| :--- | :--- | :--- | :--- |
| `q_act2_begins` | Arrival at the Ridgeline | Travel to Eastern Ridgeline outpost and report to Commander Harek. | 400 XP, 150 Gold |
| `q_act2_ridgeline_rescue` | Ridgeline Ambush | Rescue Seraphine Vael from Shadow Cult Vanguard ambushers. | Seraphine Companion Unlock, 600 XP |
| `q_act2_watchtower` | Watchtower Liberation | Liberate the Mirkwood Border Watchtower from Captain Drael. | Mirkwood Region Unlock, 800 XP, Tier 2 Gear |
| `q_act2_mirkwood_intel` | Swamps of Shadow | Gather Void Cult intelligence deep within Mirkwood Swamps. | 1000 XP, 300 Gold, Crafting Recipes |

---

## 3. AI Asset Production Report

### A. 3D Model Generation Prompts
1. **Shadow Cult Vanguard Captain Drael (`enemy_boss_drael`)**
   - **Asset Name**: `CaptainDrael.glb`
   - **Purpose**: Chapter 4 mini-boss enemy model.
   - **Style & Art Direction**: Heavily armored corrupt knight with jagged shadow plate armor and a glowing greatsword.
   - **Technical Specifications**: 3,800 Polygons (LOD0), 1,900 (LOD1), 600 (LOD2).
   - **AI Prompt**: `"3D enemy boss model of a corrupt knight in dark spiked steel armor, glowing purple aura, holding a massive runic greatsword, game-ready PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/CaptainDrael.glb`

---

## 4. Verification

All Chapter 4 quests are registered in `QuestDatabase` via `Act2QuestChain.cs` and verified by `Act2SystemTests`.
