using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public interface IAct2Plugin
    {
        string PluginName { get; }
        void OnAct2Opened();
    }

    /// <summary>
    /// Central Act II orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Eastern Ridgeline and Mirkwood Swamps regions, Valenhold City, Faction Politics,
    /// Advanced Exploration, companion registration, quest chain, enemy roster, crafting, and presentation.
    /// </summary>
    public partial class Act2Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public Act2RegionContent Regions { get; private set; } = new();
        public CompanionRegistry Companions { get; private set; } = new();
        public Act2QuestChain QuestChain { get; private set; } = new();
        public Act2EnemyDefinitions Enemies { get; private set; } = new();
        public Act2NpcDefinitions Npcs { get; private set; } = new();
        public Act2CraftingContent Crafting { get; private set; } = new();
        public ValenholdCityContent ValenholdCity { get; private set; } = new();
        public FactionPoliticsManager PoliticsManager { get; private set; } = new();
        public AdvancedExplorationManager ExplorationManager { get; private set; } = new();

        private readonly List<IAct2Plugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Act2Manager: Initializing Act II — Eastern Ridgeline, Mirkwood Swamps & Valenhold Metropolis...");

            Regions.InitializeRegions();
            Companions.RegisterCompanions();
            QuestChain.RegisterAct2Quests();
            Enemies.RegisterEnemies();
            Npcs.RegisterNpcs();
            Crafting.RegisterCraftingContent();
            ValenholdCity.InitializeDistricts();
            PoliticsManager.Initialize();
            ExplorationManager.Initialize();

            ServiceLocator.Register(this);

            Logger.Info("Act2Manager: Act II framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Act2Manager: Shutting down Act II framework...");
            _plugins.Clear();
            PoliticsManager.Shutdown();
            ExplorationManager.Shutdown();
            ServiceLocator.Unregister<Act2Manager>();
            _initialized = false;
        }

        public void RegisterPlugin(IAct2Plugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
                _plugins.Add(plugin);
        }

        public void OnAct2Opened()
        {
            Logger.Info("Act2Manager: Act II opened — unlocking Eastern Ridgeline & Valenhold...");
            Regions.UnlockRegion("region_eastern_ridgeline");
            foreach (var plugin in _plugins)
                plugin.OnAct2Opened();
        }

        public void OnMirkwoodUnlocked()
        {
            Logger.Info("Act2Manager: Mirkwood Swamps unlocked after Watchtower liberation.");
            Regions.UnlockRegion("region_mirkwood_swamps");
        }
    }
}
