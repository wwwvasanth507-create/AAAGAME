# LIGHTING SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 23)

## System Overview
`LightingManager.cs` and `LightingProfile.cs` manage environmental lighting, sunlight energy, ambient lighting, and fog density across 10 context presets.

## Lighting Contexts
* **Morning / Day / Evening / Night**: Smooth time-of-day solar transition profiles.
* **Dungeon / Indoor / Settlement**: Environment override profiles for indoor and subterranean areas.
* **Storm / Fog / BossArena**: Dynamic weather and combat atmosphere profiles.

## Smooth Interpolation
Profiles blend using `LightingProfile.Lerp(a, b, t)` over a configurable duration (default 2.0s) to prevent jarring lighting pops.
