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
        public const string ANIM_IDLE       = "idle";
        public const string ANIM_WALK       = "walk";
        public const string ANIM_RUN        = "run";
        public const string ANIM_SPRINT     = "sprint";
        public const string ANIM_JUMP       = "jump";
        public const string ANIM_FALL       = "fall";
        public const string ANIM_LAND       = "land";
        public const string ANIM_ROLL       = "roll";
        public const string ANIM_SWIM       = "swim";
        public const string ANIM_CLIMB      = "climb";
        public const string ANIM_CROUCH     = "crouch";
        public const string ANIM_DEAD       = "dead";
        public const string ANIM_TURN_LEFT  = "turn_left";
        public const string ANIM_TURN_RIGHT = "turn_right";
        public const string ANIM_LOOK_AROUND= "look_around";
        public const string ANIM_PUSH       = "push";
        public const string ANIM_PULL       = "pull";
        public const string ANIM_INTERACT   = "interact";
        public const string ANIM_SLEEP      = "sleep";
        public const string ANIM_SIT        = "sit";
        public const string ANIM_CELEBRATE  = "celebrate";
        public const string ANIM_RESPAWN    = "respawn";

        // ---------------------------------------------------------------
        // BLEND TIMES (seconds) — tune for feel
        // ---------------------------------------------------------------
        private const float BLEND_DEFAULT  = 0.15f;
        private const float BLEND_LAND     = 0.05f;
        private const float BLEND_ROLL     = 0.08f;
        private const float BLEND_JUMP     = 0.08f;

        private AnimationPlayer? _animPlayer;
        private AnimationTree? _animTree;
        private string _currentAnim = "";
        private bool _rootMotionEnabled = false;

        // ---------------------------------------------------------------
        // LIFECYCLE
        // ---------------------------------------------------------------

        public override void _Ready()
        {
            // Expect AnimationPlayer and AnimationTree as sibling under Model node
            _animPlayer = GetParent().FindChild("AnimationPlayer", true, false) as AnimationPlayer;
            _animTree   = GetParent().FindChild("AnimationTree", true, false) as AnimationTree;

            if (_animPlayer == null)
            {
                Core.Logger.Warning("PlayerAnimationController: AnimationPlayer not found. " +
                                    "Animations will be no-ops until a model is attached.");
            }
            else
            {
                _animPlayer.AnimationFinished += OnAnimationFinished;
                Core.Logger.Info("PlayerAnimationController: Ready. AnimationPlayer found.");
            }

            if (_animTree != null)
            {
                Core.Logger.Info("PlayerAnimationController: AnimationTree detected. Advanced blending active.");
            }
        }

        // ---------------------------------------------------------------
        // PUBLIC API
        // ---------------------------------------------------------------

        /// <summary>Cross-fade to a new animation with automatic blend time selection.</summary>
        public void Play(string animName)
        {
            if (_animPlayer == null || _currentAnim == animName) return;

            // If AnimationTree is active, we set state or parameters on the tree
            if (_animTree != null && _animTree.Active)
            {
                string statePath = "parameters/playback";
                var playback = _animTree.Get(statePath).As<AnimationNodeStateMachinePlayback>();
                if (playback != null)
                {
                    playback.Travel(animName);
                    _currentAnim = animName;
                    return;
                }
            }

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
            Play(animName);
        }

        /// <summary>Pause the current animation (cutscene freeze).</summary>
        public void Pause()
        {
            _animPlayer?.Pause();
            if (_animTree != null) _animTree.Active = false;
        }

        /// <summary>Resume a paused animation.</summary>
        public void Resume()
        {
            if (_animPlayer != null) _animPlayer.Play();
            if (_animTree != null) _animTree.Active = true;
        }

        /// <summary>Sets a specific parameter in the AnimationTree (e.g. blend speeds).</summary>
        public void SetBlendParameter(string parameterPath, float value)
        {
            if (_animTree != null)
            {
                _animTree.Set($"parameters/{parameterPath}", value);
            }
        }

        /// <summary>Sets layer blending weights (e.g. blending combat/upper body actions).</summary>
        public void SetLayerWeight(string layerPath, float weight)
        {
            if (_animTree != null)
            {
                _animTree.Set($"parameters/{layerPath}/blend_amount", weight);
            }
        }

        /// <summary>Toggles Root Motion application.</summary>
        public void SetRootMotionEnabled(bool enabled)
        {
            _rootMotionEnabled = enabled;
            Core.Logger.Info($"PlayerAnimationController: Root Motion set to {enabled}");
        }

        /// <summary>Gets the Root Motion velocity vector from the AnimationTree.</summary>
        public Vector3 GetRootMotionVelocity()
        {
            if (_animTree != null && _rootMotionEnabled)
            {
                return _animTree.GetRootMotionPosition();
            }
            return Vector3.Zero;
        }

        /// <summary>Configures the upper body layer blending animation.</summary>
        public void SetUpperBodyLayer(string animName, float weight)
        {
            if (_animTree != null)
            {
                SetBlendParameter("UpperBodyBlend/blend_amount", weight);
                // Future production: assign dynamic animName to upper body node
            }
        }

        /// <summary>Configures the lower body layer blending animation.</summary>
        public void SetLowerBodyLayer(string animName, float weight)
        {
            if (_animTree != null)
            {
                SetBlendParameter("LowerBodyBlend/blend_amount", weight);
            }
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
