using System;
using System.Collections.Generic;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Resource preloading manager. Pools instantiated nodes and mesh buffers
    /// to avoid run-time garbage collection pauses.
    /// </summary>
    public class ResourceManager
    {
        private readonly Dictionary<string, object> _loadedCache = new Dictionary<string, object>();

        public void PreloadAsset(string path)
        {
            Logger.Info($"ResourceManager: Asynchronously caching resource package: {path}");
            
            // Mock preloading assets (Real implementation uses ResourceLoader.Load)
            _loadedCache[path] = new object();
        }

        public object? GetAsset(string path)
        {
            if (_loadedCache.TryGetValue(path, out object? asset))
            {
                return asset;
            }
            Logger.Warning($"ResourceManager: Asset '{path}' was not preloaded! Performance stall risk.");
            PreloadAsset(path);
            return _loadedCache[path];
        }

        public void UnloadCache()
        {
            Logger.Info("ResourceManager: Sweeping asset caches. Cleaning unused mesh instances.");
            _loadedCache.Clear();
        }
    }
}
