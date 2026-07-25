# AI_FRAMEWORK.md
# Hero of Eternia — AI Framework Documentation

**Version:** 1.0.0
**Phase:** Prompt 9 / 150
**Status:** Production Ready

---

## Overview

The Hero of Eternia AI Framework governs all non-combat NPC behaviour. It is built around three decoupled components:

1. **NpcStateMachine** — FSM controlling which behaviour state an NPC is in.
2. **NpcScheduler** — Time-driven schedule evaluating which state the NPC *should* be in.
3. **NpcNavigationAgent** — Cell-grid movement engine connecting FSM states to world positions.

---

## AI State Machine (NpcStateMachine)

### States

| State | Description |
|-------|-------------|
| Idle | Default resting state |
| Walking | Moving between locations |
| Working | Performing job action |
| Eating | Consuming food |
| Sleeping | Night rest |
| Talking | Engaged in dialogue |
| Inspecting | Examining object/area |
| Patrolling | Guard route loop |
| Waiting | Stationary pause |
| Celebrating | Festival/event activity |
| Fleeing | ⚠️ Framework stub — no combat logic |
| Searching | ⚠️ Framework stub — no combat logic |

### Transition Registration

```csharp
fsm.RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Walking);
fsm.RegisterTransition(NpcStateEnum.Idle, NpcStateEnum.Fleeing, "threat_detected");
fsm.RegisterDefaultTransitions(); // Registers all civilian transitions
```

### Transition Logic

- Registered transitions are checked on each `TransitionTo()` call.
- Optional `conditionTag` must match if specified on the rule.
- Unknown/unregistered transitions silently fail — no crash.
- `TimeInCurrentState` tracks how long the current state has been active.

### Update Cycle

```csharp
// NpcManager calls every tick (throttled to 0.5 s)
fsm.Update(delta);
```

---

## Daily Schedule System (NpcScheduler)

### Schedule Blocks

Each block defines a time window, target state, location, and override priority:

```csharp
new ScheduleBlock
{
    Period      = SchedulePeriod.Morning,
    TimeStart   = 0.20,   // fraction of day (0.0 = midnight, 0.5 = noon)
    TimeEnd     = 0.45,
    TargetState = NpcStateEnum.Working,
    LocationTag = "market",
    Priority    = 1
}
```

### Override Stack

| Override Type | Priority | Example |
|---------------|----------|---------|
| None (normal) | 1 | Daily routine |
| Weather | 5 | Storm — stay indoors |
| Festival | 10 | Celebration — go to square |
| Emergency | 20 | Attack — seek shelter |

Higher priority always wins on time-range conflict.

### Resolution

```csharp
scheduler.SetOverride(ScheduleOverrideType.Festival);
var block = scheduler.GetActiveBlock(worldTimeFraction);
// Returns highest-priority ScheduleBlock covering current time
```

---

## Navigation Agent (NpcNavigationAgent)

### Design

- Uses `NavigationFoundation.IsWalkable(x, z)` to validate each step cell.
- Fully headless-safe — no live Godot `NavigationAgent3D` dependency.
- Step size defaults to 1.0 m per tick.
- Arrival threshold defaults to 1.5 m.

### API

```csharp
var nav = new NpcNavigationAgent("npc_001", navigation, startX, startY, startZ);
nav.SetDestination(targetX, targetY, targetZ); // Returns false if blocked
nav.AdvanceStep();                             // Move one step, returns false if blocked
bool arrived = nav.HasReached;
(float x, float y, float z) pos = nav.GetPosition();
```

### Save/Restore

```csharp
float[] snapshot = nav.GetPositionSnapshot();     // [x, y, z]
nav.RestorePosition(snapshot);
```

---

## Update Flow (NpcManager.UpdateAll)

```
Every frame delta → accumulate _tickAccumulator
When _tickAccumulator >= 0.5 s:
  For each registered NPC:
    1. Apply schedule override (Weather / Festival / Emergency)
    2. Evaluate scheduler.GetActiveBlock(worldTimeFraction)
    3. If block.TargetState != fsm.CurrentState → TransitionTo()
    4. fsm.Update(TickInterval)
    5. If Walking → navAgent.AdvanceStep()
  Reset _tickAccumulator
```

---

## Future AI Expansion

| Feature | Hook |
|---------|------|
| Combat AI | `Fleeing` / `Searching` states + combat profile key |
| Mount Riding | New FSM state + NavAgent vehicle mode |
| Trading Behaviour | `Merchant` type + inventory reference |
| Crafting Behaviour | `Blacksmith` type + crafting state |
| Crowd Simulation | Spatial grid + LOD tier switching |
| Group Formations | NavAgent group leader/follower pattern |
| Emotion Reactions | `EmotionState` on NpcData drives animation blend |

---

## Performance Targets (Android)

| Load | Target |
|------|--------|
| 100 NPCs | < 0.5 ms / tick |
| 300 NPCs | < 1.0 ms / tick |
| 500 NPCs | < 2.0 ms / tick |

Achieved by: 0.5 s throttle, O(1) dictionary lookups, single cell walkability check per nav step.
