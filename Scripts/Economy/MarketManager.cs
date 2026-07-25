using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Items;

namespace HeroOfEternia.Economy
{
    /// <summary>
    /// Central market system managing supply, demand, and price calculations.
    /// Supports regional pricing, daily updates, and economic events.
    /// The economy runs autonomously without player interaction.
    /// </summary>
    public class MarketManager : IInitializable
    {
        private readonly Dictionary<string, Dictionary<string, RegionalPriceData>> _regionalPrices = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, float> _regionalModifiers = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<MarketHistoryRecord> _priceHistory = new();
        private readonly ItemDatabase _itemDatabase;
        private readonly Random _rng = new();
        
        private int _currentDay = 0;
        private float _globalInflation = 1.0f;
        private const int MaxHistoryRecords = 10000;
        
        public bool IsInitialized { get; private set; }
        public int CurrentDay => _currentDay;
        public float GlobalInflation => _globalInflation;

        public MarketManager()
        {
            _itemDatabase = ServiceLocator.Get<ItemDatabase>();
        }

        public void Initialize()
        {
            _regionalPrices.Clear();
            _regionalModifiers.Clear();
            _priceHistory.Clear();
            _currentDay = 0;
            _globalInflation = 1.0f;
            
            LoadRegionalModifiers();
            InitializePrices();
            IsInitialized = true;
            Logger.Info("MarketManager: Initialized with regional pricing data.");
        }

        private void LoadRegionalModifiers()
        {
            // Default region modifiers - data-driven from database
            _regionalModifiers["default"] = 1.0f;
            _regionalModifiers["forest"] = 1.1f;
            _regionalModifiers["desert"] = 1.3f;
            _regionalModifiers["mountain"] = 1.2f;
            _regionalModifiers["coastal"] = 0.9f;
            _regionalModifiers["plains"] = 1.0f;
            _regionalModifiers["swamp"] = 1.4f;
            _regionalModifiers["tundra"] = 1.5f;
            _regionalModifiers["city_large"] = 0.8f;
            _regionalModifiers["city_small"] = 1.1f;
            _regionalModifiers["village_poor"] = 1.3f;
            _regionalModifiers["village_rich"] = 0.9f;
            
            Logger.Info($"MarketManager: Loaded {_regionalModifiers.Count} regional modifiers.");
        }

        private void InitializePrices()
        {
            var items = _itemDatabase.GetAllItems();
            if (items.Count == 0)
            {
                Logger.Warning("MarketManager: No items in database to initialize prices.");
                return;
            }

            foreach (var item in items)
            {
                foreach (var regionKey in _regionalModifiers.Keys)
                {
                    var priceData = CalculateInitialPrice(item, regionKey);
                    if (!_regionalPrices.ContainsKey(regionKey))
                        _regionalPrices[regionKey] = new Dictionary<string, RegionalPriceData>(StringComparer.OrdinalIgnoreCase);
                    _regionalPrices[regionKey][item.UniqueId] = priceData;
                }
            }
            
            Logger.Info($"MarketManager: Initialized prices for {items.Count} items across {_regionalModifiers.Count} regions.");
        }

        private RegionalPriceData CalculateInitialPrice(ItemRecord item, string regionKey)
        {
            float regionalMod = _regionalModifiers.GetValueOrDefault(regionKey, 1.0f);
            float basePrice = item.BuyValue > 0 ? item.BuyValue : item.SellValue > 0 ? item.SellValue : 10;
            
            // Apply rarity multiplier
            float rarityMult = GetRarityValueMultiplier(item.Rarity);
            float baseCalc = basePrice * rarityMult * regionalMod;
            
            // Clamp to min/max
            int minVal = Math.Max(1, item.SellValue > 0 ? item.SellValue / 2 : 1);
            int maxVal = Math.Max(minVal * 10, item.BuyValue * 3);
            
            float clamped = Math.Clamp(baseCalc, minVal, maxVal);
            
            return new RegionalPriceData
            {
                ItemId = item.UniqueId,
                RegionKey = regionKey,
                BasePrice = clamped,
                CurrentPrice = clamped,
                SupplyRating = 0.5f,
                DemandRating = 0.5f,
                PriceHistory = clamped,
                LastUpdateDay = 0
            };
        }

        /// <summary>
        /// Get current price for an item in a region.
        /// </summary>
        public int GetPrice(string itemId, string regionKey = "default")
        {
            if (string.IsNullOrEmpty(itemId)) return 0;
            
            if (_regionalPrices.TryGetValue(regionKey, out var region) &&
                region.TryGetValue(itemId, out var priceData))
            {
                return (int)Math.Max(1, priceData.CurrentPrice);
            }
            
            // Fallback: calculate from item database
            var item = _itemDatabase.GetItem(itemId);
            if (item == null) return 1;
            
            float price = item.BuyValue > 0 ? item.BuyValue : item.SellValue > 0 ? item.SellValue : 10;
            return (int)Math.Max(1, price);
        }

        /// <summary>Get buy price (merchant sells to player).</summary>
        public int GetBuyPrice(string itemId, string regionKey, float merchantSellModifier = 1.3f)
        {
            float basePrice = GetPrice(itemId, regionKey);
            return (int)Math.Max(1, basePrice * merchantSellModifier);
        }

        /// <summary>Get sell price (merchant buys from player).</summary>
        public int GetSellPrice(string itemId, string regionKey, float merchantBuyModifier = 0.7f)
        {
            float basePrice = GetPrice(itemId, regionKey);
            return (int)Math.Max(1, basePrice * merchantBuyModifier);
        }

        /// <summary>Get price with all modifiers applied for a specific merchant.</summary>
        public int GetEffectiveBuyPrice(string itemId, string regionKey, MerchantData merchant)
        {
            float basePrice = GetPrice(itemId, regionKey);
            float merchantMod = merchant?.SellModifier ?? 1.3f;
            float preferredMod = IsPreferredItem(merchant, itemId) ? 1.1f : 1.0f;
            float dislikedMod = IsDislikedItem(merchant, itemId) ? 1.2f : 1.0f;
            
            return (int)Math.Max(1, basePrice * merchantMod * preferredMod * dislikedMod * _globalInflation);
        }

        /// <summary>Get effective sell price for player selling to merchant.</summary>
        public int GetEffectiveSellPrice(string itemId, string regionKey, MerchantData merchant)
        {
            float basePrice = GetPrice(itemId, regionKey);
            float merchantMod = merchant?.BuyModifier ?? 0.7f;
            float preferredMod = IsPreferredItem(merchant, itemId) ? 1.2f : 1.0f;
            float dislikedMod = IsDislikedItem(merchant, itemId) ? 0.8f : 1.0f;
            
            return (int)Math.Max(1, basePrice * merchantMod * preferredMod * dislikedMod * _globalInflation);
        }

        /// <summary>
        /// Perform daily market update.
        /// Adjusts supply, demand, and prices for all items across all regions.
        /// This is the core autonomous economy simulation.
        /// </summary>
        public void DailyUpdate(int day)
        {
            _currentDay = day;
            int itemsUpdated = 0;
            
            foreach (var (regionKey, region) in _regionalPrices)
            {
                foreach (var (itemId, priceData) in region)
                {
                    UpdateSinglePrice(priceData, regionKey);
                    itemsUpdated++;
                }
            }
            
            // Apply inflation drift (very gradual)
            _globalInflation += (float)(_rng.NextDouble() * 0.002 - 0.001);
            _globalInflation = Math.Clamp(_globalInflation, 0.5f, 3.0f);
            
            // Trim history if needed
            if (_priceHistory.Count > MaxHistoryRecords)
            {
                _priceHistory.RemoveRange(0, _priceHistory.Count - MaxHistoryRecords);
            }
            
            Logger.Info($"MarketManager: Daily update complete. Updated {itemsUpdated} prices across {_regionalPrices.Count} regions. Day {day}, Inflation: {_globalInflation:F3}");
            
            // Publish market update event
            EventBus.Publish(new MarketUpdateEvent
            {
                RegionKey = "all",
                Day = day,
                ItemsUpdated = itemsUpdated
            });
        }

        private void UpdateSinglePrice(RegionalPriceData priceData, string regionKey)
        {
            // Simulate supply/demand drift
            float supplyDrift = (float)(_rng.NextDouble() * 0.1 - 0.05);
            float demandDrift = (float)(_rng.NextDouble() * 0.1 - 0.05);
            
            priceData.SupplyRating = Math.Clamp(priceData.SupplyRating + supplyDrift, 0.05f, 0.95f);
            priceData.DemandRating = Math.Clamp(priceData.DemandRating + demandDrift, 0.05f, 0.95f);
            
            // Price calculation based on supply and demand
            float supplyFactor = 1.0f + (0.5f - priceData.SupplyRating); // Low supply = higher price
            float demandFactor = 1.0f + (priceData.DemandRating - 0.5f); // High demand = higher price
            float regionalMod = _regionalModifiers.GetValueOrDefault(regionKey, 1.0f);
            
            float newPrice = priceData.BasePrice * supplyFactor * demandFactor * regionalMod * _globalInflation;
            newPrice += (float)(_rng.NextDouble() * newPrice * 0.05 - newPrice * 0.025); // +/- 2.5% random noise
            
            // Record history
            _priceHistory.Add(new MarketHistoryRecord
            {
                ItemId = priceData.ItemId,
                RegionKey = regionKey,
                Price = priceData.CurrentPrice,
                Supply = priceData.SupplyRating,
                Demand = priceData.DemandRating,
                Day = _currentDay
            });
            
            priceData.PriceHistory = priceData.CurrentPrice;
            priceData.CurrentPrice = Math.Max(1, newPrice);
            priceData.LastUpdateDay = _currentDay;
        }

        /// <summary>
        /// Apply an economic event to a region.
        /// </summary>
        public void ApplyEconomicEvent(string regionKey, EconomicEventType eventType, float severity = 0.3f)
        {
            if (!_regionalPrices.TryGetValue(regionKey, out var region)) return;
            
            foreach (var (itemId, priceData) in region)
            {
                switch (eventType)
                {
                    case EconomicEventType.Shortage:
                        priceData.SupplyRating = Math.Max(0.05f, priceData.SupplyRating - severity);
                        priceData.DemandRating = Math.Min(0.95f, priceData.DemandRating + severity * 0.5f);
                        break;
                    case EconomicEventType.Surplus:
                        priceData.SupplyRating = Math.Min(0.95f, priceData.SupplyRating + severity);
                        priceData.DemandRating = Math.Max(0.05f, priceData.DemandRating - severity * 0.5f);
                        break;
                    case EconomicEventType.Festival:
                        priceData.DemandRating = Math.Min(0.95f, priceData.DemandRating + severity * 0.5f);
                        break;
                    case EconomicEventType.Drought:
                        priceData.SupplyRating = Math.Max(0.05f, priceData.SupplyRating - severity * 0.8f);
                        break;
                    case EconomicEventType.War:
                        priceData.SupplyRating = Math.Max(0.05f, priceData.SupplyRating - severity);
                        priceData.DemandRating = Math.Min(0.95f, priceData.DemandRating + severity);
                        break;
                    case EconomicEventType.TradeEmbargo:
                        priceData.SupplyRating = Math.Max(0.05f, priceData.SupplyRating - severity * 0.7f);
                        break;
                }
                
                // Recalculate price after event
                float supplyFactor = 1.0f + (0.5f - priceData.SupplyRating);
                float demandFactor = 1.0f + (priceData.DemandRating - 0.5f);
                float regionalMod = _regionalModifiers.GetValueOrDefault(regionKey, 1.0f);
                
                priceData.CurrentPrice = Math.Max(1, priceData.BasePrice * supplyFactor * demandFactor * regionalMod * _globalInflation);
            }
            
            Logger.Info($"MarketManager: Applied event '{eventType}' to region '{regionKey}' with severity {severity:F2}.");
        }

        /// <summary>Get price data for an item in a region.</summary>
        public RegionalPriceData? GetPriceData(string itemId, string regionKey = "default")
        {
            if (_regionalPrices.TryGetValue(regionKey, out var region) &&
                region.TryGetValue(itemId, out var priceData))
            {
                return priceData;
            }
            return null;
        }

        /// <summary>Get all price data for a region.</summary>
        public List<RegionalPriceData> GetRegionalPrices(string regionKey)
        {
            if (_regionalPrices.TryGetValue(regionKey, out var region))
            {
                return new List<RegionalPriceData>(region.Values);
            }
            return new List<RegionalPriceData>();
        }

        /// <summary>Get price history for an item in a region.</summary>
        public List<MarketHistoryRecord> GetPriceHistory(string itemId, string regionKey = "default")
        {
            return _priceHistory
                .Where(h => h.ItemId == itemId && h.RegionKey == regionKey)
                .ToList();
        }

        /// <summary>Get all region keys.</summary>
        public List<string> GetRegionKeys()
        {
            return new List<string>(_regionalModifiers.Keys);
        }

        /// <summary>Get or set a regional modifier.</summary>
        public float GetRegionalModifier(string regionKey)
        {
            return _regionalModifiers.GetValueOrDefault(regionKey, 1.0f);
        }

        public void SetRegionalModifier(string regionKey, float modifier)
        {
            _regionalModifiers[regionKey] = Math.Max(0.1f, modifier);
        }

        /// <summary>Add a new region at runtime.</summary>
        public void AddRegion(string regionKey, float baseModifier = 1.0f)
        {
            if (!_regionalModifiers.ContainsKey(regionKey))
            {
                _regionalModifiers[regionKey] = baseModifier;
                _regionalPrices[regionKey] = new Dictionary<string, RegionalPriceData>(StringComparer.OrdinalIgnoreCase);
                
                // Initialize prices for all items in this region
                var items = _itemDatabase.GetAllItems();
                foreach (var item in items)
                {
                    _regionalPrices[regionKey][item.UniqueId] = CalculateInitialPrice(item, regionKey);
                }
                Logger.Info($"MarketManager: Added region '{regionKey}' with modifier {baseModifier}.");
            }
        }

        /// <summary>
        /// Get market export data for save/load.
        /// </summary>
        public List<RegionalPriceData> GetAllPriceData()
        {
            var allData = new List<RegionalPriceData>();
            foreach (var region in _regionalPrices.Values)
            {
                allData.AddRange(region.Values);
            }
            return allData;
        }

        /// <summary>
        /// Restore price data from save.
        /// </summary>
        public void RestorePriceData(List<RegionalPriceData> priceData, int day, float inflation)
        {
            _regionalPrices.Clear();
            _currentDay = day;
            _globalInflation = inflation;
            
            foreach (var data in priceData)
            {
                if (!_regionalPrices.ContainsKey(data.RegionKey))
                    _regionalPrices[data.RegionKey] = new Dictionary<string, RegionalPriceData>(StringComparer.OrdinalIgnoreCase);
                _regionalPrices[data.RegionKey][data.ItemId] = data;
            }
            
            Logger.Info($"MarketManager: Restored {priceData.Count} price entries for {_regionalPrices.Count} regions. Day {day}.");
        }

        private static float GetRarityValueMultiplier(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => 1.0f,
                ItemRarity.Uncommon => 1.5f,
                ItemRarity.Rare => 2.5f,
                ItemRarity.Epic => 4.0f,
                ItemRarity.Legendary => 7.0f,
                ItemRarity.Mythic => 12.0f,
                ItemRarity.Ancient => 20.0f,
                ItemRarity.Divine => 35.0f,
                _ => 1.0f
            };
        }

        private static bool IsPreferredItem(MerchantData? merchant, string itemId)
        {
            if (merchant == null) return false;
            // Would need item category lookup, simplified for now
            return false;
        }

        private static bool IsDislikedItem(MerchantData? merchant, string itemId)
        {
            if (merchant == null) return false;
            return false;
        }
    }
}