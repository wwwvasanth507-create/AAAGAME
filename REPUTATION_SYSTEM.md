# REPUTATION_SYSTEM.md
# Hero of Eternia — Reputation System Documentation

**Version:** 1.0.0
**Phase:** Prompt 9 / 150
**Status:** Production Ready

---

## Overview

The Reputation System tracks the player's standing across four independent scopes. All changes are event-driven and fire through the `OnReputationChanged` event for decoupled system consumption (e.g., dialogue system, NPC behaviour, UI indicators).

---

## Scopes

| Scope | Key | Range | Description |
|-------|-----|-------|-------------|
| Global | `"global"` | –1000 to +1000 | World-wide hero fame |
| Regional | `regionId` | –1000 to +1000 | Per-region standing |
| Faction | `factionId` | –1000 to +1000 | Per-faction standing |
| Individual | `npcId` | –1000 to +1000 | Per-NPC personal standing |

---

## API

```csharp
var rep = new ReputationSystem();

// Adjust
rep.AdjustGlobal(20, "saved_villager");
rep.AdjustRegional("forest_region", 30, "completed_quest");
rep.AdjustFaction("guild_faction", -10, "stolen_item");
rep.AdjustIndividual("npc_king_001", 50, "hero_recognition");

// Query
int global  = rep.GetGlobal();
int region  = rep.GetRegional("forest_region");
int faction = rep.GetFaction("guild_faction");
int npc     = rep.GetIndividual("npc_king_001");
```

---

## Event-Driven Changes

```csharp
rep.OnReputationChanged += (evt) =>
{
    // evt.Scope    = ReputationScope.Regional
    // evt.ScopeKey = "forest_region"
    // evt.OldValue = 0
    // evt.NewValue = 30
    // evt.EventTag = "completed_quest"
    DialogueSystem.RefreshLines(evt.ScopeKey);
    UIManager.UpdateReputationBar(evt.Scope, evt.NewValue);
};
```

---

## Reputation Event Weights (reputation_events_config.json)

| Event Tag | Global | Regional |
|-----------|--------|----------|
| saved_villager | +20 | +30 |
| stolen_item | –10 | –20 |
| completed_quest | +15 | +25 |
| attacked_npc | –25 | –40 |
| donated_gold | +5 | +10 |

Weights are fully configurable via `Settings/reputation_events_config.json`.

---

## Save V6 Integration

Reputation is persisted as a flat dictionary in `SaveProfile.ReputationSnapshot`:

```json
{
  "global":         100,
  "reg:forest":     50,
  "fac:guild":     -20,
  "ind:npc_king":  200
}
```

### Snapshot API

```csharp
// Export for save
Dictionary<string, int> snap = rep.ExportSnapshot();

// Restore from save
rep.RestoreSnapshot(snap);
```

---

## Reputation Tiers (Suggested Thresholds)

| Score | Label | NPC Reaction |
|-------|-------|--------------|
| +750 to +1000 | Legendary Hero | NPCs bow, festivals triggered |
| +400 to +749 | Celebrated | Friendly dialogue, discounts |
| +50 to +399 | Respected | Normal positive reactions |
| –50 to +49 | Neutral | No reaction change |
| –400 to –51 | Distrusted | NPCs suspicious, doors closed |
| –750 to –401 | Feared | NPCs flee, guards alerted |
| –1000 to –751 | Villain | Town lockdown, hostile world |

Tier thresholds are suggestions — they will be applied when dialogue trees and NPC reaction logic are implemented in future phases.

---

## Future Expansion

| Feature | Design |
|---------|--------|
| Crime System | Individual reputation < –200 triggers bounty |
| Hero Titles | Faction reputation > +800 unlocks title |
| Faction Wars | Raising one faction rep lowers enemy faction |
| NPC Memory | Individual reputation persists per-NPC |
| Dynamic Events | Reputation thresholds trigger world events |
