using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Economy
{
    /// <summary>
    /// Manages merchant AI behaviors: opening/closing shop, restocking,
    /// buying/selling goods, traveling between settlements, emergency behaviors.
    /// Runs autonomously without player interaction.
    /// </summary>
    public class MerchantAIManager : IInitializable
    {
        private readonly MerchantDatabase _merchantDatabase;
        private readonly MarketManager _marketManager;
        private readonly TradeRouteManager _tradeRouteManager;
        private readonly Random _rng = new();
        private float _timeOfDay = 12f;
        
        public bool IsInitialized { get; private set; }

        public MerchantAIManager()
        {
            _merchantDatabase = ServiceLocator.Get<MerchantDatabase>();
            _marketManager = ServiceLocator.Get<MarketManager>();
            _tradeRouteManager = ServiceLocator.Get<TradeRouteManager>();
        }

        public void Initialize()
        {
            IsInitialized = true;
            Logger.Info("MerchantAIManager: Initialized.");
        }

        /// <summary>
        /// Update all merchant AI. Called each game tick.
        /// </summary>
        public void UpdateAllMerchants(float timeOfDay, float deltaHours)
        {
            _timeOfDay = timeOfDay;
            var merchants = _merchantDatabase.GetAllMerchants();

            foreach (var merchant in merchants)
            {
                UpdateMerchantState(merchant, timeOfDay, deltaHours);
            }
        }

        private void UpdateMerchantState(MerchantData merchant, float timeOfDay, float deltaHours)
        {
            switch (merchant.CurrentState)
            {
                case MerchantAIState.Closed:
                    if (timeOfDay >= merchant.OpenHour && timeOfDay < merchant.CloseHour)
                        merchant.CurrentState = MerchantAIState.Opening;
                    break;

                case MerchantAIState.Opening:
                    merchant.CurrentState = MerchantAIState.Open;
                    Logger.Info($"MerchantAI: {merchant.DisplayName} opened shop.");
                    break;

                case MerchantAIState.Open:
                    if (timeOfDay < merchant.OpenHour || timeOfDay >= merchant.CloseHour)
                    {
                        merchant.CurrentState = MerchantAIState.Closed;
                        Logger.Info($"MerchantAI: {merchant.DisplayName} closed shop.");
                    }
                    // Chance to restock while open
                    else if (_rng.NextDouble() < 0.01 * deltaHours)
                    {
                        merchant.CurrentState = MerchantAIState.Restocking;
                    }
                    break;

                case MerchantAIState.Restocking:
                    PerformRestock(merchant);
                    merchant.CurrentState = MerchantAIState.Open;
                    break;

                case MerchantAIState.Traveling:
                    // Handle travel progress
                    // Simplified: merchants return home after a period
                    if (_rng.NextDouble() < 0.1 * deltaHours)
                    {
                        merchant.CurrentState = MerchantAIState.Returning;
                    }
                    break;

                case MerchantAIState.Returning:
                    merchant.CurrentState = MerchantAIState.Open;
                    Logger.Info($"MerchantAI: {merchant.DisplayName} returned home.");
                    break;

                case MerchantAIState.Emergency:
                    if (_rng.NextDouble() < 0.2 * deltaHours)
                        merchant.CurrentState = MerchantAIState.Opening;
                    break;

                case MerchantAIState.Idle:
                    if (timeOfDay >= merchant.OpenHour && timeOfDay < merchant.CloseHour)
                        merchant.CurrentState = MerchantAIState.Opening;
                    break;
            }
        }

        /// <summary>
        /// Perform restock for a merchant based on their inventory rules.
        /// </summary>
        public void PerformRestock(MerchantData merchant)
        {
            if (merchant == null) return;

            int restockCount = _rng.Next(3, 10);
            
            foreach (var ruleId in merchant.InventoryRules)
            {
                if (restockCount <= 0) break;

                // Check if merchant can afford restock
                int buyPrice = _marketManager.GetBuyPrice(ruleId, "default", 1.0f);
                if (buyPrice <= 0) continue;

                int affordable = merchant.CurrentGold / buyPrice;
                if (affordable <= 0) continue;

                int toAdd = _rng.Next(1, Math.Min(10, affordable + 1));
                int cost = toAdd * buyPrice;

                if (cost <= merchant.CurrentGold)
                {
                    merchant.CurrentGold -= cost;
                    if (!merchant.Inventory.ContainsKey(ruleId))
                        merchant.Inventory[ruleId] = 0;
                    merchant.Inventory[ruleId] += toAdd;
                    restockCount--;
                }
            }

            Logger.Info($"MerchantAI: {merchant.DisplayName} restocked. Gold remaining: {merchant.CurrentGold}");
        }

        /// <summary>
        /// Sell items to a merchant (merchant buys from player/NPC).
        /// </summary>
        public bool MerchantBuyItems(MerchantData merchant, string itemId, int quantity, int totalPrice)
        {
            if (merchant == null) return false;
            if (merchant.CurrentGold < totalPrice) return false;

            merchant.CurrentGold -= totalPrice;
            if (!merchant.Inventory.ContainsKey(itemId))
                merchant.Inventory[itemId] = 0;
            merchant.Inventory[itemId] += quantity;

            // Trigger supply/demand shift based on transaction
            merchant.CurrentState = MerchantAIState.Buying;
            return true;
        }

        /// <summary>
        /// Sell items from merchant to player/NPC.
        /// </summary>
        public bool MerchantSellItems(MerchantData merchant, string itemId, int quantity, int totalPrice)
        {
            if (merchant == null) return false;
            if (!merchant.Inventory.TryGetValue(itemId, out var stock) || stock < quantity)
                return false;

            merchant.Inventory[itemId] -= quantity;
            if (merchant.Inventory[itemId] <= 0)
                merchant.Inventory.Remove(itemId);

            merchant.CurrentGold += totalPrice;

            // Cap gold at capacity
            merchant.CurrentGold = Math.Min(merchant.CurrentGold, merchant.GoldCapacity);

            merchant.CurrentState = MerchantAIState.Selling;
            return true;
        }

        /// <summary>
        /// Trigger emergency behavior for a merchant.
        /// </summary>
        public void TriggerEmergency(MerchantData merchant)
        {
            if (merchant == null) return;
            merchant.CurrentState = MerchantAIState.Emergency;
            Logger.Warning($"MerchantAI: {merchant.DisplayName} triggered emergency state.");
        }

        /// <summary>
        /// Get merchants that are currently open.
        /// </summary>
        public List<MerchantData> GetOpenMerchants(float timeOfDay)
        {
            return _merchantDatabase.GetAllMerchants()
                .Where(m => m.CurrentState == MerchantAIState.Open && 
                           timeOfDay >= m.OpenHour && timeOfDay < m.CloseHour)
                .ToList();
        }

        /// <summary>
        /// Get merchants in a specific state.
        /// </summary>
        public List<MerchantData> GetMerchantsByState(MerchantAIState state)
        {
            return _merchantDatabase.GetAllMerchants()
                .Where(m => m.CurrentState == state)
                .ToList();
        }

        /// <summary>Get save state for merchants.</summary>
        public List<MerchantSaveState> GetSaveState()
        {
            var states = new List<MerchantSaveState>();
            foreach (var merchant in _merchantDatabase.GetAllMerchants())
            {
                states.Add(new MerchantSaveState
                {
                    MerchantId = merchant.MerchantId,
                    CurrentGold = merchant.CurrentGold,
                    CurrentState = merchant.CurrentState,
                    Inventory = new Dictionary<string, int>(merchant.Inventory),
                    Version = 1
                });
            }
            return states;
        }

        /// <summary>Restore merchants from save.</summary>
        public void RestoreSaveState(List<MerchantSaveState> states)
        {
            foreach (var state in states)
            {
                var merchant = _merchantDatabase.GetMerchant(state.MerchantId);
                if (merchant != null)
                {
                    merchant.CurrentGold = state.CurrentGold;
                    merchant.CurrentState = state.CurrentState;
                    merchant.Inventory = new Dictionary<string, int>(state.Inventory);
                }
            }
            Logger.Info($"MerchantAIManager: Restored {states.Count} merchant states.");
        }
    }
}