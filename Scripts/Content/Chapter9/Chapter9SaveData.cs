using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter9
{
    public class Chapter9SaveData
    {
        public bool FortressDiscovered { get; set; } = false;
        public bool GeneralVaelisDefeated { get; set; } = false;
        public LegionAlertLevel SavedAlertLevel { get; set; } = LegionAlertLevel.Low;
        public bool LegionSupplyDisrupted { get; set; } = false;
        public List<string> ClearedFortressSectorIds { get; set; } = new();
        public int SaveVersion { get; set; } = 36;
    }
}
