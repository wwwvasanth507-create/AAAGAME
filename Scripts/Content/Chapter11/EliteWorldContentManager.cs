using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter11
{
    public class EliteEncounterRecord
    {
        public string EncounterId { get; set; } = "";
        public string Name { get; set; } = "";
        public string ZoneId { get; set; } = "";
        public string EnemyType { get; set; } = "WorldMiniBoss";
        public int RecommendedLevel { get; set; } = 42;
        public bool IsCleared { get; set; } = false;
        public string RewardLegendaryMaterial { get; set; } = "";
    }

    /// <summary>
    /// Elite World Content Manager for Act IV.
    /// Manages elite patrols, world mini-boss encounters, challenge arenas, and legendary reward nodes across The Astral Divide.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class EliteWorldContentManager : IInitializable
    {
        private readonly Dictionary<string, EliteEncounterRecord> _encounters = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<EliteEncounterRecord>? OnEncounterCleared;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultEncounters();

            // Register with ServiceLocator
            ServiceLocator.Register<EliteWorldContentManager>(this);

            IsInitialized = true;
            Logger.Info("EliteWorldContentManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _encounters.Clear();

            ServiceLocator.Unregister<EliteWorldContentManager>();
            IsInitialized = false;
            Logger.Info("EliteWorldContentManager: Shutdown completed.");
        }

        private void RegisterDefaultEncounters()
        {
            // 1. Crystal Behemoth Mini-Boss
            RegisterEncounter(new EliteEncounterRecord
            {
                EncounterId = "elite_crystal_behemoth",
                Name = "Apex Crystal Behemoth",
                ZoneId = "zone_crystal_wasteland",
                EnemyType = "WorldMiniBoss",
                RecommendedLevel = 41,
                RewardLegendaryMaterial = "material_astral_essence"
            });

            // 2. High Priest Lich Mini-Boss
            RegisterEncounter(new EliteEncounterRecord
            {
                EncounterId = "elite_sun_priest_lich",
                Name = "Corrupted Sun High Priest",
                ZoneId = "zone_forgotten_sun_temple",
                EnemyType = "WorldMiniBoss",
                RecommendedLevel = 44,
                RewardLegendaryMaterial = "material_sun_core_fragment"
            });
        }

        public void RegisterEncounter(EliteEncounterRecord encounter)
        {
            if (encounter != null && !string.IsNullOrEmpty(encounter.EncounterId))
            {
                _encounters[encounter.EncounterId] = encounter;
            }
        }

        public bool ClearEncounter(string encounterId)
        {
            if (!_encounters.TryGetValue(encounterId, out var enc)) return false;
            if (enc.IsCleared) return true;

            enc.IsCleared = true;
            OnEncounterCleared?.Invoke(enc);

            Logger.Info($"EliteWorldContentManager: Player defeated elite encounter '{enc.Name}' ({encounterId}) in {enc.ZoneId}! Awarded material: {enc.RewardLegendaryMaterial}.");
            return true;
        }

        public EliteEncounterRecord? GetEncounter(string encounterId)
        {
            return _encounters.TryGetValue(encounterId, out var e) ? e : null;
        }

        public List<EliteEncounterRecord> GetAllEncounters()
        {
            return new List<EliteEncounterRecord>(_encounters.Values);
        }
    }
}
