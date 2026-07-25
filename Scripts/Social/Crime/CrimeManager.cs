using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Social.Crime
{
    /// <summary>
    /// Crime types supported by the crime system.
    /// </summary>
    public enum CrimeType
    {
        Theft,
        Trespassing,
        Assault,
        Murder,
        PropertyDamage,
        IllegalTrading,
        RestrictedAreaEntry
    }

    /// <summary>
    /// Severity level of a crime.
    /// </summary>
    public enum CrimeSeverity
    {
        Minor,
        Moderate,
        Serious,
        Severe,
        Capital
    }

    /// <summary>
    /// Record of a committed crime.
    /// </summary>
    public class CrimeRecord
    {
        public string CrimeId { get; set; } = Guid.NewGuid().ToString("N");
        public CrimeType Type { get; set; }
        public CrimeSeverity Severity { get; set; }
        public string PerpetratorId { get; set; } = "player";
        public string VictimId { get; set; } = "";
        public string LocationId { get; set; } = "";
        public string RegionId { get; set; } = "";
        public string SettlementId { get; set; } = "";
        public double WorldTime { get; set; }
        public int BountyValue { get; set; } = 0;
        public bool IsResolved { get; set; } = false;
        public List<string> WitnessIds { get; set; } = new();
        public string ModifierTag { get; set; } = "";
        public bool Expired { get; set; } = false;
        public double ExpirationTime { get; set; } = -1; // -1 = never expires
    }

    /// <summary>
    /// Event fired when a crime is committed.
    /// </summary>
    public class CrimeCommittedEvent
    {
        public CrimeRecord Crime { get; set; } = new();
        public bool HasWitnesses { get; set; }
        public bool IsReported { get; set; }
    }

    /// <summary>
    /// Event fired when a bounty is updated.
    /// </summary>
    public class BountyUpdatedEvent
    {
        public string TargetId { get; set; } = "";
        public string FactionId { get; set; } = "";
        public int OldBounty { get; set; }
        public int NewBounty { get; set; }
    }

    /// <summary>
    /// Manages crime detection, tracking, bounties, and expiration.
    /// Supports witness detection, severity calculation, and future prison integration.
    /// </summary>
    public class CrimeManager
    {
        public const string ServiceKey = "CrimeManager";

        private readonly List<CrimeRecord> _crimeHistory = new();
        private readonly Dictionary<string, int> _activeBounties = new(); // targetId -> total bounty
        private readonly Dictionary<string, Dictionary<string, int>> _factionBounties = new(); // factionId -> targetId -> bounty
        private readonly Dictionary<string, List<CrimeRecord>> _perpetratorCrimes = new();
        
        private readonly object _lock = new();

        /// <summary>
        /// Fired whenever a crime is committed.
        /// </summary>
        public event Action<CrimeCommittedEvent>? OnCrimeCommitted;

        /// <summary>
        /// Fired when a bounty changes.
        /// </summary>
        public event Action<BountyUpdatedEvent>? OnBountyUpdated;

        // ─────────────────────── Crime Definitions ───────────────────────

        /// <summary>
        /// Returns the base severity and bounty for a crime type.
        /// Values are data-driven reference points that can be scaled.
        /// </summary>
        public static (CrimeSeverity Severity, int BaseBounty) GetCrimeDefaults(CrimeType type)
        {
            return type switch
            {
                CrimeType.Theft => (CrimeSeverity.Minor, 25),
                CrimeType.Trespassing => (CrimeSeverity.Minor, 10),
                CrimeType.Assault => (CrimeSeverity.Serious, 100),
                CrimeType.Murder => (CrimeSeverity.Capital, 500),
                CrimeType.PropertyDamage => (CrimeSeverity.Minor, 15),
                CrimeType.IllegalTrading => (CrimeSeverity.Moderate, 50),
                CrimeType.RestrictedAreaEntry => (CrimeSeverity.Serious, 75),
                _ => (CrimeSeverity.Minor, 10)
            };
        }

        // ─────────────────────── Report Crime ───────────────────────

        /// <summary>
        /// Report a crime. Handles witness detection and bounty assignment.
        /// </summary>
        public CrimeRecord ReportCrime(
            CrimeType type,
            string perpetratorId,
            string victimId,
            string locationId,
            string regionId,
            string settlementId,
            double worldTime,
            List<string>? witnessIds = null,
            int severityOverride = -1,
            string modifierTag = "")
        {
            var (severity, baseBounty) = GetCrimeDefaults(type);
            if (severityOverride >= 0)
            {
                severity = severityOverride switch
                {
                    0 => CrimeSeverity.Minor,
                    1 => CrimeSeverity.Moderate,
                    2 => CrimeSeverity.Serious,
                    3 => CrimeSeverity.Severe,
                    _ => CrimeSeverity.Capital
                };
            }

            // Scale bounty by severity
            int bountyMultiplier = severity switch
            {
                CrimeSeverity.Minor => 1,
                CrimeSeverity.Moderate => 2,
                CrimeSeverity.Serious => 5,
                CrimeSeverity.Severe => 10,
                CrimeSeverity.Capital => 20,
                _ => 1
            };

            int bountyValue = baseBounty * bountyMultiplier;

            // Determine expiration (crimes expire after in-game time)
            double expirationHours = severity switch
            {
                CrimeSeverity.Minor => 24,
                CrimeSeverity.Moderate => 72,
                CrimeSeverity.Serious => 168,
                CrimeSeverity.Severe => 720,
                CrimeSeverity.Capital => -1, // never expires
                _ => 24
            };

            var crime = new CrimeRecord
            {
                CrimeId = Guid.NewGuid().ToString("N"),
                Type = type,
                Severity = severity,
                PerpetratorId = perpetratorId,
                VictimId = victimId,
                LocationId = locationId,
                RegionId = regionId,
                SettlementId = settlementId,
                WorldTime = worldTime,
                BountyValue = bountyValue,
                WitnessIds = witnessIds ?? new List<string>(),
                ModifierTag = modifierTag,
                ExpirationTime = expirationHours > 0 ? worldTime + expirationHours : -1
            };

            lock (_lock)
            {
                _crimeHistory.Add(crime);

                if (!_perpetratorCrimes.ContainsKey(perpetratorId))
                    _perpetratorCrimes[perpetratorId] = new List<CrimeRecord>();
                _perpetratorCrimes[perpetratorId].Add(crime);

                // Add bounties if witnesses exist
                if (crime.WitnessIds.Count > 0)
                {
                    AddBounty(perpetratorId, "global", bountyValue);
                }
            }

            OnCrimeCommitted?.Invoke(new CrimeCommittedEvent
            {
                Crime = crime,
                HasWitnesses = crime.WitnessIds.Count > 0,
                IsReported = crime.WitnessIds.Count > 0
            });

            return crime;
        }

        // ─────────────────────── Bounty Management ───────────────────────

        public int GetTotalBounty(string targetId)
        {
            lock (_lock) { return _activeBounties.TryGetValue(targetId, out var b) ? b : 0; }
        }

        public int GetFactionBounty(string factionId, string targetId)
        {
            lock (_lock)
            {
                if (_factionBounties.TryGetValue(factionId, out var targets) &&
                    targets.TryGetValue(targetId, out var bounty))
                    return bounty;
                return 0;
            }
        }

        public Dictionary<string, int> GetAllActiveBounties()
        {
            lock (_lock) { return new Dictionary<string, int>(_activeBounties); }
        }

        public void AddBounty(string targetId, string factionId, int amount)
        {
            lock (_lock)
            {
                int oldTotal = _activeBounties.TryGetValue(targetId, out var existing) ? existing : 0;
                _activeBounties[targetId] = existing + amount;

                if (!_factionBounties.ContainsKey(factionId))
                    _factionBounties[factionId] = new Dictionary<string, int>();
                _factionBounties[factionId][targetId] = 
                    (_factionBounties[factionId].TryGetValue(targetId, out var fBounty) ? fBounty : 0) + amount;

                OnBountyUpdated?.Invoke(new BountyUpdatedEvent
                {
                    TargetId = targetId,
                    FactionId = factionId,
                    OldBounty = oldTotal,
                    NewBounty = _activeBounties[targetId]
                });
            }
        }

        public void ClearBounty(string targetId)
        {
            lock (_lock)
            {
                int old = _activeBounties.TryGetValue(targetId, out var b) ? b : 0;
                _activeBounties.Remove(targetId);
                foreach (var kv in _factionBounties)
                    kv.Value.Remove(targetId);

                if (old > 0)
                {
                    OnBountyUpdated?.Invoke(new BountyUpdatedEvent
                    {
                        TargetId = targetId,
                        FactionId = "global",
                        OldBounty = old,
                        NewBounty = 0
                    });
                }
            }
        }

        // ─────────────────────── Crime Queries ───────────────────────

        public List<CrimeRecord> GetCrimeHistory(string perpetratorId)
        {
            lock (_lock)
            {
                return _perpetratorCrimes.TryGetValue(perpetratorId, out var crimes)
                    ? new List<CrimeRecord>(crimes)
                    : new List<CrimeRecord>();
            }
        }

        public List<CrimeRecord> GetActiveCrimes(string perpetratorId)
        {
            lock (_lock)
            {
                if (!_perpetratorCrimes.TryGetValue(perpetratorId, out var crimes))
                    return new List<CrimeRecord>();
                return crimes.Where(c => !c.Expired && !c.IsResolved).ToList();
            }
        }

        public List<CrimeRecord> GetCrimesInSettlement(string settlementId)
        {
            lock (_lock)
            {
                return _crimeHistory
                    .Where(c => c.SettlementId == settlementId && !c.Expired)
                    .ToList();
            }
        }

        public CrimeRecord? GetCrime(string crimeId)
        {
            lock (_lock) { return _crimeHistory.FirstOrDefault(c => c.CrimeId == crimeId); }
        }

        public int GetTotalCrimeCount() { lock (_lock) { return _crimeHistory.Count; } }
        public int GetActiveCrimeCount() { lock (_lock) { return _crimeHistory.Count(c => !c.Expired && !c.IsResolved); } }

        // ─────────────────────── Witness Detection ───────────────────────

        /// <summary>
        /// Checks if there are witnesses in range of a crime location.
        /// Returns list of NPC IDs that witnessed the crime.
        /// </summary>
        public List<string> DetectWitnesses(
            string locationId,
            float detectionRadius,
            List<string> nearbyNpcIds,
            string perpetratorId,
            CrimeType crimeType,
            bool isHidden)
        {
            if (isHidden)
                return new List<string>();

            var witnesses = new List<string>();
            float detectionChance = crimeType switch
            {
                CrimeType.Murder => 0.9f,
                CrimeType.Assault => 0.7f,
                CrimeType.Theft => 0.3f,
                CrimeType.Trespassing => 0.2f,
                CrimeType.PropertyDamage => 0.4f,
                CrimeType.IllegalTrading => 0.5f,
                CrimeType.RestrictedAreaEntry => 0.6f,
                _ => 0.3f
            };

            // Scale by distance
            float proximityMultiplier = detectionRadius <= 5f ? 1.0f :
                                         detectionRadius <= 10f ? 0.7f :
                                         detectionRadius <= 20f ? 0.4f : 0.1f;

            detectionChance *= proximityMultiplier;

            var rng = new Random();
            foreach (var npcId in nearbyNpcIds)
            {
                if (npcId == perpetratorId) continue;
                if (rng.NextDouble() < detectionChance)
                    witnesses.Add(npcId);
            }

            return witnesses;
        }

        // ─────────────────────── Crime Expiration ───────────────────────

        /// <summary>
        /// Called each game day to expire old crimes.
        /// </summary>
        public void ProcessExpirations(double currentWorldTime)
        {
            lock (_lock)
            {
                foreach (var crime in _crimeHistory)
                {
                    if (!crime.Expired && !crime.IsResolved &&
                        crime.ExpirationTime > 0 && currentWorldTime >= crime.ExpirationTime)
                    {
                        crime.Expired = true;
                        Logger.Info($"CrimeManager: Crime {crime.CrimeId} ({crime.Type}) expired.");
                    }
                }

                // Clean up bounties if all crimes for that target expired
                var toRemove = new List<string>();
                foreach (var kv in _activeBounties)
                {
                    var active = _crimeHistory.Where(c => c.PerpetratorId == kv.Key && !c.Expired && !c.IsResolved).ToList();
                    if (active.Count == 0 && kv.Value > 0)
                    {
                        toRemove.Add(kv.Key);
                    }
                }
                foreach (var id in toRemove)
                {
                    ClearBounty(id);
                }
            }
        }

        // ─────────────────────── Save / Load ───────────────────────

        public CrimeSaveData ExportSaveData()
        {
            lock (_lock)
            {
                return new CrimeSaveData
                {
                    CrimeHistory = _crimeHistory.Select(c => new CrimeRecord
                    {
                        CrimeId = c.CrimeId,
                        Type = c.Type,
                        Severity = c.Severity,
                        PerpetratorId = c.PerpetratorId,
                        VictimId = c.VictimId,
                        LocationId = c.LocationId,
                        RegionId = c.RegionId,
                        SettlementId = c.SettlementId,
                        WorldTime = c.WorldTime,
                        BountyValue = c.BountyValue,
                        IsResolved = c.IsResolved,
                        WitnessIds = new List<string>(c.WitnessIds),
                        ModifierTag = c.ModifierTag,
                        Expired = c.Expired,
                        ExpirationTime = c.ExpirationTime
                    }).ToList(),
                    ActiveBounties = new Dictionary<string, int>(_activeBounties),
                    FactionBounties = _factionBounties.ToDictionary(
                        f => f.Key, f => new Dictionary<string, int>(f.Value))
                };
            }
        }

        public void RestoreSaveData(CrimeSaveData? data)
        {
            if (data == null) return;
            lock (_lock)
            {
                _crimeHistory.Clear();
                _activeBounties.Clear();
                _factionBounties.Clear();
                _perpetratorCrimes.Clear();

                foreach (var c in data.CrimeHistory)
                {
                    _crimeHistory.Add(c);
                    if (!_perpetratorCrimes.ContainsKey(c.PerpetratorId))
                        _perpetratorCrimes[c.PerpetratorId] = new List<CrimeRecord>();
                    _perpetratorCrimes[c.PerpetratorId].Add(c);
                }

                foreach (var kv in data.ActiveBounties)
                    _activeBounties[kv.Key] = kv.Value;

                foreach (var fkv in data.FactionBounties)
                {
                    if (!_factionBounties.ContainsKey(fkv.Key))
                        _factionBounties[fkv.Key] = new Dictionary<string, int>();
                    foreach (var tkv in fkv.Value)
                        _factionBounties[fkv.Key][tkv.Key] = tkv.Value;
                }
            }
        }
    }

    /// <summary>
    /// Save data container for the crime system.
    /// </summary>
    public class CrimeSaveData
    {
        public List<CrimeRecord> CrimeHistory { get; set; } = new();
        public Dictionary<string, int> ActiveBounties { get; set; } = new();
        public Dictionary<string, Dictionary<string, int>> FactionBounties { get; set; } = new();
    }
}