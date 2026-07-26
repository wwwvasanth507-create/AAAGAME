using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter7
{
    /// <summary>
    /// Chapter 7 Quest Chain builder registering quests into QuestDatabase.
    /// Handles the regional crisis response, Valenhold siege defense, final assault on Shadow Lord Emissary, and Act II climax conclusion.
    /// </summary>
    public class Chapter7QuestChain
    {
        public void RegisterChapter7Quests()
        {
            // Quest 1: Crisis Mobilization
            var q1 = new QuestDefinition
            {
                QuestId = "q_chapter7_crisis_call",
                DisplayName = "Call to Arms — Crisis Mobilization",
                Description = "A regional void rift breach threatens Valenhold and Eternia Prime. Mobilize allied faction forces for siege defense.",
                Category = QuestCategory.Main,
                RecommendedLevel = 28
            };
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter6_boss_climax" });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1200 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 500 });

            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch7_rally_factions",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_commander_harek",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2: Siege Defense
            var q2 = new QuestDefinition
            {
                QuestId = "q_chapter7_siege_defense",
                DisplayName = "The Defense of Valenhold Gates",
                Description = "Hold the outer gate barricades against three incoming shadow waves and destroy siege engines.",
                Category = QuestCategory.Main,
                RecommendedLevel = 29
            };
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter7_crisis_call" });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1500 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 750 });

            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch7_defend_waves",
                Type = ObjectiveType.DefeatEnemy,
                TargetId = "enemy_veteran_bandit",
                RequiredCount = 15
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3: Act II Finale Boss Climax
            var q3 = new QuestDefinition
            {
                QuestId = "q_chapter7_final_assault",
                DisplayName = "Act II Finale — Fall of Malakor's Harbinger",
                Description = "Lead the counter-assault into the Shadow Crucible and defeat Malakor's Harbinger to end the regional siege.",
                Category = QuestCategory.Main,
                RecommendedLevel = 30
            };
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter7_siege_defense" });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 2500 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 1200 });

            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch7_defeat_malakor_emissary",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_boss_malakor_emissary",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4: Act II Epilogue
            var q4 = new QuestDefinition
            {
                QuestId = "q_act2_conclusion",
                DisplayName = "Act II Epilogue — Dawn of a Shattered Realm",
                Description = "Witness the aftermath of the regional siege, celebrate victory at the High Court, and prepare for Act III.",
                Category = QuestCategory.Main,
                RecommendedLevel = 30
            };
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_chapter7_final_assault" });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1000 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 500 });

            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_ch7_audience_king",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_high_king_roderick",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
