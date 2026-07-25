using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Input;
using HeroOfEternia.Player.States;
using HeroOfEternia.Camera;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// PlayerRoot is the top-level CharacterBody3D node for the player character.
    ///
    /// Architecture — modules are separate classes, owned by PlayerRoot:
    ///   Data        — PlayerData (stats, vitals, XP)
    ///   Movement    — PlayerMovement (velocity, physics)
    ///   StateMachine— PlayerStateMachine (state transitions)
    ///   Animation   — PlayerAnimationController (AnimationPlayer wrapper)
    ///   Audio       — PlayerAudioController (footsteps, SFX)
    ///   Settings    — PlayerSettings (per-player preferences)
    ///   Input       — InputHandler (shared scene node)
    ///   Camera      — CameraController (scene node reference)
    ///
    /// PlayerRoot initialises all modules, registers states, and delegates
    /// per-frame work to the state machine. External systems (inventory, combat,
    /// quests) interact only through Data and EventBus — never through this class.
    /// </summary>
    public partial class PlayerRoot : CharacterBody3D
    {
        // ---------------------------------------------------------------
        // MODULE REFERENCES — injected in _Ready
        // ---------------------------------------------------------------
        public PlayerData                 Data      { get; private set; } = null!;
        public PlayerMovement             Movement  { get; private set; } = null!;
        public PlayerStateMachine         FSM       { get; private set; } = null!;
        public PlayerModelController      Model               { get; private set; } = null!;
        public PlayerInteractionDetector  InteractionDetector { get; private set; } = null!;
        public PlayerAnimationController  Animation           { get; private set; } = null!;
        public PlayerEffectsController    Effects             { get; private set; } = null!;
        public PlayerAudioController      Audio     { get; private set; } = null!;
        public PlayerSettings             Settings  { get; private set; } = null!;
        public InputHandler               Input     { get; private set; } = null!;
        public CameraController?          Camera    { get; private set; }

        // ---------------------------------------------------------------
        // PHYSICS CONSTANTS
        // ---------------------------------------------------------------
        private const float MaxFallSpeed = 30f;

        // ---------------------------------------------------------------
        // LIFECYCLE
        // ---------------------------------------------------------------

        public override void _Ready()
        {
            // --- Load player settings ---
            Settings = new PlayerSettings();
            Settings.Load();

            // --- Initialise data ---
            Data = new PlayerData();

            // --- Initialise movement ---
            Movement = new PlayerMovement();

            // --- Find scene-level InputHandler ---
            Input = GetTree().Root.FindChild("InputHandler", true, false) as InputHandler
                 ?? GetParent().GetNodeOrNull<InputHandler>("InputHandler")
                 ?? AddDefaultInputHandler();

            // --- Find camera ---
            Camera = GetTree().Root.FindChild("CameraController", true, false) as CameraController;
            if (Camera != null)
            {
                Camera.SetSensitivity(Settings.Data.CameraSensitivity);
                Camera.SetInvertY(Settings.Data.InvertY);
            }

            // --- Initialise module child nodes ---
            Model = GetNodeOrNull<PlayerModelController>("PlayerModelController")
                 ?? CreateChildModule<PlayerModelController>("PlayerModelController");

            InteractionDetector = GetNodeOrNull<PlayerInteractionDetector>("PlayerInteractionDetector")
                               ?? CreateChildModule<PlayerInteractionDetector>("PlayerInteractionDetector");

            Animation = GetNodeOrNull<PlayerAnimationController>("PlayerAnimationController")
                     ?? CreateChildModule<PlayerAnimationController>("PlayerAnimationController");

            Effects = GetNodeOrNull<PlayerEffectsController>("PlayerEffectsController")
                   ?? CreateChildModule<PlayerEffectsController>("PlayerEffectsController");

            Audio     = GetNodeOrNull<PlayerAudioController>("PlayerAudioController")
                     ?? CreateChildModule<PlayerAudioController>("PlayerAudioController");

            // --- Register all player states into FSM ---
            FSM = new PlayerStateMachine();
            FSM.Register(new IdleState());
            FSM.Register(new WalkState());
            FSM.Register(new RunState());
            FSM.Register(new SprintState());
            FSM.Register(new JumpState());
            FSM.Register(new FallState());
            FSM.Register(new LandState());
            FSM.Register(new RollState());
            FSM.Register(new CrouchingState());
            FSM.Register(new SwimState());
            FSM.Register(new ClimbState());
            FSM.Register(new TurnLeftState());
            FSM.Register(new TurnRightState());
            FSM.Register(new LookingAroundState());
            FSM.Register(new PushingState());
            FSM.Register(new PullingState());
            FSM.Register(new InteractingState());
            FSM.Register(new SleepingState());
            FSM.Register(new SittingState());
            FSM.Register(new CelebratingState());
            FSM.Register(new DeadState());
            FSM.Register(new RespawnState());
            FSM.Register(new FrozenState());
            FSM.Register(new DisabledState());

            // Combat States
            FSM.Register(new AttackState());
            FSM.Register(new HeavyAttackState());
            FSM.Register(new CastingState());
            FSM.Register(new BlockingState());
            FSM.Register(new ParryingState());
            FSM.Register(new HitReactionState());
            FSM.Register(new KnockdownState());
            FSM.Register(new RecoveryState());

            FSM.OnStateChanged += OnStateChanged;
            FSM.Start(this, PlayerStateId.Idle);

            Logger.Info("PlayerRoot: Fully initialised.");
        }

        public override void _PhysicsProcess(double delta)
        {
            // Regen vitals over time
            Data.RegenVitals((float)delta);
            Data.Attributes.Update((float)delta);

            // Clamp fall speed to prevent tunnelling
            var vel = Velocity;
            vel.Y = Mathf.Max(vel.Y, -MaxFallSpeed);
            Velocity = vel;

            // Delegate per-frame logic to the active state
            FSM.Update(this, delta);

            // Drive audio footstep pacing from current speed
            float speed = new Godot.Vector2(Velocity.X, Velocity.Z).Length();
            float maxSpeed = Data.SprintSpeed;
            Audio.SetMovementPace(speed / maxSpeed);
        }

        // ---------------------------------------------------------------
        // PUBLIC API (called by UI or save system)
        // ---------------------------------------------------------------

        /// <summary>Kill the player, trigger death state and notify EventBus.</summary>
        public void Kill()
        {
            FSM.ForceTransition(this, PlayerStateId.Dead);
            EventBus.Publish(new PlayerDiedEvent { Player = this });
        }

        /// <summary>Freeze input and animation (cutscenes, dialogues).</summary>
        public void Freeze()  => FSM.ForceTransition(this, PlayerStateId.Frozen);

        /// <summary>Resume normal state from frozen.</summary>
        public void Unfreeze() => FSM.ForceTransition(this, PlayerStateId.Idle);

        /// <summary>Apply damage to the player (combat system hook).</summary>
        public void TakeDamage(float amount)
        {
            Data.CurrentHealth = System.Math.Max(0f, Data.CurrentHealth - amount);
            Camera?.AddTrauma(0.25f);
            Logger.Info($"PlayerRoot: Took {amount} damage. HP={Data.CurrentHealth}/{Data.MaxHealth}");

            if (Data.CurrentHealth <= 0f) Kill();
        }

        // ---------------------------------------------------------------
        // EVENT HANDLERS
        // ---------------------------------------------------------------

        private void OnStateChanged(PlayerStateId from, PlayerStateId to)
        {
            // Adjust footstep audio pacing per state
            Audio.SetEnabled(to is PlayerStateId.Walking
                               or PlayerStateId.Running
                               or PlayerStateId.Sprinting);

            // Notify the rest of the game (UI health bar, etc.)
            EventBus.Publish(new PlayerStateChangedEvent { NewState = to });
        }

        // ---------------------------------------------------------------
        // PRIVATE HELPERS
        // ---------------------------------------------------------------

        private T CreateChildModule<T>(string nodeName) where T : Node, new()
        {
            var node = new T { Name = nodeName };
            AddChild(node);
            return node;
        }

        private InputHandler AddDefaultInputHandler()
        {
            var handler = new InputHandler { Name = "InputHandler" };
            AddChild(handler);
            Logger.Warning("PlayerRoot: InputHandler not found in scene. Created fallback child node.");
            return handler;
        }
    }
}
