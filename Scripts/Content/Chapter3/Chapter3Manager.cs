using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter3
{
    public interface IChapter3Plugin
    {
        string PluginName { get; }
        void OnActICompleted();
    }

    /// <summary>
    /// Central Chapter 3 and Act I Finale orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates the Citadel of Void Shadows dungeon, regional boss, quest chain,
    /// faction escalation, world evolution consequences, rewards, and presentation.
    /// </summary>
    public partial class Chapter3Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public FirstDungeonContent Dungeon { get; private set; } = new();
        public RegionalBossDefinition Boss { get; private set; } = new();
        public Chapter3QuestChain QuestChain { get; private set; } = new();
        public FactionEscalationManager FactionEscalation { get; private set; } = new();
        public ActIWorldEvolution WorldEvolution { get; private set; } = new();
        public Chapter3Rewards Rewards { get; private set; } = new();
        public Chapter3PresentationController Presentation { get; private set; } = new();

        private readonly List<IChapter3Plugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter3Manager: Initializing Chapter 3 — Citadel of Void Shadows & Act I Finale...");

            Dungeon.InitializeDungeon();
            Boss.RegisterAbilities();
            FactionEscalation.InitializeFactions();
            WorldEvolution.InitializeActIEvolution();
            Rewards.RegisterTier2Rewards();
            QuestChain.RegisterChapter3Quests();
            Presentation.ApplyDungeonLighting();
            Presentation.PlayDungeonAmbience();

            ServiceLocator.Register(this);

            Logger.Info("Chapter3Manager: Chapter 3 & Act I Finale framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter3Manager: Shutting down Chapter 3 framework...");
            _plugins.Clear();
            ServiceLocator.Unregister<Chapter3Manager>();
            _initialized = false;
        }

        public void RegisterPlugin(IChapter3Plugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
                _plugins.Add(plugin);
        }

        public void OnActICompleted()
        {
            Logger.Info("Chapter3Manager: Act I completed — triggering world evolution events...");

            WorldEvolution.UnlockEvent("evt_citadel_sealed");
            WorldEvolution.UnlockEvent("evt_oakvale_celebration");
            WorldEvolution.UnlockEvent("evt_sylvan_alliance_formal");
            WorldEvolution.UnlockEvent("evt_shadow_cult_retreat");
            WorldEvolution.UnlockEvent("evt_tier2_merchants");

            FactionEscalation.EscalateFactionRelation("faction_shadow_cult", FactionRelation.AtWar);

            foreach (var plugin in _plugins)
                plugin.OnActICompleted();

            Presentation.PlayActIVictoryFanfare();
        }
    }
}
