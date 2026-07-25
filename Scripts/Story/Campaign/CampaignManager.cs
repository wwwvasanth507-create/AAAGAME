using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Story.Campaign
{
    public interface ICampaignPlugin
    {
        string PluginName { get; }
        void OnActStarted(string actId);
    }

    /// <summary>
    /// Central campaign design and narrative blueprint orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates world regions, character profiles, villain registries, campaign outline acts,
    /// and plugin extensions.
    /// </summary>
    public partial class CampaignManager : Node, IInitializable
    {
        private bool _initialized = false;

        public RegionDatabase Regions { get; private set; } = new();
        public CharacterDatabase Characters { get; private set; } = new();
        public VillainDatabase Villains { get; private set; } = new();
        public CampaignDatabase Campaign { get; private set; } = new();

        private readonly List<ICampaignPlugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("CampaignManager: Initializing campaign design framework...");

            Regions.RegisterDefaultRegions();
            Characters.RegisterDefaultCharacters();
            Villains.RegisterDefaultVillains();
            Campaign.RegisterDefaultCampaign();

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("CampaignManager: Campaign design framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("CampaignManager: Shutting down campaign framework...");
            _plugins.Clear();

            ServiceLocator.Unregister<CampaignManager>();
            _initialized = false;
        }

        public void RegisterPlugin(ICampaignPlugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }
}
