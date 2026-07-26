# HERO OF ETERNIA — CHAPTER 15: ENDING, EPILOGUE & CAMPAIGN COMPLETION

---

## 1. Overview & Campaign Conclusion Scope

Chapter 15 delivers the narrative resolution of Hero of Eternia — transitioning the player from Malakor's defeated Void Spire into the restored world of Eternia (`WorldState_DawnOfSol`), featuring an ending choice system (`EndingChoice`), interactive regional epilogue visits, a scrollable credits system (`CreditsSystemManager`), campaign completion statistics (`CampaignCompletionTracker`), and Save Profile Version 42.

---

## 2. Quest Chain Structure

- `q_chapter15_sun_spire_restoration`: Channel celestial light to ignite the restored Sun Spire atop the Citadel.
- `q_chapter15_settlement_victories`: Visit Valenhold, Eternia Prime, and Sun Archivist strongholds to share victory tidings.
- `q_chapter15_epilogue_celebration`: Attend the grand victory feast held in your honor at Eternia Prime.
- `q_chapter15_post_campaign_horizon`: Stand ready as Eternia's champion for post-game challenges beyond the horizon.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 15.

### A. 3D Model Generation Prompts
1. **Restored Celestial Sun Spire Prop (`prop_restored_sun_spire`)**
   - **Asset Name**: `RestoredSunSpire.glb`
   - **Purpose**: Interactive prop 3D model placed atop the Citadel in Sector 8 during Chapter 15.
   - **Style & Art Direction**: Majestic white marble spire wrapped in polished gold filigree and emitting a brilliant vertical beam of pure golden sunlight.
   - **Technical Specifications**: 3,200 Polygons (LOD0), 1,600 (LOD1), 600 (LOD2). Bounding size: 4.0m x 12.0m x 4.0m.
   - **AI Prompt**: `"3D architecture prop asset of a towering white marble and polished gold sun spire emitting a beam of light, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Props/RestoredSunSpire.glb`

---

## 4. Save Integration (Save V42)

Campaign completion flag (`IsCampaignCompleted = true`), completion timestamp, total play statistics, awarded title (`Champion of Sol`), credits viewed flag, and Chapter 15 quest states persist under `SaveVersion = 42` in `Chapter15SaveData.cs`.
