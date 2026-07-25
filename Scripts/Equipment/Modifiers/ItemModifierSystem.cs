using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Player.Stats;

namespace HeroOfEternia.Equipment.Modifiers
{
    /// <summary>
    /// Defines the type of item modifier for stacking rules.
    /// </summary>
    public enum ModifierStackType
    {
        /// <summary>Values add together (e.g., +5 Attack + +3 Attack = +8 Attack)</summary>
        Additive,
        /// <summary>Only the highest value applies (e.g., +5% Crit from two sources = +5% Crit)</summary>
        HighestOnly,
        /// <summary>Only the lowest value applies (e.g., damage reduction)</summary>
        LowestOnly,
        /// <summary>Values multiply together (e.g., 1.1 * 1.05 = 1.155)</summary>
        Multiplicative,
        /// <summary>Values override each other (last applied wins)</summary>
        Override
    }

    /// <summary>
    /// A reusable item modifier definition.
    /// Can be applied to equipment, buffs, or any game system.
    /// </summary>
    public class ItemModifier
    {
        public string Id { get; }
        public string DisplayName { get; }
        public AttributeType TargetAttribute { get; }
        public float Value { get; }
        public ModifierType ModifierType { get; }
        public ModifierStackType StackType { get; }
        public string Category { get; }
        public string Description { get; }

        public ItemModifier(
            string id,
            string displayName,
            AttributeType targetAttribute,
            float value,
            ModifierType modifierType,
            ModifierStackType stackType = ModifierStackType.Additive,
            string category = "",
            string description = "")
        {
            Id = id;
            DisplayName = displayName;
            TargetAttribute = targetAttribute;
            Value = value;
            ModifierType = modifierType;
            StackType = stackType;
            Category = category;
            Description = description;
        }

        /// <summary>
        /// Creates a StatModifier from this ItemModifier for use in the attribute system.
        /// </summary>
        public StatModifier ToStatModifier(string sourceId, ModifierSource source = ModifierSource.Equipment)
        {
            return new StatModifier(sourceId, Value, ModifierType, source);
        }
    }

    /// <summary>
    /// Manages the registration, lookup, and stacking of item modifiers.
    /// Supports configurable stacking rules and category-based organization.
    /// </summary>
    public class ItemModifierSystem
    {
        private readonly Dictionary<string, ItemModifier> _modifierRegistry = new();
        private readonly Dictionary<string, List<string>> _categoryIndex = new();

        /// <summary>
        /// Registers a modifier definition for reuse.
        /// </summary>
        public void RegisterModifier(ItemModifier modifier)
        {
            _modifierRegistry[modifier.Id] = modifier;

            if (!string.IsNullOrEmpty(modifier.Category))
            {
                if (!_categoryIndex.TryGetValue(modifier.Category, out var list))
                {
                    list = new List<string>();
                    _categoryIndex[modifier.Category] = list;
                }
                if (!list.Contains(modifier.Id))
                    list.Add(modifier.Id);
            }
        }

        /// <summary>
        /// Gets a registered modifier by ID.
        /// </summary>
        public ItemModifier GetModifier(string id)
        {
            return _modifierRegistry.TryGetValue(id, out var mod) ? mod : null;
        }

        /// <summary>
        /// Gets all modifiers in a category.
        /// </summary>
        public List<ItemModifier> GetModifiersByCategory(string category)
        {
            if (_categoryIndex.TryGetValue(category, out var ids))
            {
                return ids.Select(id => _modifierRegistry[id]).ToList();
            }
            return new List<ItemModifier>();
        }

        /// <summary>
        /// Gets all registered modifiers.
        /// </summary>
        public List<ItemModifier> GetAllModifiers()
        {
            return _modifierRegistry.Values.ToList();
        }

        /// <summary>
        /// Calculates the final value of a set of modifiers on the same attribute,
        /// respecting stacking rules.
        /// </summary>
        public static float CalculateStackedValue(List<ItemModifier> modifiers, float baseValue = 0f)
        {
            if (modifiers == null || modifiers.Count == 0)
                return baseValue;

            // Group by stack type
            var additive = modifiers.Where(m => m.StackType == ModifierStackType.Additive).ToList();
            var highest = modifiers.Where(m => m.StackType == ModifierStackType.HighestOnly).ToList();
            var lowest = modifiers.Where(m => m.StackType == ModifierStackType.LowestOnly).ToList();
            var multiplicative = modifiers.Where(m => m.StackType == ModifierStackType.Multiplicative).ToList();
            var overrides = modifiers.Where(m => m.StackType == ModifierStackType.Override).ToList();

            float result = baseValue;

            // Additive: sum all values
            if (additive.Count > 0)
            {
                float sum = additive.Sum(m => m.Value);
                result += sum;
            }

            // Highest: take the max
            if (highest.Count > 0)
            {
                float max = highest.Max(m => m.Value);
                result = Math.Max(result, max);
            }

            // Lowest: take the min
            if (lowest.Count > 0)
            {
                float min = lowest.Min(m => m.Value);
                result = Math.Min(result, min);
            }

            // Multiplicative: multiply all
            if (multiplicative.Count > 0)
            {
                float product = multiplicative.Aggregate(1f, (acc, m) => acc * (1f + m.Value));
                result *= product;
            }

            // Override: last one wins
            if (overrides.Count > 0)
            {
                result = overrides.Last().Value;
            }

            return result;
        }

        /// <summary>
        /// Creates default modifier definitions for common equipment stats.
        /// </summary>
        public static List<ItemModifier> CreateDefaultModifiers()
        {
            return new List<ItemModifier>
            {
                // Attack modifiers
                new("mod_atk_flat_5", "+5 Attack", AttributeType.Attack, 5f, ModifierType.Flat, ModifierStackType.Additive, "attack"),
                new("mod_atk_flat_10", "+10 Attack", AttributeType.Attack, 10f, ModifierType.Flat, ModifierStackType.Additive, "attack"),
                new("mod_atk_pct_5", "+5% Attack", AttributeType.Attack, 0.05f, ModifierType.PercentAdd, ModifierStackType.Additive, "attack"),

                // Health modifiers
                new("mod_hp_flat_10", "+10 Health", AttributeType.Health, 10f, ModifierType.Flat, ModifierStackType.Additive, "health"),
                new("mod_hp_flat_50", "+50 Health", AttributeType.Health, 50f, ModifierType.Flat, ModifierStackType.Additive, "health"),
                new("mod_hp_pct_5", "+5% Health", AttributeType.Health, 0.05f, ModifierType.PercentAdd, ModifierStackType.Additive, "health"),

                // Defense modifiers
                new("mod_def_flat_3", "+3 Defense", AttributeType.Defense, 3f, ModifierType.Flat, ModifierStackType.Additive, "defense"),
                new("mod_def_flat_5", "+5 Defense", AttributeType.Defense, 5f, ModifierType.Flat, ModifierStackType.Additive, "defense"),

                // Critical modifiers
                new("mod_crit_rate_2", "+2% Critical Chance", AttributeType.CriticalRate, 0.02f, ModifierType.Flat, ModifierStackType.Additive, "critical"),
                new("mod_crit_rate_5", "+5% Critical Chance", AttributeType.CriticalRate, 0.05f, ModifierType.Flat, ModifierStackType.Additive, "critical"),
                new("mod_crit_dmg_10", "+10% Critical Damage", AttributeType.CriticalDamage, 0.10f, ModifierType.PercentAdd, ModifierStackType.Additive, "critical"),

                // Speed modifiers
                new("mod_movespeed_5", "+5% Movement Speed", AttributeType.MovementSpeed, 0.05f, ModifierType.PercentAdd, ModifierStackType.Additive, "speed"),
                new("mod_atkspeed_3", "+3% Attack Speed", AttributeType.AttackSpeed, 0.03f, ModifierType.PercentAdd, ModifierStackType.Additive, "speed"),

                // Elemental resistance modifiers
                new("mod_fire_res_10", "+10 Fire Resistance", AttributeType.FireResistance, 10f, ModifierType.Flat, ModifierStackType.Additive, "resistance"),
                new("mod_ice_res_10", "+10 Ice Resistance", AttributeType.IceResistance, 10f, ModifierType.Flat, ModifierStackType.Additive, "resistance"),
                new("mod_lightning_res_10", "+10 Lightning Resistance", AttributeType.LightningResistance, 10f, ModifierType.Flat, ModifierStackType.Additive, "resistance"),
                new("mod_poison_res_10", "+10 Poison Resistance", AttributeType.PoisonResistance, 10f, ModifierType.Flat, ModifierStackType.Additive, "resistance"),

                // Special hooks
                new("mod_lifesteal_2", "+2% Life Steal", AttributeType.LifeSteal, 0.02f, ModifierType.Flat, ModifierStackType.Additive, "special"),
                new("mod_manaregen_2", "+2 Mana/sec", AttributeType.ManaRegen, 2f, ModifierType.Flat, ModifierStackType.Additive, "special"),
                new("mod_xp_bonus_5", "+5% Experience", AttributeType.ExperienceBonus, 0.05f, ModifierType.PercentAdd, ModifierStackType.Additive, "special"),
                new("mod_gold_bonus_5", "+5% Gold", AttributeType.GoldBonus, 0.05f, ModifierType.PercentAdd, ModifierStackType.Additive, "special"),

                // Magic modifiers
                new("mod_matk_flat_5", "+5 Magic Attack", AttributeType.MagicAttack, 5f, ModifierType.Flat, ModifierStackType.Additive, "magic"),
                new("mod_mdef_flat_3", "+3 Magic Defense", AttributeType.MagicDefense, 3f, ModifierType.Flat, ModifierStackType.Additive, "magic"),

                // Defensive modifiers
                new("mod_block_3", "+3% Block Chance", AttributeType.BlockChance, 0.03f, ModifierType.Flat, ModifierStackType.Additive, "defensive"),
                new("mod_dodge_2", "+2% Dodge Chance", AttributeType.DodgeChance, 0.02f, ModifierType.Flat, ModifierStackType.Additive, "defensive"),
            };
        }
    }
}