using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Story
{
    public interface IStoryContentPlugin
    {
        string PluginName { get; }
        void OnChapterCompleted(string chapterId);
    }

    /// <summary>
    /// Central story progression orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates campaign chapters, world-state transitions, cinematic triggers,
    /// mission checkpoints, story events, lore codex, and plugin extensions.
    /// </summary>
    public partial class StoryFrameworkManager : Node, IInitializable
    {
        private bool _initialized = false;

        public StoryProgressionManager Progression { get; private set; } = new();
        public WorldStateManager WorldState { get; private set; } = new();
        public CinematicTriggerFramework CinematicTriggers { get; private set; } = new();
        public MissionFlowController MissionFlow { get; private set; } = new();
        public StoryEventManager EventManager { get; private set; } = new();
        public LoreManager LoreManager { get; private set; } = new();

        private readonly List<IStoryContentPlugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("StoryFrameworkManager: Initializing story progression framework...");

            Progression.StoryDatabase.RegisterDefaultEntries();
            Progression.Chapters.RegisterDefaultChapters();

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("StoryFrameworkManager: Story framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("StoryFrameworkManager: Shutting down story framework...");
            _plugins.Clear();

            ServiceLocator.Unregister<StoryFrameworkManager>();
            _initialized = false;
        }

        public void RegisterPlugin(IStoryContentPlugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }
}
