using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Equipment.Sets
{
    /// <summary>
    /// Defines an equipment set with piece requirements and bonus hooks.
    /// </summary>
    public class GearSetDefinition
    {
        public string SetId { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public List<string> PieceIds { get; }
        public List<SetBonusTier> BonusTiers { get; }
        public string VisualOverridePath { get; set; }

        public GearSetDefinition(
            string setId,
            string displayName,
            string description,
            List<string> pieceIds,
            List<SetBonusTier> bonusTiers,
            string visualOverridePath = "")
        {
            SetId = setId;
            DisplayName = displayName;
            Description = description;
            PieceIds = pieceIds;
            BonusTiers = bonusTiers.OrderBy(t => t.PiecesRequired).ToList();
            VisualOverridePath = visualOverridePath;
        }
    }

    /// <summary>
    /// A tier of bonuses unlocked when a certain number of set pieces are equipped.
    /// </summary>
    public class SetBonusTier
    {
        public int PiecesRequired { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public List<SetBonusEffect> Effects { get; }

        public SetBonusTier(int piecesRequired, string displayName, string description, List<SetBonusEffect> effects)
        {
            PiecesRequired = piecesRequired;
            DisplayName = displayName;
            Description = description;
            Effects = effects;
        }
    }

    /// <summary>
    /// A single bonus effect granted by a set bonus tier.
    /// </summary>
    public class SetBonusEffect
    {
        public string EffectId { get; }
        public string StatAffected { get; }
        public float Value { get; }
        public bool IsPercent { get; }

        public SetBonusEffect(string effectId, string statAffected, float value, bool isPercent = false)
        {
            EffectId = effectId;
            StatAffected = statAffected;
            Value = value;
            IsPercent = isPercent;
        }
    }

    /// <summary>
    /// Runtime state tracking which set bonuses are active for a player.
    /// </summary>
    public class ActiveSetBonus
    {
        public GearSetDefinition SetDefinition { get; }
        public int EquippedPieces { get; private set; }
        public List<SetBonusTier> ActiveTiers { get; private set; }
        public bool HasFullSet { get; private set; }

        public ActiveSetBonus(GearSetDefinition definition)
        {
            SetDefinition = definition;
            EquippedPieces = 0;
            ActiveTiers = new List<SetBonusTier>();
            HasFullSet = false;
        }

        /// <summary>
        /// Updates the equipped piece count and recalculates active tiers.
        /// </summary>
        public void UpdateEquippedPieces(int count)
        {
            EquippedPieces = count;
            HasFullSet = EquippedPieces >= SetDefinition.PieceIds.Count;
            
            ActiveTiers = SetDefinition.BonusTiers
                .Where(t => EquippedPieces >= t.PiecesRequired)
                .ToList();
        }
    }

    /// <summary>
    /// Central system for managing gear sets and their bonuses.
    /// </summary>
    public class GearSetManager
    {
        private readonly Dictionary<string, GearSetDefinition> _setRegistry = new();
        private readonly Dictionary<string, ActiveSetBonus> _activeSets = new();

        /// <summary>Fired when a set bonus tier is activated.</summary>
        public event Action<string, int> OnSetBonusActivated;
        /// <summary>Fired when a set bonus tier is deactivated.</summary>
        public event Action<string, int> OnSetBonusDeactivated;

        // ---------------------------------------------------------------
        // REGISTRATION
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers a gear set definition.
        /// </summary>
        public void RegisterSet(GearSetDefinition set)
        {
            _setRegistry[set.SetId] = set;
        }

        /// <summary>
        /// Gets a gear set definition by ID.
        /// </summary>
        public GearSetDefinition GetSet(string setId)
        {
            return _setRegistry.TryGetValue(setId, out var set) ? set : null;
        }

        /// <summary>
        /// Gets all registered gear sets.
        /// </summary>
        public List<GearSetDefinition> GetAllSets()
        {
            return _setRegistry.Values.ToList();
        }

        // ---------------------------------------------------------------
        // ACTIVE SET TRACKING
        // ---------------------------------------------------------------

        /// <summary>
        /// Updates the equipped piece count for a set and recalculates bonuses.
        /// </summary>
        public void UpdateSetPieces(string setId, int equippedCount)
        {
            if (!_setRegistry.ContainsKey(setId)) return;

            if (!_activeSets.TryGetValue(setId, out var active))
            {
                active = new ActiveSetBonus(_setRegistry[setId]);
                _activeSets[setId] = active;
            }

            var previousTiers = new HashSet<string>(active.ActiveTiers.Select(t => t.DisplayName));
            active.UpdateEquippedPieces(equippedCount);
            var newTiers = new HashSet<string>(active.ActiveTiers.Select(t => t.DisplayName));

            // Fire events for changes
            foreach (var tier in previousTiers)
            {
                if (!newTiers.Contains(tier))
                {
                    OnSetBonusDeactivated?.Invoke(setId, active.ActiveTiers.FirstOrDefault(t => t.DisplayName == tier)?.PiecesRequired ?? 0);
                }
            }
            foreach (var tier in newTiers)
            {
                if (!previousTiers.Contains(tier))
                {
                    OnSetBonusActivated?.Invoke(setId, active.ActiveTiers.First(t => t.DisplayName == tier).PiecesRequired);
                }
            }

            if (equippedCount <= 0)
            {
                _activeSets.Remove(setId);
            }
        }

        /// <summary>
        /// Gets the active set bonus state for a set.
        /// </summary>
        public ActiveSetBonus GetActiveSet(string setId)
        {
            return _activeSets.TryGetValue(setId, out var active) ? active : null;
        }

        /// <summary>
        /// Gets all currently active set bonuses.
        /// </summary>
        public List<ActiveSetBonus> GetAllActiveSets()
        {
            return _activeSets.Values.ToList();
        }

        /// <summary>
        /// Clears all active set tracking (e.g., on death or zone change).
        /// </summary>
        public void ClearActiveSets()
        {
            foreach (var kvp in _activeSets)
            {
                foreach (var tier in kvp.Value.ActiveTiers)
                {
                    OnSetBonusDeactivated?.Invoke(kvp.Key, tier.PiecesRequired);
                }
            }
            _activeSets.Clear();
        }

        // ---------------------------------------------------------------
        // DEFAULT SETS
        // ---------------------------------------------------------------

        /// <summary>
        /// Creates default gear set definitions.
        /// </summary>
        public static List<GearSetDefinition> CreateDefaultSets()
        {
            return new List<GearSetDefinition>
            {
                new("set_iron_warrior", "Iron Warrior", "A sturdy set for front-line combat",
                    new List<string> { "arm_iron_helmet", "arm_iron_chest", "arm_iron_boots", "arm_iron_gloves" },
                    new List<SetBonusTier>
                    {
                        new(2, "Iron Fortitude", "+10% Defense", new List<SetBonusEffect>
                        {
                            new("set_iron_2_def", "Defense", 0.10f, true)
                        }),
                        new(4, "Iron Will", "+20% Defense, +5% Health", new List<SetBonusEffect>
                        {
                            new("set_iron_4_def", "Defense", 0.20f, true),
                            new("set_iron_4_hp", "Health", 0.05f, true)
                        })
                    }),

                new("set_flame_mage", "Flame Mage", "Enhances fire magic abilities",
                    new List<string> { "arm_flame_hat", "arm_flame_robe", "arm_flame_ring" },
                    new List<SetBonusTier>
                    {
                        new(2, "Flame Affinity", "+15% Fire Damage", new List<SetBonusEffect>
                        {
                            new("set_flame_2_fire", "FireDamage", 0.15f, true)
                        }),
                        new(3, "Inferno", "+30% Fire Damage, +10 Mana/sec", new List<SetBonusEffect>
                        {
                            new("set_flame_3_fire", "FireDamage", 0.30f, true),
                            new("set_flame_3_mana", "ManaRegen", 10f, false)
                        })
                    }),

                new("set_shadow_assassin", "Shadow Assassin", "Deadly from the shadows",
                    new List<string> { "arm_shadow_hood", "arm_shadow_tunic", "arm_shadow_daggers" },
                    new List<SetBonusTier>
                    {
                        new(2, "Shadow Step", "+10% Critical Chance", new List<SetBonusEffect>
                        {
                            new("set_shadow_2_crit", "CriticalRate", 0.10f, true)
                        }),
                        new(3, "Death Strike", "+20% Critical Chance, +30% Critical Damage", new List<SetBonusEffect>
                        {
                            new("set_shadow_3_crit", "CriticalRate", 0.20f, true),
                            new("set_shadow_3_critdmg", "CriticalDamage", 0.30f, true)
                        })
                    }),

                new("set_guardian", "Guardian", "Unbreakable defense",
                    new List<string> { "arm_guardian_helm", "arm_guardian_plate", "arm_guardian_shield", "arm_guardian_greaves" },
                    new List<SetBonusTier>
                    {
                        new(2, "Guardian's Resolve", "+15% Block Chance", new List<SetBonusEffect>
                        {
                            new("set_guardian_2_block", "BlockChance", 0.15f, true)
                        }),
                        new(4, "Unbreakable", "+30% Block Chance, +20% All Resistances", new List<SetBonusEffect>
                        {
                            new("set_guardian_4_block", "BlockChance", 0.30f, true),
                            new("set_guardian_4_res", "AllResistances", 0.20f, true)
                        })
                    })
            };
        }
    }
}