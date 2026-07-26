using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter11
{
    public class Chapter11SaveData
    {
        public bool Act4Started { get; set; } = false;
        public bool ObsidianThresholdBreached { get; set; } = false;
        public List<string> DiscoveredZoneIds { get; set; } = new();
        public List<string> UnlockedLegendaryRecipeIds { get; set; } = new();
        public List<string> ClearedEliteEncounterIds { get; set; } = new();
        public Dictionary<string, int> SavedLegendaryMaterials { get; set; } = new();
        public int SaveVersion { get; set; } = 38;
    }
}
