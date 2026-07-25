using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Gathering
{
    /// <summary>
    /// Profession types supported in the game.
    /// String-based for extensibility — new professions can be added without code changes.
    /// </summary>
    public enum ProfessionType
    {
        Woodcutting,
        Mining,
        Fishing,
        Cooking,
        Blacksmithing,
        Alchemy,
        Tailoring,
        Leatherworking,
        Carpentry,
        Engineering,
        Jewelry,
        Enchanting,
        Farming,
        AnimalCare
    }

    /// <summary>
    /// Profession experience curve type.
    /// </summary>
    public enum XpCurveType
    {
        Linear,
        Moderate,
        Steep,
        Exponential
    }

    /// <summary>
    /// Per-profession data definition.
    /// Each profession tracks its own experience, level, unlocks, and bonuses.
    /// </summary>
    public class ProfessionData
    {
        public ProfessionType Type { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
        public int Experience { get; set; }
        public XpCurveType CurveType { get; set; } = XpCurveType.Moderate;
        
        /// <summary>Base XP required to reach level 2.</summary>
        public int BaseXpRequired { get; set; } = 100;
        
        /// <summary>XP growth factor per level (e.g. 1.2 = 20% increase per level).</summary>
        public float XpGrowthFactor { get; set; } = 1.15f;
        
        /// <summary>Maximum possible level.</summary>
        public int MaxLevel { get; set; } = 100;
        
        /// <summary>Unlock IDs granted at various levels (format: "level:unlock_id").</summary>
        public List<string> Unlocks { get; set; } = new();
        
        /// <summary>Active bonus modifiers keyed by bonus type.</summary>
        public Dictionary<string, float> Bonuses { get; set; } = new();
        
        /// <summary>Achievement IDs earned for this profession.</summary>
        public List<string> Achievements { get; set; } = new();
        
        /// <summary>Future specialization slot.</summary>
        public string Specialization { get; set; } = string.Empty;
        
        /// <summary>Is this profession unlocked for the player?</summary>
        public bool IsUnlocked { get; set; } = true;

        /// <summary>Returns XP required for the next level.</summary>
        public int XpForNextLevel()
        {
            return CalculateXpForLevel(Level + 1);
        }

        /// <summary>Returns total XP required to reach a specific level.</summary>
        public int CalculateXpForLevel(int targetLevel)
        {
            if (targetLevel <= 1) return 0;
            float total = 0;
            for (int i = 1; i < targetLevel; i++)
            {
                total += BaseXpRequired * Mathf.Pow(XpGrowthFactor, i - 1);
            }
            return Mathf.RoundToInt(total);
        }

        /// <summary>Adds experience and returns levels gained.</summary>
        public int AddExperience(int amount, out bool leveledUp)
        {
            leveledUp = false;
            if (Level >= MaxLevel) return 0;
            
            Experience += amount;
            int levelsGained = 0;
            
            while (Level < MaxLevel && Experience >= XpForNextLevel())
            {
                Experience -= XpForNextLevel();
                Level++;
                levelsGained++;
                leveledUp = true;
                
                // Check for unlocks at this level
                string unlockKey = $"{Level}:";
                foreach (var unlock in Unlocks.Where(u => u.StartsWith(unlockKey)))
                {
                    string[] parts = unlock.Split(':');
                    if (parts.Length >= 2)
                    {
                        string unlockId = parts[1];
                        GD.Print($"[ProfessionSystem] Unlocked: {unlockId} for {DisplayName} (Level {Level})");
                    }
                }
                
                if (Level >= MaxLevel)
                {
                    Experience = 0;
                    break;
                }
            }
            
            return levelsGained;
        }

        /// <summary>Checks if a specific unlock is available at the current level.</summary>
        public bool HasUnlock(string unlockId)
        {
            return Unlocks.Any(u => u.EndsWith($":{unlockId}") && 
                int.TryParse(u.Split(':')[0], out int level) && Level >= level);
        }

        /// <summary>Gets the bonus value for a specific bonus key.</summary>
        public float GetBonus(string bonusKey, float defaultValue = 0f)
        {
            return Bonuses.TryGetValue(bonusKey, out float value) ? value : defaultValue;
        }
    }

    /// <summary>
    /// Save snapshot for a single profession's state.
    /// </summary>
    public class ProfessionSaveState
    {
        public string Type { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
        public int Experience { get; set; }
        public bool IsUnlocked { get; set; } = true;
        public string Specialization { get; set; } = string.Empty;
        public List<string> Achievements { get; set; } = new();
    }

    /// <summary>
    /// Central profession manager service.
    /// Handles all 14+ professions with XP, leveling, unlocks, and bonuses.
    /// Designers can add new professions without code changes.
    /// </summary>
    public class ProfessionManager : IInitializable
    {
        private static ProfessionManager? _instance;
        public static ProfessionManager Instance => _instance ??= new ProfessionManager();

        private Dictionary<ProfessionType, ProfessionData> _professions = new();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            InitializeDefaultProfessions();
            GD.Print("[ProfessionManager] Initialized with 14 professions.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _professions.Clear();
        }

        private void InitializeDefaultProfessions()
        {
            _professions.Clear();

            // Woodcutting
            _professions[ProfessionType.Woodcutting] = new ProfessionData
            {
                Type = ProfessionType.Woodcutting,
                DisplayName = "Woodcutting",
                Description = "Chop trees and gather wood resources.",
                CurveType = XpCurveType.Moderate,
                BaseXpRequired = 100,
                XpGrowthFactor = 1.15f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_iron_axe", "10:unlock_steel_axe", "15:unlock_double_yield",
                    "20:unlock_ancient_wood", "25:unlock_lumber_axe",
                    "30:unlock_critical_chop", "40:unlock_magic_wood",
                    "50:unlock_bonus_yield_50", "75:unlock_instant_chop", "100:unlock_woodcutting_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "gather_speed", 1.0f },
                    { "yield_bonus", 0.0f },
                    { "critical_chance", 0.05f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Mining
            _professions[ProfessionType.Mining] = new ProfessionData
            {
                Type = ProfessionType.Mining,
                DisplayName = "Mining",
                Description = "Mine ores, stone, and crystals from deposits.",
                CurveType = XpCurveType.Moderate,
                BaseXpRequired = 120,
                XpGrowthFactor = 1.15f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_iron_pick", "10:unlock_steel_pick", "15:unlock_double_ore",
                    "20:unlock_gold_vein", "25:unlock_advanced_pick",
                    "30:unlock_critical_mine", "40:unlock_mythril",
                    "50:unlock_bonus_ore_50", "75:unlock_instant_mine", "100:unlock_mining_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "gather_speed", 1.0f },
                    { "yield_bonus", 0.0f },
                    { "critical_chance", 0.05f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Fishing
            _professions[ProfessionType.Fishing] = new ProfessionData
            {
                Type = ProfessionType.Fishing,
                DisplayName = "Fishing",
                Description = "Catch fish and aquatic resources from water sources.",
                CurveType = XpCurveType.Linear,
                BaseXpRequired = 80,
                XpGrowthFactor = 1.10f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_fiber_rod", "10:unlock_iron_hook", "15:unlock_bait",
                    "20:unlock_rare_fish", "25:unlock_net",
                    "30:unlock_bonus_catch", "40:unlock_magic_fish",
                    "50:unlock_deep_sea", "75:unlock_instant_catch", "100:unlock_fishing_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "gather_speed", 1.0f },
                    { "yield_bonus", 0.0f },
                    { "rare_chance", 0.02f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Cooking
            _professions[ProfessionType.Cooking] = new ProfessionData
            {
                Type = ProfessionType.Cooking,
                DisplayName = "Cooking",
                Description = "Cook food and create consumables for buffs.",
                CurveType = XpCurveType.Linear,
                BaseXpRequired = 60,
                XpGrowthFactor = 1.12f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_grill", "10:unlock_recipe_tier2", "15:unlock_stew",
                    "20:unlock_recipe_tier3", "25:unlock_feast",
                    "30:unlock_bonus_effect", "40:unlock_gourmet",
                    "50:unlock_recipe_tier4", "75:unlock_instant_cook", "100:unlock_cooking_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "quality_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Blacksmithing
            _professions[ProfessionType.Blacksmithing] = new ProfessionData
            {
                Type = ProfessionType.Blacksmithing,
                DisplayName = "Blacksmithing",
                Description = "Forge weapons, armor, and tools from metal.",
                CurveType = XpCurveType.Steep,
                BaseXpRequired = 150,
                XpGrowthFactor = 1.18f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_iron_recipes", "10:unlock_steel_recipes", "15:unlock_upgrade",
                    "20:unlock_mythril_recipes", "25:unlock_enhance",
                    "30:unlock_bonus_stats", "40:unlock_legendary_recipes",
                    "50:unlock_masterwork", "75:unlock_instant_forge", "100:unlock_blacksmithing_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "quality_bonus", 0.0f },
                    { "stat_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Alchemy
            _professions[ProfessionType.Alchemy] = new ProfessionData
            {
                Type = ProfessionType.Alchemy,
                DisplayName = "Alchemy",
                Description = "Brew potions, elixirs, and magical concoctions.",
                CurveType = XpCurveType.Steep,
                BaseXpRequired = 130,
                XpGrowthFactor = 1.16f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_health_potion_2", "10:unlock_mana_potion", "15:unlock_elixir",
                    "20:unlock_antidote", "25:unlock_strength_potion",
                    "30:unlock_bonus_duration", "40:unlock_greater_potions",
                    "50:unlock_legendary_elixir", "75:unlock_instant_brew", "100:unlock_alchemy_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "potency_bonus", 0.0f },
                    { "duration_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Tailoring
            _professions[ProfessionType.Tailoring] = new ProfessionData
            {
                Type = ProfessionType.Tailoring,
                DisplayName = "Tailoring",
                Description = "Craft cloth armor, bags, and decorative items.",
                CurveType = XpCurveType.Moderate,
                BaseXpRequired = 100,
                XpGrowthFactor = 1.14f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_linen_recipes", "10:unlock_silk_recipes", "15:unlock_bags",
                    "20:unlock_magic_fabric", "25:unlock_embroidery",
                    "30:unlock_bonus_armor", "40:unlock_enchanted_fabric",
                    "50:unlock_legendary_robe", "75:unlock_instant_sew", "100:unlock_tailoring_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "quality_bonus", 0.0f },
                    { "armor_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Leatherworking
            _professions[ProfessionType.Leatherworking] = new ProfessionData
            {
                Type = ProfessionType.Leatherworking,
                DisplayName = "Leatherworking",
                Description = "Craft leather armor and accessories from hides.",
                CurveType = XpCurveType.Moderate,
                BaseXpRequired = 110,
                XpGrowthFactor = 1.14f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_leather_recipes", "10:unlock_hardened_leather", "15:unlock_scale_armor",
                    "20:unlock_dragonhide", "25:unlock_quiver",
                    "30:unlock_bonus_armor", "40:unlock_magic_leather",
                    "50:unlock_legendary_armor", "75:unlock_instant_craft", "100:unlock_leatherworking_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "quality_bonus", 0.0f },
                    { "durability_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Carpentry
            _professions[ProfessionType.Carpentry] = new ProfessionData
            {
                Type = ProfessionType.Carpentry,
                DisplayName = "Carpentry",
                Description = "Build furniture, wooden structures, and tools.",
                CurveType = XpCurveType.Moderate,
                BaseXpRequired = 90,
                XpGrowthFactor = 1.13f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_wooden_tools", "10:unlock_furniture", "15:unlock_enhanced_wood",
                    "20:unlock_ancient_woodworking", "25:unlock_structural",
                    "30:unlock_bonus_durability", "40:unlock_magic_carpentry",
                    "50:unlock_masterwork_furniture", "75:unlock_instant_build", "100:unlock_carpentry_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "durability_bonus", 0.0f },
                    { "quality_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Engineering
            _professions[ProfessionType.Engineering] = new ProfessionData
            {
                Type = ProfessionType.Engineering,
                DisplayName = "Engineering",
                Description = "Create mechanical devices, gadgets, and explosives.",
                CurveType = XpCurveType.Exponential,
                BaseXpRequired = 200,
                XpGrowthFactor = 1.20f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_basic_gadgets", "10:unlock_explosives", "15:unlock_mechanical",
                    "20:unlock_advanced_gadgets", "25:unlock_auto_turret",
                    "30:unlock_bonus_damage", "40:unlock_magic_tech",
                    "50:unlock_legendary_device", "75:unlock_instant_assemble", "100:unlock_engineering_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "damage_bonus", 0.0f },
                    { "durability_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Jewelry
            _professions[ProfessionType.Jewelry] = new ProfessionData
            {
                Type = ProfessionType.Jewelry,
                DisplayName = "Jewelry",
                Description = "Craft rings, necklaces, and gem-studded accessories.",
                CurveType = XpCurveType.Exponential,
                BaseXpRequired = 180,
                XpGrowthFactor = 1.19f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_copper_rings", "10:unlock_silver_jewelry", "15:unlock_gem_setting",
                    "20:unlock_gold_jewelry", "25:unlock_enchanted_gems",
                    "30:unlock_bonus_stats", "40:unlock_mythril_jewelry",
                    "50:unlock_legendary_gems", "75:unlock_instant_set", "100:unlock_jewelry_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "craft_speed", 1.0f },
                    { "quality_bonus", 0.0f },
                    { "stat_bonus", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Enchanting
            _professions[ProfessionType.Enchanting] = new ProfessionData
            {
                Type = ProfessionType.Enchanting,
                DisplayName = "Enchanting",
                Description = "Enchant weapons and armor with magical properties.",
                CurveType = XpCurveType.Exponential,
                BaseXpRequired = 250,
                XpGrowthFactor = 1.22f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_basic_enchant", "10:unlock_fire_enchant", "15:unlock_ice_enchant",
                    "20:unlock_lightning_enchant", "25:unlock_holy_enchant",
                    "30:unlock_bonus_power", "40:unlock_shadow_enchant",
                    "50:unlock_legendary_enchant", "75:unlock_instant_enchant", "100:unlock_enchanting_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "enchant_power", 1.0f },
                    { "quality_bonus", 0.0f },
                    { "success_rate", 0.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Farming
            _professions[ProfessionType.Farming] = new ProfessionData
            {
                Type = ProfessionType.Farming,
                DisplayName = "Farming",
                Description = "Cultivate crops, raise plants, and harvest resources.",
                CurveType = XpCurveType.Linear,
                BaseXpRequired = 70,
                XpGrowthFactor = 1.10f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_basic_seeds", "10:unlock_fertilizer", "15:unlock_irrigation",
                    "20:unlock_rare_seeds", "25:unlock_greenhouse",
                    "30:unlock_bonus_yield", "40:unlock_magic_seeds",
                    "50:unlock_instant_growth", "75:unlock_master_farmer", "100:unlock_farming_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "gather_speed", 1.0f },
                    { "yield_bonus", 0.0f },
                    { "growth_speed", 1.0f },
                    { "xp_bonus", 1.0f }
                }
            };

            // Animal Care
            _professions[ProfessionType.AnimalCare] = new ProfessionData
            {
                Type = ProfessionType.AnimalCare,
                DisplayName = "Animal Care",
                Description = "Tend to animals, gather animal products, and raise pets.",
                CurveType = XpCurveType.Linear,
                BaseXpRequired = 80,
                XpGrowthFactor = 1.10f,
                MaxLevel = 100,
                IsUnlocked = true,
                Unlocks = new List<string>
                {
                    "5:unlock_basic_tending", "10:unlock_breeding", "15:unlock_animal_feed",
                    "20:unlock_rare_breeds", "25:unlock_training",
                    "30:unlock_bonus_yield", "40:unlock_magic_beasts",
                    "50:unlock_legendary_breed", "75:unlock_master_tamer", "100:unlock_animal_care_mastery"
                },
                Bonuses = new Dictionary<string, float>
                {
                    { "gather_speed", 1.0f },
                    { "yield_bonus", 0.0f },
                    { "taming_chance", 0.05f },
                    { "xp_bonus", 1.0f }
                }
            };
        }

        /// <summary>Gets profession data by type. Returns null if not found.</summary>
        public ProfessionData? GetProfession(ProfessionType type)
        {
            return _professions.TryGetValue(type, out var data) ? data : null;
        }

        /// <summary>Gets profession data by name string (case-insensitive).</summary>
        public ProfessionData? GetProfessionByName(string name)
        {
            foreach (var kvp in _professions)
            {
                if (kvp.Value.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }
            return null;
        }

        /// <summary>Returns all registered professions.</summary>
        public IEnumerable<ProfessionData> GetAllProfessions() => _professions.Values;

        /// <summary>Adds experience to a profession. Returns levels gained.</summary>
        public int AddExperience(ProfessionType type, int amount)
        {
            var prof = GetProfession(type);
            if (prof == null || !prof.IsUnlocked) return 0;
            
            float xpBonus = prof.GetBonus("xp_bonus", 1.0f);
            int adjustedAmount = Mathf.RoundToInt(amount * xpBonus);
            
            int levelsGained = prof.AddExperience(adjustedAmount, out bool leveledUp);
            
            if (leveledUp)
            {
                GD.Print($"[ProfessionManager] {prof.DisplayName} reached level {prof.Level}!");
            }
            
            return levelsGained;
        }

        /// <summary>Checks if a profession meets the level requirement.</summary>
        public bool MeetsRequirement(ProfessionType type, int requiredLevel)
        {
            var prof = GetProfession(type);
            return prof != null && prof.IsUnlocked && prof.Level >= requiredLevel;
        }

        /// <summary>Gets a profession's bonus value.</summary>
        public float GetBonus(ProfessionType type, string bonusKey, float defaultValue = 0f)
        {
            return GetProfession(type)?.GetBonus(bonusKey, defaultValue) ?? defaultValue;
        }

        /// <summary>Exports all profession states for save serialization.</summary>
        public List<ProfessionSaveState> ExportStates()
        {
            var states = new List<ProfessionSaveState>();
            foreach (var kvp in _professions)
            {
                states.Add(new ProfessionSaveState
                {
                    Type = kvp.Key.ToString(),
                    Level = kvp.Value.Level,
                    Experience = kvp.Value.Experience,
                    IsUnlocked = kvp.Value.IsUnlocked,
                    Specialization = kvp.Value.Specialization,
                    Achievements = new List<string>(kvp.Value.Achievements)
                });
            }
            return states;
        }

        /// <summary>Restores profession states from save data.</summary>
        public void RestoreStates(List<ProfessionSaveState> states)
        {
            if (states == null) return;
            
            foreach (var state in states)
            {
                if (Enum.TryParse<ProfessionType>(state.Type, out var type) && _professions.ContainsKey(type))
                {
                    _professions[type].Level = state.Level;
                    _professions[type].Experience = state.Experience;
                    _professions[type].IsUnlocked = state.IsUnlocked;
                    _professions[type].Specialization = state.Specialization;
                    _professions[type].Achievements = state.Achievements ?? new List<string>();
                }
            }
        }

        public bool IsInitialized => _isInitialized;
    }
}