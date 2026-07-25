using System;
using System.Collections.Generic;
using HeroOfEternia.Enemies;

namespace HeroOfEternia.Combat
{
    [Flags]
    public enum EliteModifierType
    {
        None = 0,
        Fortified = 1 << 0,  // Increased HP and armor
        Swift = 1 << 1,      // Increased move and attack speed
        Fireborn = 1 << 2,   // Fire elemental affinity, deals fire damage
        Frostshield = 1 << 3,// Ice elemental, higher defense
        Vampiric = 1 << 4,   // Heals on hits
        Summoner = 1 << 5    // Periodically summons goblin grunts
    }

    public record EliteDefinition
    {
        public EliteModifierType Modifiers { get; init; } = EliteModifierType.None;
        public float HpMultiplier { get; init; } = 1.0f;
        public float DamageMultiplier { get; init; } = 1.0f;
        public float SpeedMultiplier { get; init; } = 1.0f;
        public float LootMultiplier { get; init; } = 1.0f;
        public float XpMultiplier { get; init; } = 1.0f;
        public string NamePrefix { get; init; } = string.Empty;
        public string NameSuffix { get; init; } = string.Empty;
        public string VisualColorOverlay { get; init; } = string.Empty; // Hex color overlay code
    }

    public static class EliteSystem
    {
        public static EliteDefinition ResolveModifiers(EliteModifierType modifiers)
        {
            float hpMult = 1.0f;
            float dmgMult = 1.0f;
            float spdMult = 1.0f;
            float lootMult = 1.0f;
            float xpMult = 1.0f;
            var prefixes = new List<string>();
            var suffixes = new List<string>();
            string color = "#FFFFFF";

            if ((modifiers & EliteModifierType.Fortified) != 0)
            {
                hpMult *= 2.0f;
                prefixes.Add("Fortified");
                lootMult *= 1.2f;
                xpMult *= 1.3f;
                color = "#999999";
            }

            if ((modifiers & EliteModifierType.Swift) != 0)
            {
                spdMult *= 1.35f;
                dmgMult *= 1.1f;
                prefixes.Add("Swift");
                lootMult *= 1.15f;
                xpMult *= 1.25f;
                color = "#FFFF99";
            }

            if ((modifiers & EliteModifierType.Fireborn) != 0)
            {
                dmgMult *= 1.25f;
                prefixes.Add("Fireborn");
                lootMult *= 1.3f;
                xpMult *= 1.4f;
                color = "#FF3333";
            }

            if ((modifiers & EliteModifierType.Frostshield) != 0)
            {
                hpMult *= 1.3f;
                prefixes.Add("Glacial");
                lootMult *= 1.3f;
                xpMult *= 1.4f;
                color = "#33CCFF";
            }

            if ((modifiers & EliteModifierType.Vampiric) != 0)
            {
                dmgMult *= 1.15f;
                suffixes.Add("the Leech");
                lootMult *= 1.4f;
                xpMult *= 1.5f;
                color = "#CC0000";
            }

            if ((modifiers & EliteModifierType.Summoner) != 0)
            {
                hpMult *= 1.5f;
                suffixes.Add("the Broodmother");
                lootMult *= 1.5f;
                xpMult *= 1.6f;
                color = "#6600CC";
            }

            string finalPrefix = prefixes.Count > 0 ? string.Join(" ", prefixes) + " " : string.Empty;
            string finalSuffix = suffixes.Count > 0 ? " " + string.Join(" and ", suffixes) : string.Empty;

            return new EliteDefinition
            {
                Modifiers = modifiers,
                HpMultiplier = hpMult,
                DamageMultiplier = dmgMult,
                SpeedMultiplier = spdMult,
                LootMultiplier = lootMult,
                XpMultiplier = xpMult,
                NamePrefix = finalPrefix,
                NameSuffix = finalSuffix,
                VisualColorOverlay = color
            };
        }

        public static EnemyData ApplyEliteModifiers(EnemyData baseEnemy, EliteModifierType modifiers)
        {
            if (modifiers == EliteModifierType.None) return baseEnemy;

            var elite = ResolveModifiers(modifiers);
            string finalName = elite.NamePrefix + baseEnemy.DisplayName + elite.NameSuffix;

            // Scale weaknesses/resistances based on modifier flags
            var resistances = new Dictionary<string, float>(baseEnemy.Resistances, StringComparer.OrdinalIgnoreCase);
            var weaknesses = new Dictionary<string, float>(baseEnemy.Weaknesses, StringComparer.OrdinalIgnoreCase);

            if ((modifiers & EliteModifierType.Fireborn) != 0)
            {
                resistances["fire"] = 0.25f; // Strong resistance to fire
                weaknesses["ice"] = 1.75f;    // Weak to cold
            }

            if ((modifiers & EliteModifierType.Frostshield) != 0)
            {
                resistances["ice"] = 0.25f;
                weaknesses["fire"] = 1.75f;
            }

            return baseEnemy with
            {
                DisplayName = finalName,
                MaxHp = MathF.Round(baseEnemy.MaxHp * elite.HpMultiplier, 1),
                AttackDamage = MathF.Round(baseEnemy.AttackDamage * elite.DamageMultiplier, 1),
                MoveSpeed = MathF.Round(baseEnemy.MoveSpeed * elite.SpeedMultiplier, 1),
                XpReward = (int)MathF.Round(baseEnemy.XpReward * elite.XpMultiplier),
                Resistances = resistances,
                Weaknesses = weaknesses
            };
        }
    }
}
