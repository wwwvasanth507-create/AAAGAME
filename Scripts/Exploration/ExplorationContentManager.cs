using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Exploration
{
    public interface IExplorationContentPlugin
    {
        string PluginName { get; }
        void OnActivityCompleted(string activityId);
    }

    /// <summary>
    /// Central exploration content orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates activities, puzzles, secrets, collectibles, environmental interactions,
    /// dynamic events, rewards, and plugin extensions.
    /// </summary>
    public partial class ExplorationContentManager : Node, IInitializable
    {
        private bool _initialized = false;

        public ActivityDatabase ActivityDatabase { get; private set; } = new();
        public PuzzleManager PuzzleManager { get; private set; } = new();
        public SecretManager SecretManager { get; private set; } = new();
        public CollectibleDatabase CollectibleDatabase { get; private set; } = new();
        public EnvironmentalInteractionEngine InteractionEngine { get; private set; } = new();
        public ExplorationEventManager EventManager { get; private set; } = new();
        public ExplorationRewardFramework RewardFramework { get; private set; } = new();

        private readonly HashSet<string> _completedActivities = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<IExplorationContentPlugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("ExplorationContentManager: Initializing exploration content framework...");

            ActivityDatabase.RegisterDefaultActivities();

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("ExplorationContentManager: Exploration content framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("ExplorationContentManager: Shutting down exploration framework...");
            _completedActivities.Clear();
            _plugins.Clear();

            ServiceLocator.Unregister<ExplorationContentManager>();
            _initialized = false;
        }

        public bool CompleteActivity(string activityId)
        {
            if (_completedActivities.Add(activityId))
            {
                foreach (var plugin in _plugins)
                {
                    plugin.OnActivityCompleted(activityId);
                }
                return true;
            }
            return false;
        }

        public bool IsActivityCompleted(string activityId)
        {
            return _completedActivities.Contains(activityId);
        }

        public void RegisterPlugin(IExplorationContentPlugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }
}
