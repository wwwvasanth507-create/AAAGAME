using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    public enum RewardType
    {
        Experience,
        Currency,
        Equipment,
        CraftingMaterial,
        Achievement,
        Title
    }

    public record RewardItem
    {
        public RewardType Type { get; init; } = RewardType.Experience;
        public string ItemId { get; init; } = string.Empty;
        public int Quantity { get; init; } = 1;
        public string DisplayName { get; init; } = string.Empty;
    }

    public record RewardDefinition
    {
        public string RewardId { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public List<RewardItem> Items { get; init; } = new();
    }

    public class RewardClaimTracker
    {
        private readonly HashSet<string> _claimedRewards = new();

        public IReadOnlyCollection<string> ClaimedRewards => _claimedRewards;

        public bool IsClaimed(string rewardId)
        {
            return _claimedRewards.Contains(rewardId);
        }

        public bool Claim(string rewardId, Action<RewardItem> onRewardGranted)
        {
            if (IsClaimed(rewardId))
            {
                Logger.Warning($"RewardClaimTracker: Reward '{rewardId}' has already been claimed.");
                return false;
            }

            // Look up reward definition (in a real game, this registers to a database.
            // For prompt 12, we mock lookup matching standard reward IDs or generate dynamic entries)
            var def = GetRewardDefinition(rewardId);
            if (def == null)
            {
                Logger.Error($"RewardClaimTracker: Reward '{rewardId}' definition not found.");
                return false;
            }

            _claimedRewards.Add(rewardId);

            foreach (var item in def.Items)
            {
                onRewardGranted(item);
                Logger.Info($"RewardClaimTracker: Granted reward item '{item.DisplayName}' (Qty: {item.Quantity}) for '{rewardId}'.");
            }

            EventBus.Publish(new RewardClaimedEvent(rewardId, def.DisplayName));
            return true;
        }

        public void LoadClaimedList(IEnumerable<string> claimedIds)
        {
            _claimedRewards.Clear();
            foreach (var id in claimedIds)
            {
                _claimedRewards.Add(id);
            }
        }

        public void Reset()
        {
            _claimedRewards.Clear();
        }

        private static RewardDefinition? GetRewardDefinition(string rewardId)
        {
            // Simple mock database retrieval
            if (rewardId.Equals("reward_golem_titan", StringComparison.OrdinalIgnoreCase))
            {
                return new RewardDefinition
                {
                    RewardId = "reward_golem_titan",
                    DisplayName = "Titan's Legacy",
                    Items = new List<RewardItem>
                    {
                        new() { Type = RewardType.Experience, Quantity = 500, DisplayName = "EXP Bonus" },
                        new() { Type = RewardType.Currency, Quantity = 150, DisplayName = "Gold Coins" },
                        new() { Type = RewardType.Equipment, ItemId = "titan_core_plate", Quantity = 1, DisplayName = "Titan Core Plate" },
                        new() { Type = RewardType.Achievement, ItemId = "ach_titan_slayer", DisplayName = "Titan Slayer Achievement" }
                    }
                };
            }
            return null;
        }
    }

    public record RewardClaimedEvent(string RewardId, string RewardName);
}
