# HERO OF ETERNIA — CHAPTER 05: BRANCHING STORYLINES & FACTION DUNGEON

---

## 1. Overview & Chapter Scope

Chapter 5 introduces **Hero of Eternia**'s first major branching narrative arc. Players must choose how to approach the heavily fortified **Stronghold of Iron & Shadow** by aligning with one of three competing faction strategies:

1. **Iron Vanguard Assault Branch**: Direct frontal assault using heavy siege battery support and breaching gates.
2. **Silver Syndicate Shadow Branch**: Stealth infiltration via hidden underground smuggling tunnels to bypass outer guard towers.
3. **Sylvan Circle Secret Branch**: Environmental navigation through sunken sewer waterways using elemental alchemy to dissolve barrier seals.

---

## 2. Quest Chain Overview

- `q_chapter5_infiltration`: Scout the Stronghold perimeter and identify faction emissary positions.
- `q_chapter5_alliance_choice`: Make the pivotal choice decision pledging support to Vanguard, Syndicate, or Sylvan commanders.
- `q_chapter5_dungeon_climax`: Breach the Inner Sanctuary and defeat Grand Marshal Kaelen.

---

## 3. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for Chapter 5.

### A. 3D Model Generation Prompts
1. **Grand Marshal Kaelen — Boss Model (`enemy_boss_grand_marshal_kaelen`)**
   - **Asset Name**: `GrandMarshalKaelen.glb`
   - **Purpose**: Chapter 5 Faction Dungeon main boss model.
   - **Style & Art Direction**: Imposing warlord in dark steel plate armor adorned with glowing gold runic engravings and a tower shield.
   - **Technical Specifications**: 4,500 Polygons (LOD0), 2,200 (LOD1), 750 (LOD2). Bounding size: 2.2m x 2.8m x 1.8m.
   - **AI Prompt**: `"Game-ready 3D character boss model of a dark knight warlord, ornate black and gold armor, glowing runic tower shield and broadsword, PBR textures, clean topology"`
   - **Folder Location**: `res://Assets/Models/Enemies/GrandMarshalKaelen.glb`

### B. Audio & Music Prompts
1. **Stronghold Dungeon Combat Theme (`music_dungeon_stronghold`)**
   - **Specification**: 24-bit 48kHz WAV audio, 2 min 30 sec loop.
   - **AI Music Prompt**: `"Tense dramatic orchestral combat track with pounding war drums, heavy cellos, and brass stabs, dark fantasy dungeon battle music"`
   - **Folder Location**: `res://Assets/Audio/Music/music_dungeon_stronghold.wav`

---

## 4. Save Integration (Save V32)

Chapter 5 progress and active branch choices persist under `SaveVersion = 32` in `Chapter5SaveData.cs`.
