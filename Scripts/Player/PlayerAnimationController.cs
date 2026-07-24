using Godot;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// PlayerAnimationController wraps an AnimationPlayer and exposes named
    /// animation constants. States call Play() — they never hardcode strings.
    /// Supports blend times for smooth cross-fades and animation events via signals.
    /// </summary>
    public partial class PlayerAnimationController : Node
    {
        // ---------------------------------------------------------------
        // ANIMATION NAME CONSTANTS — only one place to update strings
        // ---------------------------------------------------------------
        public const string ANIM_IDLE   = "idle";
        public const string ANIM_WALK   = "walk";
        public const string ANIM_RUN    = "run";
        public const string ANIM_SPRINT = "sprint";
        public const string ANIM_JUMP   = "jump";
        public const string ANIM_FALL   = "fall";
        public const string ANIM_LAND   = "land";
        public const string ANIM_ROLL   = "roll";
        public const string ANIM_SWIM   = "swim";
        public const string ANIM_CLIMB  = "climb";
        public const string ANIM_CROUCH = "crouch";
        public const string ANIM_DEAD   = "dead";

        // ---------------------------------------------------------------
        // BLEND TIMES (seconds) — tune for feel
        // ---------------------------------------------------------------
        private const float BLEND_DEFAULT  = 0.15f;
        private const float BLEND_LAND     = 0.05f;
        private const float BLEND_ROLL     = 0.08f;
        private const float BLEND_JUMP     = 0.08f;

        private AnimationPlayer? _animPlayer;
        private string _currentAnim = "";

        // ---------------------------------------------------------------
        // LIFECYCLE
        // ---------------------------------------------------------------

        public override void _Ready()
        {
            // Expect AnimationPlayer as sibling under Model node
            _animPlayer = GetParent().FindChild("AnimationPlayer", true, false) as AnimationPlayer;

            if (_animPlayer == null)
            {
                Core.Logger.Warning("PlayerAnimationController: AnimationPlayer not found. " +
                                    "Animations will be no-ops until a model is attached.");
            }
            else
            {
                // Wire up Godot animation events (called from animation tracks)
                _animPlayer.AnimationFinished += OnAnimationFinished;
                Core.Logger.Info("PlayerAnimationController: Ready. AnimationPlayer found.");
            }
        }

        // ---------------------------------------------------------------
        // PUBLIC API
        // ---------------------------------------------------------------

        /// <summary>Cross-fade to a new animation with automatic blend time selection.</summary>
        public void Play(string animName)
        {
            if (_animPlayer == null || _currentAnim == animName) return;

            float blend = animName switch
            {
                ANIM_LAND  => BLEND_LAND,
                ANIM_ROLL  => BLEND_ROLL,
                ANIM_JUMP  => BLEND_JUMP,
                _          => BLEND_DEFAULT,
            };

            _animPlayer.Play(animName, blend);
            _currentAnim = animName;
        }

        /// <summary>Play a one-shot animation then return to idle.</summary>
        public void PlayOnce(string animName)
        {
            if (_animPlayer == null) return;
            _animPlayer.Play(animName, BLEND_DEFAULT);
            _currentAnim = animName;
        }

        /// <summary>Pause the current animation (cutscene freeze).</summary>
        public void Pause() => _animPlayer?.Pause();

        /// <summary>Resume a paused animation.</summary>
        public void Resume()
        {
            if (_animPlayer != null) _animPlayer.Play();
        }

        /// <summary>Current playback position in the active animation (0..1).</summary>
        public float Progress =>
            _animPlayer != null
            ? (float)(_animPlayer.CurrentAnimationPosition / Mathf.Max(0.001, _animPlayer.CurrentAnimationLength))
            : 0f;

        public string CurrentAnimation => _currentAnim;

        // ---------------------------------------------------------------
        // ANIMATION EVENTS (called from AnimationPlayer signal or track)
        // ---------------------------------------------------------------

        /// <summary>Fires when a one-shot animation completes.</summary>
        private void OnAnimationFinished(StringName animName)
        {
            Core.Logger.Info($"PlayerAnimationController: Animation '{animName}' finished.");
            // States handle transition logic — we only log here.
            // Future: emit an event on EventBus for combo systems.
        }

        /// <summary>
        /// Called from an AnimationPlayer CallMethod track at a footstep frame.
        /// Relays to PlayerAudioController for surface-matched sound.
        /// </summary>
        public void OnFootstepEvent(string foot) =>
            GetParent()
            .FindChild("PlayerAudioController", true, false)?
            .Call("PlayFootstep", foot);
    }
}
