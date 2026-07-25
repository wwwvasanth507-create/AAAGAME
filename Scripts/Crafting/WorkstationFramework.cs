using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Crafting
{
    /// <summary>
    /// Workstation type definitions.
    /// Each workstation supports specific professions and recipes.
    /// </summary>
    public enum WorkstationType
    {
        None,
        Campfire,
        Forge,
        Anvil,
        Workbench,
        AlchemyTable,
        CookingPot,
        TailorBench,
        EnchantingTable,
        JewelryStation,
        Smelter,
        Grinder,
        Loom,
        TanningRack,
        Sawmill,
        AdvancedForge,
        ArcaneAltar
    }

    /// <summary>
    /// Workstation tier/quality level.
    /// </summary>
    public enum WorkstationTier
    {
        Basic = 1,
        Standard = 2,
        Advanced = 3,
        Masterwork = 4,
        Legendary = 5
    }

    /// <summary>
    /// Definition of a workstation in the game world.
    /// </summary>
    public class WorkstationDefinition
    {
        /// <summary>Unique workstation identifier.</summary>
        public string UniqueId { get; set; } = string.Empty;
        
        /// <summary>Workstation type.</summary>
        public string Type { get; set; } = string.Empty;
        
        /// <summary>Display name.</summary>
        public string DisplayName { get; set; } = string.Empty;
        
        /// <summary>Description.</summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>Workstation tier (1-5).</summary>
        public int Tier { get; set; } = 1;
        
        /// <summary>Professions supported by this workstation.</summary>
        public List<string> SupportedProfessions { get; set; } = new();
        
        /// <summary>Craft speed multiplier (1.0 = normal).</summary>
        public float CraftSpeedMultiplier { get; set; } = 1.0f;
        
        /// <summary>Quality bonus multiplier.</summary>
        public float QualityBonus { get; set; } = 0.0f;
        
        /// <summary>Success rate bonus (added to recipe success chance).</summary>
        public float SuccessRateBonus { get; set; } = 0.0f;
        
        /// <summary>Experience bonus multiplier.</summary>
        public float ExperienceBonus { get; set; } = 1.0f;
        
        /// <summary>Resource cost reduction (0.0 = none, 0.5 = 50% less).</summary>
        public float CostReduction { get; set; } = 0.0f;
        
        /// <summary>Model path for 3D representation.</summary>
        public string ModelPath { get; set; } = string.Empty;
        
        /// <summary>Icon path for UI.</summary>
        public string IconPath { get; set; } = string.Empty;
        
        /// <summary>Audio key for interaction.</summary>
        public string AudioKey { get; set; } = string.Empty;
        
        /// <summary>Particle effect key for active use.</summary>
        public string ParticleEffectKey { get; set; } = string.Empty;
        
        /// <summary>Is this workstation portable (can be carried in inventory)?</summary>
        public bool IsPortable { get; set; }
        
        /// <summary>Schema version.</summary>
        public int Version { get; set; } = 1;
    }

    /// <summary>
    /// Runtime state of a workstation placed in the world.
    /// </summary>
    public class WorkstationState
    {
        public string UniqueId { get; set; } = string.Empty;
        public string DefinitionId { get; set; } = string.Empty;
        public string WorldPositionKey { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public float Durability { get; set; } = 100f;
        public float MaxDurability { get; set; } = 100f;
        public string OwnerId { get; set; } = string.Empty;
        public bool IsPlayerPlaced { get; set; }
    }

    /// <summary>
    /// Manages workstation definitions and runtime states.
    /// No placement gameplay — just definitions and state tracking.
    /// </summary>
    public class WorkstationManager : IInitializable
    {
        private static WorkstationManager? _instance;
        public static WorkstationManager Instance => _instance ??= new WorkstationManager();

        private Dictionary<string, WorkstationDefinition> _definitions = new();
        private Dictionary<string, WorkstationState> _states = new();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            InitializeDefaultWorkstations();
            GD.Print("[WorkstationManager] Initialized with 16 workstation types.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _definitions.Clear();
            _states.Clear();
        }

        private void InitializeDefaultWorkstations()
        {
            _definitions.Clear();

            _definitions["ws_campfire"] = new WorkstationDefinition
            {
                UniqueId = "ws_campfire",
                Type = "Campfire",
                DisplayName = "Campfire",
                Description = "A basic campfire for cooking and simple crafting.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Cooking" },
                CraftSpeedMultiplier = 0.8f,
                QualityBonus = 0.0f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_forge"] = new WorkstationDefinition
            {
                UniqueId = "ws_forge",
                Type = "Forge",
                DisplayName = "Forge",
                Description = "A coal-fired forge for smelting ores and heating metal.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Blacksmithing" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.0f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_anvil"] = new WorkstationDefinition
            {
                UniqueId = "ws_anvil",
                Type = "Anvil",
                DisplayName = "Anvil",
                Description = "A sturdy anvil for shaping metal into weapons and armor.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Blacksmithing" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.05f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_workbench"] = new WorkstationDefinition
            {
                UniqueId = "ws_workbench",
                Type = "Workbench",
                DisplayName = "Workbench",
                Description = "A general-purpose workbench for carpentry and engineering.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Carpentry", "Engineering" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.0f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_alchemy_table"] = new WorkstationDefinition
            {
                UniqueId = "ws_alchemy_table",
                Type = "AlchemyTable",
                DisplayName = "Alchemy Table",
                Description = "A table equipped with alchemical apparatus for brewing potions.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Alchemy" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.05f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_cooking_pot"] = new WorkstationDefinition
            {
                UniqueId = "ws_cooking_pot",
                Type = "CookingPot",
                DisplayName = "Cooking Pot",
                Description = "A large cooking pot for preparing meals and stews.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Cooking" },
                CraftSpeedMultiplier = 1.2f,
                QualityBonus = 0.0f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_tailor_bench"] = new WorkstationDefinition
            {
                UniqueId = "ws_tailor_bench",
                Type = "TailorBench",
                DisplayName = "Tailor Bench",
                Description = "A bench with sewing tools for tailoring cloth items.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Tailoring" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.05f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_enchanting_table"] = new WorkstationDefinition
            {
                UniqueId = "ws_enchanting_table",
                Type = "EnchantingTable",
                DisplayName = "Enchanting Table",
                Description = "An arcane table for enchanting weapons and armor.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Enchanting" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.1f,
                SuccessRateBonus = 0.05f,
                ExperienceBonus = 1.2f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_jewelry_station"] = new WorkstationDefinition
            {
                UniqueId = "ws_jewelry_station",
                Type = "JewelryStation",
                DisplayName = "Jewelry Station",
                Description = "A precision workbench for crafting rings, necklaces, and gems.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Jewelry" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.1f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_smelter"] = new WorkstationDefinition
            {
                UniqueId = "ws_smelter",
                Type = "Smelter",
                DisplayName = "Smelter",
                Description = "A furnace for smelting ores into ingots.",
                Tier = 2,
                SupportedProfessions = new List<string> { "Blacksmithing", "Mining" },
                CraftSpeedMultiplier = 1.5f,
                QualityBonus = 0.0f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.1f,
                CostReduction = 0.1f,
                IsPortable = false
            };

            _definitions["ws_grinder"] = new WorkstationDefinition
            {
                UniqueId = "ws_grinder",
                Type = "Grinder",
                DisplayName = "Grinder",
                Description = "A grinding wheel for sharpening tools and processing materials.",
                Tier = 2,
                SupportedProfessions = new List<string> { "Blacksmithing", "Carpentry" },
                CraftSpeedMultiplier = 1.3f,
                QualityBonus = 0.05f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_loom"] = new WorkstationDefinition
            {
                UniqueId = "ws_loom",
                Type = "Loom",
                DisplayName = "Loom",
                Description = "A weaving loom for creating cloth and fabrics.",
                Tier = 2,
                SupportedProfessions = new List<string> { "Tailoring" },
                CraftSpeedMultiplier = 1.2f,
                QualityBonus = 0.05f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_tanning_rack"] = new WorkstationDefinition
            {
                UniqueId = "ws_tanning_rack",
                Type = "TanningRack",
                DisplayName = "Tanning Rack",
                Description = "A rack for curing hides into leather.",
                Tier = 1,
                SupportedProfessions = new List<string> { "Leatherworking" },
                CraftSpeedMultiplier = 1.0f,
                QualityBonus = 0.0f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.0f,
                CostReduction = 0.0f,
                IsPortable = false
            };

            _definitions["ws_sawmill"] = new WorkstationDefinition
            {
                UniqueId = "ws_sawmill",
                Type = "Sawmill",
                DisplayName = "Sawmill",
                Description = "A sawmill for processing logs into planks.",
                Tier = 2,
                SupportedProfessions = new List<string> { "Carpentry", "Woodcutting" },
                CraftSpeedMultiplier = 1.5f,
                QualityBonus = 0.0f,
                SuccessRateBonus = 0.0f,
                ExperienceBonus = 1.1f,
                CostReduction = 0.1f,
                IsPortable = false
            };

            _definitions["ws_advanced_forge"] = new WorkstationDefinition
            {
                UniqueId = "ws_advanced_forge",
                Type = "AdvancedForge",
                DisplayName = "Advanced Forge",
                Description = "A masterwork forge capable of creating legendary items.",
                Tier = 4,
                SupportedProfessions = new List<string> { "Blacksmithing" },
                CraftSpeedMultiplier = 2.0f,
                QualityBonus = 0.2f,
                SuccessRateBonus = 0.1f,
                ExperienceBonus = 1.5f,
                CostReduction = 0.2f,
                IsPortable = false
            };

            _definitions["ws_arcane_altar"] = new WorkstationDefinition
            {
                UniqueId = "ws_arcane_altar",
                Type = "ArcaneAltar",
                DisplayName = "Arcane Altar",
                Description = "A powerful arcane altar for the most advanced enchantments.",
                Tier = 5,
                SupportedProfessions = new List<string> { "Enchanting", "Alchemy" },
                CraftSpeedMultiplier = 2.5f,
                QualityBonus = 0.3f,
                SuccessRateBonus = 0.15f,
                ExperienceBonus = 2.0f,
                CostReduction = 0.25f,
                IsPortable = false
            };
        }

        /// <summary>Gets a workstation definition by ID.</summary>
        public WorkstationDefinition? GetDefinition(string uniqueId)
        {
            return _definitions.TryGetValue(uniqueId, out var def) ? def : null;
        }

        /// <summary>Gets a workstation definition by type string.</summary>
        public WorkstationDefinition? GetDefinitionByType(string type)
        {
            foreach (var kvp in _definitions)
            {
                if (kvp.Value.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }
            return null;
        }

        /// <summary>Returns all workstation definitions.</summary>
        public IEnumerable<WorkstationDefinition> GetAllDefinitions() => _definitions.Values;

        /// <summary>Returns workstations supporting a specific profession.</summary>
        public List<WorkstationDefinition> GetWorkstationsForProfession(string profession)
        {
            return _definitions.Values
                .Where(w => w.SupportedProfessions.Contains(profession, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>Registers a workstation state in the world.</summary>
        public void RegisterState(WorkstationState state)
        {
            _states[state.UniqueId] = state;
        }

        /// <summary>Gets a workstation state by ID.</summary>
        public WorkstationState? GetState(string uniqueId)
        {
            return _states.TryGetValue(uniqueId, out var state) ? state : null;
        }

        /// <summary>Applies workstation bonuses to a craft operation.</summary>
        public (float speedMult, float qualityBonus, float successBonus, float xpMult, float costReduction) 
            GetWorkstationBonuses(string workstationType)
        {
            var def = GetDefinitionByType(workstationType);
            if (def == null)
                return (1.0f, 0.0f, 0.0f, 1.0f, 0.0f);

            return (def.CraftSpeedMultiplier, def.QualityBonus, def.SuccessRateBonus, 
                    def.ExperienceBonus, def.CostReduction);
        }

        public bool IsInitialized => _isInitialized;
    }
}