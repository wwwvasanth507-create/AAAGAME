using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// PlayerStateMachine manages transitions between IPlayerState instances.
    /// Automatically logs every state change. Future states can be registered
    /// via Register() without modifying this class (Open/Closed Principle).
    /// </summary>
    public class PlayerStateMachine
    {
        private readonly Dictionary<PlayerStateId, IPlayerState> _states = new();
        private IPlayerState? _currentState;

        public PlayerStateId CurrentStateId =>
            _currentState?.Id ?? PlayerStateId.Idle;

        public event Action<PlayerStateId, PlayerStateId>? OnStateChanged;

        // ---------------------------------------------------------------
        // REGISTRATION
        // ---------------------------------------------------------------

        /// <summary>Register a state implementation. Call once during player setup.</summary>
        public void Register(IPlayerState state)
        {
            _states[state.Id] = state;
        }

        // ---------------------------------------------------------------
        // LIFECYCLE
        // ---------------------------------------------------------------

        /// <summary>Start the machine in the given initial state.</summary>
        public void Start(PlayerRoot player, PlayerStateId initialState)
        {
            if (!_states.TryGetValue(initialState, out var state))
            {
                Logger.Error($"PlayerStateMachine: Cannot start — state {initialState} not registered.");
                return;
            }
            _currentState = state;
            _currentState.Enter(player);
            Logger.Info($"PlayerStateMachine: Started in state [{initialState}].");
        }

        /// <summary>Called every physics frame. Handles transitions automatically.</summary>
        public void Update(PlayerRoot player, double delta)
        {
            if (_currentState == null) return;

            PlayerStateId? requested = _currentState.Update(player, delta);
            if (requested.HasValue && requested.Value != CurrentStateId)
            {
                TransitionTo(player, requested.Value);
            }
        }

        /// <summary>Force a transition from external code (e.g., fall damage, death).</summary>
        public void ForceTransition(PlayerRoot player, PlayerStateId target)
        {
            TransitionTo(player, target);
        }

        // ---------------------------------------------------------------
        // PRIVATE
        // ---------------------------------------------------------------

        private void TransitionTo(PlayerRoot player, PlayerStateId target)
        {
            if (!_states.TryGetValue(target, out var nextState))
            {
                Logger.Warning($"PlayerStateMachine: Target state {target} not registered — ignoring.");
                return;
            }

            var from = CurrentStateId;
            _currentState?.Exit(player);
            _currentState = nextState;
            _currentState.Enter(player);

            Logger.Info($"PlayerStateMachine: {from} → {target}");
            OnStateChanged?.Invoke(from, target);
        }
    }
}
