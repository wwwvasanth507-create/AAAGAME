using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeroOfEternia.Social.Reputation
{
    /// <summary>
    /// Data-driven reputation modifier definition.
    /// Each modifier describes how an action affects reputation across all scopes.
    /// </summary>
    public class ReputationModifier
    {
        public string ModifierId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Category { get; set; } = ""; // "help", "attack", "trade", "crime", "donation", etc.
        
        // Reputation changes per scope (keyed by scope: "global", "faction", etc.)
        // For faction scope, value is the delta; for per-faction, use perFactionOverrides
        public int GlobalDelta { get; set; } = 0;
        public int RegionDelta { get; set; } = 0;
        public int SettlementDelta { get; set; } = 0;
        public int FactionDelta { get; set; } = 0;
        public int IndividualDelta { get; set; } = 0;
        
        // Per-faction overrides: factionId -> delta
        public Dictionary<string, int> PerFactionDeltas { get; set; } = new();
        
        // Per-settlement overrides
        public Dictionary<string, int> PerSettlementDeltas { get; set; } = new();
        
        // Conditional requirements
        public bool RequiresWitness { get; set; } = false;
        public int MinPlayerLevel { get; set; } = 0;
        public float CooldownSeconds { get; set; } = 0f;
        public bool Stackable { get; set; } = true;
        
        // Future quest/story event integration
        public string QuestHook { get; set; } = "";
        public string StoryEventHook { get; set; } = "";
        
        // Localization
        public string LocalizationKey { get; set; } = "";
    }

    /// <summary>
    /// Registry of all available reputation modifiers. Loaded from JSON.
    /// Supports runtime registration for mods and DLC.
    /// </summary>
    public class ReputationModifierRegistry
    {
        private readonly Dictionary<string, ReputationModifier> _modifiers = new();
        private readonly Dictionary<string, List<string>> _modifiersByCategory = new();
        private readonly object _lock = new();

        /// <summary>
        /// Load modifiers from a JSON configuration file.
        /// </summary>
        public void Initialize(string jsonPath = "res://Settings/reputation_modifiers.json")
        {
            try
            {
                var file = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read);
                if (file == null)
                {
                    LoadDefaults();
                    return;
                }

                string json = file.GetAsText();
                file.Close();

                var loaded = JsonConvert.DeserializeObject<List<ReputationModifier>>(json);
                if (loaded == null || loaded.Count == 0)
                {
                    LoadDefaults();
                    return;
                }

                lock (_lock)
                {
                    foreach (var mod in loaded)
                    {
                        RegisterInternal(mod);
                    }
                }
            }
            catch
            {
                LoadDefaults();
            }
        }

        /// <summary>
        /// Register a modifier at runtime. No code changes needed.
        /// </summary>
        public void RegisterModifier(ReputationModifier modifier)
        {
            if (modifier == null || string.IsNullOrEmpty(modifier.ModifierId)) return;
            lock (_lock)
            {
                RegisterInternal(modifier);
            }
        }

        public ReputationModifier? GetModifier(string modifierId)
        {
            lock (_lock) { return _modifiers.TryGetValue(modifierId, out var m) ? m : null; }
        }

        public List<ReputationModifier> GetModifiersByCategory(string category)
        {
            lock (_lock)
            {
                if (!_modifiersByCategory.TryGetValue(category.ToLowerInvariant(), out var ids))
                    return new List<ReputationModifier>();
                
                var result = new List<ReputationModifier>();
                foreach (var id in ids)
                {
                    if (_modifiers.TryGetValue(id, out var m))
                        result.Add(m);
                }
                return result;
            }
        }

        public List<ReputationModifier> GetAllModifiers()
        {
            lock (_lock) { return new List<ReputationModifier>(_modifiers.Values); }
        }

        private void RegisterInternal(ReputationModifier modifier)
        {
            _modifiers[modifier.ModifierId] = modifier;
            
            string cat = modifier.Category.ToLowerInvariant();
            if (!_modifiersByCategory.ContainsKey(cat))
                _modifiersByCategory[cat] = new List<string>();
            if (!_modifiersByCategory[cat].Contains(modifier.ModifierId))
                _modifiersByCategory[cat].Add(modifier.ModifierId);
        }

        private void LoadDefaults()
        {
            var defaults = new List<ReputationModifier>
            {
                new() { ModifierId = "help_npc_quest", DisplayName = "Helped NPC with task", Category = "help", GlobalDelta = 1, FactionDelta = 3, IndividualDelta = 10 },
                new() { ModifierId = "help_npc_combat", DisplayName = "Saved NPC from enemy", Category = "help", GlobalDelta = 2, FactionDelta = 5, IndividualDelta = 15 },
                new() { ModifierId = "help_npc_gift", DisplayName = "Gave gift to NPC", Category = "help", IndividualDelta = 5 },
                new() { ModifierId = "attack_npc", DisplayName = "Attacked NPC", Category = "attack", GlobalDelta = -5, FactionDelta = -10, IndividualDelta = -20 },
                new() { ModifierId = "attack_npc_lethal", DisplayName = "Killed NPC", Category = "attack", GlobalDelta = -20, FactionDelta = -30, IndividualDelta = -50 },
                new() { ModifierId = "attack_npc_guard", DisplayName = "Attacked guard", Category = "attack", GlobalDelta = -30, FactionDelta = -50, IndividualDelta = -40 },
                new() { ModifierId = "trade_profitable", DisplayName = "Profitable trade", Category = "trade", GlobalDelta = 0, FactionDelta = 2, SettlementDelta = 3 },
                new() { ModifierId = "trade_scam", DisplayName = "Cheated in trade", Category = "trade", GlobalDelta = -2, FactionDelta = -5, SettlementDelta = -8 },
                new() { ModifierId = "defeat_monster", DisplayName = "Defeated monster", Category = "combat", GlobalDelta = 1, FactionDelta = 2, SettlementDelta = 3 },
                new() { ModifierId = "defeat_bandit", DisplayName = "Defeated bandit", Category = "combat", GlobalDelta = 2, FactionDelta = 5, SettlementDelta = 5 },
                new() { ModifierId = "donation_small", DisplayName = "Small donation", Category = "donation", GlobalDelta = 1, FactionDelta = 2, SettlementDelta = 3 },
                new() { ModifierId = "donation_large", DisplayName = "Large donation", Category = "donation", GlobalDelta = 5, FactionDelta = 10, SettlementDelta = 15 },
                new() { ModifierId = "crime_theft", DisplayName = "Committed theft", Category = "crime", GlobalDelta = -5, FactionDelta = -8, SettlementDelta = -10 },
                new() { ModifierId = "crime_trespass", DisplayName = "Trespassed", Category = "crime", GlobalDelta = -1, FactionDelta = -2, SettlementDelta = -3 },
                new() { ModifierId = "crime_assault", DisplayName = "Assaulted someone", Category = "crime", GlobalDelta = -10, FactionDelta = -15, SettlementDelta = -20 },
                new() { ModifierId = "crime_murder", DisplayName = "Committed murder", Category = "crime", GlobalDelta = -50, FactionDelta = -80, SettlementDelta = -100 },
                new() { ModifierId = "crime_property_damage", DisplayName = "Damaged property", Category = "crime", GlobalDelta = -2, FactionDelta = -3, SettlementDelta = -5 },
                new() { ModifierId = "crime_illegal_trade", DisplayName = "Illegal trading", Category = "crime", GlobalDelta = -3, FactionDelta = -5, SettlementDelta = -5 },
                new() { ModifierId = "dialogue_respectful", DisplayName = "Respectful dialogue", Category = "dialogue", IndividualDelta = 3 },
                new() { ModifierId = "dialogue_rude", DisplayName = "Rude dialogue", Category = "dialogue", IndividualDelta = -5 },
                new() { ModifierId = "faction_event_supported", DisplayName = "Supported faction event", Category = "faction_event", FactionDelta = 15 },
                new() { ModifierId = "faction_event_opposed", DisplayName = "Opposed faction event", Category = "faction_event", FactionDelta = -15 },
                new() { ModifierId = "quest_completed", DisplayName = "Completed quest", Category = "quest", GlobalDelta = 5, FactionDelta = 10, SettlementDelta = 10, IndividualDelta = 20 },
                new() { ModifierId = "story_event_good", DisplayName = "Positive story outcome", Category = "story", GlobalDelta = 10, FactionDelta = 20 },
                new() { ModifierId = "story_event_bad", DisplayName = "Negative story outcome", Category = "story", GlobalDelta = -10, FactionDelta = -20 }
            };

            lock (_lock)
            {
                foreach (var mod in defaults)
                {
                    RegisterInternal(mod);
                }
            }
        }
    }
}