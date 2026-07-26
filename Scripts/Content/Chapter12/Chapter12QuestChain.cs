using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter12
{
    /// <summary>
    /// Chapter 12 Quest Chain builder registering quests into QuestDatabase.
    /// Handles Grand Alliance council assembly, supply line liberation, Solwarden legendary artifact recovery, and final alliance briefing.
    /// </summary>
    public class Chapter12QuestChain
    {
        public void RegisterChapter12Quests()
        {
            // Quest 1: Alliance Council Assembly
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter12_alliance_council_assembly",
                DisplayName = "The Grand Alliance Assembly",
                Description = "Convene the leaders of Valenhold, Eternia Prime, Shadow Rangers, and Archivists in the Council Hall.",
                Category = QuestCategory.Main,
                RecommendedLevel = 44
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter11_astral_champion_confrontation" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 4000 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1800 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch12_convene_council",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_commander_valen",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Supply Line Liberation
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter12_supply_line_liberation",
                DisplayName = "Liberation of the Astral Supply Line",
                Description = "Escort the allied war caravan through The Crystal Wasteland and secure forward supply outposts.",
                Category = QuestCategory.Main,
                RecommendedLevel = 44
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter12_alliance_council_assembly" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 4800 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 2200 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch12_escort_caravan",
                Type = ObjectiveType.Interact,
                TargetId = "event_caravan_escort",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Solwarden Artifact Recovery
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter12_solwarden_artifact_recovery",
                DisplayName = "Forge of the Solwarden King",
                Description = "Recover the ancient Solwarden Astral Greatsword and craft the complete Tier 5 Solwarden Legendary Regalia.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter12_supply_line_liberation" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 6000 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 3000 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch12_acquire_solwarden",
                Type = ObjectiveType.Interact,
                TargetId = "item_legendary_solwarden_greatsword",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Final Alliance Briefing
            var q4 = new QuestDefinition
            {
                QuestId = "q_chapter12_final_alliance_briefing",
                DisplayName = "Eve of the Obsidian Siege",
                Description = "Review final alliance readiness, mobilize siege armies at the Obsidian Citadel Gate, and prepare for Chapter 13.",
                Category = QuestCategory.Main,
                RecommendedLevel = 45
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter12_solwarden_artifact_recovery" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 5000 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 2500 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch12_final_briefing",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_commander_valen",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
