# HERO OF ETERNIA — ELITE WORLD CONTENT FRAMEWORK

---

## 1. Scope & Elite World Encounters

The **Elite World Content Manager** (`EliteWorldContentManager.cs`) manages high-level open-world challenges:

- **Apex Crystal Behemoth (`elite_crystal_behemoth`)**: Level 41 world mini-boss roaming The Crystal Wasteland (4,500 HP). Drops `material_astral_essence`.
- **Corrupted Sun High Priest (`elite_sun_priest_lich`)**: Level 44 lich mini-boss inside the Forgotten Sun Spire (4,200 HP). Drops `material_sun_core_fragment`.

---

## 2. AI Asset Production Report

### A. SFX Prompts & Sound Specs
1. **Crystal Behemoth Roar SFX (`sfx_crystal_behemoth_roar`)**
   - **Specification**: 24-bit 48kHz WAV audio, 2.5 sec effect.
   - **AI Prompt**: `"Deep resonant monster roar blending shattered glass crystal resonance and heavy subterranean bass growl"`
   - **Folder Location**: `res://Assets/Audio/SFX/sfx_crystal_behemoth_roar.wav`

---

## 3. Code Reference

Managed by `EliteWorldContentManager.cs` and tested by `Chapter11SystemTests`.
