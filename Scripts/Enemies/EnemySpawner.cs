using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Enemies
{
    // ----------------------------------------------------------------
    // Events
    // ----------------------------------------------------------------
    public record WaveStartedEvent(int WaveNumber, int TotalEnemies);
    public record WaveCompleteEvent(int WaveNumber, int WavesRemaining);
    public record AllWavesCompleteEvent(int TotalWaves);

    /// <summary>
    /// EnemySpawner manages wave-based enemy spawning.
    /// - Configurable spawn points (Node3D markers)
    /// - Cooldown between waves
    /// - Max active enemies cap (default 8 — Android safe)
    /// - Difficulty scaling via WaveIndex passed to EnemyController
    /// - Fires WaveStarted / WaveComplete / AllWavesComplete on EventBus
    /// </summary>
    public partial class EnemySpawner : Node3D
    {
        // ----------------------------------------------------------------
        // Configuration (exported for Godot editor)
        // ----------------------------------------------------------------
        [Export] public int   MaxActiveEnemies { get; set; } = 8;
        [Export] public int   TotalWaves       { get; set; } = 5;
        [Export] public float WaveCooldown     { get; set; } = 5.0f;  // Seconds between waves
        [Export] public bool  AutoStart        { get; set; } = true;

        // ----------------------------------------------------------------
        // Spawn data
        // ----------------------------------------------------------------
        private static readonly string[][] WaveCompositions = new[]
        {
            // Wave 1 — 3 goblins
            new[] { "goblin_grunt", "goblin_grunt", "goblin_grunt" },
            // Wave 2 — 2 goblins + 1 wolf
            new[] { "goblin_grunt", "goblin_grunt", "forest_wolf" },
            // Wave 3 — 2 wolves + 1 skeleton
            new[] { "forest_wolf", "forest_wolf", "skeleton_warrior" },
            // Wave 4 — 2 skeletons + 1 dark mage
            new[] { "skeleton_warrior", "skeleton_warrior", "dark_mage" },
            // Wave 5 — boss wave: 1 stone golem + 2 goblins
            new[] { "stone_golem", "goblin_grunt", "goblin_grunt" },
        };

        // ----------------------------------------------------------------
        // Runtime state
        // ----------------------------------------------------------------
        public int  CurrentWave    { get; private set; } = 0;
        public bool IsActive       { get; private set; } = false;
        public bool IsComplete     { get; private set; } = false;

        private readonly List<EnemyController> _activeEnemies = new();
        private float    _waveCooldownTimer = 0f;
        private bool     _waitingForNextWave = false;

        // Optional: player reference for enemy targeting
        private Node3D? _playerNode;

        // Spawn point children — any Node3D child named "SpawnPoint*"
        private readonly List<Node3D> _spawnPoints = new();

        // ----------------------------------------------------------------
        // Lifecycle
        // ----------------------------------------------------------------
        public override void _Ready()
        {
            // Collect spawn point children
            foreach (Node child in GetChildren())
            {
                if (child is Node3D marker && child.Name.ToString().StartsWith("SpawnPoint"))
                    _spawnPoints.Add(marker);
            }

            if (_spawnPoints.Count == 0)
            {
                // Use self position as fallback spawn point
                _spawnPoints.Add(this);
                Logger.Warning("EnemySpawner: No SpawnPoint children found. Using spawner origin.");
            }

            // Subscribe to enemy death events
            EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);

            if (AutoStart) StartWaves();

            Logger.Info($"EnemySpawner: Ready. SpawnPoints={_spawnPoints.Count} Waves={TotalWaves}");
        }

        public override void _ExitTree()
        {
            EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
        }

        public override void _Process(double delta)
        {
            if (!IsActive || IsComplete) return;

            // Clean up freed enemy references
            _activeEnemies.RemoveAll(e => !IsInstanceValid(e));

            // Wave cooldown countdown
            if (_waitingForNextWave)
            {
                _waveCooldownTimer -= (float)delta;
                if (_waveCooldownTimer <= 0f)
                {
                    _waitingForNextWave = false;
                    SpawnNextWave();
                }
            }
        }

        // ----------------------------------------------------------------
        // Public API
        // ----------------------------------------------------------------
        public void StartWaves()
        {
            if (IsActive) return;
            IsActive    = false;
            IsComplete  = false;
            CurrentWave = 0;
            _activeEnemies.Clear();
            IsActive = true;
            SpawnNextWave();
        }

        public void SetPlayerTarget(Node3D player)
        {
            _playerNode = player;
            foreach (var e in _activeEnemies)
                if (IsInstanceValid(e)) e.SetTarget(player);
        }

        // ----------------------------------------------------------------
        // Internal wave management
        // ----------------------------------------------------------------
        private void SpawnNextWave()
        {
            if (CurrentWave >= TotalWaves)
            {
                OnAllWavesComplete();
                return;
            }

            CurrentWave++;
            int waveIdx = Math.Min(CurrentWave - 1, WaveCompositions.Length - 1);
            string[] composition = WaveCompositions[waveIdx];

            // Clamp to active enemy cap
            int toSpawn = Math.Min(composition.Length, MaxActiveEnemies - _activeEnemies.Count);
            if (toSpawn <= 0) toSpawn = 1;

            Logger.Info($"EnemySpawner: Starting Wave {CurrentWave}/{TotalWaves} — {toSpawn} enemies.");
            EventBus.Publish(new WaveStartedEvent(CurrentWave, toSpawn));

            for (int i = 0; i < toSpawn; i++)
            {
                string id    = composition[i % composition.Length];
                Vector3 pos  = GetSpawnPosition(i);
                SpawnEnemy(id, pos, CurrentWave);
            }
        }

        private void SpawnEnemy(string enemyId, Vector3 position, int waveIndex)
        {
            if (_activeEnemies.Count >= MaxActiveEnemies)
            {
                Logger.Warning($"EnemySpawner: Max enemy cap ({MaxActiveEnemies}) reached. Skipping '{enemyId}'.");
                return;
            }

            var enemy = new EnemyController
            {
                EnemyId   = enemyId,
                WaveIndex = waveIndex
            };

            enemy.GlobalPosition = position;
            AddChild(enemy);

            if (_playerNode != null) enemy.SetTarget(_playerNode);
            _activeEnemies.Add(enemy);

            Logger.Info($"EnemySpawner: Spawned '{enemyId}' at {position} (wave {waveIndex}).");
        }

        private Vector3 GetSpawnPosition(int index)
        {
            var point = _spawnPoints[index % _spawnPoints.Count];
            // Scatter slightly to avoid overlapping spawns
            float scatter = 1.5f;
            float angle   = index * MathF.PI * 0.618f;  // Golden ratio spread
            return point.GlobalPosition + new Vector3(
                MathF.Cos(angle) * scatter,
                0f,
                MathF.Sin(angle) * scatter);
        }

        // ----------------------------------------------------------------
        // Event handlers
        // ----------------------------------------------------------------
        private void OnEnemyDied(EnemyDiedEvent e)
        {
            // Remove from active list (invalid refs cleaned in _Process)
            Logger.Info($"EnemySpawner: Enemy '{e.EnemyId}' died. Active={_activeEnemies.Count - 1}");

            // Check if wave is clear
            int alive = _activeEnemies.Count(c => IsInstanceValid(c));
            if (alive <= 1)  // <=1 because the dying enemy may still be valid this frame
                OnWaveClear();
        }

        private void OnWaveClear()
        {
            if (_waitingForNextWave || IsComplete) return;

            int remaining = TotalWaves - CurrentWave;
            Logger.Info($"EnemySpawner: Wave {CurrentWave} cleared! {remaining} wave(s) remaining.");
            EventBus.Publish(new WaveCompleteEvent(CurrentWave, remaining));

            if (CurrentWave >= TotalWaves)
            {
                OnAllWavesComplete();
                return;
            }

            _waitingForNextWave    = true;
            _waveCooldownTimer     = WaveCooldown;
        }

        private void OnAllWavesComplete()
        {
            IsComplete = true;
            Logger.Info($"EnemySpawner: All {TotalWaves} waves complete!");
            EventBus.Publish(new AllWavesCompleteEvent(TotalWaves));
        }
    }
}
