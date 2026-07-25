using System;
using System.Collections.Generic;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Configures spawn boundaries and density targets for a single natural resource type.
    /// </summary>
    public class ResourceSpawnRule
    {
        public string ElementRecordId { get; set; } = "";
        
        // Placement limitations
        public List<BiomeType> AllowedBiomes { get; set; } = new();
        public float MinElevation { get; set; } = 0.0f; // 0.0 to 1.0 elevation bounds
        public float MaxElevation { get; set; } = 1.0f;
        public float MaxSlopeAngle { get; set; } = 30.0f; // Max tilt alignment limit
        
        // Spawn Density Heuristics
        public float BaseChance { get; set; } = 0.5f; // Spawn probability rate (0.0 to 1.0)
        public int MinPerChunk { get; set; } = 0;
        public int MaxPerChunk { get; set; } = 3;
    }

    /// <summary>
    /// Framework resolving natural resource distributions (Trees, Stone, Iron, Gold, Herbs).
    /// Bypasses active gathering loop logic, focusing on placement criteria.
    /// </summary>
    public static class ResourceSpawner
    {
        /// <summary>
        /// Evaluates if a resource is eligible to spawn at specific terrain coordinates.
        /// </summary>
        public static bool CanSpawn(ResourceSpawnRule rule, BiomeType biome, float elevation, float slopeAngle)
        {
            if (rule == null) return false;

            // Biome match verification
            if (rule.AllowedBiomes.Count > 0 && !rule.AllowedBiomes.Contains(biome))
            {
                return false;
            }

            // Elevation limits verification
            if (elevation < rule.MinElevation || elevation > rule.MaxElevation)
            {
                return false;
            }

            // Slope angle limits verification
            if (slopeAngle > rule.MaxSlopeAngle)
            {
                return false;
            }

            return true;
        }
    }
}
