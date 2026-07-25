using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player.Stats
{
    /// <summary>
    /// Configuration model matching the JSON structure for player attributes.
    /// </summary>
    public class AttributesConfig
    {
        public float Health { get; set; } = 100f;
        public float Mana { get; set; } = 50f;
        public float Energy { get; set; } = 100f;
        public float Stamina { get; set; } = 100f;
        public float Strength { get; set; } = 10f;
        public float Vitality { get; set; } = 10f;
        public float Magic { get; set; } = 5f;
        public float Dexterity { get; set; } = 5f;
        public float Luck { get; set; } = 5f;
        public float Attack { get; set; } = 15f;
        public float Defense { get; set; } = 5f;
        public float Speed { get; set; } = 10f;
        public float CriticalRate { get; set; } = 0.05f;
        public float CriticalDamage { get; set; } = 1.5f;
    }

    /// <summary>
    /// Manages the full set of active attributes for a player character.
    /// Handles timed modifier cleanup and data-driven base stat loading.
    /// </summary>
    public class PlayerAttributeSet
    {
        private readonly Dictionary<AttributeType, CharacterAttribute> _attributes = new();

        public PlayerAttributeSet()
        {
            InitializeDefaultAttributes();
        }

        private void InitializeDefaultAttributes()
        {
            AttributesConfig config = new AttributesConfig();

            // Attempt to load from ConfigManager in ServiceLocator
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                if (configManager != null)
                {
                    var loadedConfig = configManager.GetConfig<AttributesConfig>("player_attributes");
                    if (loadedConfig != null)
                    {
                        config = loadedConfig;
                    }
                }
            }
            catch
            {
                // In headless tests, ServiceLocator may not contain ConfigManager. Safe fallback occurs.
            }

            // Create attribute wrappers
            _attributes[AttributeType.Health] = new CharacterAttribute(AttributeType.Health, config.Health);
            _attributes[AttributeType.Mana] = new CharacterAttribute(AttributeType.Mana, config.Mana);
            _attributes[AttributeType.Energy] = new CharacterAttribute(AttributeType.Energy, config.Energy);
            _attributes[AttributeType.Stamina] = new CharacterAttribute(AttributeType.Stamina, config.Stamina);
            _attributes[AttributeType.Strength] = new CharacterAttribute(AttributeType.Strength, config.Strength);
            _attributes[AttributeType.Vitality] = new CharacterAttribute(AttributeType.Vitality, config.Vitality);
            _attributes[AttributeType.Magic] = new CharacterAttribute(AttributeType.Magic, config.Magic);
            _attributes[AttributeType.Dexterity] = new CharacterAttribute(AttributeType.Dexterity, config.Dexterity);
            _attributes[AttributeType.Luck] = new CharacterAttribute(AttributeType.Luck, config.Luck);
            _attributes[AttributeType.Attack] = new CharacterAttribute(AttributeType.Attack, config.Attack);
            _attributes[AttributeType.Defense] = new CharacterAttribute(AttributeType.Defense, config.Defense);
            _attributes[AttributeType.Speed] = new CharacterAttribute(AttributeType.Speed, config.Speed);
            _attributes[AttributeType.CriticalRate] = new CharacterAttribute(AttributeType.CriticalRate, config.CriticalRate);
            _attributes[AttributeType.CriticalDamage] = new CharacterAttribute(AttributeType.CriticalDamage, config.CriticalDamage);
        }

        public float GetValue(AttributeType type)
        {
            return _attributes.TryGetValue(type, out var attr) ? attr.CurrentValue : 0f;
        }

        public float GetBaseValue(AttributeType type)
        {
            return _attributes.TryGetValue(type, out var attr) ? attr.BaseValue : 0f;
        }

        public void SetBaseValue(AttributeType type, float baseValue)
        {
            if (_attributes.TryGetValue(type, out var attr))
            {
                attr.BaseValue = baseValue;
            }
        }

        public void AddModifier(AttributeType type, StatModifier modifier)
        {
            if (_attributes.TryGetValue(type, out var attr))
            {
                attr.AddModifier(modifier);
            }
        }

        public void RemoveModifier(AttributeType type, string modifierId)
        {
            if (_attributes.TryGetValue(type, out var attr))
            {
                attr.RemoveModifier(modifierId);
            }
        }

        public void RemoveModifiersFromSource(ModifierSource source)
        {
            foreach (var attr in _attributes.Values)
            {
                attr.RemoveModifiersFromSource(source);
            }
        }

        /// <summary>
        /// Ticks all active modifiers. Should be called per-frame.
        /// </summary>
        public void Update(float delta)
        {
            foreach (var attr in _attributes.Values)
            {
                attr.Update(delta);
            }
        }
    }
}
