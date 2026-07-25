using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter3
{
    public enum FactionRelation
    {
        Allied,
        Neutral,
        Hostile,
        AtWar
    }

    public class FactionState
    {
        public string FactionId { get; set; } = string.Empty;
        public FactionRelation RelationToPlayer { get; set; } = FactionRelation.Neutral;
        public int TerritoryControl { get; set; } = 0; // 0-100 territory dominance
        public bool NewAllianceUnlocked { get; set; } = false;
    }

    /// <summary>
    /// Faction conflict escalation manager reacting to Act I boss defeat and Titan Seal
    /// reinforcement. Tracks territory changes, alliance unlocks, and hostility spikes.
    /// </summary>
    public class FactionEscalationManager
    {
        private readonly Dictionary<string, FactionState> _factions = new(StringComparer.OrdinalIgnoreCase);

        public event Action<string, FactionRelation>? OnFactionRelationChanged;

        public void InitializeFactions()
        {
            RegisterFaction(new FactionState
            {
                FactionId = "faction_valen_crown",
                RelationToPlayer = FactionRelation.Allied,
                TerritoryControl = 70
            });

            RegisterFaction(new FactionState
            {
                FactionId = "faction_sylvan_guardians",
                RelationToPlayer = FactionRelation.Allied,
                TerritoryControl = 60,
                NewAllianceUnlocked = true
            });

            RegisterFaction(new FactionState
            {
                FactionId = "faction_shadow_cult",
                RelationToPlayer = FactionRelation.AtWar,
                TerritoryControl = 25
            });

            RegisterFaction(new FactionState
            {
                FactionId = "faction_merchants_guild",
                RelationToPlayer = FactionRelation.Neutral,
                TerritoryControl = 40
            });
        }

        public void RegisterFaction(FactionState fs)
        {
            if (fs != null && !string.IsNullOrEmpty(fs.FactionId))
                _factions[fs.FactionId] = fs;
        }

        public void EscalateFactionRelation(string factionId, FactionRelation newRelation)
        {
            if (_factions.TryGetValue(factionId, out var fs))
            {
                fs.RelationToPlayer = newRelation;
                Logger.Info($"FactionEscalationManager: '{factionId}' relation changed to '{newRelation}'");
                OnFactionRelationChanged?.Invoke(factionId, newRelation);
            }
        }

        public FactionState? GetFaction(string factionId)
            => _factions.TryGetValue(factionId, out var f) ? f : null;

        public IReadOnlyCollection<FactionState> AllFactions => _factions.Values;
    }
}
