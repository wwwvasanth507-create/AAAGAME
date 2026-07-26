using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter8
{
    public class ShadowFrontierEnemyDefinition
    {
        public string EnemyId { get; set; } = "";
        public string Name { get; set; } = "";
        public int Level { get; set; } = 32;
        public float MaxHealth { get; set; } = 800f;
        public float BaseDamage { get; set; } = 65f;
        public string SpecialAbility { get; set; } = "";
        public bool IsElite { get; set; } = false;
    }

    /// <summary>
    /// Roster of high-level Act III enemies populating The Shadow Frontier.
    /// Introduces corrupted knights, shadow stalkers, ancient obelisk guardians, and void spellweavers.
    /// </summary>
    public class ShadowFrontierEnemies
    {
        private readonly Dictionary<string, ShadowFrontierEnemyDefinition> _enemies = new(StringComparer.OrdinalIgnoreCase);

        public ShadowFrontierEnemies()
        {
            InitializeEnemies();
        }

        private void InitializeEnemies()
        {
            // 1. Shadow Stalker
            RegisterEnemy(new ShadowFrontierEnemyDefinition
            {
                EnemyId = "enemy_shadow_stalker",
                Name = "Shadow Stalker Beast",
                Level = 31,
                MaxHealth = 650f,
                BaseDamage = 55f,
                SpecialAbility = "ability_shadow_pounce",
                IsElite = false
            });

            // 2. Corrupted Iron Knight
            RegisterEnemy(new ShadowFrontierEnemyDefinition
            {
                EnemyId = "enemy_corrupted_iron_knight",
                Name = "Corrupted Iron Knight",
                Level = 33,
                MaxHealth = 1100f,
                BaseDamage = 75f,
                SpecialAbility = "ability_void_shield_bash",
                IsElite = true
            });

            // 3. Ancient Obelisk Guardian
            RegisterEnemy(new ShadowFrontierEnemyDefinition
            {
                EnemyId = "enemy_ancient_obelisk_guardian",
                Name = "Ancient Obelisk Guardian",
                Level = 34,
                MaxHealth = 1400f,
                BaseDamage = 90f,
                SpecialAbility = "ability_obelisk_pulse",
                IsElite = true
            });

            // 4. Void Spellweaver
            RegisterEnemy(new ShadowFrontierEnemyDefinition
            {
                EnemyId = "enemy_void_spellweaver",
                Name = "Void Spellweaver Sorcerer",
                Level = 33,
                MaxHealth = 750f,
                BaseDamage = 85f,
                SpecialAbility = "ability_void_bolt_salvo",
                IsElite = false
            });

            // 5. Shadow Behemoth (Regional Champion)
            RegisterEnemy(new ShadowFrontierEnemyDefinition
            {
                EnemyId = "enemy_shadow_behemoth",
                Name = "Shadow Behemoth Warlord",
                Level = 35,
                MaxHealth = 2200f,
                BaseDamage = 130f,
                SpecialAbility = "ability_behemoth_slam",
                IsElite = true
            });
        }

        public void RegisterEnemy(ShadowFrontierEnemyDefinition enemy)
        {
            if (enemy != null && !string.IsNullOrEmpty(enemy.EnemyId))
            {
                _enemies[enemy.EnemyId] = enemy;
            }
        }

        public ShadowFrontierEnemyDefinition? GetEnemy(string enemyId)
        {
            return _enemies.TryGetValue(enemyId, out var e) ? e : null;
        }

        public List<ShadowFrontierEnemyDefinition> GetAllEnemies()
        {
            return new List<ShadowFrontierEnemyDefinition>(_enemies.Values);
        }
    }
}
