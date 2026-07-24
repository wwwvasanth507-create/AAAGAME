using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// ConfigManager manages loading, caching, and hot-reloading configurations
    /// inside the Settings/ directory.
    /// </summary>
    public class ConfigManager
    {
        private readonly string _configDirectory;
        private readonly Dictionary<string, string> _cachedConfigs = new Dictionary<string, string>();

        public ConfigManager(string configDirectory)
        {
            _configDirectory = configDirectory;
            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }
        }

        /// <summary>
        /// Loads a configuration JSON string. Caches it for subsequent reads.
        /// </summary>
        public string GetConfigJson(string configName)
        {
            if (_cachedConfigs.TryGetValue(configName, out string? cached))
            {
                return cached;
            }

            string filePath = Path.Combine(_configDirectory, $"{configName}_config.json");
            if (!File.Exists(filePath))
            {
                Logger.Warning($"ConfigManager: File not found for configuration '{configName}'. Creating template...");
                CreateDefaultConfigTemplate(configName, filePath);
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);
                _cachedConfigs[configName] = jsonString;
                Logger.Info($"ConfigManager: Loaded and cached configuration '{configName}'");
                return jsonString;
            }
            catch (Exception ex)
            {
                Logger.Error($"ConfigManager: Failed to read config '{configName}': {ex.Message}");
                return "{}";
            }
        }

        /// <summary>
        /// Parses a configuration into a C# typed object.
        /// </summary>
        public T? GetConfig<T>(string configName) where T : class
        {
            string json = GetConfigJson(configName);
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Logger.Error($"ConfigManager: Deserialization exception for '{configName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Clears cache to force reloading from disk on next read (Hot Reloading).
        /// </summary>
        public void HotReloadAll()
        {
            Logger.Info("ConfigManager: Hot-reloading all configuration files from disk...");
            _cachedConfigs.Clear();
        }

        private void CreateDefaultConfigTemplate(string configName, string filePath)
        {
            string defaultJson = "{}";
            switch (configName.ToLower())
            {
                case "physics":
                    defaultJson = "{\n  \"gravity\": -9.81,\n  \"air_resistance\": 0.05,\n  \"terminal_velocity\": 50.0\n}";
                    break;
                case "camera":
                    defaultJson = "{\n  \"fov\": 75.0,\n  \"sensitivity\": 1.2,\n  \"invert_y\": false\n}";
                    break;
                case "debug":
                    defaultJson = "{\n  \"enable_cheats\": false,\n  \"show_wireframe\": false,\n  \"perf_logs_interval_seconds\": 5.0\n}";
                    break;
            }

            try
            {
                File.WriteAllText(filePath, defaultJson);
                Logger.Info($"ConfigManager: Initialized default JSON file for config '{configName}'");
            }
            catch (Exception ex)
            {
                Logger.Error($"ConfigManager: Failed to write default template for '{configName}': {ex.Message}");
            }
        }
    }
}
