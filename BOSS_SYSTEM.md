# Boss System — Hero of Eternia

**Version:** 0.12.0  
**Phase:** Prompt 12 / 150 — Boss Framework  
**Status:** ✅ Production Ready

---

## Overview

The Boss System provides a data-driven, headless-safe architecture for scripting boss battles. All components run completely detached from the Godot scene tree structure, allowing unit-test verification in a headless CI pipeline.

---

## Core Architecture

```
   boss_database.json
           │
           ▼
     BossDatabase ──► BossDefinition (BossData & Class)
                              │
                              ├── BossPhaseSystem (Transitions, Multipliers, triggers)
                              └── SpecialAttackData (Cast profiles)
```

---

## BossDefinition

**File:** `Scripts/Combat/Boss/BossDefinition.cs`

Wraps `BossData` which defines:
- **Class Profile**: Guardian, Behemoth, Mage, Summoner, Stalker.
- **Base Attributes**: MaxHp, MaxShield, Armor, MoveSpeed, Element, Weaknesses, Resistances, LootTableId.
- **Profiles**: Music, VFX, Voice, Camera, and Rewards.
- **Phases**: Scalable multi-stage battle setups.
- **ExtensionData**: Dynamic properties mapping.

---

## Boss Phase System

**File:** `Scripts/Combat/Boss/BossPhaseSystem.cs`

Calculates transitions and state modifications dynamically during ticks:
* **HpThresholdPct**: Transition triggers when boss health drops below threshold (e.g. 0.5f = 50% HP).
* **Backup Timer**: Enrages automatically after 120 seconds in the same phase.
* **State Modifications**: Multiplies Speed, Damage, changes active attack indexes, and publishes EventBus triggers (`BossVfxTriggerEvent`, `BossSfxTriggerEvent`).

---

## Special Attacks

* **Types**: MeleeCombo, AreaOfEffect, ProjectilePattern, SummonHook, MovementCharge, BeamAttack, GroundHazard.
* **Configuration**: CastTime, Cooldown, BaseDamage, Range, AoeRadius, ProjectileCount, and VFX/SFX hooks.

---

## AI Asset Production Specs

### 1. Golem Titan Model

| Property | Value |
|----------|-------|
| **Asset Name** | `golem_titan` |
| **Class** | Behemoth |
| **Model Format** | glTF 2.0 (.glb) |
| **Target Polygon Budget** | 4,000 triangles |
| **PBR Textures** | 2048×2048 (BaseColor, Metallic, Roughness, Normal, AO, Emission) |

**AI Generation Prompt:**
> A massive ancient stone golem boss character, 4 meters tall, cracked dark granite rock textures, glowing orange runes engraved on arms and chest, brass joints, heavy broad silhouette. T-pose, low-poly mobile optimized, 3D model game asset.

---

### 2. Audio SFX Specs

| Hook Key | Description | AI Generation Prompt |
|----------|-------------|----------------------|
| `sfx_titan_slam_cast` | Slam wind-up | Heavy stone grinding with low rumble charging energy, 1.2s |
| `sfx_titan_charge_cast` | Rush charge | Deep mechanical piston hum and stones scraping rapidly, 1.0s |
| `sfx_titan_enrage` | Phase 2 transition | Giant rocky roar with crystal resonance explosion, 2.2s |
