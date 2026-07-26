using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter13
{
    public class Chapter13SaveData
    {
        public bool CitadelBreached { get; set; } = false;
        public bool PreFinalAntechamberReached { get; set; } = false;
        public string ActiveCheckpointId { get; set; } = "chk_outer_breach";
        public List<string> ClearedSectorIds { get; set; } = new();
        public List<string> DefeatedEncounterIds { get; set; } = new();
        public List<string> UnlockedShortcutIds { get; set; } = new();
        public int SaveVersion { get; set; } = 40;
    }
}
