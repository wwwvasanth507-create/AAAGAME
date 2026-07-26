using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter12
{
    /// <summary>
    /// Central Chapter 12 & Alliance Campaign orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Alliance Campaign Manager, World War Event Manager, Solwarden Legendary Equipment Set,
    /// Chapter 12 Quest Chain, and Save V39 integration.
    /// </summary>
    public partial class Chapter12Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public AllianceCampaignManager Alliance { get; private set; } = new();
        public WorldWarEventManager WarEvents { get; private set; } = new();
        public LegendaryEquipmentSet LegendarySet { get; private set; } = new();
        public Chapter12QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter12Manager: Initializing Chapter 12 — Alliance Campaign, World War & Legendary Equipment...");

            Alliance.Initialize();
            WarEvents.Initialize();
            LegendarySet.InitializeSetPieces();
            QuestChain.RegisterChapter12Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter12Manager: Chapter 12 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter12Manager: Shutting down Chapter 12 framework...");
            Alliance.Shutdown();
            WarEvents.Shutdown();
            ServiceLocator.Unregister<Chapter12Manager>();
            _initialized = false;
        }
    }
}
