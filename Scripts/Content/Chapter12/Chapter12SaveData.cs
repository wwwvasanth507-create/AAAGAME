using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter12
{
    public class Chapter12SaveData
    {
        public bool AllianceCouncilConvened { get; set; } = false;
        public bool FinalBriefingCompleted { get; set; } = false;
        public int AllianceReadinessPercentage { get; set; } = 85;
        public Dictionary<string, int> FactionLoyaltyRatings { get; set; } = new();
        public List<string> CompletedWarEventIds { get; set; } = new();
        public List<string> AcquiredLegendaryPieceIds { get; set; } = new();
        public int SaveVersion { get; set; } = 39;
    }
}
