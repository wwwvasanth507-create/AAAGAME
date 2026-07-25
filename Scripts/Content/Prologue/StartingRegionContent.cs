using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Content.Prologue
{
    public class LocationNode
    {
        public string LocationId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Vector3 Position { get; set; }
        public float Radius { get; set; } = 20f;
    }

    /// <summary>
    /// Builder and manager for Oakvale Village starting region, training fields, forge,
    /// river crossing, farmstead, and hidden cave entrance.
    /// </summary>
    public class StartingRegionContent
    {
        private readonly Dictionary<string, LocationNode> _locations = new(StringComparer.OrdinalIgnoreCase);

        public void InitializeStartingRegion()
        {
            RegisterLocation(new LocationNode
            {
                LocationId = "loc_oakvale_square",
                DisplayName = "Oakvale Village Square",
                Position = new Vector3(0, 0, 0),
                Radius = 25f
            });

            RegisterLocation(new LocationNode
            {
                LocationId = "loc_oakvale_training_field",
                DisplayName = "Training Field",
                Position = new Vector3(30, 0, -20),
                Radius = 15f
            });

            RegisterLocation(new LocationNode
            {
                LocationId = "loc_oakvale_blacksmith",
                DisplayName = "Thorin's Forge",
                Position = new Vector3(-25, 0, 15),
                Radius = 12f
            });

            RegisterLocation(new LocationNode
            {
                LocationId = "loc_oakvale_inn",
                DisplayName = "The Boar & Lantern Inn",
                Position = new Vector3(20, 0, 25),
                Radius = 15f
            });

            RegisterLocation(new LocationNode
            {
                LocationId = "loc_oakvale_shrine",
                DisplayName = "Shrine of Eternia",
                Position = new Vector3(0, 5, -50),
                Radius = 18f
            });

            RegisterLocation(new LocationNode
            {
                LocationId = "loc_hidden_cave",
                DisplayName = "Whispering Cavern Entrance",
                Position = new Vector3(-80, 0, -90),
                Radius = 10f
            });
        }

        public void RegisterLocation(LocationNode loc)
        {
            if (loc != null && !string.IsNullOrEmpty(loc.LocationId))
            {
                _locations[loc.LocationId] = loc;
            }
        }

        public LocationNode? GetLocation(string locationId)
        {
            return _locations.TryGetValue(locationId, out var loc) ? loc : null;
        }

        public IReadOnlyCollection<LocationNode> AllLocations => _locations.Values;
    }
}
