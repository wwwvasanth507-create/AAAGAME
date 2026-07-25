using System;
using System.Collections.Generic;

namespace HeroOfEternia.World.Content
{
    public class RegionalVariationProfile
    {
        public string RegionId { get; set; } = string.Empty;
        public float VegetationDensity { get; set; } = 1.0f;
        public float RockDensity { get; set; } = 1.0f;
        public float WaterFrequency { get; set; } = 0.5f;
        public float WildlifeDensity { get; set; } = 1.0f;
        public float FogDensityMultiplier { get; set; } = 1.0f;
        public string ArchitectureStyle { get; set; } = "EternianWood";
    }

    /// <summary>
    /// Regional variation manager configuring density multipliers, flora/fauna ratios,
    /// and architectural themes across world biomes.
    /// </summary>
    public class RegionalVariationManager
    {
        private readonly Dictionary<string, RegionalVariationProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterRegion(RegionalVariationProfile profile)
        {
            if (profile != null && !string.IsNullOrEmpty(profile.RegionId))
            {
                _profiles[profile.RegionId] = profile;
            }
        }

        public RegionalVariationProfile GetRegionProfile(string regionId)
        {
            return _profiles.TryGetValue(regionId, out var p) ? p : new RegionalVariationProfile { RegionId = regionId };
        }
    }
}
