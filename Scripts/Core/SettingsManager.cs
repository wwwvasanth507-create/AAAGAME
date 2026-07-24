using System;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Governs audio, graphics presets, language locale, and developer settings checks.
    /// </summary>
    public class SettingsManager
    {
        public string QualityPreset { get; private set; } = "HIGH";
        public float MasterVolume { get; private set; } = 0.8f;
        public string Language { get; private set; } = "en";
        public bool DebugModeEnabled { get; private set; } = false;

        public event Action? OnSettingsUpdated;

        public void LoadSettings(string graphicsJson, string audioJson, string languageJson, string devJson)
        {
            Logger.Info("SettingsManager: Loading configurations from JSON buffers...");
            
            // Mock parsing configs (Real implementation uses JsonSerializer)
            QualityPreset = "HIGH";
            MasterVolume = 0.8f;
            Language = "en";
            DebugModeEnabled = true;

            Logger.Info($"SettingsManager: Settings active. Preset={QualityPreset}, Volume={MasterVolume}, Locale={Language}, Debug={DebugModeEnabled}");
            OnSettingsUpdated?.Invoke();
        }

        public void ApplyGraphicsPreset(string preset)
        {
            Logger.Info($"SettingsManager: Applying graphics preset override: {preset}");
            QualityPreset = preset;
            OnSettingsUpdated?.Invoke();
        }

        public void SetVolume(float volume)
        {
            MasterVolume = Math.Clamp(volume, 0.0f, 1.0f);
            Logger.Info($"SettingsManager: Set master volume to {MasterVolume}");
            OnSettingsUpdated?.Invoke();
        }
    }
}
