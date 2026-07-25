# Guard AI System — Hero of Eternia

## Overview

The Guard AI System manages law enforcement NPC behavior including patrol, investigation, arrest, combat, and reinforcement calling. Designed to scale to large cities with many guards.

## Architecture

```
SocialManager
  └─ GuardAISystem (ServiceKey: "GuardAISystem")
       ├─ GuardConfig (per-guard parameters)
       ├─ GuardRuntimeState (runtime state machine)
       ├─ GuardState enum (12 states)
       ├─ GuardAlertLevel enum (4 levels)
       └─ Settlement-level alert control
```

## Guard States

| State | Description |
|-------|-------------|
| Idle | Standing by, no active duty |
| Patrol | Following patrol route |
| Investigate | Checking reported activity |
| Question | Interrogating a suspect |
| Warn | Issuing a warning |
| Arrest | Attempting to arrest |
| Combat | Engaged in combat |
| CallReinforcements | Alerting nearby guards |
| ProtectCitizen | Defending a civilian |
| Escort | Escorting someone |
| Search | Searching for a suspect |
| ReturnToPatrol | Returning to normal duty |

## Alert Levels

| Level | Color | Behavior |
|-------|-------|----------|
| Green | Normal | Standard patrol |
| Yellow | Suspicious | Increased vigilance |
| Orange | Active | Investigation mode |
| Red | Threat | Combat readiness |

## Guard Configuration

Each guard has configurable parameters:
- PatrolRadius, InvestigationRadius, DetectionRadius, HearingRadius, CombatRadius
- SearchDuration, InvestigationDuration
- CombatStrength
- CanCallReinforcements, ReinforcementCallRadius
- PatrolRoute (waypoint list)

## Integration

- CrimeManager reports trigger guard investigation
- SettlementManager provides guard placement data
- NpcReactionSystem evaluates guard disposition
- Event-driven state changes for UI/audio feedback

## Performance

- Throttled to 0.25s update interval
- O(1) dictionary lookups for guard state
- Settlement-indexed guard lists
- Stress tested with 100+ guards