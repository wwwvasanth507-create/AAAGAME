# Responsive Layout — Hero of Eternia

> Responsive mobile layout system documentation.
> Last Updated: 2026-07-25

---

## Device Categories

| Category | Width (DP) | UI Scale | Columns | Sidebar | Bottom Nav |
|----------|------------|----------|---------|---------|------------|
| Phone | ≤480 | 1.4x | 2 | No | Yes |
| Small Tablet | ≤768 | 1.2x | 3 | No | Yes |
| Large Tablet | ≤1024 | 1.0x | 4 | Yes | No |
| Desktop | >1024 | 1.0x | 6 | Yes | No |

## Features

### DPI-Aware Scaling
- Base DPI: 160
- Automatic DPI detection via DisplayServer
- Clamped UI scale range: 0.5x - 2.0x
- Device category determined by DP (density-independent pixels)

### Safe Areas
- Top: Status bar height (portrait)
- Bottom: Navigation bar height (portrait)
- Left: Status bar height (landscape)
- Right: Navigation bar height (landscape)

### Orientation Handling
- Automatic detection on size change
- Layout preset reload on orientation change
- Event: `OnOrientationChanged(bool isLandscape)`

### Foldable Device Support
- Detection via Android project settings
- Fold state toggle via `SetFoldState(bool)`
- Layout recalculation on fold/unfold

## IResponsiveElement Interface

```csharp
public class MyResponsiveWidget : Control, IResponsiveElement
{
    public void OnLayoutChanged(ResponsiveInfo info)
    {
        // Adjust layout based on device info
        Scale = new Vector2(info.Scale, info.Scale);
        // Use info.GridColumns, info.Padding, etc.
    }
}
```

## Usage

```csharp
// Register element
var layout = GetNode<ResponsiveLayout>("/root/ResponsiveLayout");
layout.RegisterElement(myWidget);

// Get current info
bool isMobile = layout.IsMobile();
float scale = layout.CurrentScale;
DeviceCategory category = layout.CurrentCategory;
SafeAreaMargins safeArea = layout.SafeArea;