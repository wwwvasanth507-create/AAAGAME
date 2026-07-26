using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter5
{
    public class Chapter5SaveData
    {
        public StoryBranchId SelectedBranch { get; set; } = StoryBranchId.Undecided;
        public string ChosenFactionId { get; set; } = "";
        public bool InfiltrationCompleted { get; set; } = false;
        public bool AllianceChoiceCompleted { get; set; } = false;
        public bool StrongholdBossDefeated { get; set; } = false;
        public List<string> RecordedChoiceIds { get; set; } = new();
        public List<string> ActiveConsequenceIds { get; set; } = new();
        public int MaxClearedFloor { get; set; } = 0;
        public int SaveVersion { get; set; } = 32;
    }
}
