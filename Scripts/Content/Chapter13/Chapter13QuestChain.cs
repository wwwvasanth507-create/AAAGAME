using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter13
{
    /// <summary>
    /// Chapter 13 Quest Chain builder registering quests into QuestDatabase.
    /// Handles breaching the Citadel of Obsidian Void, sabotaging the Machine Core, defeating Citadel mini-bosses, and reaching Malakor's Antechamber.
    /// </summary>
    public class Chapter13QuestChain
    {
        public void RegisterChapter13Quests()
        {
            // Quest 1: Citadel Outer Breach
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter13_breaching_citadel",
                DisplayName = "Breaching the Obsidian Citadel",
                Description = "Lead the Grand Alliance assault team through the shattered Outer Breach and capture the Fortified Gatehouse.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter12_final_alliance_briefing" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 5000 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 2500 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch13_capture_gatehouse",
                Type = ObjectiveType.ReachLocation,
                TargetId = "sector_fortified_gatehouse",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Machine Core Sabotage
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter13_machine_core_sabotage",
                DisplayName = "Sabotage of the Void Core Engine",
                Description = "Disable the Citadel's mechanical foundry and overload the void shield generators in the Machine Core.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter13_breaching_citadel" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 5500 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 2800 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch13_sabotage_core",
                Type = ObjectiveType.Interact,
                TargetId = "sector_machine_core",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Gatekeeper Confrontation
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter13_gatekeeper_confrontation",
                DisplayName = "Champions of the Sunless Void",
                Description = "Defeat the Archon of the Sunless Void and High Commander Vaelis Remnant on the Grand Promenade of Shadows.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter13_machine_core_sabotage" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 6500 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 3500 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch13_defeat_vaelis_remnant",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "encounter_vaelis_remnant",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Pre-Final Antechamber Threshold
            var q4 = new QuestDefinition
            {
                QuestId = "q_chapter13_pre_final_antechamber_reached",
                DisplayName = "Threshold of the Arch-Sorcerer",
                Description = "Unseal the final sanctuary doors using the 4 Citadel Keys and stand ready in the Pre-Final Antechamber.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter13_gatekeeper_confrontation" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 7500 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 4000 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch13_reach_antechamber",
                Type = ObjectiveType.ReachLocation,
                TargetId = "sector_pre_final_antechamber",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
