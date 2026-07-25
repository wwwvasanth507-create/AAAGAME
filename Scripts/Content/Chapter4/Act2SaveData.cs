using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter4
{
    public class Act2SaveData
    {
        public bool RidgelineUnlocked { get; set; } = false;
        public bool MirkwoodUnlocked { get; set; } = false;
        public bool SeraphineJoined { get; set; } = false;
        public bool WatchtowerLiberated { get; set; } = false;
        public bool VanguardCaptainDefeated { get; set; } = false;
        public List<string> DiscoveredAct2Regions { get; set; } = new();
        public List<string> UnlockedAct2Recipes { get; set; } = new();
        public int SaveVersion { get; set; } = 26;
    }
}
