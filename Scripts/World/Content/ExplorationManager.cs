using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.World.Content
{
    public class DiscoveryEvent
    {
        public string LocationId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Vector3 Position { get; set; }
        public int XpReward { get; set; } = 50;
    }

    /// <summary>
    /// Exploration manager tracking discovered POIs, landmarks, fog-of-war map reveals,
    /// region completion stats, and achievement triggers.
    /// </summary>
    public class ExplorationManager
    {
        private readonly HashSet<string> _discoveredLocations = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _clearedDungeons = new(StringComparer.OrdinalIgnoreCase);

        public event Action<DiscoveryEvent>? OnLocationDiscovered;

        public bool DiscoverLocation(string locationId, string displayName, Vector3 position, int xpReward = 50)
        {
            if (_discoveredLocations.Add(locationId))
            {
                var evt = new DiscoveryEvent
                {
                    LocationId = locationId,
                    DisplayName = displayName,
                    Position = position,
                    XpReward = xpReward
                };

                OnLocationDiscovered?.Invoke(evt);

                // Send notification through NotificationManager convenience hook
                HeroOfEternia.Core.EventBus.Publish(evt);
                return true;
            }
            return false;
        }

        public bool IsDiscovered(string locationId)
        {
            return _discoveredLocations.Contains(locationId);
        }

        public int DiscoveredCount => _discoveredLocations.Count;

        public void LoadDiscoveredLocations(IEnumerable<string> locations)
        {
            _discoveredLocations.Clear();
            if (locations != null)
            {
                foreach (var loc in locations) _discoveredLocations.Add(loc);
            }
        }

        public IEnumerable<string> DiscoveredLocations => _discoveredLocations;
    }
}
