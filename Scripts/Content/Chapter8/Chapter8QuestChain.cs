using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter8
{
    /// <summary>
    /// Chapter 8 Quest Chain builder registering quests into QuestDatabase.
    /// Handles entry into The Shadow Frontier, traversal chasm crossing, discovery of ancient sanctuaries, and regional champion confrontation.
    /// </summary>
    public class Chapter8QuestChain
    {
        public void RegisterChapter8Quests()
        {
            // Quest 1: Into the Shadow Frontier
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter8_shadow_frontier_entry",
                DisplayName = "Act III — Beyond the Wall of Shadows",
                Description = "Enter the high-level wasteland of The Shadow Frontier and establish an advance outpost in Corrupted Whispering Woods.",
                Category = QuestCategory.Main,
                RecommendedLevel = 31
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_act2_conclusion" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1400 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 600 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch8_reach_frontier",
                Type = ObjectiveType.ReachLocation,
                TargetId = "region_shadow_frontier",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Traversal Challenge
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter8_traversal_challenge",
                DisplayName = "Crossing the Dread Ravine",
                Description = "Use the Iron Grapple Hook to traverse the Dread Ravine chasm and locate Ruined Fort Ironwood.",
                Category = QuestCategory.Main,
                RecommendedLevel = 32
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter8_shadow_frontier_entry" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1800 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 800 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch8_use_grapple",
                Type = ObjectiveType.Interact,
                TargetId = "node_dread_ravine_grapple",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Regional Champion Confrontation
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter8_shadow_champion_confrontation",
                DisplayName = "Champion of the Obsidian Crag",
                Description = "Ascend the Obsidian Crag Sanctuary and defeat the Shadow Behemoth Warlord.",
                Category = QuestCategory.Main,
                RecommendedLevel = 34
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter8_traversal_challenge" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 2600 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1200 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch8_defeat_behemoth",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_shadow_behemoth",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);
        }
    }
}
