using System;
using System.Collections.Generic;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Content.Chapter4
{
    /// <summary>
    /// Act II opening quest chain builder — Eastern Ridgeline arc.
    /// Introduces Act II stakes, Seraphine companion rescue, Mirkwood Swamp unlock,
    /// and the first encounter with Malakor's field commander.
    /// </summary>
    public class Act2QuestChain
    {
        public void RegisterAct2Quests()
        {
            // Quest 1 — Act II Opens
            var q1 = new QuestDefinition
            {
                QuestId = "q_act2_begins",
                DisplayName = "Act II — Shadows Rising",
                Description = "Elder Alden reveals Malakor has dispatched his elite forces to the Eastern Ridgeline. A new threat emerges beyond the sealed Citadel.",
                Category = QuestCategory.Main,
                RecommendedLevel = 19
            };
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 800 });
            q1.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 300 });
            q1.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_act1_conclusion" });
            var b1 = new QuestBranch { BranchId = "branch_main" };
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_talk_alden_act2",
                Type = ObjectiveType.TalkToNpc,
                TargetId = "npc_elder_alden",
                RequiredCount = 1
            });
            b1.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_reach_ridgeline",
                Type = ObjectiveType.ReachLocation,
                TargetId = "region_eastern_ridgeline",
                RequiredCount = 1
            });
            q1.Branches.Add(b1);
            QuestDatabase.RegisterQuest(q1);

            // Quest 2 — Ridgeline Rescue (Companion Join)
            var q2 = new QuestDefinition
            {
                QuestId = "q_act2_ridgeline_rescue",
                DisplayName = "The Arcane Scout",
                Description = "A former Crown spy named Seraphine is held prisoner in a Shadow Cult encampment on the Eastern Ridgeline. Free her.",
                Category = QuestCategory.Main,
                RecommendedLevel = 20
            };
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1100 });
            q2.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 400 });
            q2.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_act2_begins" });
            var b2 = new QuestBranch { BranchId = "branch_main" };
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_defeat_camp_guards",
                Type = ObjectiveType.DefeatEnemy,
                TargetId = "enemy_shadow_cult_vanguard",
                RequiredCount = 8
            });
            b2.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_free_seraphine",
                Type = ObjectiveType.Interact,
                TargetId = "npc_seraphine_prison_cell",
                RequiredCount = 1
            });
            q2.Branches.Add(b2);
            QuestDatabase.RegisterQuest(q2);

            // Quest 3 — Ridgeline Watchtower
            var q3 = new QuestDefinition
            {
                QuestId = "q_act2_watchtower",
                DisplayName = "Reclaim the Ridgeline",
                Description = "Drive Malakor's vanguard from the Eastern Ridgeline watchtower and establish a forward base for the Valen Crown.",
                Category = QuestCategory.Main,
                RecommendedLevel = 21
            };
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1400 });
            q3.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 500 });
            q3.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_act2_ridgeline_rescue" });
            var b3 = new QuestBranch { BranchId = "branch_main" };
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_clear_watchtower",
                Type = ObjectiveType.ReachLocation,
                TargetId = "poi_ridgeline_watchtower",
                RequiredCount = 1
            });
            b3.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_defeat_vanguard_captain",
                Type = ObjectiveType.DefeatBoss,
                TargetId = "enemy_malakor_vanguard_captain",
                RequiredCount = 1
            });
            q3.Branches.Add(b3);
            QuestDatabase.RegisterQuest(q3);

            // Quest 4 — Mirkwood Unlock
            var q4 = new QuestDefinition
            {
                QuestId = "q_act2_mirkwood_intel",
                DisplayName = "Into the Swamps",
                Description = "Seraphine's intelligence indicates the Shadow Cult's new base lies deep within Mirkwood Swamps. Chart a path through.",
                Category = QuestCategory.Main,
                RecommendedLevel = 22
            };
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Experience, FloatValue = 1600 });
            q4.CompletionRewards.Add(new QuestReward { Type = RewardType.Gold, Quantity = 600 });
            q4.Prerequisites.Add(new QuestPrerequisite { PrerequisiteType = "quest_completed", RequiredId = "q_act2_watchtower" });
            var b4 = new QuestBranch { BranchId = "branch_main" };
            b4.Objectives.Add(new ObjectiveDefinition
            {
                ObjectiveId = "obj_find_swamp_path",
                Type = ObjectiveType.ReachLocation,
                TargetId = "region_mirkwood_swamps",
                RequiredCount = 1
            });
            q4.Branches.Add(b4);
            QuestDatabase.RegisterQuest(q4);
        }
    }
}
