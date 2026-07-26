using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter10
{
    /// <summary>
    /// Central Chapter 10 & Act III Finale orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Ancient Temple Content (Temple of Eternal Sun), Environmental Lore Manager, Temple Puzzle Sequences,
    /// Chapter 10 Quest Chain, and Save V37 integration.
    /// </summary>
    public partial class Chapter10Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public AncientTempleContent Temple { get; private set; } = new();
        public EnvironmentalLoreManager Lore { get; private set; } = new();
        public TemplePuzzleSequence Puzzles { get; private set; } = new();
        public Chapter10QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter10Manager: Initializing Chapter 10 — Ancient Temple Complex & Act III Finale...");

            Temple.InitializeChambers();
            Lore.Initialize();
            QuestChain.RegisterChapter10Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter10Manager: Chapter 10 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter10Manager: Shutting down Chapter 10 framework...");
            Lore.Shutdown();
            ServiceLocator.Unregister<Chapter10Manager>();
            _initialized = false;
        }
    }
}
