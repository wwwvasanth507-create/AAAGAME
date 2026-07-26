# HERO OF ETERNIA — ANTAGONIST FACTION: THE SHADOW LEGION OF MALAKOR

---

## 1. Overview & Military Hierarchy

The **Antagonist Faction Engine** (`AntagonistFactionManager.cs`) manages Malakor's organized military legion:

- **Supreme Warlord**: Arch-Sorcerer Malakor.
- **High Field Commander**: General Vaelis the Unforgiving.
- **Unit Officers**: Void Spellweavers & Legion Officers.
- **Frontline Vanguard**: Corrupted Iron Knights & Shadow Brutes.
- **Recon & Support**: Shadow Scouts & Legion Engineers.

---

## 2. Alert Escalation Tiers

| Alert Level | Trigger Conditions | Legion Behavior Changes |
| :--- | :--- | :--- |
| **Low** | Normal un-alerted state. | Standard patrol routes, 12m aggro radius. |
| **Elevated** | Player spotted in non-combat zone. | Patrol density increases, guard rotations double. |
| **HighAlert** | Alarm gong struck in any sector. | Reinforcement waves spawn, aggro radius increases to 20m. |
| **Lockdown** | General Vaelis arena assault. | All portcullis gates seal, archers deploy on battlements. |

---

## 3. AI Asset Production Report

### A. UI Faction Emblem
1. **Shadow Legion Emblem (`icon_faction_shadow_legion`)**
   - **Resolution**: 512x512 PNG transparent.
   - **AI Prompt**: `"Game UI faction emblem icon of a dark obsidian skull with glowing purple void eyes, sharp vector artwork"`
   - **Folder Location**: `res://Assets/UI/Icons/icon_faction_shadow_legion.png`
