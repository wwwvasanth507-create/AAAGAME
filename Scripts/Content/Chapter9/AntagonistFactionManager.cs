using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter9
{
    public enum LegionAlertLevel
    {
        Low,
        Elevated,
        HighAlert,
        Lockdown
    }

    public class LegionUnitTypeRecord
    {
        public string UnitTypeId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public float AggroRadiusMeters { get; set; } = 12f;
        public bool TriggersAlarms { get; set; } = true;
    }

    /// <summary>
    /// Antagonist Faction Manager for The Shadow Legion of Malakor.
    /// Manages Legion hierarchy, alert levels, alarm gong mechanics, patrol coordination, and supply disruptions.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class AntagonistFactionManager : IInitializable
    {
        private readonly Dictionary<string, LegionUnitTypeRecord> _units = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }
        public LegionAlertLevel CurrentAlertLevel { get; private set; } = LegionAlertLevel.Low;
        public bool SupplyRouteDisrupted { get; private set; } = false;

        public event Action<LegionAlertLevel>? OnAlertLevelChanged;
        public event Action<string>? OnAlarmTriggered;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultUnits();

            // Register with ServiceLocator
            ServiceLocator.Register<AntagonistFactionManager>(this);

            IsInitialized = true;
            Logger.Info("AntagonistFactionManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _units.Clear();

            ServiceLocator.Unregister<AntagonistFactionManager>();
            IsInitialized = false;
            Logger.Info("AntagonistFactionManager: Shutdown completed.");
        }

        private void RegisterDefaultUnits()
        {
            RegisterUnit(new LegionUnitTypeRecord
            {
                UnitTypeId = "enemy_shadow_scout",
                Name = "Shadow Legion Scout",
                Role = "Recon & Alarm Trigger",
                AggroRadiusMeters = 15f,
                TriggersAlarms = true
            });

            RegisterUnit(new LegionUnitTypeRecord
            {
                UnitTypeId = "enemy_corrupted_iron_knight",
                Name = "Corrupted Iron Knight",
                Role = "Heavy Vanguard Guard",
                AggroRadiusMeters = 10f,
                TriggersAlarms = true
            });

            RegisterUnit(new LegionUnitTypeRecord
            {
                UnitTypeId = "enemy_legion_engineer",
                Name = "Legion Siege Engineer",
                Role = "Support & Trap Deployment",
                AggroRadiusMeters = 8f,
                TriggersAlarms = false
            });
        }

        public void RegisterUnit(LegionUnitTypeRecord unit)
        {
            if (unit != null && !string.IsNullOrEmpty(unit.UnitTypeId))
            {
                _units[unit.UnitTypeId] = unit;
            }
        }

        public bool RaiseAlert(LegionAlertLevel newLevel)
        {
            if (newLevel <= CurrentAlertLevel) return false;

            CurrentAlertLevel = newLevel;
            OnAlertLevelChanged?.Invoke(CurrentAlertLevel);

            Logger.Info($"AntagonistFactionManager: Raised Legion alert level to '{CurrentAlertLevel}'.");
            return true;
        }

        public bool SoundAlarm(string sectorId)
        {
            OnAlarmTriggered?.Invoke(sectorId);
            RaiseAlert(LegionAlertLevel.HighAlert);

            Logger.Warning($"AntagonistFactionManager: Alarm sounded in sector '{sectorId}'! Alert elevated to HighAlert.");
            return true;
        }

        public void SetSupplyDisrupted(bool state)
        {
            SupplyRouteDisrupted = state;
            Logger.Info($"AntagonistFactionManager: Legion supply route disruption set to '{state}'.");
        }

        public LegionUnitTypeRecord? GetUnit(string unitTypeId)
        {
            return _units.TryGetValue(unitTypeId, out var u) ? u : null;
        }
    }
}
