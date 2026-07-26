using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter14
{
    /// <summary>
    /// Central Chapter 14 & Final Boss Encounter orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Final Boss Definition (Arch-Sorcerer Malakor), Final Boss AI Engine, Final Boss Arena Manager,
    /// Chapter 14 Quest Chain, and Save V41 integration.
    /// </summary>
    public partial class Chapter14Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public FinalBossDefinition Boss { get; private set; } = new();
        public FinalBossAIEngine BossAI { get; private set; } = new();
        public FinalBossArenaManager Arena { get; private set; } = new();
        public Chapter14QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter14Manager: Initializing Chapter 14 — Final Boss Encounter & Multi-Phase Finale...");

            Boss.InitializePhases();
            BossAI.Initialize();
            Arena.Initialize();
            QuestChain.RegisterChapter14Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter14Manager: Chapter 14 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter14Manager: Shutting down Chapter 14 framework...");
            BossAI.Shutdown();
            Arena.Shutdown();
            ServiceLocator.Unregister<Chapter14Manager>();
            _initialized = false;
        }
    }
}
