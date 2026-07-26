using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter11
{
    /// <summary>
    /// Central Chapter 11 & Act IV Begins orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Endgame Region Content (The Astral Divide), Legendary Progression Manager, Elite World Content Manager,
    /// Chapter 11 Quest Chain, and Save V38 integration.
    /// </summary>
    public partial class Chapter11Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public EndgameRegionContent Region { get; private set; } = new();
        public LegendaryProgressionManager Legendary { get; private set; } = new();
        public EliteWorldContentManager EliteContent { get; private set; } = new();
        public Chapter11QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter11Manager: Initializing Chapter 11 — Act IV Begins: Endgame Region & Legendary Progression...");

            Region.InitializeZones();
            Legendary.Initialize();
            EliteContent.Initialize();
            QuestChain.RegisterChapter11Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter11Manager: Chapter 11 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter11Manager: Shutting down Chapter 11 framework...");
            Legendary.Shutdown();
            EliteContent.Shutdown();
            ServiceLocator.Unregister<Chapter11Manager>();
            _initialized = false;
        }
    }
}
