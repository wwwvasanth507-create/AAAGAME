using System;
using System.Collections.Generic;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Global EventBus to allow decoupled communication between managers and entities.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _eventListeners = new Dictionary<Type, List<Delegate>>();

        public static void Subscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<Delegate>();
            }
            _eventListeners[eventType].Add(listener);
            Logger.Info($"EventBus: Subscribed listener to event {eventType.Name}");
        }

        public static void Unsubscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType].Remove(listener);
                Logger.Info($"EventBus: Unsubscribed listener from event {eventType.Name}");
            }
        }

        public static void Publish<T>(T eventArgs)
        {
            Type eventType = typeof(T);
            if (_eventListeners.ContainsKey(eventType))
            {
                // Create a temporary copy to prevent modification errors during iteration loops
                var listenersCopy = new List<Delegate>(_eventListeners[eventType]);
                foreach (var listener in listenersCopy)
                {
                    try
                    {
                        (listener as Action<T>)?.Invoke(eventArgs);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"EventBus: Exception while dispatching event {eventType.Name}: {ex.Message}");
                    }
                }
            }
        }
    }
}
