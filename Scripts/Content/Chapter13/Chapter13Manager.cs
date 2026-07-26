using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter13
{
    /// <summary>
    /// Central Chapter 13 & Final Dungeon orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Final Dungeon Content (The Citadel of Obsidian Void), Dungeon Checkpoint Network, Pre-Final Encounter Definitions,
    /// Chapter 13 Quest Chain, and Save V40 integration.
    /// </summary>
    public partial class Chapter13Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public FinalDungeonContent Dungeon { get; private set; } = new();
        public DungeonCheckpointNetwork Checkpoints { get; private set; } = new();
        public PreFinalEncounterDefinitions Encounters { get; private set; } = new();
        public Chapter13QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter13Manager: Initializing Chapter 13 — Final Dungeon & Pre-Final Encounters...");

            Dungeon.InitializeSectors();
            Checkpoints.Initialize();
            Encounters.InitializeEncounters();
            QuestChain.RegisterChapter13Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter13Manager: Chapter 13 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter13Manager: Shutting down Chapter 13 framework...");
            Checkpoints.Shutdown();
            ServiceLocator.Unregister<Chapter13Manager>();
            _initialized = false;
        }
    }
}
