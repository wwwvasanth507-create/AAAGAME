using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.World.Content
{
    public interface IWorldContentPlugin
    {
        string PluginName { get; }
        void OnPoiSpawned(POISpawnInstance poiInstance);
    }

    /// <summary>
    /// Central world content orchestrator implementing <see cref="IInitializable"/>.
    /// Coordinates Points of Interest, landmarks, procedural decoration, dungeon graphs,
    /// exploration tracking, and streaming compatibility.
    /// </summary>
    public partial class WorldContentManager : Node, IInitializable
    {
        private bool _initialized = false;

        public PointOfInterestDatabase POIDatabase { get; private set; } = new();
        public WorldGenerationRules PlacementRules { get; private set; } = new();
        public LandmarkDatabase LandmarkDatabase { get; private set; } = new();
        public DungeonFramework DungeonFramework { get; private set; } = new();
        public ExplorationManager ExplorationManager { get; private set; } = new();
        public WorldDecorationSystem DecorationSystem { get; private set; } = new();
        public RegionalVariationManager RegionalVariation { get; private set; } = new();

        private readonly List<POISpawnInstance> _spawnedPois = new();
        private readonly List<IWorldContentPlugin> _plugins = new();

        public bool IsInitialized => _initialized;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("WorldContentManager: Initializing world content framework...");

            POIDatabase.RegisterDefaultPOIs();
            LandmarkDatabase.RegisterDefaultLandmarks();

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("WorldContentManager: World content framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("WorldContentManager: Shutting down world content framework...");
            _spawnedPois.Clear();
            _plugins.Clear();

            ServiceLocator.Unregister<WorldContentManager>();
            _initialized = false;
        }

        public bool TrySpawnPOI(string poiId, PlacementValidationContext context, out POISpawnInstance? spawn)
        {
            spawn = null;
            var poiDef = POIDatabase.GetPOI(poiId);
            if (poiDef == null) return false;

            if (PlacementRules.ValidatePOIPosition(poiDef, context, _spawnedPois))
            {
                spawn = new POISpawnInstance
                {
                    PoiId = poiId,
                    WorldPosition = context.TargetPosition,
                    RotationDegreesY = 0.0f
                };

                _spawnedPois.Add(spawn);

                foreach (var plugin in _plugins)
                {
                    plugin.OnPoiSpawned(spawn);
                }
                return true;
            }
            return false;
        }

        public IReadOnlyList<POISpawnInstance> SpawnedPOIs => _spawnedPois;

        public void RegisterPlugin(IWorldContentPlugin plugin)
        {
            if (plugin != null && !_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }
    }
}
