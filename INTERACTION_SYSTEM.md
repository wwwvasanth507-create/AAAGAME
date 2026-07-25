# Interaction & Detection System - Hero of Eternia

This document details the universal interaction interface, Area3D detection sweeps, target selection rules, and input trigger modes.

---

## 1. Interface Blueprint (`IInteractable`)

All interactive entities (Doors, Chests, Signs, Levers, Buttons, NPC, Animals) implement the `IInteractable` interface:
```csharp
public interface IInteractable
{
    string InteractionPrompt { get; }
    float InteractionDistance { get; }
    InteractionType Type { get; } // Tap, Hold, Auto
    float HoldDuration { get; }

    void OnInteract(PlayerRoot player);
    void OnInteractionStart(PlayerRoot player);
    void OnInteractionEnd(PlayerRoot player, bool completed);
    void SetHighlight(bool highlighted);
    Vector3 GetGlobalPosition();
}
```

---

## 2. Interaction Modes

The system supports three interaction trigger archetypes:

1. **Single Tap:** Triggers immediately on button press (e.g. Buttons, Levers, Doors).
2. **Hold:** Requires the player to hold the interact button for `HoldDuration` seconds. Displays a loading circle/progress bar (e.g. Chest opening, sleeping, gathering).
3. **Auto Interact:** Activates automatically as soon as the player enters the interaction range (e.g. stepping on a pressure plate or triggering scene entries).

---

## 3. Detection & Highlighting Heuristics

The `PlayerInteractionDetector` is attached as a child node to the player:
- **Scan Method:** Uses an `Area3D` with a programmatically generated `SphereShape3D` scanning Layer 4 (Interactables).
- **Target Selection:** Filters overlapping candidates. Sorts them by distance to identify the closest valid target.
- **Highlighting Hook:** Calls `SetHighlight(true)` on the closest target, applying a neon cyan unshaded shader outline override to make it visually distinct.
- **Headless Fallback:** Maintains a manual registration list so that unit tests can mock interaction scenarios headlessly without active physics spaces.
