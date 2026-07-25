using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story
{
    public class StoryProgressionSaveData
    {
        public string ActiveChapterId { get; set; } = string.Empty;
        public string ActiveMissionId { get; set; } = string.Empty;
        public int MissionCheckpointIndex { get; set; } = 0;
        public List<string> CompletedChapterIds { get; set; } = new();
        public List<string> CompletedMissionIds { get; set; } = new();
        public Dictionary<string, string> WorldStateFlags { get; set; } = new();
        public List<string> DiscoveredLoreIds { get; set; } = new();
        public int SaveVersion { get; set; } = 21;
    }
}
