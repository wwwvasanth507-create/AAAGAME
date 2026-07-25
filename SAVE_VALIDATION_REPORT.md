# Save Validation Report — Hero of Eternia (v0.8.0)

This report validates save profile migrations (V1 to V5) and verification recoveries.

---

## 1. Migration Checks

The `SaveManager.MigrateProfile` method handles upgrade paths on startup:

| Starting Version | Upgraded Fields Instantiated | Target Version |
|---|---|---|
| **Version 1** | EquippedParts, BaseAttributes, ActiveEffects | Version 5 |
| **Version 2** | PlayerInventory, EquippedSlots, StorageChests | Version 5 |
| **Version 3** | WorldSeed, DiscoveredRegions, ModifiedChunkNodes | Version 5 |
| **Version 4** | ModifiedDecorations, DiscoveredNavRegions, PopulatedLandmarks | Version 5 |

---

## 2. Integrity & Cryptography

- **PBKDF2 Keys:** Encryptions use dynamic derived keys matching active device unique IDs.
- **SHA-256 Checksums:** Appended hash values prevent manual hex editing of profiles.
- **Backup Recoveries:** Backup restoration sweeps (.bak slots) restore profiles if a write fails.
