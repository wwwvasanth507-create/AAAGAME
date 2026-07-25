# Narrative System — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 19)

## Architecture

The NarrativeManager serves as the central narrative state tracker, managing all storytelling-related state across the game. It works in conjunction with the Quest System and Dialogue System to provide a complete narrative framework.

### Core Features

- **Global Flags**: World-wide narrative progression flags (e.g., "has_met_king", "world_saved")
- **Regional Flags**: Region-specific narrative state
- **World Variables**: Time of day, season, world events state
- **NPC Variables**: Per-NPC state tracking (mood, trust, dialogue flags)
- **Dialogue Variables**: Per-conversation scoped variables
- **Player Decisions**: Record of all player choices for branching narratives
- **Story Chapters**: Unlockable story progression markers
- **Condition Evaluation**: Flexible condition string parser for dialogue/quest conditions

### Condition Format

Conditions use a string-based format:
- `flag:flagName` — Check global flag exists
- `flag:flagName=value` — Check flag equals value
- `!flag:flagName` — Negation
- `region:regionId:flagName` — Check regional flag
- `var:varName=value` — Check world variable equality
- `var:varName>value` — Numeric comparison (>, <, >=, <=, !=, =)
- `npc:npcId:varName=value` — Check NPC variable
- `quest:questId=completed` — Check quest state
- `decision:decisionId=choice` — Check player decision
- `chapter:chapterId` — Check unlocked story chapter