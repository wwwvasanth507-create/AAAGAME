using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Exploration
{
    public enum ExplorationEventType
    {
        FallingMeteor,
        TravelingMerchant,
        RareCreatureAppearance,
        WeatherDiscovery,
        TreasureCaravan,
        LostTraveler,
        ResourceSurge,
        AncientPortal,
        MagicStorm
    }

    public class DynamicExplorationEvent
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public ExplorationEventType Type { get; set; } = ExplorationEventType.ResourceSurge;
        public Vector3 Position { get; set; }
        public float DurationSeconds { get; set; } = 300f;
        public float ElapsedSeconds { get; set; } = 0.0f;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Dynamic exploration event scheduler spawning meteors, traveling merchants,
    /// rare creatures, portals, and magic storms.
    /// </summary>
    public class ExplorationEventManager
    {
        private readonly List<DynamicExplorationEvent> _activeEvents = new();

        public event Action<DynamicExplorationEvent>? OnEventSpawned;
        public event Action<DynamicExplorationEvent>? OnEventExpired;

        public DynamicExplorationEvent TriggerEvent(ExplorationEventType type, Vector3 position, float duration = 300f)
        {
            var evt = new DynamicExplorationEvent
            {
                Type = type,
                Position = position,
                DurationSeconds = duration
            };

            _activeEvents.Add(evt);
            OnEventSpawned?.Invoke(evt);
            return evt;
        }

        public void Update(float delta)
        {
            for (int i = _activeEvents.Count - 1; i >= 0; i--)
            {
                var evt = _activeEvents[i];
                if (evt.IsActive)
                {
                    evt.ElapsedSeconds += delta;
                    if (evt.ElapsedSeconds >= evt.DurationSeconds)
                    {
                        evt.IsActive = false;
                        OnEventExpired?.Invoke(evt);
                        _activeEvents.RemoveAt(i);
                    }
                }
            }
        }

        public IReadOnlyList<DynamicExplorationEvent> ActiveEvents => _activeEvents;
    }
}
