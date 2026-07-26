using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter11
{
    /// <summary>
    /// Chapter 11 Quest Chain builder registering quests into QuestDatabase.
    /// Handles entry into The Astral Divide, legendary crafting research, elite world mini-boss trials, and champion confrontation.
    /// </summary>
    public class Chapter11QuestChain
    {
        public void RegisterChapter11Quests()
        {
            // Quest 1: Entry into Astral Divide
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter11_astral_divide_entry",
                DisplayName = "Across the Astral Divide",
                Description = "Cross the shattered border of Eternia Prime and establish a forward camp in The Crystal Wasteland.",
                Category = QuestCategory.Main,
                RecommendedLevel = 40
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_act3_conclusion" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 3000 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1200 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch11_reach_wasteland",
                Type = ObjectiveType.ReachLocation,
                TargetId = "zone_crystal_wasteland",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Legendary Research
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter11_legendary_research",
                DisplayName = "Forging the Astral Core",
                Description = "Gather Astral Essences and unlock Tier 5 Legendary Crafting at the Sun Spire Altar.",
                Category = QuestCategory.Main,
                RecommendedLevel = 41
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter11_astral_divide_entry" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 3800 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1500 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch11_unlock_recipe",
                Type = ObjectiveType.Interact,
                TargetId = "recipe_legendary_sol_blade",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Elite World Trial
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter11_elite_trial",
                DisplayName = "Trial of the Apex Behemoth",
                Description = "Defeat the Apex Crystal Behemoth roaming the Crystal Wasteland to claim the first Sun Core Fragment.",
                Category = QuestCategory.Main,
                RecommendedLevel = 42
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter11_legendary_research" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 4500 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 2000 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch11_defeat_behemoth",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "elite_crystal_behemoth",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Champion Confrontation
            var q4 = new QuestDefinition
            {
                QuestId = "q_chapter11_astral_champion_confrontation",
                DisplayName = "Breaching the Obsidian Threshold",
                Description = "Secure the perimeter of the Obsidian Citadel Gate and prepare the allied offensive for Chapter 12.",
                Category = QuestCategory.Main,
                RecommendedLevel = 43
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter11_elite_trial" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 5000 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 2500 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch11_reach_threshold",
                Type = ObjectiveType.ReachLocation,
                TargetId = "zone_obsidian_threshold",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
