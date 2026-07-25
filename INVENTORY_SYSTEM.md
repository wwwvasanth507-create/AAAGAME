# Inventory System Architecture - Hero of Eternia

This document details the inventory slots, containers, stack operations, sorting, and save game integrations.

---

## 1. Inventory Structures

The inventory ecosystem is decoupled from visual UI structures. It consists of two main classes:

### 1.1 `InventorySlot`
Represents an individual slot. Contains:
- `ItemId`: String ID of the item.
- `Quantity`: Current count in the stack.
- `IsLocked`: Lock flag preventing accidental selling or discarding.
- `IsFavorite`: Favorited flag prioritizing the item during sorting.
- `CustomData`: Key-value string map storing durability, custom upgrade values, or socketed runes.

### 1.2 `InventoryContainer`
Manages the slots array. Used for:
- Player Bag (default capacity: 40 slots).
- Storage Chests.
- Temporary Loot Boxes.
- Quest inventories.

---

## 2. Container Operations

### 2.1 Stack Merging & Splitting
- **`AddItem()`**: Scans for non-locked slots with the same ItemId that have room (`Quantity < MaxStackSize`). It compares custom stats to ensure upgraded items do not merge with base items. If no existing stacks have space, it occupies the first empty slot.
- **`SplitStack(int sourceSlot, int targetSlot, int splitAmount)`**: Shifts `splitAmount` items to target slot, copying the custom item stats.
- **`MergeStacks(int sourceSlot, int targetSlot)`**: Merges matching items up to the maximum stack limit.

### 2.2 Sorting Heuristics
- **`Sort(InventorySortType sortType)`**: Sorts by Name, Rarity, Value, Weight, or Tier.
- **Priority Rules:**
  1. **Favorites First:** Slots marked as `IsFavorite` are automatically moved to the beginning of the inventory array before any other sorting rules apply.
  2. **Empty Slots Last:** Empty slots are shifted to the end of the array.

### 2.3 Search & Filters
- **`Filter(string category, string searchMask)`**: Returns a filtered subset list of active slots. Searches check both item names and descriptions.

---

## 3. Save Game Integration

All inventory configurations are fully integrated with the save manager.
- **Version 3 Format:** Saves include arrays for player inventory slots, active equipment slots, and named storage chests.
- **Corruption Checks:** SHA-256 validation verifies that slot values and item IDs match correct database records upon loading.
