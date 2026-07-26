# HERO OF ETERNIA — FACTION DUNGEON: STRONGHOLD OF IRON & SHADOW

---

## 1. Overview & Floor Layout

The **Stronghold of Iron & Shadow** is a 6-floor handcrafted faction dungeon featuring alternative entrance routes corresponding to the player's chosen narrative alignment:

- **Floor 1**: Alternative Entrance Routes:
  - Vanguard Assault Gate (`room_stronghold_vanguard_gate`)
  - Syndicate Smugglers' Tunnel (`room_stronghold_syndicate_tunnels`)
  - Sylvan Sewer Passage (`room_stronghold_sylvan_sewer`)
- **Floor 2**: Central Armory Courtyard (`room_stronghold_courtyard`)
- **Floor 3**: Elemental Relic Crucible & Puzzles (`room_stronghold_puzzle_crucible`)
- **Floor 4**: Inquisitor's Tribunal & Checkpoint (`room_stronghold_inquisitor_hall`)
- **Floor 5**: Secret Vault of the Forgotten Crown (`room_stronghold_secret_vault`)
- **Floor 6**: Boss Arena: Grand Marshal's Sanctuary (`room_stronghold_boss_arena`)

---

## 2. AI Asset Production Report

### A. 3D Prop Prompts
1. **Stronghold Heavy Iron Gate (`prop_iron_gate_heavy`)**
   - **Asset Name**: `StrongholdIronGate.glb`
   - **Purpose**: Interactive locked door gate separating dungeon floors.
   - **Technical Specifications**: 1,200 Polygons (LOD0), 600 (LOD1), 200 (LOD2).
   - **AI Prompt**: `"3D model asset of a massive medieval iron dungeon portcullis gate with heavy rivets and rusted steel chains, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Props/StrongholdIronGate.glb`

---

## 3. Code Reference

Managed by `FactionDungeonContent.cs` and evaluated in `Chapter5Manager.cs`. Tested by `Chapter5SystemTests`.
