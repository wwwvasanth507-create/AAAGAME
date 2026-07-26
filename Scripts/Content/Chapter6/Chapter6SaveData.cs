using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter6
{
    public class Chapter6SaveData
    {
        public bool CapitalDiscovered { get; set; } = false;
        public bool HighInquisitorVesperDefeated { get; set; } = false;
        public List<string> UnlockedCapitalDistricts { get; set; } = new();
        public Dictionary<string, string> JoinedGuildRanks { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> GuildReputationScores { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public int SaveVersion { get; set; } = 33;
    }
}
