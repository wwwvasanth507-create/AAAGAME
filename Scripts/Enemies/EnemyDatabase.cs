using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Enemies
{
    /// <summary>
    /// EnemyDatabase is the authoritative registry of all enemy definitions.
    /// Loads from Settings/enemy_database.json at startup.
    /// Falls back to embedded defaults if the file is missing.
    /// </summary>
    public class EnemyDatabase
    {
        private readonly Dictionary<string, EnemyDefinition> _registry = new(StringComparer.OrdinalIgnoreCase);
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        // ----------------------------------------------------------------
        // Initialisation
        // ----------------------------------------------------------------
        public void Load(string settingsDir)
        {
            string path = Path.Combine(settingsDir, "enemy_database.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<List<EnemyData>>(json, JsonOpts);
                    if (list != null) Register(list);
                    Logger.Info($"EnemyDatabase: Loaded {_registry.Count} enemies from '{path}'.");
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Error($"EnemyDatabase: Failed to parse '{path}': {ex.Message}. Using defaults.");
                }
            }
            else
            {
                Logger.Warning($"EnemyDatabase: '{path}' not found. Using embedded defaults.");
            }

            RegisterDefaults();
        }

        // ----------------------------------------------------------------
        // Lookup
        // ----------------------------------------------------------------
        public EnemyDefinition? Get(string enemyId)
        {
            _registry.TryGetValue(enemyId, out var def);
            return def;
        }

        public EnemyDefinition GetOrThrow(string enemyId)
        {
            if (_registry.TryGetValue(enemyId, out var def)) return def;
            throw new KeyNotFoundException($"EnemyDatabase: No enemy found with ID '{enemyId}'.");
        }

        public IReadOnlyCollection<EnemyDefinition> GetAll() => _registry.Values;

        public bool Contains(string enemyId) => _registry.ContainsKey(enemyId);

        public int Count => _registry.Count;

        // ----------------------------------------------------------------
        // Registration helpers
        // ----------------------------------------------------------------
        private void Register(List<EnemyData> list)
        {
            foreach (var data in list)
            {
                try
                {
                    var def = new EnemyDefinition(data);
                    _registry[data.EnemyId] = def;
                }
                catch (Exception ex)
                {
                    Logger.Error($"EnemyDatabase: Skipping invalid enemy '{data.EnemyId}': {ex.Message}");
                }
            }
        }

        // ----------------------------------------------------------------
        // Embedded default definitions — used when JSON is unavailable
        // ----------------------------------------------------------------
        private void RegisterDefaults()
        {
            var defaults = new List<EnemyData>
            {
                // ---- 1. Goblin Grunt ----
                new EnemyData
                {
                    EnemyId       = "goblin_grunt",
                    DisplayName   = "Goblin Grunt",
                    Species       = "Goblin",
                    Description   = "A small but cunning goblin scout that attacks in packs.",
                    MaxHp         = 40f,
                    MoveSpeed     = 4.5f,
                    AttackDamage  = 6f,
                    AttackRange   = 1.5f,
                    AggroRange    = 12f,
                    AttackCooldown= 1.2f,
                    Defense       = 2f,
                    XpReward      = 8,
                    LootTableId   = "loot_goblin",
                    Behaviour     = EnemyBehaviour.Aggressive,
                    Element       = EnemyElement.None,
                    Weaknesses    = new() { { "fire", 1.5f } },
                    Resistances   = new() { { "poison", 0.5f } },
                    VfxHitKey     = "vfx_hit_slash",
                    VfxDeathKey   = "vfx_death_goblin",
                    SfxAggroKey   = "sfx_goblin_aggro",
                    SfxAttackKey  = "sfx_goblin_attack",
                    SfxDeathKey   = "sfx_goblin_death",
                    ModelPath     = "res://Assets/Characters/Enemies/goblin_grunt.glb",
                    PolyBudget    = 1800
                },

                // ---- 2. Skeleton Warrior ----
                new EnemyData
                {
                    EnemyId       = "skeleton_warrior",
                    DisplayName   = "Skeleton Warrior",
                    Species       = "Undead",
                    Description   = "An ancient warrior risen from death, wielding a rusted sword and shield.",
                    MaxHp         = 70f,
                    MoveSpeed     = 3.0f,
                    AttackDamage  = 12f,
                    AttackRange   = 2.0f,
                    AggroRange    = 10f,
                    AttackCooldown= 1.8f,
                    Defense       = 8f,
                    XpReward      = 20,
                    LootTableId   = "loot_skeleton",
                    Behaviour     = EnemyBehaviour.Patrol,
                    Element       = EnemyElement.Shadow,
                    Weaknesses    = new() { { "holy", 2.0f }, { "fire", 1.25f } },
                    Resistances   = new() { { "poison", 0f }, { "ice", 0.5f } },
                    VfxHitKey     = "vfx_hit_bone",
                    VfxDeathKey   = "vfx_death_skeleton",
                    SfxAggroKey   = "sfx_skeleton_rattle",
                    SfxAttackKey  = "sfx_skeleton_swing",
                    SfxDeathKey   = "sfx_skeleton_crumble",
                    ModelPath     = "res://Assets/Characters/Enemies/skeleton_warrior.glb",
                    PolyBudget    = 2000
                },

                // ---- 3. Forest Wolf ----
                new EnemyData
                {
                    EnemyId       = "forest_wolf",
                    DisplayName   = "Forest Wolf",
                    Species       = "Beast",
                    Description   = "A large grey wolf that hunts in packs in the Verdant Wilds.",
                    MaxHp         = 55f,
                    MoveSpeed     = 6.5f,
                    AttackDamage  = 10f,
                    AttackRange   = 1.8f,
                    AggroRange    = 15f,
                    AttackCooldown= 0.9f,
                    Defense       = 0f,
                    XpReward      = 12,
                    LootTableId   = "loot_beast",
                    Behaviour     = EnemyBehaviour.Aggressive,
                    Element       = EnemyElement.None,
                    Weaknesses    = new() { { "fire", 1.5f } },
                    Resistances   = new(),
                    VfxHitKey     = "vfx_hit_slash",
                    VfxDeathKey   = "vfx_death_beast",
                    SfxAggroKey   = "sfx_wolf_howl",
                    SfxAttackKey  = "sfx_wolf_bite",
                    SfxDeathKey   = "sfx_wolf_death",
                    ModelPath     = "res://Assets/Characters/Enemies/forest_wolf.glb",
                    PolyBudget    = 1500
                },

                // ---- 4. Stone Golem ----
                new EnemyData
                {
                    EnemyId       = "stone_golem",
                    DisplayName   = "Stone Golem",
                    Species       = "Construct",
                    Description   = "A massive animated stone construct. Slow but devastating.",
                    MaxHp         = 200f,
                    MoveSpeed     = 1.8f,
                    AttackDamage  = 30f,
                    AttackRange   = 3.0f,
                    AggroRange    = 8f,
                    AttackCooldown= 3.0f,
                    Defense       = 20f,
                    XpReward      = 60,
                    LootTableId   = "loot_golem",
                    Behaviour     = EnemyBehaviour.Guard,
                    Element       = EnemyElement.None,
                    Weaknesses    = new() { { "lightning", 1.5f } },
                    Resistances   = new() { { "fire", 0.5f }, { "ice", 0.5f }, { "poison", 0f } },
                    VfxHitKey     = "vfx_hit_stone",
                    VfxDeathKey   = "vfx_death_golem",
                    SfxAggroKey   = "sfx_golem_awaken",
                    SfxAttackKey  = "sfx_golem_slam",
                    SfxDeathKey   = "sfx_golem_shatter",
                    ModelPath     = "res://Assets/Characters/Enemies/stone_golem.glb",
                    PolyBudget    = 2500,
                    HpScaleFactor = 1.2f
                },

                // ---- 5. Dark Mage ----
                new EnemyData
                {
                    EnemyId       = "dark_mage",
                    DisplayName   = "Dark Mage",
                    Species       = "Humanoid",
                    Description   = "A corrupted sorcerer who launches shadow projectiles from a distance.",
                    MaxHp         = 45f,
                    MoveSpeed     = 2.5f,
                    AttackDamage  = 18f,
                    AttackRange   = 12.0f,  // Ranged enemy
                    AggroRange    = 18f,
                    AttackCooldown= 2.5f,
                    Defense       = 3f,
                    XpReward      = 35,
                    LootTableId   = "loot_mage",
                    Behaviour     = EnemyBehaviour.Aggressive,
                    Element       = EnemyElement.Shadow,
                    Weaknesses    = new() { { "holy", 2.0f } },
                    Resistances   = new() { { "shadow", 0f } },
                    VfxHitKey     = "vfx_hit_magic",
                    VfxDeathKey   = "vfx_death_mage",
                    SfxAggroKey   = "sfx_mage_cackle",
                    SfxAttackKey  = "sfx_mage_cast",
                    SfxDeathKey   = "sfx_mage_death",
                    ModelPath     = "res://Assets/Characters/Enemies/dark_mage.glb",
                    PolyBudget    = 2000,
                    DamageScaleFactor = 1.1f
                }
            };

            Register(defaults);
            Logger.Info($"EnemyDatabase: Registered {_registry.Count} default enemies.");
        }
    }
}
