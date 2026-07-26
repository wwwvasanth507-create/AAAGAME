using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter8
{
    /// <summary>
    /// Central Chapter 8 & Act III Begins orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Shadow Frontier content, Traversal System Manager, Shadow Frontier Enemies,
    /// Chapter 8 Quest Chain, and Save V35 integration.
    /// </summary>
    public partial class Chapter8Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public ShadowFrontierContent Frontier { get; private set; } = new();
        public TraversalSystemManager Traversal { get; private set; } = new();
        public ShadowFrontierEnemies Enemies { get; private set; } = new();
        public Chapter8QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter8Manager: Initializing Act III — Chapter 8, Shadow Frontier & Advanced Traversal...");

            Frontier.InitializeZones();
            Traversal.Initialize();
            QuestChain.RegisterChapter8Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter8Manager: Chapter 8 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter8Manager: Shutting down Chapter 8 framework...");
            Traversal.Shutdown();
            ServiceLocator.Unregister<Chapter8Manager>();
            _initialized = false;
        }
    }
}
