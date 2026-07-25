using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter3
{
    public class ActIWorldEvolutionNode
    {
        public string EventId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsUnlocked { get; set; } = false;
    }

    /// <summary>
    /// Act I world evolution tracker — opens travel routes, unlocks regions,
    /// updates NPC schedules, and reflects boss defeat consequences across Eternia.
    /// </summary>
    public class ActIWorldEvolution
    {
        private readonly Dictionary<string, ActIWorldEvolutionNode> _events = new(StringComparer.OrdinalIgnoreCase);

        public void InitializeActIEvolution()
        {
            RegisterEvent(new ActIWorldEvolutionNode
            {
                EventId = "evt_citadel_sealed",
                Description = "Citadel of Void Shadows sealed after Vareth's defeat. New travel route opened from Elderwood Grove to Eastern Ridgeline."
            });

            RegisterEvent(new ActIWorldEvolutionNode
            {
                EventId = "evt_oakvale_celebration",
                Description = "Oakvale holds a victory celebration. Elder Alden offers advanced skill tutelage. Merchant stock upgrades to Tier 2."
            });

            RegisterEvent(new ActIWorldEvolutionNode
            {
                EventId = "evt_sylvan_alliance_formal",
                Description = "Sylvan Guardians formally ally with Valen Crown. Captain Valerius stationed in Elderwood Grove."
            });

            RegisterEvent(new ActIWorldEvolutionNode
            {
                EventId = "evt_shadow_cult_retreat",
                Description = "Shadow Cult forces retreat deeper into Mirkwood Swamps. Enemy population in Sylvanwood Wilds reduced."
            });

            RegisterEvent(new ActIWorldEvolutionNode
            {
                EventId = "evt_tier2_merchants",
                Description = "Tier 2 merchant inventories unlocked in Oakvale and Elderwood Grove."
            });
        }

        public void RegisterEvent(ActIWorldEvolutionNode evt)
        {
            if (evt != null && !string.IsNullOrEmpty(evt.EventId))
                _events[evt.EventId] = evt;
        }

        public void UnlockEvent(string eventId)
        {
            if (_events.TryGetValue(eventId, out var evt) && !evt.IsUnlocked)
            {
                evt.IsUnlocked = true;
                Logger.Info($"ActIWorldEvolution: Event '{eventId}' unlocked — {evt.Description}");
            }
        }

        public ActIWorldEvolutionNode? GetEvent(string eventId)
            => _events.TryGetValue(eventId, out var e) ? e : null;

        public IReadOnlyCollection<ActIWorldEvolutionNode> AllEvents => _events.Values;
    }
}
