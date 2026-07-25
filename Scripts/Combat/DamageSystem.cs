using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    /// <summary>
    /// Per-entity resistance profile. Each DamageType has a float resistance 0.0–1.0.
    /// 0.0 = no resistance, 1.0 = full immunity, negative = vulnerability.
    /// </summary>
    public class ResistanceProfile
    {
        private readonly Dictionary<DamageType, float> _values = new();

        public void Set(DamageType type, float value) =>
            _values[type] = Math.Clamp(value, -1f, 1f);

        public float Get(DamageType type) =>
            _values.TryGetValue(type, out var v) ? v : 0f;

        /// <summary>Convenience builder for elemental resistances.</summary>
        public static ResistanceProfile FromDictionary(Dictionary<DamageType, float> values)
        {
            var p = new ResistanceProfile();
            foreach (var kv in values) p.Set(kv.Key, kv.Value);
            return p;
        }

        public static ResistanceProfile Default() => new ResistanceProfile();
    }

    /// <summary>
    /// Static damage processing service.
    /// Applies resistances, elemental multipliers, critical hit rolls, and True Damage bypass.
    /// </summary>
    public static class DamageSystem
    {
        // Elemental bonus multipliers when hitting a vulnerable target
        private static readonly Dictionary<DamageType, float> ElementalMultipliers = new()
        {
            { DamageType.Fire,      1.25f },
            { DamageType.Ice,       1.20f },
            { DamageType.Lightning, 1.30f },
            { DamageType.Poison,    1.10f },
            { DamageType.Holy,      1.15f },
            { DamageType.Shadow,    1.15f },
            { DamageType.Physical,  1.00f },
            { DamageType.True,      1.00f },
            { DamageType.Healing,   1.00f }
        };

        // ─────────────────────── Main Pipeline ───────────────────────

        /// <summary>
        /// Processes a DamageInstance through the full pipeline:
        /// 1. Critical hit roll
        /// 2. Elemental multiplier
        /// 3. Resistance deduction (True Damage bypasses)
        /// 4. Returns final damage float (negative = heal)
        /// </summary>
        public static float ProcessDamage(DamageInstance dmg, ResistanceProfile resistance,
                                          Random? rng = null)
        {
            rng ??= new Random();

            float final = dmg.BaseDamage;

            // 1. Critical hit
            float critRoll = (float)rng.NextDouble();
            if (critRoll < dmg.CritChance)
            {
                final *= dmg.CritMultiplier;
                dmg.IsCritical = true;
                Logger.Info($"DamageSystem: CRITICAL HIT! {dmg.AttackerId} → {dmg.TargetId} ({final:F1} dmg)");
            }

            // 2. Healing is negative damage — skip resistance/elemental
            if (dmg.Type == DamageType.Healing)
                return -MathF.Abs(final);

            // 3. True Damage bypasses all resistances
            if (dmg.Type == DamageType.True)
                return MathF.Max(0f, final);

            // 4. Elemental multiplier (from attacker's damage type)
            if (ElementalMultipliers.TryGetValue(dmg.Type, out float mult))
                final *= mult;

            // 5. Resistance deduction: resistance 1.0 = immune, 0.0 = no reduction
            float res = resistance.Get(dmg.Type);
            final *= (1f - res);

            return MathF.Max(0f, final);
        }

        /// <summary>
        /// Quick helper: applies damage to a health value and returns the new health.
        /// </summary>
        public static float ApplyToHealth(float currentHealth, float maxHealth,
                                          float processedDamage)
        {
            return Math.Clamp(currentHealth - processedDamage, 0f, maxHealth);
        }

        /// <summary>
        /// Returns whether an entity is dead after taking damage.
        /// </summary>
        public static bool IsLethal(float currentHealth, float processedDamage) =>
            currentHealth - processedDamage <= 0f;
    }
}
