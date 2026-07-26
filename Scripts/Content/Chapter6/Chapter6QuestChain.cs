using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter6
{
    /// <summary>
    /// Chapter 6 Quest Chain builder registering quests into QuestDatabase.
    /// Handles arrival in Eternia Prime, Guild induction, urban shadow investigations, and Boss Vesper climax.
    /// </summary>
    public class Chapter6QuestChain
    {
        public void RegisterChapter6Quests()
        {
            // Quest 1: Capital Arrival
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter6_capital_arrival",
                DisplayName = "Gates of Eternia Prime",
                Description = "Travel to the imperial capital of Eternia Prime and report to the Royal Court.",
                Category = QuestCategory.Main,
                RecommendedLevel = 25
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter5_dungeon_climax" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 800 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 350 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch6_enter_capital",
                Type = ObjectiveType.ReachLocation,
                TargetId = "city_eternia_prime",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Guild Induction
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter6_guild_induction",
                DisplayName = "The Guild Enclave",
                Description = "Visit the Grand Guild Enclave and enlist in an imperial guild.",
                Category = QuestCategory.Main,
                RecommendedLevel = 26
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter6_capital_arrival" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1000 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 450 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch6_talk_guildmaster",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_guildmaster_vane",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Regional Boss Climax
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter6_boss_climax",
                DisplayName = "Judgment of High Inquisitor Vesper",
                Description = "Confront High Inquisitor Vesper in the Sunken Catacombs beneath Eternia Prime.",
                Category = QuestCategory.Main,
                RecommendedLevel = 27
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter6_guild_induction" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1500 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 750 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch6_defeat_vesper",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_boss_high_inquisitor_vesper",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);
        }
    }
}
