using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter7
{
    public class Chapter7SaveData
    {
        public bool RegionalCrisisActive { get; set; } = false;
        public SiegeStage SavedSiegeStage { get; set; } = SiegeStage.NotStarted;
        public bool ShadowLordEmissaryDefeated { get; set; } = false;
        public bool Act2Completed { get; set; } = false;
        public List<string> ActiveCrisisEventIds { get; set; } = new();
        public List<string> TriggeredWorldAftermathFlags { get; set; } = new();
        public int SaveVersion { get; set; } = 34;
    }
}
