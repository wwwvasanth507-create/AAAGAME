using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public class FactionInfluenceRecord
    {
        public string FactionId { get; set; } = "";
        public string FactionName { get; set; } = "";
        public int InfluenceScore { get; set; } = 50; // 0 to 100
        public int ReputationWithPlayer { get; set; } = 0; // -100 to +100
        public bool IsAlliedWithPlayer { get; set; } = false;
        public bool IsAtWar { get; set; } = false;
    }

    /// <summary>
    /// Faction Politics Engine for Act II.
    /// Manages territorial influence, political disputes, alliance negotiations, and faction rewards.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class FactionPoliticsManager : IInitializable
    {
        private readonly Dictionary<string, FactionInfluenceRecord> _factions = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<string, int>? OnInfluenceChanged;
        public event Action<string, string>? OnTerritoryConceded;
        public event Action<string>? OnAllianceFormed;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultFactions();

            // Register with ServiceLocator
            ServiceLocator.Register<FactionPoliticsManager>(this);

            IsInitialized = true;
            Logger.Info("FactionPoliticsManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _factions.Clear();

            ServiceLocator.Unregister<FactionPoliticsManager>();
            IsInitialized = false;
            Logger.Info("FactionPoliticsManager: Shutdown completed.");
        }

        private void RegisterDefaultFactions()
        {
            // 1. The Iron Vanguard (Military & Order)
            RegisterFaction(new FactionInfluenceRecord
            {
                FactionId = "faction_iron_vanguard",
                FactionName = "The Iron Vanguard",
                InfluenceScore = 65,
                ReputationWithPlayer = 20,
                IsAlliedWithPlayer = false
            });

            // 2. The Silver Syndicate (Trade & Wealth)
            RegisterFaction(new FactionInfluenceRecord
            {
                FactionId = "faction_silver_syndicate",
                FactionName = "The Silver Syndicate",
                InfluenceScore = 55,
                ReputationWithPlayer = 10,
                IsAlliedWithPlayer = false
            });

            // 3. The Sylvan Circle (Nature & Alchemy)
            RegisterFaction(new FactionInfluenceRecord
            {
                FactionId = "faction_sylvan_circle",
                FactionName = "The Sylvan Circle",
                InfluenceScore = 45,
                ReputationWithPlayer = 15,
                IsAlliedWithPlayer = false
            });
        }

        public void RegisterFaction(FactionInfluenceRecord record)
        {
            if (record != null && !string.IsNullOrEmpty(record.FactionId))
            {
                _factions[record.FactionId] = record;
            }
        }

        public bool ModifyInfluence(string factionId, int delta)
        {
            if (!_factions.TryGetValue(factionId, out var record))
            {
                Logger.Warning($"FactionPoliticsManager: Faction '{factionId}' not found.");
                return false;
            }

            record.InfluenceScore = Math.Clamp(record.InfluenceScore + delta, 0, 100);
            OnInfluenceChanged?.Invoke(factionId, record.InfluenceScore);

            Logger.Info($"FactionPoliticsManager: Faction '{record.FactionName}' influence updated to {record.InfluenceScore}.");
            return true;
        }

        public bool FormAlliance(string factionId)
        {
            if (!_factions.TryGetValue(factionId, out var record)) return false;

            record.IsAlliedWithPlayer = true;
            OnAllianceFormed?.Invoke(factionId);
            Logger.Info($"FactionPoliticsManager: Formed official alliance with '{record.FactionName}'.");
            return true;
        }

        public FactionInfluenceRecord? GetFaction(string factionId)
        {
            return _factions.TryGetValue(factionId, out var record) ? record : null;
        }

        public List<FactionInfluenceRecord> GetAllFactions()
        {
            return new List<FactionInfluenceRecord>(_factions.Values);
        }
    }
}
