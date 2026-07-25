using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter2
{
    /// <summary>
    /// Presentation orchestrator for Sylvanwood forest ambience, Elderwood Grove theme,
    /// ancient ruins dungeon track, and blighted fog weather overlay.
    /// </summary>
    public class Chapter2PresentationController
    {
        public string ActiveMusicTrack { get; private set; } = "music_sylvanwood_forest";
        public string ActiveLightingProfile { get; private set; } = "profile_sylvanwood_canopy";

        public void PlayElderwoodTheme()
        {
            ActiveMusicTrack = "music_elderwood_town";
            Logger.Info($"Chapter2PresentationController: Playing music '{ActiveMusicTrack}'");
        }

        public void PlayRuinsBossMusic()
        {
            ActiveMusicTrack = "music_ruin_guardian_boss";
            Logger.Info($"Chapter2PresentationController: Playing music '{ActiveMusicTrack}'");
        }

        public void ApplyCanopyLighting()
        {
            ActiveLightingProfile = "profile_sylvanwood_canopy";
            Logger.Info($"Chapter2PresentationController: Applied lighting profile '{ActiveLightingProfile}'");
        }
    }
}
