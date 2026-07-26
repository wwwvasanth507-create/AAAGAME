using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter15
{
    public class Chapter15SaveData
    {
        public bool IsCampaignCompleted { get; set; } = false;
        public EndingChoice ChosenEnding { get; set; } = EndingChoice.Restoration_SolWarden;
        public string CompletionTimestamp { get; set; } = "";
        public float TotalPlayTimeHours { get; set; } = 48.5f;
        public float CompletionPercentage { get; set; } = 100.0f;
        public string AwardedTitle { get; set; } = "Champion of Sol";
        public bool HasViewedCredits { get; set; } = false;
        public List<string> UnlockedEpilogueLoreIds { get; set; } = new();
        public int SaveVersion { get; set; } = 42;
    }
}
