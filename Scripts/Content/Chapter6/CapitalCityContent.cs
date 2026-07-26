using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter6
{
    public class CapitalDistrictDefinition
    {
        public string DistrictId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Services { get; set; } = new();
        public List<string> KeyNpcs { get; set; } = new();
        public List<string> Shops { get; set; } = new();
        public bool RoyalGuardsActive { get; set; } = true;
    }

    /// <summary>
    /// Content definition for Eternia Prime, the magnificent capital city of the realm.
    /// Manages 8 major urban districts, royal archives, guild headquarters, and underground passages.
    /// </summary>
    public class CapitalCityContent
    {
        private readonly Dictionary<string, CapitalDistrictDefinition> _districts = new(StringComparer.OrdinalIgnoreCase);

        public string CityId { get; } = "city_eternia_prime";
        public string DisplayName { get; } = "Eternia Prime Capital";
        public string CapitalRuler { get; } = "High King Roderick III";

        public CapitalCityContent()
        {
            InitializeDistricts();
        }

        public void InitializeDistricts()
        {
            // 1. Royal District
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_royal",
                Name = "The Royal Citadel & Throne Spire",
                Description = "The seat of imperial majesty housing the High King's Palace, royal court, and diplomatic embassies.",
                Services = new List<string> { "royal_audience", "imperial_bounties", "knighthood_conferral" },
                KeyNpcs = new List<string> { "npc_high_king_roderick", "npc_grand_chancellor_valen" },
                Shops = new List<string> { "shop_royal_armorer" }
            });

            // 2. Commercial Bazaar
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_commercial",
                Name = "Grand Bazaar of the Sun",
                Description = "Sprawling open-air marketplace filled with international merchants, luxury traders, and auction houses.",
                Services = new List<string> { "luxury_trading", "vault_storage", "grand_auction" },
                KeyNpcs = new List<string> { "npc_master_merchant_silk", "npc_banker_aurelius" },
                Shops = new List<string> { "shop_capital_general", "shop_luxury_gems" }
            });

            // 3. Iron Foundry Quarter
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_foundry",
                Name = "Imperial Foundry & War Quarter",
                Description = "Massive steam forges, siege weapon workshops, and master blacksmith guildhalls.",
                Services = new List<string> { "master_forge", "tier4_gear_enhancement", "siege_crafting" },
                KeyNpcs = new List<string> { "npc_grand_smith_thorin" },
                Shops = new List<string> { "shop_foundry_ingots" }
            });

            // 4. Guild District
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_guild",
                Name = "Grand Guild Enclave",
                Description = "Headquarters of the Adventurers' Guild, Mages' Arcane Circle, and Merchant Consortium.",
                Services = new List<string> { "guild_induction", "guild_rank_promotion", "guild_bounty_board" },
                KeyNpcs = new List<string> { "npc_guildmaster_vane", "npc_archmage_serena" },
                Shops = new List<string> { "shop_guild_rewards" }
            });

            // 5. Temple District
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_temple",
                Name = "Cathedral of the Eternal Light",
                Description = "Towering white marble cathedral devoted to ancient blessings, high healing, and divine magic.",
                Services = new List<string> { "high_healing", "divine_blessing", "curse_removal" },
                KeyNpcs = new List<string> { "npc_high_cleric_elena" },
                Shops = new List<string> { "shop_temple_elixirs" }
            });

            // 6. Residential Quarters
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_residential",
                Name = "Old City Residential Heights",
                Description = "Winding cobbled streets lined with merchant townhouses, taverns, and local artisan shops.",
                Services = new List<string> { "player_housing", "inn_rest" },
                KeyNpcs = new List<string> { "npc_tavernkeeper_barnaby" },
                Shops = new List<string> { "shop_local_food" }
            });

            // 7. Royal Barracks & Archives
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_archives",
                Name = "Royal Archives & Imperial Guard Barracks",
                Description = "Vast subterranean archives storing ancient Eternian history and the main barracks of the Royal Guard.",
                Services = new List<string> { "ancient_lore_research", "tactical_training" },
                KeyNpcs = new List<string> { "npc_archivist_orion", "npc_captain_gideon" },
                Shops = new List<string> { "shop_archival_tomes" }
            });

            // 8. Underground Catacombs
            RegisterDistrict(new CapitalDistrictDefinition
            {
                DistrictId = "district_capital_underground",
                Name = "The Sunken Catacombs & Undercity",
                Description = "Shadowy tunnels running beneath the capital, used by rogues and black market traders.",
                Services = new List<string> { "black_market_trading", "secret_passages" },
                KeyNpcs = new List<string> { "npc_shadow_broker_nyx" },
                Shops = new List<string> { "shop_black_market" },
                RoyalGuardsActive = false
            });
        }

        public void RegisterDistrict(CapitalDistrictDefinition district)
        {
            if (district != null && !string.IsNullOrEmpty(district.DistrictId))
            {
                _districts[district.DistrictId] = district;
            }
        }

        public CapitalDistrictDefinition? GetDistrict(string districtId)
        {
            return _districts.TryGetValue(districtId, out var d) ? d : null;
        }

        public List<CapitalDistrictDefinition> GetAllDistricts()
        {
            return new List<CapitalDistrictDefinition>(_districts.Values);
        }
    }
}
