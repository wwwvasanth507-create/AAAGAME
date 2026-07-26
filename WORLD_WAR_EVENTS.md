# HERO OF ETERNIA — DYNAMIC WORLD WAR EVENTS FRAMEWORK

---

## 1. Scope & War Event Types

The **World War Event Manager** (`WorldWarEventManager.cs`) controls dynamic open-world war events across Act IV:

- **Supply Caravan Escorts (`event_caravan_escort`)**: Escort armored war wagons through crystal storm hazards to forward camps.
- **Spire Liberation Skirmishes (`event_caelum_skirmish`)**: Reclaim floating spires from Malakor shadow patrols to earn Alliance Readiness boosts.

---

## 2. AI Asset Production Report

### A. VFX Prompts & Particle Specs
1. **Alliance Signal Flare Burst (`vfx_alliance_signal_flare`)**
   - **Resolution**: 512x512 PNG sprite sheet.
   - **AI Prompt**: `"Game VFX sprite sheet of golden sun flare explosion burst launching skyward with trailing sparks, transparent PNG background"`
   - **Folder Location**: `res://Assets/VFX/Textures/vfx_alliance_signal_flare.png`

---

## 3. Code Reference

Managed by `WorldWarEventManager.cs` and verified by `Chapter12SystemTests`.
