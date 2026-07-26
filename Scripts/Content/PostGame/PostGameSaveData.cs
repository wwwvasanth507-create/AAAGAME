using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.PostGame
{
    public class PostGameSaveData
    {
        public bool PostGameUnlocked { get; set; } = true;
        public List<string> DefeatedSuperBossIds { get; set; } = new();
        public Dictionary<string, float> RegionCompletionPercentages { get; set; } = new();
        public float OverallWorldCompletion { get; set; } = 92.5f;
        public List<string> AcquiredSuperTrophyIds { get; set; } = new();
        public int SaveVersion { get; set; } = 43;
    }
}
