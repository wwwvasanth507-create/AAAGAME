using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Social.Diplomacy
{
    /// <summary>
    /// Diplomatic relationship states between factions.
    /// </summary>
    public enum DiplomaticRelation
    {
        Alliance,
        Neutral,
        TradeAgreement,
        Conflict,
        War,
        Peace,
        Ceasefire
    }

    /// <summary>
    /// Event fired when diplomatic relations change.
    /// </summary>
    public class DiplomaticEvent
    {
        public string FactionIdA { get; set; } = "";
        public string FactionIdB { get; set; } = "";
        public DiplomaticRelation OldRelation { get; set; }
        public DiplomaticRelation NewRelation { get; set; }
        public string EventType { get; set; } = ""; // "alliance", "war", "peace", "trade", "ceasefire"
    }

    /// <summary>
    /// Manages diplomatic relationships between factions.
    /// Supports alliances, trade agreements, war, peace, and ceasefire.
    /// Includes reputation modifiers based on diplomatic state.
    /// </summary>
    public class DiplomacyManager
    {
        public const string ServiceKey = "DiplomacyManager";

        // Key: "factionA_factionB" (alphabetically sorted)
        private readonly Dictionary<string, DiplomaticRelation> _relations = new();
        private readonly Dictionary<string, double> _relationTimestamps = new();
        private readonly object _lock = new();

        /// <summary>Fired when diplomatic relations change.</summary>
        public event Action<DiplomaticEvent>? OnDiplomaticChange;

        // ─────────────────────── Default Relations ───────────────────────

        /// <summary>
        /// Initialize default diplomatic relationships based on faction database.
        /// </summary>
        public void InitializeDefaults(Factions.FactionDatabase factionDb)
        {
            if (factionDb == null) return;

            var factions = factionDb.GetAllFactions();
            foreach (var faction in factions)
            {
                foreach (var friendlyId in faction.FriendlyFactions)
                {
                    SetRelation(faction.FactionId, friendlyId, DiplomaticRelation.Alliance);
                }

                foreach (var hostileId in faction.HostileFactions)
                {
                    SetRelation(faction.FactionId, hostileId, DiplomaticRelation.War);
                }

                foreach (var neutralId in faction.NeutralFactions)
                {
                    SetRelation(faction.FactionId, neutralId, DiplomaticRelation.Neutral);
                }
            }

            // Set trade agreements based on merchant-friendly relations
            foreach (var faction in factions)
            {
                if (faction.Type == Factions.FactionType.MerchantGuild || 
                    faction.Type == Factions.FactionType.Kingdom)
                {
                    foreach (var friendlyId in faction.FriendlyFactions)
                    {
                        var current = GetRelation(faction.FactionId, friendlyId);
                        if (current == DiplomaticRelation.Alliance)
                        {
                            // Already alliance, keep it
                        }
                        else if (current == DiplomaticRelation.Neutral)
                        {
                            SetRelation(faction.FactionId, friendlyId, DiplomaticRelation.TradeAgreement);
                        }
                    }
                }
            }
        }

        // ─────────────────────── Relation Management ───────────────────────

        public DiplomaticRelation GetRelation(string factionIdA, string factionIdB)
        {
            string key = GetRelationKey(factionIdA, factionIdB);
            lock (_lock)
            {
                return _relations.TryGetValue(key, out var rel) ? rel : DiplomaticRelation.Neutral;
            }
        }

        public void SetRelation(string factionIdA, string factionIdB, DiplomaticRelation newRelation, double worldTime = 0)
        {
            string key = GetRelationKey(factionIdA, factionIdB);
            lock (_lock)
            {
                var old = _relations.TryGetValue(key, out var existing) ? existing : DiplomaticRelation.Neutral;
                if (old == newRelation) return;

                _relations[key] = newRelation;
                _relationTimestamps[key] = worldTime;

                OnDiplomaticChange?.Invoke(new DiplomaticEvent
                {
                    FactionIdA = factionIdA,
                    FactionIdB = factionIdB,
                    OldRelation = old,
                    NewRelation = newRelation,
                    EventType = GetEventType(newRelation)
                });

                Logger.Info($"Diplomacy: {factionIdA} ↔ {factionIdB}: {old} → {newRelation}");
            }
        }

        // ─────────────────────── Diplomatic Actions ───────────────────────

        public void DeclareAlliance(string factionIdA, string factionIdB, double worldTime = 0)
        {
            SetRelation(factionIdA, factionIdB, DiplomaticRelation.Alliance, worldTime);
        }

        public void DeclareWar(string factionIdA, string factionIdB, double worldTime = 0)
        {
            SetRelation(factionIdA, factionIdB, DiplomaticRelation.War, worldTime);
        }

        public void DeclarePeace(string factionIdA, string factionIdB, double worldTime = 0)
        {
            SetRelation(factionIdA, factionIdB, DiplomaticRelation.Peace, worldTime);
        }

        public void EstablishTradeAgreement(string factionIdA, string factionIdB, double worldTime = 0)
        {
            SetRelation(factionIdA, factionIdB, DiplomaticRelation.TradeAgreement, worldTime);
        }

        public void DeclareCeasefire(string factionIdA, string factionIdB, double worldTime = 0)
        {
            SetRelation(factionIdA, factionIdB, DiplomaticRelation.Ceasefire, worldTime);
        }

        // ─────────────────────── Queries ───────────────────────

        public bool AreAllied(string factionIdA, string factionIdB)
        {
            return GetRelation(factionIdA, factionIdB) == DiplomaticRelation.Alliance;
        }

        public bool AreAtWar(string factionIdA, string factionIdB)
        {
            return GetRelation(factionIdA, factionIdB) == DiplomaticRelation.War;
        }

        public bool HaveTradeAgreement(string factionIdA, string factionIdB)
        {
            return GetRelation(factionIdA, factionIdB) == DiplomaticRelation.TradeAgreement;
        }

        /// <summary>
        /// Returns all factions that are allied with the given faction.
        /// </summary>
        public List<string> GetAllies(string factionId)
        {
            lock (_lock)
            {
                return _relations
                    .Where(kv => kv.Value == DiplomaticRelation.Alliance &&
                                 (kv.Key.StartsWith(factionId + "_") || kv.Key.EndsWith("_" + factionId)))
                    .Select(kv => ExtractOther(kv.Key, factionId))
                    .ToList();
            }
        }

        /// <summary>
        /// Returns all factions at war with the given faction.
        /// </summary>
        public List<string> GetEnemies(string factionId)
        {
            lock (_lock)
            {
                return _relations
                    .Where(kv => kv.Value == DiplomaticRelation.War &&
                                 (kv.Key.StartsWith(factionId + "_") || kv.Key.EndsWith("_" + factionId)))
                    .Select(kv => ExtractOther(kv.Key, factionId))
                    .ToList();
            }
        }

        /// <summary>
        /// Gets reputation modifier based on a faction's diplomatic relations with another.
        /// Positive for allies, negative for enemies.
        /// </summary>
        public int GetDiplomaticReputationModifier(string observerFactionId, string targetFactionId)
        {
            var relation = GetRelation(observerFactionId, targetFactionId);
            return relation switch
            {
                DiplomaticRelation.Alliance => 30,
                DiplomaticRelation.TradeAgreement => 15,
                DiplomaticRelation.Neutral => 0,
                DiplomaticRelation.Peace => 5,
                DiplomaticRelation.Ceasefire => 0,
                DiplomaticRelation.Conflict => -20,
                DiplomaticRelation.War => -50,
                _ => 0
            };
        }

        // ─────────────────────── Helpers ───────────────────────

        private static string GetRelationKey(string a, string b)
        {
            return string.Compare(a, b, StringComparison.Ordinal) <= 0
                ? $"{a}_{b}"
                : $"{b}_{a}";
        }

        private static string ExtractOther(string key, string factionId)
        {
            var parts = key.Split('_');
            return parts[0] == factionId ? parts[1] : parts[0];
        }

        private static string GetEventType(DiplomaticRelation relation)
        {
            return relation switch
            {
                DiplomaticRelation.Alliance => "alliance",
                DiplomaticRelation.War => "war",
                DiplomaticRelation.Peace => "peace",
                DiplomaticRelation.TradeAgreement => "trade",
                DiplomaticRelation.Ceasefire => "ceasefire",
                _ => "neutral"
            };
        }

        // ─────────────────────── Save / Load ───────────────────────

        public DiplomaticSaveData ExportSaveData()
        {
            lock (_lock)
            {
                return new DiplomaticSaveData
                {
                    Relations = new Dictionary<string, DiplomaticRelation>(_relations),
                    Timestamps = new Dictionary<string, double>(_relationTimestamps)
                };
            }
        }

        public void RestoreSaveData(DiplomaticSaveData? data)
        {
            if (data == null) return;
            lock (_lock)
            {
                _relations.Clear();
                _relationTimestamps.Clear();
                foreach (var kv in data.Relations)
                    _relations[kv.Key] = kv.Value;
                foreach (var kv in data.Timestamps)
                    _relationTimestamps[kv.Key] = kv.Value;
            }
        }
    }

    /// <summary>
    /// Save data container for the diplomacy system.
    /// </summary>
    public class DiplomaticSaveData
    {
        public Dictionary<string, DiplomaticRelation> Relations { get; set; } = new();
        public Dictionary<string, double> Timestamps { get; set; } = new();
    }
}