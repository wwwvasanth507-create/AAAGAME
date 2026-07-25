using System;
using System.Collections.Generic;

namespace HeroOfEternia.Exploration
{
    public class ExplorationContentSaveData
    {
        public List<string> CompletedActivityIds { get; set; } = new();
        public List<string> SolvedPuzzleIds { get; set; } = new();
        public List<string> DiscoveredSecretIds { get; set; } = new();
        public List<string> CollectedItemIds { get; set; } = new();
        public int SaveVersion { get; set; } = 20;
    }
}
