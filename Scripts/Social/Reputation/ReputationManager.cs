using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Social.Reputation
{
    /// <summary>
    /// Reputation tier threshold configuration. Data-driven for custom ranges.
    /// </summary>
    public class ReputationTier
    {
        public string Name { get; set; } = "Neutral";
        public int MinValue { get; set; } = -10;
        public int MaxValue { get; set; } = 10;
        public string LocalizationKey { get; set; } = "rep_neutral";
    }

    /// <summary>
    /// Event raised when reputation changes in any scope.
    /// </summary>
    public class ReputationChangedEvent
    {
        public ReputationScope Scope { get; set; }
        public string ScopeKey { get; set; } = "";
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public int Delta { get; set; }
        public string ModifierTag { get; set; } = "";
    }

    public enum ReputationScope
    {
        Global,
        Region,
        Faction,
        Settlement,
        Individual
    }

    /// <summary>
    /// Central reputation tracking manager. Supports individual NPCs, factions,
    /// settlements, regions, and global reputation. Configurable tier thresholds.
    /// Thread-safe for concurrent access. Event-driven for decoupled consumption.
    /// </summary>
    public class ReputationManager
    {
        public const string ServiceKey = "ReputationManager";

        // Default reputation range: -1000 to +1000
        private const int MinRep = -1000;
        private const int MaxRep = 1000;

        // Scope storage
        private int _globalReputation = 0;
        private readonly Dictionary<string, int> _regions = new();
        private readonly Dictionary<string, int> _factions = new();
        private readonly Dictionary<string, int> _settlements = new();
        private readonly Dictionary<string, int> _individuals = new();

        private readonly object _lock = new();

        // Tier configuration
        private List<ReputationTier> _tiers = DefaultTiers();

        /// <summary>Fired after any reputation change across any scope.</summary>
        public event Action<ReputationChangedEvent>? OnReputationChanged;

        // ─────────────────────── Tier Configuration ───────────────────────

        /// <summary>
        /// Allows setting custom reputation tiers. Must include at least one tier.
        /// </summary>
        public void SetCustomTiers(List<ReputationTier> tiers)
        {
            if (tiers == null || tiers.Count == 0)
            {
                Logger.Warn("ReputationManager: Cannot set empty tier list. Using defaults.");
                return;
            }

            lock (_lock)
            {
                _tiers = new List<ReputationTier>(tiers);
            }
        }

        public List<ReputationTier> GetCurrentTiers()
        {
            lock (_lock) { return new List<ReputationTier>(_tiers); }
        }

        /// <summary>
        /// Returns the tier name for a given reputation value.
        /// </summary>
        public string GetTierName(int reputation)
        {
            lock (_lock)
            {
                foreach (var tier in _tiers)
                {
                    if (reputation >= tier.MinValue && reputation <= tier.MaxValue)
                        return tier.Name;
                }
                return reputation < _tiers[0].MinValue ? _tiers[0].Name : _tiers[^1].Name;
            }
        }

        // ─────────────────────── Adjust Reputation ───────────────────────

        public int AdjustGlobal(int delta, string modifierTag = "")
        {
            lock (_lock)
            {
                int old = _globalReputation;
                _globalReputation = Math.Clamp(_globalReputation + delta, MinRep, MaxRep);
                Fire(ReputationScope.Global, "global", old, _globalReputation, delta, modifierTag);
                return _globalReputation;
            }
        }

        public int AdjustRegion(string regionId, int delta, string modifierTag = "")
        {
            lock (_lock)
            {
                _regions.TryGetValue(regionId, out int old);
                int next = Math.Clamp(old + delta, MinRep, MaxRep);
                _regions[regionId] = next;
                Fire(ReputationScope.Region, regionId, old, next, delta, modifierTag);
                return next;
            }
        }

        public int AdjustFaction(string factionId, int delta, string modifierTag = "")
        {
            lock (_lock)
            {
                _factions.TryGetValue(factionId, out int old);
                int next = Math.Clamp(old + delta, MinRep, MaxRep);
                _factions[factionId] = next;
                Fire(ReputationScope.Faction, factionId, old, next, delta, modifierTag);
                return next;
            }
        }

        public int AdjustSettlement(string settlementId, int delta, string modifierTag = "")
        {
            lock (_lock)
            {
                _settlements.TryGetValue(settlementId, out int old);
                int next = Math.Clamp(old + delta, MinRep, MaxRep);
                _settlements[settlementId] = next;
                Fire(ReputationScope.Settlement, settlementId, old, next, delta, modifierTag);
                return next;
            }
        }

        public int AdjustIndividual(string npcId, int delta, string modifierTag = "")
        {
            lock (_lock)
            {
                _individuals.TryGetValue(npcId, out int old);
                int next = Math.Clamp(old + delta, MinRep, MaxRep);
                _individuals[npcId] = next;
                Fire(ReputationScope.Individual, npcId, old, next, delta, modifierTag);
                return next;
            }
        }

        // ─────────────────────── Query Reputation ───────────────────────

        public int GetGlobal()
        {
            lock (_lock) { return _globalReputation; }
        }

        public int GetRegion(string regionId)
        {
            lock (_lock) { return _regions.TryGetValue(regionId, out var v) ? v : 0; }
        }

        public int GetFaction(string factionId)
        {
            lock (_lock) { return _factions.TryGetValue(factionId, out var v) ? v : 0; }
        }

        public int GetSettlement(string settlementId)
        {
            lock (_lock) { return _settlements.TryGetValue(settlementId, out var v) ? v : 0; }
        }

        public int GetIndividual(string npcId)
        {
            lock (_lock) { return _individuals.TryGetValue(npcId, out var v) ? v : 0; }
        }

        public string GetGlobalTier()
        {
            lock (_lock) { return GetTierName(_globalReputation); }
        }

        public string GetFactionTier(string factionId)
        {
            lock (_lock) { return GetTierName(GetFaction(factionId)); }
        }

        // ─────────────────────── Bulk Operations ───────────────────────

        /// <summary>
        /// Adjusts reputation across multiple scopes simultaneously.
        /// Useful for actions that affect the whole world (e.g., saving a kingdom).
        /// </summary>
        public void AdjustMulti(Dictionary<ReputationScope, Dictionary<string, int>> adjustments, string modifierTag = "")
        {
            lock (_lock)
            {
                foreach (var scopeEntry in adjustments)
                {
                    foreach (var kv in scopeEntry.Value)
                    {
                        switch (scopeEntry.Key)
                        {
                            case ReputationScope.Global:
                                _globalReputation = Math.Clamp(_globalReputation + kv.Value, MinRep, MaxRep);
                                break;
                            case ReputationScope.Region:
                                _regions.TryGetValue(kv.Key, out int rOld);
                                _regions[kv.Key] = Math.Clamp(rOld + kv.Value, MinRep, MaxRep);
                                break;
                            case ReputationScope.Faction:
                                _factions.TryGetValue(kv.Key, out int fOld);
                                _factions[kv.Key] = Math.Clamp(fOld + kv.Value, MinRep, MaxRep);
                                break;
                            case ReputationScope.Settlement:
                                _settlements.TryGetValue(kv.Key, out int sOld);
                                _settlements[kv.Key] = Math.Clamp(sOld + kv.Value, MinRep, MaxRep);
                                break;
                        }
                    }
                }
            }
        }

        // ─────────────────────── Save / Load ───────────────────────

        /// <summary>
        /// Exports all reputation data to a flat dictionary for serialization.
        /// </summary>
        public Dictionary<string, int> ExportSnapshot()
        {
            lock (_lock)
            {
                var snap = new Dictionary<string, int>();
                snap["global"] = _globalReputation;
                foreach (var kv in _regions) snap[$"reg:{kv.Key}"] = kv.Value;
                foreach (var kv in _factions) snap[$"fac:{kv.Key}"] = kv.Value;
                foreach (var kv in _settlements) snap[$"set:{kv.Key}"] = kv.Value;
                foreach (var kv in _individuals) snap[$"ind:{kv.Key}"] = kv.Value;
                return snap;
            }
        }

        /// <summary>
        /// Restores reputation data from a saved snapshot.
        /// </summary>
        public void RestoreSnapshot(Dictionary<string, int>? snapshot)
        {
            if (snapshot == null) return;
            lock (_lock)
            {
                foreach (var kv in snapshot)
                {
                    if (kv.Key == "global") { _globalReputation = kv.Value; }
                    else if (kv.Key.StartsWith("reg:")) _regions[kv.Key[4..]] = kv.Value;
                    else if (kv.Key.StartsWith("fac:")) _factions[kv.Key[4..]] = kv.Value;
                    else if (kv.Key.StartsWith("set:")) _settlements[kv.Key[4..]] = kv.Value;
                    else if (kv.Key.StartsWith("ind:")) _individuals[kv.Key[4..]] = kv.Value;
                }
            }
        }

        // ─────────────────────── Private ───────────────────────

        private void Fire(ReputationScope scope, string key, int old, int next, int delta, string tag)
        {
            if (old == next) return;
            var evt = new ReputationChangedEvent
            {
                Scope = scope,
                ScopeKey = key,
                OldValue = old,
                NewValue = next,
                Delta = delta,
                ModifierTag = tag
            };
            OnReputationChanged?.Invoke(evt);
        }

        private static List<ReputationTier> DefaultTiers()
        {
            return new List<ReputationTier>
            {
                new() { Name = "Hated",    MinValue = -1000, MaxValue = -801, LocalizationKey = "rep_hated" },
                new() { Name = "Hostile",  MinValue = -800,  MaxValue = -401, LocalizationKey = "rep_hostile" },
                new() { Name = "Distrusted", MinValue = -400, MaxValue = -101, LocalizationKey = "rep_distrusted" },
                new() { Name = "Neutral",  MinValue = -100,  MaxValue = 100,  LocalizationKey = "rep_neutral" },
                new() { Name = "Friendly", MinValue = 101,   MaxValue = 300,  LocalizationKey = "rep_friendly" },
                new() { Name = "Honored",  MinValue = 301,   MaxValue = 600,  LocalizationKey = "rep_honored" },
                new() { Name = "Revered",  MinValue = 601,   MaxValue = 800,  LocalizationKey = "rep_revered" },
                new() { Name = "Legendary", MinValue = 801,  MaxValue = 1000, LocalizationKey = "rep_legendary" }
            };
        }
    }
}