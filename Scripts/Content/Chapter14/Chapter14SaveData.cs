using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter14
{
    public class Chapter14SaveData
    {
        public bool FinalBossEngaged { get; set; } = false;
        public bool ArchSorcererMalakorDefeated { get; set; } = false;
        public BossPhaseType HighestPhaseReached { get; set; } = BossPhaseType.Phase1_HighWarden;
        public List<string> DefeatedPhaseIds { get; set; } = new();
        public List<string> AcquiredBossTrophyIds { get; set; } = new();
        public int SaveVersion { get; set; } = 41;
    }
}
