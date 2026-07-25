# REGIONAL BOSS SPECIFICATION — HERO OF ETERNIA (PROMPT 30)

## Commander Vareth — Void Knight (`RegionalBossDefinition.cs`)

| Property | Value |
|---|---|
| **Boss ID** | `boss_commander_vareth` |
| **Level** | 18 |
| **Max HP** | 1,800 |
| **Phase 2 Threshold** | 65% HP |
| **Phase 3 Threshold** | 30% HP |
| **Loot Table** | `loot_boss_vareth_act1` |

---

## Combat Phases

### Phase 1 — Guardian of the Gate (HP: 100% → 65%)
- **Void Slash** (55 dmg, 3.5s CD): Telegraphed melee cleave.
- **Shadow Bolt Barrage** (35 dmg, 6s CD): Ranged projectile fan.
- Arena: Stable ground, predictable patrol patterns.

### Phase 2 — Unleashed Darkness (HP: 65% → 30%)
- All Phase 1 abilities, plus:
- **Void Surge Wave** (80 dmg, 12s CD): Large floor-sweeping AoE requiring side-step.
- Arena: Void energy cracks appear in floor — environmental hazard zones.

### Phase 3 — Desperate Void (HP: 30% → 0%)
- All Phase 2 abilities, plus:
- **Void Gate Summon** (CD 20s): Spawns 2 Shadow Wraith adds.
- **Desperate Voidstrike** (120 dmg, 25s CD): Fully telegraphed charged slam — requires dodge roll.
- Arena: Floor largely cracked — reduced safe zones.

---

## Design Notes
- All attacks are telegraphed with 1.5–2.5s wind-up animations.
- Android-optimized: Boss AI ticks every 100ms, not every frame.
- Checkpoint at `room_boss_antechamber` prevents progress loss on death.
