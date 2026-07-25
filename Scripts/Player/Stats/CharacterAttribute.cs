using System;
using System.Collections.Generic;

namespace HeroOfEternia.Player.Stats
{
    /// <summary>
    /// Type identifier for stats/attributes.
    /// Extended to support all equipment-related stats in Prompt 14.
    /// </summary>
    public enum AttributeType
    {
        // Core Vitals
        Health,
        Mana,
        Energy,
        Stamina,
        
        // Core Stats
        Strength,
        Vitality,
        Magic,
        Dexterity,
        Luck,
        
        // Combat Stats
        Attack,
        MagicAttack,
        Defense,
        MagicDefense,
        Speed,
        CriticalRate,
        CriticalDamage,
        AttackSpeed,
        CastingSpeed,
        MovementSpeed,
        
        // Defensive Stats
        BlockChance,
        DodgeChance,
        
        // Elemental Resistances
        FireResistance,
        IceResistance,
        LightningResistance,
        PoisonResistance,
        HolyResistance,
        ShadowResistance,
        
        // Status Resistances
        StunResistance,
        FreezeResistance,
        BurnResistance,
        BleedResistance,
        SilenceResistance,
        KnockbackResistance,
        
        // Special Hooks (value-based)
        LifeSteal,
        ManaRegen,
        HealthRegen,
        ExperienceBonus,
        GoldBonus,
        
        // Custom / Future
        Custom
    }

    /// <summary>
    /// Represents a single character attribute, managing its modifiers and calculating current value.
    /// </summary>
    public class CharacterAttribute
    {
        public AttributeType Type { get; }
        
        private float _baseValue;
        private float _currentValue;
        private bool _isDirty = true;

        private readonly List<StatModifier> _modifiers = new();

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                if (Math.Abs(_baseValue - value) > 0.0001f)
                {
                    _baseValue = value;
                    _isDirty = true;
                }
            }
        }

        public float CurrentValue
        {
            get
            {
                if (_isDirty)
                {
                    RecalculateValue();
                }
                return _currentValue;
            }
        }

        public IReadOnlyList<StatModifier> Modifiers => _modifiers;

        public CharacterAttribute(AttributeType type, float baseValue)
        {
            Type = type;
            _baseValue = baseValue;
            _isDirty = true;
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            _isDirty = true;
        }

        public bool RemoveModifier(string modifierId)
        {
            int index = _modifiers.FindIndex(m => m.Id == modifierId);
            if (index >= 0)
            {
                _modifiers.RemoveAt(index);
                _isDirty = true;
                return true;
            }
            return false;
        }

        public void RemoveModifiersFromSource(ModifierSource source)
        {
            int removedCount = _modifiers.RemoveAll(m => m.Source == source);
            if (removedCount > 0)
            {
                _isDirty = true;
            }
        }

        /// <summary>
        /// Updates timed modifiers. Returns true if any modifier expired and values changed.
        /// </summary>
        public bool Update(float delta)
        {
            bool anyExpired = false;
            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                if (_modifiers[i].Update(delta))
                {
                    _modifiers.RemoveAt(i);
                    anyExpired = true;
                }
            }

            if (anyExpired)
            {
                _isDirty = true;
            }
            return anyExpired;
        }

        // ---------------------------------------------------------------
        // PRIVATE CALCULATION
        // ---------------------------------------------------------------

        private void RecalculateValue()
        {
            float flatSum = 0f;
            float pctAddSum = 0f;
            float pctMultProduct = 1f;

            foreach (var mod in _modifiers)
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

            // RPG Standard Formula: (Base + Flat) * (1 + PercentAdd) * Product(1 + PercentMult)
            _currentValue = (_baseValue + flatSum) * (1f + pctAddSum) * pctMultProduct;

            // Clamping rules depending on type (rates/percentage targets shouldn't go below 0)
            if (Type == AttributeType.CriticalRate || Type == AttributeType.CriticalDamage ||
                Type == AttributeType.Health || Type == AttributeType.Mana || Type == AttributeType.Stamina)
            {
                _currentValue = Math.Max(0f, _currentValue);
            }

            _isDirty = false;
        }
    }
}
