using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Audio
{
    // Sound Event Payload Records
    public record PlaySoundEvent(string SoundId, AudioCategory Category, Vector3 Position = default, float Volume = 1.0f, float Pitch = 1.0f);
    public record PlayMusicEvent(string TrackId, float FadeTime = 2.0f);
    public record PlayAmbienceEvent(string ZoneId, float FadeTime = 3.0f);
    public record PlayVoiceBarkEvent(string SpeakerName, string Text, string StreamPath = "", float Duration = 2.5f);
    public record FootstepEvent(Vector3 Position, string SurfaceType = "Dirt");
    public record AbilitySoundEvent(string AbilityId, Vector3 Position);

    /// <summary>
    /// Event-driven sound trigger listener. Subscribes to EventBus and converts
    /// game triggers (ability activation, hits, footsteps, UI clicks) into audio playback commands.
    /// </summary>
    public class SoundEventSystem
    {
        public void Initialize()
        {
            EventBus.Subscribe<PlaySoundEvent>(OnPlaySound);
            EventBus.Subscribe<PlayMusicEvent>(OnPlayMusic);
            EventBus.Subscribe<PlayAmbienceEvent>(OnPlayAmbience);
            EventBus.Subscribe<PlayVoiceBarkEvent>(OnPlayVoiceBark);
            EventBus.Subscribe<FootstepEvent>(OnFootstep);
            EventBus.Subscribe<AbilitySoundEvent>(OnAbilitySound);
        }

        public void Shutdown()
        {
            EventBus.Unsubscribe<PlaySoundEvent>(OnPlaySound);
            EventBus.Unsubscribe<PlayMusicEvent>(OnPlayMusic);
            EventBus.Unsubscribe<PlayAmbienceEvent>(OnPlayAmbience);
            EventBus.Unsubscribe<PlayVoiceBarkEvent>(OnPlayVoiceBark);
            EventBus.Unsubscribe<FootstepEvent>(OnFootstep);
            EventBus.Unsubscribe<AbilitySoundEvent>(OnAbilitySound);
        }

        private void OnPlaySound(PlaySoundEvent e)
        {
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.PlaySound(e.SoundId, e.Category, e.Position, e.Volume, e.Pitch);
        }

        private void OnPlayMusic(PlayMusicEvent e)
        {
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.MusicManager?.PlayTrack(e.TrackId, e.FadeTime);
        }

        private void OnPlayAmbience(PlayAmbienceEvent e)
        {
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.AmbientManager?.SetZone(e.ZoneId, e.FadeTime);
        }

        private void OnPlayVoiceBark(PlayVoiceBarkEvent e)
        {
            var audio = ServiceLocator.Get<AudioManager>();
            AudioStream stream = !string.IsNullOrEmpty(e.StreamPath) && ResourceLoader.Exists(e.StreamPath) ? GD.Load<AudioStream>(e.StreamPath) : null;
            audio?.VoiceFramework?.PlayBark(e.SpeakerName, e.Text, stream, e.Duration);
        }

        private void OnFootstep(FootstepEvent e)
        {
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.PlaySound($"footstep_{e.SurfaceType.ToLower()}", AudioCategory.Footsteps, e.Position, 0.7f, (float)GD.RandRange(0.9, 1.1));
        }

        private void OnAbilitySound(AbilitySoundEvent e)
        {
            var audio = ServiceLocator.Get<AudioManager>();
            audio?.PlaySound($"ability_{e.AbilityId}", AudioCategory.Abilities, e.Position);
        }
    }
}
