using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.Economy
{
    /// <summary>
    /// Manages trade routes between settlements.
    /// Simulates caravans transporting goods, travel time, and bandit threats.
    /// Runs autonomously without player interaction.
    /// </summary>
    public class TradeRouteManager : IInitializable
    {
        private readonly Dictionary<string, TradeRouteData> _routes = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _routesBySettlement = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<ActiveCaravan> _activeCaravans = new();
        private readonly Random _rng = new();
        
        public bool IsInitialized { get; private set; }

        public class ActiveCaravan
        {
            public string CaravanId { get; set; } = "";
            public string RouteId { get; set; } = "";
            public string SourceSettlementId { get; set; } = "";
            public string DestinationSettlementId { get; set; } = "";
            public float Progress { get; set; } = 0f;
            public float TotalTravelTime { get; set; }
            public Dictionary<string, int> Goods { get; set; } = new();
            public bool IsReturnTrip { get; set; }
            public bool IsActive { get; set; } = true;
        }

        public void Initialize()
        {
            _routes.Clear();
            _routesBySettlement.Clear();
            _activeCaravans.Clear();
            LoadTradeRoutes();
            IsInitialized = true;
            Logger.Info($"TradeRouteManager: Loaded {_routes.Count} trade routes.");
        }

        private void LoadTradeRoutes()
        {
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("trade_routes");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<TradeRouteData>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var route in list)
                        {
                            if (!string.IsNullOrEmpty(route.RouteId))
                                RegisterRoute(route);
                        }
                        Logger.Info($"TradeRouteManager: Loaded {_routes.Count} routes from config.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"TradeRouteManager: Config load exception: {ex.Message}");
            }

            Logger.Warning("TradeRouteManager: Using fallback trade routes.");
            PopulateFallbackRoutes();
        }

        private void RegisterRoute(TradeRouteData route)
        {
            _routes[route.RouteId] = route;

            if (!_routesBySettlement.ContainsKey(route.SourceSettlementId))
                _routesBySettlement[route.SourceSettlementId] = new List<string>();
            _routesBySettlement[route.SourceSettlementId].Add(route.RouteId);

            if (!_routesBySettlement.ContainsKey(route.DestinationSettlementId))
                _routesBySettlement[route.DestinationSettlementId] = new List<string>();
            _routesBySettlement[route.DestinationSettlementId].Add(route.RouteId);
        }

        /// <summary>Get a route by ID.</summary>
        public TradeRouteData? GetRoute(string routeId)
        {
            return _routes.TryGetValue(routeId, out var route) ? route : null;
        }

        /// <summary>Get all routes connected to a settlement.</summary>
        public List<TradeRouteData> GetRoutesForSettlement(string settlementId)
        {
            var results = new List<TradeRouteData>();
            if (_routesBySettlement.TryGetValue(settlementId, out var ids))
            {
                foreach (var id in ids)
                {
                    if (_routes.TryGetValue(id, out var r))
                        results.Add(r);
                }
            }
            return results;
        }

        /// <summary>Get all routes.</summary>
        public List<TradeRouteData> GetAllRoutes()
        {
            return new List<TradeRouteData>(_routes.Values);
        }

        /// <summary>Add a new route at runtime.</summary>
        public void AddRoute(TradeRouteData route)
        {
            if (route == null || string.IsNullOrEmpty(route.RouteId)) return;
            if (!_routes.ContainsKey(route.RouteId))
            {
                RegisterRoute(route);
                Logger.Info($"TradeRouteManager: Added route '{route.RouteId}'.");
            }
        }

        /// <summary>
        /// Start a caravan on a route.
        /// </summary>
        public ActiveCaravan? StartCaravan(string routeId, Dictionary<string, int>? goods = null)
        {
            if (!_routes.TryGetValue(routeId, out var route) || !route.IsActive) return null;

            var caravan = new ActiveCaravan
            {
                CaravanId = $"caravan_{routeId}_{DateTime.UtcNow.Ticks}",
                RouteId = routeId,
                SourceSettlementId = route.SourceSettlementId,
                DestinationSettlementId = route.DestinationSettlementId,
                Progress = 0f,
                TotalTravelTime = route.TravelTimeHours,
                Goods = goods ?? new Dictionary<string, int>(),
                IsReturnTrip = false,
                IsActive = true
            };

            _activeCaravans.Add(caravan);
            Logger.Info($"TradeRouteManager: Caravan '{caravan.CaravanId}' started on route '{routeId}'.");
            return caravan;
        }

        /// <summary>
        /// Process all active caravans. Called during daily update.
        /// </summary>
        public void ProcessCaravans(float hoursElapsed)
        {
            var completed = new List<ActiveCaravan>();

            foreach (var caravan in _activeCaravans)
            {
                if (!caravan.IsActive) continue;

                caravan.Progress += hoursElapsed;

                if (caravan.Progress >= caravan.TotalTravelTime)
                {
                    // Caravan arrived
                    string arrivalSettlement = caravan.IsReturnTrip 
                        ? caravan.SourceSettlementId 
                        : caravan.DestinationSettlementId;

                    Logger.Info($"TradeRouteManager: Caravan '{caravan.CaravanId}' arrived at '{arrivalSettlement}'.");

                    // Publish arrival event
                    var eventBus = ServiceLocator.Get<EventBus>();
                    eventBus?.Publish("CaravanArrived", new CaravanArrivalEvent
                    {
                        RouteId = caravan.RouteId,
                        SettlementId = arrivalSettlement,
                        GoodsDelivered = new List<string>(caravan.Goods.Keys)
                    });

                    // Check for bandit attack
                    var route = GetRoute(caravan.RouteId);
                    if (route != null && _rng.NextDouble() < route.BanditThreat)
                    {
                        // Caravan was attacked - lose some goods
                        int lossCount = _rng.Next(1, Math.Max(2, caravan.Goods.Count / 2));
                        var keys = caravan.Goods.Keys.ToList();
                        for (int i = 0; i < lossCount && keys.Count > 0; i++)
                        {
                            var lostKey = keys[_rng.Next(keys.Count)];
                            caravan.Goods[lostKey] = Math.Max(0, caravan.Goods[lostKey] - _rng.Next(1, 5));
                            if (caravan.Goods[lostKey] <= 0)
                                caravan.Goods.Remove(lostKey);
                        }
                        Logger.Warning($"TradeRouteManager: Caravan '{caravan.CaravanId}' was attacked by bandits! Lost {lossCount} goods.");
                    }

                    // Start return trip or mark complete
                    if (!caravan.IsReturnTrip)
                    {
                        caravan.Progress = 0f;
                        caravan.IsReturnTrip = true;
                        // Swap source/destination for return
                        (caravan.SourceSettlementId, caravan.DestinationSettlementId) = 
                            (caravan.DestinationSettlementId, caravan.SourceSettlementId);
                    }
                    else
                    {
                        caravan.IsActive = false;
                        completed.Add(caravan);
                    }
                }
            }

            // Remove completed caravans
            foreach (var c in completed)
                _activeCaravans.Remove(c);
        }

        /// <summary>Get all active caravans.</summary>
        public List<ActiveCaravan> GetActiveCaravans()
        {
            return new List<ActiveCaravan>(_activeCaravans);
        }

        /// <summary>Get active caravans for a settlement.</summary>
        public List<ActiveCaravan> GetCaravansForSettlement(string settlementId)
        {
            return _activeCaravans
                .Where(c => c.IsActive && (c.SourceSettlementId == settlementId || c.DestinationSettlementId == settlementId))
                .ToList();
        }

        /// <summary>Get save state for trade routes.</summary>
        public List<TradeRouteSaveState> GetSaveState()
        {
            var states = new List<TradeRouteSaveState>();
            foreach (var route in _routes.Values)
            {
                states.Add(new TradeRouteSaveState
                {
                    RouteId = route.RouteId,
                    IsActive = route.IsActive,
                    BanditThreat = route.BanditThreat,
                    Version = 1
                });
            }
            return states;
        }

        /// <summary>Restore trade routes from save.</summary>
        public void RestoreSaveState(List<TradeRouteSaveState> states)
        {
            foreach (var state in states)
            {
                if (_routes.TryGetValue(state.RouteId, out var route))
                {
                    route.IsActive = state.IsActive;
                    route.BanditThreat = state.BanditThreat;
                }
            }
            Logger.Info($"TradeRouteManager: Restored {states.Count} route states.");
        }

        private void PopulateFallbackRoutes()
        {
            AddRoute(new TradeRouteData
            {
                RouteId = "route_village_to_town",
                Name = "Harmony-Haven Road",
                SourceSettlementId = "village_harmony",
                DestinationSettlementId = "town_haven",
                SourceType = SettlementType.Village,
                DestinationType = SettlementType.Town,
                TravelTimeHours = 3.0f,
                RiskLevel = RouteRiskLevel.Low,
                BanditThreat = 0.05f,
                TypicalGoods = new List<string> { "food_bread", "material_wood", "material_iron_ore" }
            });

            AddRoute(new TradeRouteData
            {
                RouteId = "route_town_to_city",
                Name = "Haven-Eternia Highway",
                SourceSettlementId = "town_haven",
                DestinationSettlementId = "city_eternia",
                SourceType = SettlementType.Town,
                DestinationType = SettlementType.City,
                TravelTimeHours = 6.0f,
                RiskLevel = RouteRiskLevel.Moderate,
                BanditThreat = 0.15f,
                TypicalGoods = new List<string> { "pot_minor_health", "armor_leather", "gem_ruby" }
            });

            AddRoute(new TradeRouteData
            {
                RouteId = "route_city_to_port",
                Name = "Eternia-Brinewall Coast Road",
                SourceSettlementId = "city_eternia",
                DestinationSettlementId = "port_brinewall",
                SourceType = SettlementType.City,
                DestinationType = SettlementType.Port,
                TravelTimeHours = 8.0f,
                RiskLevel = RouteRiskLevel.Dangerous,
                BanditThreat = 0.25f,
                TypicalGoods = new List<string> { "food_fish", "material_rope", "potion_ultimate" }
            });

            AddRoute(new TradeRouteData
            {
                RouteId = "route_village_to_mine",
                Name = "Harmony-Mountain Pass",
                SourceSettlementId = "village_harmony",
                DestinationSettlementId = "mine_shadowpeak",
                SourceType = SettlementType.Village,
                DestinationType = SettlementType.Outpost,
                TravelTimeHours = 4.0f,
                RiskLevel = RouteRiskLevel.Dangerous,
                BanditThreat = 0.3f,
                TypicalGoods = new List<string> { "material_iron_ore", "material_crystal_shard", "food_bread" }
            });

            AddRoute(new TradeRouteData
            {
                RouteId = "route_port_to_village",
                Name = "Brinewall-Harmony Coastal Route",
                SourceSettlementId = "port_brinewall",
                DestinationSettlementId = "village_harmony",
                SourceType = SettlementType.Port,
                DestinationType = SettlementType.Village,
                TravelTimeHours = 5.0f,
                RiskLevel = RouteRiskLevel.Moderate,
                BanditThreat = 0.1f,
                TypicalGoods = new List<string> { "food_fish", "material_rope", "food_bread" }
            });
        }
    }
}