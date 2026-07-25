using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public class CompanionDefinition
    {
        public string CompanionId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Backstory { get; set; } = string.Empty;
        public int StartLevel { get; set; } = 18;
        public string JoinConditionQuestId { get; set; } = string.Empty;
        public List<string> UniqueAbilities { get; set; } = new();
        public string VoiceStyle { get; set; } = string.Empty;
    }

    /// <summary>
    /// First major companion registry for Act II. Registers Seraphine the Arcane Scout,
    /// the player's first full-featured party companion who joins after liberating the
    /// Eastern Ridgeline watchtower.
    /// </summary>
    public class CompanionRegistry
    {
        private readonly Dictionary<string, CompanionDefinition> _companions = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterCompanions()
        {
            var seraphine = new CompanionDefinition
            {
                CompanionId = "companion_seraphine",
                DisplayName = "Seraphine Vael",
                Class = "Arcane Scout",
                Backstory = "A former Valen Crown intelligence operative who survived the Citadel siege. She carries knowledge of Malakor's network and joins the hero after being rescued from a Shadow Cult encampment on the Eastern Ridgeline.",
                StartLevel = 18,
                JoinConditionQuestId = "q_act2_ridgeline_rescue",
                VoiceStyle = "Sharp, observant, dry humor — trusts actions over words."
            };
            seraphine.UniqueAbilities.Add("ability_arcane_pulse");
            seraphine.UniqueAbilities.Add("ability_shadow_sight");
            seraphine.UniqueAbilities.Add("ability_barrier_weave");
            RegisterCompanion(seraphine);

            Logger.Info("CompanionRegistry: 1 companion registered — Seraphine Vael.");
        }

        public void RegisterCompanion(CompanionDefinition companion)
        {
            if (companion != null && !string.IsNullOrEmpty(companion.CompanionId))
                _companions[companion.CompanionId] = companion;
        }

        public CompanionDefinition? GetCompanion(string companionId)
            => _companions.TryGetValue(companionId, out var c) ? c : null;

        public IReadOnlyCollection<CompanionDefinition> AllCompanions => _companions.Values;
    }
}
