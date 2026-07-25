using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Items;
using HeroOfEternia.Inventory;

namespace HeroOfEternia.Economy
{
    /// <summary>
    /// Reusable trading framework for player-merchant transactions.
    /// Supports buy, sell, stack purchases, quantity selection,
    /// price preview, transaction validation, and inventory management.
    /// </summary>
    public class TradingManager : IInitializable
    {
        private readonly MarketManager _marketManager;
        private readonly MerchantDatabase _merchantDatabase;
        private readonly MerchantAIManager _merchantAI;
        private readonly ItemDatabase _itemDatabase;
        
        public bool IsInitialized { get; private set; }

        public TradingManager()
        {
            _marketManager = ServiceLocator.Get<MarketManager>();
            _merchantDatabase = ServiceLocator.Get<MerchantDatabase>();
            _merchantAI = ServiceLocator.Get<MerchantAIManager>();
            _itemDatabase = ServiceLocator.Get<ItemDatabase>();
        }

        public void Initialize()
        {
            IsInitialized = true;
            Logger.Info("TradingManager: Initialized.");
        }

        // ==========================================================
        // PRICE PREVIEW
        // ==========================================================

        /// <summary>
        /// Get price preview for buying from a merchant.
        /// </summary>
        public PricePreview PreviewBuy(string merchantId, string itemId, int quantity, int playerGold)
        {
            var merchant = _merchantDatabase.GetMerchant(merchantId);
            if (merchant == null)
                return ErrorPreview(itemId, quantity, "Merchant not found.");

            var item = _itemDatabase.GetItem(itemId);
            if (item == null)
                return ErrorPreview(itemId, quantity, "Item not found.");

            int availableStock = merchant.Inventory.GetValueOrDefault(itemId, 0);
            int unitPrice = _marketManager.GetEffectiveBuyPrice(itemId, merchant.SettlementId, merchant);
            int totalPrice = unitPrice * quantity;
            bool canAfford = playerGold >= totalPrice;
            bool hasStock = availableStock >= quantity;

            return new PricePreview
            {
                ItemId = itemId,
                ItemName = item.DisplayName,
                UnitPrice = unitPrice,
                Quantity = quantity,
                TotalPrice = totalPrice,
                IsBuy = true,
                CanAfford = canAfford,
                HasStock = hasStock,
                AvailableStock = availableStock,
                PlayerGold = playerGold,
                MerchantGold = merchant.CurrentGold
            };
        }

        /// <summary>
        /// Get price preview for selling to a merchant.
        /// </summary>
        public PricePreview PreviewSell(string merchantId, string itemId, int quantity, int playerGold)
        {
            var merchant = _merchantDatabase.GetMerchant(merchantId);
            if (merchant == null)
                return ErrorPreview(itemId, quantity, "Merchant not found.");

            var item = _itemDatabase.GetItem(itemId);
            if (item == null)
                return ErrorPreview(itemId, quantity, "Item not found.");

            int unitPrice = _marketManager.GetEffectiveSellPrice(itemId, merchant.SettlementId, merchant);
            int totalPrice = unitPrice * quantity;
            bool merchantCanAfford = merchant.CurrentGold >= totalPrice;

            return new PricePreview
            {
                ItemId = itemId,
                ItemName = item.DisplayName,
                UnitPrice = unitPrice,
                Quantity = quantity,
                TotalPrice = totalPrice,
                IsBuy = false,
                CanAfford = merchantCanAfford,
                HasStock = true,
                AvailableStock = quantity,
                PlayerGold = playerGold,
                MerchantGold = merchant.CurrentGold
            };
        }

        // ==========================================================
        // BUYING (Player buys from Merchant)
        // ==========================================================

        /// <summary>
        /// Execute a buy transaction: player buys items from merchant.
        /// </summary>
        public TradeResult Buy(string merchantId, string itemId, int quantity, 
                               InventoryContainer playerInventory, ref int playerGold)
        {
            var result = new TradeResult { Success = false };

            // Validate merchant
            var merchant = _merchantDatabase.GetMerchant(merchantId);
            if (merchant == null)
            {
                result.Message = "Merchant not found.";
                return result;
            }

            // Validate item exists
            var item = _itemDatabase.GetItem(itemId);
            if (item == null)
            {
                result.Message = "Item not found.";
                return result;
            }

            // Validate stock
            if (!merchant.Inventory.TryGetValue(itemId, out var stock) || stock < quantity)
            {
                result.Message = $"Insufficient stock. Available: {stock}/{quantity}";
                return result;
            }

            // Calculate price
            int unitPrice = _marketManager.GetEffectiveBuyPrice(itemId, merchant.SettlementId, merchant);
            int totalPrice = unitPrice * quantity;

            // Validate player funds
            if (playerGold < totalPrice)
            {
                result.Message = $"Insufficient gold. Need {totalPrice}, have {playerGold}.";
                return result;
            }

            // Validate player inventory space
            if (!playerInventory.AddItem(itemId, quantity))
            {
                result.Message = "Inventory full.";
                return result;
            }

            // Execute transaction
            playerGold -= totalPrice;
            merchant.CurrentGold += totalPrice;
            merchant.CurrentGold = Math.Min(merchant.CurrentGold, merchant.GoldCapacity);

            // Remove from merchant inventory
            merchant.Inventory[itemId] -= quantity;
            if (merchant.Inventory[itemId] <= 0)
                merchant.Inventory.Remove(itemId);

            // Update merchant AI state
            _merchantAI.MerchantSellItems(merchant, itemId, quantity, totalPrice);

            // Publish trade event
            EventBus.Publish(new TradeEvent
            {
                MerchantId = merchantId,
                ItemId = itemId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                IsBuy = true,
                SettlementId = merchant.SettlementId
            });

            result.Success = true;
            result.Message = $"Purchased {quantity}x {item.DisplayName} for {totalPrice} gold.";
            result.QuantityTraded = quantity;
            result.TotalCost = totalPrice;
            result.MerchantGoldAfter = merchant.CurrentGold;
            result.PlayerGoldAfter = playerGold;

            Logger.Info($"Trading: Player bought {quantity}x '{itemId}' from '{merchantId}' for {totalPrice} gold.");
            return result;
        }

        // ==========================================================
        // SELLING (Player sells to Merchant)
        // ==========================================================

        /// <summary>
        /// Execute a sell transaction: player sells items to merchant.
        /// </summary>
        public TradeResult Sell(string merchantId, string itemId, int quantity,
                                InventoryContainer playerInventory, ref int playerGold)
        {
            var result = new TradeResult { Success = false };

            // Validate merchant
            var merchant = _merchantDatabase.GetMerchant(merchantId);
            if (merchant == null)
            {
                result.Message = "Merchant not found.";
                return result;
            }

            // Validate item
            var item = _itemDatabase.GetItem(itemId);
            if (item == null)
            {
                result.Message = "Item not found.";
                return result;
            }

            // Calculate price
            int unitPrice = _marketManager.GetEffectiveSellPrice(itemId, merchant.SettlementId, merchant);
            int totalPrice = unitPrice * quantity;

            // Validate merchant funds
            if (merchant.CurrentGold < totalPrice)
            {
                result.Message = $"Merchant has insufficient gold. Needs {totalPrice}, has {merchant.CurrentGold}.";
                return result;
            }

            // Validate player has items and remove them
            if (!playerInventory.RemoveItem(itemId, quantity))
            {
                result.Message = $"You don't have {quantity}x {item.DisplayName}.";
                return result;
            }

            // Execute transaction
            playerGold += totalPrice;
            _merchantAI.MerchantBuyItems(merchant, itemId, quantity, totalPrice);

            // Publish trade event
            EventBus.Publish(new TradeEvent
            {
                MerchantId = merchantId,
                ItemId = itemId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                IsBuy = false,
                SettlementId = merchant.SettlementId
            });

            result.Success = true;
            result.Message = $"Sold {quantity}x {item.DisplayName} for {totalPrice} gold.";
            result.QuantityTraded = quantity;
            result.TotalCost = totalPrice;
            result.MerchantGoldAfter = merchant.CurrentGold;
            result.PlayerGoldAfter = playerGold;

            Logger.Info($"Trading: Player sold {quantity}x '{itemId}' to '{merchantId}' for {totalPrice} gold.");
            return result;
        }

        // ==========================================================
        // QUANTITY HELPERS
        // ==========================================================

        /// <summary>
        /// Calculate max affordable quantity for buying.
        /// </summary>
        public int GetMaxBuyQuantity(string merchantId, string itemId, int playerGold)
        {
            var merchant = _merchantDatabase.GetMerchant(merchantId);
            if (merchant == null) return 0;

            int unitPrice = _marketManager.GetEffectiveBuyPrice(itemId, merchant.SettlementId, merchant);
            if (unitPrice <= 0) return 0;

            int availableStock = merchant.Inventory.GetValueOrDefault(itemId, 0);
            int affordable = playerGold / unitPrice;
            return Math.Min(availableStock, affordable);
        }

        /// <summary>
        /// Calculate max stack the merchant can afford to buy.
        /// </summary>
        public int GetMaxSellQuantity(string merchantId, string itemId, int playerQuantity)
        {
            var merchant = _merchantDatabase.GetMerchant(merchantId);
            if (merchant == null) return 0;

            int unitPrice = _marketManager.GetEffectiveSellPrice(itemId, merchant.SettlementId, merchant);
            if (unitPrice <= 0) return 0;

            int merchantCanAfford = merchant.CurrentGold / unitPrice;
            return Math.Min(playerQuantity, merchantCanAfford);
        }

        /// <summary>
        /// Check if a merchant is currently open.
        /// </summary>
        public bool IsMerchantOpen(string merchantId, float timeOfDay)
        {
            var merchant = _merchantDatabase.GetMerchant(merchantId);
            if (merchant == null) return false;

            return merchant.CurrentState == MerchantAIState.Open &&
                   timeOfDay >= merchant.OpenHour && timeOfDay < merchant.CloseHour;
        }

        private static PricePreview ErrorPreview(string itemId, int quantity, string message)
        {
            return new PricePreview
            {
                ItemId = itemId,
                Quantity = quantity,
                IsBuy = true,
                CanAfford = false,
                HasStock = false,
                AvailableStock = 0,
                PlayerGold = 0,
                MerchantGold = 0,
                UnitPrice = 0,
                TotalPrice = 0
            };
        }
    }
}