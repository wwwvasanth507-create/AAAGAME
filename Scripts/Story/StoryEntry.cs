using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story
{
    public class StoryEntry
    {
        public string StoryId { get; set; } = string.Empty;
        public string ChapterId { get; set; } = string.Empty;
        public string ActId { get; set; } = string.Empty;
        public int MissionOrder { get; set; } = 1;
        public string InternalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RecommendedLevel { get; set; } = 1;
        public List<string> RequiredStoryFlags { get; set; } = new();
        public List<string> RequiredQuestStates { get; set; } = new();
        public int RequiredReputation { get; set; } = 0;
        public float RequiredExplorationProgress { get; set; } = 0.0f;
        public Dictionary<string, string> RequiredWorldState { get; set; } = new();
        public string RewardTableId { get; set; } = string.Empty;
        public string LocalizationKey { get; set; } = string.Empty;
        public string DlcModuleId { get; set; } = string.Empty;
    }

    public class StoryDatabase
    {
        private readonly Dictionary<string, StoryEntry> _entries = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterStoryEntry(StoryEntry entry)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.StoryId))
            {
                _entries[entry.StoryId] = entry;
            }
        }

        public StoryEntry? GetStoryEntry(string storyId)
        {
            return _entries.TryGetValue(storyId, out var e) ? e : null;
        }

        public void RegisterDefaultEntries()
        {
            RegisterStoryEntry(new StoryEntry
            {
                StoryId = "story_prologue_01",
                ChapterId = "chapter_prologue",
                ActId = "act_1",
                MissionOrder = 1,
                InternalName = "Prologue Awakening",
                DisplayName = "Awakening of Eternia",
                Description = "The journey begins as ancient seals fracture.",
                RecommendedLevel = 1
            });
        }
    }
}
