using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.Settlement
{
    /// <summary>
    /// Data-driven settlement database.
    /// Loads settlement definitions from JSON and provides indexed lookups.
    /// Supports runtime registration and future DLC extension.
    /// </summary>
    public class SettlementDatabase
    {
        private readonly Dictionary<string, SettlementData> _settlements = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<SettlementType, List<SettlementData>> _settlementsByType = new();
        private readonly Dictionary<string, SettlementTypeDefinition> _typeDefinitions = new(StringComparer.OrdinalIgnoreCase);
        private bool _isLoaded = false;

        public bool IsLoaded => _isLoaded;
        public int SettlementCount => _settlements.Count;

        /// <summary>Load settlements from configuration.</summary>
        public void Load()
        {
            _settlements.Clear();
            _settlementsByType.Clear();

            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("settlement_database");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<SettlementData>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var settlement in list)
                        {
                            if (!string.IsNullOrEmpty(settlement.SettlementId))
                                RegisterSettlement(settlement);
                        }
                        Logger.Info($"SettlementDatabase: Loaded {_settlements.Count} settlements from config.");
                        _isLoaded = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SettlementDatabase: Config load exception: {ex.Message}");
            }

            Logger.Warning("SettlementDatabase: Using fallback settlement definitions.");
            LoadFallbackSettlements();
            _isLoaded = true;
        }

        /// <summary>Register a settlement at runtime.</summary>
        public void RegisterSettlement(SettlementData settlement)
        {
            if (settlement == null || string.IsNullOrEmpty(settlement.SettlementId)) return;

            _settlements[settlement.SettlementId] = settlement;

            if (!_settlementsByType.ContainsKey(settlement.Type))
                _settlementsByType[settlement.Type] = new List<SettlementData>();
            _settlementsByType[settlement.Type].Add(settlement);
        }

        /// <summary>Get a settlement by ID.</summary>
        public SettlementData? GetSettlement(string settlementId)
        {
            return _settlements.TryGetValue(settlementId, out var data) ? data : null;
        }

        /// <summary>Get all settlements.</summary>
        public List<SettlementData> GetAllSettlements()
        {
            return new List<SettlementData>(_settlements.Values);
        }

        /// <summary>Get settlements by type.</summary>
        public List<SettlementData> GetSettlementsByType(SettlementType type)
        {
            return _settlementsByType.TryGetValue(type, out var list)
                ? new List<SettlementData>(list)
                : new List<SettlementData>();
        }

        /// <summary>Get settlements in a region.</summary>
        public List<SettlementData> GetSettlementsByRegion(string region)
        {
            return _settlements.Values
                .Where(s => s.Region.Equals(region, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>Get settlements by biome.</summary>
        public List<SettlementData> GetSettlementsByBiome(string biome)
        {
            return _settlements.Values
                .Where(s => s.Biome.Equals(biome, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>Load type definitions from config or fallback.</summary>
        public void LoadTypeDefinitions()
        {
            _typeDefinitions.Clear();

            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("settlement_types");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<SettlementTypeDefinition>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var def in list)
                        {
                            _typeDefinitions[def.Type.ToString()] = def;
                        }
                        Logger.Info($"SettlementDatabase: Loaded {_typeDefinitions.Count} type definitions.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SettlementDatabase: Type definition load exception: {ex.Message}");
            }

            Logger.Warning("SettlementDatabase: Using fallback type definitions.");
            LoadFallbackTypeDefinitions();
        }

        /// <summary>Get type definition for a settlement type.</summary>
        public SettlementTypeDefinition? GetTypeDefinition(SettlementType type)
        {
            return _typeDefinitions.TryGetValue(type.ToString(), out var def) ? def : null;
        }

        /// <summary>Get all type definitions.</summary>
        public List<SettlementTypeDefinition> GetAllTypeDefinitions()
        {
            return new List<SettlementTypeDefinition>(_typeDefinitions.Values);
        }

        /// <summary>Register a custom type definition at runtime.</summary>
        public void RegisterTypeDefinition(SettlementTypeDefinition definition)
        {
            if (definition == null) return;
            _typeDefinitions[definition.Type.ToString()] = definition;
        }

        /// <summary>Search settlements by name (partial match).</summary>
        public List<SettlementData> SearchSettlements(string query)
        {
            if (string.IsNullOrEmpty(query)) return GetAllSettlements();
            return _settlements.Values
                .Where(s => s.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            s.SettlementId.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void LoadFallbackSettlements()
        {
            RegisterSettlement(new SettlementData
            {
                SettlementId = "village_harmony",
                DisplayName = "Harmony Village",
                Type = SettlementType.Village,
                Region = "eternia_valley",
                Biome = "temperate_forest",
                Population = 120,
                MaxPopulation = 300,
                Prosperity = ProsperityLevel.Stable,
                Security = SecurityRating.Low,
                PrimaryIndustries = new List<string> { "farming", "woodcutting" },
                PrimaryExports = new List<string> { "food_grain", "material_wood" },
                PrimaryImports = new List<string> { "material_iron_ore", "pot_minor_health" },
                Faction = "neutral",
                BuildingIds = new List<string> { "house_01", "house_02", "inn_01", "farm_01", "farm_02", "blacksmith_01", "market_01" },
                SettlementServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Blacksmith, ServiceType.InnRest },
                WorldPositionX = 100f,
                WorldPositionZ = 200f,
                MusicProfile = "village"
            });

            RegisterSettlement(new SettlementData
            {
                SettlementId = "town_haven",
                DisplayName = "Haven Town",
                Type = SettlementType.Town,
                Region = "eternia_valley",
                Biome = "temperate_forest",
                Population = 450,
                MaxPopulation = 1200,
                Prosperity = ProsperityLevel.Prosperous,
                Security = SecurityRating.Moderate,
                PrimaryIndustries = new List<string> { "crafting", "ironworking", "trade" },
                PrimaryExports = new List<string> { "armor_leather", "weapon_sword" },
                PrimaryImports = new List<string> { "food_fish", "gem_ruby", "pot_minor_health" },
                Faction = "eternia_crown",
                BuildingIds = new List<string> { "house_01", "house_02", "house_03", "inn_01", "blacksmith_01", "merchant_01", "market_01", "temple_01", "guard_barracks_01", "stables_01", "town_hall_01" },
                SettlementServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Blacksmith, ServiceType.InnRest, ServiceType.Temple, ServiceType.Stables, ServiceType.Training },
                WorldPositionX = 300f,
                WorldPositionZ = 150f,
                MusicProfile = "town"
            });

            RegisterSettlement(new SettlementData
            {
                SettlementId = "city_eternia",
                DisplayName = "Eternia City",
                Type = SettlementType.City,
                Region = "eternia_valley",
                Biome = "temperate_forest",
                Population = 2500,
                MaxPopulation = 8000,
                Prosperity = ProsperityLevel.Wealthy,
                Security = SecurityRating.High,
                PrimaryIndustries = new List<string> { "magic", "crafting", "trade", "governance" },
                PrimaryExports = new List<string> { "scroll_teleport", "gem_ruby", "potion_ultimate", "enchanted_weapon" },
                PrimaryImports = new List<string> { "food_grain", "material_wood", "material_iron_ore", "material_crystal" },
                Faction = "eternia_crown",
                BuildingIds = new List<string> { "house_01", "house_02", "house_03", "inn_01", "inn_02", "blacksmith_01", "blacksmith_02", "merchant_01", "merchant_02", "market_01", "market_02", "temple_01", "temple_02", "guard_barracks_01", "guard_barracks_02", "stables_01", "town_hall_01", "workshop_01", "library_01", "hospital_01" },
                SettlementServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Crafting, ServiceType.Blacksmith, ServiceType.InnRest, ServiceType.Temple, ServiceType.Stables, ServiceType.Training, ServiceType.Healing, ServiceType.Enchanting, ServiceType.Library },
                WorldPositionX = 500f,
                WorldPositionZ = 300f,
                MusicProfile = "city"
            });

            RegisterSettlement(new SettlementData
            {
                SettlementId = "port_brinewall",
                DisplayName = "Brinewall Port",
                Type = SettlementType.Port,
                Region = "coastal_plains",
                Biome = "coastal",
                Population = 800,
                MaxPopulation = 2000,
                Prosperity = ProsperityLevel.Stable,
                Security = SecurityRating.Moderate,
                PrimaryIndustries = new List<string> { "fishing", "shipbuilding", "trade" },
                PrimaryExports = new List<string> { "food_fish", "material_rope", "material_lumber" },
                PrimaryImports = new List<string> { "armor_leather", "weapon_sword", "gem_ruby" },
                Faction = "neutral",
                BuildingIds = new List<string> { "house_01", "house_02", "inn_01", "merchant_01", "market_01", "dock_01", "dock_02", "stables_01", "guard_barracks_01" },
                SettlementServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest, ServiceType.Dock, ServiceType.Stables },
                WorldPositionX = 800f,
                WorldPositionZ = 100f,
                MusicProfile = "port"
            });

            RegisterSettlement(new SettlementData
            {
                SettlementId = "mine_shadowpeak",
                DisplayName = "Shadowpeak Mining Camp",
                Type = SettlementType.MiningCamp,
                Region = "shadow_mountains",
                Biome = "mountain",
                Population = 60,
                MaxPopulation = 150,
                Prosperity = ProsperityLevel.Struggling,
                Security = SecurityRating.Minimal,
                PrimaryIndustries = new List<string> { "mining", "quarrying" },
                PrimaryExports = new List<string> { "material_iron_ore", "material_crystal_shard", "material_stone" },
                PrimaryImports = new List<string> { "food_grain", "material_wood" },
                Faction = "miners_guild",
                BuildingIds = new List<string> { "house_01", "inn_01", "merchant_01", "farm_01" },
                SettlementServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest },
                WorldPositionX = 700f,
                WorldPositionZ = 600f,
                MusicProfile = "mine"
            });

            RegisterSettlement(new SettlementData
            {
                SettlementId = "camp_wanderer",
                DisplayName = "Wanderer's Camp",
                Type = SettlementType.Camp,
                Region = "eternia_valley",
                Biome = "grasslands",
                Population = 25,
                MaxPopulation = 50,
                Prosperity = ProsperityLevel.Struggling,
                Security = SecurityRating.None,
                PrimaryIndustries = new List<string> { "hunting", "gathering" },
                PrimaryExports = new List<string> { "food_meat", "material_hide" },
                PrimaryImports = new List<string> { "pot_minor_health", "material_wood" },
                Faction = "neutral",
                BuildingIds = new List<string> { "inn_01" },
                SettlementServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest },
                WorldPositionX = 50f,
                WorldPositionZ = 50f,
                MusicProfile = "camp"
            });
        }

        private void LoadFallbackTypeDefinitions()
        {
            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Camp,
                DisplayName = "Camp",
                Description = "A temporary or semi-permanent camp. Minimal facilities.",
                MinPopulation = 5,
                MaxPopulation = 50,
                DefaultPopulation = 20,
                ProsperityFloor = 0f,
                ProsperityCeiling = 0.3f,
                MinBuildings = 1,
                MaxBuildings = 3,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Services, BuildingCategory.Storage },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest },
                SpawnDensity = 0.8f,
                HasMarket = false,
                HasInn = true,
                MusicProfile = "camp",
                SortOrder = 1
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Hamlet,
                DisplayName = "Hamlet",
                Description = "A small rural settlement with basic services.",
                MinPopulation = 20,
                MaxPopulation = 150,
                DefaultPopulation = 60,
                ProsperityFloor = 0f,
                ProsperityCeiling = 0.4f,
                MinBuildings = 2,
                MaxBuildings = 6,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Agricultural, BuildingCategory.Services, BuildingCategory.Storage },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest, ServiceType.Stables },
                SpawnDensity = 0.9f,
                HasMarket = false,
                HasInn = true,
                MusicProfile = "village",
                SortOrder = 2
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Village,
                DisplayName = "Village",
                Description = "A settled community with farms, basic trade, and a blacksmith.",
                MinPopulation = 50,
                MaxPopulation = 500,
                DefaultPopulation = 150,
                ProsperityFloor = 0.1f,
                ProsperityCeiling = 0.6f,
                MinBuildings = 3,
                MaxBuildings = 10,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Agricultural, BuildingCategory.Commercial, BuildingCategory.Services, BuildingCategory.Storage, BuildingCategory.Religious },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Blacksmith, ServiceType.InnRest, ServiceType.Stables },
                SpawnDensity = 1.0f,
                HasMarket = true,
                HasInn = true,
                HasTemple = true,
                MusicProfile = "village",
                SortOrder = 3
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Town,
                DisplayName = "Town",
                Description = "A prosperous town with walls, diverse services, and a local government.",
                MinPopulation = 200,
                MaxPopulation = 2000,
                DefaultPopulation = 600,
                ProsperityFloor = 0.2f,
                ProsperityCeiling = 0.8f,
                MinBuildings = 5,
                MaxBuildings = 20,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Commercial, BuildingCategory.Industrial, BuildingCategory.Agricultural, BuildingCategory.Civic, BuildingCategory.Military, BuildingCategory.Services, BuildingCategory.Storage, BuildingCategory.Religious, BuildingCategory.Entertainment, BuildingCategory.Medical },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Crafting, ServiceType.Blacksmith, ServiceType.InnRest, ServiceType.Stables, ServiceType.Temple, ServiceType.Training, ServiceType.Healing },
                SpawnDensity = 1.0f,
                HasWalls = true,
                HasMarket = true,
                HasInn = true,
                HasTemple = true,
                HasGuardBarracks = true,
                MusicProfile = "town",
                SortOrder = 4
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.City,
                DisplayName = "City",
                Description = "A major urban center with all services and strong defenses.",
                MinPopulation = 1000,
                MaxPopulation = 10000,
                DefaultPopulation = 3000,
                ProsperityFloor = 0.3f,
                ProsperityCeiling = 1.0f,
                MinBuildings = 10,
                MaxBuildings = 50,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Commercial, BuildingCategory.Industrial, BuildingCategory.Agricultural, BuildingCategory.Civic, BuildingCategory.Military, BuildingCategory.Religious, BuildingCategory.Services, BuildingCategory.Storage, BuildingCategory.Transportation, BuildingCategory.Entertainment, BuildingCategory.Educational, BuildingCategory.Medical },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Crafting, ServiceType.EquipmentRepair, ServiceType.Blacksmith, ServiceType.Enchanting, ServiceType.Alchemy, ServiceType.Healing, ServiceType.InnRest, ServiceType.Stables, ServiceType.Temple, ServiceType.Training, ServiceType.Library, ServiceType.TownHall, ServiceType.Banking },
                SpawnDensity = 1.2f,
                HasWalls = true,
                HasMarket = true,
                HasInn = true,
                HasTemple = true,
                HasGuardBarracks = true,
                MusicProfile = "city",
                SortOrder = 5
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Capital,
                DisplayName = "Capital",
                Description = "The grand capital city. Maximum population, all services, supreme defenses.",
                MinPopulation = 5000,
                MaxPopulation = 50000,
                DefaultPopulation = 10000,
                ProsperityFloor = 0.5f,
                ProsperityCeiling = 1.0f,
                MinBuildings = 20,
                MaxBuildings = 100,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Commercial, BuildingCategory.Industrial, BuildingCategory.Agricultural, BuildingCategory.Civic, BuildingCategory.Military, BuildingCategory.Religious, BuildingCategory.Services, BuildingCategory.Storage, BuildingCategory.Transportation, BuildingCategory.Entertainment, BuildingCategory.Educational, BuildingCategory.Medical },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Crafting, ServiceType.EquipmentRepair, ServiceType.Healing, ServiceType.InnRest, ServiceType.Storage, ServiceType.Training, ServiceType.Travel, ServiceType.Banking, ServiceType.Guild, ServiceType.Stables, ServiceType.Blacksmith, ServiceType.Enchanting, ServiceType.Alchemy, ServiceType.Library, ServiceType.Temple, ServiceType.Market, ServiceType.TownHall },
                SpawnDensity = 1.5f,
                HasWalls = true,
                HasMarket = true,
                HasInn = true,
                HasTemple = true,
                HasGuardBarracks = true,
                MusicProfile = "city",
                SortOrder = 6
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Fort,
                DisplayName = "Fort",
                Description = "A military fortification. Primarily defensive and training facilities.",
                MinPopulation = 50,
                MaxPopulation = 500,
                DefaultPopulation = 150,
                ProsperityFloor = 0.1f,
                ProsperityCeiling = 0.5f,
                MinBuildings = 3,
                MaxBuildings = 8,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Military, BuildingCategory.Storage, BuildingCategory.Services, BuildingCategory.Residential },
                DefaultServices = new List<ServiceType> { ServiceType.Training, ServiceType.Trading, ServiceType.EquipmentRepair, ServiceType.Stables },
                SpawnDensity = 1.0f,
                HasWalls = true,
                HasGuardBarracks = true,
                MusicProfile = "fort",
                SortOrder = 7
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Castle,
                DisplayName = "Castle",
                Description = "A noble's castle with attached settlement.",
                MinPopulation = 200,
                MaxPopulation = 2000,
                DefaultPopulation = 500,
                ProsperityFloor = 0.3f,
                ProsperityCeiling = 0.8f,
                MinBuildings = 5,
                MaxBuildings = 15,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Military, BuildingCategory.Civic, BuildingCategory.Services, BuildingCategory.Storage, BuildingCategory.Religious },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Training, ServiceType.Stables, ServiceType.Temple, ServiceType.TownHall },
                SpawnDensity = 0.8f,
                HasWalls = true,
                HasInn = true,
                HasTemple = true,
                HasGuardBarracks = true,
                MusicProfile = "castle",
                SortOrder = 8
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Port,
                DisplayName = "Port",
                Description = "A coastal trading port with docks and shipbuilding.",
                MinPopulation = 100,
                MaxPopulation = 3000,
                DefaultPopulation = 500,
                ProsperityFloor = 0.2f,
                ProsperityCeiling = 0.9f,
                MinBuildings = 4,
                MaxBuildings = 15,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Commercial, BuildingCategory.Industrial, BuildingCategory.Transportation, BuildingCategory.Services, BuildingCategory.Storage },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.Dock, ServiceType.InnRest, ServiceType.Stables, ServiceType.Storage, ServiceType.Travel },
                SpawnDensity = 1.1f,
                HasMarket = true,
                HasInn = true,
                MusicProfile = "port",
                SortOrder = 9
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.MiningCamp,
                DisplayName = "Mining Camp",
                Description = "A resource extraction camp focused on mining and quarrying.",
                MinPopulation = 20,
                MaxPopulation = 200,
                DefaultPopulation = 60,
                ProsperityFloor = 0f,
                ProsperityCeiling = 0.5f,
                MinBuildings = 2,
                MaxBuildings = 5,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Industrial, BuildingCategory.Storage, BuildingCategory.Services },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest, ServiceType.EquipmentRepair },
                SpawnDensity = 0.9f,
                HasInn = true,
                MusicProfile = "mine",
                SortOrder = 10
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.ForestOutpost,
                DisplayName = "Forest Outpost",
                Description = "A remote outpost in the wilderness for hunting and woodcutting.",
                MinPopulation = 10,
                MaxPopulation = 80,
                DefaultPopulation = 30,
                ProsperityFloor = 0f,
                ProsperityCeiling = 0.3f,
                MinBuildings = 1,
                MaxBuildings = 4,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Services, BuildingCategory.Storage },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest },
                SpawnDensity = 0.7f,
                HasInn = true,
                MusicProfile = "forest",
                SortOrder = 11
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.Temple,
                DisplayName = "Temple Settlement",
                Description = "A religious settlement centered around a temple or monastery.",
                MinPopulation = 30,
                MaxPopulation = 300,
                DefaultPopulation = 80,
                ProsperityFloor = 0.1f,
                ProsperityCeiling = 0.6f,
                MinBuildings = 2,
                MaxBuildings = 8,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Religious, BuildingCategory.Residential, BuildingCategory.Agricultural, BuildingCategory.Services, BuildingCategory.Educational },
                DefaultServices = new List<ServiceType> { ServiceType.Temple, ServiceType.Healing, ServiceType.Library, ServiceType.Training },
                SpawnDensity = 0.8f,
                HasTemple = true,
                MusicProfile = "temple",
                SortOrder = 12
            });

            RegisterTypeDefinition(new SettlementTypeDefinition
            {
                Type = SettlementType.NomadCamp,
                DisplayName = "Nomad Camp",
                Description = "A mobile camp of nomadic peoples.",
                MinPopulation = 15,
                MaxPopulation = 100,
                DefaultPopulation = 40,
                ProsperityFloor = 0f,
                ProsperityCeiling = 0.3f,
                MinBuildings = 1,
                MaxBuildings = 3,
                AllowedBuildingCategories = new List<BuildingCategory> { BuildingCategory.Residential, BuildingCategory.Services, BuildingCategory.Storage },
                DefaultServices = new List<ServiceType> { ServiceType.Trading, ServiceType.InnRest, ServiceType.Stables },
                SpawnDensity = 1.0f,
                HasInn = true,
                MusicProfile = "desert",
                SortOrder = 13
            });
        }
    }
}