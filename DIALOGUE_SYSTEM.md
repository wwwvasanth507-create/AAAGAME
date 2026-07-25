# Dialogue System — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 19)

## Architecture Overview

The Dialogue System provides a complete data-driven framework for creating branching conversations. Designers create conversations with conditional dialogue entries, player choices with effects, and hooks for quests, merchants, services, and cinematics—all through JSON configuration.

### Core Components

```
DialogueDatabase (Registry)
    ↓
DialogueManager (Execution Engine)
    ├── StartConversation → Entry condition check
    ├── AdvanceDialogue → Next dialogue resolution
    ├── SelectChoice → Choice effect application
    └── EndConversation → Cleanup
NarrativeManager (Condition Evaluation)
JournalManager (Dialogue Logging)
```

## Dialogue Data Models

### ConversationDefinition
| Field | Type | Description |
|-------|------|-------------|
| ConversationId | string | Unique identifier |
| NpcId | string | Primary NPC for this conversation |
| StartingDialogueId | string | First dialogue entry |
| Dialogues | List<DialogueEntry> | All dialogue entries |
| EntryConditions | List<DialogueCondition> | Conditions to start conversation |
| OnStartFlag | string | Flag set when conversation starts |
| OnEndFlag | string | Flag set when conversation ends |
| MaxDepth | int | Max nesting depth (loop prevention) |

### DialogueEntry
| Field | Type | Description |
|-------|------|-------------|
| DialogueId | string | Unique identifier |
| SpeakerId | string | NPC ID or "player"/"narrator" |
| SpeakerType | DialogueSpeakerType | Npc, Player, Narrator, System |
| TextKey | string | Localization key for dialogue text |
| AudioKey | string | Voice clip reference |
| EmotionHook | string | Emotion animation to play |
| AnimationHook | string | Animation to play |
| CameraHook | string | Camera shot type |
| VfxHook | string | Visual effect to trigger |
| Conditions | List<DialogueCondition> | Conditions to display this dialogue |
| Choices | List<DialogueChoice> | Player response options |
| QuestHookId | string | Quest ID for hooks |
| QuestHookAction | string | "advance_objective", "complete_objective", "set_flag" |
| NextDialogueId | string | Auto-advance target |
| IsEndOfConversation | bool | Ends conversation |

### DialogueChoice
| Field | Type | Description |
|-------|------|-------------|
| ChoiceId | string | Unique identifier |
| TextKey | string | Localization key |
| Conditions | List<DialogueCondition> | Conditions to show this choice |
| NextDialogueId | string | Target dialogue after selection |
| SetFlag | string | Flag to set on selection |
| SetFlagValue | string | Flag value |
| RecordDecision | string | Decision ID to record |
| RecordChoice | string | Choice value to record |
| QuestHookId | string | Quest to affect |
| QuestHookAction | string | "accept", "advance", "complete", "fail" |
| Rewards | List<QuestReward> | Immediate rewards |
| MerchantHookId | string | Open merchant shop |
| ServiceHookId | string | Open service |
| CinematicHookId | string | Trigger cutscene |

## Condition Types

| Type | Parameter Format | Description |
|------|------------------|-------------|
| flag | flagName | Check global narrative flag |
| quest | questId | Check quest state (completed/active/available) |
| reputation | reputationKey | Check reputation threshold |
| faction | factionId | Check faction membership |
| skill | skillId | Check skill level |
| variable | varName | Check world variable equality |
| npc_variable | npcId:varName | Check NPC variable |
| decision | decisionId=choice | Check player decision |
| chapter | chapterId | Check unlocked story chapter |
| level | level | Check player level |
| time | timeTag | Check time of day |
| weather | weatherTag | Check weather |

All conditions support negation via `Negate: true`.

## Dialogue Flow

```
StartConversation
    → Check EntryConditions
    → Set OnStartFlag
    → Display StartingDialogue
    
    → If no choices and NextDialogueId: Auto-advance
    → If choices: Filter available by conditions
        → Player selects choice
        → Apply choice effects (flags, decisions, quests, rewards)
        → Navigate to NextDialogueId
    → If IsEndOfConversation: End
    
    → Set OnEndFlag
    → Clear dialogue variables
EndConversation
```

## Loop Prevention

- MaxDepth limit per conversation
- Visited dialogue HashSet tracks entered dialogues
- Loop detection: if dialogue already visited, conversation ends with warning
- Default max depth: 10

## Save Integration

Dialogue manager state persisted via SaveProfile V15:
- Active conversation ID
- Current dialogue position
- Depth tracking
- Visited dialogues set

## Performance

- O(1) dialogue lookups via Dictionary
- O(1) conversation lookups via Dictionary
- O(n) for NPC conversation queries (indexed)
- Supports thousands of dialogue entries with minimal memory
- Thread-safe operations