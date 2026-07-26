# HERO OF ETERNIA — REUSABLE SUPER BOSS FRAMEWORK

---

## 1. Super Boss Roster & Stats

The **Super Boss Framework** (`SuperBossFramework.cs`) manages 3 elite post-game encounters:

1. **Chronos, Titan of Fractured Time (`boss_chronos_titan`)**
   - **Base Health**: 18,000 HP | Recommended Level: 50.
   - **Location**: Chamber of Fractured Timelines.
   - **Unique Mechanics**: Time Dilation Slow Waves & Temporal Reversal Clones.
   - **Trophy Drop**: `trophy_chronos_hourglass`.

2. **Astral Leviathan of the Void (`boss_astral_leviathan`)**
   - **Base Health**: 22,000 HP | Recommended Level: 52.
   - **Location**: Astral Rift Abyss.
   - **Unique Mechanics**: Gravity Singularity Swallows & Void Beam Sweeps.
   - **Trophy Drop**: `trophy_leviathan_astral_scale`.

3. **Sun King's Ascended Memory (`boss_sol_prime_avatar`)**
   - **Base Health**: 25,000 HP | Recommended Level: 55.
   - **Location**: Sanctum of the First Dawn.
   - **Unique Mechanics**: Solar Flare Supernovas & Radiant Aegis Invulnerability Shields.
   - **Trophy Drop**: `trophy_sol_prime_crown`.

---

## 2. Difficulty Multipliers

- **Normal**: 1.0x HP & Damage.
- **Heroic**: 1.5x HP & Damage + Faster Attack Cooldowns.
- **Mythic**: 2.2x HP & Damage + 5-Minute Enrage Timer.

---

## 3. AI Asset Production Report

### A. 3D Boss Model Prompts
1. **Chronos Titan 3D Model (`boss_chronos_titan_model`)**
   - **Asset Name**: `ChronosTitan.glb`
   - **Technical Specifications**: 6,800 Polygons (LOD0), 3,400 (LOD1).
   - **AI Prompt**: `"3D character model of a colossal ancient clockwork stone titan holding a glowing cyan hourglass staff, PBR textures"`
   - **Folder Location**: `res://Assets/Models/Enemies/ChronosTitan.glb`

---

## 4. Code Reference

Managed by `SuperBossFramework.cs` and tested by `PostGameSystemTests`.
