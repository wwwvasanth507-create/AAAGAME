using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Social.Guard
{
    /// <summary>
    /// Guard AI behavior states.
    /// </summary>
    public enum GuardState
    {
        Idle,
        Patrol,
        Investigate,
        Question,
        Warn,
        Arrest,
        Combat,
        CallReinforcements,
        ProtectCitizen,
        Escort,
        Search,
        ReturnToPatrol
    }

    /// <summary>
    /// Alert level for a guard or guard force.
    /// </summary>
    public enum GuardAlertLevel
    {
        Green,    // Normal patrol
        Yellow,   // Suspicious activity
        Orange,   // Active investigation
        Red       // Active threat
    }

    /// <summary>
    /// Configuration for a guard's behavior parameters.
    /// </summary>
    public class GuardConfig
    {
        public string GuardId { get; set; } = "";
        public string FactionId { get; set; } = "military_eternian_guard";
        public string SettlementId { get; set; } = "";
        public float PatrolRadius { get; set; } = 30f;
        public float InvestigationRadius { get; set; } = 20f;
        public float DetectionRadius { get; set; } = 15f;
        public float HearingRadius { get; set; } = 25f;
        public float CombatRadius { get; set; } = 10f;
        public float SearchDuration { get; set; } = 30f; // seconds
        public float InvestigationDuration { get; set; } = 10f;
        public int CombatStrength { get; set; } = 50;
        public bool CanCallReinforcements { get; set; } = true;
        public float ReinforcementCallRadius { get; set; } = 50f;
        public List<string> PatrolRoute { get; set; } = new();
    }

    /// <summary>
    /// Runtime state for a single guard.
    /// </summary>
    public class GuardRuntimeState
    {
        public GuardState CurrentState { get; set; } = GuardState.Patrol;
        public GuardAlertLevel AlertLevel { get; set; } = GuardAlertLevel.Green;
        public string TargetId { get; set; } = "";
        public string InvestigationLocation { get; set; } = "";
        public float StateTimer { get; set; } = 0f;
        public int PatrolRouteIndex { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public float CurrentHealth { get; set; } = 100f;
        public float MaxHealth { get; set; } = 100f;
    }

    /// <summary>
    /// Event fired when a guard changes state.
    /// </summary>
    public class GuardStateChangedEvent
    {
        public string GuardId { get; set; } = "";
        public GuardState OldState { get; set; }
        public GuardState NewState { get; set; }
        public GuardAlertLevel AlertLevel { get; set; }
        public string TargetId { get; set; } = "";
    }

    /// <summary>
    /// Event fired when reinforcements are called.
    /// </summary>
    public class ReinforcementsCalledEvent
    {
        public string GuardId { get; set; } = "";
        public string LocationId { get; set; } = "";
        public string SettlementId { get; set; } = "";
        public int ResponderCount { get; set; }
    }

    /// <summary>
    /// Manages guard AI behavior including patrol, investigation, arrest, and combat.
    /// Designed to scale to large cities with many guards.
    /// </summary>
    public class GuardAISystem
    {
        public const string ServiceKey = "GuardAISystem";

        private readonly Dictionary<string, GuardConfig> _guardConfigs = new();
        private readonly Dictionary<string, GuardRuntimeState> _guardStates = new();
        private readonly Dictionary<string, List<string>> _guardsBySettlement = new();
        private readonly Dictionary<string, GuardAlertLevel> _settlementAlertLevels = new();
        
        private readonly object _lock = new();
        private double _tickAccumulator = 0.0;
        private const double TickInterval = 0.25; // 4 ticks per second

        /// <summary>Fired when a guard's state changes.</summary>
        public event Action<GuardStateChangedEvent>? OnGuardStateChanged;
        /// <summary>Fired when reinforcements are called.</summary>
        public event Action<ReinforcementsCalledEvent>? OnReinforcementsCalled;

        // ─────────────────────── Guard Registration ───────────────────────

        public void RegisterGuard(GuardConfig config)
        {
            if (config == null || string.IsNullOrEmpty(config.GuardId)) return;
            lock (_lock)
            {
                _guardConfigs[config.GuardId] = config;
                _guardStates[config.GuardId] = new GuardRuntimeState();

                if (!string.IsNullOrEmpty(config.SettlementId))
                {
                    if (!_guardsBySettlement.ContainsKey(config.SettlementId))
                        _guardsBySettlement[config.SettlementId] = new List<string>();
                    if (!_guardsBySettlement[config.SettlementId].Contains(config.GuardId))
                        _guardsBySettlement[config.SettlementId].Add(config.GuardId);
                }
            }
        }

        public void UnregisterGuard(string guardId)
        {
            lock (_lock)
            {
                if (_guardConfigs.TryGetValue(guardId, out var config))
                {
                    if (!string.IsNullOrEmpty(config.SettlementId) &&
                        _guardsBySettlement.TryGetValue(config.SettlementId, out var guards))
                    {
                        guards.Remove(guardId);
                    }
                }
                _guardConfigs.Remove(guardId);
                _guardStates.Remove(guardId);
            }
        }

        // ─────────────────────── Main Update ───────────────────────

        /// <summary>
        /// Main update loop. Throttled to TickInterval for performance.
        /// </summary>
        public void UpdateAll(double delta, double worldTimeFraction)
        {
            _tickAccumulator += delta;
            if (_tickAccumulator < TickInterval) return;
            _tickAccumulator = 0.0;

            lock (_lock)
            {
                foreach (var guardId in _guardConfigs.Keys.ToList())
                {
                    if (!_guardStates.TryGetValue(guardId, out var state) || !state.IsActive)
                        continue;

                    UpdateGuard(guardId, state, (float)TickInterval, worldTimeFraction);
                }
            }
        }

        private void UpdateGuard(string guardId, GuardRuntimeState state, float delta, double worldTimeFraction)
        {
            if (!_guardConfigs.TryGetValue(guardId, out var config)) return;

            state.StateTimer += delta;

            switch (state.CurrentState)
            {
                case GuardState.Patrol:
                    UpdatePatrol(guardId, state, config, delta);
                    break;
                case GuardState.Investigate:
                    UpdateInvestigate(guardId, state, config, delta);
                    break;
                case GuardState.Search:
                    UpdateSearch(guardId, state, config, delta);
                    break;
                case GuardState.Combat:
                    UpdateCombat(guardId, state, config, delta);
                    break;
                case GuardState.CallReinforcements:
                    UpdateCallReinforcements(guardId, state, config, delta);
                    break;
                case GuardState.ReturnToPatrol:
                    UpdateReturnToPatrol(guardId, state, config, delta);
                    break;
                case GuardState.Warn:
                    UpdateWarn(guardId, state, config, delta);
                    break;
                case GuardState.Question:
                    UpdateQuestion(guardId, state, config, delta);
                    break;
            }
        }

        // ─────────────────────── State Behaviors ───────────────────────

        private void UpdatePatrol(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            // Simulate patrol route progression
            state.StateTimer += delta;
            if (state.StateTimer >= 5f) // Move to next patrol point every 5 seconds
            {
                state.StateTimer = 0f;
                if (config.PatrolRoute.Count > 0)
                {
                    state.PatrolRouteIndex = (state.PatrolRouteIndex + 1) % config.PatrolRoute.Count;
                }
            }
        }

        private void UpdateInvestigate(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            if (state.StateTimer >= config.InvestigationDuration)
            {
                // Investigation complete, transition to search or return
                if (state.TargetId != "")
                {
                    TransitionTo(guardId, state, GuardState.Search);
                }
                else
                {
                    TransitionTo(guardId, state, GuardState.ReturnToPatrol);
                }
            }
        }

        private void UpdateSearch(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            if (state.StateTimer >= config.SearchDuration)
            {
                TransitionTo(guardId, state, GuardState.ReturnToPatrol);
            }
        }

        private void UpdateCombat(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            // Combat handled by external combat system
            // This is a hook for future integration
            state.StateTimer += delta;
        }

        private void UpdateCallReinforcements(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            // Find nearby guards and alert them
            var responders = GetGuardsInSettlement(config.SettlementId)
                .Where(g => g != guardId)
                .Take(3)
                .ToList();

            OnReinforcementsCalled?.Invoke(new ReinforcementsCalledEvent
            {
                GuardId = guardId,
                LocationId = state.InvestigationLocation,
                SettlementId = config.SettlementId,
                ResponderCount = responders.Count
            });

            // Alert nearby guards
            foreach (var responderId in responders)
            {
                if (_guardStates.TryGetValue(responderId, out var responderState))
                {
                    if (responderState.CurrentState == GuardState.Patrol ||
                        responderState.CurrentState == GuardState.Idle)
                    {
                        responderState.TargetId = state.TargetId;
                        responderState.InvestigationLocation = state.InvestigationLocation;
                        TransitionTo(responderId, responderState, GuardState.Investigate);
                    }
                }
            }

            TransitionTo(guardId, state, GuardState.Combat);
        }

        private void UpdateReturnToPatrol(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            state.StateTimer += delta;
            if (state.StateTimer >= 3f)
            {
                state.TargetId = "";
                state.InvestigationLocation = "";
                TransitionTo(guardId, state, GuardState.Patrol);
            }
        }

        private void UpdateWarn(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            if (state.StateTimer >= 3f)
            {
                if (state.TargetId != "")
                    TransitionTo(guardId, state, GuardState.Arrest);
                else
                    TransitionTo(guardId, state, GuardState.ReturnToPatrol);
            }
        }

        private void UpdateQuestion(string guardId, GuardRuntimeState state, GuardConfig config, float delta)
        {
            if (state.StateTimer >= 5f)
            {
                TransitionTo(guardId, state, GuardState.ReturnToPatrol);
            }
        }

        // ─────────────────────── State Transitions ───────────────────────

        private void TransitionTo(string guardId, GuardRuntimeState state, GuardState newState)
        {
            var oldState = state.CurrentState;
            state.CurrentState = newState;
            state.StateTimer = 0f;

            OnGuardStateChanged?.Invoke(new GuardStateChangedEvent
            {
                GuardId = guardId,
                OldState = oldState,
                NewState = newState,
                AlertLevel = state.AlertLevel,
                TargetId = state.TargetId
            });
        }

        // ─────────────────────── External Triggers ───────────────────────

        /// <summary>
        /// Called when a crime is detected near a guard.
        /// </summary>
        public void ReportSuspiciousActivity(string guardId, string suspectId, string locationId, Crime.CrimeType crimeType)
        {
            lock (_lock)
            {
                if (!_guardStates.TryGetValue(guardId, out var state) || !state.IsActive)
                    return;

                state.TargetId = suspectId;
                state.InvestigationLocation = locationId;
                state.AlertLevel = GuardAlertLevel.Yellow;

                if (crimeType == Crime.CrimeType.Murder || crimeType == Crime.CrimeType.Assault)
                {
                    state.AlertLevel = GuardAlertLevel.Orange;
                    TransitionTo(guardId, state, GuardState.Combat);
                }
                else
                {
                    TransitionTo(guardId, state, GuardState.Investigate);
                }
            }
        }

        /// <summary>
        /// Direct a guard to arrest a suspect.
        /// </summary>
        public void IssueArrestOrder(string guardId, string suspectId)
        {
            lock (_lock)
            {
                if (!_guardStates.TryGetValue(guardId, out var state) || !state.IsActive)
                    return;

                state.TargetId = suspectId;
                state.AlertLevel = GuardAlertLevel.Red;
                TransitionTo(guardId, state, GuardState.Arrest);
            }
        }

        /// <summary>
        /// Set the alert level for an entire settlement.
        /// </summary>
        public void SetSettlementAlertLevel(string settlementId, GuardAlertLevel level)
        {
            lock (_lock)
            {
                _settlementAlertLevels[settlementId] = level;

                if (_guardsBySettlement.TryGetValue(settlementId, out var guardIds))
                {
                    foreach (var guardId in guardIds)
                    {
                        if (_guardStates.TryGetValue(guardId, out var state))
                        {
                            state.AlertLevel = level;
                            if (level == GuardAlertLevel.Red && state.CurrentState == GuardState.Patrol)
                            {
                                TransitionTo(guardId, state, GuardState.Combat);
                            }
                        }
                    }
                }
            }
        }

        // ─────────────────────── Queries ───────────────────────

        public GuardRuntimeState? GetGuardState(string guardId)
        {
            lock (_lock) { return _guardStates.TryGetValue(guardId, out var s) ? s : null; }
        }

        public GuardConfig? GetGuardConfig(string guardId)
        {
            lock (_lock) { return _guardConfigs.TryGetValue(guardId, out var c) ? c : null; }
        }

        public List<string> GetGuardsInSettlement(string settlementId)
        {
            lock (_lock)
            {
                return _guardsBySettlement.TryGetValue(settlementId, out var guards)
                    ? new List<string>(guards)
                    : new List<string>();
            }
        }

        public GuardAlertLevel GetSettlementAlertLevel(string settlementId)
        {
            lock (_lock)
            {
                return _settlementAlertLevels.TryGetValue(settlementId, out var level) ? level : GuardAlertLevel.Green;
            }
        }

        public int ActiveGuardCount
        {
            get { lock (_lock) { return _guardStates.Values.Count(s => s.IsActive); } }
        }

        // ─────────────────────── Save / Load ───────────────────────

        public GuardSaveData ExportSaveData()
        {
            lock (_lock)
            {
                return new GuardSaveData
                {
                    GuardStates = _guardStates.ToDictionary(
                        kv => kv.Key,
                        kv => new GuardRuntimeState
                        {
                            CurrentState = kv.Value.CurrentState,
                            AlertLevel = kv.Value.AlertLevel,
                            TargetId = kv.Value.TargetId,
                            InvestigationLocation = kv.Value.InvestigationLocation,
                            StateTimer = kv.Value.StateTimer,
                            PatrolRouteIndex = kv.Value.PatrolRouteIndex,
                            IsActive = kv.Value.IsActive,
                            CurrentHealth = kv.Value.CurrentHealth,
                            MaxHealth = kv.Value.MaxHealth
                        }),
                    SettlementAlertLevels = new Dictionary<string, GuardAlertLevel>(_settlementAlertLevels)
                };
            }
        }

        public void RestoreSaveData(GuardSaveData? data)
        {
            if (data == null) return;
            lock (_lock)
            {
                _guardStates.Clear();
                _settlementAlertLevels.Clear();

                foreach (var kv in data.GuardStates)
                    _guardStates[kv.Key] = kv.Value;

                foreach (var kv in data.SettlementAlertLevels)
                    _settlementAlertLevels[kv.Key] = kv.Value;
            }
        }
    }

    /// <summary>
    /// Save data container for the guard AI system.
    /// </summary>
    public class GuardSaveData
    {
        public Dictionary<string, GuardRuntimeState> GuardStates { get; set; } = new();
        public Dictionary<string, GuardAlertLevel> SettlementAlertLevels { get; set; } = new();
    }
}