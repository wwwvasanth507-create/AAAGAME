using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HeroOfEternia.Core;
using HeroOfEternia.Items;

namespace HeroOfEternia.Economy
{
    /// <summary>
    /// Central database of all merchant definitions.
    /// Data-driven loading from JSON configuration.
    /// </summary>
    public class MerchantDatabase : IInitializable
    {
        private readonly Dictionary<string, MerchantData> _merchants = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _merchantsBySettlement = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<MerchantType, List<string>> _merchantsByType = new();
        
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            _merchants.Clear();
            _merchantsBySettlement.Clear();
            _merchantsByType.Clear();
            LoadMerchantDatabase();
            IsInitialized = true;
            Logger.Info($"MerchantDatabase: Loaded {_merchants.Count} merchants across {_merchantsBySettlement.Count} settlements.");
        }

        private void LoadMerchantDatabase()
        {
            try
            {
                var configManager = ServiceLocator.Get<ConfigManager>();
                string json = configManager.GetConfigJson("merchant_database");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<MerchantData>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (list != null)
                    {
                        foreach (var merchant in list)
                        {
                            if (!string.IsNullOrEmpty(merchant.MerchantId))
                            {
                                RegisterMerchant(merchant);
                            }
                        }
                        Logger.Info($"MerchantDatabase: Successfully loaded {_merchants.Count} merchants from config.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"MerchantDatabase: Config load exception: {ex.Message}");
            }

            Logger.Warning("MerchantDatabase: Using fallback merchant definitions.");
            PopulateFallbackMerchants();
        }

        private void RegisterMerchant(MerchantData merchant)
        {
            _merchants[merchant.MerchantId] = merchant;

            // Index by settlement
            if (!string.IsNullOrEmpty(merchant.SettlementId))
            {
                if (!_merchantsBySettlement.ContainsKey(merchant.SettlementId))
                    _merchantsBySettlement[merchant.SettlementId] = new List<string>();
                _merchantsBySettlement[merchant.SettlementId].Add(merchant.MerchantId);
            }

            // Index by type
            if (!_merchantsByType.ContainsKey(merchant.Type))
                _merchantsByType[merchant.Type] = new List<string>();
            _merchantsByType[merchant.Type].Add(merchant.MerchantId);
        }

        /// <summary>Get merchant by ID. Returns null if not found.</summary>
        public MerchantData? GetMerchant(string merchantId)
        {
            if (string.IsNullOrEmpty(merchantId)) return null;
            return _merchants.TryGetValue(merchantId, out var merchant) ? merchant : null;
        }

        /// <summary>Get all merchants in a settlement.</summary>
        public List<MerchantData> GetMerchantsBySettlement(string settlementId)
        {
            var results = new List<MerchantData>();
            if (_merchantsBySettlement.TryGetValue(settlementId, out var ids))
            {
                foreach (var id in ids)
                {
                    if (_merchants.TryGetValue(id, out var m))
                        results.Add(m);
                }
            }
            return results;
        }

        /// <summary>Get all merchants of a specific type.</summary>
        public List<MerchantData> GetMerchantsByType(MerchantType type)
        {
            var results = new List<MerchantData>();
            if (_merchantsByType.TryGetValue(type, out var ids))
            {
                foreach (var id in ids)
                {
                    if (_merchants.TryGetValue(id, out var m))
                        results.Add(m);
                }
            }
            return results;
        }

        /// <summary>Get all merchants.</summary>
        public List<MerchantData> GetAllMerchants()
        {
            return new List<MerchantData>(_merchants.Values);
        }

        /// <summary>Register a new merchant at runtime (no code change needed).</summary>
        public void AddMerchant(MerchantData merchant)
        {
            if (merchant == null || string.IsNullOrEmpty(merchant.MerchantId)) return;
            if (!_merchants.ContainsKey(merchant.MerchantId))
            {
                RegisterMerchant(merchant);
                Logger.Info($"MerchantDatabase: Added merchant '{merchant.MerchantId}' at runtime.");
            }
        }

        /// <summary>Get count of merchants.</summary>
        public int MerchantCount => _merchants.Count;

        /// <summary>Get all settlement IDs that have merchants.</summary>
        public List<string> GetSettlementIds()
        {
            return new List<string>(_merchantsBySettlement.Keys);
        }

        private void PopulateFallbackMerchants()
        {
            // Village General Store
            AddMerchant(new MerchantData
            {
                MerchantId = "mer_intro_general",
                Name = "Elder_Merchant",
                DisplayName = "Elder Marcus",
                Type = MerchantType.GeneralStore,
                SettlementId = "village_harmony",
                Profession = "shopkeeper",
                PriceModifier = 1.0f,
                BuyModifier = 0.6f,
                SellModifier = 1.4f,
                GoldCapacity = 5000,
                CurrentGold = 2000,
                OpenHour = 6f,
                CloseHour = 21f,
                PreferredGoods = new List<MerchantCategory> { MerchantCategory.Food, MerchantCategory.Materials },
                DislikedGoods = new List<MerchantCategory> { MerchantCategory.Illegal },
                InventoryRules = new List<string> { "pot_minor_health", "food_bread", "material_wood" },
                Inventory = new Dictionary<string, int> { { "pot_minor_health", 10 }, { "food_bread", 20 }, { "material_wood", 50 } }
            });

            // Village Blacksmith
            AddMerchant(new MerchantData
            {
                MerchantId = "mer_intro_blacksmith",
                Name = "Smith_Brenna",
                DisplayName = "Brenna Ironhand",
                Type = MerchantType.Blacksmith,
                SettlementId = "village_harmony",
                Profession = "blacksmith",
                PriceModifier = 1.2f,
                BuyModifier = 0.5f,
                SellModifier = 1.5f,
                GoldCapacity = 8000,
                CurrentGold = 3000,
                OpenHour = 7f,
                CloseHour = 19f,
                PreferredGoods = new List<MerchantCategory> { MerchantCategory.Materials, MerchantCategory.Weapons },
                DislikedGoods = new List<MerchantCategory> { MerchantCategory.Food, MerchantCategory.Potions },
                InventoryRules = new List<string> { "wpn_iron_sword", "material_iron_ore", "armor_leather" },
                Inventory = new Dictionary<string, int> { { "wpn_iron_sword", 3 }, { "material_iron_ore", 30 }, { "armor_leather", 2 } }
            });

            // Town Alchemist
            AddMerchant(new MerchantData
            {
                MerchantId = "mer_town_alchemist",
                Name = "Alchemist_Elara",
                DisplayName = "Elara Moonwhisper",
                Type = MerchantType.Alchemist,
                SettlementId = "town_haven",
                Profession = "alchemist",
                PriceModifier = 1.1f,
                BuyModifier = 0.6f,
                SellModifier = 1.4f,
                GoldCapacity = 12000,
                CurrentGold = 5000,
                OpenHour = 8f,
                CloseHour = 20f,
                PreferredGoods = new List<MerchantCategory> { MerchantCategory.Potions, MerchantCategory.Materials, MerchantCategory.Magic },
                InventoryRules = new List<string> { "pot_minor_health", "pot_mana", "herb_moonflower", "material_crystal_shard" },
                Inventory = new Dictionary<string, int> { { "pot_minor_health", 15 }, { "pot_mana", 10 }, { "herb_moonflower", 25 }, { "material_crystal_shard", 12 } }
            });

            // City Exotic Trader
            AddMerchant(new MerchantData
            {
                MerchantId = "mer_city_exotic",
                Name = "Exotic_Zephyr",
                DisplayName = "Zephyr Starfall",
                Type = MerchantType.ExoticTrader,
                SettlementId = "city_eternia",
                Profession = "exotic_merchant",
                PriceModifier = 1.5f,
                BuyModifier = 0.4f,
                SellModifier = 2.0f,
                GoldCapacity = 50000,
                CurrentGold = 20000,
                OpenHour = 10f,
                CloseHour = 22f,
                PreferredGoods = new List<MerchantCategory> { MerchantCategory.Exotic, MerchantCategory.Magic, MerchantCategory.Jeweler },
                InventoryRules = new List<string> { "gem_ruby", "gem_sapphire", "scroll_teleport", "potion_ultimate" },
                Inventory = new Dictionary<string, int> { { "gem_ruby", 3 }, { "gem_sapphire", 2 }, { "scroll_teleport", 5 }, { "potion_ultimate", 1 } }
            });

            // Port Fishing Merchant
            AddMerchant(new MerchantData
            {
                MerchantId = "mer_port_fish",
                Name = "Fisher_Kai",
                DisplayName = "Kai Tidehunter",
                Type = MerchantType.Fisherman,
                SettlementId = "port_brinewall",
                Profession = "fisherman",
                PriceModifier = 0.9f,
                BuyModifier = 0.7f,
                SellModifier = 1.2f,
                GoldCapacity = 3000,
                CurrentGold = 1500,
                OpenHour = 5f,
                CloseHour = 18f,
                PreferredGoods = new List<MerchantCategory> { MerchantCategory.Food, MerchantCategory.General },
                InventoryRules = new List<string> { "food_fish", "food_bread", "material_rope" },
                Inventory = new Dictionary<string, int> { { "food_fish", 30 }, { "food_bread", 10 }, { "material_rope", 15 } }
            });

            Logger.Info($"MerchantDatabase: Populated {_merchants.Count} fallback merchants.");
        }
    }
}