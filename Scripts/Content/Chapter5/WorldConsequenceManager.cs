using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter5
{
    public class ConsequenceStateRecord
    {
        public string ConsequenceId { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = false;
        public string AssociatedFactionId { get; set; } = "";
        public string SetWorldFlag { get; set; } = "";
    }

    /// <summary>
    /// World Consequences Manager for Chapter 5 and Act II.
    /// Manages persistent world reactivity resulting from player choices, including patrol shifts, shop inventories, and road safety.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class WorldConsequenceManager : IInitializable
    {
        private readonly Dictionary<string, ConsequenceStateRecord> _consequences = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<ConsequenceStateRecord>? OnConsequenceTriggered;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultConsequences();

            // Register with ServiceLocator
            ServiceLocator.Register<WorldConsequenceManager>(this);

            IsInitialized = true;
            Logger.Info("WorldConsequenceManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _consequences.Clear();

            ServiceLocator.Unregister<WorldConsequenceManager>();
            IsInitialized = false;
            Logger.Info("WorldConsequenceManager: Shutdown completed.");
        }

        private void RegisterDefaultConsequences()
        {
            // 1. Vanguard Patrol Dominance
            RegisterConsequence(new ConsequenceStateRecord
            {
                ConsequenceId = "consequence_vanguard_patrols",
                Description = "Iron Vanguard heavily patrols Eastern Ridgeline roads, reducing bandit attacks.",
                AssociatedFactionId = "faction_iron_vanguard",
                SetWorldFlag = "flag_vanguard_patrol_dominance"
            });

            // 2. Syndicate Trade Surge
            RegisterConsequence(new ConsequenceStateRecord
            {
                ConsequenceId = "consequence_syndicate_trade",
                Description = "Silver Syndicate expands Valenhold market stock with discounted rare contraband.",
                AssociatedFactionId = "faction_silver_syndicate",
                SetWorldFlag = "flag_syndicate_trade_surge"
            });

            // 3. Sylvan Nature Sanctuary
            RegisterConsequence(new ConsequenceStateRecord
            {
                ConsequenceId = "consequence_sylvan_sanctuary",
                Description = "Sylvan Circle purifies Mirkwood Swamps, unlocking rare herbal gathering nodes.",
                AssociatedFactionId = "faction_sylvan_circle",
                SetWorldFlag = "flag_sylvan_swamp_purified"
            });
        }

        public void RegisterConsequence(ConsequenceStateRecord record)
        {
            if (record != null && !string.IsNullOrEmpty(record.ConsequenceId))
            {
                _consequences[record.ConsequenceId] = record;
            }
        }

        public bool TriggerConsequence(string consequenceId)
        {
            if (!_consequences.TryGetValue(consequenceId, out var record))
            {
                Logger.Warning($"WorldConsequenceManager: Consequence '{consequenceId}' not found.");
                return false;
            }

            if (record.IsActive) return true;

            record.IsActive = true;

            // Set world state flag if configured
            if (!string.IsNullOrEmpty(record.SetWorldFlag))
            {
                try
                {
                    var worldState = ServiceLocator.Get<Story.WorldStateManager>();
                    worldState?.SetFlag(record.SetWorldFlag, "true");
                }
                catch
                {
                    // WorldStateManager not registered in lightweight unit tests
                }
            }

            OnConsequenceTriggered?.Invoke(record);
            Logger.Info($"WorldConsequenceManager: Triggered consequence '{record.Description}' ({consequenceId}).");
            return true;
        }

        public ConsequenceStateRecord? GetConsequence(string consequenceId)
        {
            return _consequences.TryGetValue(consequenceId, out var record) ? record : null;
        }

        public List<ConsequenceStateRecord> GetAllConsequences()
        {
            return new List<ConsequenceStateRecord>(_consequences.Values);
        }
    }
}
