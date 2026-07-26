# HERO OF ETERNIA — FINAL BOSS ADAPTIVE AI ENGINE

---

## 1. Scope & AI Behavior Mechanics

The **Final Boss AI Engine** (`FinalBossAIEngine.cs`) governs Malakor's tactical combat behavior:

- **Adaptive Phase Shifting**: Automatically shifts phases when remaining phase HP reaches 0.
- **Anti-Exploit Safeguards**: If player remains at extreme range for >5s, triggers instant *Void Step Strike* teleporting boss directly into melee range.
- **Positioning Awareness**: Evaluates player distance to activate melee cleaves or ranged beam storms dynamically.

---

## 2. AI Asset Production Report

### A. VFX Ability Prompts
1. **Singularity Nova Ability Burst (`vfx_singularity_nova_burst`)**
   - **Resolution**: 1024x1024 PNG sprite sheet.
   - **AI Prompt**: `"Game VFX sprite sheet of a massive black hole implosion explosion with purple shockwaves, transparent PNG background"`
   - **Folder Location**: `res://Assets/VFX/Textures/vfx_singularity_nova_burst.png`

---

## 3. Code Reference

Managed by `FinalBossAIEngine.cs` and verified by `Chapter14SystemTests`.
