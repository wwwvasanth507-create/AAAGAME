# Faction System — Hero of Eternia

## Overview

The Faction System provides a data-driven, extensible framework for managing factions in the game world. Players interact with factions through reputation, diplomacy, and crime systems.

## Architecture

```
SocialManager
  └─ FactionDatabase (ServiceKey: "FactionDatabase")
       ├─ FactionDefinition (data model)
       ├─ JSON loading from Settings/faction_database.json
       └─ Runtime registration for mods/DLC
```

## FactionDefinition Fields

| Field | Type | Description |
|-------|------|-------------|
| FactionId | string | Unique identifier |
| DisplayName | string | Human-readable name |
| Description | string | Lore/flavor text |
| Type | FactionType | Enum: Kingdom, Empire, VillageCouncil, etc. |
| Headquarters | string | Location ID |
| Territory | string | Region IDs |
| LeadershipHook | string | Leader name/description |
| Alignment | FactionAlignment | Moral alignment axis |
| PrimaryGoals | List<string> | Strategic objectives |
| FriendlyFactions | List<string> | Allied faction IDs |
| HostileFactions | List<string> | Enemy faction IDs |
| NeutralFactions | List<string> | Neutral faction IDs |
| UniformProfile | string | Visual appearance key |
| Symbol | string | Heraldry/icon key |
| ColorTheme | FactionColorTheme | UI colors |
| MusicHook | string | Music theme key |
| LocalizationKey | string | For UI text |
| DlcFields | Dictionary | Future extension |
| CurrentStrength | int | Runtime: 0-MaxStrength |
| MaxStrength | int | Maximum strength |
| MemberCount | int | Number of members |
| Treasury | float | Gold/reserves |
| IsActive | bool | Whether faction exists |

## FactionType Enum

Kingdom, Empire, VillageCouncil, MerchantGuild, AdventurersGuild, MagesGuild, ReligiousOrder, Military, Bandits, Mercenaries, Pirates, Scholars, Nomads, MonsterTribe, SecretSociety, PlayerCreated

## FactionAlignment Enum

LawfulGood, NeutralGood, ChaoticGood, LawfulNeutral, TrueNeutral, ChaoticNeutral, LawfulEvil, NeutralEvil, ChaoticEvil

## Default Factions (9)

1. Kingdom of Eternia (Kingdom, LawfulGood)
2. Adventurers Guild (AdventurersGuild, NeutralGood)
3. Eternian Merchant Guild (MerchantGuild, LawfulNeutral)
4. Arcane Mages Guild (MagesGuild, TrueNeutral)
5. Blackfang Bandits (Bandits, ChaoticEvil)
6. Iron Company Mercenaries (Mercenaries, TrueNeutral)
7. Order of the Eternal Light (ReligiousOrder, LawfulGood)
8. Archive of Scholars (Scholars, TrueNeutral)
9. Eternian Royal Guard (Military, LawfulGood)

## Adding New Factions

No code changes required. Either:
- Add to Settings/faction_database.json
- Call FactionDatabase.RegisterFaction() at runtime

## Save Integration

Faction runtime states (strength, member count, treasury, active status) are exported through SocialManager.ExportSaveData() and restored through SocialManager.RestoreSaveData().

## Performance

- All lookups are O(1) dictionary access
- Thread-safe via lock
- Lightweight FactionReference for UI lists
- Supports 100+ factions without performance impact