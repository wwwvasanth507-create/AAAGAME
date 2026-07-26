# HERO OF ETERNIA — REUSABLE CREDITS SYSTEM ENGINE

---

## 1. Scope & System Mechanics

The **Credits System Manager** (`CreditsSystemManager.cs`) governs post-campaign credits presentation:

- **Data-Driven Contributors**: Credit categories and contributor names stored in modular records (`CreditCategoryRecord`).
- **Scroll Controls**: Adjustable scroll speed, pause/resume, and skip options with full accessibility support.
- **Audio Integration**: Plays `music_credits_theme` during credits playback.

---

## 2. AI Asset Production Report

### A. Music Prompts
1. **Campaign Completion Credits Theme (`music_credits_theme`)**
   - **Specification**: 24-bit 48kHz WAV audio, 3 min 45 sec loopable track.
   - **AI Music Prompt**: `"Emotional inspiring orchestral credits theme starting with piano solo and swelling into full symphonic string harmony"`
   - **Folder Location**: `res://Assets/Audio/Music/music_credits_theme.wav`

---

## 3. Code Reference

Managed by `CreditsSystemManager.cs` and tested by `Chapter15SystemTests`.
