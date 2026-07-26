using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter5
{
    /// <summary>
    /// Chapter 5 Quest Chain builder registering quests into QuestDatabase.
    /// Handles infiltration, the pivotal alliance choice event, branch-specific objectives, and dungeon climax.
    /// </summary>
    public class Chapter5QuestChain
    {
        public void RegisterChapter5Quests()
        {
            // 1. Infiltration & Reconnaissance
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter5_infiltration",
                DisplayName = "Shadows Over Valenhold",
                Description = "Infiltrate the perimeter of the Stronghold of Iron & Shadow and identify the competing faction emissaries.",
                Category = QuestCategory.Main,
                RecommendedLevel = 22
            };
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 500 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 200 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch5_scout_stronghold",
                Type = ObjectiveType.ExploreArea,
                TargetId = "region_eastern_ridgeline",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // 2. Pivotal Choice Event
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter5_alliance_choice",
                DisplayName = "The Alignment Decision",
                Description = "Choose which faction to support for the Stronghold assault: Iron Vanguard, Silver Syndicate, or Sylvan Circle.",
                Category = QuestCategory.Main,
                RecommendedLevel = 23
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter5_infiltration" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 700 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 300 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch5_make_choice",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_commander_harek",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // 3. Dungeon Climax
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter5_dungeon_climax",
                DisplayName = "Fall of the Grand Marshal",
                Description = "Assault the Stronghold inner sanctuary and defeat Grand Marshal Kaelen.",
                Category = QuestCategory.Main,
                RecommendedLevel = 25
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter5_alliance_choice" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1200 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 500 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch5_defeat_boss",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_boss_grand_marshal_kaelen",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);
        }
    }
}
