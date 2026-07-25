using System;
using System.Collections.Generic;
using HeroOfEternia.Inventory;

namespace HeroOfEternia.Items
{
    /// <summary>
    /// Definition for a single item drop record.
    /// </summary>
    public class LootEntry
    {
        public string ItemId { get; set; } = "";
        public float Chance { get; set; } = 1.0f; // 0.0 to 1.0 (e.g. 0.25 is 25% drop rate)
        public int MinQuantity { get; set; } = 1;
        public int MaxQuantity { get; set; } = 1;
    }

    /// <summary>
    /// Generic loot table resolver. Roll logic generates random loot drops
    /// from enemies, mining, fishing, chest loot, or quest events.
    /// </summary>
    public class LootTable
    {
        public string TableId { get; set; } = "";
        public List<LootEntry> Entries { get; set; } = new();

        private static readonly Random _rng = new();

        /// <summary>
        /// Rolls loot entries and returns a collection of item slots containing dropped items.
        /// </summary>
        public List<InventorySlot> RollLoot()
        {
            var drops = new List<InventorySlot>();

            foreach (var entry in Entries)
            {
                if (string.IsNullOrEmpty(entry.ItemId)) continue;

                double roll = _rng.NextDouble();
                if (roll <= entry.Chance)
                {
                    int qty = entry.MinQuantity;
                    if (entry.MaxQuantity > entry.MinQuantity)
                    {
                        qty = _rng.Next(entry.MinQuantity, entry.MaxQuantity + 1);
                    }

                    drops.Add(new InventorySlot
                    {
                        ItemId = entry.ItemId,
                        Quantity = qty
                    });
                }
            }

            return drops;
        }
    }
}
