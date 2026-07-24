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
    // SWIM STATE — Framework stub
    // ===================================================================
    public class SwimState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Swimming;
        public void Enter(PlayerRoot p) { p?.Animation?.Play(PlayerAnimationController.ANIM_SWIM); }

        public PlayerStateId? Update(PlayerRoot p, double delta) { return null; }
        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // CLIMB STATE — Framework stub
    // ===================================================================
    public class ClimbState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Climbing;
        public void Enter(PlayerRoot p) { p?.Animation?.Play(PlayerAnimationController.ANIM_CLIMB); }

        public PlayerStateId? Update(PlayerRoot p, double delta) { return null; }
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
            p?.Animation?.Play(PlayerAnimationController.ANIM_IDLE);
            Core.Logger.Info("PlayerStateMachine: Player has died.");
        }
        public PlayerStateId? Update(PlayerRoot p, double delta) { return null; }
        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // FROZEN STATE (CC — stun, freeze, cutscene)
    // ===================================================================
    public class FrozenState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Frozen;
        public void Enter(PlayerRoot p) { }
        public PlayerStateId? Update(PlayerRoot p, double delta) { return null; }
        public void Exit(PlayerRoot p) { }
    }
}
