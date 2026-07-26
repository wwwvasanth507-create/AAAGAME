using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter10
{
    /// <summary>
    /// Chapter 10 Quest Chain builder registering quests into QuestDatabase.
    /// Handles discovery of the Temple of the Eternal Sun, puzzle sanctum progression, campaign revelation, and Act III conclusion.
    /// </summary>
    public class Chapter10QuestChain
    {
        public void RegisterChapter10Quests()
        {
            // Quest 1: Temple Discovery
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter10_temple_discovery",
                DisplayName = "Sanctuary of the Ancient Sun",
                Description = "Locate and unseal the Portal of Astral Light to enter the Temple of the Eternal Sun.",
                Category = QuestCategory.Main,
                RecommendedLevel = 36
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter9_command_assault" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 2000 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 900 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch10_enter_temple",
                Type = ObjectiveType.ReachLocation,
                TargetId = "chamber_temple_entrance",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Puzzle Sanctum Progression
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter10_puzzle_sanctum",
                DisplayName = "Secrets of the Water & Light",
                Description = "Solve the Water Prism Reflection Array and Weight Plate Sequence to access the Core Astral Vault.",
                Category = QuestCategory.Main,
                RecommendedLevel = 37
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter10_temple_discovery" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 2500 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1100 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch10_solve_prism",
                Type = ObjectiveType.Interact,
                TargetId = "puzzle_light_reflection_array",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Act III Campaign Revelation
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter10_astral_revelation",
                DisplayName = "The Shattered Crown Revelation",
                Description = "Reach the Core Astral Vault, discover the Golden Codex plate, and witness the true origin of Malakor's Void Core.",
                Category = QuestCategory.Main,
                RecommendedLevel = 38
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter10_puzzle_sanctum" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 3500 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1800 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch10_discover_codex",
                Type = ObjectiveType.Interact,
                TargetId = "lore_codex_malakor_truth",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Act III Epilogue
            var q4 = new QuestDefinition
            {
                QuestId = "q_act3_conclusion",
                DisplayName = "Act III Epilogue — Twilight of the Ancients",
                Description = "Conclude Act III, unlock late-game temple recipes, and prepare to enter Act IV.",
                Category = QuestCategory.Main,
                RecommendedLevel = 38
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter10_astral_revelation" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1500 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 800 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch10_report_revelation",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_archivist_orion",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
