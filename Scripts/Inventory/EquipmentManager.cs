using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Items;
using HeroOfEternia.Player;
using HeroOfEternia.Player.Stats;

namespace HeroOfEternia.Inventory
{
    /// <summary>
    /// Coordinates equipped slots on the player character, applying PBR meshes
    /// and dynamic attribute stat modifiers to PlayerRoot.
    /// </summary>
    public class EquipmentManager
    {
        public Dictionary<EquipmentSlotType, InventorySlot> EquippedItems { get; } = new();
        private readonly Dictionary<EquipmentSlotType, List<Tuple<AttributeType, StatModifier>>> _appliedModifiers = new();

        public EquipmentManager()
        {
            // Initialise all slots as empty
            foreach (EquipmentSlotType slot in Enum.GetValues(typeof(EquipmentSlotType)))
            {
                EquippedItems[slot] = new InventorySlot();
                _appliedModifiers[slot] = new List<Tuple<AttributeType, StatModifier>>();
            }
        }

        /// <summary>
        /// Equips an item from the player inventory slot to the specified equipment slot.
        /// Removes previous equipment modifiers if slot was occupied.
        /// </summary>
        public bool EquipItem(EquipmentSlotType slot, InventorySlot itemSlot, PlayerRoot player)
        {
            if (itemSlot.IsEmpty) return false;

            var db = ServiceLocator.Get<ItemDatabase>();
            var record = db.GetItem(itemSlot.ItemId);
            if (record == null) return false;

            // Verify item category fits the target slot (flexible string match check)
            if (!IsItemValidForSlot(record, slot))
            {
                Logger.Warning($"EquipmentManager: Cannot equip '{record.DisplayName}' in slot '{slot}'. Category mismatch.");
                return false;
            }

            // Unequip current item in slot first
            UnequipItem(slot, player);

            // Copy item slot structure (equipment quantity is 1)
            var equipSlot = EquippedItems[slot];
            equipSlot.ItemId = itemSlot.ItemId;
            equipSlot.Quantity = 1;
            equipSlot.IsLocked = itemSlot.IsLocked;
            equipSlot.IsFavorite = itemSlot.IsFavorite;
            equipSlot.CustomData = new Dictionary<string, string>(itemSlot.CustomData);

            // Apply attribute modifiers to the player
            ApplyModifiers(slot, record, player);

            // Apply visual mesh to player model customizer if loaded
            if (player.Model != null)
            {
                var category = MapSlotToPartCategory(slot);
                if (category.HasValue)
                {
                    player.Model.SwapPart(category.Value, record.ModelPath);
                    if (!string.IsNullOrEmpty(record.MaterialPath))
                    {
                        // Custom material override check if present
                        player.Model.SetPartMaterial(category.Value, GD.Load<Material>(record.MaterialPath));
                    }
                }
            }

            Logger.Info($"EquipmentManager: Equipped '{record.DisplayName}' into slot '{slot}'.");
            return true;
        }

        /// <summary>
        /// Unequips the item from the specified equipment slot and cleans up modifiers.
        /// Returns a clone of the unequipped slot data.
        /// </summary>
        public InventorySlot UnequipItem(EquipmentSlotType slot, PlayerRoot player)
        {
            var equipSlot = EquippedItems[slot];
            if (equipSlot.IsEmpty) return new InventorySlot();

            var unequippedClone = equipSlot.Clone();
            
            // Remove applied attribute modifiers
            RemoveModifiers(slot, player);

            // Reset visual mesh on model customizer
            if (player.Model != null)
            {
                var category = MapSlotToPartCategory(slot);
                if (category.HasValue)
                {
                    player.Model.SwapPart(category.Value, ""); // Reset to default fallback mesh
                }
            }

            equipSlot.Clear();
            Logger.Info($"EquipmentManager: Unequipped slot '{slot}'.");
            return unequippedClone;
        }

        private void ApplyModifiers(EquipmentSlotType slot, ItemRecord record, PlayerRoot player)
        {
            var activeModifiers = _appliedModifiers[slot];
            activeModifiers.Clear();

            foreach (var modData in record.StatModifiers)
            {
                if (Enum.TryParse<AttributeType>(modData.AttributeType, true, out var attrType) &&
                    Enum.TryParse<ModifierType>(modData.ModifierType, true, out var modType))
                {
                    string modifierName = $"Equip_{slot}_{modData.AttributeType}";
                    var mod = new StatModifier(modifierName, modData.Value, modType, ModifierSource.Equipment);
                    
                    player.Data.Attributes.AddModifier(attrType, mod);
                    activeModifiers.Add(new Tuple<AttributeType, StatModifier>(attrType, mod));
                }
                else
                {
                    Logger.Warning($"EquipmentManager: Failed to parse modifier: Attribute='{modData.AttributeType}', Type='{modData.ModifierType}'.");
                }
            }
        }

        private void RemoveModifiers(EquipmentSlotType slot, PlayerRoot player)
        {
            var activeModifiers = _appliedModifiers[slot];
            foreach (var pair in activeModifiers)
            {
                player.Data.Attributes.RemoveModifier(pair.Item1, pair.Item2.Id);
            }
            activeModifiers.Clear();
        }

        private bool IsItemValidForSlot(ItemRecord record, EquipmentSlotType slot)
        {
            // Extensible matches of category names with slot categories
            string category = record.Category.ToLowerInvariant();
            return slot switch
            {
                EquipmentSlotType.Helmet => category == "helmet",
                EquipmentSlotType.Chest => category == "armor" || category == "chest",
                EquipmentSlotType.Legs => category == "legs" || category == "pants" || category == "armor",
                EquipmentSlotType.Boots => category == "boots" || category == "feet",
                EquipmentSlotType.Gloves => category == "gloves" || category == "hands",
                EquipmentSlotType.MainWeapon => category == "weapon" || category == "tool",
                EquipmentSlotType.OffHand => category == "shield" || category == "offhand" || category == "weapon",
                EquipmentSlotType.Ring1 => category == "ring" || category == "accessory",
                EquipmentSlotType.Ring2 => category == "ring" || category == "accessory",
                EquipmentSlotType.Necklace => category == "necklace" || category == "accessory",
                EquipmentSlotType.Pet => category == "pet",
                EquipmentSlotType.Mount => category == "mount" || category == "mounttoken",
                _ => false
            };
        }

        private PartCategory? MapSlotToPartCategory(EquipmentSlotType slot)
        {
            return slot switch
            {
                EquipmentSlotType.Helmet => PartCategory.Helmet,
                EquipmentSlotType.Chest => PartCategory.Armor,
                EquipmentSlotType.Boots => PartCategory.Feet,
                EquipmentSlotType.Gloves => PartCategory.Hands,
                EquipmentSlotType.MainWeapon => PartCategory.Weapon,
                EquipmentSlotType.OffHand => PartCategory.Cape, // Off-hand visual maps to Cape for demo fallback
                _ => null
            };
        }
    }
}
