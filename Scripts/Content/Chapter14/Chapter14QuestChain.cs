using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter14
{
    /// <summary>
    /// Chapter 14 Quest Chain builder registering quests into QuestDatabase.
    /// Handles entering Malakor's Throne Room, phase 1-3 boss defeats, and the final defeat of Arch-Sorcerer Malakor.
    /// </summary>
    public class Chapter14QuestChain
    {
        public void RegisterChapter14Quests()
        {
            // Quest 1: Entering Throne Room
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter14_entering_throne_room",
                DisplayName = "Throne Room of the Void",
                Description = "Pass through the Pre-Final Antechamber threshold and confront Arch-Sorcerer Malakor atop the Void Spire.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter13_pre_final_antechamber_reached" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 5000 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 2500 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch14_enter_throne_room",
                Type = ObjectiveType.ReachLocation,
                TargetId = "sector_malakor_throne_room",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Malakor Phase 1 & 2 Defeat
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter14_malakor_phase1_defeat",
                DisplayName = "Shattering the Sunfire Shield",
                Description = "Defeat Malakor's High Warden form and strip his corrupted solar armor.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter14_entering_throne_room" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 6000 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 3000 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch14_defeat_phase1",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "boss_malakor_phase1",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Malakor Phase 3 Defeat
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter14_malakor_phase2_defeat",
                DisplayName = "Conquering the Void Avatar",
                Description = "Survive gravity distortions and shatter Malakor's Void Avatar form.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter14_malakor_phase1_defeat" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 7500 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 4000 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch14_defeat_phase3",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "boss_malakor_phase3",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Final Malakor Defeat
            var q4 = new QuestDefinition
            {
                QuestId = "q_chapter14_malakor_final_defeat",
                DisplayName = "Fall of the Arch-Sorcerer",
                Description = "Deliver the final strike to the Unbound Void Core and extinguish Malakor's power forever.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter14_malakor_phase2_defeat" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 10000 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 5000 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch14_defeat_malakor_final",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "boss_malakor_void_avatar",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
