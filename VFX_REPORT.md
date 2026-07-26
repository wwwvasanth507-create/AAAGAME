# HERO OF ETERNIA — VFX SYSTEM AUDIT REPORT (PROMPTS 0–36)

---

## 1. Overview & VFX Framework

The **Visual Effects Engine** utilizes GPU particle pooling, custom spatial shaders, dynamic light emitters, and atmospheric particle volumes:

- **Combat VFX**: Slash trails, elemental weapon impacts, blood/spark splatters, and void aura bursts.
- **Ability VFX**: Fireball explosions, lightning chain arcs, holy healing rays, and void devastation novas.
- **Environmental VFX**: Spore miasma fog volumes, floating ash particles, falling rain, and void crystal ambient sparkles.
- **UI VFX**: Menu button glow effects, level-up particle bursts, and quest objective ping trails.

---

## 2. AI Asset Production Report

### A. Texture Prompts & Specifications
1. **Void Energy Particle Sheet (`vfx_void_energy_sheet`)**
   - **Resolution**: 1024x1024 4x4 Sprite Sheet PNG.
   - **AI Prompt**: `"Game VFX sprite sheet of 16 frames dark purple void energy explosion burst with glowing embers, transparent PNG background"`
   - **Folder Location**: `res://Assets/VFX/Textures/vfx_void_energy_sheet.png`

---

## 3. Code Reference

Managed by `VFXManager.cs` (`IInitializable` registered with `ServiceLocator`).
