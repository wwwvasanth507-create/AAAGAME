using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter9
{
    /// <summary>
    /// Central Chapter 9 orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Corrupted Fortress Content, Antagonist Faction Manager (Shadow Legion), Fortress Commander Boss (General Vaelis),
    /// Chapter 9 Quest Chain, and Save V36 integration.
    /// </summary>
    public partial class Chapter9Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public CorruptedFortressContent Fortress { get; private set; } = new();
        public AntagonistFactionManager Faction { get; private set; } = new();
        public FortressCommanderBossDefinition BossVaelis { get; private set; } = new();
        public Chapter9QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter9Manager: Initializing Chapter 9 — Corrupted Fortress & Antagonist Faction...");

            Fortress.InitializeSectors();
            Faction.Initialize();
            BossVaelis.InitializeAbilities();
            QuestChain.RegisterChapter9Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter9Manager: Chapter 9 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter9Manager: Shutting down Chapter 9 framework...");
            Faction.Shutdown();
            ServiceLocator.Unregister<Chapter9Manager>();
            _initialized = false;
        }
    }
}
