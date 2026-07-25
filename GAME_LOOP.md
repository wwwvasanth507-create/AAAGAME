# Game Loop — Hero of Eternia

**Version:** 0.11.0  
**Phase:** Prompt 11 / 150 — Gameplay Expansion  
**Status:** ✅ Production Ready

---

## Overview

`GameLoop` is the top-level Godot `Node` that manages the full runtime gameplay session.
It ties together the EnemySpawner wave system, XP/levelling, pause/resume, and autosave pipeline.

---

## Architecture

```
GameLoop (Node)
    │
    ├── Subscribes:  EnemyDiedEvent ──► AwardXp(amount)
    ├── Subscribes:  WaveCompleteEvent ──► WavesCompleted++ → Autosave
    ├── Subscribes:  AllWavesCompleteEvent ──► TriggerAutosave()
    ├── Subscribes:  PlayerDiedEvent ──► IsGameOver=true → GameOverEvent
    │
    ├── Publishes:  XpGainedEvent (amount, total, toNextLevel)
    ├── Publishes:  PlayerLeveledUpEvent (newLevel, xp)
    ├── Publishes:  GameOverEvent (wavesCompleted, kills, totalXp)
    └── Publishes:  GamePausedEvent (isPaused)
```

---

## Session Lifecycle

```
_Ready()
    └── Subscribe to events → session begins

_Process(delta)
    └── SessionTimeSec += delta (if not paused/game over)

Enemy dies → EnemyDiedEvent
    └── EnemiesKilled++
    └── AwardXp(e.XpReward)
        └── PlayerXp += amount
        └── While XP >= XpToNextLevel → Level Up! → PlayerLeveledUpEvent

Wave cleared → WaveCompleteEvent
    └── WavesCompleted++
    └── Autosave → slot 0

All waves → AllWavesCompleteEvent
    └── Autosave
    └── Victory (future: load VictoryScreen)

Player dies → PlayerDiedEvent
    └── IsGameOver = true
    └── Publish GameOverEvent
    └── Future: load GameOver scene
```

---

## XP & Levelling

### Formula

```
XpToNextLevel(level) = BaseXpToLevel × XpScaleFactor^(level − 1)
```

**Default values:**
- `BaseXpToLevel` = 100
- `XpScaleFactor` = 1.5

| Level | XP Required |
|-------|------------|
| 1 → 2 | 100 |
| 2 → 3 | 150 |
| 3 → 4 | 225 |
| 4 → 5 | 338 |
| 5 → 6 | 506 |

### Kill XP Values (Wave 1)

| Enemy | XP |
|-------|----|
| Goblin Grunt | 8 |
| Forest Wolf | 12 |
| Skeleton Warrior | 20 |
| Dark Mage | 35 |
| Stone Golem | 60 |

---

## Pause / Resume

```csharp
GameLoop.Pause()    // GetTree().Paused = true → fires GamePausedEvent(true)
GameLoop.Resume()   // GetTree().Paused = false → fires GamePausedEvent(false)
```

All Godot physics and `_Process` callbacks freeze when paused.
GameLoop._Process is PAUSABLE (default mode) — session timer stops.

---

## Autosave Integration

On each wave completion, `GameLoop` calls:

```csharp
SaveManager.UpdateSessionStats(PlayerLevel, PlayerXp, EnemiesKilled, WavesCompleted)
SaveManager.Save(slot: 0)
```

This writes all session progress to the SaveProfile V8 schema, including:
- `PlayerLevel`, `PlayerXp`
- `EnemiesKilledTotal`, `WavesCompleted`
- `Stats.Level`, `Stats.CurrentXp`, `StatsData.KillsCount`

---

## Exported Properties

| Property | Default | Description |
|----------|---------|-------------|
| `BaseXpToLevel` | `100` | XP needed for first level-up |
| `XpScaleFactor` | `1.5` | Multiplicative XP curve steepness |
| `AutosaveOnWave` | `true` | Autosave after each wave clears |

---

## State Query Helpers

| Method | Returns | Description |
|--------|---------|-------------|
| `PlayerLevel` | `int` | Current player level |
| `PlayerXp` | `int` | XP in current level |
| `EnemiesKilled` | `int` | Session kill count |
| `WavesCompleted` | `int` | Completed waves this session |
| `SessionTimeSec` | `float` | Seconds played this session |
| `IsPaused` | `bool` | Is game paused |
| `IsGameOver` | `bool` | Has player died |
| `GetSessionTimeFormatted()` | `string` | `HH:MM:SS` formatted |

---

## Events Published

| Event | Payload | When |
|-------|---------|------|
| `XpGainedEvent` | `(amount, total, toNext)` | After any XP award |
| `PlayerLeveledUpEvent` | `(newLevel, xp)` | On level crossing |
| `GameOverEvent` | `(waves, kills, xp)` | On player death |
| `GamePausedEvent` | `(isPaused)` | On pause/resume |

---

## Events Consumed

| Event | Source | Action |
|-------|--------|--------|
| `EnemyDiedEvent` | EnemyController | Kill count + XP award |
| `WaveCompleteEvent` | EnemySpawner | Wave count + autosave |
| `AllWavesCompleteEvent` | EnemySpawner | Victory autosave |
| `PlayerDiedEvent` | PlayerRoot (future) | Game over sequence |
