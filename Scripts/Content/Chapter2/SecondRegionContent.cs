using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Content.Chapter2
{
    public class RegionLocationNode
    {
        public string LocationId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Vector3 Position { get; set; }
        public float Radius { get; set; } = 25f;
    }

    /// <summary>
    /// Builder and manager for Sylvanwood Wilds (second major region) locations including
    /// Sylvanwood Canopy, Ancient Elven Ruins, River Mist, Serpent Cave, and High Cliff Path.
    /// </summary>
    public class SecondRegionContent
    {
        private readonly Dictionary<string, RegionLocationNode> _locations = new(StringComparer.OrdinalIgnoreCase);

        public void InitializeSecondRegion()
        {
            RegisterLocation(new RegionLocationNode
            {
                LocationId = "loc_sylvanwood_canopy",
                DisplayName = "Sylvanwood Main Canopy",
                Position = new Vector3(200, 0, 300),
                Radius = 40f
            });

            RegisterLocation(new RegionLocationNode
            {
                LocationId = "loc_ancient_elven_ruins",
                DisplayName = "Ruins of Aethelgard",
                Position = new Vector3(350, 0, 450),
                Radius = 35f
            });

            RegisterLocation(new RegionLocationNode
            {
                LocationId = "loc_river_mist",
                DisplayName = "Mistveil River Crossing",
                Position = new RegionLocationNode().Position = new Vector3(150, 0, 200),
                Radius = 30f
            });

            RegisterLocation(new RegionLocationNode
            {
                LocationId = "loc_serpent_cave",
                DisplayName = "Venomous Cavern Entrance",
                Position = new Vector3(450, 0, 200),
                Radius = 15f
            });

            RegisterLocation(new RegionLocationNode
            {
                LocationId = "loc_high_cliff_path",
                DisplayName = "Stormwatch Ridge",
                Position = new Vector3(100, 30, 500),
                Radius = 25f
            });

            RegisterLocation(new RegionLocationNode
            {
                LocationId = "loc_abandoned_watchtower_north",
                DisplayName = "Northguard Spire",
                Position = new Vector3(300, 20, 150),
                Radius = 20f
            });
        }

        public void RegisterLocation(RegionLocationNode loc)
        {
            if (loc != null && !string.IsNullOrEmpty(loc.LocationId))
            {
                _locations[loc.LocationId] = loc;
            }
        }

        public RegionLocationNode? GetLocation(string locationId)
        {
            return _locations.TryGetValue(locationId, out var loc) ? loc : null;
        }

        public IReadOnlyCollection<RegionLocationNode> AllLocations => _locations.Values;
    }
}
