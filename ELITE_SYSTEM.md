# Elite Enemy System — Hero of Eternia

**Version:** 0.12.0  
**Phase:** Prompt 12 / 150 — Elite Modifiers  
**Status:** ✅ Production Ready

---

## Overview

The Elite Enemy System dynamically generates combat modifiers and stat scaling rules to apply to standard enemy base profiles.

---

## Modifiers Registry

`EliteModifierType` is a flags enum supporting multiple combinable modifiers:

| Modifier | Value | Description | Stat Scales | Name Affix | Color Overlay |
|----------|-------|-------------|-------------|------------|---------------|
| `Fortified`| `1 << 0` | Boosts defense and armor | HP ×2.0, Loot ×1.2, XP ×1.3 | "Fortified" Prefix | Grey (`#999999`) |
| `Swift` | `1 << 1` | Increases combat speed | Speed ×1.35, Damage ×1.1, Loot ×1.15, XP ×1.25 | "Swift" Prefix | Yellow (`#FFFF99`) |
| `Fireborn` | `1 << 2` | Attaches fire damage | Damage ×1.25, Fire resist 75%, Cold weak 75%, Loot ×1.3, XP ×1.4 | "Fireborn" Prefix | Red (`#FF3333`) |
| `Frostshield`| `1 << 3`| Adds frost barrier | HP ×1.3, Cold resist 75%, Fire weak 75%, Loot ×1.3, XP ×1.4 | "Glacial" Prefix | Blue (`#33CCFF`) |
| `Vampiric` | `1 << 4` | Lifesteal on attacks | Damage ×1.15, Loot ×1.4, XP ×1.5 | "the Leech" Suffix | Crimson (`#CC0000`) |
| `Summoner` | `1 << 5` | Spawns helper minions | HP ×1.5, Loot ×1.5, XP ×1.6 | "the Broodmother" Suffix | Purple (`#6600CC`) |

---

## Application Flow

Standard enemy profile instantiation is passed through:

```csharp
EnemyData eliteData = EliteSystem.ApplyEliteModifiers(baseEnemy, EliteModifierType.Fortified | EliteModifierType.Swift);
```

This returns a scaled, renamed `EnemyData` block matching the visual overlay and stats configuration.

---

## Performance Considerations

* **Stat Cache**: Modifiers are calculated once on enemy spawn and stored on the controller. No per-frame evaluations are done.
* **Colors**: Shader overlays use dynamic material colors based on `VisualColorOverlay` hex code, keeping dynamic material duplication low.
