using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HeroOfEternia.Core
{
    public class SettingsPayload
    {
        public float MasterVolume { get; set; } = 0.8f;
        public float MusicVolume { get; set; } = 0.7f;
        public float SfxVolume { get; set; } = 0.9f;
        
        public string QualityPreset { get; set; } = "HIGH";
        public string Language { get; set; } = "en";
        
        // Touch Controls
        public float JoystickDeadzone { get; set; } = 0.15f;
        public float JoystickSensitivity { get; set; } = 1.0f;
        public bool VibrationEnabled { get; set; } = true;
        
        // Accessibility
        public bool LargeFonts { get; set; } = false;
        public bool ColorblindMode { get; set; } = false;
        public float SubtitlesBackgroundOpacity { get; set; } = 0.5f;

        // Gameplay
        public bool AutoSaveOnTransition { get; set; } = true;
        public bool ShowDevConsole { get; set; } = false;
    }

    /// <summary>
    /// SettingsManager handles user options, default resets, and autosaves settings changes.
    /// </summary>
    public class SettingsManager
    {
        private readonly string _settingsFilePath;
        private SettingsPayload _current = new SettingsPayload();

        public float MasterVolume => _current.MasterVolume;
        public float MusicVolume => _current.MusicVolume;
        public float SfxVolume => _current.SfxVolume;
        public string QualityPreset => _current.QualityPreset;
        public string Language => _current.Language;
        public float JoystickDeadzone => _current.JoystickDeadzone;
        public float JoystickSensitivity => _current.JoystickSensitivity;
        public bool VibrationEnabled => _current.VibrationEnabled;
        public bool LargeFonts => _current.LargeFonts;
        public bool ColorblindMode => _current.ColorblindMode;
        public bool AutoSaveOnTransition => _current.AutoSaveOnTransition;
        public bool DebugModeEnabled => _current.ShowDevConsole;

        public event Action? OnSettingsUpdated;

        public SettingsManager(string settingsDir)
        {
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }
            _settingsFilePath = Path.Combine(settingsDir, "user_settings.json");
        }

        public void LoadSettings(string graphicsJson, string audioJson, string languageJson, string devJson)
        {
            Logger.Info("SettingsManager: Loading user settings profile...");
            
            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var loaded = JsonSerializer.Deserialize<SettingsPayload>(json);
                    if (loaded != null)
                    {
                        _current = loaded;
                        Logger.Info("SettingsManager: User settings loaded successfully.");
                        OnSettingsUpdated?.Invoke();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"SettingsManager: Settings parse exception: {ex.Message}");
                }
            }

            // Fallback: Reset to hardware auto-detected defaults if file doesn't exist
            ResetToDefaults();
        }

        public void SaveSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(_current, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, json);
                Logger.Info("SettingsManager: Saved user options profile to disk.");
            }
            catch (Exception ex)
            {
                Logger.Error($"SettingsManager: Failed to write options file: {ex.Message}");
            }
        }

        public void ResetToDefaults()
        {
            Logger.Info("SettingsManager: Resetting options to factory defaults...");
            _current = new SettingsPayload();
            SaveSettings();
            OnSettingsUpdated?.Invoke();
        }

        public void SetVolume(float volume)
        {
            _current.MasterVolume = Math.Clamp(volume, 0.0f, 1.0f);
            Logger.Info($"SettingsManager: Updated Master Volume to {_current.MasterVolume}");
            SaveSettings();
            OnSettingsUpdated?.Invoke();
        }

        public void ApplyGraphicsPreset(string preset)
        {
            _current.QualityPreset = preset;
            Logger.Info($"SettingsManager: Updated Graphics Quality preset to {_current.QualityPreset}");
            SaveSettings();
            OnSettingsUpdated?.Invoke();
        }

        public void SetLanguage(string lang)
        {
            _current.Language = lang;
            Logger.Info($"SettingsManager: Language locale updated to {_current.Language}");
            SaveSettings();
            OnSettingsUpdated?.Invoke();
        }

        public void SetJoystickSpecs(float deadzone, float sensitivity)
        {
            _current.JoystickDeadzone = Math.Clamp(deadzone, 0.05f, 0.5f);
            _current.JoystickSensitivity = Math.Clamp(sensitivity, 0.2f, 3.0f);
            Logger.Info($"SettingsManager: Controls calibrated. Deadzone={_current.JoystickDeadzone}, Sensitivity={_current.JoystickSensitivity}");
            SaveSettings();
            OnSettingsUpdated?.Invoke();
        }

        public void ToggleDevConsole(bool enabled)
        {
            _current.ShowDevConsole = enabled;
            Logger.Info($"SettingsManager: Developer options console visibility updated: {enabled}");
            SaveSettings();
            OnSettingsUpdated?.Invoke();
        }
    }
}
