# Save System Validation — Hero of Eternia

**Version:** 0.12.0  
**Audit Date:** 2026-07-25  

---

## 1. Security & Cryptography Validation

- **Encryption**: AES-256 with key derivation via PBKDF2 (1000 iterations).
- **Device Lock**: Derives key dynamically from local salt + `OS.GetUniqueId()`. Saves are bound to the hardware device and cannot be copied between handsets without server re-keys. Falls back to constants during unit testing.
- **Integrity**: Appends SHA-256 hashes at the end of save files. Altering any character in the save file fails verification and redirects to backup loads.

---

## 2. Backup & Corruption Recovery

```
Load(slotId)
   │
   ├─► Read main file (.sav)
   ├─► Verify SHA-256 checksum
   │     ├─► PASS: Decrypt and load profile
   │     └─► FAIL: Log warning/tampering ──► Load backup (.bak)
   │                                            ├─► PASS: Restore backup as main file
   │                                            └─► FAIL: Return null (New Profile)
```

---

## 3. Schema Migrations Log (V1 to V9)

Save profiles support smooth upward transformations during load ticks:

| Schema Version | Introduced | Migrated Fields |
|----------------|------------|-----------------|
| **V1** | Initial Save | Level, Health, Mana, Position |
| **V2** | Player Model | Equipped visual parts, BaseAttributes list, ActiveEffects list |
| **V3** | Inventory | PlayerInventory list, EquippedSlots, StorageChests |
| **V4** | World | WorldSeed value, DiscoveredRegions list, ModifiedChunkNodes |
| **V5** | Terrain | ModifiedDecorations, DiscoveredNavRegions, Landmark mappings |
| **V6** | NPC | NpcSaveState, ReputationSnapshot, RelationshipSnapshot |
| **V7** | Combat | UnlockedCombatStyles, LearnedAbilities, WeaponDurability |
| **V8** | Early Loop | UnlockedAbilityIds, EquippedAbilitySlots, Wave progressions |
| **V9** | Encounters | CompletedEncounters, DefeatedBossIds, ClaimedRewards |

---

## 4. Test Verification
V1 to V9 migrations have been tested. Test case `P12-9` performs encryption roundtrips and asserts correct data schema conversions.
