using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter3
{
    public class Tier2Item
    {
        public string ItemId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Type { get; set; } = "Weapon";
        public int ValueGold { get; set; }
        public int StatBonus { get; set; }
    }

    public class Chapter3Rewards
    {
        private readonly List<Tier2Item> _tier2Items = new();

        public void RegisterTier2Rewards()
        {
            _tier2Items.Add(new Tier2Item
            {
                ItemId = "item_weapon_void_cleaver",
                DisplayName = "Void-Cleaved Broadsword",
                Type = "Weapon",
                ValueGold = 600,
                StatBonus = 28
            });

            _tier2Items.Add(new Tier2Item
            {
                ItemId = "item_armor_shadow_plate",
                DisplayName = "Shadow-Forged Plate",
                Type = "Armor",
                ValueGold = 750,
                StatBonus = 22
            });

            _tier2Items.Add(new Tier2Item
            {
                ItemId = "item_resource_void_crystal",
                DisplayName = "Void Crystal Fragment",
                Type = "Crafting",
                ValueGold = 120,
                StatBonus = 0
            });

            _tier2Items.Add(new Tier2Item
            {
                ItemId = "item_potion_elixir_of_resilience",
                DisplayName = "Elixir of Resilience",
                Type = "Consumable",
                ValueGold = 80,
                StatBonus = 0
            });
        }

        public IReadOnlyList<Tier2Item> AllTier2Items => _tier2Items;
    }
}
