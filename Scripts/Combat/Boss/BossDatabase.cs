using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    public class BossDatabase
    {
        private readonly Dictionary<string, BossDefinition> _registry = new(StringComparer.OrdinalIgnoreCase);

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public void Load(string settingsDir)
        {
            string path = Path.Combine(settingsDir, "boss_database.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<List<BossData>>(json, JsonOpts);
                    if (list != null)
                    {
                        foreach (var data in list)
                        {
                            _registry[data.BossId] = new BossDefinition(data);
                        }
                    }
                    Logger.Info($"BossDatabase: Loaded {_registry.Count} boss definitions from '{path}'.");
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Error($"BossDatabase: Failed to load from '{path}': {ex.Message}. Registering defaults.");
                }
            }
            else
            {
                Logger.Warning($"BossDatabase: '{path}' not found. Registering defaults.");
            }

            RegisterDefaults();
        }

        public BossDefinition? Get(string bossId)
        {
            _registry.TryGetValue(bossId, out var def);
            return def;
        }

        public bool Contains(string bossId) => _registry.ContainsKey(bossId);

        public int Count => _registry.Count;

        private void RegisterDefaults()
        {
            var golem = new BossData
            {
                BossId = "golem_titan",
                DisplayName = "Golem Titan",
                Class = BossClass.Behemoth,
                ArenaId = "arena_titan",
                Biome = "mountains",
                MinLevel = 15,
                MaxLevel = 25,
                MaxHp = 800f,
                MaxShield = 200f,
                Armor = 50f,
                MoveSpeed = 3.5f,
                Element = "earth",
                Weaknesses = new List<string> { "lightning" },
                Resistances = new List<string> { "fire", "ice" },
                LootTableId = "loot_golem_titan",
                MusicProfileId = "music_golem_titan",
                VfxProfileId = "vfx_golem_titan",
                VoiceProfileId = "voice_golem_titan",
                CameraProfileId = "camera_golem_titan",
                RewardProfileId = "reward_golem_titan",
                SpecialAttacks = new List<SpecialAttackData>
                {
                    new()
                    {
                        AttackId = "titan_slam",
                        DisplayName = "Titan Slam",
                        AttackType = SpecialAttackType.AreaOfEffect,
                        BaseDamage = 45f,
                        Range = 6f,
                        Cooldown = 8f,
                        CastTime = 2.0f,
                        AoeRadius = 8f,
                        VfxCastKey = "vfx_titan_slam_cast",
                        SfxCastKey = "sfx_titan_slam_cast"
                    },
                    new()
                    {
                        AttackId = "titan_charge",
                        DisplayName = "Titan Charge",
                        AttackType = SpecialAttackType.MovementCharge,
                        BaseDamage = 35f,
                        Range = 15f,
                        Cooldown = 12f,
                        CastTime = 1.5f,
                        VfxCastKey = "vfx_titan_charge_cast",
                        SfxCastKey = "sfx_titan_charge_cast"
                    }
                },
                Phases = new List<BossPhaseData>
                {
                    new()
                    {
                        PhaseIndex = 1,
                        HpThresholdPct = 1.0f,
                        SpeedMultiplier = 1.0f,
                        DamageMultiplier = 1.0f,
                        PhaseSpecialAttackIds = new List<string> { "titan_slam" }
                    },
                    new()
                    {
                        PhaseIndex = 2,
                        HpThresholdPct = 0.5f,
                        SpeedMultiplier = 1.25f,
                        DamageMultiplier = 1.5f,
                        VfxTriggerKey = "vfx_titan_enrage",
                        SfxTriggerKey = "sfx_titan_enrage",
                        PhaseSpecialAttackIds = new List<string> { "titan_slam", "titan_charge" }
                    }
                }
            };

            _registry[golem.BossId] = new BossDefinition(golem);
            Logger.Info("BossDatabase: Default Golem Titan registered.");
        }
    }
}
