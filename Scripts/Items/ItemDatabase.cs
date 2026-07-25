using System;
using System.Collections.Generic;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.Items
{
    /// <summary>
    /// Service managing the central database of all item records.
    /// Loaded dynamically from configuration layers.
    /// </summary>
    public class ItemDatabase : IInitializable
    {
        private readonly Dictionary<string, ItemRecord> _items = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<ItemRarity, RarityDefinition> _rarities = new();

        public void Initialize()
        {
            LoadRarityDefinitions();
            LoadItemRecords();
        }

        private void LoadRarityDefinitions()
        {
            _rarities.Clear();
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("rarities");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<RarityDefinition>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            _rarities[item.Rarity] = item;
                        }
                        Logger.Info($"ItemDatabase: Loaded {_rarities.Count} Rarity definitions.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"ItemDatabase: Rarity loading exception: {ex.Message}");
            }

            // Fallback default definitions if config load fails
            Logger.Warning("ItemDatabase: Using fallback Rarity definitions.");
            PopulateFallbackRarities();
        }

        private void LoadItemRecords()
        {
            _items.Clear();
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("item_database");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<ItemRecord>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            if (!string.IsNullOrEmpty(item.UniqueId))
                            {
                                _items[item.UniqueId] = item;
                            }
                        }
                        Logger.Info($"ItemDatabase: Successfully indexed {_items.Count} items.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"ItemDatabase: Item records load exception: {ex.Message}");
            }

            // Fallback default records
            Logger.Warning("ItemDatabase: Initializing database with fallback core items.");
            PopulateFallbackItems();
        }

        /// <summary>
        /// Retrieves an item by its unique ID. Returns null if not found.
        /// </summary>
        public ItemRecord? GetItem(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId)) return null;
            return _items.TryGetValue(uniqueId, out var item) ? item : null;
        }

        /// <summary>
        /// Retrieves all items matching the specified category string.
        /// </summary>
        public List<ItemRecord> GetItemsByCategory(string category)
        {
            var results = new List<ItemRecord>();
            foreach (var item in _items.Values)
            {
                if (string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(item);
                }
            }
            return results;
        }

        /// <summary>
        /// Returns rarity definitions.
        /// </summary>
        public RarityDefinition? GetRarity(ItemRarity rarity)
        {
            return _rarities.TryGetValue(rarity, out var def) ? def : null;
        }

        /// <summary>
        /// Returns all items in the database.
        /// </summary>
        public List<ItemRecord> GetAllItems()
        {
            return new List<ItemRecord>(_items.Values);
        }

        private void PopulateFallbackRarities()
        {
            foreach (ItemRarity rarity in Enum.GetValues(typeof(ItemRarity)))
            {
                _rarities[rarity] = new RarityDefinition
                {
                    Rarity = rarity,
                    ColorHex = GetDefaultRarityColorHex(rarity),
                    DropWeight = GetDefaultRarityDropWeight(rarity),
                    VisualEffectHook = $"Vfx_{rarity}"
                };
            }
        }

        private string GetDefaultRarityColorHex(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => "#9D9D9D",
                ItemRarity.Uncommon => "#1EFF00",
                ItemRarity.Rare => "#0070DD",
                ItemRarity.Epic => "#A335EE",
                ItemRarity.Legendary => "#FF8000",
                ItemRarity.Mythic => "#E6CC80",
                ItemRarity.Ancient => "#FF4500",
                ItemRarity.Divine => "#00FFFF",
                _ => "#FFFFFF"
            };
        }

        private float GetDefaultRarityDropWeight(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => 100f,
                ItemRarity.Uncommon => 40f,
                ItemRarity.Rare => 15f,
                ItemRarity.Epic => 5f,
                ItemRarity.Legendary => 1f,
                ItemRarity.Mythic => 0.2f,
                ItemRarity.Ancient => 0.05f,
                ItemRarity.Divine => 0.01f,
                _ => 1f
            };
        }

        private void PopulateFallbackItems()
        {
            // Simple fallback items for unit test reliability and editor defaults
            var sword = new ItemRecord
            {
                UniqueId = "wpn_iron_sword",
                InternalName = "Iron Sword",
                DisplayName = "Rusty Iron Sword",
                Description = "A simple iron blade, dull but functional.",
                Category = "Weapon",
                Subcategory = "OneHandSword",
                Tier = 1,
                Rarity = ItemRarity.Common,
                Weight = 2.5f,
                StackSize = 1,
                SellValue = 5,
                BuyValue = 15,
                LocKey = "item.rusty_sword"
            };
            sword.StatModifiers.Add(new AttributeModifierData { AttributeType = "Strength", Value = 2f, ModifierType = "Flat" });
            _items[sword.UniqueId] = sword;

            var potion = new ItemRecord
            {
                UniqueId = "pot_minor_health",
                InternalName = "Minor Health Potion",
                DisplayName = "Minor Health Potion",
                Description = "Restores 30 Health over 3 seconds.",
                Category = "Potion",
                Subcategory = "Health",
                Tier = 1,
                Rarity = ItemRarity.Common,
                Weight = 0.5f,
                StackSize = 20,
                SellValue = 2,
                BuyValue = 8,
                LocKey = "item.health_potion"
            };
            _items[potion.UniqueId] = potion;
        }
    }
}
