# HERO OF ETERNIA — GRAND ALLIANCE CAMPAIGN ENGINE

---

## 1. Overview & Faction Assembly

The **Alliance Campaign Manager** (`AllianceCampaignManager.cs`) tracks multi-faction cooperation:

- **Valenhold Militia (`faction_valenhold`)**: 250 Troops, base in Crystal Wasteland.
- **Eternia Royal Guard (`faction_eternia_prime`)**: 500 Troops, base in Ashen Astral Battlefield.
- **Shadow Frontier Scouts (`faction_shadow_rangers`)**: 180 Troops, base in Caelum Ruins.
- **Archivists of Sol (`faction_sun_archivists`)**: 120 Troops, base in Forgotten Sun Spire.
- **Alliance Readiness Score**: 0-100% composite score driving siege readiness at the Obsidian Citadel Gate.

---

## 2. AI Asset Production Report

### A. UI Banner Prompts
1. **Grand Alliance Council Banner (`ui_alliance_council_banner`)**
   - **Resolution**: 1024x256 PNG transparent.
   - **AI Prompt**: `"Game UI alliance crest banner depicting a sun wheel flanked by twin silver swords, gold metallic texture, transparent background"`
   - **Folder Location**: `res://Assets/UI/Banners/ui_alliance_council_banner.png`

---

## 3. Code Reference

Managed by `AllianceCampaignManager.cs` and tested by `Chapter12SystemTests`.
