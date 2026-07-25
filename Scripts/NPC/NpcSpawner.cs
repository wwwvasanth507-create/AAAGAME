using System;
using System.Collections.Generic;
using HeroOfEternia.Core;
using HeroOfEternia.World;

namespace HeroOfEternia.NPC
{
    public enum SpawnCategory
    {
        Village,
        City,
        Road,
        Home,
        SpecialLocation,
        RandomTraveler
    }

    /// <summary>
    /// Placement record for a single spawned NPC.
    /// </summary>
    public class SpawnedNpc
    {
        public NpcData Data { get; set; } = new();
        public SpawnCategory Category { get; set; }
        public float WorldX { get; set; }
        public float WorldY { get; set; }
        public float WorldZ { get; set; }
        public bool IsActive { get; set; } = false;
    }

    /// <summary>
    /// NPC spawn rule — configures a population rule for one spawn category.
    /// Loaded from npc_spawns.json via ConfigManager.
    /// </summary>
    public class NpcSpawnRule
    {
        public SpawnCategory Category { get; set; }
        public NpcTypeEnum NpcType { get; set; }
        public int MinCount { get; set; } = 1;
        public int MaxCount { get; set; } = 5;
        public string LandmarkTag { get; set; } = ""; // e.g. "village_center"
        public float SpawnRadius { get; set; } = 32f;
    }

    /// <summary>
    /// Deterministically generates SpawnedNpc placement records from a ulong world seed.
    /// Uses WorldSeed.Parse() for consistent hashing of region/type/index strings.
    /// No Godot scene instantiation — purely data-side placement planning.
    /// </summary>
    public class NpcSpawner
    {
        private readonly ulong _seedValue;
        private readonly List<NpcSpawnRule> _rules = new();

        public NpcSpawner(ulong seedValue)
        {
            _seedValue = seedValue;
        }

        /// <summary>
        /// Convenience constructor accepting a seed string (hashed with WorldSeed.Parse).
        /// </summary>
        public NpcSpawner(string seedString)
        {
            _seedValue = WorldSeed.Parse(seedString);
        }

        public void AddRule(NpcSpawnRule rule) => _rules.Add(rule);

        /// <summary>
        /// Registers the default set of spawn rules covering all categories.
        /// </summary>
        public void RegisterDefaultRules()
        {
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.Village,        NpcType = NpcTypeEnum.Villager,   MinCount = 6, MaxCount = 12, LandmarkTag = "village_center", SpawnRadius = 48f });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.Village,        NpcType = NpcTypeEnum.Farmer,     MinCount = 2, MaxCount = 6,  LandmarkTag = "farm",           SpawnRadius = 64f });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.Village,        NpcType = NpcTypeEnum.Child,      MinCount = 1, MaxCount = 4,  LandmarkTag = "village_center", SpawnRadius = 32f });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.City,          NpcType = NpcTypeEnum.Guard,      MinCount = 4, MaxCount = 8,  LandmarkTag = "city_gate",      SpawnRadius = 24f });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.City,          NpcType = NpcTypeEnum.Scholar,    MinCount = 1, MaxCount = 3,  LandmarkTag = "library",        SpawnRadius = 16f });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.City,          NpcType = NpcTypeEnum.Blacksmith, MinCount = 1, MaxCount = 2,  LandmarkTag = "smithy",         SpawnRadius = 8f  });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.Road,          NpcType = NpcTypeEnum.Traveler,   MinCount = 1, MaxCount = 3,  LandmarkTag = "road",           SpawnRadius = 128f});
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.Road,          NpcType = NpcTypeEnum.Bandit,     MinCount = 1, MaxCount = 2,  LandmarkTag = "road",           SpawnRadius = 80f });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.SpecialLocation,NpcType = NpcTypeEnum.King,      MinCount = 1, MaxCount = 1,  LandmarkTag = "throne_room",    SpawnRadius = 4f  });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.SpecialLocation,NpcType = NpcTypeEnum.Queen,     MinCount = 1, MaxCount = 1,  LandmarkTag = "throne_room",    SpawnRadius = 4f  });
            _rules.Add(new NpcSpawnRule { Category = SpawnCategory.RandomTraveler, NpcType = NpcTypeEnum.Traveler,  MinCount = 0, MaxCount = 2,  LandmarkTag = "",               SpawnRadius = 256f});
        }

        /// <summary>
        /// Generates deterministic NPC placements for a given region.
        /// </summary>
        public List<SpawnedNpc> GenerateForRegion(string regionId, float regionCenterX, float regionCenterZ)
        {
            var results = new List<SpawnedNpc>();

            int npcIndex = 0;
            foreach (var rule in _rules)
            {
                ulong ruleSeed = WorldSeed.Parse($"{regionId}_{rule.Category}_{rule.NpcType}") ^ _seedValue;
                int count = (int)(ruleSeed % (uint)(rule.MaxCount - rule.MinCount + 1)) + rule.MinCount;

                for (int i = 0; i < count; i++)
                {
                    ulong posSeed = WorldSeed.Parse($"{regionId}_{rule.Category}_{rule.NpcType}_{i}") ^ _seedValue;
                    float angle = (float)(posSeed % 360u) * (MathF.PI / 180f);
                    float dist  = (float)(posSeed % (ulong)(rule.SpawnRadius * 100)) / 100f;

                    float wx = regionCenterX + MathF.Cos(angle) * dist;
                    float wz = regionCenterZ + MathF.Sin(angle) * dist;

                    string npcId = $"npc_{regionId}_{rule.NpcType}_{i:D3}";

                    var spawned = new SpawnedNpc
                    {
                        Category = rule.Category,
                        WorldX   = wx,
                        WorldY   = 0f,
                        WorldZ   = wz,
                        Data = new NpcData
                        {
                            UniqueId            = npcId,
                            DisplayName         = $"{rule.NpcType} #{i + 1}",
                            Occupation          = rule.NpcType,
                            CurrentRegionId     = regionId,
                            HomeLocationId      = rule.LandmarkTag,
                            AnimationProfileKey = "anim_humanoid",
                            WorldX = wx,
                            WorldY = 0f,
                            WorldZ = wz
                        }
                    };

                    results.Add(spawned);
                    npcIndex++;
                }
            }

            Logger.Info($"NpcSpawner: generated {results.Count} NPC placements for region '{regionId}'");
            return results;
        }
    }
}
