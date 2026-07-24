using System;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Coordinates audio bus routing and music/SFX play triggers.
    /// </summary>
    public class AudioManager
    {
        public void Initialize(float initialVolume)
        {
            Logger.Info($"AudioManager: Initializing sound buffers with volume level: {initialVolume}");
        }

        public void PlayMusic(string trackPath, bool loop = true)
        {
            Logger.Info($"AudioManager: Blending background music track: {trackPath} (Loop={loop})");
        }

        public void PlaySfx(string sfxPath, float volumeScale = 1.0f)
        {
            Logger.Info($"AudioManager: Triggering sound effect transient: {sfxPath} (Scale={volumeScale})");
        }

        public void StopMusic()
        {
            Logger.Info("AudioManager: Fading music tracks.");
        }
    }
}
