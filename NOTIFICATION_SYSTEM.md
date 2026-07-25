# Notification System — Hero of Eternia

> Central notification manager documentation.
> Last Updated: 2026-07-25

---

## Overview

The NotificationManager provides a priority-based queuing system for all in-game notifications. It supports quest updates, level ups, item acquisitions, achievements, crafting completions, system messages, warnings, errors, and future event hooks.

## Priority Levels

| Priority | Duration | Color | Use Case |
|----------|----------|-------|----------|
| Low | 2.0s | Dark gray | System messages |
| Normal | 3.0s | Blue | Quest updates, items |
| High | 4.0s | Orange | Level up, achievements |
| Critical | 6.0s | Red | Errors, warnings |

## Queue Management

- Max visible: 5 notifications at once
- Max queue: 50 pending notifications
- Auto-dismissal with fade-out animation
- Persistent notifications (duration = 0) stay until dismissed

## Convenience Methods

```csharp
notificationManager.QuestUpdated("The Awakening");
notificationManager.LevelUp(5);
notificationManager.ItemAcquired("Iron Sword", 2);
notificationManager.AchievementUnlocked("First Steps");
notificationManager.CraftComplete("Health Potion");
notificationManager.SystemMessage("Welcome to Eternia");
notificationManager.Warning("Low durability on weapon");
notificationManager.Error("Failed to save game");
```

## Handler System

Implement `INotificationHandler` to receive callbacks:
```csharp
public class MyHandler : INotificationHandler
{
    public void OnNotificationShown(UINotification notification) { }
    public void OnNotificationDismissed(UINotification notification) { }
}
```

## History

All notifications are recorded in history for review via the Notification History screen.