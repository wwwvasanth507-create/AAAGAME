using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Graphics
{
    public interface IVFXPlugin
    {
        string PluginName { get; }
        void OnEffectSpawned(string effectId, Vector3 position);
    }

    /// <summary>
    /// Central visual presentation manager implementing <see cref="IInitializable"/>.
    /// Orchestrates particle effect pooling, lighting profiles, post-processing,
    /// weather visuals, decal spawning, camera effects, and rendering budgets.
    /// </summary>
    public partial class VisualEffectManager : Node, IInitializable
    {
        private bool _initialized = false;

        public ShaderManager ShaderManager { get; private set; } = new();
        public LightingManager LightingManager { get; private set; }
        public PostProcessingManager PostProcessingManager { get; private set; }
        public WeatherVisualsController WeatherVisuals { get; private set; }
        public DecalSystem DecalSystem { get; private set; } = new();
        public CameraEffectsController CameraEffects { get; private set; }
        public RenderingOptimizationManager RenderingOptimization { get; private set; } = new();

        private readonly Dictionary<string, ParticleEffectConfig> _effectRegistry = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<IVFXPlugin> _plugins = new();

        public bool IsInitialized => _initialized;
        public bool DebugVisualization { get; set; } = false;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("VisualEffectManager: Initializing presentation framework...");

            // Create sub-components
            LightingManager = new LightingManager { Name = "LightingManager" };
            AddChild(LightingManager);

            PostProcessingManager = new PostProcessingManager { Name = "PostProcessingManager" };
            AddChild(PostProcessingManager);

            WeatherVisuals = new WeatherVisualsController { Name = "WeatherVisuals" };
            AddChild(WeatherVisuals);

            CameraEffects = new CameraEffectsController { Name = "CameraEffects" };
            AddChild(CameraEffects);

            // Register default particle presets
            RegisterEffect(new ParticleEffectConfig { EffectId = "vfx_dust_step", Type = ParticleType.Dust, LifetimeSeconds = 1.0f });
            RegisterEffect(new ParticleEffectConfig { EffectId = "vfx_fire_burst", Type = ParticleType.Fire, LifetimeSeconds = 2.0f, Priority = VFXPriority.High });
            RegisterEffect(new ParticleEffectConfig { EffectId = "vfx_magic_cast", Type = ParticleType.Magic, LifetimeSeconds = 2.5f, Priority = VFXPriority.High });

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("VisualEffectManager: Visual presentation framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("VisualEffectManager: Shutting down presentation framework...");
            LightingManager?.QueueFree();
            PostProcessingManager?.QueueFree();
            WeatherVisuals?.QueueFree();
            CameraEffects?.QueueFree();

            _effectRegistry.Clear();
            _plugins.Clear();
            ShaderManager.ClearCache();
            DecalSystem.ClearDecals();

            ServiceLocator.Unregister<VisualEffectManager>();
            _initialized = false;
        }

        public void RegisterEffect(ParticleEffectConfig config)
        {
            if (config != null && !string.IsNullOrEmpty(config.EffectId))
            {
                _effectRegistry[config.EffectId] = config;
            }
        }

        public bool SpawnEffect(string effectId, Vector3 position)
        {
            if (!_effectRegistry.TryGetValue(effectId, out var config)) return false;

            // Forward event to plugins
            foreach (var plugin in _plugins)
            {
                plugin.OnEffectSpawned(effectId, position);
            }
            return true;
        }

        public void RegisterPlugin(IVFXPlugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }
}
