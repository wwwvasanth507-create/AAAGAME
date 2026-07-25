# Objective System — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 19)

## Architecture

The ObjectiveManager handles quest objective progression with support for 16 objective types, unlimited chains, prerequisites, branching, and optional objectives.

### Supported Objective Types

| Type | Description | Progress Method |
|------|-------------|-----------------|
| TalkToNpc | Speak with a specific NPC | AdvanceObjective |
| ReachLocation | Arrive at a location | AdvanceObjective |
| DefeatEnemy | Kill enemies of a type | AdvanceObjective (count) |
| DefeatBoss | Defeat a boss enemy | AdvanceObjective |
| CollectItem | Gather/collect items | AdvanceObjective (count) |
| CraftItem | Craft a specific item | AdvanceObjective |
| GatherResource | Harvest resources | AdvanceObjective (count) |
| DeliverItem | Deliver items to NPC | AdvanceObjective |
| Interact | Use an interactable | AdvanceObjective |
| EscortNpc | Escort an NPC to destination | AdvanceObjectiveFloat (distance) |
| Survive | Survive for a duration | AdvanceObjectiveFloat (time) |
| UseAbility | Use a specific ability | AdvanceObjective (count) |
| VisitSettlement | Discover a settlement | AdvanceObjective |
| ExploreArea | Explore a region | AdvanceObjectiveFloat (distance) |
| TriggerEvent | Trigger a world event | AdvanceObjective |
| Custom | Designer-defined logic | Custom handler |

### Prerequisite Chains

Objectives can have prerequisites. When all prerequisites are completed, the objective automatically activates.

### Branching

Objectives can trigger branch transitions on completion or failure via `OnCompleteBranchId` and `OnFailBranchId`.

### Optional Objectives

Optional objectives do not block quest completion but provide bonus rewards.