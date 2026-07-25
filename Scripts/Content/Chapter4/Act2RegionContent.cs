using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public class Act2Region
    {
        public string RegionId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RecommendedLevelMin { get; set; }
        public int RecommendedLevelMax { get; set; }
        public Vector3 WorldPosition { get; set; }
        public bool IsUnlocked { get; set; } = false;
    }

    /// <summary>
    /// Act II region layout builder. Registers Eastern Ridgeline and Mirkwood Swamps
    /// — two distinct biomes bridging Act I completion to Act II content.
    /// </summary>
    public class Act2RegionContent
    {
        private readonly Dictionary<string, Act2Region> _regions = new(StringComparer.OrdinalIgnoreCase);

        public void InitializeRegions()
        {
            // Region 1 — Eastern Ridgeline
            RegisterRegion(new Act2Region
            {
                RegionId = "region_eastern_ridgeline",
                DisplayName = "Eastern Ridgeline",
                Description = "Storm-battered cliffs and mountain passes east of the Citadel Ruins. Malakor's scouts operate openly here.",
                RecommendedLevelMin = 19,
                RecommendedLevelMax = 24,
                WorldPosition = new Vector3(800, 30, 700),
                IsUnlocked = true
            });

            // Region 2 — Mirkwood Swamps
            RegisterRegion(new Act2Region
            {
                RegionId = "region_mirkwood_swamps",
                DisplayName = "Mirkwood Swamps",
                Description = "Dense fog-choked wetlands where Shadow Cult survivors regroup under a new commander.",
                RecommendedLevelMin = 21,
                RecommendedLevelMax = 27,
                WorldPosition = new Vector3(750, 0, 850),
                IsUnlocked = false // Unlocked after Eastern Ridgeline quest arc
            });

            Logger.Info("Act2RegionContent: 2 Act II regions initialized.");
        }

        public void RegisterRegion(Act2Region region)
        {
            if (region != null && !string.IsNullOrEmpty(region.RegionId))
                _regions[region.RegionId] = region;
        }

        public void UnlockRegion(string regionId)
        {
            if (_regions.TryGetValue(regionId, out var r))
            {
                r.IsUnlocked = true;
                Logger.Info($"Act2RegionContent: Region '{regionId}' unlocked.");
            }
        }

        public Act2Region? GetRegion(string regionId)
            => _regions.TryGetValue(regionId, out var r) ? r : null;

        public IReadOnlyCollection<Act2Region> AllRegions => _regions.Values;
    }
}
