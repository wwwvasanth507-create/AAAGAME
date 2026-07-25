# Animation System - Hero of Eternia

This document details the player's animation states, blend tree configurations, skeletal layers, and root motion parameters.

---

## 1. Animation States

The player state machine features 24 fully-implemented states driving the character animations:

| State ID | Animation Name | Description |
|---|---|---|
| `Idle` | `idle` | Standing breath variant. |
| `Walking` | `walk` | Basic locomotive walk. Speed scales pace. |
| `Running` | `run` | Jog speed. |
| `Sprinting` | `sprint` | Fast run, drains stamina. |
| `Jumping` | `jump` | Upward leap impulse. |
| `Falling` | `fall` | Gravity drop. |
| `Landing` | `land` | Soft recovery frames with camera shake. |
| `Rolling` | `roll` | Dodge rolls with speed multipliers. |
| `Crouching` | `crouch` | Capsule height shrinks to 1.0m, uses crouch speed. |
| `Swimming` | `swim` | Water-buoyancy locomotion. |
| `Climbing` | `climb` | Ladder/ledge alignment and vertical climb. |
| `TurnLeft` | `turn_left` | Pivot 90 degrees counter-clockwise in place. |
| `TurnRight` | `turn_right` | Pivot 90 degrees clockwise in place. |
| `LookingAround` | `look_around` | Idle curiosity look around state. |
| `Pushing` | `push` | Heavy locomotive block push. |
| `Pulling` | `pull` | Heavy locomotive block pull. |
| `Interacting` | `interact` | One-shot interaction lock. |
| `Sleeping` | `sleep` | Lie down, disables normal inputs. |
| `Sitting` | `sit` | Chair/ground rest pose. |
| `Celebrating` | `celebrate` | Victory pose. |
| `Dead` | `dead` | Knockdown death pose. |
| `Respawn` | `respawn` | Re-appear flash, restores vitals. |
| `Frozen` | N/A | Total stun, pauses animations. |
| `Disabled` | N/A | Interaction lock, input block. |

---

## 2. Advanced Animation Features

The `PlayerAnimationController` acts as a facade pattern wrapper, checking for a Godot `AnimationTree` and exposing hooks for advanced systems:

### 2.1 Blend Trees
Exposes `SetBlendParameter(path, value)` to smoothly update locomotion blend vectors based on speed parameters.

### 2.2 Skeletal Layering & Filtering
Exposes bone masking options to combine separate upper and lower body tracks:
- **Lower Body:** Controls walking/running/sprinting leg states.
- **Upper Body:** Fuses combat attacks, block poses, and interaction one-shots without overriding the leg movement.

### 2.3 Root Motion Integration
- **Toggle:** `SetRootMotionEnabled(bool)` activates root motion transform translations.
- **Velocity Vector:** `GetRootMotionVelocity()` pulls active delta translation vectors directly from the `AnimationTree` in `_PhysicsProcess` to apply movement.
