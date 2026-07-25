# AMBIENT AUDIO SPECIFICATION — HERO OF ETERNIA (PROMPT 21)

## Overview
`AmbientAudioManager.cs` handles spatial environmental ambience, dynamic weather audio blending, and biome zone audio transitions.

## Features
* Dual-player crossfade for seamless biome transitions (`Forest`, `Subterranean Crypt`, `Volcanic Wastes`, `Coastal Town`).
* Master ambient volume gain scalar tied to `AudioCategory.Ambient`.
* ResourceLoader path validation to ensure zero runtime crashes on missing audio files.
