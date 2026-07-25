using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// AbilityDatabase is the authoritative registry of all player abilities.
    /// Loads from Settings/ability_database.json at startup.
    /// Falls back to 5 embedded starter abilities if file not found.
    /// </summary>
    public class AbilityDatabase
    {
        private readonly Dictionary<string, AbilityDefinition> _registry
            = new(StringComparer.OrdinalIgnoreCase);

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        // ----------------------------------------------------------------
        // Load
        // ----------------------------------------------------------------
        public void Load(string settingsDir)
        {
            string path = Path.Combine(settingsDir, "ability_database.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<List<AbilityData>>(json, JsonOpts);
                    if (list != null) Register(list);
                    Logger.Info($"AbilityDatabase: Loaded {_registry.Count} abilities from '{path}'.");
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AbilityDatabase: Failed to parse '{path}': {ex.Message}. Using defaults.");
                }
            }
            else
            {
                Logger.Warning($"AbilityDatabase: '{path}' not found. Using embedded defaults.");
            }

            RegisterDefaults();
        }

        // ----------------------------------------------------------------
        // Lookup
        // ----------------------------------------------------------------
        public AbilityDefinition? Get(string abilityId)
        {
            _registry.TryGetValue(abilityId, out var def);
            return def;
        }

        public AbilityDefinition GetOrThrow(string abilityId)
        {
            if (_registry.TryGetValue(abilityId, out var def)) return def;
            throw new KeyNotFoundException($"AbilityDatabase: No ability with ID '{abilityId}'.");
        }

        public IReadOnlyCollection<AbilityDefinition> GetAll()    => _registry.Values;
        public bool Contains(string id)                            => _registry.ContainsKey(id);
        public int  Count                                          => _registry.Count;

        public List<AbilityDefinition> GetUnlocked(int playerLevel)
        {
            var list = new List<AbilityDefinition>();
            foreach (var def in _registry.Values)
                if (def.IsUnlocked(playerLevel)) list.Add(def);
            return list;
        }

        // ----------------------------------------------------------------
        // Internal registration
        // ----------------------------------------------------------------
        private void Register(List<AbilityData> list)
        {
            foreach (var data in list)
            {
                try
                {
                    _registry[data.AbilityId] = new AbilityDefinition(data);
                }
                catch (Exception ex)
                {
                    Logger.Error($"AbilityDatabase: Skipping invalid ability '{data.AbilityId}': {ex.Message}");
                }
            }
        }

        // ----------------------------------------------------------------
        // 5 embedded starter abilities
        // ----------------------------------------------------------------
        private void RegisterDefaults()
        {
            var defaults = new List<AbilityData>
            {
                // ── 1. Power Strike ─────────────────────────────────────
                new AbilityData
                {
                    AbilityId     = "power_strike",
                    DisplayName   = "Power Strike",
                    Description   = "A focused melee blow dealing 2.5× weapon damage to a single target.",
                    CooldownSec   = 6f,
                    ManaCost      = 0f,
                    StaminaCost   = 25f,
                    TargetType    = AbilityTargetType.SingleEnemy,
                    DamageType    = AbilityDamageType.Physical,
                    BaseDamage    = 40f,
                    Range         = 2.5f,
                    LevelRequired = 1,
                    VfxCastKey    = "vfx_heavy_slash",
                    SfxCastKey    = "sfx_greatsword",
                    VfxHitKey     = "vfx_hit_slash",
                    IconPath      = "res://Assets/UI/Icons/Abilities/icon_ability_powerstrike.png",
                    Tags          = new() { "melee", "physical", "single-target" }
                },

                // ── 2. Dodge Roll ────────────────────────────────────────
                new AbilityData
                {
                    AbilityId     = "dodge_roll",
                    DisplayName   = "Dodge Roll",
                    Description   = "A quick roll that grants a brief invincibility window and resets combo.",
                    CooldownSec   = 3f,
                    ManaCost      = 0f,
                    StaminaCost   = 15f,
                    TargetType    = AbilityTargetType.Directional,
                    DamageType    = AbilityDamageType.None,
                    BaseDamage    = 0f,
                    Duration      = 0.4f,   // i-frame duration
                    LevelRequired = 1,
                    VfxCastKey    = "vfx_roll_dust",
                    SfxCastKey    = "sfx_dodge",
                    IconPath      = "res://Assets/UI/Icons/Abilities/icon_ability_dodgeroll.png",
                    Tags          = new() { "movement", "defensive", "i-frame" }
                },

                // ── 3. Arrow Rain ────────────────────────────────────────
                new AbilityData
                {
                    AbilityId     = "arrow_rain",
                    DisplayName   = "Arrow Rain",
                    Description   = "Launches a volley of arrows that rain down in a 6m radius AoE.",
                    CooldownSec   = 12f,
                    ManaCost      = 20f,
                    StaminaCost   = 0f,
                    TargetType    = AbilityTargetType.AoE,
                    DamageType    = AbilityDamageType.Physical,
                    BaseDamage    = 15f,   // Per arrow; 6 arrows
                    AoeRadius     = 6f,
                    Range         = 20f,
                    LevelRequired = 3,
                    VfxCastKey    = "vfx_arrow_rain",
                    SfxCastKey    = "sfx_bowshot",
                    VfxHitKey     = "vfx_arrow",
                    IconPath      = "res://Assets/UI/Icons/Abilities/icon_ability_arrowrain.png",
                    Tags          = new() { "ranged", "aoe", "physical" }
                },

                // ── 4. Barrier ───────────────────────────────────────────
                new AbilityData
                {
                    AbilityId     = "barrier",
                    DisplayName   = "Barrier",
                    Description   = "Creates an energy shield that absorbs up to 80 damage for 5 seconds.",
                    CooldownSec   = 18f,
                    ManaCost      = 35f,
                    StaminaCost   = 0f,
                    TargetType    = AbilityTargetType.Self,
                    DamageType    = AbilityDamageType.None,
                    BaseDamage    = 0f,
                    Duration      = 5f,
                    LevelRequired = 5,
                    VfxCastKey    = "vfx_barrier",
                    SfxCastKey    = "sfx_magic_cast",
                    IconPath      = "res://Assets/UI/Icons/Abilities/icon_ability_barrier.png",
                    Tags          = new() { "defensive", "self", "shield" }
                },

                // ── 5. Fireball ──────────────────────────────────────────
                new AbilityData
                {
                    AbilityId     = "fireball",
                    DisplayName   = "Fireball",
                    Description   = "Launches a blazing fireball that explodes on impact, burning nearby enemies.",
                    CooldownSec   = 8f,
                    ManaCost      = 30f,
                    StaminaCost   = 0f,
                    TargetType    = AbilityTargetType.Projectile,
                    DamageType    = AbilityDamageType.Fire,
                    BaseDamage    = 55f,
                    AoeRadius     = 3f,
                    Range         = 25f,
                    LevelRequired = 4,
                    VfxCastKey    = "vfx_fireball",
                    VfxHitKey     = "vfx_explosion",
                    SfxCastKey    = "sfx_magic_cast",
                    IconPath      = "res://Assets/UI/Icons/Abilities/icon_ability_fireball.png",
                    Tags          = new() { "magic", "projectile", "fire", "aoe" }
                }
            };

            Register(defaults);
            Logger.Info($"AbilityDatabase: Registered {_registry.Count} default abilities.");
        }
    }
}
