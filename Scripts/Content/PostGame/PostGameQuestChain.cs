using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.PostGame
{
    /// <summary>
    /// Post-Game Investigative Quest Chain builder registering quests into QuestDatabase.
    /// Handles investigating ancient rift anomalies, defeating the Chronos Titan, conquering the Astral Leviathan, and confronting the Sun King's Ascended Memory.
    /// </summary>
    public class PostGameQuestChain
    {
        public void RegisterPostGameQuests()
        {
            // Quest 1: Investigating Astral Rifts
            var q1 = new QuestDefinition
            {
                QuestId = "q_postgame_investigating_astral_rifts",
                DisplayName = "Anomalies in the Rift",
                Description = "Investigate spatial rift distortions appearing across the Astral Divide after Malakor's defeat.",
                Category = QuestCategory.Side,
                RecommendedLevel = 50
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter15_post_campaign_horizon" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 12000 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 6000 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_pg_investigate_rift",
                Type = ObjectiveType.ReachLocation,
                TargetId = "location_astral_rift_chamber",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Chronos Titan Defeat
            var q2 = new QuestDefinition
            {
                QuestId = "q_postgame_chronos_titan_defeat",
                DisplayName = "Master of Fractured Time",
                Description = "Confront and defeat Chronos, Titan of Fractured Time, inside the Temporal Vault.",
                Category = QuestCategory.Side,
                RecommendedLevel = 50
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_postgame_investigating_astral_rifts" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 15000 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 8000 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_pg_defeat_chronos",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "boss_chronos_titan",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Astral Leviathan Defeat
            var q3 = new QuestDefinition
            {
                QuestId = "q_postgame_astral_leviathan_defeat",
                DisplayName = "Terror of the Abyss",
                Description = "Plunge into the Void Astral Abyss and vanquish the colossal Astral Leviathan.",
                Category = QuestCategory.Side,
                RecommendedLevel = 52
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_postgame_chronos_titan_defeat" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 18000 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 10000 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_pg_defeat_leviathan",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "boss_astral_leviathan",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Sol Prime Ascended Memory Confrontation
            var q4 = new QuestDefinition
            {
                QuestId = "q_postgame_sol_prime_confrontation",
                DisplayName = "The First Champion",
                Description = "Enter the Sanctum of the First Dawn and prove your worth to the Sun King's Ascended Memory.",
                Category = QuestCategory.Side,
                RecommendedLevel = 55
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_postgame_astral_leviathan_defeat" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 25000 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 15000 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_pg_defeat_sol_prime",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "boss_sol_prime_avatar",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
