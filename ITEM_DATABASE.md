# Item Database Specification - Hero of Eternia

This document details the architecture, configuration format, rarity properties, and DLC expansion strategies of the data-driven Item Database.

---

## 1. Item Database Architecture

The item database is fully data-driven. Items are defined in JSON configurations rather than hardcoded in C# code, allowing adjustments, balancing, and additions without rebuilding the binary.

### 1.1 ItemRecord Layout

Each item is loaded into an `ItemRecord` containing these fields:

| Property | Type | Description |
|---|---|---|
| `UniqueId` | `string` | Unique identifier (e.g. `wpn_iron_sword`). |
| `InternalName` | `string` | Development identifier. |
| `DisplayName` | `string` | User-facing localized string. |
| `Description` | `string` | Flavor text / tooltip. |
| `Category` | `string` | Main category string (e.g. `Weapon`, `Potion`). Extensible. |
| `Subcategory` | `string` | Detailed subcategory (e.g. `OneHandSword`, `Health`). |
| `Tier` | `int` | Level / Quality tier (1, 2, 3, etc.). |
| `Rarity` | `ItemRarity` | Common, Uncommon, Rare, Epic, Legendary, Mythic, Ancient, Divine. |
| `Weight` | `float` | Item weight per unit (used for inventory capacity limits). |
| `StackSize` | `int` | Maximum items permitted in a single slot. |
| `SellValue` | `int` | Gold earned from selling. |
| `BuyValue` | `int` | Gold cost to purchase. |
| `IconPath` | `string` | Icon resource path. |
| `ModelPath` | `string` | 3D mesh resource path. |
| `MaterialPath` | `string` | Custom Material resource path. |
| `AnimRef` | `string` | Key to Animation libraries. |
| `SoundRef` | `string` | Key to Audio clips libraries. |
| `LocKey` | `string` | Key to Localization Manager dictionaries. |
| `Version` | `int` | Database record schema version number. |
| `StatModifiers` | `List` | Modifiers applied to player stats when equipped. |
| `ExtensionData` | `Dictionary` | JSON catch-all extension data supporting future updates/DLCs. |

---

## 2. Rarity System Definitions

Item rarity defines border styling, color values, spawn probabilities, and cosmetic effects hooks:

| Rarity | Default Color Hex | Drop Weight | VFX Hook |
|---|---|---|---|
| **Common** | `#9D9D9D` | 100.0 | `Vfx_Common` |
| **Uncommon** | `#1EFF00` | 40.0 | `Vfx_Uncommon` |
| **Rare** | `#0070DD` | 15.0 | `Vfx_Rare` |
| **Epic** | `#A335EE` | 5.0 | `Vfx_Epic` |
| **Legendary** | `#FF8000` | 1.0 | `Vfx_Legendary` |
| **Mythic** | `#E6CC80` | 0.2 | `Vfx_Mythic` |
| **Ancient** | `#FF4500` | 0.05 | `Vfx_Ancient` |
| **Divine** | `#00FFFF` | 0.01 | `Vfx_Divine` |

---

## 3. DLC & Plugin Compatibility

The system is designed to support 100,000+ items and future expansions smoothly:
1. **No Code Recompiles:** Category and subcategory fields are raw string checks instead of hardcoded C# enums. This allows adding any custom categories (e.g., `BuildingParts`, `GuildToken`) directly in config files.
2. **JsonExtensionData Catch-All:** Any unrecognized properties in a JSON item entry are automatically parsed into `ExtensionData`. Custom plugins can read their parameters from this map.
3. **Lazy Memory Allocations:** Repeating database queries bypass file reads, caching index lookups into an in-memory dictionary.
