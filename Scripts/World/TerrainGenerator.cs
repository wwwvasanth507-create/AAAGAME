using System;
using Godot;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Layered noise terrain generator. Combines continent, mountain, and valley
    /// noise passes to compute heights deterministically from a 64-bit seed.
    /// </summary>
    public class TerrainGenerator
    {
        private readonly FastNoiseLite _baseNoise = new();
        private readonly FastNoiseLite _mountainNoise = new();
        private readonly FastNoiseLite _valleyNoise = new();
        
        public ulong Seed { get; private set; }

        public TerrainGenerator(ulong seed)
        {
            Seed = seed;
            
            // Map ulong to a 32-bit int seed for FastNoiseLite
            int noiseSeed = (int)(seed ^ (seed >> 32));

            // Base terrain (Low frequency hills)
            _baseNoise.Seed = noiseSeed;
            _baseNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
            _baseNoise.Frequency = 0.005f;

            // Mountains (High frequency ridges)
            _mountainNoise.Seed = noiseSeed + 1;
            _mountainNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
            _mountainNoise.Frequency = 0.015f;
            _mountainNoise.FractalType = FastNoiseLite.FractalTypeEnum.Ridged;

            // Valleys / Rivers (Carves trenches)
            _valleyNoise.Seed = noiseSeed + 2;
            _valleyNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
            _valleyNoise.Frequency = 0.01f;
        }

        /// <summary>
        /// Returns the deterministic terrain Y height at specific global coordinates.
        /// </summary>
        public float GetHeight(float x, float z)
        {
            // Layer 1: Base Continent elevation (-10 to +10 units)
            float baseElevation = _baseNoise.GetNoise2D(x, z) * 12f;

            // Layer 2: Mountain ranges (adds up to +35 units)
            float rawMountain = _mountainNoise.GetNoise2D(x, z); // -1.0 to 1.0 range
            float mountainMask = Math.Max(0f, _baseNoise.GetNoise2D(x + 100f, z + 100f)); // only spawns in specific areas
            float mountainElevation = rawMountain * 35f * mountainMask;

            // Layer 3: Valley/River Carving (carves down up to 10 units)
            float rawValley = Math.Abs(_valleyNoise.GetNoise2D(x, z)); // 0.0 to 1.0
            float valleyCarve = 0f;
            if (rawValley < 0.15f)
            {
                // Carve a smooth valley ditch
                float t = rawValley / 0.15f; // 0.0 to 1.0
                valleyCarve = (1f - t) * 12f;
            }

            float finalHeight = baseElevation + mountainElevation - valleyCarve;

            // Plateau override check: flat land tops
            if (finalHeight > 25f && finalHeight < 28f)
            {
                finalHeight = 25f; // Plateaus cap
            }

            return finalHeight;
        }

        /// <summary>
        /// Evaluates current biome based on coordinate noise temperature and humidity levels.
        /// </summary>
        public BiomeType GetBiomeAt(float x, float z)
        {
            // Deterministic temperature and humidity maps
            float tempNoise = (_baseNoise.GetNoise2D(x - 500f, z - 500f) + 1f) / 2f; // 0.0 to 1.0
            float humidNoise = (_baseNoise.GetNoise2D(x + 500f, z + 500f) + 1f) / 2f; // 0.0 to 1.0
            float height = GetHeight(x, z);

            if (height < -3f) return BiomeType.Ocean;
            if (height < 0f) return BiomeType.Beach;
            if (height > 20f) return BiomeType.Mountain;

            if (tempNoise < 0.25f)
            {
                return humidNoise > 0.5f ? BiomeType.Snow : BiomeType.Grassland;
            }
            if (tempNoise > 0.75f)
            {
                return humidNoise < 0.3f ? BiomeType.Desert : BiomeType.Volcano;
            }

            // Temperate zone
            if (humidNoise > 0.7f) return BiomeType.Swamp;
            if (humidNoise > 0.4f) return BiomeType.Forest;
            return BiomeType.Grassland;
        }
    }
}
