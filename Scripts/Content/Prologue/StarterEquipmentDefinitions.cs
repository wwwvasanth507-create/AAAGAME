using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Prologue
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Tool,
        Consumable,
        Material,
        QuestItem
    }

    public class StarterItemProfile
    {
        public string ItemId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ItemType Type { get; set; } = ItemType.Consumable;
        public int ValueGold { get; set; } = 5;
        public string Description { get; set; } = string.Empty;
        public int StatBonus { get; set; } = 0;
    }

    public class StarterEquipmentDefinitions
    {
        private readonly Dictionary<string, StarterItemProfile> _items = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterDefaultItems()
        {
            RegisterItem(new StarterItemProfile
            {
                ItemId = "item_weapon_rusty_sword",
                DisplayName = "Rusty Iron Sword",
                Type = ItemType.Weapon,
                ValueGold = 10,
                Description = "A simple iron blade weathered by time.",
                StatBonus = 5
            });

            RegisterItem(new StarterItemProfile
            {
                ItemId = "item_armor_leather_tunic",
                DisplayName = "Oakvale Leather Tunic",
                Type = ItemType.Armor,
                ValueGold = 15,
                Description = "Basic protective tunic favored by woodsmen.",
                StatBonus = 3
            });

            RegisterItem(new StarterItemProfile
            {
                ItemId = "item_tool_pickaxe",
                DisplayName = "Miner's Pickaxe",
                Type = ItemType.Tool,
                ValueGold = 8,
                Description = "Sturdy tool for mining ore veins."
            });

            RegisterItem(new StarterItemProfile
            {
                ItemId = "item_potion_healing_salve",
                DisplayName = "Healing Salve",
                Type = ItemType.Consumable,
                ValueGold = 5,
                Description = "Restores 25 Health points over 3 seconds."
            });

            RegisterItem(new StarterItemProfile
            {
                ItemId = "item_mat_iron_ore",
                DisplayName = "Iron Ore",
                Type = ItemType.Material,
                ValueGold = 4,
                Description = "Raw iron mined from rock deposits."
            });
        }

        public void RegisterItem(StarterItemProfile item)
        {
            if (item != null && !string.IsNullOrEmpty(item.ItemId))
            {
                _items[item.ItemId] = item;
            }
        }

        public StarterItemProfile? GetItem(string itemId)
        {
            return _items.TryGetValue(itemId, out var it) ? it : null;
        }

        public IReadOnlyCollection<StarterItemProfile> AllItems => _items.Values;
    }
}
