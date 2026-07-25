using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.NPC
{
    public enum ReputationScope
    {
        Global,
        Regional,
        Faction,
        Individual
    }

    /// <summary>
    /// Describes an event that triggered a reputation change.
    /// Fired through the EventBus so any system can react.
    /// </summary>
    public class ReputationChangedEvent
    {
        public ReputationScope Scope { get; set; }
        public string ScopeKey { get; set; } = "";   // region id / faction id / npc id
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public string EventTag { get; set; } = "";   // e.g. "saved_villager", "stolen_item"
    }

    /// <summary>
    /// Tracks player reputation across all scopes.
    /// All values clamped to –1000..+1000.
    /// Changes are event-driven for decoupled system consumption.
    /// </summary>
    public class ReputationSystem
    {
        private int _globalReputation = 0;

        // keyed by regionId, factionId, or npcId
        private readonly Dictionary<string, int> _regional  = new();
        private readonly Dictionary<string, int> _faction   = new();
        private readonly Dictionary<string, int> _individual = new();

        /// <summary>Event fired after any reputation change.</summary>
        public event Action<ReputationChangedEvent>? OnReputationChanged;

        // ─────────────────────── Public API ───────────────────────

        public void AdjustGlobal(int delta, string eventTag = "")
        {
            int old = _globalReputation;
            _globalReputation = Math.Clamp(_globalReputation + delta, -1000, 1000);
            Fire(ReputationScope.Global, "global", old, _globalReputation, eventTag);
        }

        public void AdjustRegional(string regionId, int delta, string eventTag = "")
        {
            _regional.TryGetValue(regionId, out int old);
            int next = Math.Clamp(old + delta, -1000, 1000);
            _regional[regionId] = next;
            Fire(ReputationScope.Regional, regionId, old, next, eventTag);
        }

        public void AdjustFaction(string factionId, int delta, string eventTag = "")
        {
            _faction.TryGetValue(factionId, out int old);
            int next = Math.Clamp(old + delta, -1000, 1000);
            _faction[factionId] = next;
            Fire(ReputationScope.Faction, factionId, old, next, eventTag);
        }

        public void AdjustIndividual(string npcId, int delta, string eventTag = "")
        {
            _individual.TryGetValue(npcId, out int old);
            int next = Math.Clamp(old + delta, -1000, 1000);
            _individual[npcId] = next;
            Fire(ReputationScope.Individual, npcId, old, next, eventTag);
        }

        // ─────────────────────── Getters ───────────────────────

        public int GetGlobal() => _globalReputation;
        public int GetRegional(string regionId) => _regional.TryGetValue(regionId, out var v) ? v : 0;
        public int GetFaction(string factionId) => _faction.TryGetValue(factionId, out var v) ? v : 0;
        public int GetIndividual(string npcId) => _individual.TryGetValue(npcId, out var v) ? v : 0;

        // ─────────────────────── Snapshot for Save V6 ───────────────────────

        /// <summary>Exports all scoped scores to a flat dictionary for serialization.</summary>
        public Dictionary<string, int> ExportSnapshot()
        {
            var snap = new Dictionary<string, int>();
            snap["global"] = _globalReputation;
            foreach (var kv in _regional)  snap[$"reg:{kv.Key}"]  = kv.Value;
            foreach (var kv in _faction)   snap[$"fac:{kv.Key}"]  = kv.Value;
            foreach (var kv in _individual) snap[$"ind:{kv.Key}"] = kv.Value;
            return snap;
        }

        /// <summary>Restores from a saved snapshot dictionary.</summary>
        public void RestoreSnapshot(Dictionary<string, int> snapshot)
        {
            if (snapshot == null) return;
            foreach (var kv in snapshot)
            {
                if (kv.Key == "global") { _globalReputation = kv.Value; }
                else if (kv.Key.StartsWith("reg:")) _regional[kv.Key[4..]] = kv.Value;
                else if (kv.Key.StartsWith("fac:")) _faction[kv.Key[4..]] = kv.Value;
                else if (kv.Key.StartsWith("ind:")) _individual[kv.Key[4..]] = kv.Value;
            }
        }

        // ─────────────────────── Private ───────────────────────

        private void Fire(ReputationScope scope, string key, int old, int next, string tag)
        {
            if (old == next) return;
            Logger.Info($"Reputation[{scope}/{key}] {old:+#;-#;0} → {next:+#;-#;0} ({tag})");
            OnReputationChanged?.Invoke(new ReputationChangedEvent
            {
                Scope = scope, ScopeKey = key,
                OldValue = old, NewValue = next, EventTag = tag
            });
        }
    }
}
