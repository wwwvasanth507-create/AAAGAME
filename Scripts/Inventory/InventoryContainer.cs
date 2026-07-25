using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Items;

namespace HeroOfEternia.Inventory
{
    public enum InventorySortType
    {
        Name,
        Rarity,
        Value,
        Weight,
        Tier
    }

    /// <summary>
    /// Governs collections of InventorySlots (e.g. Player Bag, Chest, Loot drops).
    /// Implements stack arithmetic, splitting, sorting algorithms, and filters.
    /// </summary>
    public class InventoryContainer
    {
        public InventorySlot[] Slots { get; private set; }
        public int Capacity => Slots.Length;

        public InventoryContainer(int capacity)
        {
            Slots = new InventorySlot[capacity];
            for (int i = 0; i < capacity; i++)
            {
                Slots[i] = new InventorySlot();
            }
        }

        /// <summary>
        /// Populates container with list of slot states (useful for loading saves).
        /// </summary>
        public void LoadSlots(List<InventorySlot> loadedSlots)
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (i < loadedSlots.Count)
                {
                    Slots[i] = loadedSlots[i].Clone();
                }
                else
                {
                    Slots[i].Clear();
                }
            }
        }

        /// <summary>
        /// Converts active slot states into a list representation for saving.
        /// </summary>
        public List<InventorySlot> SaveSlots()
        {
            var list = new List<InventorySlot>();
            foreach (var slot in Slots)
            {
                list.Add(slot.Clone());
            }
            return list;
        }

        /// <summary>
        /// Attempts to add an item to the inventory container.
        /// Merges into existing stacks first, then populates empty slots.
        /// Returns true if the entire quantity was added.
        /// </summary>
        public bool AddItem(string itemId, int quantity = 1, Dictionary<string, string>? customData = null)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0) return false;

            var db = ServiceLocator.Get<ItemDatabase>();
            var record = db.GetItem(itemId);
            if (record == null)
            {
                Logger.Error($"InventoryContainer: Failed to add item. ID '{itemId}' not found in database.");
                return false;
            }

            int remaining = quantity;
            int maxStack = record.StackSize;

            // 1. Merge into existing stacks (if stackable)
            if (maxStack > 1)
            {
                for (int i = 0; i < Capacity; i++)
                {
                    var slot = Slots[i];
                    if (!slot.IsEmpty && slot.ItemId == itemId && slot.Quantity < maxStack && !slot.IsLocked)
                    {
                        // Check custom data matches (e.g. don't stack items with different modifications)
                        if (AreCustomDataMatching(slot.CustomData, customData))
                        {
                            int space = maxStack - slot.Quantity;
                            int toAdd = Math.Min(space, remaining);
                            slot.Quantity += toAdd;
                            remaining -= toAdd;

                            if (remaining <= 0) break;
                        }
                    }
                }
            }

            // 2. Occupy empty slots
            if (remaining > 0)
            {
                for (int i = 0; i < Capacity; i++)
                {
                    var slot = Slots[i];
                    if (slot.IsEmpty)
                      {
                        int toAdd = Math.Min(maxStack, remaining);
                        slot.ItemId = itemId;
                        slot.Quantity = toAdd;
                        slot.IsLocked = false;
                        slot.IsFavorite = false;
                        slot.CustomData = customData != null ? new Dictionary<string, string>(customData) : new Dictionary<string, string>();
                        remaining -= toAdd;

                        if (remaining <= 0) break;
                    }
                }
            }

            if (remaining > 0)
            {
                Logger.Warning($"InventoryContainer: Container full. Failed to place {remaining}x '{itemId}'.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes a quantity of an item from the container, scanning from the end.
        /// </summary>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0) return false;

            // Count total inventory matches
            int totalAvailable = Slots.Where(s => !s.IsEmpty && s.ItemId == itemId && !s.IsLocked).Sum(s => s.Quantity);
            if (totalAvailable < quantity)
            {
                Logger.Warning($"InventoryContainer: Insufficient items to remove. Requested {quantity}x '{itemId}', available {totalAvailable}x.");
                return false;
            }

            int remaining = quantity;

            // Remove from non-locked slots (prioritizing non-favorite slots first)
            var targetSlots = Slots
                .Select((s, index) => new { Slot = s, Index = index })
                .Where(x => !x.Slot.IsEmpty && x.Slot.ItemId == itemId && !x.Slot.IsLocked)
                .OrderBy(x => x.Slot.IsFavorite) // non-favorites first
                .ToList();

            foreach (var item in targetSlots)
            {
                var slot = item.Slot;
                if (slot.Quantity <= remaining)
                {
                    remaining -= slot.Quantity;
                    slot.Clear();
                }
                else
                {
                    slot.Quantity -= remaining;
                    remaining = 0;
                }

                if (remaining <= 0) break;
            }

            return true;
        }

        /// <summary>
        /// Splits a stack from a source slot into a target slot.
        /// </summary>
        public bool SplitStack(int sourceIdx, int targetIdx, int splitAmount)
        {
            if (sourceIdx < 0 || sourceIdx >= Capacity || targetIdx < 0 || targetIdx >= Capacity) return false;
            var source = Slots[sourceIdx];
            var target = Slots[targetIdx];

            if (source.IsEmpty || source.IsLocked || splitAmount <= 0 || splitAmount >= source.Quantity) return false;
            if (!target.IsEmpty && (target.ItemId != source.ItemId || target.IsLocked)) return false;

            var db = ServiceLocator.Get<ItemDatabase>();
            var record = db.GetItem(source.ItemId);
            int maxStack = record?.StackSize ?? 99;

            if (!target.IsEmpty && target.Quantity + splitAmount > maxStack) return false;

            if (target.IsEmpty)
            {
                target.ItemId = source.ItemId;
                target.Quantity = splitAmount;
                target.CustomData = new Dictionary<string, string>(source.CustomData);
            }
            else
            {
                target.Quantity += splitAmount;
            }

            source.Quantity -= splitAmount;
            return true;
        }

        /// <summary>
        /// Merges stack from source slot into target slot.
        /// </summary>
        public bool MergeStacks(int sourceIdx, int targetIdx)
        {
            if (sourceIdx < 0 || sourceIdx >= Capacity || targetIdx < 0 || targetIdx >= Capacity) return false;
            var source = Slots[sourceIdx];
            var target = Slots[targetIdx];

            if (source.IsEmpty || target.IsEmpty || source.ItemId != target.ItemId) return false;
            if (source.IsLocked || target.IsLocked) return false;

            // Must have matching custom stats to merge (e.g. same upgrades)
            if (!AreCustomDataMatching(source.CustomData, target.CustomData)) return false;

            var db = ServiceLocator.Get<ItemDatabase>();
            var record = db.GetItem(target.ItemId);
            int maxStack = record?.StackSize ?? 99;

            int space = maxStack - target.Quantity;
            if (space <= 0) return false;

            int toMove = Math.Min(space, source.Quantity);
            target.Quantity += toMove;
            source.Quantity -= toMove;

            if (source.Quantity <= 0)
            {
                source.Clear();
            }

            return true;
        }

        /// <summary>
        /// Swaps positions of two slots.
        /// </summary>
        public void SwapSlots(int idxA, int idxB)
        {
            if (idxA < 0 || idxA >= Capacity || idxB < 0 || idxB >= Capacity || idxA == idxB) return;
            var temp = Slots[idxA];
            Slots[idxA] = Slots[idxB];
            Slots[idxB] = temp;
        }

        /// <summary>
        /// Locks / Unlocks a slot.
        /// </summary>
        public void LockSlot(int index, bool lockState)
        {
            if (index >= 0 && index < Capacity)
            {
                Slots[index].IsLocked = lockState;
            }
        }

        /// <summary>
        /// Favorites / Unfavorites a slot.
        /// </summary>
        public void FavoriteSlot(int index, bool favoriteState)
        {
            if (index >= 0 && index < Capacity)
            {
                Slots[index].IsFavorite = favoriteState;
            }
        }

        /// <summary>
        /// Filters the container slots returning references matching criteria.
        /// </summary>
        public List<InventorySlot> Filter(string? category = null, string? searchMask = null)
        {
            var results = new List<InventorySlot>();
            var db = ServiceLocator.Get<ItemDatabase>();

            foreach (var slot in Slots)
            {
                if (slot.IsEmpty) continue;

                var record = db.GetItem(slot.ItemId);
                if (record == null) continue;

                // Category filter
                if (!string.IsNullOrEmpty(category) && !string.Equals(record.Category, category, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Text search filter
                if (!string.IsNullOrEmpty(searchMask))
                {
                    bool matchName = record.DisplayName.Contains(searchMask, StringComparison.OrdinalIgnoreCase);
                    bool matchDesc = record.Description.Contains(searchMask, StringComparison.OrdinalIgnoreCase);
                    if (!matchName && !matchDesc) continue;
                }

                results.Add(slot);
            }

            return results;
        }

        /// <summary>
        /// Sorts slots according to criteria.
        /// Favorites are always placed at the front of the list.
        /// Empty slots are moved to the end.
        /// </summary>
        public void Sort(InventorySortType sortType)
        {
            var db = ServiceLocator.Get<ItemDatabase>();

            // Temporarily extract non-empty slots with their associated ItemRecords
            var activeSlots = Slots
                .Where(s => !s.IsEmpty)
                .Select(s => new { Slot = s, Record = db.GetItem(s.ItemId)! })
                .Where(x => x.Record != null)
                .ToList();

            // Sort by Favorite first (descending), then specified type
            var query = activeSlots.OrderByDescending(x => x.Slot.IsFavorite);

            IOrderedEnumerable<dynamic> sorted = sortType switch
            {
                InventorySortType.Name => query.ThenBy(x => x.Record.DisplayName),
                InventorySortType.Rarity => query.ThenByDescending(x => (int)x.Record.Rarity),
                InventorySortType.Value => query.ThenByDescending(x => x.Record.SellValue),
                InventorySortType.Weight => query.ThenBy(x => x.Record.Weight),
                InventorySortType.Tier => query.ThenByDescending(x => x.Record.Tier),
                _ => query.ThenBy(x => x.Record.UniqueId)
            };

            var sortedList = sorted.Select(x => x.Slot.Clone()).ToList();

            // Refill the slots array
            for (int i = 0; i < Capacity; i++)
            {
                if (i < sortedList.Count)
                {
                    Slots[i] = sortedList[i];
                }
                else
                {
                    Slots[i] = new InventorySlot();
                }
            }
        }

        private bool AreCustomDataMatching(Dictionary<string, string> dictA, Dictionary<string, string>? dictB)
        {
            if (dictB == null) return dictA.Count == 0;
            if (dictA.Count != dictB.Count) return false;

            foreach (var kvp in dictA)
            {
                if (!dictB.TryGetValue(kvp.Key, out string? valB) || kvp.Value != valB)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
