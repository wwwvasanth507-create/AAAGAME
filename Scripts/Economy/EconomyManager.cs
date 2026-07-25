using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Economy
{
    /// <summary>
    /// Central orchestrator for all economy systems.
    /// Ties together MarketManager, MerchantDatabase, MerchantAIManager,
    /// TradingManager, TradeRouteManager, and SettlementEconomyManager.
    /// Runs daily updates to simulate autonomous economy.
    /// </summary>
    public class EconomyManager : IInitializable
    {
        private readonly MarketManager _marketManager;
        private readonly MerchantDatabase _merchantDatabase;
        private readonly MerchantAIManager _merchantAI;
        private readonly TradingManager _tradingManager;
        private readonly TradeRouteManager _tradeRouteManager;
        private readonly SettlementEconomyManager _settlementEconomy;
        private readonly Random _rng = new();
        
        private int _currentDay = 0;
        private float _accumulatedTime = 0f;
        private const float DayLengthHours = 24f;
        private float _timeOfDay = 6f; // Start at 6 AM
        
        public bool IsInitialized { get; private set; }
        public int CurrentDay => _currentDay;
        public float TimeOfDay => _timeOfDay;

        public EconomyManager()
        {
            _marketManager = ServiceLocator.Get<MarketManager>();
            _merchantDatabase = ServiceLocator.Get<MerchantDatabase>();
            _merchantAI = ServiceLocator.Get<MerchantAIManager>();
            _tradingManager = ServiceLocator.Get<TradingManager>();
            _tradeRouteManager = ServiceLocator.Get<TradeRouteManager>();
            _settlementEconomy = ServiceLocator.Get<SettlementEconomyManager>();
        }

        public void Initialize()
        {
            // Initialize all sub-systems
            _marketManager.Initialize();
            _merchantDatabase.Initialize();
            _merchantAI.Initialize();
            _tradingManager.Initialize();
            _tradeRouteManager.Initialize();
            _settlementEconomy.Initialize();
            
            _currentDay = 0;
            _accumulatedTime = 0f;
            _timeOfDay = 6f;
            IsInitialized = true;
            
            Logger.Info("EconomyManager: All economy systems initialized.");
            Logger.Info($"EconomyManager: {_merchantDatabase.MerchantCount} merchants, {_marketManager.GetRegionKeys().Count} regions, {_settlementEconomy.GetAllSettlements().Count} settlements.");
        }

        /// <summary>
        /// Update the entire economy simulation. Called each game tick.
        /// </summary>
        public void Update(float deltaHours)
        {
            if (!IsInitialized) return;

            _accumulatedTime += deltaHours;
            _timeOfDay = (_timeOfDay + deltaHours) % 24f;

            // Update merchant AI
            _merchantAI.UpdateAllMerchants(_timeOfDay, deltaHours);

            // Process trade route caravans
            _tradeRouteManager.ProcessCaravans(deltaHours);

            // Spawn caravans periodically
            if (_accumulatedTime >= 12f) // Every 12 in-game hours
            {
                SpawnCaravans();
            }

            // Daily update when day changes
            if (_accumulatedTime >= DayLengthHours)
            {
                _accumulatedTime -= DayLengthHours;
                _currentDay++;
                PerformDailyUpdate();
            }
        }

        private void SpawnCaravans()
        {
            var routes = _tradeRouteManager.GetAllRoutes();
            foreach (var route in routes)
            {
                if (route.IsActive && _rng.NextDouble() < 0.3) // 30% chance per cycle
                {
                    var sourceEconomy = _settlementEconomy.GetSettlement(route.SourceSettlementId);
                    if (sourceEconomy != null)
                    {
                        // Build goods from settlement exports
                        var goods = new Dictionary<string, int>();
                        foreach (var export in sourceEconomy.Exports)
                        {
                            goods[export] = _rng.Next(5, 25);
                        }
                        _tradeRouteManager.StartCaravan(route.RouteId, goods);
                    }
                }
            }
        }

        private void PerformDailyUpdate()
        {
            Logger.Info($"EconomyManager: Day {_currentDay} starting daily update...");

            // Update market prices
            _marketManager.DailyUpdate(_currentDay);

            // Update settlement economies
            _settlementEconomy.DailyUpdate();

            // Auto-restock all merchants
            var merchants = _merchantDatabase.GetAllMerchants();
            foreach (var merchant in merchants)
            {
                _merchantAI.PerformRestock(merchant);
            }

            Logger.Info($"EconomyManager: Day {_currentDay} complete. Economy running autonomously.");
        }

        /// <summary>
        /// Get complete economy save state.
        /// </summary>
        public EconomySaveData GetSaveData()
        {
            return new EconomySaveData
            {
                Version = 1,
                CurrentDay = _currentDay,
                TimeOfDay = _timeOfDay,
                MarketPrices = _marketManager.GetAllPriceData(),
                MerchantStates = _merchantAI.GetSaveState(),
                SettlementStates = _settlementEconomy.GetSaveState(),
                TradeRouteStates = _tradeRouteManager.GetSaveState()
            };
        }

        /// <summary>
        /// Restore economy state from save data.
        /// </summary>
        public void RestoreSaveData(EconomySaveData data)
        {
            if (data == null) return;

            _currentDay = data.CurrentDay;
            _timeOfDay = data.TimeOfDay;

            _marketManager.RestorePriceData(data.MarketPrices, data.CurrentDay, _marketManager.GlobalInflation);
            _merchantAI.RestoreSaveState(data.MerchantStates);
            _settlementEconomy.RestoreSaveState(data.SettlementStates);
            _tradeRouteManager.RestoreSaveState(data.TradeRouteStates);

            Logger.Info($"EconomyManager: Restored economy state from day {data.CurrentDay}.");
        }
    }

    /// <summary>
    /// Save data container for all economy systems.
    /// </summary>
    public class EconomySaveData
    {
        public int Version { get; set; } = 1;
        public int CurrentDay { get; set; }
        public float TimeOfDay { get; set; }
        public List<RegionalPriceData> MarketPrices { get; set; } = new();
        public List<MerchantSaveState> MerchantStates { get; set; } = new();
        public List<SettlementSaveState> SettlementStates { get; set; } = new();
        public List<TradeRouteSaveState> TradeRouteStates { get; set; } = new();
    }
}