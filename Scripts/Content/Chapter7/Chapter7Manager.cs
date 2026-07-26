using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter7
{
    /// <summary>
    /// Central Chapter 7 & Act II Finale orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Regional Crisis Engine, Siege Battle Controller, Act II Finale Boss (Shadow Lord Emissary),
    /// Chapter 7 Quest Chain, World Aftermath, and Save V34 integration.
    /// </summary>
    public partial class Chapter7Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public RegionalCrisisManager Crisis { get; private set; } = new();
        public SiegeEncounterManager Siege { get; private set; } = new();
        public Act2FinaleBossDefinition FinaleBoss { get; private set; } = new();
        public Chapter7QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter7Manager: Initializing Chapter 7 — Act II Finale, Regional Crisis & Siege Battle...");

            Crisis.Initialize();
            FinaleBoss.InitializeAbilities();
            QuestChain.RegisterChapter7Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter7Manager: Act II Finale framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter7Manager: Shutting down Act II Finale framework...");
            Crisis.Shutdown();
            ServiceLocator.Unregister<Chapter7Manager>();
            _initialized = false;
        }
    }
}
