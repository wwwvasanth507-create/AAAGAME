using Godot;

namespace HeroOfEternia.Player.States
{
    // ===================================================================
    // ATTACK STATE (Light Attack)
    // ===================================================================
    public class AttackState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Attack;
        private float _timer;
        private const float AttackDuration = 0.35f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_ATTACK);
            // Stamina drain
            p?.Data?.DrainStamina(15f);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.Speed * 0.3f); // decelerate/lunge
            if (_timer >= AttackDuration) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // HEAVY ATTACK STATE
    // ===================================================================
    public class HeavyAttackState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.HeavyAttack;
        private float _timer;
        private const float AttackDuration = 0.6f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_HEAVY_ATTACK);
            p?.Data?.DrainStamina(30f);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.Speed * 0.1f);
            if (_timer >= AttackDuration) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // CASTING STATE
    // ===================================================================
    public class CastingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Casting;
        private float _timer;
        private const float CastDuration = 0.5f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_CAST);
            p?.Data?.DrainStamina(10f);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, 0f); // stationary while casting
            if (_timer >= CastDuration) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // BLOCKING STATE
    // ===================================================================
    public class BlockingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Blocking;

        public void Enter(PlayerRoot p)
        {
            p?.Animation?.Play(PlayerAnimationController.ANIM_BLOCK);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            var inp = p.Input.Current;
            // Drain stamina slowly while holding block
            p?.Data?.DrainStamina(5f * (float)delta);

            if (inp == null || p?.Data == null || !inp.Block || p.Data.CurrentStamina <= 0f)
            {
                return PlayerStateId.Idle;
            }

            // Allow slow walking while blocking
            p.Movement.ApplyGroundMovement(p, (float)delta, p.Data.Speed * 0.4f);
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // PARRYING STATE
    // ===================================================================
    public class ParryingState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Parrying;
        private float _timer;
        private const float ParryDuration = 0.25f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_PARRY);
            p?.Data?.DrainStamina(10f);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, 0f);
            if (_timer >= ParryDuration)
            {
                // Go to blocking if block is held, else idle
                return p.Input.Current.Block ? PlayerStateId.Blocking : PlayerStateId.Idle;
            }
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // HIT REACTION STATE
    // ===================================================================
    public class HitReactionState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.HitReaction;
        private float _timer;
        private const float ReactionDuration = 0.3f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_HIT_REACTION);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, 0f); // Stunned in place
            if (_timer >= ReactionDuration) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // KNOCKDOWN STATE
    // ===================================================================
    public class KnockdownState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Knockdown;
        private float _timer;
        private const float KnockdownDuration = 1.2f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_KNOCKDOWN);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, 0f);
            if (_timer >= KnockdownDuration) return PlayerStateId.Recovery;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }

    // ===================================================================
    // RECOVERY STATE
    // ===================================================================
    public class RecoveryState : IPlayerState
    {
        public PlayerStateId Id => PlayerStateId.Recovery;
        private float _timer;
        private const float RecoveryDuration = 0.6f;

        public void Enter(PlayerRoot p)
        {
            _timer = 0f;
            p?.Animation?.Play(PlayerAnimationController.ANIM_RECOVERY);
        }

        public PlayerStateId? Update(PlayerRoot p, double delta)
        {
            _timer += (float)delta;
            p.Movement.ApplyGroundMovement(p, (float)delta, 0f);
            if (_timer >= RecoveryDuration) return PlayerStateId.Idle;
            return null;
        }

        public void Exit(PlayerRoot p) { }
    }
}
