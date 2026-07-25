using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Enemies
{
    // ----------------------------------------------------------------
    // Enemy FSM states
    // ----------------------------------------------------------------
    public enum EnemyState
    {
        Idle,
        Patrol,
        Alert,
        Chase,
        Attack,
        Stagger,
        Retreat,
        Dead
    }

    // ----------------------------------------------------------------
    // Transition context — supplied each tick to decide next state
    // ----------------------------------------------------------------
    public record EnemyContext
    {
        public float  DistanceToTarget   { get; init; }
        public bool   HasLineOfSight     { get; init; }
        public float  CurrentHp          { get; init; }
        public float  MaxHp              { get; init; }
        public bool   IsStaggered        { get; init; }
        public bool   TargetExists       { get; init; }
        public float  AggroRange         { get; init; }
        public float  AttackRange        { get; init; }
        public float  AttackCooldownLeft { get; init; }
        public EnemyBehaviour Behaviour  { get; init; }

        public float HpPercent => MaxHp > 0 ? CurrentHp / MaxHp : 0f;
    }

    // ----------------------------------------------------------------
    // EnemyStateMachine
    // ----------------------------------------------------------------
    /// <summary>
    /// Headless FSM governing all enemy AI behaviour.
    /// Called from EnemyController._Process() with an EnemyContext.
    /// Emits events on state change via EventBus.
    /// </summary>
    public class EnemyStateMachine
    {
        public EnemyState Current { get; private set; } = EnemyState.Idle;

        // Transition event — (enemyId, oldState, newState)
        public event Action<string, EnemyState, EnemyState>? OnStateChanged;

        private readonly string _enemyId;
        private float _staggerTimer = 0f;
        private float _idleTimer    = 0f;
        private const float StaggerDuration = 0.6f;
        private const float AlertToChaseDelay = 1.2f;

        public EnemyStateMachine(string enemyId)
        {
            _enemyId = enemyId;
        }

        // ----------------------------------------------------------------
        // Tick — call every physics frame with latest context
        // ----------------------------------------------------------------
        public void Tick(EnemyContext ctx, float delta)
        {
            EnemyState next = Evaluate(ctx, delta);
            if (next != Current) Transition(next);
        }

        // ----------------------------------------------------------------
        // Forced external transitions (e.g. hit reaction, death)
        // ----------------------------------------------------------------
        public void ForceState(EnemyState state)
        {
            if (state != Current) Transition(state);
        }

        // ----------------------------------------------------------------
        // Core evaluation logic
        // ----------------------------------------------------------------
        private EnemyState Evaluate(EnemyContext ctx, float delta)
        {
            // Dead is terminal
            if (Current == EnemyState.Dead) return EnemyState.Dead;

            // Stagger — wait out the timer
            if (Current == EnemyState.Stagger)
            {
                _staggerTimer -= delta;
                if (ctx.IsStaggered || _staggerTimer > 0f) return EnemyState.Stagger;
                // After stagger: resume chase if target visible, otherwise idle
                return (ctx.TargetExists && ctx.DistanceToTarget <= ctx.AggroRange)
                    ? EnemyState.Chase
                    : EnemyState.Idle;
            }

            // Death threshold
            if (ctx.CurrentHp <= 0f) return EnemyState.Dead;

            // Retreat at <20% HP for non-Aggressive enemies
            if (ctx.HpPercent < 0.2f && ctx.Behaviour == EnemyBehaviour.Patrol)
                return EnemyState.Retreat;

            // No valid target
            if (!ctx.TargetExists)
            {
                return ctx.Behaviour == EnemyBehaviour.Patrol
                    ? EnemyState.Patrol
                    : EnemyState.Idle;
            }

            // In attack range and cooldown ready — attack
            if (ctx.DistanceToTarget <= ctx.AttackRange && ctx.AttackCooldownLeft <= 0f)
                return EnemyState.Attack;

            // Within aggro range and has LoS — chase
            if (ctx.DistanceToTarget <= ctx.AggroRange && ctx.HasLineOfSight)
                return EnemyState.Chase;

            // Lost sight but was chasing — alert briefly
            if (Current == EnemyState.Chase && !ctx.HasLineOfSight)
            {
                _idleTimer += delta;
                if (_idleTimer < AlertToChaseDelay) return EnemyState.Alert;
                _idleTimer = 0f;
            }

            // Patrol or idle based on behaviour
            return ctx.Behaviour == EnemyBehaviour.Patrol
                ? EnemyState.Patrol
                : EnemyState.Idle;
        }

        // ----------------------------------------------------------------
        // Apply transition
        // ----------------------------------------------------------------
        private void Transition(EnemyState next)
        {
            var prev = Current;
            Current = next;

            // Reset state-specific timers on entry
            if (next == EnemyState.Stagger) _staggerTimer = StaggerDuration;
            if (next == EnemyState.Idle)    _idleTimer    = 0f;

            Logger.Info($"EnemyFSM[{_enemyId}]: {prev} → {next}");
            OnStateChanged?.Invoke(_enemyId, prev, next);
        }

        // ----------------------------------------------------------------
        // Convenience helpers
        // ----------------------------------------------------------------
        public bool IsAlive          => Current != EnemyState.Dead;
        public bool IsEngaged        => Current == EnemyState.Chase || Current == EnemyState.Attack;
        public bool IsAttacking      => Current == EnemyState.Attack;
        public bool CanBeInterrupted => Current != EnemyState.Dead && Current != EnemyState.Stagger;

        public override string ToString() => $"EnemyFSM[{_enemyId}] State={Current}";
    }
}
