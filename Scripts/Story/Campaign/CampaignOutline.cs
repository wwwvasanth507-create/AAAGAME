using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story.Campaign
{
    public class CampaignActInfo
    {
        public string ActId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public float EstimatedPlaytimeHours { get; set; } = 10f;
        public int RecommendedLevelMin { get; set; } = 1;
        public int RecommendedLevelMax { get; set; } = 10;
        public List<string> FeaturedRegionIds { get; set; } = new();
        public List<string> KeyVillainIds { get; set; } = new();
    }

    public class CampaignDatabase
    {
        private readonly Dictionary<string, CampaignActInfo> _acts = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterAct(CampaignActInfo act)
        {
            if (act != null && !string.IsNullOrEmpty(act.ActId))
            {
                _acts[act.ActId] = act;
            }
        }

        public CampaignActInfo? GetAct(string actId)
        {
            return _acts.TryGetValue(actId, out var a) ? a : null;
        }

        public IReadOnlyCollection<CampaignActInfo> GetAllActs() => _acts.Values;

        public void RegisterDefaultCampaign()
        {
            RegisterAct(new CampaignActInfo
            {
                ActId = "act_0_prologue",
                DisplayName = "Prologue: Awakening in Oakvale",
                Summary = "The hero awakens in Oakvale, learns basic combat, and defends the village from a surprise goblin/shadow raid.",
                EstimatedPlaytimeHours = 2.5f,
                RecommendedLevelMin = 1,
                RecommendedLevelMax = 5,
                FeaturedRegionIds = new List<string> { "region_starting_kingdom" }
            });

            RegisterAct(new CampaignActInfo
            {
                ActId = "act_1_shadows",
                DisplayName = "Act I: Shadows over Valenoria",
                Summary = "Investigating corrupted shrines across the Sylvanwood Wilds and uncovering Baron Skarr's betrayal.",
                EstimatedPlaytimeHours = 12f,
                RecommendedLevelMin = 5,
                RecommendedLevelMax = 18,
                FeaturedRegionIds = new List<string> { "region_starting_kingdom", "region_forest", "region_swamp" },
                KeyVillainIds = new List<string> { "villain_baron_skarr" }
            });

            RegisterAct(new CampaignActInfo
            {
                ActId = "act_2_sun_and_frost",
                DisplayName = "Act II: Flame and Frost",
                Summary = "Seeking ancient Titan relics in the Sunfire Wastes and Frostpeak Mountains.",
                EstimatedPlaytimeHours = 20f,
                RecommendedLevelMin = 18,
                RecommendedLevelMax = 32,
                FeaturedRegionIds = new List<string> { "region_desert", "region_frozen_north", "region_highlands" }
            });

            RegisterAct(new CampaignActInfo
            {
                ActId = "act_3_fallen_empire",
                DisplayName = "Act III: The Ruined Crown",
                Summary = "Journeying through Ashen Peaks and the Eternian Empire Ruins to stop Malakor from opening the Void Gate.",
                EstimatedPlaytimeHours = 25f,
                RecommendedLevelMin = 32,
                RecommendedLevelMax = 45,
                FeaturedRegionIds = new List<string> { "region_volcanic", "region_ancient_ruins", "region_magical_islands" }
            });

            RegisterAct(new CampaignActInfo
            {
                ActId = "act_4_climax",
                DisplayName = "Act IV: The Void Siege",
                Summary = "Final assault on the Abyssal Wastes to confront Malakor the Void Lord and decide the fate of Eternia.",
                EstimatedPlaytimeHours = 15f,
                RecommendedLevelMin = 45,
                RecommendedLevelMax = 55,
                FeaturedRegionIds = new List<string> { "region_dark_wastes" },
                KeyVillainIds = new List<string> { "villain_malakor_voidlord" }
            });
        }
    }
}
