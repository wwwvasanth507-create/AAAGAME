using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Prologue
{
    public class StarterEnemyProfile
    {
        public string EnemyId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
        public int MaxHp { get; set; } = 30;
        public int BaseDamage { get; set; } = 5;
        public float MovementSpeed { get; set; } = 3.5f;
        public string LootTableId { get; set; } = "loot_starter_basic";
        public bool IsBoss { get; set; } = false;
    }

    public class StarterEnemyDefinitions
    {
        private readonly Dictionary<string, StarterEnemyProfile> _enemies = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterDefaultStarterEnemies()
        {
            RegisterEnemy(new StarterEnemyProfile
            {
                EnemyId = "enemy_green_slime",
                DisplayName = "Green Slime",
                Level = 1,
                MaxHp = 20,
                BaseDamage = 3
            });

            RegisterEnemy(new StarterEnemyProfile
            {
                EnemyId = "enemy_forest_wolf",
                DisplayName = "Forest Wolf",
                Level = 2,
                MaxHp = 35,
                BaseDamage = 6
            });

            RegisterEnemy(new StarterEnemyProfile
            {
                EnemyId = "enemy_wild_boar",
                DisplayName = "Wild Boar",
                Level = 2,
                MaxHp = 45,
                BaseDamage = 7
            });

            RegisterEnemy(new StarterEnemyProfile
            {
                EnemyId = "enemy_cave_spider",
                DisplayName = "Cave Spider",
                Level = 3,
                MaxHp = 30,
                BaseDamage = 5
            });

            RegisterEnemy(new StarterEnemyProfile
            {
                EnemyId = "enemy_bandit_scout",
                DisplayName = "Bandit Scout",
                Level = 3,
                MaxHp = 50,
                BaseDamage = 8
            });

            RegisterEnemy(new StarterEnemyProfile
            {
                EnemyId = "enemy_boss_skarr",
                DisplayName = "Baron Skarr (Bandit Commander)",
                Level = 5,
                MaxHp = 180,
                BaseDamage = 14,
                IsBoss = true,
                LootTableId = "loot_boss_skarr"
            });
        }

        public void RegisterEnemy(StarterEnemyProfile enemy)
        {
            if (enemy != null && !string.IsNullOrEmpty(enemy.EnemyId))
            {
                _enemies[enemy.EnemyId] = enemy;
            }
        }

        public StarterEnemyProfile? GetEnemy(string enemyId)
        {
            return _enemies.TryGetValue(enemyId, out var e) ? e : null;
        }

        public IReadOnlyCollection<StarterEnemyProfile> AllEnemies => _enemies.Values;
    }
}
