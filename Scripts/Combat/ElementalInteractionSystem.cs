using Godot;
using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    public enum ElementType
    {
        Physical = 0,
        Fire = 1,
        Ice = 2,
        Lightning = 3,
        Poison = 4,
        Arcane = 5,
        Oil = 6,
        Water = 7
    }

    public enum ElementalReactionType
    {
        None = 0,
        Flameburst = 1,    // Fire + Oil => Big AoE explosion + Burn
        Freeze = 2,        // Ice + Water => Hard Stun + Shatter vulnerability
        Electrocharge = 3, // Lightning + Water => Chain electric shock
        Combustion = 4,    // Fire + Poison => Corrosive blast
        ArcaneOverload = 5 // Arcane + Any Element => Burst magic damage
    }

    public readonly struct ReactionEvent
    {
        public readonly ulong TargetId;
        public readonly ElementalReactionType Reaction;
        public readonly float ExtraDamage;
        public readonly Vector3 Position;

        public ReactionEvent(ulong targetId, ElementalReactionType reaction, float extraDamage, Vector3 position)
        {
            TargetId = targetId;
            Reaction = reaction;
            ExtraDamage = extraDamage;
            Position = position;
        }
    }

    /// <summary>
    /// Evaluates elemental status combinations on combat targets and triggers elemental reactions.
    /// Fully compliant with Phase 0 rules (EventBus decoupled, zero GC in combat ticks).
    /// </summary>
    public class ElementalInteractionSystem : IInitializable
    {
        private static ElementalInteractionSystem? _instance;
        public static ElementalInteractionSystem Instance => _instance ??= new ElementalInteractionSystem();

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            GD.Print("[ElementalInteractionSystem] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        /// <summary>
        /// Evaluates elemental reaction when a new element is applied to an active set of elements on a target.
        /// </summary>
        public ElementalReactionType EvaluateReaction(ElementType applied, List<ElementType> activeElements, out float bonusDamageMultiplier)
        {
            bonusDamageMultiplier = 1.0f;
            if (activeElements == null || activeElements.Count == 0) return ElementalReactionType.None;

            for (int i = 0; i < activeElements.Count; i++)
            {
                var existing = activeElements[i];

                if ((applied == ElementType.Fire && existing == ElementType.Oil) ||
                    (applied == ElementType.Oil && existing == ElementType.Fire))
                {
                    bonusDamageMultiplier = 2.2f;
                    return ElementalReactionType.Flameburst;
                }

                if ((applied == ElementType.Ice && existing == ElementType.Water) ||
                    (applied == ElementType.Water && existing == ElementType.Ice))
                {
                    bonusDamageMultiplier = 1.75f;
                    return ElementalReactionType.Freeze;
                }

                if ((applied == ElementType.Lightning && existing == ElementType.Water) ||
                    (applied == ElementType.Water && existing == ElementType.Lightning))
                {
                    bonusDamageMultiplier = 1.6f;
                    return ElementalReactionType.Electrocharge;
                }

                if ((applied == ElementType.Fire && existing == ElementType.Poison) ||
                    (applied == ElementType.Poison && existing == ElementType.Fire))
                {
                    bonusDamageMultiplier = 1.9f;
                    return ElementalReactionType.Combustion;
                }

                if (applied == ElementType.Arcane && existing != ElementType.Physical)
                {
                    bonusDamageMultiplier = 2.0f;
                    return ElementalReactionType.ArcaneOverload;
                }
            }

            return ElementalReactionType.None;
        }

        /// <summary>
        /// Applies reaction result and broadcasts event via EventBus.
        /// </summary>
        public void TriggerReaction(ulong targetId, ElementalReactionType reaction, float baseDamage, Vector3 position)
        {
            if (reaction == ElementalReactionType.None) return;

            float extraDamage = reaction switch
            {
                ElementalReactionType.Flameburst => baseDamage * 1.2f,
                ElementalReactionType.Freeze => baseDamage * 0.75f,
                ElementalReactionType.Electrocharge => baseDamage * 0.6f,
                ElementalReactionType.Combustion => baseDamage * 0.9f,
                ElementalReactionType.ArcaneOverload => baseDamage * 1.0f,
                _ => 0f
            };

            var rxEvent = new ReactionEvent(targetId, reaction, extraDamage, position);
            EventBus.Publish(rxEvent);
        }
    }
}
