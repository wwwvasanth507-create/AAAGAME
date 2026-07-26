using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter5
{
    /// <summary>
    /// Central Chapter 5 orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Branching Story Framework, Faction Dungeon, Chapter 5 Quest Chain, World Consequence Manager, and Save V32 integration.
    /// </summary>
    public partial class Chapter5Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public BranchingStoryFramework StoryFramework { get; private set; } = new();
        public FactionDungeonContent FactionDungeon { get; private set; } = new();
        public Chapter5QuestChain QuestChain { get; private set; } = new();
        public WorldConsequenceManager ConsequenceManager { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter5Manager: Initializing Chapter 5 — Branching Storylines & Faction Dungeon...");

            FactionDungeon.InitializeRooms();
            QuestChain.RegisterChapter5Quests();
            ConsequenceManager.Initialize();

            ServiceLocator.Register(this);

            Logger.Info("Chapter5Manager: Chapter 5 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter5Manager: Shutting down Chapter 5 framework...");
            ConsequenceManager.Shutdown();
            ServiceLocator.Unregister<Chapter5Manager>();
            _initialized = false;
        }

        public void SetActiveBranch(StoryBranchId branch, string factionId)
        {
            StoryFramework.SelectBranch(branch, factionId);
            Logger.Info($"Chapter5Manager: Active branch set to '{branch}' with faction '{factionId}'.");
        }
    }
}
