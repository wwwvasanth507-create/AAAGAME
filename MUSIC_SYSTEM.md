# MUSIC SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 21)

## Overview
The Adaptive Music Engine (`MusicManager.cs`) manages non-linear track transitions, stem intensity scaling, state crossfading, and biome-specific track defaults for **Hero of Eternia**.

## Music States
* **Exploration**: Peaceful ambient themes played during field exploration and gathering.
* **Settlement**: Warm, acoustic village themes played in towns and safe havens.
* **Combat**: High-intensity battle music triggered upon entering hostiles' aggro ranges.
* **Boss**: Heavy orchestral themes with dynamic phase changes for Titan and Golem fights.
* **Dungeon**: Suspenseful, low-frequency atmospheric tracks for subterranean crypts.
* **Victory / Defeat**: Stingers played on combat outcomes.

## Technical Specifications
* **Crossfade Duration**: 2.0s linear power curve between dual `AudioStreamPlayer` nodes.
* **Format**: 16-bit 44.1kHz OGG / WAV streams.
* **Audio Bus**: `Music` bus.
