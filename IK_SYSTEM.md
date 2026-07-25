# IK SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 22)

## Overview
`IKSystem.cs` provides Inverse Kinematics solving for leg foot placement, hand object interaction, and weapon stance alignment.

## IK Targets
* **LeftFoot & RightFoot**: Raycast-driven ground adaptation to align feet with uneven terrain and stairs.
* **LeftHand & RightHand**: Object interaction IK target for levers, doors, chest opening, and climbing.
* **WeaponAlignment**: Recoil adjustment and 2-handed weapon gripping alignment.

## Configuration
* Can be toggled on/off globally or per-character.
* `GlobalIKWeight` scalar (0.0 to 1.0) smooths IK transitions on mobile hardware.
