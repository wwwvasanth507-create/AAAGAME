using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter2
{
    public interface IChapter2Plugin
    {
        string PluginName { get; }
        void OnWorldPhaseAdvanced(WorldPhase phase);
    }

    /// <summary>
    /// Central Chapter 2 orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Sylvanwood Wilds region, Elderwood Grove settlement, Chapter 2 quest chain,
    /// NPC roster, enemy definitions, content additions, world evolution, presentation, and plugins.
    /// </summary>
    public partial class Chapter2Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public SecondRegionContent RegionContent { get; private set; } = new();
        public SecondSettlementContent SettlementContent { get; private set; } = new();
        public Chapter2QuestChain QuestChain { get; private set; } = new();
        public Chapter2NpcDefinitions NpcDefinitions { get; private set; } = new();
        public Chapter2EnemyDefinitions EnemyDefinitions { get; private set; } = new();
        public Chapter2ContentAdditions ContentAdditions { get; private set; } = new();
        public WorldEvolutionManager Evolution { get; private set; } = new();
        public Chapter2PresentationController Presentation { get; private set; } = new();

        private readonly List<IChapter2Plugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter2Manager: Initializing Chapter 2 Sylvanwood Wilds & Elderwood Grove content...");

            RegionContent.InitializeSecondRegion();
            SettlementContent.InitializeSecondSettlement();
            NpcDefinitions.RegisterDefaultChapter2NPCs();
            EnemyDefinitions.RegisterDefaultChapter2Enemies();
            ContentAdditions.RegisterChapter2Recipes();
            QuestChain.RegisterChapter2Quests();
            Presentation.ApplyCanopyLighting();
            Presentation.PlayElderwoodTheme();

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("Chapter2Manager: Chapter 2 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("Chapter2Manager: Shutting down chapter 2 framework...");
            _plugins.Clear();

            ServiceLocator.Unregister<Chapter2Manager>();
            _initialized = false;
        }

        public void RegisterPlugin(IChapter2Plugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }
}
