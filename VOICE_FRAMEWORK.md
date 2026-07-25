# VOICE FRAMEWORK SPECIFICATION — HERO OF ETERNIA (PROMPT 21)

## Overview
`VoiceFramework.cs` provides dialogue barks, NPC combat barks, quest speech synchronization, and automatic subtitle generation.

## Subtitle Configuration
* **Speaker Color Hashing**: Automatically calculates unique HSV color highlights per speaker name.
* **Duration Timer**: Auto-clears subtitles after configurable display durations.
* **Event Dispatcher**: Fires `OnSubtitleTriggered` and `OnSubtitleCleared` for UI rendering on HUD subtitle widgets.
