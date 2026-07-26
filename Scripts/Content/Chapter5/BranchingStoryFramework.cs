using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter5
{
    public enum StoryBranchId
    {
        Undecided,
        IronVanguardAlliance,
        SilverSyndicateAlliance,
        SylvanCircleNeutrality
    }

    public class ChoiceDecisionRecord
    {
        public string ChoiceId { get; set; } = "";
        public string Title { get; set; } = "";
        public string ChosenOption { get; set; } = "";
        public string SetWorldFlag { get; set; } = "";
        public int FactionInfluenceImpact { get; set; } = 0;
        public string FavoredFactionId { get; set; } = "";
    }

    /// <summary>
    /// Branching Narrative Framework for Chapter 5.
    /// Manages choice points, alignment paths (Vanguard / Syndicate / Sylvan), decision logs, and consequence triggers.
    /// </summary>
    public class BranchingStoryFramework
    {
        private readonly List<ChoiceDecisionRecord> _decisionHistory = new();

        public StoryBranchId ActiveBranch { get; private set; } = StoryBranchId.Undecided;
        public string ChosenFactionId { get; private set; } = "";

        public event Action<StoryBranchId, string>? OnBranchSelected;
        public event Action<ChoiceDecisionRecord>? OnChoiceMade;

        public bool SelectBranch(StoryBranchId branch, string factionId)
        {
            if (ActiveBranch != StoryBranchId.Undecided && ActiveBranch != branch)
            {
                Core.Logger.Warning($"BranchingStoryFramework: Switching active branch from '{ActiveBranch}' to '{branch}'.");
            }

            ActiveBranch = branch;
            ChosenFactionId = factionId;

            OnBranchSelected?.Invoke(branch, factionId);
            Core.Logger.Info($"BranchingStoryFramework: Selected narrative branch '{branch}' (Faction: {factionId}).");
            return true;
        }

        public void RecordChoice(ChoiceDecisionRecord decision)
        {
            if (decision == null || string.IsNullOrEmpty(decision.ChoiceId)) return;

            _decisionHistory.Add(decision);
            OnChoiceMade?.Invoke(decision);

            // Notify FactionPoliticsManager if present
            if (!string.IsNullOrEmpty(decision.FavoredFactionId) && decision.FactionInfluenceImpact != 0)
            {
                try
                {
                    var politics = Core.ServiceLocator.Get<Chapter4.FactionPoliticsManager>();
                    politics?.ModifyInfluence(decision.FavoredFactionId, decision.FactionInfluenceImpact);
                }
                catch
                {
                    // FactionPoliticsManager not registered in test mode
                }
            }

            Core.Logger.Info($"BranchingStoryFramework: Recorded choice '{decision.ChoiceId}' - Option: {decision.ChosenOption}.");
        }

        public IReadOnlyList<ChoiceDecisionRecord> GetDecisionHistory() => _decisionHistory.AsReadOnly();
    }
}
