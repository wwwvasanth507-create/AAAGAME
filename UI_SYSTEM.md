# UI System — Hero of Eternia

> Complete UI/UX framework for Hero of Eternia.
> Last Updated: 2026-07-25 (Phase 20)

---

## Architecture Overview

```
UIManager (IInitializable)
├── Screen Registry (20+ screens)
├── Navigation Stack (max depth 20)
├── Modal Dialog System
├── Layer Management (10 layers)
├── Focus Management
├── Transition Animations (Tween-based)
├── Input Routing
├── State Persistence (UIPreferences)
└── Plugin System (IUIPlugin)

HUDController (CanvasLayer)
├── Health Widget
├── Mana Widget
├── Stamina Widget
├── Experience Widget
├── Compass Widget
├── MiniMap Widget
├── Quest Tracker Widget
├── Ability Bar Widget
├── Interaction Prompt Widget
├── Buff/Debuff Widget
├── Status Effect Widget
├── Target Info Widget
├── Boss Health Widget
└── FPS Debug Widget (dev only)

NotificationManager (IInitializable)
├── Priority Queue (4 levels)
├── Visual Notifications
├── Convenience Methods (Quest, Level, Item, etc.)
├── Handler System
└── History Tracking

UIInputHandler (Node)
├── Touch Support
├── Mouse Support
├── Keyboard Support
├── Gesture Hooks
├── Input Rebinding
└── Haptic Feedback

ResponsiveLayout (Node)
├── Device Detection (Phone/Tablet/Desktop)
├── DPI Scaling
├── Safe Areas
├── Foldable Support
└── Layout Presets

AccessibilityManager (IInitializable)
├── Text Scaling
├── High Contrast Mode
├── Color Blind Filters
├── Subtitle System
├── Reduced Motion
├── Screen Reader Labels
└── Haptic Feedback
```

---

## Layer Order

| Layer | Z-Index | Purpose |
|-------|---------|---------|
| Background | 0 | Background elements |
| Game | 10 | 3D game world |
| HUD | 20 | In-game HUD |
| Screens | 30 | Full-screen menus |
| Popups | 40 | Temporary popups |
| Modals | 50 | Modal dialogs |
| Notifications | 60 | Toast notifications |
| Tooltips | 70 | Tooltip overlays |
| Debug | 80 | Debug overlays |
| Overlay | 90 | Topmost overlays |

---

## Screen Lifecycle

```
Registered (in UIManager)
    ↓
OnActivate(args) → Visible, Active
    ↓
OnDeactivate() → Hidden, Inactive
    ↓
OnBackPressed() → Close or custom behavior
    ↓
OnScreenResized(newSize) → Responsive adjustment
```

Lazy loading: Screens with `LazyLoad = true` only initialize their UI on first activation.

---

## Key Classes

### UIManager
- `Initialize()` - Creates layers, loads preferences
- `RegisterScreen(id, screen)` - Registers a screen
- `OpenScreen(id, args, animated)` - Navigates to screen
- `CloseScreen(animated)` - Returns to previous screen
- `CloseToRoot(animated)` - Returns to root screen
- `ShowModal(modal)` - Shows modal dialog
- `CloseModal(modal)` - Closes modal dialog
- `HandleBackButton()` - Handles back navigation
- `RegisterPlugin(plugin)` - Registers UI plugin
- `SavePreferences()` / `ApplyPreferences()` - UI state

### HUDController
- `ToggleWidget(name, visible)` - Toggle widget visibility
- `ApplyPreferences()` - Apply accessibility settings
- EventBus-driven updates for all widgets

### NotificationManager
- `QueueNotification(notification)` - Queue with priority
- `QuestUpdated(name)` - Quest notification
- `LevelUp(level)` - Level up notification
- `ItemAcquired(name, qty)` - Item notification
- `Update(delta)` - Process queue

### AccessibilityManager
- `SetTextScale(scale)` - 0.5 to 2.0
- `SetHighContrast(bool)` - Toggle high contrast
- `SetColorBlindMode(mode)` - Color blindness filter
- `ShowSubtitle(text, duration)` - Subtitle display
- `SetReducedMotion(bool)` - Reduced animations

---

## New Screens (Adding Without Modifying Code)

1. Create a class extending UIScreen
2. Override OnLazyLoad() to build UI
3. Add constant to ScreenRegistry
4. Call RegisterScreen in ScreenRegistry.RegisterAll

No modifications to UIManager required.

---

## UI Preferences (Saved)

- HUD enabled/disabled
- UI scale (0.5x - 2.0x)
- Text size (0.5x - 2.0x)
- High contrast mode
- Reduced motion
- Last opened screen
- Notification enabled
- Accessibility font
- Color blind mode
- Subtitle enabled
- Haptic feedback
- Show FPS

---

## Performance Targets

- UI redraw: < 5ms per frame
- Layout recalc: < 3ms
- List virtualization for 1000+ items
- Texture memory: < 32MB for UI
- Notification processing: < 1ms per queue
- Smooth 30fps+ on mid-range Android
- Object pooling for repeated elements