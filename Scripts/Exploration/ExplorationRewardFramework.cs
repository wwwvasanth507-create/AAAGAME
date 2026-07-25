using System;
using System.Collections.Generic;

namespace HeroOfEternia.Exploration
{
    public class ExplorationRewardPackage
    {
        public int Experience { get; set; } = 0;
        public int Gold { get; set; } = 0;
        public List<string> ItemIds { get; set; } = new();
        public List<string> MaterialIds { get; set; } = new();
        public string LoreId { get; set; } = string.Empty;
        public string AchievementId { get; set; } = string.Empty;
        public string TitleAwarded { get; set; } = string.Empty;
        public string FactionId { get; set; } = string.Empty;
        public int ReputationAmount { get; set; } = 0;
    }

    /// <summary>
    /// Reward distribution engine converting activity & secret completion into XP,
    /// currency, items, crafting materials, achievements, and reputation.
    /// </summary>
    public class ExplorationRewardFramework
    {
        public event Action<ExplorationRewardPackage>? OnRewardDistributed;

        public void DistributeReward(ExplorationRewardPackage package)
        {
            if (package != null)
            {
                OnRewardDistributed?.Invoke(package);
            }
        }
    }
}
