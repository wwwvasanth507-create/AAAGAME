# Encounter System — Hero of Eternia

**Version:** 0.12.0  
**Phase:** Prompt 12 / 150 — Encounter Management  
**Status:** ✅ Production Ready

---

## Overview

The Encounter System governs combat states, arena boundaries, resets, victory and defeat logs for boss battles.

---

## Architecture

```
   EncounterManager (Service)
           │
           ├── Active State: Inactive, warmup, Active, Resetting, Victory, Defeat
           ├── ArenaInstance (boundary checks, hazards, lock gates)
           ├── EventBus (Publish: EncounterStarted, EncounterReset, EncounterVictory, EncounterDefeat)
           └── RewardTracker (Claims, anti-double-claim validation)
```

---

## State Flow

```
   Inactive ──► Active (StartEncounter)
                   │
                   ├──► Resetting (Player out of bounds / manual reset) ──► Inactive
                   ├──► Defeat (Player death event) ──► Resetting ──► Inactive
                   └──► Victory (Boss HP reaches 0) ──► Unlock Gates ──► Inactive
```

---

## Arena Boundaries Containment

**File:** `Scripts/Combat/Arena/ArenaFramework.cs`

- **Cylindrical Boundary Math**: Containment checks evaluate player position against center coordinates using a cylindrical radius and height query:
  ```csharp
  float distHorizontalSq = diff.X * diff.X + diff.Z * diff.Z;
  bool withinRadius = distHorizontalSq <= radius * radius;
  bool withinHeight = Y >= centerY && Y <= centerY + height;
  ```
- **Safe Zones**: Ignores hazard overlap logic while player is inside designated coordinate zones.
- **Hazards**: Delivers ticking damage (e.g. lava, poison zones) if player is in hazard zones.

---

## Event Messages Broadcasted

| Event | Payload | Description |
|-------|---------|-------------|
| `EncounterStartedEvent` | `(BossId, ArenaId)` | Triggers arena wall locking |
| `EncounterResetEvent` | — | Teleports player to entry and resets HP |
| `EncounterVictoryEvent`| `(BossId, ArenaId)` | Grants XP and marks boss as defeated |
| `EncounterDefeatEvent` | `(BossId)` | Freezes input and displays defeat UI |
| `ArenaHazardDamagedEvent`| `(HazardId, Damage)` | Delivers tick damage to player stats |
