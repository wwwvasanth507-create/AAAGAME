using System;
using Godot;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Governs density scaling rules and placement calculations for environmental static assets
    /// (Trees, Bushes, Grass, Pebbles) based on dynamic graphic settings.
    /// </summary>
    public static class VegetationSystem
    {
        /// <summary>
        /// Calculates density scaling multipliers based on active graphics preset string keys.
        /// </summary>
        public static float GetDensityMultiplier(string presetName)
        {
            if (string.IsNullOrEmpty(presetName)) return 1.0f;

            return presetName.ToUpperInvariant() switch
            {
                "LOW" => 0.25f,
                "MEDIUM" => 0.60f,
                "HIGH" => 1.00f,
                "ULTRA" => 1.50f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// Computes modified spawner count targets based on active graphics quality level.
        /// </summary>
        public static int ScaleSpawnCount(int baseCount, string graphicsPreset)
        {
            float multiplier = GetDensityMultiplier(graphicsPreset);
            int adjusted = (int)Math.Round(baseCount * multiplier);
            return Math.Max(0, adjusted);
        }

        /// <summary>
        /// Deterministically checks if a dynamic decoration node should spawn under current density thresholds.
        /// </summary>
        public static bool ShouldSpawnDecoration(float spawnFactor, string graphicsPreset, RandomNumberGenerator rng)
        {
            float multiplier = GetDensityMultiplier(graphicsPreset);
            double roll = rng.Randf();
            return roll <= (spawnFactor * multiplier);
        }
    }
}
