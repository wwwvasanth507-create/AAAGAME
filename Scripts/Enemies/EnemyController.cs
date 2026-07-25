using System;
using Godot;
using HeroOfEternia.Combat;
using HeroOfEternia.Core;

namespace HeroOfEternia.Enemies
{
    // ----------------------------------------------------------------
    // Events fired by EnemyController on EventBus
    // ----------------------------------------------------------------
    public record EnemyDiedEvent(string EnemyId, string DisplayName, int XpReward, Godot.Vector3 Position);
    public record EnemyHitEvent(string EnemyId, float DamageDealt, float RemainingHp);
    public record EnemyAttackedPlayerEvent(string EnemyId, float Damage);

    /// <summary>
    /// EnemyController is the top-level Godot CharacterBody3D node for every enemy.
    /// Hosts the EnemyStateMachine, applies movement physics, delegates combat
    /// to CombatManager, and fires events on death / hit / attack.
    /// </summary>
    public partial class EnemyController : CharacterBody3D
    {
        // ----------------------------------------------------------------
        // Exported configuration
        // ----------------------------------------------------------------
        [Export] public string EnemyId   { get; set; } = "goblin_grunt";
        [Export] public int    WaveIndex { get; set; } = 1;

        // ----------------------------------------------------------------
        // Runtime state
        // ----------------------------------------------------------------
        public  EnemyDefinition? Definition  { get; private set; }
        private EnemyStateMachine? _fsm;

        private float _currentHp         = 0f;
        private float _maxHp             = 0f;
        private float _attackCooldown    = 0f;
        private bool  _isDead            = false;

        // Target tracking
        private Node3D? _target;
        private float   _targetDist      = float.MaxValue;

        // Gravity
        private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

        // ----------------------------------------------------------------
        // Lifecycle
        // ----------------------------------------------------------------
        public override void _Ready()
        {
            // Initialise from EnemyDatabase
            var db = new EnemyDatabase();
            db.Load(GetDatabasePath());
            Definition = db.Get(EnemyId);

            if (Definition == null)
            {
                Logger.Error($"EnemyController: Unknown enemy ID '{EnemyId}'. Node freed.");
                QueueFree();
                return;
            }

            var scaled = Definition.GetScaledData(WaveIndex);
            _maxHp     = scaled.MaxHp;
            _currentHp = _maxHp;

            // Build FSM
            _fsm = new EnemyStateMachine(EnemyId);
            _fsm.OnStateChanged += OnFsmStateChanged;

            Logger.Info($"EnemyController: Spawned '{Definition.Data.DisplayName}' HP={_maxHp} Wave={WaveIndex}");
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_isDead || Definition == null || _fsm == null) return;

            float dt = (float)delta;

            // Update attack cooldown
            if (_attackCooldown > 0f) _attackCooldown -= dt;

            // Measure distance to target
            _targetDist = (_target != null && IsInstanceValid(_target))
                ? GlobalPosition.DistanceTo(_target.GlobalPosition)
                : float.MaxValue;

            bool hasLoS = _targetDist < Definition.Data.AggroRange * 1.5f; // simplified LoS

            // Build context and tick FSM
            var ctx = new EnemyContext
            {
                DistanceToTarget   = _targetDist,
                HasLineOfSight     = hasLoS,
                CurrentHp          = _currentHp,
                MaxHp              = _maxHp,
                IsStaggered        = false,
                TargetExists       = _target != null && IsInstanceValid(_target),
                AggroRange         = Definition.Data.AggroRange,
                AttackRange        = Definition.Data.AttackRange,
                AttackCooldownLeft = _attackCooldown,
                Behaviour          = Definition.Data.Behaviour
            };

            _fsm.Tick(ctx, dt);

            // Apply movement and actions based on FSM state
            ApplyStateBehaviour(dt);

            // Apply gravity
            if (!IsOnFloor())
                Velocity = new Godot.Vector3(Velocity.X, Velocity.Y - _gravity * dt, Velocity.Z);

            MoveAndSlide();
        }

        // ----------------------------------------------------------------
        // Public: Receive damage
        // ----------------------------------------------------------------
        public void TakeDamage(float amount, string sourceId = "player")
        {
            if (_isDead || Definition == null) return;

            float mitigated = MathF.Max(0f, amount - Definition.Data.Defense);
            _currentHp -= mitigated;
            _currentHp  = MathF.Max(0f, _currentHp);

            Logger.Info($"EnemyController[{EnemyId}]: Took {mitigated:F1} damage. HP={_currentHp}/{_maxHp}");
            EventBus.Publish(new EnemyHitEvent(EnemyId, mitigated, _currentHp));

            if (_currentHp <= 0f) Die();
        }

        // ----------------------------------------------------------------
        // Public: Assign a target (player node)
        // ----------------------------------------------------------------
        public void SetTarget(Node3D? target) => _target = target;

        // ----------------------------------------------------------------
        // Behaviour execution per FSM state
        // ----------------------------------------------------------------
        private void ApplyStateBehaviour(float delta)
        {
            if (_fsm == null || Definition == null) return;
            var scaled = Definition.GetScaledData(WaveIndex);

            switch (_fsm.Current)
            {
                case EnemyState.Chase:
                case EnemyState.Patrol:
                    MoveTowardTarget(scaled.MoveSpeed, delta);
                    break;

                case EnemyState.Attack:
                    PerformAttack(scaled.AttackDamage, scaled.AttackCooldown);
                    break;

                case EnemyState.Retreat:
                    MoveAwayFromTarget(scaled.MoveSpeed * 0.8f, delta);
                    break;

                case EnemyState.Idle:
                case EnemyState.Alert:
                case EnemyState.Stagger:
                default:
                    Velocity = new Godot.Vector3(0, Velocity.Y, 0);
                    break;
            }
        }

        private void MoveTowardTarget(float speed, float delta)
        {
            if (_target == null || !IsInstanceValid(_target)) return;
            var dir = ((_target.GlobalPosition - GlobalPosition) with { Y = 0 }).Normalized();
            Velocity = new Godot.Vector3(dir.X * speed, Velocity.Y, dir.Z * speed);

            // Face target
            if (dir.Length() > 0.01f)
                Rotation = Rotation with { Y = MathF.Atan2(dir.X, dir.Z) };
        }

        private void MoveAwayFromTarget(float speed, float delta)
        {
            if (_target == null || !IsInstanceValid(_target)) return;
            var dir = ((GlobalPosition - _target.GlobalPosition) with { Y = 0 }).Normalized();
            Velocity = new Godot.Vector3(dir.X * speed, Velocity.Y, dir.Z * speed);
        }

        private void PerformAttack(float damage, float cooldown)
        {
            if (_attackCooldown > 0f) return;
            _attackCooldown = cooldown;
            Logger.Info($"EnemyController[{EnemyId}]: Attacking for {damage} damage.");
            EventBus.Publish(new EnemyAttackedPlayerEvent(EnemyId, damage));
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;
            _fsm?.ForceState(EnemyState.Dead);

            int xp = Definition?.Data.XpReward ?? 0;
            string name = Definition?.Data.DisplayName ?? EnemyId;

            Logger.Info($"EnemyController[{EnemyId}]: Died. Awarding {xp} XP.");
            EventBus.Publish(new EnemyDiedEvent(EnemyId, name, xp, GlobalPosition));

            // Deferred free — let animations play before removal
            CallDeferred(Node.MethodName.QueueFree);
        }

        private void OnFsmStateChanged(string id, EnemyState from, EnemyState to)
        {
            // Hook for animation controller — future AnimationPlayer integration
            // EventBus.Publish(new EnemyStateChangedEvent(id, from, to));
        }

        private static string GetDatabasePath()
        {
            // Resolve to user settings dir at runtime
            return System.IO.Path.Combine(
                Godot.OS.GetUserDataDir(), "..", "..", "AAA", "Settings");
        }
    }
}
