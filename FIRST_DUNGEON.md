# FIRST DUNGEON SPECIFICATION — HERO OF ETERNIA (PROMPT 30)

## Citadel of Void Shadows (`FirstDungeonContent.cs`)

### Floor Map

| Room ID | Display Name | Floor | Type | Checkpoint |
|---|---|---|---|---|
| `room_citadel_entrance` | Shattered Gates of Aethelgard | Entrance | Entrance | ✅ `cp_citadel_entrance` |
| `room_floor1_shadow_corridor` | Shadow Corridor | Floor 1 | Combat | ✅ `cp_floor1` |
| `room_floor1_puzzle_rune` | Rune Pressure Chamber | Floor 1 | Puzzle | — |
| `room_floor2_hazard_hall` | Void Spike Gauntlet | Floor 2 | Hazard | — |
| `room_floor2_secret_vault` | Ancient Etherian Vault | Floor 2 | **Secret** | — |
| `room_miniboss_arena` | Shadow Knight's Crucible | Mini-Boss | Boss Arena | ✅ `cp_miniboss` |
| `room_floor3_lore_chamber` | Codex Hall of the Void | Floor 3 | Lore | — |
| `room_boss_antechamber` | Sanctum of the Void Gate | Boss Ante | Rest | ✅ `cp_boss_antechamber` |
| `room_boss_arena` | Throne of the Void Gate | Boss | **Final Boss** | — |

---

### Design Principles
- **Exploration-first**: Secret vault rewards players who veer off the main path.
- **Hazard variety**: Void Spike Gauntlet uses environmental damage rather than combat.
- **Puzzle integration**: Rune Pressure Chamber uses the Shrine system from Chapter 1.
- **Checkpoint pacing**: 4 checkpoints prevent frustration on mobile (Android).
- **Streaming**: Each floor is a discrete streaming chunk to maintain Android performance targets.
