using System;
using System.Collections.Generic;
using HeroOfEternia.Core;
using HeroOfEternia.Player;
using HeroOfEternia.Player.Stats;

namespace HeroOfEternia.Items
{
    /// <summary>
    /// Configuration record defining single consumable item effect parameters.
    /// </summary>
    public class ItemEffectData
    {
        public string EffectType { get; set; } = ""; // e.g. "Healing", "Buff", "Teleport"
        public float Magnitude { get; set; } = 0f;
        public float Duration { get; set; } = 0f; // > 0 means timed buff/debuff
        public Dictionary<string, string> CustomParams { get; set; } = new();
    }

    /// <summary>
    /// Reusable engine resolving item consumption behaviors and gameplay triggers.
    /// Uses stubs to support unlimited future quest triggers, teleports, and spells.
    /// </summary>
    public static class ItemEffectsFramework
    {
        /// <summary>
        /// Resolves and executes the effect behaviors on the player.
        /// </summary>
        public static bool TriggerEffect(ItemEffectData effect, PlayerRoot player)
        {
            if (effect == null || player == null) return false;

            Logger.Info($"ItemEffectsFramework: Resolving effect '{effect.EffectType}' (Mag={effect.Magnitude}, Dur={effect.Duration})");

            switch (effect.EffectType.ToLowerInvariant())
            {
                case "healing":
                    // Restore health instantly
                    float targetHealth = Math.Min(player.Data.CurrentHealth + effect.Magnitude, player.Data.MaxHealth);
                    player.Data.CurrentHealth = targetHealth;
                    Logger.Info($"ItemEffectsFramework: Restored {effect.Magnitude} HP. Current HP: {player.Data.CurrentHealth}");
                    break;

                case "manarestore":
                    // Restore mana instantly
                    float targetMana = Math.Min(player.Data.CurrentMana + effect.Magnitude, player.Data.MaxMana);
                    player.Data.CurrentMana = targetMana;
                    Logger.Info($"ItemEffectsFramework: Restored {effect.Magnitude} MP. Current MP: {player.Data.CurrentMana}");
                    break;

                case "buff":
                    // Apply temporary stat modifier to player attribute
                    if (effect.CustomParams.TryGetValue("Attribute", out string? attrName))
                    {
                        if (Enum.TryParse<AttributeType>(attrName, true, out var attrType))
                        {
                            var mod = new StatModifier(
                                $"ItemBuff_{attrName}",
                                effect.Magnitude,
                                ModifierType.Flat,
                                ModifierSource.Potion,
                                effect.Duration > 0f ? (double)effect.Duration : -1.0
                            );
                            player.Data.Attributes.AddModifier(attrType, mod);
                            Logger.Info($"ItemEffectsFramework: Applied temporary flat +{effect.Magnitude} buff to attribute '{attrType}'.");
                        }
                    }
                    break;

                case "teleport":
                    // Hook for future world maps transitions
                    if (effect.CustomParams.TryGetValue("TargetZone", out string? zone))
                    {
                        Logger.Info($"ItemEffectsFramework: [STUB] Player queued for zoning transfer to '{zone}'.");
                    }
                    break;

                case "questtrigger":
                    // Hook for storyline progressions
                    if (effect.CustomParams.TryGetValue("QuestId", out string? quest))
                    {
                        Logger.Info($"ItemEffectsFramework: [STUB] Triggered quest state evaluation for '{quest}'.");
                    }
                    break;

                default:
                    Logger.Warning($"ItemEffectsFramework: Unknown effect type '{effect.EffectType}' bypassed.");
                    return false;
            }

            return true;
        }
    }
}
