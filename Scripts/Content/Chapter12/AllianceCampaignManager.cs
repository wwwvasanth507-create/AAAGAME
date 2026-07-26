using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter12
{
    public class FactionAllianceRecord
    {
        public string FactionId { get; set; } = "";
        public string Name { get; set; } = "";
        public int LoyaltyRating { get; set; } = 50; // 0 - 100
        public int TroopsPledged { get; set; } = 100;
        public bool CouncilDelegateAssigned { get; set; } = false;
        public string OutpostBaseZoneId { get; set; } = "";
    }

    /// <summary>
    /// Alliance Campaign Manager for Chapter 12 & Act IV.
    /// Manages the Grand Alliance Council, faction standings (Valenhold, Eternia Prime, Shadow Frontier Scouts, Archivists),
    /// alliance readiness percentage (0-100%), supply allocations, and military outposts.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class AllianceCampaignManager : IInitializable
    {
        private readonly Dictionary<string, FactionAllianceRecord> _factions = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<FactionAllianceRecord>? OnFactionLoyaltyChanged;
        public event Action<int>? OnAllianceReadinessUpdated;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultFactions();

            // Register with ServiceLocator
            ServiceLocator.Register<AllianceCampaignManager>(this);

            IsInitialized = true;
            Logger.Info("AllianceCampaignManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _factions.Clear();

            ServiceLocator.Unregister<AllianceCampaignManager>();
            IsInitialized = false;
            Logger.Info("AllianceCampaignManager: Shutdown completed.");
        }

        private void RegisterDefaultFactions()
        {
            // 1. Valenhold Settlement Alliance
            RegisterFaction(new FactionAllianceRecord
            {
                FactionId = "faction_valenhold",
                Name = "Valenhold Militia",
                LoyaltyRating = 85,
                TroopsPledged = 250,
                CouncilDelegateAssigned = true,
                OutpostBaseZoneId = "zone_crystal_wasteland"
            });

            // 2. Eternia Prime High Command
            RegisterFaction(new FactionAllianceRecord
            {
                FactionId = "faction_eternia_prime",
                Name = "Eternia Royal Guard",
                LoyaltyRating = 90,
                TroopsPledged = 500,
                CouncilDelegateAssigned = true,
                OutpostBaseZoneId = "zone_astral_battlefield"
            });

            // 3. Shadow Frontier Rangers
            RegisterFaction(new FactionAllianceRecord
            {
                FactionId = "faction_shadow_rangers",
                Name = "Shadow Frontier Scouts",
                LoyaltyRating = 75,
                TroopsPledged = 180,
                CouncilDelegateAssigned = true,
                OutpostBaseZoneId = "zone_caelum_ruins"
            });

            // 4. Sun Archivists
            RegisterFaction(new FactionAllianceRecord
            {
                FactionId = "faction_sun_archivists",
                Name = "Archivists of Sol",
                LoyaltyRating = 80,
                TroopsPledged = 120,
                CouncilDelegateAssigned = true,
                OutpostBaseZoneId = "zone_forgotten_sun_temple"
            });
        }

        public void RegisterFaction(FactionAllianceRecord faction)
        {
            if (faction != null && !string.IsNullOrEmpty(faction.FactionId))
            {
                _factions[faction.FactionId] = faction;
            }
        }

        public bool SetFactionLoyalty(string factionId, int newLoyalty)
        {
            if (!_factions.TryGetValue(factionId, out var fac)) return false;

            fac.LoyaltyRating = Math.Clamp(newLoyalty, 0, 100);
            OnFactionLoyaltyChanged?.Invoke(fac);

            int readiness = GetAllianceReadinessPercentage();
            OnAllianceReadinessUpdated?.Invoke(readiness);

            Logger.Info($"AllianceCampaignManager: Faction '{fac.Name}' loyalty updated to {fac.LoyaltyRating}. Grand Alliance Readiness: {readiness}%.");
            return true;
        }

        public int GetAllianceReadinessPercentage()
        {
            if (_factions.Count == 0) return 0;
            int sum = 0;
            foreach (var fac in _factions.Values)
            {
                sum += fac.LoyaltyRating;
            }
            return (int)Math.Round((double)sum / _factions.Count);
        }

        public FactionAllianceRecord? GetFaction(string factionId)
        {
            return _factions.TryGetValue(factionId, out var f) ? f : null;
        }

        public List<FactionAllianceRecord> GetAllFactions()
        {
            return new List<FactionAllianceRecord>(_factions.Values);
        }
    }
}
