using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public class Act2EnemyDefinition
    {
        public string EnemyId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int AttackDamage { get; set; }
        public string Region { get; set; } = string.Empty;
        public bool IsBoss { get; set; } = false;
    }

    /// <summary>
    /// Act II enemy roster for the Eastern Ridgeline and Mirkwood Swamps regions.
    /// Introduces Malakor's Vanguard elites and Swamp cult variants.
    /// </summary>
    public class Act2EnemyDefinitions
    {
        private readonly List<Act2EnemyDefinition> _enemies = new();

        public void RegisterEnemies()
        {
            // Eastern Ridgeline enemies
            _enemies.Add(new Act2EnemyDefinition
            {
                EnemyId = "enemy_shadow_cult_vanguard",
                DisplayName = "Shadow Cult Vanguard",
                Level = 19,
                MaxHp = 220,
                AttackDamage = 38,
                Region = "region_eastern_ridgeline"
            });

            _enemies.Add(new Act2EnemyDefinition
            {
                EnemyId = "enemy_ridgeline_harpy",
                DisplayName = "Ridgeline Harpy",
                Level = 20,
                MaxHp = 180,
                AttackDamage = 42,
                Region = "region_eastern_ridgeline"
            });

            _enemies.Add(new Act2EnemyDefinition
            {
                EnemyId = "enemy_storm_golem",
                DisplayName = "Storm Golem",
                Level = 21,
                MaxHp = 400,
                AttackDamage = 55,
                Region = "region_eastern_ridgeline"
            });

            _enemies.Add(new Act2EnemyDefinition
            {
                EnemyId = "enemy_malakor_vanguard_captain",
                DisplayName = "Vanguard Captain Drael",
                Level = 22,
                MaxHp = 700,
                AttackDamage = 65,
                Region = "region_eastern_ridgeline",
                IsBoss = true
            });

            // Mirkwood Swamps enemies
            _enemies.Add(new Act2EnemyDefinition
            {
                EnemyId = "enemy_swamp_lurker",
                DisplayName = "Mirkwood Lurker",
                Level = 21,
                MaxHp = 260,
                AttackDamage = 40,
                Region = "region_mirkwood_swamps"
            });

            _enemies.Add(new Act2EnemyDefinition
            {
                EnemyId = "enemy_bog_witch",
                DisplayName = "Bog Witch",
                Level = 22,
                MaxHp = 230,
                AttackDamage = 50,
                Region = "region_mirkwood_swamps"
            });

            _enemies.Add(new Act2EnemyDefinition
            {
                EnemyId = "enemy_plague_shambler",
                DisplayName = "Plague Shambler",
                Level = 23,
                MaxHp = 350,
                AttackDamage = 45,
                Region = "region_mirkwood_swamps"
            });

            Logger.Info($"Act2EnemyDefinitions: {_enemies.Count} enemies registered for Act II.");
        }

        public IReadOnlyList<Act2EnemyDefinition> AllEnemies => _enemies;
    }
}
