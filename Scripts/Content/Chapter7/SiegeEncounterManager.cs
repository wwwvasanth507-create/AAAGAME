using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter7
{
    public enum SiegeStage
    {
        NotStarted,
        Preparation,
        WallDefense,
        BreachCounterAssault,
        VictorySequence
    }

    public class SiegeWaveDefinition
    {
        public int WaveNumber { get; set; } = 1;
        public string Name { get; set; } = "";
        public int TotalEnemies { get; set; } = 10;
        public List<string> EnemyTypes { get; set; } = new();
        public bool ContainsElite { get; set; } = false;
    }

    /// <summary>
    /// Multi-stage Siege Encounter Manager for Chapter 7 Act II Finale.
    /// Controls battle phases, defender NPC coordination, enemy waves, barricade destruction, and victory transitions.
    /// </summary>
    public class SiegeEncounterManager
    {
        private readonly List<SiegeWaveDefinition> _waves = new();

        public SiegeStage CurrentStage { get; private set; } = SiegeStage.NotStarted;
        public int CurrentWaveIndex { get; private set; } = 0;
        public bool IsSiegeActive => CurrentStage != SiegeStage.NotStarted && CurrentStage != SiegeStage.VictorySequence;

        public event Action<SiegeStage>? OnSiegeStageChanged;
        public event Action<SiegeWaveDefinition>? OnSiegeWaveStarted;

        public SiegeEncounterManager()
        {
            InitializeWaves();
        }

        private void InitializeWaves()
        {
            _waves.Add(new SiegeWaveDefinition
            {
                WaveNumber = 1,
                Name = "Vanguard Shadow Assault",
                TotalEnemies = 12,
                EnemyTypes = new List<string> { "enemy_veteran_bandit", "enemy_shadow_lurker" }
            });

            _waves.Add(new SiegeWaveDefinition
            {
                WaveNumber = 2,
                Name = "Siege Engine Breach Force",
                TotalEnemies = 16,
                EnemyTypes = new List<string> { "enemy_heavy_defender", "enemy_spellcaster" },
                ContainsElite = true
            });

            _waves.Add(new SiegeWaveDefinition
            {
                WaveNumber = 3,
                Name = "Shadow Lord Harbinger Vanguard",
                TotalEnemies = 20,
                EnemyTypes = new List<string> { "enemy_boss_drael", "enemy_regional_champion" },
                ContainsElite = true
            });
        }

        public bool StartSiege()
        {
            CurrentStage = SiegeStage.Preparation;
            CurrentWaveIndex = 0;

            OnSiegeStageChanged?.Invoke(CurrentStage);
            Core.Logger.Info("SiegeEncounterManager: Siege battle initialized - Stage: Preparation.");
            return true;
        }

        public bool AdvanceStage()
        {
            if (CurrentStage == SiegeStage.VictorySequence) return false;

            CurrentStage = CurrentStage switch
            {
                SiegeStage.Preparation => SiegeStage.WallDefense,
                SiegeStage.WallDefense => SiegeStage.BreachCounterAssault,
                SiegeStage.BreachCounterAssault => SiegeStage.VictorySequence,
                _ => SiegeStage.VictorySequence
            };

            OnSiegeStageChanged?.Invoke(CurrentStage);

            if (CurrentStage == SiegeStage.WallDefense && _waves.Count > 0)
            {
                OnSiegeWaveStarted?.Invoke(_waves[0]);
            }

            Core.Logger.Info($"SiegeEncounterManager: Advanced siege stage to '{CurrentStage}'.");
            return true;
        }

        public bool AdvanceWave()
        {
            if (CurrentWaveIndex + 1 < _waves.Count)
            {
                CurrentWaveIndex++;
                var wave = _waves[CurrentWaveIndex];
                OnSiegeWaveStarted?.Invoke(wave);
                Core.Logger.Info($"SiegeEncounterManager: Advanced to siege wave {wave.WaveNumber} - '{wave.Name}'.");
                return true;
            }
            else
            {
                AdvanceStage();
                return false;
            }
        }

        public SiegeWaveDefinition? GetCurrentWave()
        {
            return CurrentWaveIndex < _waves.Count ? _waves[CurrentWaveIndex] : null;
        }

        public IReadOnlyList<SiegeWaveDefinition> GetAllWaves() => _waves.AsReadOnly();
    }
}
