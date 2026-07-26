# HERO OF ETERNIA — ACT II OVERVIEW & DESIGN SYSTEM

---

## 1. Overview & Campaign Scope

Act II widens the scope of **Hero of Eternia** beyond the local conflicts of Act I into a sprawling regional adventure. Players enter the high-altitude **Eastern Ridgeline** and the treacherous **Mirkwood Swamps**, navigating complex faction politics in the metropolis of **Valenhold Citadel**.

---

## 2. Key Systems Introduced

- **Major Regional Metropolis**: Valenhold Citadel featuring 6 distinct districts (Government, Market & Harbor, Crafting Foundry, Guild Hall & Arena, Temple Heights, Guard HQ).
- **Faction Politics Engine**: Dynamic political influence meters per region (Iron Vanguard, Silver Syndicate, Sylvan Circle), trade disputes, territorial concessions, and alliance paths.
- **Companion Mechanics**: Seraphine Vael (Arcane Scout) joins the player's party with 3 unique tactical abilities.
- **Advanced Progression**: Tier 3 gear preview, advanced crafting stations (War Forge, Alchemy Cauldron), and multi-stage exploration vaults.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Act II.

### A. 3D Model Generation Prompts
1. **Valenhold Citadel Gatehouse (`building_valenhold_gate`)**
   - **Asset Name**: `ValenholdGatehouse.glb`
   - **Purpose**: Main entry fortress model into Valenhold Citadel.
   - **Style & Art Direction**: High fantasy stone fortress with iron-reinforced portcullis, banner posts, and runic battlements.
   - **Technical Specifications**: 3,500 Polygons (LOD0), 1,800 (LOD1), 600 (LOD2). Bounding size: 12.0m x 15.0m x 8.0m. PBR maps (Albedo, Normal, Roughness, AO).
   - **AI Prompt**: `"Game-ready 3D asset model of a massive medieval stone gatehouse fortress, iron portcullis, heraldic banners, dark fantasy style, PBR textures, clean topology"`
   - **Folder Location**: `res://Assets/Models/Buildings/ValenholdGatehouse.glb`

2. **Seraphine Vael — Companion Model (`char_companion_seraphine`)**
   - **Asset Name**: `SeraphineVael.glb`
   - **Purpose**: Arcane Scout companion character model.
   - **Style & Art Direction**: Agile female scout wearing leather-mithril weave armor with glowing arcane dagger sheaths.
   - **Technical Specifications**: 4,200 Polygons (LOD0), 2,100 (LOD1), 800 (LOD2). Rigged with humanoid skeleton.
   - **AI Prompt**: `"Full body 3D character model of a female elf arcane scout, dark leather and silver mithril armor, glowing cyan dagger handles, athletic build, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Characters/SeraphineVael.glb`

### B. Audio & Voice Prompts
1. **Valenhold Metropolis Orchestral Theme (`music_valenhold_metropolis`)**
   - **Specification**: 24-bit 48kHz WAV audio, 2 min 45 sec loop.
   - **AI Music Prompt**: `"Grand heroic orchestral theme with majestic brass fanfares, sweeping strings, and steady snare march, high fantasy city theme"`
   - **Folder Location**: `res://Assets/Audio/Music/music_valenhold_metropolis.wav`

---

## 4. Save Persistence (Save V31)

Act II state is fully persisted under `SaveVersion = 31` in `Act2SaveData.cs`, saving region discoveries, Companion join flags, city district unlocks, faction influence scores, and cleared exploration vaults.
