using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using HeroOfEternia.Core;

namespace HeroOfEternia.Gathering
{
    /// <summary>
    /// Biome constraint for resource spawns.
    /// </summary>
    public enum ResourceBiome
    {
        Any,
        Forest,
        Desert,
        Snow,
        Plains,
        Swamp,
        Volcanic,
        Ocean,
        Underground,
        Corrupted,
        MagicForest,
        CrystalCave,
        AncientRuins,
        Seasonal
    }

    /// <summary>
    /// Spawn condition type for resources.
    /// </summary>
    public enum SpawnCondition
    {
        Surface,
        Underground,
        Underwater,
        Cliffside,
        CaveWall,
        TreeTrunk,
        RockSurface,
        WaterSurface,
        NightOnly,
        DayOnly,
        RainOnly,
        Seasonal,
        CorruptionZone,
        MagicAura
    }

    /// <summary>
    /// Gather tool type required to harvest a resource.
    /// </summary>
    public enum ToolType
    {
        None,
        Axe,
        Pickaxe,
        Sickle,
        FishingRod,
        Shovel,
        Knife,
        Hammer,
        Shears,
        Staff,
        Gloves,
        MiningDrill,
        LumberAxe,
        AdvancedPickaxe,
        CrystalScythe
    }

    /// <summary>
    /// Resource category groups.
    /// </summary>
    public enum ResourceCategory
    {
        Wood,
        Ore,
        Stone,
        Plant,
        Herb,
        Water,
        Food,
        Crystal,
        Relic,
        Magic,
        Corrupted,
        Seasonal,
        Animal,
        Liquid,
        Gem
    }

    /// <summary>
    /// Resource subcategory for fine-grained grouping.
    /// </summary>
    public enum ResourceSubcategory
    {
        // Wood
        Softwood,
        Hardwood,
        AncientWood,
        MagicWood,
        CorruptedWood,
        
        // Ore
        BaseOre,
        PreciousOre,
        AlloyOre,
        MagicOre,
        
        // Stone
        BaseStone,
        Marble,
        Granite,
        Obsidian,
        
        // Plant
        Fiber,
        Flower,
        Vine,
        Moss,
        
        // Herb
        CommonHerb,
        MagicHerb,
        PoisonHerb,
        HealingHerb,
        
        // Water
        FreshWater,
        SaltWater,
        SpringWater,
        MagicWater,
        
        // Food
        Berry,
        Mushroom,
        Meat,
        Fish,
        Vegetable,
        
        // Crystal
        BaseCrystal,
        MagicCrystal,
        PowerCrystal,
        
        // Relic
        AncientRelic,
        Fossil,
        Artifact,
        
        // Magic
        ArcaneEssence,
        NatureEssence,
        VoidEssence,
        
        // Corrupted
        CorruptedEssence,
        CorruptedCrystal,
        
        // Seasonal
        Spring,
        Summer,
        Autumn,
        Winter,
        
        // Animal
        Bone,
        Hide,
        Fang,
        Feather,
        
        // Liquid
        Oil,
        Lava,
        Honey,
        Sap
    }

    /// <summary>
    /// Rarity weight for resource spawn chances.
    /// </summary>
    public enum ResourceRarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
        Mythic = 5
    }

    /// <summary>
    /// Complete resource definition record.
    /// Every resource in the game is defined by this data model.
    /// Supports future DLC expansion via ExtensionData.
    /// </summary>
    public class ResourceDefinition
    {
        /// <summary>Unique resource identifier (e.g. "res_oak_tree").</summary>
        public string UniqueId { get; set; } = string.Empty;
        
        /// <summary>Internal development name.</summary>
        public string InternalName { get; set; } = string.Empty;
        
        /// <summary>Localized display name.</summary>
        public string LocalizedName { get; set; } = string.Empty;
        
        /// <summary>Flavor text / description.</summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>Main resource category.</summary>
        public string Category { get; set; } = string.Empty;
        
        /// <summary>Resource subcategory.</summary>
        public string Subcategory { get; set; } = string.Empty;
        
        /// <summary>Primary biome where this resource spawns.</summary>
        public string Biome { get; set; } = "Any";
        
        /// <summary>Spawn condition type.</summary>
        public string SpawnCondition { get; set; } = "Surface";
        
        /// <summary>Spawn rarity weight (higher = more common).</summary>
        public int RarityWeight { get; set; } = 100;
        
        /// <summary>Weight per unit (kg).</summary>
        public float Weight { get; set; } = 1.0f;
        
        /// <summary>Maximum stack size per inventory slot.</summary>
        public int StackSize { get; set; } = 99;
        
        /// <summary>Tool type required to gather.</summary>
        public string ToolRequirement { get; set; } = "None";
        
        /// <summary>Minimum tool tier to gather (0 = no requirement).</summary>
        public int MinimumToolTier { get; set; } = 0;
        
        /// <summary>Time in seconds for resource to respawn.</summary>
        public float RespawnTimeSeconds { get; set; } = 120.0f;
        
        /// <summary>Path to 3D model resource.</summary>
        public string ModelPath { get; set; } = string.Empty;
        
        /// <summary>Path to UI icon.</summary>
        public string IconPath { get; set; } = string.Empty;
        
        /// <summary>Audio key for gather sound.</summary>
        public string AudioKey { get; set; } = string.Empty;
        
        /// <summary>Particle effect key for gather VFX.</summary>
        public string ParticleEffectKey { get; set; } = string.Empty;
        
        /// <summary>Animation key for gather animation.</summary>
        public string GatherAnimationKey { get; set; } = string.Empty;
        
        /// <summary>Experience awarded per gather action.</summary>
        public int BaseExperience { get; set; } = 10;
        
        /// <summary>Base gather time in seconds.</summary>
        public float BaseGatherTime { get; set; } = 2.0f;
        
        /// <summary>Base yield per gather action.</summary>
        public int BaseYield { get; set; } = 1;
        
        /// <summary>Maximum health of the resource node.</summary>
        public int NodeHealth { get; set; } = 1;
        
        /// <summary>Can this resource deplete and need respawn?</summary>
        public bool IsDepletable { get; set; } = true;
        
        /// <summary>Seasonal availability (empty = always).</summary>
        public string Season { get; set; } = string.Empty;
        
        /// <summary>Localization key.</summary>
        public string LocKey { get; set; } = string.Empty;
        
        /// <summary>Schema version.</summary>
        public int Version { get; set; } = 1;
        
        /// <summary>Future DLC / extension catch-all.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    /// <summary>
    /// Runtime resource node state in the world.
    /// Tracks depletion, respawn timers, and modification state.
    /// </summary>
    public class ResourceNodeState
    {
        /// <summary>Resource definition ID.</summary>
        public string ResourceId { get; set; } = string.Empty;
        
        /// <summary>World position (serialized as string for JSON compatibility).</summary>
        public string WorldPositionKey { get; set; } = string.Empty;
        
        /// <summary>Current health of the node.</summary>
        public int CurrentHealth { get; set; } = 1;
        
        /// <summary>Is the node fully depleted?</summary>
        public bool IsDepleted { get; set; }
        
        /// <summary>Remaining respawn time in seconds (0 = ready).</summary>
        public float RemainingRespawnTime { get; set; }
        
        /// <summary>Has the node been modified from its original state?</summary>
        public bool IsModified { get; set; }
        
        /// <summary>Chunk coordinates for lookup.</summary>
        public string ChunkKey { get; set; } = string.Empty;
        
        /// <summary>Local cell index within chunk.</summary>
        public int CellIndex { get; set; }
    }

    /// <summary>
    /// Data-driven resource database.
    /// Loads from Settings/resource_database.json.
    /// Supports unlimited resource definitions without code changes.
    /// </summary>
    public class ResourceDatabase : IInitializable
    {
        private static ResourceDatabase? _instance;
        public static ResourceDatabase Instance => _instance ??= new ResourceDatabase();

        private Dictionary<string, ResourceDefinition> _resources = new();
        private Dictionary<string, List<string>> _biomeIndex = new();
        private Dictionary<string, List<string>> _categoryIndex = new();
        private Dictionary<string, List<string>> _subcategoryIndex = new();
        private Dictionary<string, List<string>> _toolIndex = new();
        private bool _isInitialized;
        private bool _isLoaded;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            GD.Print("[ResourceDatabase] Initialized. Call LoadDatabase() to load resources.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _isLoaded = false;
            _resources.Clear();
            _biomeIndex.Clear();
            _categoryIndex.Clear();
            _subcategoryIndex.Clear();
            _toolIndex.Clear();
        }

        /// <summary>
        /// Loads resource definitions from a JSON file path.
        /// Can be called multiple times (reloads).
        /// </summary>
        public bool LoadDatabase(string filePath = "res://Settings/resource_database.json")
        {
            try
            {
                var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
                if (file == null)
                {
                    Logger.Error($"[ResourceDatabase] Failed to open file: {filePath}");
                    return false;
                }

                string json = file.GetAsText();
                file.Close();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var wrapper = JsonSerializer.Deserialize<ResourceDatabaseWrapper>(json, options);
                if (wrapper?.Resources == null)
                {
                    Logger.Error("[ResourceDatabase] Invalid database format.");
                    return false;
                }

                _resources.Clear();
                _biomeIndex.Clear();
                _categoryIndex.Clear();
                _subcategoryIndex.Clear();
                _toolIndex.Clear();

                foreach (var resource in wrapper.Resources)
                {
                    if (string.IsNullOrEmpty(resource.UniqueId))
                    {
                        Logger.Warning("[ResourceDatabase] Skipping resource with empty UniqueId.");
                        continue;
                    }

                    _resources[resource.UniqueId] = resource;

                    // Build biome index
                    string biome = string.IsNullOrEmpty(resource.Biome) ? "Any" : resource.Biome;
                    if (!_biomeIndex.ContainsKey(biome))
                        _biomeIndex[biome] = new List<string>();
                    _biomeIndex[biome].Add(resource.UniqueId);

                    // Build category index
                    string category = string.IsNullOrEmpty(resource.Category) ? "Unknown" : resource.Category;
                    if (!_categoryIndex.ContainsKey(category))
                        _categoryIndex[category] = new List<string>();
                    _categoryIndex[category].Add(resource.UniqueId);

                    // Build subcategory index
                    string subcat = string.IsNullOrEmpty(resource.Subcategory) ? "None" : resource.Subcategory;
                    if (!_subcategoryIndex.ContainsKey(subcat))
                        _subcategoryIndex[subcat] = new List<string>();
                    _subcategoryIndex[subcat].Add(resource.UniqueId);

                    // Build tool index
                    string tool = string.IsNullOrEmpty(resource.ToolRequirement) ? "None" : resource.ToolRequirement;
                    if (!_toolIndex.ContainsKey(tool))
                        _toolIndex[tool] = new List<string>();
                    _toolIndex[tool].Add(resource.UniqueId);
                }

                _isLoaded = true;
                GD.Print($"[ResourceDatabase] Loaded {_resources.Count} resources with {_biomeIndex.Count} biomes, {_categoryIndex.Count} categories.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"[ResourceDatabase] Load failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Gets a resource definition by ID. Returns null if not found.</summary>
        public ResourceDefinition? GetResource(string resourceId)
        {
            return _resources.TryGetValue(resourceId, out var r) ? r : null;
        }

        /// <summary>Returns all loaded resource definitions.</summary>
        public IEnumerable<ResourceDefinition> GetAllResources() => _resources.Values;

        /// <summary>Returns total count of loaded resources.</summary>
        public int ResourceCount => _resources.Count;

        /// <summary>Returns resources available in a specific biome.</summary>
        public List<string> GetResourcesByBiome(string biome)
        {
            var results = new List<string>();
            if (_biomeIndex.TryGetValue(biome, out var list))
                results.AddRange(list);
            if (biome != "Any" && _biomeIndex.TryGetValue("Any", out var anyList))
                results.AddRange(anyList);
            return results;
        }

        /// <summary>Returns resources by category.</summary>
        public List<string> GetResourcesByCategory(string category)
        {
            return _categoryIndex.TryGetValue(category, out var list) 
                ? new List<string>(list) 
                : new List<string>();
        }

        /// <summary>Returns resources by subcategory.</summary>
        public List<string> GetResourcesBySubcategory(string subcategory)
        {
            return _subcategoryIndex.TryGetValue(subcategory, out var list) 
                ? new List<string>(list) 
                : new List<string>();
        }

        /// <summary>Returns resources requiring a specific tool.</summary>
        public List<string> GetResourcesByTool(string toolType)
        {
            return _toolIndex.TryGetValue(toolType, out var list) 
                ? new List<string>(list) 
                : new List<string>();
        }

        /// <summary>Fast lookup check if a resource exists.</summary>
        public bool HasResource(string resourceId) => _resources.ContainsKey(resourceId);

        public bool IsInitialized => _isInitialized;
        public bool IsLoaded => _isLoaded;
    }

    /// <summary>
    /// JSON wrapper for resource database deserialization.
    /// </summary>
    internal class ResourceDatabaseWrapper
    {
        public string Version { get; set; } = "1.0";
        public List<ResourceDefinition> Resources { get; set; } = new();
    }
}