using System;

namespace HeroOfEternia.Player.Stats
{
    /// <summary>
    /// Math application type of the modifier.
    /// </summary>
    public enum ModifierType
    {
        Flat,         // Adds directly: value + mod
        PercentAdd,   // Adds to scaling: value * (1 + sum(mods))
        PercentMult   // Multiplies separately: value * prod(1 + mod)
    }

    /// <summary>
    /// Source origin of the modifier.
    /// </summary>
    public enum ModifierSource
    {
        Equipment,
        Potion,
        Buff,
        Debuff,
        Skill,
        Temporary,
        Permanent
    }

    /// <summary>
    /// Represents a temporary or permanent modifier applied to a character attribute.
    /// </summary>
    public class StatModifier
    {
        public string Id { get; }
        public float Value { get; }
        public ModifierType Type { get; }
        public ModifierSource Source { get; }
        
        /// <summary>Duration in seconds. Values <= 0 indicate permanent modifiers.</summary>
        public double Duration { get; }
        public double TimeElapsed { get; private set; }

        public bool IsExpired => Duration > 0f && TimeElapsed >= Duration;

        public StatModifier(string id, float value, ModifierType type, ModifierSource source, double duration = 0.0)
        {
            Id = id;
            Value = value;
            Type = type;
            Source = source;
            Duration = duration;
            TimeElapsed = 0.0;
        }

        /// <summary>
        /// Updates the duration timer. Returns true if the modifier has expired.
        /// </summary>
        public bool Update(float delta)
        {
            if (Duration <= 0.0) return false;
            TimeElapsed += delta;
            return IsExpired;
        }
    }
}
