using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    public enum EncounterState
    {
        Inactive,
        Warmup,
        Active,
        Resetting,
        Victory,
        Defeat
    }

    public class EncounterManager
    {
        private EncounterState _state = EncounterState.Inactive;
        private BossDefinition? _activeBoss;
        private ArenaInstance? _activeArena;
        private BossPhaseSystem? _phaseSystem;
        private readonly RewardClaimTracker _rewardTracker = new();
        private float _activeBossHp = 0f;
        private float _activeBossMaxHp = 0f;
        private float _encounterTime = 0f;

        private readonly HashSet<string> _completedEncounters = new();
        private readonly HashSet<string> _defeatedBossIds = new();
        private readonly HashSet<string> _encounteredElites = new();

        public EncounterState State => _state;
        public BossDefinition? ActiveBoss => _activeBoss;
        public ArenaInstance? ActiveArena => _activeArena;
        public BossPhaseSystem? PhaseSystem => _phaseSystem;
        public RewardClaimTracker RewardTracker => _rewardTracker;
        public float EncounterTime => _encounterTime;

        public IReadOnlyCollection<string> CompletedEncounters => _completedEncounters;
        public IReadOnlyCollection<string> DefeatedBossIds => _defeatedBossIds;
        public IReadOnlyCollection<string> EncounteredElites => _encounteredElites;

        public event Action<EncounterState>? OnStateChanged;

        public void StartEncounter(BossDefinition boss, ArenaDefinition arena)
        {
            if (_state != EncounterState.Inactive)
            {
                Logger.Warning("EncounterManager: Cannot start encounter while another is active.");
                return;
            }

            _activeBoss = boss;
            _activeBossMaxHp = boss.Data.MaxHp;
            _activeBossHp = _activeBossMaxHp;
            _activeArena = new ArenaInstance(arena);
            _activeArena.LockGates();
            _encounterTime = 0f;

            _phaseSystem = new BossPhaseSystem(boss);
            _state = EncounterState.Active;

            Logger.Info($"EncounterManager: Started boss encounter '{boss.Data.DisplayName}' in arena '{arena.DisplayName}'.");
            EventBus.Publish(new EncounterStartedEvent(boss.Data.BossId, arena.ArenaId));
            OnStateChanged?.Invoke(_state);
        }

        public void Update(float delta, Vector3 playerPosition, bool playerAlive)
        {
            if (_state != EncounterState.Active) return;

            _encounterTime += delta;

            // Check reset condition: player died
            if (!playerAlive)
            {
                TriggerDefeat();
                return;
            }

            // Check reset condition: player out of arena boundaries
            if (_activeArena != null && !_activeArena.IsWithinBoundaries(playerPosition))
            {
                Logger.Info("EncounterManager: Player exited arena boundaries. Resetting encounter...");
                TriggerReset();
                return;
            }

            // Update Boss Phase transitions
            _phaseSystem?.Update(_activeBossHp, _activeBossMaxHp, delta);

            // Handle hazard checks
            var hazard = _activeArena?.GetActiveHazardCollision(playerPosition);
            if (hazard != null)
            {
                // Deliver environment hazard ticks (e.g. burn player)
                EventBus.Publish(new ArenaHazardDamagedEvent(hazard.HazardId, hazard.DamagePerSecond * delta));
            }
        }

        public void UpdateBossHp(float currentHp)
        {
            if (_state != EncounterState.Active) return;

            _activeBossHp = MathF.Max(0f, currentHp);
            if (_activeBossHp <= 0f)
            {
                TriggerVictory();
            }
        }

        public void TriggerReset()
        {
            _state = EncounterState.Resetting;
            Logger.Info($"EncounterManager: Resetting boss encounter.");

            _activeArena?.Reset();
            _phaseSystem?.Reset();
            _activeBossHp = _activeBossMaxHp;
            _encounterTime = 0f;
            _state = EncounterState.Inactive;

            EventBus.Publish(new EncounterResetEvent());
            OnStateChanged?.Invoke(_state);
        }

        private void TriggerVictory()
        {
            _state = EncounterState.Victory;
            string bossId = _activeBoss?.Data.BossId ?? string.Empty;
            string arenaId = _activeArena?.Definition.ArenaId ?? string.Empty;

            Logger.Info($"EncounterManager: Boss defeated! Victory achieved.");
            _defeatedBossIds.Add(bossId);
            _completedEncounters.Add(arenaId);

            _activeArena?.UnlockGates();

            EventBus.Publish(new EncounterVictoryEvent(bossId, arenaId));
            OnStateChanged?.Invoke(_state);

            // Transition to Inactive state ready for next battle
            _state = EncounterState.Inactive;
        }

        private void TriggerDefeat()
        {
            _state = EncounterState.Defeat;
            string bossId = _activeBoss?.Data.BossId ?? string.Empty;

            Logger.Info($"EncounterManager: Player died. Defeat registered.");
            _activeArena?.UnlockGates();

            EventBus.Publish(new EncounterDefeatEvent(bossId));
            OnStateChanged?.Invoke(_state);

            // Auto reset
            TriggerReset();
        }

        public void RegisterEliteEncountered(string eliteName)
        {
            _encounteredElites.Add(eliteName);
        }

        // ----------------------------------------------------------------
        // Save state operations
        // ----------------------------------------------------------------
        public void LoadSaveState(
            IEnumerable<string> completed,
            IEnumerable<string> defeated,
            IEnumerable<string> elites,
            IEnumerable<string> claimedRewards)
        {
            _completedEncounters.Clear();
            _completedEncounters.UnionWith(completed);

            _defeatedBossIds.Clear();
            _defeatedBossIds.UnionWith(defeated);

            _encounteredElites.Clear();
            _encounteredElites.UnionWith(elites);

            _rewardTracker.LoadClaimedList(claimedRewards);
        }
    }

    // Events
    public record EncounterStartedEvent(string BossId, string ArenaId);
    public record EncounterResetEvent();
    public record EncounterVictoryEvent(string BossId, string ArenaId);
    public record EncounterDefeatEvent(string BossId);
    public record ArenaHazardDamagedEvent(string HazardId, float Damage);
}
