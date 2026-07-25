using System;
using System.Collections.Generic;

namespace HeroOfEternia.Audio
{
    /// <summary>
    /// Dynamic range profiles for audio output adaptation.
    /// </summary>
    public enum DynamicRangeProfile
    {
        Full,       // Uncompressed dynamic range for high-end speakers/home theater
        Midnight,   // Compressed dynamics (boosts quiet sounds, tames explosions)
        Headphones, // Optimized for stereo headphone positioning
        Mobile      // Boosted clarity for mobile phone speakers
    }

    /// <summary>
    /// Subtitle display configuration for dialogue and barks.
    /// </summary>
    public class SubtitleSettings
    {
        public bool Enabled { get; set; } = true;
        public int FontSize { get; set; } = 18;
        public bool ShowSpeakerName { get; set; } = true;
        public bool ColorCodeSpeakers { get; set; } = true;
        public bool ShowBackground { get; set; } = true;
        public float BackgroundOpacity { get; set; } = 0.6f;
    }

    /// <summary>
    /// Complete user audio preferences data structure.
    /// Integrated with SaveManager profile persistence.
    /// </summary>
    public class AudioSettings
    {
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 0.8f;
        public float AmbientVolume { get; set; } = 0.8f;
        public float EnvironmentVolume { get; set; } = 0.8f;
        public float CombatVolume { get; set; } = 0.9f;
        public float UIVolume { get; set; } = 0.8f;
        public float DialogueVolume { get; set; } = 1.0f;
        public float NPCVolume { get; set; } = 0.9f;
        public float CreaturesVolume { get; set; } = 0.9f;
        public float WeatherVolume { get; set; } = 0.8f;
        public float FootstepsVolume { get; set; } = 0.7f;
        public float AbilitiesVolume { get; set; } = 0.9f;
        public float VoiceOverVolume { get; set; } = 1.0f;

        public bool IsMuted { get; set; } = false;
        public bool MuteInBackground { get; set; } = true;

        public DynamicRangeProfile DynamicRange { get; set; } = DynamicRangeProfile.Mobile;
        public SubtitleSettings Subtitles { get; set; } = new();

        private readonly Dictionary<AudioCategory, float> _categoryVolumes = new();

        public AudioSettings()
        {
            SyncCategoryDictionary();
        }

        public void SyncCategoryDictionary()
        {
            _categoryVolumes[AudioCategory.Master] = MasterVolume;
            _categoryVolumes[AudioCategory.Music] = MusicVolume;
            _categoryVolumes[AudioCategory.Ambient] = AmbientVolume;
            _categoryVolumes[AudioCategory.Environment] = EnvironmentVolume;
            _categoryVolumes[AudioCategory.Combat] = CombatVolume;
            _categoryVolumes[AudioCategory.UI] = UIVolume;
            _categoryVolumes[AudioCategory.Dialogue] = DialogueVolume;
            _categoryVolumes[AudioCategory.NPC] = NPCVolume;
            _categoryVolumes[AudioCategory.Creatures] = CreaturesVolume;
            _categoryVolumes[AudioCategory.Weather] = WeatherVolume;
            _categoryVolumes[AudioCategory.Footsteps] = FootstepsVolume;
            _categoryVolumes[AudioCategory.Abilities] = AbilitiesVolume;
            _categoryVolumes[AudioCategory.VoiceOver] = VoiceOverVolume;
            _categoryVolumes[AudioCategory.DeveloperDebug] = 1.0f;
        }

        public float GetCategoryVolume(AudioCategory category)
        {
            return _categoryVolumes.TryGetValue(category, out float vol) ? vol : 1.0f;
        }

        public void SetCategoryVolume(AudioCategory category, float volume)
        {
            volume = Math.Clamp(volume, 0f, 1f);
            _categoryVolumes[category] = volume;

            switch (category)
            {
                case AudioCategory.Master: MasterVolume = volume; break;
                case AudioCategory.Music: MusicVolume = volume; break;
                case AudioCategory.Ambient: AmbientVolume = volume; break;
                case AudioCategory.Environment: EnvironmentVolume = volume; break;
                case AudioCategory.Combat: CombatVolume = volume; break;
                case AudioCategory.UI: UIVolume = volume; break;
                case AudioCategory.Dialogue: DialogueVolume = volume; break;
                case AudioCategory.NPC: NPCVolume = volume; break;
                case AudioCategory.Creatures: CreaturesVolume = volume; break;
                case AudioCategory.Weather: WeatherVolume = volume; break;
                case AudioCategory.Footsteps: FootstepsVolume = volume; break;
                case AudioCategory.Abilities: AbilitiesVolume = volume; break;
                case AudioCategory.VoiceOver: VoiceOverVolume = volume; break;
            }
        }
    }
}
