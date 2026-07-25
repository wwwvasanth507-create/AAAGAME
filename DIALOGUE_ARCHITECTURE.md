# DIALOGUE_ARCHITECTURE.md
# Hero of Eternia — Dialogue Architecture Documentation

**Version:** 1.0.0
**Phase:** Prompt 9 / 150
**Status:** Production Ready (Framework Only)

---

## Overview

The Dialogue Architecture provides a localization-key-based framework for NPC speech. All content is referenced by key strings — no story text exists in code. Keys resolve to strings at runtime via the LocalizationManager.

This phase establishes the dialogue routing infrastructure. Story content and branching choices will be added in future phases.

---

## Dialogue Line Structure

```csharp
public class DialogueLine
{
    public string LocalizationKey { get; set; }        // "npc.villager.greeting.morning"
    public DialogueCategory Category { get; set; }     // Greeting, Farewell, IdleComment...
    public string ConditionTag { get; set; }           // "time_morning", "weather_rain", "rel_friend"
    public float RelationshipThreshold { get; set; }   // Min aggregate score to show line
    public string VoiceClipKey { get; set; }           // Future voice playback bundle key
    public string LocaleOverride { get; set; }         // e.g. "fr" — empty = default locale
}
```

---

## Dialogue Categories

| Category | Description |
|----------|-------------|
| Greeting | Opening line when player initiates conversation |
| Farewell | Closing line when conversation ends |
| IdleComment | Ambient NPC comment (no interaction required) |
| WeatherComment | Reaction to current weather condition |
| TimeOfDayComment | Reaction to time of day |
| RelationshipVariant | Line variant driven by relationship level |

---

## Condition Tags

### Time of Day
| Tag | Time Fraction |
|-----|--------------|
| `time_night` | 0.00–0.20 and 0.80–1.00 |
| `time_morning` | 0.20–0.45 |
| `time_afternoon` | 0.45–0.65 |
| `time_evening` | 0.65–0.80 |

### Weather
| Tag | Condition |
|-----|-----------|
| `weather_rain` | Raining |
| `weather_storm` | Stormy |
| `weather_sunny` | Clear sky |
| `weather_snow` | Snowing |
| `weather_fog` | Foggy |

### Relationship
| Tag | Score Range |
|-----|------------|
| `rel_rival` | ≤ –60 |
| `rel_neutral` | –40 to +40 |
| `rel_friend` | ≥ +40 |
| `rel_bestfriend` | ≥ +75 |

---

## Resolution Algorithm

```
Input: npcId, category, relationshipScore, timeOfDay, weatherTag, locale

For each registered DialogueLine for npcId:
  1. Skip if category != requested category
  2. Skip if relationshipScore < line.RelationshipThreshold
  3. Skip if localeOverride != "" and localeOverride != locale

  Score = 0
  if conditionTag == timeTag    → Score += 2
  if conditionTag == weatherTag → Score += 2
  if conditionTag matches rel_* → Score += 3

Return highest-scoring line (or null if none match)
```

---

## Default Line Set Per NPC Type

Calling `DialogueFramework.BuildDefaultLines(NpcTypeEnum.Villager)` generates:

| Category | Condition | Key |
|----------|-----------|-----|
| Greeting | (default) | `npc.villager.greeting.default` |
| Greeting | time_morning | `npc.villager.greeting.morning` |
| Greeting | rel_friend | `npc.villager.greeting.friend` |
| Farewell | (default) | `npc.villager.farewell.default` |
| IdleComment | (default) | `npc.villager.idle.default` |
| WeatherComment | weather_rain | `npc.villager.weather.rain` |
| WeatherComment | weather_sunny | `npc.villager.weather.sunny` |
| TimeOfDayComment | time_night | `npc.villager.time.night` |
| RelationshipVariant | rel_rival | `npc.villager.rel.rival` |

Pattern: `npc.{type}.{category}.{condition}` (all lowercase).

---

## Registration API

```csharp
var framework = new DialogueFramework();

// Register a single line
framework.RegisterLine("npc_001", new DialogueLine
{
    LocalizationKey = "npc.guard.greeting.morning",
    Category        = DialogueCategory.Greeting,
    ConditionTag    = "time_morning"
});

// Register default set
framework.RegisterLines("npc_001", DialogueFramework.BuildDefaultLines(NpcTypeEnum.Guard));

// Resolve
var line = framework.Resolve("npc_001", DialogueCategory.Greeting,
    relationshipScore: 60f, timeOfDay: 0.30, weatherTag: "weather_sunny");

// Get localized text (future)
string text = LocalizationManager.Get(line.LocalizationKey);
```

---

## Localization Key Naming Convention

```
npc.{npctype}.{category}.{variant}

Examples:
  npc.villager.greeting.default
  npc.guard.farewell.default
  npc.wizard.weather.rain
  npc.king.greeting.friend
  npc.bandit.rel.rival
```

All keys are pre-registered strings. The LocalizationManager maps them to locale-specific strings at runtime.

---

## Future Expansion Hooks

| Feature | Design |
|---------|--------|
| Branching Choices | `DialogueLine.ChildKeys` list per line |
| Quest Dialogue | DialogueLine condition `"quest_active:{questId}"` |
| Voice Playback | `VoiceClipKey` → AudioManager bundle lookup |
| Emotion-Driven Lines | `EmotionState` condition tag on DialogueLine |
| Localization Pipeline | All keys map to `Locale/{lang}/npc.json` bundles |
| Story Content | Future phases add story-content keys and branch trees |
