using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroOfEternia.Settlement
{
    // ==========================================================
    // ENUMS
    // ==========================================================

    /// <summary>Expanded settlement type classification.</summary>
    public enum SettlementType
    {
        Camp,
        Hamlet,
        Village,
        Town,
        City,
        Capital,
        Fort,
        Castle,
        Port,
        MiningCamp,
        ForestOutpost,
        Temple,
        NomadCamp,
        // Future DLC
        FloatingSettlement,
        UndergroundSettlement
    }

    /// <summary>Security rating for a settlement.</summary>
    public enum SecurityRating
    {
        None,
        Minimal,
        Low,
        Moderate,
        High,
        Maximum,
        Fortified
    }

    /// <summary>Government type (future use).</summary>
    public enum GovernmentType
    {
        None,
        Monarchy,
        Democracy,
        Oligarchy,
        Theocracy,
        Military,
        Tribal,
        MerchantCouncil,
        GuildRule,
        Anarchy
    }

    /// <summary>Prosperity level of a settlement.</summary>
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

    /// <summary>Building category classification.</summary>
    public enum BuildingCategory
    {
        Residential,
        Commercial,
        Industrial,
        Agricultural,
        Civic,
        Military,
        Religious,
        Services,
        Storage,
        Transportation,
        Entertainment,
        Educational,
        Medical,
        Custom
    }

    /// <summary>Building operational state.</summary>
    public enum BuildingState
    {
        Inactive,
        Active,
        Upgrading,
        Damaged,
        Destroyed,
        Closed
    }

    /// <summary>NPC schedule activity type.</summary>
    public enum ScheduleActivity
    {
        Wake,
        Sleep,
        Breakfast,
        TravelToWork,
        Work,
        Lunch,
        Shopping,
        Socialize,
        Patrol,
        Relax,
        GoHome,
        Emergency,
        Festival,
        Idle,
        Train,
        Worship,
        Study,
        Guard,
        Hunt,
        Fish,
        Gather,
        Craft,
        Trade,
        Travel
    }

    /// <summary>Public service type.</summary>
    public enum ServiceType
    {
        Trading,
        Crafting,
        EquipmentRepair,
        Healing,
        InnRest,
        Storage,
        Training,
        Travel,
        Banking,
        Guild,
        Housing,
        Stables,
        Blacksmith,
        Enchanting,
        Alchemy,
        Library,
        Temple,
        Market,
        Dock,
        TownHall
    }

    /// <summary>World event type for settlements.</summary>
    public enum WorldEventType
    {
        None,
        MarketDay,
        Festival,
        Harvest,
        StormPreparation,
        MonsterAlert,
        MerchantArrival,
        TravelingCaravan,
        ResourceShortage,
        // Future
        Invasion,
        Celebration,
        Plague,
        Drought,
        Flood,
        Discovery,
        Migration
    }

    /// <summary>World event severity.</summary>
    public enum EventSeverity
    {
        Minor,
        Moderate,
        Major,
        Critical
    }

    /// <summary>Event phase for lifecycle tracking.</summary>
    public enum EventPhase
    {
        Pending,
        Active,
        Resolving,
        Completed
    }

    /// <summary>NPC profession specialization.</summary>
    public enum NpcProfession
    {
        None,
        Farmer,
        Fisherman,
        Miner,
        Lumberjack,
        Hunter,
        Blacksmith,
        Carpenter,
        Baker,
        Butcher,
        Tailor,
        Jeweler,
        Alchemist,
        Enchanter,
        Merchant,
        Innkeeper,
        Guard,
        Soldier,
        Knight,
        Mage,
        Priest,
        Healer,
        Scholar,
        Librarian,
        Stablemaster,
        Dockworker,
        Sailor,
        Artisan,
        Builder,
        Trader,
        Noble,
        Beggar,
        Child,
        Elder,
        Traveler
    }

    // ==========================================================
    // SETTLEMENT DATA
    // ==========================================================

    /// <summary>Complete settlement definition.</summary>
    public class SettlementData
    {
        public string SettlementId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string LocalizationKey { get; set; } = "";
        public SettlementType Type { get; set; } = SettlementType.Village;
        public string Region { get; set; } = "";
        public string Biome { get; set; } = "temperate_forest";
        public int Population { get; set; } = 100;
        public int MaxPopulation { get; set; } = 500;
        public ProsperityLevel Prosperity { get; set; } = ProsperityLevel.Stable;
        public SecurityRating Security { get; set; } = SecurityRating.Low;
        public GovernmentType Government { get; set; } = GovernmentType.None;

        /// <summary>Primary industries driving the economy.</summary>
        public List<string> PrimaryIndustries { get; set; } = new();

        /// <summary>Goods this settlement exports.</summary>
        public List<string> PrimaryExports { get; set; } = new();

        /// <summary>Goods this settlement imports.</summary>
        public List<string> PrimaryImports { get; set; } = new();

        /// <summary>Faction controlling this settlement.</summary>
        public string Faction { get; set; } = "neutral";

        /// <summary>Climate modifier for weather calculations (0.5 = half effect, 2.0 = double).</summary>
        public float ClimateModifier { get; set; } = 1.0f;

        /// <summary>Services available in this settlement.</summary>
        public List<ServiceType> SettlementServices { get; set; } = new();

        /// <summary>Building IDs present in this settlement.</summary>
        public List<string> BuildingIds { get; set; } = new();

        /// <summary>Spawn rules for NPCs in this settlement.</summary>
        public SettlementSpawnRules SpawnRules { get; set; } = new();

        /// <summary>Music profile key for ambient audio.</summary>
        public string MusicProfile { get; set; } = "default";

        /// <summary>World position (x, z) for map placement.</summary>
        public float WorldPositionX { get; set; } = 0f;
        public float WorldPositionZ { get; set; } = 0f;

        /// <summary>Dynamic extension map for future DLC fields.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    /// <summary>Spawn rules for a settlement's NPC population.</summary>
    public class SettlementSpawnRules
    {
        /// <summary>NPC type distribution (NpcTypeEnum -> weight).</summary>
        public Dictionary<string, float> NpcTypeWeights { get; set; } = new()
        {
            { "Civilian", 0.6f },
            { "Merchant", 0.15f },
            { "Guard", 0.1f },
            { "Noble", 0.05f },
            { "Child", 0.08f },
            { "Traveler", 0.02f }
        };

        /// <summary>Profession distribution (NpcProfession -> weight).</summary>
        public Dictionary<string, float> ProfessionWeights { get; set; } = new();

        /// <summary>Maximum active NPCs at once.</summary>
        public int MaxActiveNpcs { get; set; } = 50;

        /// <summary>Respawn delay in seconds after NPC despawn.</summary>
        public float RespawnDelay { get; set; } = 30f;

        /// <summary>Should children NPCs spawn.</summary>
        public bool SpawnChildren { get; set; } = true;

        /// <summary>Should elderly NPCs spawn.</summary>
        public bool SpawnElderly { get; set; } = true;
    }

    // ==========================================================
    // SETTLEMENT TYPE DEFINITION
    // ==========================================================

    /// <summary>Reusable settlement type definition for data-driven design.</summary>
    public class SettlementTypeDefinition
    {
        public SettlementType Type { get; set; }
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public int MinPopulation { get; set; } = 0;
        public int MaxPopulation { get; set; } = 100;
        public int DefaultPopulation { get; set; } = 50;
        public float ProsperityFloor { get; set; } = 0.0f;
        public float ProsperityCeiling { get; set; } = 1.0f;
        public int MinBuildings { get; set; } = 1;
        public int MaxBuildings { get; set; } = 5;
        public List<BuildingCategory> AllowedBuildingCategories { get; set; } = new();
        public List<ServiceType> DefaultServices { get; set; } = new();
        public float SpawnDensity { get; set; } = 1.0f; // NPCs per unit population
        public bool HasWalls { get; set; } = false;
        public bool HasMarket { get; set; } = false;
        public bool HasInn { get; set; } = false;
        public bool HasTemple { get; set; } = false;
        public bool HasGuardBarracks { get; set; } = false;
        public string MusicProfile { get; set; } = "default";
        public string LocalizationKey { get; set; } = "";
        public int SortOrder { get; set; } = 0;

        /// <summary>Dynamic extension map for future DLC fields.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    // ==========================================================
    // BUILDING DATA
    // ==========================================================

    /// <summary>Complete building definition.</summary>
    public class BuildingData
    {
        public string BuildingId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string LocalizationKey { get; set; } = "";
        public BuildingCategory Category { get; set; } = BuildingCategory.Custom;
        public BuildingState State { get; set; } = BuildingState.Active;

        /// <summary>Interior scene resource path.</summary>
        public string InteriorScenePath { get; set; } = "";

        /// <summary>Exterior model resource path.</summary>
        public string ExteriorModelPath { get; set; } = "";

        /// <summary>Maximum NPCs that can occupy this building.</summary>
        public int NpcCapacity { get; set; } = 5;

        /// <summary>Operating hours (0.0-1.0 fraction of day).</summary>
        public float OpenTime { get; set; } = 0.25f; // 6:00 AM
        public float CloseTime { get; set; } = 0.85f; // 8:00 PM

        /// <summary>Services provided by this building.</summary>
        public List<ServiceType> Services { get; set; } = new();

        /// <summary>NPC owner ID (if owned).</summary>
        public string OwnerNpcId { get; set; } = "";

        /// <summary>Current upgrade level (0 = base).</summary>
        public int UpgradeLevel { get; set; } = 0;

        /// <summary>Maximum upgrade level.</summary>
        public int MaxUpgradeLevel { get; set; } = 3;

        /// <summary>Upgrade cost per level (gold).</summary>
        public List<int> UpgradeCosts { get; set; } = new();

        /// <summary>Building maintenance cost per day (gold).</summary>
        public int DailyMaintenanceCost { get; set; } = 0;

        /// <summary>Building revenue per day (gold).</summary>
        public int DailyRevenue { get; set; } = 0;

        /// <summary>World position offset from settlement center.</summary>
        public float PositionOffsetX { get; set; } = 0f;
        public float PositionOffsetZ { get; set; } = 0f;

        /// <summary>Building rotation (degrees).</summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>Is this building available by default.</summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>Required settlement type for this building.</summary>
        public SettlementType MinSettlementType { get; set; } = SettlementType.Camp;

        /// <summary>Dynamic extension map for future DLC fields.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    // ==========================================================
    // NPC SCHEDULE DATA (Expanded)
    // ==========================================================

    /// <summary>Expanded schedule block with detailed activity support.</summary>
    public class NpcScheduleBlock
    {
        public ScheduleActivity Activity { get; set; } = ScheduleActivity.Idle;
        public double TimeStart { get; set; } // 0.0-1.0 fraction of day
        public double TimeEnd { get; set; }
        public string LocationTag { get; set; } = "home";
        public string BuildingId { get; set; } = "";
        public int Priority { get; set; } = 0;
        public bool IsOverride { get; set; } = false;
        public WorldEventType RequiredEvent { get; set; } = WorldEventType.None;
        public WeatherCondition RequiredWeather { get; set; } = WeatherCondition.None;
        public bool Repeatable { get; set; } = true;
    }

    /// <summary>Weather condition enum for schedule adaptation.</summary>
    public enum WeatherCondition
    {
        None,
        Clear,
        Rain,
        Storm,
        Snow,
        Blizzard,
        Fog,
        Heatwave,
        Sandstorm,
        AshFall
    }

    /// <summary>Complete NPC schedule definition.</summary>
    public class NpcScheduleDefinition
    {
        public string ScheduleId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public NpcProfession Profession { get; set; } = NpcProfession.None;
        public List<NpcScheduleBlock> Blocks { get; set; } = new();
        public bool IsDefault { get; set; } = false;
    }

    // ==========================================================
    // PUBLIC SERVICE DATA
    // ==========================================================

    /// <summary>Reusable service definition.</summary>
    public class ServiceDefinition
    {
        public ServiceType Type { get; set; }
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public string LocalizationKey { get; set; } = "";
        public bool RequiresBuilding { get; set; } = true;
        public List<BuildingCategory> AllowedBuildings { get; set; } = new();
        public float BaseCost { get; set; } = 0f;
        public float QualityMultiplier { get; set; } = 1.0f;
        public int MinNpcLevel { get; set; } = 1;
        public bool IsPlayerUsable { get; set; } = true;
        public bool IsNpcUsable { get; set; } = true;
        public float UsageCooldown { get; set; } = 0f; // in-game hours
        public string DialogueHook { get; set; } = "";

        /// <summary>Dynamic extension map for future DLC fields.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    // ==========================================================
    // WORLD EVENT DATA
    // ==========================================================

    /// <summary>World event instance.</summary>
    public class WorldEventInstance
    {
        public string EventId { get; set; } = "";
        public WorldEventType Type { get; set; } = WorldEventType.None;
        public EventSeverity Severity { get; set; } = EventSeverity.Minor;
        public EventPhase Phase { get; set; } = EventPhase.Pending;
        public string SettlementId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public float DurationDays { get; set; } = 1f;
        public float ElapsedDays { get; set; } = 0f;
        public int TriggerDay { get; set; } = 0;
        public Dictionary<string, float> Effects { get; set; } = new(); // effectKey -> magnitude
        public List<string> AffectedBuildings { get; set; } = new();
        public bool IsRecurring { get; set; } = false;
        public float RecurIntervalDays { get; set; } = 0f;
        public string DialogueHook { get; set; } = "";

        /// <summary>Dynamic extension map for future DLC fields.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    /// <summary>World event template for data-driven creation.</summary>
    public class WorldEventTemplate
    {
        public WorldEventType Type { get; set; }
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public EventSeverity DefaultSeverity { get; set; } = EventSeverity.Minor;
        public float BaseDurationDays { get; set; } = 1f;
        public float MinProsperity { get; set; } = 0f;
        public float MaxProsperity { get; set; } = 1f;
        public List<SettlementType> AllowedSettlementTypes { get; set; } = new();
        public float TriggerWeight { get; set; } = 1.0f;
        public bool RequiresPlayerNearby { get; set; } = false;
        public float CooldownDays { get; set; } = 7f;
        public bool IsRecurring { get; set; } = false;
        public Dictionary<string, float> DefaultEffects { get; set; } = new();
        public string DialogueHook { get; set; } = "";

        /// <summary>Dynamic extension map for future DLC fields.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    // ==========================================================
    // SAVE DATA
    // ==========================================================

    /// <summary>Settlement save state for persistence.</summary>
    public class SettlementSaveState
    {
        public string SettlementId { get; set; } = "";
        public ProsperityLevel Prosperity { get; set; } = ProsperityLevel.Stable;
        public int Population { get; set; }
        public SecurityRating Security { get; set; } = SecurityRating.Low;
        public List<BuildingSaveState> BuildingStates { get; set; } = new();
        public List<WorldEventSaveState> ActiveEvents { get; set; } = new();
        public Dictionary<string, float> EconomyValues { get; set; } = new();
        public int Version { get; set; } = 1;
    }

    /// <summary>Building save state for persistence.</summary>
    public class BuildingSaveState
    {
        public string BuildingId { get; set; } = "";
        public BuildingState State { get; set; } = BuildingState.Active;
        public int UpgradeLevel { get; set; } = 0;
        public string OwnerNpcId { get; set; } = "";
        public float Condition { get; set; } = 1.0f; // 0.0-1.0
        public int Version { get; set; } = 1;
    }

    /// <summary>World event save state for persistence.</summary>
    public class WorldEventSaveState
    {
        public string EventId { get; set; } = "";
        public WorldEventType Type { get; set; }
        public EventSeverity Severity { get; set; }
        public EventPhase Phase { get; set; }
        public float ElapsedDays { get; set; }
        public int Version { get; set; } = 1;
    }

    /// <summary>NPC schedule save state for persistence.</summary>
    public class NpcScheduleSaveState
    {
        public string NpcId { get; set; } = "";
        public string ScheduleId { get; set; } = "";
        public ScheduleActivity CurrentActivity { get; set; } = ScheduleActivity.Idle;
        public string CurrentLocationTag { get; set; } = "home";
        public double CurrentActivityStartTime { get; set; } = 0f;
        public int Version { get; set; } = 1;
    }

    /// <summary>Complete settlement system save data for Save V14.</summary>
    public class SettlementSaveData
    {
        public List<SettlementSaveState> Settlements { get; set; } = new();
        public List<NpcScheduleSaveState> NpcSchedules { get; set; } = new();
        public List<WorldEventSaveState> GlobalEvents { get; set; } = new();
        public int Version { get; set; } = 1;
    }

    // ==========================================================
    // EVENTS
    // ==========================================================

    /// <summary>Event published when a settlement changes state.</summary>
    public class SettlementStateChangedEvent
    {
        public string SettlementId { get; set; } = "";
        public string PropertyName { get; set; } = "";
        public object OldValue { get; set; } = "";
        public object NewValue { get; set; } = "";
    }

    /// <summary>Event published when a world event triggers.</summary>
    public class WorldEventTriggeredEvent
    {
        public string EventId { get; set; } = "";
        public WorldEventType Type { get; set; }
        public EventSeverity Severity { get; set; }
        public string SettlementId { get; set; } = "";
        public string Description { get; set; } = "";
    }

    /// <summary>Event published when a building changes state.</summary>
    public class BuildingStateChangedEvent
    {
        public string BuildingId { get; set; } = "";
        public string SettlementId { get; set; } = "";
        public BuildingState OldState { get; set; }
        public BuildingState NewState { get; set; }
    }

    /// <summary>Event published when NPC schedule changes.</summary>
    public class NpcScheduleChangedEvent
    {
        public string NpcId { get; set; } = "";
        public ScheduleActivity OldActivity { get; set; }
        public ScheduleActivity NewActivity { get; set; }
        public string LocationTag { get; set; } = "";
    }

    /// <summary>Event published when a service is used.</summary>
    public class ServiceUsedEvent
    {
        public ServiceType ServiceType { get; set; }
        public string NpcId { get; set; } = "";
        public string BuildingId { get; set; } = "";
        public string SettlementId { get; set; } = "";
        public bool IsPlayer { get; set; }
    }
}