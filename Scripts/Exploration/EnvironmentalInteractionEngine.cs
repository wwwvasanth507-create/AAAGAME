using System;
using Godot;

namespace HeroOfEternia.Exploration
{
    public enum EnvironmentalInteractionType
    {
        Push,
        Pull,
        ClimbHook,
        SwimHook,
        Burn,
        Freeze,
        Electrify,
        Open,
        Close,
        RepairHook,
        Activate,
        Deactivate
    }

    public class EnvironmentalInteractionData
    {
        public EnvironmentalInteractionType InteractionType { get; set; }
        public Vector3 TargetPosition { get; set; }
        public string TargetId { get; set; } = string.Empty;
        public float ForceOrIntensity { get; set; } = 1.0f;
    }

    /// <summary>
    /// Reusable engine processing elemental interactions (Burn, Freeze, Electrify)
    /// and mechanical interactions (Push, Pull, Open, Repair, Activate).
    /// </summary>
    public class EnvironmentalInteractionEngine
    {
        public event Action<EnvironmentalInteractionData>? OnInteractionTriggered;

        public bool TriggerInteraction(EnvironmentalInteractionType type, string targetId, Vector3 position, float intensity = 1.0f)
        {
            var data = new EnvironmentalInteractionData
            {
                InteractionType = type,
                TargetId = targetId,
                TargetPosition = position,
                ForceOrIntensity = intensity
            };

            OnInteractionTriggered?.Invoke(data);
            return true;
        }
    }
}
