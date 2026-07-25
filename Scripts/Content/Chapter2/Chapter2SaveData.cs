using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter2
{
    public class Chapter2SaveData
    {
        public List<string> DiscoveredSylvanwoodLocations { get; set; } = new();
        public int ElderwoodReputation { get; set; } = 0;
        public string RelicEntrustedTo { get; set; } = string.Empty; // "WardenKaelen" or "ScholarElora"
        public WorldPhase ActiveWorldPhase { get; set; } = WorldPhase.OakvalePeace;
        public int SaveVersion { get; set; } = 24;
    }
}
