using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Story
{
    public class StoryEventOverride
    {
        public string EventId { get; set; } = string.Empty;
        public string MusicOverrideTrack { get; set; } = string.Empty;
        public string LightingProfileOverride { get; set; } = string.Empty;
        public List<string> NpcSpawnsToEnable { get; set; } = new();
        public List<string> NpcSpawnsToDisable { get; set; } = new();
    }

    /// <summary>
    /// Story event manager orchestrating dynamic environment overrides, NPC spawn changes,
    /// lighting profiles, and music overrides during campaign beats.
    /// </summary>
    public class StoryEventManager
    {
        private readonly Dictionary<string, StoryEventOverride> _overrides = new(StringComparer.OrdinalIgnoreCase);

        public event Action<StoryEventOverride>? OnStoryEventTriggered;

        public void RegisterOverride(StoryEventOverride evt)
        {
            if (evt != null && !string.IsNullOrEmpty(evt.EventId))
            {
                _overrides[evt.EventId] = evt;
            }
        }

        public bool TriggerStoryEvent(string eventId)
        {
            if (_overrides.TryGetValue(eventId, out var evt))
            {
                OnStoryEventTriggered?.Invoke(evt);
                return true;
            }
            return false;
        }
    }
}
