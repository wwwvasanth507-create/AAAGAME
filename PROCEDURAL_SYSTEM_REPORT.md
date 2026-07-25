# Procedural System Report — Hero of Eternia (v0.7.0)

This report validates the time system ticks, weather configurations, and resource spawning limits.

---

## 1. Time Ticks & Stages

The `WorldTimeSystem` coordinates in-game progression:
- **Ticks Update:** Advances `TimeOfDay` fractional value. A value of `1.0` increments `DayCount`.
- **Stages Division:** 
  - `Sunrise` [0.20 - 0.28]
  - `Day` [0.28 - 0.72]
  - `Sunset` [0.72 - 0.80]
  - `Night` [<0.20 or >=0.80]
- **Seasonal Shifts:** Every 30 days, the active season updates (Spring → Summer → Autumn → Winter).

---

## 2. Weather Climates Configurations

Weather profiles (Sandstorms, Ash Falls, Snow, Blizzards) load from `weather_profiles.json` to alter game environments dynamically:
- **Wind Strengths:** Wind strength scales from `0.0` (Clear) to `0.8` (Storm/Blizzard).
- **Lighting Tint Vectors:** Light colors hex values (e.g. gray tints for rain) adjust sky lighting.
- **Save Integrity:** Active time and weather settings parse cleanly, recovering environment states upon load.

---

## 3. Resource Spawner Rules

Resources placement rules (`ResourceSpawnRule`) check parameters before spawning:
- **Biome Matches:** Checks if current chunk biome is in the allowed biome list.
- **Slope Filters:** Coordinates exceeding 30 degrees angle are bypassed to prevent resources spawning floating on steep cliff edges.
- **Harvest History Integration:** Spawner checks the chunk's `ModifiedNodeIds` list. Mined items remain deleted.
