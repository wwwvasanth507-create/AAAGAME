# Quest System — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 19)

## Architecture Overview

The Quest System provides a complete data-driven framework for creating, managing, and tracking quests. Designers can create complex quests with branching objectives, multiple completion paths, and rich reward structures—all through JSON configuration. No code changes required.

### Core Components

```
QuestDatabase (Registry)
    ↓
QuestManager (Lifecycle Orchestrator)
    ├── AcceptQuest → QuestInstance
    ├── ObjectiveManager (Progress Tracking)
    │   ├── AdvanceObjective
    │   ├── CompleteObjective
    │   ├── FailObjective
    │   └── Branch Management
    ├── CompleteQuest → Reward Distribution
    ├── FailQuest → Failure Penalties
    └── AbandonQuest → Cleanup
```

### Data Flow

```
JSON Definition → QuestDatabase.Register → QuestManager.AcceptQuest → QuestInstance
    ↓
Active Quest → ObjectiveManager.AdvanceObjective → Completion → Rewards
    ↓
Branch Transition (if OnCompleteBranchId specified)
    ↓
Final Completion → QuestComplete event → Journal update
```

## Quest Definition (QuestDefinition)

Each quest supports these data fields:

| Field | Type | Description |
|-------|------|-------------|
| QuestId | string | Unique identifier |
| InternalName | string | Developer reference name |
| DisplayName | string | Player-facing name |
| Description | string | Full quest description |
| Category | QuestCategory | Main, Side, Faction, Daily, etc. |
| RecommendedLevel | int | Suggested player level |
| QuestGiverId | string | NPC ID who gives the quest |
| RequiredFactionId | string | Required faction membership |
| RequiredReputation | float | Minimum reputation score |
| Repeatable | bool | Can be completed more than once |
| MaxRepeatCount | int | Maximum repetitions (-1 = unlimited) |
| RepeatSchedule | string | "daily", "weekly", "monthly", "never" |
| Prerequisites | List<QuestPrerequisite> | Conditions to accept |
| Branches | List<QuestBranch> | Branching objective paths |
| FailureConditions | List<FailureCondition> | Conditions that fail the quest |
| TimeLimit | QuestTimeLimit | Optional time limit |
| CompletionRewards | List<QuestReward> | Rewards on completion |
| OptionalObjectiveRewards | List<QuestReward> | Rewards for optional objectives |
| FailurePenalties | List<QuestReward> | Penalties on failure |
| Localization Keys | string[] | TitleKey, DescriptionKey, etc. |
| DLC Fields | various | Future expansion hooks |

## Quest Categories (QuestCategory)

| Category | Description |
|----------|-------------|
| Main | Primary storyline quests |
| Side | Optional side content |
| Faction | Faction-specific quests |
| Guild | Guild-related quests |
| Tutorial | Tutorial/onboarding quests |
| Exploration | Discovery-based quests |
| Collection | Gather/collect items |
| Crafting | Craft specific items |
| Combat | Defeat enemies |
| Escort | Protect NPCs |
| Delivery | Transport items |
| Investigation | Solve mysteries |
| Puzzle | Logic challenges |
| WorldEvent | Dynamic world events |
| Timed | Time-limited quests |
| Daily | Repeatable daily quests |
| Weekly | Repeatable weekly quests |
| Seasonal | Seasonal event quests |

## Quest States

| State | Description |
|-------|-------------|
| Locked | Prerequisites not met |
| Available | Ready to accept |
| Active | Currently in progress |
| Completed | Successfully finished |
| Failed | Failed to complete |
| Abandoned | Player abandoned |
| RetryReady | Ready for retry attempt |

## Save Integration

Quest data is persisted via SaveProfile V15:
- QuestSaveData (active instances, completion records, history)
- QuestDatabase loads from `Settings/quest_database.json`

## Performance Characteristics

- O(1) quest lookups via Dictionary
- O(1) category/giver/faction lookups via indexed Dictionaries
- Thread-safe operations via lock
- Supports thousands of quests with minimal memory overhead
- Event-driven updates (no polling)

## Quest JSON Format

```json
{
  "questId": "example_quest",
  "internalName": "Example Quest",
  "displayName": "The Example",
  "category": "Side",
  "recommendedLevel": 5,
  "questGiverId": "npc_example",
  "repeatable": false,
  "branches": [
    {
      "branchId": "branch_start",
      "objectives": [
        {
          "objectiveId": "obj_talk",
          "type": "TalkToNpc",
          "targetId": "npc_target",
          "requiredCount": 1,
          "descriptionKey": "quest.example.obj_talk"
        }
      ],
      "onCompleteBranchId": "branch_reward"
    }
  ],
  "completionRewards": [
    { "type": "Experience", "floatValue": 500 },
    { "type": "Gold", "quantity": 100 }
  ]
}