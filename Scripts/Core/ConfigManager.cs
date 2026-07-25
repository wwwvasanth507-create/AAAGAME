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
                case "gameplay":
                    defaultJson = "{\n  \"difficulty\": \"NORMAL\",\n  \"auto_save_interval_seconds\": 300,\n  \"tutorials_enabled\": true,\n  \"death_penalty\": \"XP_LOSS\"\n}";
                    break;
                case "performance":
                    defaultJson = "{\n  \"target_fps\": 60,\n  \"dynamic_resolution_enabled\": true,\n  \"min_resolution_scale\": 0.5,\n  \"max_resolution_scale\": 1.0,\n  \"object_pool_size\": 64\n}";
                    break;
                case "localization":
                    defaultJson = "{\n  \"default_language\": \"en\",\n  \"supported_languages\": [\"en\", \"fr\", \"de\", \"es\", \"ja\", \"zh\"],\n  \"fallback_language\": \"en\"\n}";
                    break;
                case "debug":
                    defaultJson = "{\n  \"enable_cheats\": false,\n  \"show_wireframe\": false,\n  \"perf_logs_interval_seconds\": 5.0\n}";
                    break;
                case "player_attributes":
                    defaultJson = "{\n  \"Health\": 100.0,\n  \"Mana\": 50.0,\n  \"Energy\": 100.0,\n  \"Stamina\": 100.0,\n  \"Strength\": 10.0,\n  \"Vitality\": 10.0,\n  \"Magic\": 5.0,\n  \"Dexterity\": 5.0,\n  \"Luck\": 5.0,\n  \"Attack\": 15.0,\n  \"Defense\": 5.0,\n  \"Speed\": 10.0,\n  \"CriticalRate\": 0.05,\n  \"CriticalDamage\": 1.5\n}";
                    break;
                case "item_database":
                    defaultJson = "[\n  {\n    \"UniqueId\": \"wpn_iron_sword\",\n    \"InternalName\": \"Iron Sword\",\n    \"DisplayName\": \"Rusty Iron Sword\",\n    \"Description\": \"A simple iron blade, dull but functional.\",\n    \"Category\": \"Weapon\",\n    \"Subcategory\": \"OneHandSword\",\n    \"Tier\": 1,\n    \"Rarity\": 0,\n    \"Weight\": 2.5,\n    \"StackSize\": 1,\n    \"SellValue\": 5,\n    \"BuyValue\": 15,\n    \"IconPath\": \"res://Assets/UI/Icons/wpn_sword.png\",\n    \"ModelPath\": \"res://Assets/Characters/Meshes/Player/weapon_default.glb\",\n    \"MaterialPath\": \"\",\n    \"LocKey\": \"item.rusty_sword\",\n    \"StatModifiers\": [\n      {\n        \"AttributeType\": \"Strength\",\n        \"Value\": 2.0,\n        \"ModifierType\": \"Flat\"\n      }\n    ]\n  },\n  {\n    \"UniqueId\": \"pot_minor_health\",\n    \"InternalName\": \"Minor Health Potion\",\n    \"DisplayName\": \"Minor Health Potion\",\n    \"Description\": \"Restores 30 Health over 3 seconds.\",\n    \"Category\": \"Potion\",\n    \"Subcategory\": \"Health\",\n    \"Tier\": 1,\n    \"Rarity\": 0,\n    \"Weight\": 0.5,\n    \"StackSize\": 20,\n    \"SellValue\": 2,\n    \"BuyValue\": 8,\n    \"IconPath\": \"res://Assets/UI/Icons/pot_health.png\",\n    \"ModelPath\": \"\",\n    \"MaterialPath\": \"\",\n    \"LocKey\": \"item.health_potion\",\n    \"StatModifiers\": []\n  }\n]";
                    break;
                case "rarities":
                    defaultJson = "[\n  {\n    \"Rarity\": 0,\n    \"ColorHex\": \"#9D9D9D\",\n    \"BorderSpritePath\": \"res://Assets/UI/Borders/common.png\",\n    \"DropWeight\": 100.0,\n    \"VisualEffectHook\": \"Vfx_Common\",\n    \"AudioHook\": \"Sfx_Common\"\n  },\n  {\n    \"Rarity\": 1,\n    \"ColorHex\": \"#1EFF00\",\n    \"BorderSpritePath\": \"res://Assets/UI/Borders/uncommon.png\",\n    \"DropWeight\": 40.0,\n    \"VisualEffectHook\": \"Vfx_Uncommon\",\n    \"AudioHook\": \"Sfx_Uncommon\"\n  },\n  {\n    \"Rarity\": 2,\n    \"ColorHex\": \"#0070DD\",\n    \"BorderSpritePath\": \"res://Assets/UI/Borders/rare.png\",\n    \"DropWeight\": 15.0,\n    \"VisualEffectHook\": \"Vfx_Rare\",\n    \"AudioHook\": \"Sfx_Rare\"\n  },\n  {\n    \"Rarity\": 3,\n    \"ColorHex\": \"#A335EE\",\n    \"BorderSpritePath\": \"res://Assets/UI/Borders/epic.png\",\n    \"DropWeight\": 5.0,\n    \"VisualEffectHook\": \"Vfx_Epic\",\n    \"AudioHook\": \"Sfx_Epic\"\n  },\n  {\n    \"Rarity\": 4,\n    \"ColorHex\": \"#FF8000\",\n    \"BorderSpritePath\": \"res://Assets/UI/Borders/legendary.png\",\n    \"DropWeight\": 1.0,\n    \"VisualEffectHook\": \"Vfx_Legendary\",\n    \"AudioHook\": \"Sfx_Legendary\"\n  }\n]";
                    break;
                case "biomes":
                    defaultJson = "[\n  {\n    \"Type\": 0,\n    \"Name\": \"Forest\",\n    \"Temperature\": 0.6,\n    \"Humidity\": 0.7,\n    \"MinElevation\": 0.1,\n    \"MaxElevation\": 0.6,\n    \"TerrainType\": \"Hilly\",\n    \"SkyProfile\": \"res://Assets/Environment/Sky/forest_sky.tres\",\n    \"WeatherProfile\": \"Clear\"\n  },\n  {\n    \"Type\": 1,\n    \"Name\": \"Grassland\",\n    \"Temperature\": 0.5,\n    \"Humidity\": 0.5,\n    \"MinElevation\": 0.0,\n    \"MaxElevation\": 0.4,\n    \"TerrainType\": \"Flat\",\n    \"SkyProfile\": \"res://Assets/Environment/Sky/grassland_sky.tres\",\n    \"WeatherProfile\": \"Clear\"\n  }\n]";
                    break;
                case "world_database":
                    defaultJson = "[\n  {\n    \"Id\": \"tree_oak\",\n    \"Category\": \"Tree\",\n    \"DisplayName\": \"Oak Tree\",\n    \"ModelPath\": \"res://Assets/Environment/Meshes/tree_oak.glb\",\n    \"BaseSpawnWeight\": 10.0\n  },\n  {\n    \"Id\": \"rock_granite\",\n    \"Category\": \"Rock\",\n    \"DisplayName\": \"Granite Rock\",\n    \"ModelPath\": \"res://Assets/Environment/Meshes/rock_granite.glb\",\n    \"BaseSpawnWeight\": 5.0\n  }\n]";
                    break;
                case "weather_profiles":
                    defaultJson = "[\n  {\n    \"Type\": 0,\n    \"DisplayName\": \"Clear\",\n    \"TemperatureModifier\": 0.0,\n    \"WindStrength\": 0.0,\n    \"VisualEffectHook\": \"Vfx_Clear\",\n    \"AmbientSoundKey\": \"Ambient_Clear\"\n  },\n  {\n    \"Type\": 1,\n    \"DisplayName\": \"Rain\",\n    \"TemperatureModifier\": -0.05,\n    \"WindStrength\": 0.3,\n    \"VisualEffectHook\": \"Vfx_Rain\",\n    \"AmbientSoundKey\": \"Ambient_Rain\"\n  }\n]";
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
