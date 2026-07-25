using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.NPC
{
    public enum NpcStateEnum
    {
        Idle,
        Walking,
        Working,
        Eating,
        Sleeping,
        Talking,
        Inspecting,
        Patrolling,
        Waiting,
        Celebrating,
        Fleeing,    // Framework placeholder — no combat AI
        Searching   // Framework placeholder — no combat AI
    }

    /// <summary>
    /// Configurable transition rule between two NPC states.
    /// </summary>
    public class NpcStateTransition
    {
        public NpcStateEnum From { get; set; }
        public NpcStateEnum To { get; set; }
        public string ConditionTag { get; set; } = ""; // e.g. "time_morning", "threat_detected"
    }

    /// <summary>
    /// Modular FSM driving each NPC's behaviour. Transitions are configurable
    /// via data assets — no hardcoded gameplay logic is embedded.
    /// </summary>
    public class NpcStateMachine
    {
        public string NpcId { get; private set; }
        public NpcStateEnum CurrentState { get; private set; } = NpcStateEnum.Idle;

        private readonly List<NpcStateTransition> _allowedTransitions = new();
        private double _timeInState = 0.0;

        public NpcStateMachine(string npcId)
        {
            NpcId = npcId;
        }

        /// <summary>
        /// Registers a valid transition between two states.
        /// </summary>
        public void RegisterTransition(NpcStateEnum from, NpcStateEnum to, string conditionTag = "")
        {
            _allowedTransitions.Add(new NpcStateTransition { From = from, To = to, ConditionTag = conditionTag });
        }

        /// <summary>
        /// Attempts to transition to a new state. Returns false if transition is not registered.
        /// </summary>
        public bool TransitionTo(NpcStateEnum newState, string conditionTag = "")
        {
            foreach (var t in _allowedTransitions)
            {
                if (t.From == CurrentState && t.To == newState &&
                    (string.IsNullOrEmpty(t.ConditionTag) || t.ConditionTag == conditionTag))
                {
                    Logger.Info($"NPC[{NpcId}] {CurrentState} → {newState} (cond='{conditionTag}')");
                    CurrentState = newState;
                    _timeInState = 0.0;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Advances internal time tracker. Called once per NPC update tick.
        /// </summary>
        public void Update(double delta)
        {
            _timeInState += delta;
        }

        public double TimeInCurrentState => _timeInState;

        /// <summary>
        /// Registers all default NPC transitions (covers most civilian behaviour).
        /// </summary>
        public void RegisterDefaultTransitions()
        {
            // Morning routine
            RegisterTransition(NpcStateEnum.Sleeping, NpcStateEnum.Idle, "time_morning");
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Walking);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Working);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Eating);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Patrolling);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Waiting);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Talking);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Inspecting);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Celebrating);
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Fleeing, "threat_detected");
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Searching, "item_lost");
            RegisterTransition(NpcStateEnum.Walking, NpcStateEnum.Idle);
            RegisterTransition(NpcStateEnum.Walking, NpcStateEnum.Working);
            RegisterTransition(NpcStateEnum.Walking, NpcStateEnum.Waiting);
            RegisterTransition(NpcStateEnum.Walking, NpcStateEnum.Talking);
            RegisterTransition(NpcStateEnum.Working, NpcStateEnum.Idle);
            RegisterTransition(NpcStateEnum.Working, NpcStateEnum.Eating);
            RegisterTransition(NpcStateEnum.Eating, NpcStateEnum.Idle);
            RegisterTransition(NpcStateEnum.Eating, NpcStateEnum.Working);
            RegisterTransition(NpcStateEnum.Talking, NpcStateEnum.Idle);
            RegisterTransition(NpcStateEnum.Patrolling, NpcStateEnum.Idle);
            RegisterTransition(NpcStateEnum.Patrolling, NpcStateEnum.Waiting);
            RegisterTransition(NpcStateEnum.Waiting, NpcStateEnum.Idle);
            RegisterTransition(NpcStateEnum.Celebrating, NpcStateEnum.Idle);
            // Night routine
            RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Sleeping, "time_night");
            RegisterTransition(NpcStateEnum.Sleeping, NpcStateEnum.Idle);
        }
    }
}
