using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.Settlement
{
    /// <summary>
    /// Data-driven building database.
    /// Loads building definitions from JSON and provides indexed lookups.
    /// Supports runtime registration and future DLC extension.
    /// </summary>
    public class BuildingDatabase
    {
        private readonly Dictionary<string, BuildingData> _buildings = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<BuildingCategory, List<BuildingData>> _buildingsByCategory = new();
        private bool _isLoaded = false;

        public bool IsLoaded => _isLoaded;
        public int BuildingCount => _buildings.Count;

        /// <summary>Load buildings from configuration.</summary>
        public void Load()
        {
            _buildings.Clear();
            _buildingsByCategory.Clear();

            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("building_database");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<BuildingData>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var building in list)
                        {
                            if (!string.IsNullOrEmpty(building.BuildingId))
                                RegisterBuilding(building);
                        }
                        Logger.Info($"BuildingDatabase: Loaded {_buildings.Count} buildings from config.");
                        _isLoaded = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"BuildingDatabase: Config load exception: {ex.Message}");
            }

            Logger.Warning("BuildingDatabase: Using fallback building definitions.");
            LoadFallbackBuildings();
            _isLoaded = true;
        }

        /// <summary>Register a building at runtime.</summary>
        public void RegisterBuilding(BuildingData building)
        {
            if (building == null || string.IsNullOrEmpty(building.BuildingId)) return;

            _buildings[building.BuildingId] = building;

            if (!_buildingsByCategory.ContainsKey(building.Category))
                _buildingsByCategory[building.Category] = new List<BuildingData>();
            _buildingsByCategory[building.Category].Add(building);
        }

        /// <summary>Get a building by ID.</summary>
        public BuildingData? GetBuilding(string buildingId)
        {
            return _buildings.TryGetValue(buildingId, out var data) ? data : null;
        }

        /// <summary>Get all buildings.</summary>
        public List<BuildingData> GetAllBuildings()
        {
            return new List<BuildingData>(_buildings.Values);
        }

        /// <summary>Get buildings by category.</summary>
        public List<BuildingData> GetBuildingsByCategory(BuildingCategory category)
        {
            return _buildingsByCategory.TryGetValue(category, out var list)
                ? new List<BuildingData>(list)
                : new List<BuildingData>();
        }

        /// <summary>Get buildings that provide a specific service.</summary>
        public List<BuildingData> GetBuildingsByService(ServiceType service)
        {
            return _buildings.Values
                .Where(b => b.Services.Contains(service))
                .ToList();
        }

        /// <summary>Get buildings suitable for a settlement type.</summary>
        public List<BuildingData> GetBuildingsForSettlementType(SettlementType settlementType)
        {
            return _buildings.Values
                .Where(b => b.MinSettlementType <= settlementType)
                .ToList();
        }

        /// <summary>Get default buildings for a settlement type.</summary>
        public List<BuildingData> GetDefaultBuildings()
        {
            return _buildings.Values
                .Where(b => b.IsDefault)
                .ToList();
        }

        /// <summary>Search buildings by name (partial match).</summary>
        public List<BuildingData> SearchBuildings(string query)
        {
            if (string.IsNullOrEmpty(query)) return GetAllBuildings();
            return _buildings.Values
                .Where(b => b.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            b.BuildingId.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void LoadFallbackBuildings()
        {
            // Residential
            RegisterBuilding(new BuildingData
            {
                BuildingId = "house_01",
                DisplayName = "Small House",
                Category = BuildingCategory.Residential,
                NpcCapacity = 3,
                IsDefault = true,
                MinSettlementType = SettlementType.Camp,
                DailyMaintenanceCost = 1,
                DailyRevenue = 0
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "house_02",
                DisplayName = "Medium House",
                Category = BuildingCategory.Residential,
                NpcCapacity = 5,
                IsDefault = true,
                MinSettlementType = SettlementType.Hamlet,
                DailyMaintenanceCost = 2,
                DailyRevenue = 0
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "house_03",
                DisplayName = "Large House",
                Category = BuildingCategory.Residential,
                NpcCapacity = 8,
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 3,
                DailyRevenue = 0
            });

            // Commercial
            RegisterBuilding(new BuildingData
            {
                BuildingId = "inn_01",
                DisplayName = "Inn",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 10,
                OpenTime = 0.0f,
                CloseTime = 1.0f,
                Services = new List<ServiceType> { ServiceType.InnRest, ServiceType.Trading },
                IsDefault = true,
                MinSettlementType = SettlementType.Camp,
                DailyMaintenanceCost = 5,
                DailyRevenue = 15,
                UpgradeCosts = new List<int> { 100, 250, 500 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "inn_02",
                DisplayName = "Tavern",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 20,
                OpenTime = 0.1f,
                CloseTime = 0.95f,
                Services = new List<ServiceType> { ServiceType.InnRest, ServiceType.Trading },
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 8,
                DailyRevenue = 25,
                UpgradeCosts = new List<int> { 200, 500, 1000 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "blacksmith_01",
                DisplayName = "Blacksmith",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 3,
                OpenTime = 0.25f,
                CloseTime = 0.80f,
                Services = new List<ServiceType> { ServiceType.Blacksmith, ServiceType.EquipmentRepair, ServiceType.Crafting },
                IsDefault = true,
                MinSettlementType = SettlementType.Village,
                DailyMaintenanceCost = 5,
                DailyRevenue = 20,
                UpgradeCosts = new List<int> { 150, 300, 600 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "blacksmith_02",
                DisplayName = "Master Blacksmith",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 5,
                OpenTime = 0.25f,
                CloseTime = 0.80f,
                Services = new List<ServiceType> { ServiceType.Blacksmith, ServiceType.EquipmentRepair, ServiceType.Crafting, ServiceType.Enchanting },
                IsDefault = true,
                MinSettlementType = SettlementType.City,
                DailyMaintenanceCost = 10,
                DailyRevenue = 40,
                UpgradeCosts = new List<int> { 500, 1000, 2000 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "merchant_01",
                DisplayName = "Merchant Shop",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 3,
                OpenTime = 0.25f,
                CloseTime = 0.85f,
                Services = new List<ServiceType> { ServiceType.Trading },
                IsDefault = true,
                MinSettlementType = SettlementType.Hamlet,
                DailyMaintenanceCost = 3,
                DailyRevenue = 12,
                UpgradeCosts = new List<int> { 100, 200, 400 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "merchant_02",
                DisplayName = "General Store",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 5,
                OpenTime = 0.25f,
                CloseTime = 0.85f,
                Services = new List<ServiceType> { ServiceType.Trading, ServiceType.Storage },
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 5,
                DailyRevenue = 20,
                UpgradeCosts = new List<int> { 200, 400, 800 }
            });

            // Agricultural
            RegisterBuilding(new BuildingData
            {
                BuildingId = "farm_01",
                DisplayName = "Small Farm",
                Category = BuildingCategory.Agricultural,
                NpcCapacity = 2,
                OpenTime = 0.20f,
                CloseTime = 0.80f,
                IsDefault = true,
                MinSettlementType = SettlementType.Camp,
                DailyMaintenanceCost = 2,
                DailyRevenue = 8,
                UpgradeCosts = new List<int> { 50, 100, 200 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "farm_02",
                DisplayName = "Large Farm",
                Category = BuildingCategory.Agricultural,
                NpcCapacity = 4,
                OpenTime = 0.20f,
                CloseTime = 0.80f,
                IsDefault = true,
                MinSettlementType = SettlementType.Village,
                DailyMaintenanceCost = 4,
                DailyRevenue = 16,
                UpgradeCosts = new List<int> { 100, 200, 400 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "mill_01",
                DisplayName = "Windmill",
                Category = BuildingCategory.Agricultural,
                NpcCapacity = 2,
                OpenTime = 0.25f,
                CloseTime = 0.75f,
                IsDefault = true,
                MinSettlementType = SettlementType.Village,
                DailyMaintenanceCost = 3,
                DailyRevenue = 10,
                UpgradeCosts = new List<int> { 150, 300 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "bakery_01",
                DisplayName = "Bakery",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 2,
                OpenTime = 0.20f,
                CloseTime = 0.70f,
                Services = new List<ServiceType> { ServiceType.Trading },
                IsDefault = true,
                MinSettlementType = SettlementType.Village,
                DailyMaintenanceCost = 3,
                DailyRevenue = 12,
                UpgradeCosts = new List<int> { 80, 160, 320 }
            });

            // Civic
            RegisterBuilding(new BuildingData
            {
                BuildingId = "town_hall_01",
                DisplayName = "Town Hall",
                Category = BuildingCategory.Civic,
                NpcCapacity = 10,
                OpenTime = 0.25f,
                CloseTime = 0.75f,
                Services = new List<ServiceType> { ServiceType.TownHall },
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 10,
                DailyRevenue = 0,
                UpgradeCosts = new List<int> { 500, 1000, 2000 }
            });

            // Military
            RegisterBuilding(new BuildingData
            {
                BuildingId = "guard_barracks_01",
                DisplayName = "Guard Barracks",
                Category = BuildingCategory.Military,
                NpcCapacity = 10,
                OpenTime = 0.0f,
                CloseTime = 1.0f,
                Services = new List<ServiceType> { ServiceType.Training },
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 8,
                DailyRevenue = 0,
                UpgradeCosts = new List<int> { 200, 400, 800 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "guard_barracks_02",
                DisplayName = "Large Barracks",
                Category = BuildingCategory.Military,
                NpcCapacity = 25,
                OpenTime = 0.0f,
                CloseTime = 1.0f,
                Services = new List<ServiceType> { ServiceType.Training },
                IsDefault = true,
                MinSettlementType = SettlementType.City,
                DailyMaintenanceCost = 15,
                DailyRevenue = 0,
                UpgradeCosts = new List<int> { 500, 1000, 2000 }
            });

            // Religious
            RegisterBuilding(new BuildingData
            {
                BuildingId = "temple_01",
                DisplayName = "Temple",
                Category = BuildingCategory.Religious,
                NpcCapacity = 15,
                OpenTime = 0.15f,
                CloseTime = 0.85f,
                Services = new List<ServiceType> { ServiceType.Temple, ServiceType.Healing },
                IsDefault = true,
                MinSettlementType = SettlementType.Village,
                DailyMaintenanceCost = 5,
                DailyRevenue = 5,
                UpgradeCosts = new List<int> { 300, 600, 1200 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "temple_02",
                DisplayName = "Cathedral",
                Category = BuildingCategory.Religious,
                NpcCapacity = 40,
                OpenTime = 0.10f,
                CloseTime = 0.90f,
                Services = new List<ServiceType> { ServiceType.Temple, ServiceType.Healing, ServiceType.Library },
                IsDefault = true,
                MinSettlementType = SettlementType.City,
                DailyMaintenanceCost = 12,
                DailyRevenue = 10,
                UpgradeCosts = new List<int> { 800, 1600, 3200 }
            });

            // Medical
            RegisterBuilding(new BuildingData
            {
                BuildingId = "hospital_01",
                DisplayName = "Hospital",
                Category = BuildingCategory.Medical,
                NpcCapacity = 8,
                OpenTime = 0.0f,
                CloseTime = 1.0f,
                Services = new List<ServiceType> { ServiceType.Healing },
                IsDefault = true,
                MinSettlementType = SettlementType.City,
                DailyMaintenanceCost = 10,
                DailyRevenue = 15,
                UpgradeCosts = new List<int> { 400, 800, 1600 }
            });

            // Storage
            RegisterBuilding(new BuildingData
            {
                BuildingId = "warehouse_01",
                DisplayName = "Warehouse",
                Category = BuildingCategory.Storage,
                NpcCapacity = 2,
                OpenTime = 0.25f,
                CloseTime = 0.80f,
                Services = new List<ServiceType> { ServiceType.Storage },
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 3,
                DailyRevenue = 5,
                UpgradeCosts = new List<int> { 100, 200, 400 }
            });

            // Market
            RegisterBuilding(new BuildingData
            {
                BuildingId = "market_01",
                DisplayName = "Market",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 20,
                OpenTime = 0.20f,
                CloseTime = 0.80f,
                Services = new List<ServiceType> { ServiceType.Market, ServiceType.Trading },
                IsDefault = true,
                MinSettlementType = SettlementType.Village,
                DailyMaintenanceCost = 5,
                DailyRevenue = 25,
                UpgradeCosts = new List<int> { 200, 400, 800 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "market_02",
                DisplayName = "Grand Market",
                Category = BuildingCategory.Commercial,
                NpcCapacity = 50,
                OpenTime = 0.15f,
                CloseTime = 0.85f,
                Services = new List<ServiceType> { ServiceType.Market, ServiceType.Trading, ServiceType.Banking },
                IsDefault = true,
                MinSettlementType = SettlementType.City,
                DailyMaintenanceCost = 12,
                DailyRevenue = 60,
                UpgradeCosts = new List<int> { 500, 1000, 2000 }
            });

            // Transportation
            RegisterBuilding(new BuildingData
            {
                BuildingId = "stables_01",
                DisplayName = "Stables",
                Category = BuildingCategory.Transportation,
                NpcCapacity = 3,
                OpenTime = 0.20f,
                CloseTime = 0.85f,
                Services = new List<ServiceType> { ServiceType.Stables, ServiceType.Travel },
                IsDefault = true,
                MinSettlementType = SettlementType.Hamlet,
                DailyMaintenanceCost = 3,
                DailyRevenue = 10,
                UpgradeCosts = new List<int> { 100, 200, 400 }
            });

            // Dock
            RegisterBuilding(new BuildingData
            {
                BuildingId = "dock_01",
                DisplayName = "Small Dock",
                Category = BuildingCategory.Transportation,
                NpcCapacity = 5,
                OpenTime = 0.20f,
                CloseTime = 0.85f,
                Services = new List<ServiceType> { ServiceType.Dock, ServiceType.Travel },
                IsDefault = true,
                MinSettlementType = SettlementType.Port,
                DailyMaintenanceCost = 5,
                DailyRevenue = 15,
                UpgradeCosts = new List<int> { 200, 400, 800 }
            });

            RegisterBuilding(new BuildingData
            {
                BuildingId = "dock_02",
                DisplayName = "Large Dock",
                Category = BuildingCategory.Transportation,
                NpcCapacity = 10,
                OpenTime = 0.15f,
                CloseTime = 0.90f,
                Services = new List<ServiceType> { ServiceType.Dock, ServiceType.Travel, ServiceType.Storage },
                IsDefault = true,
                MinSettlementType = SettlementType.Port,
                DailyMaintenanceCost = 10,
                DailyRevenue = 30,
                UpgradeCosts = new List<int> { 500, 1000, 2000 }
            });

            // Workshop
            RegisterBuilding(new BuildingData
            {
                BuildingId = "workshop_01",
                DisplayName = "Workshop",
                Category = BuildingCategory.Industrial,
                NpcCapacity = 4,
                OpenTime = 0.25f,
                CloseTime = 0.80f,
                Services = new List<ServiceType> { ServiceType.Crafting },
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 4,
                DailyRevenue = 15,
                UpgradeCosts = new List<int> { 150, 300, 600 }
            });

            // Library
            RegisterBuilding(new BuildingData
            {
                BuildingId = "library_01",
                DisplayName = "Library",
                Category = BuildingCategory.Educational,
                NpcCapacity = 10,
                OpenTime = 0.25f,
                CloseTime = 0.80f,
                Services = new List<ServiceType> { ServiceType.Library },
                IsDefault = true,
                MinSettlementType = SettlementType.City,
                DailyMaintenanceCost = 5,
                DailyRevenue = 3,
                UpgradeCosts = new List<int> { 300, 600, 1200 }
            });

            // Training Grounds
            RegisterBuilding(new BuildingData
            {
                BuildingId = "training_grounds_01",
                DisplayName = "Training Grounds",
                Category = BuildingCategory.Military,
                NpcCapacity = 15,
                OpenTime = 0.20f,
                CloseTime = 0.80f,
                Services = new List<ServiceType> { ServiceType.Training },
                IsDefault = true,
                MinSettlementType = SettlementType.Town,
                DailyMaintenanceCost = 5,
                DailyRevenue = 5,
                UpgradeCosts = new List<int> { 200, 400, 800 }
            });
        }
    }
}