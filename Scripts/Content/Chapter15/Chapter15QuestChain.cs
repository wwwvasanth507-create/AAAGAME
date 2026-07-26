using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter15
{
    /// <summary>
    /// Chapter 15 Epilogue Quest Chain builder registering quests into QuestDatabase.
    /// Handles Sun Spire restoration, settlement victory visits, epilogue celebration, and post-campaign horizon.
    /// </summary>
    public class Chapter15QuestChain
    {
        public void RegisterChapter15Quests()
        {
            // Quest 1: Sun Spire Restoration
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter15_sun_spire_restoration",
                DisplayName = "Dawn of Sol",
                Description = "Channel celestial light to ignite the restored Sun Spire atop the Obsidian Citadel.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter14_malakor_final_defeat" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 10000 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 5000 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch15_ignite_sun_spire",
                Type = ObjectiveType.Interact,
                TargetId = "prop_restored_sun_spire",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Settlement Victory Visits
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter15_settlement_victories",
                DisplayName = "Tidings of Peace",
                Description = "Visit Valenhold, Eternia Prime, and Sun Archivist strongholds to share news of victory.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter15_sun_spire_restoration" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 8000 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 4000 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch15_visit_valenhold",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_commander_valen",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Epilogue Celebration
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter15_epilogue_celebration",
                DisplayName = "A Hero's Welcome",
                Description = "Attend the grand victory feast held in your honor at the Grand Plaza of Eternia Prime.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter15_settlement_victories" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 12000 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 10000 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch15_attend_celebration",
                Type = ObjectiveType.ReachLocation,
                TargetId = "location_eternia_prime_plaza",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Post-Campaign Horizon
            var q4 = new QuestDefinition
            {
                QuestId = "q_chapter15_post_campaign_horizon",
                DisplayName = "The Endless Journey",
                Description = "Stand ready as Eternia's champion for post-game challenges beyond the horizon.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter15_epilogue_celebration" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 15000 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch15_enter_post_game",
                Type = ObjectiveType.ReachLocation,
                TargetId = "location_world_map_horizon",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
