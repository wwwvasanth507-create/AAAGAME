using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Prologue
{
    public class PrologueSaveData
    {
        public List<TutorialStep> CompletedTutorialSteps { get; set; } = new();
        public List<string> InteractedNpcIds { get; set; } = new();
        public List<string> DiscoveredExplorationNodeIds { get; set; } = new();
        public bool IsPrologueCompleted { get; set; } = false;
        public int SaveVersion { get; set; } = 23;
    }
}
