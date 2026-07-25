# ANIMATION EVENTS SPECIFICATION — HERO OF ETERNIA (PROMPT 22)

## Overview
`AnimationEventSystem.cs` handles frame-accurate animation event dispatching to combat, sound, particle, and camera shake systems.

## Event Types
1. **Footstep**: Surface-aware footstep audio and particle emission.
2. **WeaponImpact**: Exact frame when melee hitboxes become active.
3. **AbilityTiming**: Projectile spawn frame / cast release frame.
4. **SoundTrigger**: Audio stream trigger point.
5. **ParticleTrigger**: Visual effect spawn point.
6. **CameraShake**: Hit impact camera impulse trigger.
7. **DamageWindowStart / End**: Melee combo hurtbox timing window.
8. **InteractionWindowStart / End**: Gathering / chest interaction timing.
