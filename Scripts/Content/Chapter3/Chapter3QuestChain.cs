using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter3
{
    /// <summary>
    /// Chapter 3 and Act I Finale quest chain builder. Registers preparation, dungeon traversal,
    /// mini-boss defeat, final puzzle, boss encounter, and Act I conclusion quests.
    /// </summary>
    public class Chapter3QuestChain
    {
        public void RegisterChapter3Quests()
        {
            // Quest 1 — Preparation
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter3_preparation",
                DisplayName = "Before the Darkness Falls",
                Description = "Gather supplies and speak with Warden Kaelen before entering the Citadel of Void Shadows.",
                Category = QuestCategory.Main,
                RecommendedLevel = 13
            };
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 600 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 200 });
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_first_choice_decision" });
            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_talk_kaelen_prep",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_warden_kaelen",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2 — Dungeon Entry & Traversal
            var q2 = new QuestDefinition
            {
                QuestId = "q_citadel_entry",
                DisplayName = "Into the Void Citadel",
                Description = "Enter the Citadel of Void Shadows and reach the inner sanctum.",
                Category = QuestCategory.Main,
                RecommendedLevel = 14
            };
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 900 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 250 });
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter3_preparation" });
            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_reach_floor2",
                Type = ObjectiveType.ReachLocation,
                TargetId = "room_floor2_hazard_hall",
                RequiredCount = 1
            });
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_solve_rune_puzzle",
                Type = ObjectiveType.Interact,
                TargetId = "room_floor1_puzzle_rune",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3 — Mini-Boss
            var q3 = new QuestDefinition
            {
                QuestId = "q_shadow_knight_miniboss",
                DisplayName = "The Shadow Knight's Trial",
                Description = "Defeat the Shadow Knight guardian of the Citadel's inner sanctum.",
                Category = QuestCategory.Main,
                RecommendedLevel = 15
            };
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1200 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 350 });
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_citadel_entry" });
            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_defeat_shadow_knight",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_boss_shadow_knight",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4 — Final Boss
            var q4 = new QuestDefinition
            {
                QuestId = "q_void_knight_boss",
                DisplayName = "Commander Vareth — Void Knight",
                Description = "Confront and defeat Commander Vareth at the Throne of the Void Gate.",
                Category = QuestCategory.Main,
                RecommendedLevel = 17
            };
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 2500 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 700 });
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_shadow_knight_miniboss" });
            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_defeat_vareth",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "boss_commander_vareth",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);

            // Quest 5 — Act I Conclusion
            var q5 = new QuestDefinition
            {
                QuestId = "q_act1_conclusion",
                DisplayName = "Act I — The Hero's Oath",
                Description = "Return to Elder Alden in Oakvale. Act I is complete — the first seal of Eternia has been reinforced.",
                Category = QuestCategory.Main,
                RecommendedLevel = 18
            };
            q5.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 3000 });
            q5.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1000 });
            q5.CompletionRewards.Add(new QuestReward { Type = RewardType.Reputation, FloatValue = 500 });
            q5.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_void_knight_boss" });
            var b5 = new QuestBranch { BranchId = "branch_main" };
            b5.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_return_to_alden",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_elder_alden",
                RequiredCount = 1
            });
            q5.Branches.Add(b5);
            QuestDatabase.RegisterQuest(q5);
        }
    }
}
