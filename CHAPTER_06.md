# HERO OF ETERNIA — CHAPTER 06: CAPITAL CITY & GUILD EXPANSION

---

## 1. Overview & Chapter Scope

Chapter 6 elevates the narrative from regional conflicts into imperial intrigue within the sprawling capital city of **Eternia Prime**. Players gain entry into the capital, join imperial guilds, investigate subterranean Shadow Cult infiltration, and defeat the second major regional boss: **High Inquisitor Vesper**.

---

## 2. Quest Chain Structure

- `q_chapter6_capital_arrival`: Pass imperial checkpoints and report to High King Roderick's court.
- `q_chapter6_guild_induction`: Enlist in an imperial guild at the Grand Guild Enclave and complete induction bounties.
- `q_chapter6_boss_climax`: Confront and defeat High Inquisitor Vesper in the Sunken Catacombs.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 6.

### A. 3D Model Generation Prompts
1. **High King Roderick III (`npc_high_king_roderick`)**
   - **Asset Name**: `HighKingRoderick.glb`
   - **Purpose**: Imperial ruler NPC model.
   - **Style & Art Direction**: Regal elderly monarch wearing golden plate armor, velvet cape, and imperial crown.
   - **Technical Specifications**: 3,800 Polygons (LOD0), 1,900 (LOD1), 650 (LOD2). Humanoid rig.
   - **AI Prompt**: `"3D character model of an imperial medieval king, golden armor, red velvet cape with ermine trim, royal crown, wise regal face, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Characters/HighKingRoderick.glb`

---

## 4. Save Integration (Save V33)

Chapter 6 state, capital discoveries, guild ranks, and boss defeat flags persist under `SaveVersion = 33` in `Chapter6SaveData.cs`.
