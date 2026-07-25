using System;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter3
{
    /// <summary>
    /// Presentation orchestrator for the Citadel of Void Shadows dungeon — ambient cave music,
    /// Void Gate boss theme, environmental VFX triggers, and dungeon lighting profiles.
    /// </summary>
    public class Chapter3PresentationController
    {
        public string ActiveMusicTrack { get; private set; } = "music_citadel_dungeon_ambient";
        public string ActiveLightingProfile { get; private set; } = "profile_dungeon_shadow";

        public void PlayDungeonAmbience()
        {
            ActiveMusicTrack = "music_citadel_dungeon_ambient";
            Logger.Info($"Chapter3PresentationController: Playing music '{ActiveMusicTrack}'");
        }

        public void PlayBossTheme()
        {
            ActiveMusicTrack = "music_void_knight_boss";
            Logger.Info($"Chapter3PresentationController: Playing music '{ActiveMusicTrack}'");
        }

        public void PlayActIVictoryFanfare()
        {
            ActiveMusicTrack = "music_act1_victory";
            Logger.Info($"Chapter3PresentationController: Playing music '{ActiveMusicTrack}'");
        }

        public void ApplyDungeonLighting()
        {
            ActiveLightingProfile = "profile_dungeon_shadow";
            Logger.Info($"Chapter3PresentationController: Applied lighting profile '{ActiveLightingProfile}'");
        }

        public void ApplyBossArenaLighting()
        {
            ActiveLightingProfile = "profile_boss_void_gate";
            Logger.Info($"Chapter3PresentationController: Applied lighting profile '{ActiveLightingProfile}'");
        }
    }
}
