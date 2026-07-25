# Reward System — Hero of Eternia

**Version:** 0.12.0  
**Phase:** Prompt 12 / 150 — Reward Framework  
**Status:** ✅ Production Ready

---

## Overview

The Reward System provides a secure, reusable framework for distributing XP, Currency, Equipment, and Achievements after boss victories, fully integrated with save state encryption to prevent double-claiming.

---

## Reward Items

A `RewardDefinition` contains a list of `RewardItem` structures:

| Reward Type | Description | Save Persistence |
|-------------|-------------|------------------|
| `Experience`| Grants character level-up points | Auto-added to PlayerXp |
| `Currency` | Adds gold coins | Auto-added to player inventory |
| `Equipment` | Spawns specific weapon or armor loot | Appended to inventory slots |
| `CraftingMaterial` | Spawns consumable raw mats | Appended to inventory slots |
| `Achievement` | Unlocks achievement status | Saved in profile metadata |
| `Title` | Grants cosmetic prefix title | Saved in profile metadata |

---

## Double-Claim Prevention (Anti-Duping)

**File:** `Scripts/Combat/Encounter/RewardFramework.cs`

All reward claims are checked against `ClaimedRewards` list in `RewardClaimTracker`:
1. Player attempts to claim reward.
2. Checks if `RewardId` exists in `_claimedRewards` set.
3. If already claimed, logs warning and drops transaction.
4. If unclaimed, registers claim to set, grants items, and triggers `RewardClaimedEvent` on EventBus.
5. Saves state immediately.
