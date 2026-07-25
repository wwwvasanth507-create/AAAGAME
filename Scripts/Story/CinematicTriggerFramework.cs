using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Story
{
    public enum TriggerConditionType
    {
        EnterArea,
        ExitArea,
        QuestCompletion,
        DialogueCompletion,
        ObjectInteraction,
        BossDefeat,
        Timer,
        ItemCollection,
        PlayerLevel,
        WorldState,
        Manual
    }

    public class CinematicTriggerDefinition
    {
        public string TriggerId { get; set; } = string.Empty;
        public TriggerConditionType ConditionType { get; set; } = TriggerConditionType.EnterArea;
        public Vector3 TargetAreaPosition { get; set; }
        public float TriggerRadius { get; set; } = 5.0f;
        public string SequenceIdToPlay { get; set; } = string.Empty;
        public bool TriggerOnceOnly { get; set; } = true;
        public bool HasTriggered { get; set; } = false;
    }

    /// <summary>
    /// Reusable cinematic trigger framework handling spatial proximity, quest completion,
    /// dialogue triggers, and boss defeat cutscene hooks.
    /// </summary>
    public class CinematicTriggerFramework
    {
        private readonly Dictionary<string, CinematicTriggerDefinition> _triggers = new(StringComparer.OrdinalIgnoreCase);

        public event Action<CinematicTriggerDefinition>? OnCinematicTriggered;

        public void RegisterTrigger(CinematicTriggerDefinition trigger)
        {
            if (trigger != null && !string.IsNullOrEmpty(trigger.TriggerId))
            {
                _triggers[trigger.TriggerId] = trigger;
            }
        }

        public bool EvaluateTrigger(string triggerId, Vector3 playerPosition)
        {
            if (_triggers.TryGetValue(triggerId, out var tr))
            {
                if (tr.TriggerOnceOnly && tr.HasTriggered) return false;

                if (tr.ConditionType == TriggerConditionType.EnterArea && playerPosition.DistanceTo(tr.TargetAreaPosition) <= tr.TriggerRadius)
                {
                    tr.HasTriggered = true;
                    OnCinematicTriggered?.Invoke(tr);
                    return true;
                }
            }
            return false;
        }

        public bool FireManualTrigger(string triggerId)
        {
            if (_triggers.TryGetValue(triggerId, out var tr) && (!tr.TriggerOnceOnly || !tr.HasTriggered))
            {
                tr.HasTriggered = true;
                OnCinematicTriggered?.Invoke(tr);
                return true;
            }
            return false;
        }
    }
}
