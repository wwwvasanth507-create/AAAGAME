using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HeroOfEternia.Core;

namespace HeroOfEternia.Quest
{
    // ==========================================================
    // QUEST CATEGORY ENUM
    // ==========================================================
    public enum QuestCategory
    {
        Main,
        Side,
        Faction,
        Guild,
        Tutorial,
        Exploration,
        Collection,
        Crafting,
        Combat,
        Escort,
        Delivery,
        Investigation,
        Puzzle,
        WorldEvent,
        Timed,
        Daily,
        Weekly,
        Seasonal
    }

    // ==========================================================
    // QUEST STATE ENUM
    // ==========================================================
    public enum QuestState
    {
        Locked,
        Available,
        Active,
        Completed,
        Failed,
        Abandoned,
        RetryReady
    }

    // ==========================================================
    // OBJECTIVE TYPE ENUM
    // ==========================================================
    public enum ObjectiveType
    {
        TalkToNpc,
        ReachLocation,
        DefeatEnemy,
        DefeatBoss,
        CollectItem,
        CraftItem,
        GatherResource,
        DeliverItem,
        Interact,
        EscortNpc,
        Survive,
        UseAbility,
        VisitSettlement,
        ExploreArea,
        TriggerEvent,
        Custom
    }

    // ==========================================================
    // OBJECTIVE STATE ENUM
    // ==========================================================
    public enum ObjectiveState
    {
        Locked,
        Active,
        Optional,
        Completed,
        Failed
    }

    // ==========================================================
    // REWARD TYPE ENUM
    // ==========================================================
    public enum RewardType
    {
        Experience,
        Gold,
        Item,
        Ability,
        Reputation,
        FactionStanding,
        Equipment,
        CraftingRecipe,
        Title,
        Custom
    }

    // ==========================================================
    // FAILURE CONDITION
    // ==========================================================
    public class FailureCondition
    {
        public string ConditionId { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";          // "death", "timeout", "npc_death", "item_lost", "location_exit", "custom"
        public float TimeoutSeconds { get; set; } = 0f;
        public string CustomCheckId { get; set; } = ""; // hook for custom failure logic
        public string FailMessageKey { get; set; } = ""; // localization key for failure message
    }

    // ==========================================================
    // PREREQUISITE
    // ==========================================================
    public class QuestPrerequisite
    {
        public string PrerequisiteType { get; set; } = ""; // "quest_completed", "quest_active", "level", "faction", "reputation", "item", "ability", "flag", "custom"
        public string RequiredId { get; set; } = "";       // quest ID, faction ID, item ID, etc.
        public float RequiredValue { get; set; } = 0f;     // level threshold, reputation threshold, count, etc.
        public string CustomCheckId { get; set; } = "";    // hook for custom prerequisite logic
    }

    // ==========================================================
    // REWARD DEFINITION
    // ==========================================================
    public class QuestReward
    {
        public RewardType Type { get; set; } = RewardType.Experience;
        public string RewardId { get; set; } = "";         // item ID, ability ID, reputation key, etc.
        public int Quantity { get; set; } = 1;
        public float FloatValue { get; set; } = 0f;        // for XP amounts, reputation amounts
        public bool Guaranteed { get; set; } = true;       // true = guaranteed, false = chance-based
        public float Chance { get; set; } = 1.0f;          // 0.0 - 1.0 probability
        public List<QuestReward> ChoiceGroup { get; set; } = new(); // player chooses one from group
    }

    // ==========================================================
    // OBJECTIVE DEFINITION
    // ==========================================================
    public class ObjectiveDefinition
    {
        public string ObjectiveId { get; set; } = "";
        public string InternalName { get; set; } = "";
        public ObjectiveType Type { get; set; } = ObjectiveType.Custom;
        public ObjectiveState InitialState { get; set; } = ObjectiveState.Active;
        public bool IsOptional { get; set; } = false;
        public string TargetId { get; set; } = "";         // NPC ID, location ID, enemy ID, item ID, etc.
        public int RequiredCount { get; set; } = 1;
        public int CurrentCount { get; set; } = 0;
        public float RequiredFloat { get; set; } = 0f;     // survival time, distance, etc.
        public float CurrentFloat { get; set; } = 0f;
        public string DescriptionKey { get; set; } = "";   // localization key
        public string LocationKey { get; set; } = "";      // region/settlement/biome where objective takes place
        public string CompletionCheckId { get; set; } = ""; // hook for custom completion logic
        public Dictionary<string, string> CustomData { get; set; } = new();
        
        // Branching hooks
        public string OnCompleteBranchId { get; set; } = ""; // branch to take when completed
        public string OnFailBranchId { get; set; } = "";     // branch to take when failed

        // Chain support
        public List<string> PrerequisiteObjectiveIds { get; set; } = new(); // must complete before this activates
    }

    // ==========================================================
    // QUEST BRANCH
    // ==========================================================
    public class QuestBranch
    {
        public string BranchId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string DescriptionKey { get; set; } = "";
        public List<QuestPrerequisite> Conditions { get; set; } = new();
        public List<ObjectiveDefinition> Objectives { get; set; } = new();
        public string OnCompleteBranchId { get; set; } = ""; // next branch after completing this
        public string OnFailBranchId { get; set; } = "";     // alternate path on failure
    }

    // ==========================================================
    // TIME LIMIT
    // ==========================================================
    public class QuestTimeLimit
    {
        public bool HasTimeLimit { get; set; } = false;
        public float TotalSeconds { get; set; } = 0f;
        public float RemainingSeconds { get; set; } = 0f;
        public bool PausesWhenOffline { get; set; } = true;
        public bool FailOnExpire { get; set; } = true;
        public string OnExpireBranchId { get; set; } = ""; // branch to trigger on expiry
    }

    // ==========================================================
    // QUEST DEFINITION (complete data-driven quest)
    // ==========================================================
    public class QuestDefinition
    {
        public string QuestId { get; set; } = "";
        public string InternalName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public QuestCategory Category { get; set; } = QuestCategory.Side;
        public int RecommendedLevel { get; set; } = 1;
        public string QuestGiverId { get; set; } = "";          // NPC ID who gives the quest
        public string RequiredFactionId { get; set; } = "";
        public float RequiredReputation { get; set; } = 0f;
        public string RequiredFactionStanding { get; set; } = ""; // "neutral", "friendly", "honored", etc.
        public bool Repeatable { get; set; } = false;
        public int MaxRepeatCount { get; set; } = -1;          // -1 = unlimited for dailies/weeklies
        public string RepeatSchedule { get; set; } = "";       // "daily", "weekly", "monthly", "never"
        
        public List<QuestPrerequisite> Prerequisites { get; set; } = new();
        public List<QuestBranch> Branches { get; set; } = new();
        public List<FailureCondition> FailureConditions { get; set; } = new();
        public QuestTimeLimit TimeLimit { get; set; } = new();
        
        public List<QuestReward> CompletionRewards { get; set; } = new();
        public List<QuestReward> OptionalObjectiveRewards { get; set; } = new();
        public List<QuestReward> FailurePenalties { get; set; } = new();
        
        // Localization
        public string TitleKey { get; set; } = "";
        public string DescriptionKey { get; set; } = "";
        public string ProgressKey { get; set; } = "";
        public string CompletionKey { get; set; } = "";
        public string FailureKey { get; set; } = "";
        public string AbandonKey { get; set; } = "";
        
        // Future DLC/expansion hooks
        public string DlcRequirement { get; set; } = "";
        public string ExpansionFlag { get; set; } = "";
        public int ContentVersion { get; set; } = 1;
        public bool IsSeasonal { get; set; } = false;
        public string SeasonId { get; set; } = "";
        
        // Metadata
        public string Author { get; set; } = "";
        public string Notes { get; set; } = "";
        public bool IsEnabled { get; set; } = true;
        
        // Override group quest values
        public bool IsGroupQuest { get; set; } = false;
        public int MinGroupSize { get; set; } = 1;
        public int MaxGroupSize { get; set; } = 4;
        public bool ScaleWithGroupSize { get; set; } = true;
    }

    // ==========================================================
    // QUEST INSTANCE (runtime state for an active quest)
    // ==========================================================
    public class QuestInstance
    {
        public string InstanceId { get; set; } = "";           // unique per-activation
        public string QuestId { get; set; } = "";
        public QuestState State { get; set; } = QuestState.Available;
        
        public string ActiveBranchId { get; set; } = "";
        public List<ObjectiveRuntimeState> ObjectiveStates { get; set; } = new();
        
        public float TimeRemaining { get; set; } = 0f;
        public int RepeatCount { get; set; } = 0;
        public bool HasFailed { get; set; } = false;
        public bool HasCompleted { get; set; } = false;
        
        public DateTime AcceptedTime { get; set; }
        public DateTime CompletedTime { get; set; }
        public float TotalPlayTimeOnQuest { get; set; } = 0f;
        
        public Dictionary<string, string> QuestVariables { get; set; } = new();
        
        // Future co-op
        public List<string> GroupMemberIds { get; set; } = new();
        public Dictionary<string, int> GroupProgress { get; set; } = new();

        public QuestDefinition GetDefinition()
        {
            return QuestDatabase.GetQuest(QuestId);
        }
    }

    // ==========================================================
    // OBJECTIVE RUNTIME STATE
    // ==========================================================
    public class ObjectiveRuntimeState
    {
        public string ObjectiveId { get; set; } = "";
        public ObjectiveState State { get; set; } = ObjectiveState.Locked;
        public int CurrentCount { get; set; } = 0;
        public float CurrentFloat { get; set; } = 0f;
        public bool IsCompleted { get; set; } = false;
        public bool IsFailed { get; set; } = false;
        public Dictionary<string, string> Variables { get; set; } = new();
    }

    // ==========================================================
    // QUEST SAVE DATA
    // ==========================================================
    public class QuestSaveData
    {
        public List<QuestInstance> ActiveQuests { get; set; } = new();
        public List<string> CompletedQuestIds { get; set; } = new();
        public List<string> FailedQuestIds { get; set; } = new();
        public List<string> AbandonedQuestIds { get; set; } = new();
        public Dictionary<string, int> RepeatCounts { get; set; } = new(); // questId -> count
        public List<QuestHistoryEntry> QuestHistory { get; set; } = new();
    }

    // ==========================================================
    // QUEST HISTORY ENTRY
    // ==========================================================
    public class QuestHistoryEntry
    {
        public string QuestId { get; set; } = "";
        public string QuestName { get; set; } = "";
        public QuestCategory Category { get; set; }
        public QuestState FinalState { get; set; }
        public DateTime AcceptedTime { get; set; }
        public DateTime CompletionTime { get; set; }
        public float TimeSpentSeconds { get; set; }
        public string CompletedBy { get; set; } = ""; // "player" or branch ID
    }
}