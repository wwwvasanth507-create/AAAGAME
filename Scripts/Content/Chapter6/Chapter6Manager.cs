using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter6
{
    /// <summary>
    /// Central Chapter 6 orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Capital City (Eternia Prime), Guild System Expansion, Second Regional Boss (High Inquisitor Vesper),
    /// Chapter 6 Quest Chain, and Save V33 integration.
    /// </summary>
    public partial class Chapter6Manager : Node, IInitializable
    {
        private bool _initialized = false;

        public CapitalCityContent CapitalCity { get; private set; } = new();
        public GuildSystemManager Guilds { get; private set; } = new();
        public SecondRegionalBossDefinition BossVesper { get; private set; } = new();
        public Chapter6QuestChain QuestChain { get; private set; } = new();

        public bool IsInitialized => _initialized;

        public override void _Ready() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("Chapter6Manager: Initializing Chapter 6 — Capital City, Guild Systems & High Inquisitor Vesper...");

            CapitalCity.InitializeDistricts();
            Guilds.Initialize();
            BossVesper.InitializeAbilities();
            QuestChain.RegisterChapter6Quests();

            ServiceLocator.Register(this);

            Logger.Info("Chapter6Manager: Chapter 6 framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            Logger.Info("Chapter6Manager: Shutting down Chapter 6 framework...");
            Guilds.Shutdown();
            ServiceLocator.Unregister<Chapter6Manager>();
            _initialized = false;
        }
    }
}
