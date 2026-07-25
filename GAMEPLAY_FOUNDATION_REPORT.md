# Gameplay Foundation Report — Hero of Eternia

**Version:** 0.12.0  
**Audit Date:** 2026-07-25  

---

## 1. Subsystems Integrated

### 1.1 Player Controller & FSM
- [PlayerRoot.cs](file:///c:/AAA/Scripts/Player/PlayerRoot.cs) governs movement loops, collision capsule resizing (crouch), and input redirects.
- Features a 24-state finite state machine including combat states (Attack, Block, Parry, Stun, Knockdown, Swim, Climb).

### 1.2 Interactive World System
- Cylinder sweep Areas detect interactive landmarks, pickable chests, or drop items.
- Supports Auto-interact and Hold-to-Interact thresholds.

### 1.3 Combat Manager
- Directs melee/ranged swings, projects projectiles in motion buffers, applies status tick checks (Burn, Freeze, Poison, Bleed), and handles resistances.
- Lock-on queries target cycling within FOV constraints.

### 1.4 Ability Executor
- Models 4 active skill slots with resource checks (stamina, mana) and cooldown counts.

---

## 2. Event-Driven Messaging Architecture

The [EventBus](file:///c:/AAA/Scripts/Core/EventBus.cs) decouples UI overlays and progression tracking from player mechanics:

| Event Class | Publisher | Subscriber | Purpose |
|-------------|-----------|------------|---------|
| `HudHealthChangedEvent` | PlayerData / CombatManager | HUD Controller | Repaints current/max HP bar |
| `HudStaminaChangedEvent`| PlayerData | HUD Controller | Repaints stamina bar |
| `HudWaveChangedEvent` | EnemySpawner | HUD Controller | Refreshes current wave index |
| `EnemyDiedEvent` | EnemyController | GameLoop / Spawner | Grants XP and decrements spawns |
| `EncounterStartedEvent` | EncounterManager | Player / Camera | Locks gates and updates boundaries |
| `RewardClaimedEvent` | RewardClaimTracker | SaveManager | Persists rewards and saves slot |

---

## 3. Scalability Assessment
The combat, abilities, and modifiers are decoupled from specific character classes.
Designers can create new weapon types, status effects, or boss encounter structures entirely within the JSON configurations, maintaining SOLID scalability.
