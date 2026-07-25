# HUD System — Hero of Eternia

> Modular HUD system documentation.
> Last Updated: 2026-07-25

---

## Architecture

The HUD system is a `CanvasLayer`-based modular framework with independently controlled widgets.

```
HUDController (CanvasLayer, Layer 20)
├── HealthWidget (top-left)
├── ManaWidget (top-left)
├── StaminaWidget (top-left)
├── ExperienceWidget (top-left)
├── CompassWidget (top-center)
├── MiniMapWidget (top-right)
├── QuestTrackerWidget (top-right)
├── AbilityBarWidget (bottom-center)
├── InteractionPromptWidget (center)
├── BuffDebuffWidget (top-right below minimap)
├── StatusEffectWidget (below health)
├── TargetInfoWidget (right side)
├── BossHealthWidget (hidden, shown on boss)
└── FPSDebugWidget (dev only, hidden by default)
```

## Widget Interfaces

Each widget implements standard interfaces for consistency:

- `IHealthBar`, `IManaBar`, `IStaminaBar`, `IExperienceBar` - Resource bars
- `IInteractionPrompt` - Interaction text
- `IBossHealthBar` - Boss fight health display
- `IQuestTracker` - Active quest tracking
- `IAccessibleWidget` - Text scale & high contrast

## EventBus Integration

All HUD updates are driven through EventBus events:
- `HudHealthChangedEvent`
- `HudManaChangedEvent`
- `HudStaminaChangedEvent`
- `HudExperienceChangedEvent`
- `HudLevelUpEvent`
- `HudInteractPromptEvent`
- `HudBossSpawnedEvent`
- `HudBossHpChangedEvent`
- `HudQuestUpdatedEvent`
- `HudItemAcquiredEvent`

## Widget Visibility

Each widget can be independently toggled:
```csharp
hudController.ToggleWidget("health", false); // Hide health bar
hudController.ToggleWidget("fps", true);     // Show FPS debug
```

## Accessibility

All widgets implement `IAccessibleWidget` for:
- Dynamic text scaling (0.5x - 2.0x)
- High contrast color mode
- Color-blind friendly defaults