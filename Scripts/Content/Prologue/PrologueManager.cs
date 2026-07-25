using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Prologue
{
    public interface IProloguePlugin
    {
        string PluginName { get; }
        void OnTutorialStepCompleted(TutorialStep step);
    }

    /// <summary>
    /// Central Prologue & Chapter 1 orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Oakvale starting region, tutorial flow, starter NPCs, Chapter 1 quest chain,
    /// starter enemies, equipment, exploration nodes, and presentation.
    /// </summary>
    public partial class PrologueManager : Node, IInitializable
    {
        private bool _initialized = false;

        public StartingRegionContent RegionContent { get; private set; } = new();
        public IntroductionFlowManager IntroFlow { get; private set; } = new();
        public StarterNpcDefinitions NpcDefinitions { get; private set; } = new();
        public Chapter1QuestChain QuestChain { get; private set; } = new();
        public StarterEnemyDefinitions EnemyDefinitions { get; private set; } = new();
        public StarterEquipmentDefinitions EquipmentDefinitions { get; private set; } = new();
        public StarterExplorationContent ExplorationContent { get; private set; } = new();
        public StarterPresentationController Presentation { get; private set; } = new();

        private readonly List<IProloguePlugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("PrologueManager: Initializing Prologue & Chapter 1 content...");

            RegionContent.InitializeStartingRegion();
            NpcDefinitions.RegisterDefaultStarterNPCs();
            EnemyDefinitions.RegisterDefaultStarterEnemies();
            EquipmentDefinitions.RegisterDefaultItems();
            ExplorationContent.RegisterDefaultExplorationContent();
            Presentation.ApplyMorningLighting();
            Presentation.PlayVillageTheme();

            QuestChain.RegisterChapter1Quests();

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("PrologueManager: Prologue & Chapter 1 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("PrologueManager: Shutting down prologue framework...");
            _plugins.Clear();

            ServiceLocator.Unregister<PrologueManager>();
            _initialized = false;
        }

        public void RegisterPlugin(IProloguePlugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }
}
