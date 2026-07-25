using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Any service that needs one-time initialization should implement this interface.
    /// ServiceLocator calls Initialize() automatically on first Get<T>().
    /// This eliminates type-coupling inside the locator.
    /// </summary>
    public interface IInitializable
    {
        void Initialize();
    }

    /// <summary>
    /// Thread-safe dependency injection and service lookup container.
    /// Handles manager initialization in a safe order and logs startup performance metrics.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static readonly HashSet<Type> _initialized = new HashSet<Type>();
        private static readonly object _lock = new object();

        public static void Register<T>(T service) where T : class
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (_services.ContainsKey(type))
                {
                    Logger.Warning($"ServiceLocator: Duplicate service registration for type '{type.Name}' bypassed.");
                    return;
                }
                _services[type] = service;
                Logger.Info($"ServiceLocator: Service '{type.Name}' registered.");
            }
        }

        public static T Get<T>() where T : class
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (!_services.TryGetValue(type, out var service))
                {
                    throw new InvalidOperationException($"ServiceLocator: Requested service '{type.Name}' is not registered.");
                }
                
                // Lazy initialization check
                if (!_initialized.Contains(type))
                {
                    InitializeService(type, service);
                }

                return (T)service;
            }
        }

        public static void Unregister<T>() where T : class
        {
            lock (_lock)
            {
                Type type = typeof(T);
                _services.Remove(type);
                _initialized.Remove(type);
                Logger.Info($"ServiceLocator: Service '{type.Name}' unregistered.");
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _services.Clear();
                _initialized.Clear();
                Logger.Info("ServiceLocator: Flushed all registered services.");
            }
        }

        private static void InitializeService(Type type, object service)
        {
            _initialized.Add(type);
            var sw = Stopwatch.StartNew();

            Logger.Info($"ServiceLocator: Initializing service '{type.Name}'...");

            // Open/Closed: any manager implementing IInitializable is initialized automatically.
            // No concrete type knowledge required inside the locator.
            if (service is IInitializable initializable)
            {
                initializable.Initialize();
            }

            sw.Stop();
            Logger.Info($"ServiceLocator: Service '{type.Name}' initialized in {sw.ElapsedMilliseconds} ms.");
        }
    }
}
