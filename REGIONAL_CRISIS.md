# HERO OF ETERNIA — REGIONAL CRISIS SYSTEM DOCUMENTATION

---

## 1. Scope & Mechanics

The **Regional Crisis Engine** (`RegionalCrisisManager.cs`) manages dynamic regional crisis events, travel alerts, location breaches, and dynamic NPC defense behaviors:

- **Alert Tiers**: `Normal`, `ElevatedAlert`, `ActiveSiege`, `RegionalCataclysm`.
- **Travel Restrictions**: Locks fast travel to breached sectors during active siege phases.
- **Dynamic NPC Behaviors**: Civilian NPCs retreat into city keeps while guard NPCs switch to combat patrol paths.

---

## 2. AI Asset Production Report

### A. UI Crisis Alert Banner
1. **Regional Crisis Alert UI Banner (`ui_crisis_alert_banner`)**
   - **Resolution**: 1024x256 PNG transparent.
   - **AI Prompt**: `"Game UI warning banner with glowing dark purple void energy, golden runic borders, and bold text field, transparent background"`
   - **Folder Location**: `res://Assets/UI/Banners/ui_crisis_alert_banner.png`

---

## 3. Code Reference

Managed by `RegionalCrisisManager.cs` (`IInitializable` registered with `ServiceLocator`).
