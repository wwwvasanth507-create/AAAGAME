using System;
using System.Collections.Generic;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.World
{
    public enum WeatherType
    {
        Clear,
        Rain,
        HeavyRain,
        Storm,
        Snow,
        Blizzard,
        Fog,
        Wind,
        Sandstorm,
        AshFall
    }

    /// <summary>
    /// Configuration record defining single weather type attributes.
    /// </summary>
    public class WeatherProfile
    {
        public WeatherType Type { get; set; }
        public string DisplayName { get; set; } = "";
        
        public float TemperatureModifier { get; set; } = 0f;
        public float WindStrength { get; set; } = 0f; // 0.0 to 1.0 scale
        
        // Hooks
        public string VisualEffectHook { get; set; } = ""; // particle system reference
        public string AmbientSoundKey { get; set; } = "";
        public string LightTintHex { get; set; } = "#FFFFFF"; // Sky light modifier
    }

    /// <summary>
    /// Service managing current weather transitions, color overrides, and wind strengths.
    /// Loaded dynamically from configuration configurations.
    /// </summary>
    public class WeatherManager : IInitializable
    {
        private readonly Dictionary<WeatherType, WeatherProfile> _profiles = new();
        public WeatherProfile CurrentWeather { get; private set; } = new();

        public void Initialize()
        {
            LoadWeatherProfiles();
            ChangeWeather(WeatherType.Clear);
        }

        private void LoadWeatherProfiles()
        {
            _profiles.Clear();
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("weather_profiles");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<WeatherProfile>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            _profiles[item.Type] = item;
                        }
                        Logger.Info($"WeatherManager: Loaded {_profiles.Count} weather profiles.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"WeatherManager: Profile loading exception: {ex.Message}");
            }

            PopulateFallbackProfiles();
        }

        /// <summary>
        /// Transitions the active weather state and triggers visual/sound updates.
        /// </summary>
        public void ChangeWeather(WeatherType newWeather)
        {
            if (_profiles.TryGetValue(newWeather, out var profile))
            {
                CurrentWeather = profile;
                Logger.Info($"WeatherManager: Weather transitioned to: {profile.DisplayName} (Wind={profile.WindStrength})");
            }
            else
            {
                CurrentWeather = new WeatherProfile
                {
                    Type = newWeather,
                    DisplayName = newWeather.ToString()
                };
            }
        }

        private void PopulateFallbackProfiles()
        {
            Logger.Warning("WeatherManager: Using fallback weather profiles.");
            foreach (WeatherType wType in Enum.GetValues(typeof(WeatherType)))
            {
                _profiles[wType] = new WeatherProfile
                {
                    Type = wType,
                    DisplayName = wType.ToString(),
                    TemperatureModifier = wType == WeatherType.Snow ? -0.3f : 0f,
                    WindStrength = wType == WeatherType.Storm ? 0.8f : 0.1f
                };
            }
        }
    }
}
