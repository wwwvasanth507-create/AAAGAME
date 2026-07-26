using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter10
{
    public class Chapter10SaveData
    {
        public bool TempleDiscovered { get; set; } = false;
        public bool Act3Completed { get; set; } = false;
        public bool CampaignRevelationWitnessed { get; set; } = false;
        public List<string> ClearedTempleChamberIds { get; set; } = new();
        public List<string> SolvedPuzzleIds { get; set; } = new();
        public List<string> DiscoveredLoreIds { get; set; } = new();
        public int SaveVersion { get; set; } = 37;
    }
}
