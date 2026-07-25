using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Prologue
{
    /// <summary>
    /// Chapter 1 quest chain builder registering initial main quests and side quests
    /// into the QuestDatabase.
    /// </summary>
    public class Chapter1QuestChain
    {
        public void RegisterChapter1Quests()
        {
            var q1 = new QuestDefinition
            {
                QuestId = "q_oakvale_awakening",
                DisplayName = "Awakening in Oakvale",
                Description = "Speak with Elder Alden at the village square to learn about your origins.",
                Category = QuestCategory.Main,
                RecommendedLevel = 1
            };
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 100 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 25 });
            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_talk_alden",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_elder_alden",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            var q2 = new QuestDefinition
            {
                QuestId = "q_forging_the_blade",
                DisplayName = "Forging a Hero's Blade",
                Description = "Gather 3 Iron Ore from the eastern hills for Blacksmith Thorin.",
                Category = QuestCategory.Main,
                RecommendedLevel = 2
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_oakvale_awakening" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 150 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 50 });
            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_gather_iron",
                Type = ObjectiveType.GatherResource,
                TargetId = "res_iron_ore",
                RequiredCount = 3
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            var q3 = new QuestDefinition
            {
                QuestId = "q_bandit_threat",
                DisplayName = "The Bandit Incursion",
                Description = "Defeat 5 Bandit Scouts troubling the Oakvale trade roads.",
                Category = QuestCategory.Main,
                RecommendedLevel = 3
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_forging_the_blade" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 250 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 100 });
            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_kill_bandits",
                Type = ObjectiveType.DefeatEnemy,
                TargetId = "enemy_bandit_scout",
                RequiredCount = 5
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            var q4 = new QuestDefinition
            {
                QuestId = "q_boss_skarr_encounter",
                DisplayName = "Confronting Baron Skarr",
                Description = "Defeat Bandit Leader Skarr in the Whispering Cavern.",
                Category = QuestCategory.Main,
                RecommendedLevel = 4
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_bandit_threat" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 500 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 250 });
            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_defeat_skarr",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_boss_skarr",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
