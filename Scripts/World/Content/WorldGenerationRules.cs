using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World.Content
{
    public class PlacementValidationContext
    {
        public Vector3 TargetPosition { get; set; }
        public string BiomeName { get; set; } = "Plains";
        public float Elevation { get; set; } = 10f;
        public float SlopeAngleDegrees { get; set; } = 5f;
        public float DistanceToWater { get; set; } = 100f;
        public float DistanceToRoad { get; set; } = 50f;
        public float DistanceToNearestSettlement { get; set; } = 200f;
    }

    /// <summary>
    /// Evaluates procedural placement rules based on terrain elevation, slope, spacing,
    /// biomes, and seed-reproducible pseudo-random values.
    /// </summary>
    public class WorldGenerationRules
    {
        public int WorldSeed { get; set; } = 42;
        public float MaxAllowedSlopeDegrees { get; set; } = 25f;
        public float MinGlobalPoiSpacing { get; set; } = 100f;

        public bool ValidatePOIPosition(POIDefinition poi, PlacementValidationContext context, List<POISpawnInstance> existingSpawns)
        {
            if (poi == null || context == null) return false;

            // Slope check
            if (context.SlopeAngleDegrees > MaxAllowedSlopeDegrees) return false;

            // Settlement distance check
            if (context.DistanceToNearestSettlement < poi.MinDistanceToSettlement) return false;

            // Spacing check against existing POIs
            foreach (var spawn in existingSpawns)
            {
                float dist = context.TargetPosition.DistanceTo(spawn.WorldPosition);
                if (dist < MinGlobalPoiSpacing) return false;

                if (spawn.PoiId.Equals(poi.PoiId, StringComparison.OrdinalIgnoreCase) && dist < poi.MinDistanceToSameType)
                {
                    return false;
                }
            }

            // Biome restriction check
            if (poi.BiomeRestrictions.Count > 0 && !poi.BiomeRestrictions.Contains(context.BiomeName, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public Random GetSeededRandom(int chunkX, int chunkZ)
        {
            int seedHash = HashCode.Combine(WorldSeed, chunkX, chunkZ);
            return new Random(seedHash);
        }
    }
}
