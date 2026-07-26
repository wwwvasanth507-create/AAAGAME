using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter9
{
    /// <summary>
    /// Chapter 9 Quest Chain builder registering quests into QuestDatabase.
    /// Handles fortress reconnaissance, prison catacombs sabotage, General Vaelis boss encounter, and major story revelation.
    /// </summary>
    public class Chapter9QuestChain
    {
        public void RegisterChapter9Quests()
        {
            // Quest 1: Fortress Reconnaissance
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter9_fortress_recon",
                DisplayName = "Breaching Obsidian Shadows",
                Description = "Infiltrate the outer battlements of the Fortress of Obsidian Shadows and scout guard rotations.",
                Category = QuestCategory.Main,
                RecommendedLevel = 34
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter8_shadow_champion_confrontation" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1600 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 700 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch9_scout_fortress",
                Type = ObjectiveType.ReachLocation,
                TargetId = "sector_fortress_battlements",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Prison Sabotage & Rescue
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter9_prison_sabotage",
                DisplayName = "Shadows of the Catacombs",
                Description = "Infiltrate the Prison Catacombs, liberate captive allied scouts, and disrupt legion supply vaults.",
                Category = QuestCategory.Main,
                RecommendedLevel = 35
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter9_fortress_recon" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 2000 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 900 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch9_rescue_scouts",
                Type = ObjectiveType.Interact,
                TargetId = "sector_fortress_prison",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Command Center Assault & Boss Encounter
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter9_command_assault",
                DisplayName = "Fall of General Vaelis",
                Description = "Assault the Grand Marshal's War Arena and defeat General Vaelis the Unforgiving.",
                Category = QuestCategory.Main,
                RecommendedLevel = 36
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter9_prison_sabotage" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 2800 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1400 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch9_defeat_vaelis",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_boss_general_vaelis",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);
        }
    }
}
