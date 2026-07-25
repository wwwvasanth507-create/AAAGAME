using System;
using System.Collections.Generic;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Definition record for a single static world element (Tree, Rock, Ore Node, etc.)
    /// </summary>
    public class WorldElementRecord
    {
        public string Id { get; set; } = "";
        public string Category { get; set; } = ""; // "Tree", "Ore", "Rock", "Vegetation", "Structure"
        public string DisplayName { get; set; } = "";
        public string ModelPath { get; set; } = "";
        public string MaterialPath { get; set; } = "";
        public float BaseSpawnWeight { get; set; } = 1.0f;
        
        // Custom variables (e.g. durability capacity, drop table references)
        public Dictionary<string, string> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Indexes and queries database records for environment elements.
    /// </summary>
    public class WorldDatabase : IInitializable
    {
        private readonly Dictionary<string, WorldElementRecord> _elements = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<BiomeType, BiomeDefinition> _biomes = new();

        public void Initialize()
        {
            LoadBiomes();
            LoadWorldElements();
        }

        private void LoadBiomes()
        {
            _biomes.Clear();
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("biomes");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<BiomeDefinition>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                      {
                        foreach (var item in list)
                        {
                            _biomes[item.Type] = item;
                        }
                        Logger.Info($"WorldDatabase: Loaded {_biomes.Count} Biome definitions.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"WorldDatabase: Biome load exception: {ex.Message}");
            }

            PopulateFallbackBiomes();
        }

        private void LoadWorldElements()
        {
            _elements.Clear();
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("world_database");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<WorldElementRecord>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            if (!string.IsNullOrEmpty(item.Id))
                            {
                                _elements[item.Id] = item;
                            }
                        }
                        Logger.Info($"WorldDatabase: Loaded {_elements.Count} world element records.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"WorldDatabase: World element loading exception: {ex.Message}");
            }

            PopulateFallbackElements();
        }

        public WorldElementRecord? GetRecord(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _elements.TryGetValue(id, out var record) ? record : null;
        }

        public List<WorldElementRecord> GetByCategory(string category)
        {
            var list = new List<WorldElementRecord>();
            foreach (var r in _elements.Values)
            {
                if (string.Equals(r.Category, category, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(r);
                }
            }
            return list;
        }

        public BiomeDefinition? GetBiome(BiomeType type)
        {
            return _biomes.TryGetValue(type, out var def) ? def : null;
        }

        public List<BiomeDefinition> GetAllBiomes()
        {
            return new List<BiomeDefinition>(_biomes.Values);
        }

        private void PopulateFallbackBiomes()
        {
            Logger.Warning("WorldDatabase: Populating fallback biomes.");
            foreach (BiomeType bType in Enum.GetValues(typeof(BiomeType)))
            {
                _biomes[bType] = new BiomeDefinition
                {
                    Type = bType,
                    Name = bType.ToString(),
                    Temperature = 0.5f,
                    Humidity = 0.5f,
                    MinElevation = 0.0f,
                    MaxElevation = 1.0f,
                    TerrainType = "Flat",
                    SkyProfile = "res://Materials/SkyDefault.tres",
                    WeatherProfile = "Clear"
                };
            }
        }

        private void PopulateFallbackElements()
        {
            Logger.Warning("WorldDatabase: Populating fallback elements.");
            _elements["tree_oak"] = new WorldElementRecord
            {
                Id = "tree_oak",
                Category = "Tree",
                DisplayName = "Oak Tree",
                ModelPath = "res://Assets/Environment/Meshes/tree_oak.glb",
                BaseSpawnWeight = 10f
            };
            _elements["rock_granite"] = new WorldElementRecord
            {
                Id = "rock_granite",
                Category = "Rock",
                DisplayName = "Granite Rock",
                ModelPath = "res://Assets/Environment/Meshes/rock_granite.glb",
                BaseSpawnWeight = 5f
            };
            _elements["ore_iron"] = new WorldElementRecord
            {
                Id = "ore_iron",
                Category = "Ore",
                DisplayName = "Iron Ore Vein",
                ModelPath = "res://Assets/Environment/Meshes/ore_iron.glb",
                BaseSpawnWeight = 2f
            };
        }
    }
}
