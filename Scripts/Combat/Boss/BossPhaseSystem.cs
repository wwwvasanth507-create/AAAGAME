using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    public class BossPhaseSystem
    {
        private readonly BossDefinition _definition;
        private int _currentPhaseIndex = 1;
        private float _elapsedTimeInPhase = 0f;
        private bool _isTransitioning = false;

        public event Action<int, BossPhaseData>? OnPhaseTransitionStarted;
        public event Action<int, BossPhaseData>? OnPhaseTransitionCompleted;

        public int CurrentPhaseIndex => _currentPhaseIndex;
        public float ElapsedTimeInPhase => _elapsedTimeInPhase;
        public bool IsTransitioning => _isTransitioning;

        public BossPhaseSystem(BossDefinition definition)
        {
            _definition = definition;
        }

        public void Reset()
        {
            _currentPhaseIndex = 1;
            _elapsedTimeInPhase = 0f;
            _isTransitioning = false;
        }

        public BossPhaseData? GetCurrentPhaseData()
        {
            return _definition.Data.Phases.Find(p => p.PhaseIndex == _currentPhaseIndex);
        }

        public void Update(float currentHp, float maxHp, float delta, bool environmentEventTriggered = false)
        {
            if (_isTransitioning) return;

            _elapsedTimeInPhase += delta;
            float hpPct = maxHp > 0f ? currentHp / maxHp : 0f;

            // Check if any higher phase index has met its transition threshold
            BossPhaseData? nextPhase = null;
            foreach (var phase in _definition.Data.Phases)
            {
                if (phase.PhaseIndex > _currentPhaseIndex)
                {
                    // Check triggers: HP threshold, elapsed time, or environmental flag
                    bool hpTrigger = hpPct <= phase.HpThresholdPct;
                    bool timeTrigger = _elapsedTimeInPhase >= 120f && phase.PhaseIndex == _currentPhaseIndex + 1; // backup time enrage
                    bool eventTrigger = environmentEventTriggered && phase.BehaviorModifier == "environmental_enraged";

                    if (hpTrigger || timeTrigger || eventTrigger)
                    {
                        if (nextPhase == null || phase.PhaseIndex > nextPhase.PhaseIndex)
                        {
                            nextPhase = phase;
                        }
                    }
                }
            }

            if (nextPhase != null)
            {
                TriggerTransition(nextPhase);
            }
        }

        private void TriggerTransition(BossPhaseData nextPhase)
        {
            _isTransitioning = true;
            Logger.Info($"BossPhaseSystem[{_definition.Data.BossId}]: Triggered transition to Phase {nextPhase.PhaseIndex}.");
            OnPhaseTransitionStarted?.Invoke(nextPhase.PhaseIndex, nextPhase);

            // Simulate transition buffer/delay (e.g. enrage scream)
            _currentPhaseIndex = nextPhase.PhaseIndex;
            _elapsedTimeInPhase = 0f;
            _isTransitioning = false;

            if (!string.IsNullOrEmpty(nextPhase.VfxTriggerKey))
            {
                EventBus.Publish(new BossVfxTriggerEvent(_definition.Data.BossId, nextPhase.VfxTriggerKey));
            }
            if (!string.IsNullOrEmpty(nextPhase.SfxTriggerKey))
            {
                EventBus.Publish(new BossSfxTriggerEvent(_definition.Data.BossId, nextPhase.SfxTriggerKey));
            }

            OnPhaseTransitionCompleted?.Invoke(_currentPhaseIndex, nextPhase);
            Logger.Info($"BossPhaseSystem[{_definition.Data.BossId}]: Transition to Phase {_currentPhaseIndex} completed.");
        }
    }

    public record BossVfxTriggerEvent(string BossId, string VfxKey);
    public record BossSfxTriggerEvent(string BossId, string SfxKey);
}
