using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Animation
{
    public enum AnimationEventType
    {
        Footstep,
        WeaponImpact,
        AbilityTiming,
        SoundTrigger,
        ParticleTrigger,
        CameraShake,
        DamageWindowStart,
        DamageWindowEnd,
        InteractionWindowStart,
        InteractionWindowEnd,
        Custom
    }

    public class AnimationEventData
    {
        public AnimationEventType EventType { get; set; }
        public string EventName { get; set; } = string.Empty;
        public float TimeSeconds { get; set; } = 0.0f;
        public string PayloadString { get; set; } = string.Empty;
        public float PayloadFloat { get; set; } = 0.0f;
        public Vector3 PositionOffset { get; set; } = Vector3.Zero;
    }

    /// <summary>
    /// Frame-accurate animation event dispatcher routing events from animation clips
    /// to combat, sound, particle, and camera systems.
    /// </summary>
    public class AnimationEventSystem
    {
        public event Action<AnimationEventData>? OnAnimationEvent;

        private readonly Dictionary<string, List<AnimationEventData>> _registeredEvents = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterEvent(string clipName, AnimationEventData eventData)
        {
            if (!_registeredEvents.TryGetValue(clipName, out var list))
            {
                list = new List<AnimationEventData>();
                _registeredEvents[clipName] = list;
            }
            list.Add(eventData);
        }

        public void DispatchEvent(AnimationEventData eventData)
        {
            OnAnimationEvent?.Invoke(eventData);

            // Forward to EventBus if sound/footstep
            if (eventData.EventType == AnimationEventType.Footstep)
            {
                HeroOfEternia.Core.EventBus.Publish(new HeroOfEternia.Audio.FootstepEvent(eventData.PositionOffset, eventData.PayloadString));
            }
        }

        public IReadOnlyList<AnimationEventData> GetEventsForClip(string clipName)
        {
            return _registeredEvents.TryGetValue(clipName, out var list) ? list : Array.Empty<AnimationEventData>();
        }

        public void ClearEvents()
        {
            _registeredEvents.Clear();
        }
    }
}
