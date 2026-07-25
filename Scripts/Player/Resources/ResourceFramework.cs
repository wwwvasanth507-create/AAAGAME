using System;
using System.Collections.Generic;
using HeroOfEternia.Core;
using HeroOfEternia.Player.Abilities;

namespace HeroOfEternia.Player.Resources
{
    /// <summary>
    /// Represents a single resource pool (e.g., Mana, Stamina, Energy, Rage, Focus, Spirit).
    /// Supports configurable max values, regeneration rates, and modifiers.
    /// </summary>
    public class ResourcePool
    {
        public string ResourceName { get; private set; }
        public float Current { get; private set; }
        public float Max { get; private set; }
        public float BaseRegenPerSec { get; private set; }
        public float RegenMultiplier { get; set; } = 1.0f;
        public bool CanRegenerate { get; set; } = true;
        public bool IsDepleted => Current <= 0.01f;
        public float Percent => Max > 0 ? Current / Max : 0f;

        public ResourcePool(string name, float max, float regenPerSec = 0f)
        {
            ResourceName = name;
            Max = max;
            Current = max;
            BaseRegenPerSec = regenPerSec;
        }

        public bool HasEnough(float amount) => Current >= amount - 0.001f;

        public bool Spend(float amount)
        {
            if (!HasEnough(amount)) return false;
            Current = MathF.Max(0, Current - amount);
            return true;
        }

        public void Restore(float amount)
        {
            Current = MathF.Min(Max, Current + amount);
        }

        public void SetMax(float newMax, bool preservePercent = true)
        {
            float percent = preservePercent ? Percent : 1.0f;
            Max = MathF.Max(1, newMax);
            Current = Max * percent;
        }

        public void Tick(float delta)
        {
            if (!CanRegenerate || BaseRegenPerSec <= 0f) return;
            float regen = BaseRegenPerSec * RegenMultiplier * delta;
            Current = MathF.Min(Max, Current + regen);
        }

        public override string ToString() =>
            $"{ResourceName}: {Current:F1}/{Max:F1} ({Percent:P0})";
    }

    /// <summary>
    /// Configuration for a custom resource type (data-driven).
    /// </summary>
    public class ResourceConfig
    {
        public ResourceType Type { get; set; } = ResourceType.Mana;
        public float BaseMax { get; set; } = 100f;
        public float BaseRegenPerSec { get; set; } = 5f;
        public bool Enabled { get; set; } = true;
        public string DisplayName { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string Color { get; set; } = "#00BFFF";
    }

    /// <summary>
    /// Manages all player resource pools with configurable definitions.
    /// Supports any combination of resources without hardcoded assumptions.
    /// </summary>
    public class ResourceManager
    {
        private readonly Dictionary<ResourceType, ResourcePool> _pools = new();
        private readonly Dictionary<ResourceType, ResourceConfig> _configs = new();

        public event Action<ResourceType, float, float>? OnResourceChanged;
        public event Action<ResourceType>? OnResourceDepleted;

        public ResourceManager()
        {
            // Register default resource configs
            RegisterDefaultConfigs();
        }

        private void RegisterDefaultConfigs()
        {
            RegisterConfig(new ResourceConfig
            {
                Type = ResourceType.Mana,
                BaseMax = 100f,
                BaseRegenPerSec = 5f,
                DisplayName = "Mana",
                Color = "#00BFFF"
            });

            RegisterConfig(new ResourceConfig
            {
                Type = ResourceType.Stamina,
                BaseMax = 100f,
                BaseRegenPerSec = 15f,
                DisplayName = "Stamina",
                Color = "#32CD32"
            });

            RegisterConfig(new ResourceConfig
            {
                Type = ResourceType.Health,
                BaseMax = 500f,
                BaseRegenPerSec = 2f,
                DisplayName = "Health",
                Color = "#FF4444"
            });

            RegisterConfig(new ResourceConfig
            {
                Type = ResourceType.Energy,
                BaseMax = 100f,
                BaseRegenPerSec = 10f,
                DisplayName = "Energy",
                Color = "#FFD700"
            });

            RegisterConfig(new ResourceConfig
            {
                Type = ResourceType.Focus,
                BaseMax = 100f,
                BaseRegenPerSec = 8f,
                DisplayName = "Focus",
                Color = "#FF69B4"
            });

            // Rage/Spirit as combat-only resources (no regen, gained through actions)
            RegisterConfig(new ResourceConfig
            {
                Type = ResourceType.Rage,
                BaseMax = 100f,
                BaseRegenPerSec = 0f,
                DisplayName = "Rage",
                Color = "#FF4500"
            });

            RegisterConfig(new ResourceConfig
            {
                Type = ResourceType.Spirit,
                BaseMax = 100f,
                BaseRegenPerSec = 0f,
                DisplayName = "Spirit",
                Color = "#9370DB"
            });
        }

        public void RegisterConfig(ResourceConfig config)
        {
            _configs[config.Type] = config;
            if (config.Enabled && !_pools.ContainsKey(config.Type))
            {
                _pools[config.Type] = new ResourcePool(
                    config.DisplayName,
                    config.BaseMax,
                    config.BaseRegenPerSec
                );
            }
        }

        public ResourcePool? GetPool(ResourceType type)
        {
            _pools.TryGetValue(type, out var pool);
            return pool;
        }

        public bool HasEnough(ResourceType type, float amount)
        {
            return _pools.TryGetValue(type, out var pool) && pool.HasEnough(amount);
        }

        public bool Spend(ResourceType type, float amount)
        {
            if (!_pools.TryGetValue(type, out var pool)) return false;
            bool result = pool.Spend(amount);
            if (result)
            {
                OnResourceChanged?.Invoke(type, pool.Current, pool.Max);
                if (pool.IsDepleted) OnResourceDepleted?.Invoke(type);
            }
            return result;
        }

        public void Restore(ResourceType type, float amount)
        {
            if (!_pools.TryGetValue(type, out var pool)) return;
            pool.Restore(amount);
            OnResourceChanged?.Invoke(type, pool.Current, pool.Max);
        }

        public void SetMax(ResourceType type, float newMax)
        {
            if (!_pools.TryGetValue(type, out var pool)) return;
            pool.SetMax(newMax);
            OnResourceChanged?.Invoke(type, pool.Current, pool.Max);
        }

        public void UpdateAllConfigs(List<ResourceConfig> configs)
        {
            foreach (var config in configs)
            {
                _configs[config.Type] = config;
                if (_pools.TryGetValue(config.Type, out var pool))
                {
                    pool.SetMax(config.BaseMax);
                }
                else if (config.Enabled)
                {
                    _pools[config.Type] = new ResourcePool(
                        config.DisplayName,
                        config.BaseMax,
                        config.BaseRegenPerSec
                    );
                }
            }
        }

        public void TickAll(float delta)
        {
            foreach (var pool in _pools.Values)
            {
                float before = pool.Current;
                pool.Tick(delta);
                if (MathF.Abs(pool.Current - before) > 0.01f)
                {
                    // Find the type for this pool (inefficient but acceptable for tick)
                    foreach (var kvp in _pools)
                    {
                        if (kvp.Value == pool)
                        {
                            OnResourceChanged?.Invoke(kvp.Key, pool.Current, pool.Max);
                            break;
                        }
                    }
                }
            }
        }

        public void RestoreAll()
        {
            foreach (var kvp in _pools)
            {
                kvp.Value.Restore(kvp.Value.Max);
                OnResourceChanged?.Invoke(kvp.Key, kvp.Value.Current, kvp.Value.Max);
            }
        }

        public string GetDisplayName(ResourceType type)
        {
            return _configs.TryGetValue(type, out var config) ? config.DisplayName : type.ToString();
        }

        public string GetColor(ResourceType type)
        {
            return _configs.TryGetValue(type, out var config) ? config.Color : "#FFFFFF";
        }

        public IReadOnlyDictionary<ResourceType, ResourcePool> AllPools => _pools;
    }
}