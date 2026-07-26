using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter13
{
    public class CheckpointNodeRecord
    {
        public string CheckpointId { get; set; } = "";
        public string Name { get; set; } = "";
        public string SectorId { get; set; } = "";
        public bool IsActive { get; set; } = false;
        public bool ShortcutUnlocked { get; set; } = false;
    }

    /// <summary>
    /// Dungeon Checkpoint Network Manager for Chapter 13 & Final Dungeon.
    /// Controls respawn waypoints, shortcut gates, and fast-travel portals within The Citadel of Obsidian Void.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class DungeonCheckpointNetwork : IInitializable
    {
        private readonly Dictionary<string, CheckpointNodeRecord> _checkpoints = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<CheckpointNodeRecord>? OnCheckpointActivated;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultCheckpoints();

            // Register with ServiceLocator
            ServiceLocator.Register<DungeonCheckpointNetwork>(this);

            IsInitialized = true;
            Logger.Info("DungeonCheckpointNetwork: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _checkpoints.Clear();

            ServiceLocator.Unregister<DungeonCheckpointNetwork>();
            IsInitialized = false;
            Logger.Info("DungeonCheckpointNetwork: Shutdown completed.");
        }

        private void RegisterDefaultCheckpoints()
        {
            // 1. Outer Breach Waypoint
            RegisterCheckpoint(new CheckpointNodeRecord
            {
                CheckpointId = "chk_outer_breach",
                Name = "Siege Camp Breach Waypoint",
                SectorId = "sector_outer_breach",
                IsActive = true
            });

            // 2. Gatehouse Waypoint
            RegisterCheckpoint(new CheckpointNodeRecord
            {
                CheckpointId = "chk_gatehouse",
                Name = "Iron Gatehouse Waypoint",
                SectorId = "sector_fortified_gatehouse"
            });

            // 3. Antechamber Threshold Waypoint
            RegisterCheckpoint(new CheckpointNodeRecord
            {
                CheckpointId = "chk_antechamber_threshold",
                Name = "Malakor's Antechamber Waypoint",
                SectorId = "sector_pre_final_antechamber"
            });
        }

        public void RegisterCheckpoint(CheckpointNodeRecord checkpoint)
        {
            if (checkpoint != null && !string.IsNullOrEmpty(checkpoint.CheckpointId))
            {
                _checkpoints[checkpoint.CheckpointId] = checkpoint;
            }
        }

        public bool ActivateCheckpoint(string checkpointId)
        {
            if (!_checkpoints.TryGetValue(checkpointId, out var chk)) return false;
            if (chk.IsActive) return true;

            chk.IsActive = true;
            OnCheckpointActivated?.Invoke(chk);

            Logger.Info($"DungeonCheckpointNetwork: Activated Checkpoint Waypoint '{chk.Name}' ({checkpointId}) in sector {chk.SectorId}.");
            return true;
        }

        public bool UnlockShortcut(string checkpointId)
        {
            if (!_checkpoints.TryGetValue(checkpointId, out var chk)) return false;
            chk.ShortcutUnlocked = true;
            Logger.Info($"DungeonCheckpointNetwork: Unlocked shortcut gate for checkpoint '{chk.Name}'.");
            return true;
        }

        public CheckpointNodeRecord? GetCheckpoint(string checkpointId)
        {
            return _checkpoints.TryGetValue(checkpointId, out var c) ? c : null;
        }

        public List<CheckpointNodeRecord> GetAllCheckpoints()
        {
            return new List<CheckpointNodeRecord>(_checkpoints.Values);
        }
    }
}
