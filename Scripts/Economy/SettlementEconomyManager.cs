using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.Economy
{
    /// <summary>
    /// Manages individual settlement economies.
    /// Tracks population, resources, imports/exports, prosperity,
    /// food/material supply, crafting output, and merchant activity.
    /// The settlement economy evolves autonomously.
    /// </summary>
    public class SettlementEconomyManager : IInitializable
    {
        private readonly Dictionary<string, SettlementEconomyData> _settlements = new(StringComparer.OrdinalIgnoreCase);
        private readonly MarketManager _marketManager;
        private readonly TradeRouteManager _tradeRouteManager;
        private readonly MerchantDatabase _merchantDatabase;
        private readonly Random _rng = new();
        
        public bool IsInitialized { get; private set; }

        public SettlementEconomyManager()
        {
            _marketManager = ServiceLocator.Get<MarketManager>();
            _tradeRouteManager = ServiceLocator.Get<TradeRouteManager>();
            _merchantDatabase = ServiceLocator.Get<MerchantDatabase>();
        }

        public void Initialize()
        {
            _settlements.Clear();
            LoadSettlements();
            IsInitialized = true;
            Logger.Info($"SettlementEconomyManager: Loaded {_settlements.Count} settlements.");
        }

        private void LoadSettlements()
        {
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("settlement_economy");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<SettlementEconomyData>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var settlement in list)
                        {
                            if (!string.IsNullOrEmpty(settlement.SettlementId))
                                _settlements[settlement.SettlementId] = settlement;
                        }
                        Logger.Info($"SettlementEconomyManager: Loaded {_settlements.Count} settlements from config.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SettlementEconomyManager: Config load exception: {ex.Message}");
            }

            Logger.Warning("SettlementEconomyManager: Using fallback settlement definitions.");
            PopulateFallbackSettlements();
        }

        /// <summary>Get settlement data by ID.</summary>
        public SettlementEconomyData? GetSettlement(string settlementId)
        {
            return _settlements.TryGetValue(settlementId, out var data) ? data : null;
        }

        /// <summary>Get all settlements.</summary>
        public List<SettlementEconomyData> GetAllSettlements()
        {
            return new List<SettlementEconomyData>(_settlements.Values);
        }

        /// <summary>Add a new settlement at runtime.</summary>
        public void AddSettlement(SettlementEconomyData settlement)
        {
            if (settlement == null || string.IsNullOrEmpty(settlement.SettlementId)) return;
            if (!_settlements.ContainsKey(settlement.SettlementId))
            {
                _settlements[settlement.SettlementId] = settlement;
                Logger.Info($"SettlementEconomyManager: Added settlement '{settlement.SettlementId}'.");
            }
        }

        /// <summary>
        /// Perform daily economic update for all settlements.
        /// Simulates production, consumption, trade, and prosperity changes.
        /// </summary>
        public void DailyUpdate()
        {
            foreach (var (id, settlement) in _settlements)
            {
                UpdateSettlementEconomy(settlement);
            }
            Logger.Info($"SettlementEconomyManager: Daily update completed for {_settlements.Count} settlements.");
        }

        private void UpdateSettlementEconomy(SettlementEconomyData settlement)
        {
            // Simulate food consumption based on population
            float foodConsumption = settlement.Population * 0.001f;
            settlement.FoodSupply = Math.Max(0f, settlement.FoodSupply - foodConsumption);

            // Simulate material consumption
            float materialConsumption = settlement.Population * 0.0005f;
            settlement.MaterialSupply = Math.Max(0f, settlement.MaterialSupply - materialConsumption);

            // Local production based on primary resources
            foreach (var resource in settlement.PrimaryResources)
            {
                float productionAmount = 0.05f + (_rng.NextSingle() * 0.05f);
                if (IsFoodResource(resource))
                    settlement.FoodSupply = Math.Min(2.0f, settlement.FoodSupply + productionAmount);
                if (IsMaterialResource(resource))
                    settlement.MaterialSupply = Math.Min(2.0f, settlement.MaterialSupply + productionAmount);
            }

            // Merchant activity drives economy
            int merchantCount = _merchantDatabase.GetMerchantsBySettlement(settlement.SettlementId).Count;
            settlement.MerchantActivity = Math.Min(1.0f, merchantCount * 0.2f);

            // Crafting output based on material supply and merchant activity
            settlement.CraftingOutput = Math.Min(1.0f, settlement.MaterialSupply * settlement.MerchantActivity * 0.5f);

            // Calculate prosperity based on supplies and activity
            ProsperityLevel oldProsperity = settlement.Prosperity;
            UpdateProsperity(settlement);

            // Handle active economic events
            if (settlement.ActiveEvent != EconomicEventType.None)
            {
                settlement.EventDuration -= 1;
                if (settlement.EventDuration <= 0)
                {
                    EndEconomicEvent(settlement);
                }
                else
                {
                    ApplyEventEffects(settlement);
                }
            }
            else
            {
                // Random chance for new event
                if (_rng.NextDouble() < 0.02) // 2% chance per day
                {
                    TriggerRandomEvent(settlement);
                }
            }

            // Publish settlement economy change event
            if (settlement.Prosperity != oldProsperity)
            {
                var eventBus = ServiceLocator.Get<EventBus>();
                eventBus?.Publish("SettlementEconomyChanged", new SettlementEconomyChangeEvent
                {
                    SettlementId = settlement.SettlementId,
                    OldProsperity = oldProsperity,
                    NewProsperity = settlement.Prosperity,
                    Event = settlement.ActiveEvent
                });
                Logger.Info($"SettlementEconomyManager: '{settlement.Name}' prosperity changed from {oldProsperity} to {settlement.Prosperity}.");
            }
        }

        private void UpdateProsperity(SettlementEconomyData settlement)
        {
            float score = settlement.FoodSupply * 2f + 
                          settlement.MaterialSupply * 1.5f + 
                          settlement.MerchantActivity * 3f + 
                          settlement.CraftingOutput * 1.5f;

            ProsperityLevel newLevel = score switch
            {
                <= 0.5f => ProsperityLevel.Collapsed,
                <= 1.5f => ProsperityLevel.Poor,
                <= 3.0f => ProsperityLevel.Struggling,
                <= 4.5f => ProsperityLevel.Stable,
                <= 6.0f => ProsperityLevel.Prosperous,
                <= 8.0f => ProsperityLevel.Wealthy,
                _ => ProsperityLevel.Booming
            };

            settlement.Prosperity = newLevel;
        }

        /// <summary>Trigger a random economic event.</summary>
        public void TriggerRandomEvent(SettlementEconomyData settlement, float severity = 0.3f)
        {
            var events = Enum.GetValues<EconomicEventType>()
                .Where(e => e != EconomicEventType.None)
                .ToArray();

            var eventType = events[_rng.Next(events.Length)];
            StartEconomicEvent(settlement.SettlementId, eventType, severity);
        }

        /// <summary>Start an economic event in a settlement.</summary>
        public void StartEconomicEvent(string settlementId, EconomicEventType eventType, float severity = 0.3f)
        {
            if (!_settlements.TryGetValue(settlementId, out var settlement)) return;

            settlement.ActiveEvent = eventType;
            settlement.EventDuration = _rng.Next(3, 8); // 3-7 days

            // Apply market effects
            _marketManager.ApplyEconomicEvent(settlementId, eventType, severity);

            Logger.Info($"SettlementEconomyManager: Started event '{eventType}' in '{settlement.Name}' for {settlement.EventDuration} days.");
        }

        /// <summary>End the current economic event.</summary>
        public void EndEconomicEvent(SettlementEconomyData settlement)
        {
            Logger.Info($"SettlementEconomyManager: Ended event '{settlement.ActiveEvent}' in '{settlement.Name}'.");
            settlement.ActiveEvent = EconomicEventType.None;
            settlement.EventDuration = 0;
        }

        private void ApplyEventEffects(SettlementEconomyData settlement)
        {
            switch (settlement.ActiveEvent)
            {
                case EconomicEventType.Drought:
                    settlement.FoodSupply = Math.Max(0f, settlement.FoodSupply - 0.1f);
                    break;
                case EconomicEventType.Flood:
                    settlement.MaterialSupply = Math.Max(0f, settlement.MaterialSupply - 0.1f);
                    break;
                case EconomicEventType.Plague:
                    settlement.Population = Math.Max(10, settlement.Population - _rng.Next(1, 5));
                    settlement.MerchantActivity = Math.Max(0f, settlement.MerchantActivity - 0.1f);
                    break;
                case EconomicEventType.Festival:
                    settlement.MerchantActivity = Math.Min(1.0f, settlement.MerchantActivity + 0.2f);
                    break;
                case EconomicEventType.BanditRaids:
                    settlement.MaterialSupply = Math.Max(0f, settlement.MaterialSupply - 0.15f);
                    break;
                case EconomicEventType.Migration:
                    settlement.Population += _rng.Next(5, 20);
                    break;
                case EconomicEventType.Discovery:
                    settlement.CraftingOutput = Math.Min(1.0f, settlement.CraftingOutput + 0.2f);
                    settlement.Prosperity = (ProsperityLevel)Math.Min((int)settlement.Prosperity + 1, (int)ProsperityLevel.Booming);
                    break;
            }
        }

        /// <summary>Get save state for all settlements.</summary>
        public List<SettlementSaveState> GetSaveState()
        {
            var states = new List<SettlementSaveState>();
            foreach (var (id, settlement) in _settlements)
            {
                states.Add(new SettlementSaveState
                {
                    SettlementId = id,
                    Prosperity = settlement.Prosperity,
                    FoodSupply = settlement.FoodSupply,
                    MaterialSupply = settlement.MaterialSupply,
                    CraftingOutput = settlement.CraftingOutput,
                    MerchantActivity = settlement.MerchantActivity,
                    ActiveEvent = settlement.ActiveEvent,
                    EventDuration = settlement.EventDuration,
                    Version = 1
                });
            }
            return states;
        }

        /// <summary>Restore settlements from save.</summary>
        public void RestoreSaveState(List<SettlementSaveState> states)
        {
            foreach (var state in states)
            {
                if (_settlements.TryGetValue(state.SettlementId, out var settlement))
                {
                    settlement.Prosperity = state.Prosperity;
                    settlement.FoodSupply = state.FoodSupply;
                    settlement.MaterialSupply = state.MaterialSupply;
                    settlement.CraftingOutput = state.CraftingOutput;
                    settlement.MerchantActivity = state.MerchantActivity;
                    settlement.ActiveEvent = state.ActiveEvent;
                    settlement.EventDuration = state.EventDuration;
                }
            }
            Logger.Info($"SettlementEconomyManager: Restored {states.Count} settlement states.");
        }

        private static bool IsFoodResource(string resource)
        {
            return resource.Contains("food", StringComparison.OrdinalIgnoreCase) ||
                   resource.Contains("grain", StringComparison.OrdinalIgnoreCase) ||
                   resource.Contains("fish", StringComparison.OrdinalIgnoreCase) ||
                   resource.Contains("crop", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsMaterialResource(string resource)
        {
            return resource.Contains("wood", StringComparison.OrdinalIgnoreCase) ||
                   resource.Contains("stone", StringComparison.OrdinalIgnoreCase) ||
                   resource.Contains("ore", StringComparison.OrdinalIgnoreCase) ||
                   resource.Contains("metal", StringComparison.OrdinalIgnoreCase) ||
                   resource.Contains("crystal", StringComparison.OrdinalIgnoreCase);
        }

        private void PopulateFallbackSettlements()
        {
            AddSettlement(new SettlementEconomyData
            {
                SettlementId = "village_harmony",
                Name = "Harmony Village",
                Type = SettlementType.Village,
                Population = 120,
                PrimaryResources = new List<string> { "food_grain", "material_wood" },
                Exports = new List<string> { "food_bread", "material_wood" },
                Imports = new List<string> { "material_iron_ore", "pot_minor_health" },
                Prosperity = ProsperityLevel.Stable,
                FoodSupply = 1.0f,
                MaterialSupply = 1.2f,
                CraftingOutput = 0.6f,
                MerchantActivity = 0.8f
            });

            AddSettlement(new SettlementEconomyData
            {
                SettlementId = "town_haven",
                Name = "Haven Town",
                Type = SettlementType.Town,
                Population = 450,
                PrimaryResources = new List<string> { "crafting", "material_iron_ore" },
                Exports = new List<string> { "armor_leather", "weapon_sword" },
                Imports = new List<string> { "food_fish", "gem_ruby" },
                Prosperity = ProsperityLevel.Prosperous,
                FoodSupply = 0.8f,
                MaterialSupply = 1.5f,
                CraftingOutput = 1.2f,
                MerchantActivity = 1.0f
            });

            AddSettlement(new SettlementEconomyData
            {
                SettlementId = "city_eternia",
                Name = "Eternia City",
                Type = SettlementType.City,
                Population = 2500,
                PrimaryResources = new List<string> { "magic", "crafting", "trade" },
                Exports = new List<string> { "scroll_teleport", "gem_ruby", "potion_ultimate" },
                Imports = new List<string> { "food_bread", "material_wood", "material_iron_ore" },
                Prosperity = ProsperityLevel.Wealthy,
                FoodSupply = 1.2f,
                MaterialSupply = 0.6f,
                CraftingOutput = 2.0f,
                MerchantActivity = 1.5f
            });

            AddSettlement(new SettlementEconomyData
            {
                SettlementId = "port_brinewall",
                Name = "Brinewall Port",
                Type = SettlementType.Port,
                Population = 800,
                PrimaryResources = new List<string> { "food_fish", "trade" },
                Exports = new List<string> { "food_fish", "material_rope" },
                Imports = new List<string> { "armor_leather", "pot_minor_health" },
                Prosperity = ProsperityLevel.Stable,
                FoodSupply = 1.8f,
                MaterialSupply = 0.7f,
                CraftingOutput = 0.8f,
                MerchantActivity = 1.2f
            });

            AddSettlement(new SettlementEconomyData
            {
                SettlementId = "mine_shadowpeak",
                Name = "Shadowpeak Mining Outpost",
                Type = SettlementType.Outpost,
                Population = 60,
                PrimaryResources = new List<string> { "material_iron_ore", "material_crystal_shard" },
                Exports = new List<string> { "material_iron_ore", "material_crystal_shard" },
                Imports = new List<string> { "food_bread", "food_fish" },
                Prosperity = ProsperityLevel.Struggling,
                FoodSupply = 0.3f,
                MaterialSupply = 1.8f,
                CraftingOutput = 0.2f,
                MerchantActivity = 0.4f
            });
        }
    }
}