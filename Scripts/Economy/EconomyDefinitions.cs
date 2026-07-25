using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroOfEternia.Economy
{
    // ==========================================================
    // ENUMS
    // ==========================================================

    /// <summary>Merchant category for item classification.</summary>
    public enum MerchantCategory
    {
        General,
        Weapons,
        Armor,
        Potions,
        Food,
        Materials,
        Magic,
        Blacksmith,
        Alchemist,
        Enchanter,
        Jeweler,
        Books,
        Exotic,
        Illegal,
        Special
    }

    /// <summary>Consumption type determines how NPCs use items.</summary>
    public enum ConsumptionType
    {
        None,
        Food,
        Drink,
        Material,
        Component,
        Fuel,
        Medicine,
        Ammo,
        Scroll,
        Tool
    }

    /// <summary>Production source for items.</summary>
    public enum ProductionSource
    {
        None,
        Gathering,
        Crafting,
        Loot,
        Farming,
        Mining,
        Fishing,
        Hunting,
        Merchant,
        Import,
        Quest
    }

    /// <summary>Merchant type classification.</summary>
    public enum MerchantType
    {
        GeneralStore,
        Blacksmith,
        Alchemist,
        Enchanter,
        Farmer,
        Fisherman,
        Miner,
        Lumberjack,
        Hunter,
        Tailor,
        Jeweler,
        Librarian,
        Innkeeper,
        Stablemaster,
        ExoticTrader,
        IllegalDealer,
        TravelingMerchant,
        CaravanMaster
    }

    /// <summary>Merchant AI state.</summary>
    public enum MerchantAIState
    {
        Closed,
        Opening,
        Open,
        Restocking,
        Buying,
        Selling,
        Traveling,
        Returning,
        Emergency,
        Idle
    }

    /// <summary>Settlement type for trade route classification.</summary>
    public enum SettlementType
    {
        Village,
        Town,
        City,
        Port,
        Fortress,
        Outpost,
        Camp
    }

    /// <summary>Economic prosperity level.</summary>
    public enum ProsperityLevel
    {
        Collapsed,
        Poor,
        Struggling,
        Stable,
        Prosperous,
        Wealthy,
        Booming
    }

    /// <summary>Trade route risk level.</summary>
    public enum RouteRiskLevel
    {
        Safe,
        Low,
        Moderate,
        Dangerous,
        Extreme,
        Lethal
    }

    /// <summary>Economic event type.</summary>
    public enum EconomicEventType
    {
        None,
        Shortage,
        Surplus,
        Festival,
        Drought,
        Flood,
        Plague,
        War,
        TradeEmbargo,
        Discovery,
        Migration,
        BanditRaids
    }

    // ==========================================================
    // ITEM ECONOMY DATA
    // ==========================================================

    /// <summary>Economy-specific metadata attached to every item.</summary>
    public class ItemEconomyData
    {
        public int BaseValue { get; set; } = 1;
        public int MinimumValue { get; set; } = 0;
        public int MaximumValue { get; set; } = 999999;
        
        /// <summary>Regional price modifier key (e.g. "forest", "desert", "mountain").</summary>
        public string RegionalModifierKey { get; set; } = "default";
        
        /// <summary>Current supply rating 0.0 (none) to 1.0 (overflowing).</summary>
        public float SupplyRating { get; set; } = 0.5f;
        
        /// <summary>Current demand rating 0.0 (none) to 1.0 (critical).</summary>
        public float DemandRating { get; set; } = 0.5f;
        
        public ProductionSource ProductionSource { get; set; } = ProductionSource.None;
        public ConsumptionType ConsumptionType { get; set; } = ConsumptionType.None;
        public MerchantCategory MerchantCategory { get; set; } = MerchantCategory.General;
        
        public bool IsIllegal { get; set; } = false;
        public bool IsLuxury { get; set; } = false;
        
        /// <summary>If > 0, item spoils after this many in-game days.</summary>
        public int SpoilageDays { get; set; } = 0;
        
        /// <summary>Future inflation data hook (reserved).</summary>
        public float InflationFactor { get; set; } = 1.0f;
        
        /// <summary>Dynamic extension map for future economy fields.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    // ==========================================================
    // MERCHANT DATA
    // ==========================================================

    /// <summary>Complete merchant definition.</summary>
    public class MerchantData
    {
        public string MerchantId { get; set; } = "";
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public MerchantType Type { get; set; } = MerchantType.GeneralStore;
        public string Faction { get; set; } = "neutral";
        public string SettlementId { get; set; } = "";
        public string Profession { get; set; } = "merchant";
        
        /// <summary>Item IDs this merchant can stock.</summary>
        public List<string> InventoryRules { get; set; } = new();
        
        /// <summary>How restocking works (e.g. "daily", "weekly", "manual").</summary>
        public string RestockRule { get; set; } = "daily";
        
        /// <summary>Price modifier (1.0 = base prices).</summary>
        public float PriceModifier { get; set; } = 1.0f;
        
        /// <summary>Buy price modifier (1.0 = base).</summary>
        public float BuyModifier { get; set; } = 0.7f;
        
        /// <summary>Sell price modifier (1.0 = base).</summary>
        public float SellModifier { get; set; } = 1.3f;
        
        /// <summary>Maximum gold the merchant can hold.</summary>
        public int GoldCapacity { get; set; } = 10000;
        
        /// <summary>Current gold held.</summary>
        public int CurrentGold { get; set; } = 1000;
        
        /// <summary>Trading hours (0-24 open, 0-24 close).</summary>
        public float OpenHour { get; set; } = 6.0f;
        public float CloseHour { get; set; } = 20.0f;
        
        /// <summary>Item categories this merchant prefers buying.</summary>
        public List<MerchantCategory> PreferredGoods { get; set; } = new();
        
        /// <summary>Item categories this merchant dislikes.</summary>
        public List<MerchantCategory> DislikedGoods { get; set; } = new();
        
        /// <summary>Dialogue hook for merchant interaction.</summary>
        public string DialogueHook { get; set; } = "";
        
        /// <summary>Future reputation modifier hook.</summary>
        public float ReputationModifier { get; set; } = 1.0f;
        
        /// <summary>Current AI state.</summary>
        public MerchantAIState CurrentState { get; set; } = MerchantAIState.Closed;
        
        /// <summary>Current inventory (itemId -> quantity).</summary>
        public Dictionary<string, int> Inventory { get; set; } = new();
        
        /// <summary>Dynamic extension map.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    /// <summary>Merchant save state for persistence.</summary>
    public class MerchantSaveState
    {
        public string MerchantId { get; set; } = "";
        public int CurrentGold { get; set; }
        public MerchantAIState CurrentState { get; set; }
        public Dictionary<string, int> Inventory { get; set; } = new();
        public float TravelProgress { get; set; } = 0f;
        public string CurrentRouteId { get; set; } = "";
        public string LocationSettlementId { get; set; } = "";
        public int Version { get; set; } = 1;
    }

    // ==========================================================
    // SETTLEMENT ECONOMY DATA
    // ==========================================================

    /// <summary>Complete settlement economic state.</summary>
    public class SettlementEconomyData
    {
        public string SettlementId { get; set; } = "";
        public string Name { get; set; } = "";
        public SettlementType Type { get; set; } = SettlementType.Village;
        public int Population { get; set; } = 100;
        
        /// <summary>Primary resources produced by this settlement.</summary>
        public List<string> PrimaryResources { get; set; } = new();
        
        /// <summary>Goods this settlement exports.</summary>
        public List<string> Exports { get; set; } = new();
        
        /// <summary>Goods this settlement imports.</summary>
        public List<string> Imports { get; set; } = new();
        
        public ProsperityLevel Prosperity { get; set; } = ProsperityLevel.Stable;
        public float FoodSupply { get; set; } = 1.0f;
        public float MaterialSupply { get; set; } = 1.0f;
        public float CraftingOutput { get; set; } = 1.0f;
        public float MerchantActivity { get; set; } = 1.0f;
        
        /// <summary>Current active economic event.</summary>
        public EconomicEventType ActiveEvent { get; set; } = EconomicEventType.None;
        public float EventDuration { get; set; } = 0f;
        
        /// <summary>Future happiness hooks (reserved).</summary>
        public float HappinessModifier { get; set; } = 1.0f;
        
        /// <summary>Dynamic extension map.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    /// <summary>Settlement save state.</summary>
    public class SettlementSaveState
    {
        public string SettlementId { get; set; } = "";
        public ProsperityLevel Prosperity { get; set; }
        public float FoodSupply { get; set; }
        public float MaterialSupply { get; set; }
        public float CraftingOutput { get; set; }
        public float MerchantActivity { get; set; }
        public EconomicEventType ActiveEvent { get; set; }
        public float EventDuration { get; set; }
        public int Version { get; set; } = 1;
    }

    // ==========================================================
    // TRADE ROUTE DATA
    // ==========================================================

    /// <summary>Trade route connecting two settlements.</summary>
    public class TradeRouteData
    {
        public string RouteId { get; set; } = "";
        public string Name { get; set; } = "";
        public string SourceSettlementId { get; set; } = "";
        public string DestinationSettlementId { get; set; } = "";
        public SettlementType SourceType { get; set; } = SettlementType.Village;
        public SettlementType DestinationType { get; set; } = SettlementType.Village;
        
        /// <summary>Travel time in in-game hours.</summary>
        public float TravelTimeHours { get; set; } = 4.0f;
        
        public RouteRiskLevel RiskLevel { get; set; } = RouteRiskLevel.Low;
        
        /// <summary>Goods typically transported on this route.</summary>
        public List<string> TypicalGoods { get; set; } = new();
        
        /// <summary>Bandit threat probability (0.0-1.0).</summary>
        public float BanditThreat { get; set; } = 0.1f;
        
        /// <summary>Is the route active.</summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>Dynamic extension map.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    /// <summary>Trade route save state.</summary>
    public class TradeRouteSaveState
    {
        public string RouteId { get; set; } = "";
        public bool IsActive { get; set; }
        public float BanditThreat { get; set; }
        public int Version { get; set; } = 1;
    }

    // ==========================================================
    // MARKET DATA
    // ==========================================================

    /// <summary>Regional price data for a single item.</summary>
    public class RegionalPriceData
    {
        public string ItemId { get; set; } = "";
        public string RegionKey { get; set; } = "default";
        public float BasePrice { get; set; }
        public float CurrentPrice { get; set; }
        public float SupplyRating { get; set; } = 0.5f;
        public float DemandRating { get; set; } = 0.5f;
        public float PriceHistory { get; set; } // last calculated price for trend
        public int LastUpdateDay { get; set; } = 0;
    }

    /// <summary>Market history record for tracking price trends.</summary>
    public class MarketHistoryRecord
    {
        public string ItemId { get; set; } = "";
        public string RegionKey { get; set; } = "default";
        public float Price { get; set; }
        public float Supply { get; set; }
        public float Demand { get; set; }
        public int Day { get; set; }
    }

    // ==========================================================
    // TRADING DATA
    // ==========================================================

    /// <summary>Result of a trade transaction.</summary>
    public class TradeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int QuantityTraded { get; set; }
        public int TotalCost { get; set; }
        public int MerchantGoldAfter { get; set; }
        public int PlayerGoldAfter { get; set; }
    }

    /// <summary>Price preview for a potential transaction.</summary>
    public class PricePreview
    {
        public string ItemId { get; set; } = "";
        public string ItemName { get; set; } = "";
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        public bool IsBuy { get; set; }
        public bool CanAfford { get; set; }
        public bool HasStock { get; set; }
        public int AvailableStock { get; set; }
        public int PlayerGold { get; set; }
        public int MerchantGold { get; set; }
    }

    // ==========================================================
    // ECONOMY EVENTS
    // ==========================================================

    /// <summary>Event published when a trade occurs.</summary>
    public class TradeEvent
    {
        public string MerchantId { get; set; } = "";
        public string PlayerId { get; set; } = "player";
        public string ItemId { get; set; } = "";
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        public bool IsBuy { get; set; } // true = player buying from merchant
        public string SettlementId { get; set; } = "";
    }

    /// <summary>Event published when market prices update.</summary>
    public class MarketUpdateEvent
    {
        public string RegionKey { get; set; } = "";
        public int Day { get; set; }
        public int ItemsUpdated { get; set; }
    }

    /// <summary>Event published when a trade route caravan arrives.</summary>
    public class CaravanArrivalEvent
    {
        public string RouteId { get; set; } = "";
        public string SettlementId { get; set; } = "";
        public List<string> GoodsDelivered { get; set; } = new();
    }

    /// <summary>Event published when settlement economy changes.</summary>
    public class SettlementEconomyChangeEvent
    {
        public string SettlementId { get; set; } = "";
        public ProsperityLevel OldProsperity { get; set; }
        public ProsperityLevel NewProsperity { get; set; }
        public EconomicEventType Event { get; set; }
    }
}