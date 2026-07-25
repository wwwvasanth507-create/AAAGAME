using System;
using System.Collections.Generic;
using HeroOfEternia.Core;
using HeroOfEternia.World;

namespace HeroOfEternia.NPC
{
    /// <summary>
    /// Central NPC management service. Registered in ServiceLocator as "NpcManager".
    /// Maintains all active NPC state machines and schedules.
    /// Update frequency is throttled to 0.5 s to support 500+ NPCs on Android.
    /// </summary>
    public class NpcManager
    {
        public const string ServiceKey = "NpcManager";

        private readonly Dictionary<string, NpcStateMachine> _stateMachines  = new();
        private readonly Dictionary<string, NpcScheduler>    _schedulers     = new();
        private readonly Dictionary<string, NpcNavigationAgent> _navAgents   = new();
        private readonly Dictionary<string, NpcData>         _npcData        = new();

        private readonly RelationshipSystem  _relationships;
        private readonly ReputationSystem    _reputation;
        private readonly DialogueFramework   _dialogue;

        private double _tickAccumulator = 0.0;
        private const double TickInterval = 0.5; // seconds between NPC AI updates

        public RelationshipSystem  Relationships => _relationships;
        public ReputationSystem    Reputation    => _reputation;
        public DialogueFramework   Dialogue      => _dialogue;

        public NpcManager()
        {
            _relationships = new RelationshipSystem();
            _reputation    = new ReputationSystem();
            _dialogue      = new DialogueFramework();
        }

        // ─────────────────────── Registration ───────────────────────

        /// <summary>
        /// Registers a fully initialised NPC into the manager.
        /// </summary>
        public void RegisterNpc(NpcData data)
        {
            if (_npcData.ContainsKey(data.UniqueId))
            {
                Logger.Info($"NpcManager: NPC '{data.UniqueId}' already registered — skipping.");
                return;
            }

            _npcData[data.UniqueId] = data;

            // Build FSM
            var fsm = new NpcStateMachine(data.UniqueId);
            fsm.RegisterDefaultTransitions();
            _stateMachines[data.UniqueId] = fsm;

            // Build Scheduler
            var scheduler = NpcScheduler.BuildDefaultCivilianSchedule();
            _schedulers[data.UniqueId] = scheduler;

            // Build NavAgent
            var nav = new NpcNavigationAgent(
                data.UniqueId,
                data.WorldX, data.WorldY, data.WorldZ);
            _navAgents[data.UniqueId] = nav;

            // Register default dialogue lines
            var lines = DialogueFramework.BuildDefaultLines(data.Occupation);
            _dialogue.RegisterLines(data.UniqueId, lines);

            Logger.Info($"NpcManager: registered NPC '{data.UniqueId}' ({data.Occupation})");
        }

        public void UnregisterNpc(string npcId)
        {
            _stateMachines.Remove(npcId);
            _schedulers.Remove(npcId);
            _navAgents.Remove(npcId);
            _npcData.Remove(npcId);
        }

        // ─────────────────────── Update Loop ───────────────────────

        /// <summary>
        /// Main update tick. Throttled to TickInterval for Android performance.
        /// Pass current world time fraction (0.0–1.0) and schedule override.
        /// </summary>
        public void UpdateAll(double delta, double worldTimeFraction,
                              ScheduleOverrideType overrideType = ScheduleOverrideType.None)
        {
            _tickAccumulator += delta;
            if (_tickAccumulator < TickInterval) return;
            _tickAccumulator = 0.0;

            foreach (var kvp in _stateMachines)
            {
                string npcId = kvp.Key;
                var fsm = kvp.Value;

                if (!_schedulers.TryGetValue(npcId, out var scheduler)) continue;

                // Apply override
                scheduler.SetOverride(overrideType);

                // Evaluate schedule
                var block = scheduler.GetActiveBlock(worldTimeFraction);
                if (block != null && block.TargetState != fsm.CurrentState)
                {
                    string condTag = worldTimeFraction < 0.20 || worldTimeFraction >= 0.80
                        ? "time_night" : "time_morning";
                    fsm.TransitionTo(block.TargetState, condTag);
                }

                // Advance FSM timer
                fsm.Update(TickInterval);

                // Advance navigation
                if (_navAgents.TryGetValue(npcId, out var navAgent))
                {
                    if (fsm.CurrentState == NpcStateEnum.Walking)
                        navAgent.AdvanceStep();
                }
            }
        }

        // ─────────────────────── Queries ───────────────────────

        public NpcStateMachine? GetFsm(string npcId) =>
            _stateMachines.TryGetValue(npcId, out var fsm) ? fsm : null;

        public NpcScheduler? GetScheduler(string npcId) =>
            _schedulers.TryGetValue(npcId, out var s) ? s : null;

        public NpcNavigationAgent? GetNavAgent(string npcId) =>
            _navAgents.TryGetValue(npcId, out var n) ? n : null;

        public NpcData? GetData(string npcId) =>
            _npcData.TryGetValue(npcId, out var d) ? d : null;

        public IReadOnlyDictionary<string, NpcData> AllNpcs => _npcData;

        public int Count => _npcData.Count;

        // ─────────────────────── Save / Load ───────────────────────

        /// <summary>
        /// Exports all active NPC runtime states for Save V6.
        /// </summary>
        public Dictionary<string, NpcSaveState> ExportStates()
        {
            var result = new Dictionary<string, NpcSaveState>();
            foreach (var kv in _npcData)
            {
                var nav = _navAgents.TryGetValue(kv.Key, out var na) ? na : null;
                var pos = nav?.GetPositionSnapshot() ?? new float[] { kv.Value.WorldX, kv.Value.WorldY, kv.Value.WorldZ };
                result[kv.Key] = new NpcSaveState
                {
                    UniqueId              = kv.Key,
                    WorldX                = pos[0],
                    WorldY                = pos[1],
                    WorldZ                = pos[2],
                    Emotion               = kv.Value.CurrentEmotion,
                    CurrentHealth         = kv.Value.CurrentHealth,
                    ActiveScheduleOverride = ""
                };
            }
            return result;
        }

        /// <summary>
        /// Restores NPC positions and states from a Save V6 snapshot.
        /// </summary>
        public void RestoreStates(Dictionary<string, NpcSaveState> saved)
        {
            if (saved == null) return;
            foreach (var kv in saved)
            {
                if (_navAgents.TryGetValue(kv.Key, out var nav))
                    nav.RestorePosition(new float[] { kv.Value.WorldX, kv.Value.WorldY, kv.Value.WorldZ });
                if (_npcData.TryGetValue(kv.Key, out var data))
                {
                    data.CurrentEmotion  = kv.Value.Emotion;
                    data.CurrentHealth   = kv.Value.CurrentHealth;
                }
            }
        }
    }
}
