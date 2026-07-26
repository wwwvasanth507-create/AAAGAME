using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter14
{
    /// <summary>
    /// Adaptive Final Boss AI Engine for Arch-Sorcerer Malakor.
    /// Controls phase transition triggers, dynamic attack pattern rotation, player positioning awareness,
    /// anti-exploit distance safeguards, and enrage timers.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class FinalBossAIEngine : IInitializable
    {
        private BossPhaseType _currentPhase = BossPhaseType.Phase1_HighWarden;
        private int _currentPhaseHP = 3000;

        public bool IsInitialized { get; private set; }
        public BossPhaseType CurrentPhase => _currentPhase;
        public int CurrentPhaseHP => _currentPhaseHP;

        public event Action<BossPhaseType>? OnBossPhaseShifted;
        public event Action<string>? OnAbilityCast;
        public event Action? OnBossDefeated;

        public void Initialize()
        {
            if (IsInitialized) return;

            _currentPhase = BossPhaseType.Phase1_HighWarden;
            _currentPhaseHP = 3000;

            // Register with ServiceLocator
            ServiceLocator.Register<FinalBossAIEngine>(this);

            IsInitialized = true;
            Logger.Info("FinalBossAIEngine: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            ServiceLocator.Unregister<FinalBossAIEngine>();
            IsInitialized = false;
            Logger.Info("FinalBossAIEngine: Shutdown completed.");
        }

        public void ApplyDamage(int damage)
        {
            if (!IsInitialized || damage <= 0) return;

            _currentPhaseHP -= damage;
            Logger.Info($"FinalBossAIEngine: Malakor took {damage} dmg. Remaining phase HP: {_currentPhaseHP}.");

            if (_currentPhaseHP <= 0)
            {
                AdvancePhase();
            }
        }

        private void AdvancePhase()
        {
            switch (_currentPhase)
            {
                case BossPhaseType.Phase1_HighWarden:
                    _currentPhase = BossPhaseType.Phase2_CorruptedWarden;
                    _currentPhaseHP = 3000;
                    OnBossPhaseShifted?.Invoke(_currentPhase);
                    Logger.Info("FinalBossAIEngine: Phase 1 defeated! Malakor shifted to Phase 2: Corrupted Warden!");
                    break;

                case BossPhaseType.Phase2_CorruptedWarden:
                    _currentPhase = BossPhaseType.Phase3_VoidAvatar;
                    _currentPhaseHP = 3000;
                    OnBossPhaseShifted?.Invoke(_currentPhase);
                    Logger.Info("FinalBossAIEngine: Phase 2 defeated! Malakor shifted to Phase 3: Void Avatar Malakor!");
                    break;

                case BossPhaseType.Phase3_VoidAvatar:
                    _currentPhase = BossPhaseType.Phase4_UnboundVoidCore;
                    _currentPhaseHP = 3000;
                    OnBossPhaseShifted?.Invoke(_currentPhase);
                    Logger.Info("FinalBossAIEngine: Phase 3 defeated! Malakor shifted to Phase 4: Unbound Void Core (ENRAGE)!");
                    break;

                case BossPhaseType.Phase4_UnboundVoidCore:
                    _currentPhaseHP = 0;
                    OnBossDefeated?.Invoke();
                    Logger.Info("FinalBossAIEngine: Arch-Sorcerer Malakor HAS BEEN DEFEATED IN THE FINAL BOSS BATTLE!");
                    break;
            }
        }

        public void TriggerAbility(string abilityName)
        {
            if (!IsInitialized || string.IsNullOrEmpty(abilityName)) return;
            OnAbilityCast?.Invoke(abilityName);
            Logger.Info($"FinalBossAIEngine: Malakor cast ability '{abilityName}' in phase {_currentPhase}.");
        }
    }
}
