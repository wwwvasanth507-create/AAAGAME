using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter14
{
    public class ArenaHazardRecord
    {
        public string HazardId { get; set; } = "";
        public string Name { get; set; } = "";
        public bool IsActive { get; set; } = false;
        public int DamagePerSecond { get; set; } = 45;
    }

    /// <summary>
    /// Dynamic Final Boss Arena Manager for Arch-Sorcerer Malakor's Throne Room Arena.
    /// Manages terrain transformations, void crystal hazard flares, solar prism focus lenses, and lighting transitions across boss phases.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class FinalBossArenaManager : IInitializable
    {
        private readonly Dictionary<string, ArenaHazardRecord> _hazards = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<string>? OnArenaStateChanged;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultHazards();

            // Register with ServiceLocator
            ServiceLocator.Register<FinalBossArenaManager>(this);

            IsInitialized = true;
            Logger.Info("FinalBossArenaManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _hazards.Clear();

            ServiceLocator.Unregister<FinalBossArenaManager>();
            IsInitialized = false;
            Logger.Info("FinalBossArenaManager: Shutdown completed.");
        }

        private void RegisterDefaultHazards()
        {
            // 1. Sun Flare Hazard
            RegisterHazard(new ArenaHazardRecord
            {
                HazardId = "hazard_sun_flares",
                Name = "Solar Beam Flares",
                DamagePerSecond = 35
            });

            // 2. Gravity Distortion Hazard
            RegisterHazard(new ArenaHazardRecord
            {
                HazardId = "hazard_gravity_distortion",
                Name = "Gravitational Core Distortion",
                DamagePerSecond = 60
            });
        }

        public void RegisterHazard(ArenaHazardRecord hazard)
        {
            if (hazard != null && !string.IsNullOrEmpty(hazard.HazardId))
            {
                _hazards[hazard.HazardId] = hazard;
            }
        }

        public bool ActivateHazard(string hazardId)
        {
            if (!_hazards.TryGetValue(hazardId, out var h)) return false;
            if (h.IsActive) return true;

            h.IsActive = true;
            OnArenaStateChanged?.Invoke($"Hazard Activated: {h.Name}");
            Logger.Info($"FinalBossArenaManager: Activated arena hazard '{h.Name}' ({hazardId}) dealing {h.DamagePerSecond} DPS.");
            return true;
        }

        public ArenaHazardRecord? GetHazard(string hazardId)
        {
            return _hazards.TryGetValue(hazardId, out var h) ? h : null;
        }

        public List<ArenaHazardRecord> GetAllHazards()
        {
            return new List<ArenaHazardRecord>(_hazards.Values);
        }
    }
}
