using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Resource preloading manager. Pools instantiated nodes and mesh buffers
    /// using Godot's ResourceLoader to avoid run-time garbage collection pauses.
    /// </summary>
    public class ResourceManager
    {
        private readonly Dictionary<string, object> _loadedCache = new();

        /// <summary>
        /// Asynchronously preloads and caches a Godot resource.
        /// </summary>
        public void PreloadAsset(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (_loadedCache.ContainsKey(path))
            {
                return; // Already preloaded
            }

            Logger.Info($"ResourceManager: Caching resource package: {path}");
            
            try
            {
                var resource = ResourceLoader.Load(path);
                if (resource != null)
                {
                    _loadedCache[path] = resource;
                }
                else
                {
                    Logger.Warning($"ResourceManager: Failed to load resource '{path}'. Resource returned null.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"ResourceManager: Exception encountered while preloading asset '{path}': {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a preloaded asset cast to the specified type T.
        /// </summary>
        public T? GetAsset<T>(string path) where T : class
        {
            if (_loadedCache.TryGetValue(path, out object? asset))
            {
                return asset as T;
            }

            Logger.Warning($"ResourceManager: Asset '{path}' was not preloaded! Performance stall risk.");
            PreloadAsset(path);

            if (_loadedCache.TryGetValue(path, out object? fallbackAsset))
            {
                return fallbackAsset as T;
            }

            return null;
        }

        /// <summary>
        /// Legacy compatibility wrapper for untyped asset retrieval.
        /// </summary>
        public object? GetAsset(string path)
        {
            return GetAsset<object>(path);
        }

        /// <summary>
        /// Clears the asset preloading cache to free memory.
        /// </summary>
        public void UnloadCache()
        {
            Logger.Info("ResourceManager: Sweeping asset caches. Cleaning unused resource instances.");
            _loadedCache.Clear();
            
            // Force Garbage Collection to reclaim memory immediately (critical on mobile/Android)
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
