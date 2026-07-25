# WORLD EVOLUTION SPECIFICATION — HERO OF ETERNIA (PROMPT 30)

## Act I World Consequence Events (`ActIWorldEvolution.cs`)

World evolution events fire automatically when `Chapter3Manager.OnActICompleted()` is called.

| Event ID | Description |
|---|---|
| `evt_citadel_sealed` | Citadel permanently sealed. Eastern Ridgeline travel route opens. |
| `evt_oakvale_celebration` | Oakvale celebration triggers. Elder Alden offers Tier 2 skill tutelage. Merchant stock upgrades. |
| `evt_sylvan_alliance_formal` | Sylvan Guardians & Valen Crown formally allied. Captain Valerius stationed in Elderwood Grove. |
| `evt_shadow_cult_retreat` | Shadow Cult retreats to Mirkwood Swamps. Sylvanwood enemy population reduced. |
| `evt_tier2_merchants` | Tier 2 merchant inventories unlocked in Oakvale and Elderwood Grove. |

---

## Faction State after Act I

| Faction | Relation | Territory |
|---|---|---|
| `faction_valen_crown` | Allied | 70% |
| `faction_sylvan_guardians` | Allied | 60% |
| `faction_shadow_cult` | **At War** | 25% (retreating) |
| `faction_merchants_guild` | Neutral | 40% |
