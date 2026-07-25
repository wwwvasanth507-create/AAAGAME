using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HeroOfEternia.Core;
using HeroOfEternia.Player.Stats;

namespace HeroOfEternia.Equipment.Attributes
{
    /// <summary>
    /// Centralized attribute calculation engine.
    /// Provides a deterministic, cache-friendly pipeline for computing final attribute values
    /// from multiple modifier sources (base, equipment, abilities, buffs, debuffs, environment, difficulty, guild, mount, pet).
    /// </summary>
    public class AttributeCalculationEngine
    {
        // ---------------------------------------------------------------
        // CACHE
        // ---------------------------------------------------------------
        private readonly Dictionary<AttributeType, CachedAttribute> _cache = new();
        private bool _globalDirty = true;

        // ---------------------------------------------------------------
        // MODIFIER LAYERS
        // ---------------------------------------------------------------
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _baseModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _equipmentModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _abilityModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _buffModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _debuffModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _environmentModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _difficultyModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _guildModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _mountModifiers = new();
        private readonly Dictionary<AttributeType, List<EquipmentModifier>> _petModifiers = new();

        // ---------------------------------------------------------------
        // CONFIGURABLE LAYER ORDER
        // ---------------------------------------------------------------
        private static readonly ModifierLayer[] LayerOrder = new[]
        {
            ModifierLayer.Base,
            ModifierLayer.Equipment,
            ModifierLayer.Ability,
            ModifierLayer.Buff,
            ModifierLayer.Debuff,
            ModifierLayer.Environment,
            ModifierLayer.Difficulty,
            ModifierLayer.Guild,
            ModifierLayer.Mount,
            ModifierLayer.Pet
        };

        // ---------------------------------------------------------------
        // EVENTS
        // ---------------------------------------------------------------
        public event Action<AttributeType, float, float> OnAttributeRecalculated;

        // ---------------------------------------------------------------
        // PUBLIC API
        // ---------------------------------------------------------------

        /// <summary>
        /// Gets the final calculated value for an attribute.
        /// Uses cached value if no changes have occurred.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetValue(AttributeType type)
        {
            if (_globalDirty || (_cache.TryGetValue(type, out var cached) && cached.IsDirty))
            {
                return Recalculate(type);
            }
            return _cache.TryGetValue(type, out var valid) ? valid.Value : 0f;
        }

        /// <summary>
        /// Gets all currently calculated attribute values as a snapshot dictionary.
        /// </summary>
        public Dictionary<AttributeType, float> GetAllValues()
        {
            var result = new Dictionary<AttributeType, float>();
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
            {
                result[type] = GetValue(type);
            }
            return result;
        }

        /// <summary>
        /// Adds a modifier to the specified layer.
        /// </summary>
        public void AddModifier(ModifierLayer layer, AttributeType type, EquipmentModifier modifier)
        {
            var dict = GetLayerDictionary(layer);
            if (!dict.TryGetValue(type, out var list))
            {
                list = new List<EquipmentModifier>();
                dict[type] = list;
            }
            list.Add(modifier);
            MarkDirty(type);
        }

        /// <summary>
        /// Removes a modifier by ID from the specified layer.
        /// </summary>
        public bool RemoveModifier(ModifierLayer layer, AttributeType type, string modifierId)
        {
            var dict = GetLayerDictionary(layer);
            if (dict.TryGetValue(type, out var list))
            {
                int count = list.RemoveAll(m => m.Id == modifierId);
                if (count > 0)
                {
                    MarkDirty(type);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all modifiers from a specific layer for a given attribute type.
        /// </summary>
        public void ClearLayer(ModifierLayer layer, AttributeType? type = null)
        {
            var dict = GetLayerDictionary(layer);
            if (type.HasValue)
            {
                if (dict.Remove(type.Value))
                    MarkDirty(type.Value);
            }
            else
            {
                foreach (var key in dict.Keys)
                    MarkDirty(key);
                dict.Clear();
            }
        }

        /// <summary>
        /// Removes all modifiers from all layers.
        /// </summary>
        public void ClearAll()
        {
            foreach (var layer in LayerOrder)
            {
                GetLayerDictionary(layer).Clear();
            }
            _globalDirty = true;
            _cache.Clear();
        }

        /// <summary>
        /// Marks a specific attribute as needing recalculation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkDirty(AttributeType type)
        {
            if (_cache.TryGetValue(type, out var cached))
            {
                cached.IsDirty = true;
            }
            else
            {
                _cache[type] = new CachedAttribute { IsDirty = true };
            }
        }

        /// <summary>
        /// Marks all attributes as needing recalculation.
        /// </summary>
        public void MarkAllDirty()
        {
            _globalDirty = true;
        }

        // ---------------------------------------------------------------
        // PRIVATE CALCULATION PIPELINE
        // ---------------------------------------------------------------

        private float Recalculate(AttributeType type)
        {
            float flatSum = 0f;
            float pctAddSum = 0f;
            float pctMultProduct = 1f;

            // Process layers in order
            foreach (var layer in LayerOrder)
            {
                var dict = GetLayerDictionary(layer);
                if (dict.TryGetValue(type, out var modifiers))
                {
                    foreach (var mod in modifiers)
                    {
                        switch (mod.Type)
                        {
                            case ModifierType.Flat:
                                flatSum += mod.Value;
                                break;
                            case ModifierType.PercentAdd:
                                pctAddSum += mod.Value;
                                break;
                            case ModifierType.PercentMult:
                                pctMultProduct *= (1f + mod.Value);
                                break;
                        }
                    }
                }
            }

            // Base value from the layer system (layer order ensures Base is first)
            float baseValue = 0f;
            if (_baseModifiers.TryGetValue(type, out var baseMods) && baseMods.Count > 0)
            {
                foreach (var mod in baseMods)
                {
                    if (mod.Type == ModifierType.Flat)
                        baseValue += mod.Value;
                }
            }

            // RPG Standard Formula: (Base + Flat) * (1 + PercentAdd) * Product(1 + PercentMult)
            float finalValue = (baseValue + flatSum) * (1f + pctAddSum) * pctMultProduct;

            // Clamping rules
            finalValue = ApplyClamping(type, finalValue);

            // Update cache
            _cache[type] = new CachedAttribute { Value = finalValue, IsDirty = false };

            // Fire event
            OnAttributeRecalculated?.Invoke(type, baseValue, finalValue);

            return finalValue;
        }

        private static float ApplyClamping(AttributeType type, float value)
        {
            switch (type)
            {
                case AttributeType.CriticalRate:
                case AttributeType.CriticalDamage:
                case AttributeType.Health:
                case AttributeType.Mana:
                case AttributeType.Stamina:
                case AttributeType.Energy:
                    return Math.Max(0f, value);
                case AttributeType.BlockChance:
                case AttributeType.DodgeChance:
                    return Math.Clamp(value, 0f, 0.95f); // Cap at 95%
                case AttributeType.AttackSpeed:
                case AttributeType.CastingSpeed:
                case AttributeType.MovementSpeed:
                    return Math.Max(0.1f, value); // Minimum 10% speed
                default:
                    return value;
            }
        }

        private Dictionary<AttributeType, List<EquipmentModifier>> GetLayerDictionary(ModifierLayer layer)
        {
            return layer switch
            {
                ModifierLayer.Base => _baseModifiers,
                ModifierLayer.Equipment => _equipmentModifiers,
                ModifierLayer.Ability => _abilityModifiers,
                ModifierLayer.Buff => _buffModifiers,
                ModifierLayer.Debuff => _debuffModifiers,
                ModifierLayer.Environment => _environmentModifiers,
                ModifierLayer.Difficulty => _difficultyModifiers,
                ModifierLayer.Guild => _guildModifiers,
                ModifierLayer.Mount => _mountModifiers,
                ModifierLayer.Pet => _petModifiers,
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }

        // ---------------------------------------------------------------
        // INNER TYPES
        // ---------------------------------------------------------------

        private class CachedAttribute
        {
            public float Value;
            public bool IsDirty = true;
        }
    }

    /// <summary>
    /// Defines the processing order for modifier layers.
    /// Lower enum values are processed first.
    /// </summary>
    public enum ModifierLayer
    {
        Base = 0,
        Equipment = 1,
        Ability = 2,
        Buff = 3,
        Debuff = 4,
        Environment = 5,
        Difficulty = 6,
        Guild = 7,
        Mount = 8,
        Pet = 9
    }

    /// <summary>
    /// A single modifier entry for the calculation engine.
    /// </summary>
    public class EquipmentModifier
    {
        public string Id { get; }
        public float Value { get; }
        public ModifierType Type { get; }
        public string SourceId { get; }

        public EquipmentModifier(string id, float value, ModifierType type, string sourceId = "")
        {
            Id = id;
            Value = value;
            Type = type;
            SourceId = sourceId;
        }
    }
}