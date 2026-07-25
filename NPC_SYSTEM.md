# NPC_SYSTEM.md
# Hero of Eternia — NPC System Documentation

**Version:** 1.0.0
**Phase:** Prompt 9 / 150
**Status:** Production Ready

---

## Overview

The NPC System provides a fully data-driven framework for creating, managing, simulating, and persisting non-player characters across all regions of Eternia. The system supports hundreds of simultaneous NPCs on Android via throttled AI updates (0.5s tick interval) and deterministic seed-based spawning.

---

## Architecture

```
Scripts/NPC/
├── NpcDefinition.cs          ← Data model: NpcData, NpcTypeEnum, GenderType, EmotionState, NpcSaveState
├── NpcStateMachine.cs        ← Modular FSM: NpcStateEnum, transitions, update tick
├── NpcScheduler.cs           ← Daily schedule: ScheduleBlock, SchedulePeriod, override stack
├── RelationshipSystem.cs     ← Relational tracking: Friendship, Trust, Respect, Fear per NPC pair
├── ReputationSystem.cs       ← Scoped reputation: Global, Regional, Faction, Individual
├── DialogueFramework.cs      ← Localization-key resolver for dialogue lines
├── NpcSpawner.cs             ← Deterministic spawn placement from world seed
├── NpcNavigationAgent.cs     ← Cell-validated movement using NavigationFoundation grid
└── NpcManager.cs             ← Central service: register, update-all, save/load
```

---

## NPC Data Model (NpcDefinition.cs)

Every NPC is described by an `NpcData` record:

| Field | Type | Description |
|-------|------|-------------|
| UniqueId | string | Globally unique identifier |
| DisplayName | string | UI-visible name |
| Age | int | Cosmetic age value |
| Gender | GenderType | Male / Female / NonBinary |
| Species | string | Human, Elf, Dwarf, etc. |
| Occupation | NpcTypeEnum | Villager, Guard, Farmer... |
| FactionId | string | Owning faction |
| HomeLocationId | string | Landmark or building id |
| CurrentRegionId | string | Active region key |
| VoiceProfileKey | string | Audio bundle key |
| AnimationProfileKey | string | Animator key |
| AppearanceProfileKey | string | Visual customisation key |
| DialogueReferenceKey | string | Dialogue asset key |
| InventoryReferenceId | string | Future inventory link |
| CurrentEmotion | EmotionState | Neutral/Happy/Sad/Angry... |
| CurrentHealth | float | 0–MaxHealth |
| MaxHealth | float | Health cap |
| RelationshipKeys | List\<string\> | Pair keys this NPC tracks |
| CombatProfileKey | string | Future — no combat yet |
| QuestHookIds | List\<string\> | Future quest hooks |

---

## NPC Types (NpcTypeEnum)

15 built-in types, extensible:

| Type | Role |
|------|------|
| Villager | Civilian population |
| Guard | Town patrol, gate security |
| Farmer | Agricultural worker |
| Merchant | Shopkeeper (trading disabled) |
| Blacksmith | Crafting NPC (crafting disabled) |
| Wizard | Spellcaster placeholder |
| Hunter | Outdoor wanderer |
| Scholar | Library / research NPC |
| Priest | Temple attendant |
| King / Queen | Throne room royalty (1 per landmark) |
| Child | Reduced-schedule civilian |
| Traveler | Road wanderer, random spawn |
| Bandit | Non-combat framework only |
| Companion | Future party member hook |

---

## NPC Manager Service (NpcManager)

Registered in `ServiceLocator` as **"NpcManager"**.

```csharp
var manager = ServiceLocator.Get<NpcManager>();
manager.RegisterNpc(npcData);              // Add NPC
manager.UpdateAll(delta, timeFraction);   // Tick all FSMs (throttled 0.5 s)
manager.GetFsm("npc_001");               // Access FSM
manager.GetNavAgent("npc_001");          // Access nav agent
var states = manager.ExportStates();     // Save V6 snapshot
manager.RestoreStates(savedStates);      // Restore from save
```

---

## Performance Profile

| Metric | Target | Design |
|--------|--------|--------|
| Update interval | 0.5 s | Tick accumulator in NpcManager |
| Active NPC cap | 500+ per region | Dictionary lookup O(1) |
| NavAgent step cost | O(1) per NPC | Single IsWalkable cell check |
| Spawn generation | < 1 ms / region | Bitwise seed hash operations |
| Save export | < 2 ms | Flat dictionary snapshot |

---

## Config Assets

| File | Purpose |
|------|---------|
| `Settings/npc_types_config.json` | Type → AnimProfile / VoiceProfile / DefaultSchedule |
| `Settings/npc_schedules_config.json` | Schedule presets: civilian, patrol, farmer... |
| `Settings/reputation_events_config.json` | Event tags → global/regional delta values |

---

## Save Integration

Save Profile V6 adds three new fields:

```json
{
  "NpcStates": { "npc_001": { "WorldX": 10.0, "WorldY": 0.0, "WorldZ": 20.0, ... } },
  "ReputationSnapshot": { "global": 100, "reg:forest": 50 },
  "RelationshipSnapshot": { "npc_001_npc_002": [50.0, 30.0, 20.0, 5.0] }
}
```

V1–V5 saves migrate cleanly to V6 (empty collections initialised).

---

## Future Expansion Hooks

- **Companion AI** — `NpcTypeEnum.Companion` pre-defined
- **Combat State** — `Fleeing` and `Searching` states are stubs in FSM
- **Mount Riding** — New FSM state slot reserved
- **Trading** — `InventoryReferenceId` on NpcData
- **Crafting** — `DialogueReferenceKey` routes to Blacksmith/Wizard
- **Quests** — `QuestHookIds` list on NpcData
- **Voice Playback** — `VoiceClipKey` on DialogueLine
- **Branching Choices** — DialogueFramework resolver is extensible
