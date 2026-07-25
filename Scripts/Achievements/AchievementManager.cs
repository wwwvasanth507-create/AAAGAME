using Godot;
using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Achievements
{
    public class Achievement
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsUnlocked { get; set; } = false;
        public int RewardGold { get; set; } = 500;
    }

    public class AchievementManager : IInitializable
    {
        private static AchievementManager? _instance;
        public static AchievementManager Instance => _instance ??= new AchievementManager();

        private readonly Dictionary<string, Achievement> _achievements = new();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            LoadDefaultAchievements();
            GD.Print("[AchievementManager] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _achievements.Clear();
        }

        private void LoadDefaultAchievements()
        {
            _achievements.Clear();
            AddAchievement(new Achievement { Id = "ach_first_blood", Title = "First Blood", Description = "Defeat your first enemy", RewardGold = 100 });
            AddAchievement(new Achievement { Id = "ach_boss_slayer", Title = "Titan Slayer", Description = "Defeat the Golem Titan", RewardGold = 1000 });
            AddAchievement(new Achievement { Id = "ach_rift_conqueror", Title = "Rift Master", Description = "Reach Floor 10 in Endless Rift", RewardGold = 2500 });
        }

        private void AddAchievement(Achievement ach)
        {
            _achievements[ach.Id] = ach;
        }

        public bool UnlockAchievement(string id)
        {
            if (!_achievements.TryGetValue(id, out var ach) || ach.IsUnlocked) return false;

            ach.IsUnlocked = true;
            EventBus.Publish(ach);
            GD.Print($"[AchievementManager] Achievement Unlocked: {ach.Title} (+{ach.RewardGold} Gold)");
            return true;
        }

        public IReadOnlyDictionary<string, Achievement> GetAllAchievements() => _achievements;
    }
}
