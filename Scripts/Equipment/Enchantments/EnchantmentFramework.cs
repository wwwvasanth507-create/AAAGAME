using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Equipment.Enchantments
{
    /// <summary>
    /// Element types available for enchantments.
    /// </summary>
    public enum EnchantmentElement
    {
        None,
        Fire,
        Ice,
        Lightning,
        Poison,
        Holy,
        Shadow,
        Wind,
        Earth,
        Water,
        Custom
    }

    /// <summary>
    /// Defines an enchantment type with elemental affinity and scaling.
    /// </summary>
    public class EnchantmentDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public EnchantmentElement Element { get; }
        public int MaxLevel { get; }
        public float BaseValue { get; }
        public float ValuePerLevel { get; }
        public EnchantmentTargetType TargetType { get; }
        public string StatAffected { get; }

        public EnchantmentDefinition(
            string id,
            string displayName,
            string description,
            EnchantmentElement element,
            int maxLevel,
            float baseValue,
            float valuePerLevel,
            EnchantmentTargetType targetType,
            string statAffected)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Element = element;
            MaxLevel = maxLevel;
            BaseValue = baseValue;
            ValuePerLevel = valuePerLevel;
            TargetType = targetType;
            StatAffected = statAffected;
        }

        /// <summary>
        /// Gets the value at a given level.
        /// </summary>
        public float GetValueForLevel(int level)
        {
            return BaseValue + (ValuePerLevel * (level - 1));
        }
    }

    /// <summary>
    /// What type of equipment this enchantment can be applied to.
    /// </summary>
    public enum EnchantmentTargetType
    {
        Weapon,
        Armor,
        Accessory,
        Any
    }

    /// <summary>
    /// Runtime instance of an enchantment applied to an item.
    /// </summary>
    public class EnchantmentInstance
    {
        public EnchantmentDefinition Definition { get; }
        public int Level { get; private set; }
        public bool IsActive { get; private set; }

        public EnchantmentInstance(EnchantmentDefinition definition, int level = 1)
        {
            Definition = definition;
            Level = Math.Clamp(level, 1, definition.MaxLevel);
            IsActive = true;
        }

        /// <summary>
        /// Gets the current value of this enchantment at its level.
        /// </summary>
        public float GetCurrentValue()
        {
            return Definition.GetValueForLevel(Level);
        }

        /// <summary>
        /// Increases the enchantment level by 1, up to MaxLevel.
        /// Returns true if the level increased.
        /// </summary>
        public bool LevelUp()
        {
            if (Level >= Definition.MaxLevel)
                return false;
            Level++;
            return true;
        }

        /// <summary>
        /// Sets the enchantment level directly.
        /// </summary>
        public void SetLevel(int level)
        {
            Level = Math.Clamp(level, 1, Definition.MaxLevel);
        }

        /// <summary>
        /// Toggles the active state.
        /// </summary>
        public void SetActive(bool active)
        {
            IsActive = active;
        }
    }

    /// <summary>
    /// Central framework for managing enchantment definitions and their application to equipment.
    /// </summary>
    public class EnchantmentFramework
    {
        private readonly Dictionary<string, EnchantmentDefinition> _enchantmentRegistry = new();
        private readonly Dictionary<EnchantmentElement, List<string>> _elementIndex = new();

        // ---------------------------------------------------------------
        // REGISTRATION
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers an enchantment definition.
        /// </summary>
        public void RegisterEnchantment(EnchantmentDefinition enchantment)
        {
            _enchantmentRegistry[enchantment.Id] = enchantment;

            if (!_elementIndex.TryGetValue(enchantment.Element, out var list))
            {
                list = new List<string>();
                _elementIndex[enchantment.Element] = list;
            }
            if (!list.Contains(enchantment.Id))
                list.Add(enchantment.Id);
        }

        /// <summary>
        /// Gets an enchantment definition by ID.
        /// </summary>
        public EnchantmentDefinition GetEnchantment(string id)
        {
            return _enchantmentRegistry.TryGetValue(id, out var def) ? def : null;
        }

        /// <summary>
        /// Gets all enchantments for a given element.
        /// </summary>
        public List<EnchantmentDefinition> GetEnchantmentsByElement(EnchantmentElement element)
        {
            if (_elementIndex.TryGetValue(element, out var ids))
            {
                return ids.Select(id => _enchantmentRegistry[id]).ToList();
            }
            return new List<EnchantmentDefinition>();
        }

        /// <summary>
        /// Gets all registered enchantments.
        /// </summary>
        public List<EnchantmentDefinition> GetAllEnchantments()
        {
            return _enchantmentRegistry.Values.ToList();
        }

        // ---------------------------------------------------------------
        // DEFAULT ENCHANTMENT DEFINITIONS
        // ---------------------------------------------------------------

        /// <summary>
        /// Creates the default set of enchantment definitions.
        /// </summary>
        public static List<EnchantmentDefinition> CreateDefaultEnchantments()
        {
            return new List<EnchantmentDefinition>
            {
                // Fire
                new("ench_fire_damage", "Burning Strike", "Adds fire damage to attacks", EnchantmentElement.Fire, 10, 5f, 3f, EnchantmentTargetType.Weapon, "FireDamage"),
                new("ench_fire_resist", "Fire Ward", "Increases fire resistance", EnchantmentElement.Fire, 10, 5f, 2f, EnchantmentTargetType.Armor, "FireResistance"),

                // Ice
                new("ench_ice_damage", "Frost Strike", "Adds ice damage to attacks", EnchantmentElement.Ice, 10, 5f, 3f, EnchantmentTargetType.Weapon, "IceDamage"),
                new("ench_ice_resist", "Frost Ward", "Increases ice resistance", EnchantmentElement.Ice, 10, 5f, 2f, EnchantmentTargetType.Armor, "IceResistance"),
                new("ench_ice_slow", "Chilling Touch", "Chance to slow enemies on hit", EnchantmentElement.Ice, 10, 0.05f, 0.03f, EnchantmentTargetType.Weapon, "SlowChance"),

                // Lightning
                new("ench_lightning_damage", "Thunder Strike", "Adds lightning damage to attacks", EnchantmentElement.Lightning, 10, 5f, 3f, EnchantmentTargetType.Weapon, "LightningDamage"),
                new("ench_lightning_resist", "Thunder Ward", "Increases lightning resistance", EnchantmentElement.Lightning, 10, 5f, 2f, EnchantmentTargetType.Armor, "LightningResistance"),

                // Poison
                new("ench_poison_damage", "Venom Strike", "Adds poison damage over time", EnchantmentElement.Poison, 10, 3f, 2f, EnchantmentTargetType.Weapon, "PoisonDamage"),
                new("ench_poison_resist", "Venom Ward", "Increases poison resistance", EnchantmentElement.Poison, 10, 5f, 2f, EnchantmentTargetType.Armor, "PoisonResistance"),

                // Holy
                new("ench_holy_damage", "Holy Strike", "Adds holy damage to attacks", EnchantmentElement.Holy, 10, 5f, 3f, EnchantmentTargetType.Weapon, "HolyDamage"),
                new("ench_holy_resist", "Holy Ward", "Increases holy resistance", EnchantmentElement.Holy, 10, 5f, 2f, EnchantmentTargetType.Armor, "HolyResistance"),
                new("ench_holy_heal", "Blessed Touch", "Chance to heal on hit", EnchantmentElement.Holy, 10, 0.02f, 0.01f, EnchantmentTargetType.Weapon, "LifeSteal"),

                // Shadow
                new("ench_shadow_damage", "Shadow Strike", "Adds shadow damage to attacks", EnchantmentElement.Shadow, 10, 5f, 3f, EnchantmentTargetType.Weapon, "ShadowDamage"),
                new("ench_shadow_resist", "Shadow Ward", "Increases shadow resistance", EnchantmentElement.Shadow, 10, 5f, 2f, EnchantmentTargetType.Armor, "ShadowResistance"),
                new("ench_shadow_lifesteal", "Vampiric Touch", "Converts damage to health", EnchantmentElement.Shadow, 10, 0.02f, 0.01f, EnchantmentTargetType.Weapon, "LifeSteal"),

                // Wind
                new("ench_wind_speed", "Zephyr", "Increases attack speed", EnchantmentElement.Wind, 10, 0.03f, 0.02f, EnchantmentTargetType.Weapon, "AttackSpeed"),
                new("ench_wind_dodge", "Gale Step", "Increases dodge chance", EnchantmentElement.Wind, 10, 0.02f, 0.01f, EnchantmentTargetType.Armor, "DodgeChance"),

                // Earth
                new("ench_earth_defense", "Stone Skin", "Increases defense", EnchantmentElement.Earth, 10, 5f, 3f, EnchantmentTargetType.Armor, "Defense"),
                new("ench_earth_block", "Fortress", "Increases block chance", EnchantmentElement.Earth, 10, 0.02f, 0.01f, EnchantmentTargetType.Armor, "BlockChance"),

                // Water
                new("ench_water_manaregen", "Spring Water", "Increases mana regeneration", EnchantmentElement.Water, 10, 1f, 0.5f, EnchantmentTargetType.Accessory, "ManaRegen"),
                new("ench_water_healing", "Healing Springs", "Increases healing received", EnchantmentElement.Water, 10, 0.03f, 0.02f, EnchantmentTargetType.Accessory, "HealingReceived"),
            };
        }
    }
}