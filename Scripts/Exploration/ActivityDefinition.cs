using System;
using System.Collections.Generic;

namespace HeroOfEternia.Exploration
{
    public class ActivityDefinition
    {
        public string ActivityId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ActivityType Type { get; set; } = ActivityType.TreasureHunt;
        public ActivityCategory Category { get; set; } = ActivityCategory.Exploration;
        public List<string> BiomeRestrictions { get; set; } = new();
        public int Difficulty { get; set; } = 1;
        public string CompletionCondition { get; set; } = string.Empty;
        public string RewardTableId { get; set; } = string.Empty;
        public bool IsRepeatable { get; set; } = false;
        public float CooldownSeconds { get; set; } = 0.0f;
        public string VisualTheme { get; set; } = "default";
        public string AudioTheme { get; set; } = "default";
        public string LocalizationKey { get; set; } = string.Empty;
        public string DlcModuleId { get; set; } = string.Empty;
    }

    public class ActivityDatabase
    {
        private readonly Dictionary<string, ActivityDefinition> _activities = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterActivity(ActivityDefinition activity)
        {
            if (activity != null && !string.IsNullOrEmpty(activity.ActivityId))
            {
                _activities[activity.ActivityId] = activity;
            }
        }

        public ActivityDefinition? GetActivity(string activityId)
        {
            return _activities.TryGetValue(activityId, out var act) ? act : null;
        }

        public void RegisterDefaultActivities()
        {
            RegisterActivity(new ActivityDefinition
            {
                ActivityId = "act_hidden_chest_forest",
                DisplayName = "Hidden Forest Chest",
                Type = ActivityType.HiddenChest,
                Category = ActivityCategory.Exploration,
                BiomeRestrictions = new List<string> { "Forest" },
                RewardTableId = "reward_chest_tier1"
            });

            RegisterActivity(new ActivityDefinition
            {
                ActivityId = "act_rune_puzzle_shrine",
                DisplayName = "Rune Activation Shrine",
                Type = ActivityType.PuzzleShrine,
                Category = ActivityCategory.Puzzle,
                Difficulty = 2,
                RewardTableId = "reward_shrine_xp"
            });
        }
    }
}
