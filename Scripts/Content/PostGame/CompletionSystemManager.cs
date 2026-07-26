using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.PostGame
{
    public class RegionCompletionRecord
    {
        public string RegionId { get; set; } = "";
        public string RegionName { get; set; } = "";
        public float CompletionPercentage { get; set; } = 0.0f;
    }

    /// <summary>
    /// 100% Completion Tracker Engine for Hero of Eternia.
    /// Manages regional completion percentages, overall world completion calculation, lore codex completion, and master rewards.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class CompletionSystemManager : IInitializable
    {
        private readonly Dictionary<string, RegionCompletionRecord> _regionCompletions = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<float>? OnOverallCompletionUpdated;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultRegions();

            // Register with ServiceLocator
            ServiceLocator.Register<CompletionSystemManager>(this);

            IsInitialized = true;
            Logger.Info("CompletionSystemManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _regionCompletions.Clear();

            ServiceLocator.Unregister<CompletionSystemManager>();
            IsInitialized = false;
            Logger.Info("CompletionSystemManager: Shutdown completed.");
        }

        private void RegisterDefaultRegions()
        {
            _regionCompletions.Clear();

            RegisterRegion(new RegionCompletionRecord { RegionId = "region_01_greenwood", RegionName = "Greenwood Vale", CompletionPercentage = 100.0f });
            RegisterRegion(new RegionCompletionRecord { RegionId = "region_02_valenhold", RegionName = "Valenhold & Outlands", CompletionPercentage = 95.0f });
            RegisterRegion(new RegionCompletionRecord { RegionId = "region_03_eternia_prime", RegionName = "Eternia Prime Capital", CompletionPercentage = 92.0f });
            RegisterRegion(new RegionCompletionRecord { RegionId = "region_04_shadow_frontier", RegionName = "The Shadow Frontier", CompletionPercentage = 88.0f });
            RegisterRegion(new RegionCompletionRecord { RegionId = "region_05_astral_divide", RegionName = "The Astral Divide", CompletionPercentage = 85.0f });
            RegisterRegion(new RegionCompletionRecord { RegionId = "region_06_obsidian_citadel", RegionName = "The Obsidian Citadel", CompletionPercentage = 90.0f });
        }

        public void RegisterRegion(RegionCompletionRecord region)
        {
            if (region != null && !string.IsNullOrEmpty(region.RegionId))
            {
                _regionCompletions[region.RegionId] = region;
            }
        }

        public void UpdateRegionCompletion(string regionId, float percentage)
        {
            if (!_regionCompletions.TryGetValue(regionId, out var r)) return;

            r.CompletionPercentage = Math.Clamp(percentage, 0.0f, 100.0f);
            float overall = GetOverallCompletionPercentage();
            OnOverallCompletionUpdated?.Invoke(overall);
            Logger.Info($"CompletionSystemManager: Region '{r.RegionName}' updated to {r.CompletionPercentage:F1}%. Overall World Completion: {overall:F1}%.");
        }

        public float GetOverallCompletionPercentage()
        {
            if (_regionCompletions.Count == 0) return 0.0f;
            float total = 0.0f;
            foreach (var r in _regionCompletions.Values)
            {
                total += r.CompletionPercentage;
            }
            return total / _regionCompletions.Count;
        }

        public List<RegionCompletionRecord> GetAllRegions()
        {
            return new List<RegionCompletionRecord>(_regionCompletions.Values);
        }
    }
}
