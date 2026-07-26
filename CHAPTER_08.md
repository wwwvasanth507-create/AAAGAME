# HERO OF ETERNIA — CHAPTER 08: THE SHADOW FRONTIER

---

## 1. Overview & Chapter Scope

Chapter 8 opens **Act III**, introducing the high-level wasteland of **The Shadow Frontier**. Players cross the ruined border post, utilize the Iron Grapple Hook to traverse treacherous chasms, explore corrupted fortresses, and confront the Shadow Behemoth Warlord on Obsidian Crag.

---

## 2. Quest Chain Structure

- `q_chapter8_shadow_frontier_entry`: Cross the Wall of Shadows and establish an advance outpost.
- `q_chapter8_traversal_challenge`: Utilize the Iron Grapple Hook to cross the Dread Ravine chasm.
- `q_chapter8_shadow_champion_confrontation`: Ascend Obsidian Crag Sanctuary and defeat the Shadow Behemoth Warlord.

---

## 3. AI Asset Production Report

### A. 3D Model Generation Prompts
1. **Shadow Behemoth Warlord (`enemy_shadow_behemoth`)**
   - **Asset Name**: `ShadowBehemoth.glb`
   - **Purpose**: Chapter 8 regional champion boss 3D model.
   - **Style & Art Direction**: Quadrupedal shadow monstrosity with obsidian stone horns, glowing void cracks along its spine, and massive bladed forelimbs.
   - **Technical Specifications**: 4,800 Polygons (LOD0), 2,400 (LOD1), 800 (LOD2). Bounding size: 3.5m x 2.8m x 4.0m.
   - **AI Prompt**: `"3D creature model of a massive shadow behemoth monster, obsidian stone horns, glowing purple void cracks, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/ShadowBehemoth.glb`

---

## 4. Save Integration (Save V35)

Act III transition, traversal tool unlocks, zone discoveries, and boss completion persist under `SaveVersion = 35` in `Chapter8SaveData.cs`.
