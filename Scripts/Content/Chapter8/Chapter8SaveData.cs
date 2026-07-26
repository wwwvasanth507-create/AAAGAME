using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter8
{
    public class Chapter8SaveData
    {
        public bool Act3Started { get; set; } = false;
        public bool ShadowFrontierDiscovered { get; set; } = false;
        public List<string> UnlockedTraversalTools { get; set; } = new();
        public List<string> DiscoveredFrontierZoneIds { get; set; } = new();
        public bool ShadowBehemothDefeated { get; set; } = false;
        public int SaveVersion { get; set; } = 35;
    }
}
