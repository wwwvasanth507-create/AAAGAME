using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Audio
{
    /// <summary>
    /// Ambient zone definition for biome environment blending.
    /// </summary>
    public class AmbientZone
    {
        public string ZoneId { get; set; } = string.Empty;
        public string AudioStreamPath { get; set; } = string.Empty;
        public float DefaultVolume { get; set; } = 0.8f;
        public float FadeTime { get; set; } = 3.0f;
    }

    /// <summary>
    /// Manages multi-layered environmental ambience (wind, rain, forest birds,
    /// cave reverberation, dungeon hums). Smoothly blends ambient soundscapes
    /// based on world region and weather events.
    /// </summary>
    public partial class AmbientAudioManager : Node
    {
        private AudioStreamPlayer _ambientPlayer1;
        private AudioStreamPlayer _ambientPlayer2;

        private readonly Dictionary<string, AmbientZone> _zones = new(StringComparer.OrdinalIgnoreCase);
        private string _activeZoneId = string.Empty;
        private float _masterAmbientVolume = 0.8f;

        public float AmbientVolume
        {
            get => _masterAmbientVolume;
            set
            {
                _masterAmbientVolume = Math.Clamp(value, 0f, 1f);
                UpdateVolumes();
            }
        }

        public override void _Ready()
        {
            _ambientPlayer1 = new AudioStreamPlayer { Name = "AmbientLayer1" };
            _ambientPlayer2 = new AudioStreamPlayer { Name = "AmbientLayer2" };

            AddChild(_ambientPlayer1);
            AddChild(_ambientPlayer2);
        }

        public void RegisterZone(AmbientZone zone)
        {
            if (zone != null && !string.IsNullOrEmpty(zone.ZoneId))
            {
                _zones[zone.ZoneId] = zone;
            }
        }

        public void SetZone(string zoneId, float fadeTime = 3.0f)
        {
            if (_activeZoneId.Equals(zoneId, StringComparison.OrdinalIgnoreCase)) return;

            _activeZoneId = zoneId;
            if (_zones.TryGetValue(zoneId, out var zone))
            {
                if (ResourceLoader.Exists(zone.AudioStreamPath))
                {
                    var stream = GD.Load<AudioStream>(zone.AudioStreamPath);
                    if (stream != null)
                    {
                        _ambientPlayer1.Stream = stream;
                        _ambientPlayer1.VolumeDb = Mathf.LinearToDb(_masterAmbientVolume * zone.DefaultVolume);
                        _ambientPlayer1.Play();
                    }
                }
            }
        }

        public void StopAmbience(float fadeTime = 2.0f)
        {
            _activeZoneId = string.Empty;
            _ambientPlayer1?.Stop();
            _ambientPlayer2?.Stop();
        }

        private void UpdateVolumes()
        {
            if (_ambientPlayer1 != null && _ambientPlayer1.Playing)
            {
                _ambientPlayer1.VolumeDb = Mathf.LinearToDb(_masterAmbientVolume);
            }
        }
    }
}
