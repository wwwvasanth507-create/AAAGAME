using System;
using System.Collections.Generic;

namespace HeroOfEternia.Inventory
{
    /// <summary>
    /// Repesents a slot container inside any inventory storage module.
    /// Tracks items references, counts, favorites, locks, and modifier properties.
    /// </summary>
    public class InventorySlot
    {
        public string ItemId { get; set; } = "";
        public int Quantity { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
        public bool IsFavorite { get; set; } = false;

        // Arbitrary item metadata parameters (e.g. durability, custom attributes, sockets)
        public Dictionary<string, string> CustomData { get; set; } = new();

        /// <summary>
        /// Returns true if the slot is empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(ItemId) || Quantity <= 0;

        /// <summary>
        /// Clears all slot properties.
        /// </summary>
        public void Clear()
        {
            ItemId = "";
            Quantity = 0;
            IsLocked = false;
            IsFavorite = false;
            CustomData.Clear();
        }

        /// <summary>
        /// Clone utility for duplicating slot configurations safely.
        /// </summary>
        public InventorySlot Clone()
        {
            return new InventorySlot
            {
                ItemId = ItemId,
                Quantity = Quantity,
                IsLocked = IsLocked,
                IsFavorite = IsFavorite,
                CustomData = new Dictionary<string, string>(CustomData)
            };
        }
    }
}
