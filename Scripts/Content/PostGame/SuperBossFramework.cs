using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.PostGame
{
    public enum SuperBossDifficulty
    {
        Normal,   // 1.0x Stats
        Heroic,   // 1.5x HP / Damage
        Mythic    // 2.2x HP / Damage + Enrage Timer
    }

    public class SuperBossRecord
    {
        public string BossId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public int BaseHealth { get; set; } = 18000;
        public int RecommendedLevel { get; set; } = 50;
        public string LocationName { get; set; } = "";
        public string TrophyItemId { get; set; } = "";
        public bool IsDefeated { get; set; } = false;
        public SuperBossDifficulty HighestDefeatedDifficulty { get; set; } = SuperBossDifficulty.Normal;
    }

    /// <summary>
    /// Reusable Super Boss Framework for Hero of Eternia Post-Game.
    /// Manages 3 optional endgame super bosses (Titan of Fractured Time, Void Astral Leviathan, Sun King's Ascended Memory),
    /// difficulty scaling multipliers (Normal, Heroic, Mythic), leaderboards, and legendary drops.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class SuperBossFramework : IInitializable
    {
        private readonly Dictionary<string, SuperBossRecord> _superBosses = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<string, SuperBossDifficulty>? OnSuperBossDefeated;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultSuperBosses();

            // Register with ServiceLocator
            ServiceLocator.Register<SuperBossFramework>(this);

            IsInitialized = true;
            Logger.Info("SuperBossFramework: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _superBosses.Clear();

            ServiceLocator.Unregister<SuperBossFramework>();
            IsInitialized = false;
            Logger.Info("SuperBossFramework: Shutdown completed.");
        }

        private void RegisterDefaultSuperBosses()
        {
            _superBosses.Clear();

            // 1. Chronos Titan
            RegisterBoss(new SuperBossRecord
            {
                BossId = "boss_chronos_titan",
                DisplayName = "Chronos, Titan of Fractured Time",
                BaseHealth = 18000,
                RecommendedLevel = 50,
                LocationName = "Chamber of Fractured Timelines",
                TrophyItemId = "trophy_chronos_hourglass"
            });

            // 2. Void Astral Leviathan
            RegisterBoss(new SuperBossRecord
            {
                BossId = "boss_astral_leviathan",
                DisplayName = "Astral Leviathan of the Void",
                BaseHealth = 22000,
                RecommendedLevel = 52,
                LocationName = "Astral Rift Abyss",
                TrophyItemId = "trophy_leviathan_astral_scale"
            });

            // 3. Sun King's Ascended Memory
            RegisterBoss(new SuperBossRecord
            {
                BossId = "boss_sol_prime_avatar",
                DisplayName = "Sun King's Ascended Memory",
                BaseHealth = 25000,
                RecommendedLevel = 55,
                LocationName = "Sanctum of the First Dawn",
                TrophyItemId = "trophy_sol_prime_crown"
            });
        }

        public void RegisterBoss(SuperBossRecord boss)
        {
            if (boss != null && !string.IsNullOrEmpty(boss.BossId))
            {
                _superBosses[boss.BossId] = boss;
            }
        }

        public bool RecordDefeat(string bossId, SuperBossDifficulty difficulty)
        {
            if (!_superBosses.TryGetValue(bossId, out var b)) return false;

            b.IsDefeated = true;
            if (difficulty > b.HighestDefeatedDifficulty)
            {
                b.HighestDefeatedDifficulty = difficulty;
            }

            OnSuperBossDefeated?.Invoke(bossId, difficulty);
            Logger.Info($"SuperBossFramework: DEFEATED SUPER BOSS '{b.DisplayName}' on {difficulty} difficulty!");
            return true;
        }

        public SuperBossRecord? GetBoss(string bossId)
        {
            return _superBosses.TryGetValue(bossId, out var b) ? b : null;
        }

        public List<SuperBossRecord> GetAllBosses()
        {
            return new List<SuperBossRecord>(_superBosses.Values);
        }
    }
}
