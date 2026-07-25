using System;
using Godot;

namespace HeroOfEternia.Graphics
{
    public enum LightingContext
    {
        Morning,
        Day,
        Evening,
        Night,
        Dungeon,
        Indoor,
        Settlement,
        Storm,
        Fog,
        BossArena,
        Cinematic
    }

    /// <summary>
    /// Data-driven profile defining environmental lighting, sunlight color/intensity,
    /// fog density, and ambient color settings.
    /// </summary>
    public class LightingProfile
    {
        public LightingContext Context { get; set; } = LightingContext.Day;
        public Color SunlightColor { get; set; } = Colors.White;
        public float SunlightEnergy { get; set; } = 1.0f;
        public Color AmbientColor { get; set; } = new Color(0.4f, 0.4f, 0.5f);
        public Color FogColor { get; set; } = new Color(0.7f, 0.8f, 0.9f);
        public float FogDensity { get; set; } = 0.01f;
        public bool EnableShadows { get; set; } = true;

        public static LightingProfile Lerp(LightingProfile a, LightingProfile b, float t)
        {
            t = Math.Clamp(t, 0.0f, 1.0f);
            return new LightingProfile
            {
                Context = t > 0.5f ? b.Context : a.Context,
                SunlightColor = a.SunlightColor.Lerp(b.SunlightColor, t),
                SunlightEnergy = Mathf.Lerp(a.SunlightEnergy, b.SunlightEnergy, t),
                AmbientColor = a.AmbientColor.Lerp(b.AmbientColor, t),
                FogColor = a.FogColor.Lerp(b.FogColor, t),
                FogDensity = Mathf.Lerp(a.FogDensity, b.FogDensity, t),
                EnableShadows = b.EnableShadows
            };
        }

        public static LightingProfile CreateDefaultDayProfile()
        {
            return new LightingProfile
            {
                Context = LightingContext.Day,
                SunlightColor = new Color(1.0f, 0.98f, 0.9f),
                SunlightEnergy = 1.2f,
                AmbientColor = new Color(0.35f, 0.4f, 0.5f),
                FogColor = new Color(0.75f, 0.85f, 0.95f),
                FogDensity = 0.005f
            };
        }

        public static LightingProfile CreateDefaultNightProfile()
        {
            return new LightingProfile
            {
                Context = LightingContext.Night,
                SunlightColor = new Color(0.2f, 0.3f, 0.5f),
                SunlightEnergy = 0.3f,
                AmbientColor = new Color(0.08f, 0.12f, 0.2f),
                FogColor = new Color(0.1f, 0.15f, 0.25f),
                FogDensity = 0.015f
            };
        }

        public static LightingProfile CreateDefaultDungeonProfile()
        {
            return new LightingProfile
            {
                Context = LightingContext.Dungeon,
                SunlightColor = new Color(0.1f, 0.1f, 0.1f),
                SunlightEnergy = 0.1f,
                AmbientColor = new Color(0.05f, 0.05f, 0.08f),
                FogColor = new Color(0.05f, 0.08f, 0.12f),
                FogDensity = 0.04f
            };
        }
    }
}
