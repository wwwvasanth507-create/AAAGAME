using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter12
{
    public class WarEventRecord
    {
        public string EventId { get; set; } = "";
        public string Title { get; set; } = "";
        public string RegionZoneId { get; set; } = "";
        public string EventType { get; set; } = "Skirmish"; // Skirmish, Escort, SupplyDisruption, SiegePatrol
        public bool IsActive { get; set; } = true;
        public bool IsCompleted { get; set; } = false;
        public int RequiredLevel { get; set; } = 42;
        public int AllianceReadinessReward { get; set; } = 5;
    }

    /// <summary>
    /// World War Event Manager for Chapter 12.
    /// Manages dynamic cross-region war events, supply caravans, refugee escorts, and alliance defense patrols.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class WorldWarEventManager : IInitializable
    {
        private readonly Dictionary<string, WarEventRecord> _warEvents = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<WarEventRecord>? OnWarEventTriggered;
        public event Action<WarEventRecord>? OnWarEventCompleted;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultWarEvents();

            // Register with ServiceLocator
            ServiceLocator.Register<WorldWarEventManager>(this);

            IsInitialized = true;
            Logger.Info("WorldWarEventManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _warEvents.Clear();

            ServiceLocator.Unregister<WorldWarEventManager>();
            IsInitialized = false;
            Logger.Info("WorldWarEventManager: Shutdown completed.");
        }

        private void RegisterDefaultWarEvents()
        {
            // 1. Crystal Wasteland Caravan Escort
            RegisterWarEvent(new WarEventRecord
            {
                EventId = "event_caravan_escort",
                Title = "Crystal Wasteland Supply Caravan Escort",
                RegionZoneId = "zone_crystal_wasteland",
                EventType = "Escort",
                RequiredLevel = 41,
                AllianceReadinessReward = 5
            });

            // 2. Caelum Ruins Skirmish
            RegisterWarEvent(new WarEventRecord
            {
                EventId = "event_caelum_skirmish",
                Title = "Liberation of Caelum Floating Spire",
                RegionZoneId = "zone_caelum_ruins",
                EventType = "Skirmish",
                RequiredLevel = 43,
                AllianceReadinessReward = 8
            });
        }

        public void RegisterWarEvent(WarEventRecord warEvent)
        {
            if (warEvent != null && !string.IsNullOrEmpty(warEvent.EventId))
            {
                _warEvents[warEvent.EventId] = warEvent;
            }
        }

        public bool CompleteWarEvent(string eventId)
        {
            if (!_warEvents.TryGetValue(eventId, out var evt)) return false;
            if (evt.IsCompleted) return true;

            evt.IsActive = false;
            evt.IsCompleted = true;

            OnWarEventCompleted?.Invoke(evt);
            Logger.Info($"WorldWarEventManager: Completed War Event '{evt.Title}' ({eventId})! Alliance Readiness +{evt.AllianceReadinessReward}%.");
            return true;
        }

        public WarEventRecord? GetWarEvent(string eventId)
        {
            return _warEvents.TryGetValue(eventId, out var e) ? e : null;
        }

        public List<WarEventRecord> GetAllWarEvents()
        {
            return new List<WarEventRecord>(_warEvents.Values);
        }
    }
}
