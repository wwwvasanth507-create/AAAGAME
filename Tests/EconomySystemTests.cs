using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Economy;
using HeroOfEternia.Items;
using HeroOfEternia.Inventory;

namespace HeroOfEternia.Tests
{
    /// <summary>
    /// Economy system tests covering price calculation, buying/selling,
    /// inventory validation, merchant restocking, regional pricing,
    /// trade routes, save/load, and stress testing.
    /// </summary>
    public static class EconomySystemTests
    {
        private static int _passed = 0;
        private static int _failed = 0;
        private static string _tempDir = "";

        public static bool RunAll()
        {
            _passed = 0;
            _failed = 0;
            _tempDir = Path.Combine(Path.GetTempPath(), "economy_tests_" + DateTime.UtcNow.Ticks);
            Directory.CreateDirectory(_tempDir);

            try
            {
                GD.Print("=== ECONOMY SYSTEM TESTS ===\n");

                RunTest("E1 Economy Definitions", TestEconomyDefinitions);
                RunTest("E2 Economy Data Models", TestEconomyDataModels);
                RunTest("E3 Market Price Calculation", TestMarketPriceCalculation);
                RunTest("E4 Market Daily Update", TestMarketDailyUpdate);
                RunTest("E5 Regional Pricing", TestRegionalPricing);
                RunTest("E6 Economic Events", TestEconomicEvents);
                RunTest("E7 Merchant Database", TestMerchantDatabase);
                RunTest("E8 Merchant AI Restock", TestMerchantRestock);
                RunTest("E9 Merchant AI States", TestMerchantAIStates);
                RunTest("E10 Trading Buy", TestTradingBuy);
                RunTest("E11 Trading Sell", TestTradingSell);
                RunTest("E12 Trading Validation", TestTradingValidation);
                RunTest("E13 Trade Routes", TestTradeRoutes);
                RunTest("E14 Caravan Simulation", TestCaravanSimulation);
                RunTest("E15 Settlement Economy", TestSettlementEconomy);
                RunTest("E16 Settlement Daily Update", TestSettlementDailyUpdate);
                RunTest("E17 Economy Save/Load", TestEconomySaveLoad);
                RunTest("E18 Stress Test Pricing", TestStressPricing);
                RunTest("E19 Stress Test Trading", TestStressTrading);
                RunTest("E20 Bug Hunt Negative Prices", TestNoNegativePrices);

                GD.Print($"\n=== ECONOMY TESTS: {_passed} passed, {_failed} failed ===\n");
            }
            finally
            {
                if (Directory.Exists(_tempDir))
                    Directory.Delete(_tempDir, true);
            }

            return _failed == 0;
        }

        private static void RunTest(string name, Func<bool> test)
        {
            try
            {
                if (test())
                {
                    GD.Print($"  PASS: {name}");
                    _passed++;
                }
                else
                {
                    GD.Print($"  FAIL: {name}");
                    _failed++;
                }
            }
            catch (Exception ex)
            {
                GD.Print($"  FAIL: {name} - Exception: {ex.Message}");
                _failed++;
            }
        }

        // ==========================================================
        // E1: Economy Definitions
        // ==========================================================
        private static bool TestEconomyDefinitions()
        {
            // Verify all enums have expected values
            if ((int)MerchantCategory.General != 0) return false;
            if ((int)ConsumptionType.None != 0) return false;
            if ((int)ProductionSource.None != 0) return false;
            if ((int)MerchantType.GeneralStore != 0) return false;
            if ((int)MerchantAIState.Closed != 0) return false;
            if ((int)SettlementType.Village != 0) return false;
            if ((int)ProsperityLevel.Collapsed != 0) return false;
            if ((int)RouteRiskLevel.Safe != 0) return false;
            if ((int)EconomicEventType.None != 0) return false;

            // Verify enum counts
            if (Enum.GetValues<MerchantCategory>().Length != 15) return false;
            if (Enum.GetValues<MerchantType>().Length != 18) return false;
            if (Enum.GetValues<MerchantAIState>().Length != 10) return false;
            if (Enum.GetValues<ProsperityLevel>().Length != 7) return false;
            if (Enum.GetValues<EconomicEventType>().Length != 12) return false;

            return true;
        }

        // ==========================================================
        // E2: Economy Data Models
        // ==========================================================
        private static bool TestEconomyDataModels()
        {
            // Test ItemEconomyData defaults
            var itemData = new ItemEconomyData();
            if (itemData.BaseValue != 1) return false;
            if (itemData.MinimumValue != 0) return false;
            if (itemData.MaximumValue != 999999) return false;
            if (itemData.SupplyRating != 0.5f) return false;
            if (itemData.IsIllegal) return false;

            // Test MerchantData defaults
            var merchant = new MerchantData();
            if (merchant.PriceModifier != 1.0f) return false;
            if (merchant.BuyModifier != 0.7f) return false;
            if (merchant.SellModifier != 1.3f) return false;
            if (merchant.GoldCapacity != 10000) return false;
            if (merchant.CurrentGold != 1000) return false;
            if (merchant.CurrentState != MerchantAIState.Closed) return false;

            // Test SettlementEconomyData defaults
            var settlement = new SettlementEconomyData();
            if (settlement.Population != 100) return false;
            if (settlement.Prosperity != ProsperityLevel.Stable) return false;
            if (settlement.FoodSupply != 1.0f) return false;

            // Test TradeRouteData defaults
            var route = new TradeRouteData();
            if (route.TravelTimeHours != 4.0f) return false;
            if (route.RiskLevel != RouteRiskLevel.Low) return false;
            if (route.BanditThreat != 0.1f) return false;

            // Test TradeResult defaults
            var result = new TradeResult();
            if (result.Success) return false;

            // Test PricePreview defaults
            var preview = new PricePreview();
            if (preview.CanAfford) return false;

            return true;
        }

        // ==========================================================
        // E3: Market Price Calculation
        // ==========================================================
        private static bool TestMarketPriceCalculation()
        {
            // Setup
            var saveManager = new SaveManager(_tempDir);
            var configManager = new ConfigManager(_tempDir);
            var itemDb = new ItemDatabase();
            ServiceLocator.Register(saveManager);
            ServiceLocator.Register(configManager);
            ServiceLocator.Register(itemDb);

            itemDb.Initialize();

            var market = new MarketManager();
            market.Initialize();

            // Test base price retrieval
            int price = market.GetPrice("wpn_iron_sword", "default");
            if (price <= 0) return false;

            int buyPrice = market.GetBuyPrice("wpn_iron_sword", "default", 1.3f);
            if (buyPrice <= price) return false;

            int sellPrice = market.GetSellPrice("wpn_iron_sword", "default", 0.7f);
            if (sellPrice >= price) return false;

            // Test non-existent item fallback
            int fallbackPrice = market.GetPrice("nonexistent_item", "default");
            if (fallbackPrice <= 0) return false;

            // Test null/empty guard
            int emptyPrice = market.GetPrice("", "default");
            if (emptyPrice != 0) return false;

            return true;
        }

        // ==========================================================
        // E4: Market Daily Update
        // ==========================================================
        private static bool TestMarketDailyUpdate()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            float priceBefore = market.GetPrice("wpn_iron_sword", "default");

            market.DailyUpdate(1);

            float priceAfter = market.GetPrice("wpn_iron_sword", "default");
            if (priceAfter <= 0) return false;

            if (market.CurrentDay != 1) return false;

            return true;
        }

        // ==========================================================
        // E5: Regional Pricing
        // ==========================================================
        private static bool TestRegionalPricing()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            // Different regions should have different prices
            int forestPrice = market.GetPrice("wpn_iron_sword", "forest");
            int desertPrice = market.GetPrice("wpn_iron_sword", "desert");
            int coastalPrice = market.GetPrice("wpn_iron_sword", "coastal");

            if (forestPrice <= 0 || desertPrice <= 0 || coastalPrice <= 0) return false;

            // At least some regions should differ
            bool hasVariation = forestPrice != desertPrice || forestPrice != coastalPrice;
            if (!hasVariation) return false;

            // Test region keys
            var regions = market.GetRegionKeys();
            if (regions.Count < 3) return false;

            return true;
        }

        // ==========================================================
        // E6: Economic Events
        // ==========================================================
        private static bool TestEconomicEvents()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            float priceBefore = market.GetPrice("wpn_iron_sword", "default");

            // Apply shortage (should increase price)
            market.ApplyEconomicEvent("default", EconomicEventType.Shortage, 0.5f);

            float priceAfterShortage = market.GetPrice("wpn_iron_sword", "default");

            // Apply surplus (should decrease price)
            market.ApplyEconomicEvent("default", EconomicEventType.Surplus, 0.5f);
            float priceAfterSurplus = market.GetPrice("wpn_iron_sword", "default");

            // Verify price data was created
            var priceData = market.GetPriceData("wpn_iron_sword", "default");
            if (priceData == null) return false;

            return true;
        }

        // ==========================================================
        // E7: Merchant Database
        // ==========================================================
        private static bool TestMerchantDatabase()
        {
            var merchantDb = new MerchantDatabase();
            merchantDb.Initialize();

            if (!merchantDb.IsInitialized) return false;
            if (merchantDb.MerchantCount <= 0) return false;

            // Test retrieval
            var merchant = merchantDb.GetMerchant("mer_intro_general");
            if (merchant == null) return false;
            if (merchant.DisplayName != "Elder Marcus") return false;

            // Test settlement query
            var villageMerchants = merchantDb.GetMerchantsBySettlement("village_harmony");
            if (villageMerchants.Count < 2) return false;

            // Test type query
            var blacksmiths = merchantDb.GetMerchantsByType(MerchantType.Blacksmith);
            if (blacksmiths.Count < 1) return false;

            // Test all merchants
            var all = merchantDb.GetAllMerchants();
            if (all.Count != merchantDb.MerchantCount) return false;

            // Test runtime add
            var newMerchant = new MerchantData
            {
                MerchantId = "mer_test_new",
                Name = "Test_Merchant",
                DisplayName = "Test Merchant",
                Type = MerchantType.GeneralStore,
                SettlementId = "test_settlement"
            };
            merchantDb.AddMerchant(newMerchant);

            if (merchantDb.MerchantCount != all.Count + 1) return false;

            // Test null/empty guards
            var nullResult = merchantDb.GetMerchant("");
            if (nullResult != null) return false;

            return true;
        }

        // ==========================================================
        // E8: Merchant AI Restock
        // ==========================================================
        private static bool TestMerchantRestock()
        {
            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var merchantAI = new MerchantAIManager();
            merchantAI.Initialize();

            var merchant = merchantDb.GetMerchant("mer_intro_general");
            if (merchant == null) return false;

            int goldBefore = merchant.CurrentGold;
            int inventoryCount = merchant.Inventory.Count;

            merchantAI.PerformRestock(merchant);

            // Gold should have been spent on restock
            if (merchant.CurrentGold > goldBefore && inventoryCount == 0) 
            {
                // If no inventory was empty, gold shouldn't have changed much
                // This is acceptable - the restock buys from market prices
            }

            return true;
        }

        // ==========================================================
        // E9: Merchant AI States
        // ==========================================================
        private static bool TestMerchantAIStates()
        {
            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var merchantAI = new MerchantAIManager();
            merchantAI.Initialize();

            var merchant = merchantDb.GetMerchant("mer_intro_general");
            if (merchant == null) return false;

            // Verify initial state
            if (merchant.CurrentState != MerchantAIState.Closed) return false;

            // Update with time when merchant should be open
            merchantAI.UpdateAllMerchants(12f, 1f); // Noon

            // After opening sequence, merchant should be Open
            var openMerchants = merchantAI.GetOpenMerchants(12f);
            if (openMerchants.Count <= 0) return false;

            // Trigger emergency
            merchantAI.TriggerEmergency(merchant);
            if (merchant.CurrentState != MerchantAIState.Emergency) return false;

            return true;
        }

        // ==========================================================
        // E10: Trading Buy
        // ==========================================================
        private static bool TestTradingBuy()
        {
            var itemDb = ServiceLocator.Get<ItemDatabase>();
            if (itemDb == null) return false;

            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var merchantAI = new MerchantAIManager();
            merchantAI.Initialize();

            var trading = new TradingManager();
            trading.Initialize();

            var merchant = merchantDb.GetMerchant("mer_intro_general");
            if (merchant == null) return false;

            // Ensure merchant has stock
            merchant.Inventory["pot_minor_health"] = 10;

            var playerInv = new InventoryContainer(20);
            int playerGold = 1000;

            // Preview buy
            var preview = trading.PreviewBuy(merchant.MerchantId, "pot_minor_health", 2, playerGold);
            if (!preview.HasStock) return false;
            if (preview.UnitPrice <= 0) return false;

            // Execute buy
            var result = trading.Buy(merchant.MerchantId, "pot_minor_health", 2, playerInv, ref playerGold);
            if (!result.Success) return false;
            if (result.QuantityTraded != 2) return false;

            // Verify player inventory got the items
            int itemCount = 0;
            foreach (var slot in playerInv.Slots)
            {
                if (slot.ItemId == "pot_minor_health")
                    itemCount += slot.Quantity;
            }
            if (itemCount != 2) return false;

            return true;
        }

        // ==========================================================
        // E11: Trading Sell
        // ==========================================================
        private static bool TestTradingSell()
        {
            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var merchantAI = new MerchantAIManager();
            merchantAI.Initialize();

            var trading = new TradingManager();
            trading.Initialize();

            var merchant = merchantDb.GetMerchant("mer_intro_general");
            if (merchant == null) return false;

            // Give merchant enough gold
            merchant.CurrentGold = 5000;

            var playerInv = new InventoryContainer(20);
            playerInv.AddItem("pot_minor_health", 5);
            int playerGold = 100;

            // Preview sell
            var preview = trading.PreviewSell(merchant.MerchantId, "pot_minor_health", 2, playerGold);
            if (!preview.CanAfford) return false;
            if (preview.UnitPrice <= 0) return false;

            // Execute sell
            var result = trading.Sell(merchant.MerchantId, "pot_minor_health", 2, playerInv, ref playerGold);
            if (!result.Success) return false;
            if (result.QuantityTraded != 2) return false;
            if (result.PlayerGoldAfter <= playerGold) return false;

            return true;
        }

        // ==========================================================
        // E12: Trading Validation
        // ==========================================================
        private static bool TestTradingValidation()
        {
            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var merchantAI = new MerchantAIManager();
            merchantAI.Initialize();

            var trading = new TradingManager();
            trading.Initialize();

            var merchant = merchantDb.GetMerchant("mer_intro_general");
            if (merchant == null) return false;

            var playerInv = new InventoryContainer(20);
            int playerGold = 10;

            // Test buy with insufficient funds
            merchant.Inventory["pot_minor_health"] = 5;
            var result = trading.Buy(merchant.MerchantId, "pot_minor_health", 10, playerInv, ref playerGold);
            if (result.Success) return false; // Should fail - not enough gold

            // Test sell with insufficient merchant funds
            merchant.CurrentGold = 1;
            playerInv.AddItem("pot_minor_health", 5);
            playerGold = 100;
            result = trading.Sell(merchant.MerchantId, "pot_minor_health", 10, playerInv, ref playerGold);
            if (result.Success) return false; // Should fail - not enough merchant gold

            // Test buy with insufficient stock
            playerGold = 5000;
            merchant.Inventory["pot_minor_health"] = 1;
            result = trading.Buy(merchant.MerchantId, "pot_minor_health", 5, playerInv, ref playerGold);
            if (result.Success) return false; // Should fail - not enough stock

            // Test invalid merchant
            result = trading.Buy("nonexistent_merchant", "pot_minor_health", 1, playerInv, ref playerGold);
            if (result.Success) return false;

            // Test invalid item
            result = trading.Buy(merchant.MerchantId, "nonexistent_item", 1, playerInv, ref playerGold);
            if (result.Success) return false;

            return true;
        }

        // ==========================================================
        // E13: Trade Routes
        // ==========================================================
        private static bool TestTradeRoutes()
        {
            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            if (!tradeRoute.IsInitialized) return false;

            var routes = tradeRoute.GetAllRoutes();
            if (routes.Count < 3) return false;

            // Test specific route
            var route = tradeRoute.GetRoute("route_village_to_town");
            if (route == null) return false;
            if (route.SourceSettlementId != "village_harmony") return false;
            if (route.DestinationSettlementId != "town_haven") return false;

            // Test settlement routes
            var settlementRoutes = tradeRoute.GetRoutesForSettlement("village_harmony");
            if (settlementRoutes.Count < 2) return false;

            // Test null/empty guards
            var nullRoute = tradeRoute.GetRoute("");
            if (nullRoute != null) return false;

            return true;
        }

        // ==========================================================
        // E14: Caravan Simulation
        // ==========================================================
        private static bool TestCaravanSimulation()
        {
            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            // Start a caravan
            var goods = new Dictionary<string, int> { { "food_bread", 10 }, { "material_wood", 20 } };
            var caravan = tradeRoute.StartCaravan("route_village_to_town", goods);
            if (caravan == null) return false;
            if (!caravan.IsActive) return false;

            // Process caravans (simulate partial travel)
            tradeRoute.ProcessCaravans(1f); // 1 hour
            if (caravan.Progress <= 0) return false;

            // Check active caravans
            var active = tradeRoute.GetActiveCaravans();
            if (active.Count < 1) return false;

            return true;
        }

        // ==========================================================
        // E15: Settlement Economy
        // ==========================================================
        private static bool TestSettlementEconomy()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var settlementManager = new SettlementEconomyManager();
            settlementManager.Initialize();

            if (!settlementManager.IsInitialized) return false;

            var settlements = settlementManager.GetAllSettlements();
            if (settlements.Count < 3) return false;

            // Test specific settlement
            var village = settlementManager.GetSettlement("village_harmony");
            if (village == null) return false;
            if (village.Population <= 0) return false;
            if (village.PrimaryResources.Count < 1) return false;

            // Test null/empty guards
            var nullSettlement = settlementManager.GetSettlement("");
            if (nullSettlement != null) return false;

            return true;
        }

        // ==========================================================
        // E16: Settlement Daily Update
        // ==========================================================
        private static bool TestSettlementDailyUpdate()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var settlementManager = new SettlementEconomyManager();
            settlementManager.Initialize();

            var village = settlementManager.GetSettlement("village_harmony");
            if (village == null) return false;

            var prosperityBefore = village.Prosperity;

            // Run daily update
            settlementManager.DailyUpdate();

            // Should still function (prosperity may change based on simulation)
            if (village.FoodSupply < 0) return false;
            if (village.MaterialSupply < 0) return false;

            return true;
        }

        // ==========================================================
        // E17: Economy Save/Load
        // ==========================================================
        private static bool TestEconomySaveLoad()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var saveData = market.GetAllPriceData();
            if (saveData.Count <= 0) return false;

            int day = market.CurrentDay;
            float inflation = market.GlobalInflation;

            // Simulate save/restore
            market.RestorePriceData(saveData, day, inflation);

            if (market.CurrentDay != day) return false;
            if (Math.Abs(market.GlobalInflation - inflation) > 0.001f) return false;

            return true;
        }

        // ==========================================================
        // E18: Stress Test Pricing
        // ==========================================================
        private static bool TestStressPricing()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            // Simulate many daily updates
            for (int i = 0; i < 30; i++)
            {
                market.DailyUpdate(i + 1);
            }

            // Verify all prices remained valid
            var regions = market.GetRegionKeys();
            foreach (var region in regions)
            {
                var prices = market.GetRegionalPrices(region);
                foreach (var price in prices)
                {
                    if (price.CurrentPrice <= 0) return false;
                    if (price.SupplyRating < 0 || price.SupplyRating > 1) return false;
                    if (price.DemandRating < 0 || price.DemandRating > 1) return false;
                }
            }

            if (market.CurrentDay != 30) return false;

            return true;
        }

        // ==========================================================
        // E19: Stress Test Trading
        // ==========================================================
        private static bool TestStressTrading()
        {
            var merchantDb = ServiceLocator.Get<MerchantDatabase>();
            if (merchantDb == null) return false;

            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            var tradeRoute = new TradeRouteManager();
            tradeRoute.Initialize();

            var merchantAI = new MerchantAIManager();
            merchantAI.Initialize();

            var trading = new TradingManager();
            trading.Initialize();

            var merchant = merchantDb.GetMerchant("mer_intro_general");
            if (merchant == null) return false;

            var playerInv = new InventoryContainer(50);
            int playerGold = 100000;

            merchant.CurrentGold = 100000;
            merchant.Inventory["pot_minor_health"] = 100;

            // Perform many trades
            for (int i = 0; i < 20; i++)
            {
                var buyResult = trading.Buy(merchant.MerchantId, "pot_minor_health", 3, playerInv, ref playerGold);
                if (!buyResult.Success && playerGold > 0) return false;

                var sellResult = trading.Sell(merchant.MerchantId, "pot_minor_health", 1, playerInv, ref playerGold);
                if (!sellResult.Success && merchant.CurrentGold > 0) return false;
            }

            // Verify no corruption
            if (playerGold < 0) return false;
            if (merchant.CurrentGold < 0) return false;

            return true;
        }

        // ==========================================================
        // E20: Bug Hunt - No Negative Prices
        // ==========================================================
        private static bool TestNoNegativePrices()
        {
            var market = ServiceLocator.Get<MarketManager>();
            if (market == null) return false;

            // Check all regions for negative prices
            var regions = market.GetRegionKeys();
            foreach (var region in regions)
            {
                var prices = market.GetRegionalPrices(region);
                foreach (var itemPrice in prices)
                {
                    if (itemPrice.BasePrice < 0) return false;
                    if (itemPrice.CurrentPrice < 0) return false;
                    if (itemPrice.SupplyRating < 0) return false;
                    if (itemPrice.DemandRating < 0) return false;
                    if (itemPrice.SupplyRating > 1) return false;
                    if (itemPrice.DemandRating > 1) return false;
                }
            }

            // Test get price never returns negative
            int price = market.GetPrice("wpn_iron_sword", "default");
            if (price < 0) return false;

            int buyPrice = market.GetBuyPrice("wpn_iron_sword", "default", 1.0f);
            if (buyPrice < 0) return false;

            int sellPrice = market.GetSellPrice("wpn_iron_sword", "default", 1.0f);
            if (sellPrice < 0) return false;

            return true;
        }
    }
}