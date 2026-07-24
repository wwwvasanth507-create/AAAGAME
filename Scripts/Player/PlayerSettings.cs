using System.IO;
using System.Text.Json;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// Player-specific settings — separate from global SettingsManager.
    /// Serialized to player_settings.json for per-player persistence.
    /// </summary>
    public class PlayerSettingsData
    {
        // Camera
        public float CameraSensitivity  { get; set; } = 0.4f;
        public float CameraDistance     { get; set; } = 5.0f;
        public float CameraHeight       { get; set; } = 1.8f;
        public bool  InvertY            { get; set; } = false;

        // Movement
        public bool  SprintToggle       { get; set; } = false; // hold vs toggle
        public bool  AutoSprint         { get; set; } = false;

        // Touch
        public float JoystickOpacity    { get; set; } = 0.55f;
        public float TouchButtonSize    { get; set; } = 90f;
        public bool  LeftHandedMode     { get; set; } = false;
        public string ButtonLayout      { get; set; } = "default"; // future layouts

        // Future: Aim Assist (Phase 7+)
        public bool  AimAssist          { get; set; } = false;
    }

    /// <summary>
    /// Manages loading, saving, and applying player-specific settings.
    /// Uses the global Logger and persists to user data directory.
    /// </summary>
    public class PlayerSettings
    {
        private readonly string _filePath;
        private PlayerSettingsData _data = new();

        public PlayerSettingsData Data => _data;

        public PlayerSettings()
        {
            _filePath = Path.Combine(OS.GetUserDataDir(), "player_settings.json");
        }

        public void Load()
        {
            if (!File.Exists(_filePath)) { Save(); return; }
            try
            {
                string json = File.ReadAllText(_filePath);
                var loaded  = JsonSerializer.Deserialize<PlayerSettingsData>(json);
                if (loaded != null) _data = loaded;
                Logger.Info("PlayerSettings: Loaded from disk.");
            }
            catch (System.Exception ex)
            {
                Logger.Error($"PlayerSettings: Load failed — {ex.Message}. Using defaults.");
                _data = new PlayerSettingsData();
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(_data,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                Logger.Info("PlayerSettings: Saved to disk.");
            }
            catch (System.Exception ex)
            {
                Logger.Error($"PlayerSettings: Save failed — {ex.Message}");
            }
        }

        // ---------------------------------------------------------------
        // SETTERS — auto-save after every change
        // ---------------------------------------------------------------

        public void SetSensitivity(float value)
        {
            _data.CameraSensitivity = System.Math.Clamp(value, 0.05f, 2.0f);
            Save();
        }

        public void SetCameraDistance(float value)
        {
            _data.CameraDistance = System.Math.Clamp(value, 1.5f, 12f);
            Save();
        }

        public void SetInvertY(bool value) { _data.InvertY = value; Save(); }
        public void SetSprintToggle(bool value) { _data.SprintToggle = value; Save(); }
        public void SetLeftHanded(bool value) { _data.LeftHandedMode = value; Save(); }

        public void SetJoystickOpacity(float value)
        {
            _data.JoystickOpacity = System.Math.Clamp(value, 0.1f, 1.0f);
            Save();
        }

        public void SetTouchButtonSize(float value)
        {
            _data.TouchButtonSize = System.Math.Clamp(value, 60f, 150f);
            Save();
        }

        public void ResetToDefaults()
        {
            _data = new PlayerSettingsData();
            Save();
            Logger.Info("PlayerSettings: Reset to defaults.");
        }
    }
}
