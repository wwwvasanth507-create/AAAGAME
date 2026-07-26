using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter15
{
    /// <summary>
    /// Central Chapter 15, Ending, Epilogue & Campaign Completion orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Ending Sequence Manager, Credits System Manager, Campaign Completion Tracker, Chapter 15 Quest Chain, and Save V42 integration.
    /// </summary>
    public partial class Chapter15Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public EndingSequenceManager Ending { get; private set; } = new();
        public CreditsSystemManager Credits { get; private set; } = new();
        public CampaignCompletionTracker Completion { get; private set; } = new();
        public Chapter15QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter15Manager: Initializing Chapter 15 — Ending, Epilogue & Campaign Completion...");

            Ending.Initialize();
            Credits.Initialize();
            Completion.Initialize();
            QuestChain.RegisterChapter15Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter15Manager: Chapter 15 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter15Manager: Shutting down Chapter 15 framework...");
            Ending.Shutdown();
            Credits.Shutdown();
            Completion.Shutdown();
            ServiceLocator.Unregister<Chapter15Manager>();
            _initialized = false;
        }
    }
}
