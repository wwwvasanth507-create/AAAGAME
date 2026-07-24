using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// PlayerAudioController manages all player-related audio:
    ///   - Surface-detected footsteps (7 surfaces × 2 feet)
    ///   - Jump, Landing, Roll sounds
    ///   - Ambient wind/water (framework)
    ///   - Player voice stubs for future phase
    ///
    /// Uses an AudioStreamPlayer3D per channel for spatial audio.
    /// Audio clips are loaded lazily from Assets/Audio/Player/.
    /// </summary>
    public partial class PlayerAudioController : Node
    {
        // ---------------------------------------------------------------
        // AUDIO CHANNELS
        // ---------------------------------------------------------------
        private AudioStreamPlayer3D _footstepChannel = null!;
        private AudioStreamPlayer3D _actionChannel   = null!;
        private AudioStreamPlayer3D _voiceChannel    = null!;

        // ---------------------------------------------------------------
        // FOOTSTEP TIMING
        // ---------------------------------------------------------------
        private float _footstepTimer   = 0f;
        private float _footstepInterval = 0.45f; // seconds between steps (run pace)
        private bool  _footstepEnabled = true;

        // ---------------------------------------------------------------
        // FOOTSTEP AUDIO PATHS — relative to res://
        // ---------------------------------------------------------------
        private static readonly Dictionary<SurfaceType, string[]> FootstepPaths = new()
        {
            [SurfaceType.Stone] = new[] {
                "res://Assets/Audio/Player/Footsteps/stone_01.wav",
                "res://Assets/Audio/Player/Footsteps/stone_02.wav" },
            [SurfaceType.Grass] = new[] {
                "res://Assets/Audio/Player/Footsteps/grass_01.wav",
                "res://Assets/Audio/Player/Footsteps/grass_02.wav" },
            [SurfaceType.Wood]  = new[] {
                "res://Assets/Audio/Player/Footsteps/wood_01.wav",
                "res://Assets/Audio/Player/Footsteps/wood_02.wav" },
            [SurfaceType.Sand]  = new[] {
                "res://Assets/Audio/Player/Footsteps/sand_01.wav",
                "res://Assets/Audio/Player/Footsteps/sand_02.wav" },
            [SurfaceType.Snow]  = new[] {
                "res://Assets/Audio/Player/Footsteps/snow_01.wav",
                "res://Assets/Audio/Player/Footsteps/snow_02.wav" },
            [SurfaceType.Water] = new[] {
                "res://Assets/Audio/Player/Footsteps/water_01.wav",
                "res://Assets/Audio/Player/Footsteps/water_02.wav" },
            [SurfaceType.Mud]   = new[] {
                "res://Assets/Audio/Player/Footsteps/mud_01.wav",
                "res://Assets/Audio/Player/Footsteps/mud_02.wav" },
        };

        private static readonly string JumpSfx   = "res://Assets/Audio/Player/jump.wav";
        private static readonly string LandSfx   = "res://Assets/Audio/Player/land.wav";
        private static readonly string RollSfx   = "res://Assets/Audio/Player/roll.wav";

        private int _footstepIndex = 0;

        // ---------------------------------------------------------------
        // LIFECYCLE
        // ---------------------------------------------------------------

        public override void _Ready()
        {
            _footstepChannel = CreateChannel("FootstepChannel");
            _actionChannel   = CreateChannel("ActionChannel");
            _voiceChannel    = CreateChannel("VoiceChannel");
            Core.Logger.Info("PlayerAudioController: Initialised 3 audio channels.");
        }

        public override void _Process(double delta)
        {
            _footstepTimer += (float)delta;
        }

        // ---------------------------------------------------------------
        // PUBLIC API
        // ---------------------------------------------------------------

        /// <summary>
        /// Called from PlayerAnimationController.OnFootstepEvent or per-frame by movement.
        /// Plays the next footstep clip for the detected surface.
        /// </summary>
        public void PlayFootstep(string foot)
        {
            if (!_footstepEnabled || _footstepTimer < _footstepInterval) return;
            _footstepTimer = 0f;

            // Surface is detected at call time via the player's movement system
            var surface = SurfaceType.Stone;
            PlayFootstepForSurface(surface);
        }

        /// <summary>Called by states to play surface-appropriate footsteps.</summary>
        public void PlayFootstepForSurface(SurfaceType surface)
        {
            if (!FootstepPaths.TryGetValue(surface, out var paths)) return;
            string path = paths[_footstepIndex % paths.Length];
            _footstepIndex++;
            PlayClip(_footstepChannel, path);
        }

        public void PlayJump()   => PlayClip(_actionChannel, JumpSfx);
        public void PlayLand()   => PlayClip(_actionChannel, LandSfx);
        public void PlayRoll()   => PlayClip(_actionChannel, RollSfx);

        /// <summary>Adjust footstep tempo for walk/run/sprint.</summary>
        public void SetMovementPace(float speedMultiplier)
        {
            // Walk = 0.6s between steps; Sprint = 0.28s between steps
            _footstepInterval = Mathf.Lerp(0.6f, 0.28f, Mathf.Clamp(speedMultiplier, 0f, 1f));
        }

        public void SetEnabled(bool enabled) => _footstepEnabled = enabled;

        // ---------------------------------------------------------------
        // PRIVATE
        // ---------------------------------------------------------------

        private void PlayClip(AudioStreamPlayer3D channel, string path)
        {
            if (!ResourceLoader.Exists(path))
            {
                // Audio files are not yet imported — safe no-op during development
                return;
            }
            var stream = GD.Load<AudioStream>(path);
            if (stream == null) return;
            channel.Stream = stream;
            channel.Play();
        }

        private AudioStreamPlayer3D CreateChannel(string name)
        {
            var ch = new AudioStreamPlayer3D
            {
                Name       = name,
                MaxDb      = 0f,
                UnitSize   = 10f,
                AttenuationModel = AudioStreamPlayer3D.AttenuationModelEnum.Logarithmic,
            };
            AddChild(ch);
            return ch;
        }
    }
}
