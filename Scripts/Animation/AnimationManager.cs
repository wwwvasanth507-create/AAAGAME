using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Animation
{
    public interface IAnimationPlugin
    {
        string PluginName { get; }
        void OnStateChanged(AnimationState oldState, AnimationState newState);
    }

    /// <summary>
    /// Central animation orchestrator implementing <see cref="IInitializable"/>.
    /// Manages state transitions, 10 blend layers, IK solvers, procedural motion,
    /// event routing, clip caching, and plugin extensions.
    /// </summary>
    public partial class AnimationManager : Node, IInitializable
    {
        private bool _initialized = false;

        public CharacterAnimationProfile Profile { get; private set; } = CharacterAnimationProfile.CreateDefaultPlayerProfile();
        public IKSystem IKSystem { get; private set; }
        public ProceduralAnimationEngine ProceduralEngine { get; private set; }
        public AnimationEventSystem EventSystem { get; private set; } = new();
        public RootMotionController RootMotion { get; private set; }

        private readonly Dictionary<AnimationLayerType, AnimationLayer> _layers = new();
        private readonly List<IAnimationPlugin> _plugins = new();
        private readonly Dictionary<string, AnimationPlayer> _animatorPool = new(StringComparer.OrdinalIgnoreCase);

        public AnimationState CurrentState => _layers[AnimationLayerType.FullBody].CurrentState;
        public bool IsInitialized => _initialized;
        public bool DebugVisualization { get; set; } = false;

        public event Action<AnimationState, AnimationState>? OnStateChanged;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("AnimationManager: Initializing animation framework...");

            // Create sub-systems
            IKSystem = new IKSystem { Name = "IKSystem" };
            AddChild(IKSystem);

            ProceduralEngine = new ProceduralAnimationEngine { Name = "ProceduralEngine" };
            AddChild(ProceduralEngine);

            RootMotion = new RootMotionController { Name = "RootMotionController" };
            AddChild(RootMotion);

            // Initialize blend layers
            foreach (AnimationLayerType layerType in Enum.GetValues(typeof(AnimationLayerType)))
            {
                _layers[layerType] = new AnimationLayer(layerType);
            }

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("AnimationManager: Animation framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("AnimationManager: Shutting down animation framework...");
            IKSystem?.QueueFree();
            ProceduralEngine?.QueueFree();
            RootMotion?.QueueFree();

            _layers.Clear();
            _plugins.Clear();
            _animatorPool.Clear();
            EventSystem.ClearEvents();

            ServiceLocator.Unregister<AnimationManager>();
            _initialized = false;
        }

        public bool PlayState(AnimationState state, AnimationLayerType layerType = AnimationLayerType.FullBody, AnimationPriority priority = AnimationPriority.Normal)
        {
            if (!_layers.TryGetValue(layerType, out var layer)) return false;

            AnimationState oldState = layer.CurrentState;
            if (layer.SetState(state, priority))
            {
                var config = Profile.GetConfig(state);
                if (config != null)
                {
                    RootMotion.RootMotionEnabled = config.EnablesRootMotion;
                }

                OnStateChanged?.Invoke(oldState, state);
                foreach (var plugin in _plugins)
                {
                    plugin.OnStateChanged(oldState, state);
                }
                return true;
            }
            return false;
        }

        public void SetProfile(CharacterAnimationProfile profile)
        {
            Profile = profile ?? CharacterAnimationProfile.CreateDefaultPlayerProfile();
        }

        public void RegisterPlugin(IAnimationPlugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }

        public AnimationLayer? GetLayer(AnimationLayerType type)
        {
            return _layers.TryGetValue(type, out var layer) ? layer : null;
        }
    }
}
