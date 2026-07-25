using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Prologue
{
    /// <summary>
    /// Presentation orchestrator configuring ambient audio, village music, exploration track,
    /// combat music triggers, and weather overlays for Oakvale.
    /// </summary>
    public class StarterPresentationController
    {
        public string ActiveMusicTrack { get; private set; } = "music_oakvale_peaceful";
        public string ActiveLightingProfile { get; private set; } = "profile_oakvale_morning";

        public void PlayVillageTheme()
        {
            ActiveMusicTrack = "music_oakvale_peaceful";
            Logger.Info($"StarterPresentationController: Playing music '{ActiveMusicTrack}'");
        }

        public void PlayCombatTheme()
        {
            ActiveMusicTrack = "music_oakvale_combat";
            Logger.Info($"StarterPresentationController: Playing music '{ActiveMusicTrack}'");
        }

        public void ApplyMorningLighting()
        {
            ActiveLightingProfile = "profile_oakvale_morning";
            Logger.Info($"StarterPresentationController: Applied lighting profile '{ActiveLightingProfile}'");
        }
    }
}
