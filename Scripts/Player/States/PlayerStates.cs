using Godot;

namespace HeroOfEternia.Player.States
{
    // ===================================================================
    // IDLE STATE
    // ===================================================================
    public class IdleState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Idle;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_IDLE);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;

            if (!p.IsOnFloor())                    return PlayerStateId.Falling;
            if (inp.Jump && p.Data.HasStamina(p.Data.JumpStaminaCost))
                                                   return PlayerStateId.Jumping;
            if (inp.Roll && p.Data.HasStamina(p.Data.RollStaminaCost))
                                                   return PlayerStateId.Rolling;

            float speed = inp.MoveAxis.Length();
            if (speed > 0.1f)
            {
                return inp.Sprint ? PlayerStateId.Sprinting : PlayerStateId.Running;
            }
            return null; // Stay idle
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // WALK STATE
    // ===================================================================
    public class WalkState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Walking;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_WALK);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            if (!p.IsOnFloor())                    return PlayerStateId.Falling;
            if (inp.Jump && p.Data.HasStamina(p.Data.JumpStaminaCost))
                                                   return PlayerStateId.Jumping;
            if (inp.MoveAxis.Length() < 0.1f)      return PlayerStateId.Idle;
            if (!inp.Walk && !inp.Sprint)           return PlayerStateId.Running;
            if (inp.Sprint)                         return PlayerStateId.Sprinting;

            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.WalkSpeed);
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // RUN STATE
    // ===================================================================
    public class RunState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Running;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_RUN);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            if (!p.IsOnFloor())                    return PlayerStateId.Falling;
            if (inp.Jump && p.Data.HasStamina(p.Data.JumpStaminaCost))
                                                   return PlayerStateId.Jumping;
            if (inp.Roll && p.Data.HasStamina(p.Data.RollStaminaCost))
                                                   return PlayerStateId.Rolling;
            if (inp.MoveAxis.Length() < 0.1f)      return PlayerStateId.Idle;
            if (inp.Walk)                          return PlayerStateId.Walking;
            if (inp.Sprint)                        return PlayerStateId.Sprinting;

            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.RunSpeed);
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // SPRINT STATE
    // ===================================================================
    public class SprintState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Sprinting;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_SPRINT);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;

            // Drain stamina while sprinting
            p.Data.DrainStamina(p.Data.SprintStaminaCost * (float)delta);

            if (!p.IsOnFloor())                    return PlayerStateId.Falling;
            if (inp.Jump && p.Data.HasStamina(p.Data.JumpStaminaCost))
                                                   return PlayerStateId.Jumping;
            if (inp.Roll && p.Data.HasStamina(p.Data.RollStaminaCost))
                                                   return PlayerStateId.Rolling;
            if (inp.MoveAxis.Length() < 0.1f)      return PlayerStateId.Idle;
            if (!inp.Sprint || p.Data.CurrentStamina <= 0f)
                                                   return PlayerStateId.Running;

            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.SprintSpeed);
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // JUMP STATE
    // ===================================================================
    public class JumpState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Jumping;

        public void Enter(PlayerRoot p)
        {
            p?.Data?.DrainStamina(p.Data.JumpStaminaCost);
            p?.Movement?.ApplyJump(p);
            p?.Animation?.Play(PlayerAnimationController.ANIM_JUMP);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            p.Movement.ApplyAirMovement(p, (float)delta);

            if (p.Velocity.Y < 0f) return PlayerStateId.Falling;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // FALL STATE
    // ===================================================================
    public class FallState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Falling;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_FALL);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            p.Movement.ApplyAirMovement(p, (float)delta);
            if (p.IsOnFloor()) return PlayerStateId.Landing;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // LAND STATE (brief recovery frame)
    // ===================================================================
    public class LandState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Landing;
        private float _timer;
        private const float LandDuration = 0.15f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_LAND);
            p?.Camera?.AddTrauma(0.15f);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            if (_timer >= LandDuration) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // ROLL STATE (dodge roll — brief invincibility window in future phase)
    // ===================================================================
    public class RollState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Rolling;
        private float _timer;
        private const float RollDuration = 0.55f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Data?.DrainStamina(p.Data.RollStaminaCost);
            p?.Animation?.Play(PlayerAnimationController.ANIM_ROLL);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.RollSpeed);
            if (_timer >= RollDuration) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // CROUCHING STATE
    // ===================================================================
    public class CrouchingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Crouching;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_CROUCH);
            
            // Try to shrink CollisionShape3D height to represent crouching profile
            var col = p?.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
            if (col != null && col.Shape is CapsuleShape3D capsule)
            {
                capsule.Height = 1.0f;
            }
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            if (!p.IsOnFloor()) return PlayerStateId.Falling;

            // Toggle crouch off
            if (!inp.Walk) // We map walk button as crouch toggle/hold in crouch FSM checks
            {
                return PlayerStateId.Idle;
            }

            if (inp.Jump) return PlayerStateId.Jumping;

            float speed = inp.MoveAxis.Length();
            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.CrouchSpeed);
            
            if (speed < 0.1f && p.Velocity.Length() < 0.1f)
            {
                p.Animation?.Play(PlayerAnimationController.ANIM_CROUCH);
            }
            else
            {
                p.Animation?.Play(PlayerAnimationController.ANIM_CROUCH); // Or crouch_walk in future
            }

            return null;
        }

        public void Exit(PlayerRoot p)
        {
            // Restore normal capsule height
            var col = p?.GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
            if (col != null && col.Shape is CapsuleShape3D capsule)
            {
                capsule.Height = 1.8f;
            }
        }
    }

    // ===================================================================
    // SWIM STATE
    // ===================================================================
    public class SwimState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Swimming;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_SWIM);
            Core.Logger.Info("Player entered water/swimming state.");
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            
            // Swim movement speed is reduced walk speed
            float swimSpeed = p.Data.WalkSpeed * 0.7f;
            p.Movement.ApplyGroundMovement(p, (float)delta, swimSpeed);

            // Exit swim if jump is pressed near shallow area (simulated exit)
            if (inp.Jump)
            {
                Core.Logger.Info("Player jumped out of water.");
                return PlayerStateId.Idle;
            }

            return null;
        }

        public void Exit(PlayerRoot p)
        {
            Core.Logger.Info("Player exited swimming state.");
        }
    }

    // ===================================================================
    // CLIMB STATE
    // ===================================================================
    public class ClimbState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Climbing;

        public void Enter(PlayerRoot p)
        {
            p.Animation.Play(PlayerAnimationController.ANIM_CLIMB);
            var vel = p.Velocity;
            vel.Y = 0f;
            p.Velocity = vel;
            Core.Logger.Info("Player entered climbing state.");
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;

            // Move vertically along climbing axis (mapped to vertical move inputs)
            Vector3 vel = p.Velocity;
            float climbSpeed = 2.5f;

            if (Math.Abs(inp.MoveAxis.Y) > 0.1f)
            {
                vel.Y = -inp.MoveAxis.Y * climbSpeed; // Y input pushes forward/backward
                p.Animation?.Play(PlayerAnimationController.ANIM_CLIMB);
            }
            else
            {
                vel.Y = 0f;
                p.Animation?.Pause();
            }

            p.Velocity = vel;
            p.MoveAndSlide();

            // Exit climbing if jump is pressed
            if (inp.Jump)
            {
                return PlayerStateId.Falling;
            }

            // Return to ground check
            if (p.IsOnFloor() && inp.MoveAxis.Y > 0.1f)
            {
                return PlayerStateId.Idle;
            }

            return null;
        }

        public void Exit(PlayerRoot p)
        {
            p?.Animation?.Resume();
            Core.Logger.Info("Player exited climbing state.");
        }
    }

    // ===================================================================
    // TURN LEFT STATE
    // ===================================================================
    public class TurnLeftState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.TurnLeft;
        private float _timer = 0f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_TURN_LEFT);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            if (_timer >= 0.5f) return PlayerStateId.Idle;

            // Turn player left in place
            var rot = p.GlobalRotation;
            rot.Y += (float)delta * Mathf.Pi;
            p.GlobalRotation = rot;

            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // TURN RIGHT STATE
    // ===================================================================
    public class TurnRightState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.TurnRight;
        private float _timer = 0f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_TURN_RIGHT);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            if (_timer >= 0.5f) return PlayerStateId.Idle;

            // Turn player right in place
            var rot = p.GlobalRotation;
            rot.Y -= (float)delta * Mathf.Pi;
            p.GlobalRotation = rot;

            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // LOOKING AROUND STATE
    // ===================================================================
    public class LookingAroundState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.LookingAround;
        private float _timer = 0f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_LOOK_AROUND);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            var inp = p.Input.Current;

            if (inp.MoveAxis.Length() > 0.1f) return PlayerStateId.Running;
            if (_timer >= 2.0f) return PlayerStateId.Idle;

            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // PUSHING STATE
    // ===================================================================
    public class PushingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Pushing;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_PUSH);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            if (inp.MoveAxis.Length() < 0.1f) return PlayerStateId.Idle;

            // Push forward
            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.WalkSpeed * 0.5f);
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // PULLING STATE
    // ===================================================================
    public class PullingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Pulling;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_PULL);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            if (inp.MoveAxis.Length() < 0.1f) return PlayerStateId.Idle;

            // Pull backward
            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.WalkSpeed * 0.5f);
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // INTERACTING STATE (locks movement during one-shot trigger)
    // ===================================================================
    public class InteractingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Interacting;
        private float _timer = 0f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p.Animation.PlayOnce(PlayerAnimationController.ANIM_INTERACT);
            
            // Stop movement
            var vel = p.Velocity;
            vel.X = 0f;
            vel.Z = 0f;
            p.Velocity = vel;
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            if (_timer >= 0.5f) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // SLEEPING STATE
    // ===================================================================
    public class SleepingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Sleeping;

        public void Enter(PlayerRoot p)
        {
            p.Animation.Play(PlayerAnimationController.ANIM_SLEEP);
            p.Velocity = Vector3.Zero;
            Core.Logger.Info("Player went to sleep.");
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            // Any movement input wakes up the player
            if (inp.MoveAxis.Length() > 0.1f || inp.Jump || inp.Roll)
            {
                return PlayerStateId.Idle;
            }
            return null;
        }

        public void Exit(PlayerRoot p)
        {
            Core.Logger.Info("Player woke up.");
        }
    }

    // ===================================================================
    // SITTING STATE
    // ===================================================================
    public class SittingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Sitting;

        public void Enter(PlayerRoot p)
        {
            p.Animation.Play(PlayerAnimationController.ANIM_SIT);
            p.Velocity = Vector3.Zero;
            Core.Logger.Info("Player sat down.");
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            // Standing up triggered by movement or jump
            if (inp.MoveAxis.Length() > 0.1f || inp.Jump)
            {
                return PlayerStateId.Idle;
            }
            return null;
        }

        public void Exit(PlayerRoot p)
        {
            Core.Logger.Info("Player stood up.");
        }
    }

    // ===================================================================
    // CELEBRATING STATE
    // ===================================================================
    public class CelebratingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Celebrating;
        private float _timer = 0f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p.Animation.Play(PlayerAnimationController.ANIM_CELEBRATE);
            p.Velocity = Vector3.Zero;
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            if (_timer >= 1.5f) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // DEAD STATE
    // ===================================================================
    public class DeadState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Dead;

        public void Enter(PlayerRoot p)
        {
            p.Animation.Play(PlayerAnimationController.ANIM_DEAD);
            p.Velocity = Vector3.Zero;
            Core.Logger.Info("PlayerStateMachine: Player has died.");
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            // If respawn triggered externally, we can transition
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // RESPAWN STATE
    // ===================================================================
    public class RespawnState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Respawn;
        private float _timer = 0f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p.Animation.Play(PlayerAnimationController.ANIM_RESPAWN);
            p.Velocity = Vector3.Zero;
            
            // Full heal vitals
            if (p != null)
            {
                p.Data.CurrentHealth = p.Data.MaxHealth;
                p.Data.CurrentMana = p.Data.MaxMana;
                p.Data.CurrentStamina = p.Data.MaxStamina;
            }
            Core.Logger.Info("Player respawned.");
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            if (_timer >= 1.0f) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // FROZEN STATE (stun, freeze, cutscene)
    // ===================================================================
    public class FrozenState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Frozen;
        public void Enter(PlayerRoot p) 
        {
            p.Velocity = Vector3.Zero;
            p?.Animation?.Pause();
        }
        public PlayerStateId? Update(PlayerRoot p, double delta) { return null; }
        public void Exit(PlayerRoot p) 
        {
            p?.Animation?.Resume();
        }
    }

    // ===================================================================
    // DISABLED STATE
    // ===================================================================
    public class DisabledState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Disabled;
        public void Enter(PlayerRoot p)
        {
            p.Velocity = Vector3.Zero;
            p?.Animation?.Pause();
        }
        public PlayerStateId? Update(PlayerRoot p, double delta) { return null; }
        public void Exit(PlayerRoot p)
        {
            p?.Animation?.Resume();
        }
    }
}
