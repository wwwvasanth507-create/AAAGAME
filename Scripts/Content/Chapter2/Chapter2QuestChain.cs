using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter2
{
    /// <summary>
    /// Chapter 2 quest chain builder registering main storyline missions for Sylvanwood Wilds
    /// into the QuestDatabase.
    /// </summary>
    public class Chapter2QuestChain
    {
        public void RegisterChapter2Quests()
        {
            var q1 = new QuestDefinition
            {
                QuestId = "q_sylvanwood_investigation",
                DisplayName = "Whispers in Sylvanwood",
                Description = "Travel to Elderwood Grove and report to Warden Kaelen regarding shadow cult activity.",
                Category = QuestCategory.Main,
                RecommendedLevel = 6
            };
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 400 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 100 });
            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_talk_kaelen",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_warden_kaelen",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            var q2 = new QuestDefinition
            {
                QuestId = "q_corrupted_shrine",
                DisplayName = "The Blighted Shrine",
                Description = "Investigate the corrupted leyline shrine in the Sylvanwood Canopy.",
                Category = QuestCategory.Main,
                RecommendedLevel = 7
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_sylvanwood_investigation" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 550 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 150 });
            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_clear_corrupted_beasts",
                Type = ObjectiveType.DefeatEnemy,
                TargetId = "enemy_corrupted_boar",
                RequiredCount = 6
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            var q3 = new QuestDefinition
            {
                QuestId = "q_ruin_guardian_boss",
                DisplayName = "Secrets of Aethelgard",
                Description = "Enter the Ancient Elven Ruins and defeat the Ancient Ruin Guardian.",
                Category = QuestCategory.Main,
                RecommendedLevel = 10
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_corrupted_shrine" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1000 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 400 });
            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_defeat_ruin_guardian",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_boss_ruin_guardian",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            var q4 = new QuestDefinition
            {
                QuestId = "q_first_choice_decision",
                DisplayName = "The Fate of Aethelgard Relic",
                Description = "Decide whether to entrust the ancient Titan Relic to Warden Kaelen or Scholar Elora.",
                Category = QuestCategory.Main,
                RecommendedLevel = 11
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_ruin_guardian_boss" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 800 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 300 });
            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_make_choice",
                Type = ObjectiveType.Interact,
                TargetId = "relic_choice_pedestal",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
