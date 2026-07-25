# SAVE_SYSTEM_REPORT.md
# Hero of Eternia — Save System Full Audit Report

**Date:** 2026-07-25
**Current Save Version:** 6
**Status:** ✅ All migrations validated

---

## Save Version History

| Version | Phase | New Fields | Migration Test |
|---------|-------|------------|---------------|
| V1 | P2/P3 Base | Stats, Inventory (legacy), Quests, World | ✅ |
| V2 | P5 | EquippedParts, BaseAttributes, ActiveEffects | ✅ |
| V3 | P6 | PlayerInventory, EquippedSlots, StorageChests | ✅ |
| V4 | P7 | WorldSeed (ulong), DiscoveredRegions, ModifiedChunkNodes | ✅ |
| V5 | P8 | ModifiedDecorations, DiscoveredNavRegions, PopulatedLandmarks | ✅ |
| V6 | P9 | NpcStates, ReputationSnapshot, RelationshipSnapshot | ✅ |

---

## Migration Chain Validation

```
V1 → V2: Initialize EquippedParts {}, BaseAttributes {}, ActiveEffects []
V2 → V3: Initialize PlayerInventory [], EquippedSlots {}, StorageChests {}
V3 → V4: Initialize WorldSeed 12345, DiscoveredRegions {}, ModifiedChunkNodes {}
V4 → V5: Initialize ModifiedDecorations {}, DiscoveredNavRegions {}, PopulatedLandmarks {}
V5 → V6: Initialize NpcStates {}, ReputationSnapshot {}, RelationshipSnapshot {}
```

Each migration step is additive — no existing fields are removed or renamed. ✅

---

## Save Profile Structure (V6)

```json
{
  "SaveVersion": 6,
  "GameVersion": "1.0.0",

  // Player
  "Stats":           { "CharacterName", "Level", "CurrentXp", "Health", "Mana", "Stamina" },
  "Inventory":       { "Items", "Equipment", "CraftedItems" },
  "Quests":          { "ActiveQuests", "CompletedQuests" },
  "World":           { "WorldSeed", "TimeOfDay", "MapDiscovery", "NpcStates (legacy)" },
  "StatsData":       { "PlayTimeSeconds", "KillsCount", "SavesCount" },

  // V2 — Player Visuals & Attributes
  "EquippedParts":   { "PartCategory": "ResPath" },
  "BaseAttributes":  { "AttributeName": float },
  "ActiveEffects":   [ "effectType" ],

  // V3 — Inventory Systems
  "PlayerInventory": [ InventorySlot ],
  "EquippedSlots":   { "slotName": InventorySlot },
  "StorageChests":   { "chestId": [ InventorySlot ] },

  // V4 — Procedural World
  "WorldSeed":            ulong,
  "DiscoveredRegions":    [ "regionId" ],
  "ModifiedChunkNodes":   { "chunkId": [ "nodeId" ] },

  // V5 — World Decorations & Navigation
  "ModifiedDecorations":  { "chunkId": [ "decorId" ] },
  "DiscoveredNavRegions": [ "regionId" ],
  "PopulatedLandmarks":   { "landmarkId": "status" },

  // V6 — NPC Systems
  "NpcStates":            { "npcId": NpcSaveState },
  "ReputationSnapshot":   { "global": int, "reg:id": int, "fac:id": int, "ind:id": int },
  "RelationshipSnapshot": { "npcA_npcB": [float, float, float, float] },

  // Future-proof
  "ExtensionData": {}
}
```

---

## Encryption & Integrity

| Feature | Implementation |
|---------|---------------|
| Encryption | AES-256-CBC via Rfc2898DeriveBytes (PBKDF2) |
| Key derivation | AppSalt + DeviceUniqueId (fallback: TEST_DEVICE) |
| Integrity | SHA-256 checksum appended to file |
| Backup | .bak file created before each save overwrite |
| Recovery | Auto-restore from .bak if primary file is corrupt |
| Corruption detection | Checksum mismatch → load backup → re-save |

---

## Test Coverage (Save-related Tests)

| Test | Version | Status |
|------|---------|--------|
| AES write + load + backup restore | V1 | ✅ |
| V1 → V2 migration (EquippedParts + Attributes) | V2 | ✅ |
| V2 → V3 migration (Inventory slots) | V3 | ✅ |
| V3 → V4 migration (World seed + regions) | V4 | ✅ |
| V4 → V5 migration (Decorations + nav) | V5 | ✅ |
| V5 → V6 migration (NpcStates + Reputation) | V6 | ✅ |
| V6 NpcStates round-trip | V6 | ✅ |
| V6 ReputationSnapshot round-trip | V6 | ✅ |

---

## Known Limitations

| Issue | Severity | Mitigation |
|-------|----------|-----------|
| NpcStates dictionary grows with world size | Low | Future: chunk-scoped save partitioning |
| RelationshipSnapshot: pair key is string, not index | Low | Acceptable for < 10,000 NPC pairs |
| No cloud save | Informational | Intentionally offline-first |

---

## Verdict

**Save System: PRODUCTION READY ✅**
- All 6 versions migrate cleanly.
- AES-256 encryption + SHA-256 integrity verified.
- Offline-first, device-bound, backup-protected.
