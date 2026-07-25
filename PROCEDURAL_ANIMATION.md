# PROCEDURAL ANIMATION SPECIFICATION — HERO OF ETERNIA (PROMPT 22)

## Overview
`ProceduralAnimationEngine.cs` manages real-time procedural posture adjustments, breathing motion, head look-at tracking, and weapon movement sway.

## Features
* **Head Look-At**: Smoothly turns character head towards interactive NPCs or interest targets within a configurable max angle (75 degrees).
* **Idle Breathing**: Sine-wave additive displacement over character spine bones.
* **Weapon Sway**: Velocity-based offset applied to hands during locomotion to eliminate stiff weapon posture.
* **Aim Adjustment**: Vertical pitch adjustment for ranged abilities and archery.
