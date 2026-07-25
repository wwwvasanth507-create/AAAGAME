# Accessibility — Hero of Eternia

> Accessibility framework documentation.
> Last Updated: 2026-07-25

---

## Features

### Adjustable Text Size
- Range: 0.5x to 2.0x
- Applied to all HUD widgets and screen text
- Persisted across sessions

### High Contrast Mode
- Toggle on/off
- High-contrast colors for all UI elements
- Yellow text on dark backgrounds
- Red/green/blue color coding for resource bars

### Color-Blind Friendly Hooks
- Protanopia (red-blind) filter hook
- Deuteranopia (green-blind) filter hook
- Tritanopia (blue-blind) filter hook
- Framework ready for shader-based color correction

### Subtitle Framework
- Toggle on/off
- Adjustable subtitle size (0.5x - 2.0x)
- Auto-hide with fade animation
- High contrast subtitle colors

### UI Scaling
- Dynamic DPI-based scaling
- Manual override in settings
- Range: 0.5x to 2.0x

### Reduced Motion Mode
- Shortens all transition animations
- Reduces tween durations by 70%
- Respects user preference for reduced motion

### Screen Reader Labels
- Accessible property set on controls
- Tooltip text as screen reader fallback
- Future Android AccessibilityEvent integration

### Haptic Feedback Toggle
- Light, Medium, Heavy haptic types
- Toggle on/off in settings
- Android vibration API integration

### Future Voice Navigation
- Framework hooks for voice commands
- Extensible command processing
- Ready for speech recognition integration

## Implementation

```csharp
// Get accessibility manager
var accessibility = ServiceLocator.Get<AccessibilityManager>();

// Adjust text size
accessibility.SetTextScale(1.5f);

// Enable high contrast
accessibility.SetHighContrast(true);

// Set color blind mode
accessibility.SetColorBlindMode(ColorBlindMode.Protanopia);

// Show subtitle
accessibility.ShowSubtitle("Hello, adventurer!", 3.0f);

// Enable reduced motion
accessibility.SetReducedMotion(true);

// Trigger haptic feedback
accessibility.TriggerHaptic(HapticType.Medium);
```

## IAccessibleElement Interface

UI elements can implement `IAccessibleElement` to receive accessibility updates:

```csharp
public class MyWidget : Control, IAccessibleElement
{
    public void OnAccessibilityChanged(AccessibilitySettings settings)
    {
        Scale = new Vector2(settings.TextScale, settings.TextScale);
        Modulate = settings.HighContrast ? Colors.Yellow : Colors.White;
    }
}