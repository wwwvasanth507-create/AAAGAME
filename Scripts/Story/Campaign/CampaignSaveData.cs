using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story.Campaign
{
    public class CampaignSaveData
    {
        public List<string> DiscoveredRegionIds { get; set; } = new();
        public List<string> DefeatedVillainIds { get; set; } = new();
        public Dictionary<string, int> CharacterRelationshipLevels { get; set; } = new();
        public string ActiveActId { get; set; } = "act_0_prologue";
        public int SaveVersion { get; set; } = 22;
    }
}
