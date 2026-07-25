using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter2
{
    public class Chapter2EnemyProfile
    {
        public string EnemyId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Level { get; set; } = 6;
        public int MaxHp { get; set; } = 80;
        public int BaseDamage { get; set; } = 15;
        public float MovementSpeed { get; set; } = 4.0f;
        public string LootTableId { get; set; } = "loot_chapter2_forest";
        public bool IsBoss { get; set; } = false;
    }

    public class Chapter2EnemyDefinitions
    {
        private readonly Dictionary<string, Chapter2EnemyProfile> _enemies = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterDefaultChapter2Enemies()
        {
            RegisterEnemy(new Chapter2EnemyProfile
            {
                EnemyId = "enemy_elite_wolf",
                DisplayName = "Sylvan Elite Wolf",
                Level = 8,
                MaxHp = 100,
                BaseDamage = 18
            });

            RegisterEnemy(new Chapter2EnemyProfile
            {
                EnemyId = "enemy_forest_spirit",
                DisplayName = "Corrupted Forest Spirit",
                Level = 9,
                MaxHp = 90,
                BaseDamage = 22
            });

            RegisterEnemy(new Chapter2EnemyProfile
            {
                EnemyId = "enemy_corrupted_boar",
                DisplayName = "Blighted Forest Boar",
                Level = 9,
                MaxHp = 120,
                BaseDamage = 20
            });

            RegisterEnemy(new Chapter2EnemyProfile
            {
                EnemyId = "enemy_bandit_archer",
                DisplayName = "Sylvan Bandit Marksman",
                Level = 10,
                MaxHp = 85,
                BaseDamage = 24
            });

            RegisterEnemy(new Chapter2EnemyProfile
            {
                EnemyId = "enemy_venom_spider",
                DisplayName = "Giant Venom Spider",
                Level = 11,
                MaxHp = 110,
                BaseDamage = 26
            });

            RegisterEnemy(new Chapter2EnemyProfile
            {
                EnemyId = "enemy_boss_ruin_guardian",
                DisplayName = "Ancient Ruin Guardian",
                Level = 15,
                MaxHp = 450,
                BaseDamage = 40,
                IsBoss = true,
                LootTableId = "loot_boss_ruin_guardian"
            });
        }

        public void RegisterEnemy(Chapter2EnemyProfile enemy)
        {
            if (enemy != null && !string.IsNullOrEmpty(enemy.EnemyId))
            {
                _enemies[enemy.EnemyId] = enemy;
            }
        }

        public Chapter2EnemyProfile? GetEnemy(string enemyId)
        {
            return _enemies.TryGetValue(enemyId, out var e) ? e : null;
        }

        public IReadOnlyCollection<Chapter2EnemyProfile> AllEnemies => _enemies.Values;
    }
}
