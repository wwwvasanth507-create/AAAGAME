using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.World.Content
{
    /// <summary>
    /// Thread-safe registry and query engine for Point of Interest definitions.
    /// </summary>
    public class PointOfInterestDatabase
    {
        private readonly Dictionary<string, POIDefinition> _pois = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterPOI(POIDefinition poi)
        {
            if (poi != null && !string.IsNullOrEmpty(poi.PoiId))
            {
                _pois[poi.PoiId] = poi;
            }
        }

        public POIDefinition? GetPOI(string poiId)
        {
            return _pois.TryGetValue(poiId, out var poi) ? poi : null;
        }

        public List<POIDefinition> GetPOIsByBiome(string biome)
        {
            return _pois.Values
                .Where(p => p.BiomeRestrictions.Count == 0 || p.BiomeRestrictions.Contains(biome, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        public List<POIDefinition> GetPOIsByType(POIType type)
        {
            return _pois.Values.Where(p => p.Type == type).ToList();
        }

        public void RegisterDefaultPOIs()
        {
            RegisterPOI(new POIDefinition
            {
                PoiId = "ruins_ancient_watchtower",
                DisplayName = "Ancient Watchtower",
                Type = POIType.Watchtower,
                BiomeRestrictions = new List<string> { "Plains", "Forest", "Hills" },
                SpawnWeight = 2.0f,
                Size = POISize.Medium,
                MusicHookTrack = "exploration_ruins"
            });

            RegisterPOI(new POIDefinition
            {
                PoiId = "bandit_camp_outpost",
                DisplayName = "Bandit Outpost",
                Type = POIType.BanditCamp,
                BiomeRestrictions = new List<string> { "Forest", "Hills" },
                SpawnWeight = 1.5f,
                DifficultyRating = 2,
                Size = POISize.Medium,
                LootTableId = "loot_bandit_tier1"
            });

            RegisterPOI(new POIDefinition
            {
                PoiId = "shrine_of_eternia",
                DisplayName = "Shrine of Eternia",
                Type = POIType.Shrine,
                SpawnWeight = 1.0f,
                Size = POISize.Small,
                AmbientAudioZone = "zone_shrine"
            });
        }
    }
}
