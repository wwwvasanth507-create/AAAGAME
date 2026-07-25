using System;
using Godot;

namespace HeroOfEternia.Audio
{
    /// <summary>
    /// Distance attenuation models for 3D spatialized audio emitters.
    /// </summary>
    public enum AttenuationModel
    {
        Linear,
        Inverse,
        Exponential
    }

    /// <summary>
    /// Wrapper component for 3D spatial sound sources with dynamic positioning,
    /// distance attenuation, occlusion queries, and channel pooling.
    /// </summary>
    public partial class PositionalAudioPlayer : Node3D
    {
        private AudioStreamPlayer3D _player;

        public AudioCategory Category { get; set; } = AudioCategory.Environment;
        public AudioPriority Priority { get; set; } = AudioPriority.Medium;

        public float MaxDistance { get; set; } = 40.0f;
        public float UnitDistance { get; set; } = 1.0f;
        public AttenuationModel Attenuation { get; set; } = AttenuationModel.Inverse;
        public bool IsOcclusionEnabled { get; set; } = true;

        public bool IsActive => _player != null && _player.Playing;

        public override void _Ready()
        {
            _player = new AudioStreamPlayer3D
            {
                MaxDistance = MaxDistance,
                UnitSize = UnitDistance,
                MaxDb = 0f
            };
            AddChild(_player);
        }

        public void PlaySound(AudioStream stream, Vector3 position, float volume = 1.0f, float pitch = 1.0f)
        {
            GlobalPosition = position;
            if (_player == null) return;

            _player.Stream = stream;
            _player.VolumeDb = Mathf.LinearToDb(Math.Clamp(volume, 0.001f, 2.0f));
            _player.PitchScale = Math.Clamp(pitch, 0.5f, 2.0f);
            _player.Play();
        }

        public void Stop()
        {
            _player?.Stop();
        }
    }
}
