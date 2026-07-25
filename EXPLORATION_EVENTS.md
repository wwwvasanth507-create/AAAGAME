# EXPLORATION EVENTS SPECIFICATION — HERO OF ETERNIA (PROMPT 25)

## System Overview
`ExplorationEventManager.cs` schedules dynamic world events including falling meteors, traveling merchants, rare creature spawns, resource surges, and magic storms.

## Dynamic Event Lifecycle
1. **Spawn**: `TriggerEvent(type, position, duration)` creates an active event and dispatches `OnEventSpawned`.
2. **Update**: Decrements active event timers based on delta time.
3. **Expiration**: Dispatches `OnEventExpired` when duration completes and purges event node.
