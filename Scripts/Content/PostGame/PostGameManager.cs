using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.PostGame
{
    /// <summary>
    /// Central Post-Game Framework, Super Bosses & Endgame Exploration orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Super Boss Framework, 100% Completion System Manager, Post-Game Quest Chain, and Save V43 integration.
    /// </summary>
    public partial class PostGameManager : Node, IInitializable
    {
        private bool _initialized = false;

        public SuperBossFramework SuperBosses { get; private set; } = new();
        public CompletionSystemManager Completion { get; private set; } = new();
        public PostGameQuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("PostGameManager: Initializing Post-Game Framework, Super Bosses & Endgame Exploration...");

            SuperBosses.Initialize();
            Completion.Initialize();
            QuestChain.RegisterPostGameQuests();

            ServiceLocator.Register(this);

            Logger.Info("PostGameManager: Post-Game framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("PostGameManager: Shutting down Post-Game framework...");
            SuperBosses.Shutdown();
            Completion.Shutdown();
            ServiceLocator.Unregister<PostGameManager>();
            _initialized = false;
        }
    }
}
