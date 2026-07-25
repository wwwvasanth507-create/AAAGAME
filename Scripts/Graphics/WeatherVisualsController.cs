using System;
using Godot;

namespace HeroOfEternia.Graphics
{
    public enum WeatherVisualType
    {
        Clear,
        Rain,
        Snow,
        Fog,
        Wind,
        Lightning,
        Sandstorm,
        Ash,
        MagicStorm
    }

    /// <summary>
    /// Visual controller for weather particle overlays, fog intensity, and storm flash effects.
    /// </summary>
    public partial class WeatherVisualsController : Node
    {
        public WeatherVisualType CurrentWeather { get; private set; } = WeatherVisualType.Clear;
        public float WeatherIntensity { get; private set; } = 0.0f;

        public event Action<WeatherVisualType, float>? OnWeatherVisualChanged;

        public void SetWeatherVisual(WeatherVisualType weather, float intensity = 1.0f, float transitionTime = 2.0f)
        {
            CurrentWeather = weather;
            WeatherIntensity = Math.Clamp(intensity, 0.0f, 1.0f);
            OnWeatherVisualChanged?.Invoke(CurrentWeather, WeatherIntensity);
        }

        public void ClearWeather(float transitionTime = 2.0f)
        {
            SetWeatherVisual(WeatherVisualType.Clear, 0.0f, transitionTime);
        }
    }
}
