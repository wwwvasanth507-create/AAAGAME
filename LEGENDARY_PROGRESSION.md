# HERO OF ETERNIA — LEGENDARY PROGRESSION FRAMEWORK

---

## 1. Scope & Tier 5 Legendary Crafting

The **Legendary Progression Manager** (`LegendaryProgressionManager.cs`) manages endgame item upgrades, Tier 5 recipes, and legendary materials:

- **Legendary Materials**:
  - `material_astral_essence`: Dropped by world mini-bosses in The Crystal Wasteland.
  - `material_sun_core_fragment`: Harvested from the Forgotten Sun Spire altar.
- **Recipes Unlocked in Chapter 11**:
  - `recipe_legendary_sol_blade`: Astral Sunblade of Sol (Tier 5 Weapon).
  - `recipe_legendary_celestial_crown`: Diadem of Astral Light (Tier 5 Helmet).

---

## 2. AI Asset Production Report

### A. Icon Assets
1. **Astral Sunblade Icon (`icon_weapon_sol_blade`)**
   - **Resolution**: 512x512 PNG transparent.
   - **AI Prompt**: `"Game UI inventory icon of a glowing golden sunblade with purple crystal inlay, transparent PNG background"`
   - **Folder Location**: `res://Assets/UI/Icons/icon_weapon_sol_blade.png`

---

## 3. Code Reference

Managed by `LegendaryProgressionManager.cs` and verified by `Chapter11SystemTests`.
