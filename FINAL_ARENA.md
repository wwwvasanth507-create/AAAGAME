# HERO OF ETERNIA — DYNAMIC FINAL BOSS ARENA FRAMEWORK

---

## 1. Scope & Arena Mechanics

The **Final Boss Arena Manager** (`FinalBossArenaManager.cs`) manages Malakor's Throne Room:

- **Phase 1 Terrain**: White marble floor with solar beam flares (`hazard_sun_flares`).
- **Phase 2 & 3 Terrain**: Floor fractures open, revealing floating obsidian platforms with gravity distortion fields (`hazard_gravity_distortion`).
- **Phase 4 Terrain**: Complete arena collapse into the Void Core singularity chamber with cataclysm pulses (`hazard_unbound_cataclysm`).

---

## 2. AI Asset Production Report

### A. Music & Audio Prompts
1. **Malakor Final Boss Phase 4 Track (`music_boss_malakor_phase4`)**
   - **Specification**: 24-bit 48kHz WAV audio, 4 min 30 sec loopable track.
   - **AI Music Prompt**: `"Climatic fast-tempo orchestral boss battle theme featuring ferocious brass, apocalyptic choir, blasting percussion, and intense violin arpeggios"`
   - **Folder Location**: `res://Assets/Audio/Music/music_boss_malakor_phase4.wav`

---

## 3. Code Reference

Managed by `FinalBossArenaManager.cs` and tested by `Chapter14SystemTests`.
