# HERO OF ETERNIA — CHAPTER 12: ALLIANCE CAMPAIGN, WORLD WAR & LEGENDARY EQUIPMENT

---

## 1. Overview & Chapter Scope

Chapter 12 brings together all major factions, settlements, and companions for the **Grand Alliance Campaign**. Players assemble the Council of Sol, secure cross-region supply lines, acquire the first complete **Solwarden Legendary Equipment Set**, and conduct the final campaign briefing at the Obsidian Citadel Gate.

---

## 2. Quest Chain Structure

- `q_chapter12_alliance_council_assembly`: Convene leaders in the Grand Alliance Council.
- `q_chapter12_supply_line_liberation`: Escort allied war caravans through the Crystal Wasteland.
- `q_chapter12_solwarden_artifact_recovery`: Forge the Solwarden Astral Greatsword and acquire the complete Tier 5 Legendary Regalia.
- `q_chapter12_final_alliance_briefing`: Conduct final briefing on the eve of the Obsidian Citadel siege.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 12.

### A. 3D Model Generation Prompts
1. **Solwarden Astral Greatsword (`item_legendary_solwarden_greatsword`)**
   - **Asset Name**: `SolwardenAstralGreatsword.glb`
   - **Purpose**: Tier 5 Legendary Weapon 3D model.
   - **Style & Art Direction**: Ornate white-gold sun greatsword with floating sun rune core and glowing golden edge aura.
   - **Technical Specifications**: 2,800 Polygons (LOD0), 1,400 (LOD1). Bounding size: 0.3m x 1.6m x 0.1m.
   - **AI Prompt**: `"3D item model of a legendary white-gold sun greatsword with glowing floating rune core, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Equipment/SolwardenAstralGreatsword.glb`

---

## 4. Save Integration (Save V39)

Grand Alliance readiness percentage, faction loyalty ratings, completed war events, acquired Solwarden set pieces, and Chapter 12 quest states persist under `SaveVersion = 39` in `Chapter12SaveData.cs`.
