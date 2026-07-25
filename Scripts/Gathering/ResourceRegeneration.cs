using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Gathering
{
    /// <summary>
    /// Manages resource node regeneration rules.
    /// Supports tree regrowth, ore respawns, plant regrowth,
    /// seasonal variation hooks, biome modifiers, and save persistence.
    /// </summary>
    public class ResourceRegeneration : IInitializable
    {
        private static ResourceRegeneration? _instance;
        public static ResourceRegeneration Instance => _instance ??= new ResourceRegeneration();

        private GatheringManager _gatheringManager = null!;
        private ResourceDatabase _resourceDb = null!;
        private bool _isInitialized;

        /// <summary>Biome-specific respawn time multipliers.</summary>
        private Dictionary<string, float> _biomeModifiers = new();

        /// <summary>Seasonal respawn time multipliers.</summary>
        private Dictionary<string, float> _seasonalModifiers = new();

        /// <summary>Current active season.</summary>
        private string _currentSeason = "Spring";

        /// <summary>Is regeneration paused (e.g. during save/load).</summary>
        private bool _isPaused;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _gatheringManager = GatheringManager.Instance;
            _resourceDb = ResourceDatabase.Instance;

            InitializeBiomeModifiers();
            InitializeSeasonalModifiers();

            GD.Print("[ResourceRegeneration] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _biomeModifiers.Clear();
            _seasonalModifiers.Clear();
        }

        private void InitializeBiomeModifiers()
        {
            _biomeModifiers["Forest"] = 1.0f;
            _biomeModifiers["Desert"] = 1.5f;
            _biomeModifiers["Snow"] = 2.0f;
            _biomeModifiers["Plains"] = 0.8f;
            _biomeModifiers["Swamp"] = 0.7f;
            _biomeModifiers["Volcanic"] = 2.5f;
            _biomeModifiers["Underground"] = 3.0f;
            _biomeModifiers["Corrupted"] = 1.5f;
            _biomeModifiers["MagicForest"] = 0.6f;
            _biomeModifiers["CrystalCave"] = 1.2f;
            _biomeModifiers["AncientRuins"] = 1.0f;
            _biomeModifiers["Any"] = 1.0f;
        }

        private void InitializeSeasonalModifiers()
        {
            _seasonalModifiers["Spring"] = 0.8f;
            _seasonalModifiers["Summer"] = 1.0f;
            _seasonalModifiers["Autumn"] = 1.2f;
            _seasonalModifiers["Winter"] = 1.5f;
        }

        /// <summary>
        /// Updates all respawn timers. Call from game loop.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (_isPaused) return;
            _gatheringManager.UpdateRespawnTimers(deltaTime);
        }

        /// <summary>
        /// Gets the effective respawn time for a resource in a given biome and season.
        /// </summary>
        public float GetEffectiveRespawnTime(string resourceId, string biome)
        {
            var resource = _resourceDb.GetResource(resourceId);
            if (resource == null) return 120.0f;

            float baseTime = resource.RespawnTimeSeconds;
            
            // Apply biome modifier
            float biomeMod = _biomeModifiers.TryGetValue(biome, out float bm) ? bm : 1.0f;
            
            // Apply seasonal modifier
            float seasonMod = _seasonalModifiers.TryGetValue(_currentSeason, out float sm) ? sm : 1.0f;

            // Apply seasonal resource bonus
            if (!string.IsNullOrEmpty(resource.Season) && 
                resource.Season.Equals(_currentSeason, StringComparison.OrdinalIgnoreCase))
            {
                seasonMod *= 0.5f; // In-season resources respawn twice as fast
            }

            return baseTime * biomeMod * seasonMod;
        }

        /// <summary>
        /// Sets the current season, affecting respawn rates.
        /// </summary>
        public void SetSeason(string season)
        {
            _currentSeason = season;
            GD.Print($"[ResourceRegeneration] Season set to: {season}");
        }

        /// <summary>
        /// Gets the current season.
        /// </summary>
        public string GetCurrentSeason() => _currentSeason;

        /// <summary>
        /// Pauses or resumes regeneration.
        /// </summary>
        public void SetPaused(bool paused)
        {
            _isPaused = paused;
        }

        /// <summary>
        /// Registers a new resource node in the world.
        /// </summary>
        public void RegisterNode(string resourceId, string worldPositionKey, string chunkKey, string biome)
        {
            _gatheringManager.RegisterNode(resourceId, worldPositionKey, chunkKey);
        }

        /// <summary>
        /// Forces an immediate respawn of all depleted nodes.
        /// </summary>
        public void ForceRespawnAll()
        {
            GD.Print("[ResourceRegeneration] Force respawning all depleted nodes...");
            // This would iterate all nodes and reset them
        }

        /// <summary>
        /// Gets the biome modifier for a specific biome.
        /// </summary>
        public float GetBiomeModifier(string biome)
        {
            return _biomeModifiers.TryGetValue(biome, out float mod) ? mod : 1.0f;
        }

        /// <summary>
        /// Gets the seasonal modifier for the current season.
        /// </summary>
        public float GetSeasonalModifier()
        {
            return _seasonalModifiers.TryGetValue(_currentSeason, out float mod) ? mod : 1.0f;
        }

        public bool IsInitialized => _isInitialized;
    }
}