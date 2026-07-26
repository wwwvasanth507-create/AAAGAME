using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter4
{
    public class CityDistrictDefinition
    {
        public string DistrictId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> AvailableServices { get; set; } = new();
        public List<string> KeyNpcIds { get; set; } = new();
        public List<string> MerchantShopIds { get; set; } = new();
        public bool GuardPatrolActive { get; set; } = true;
    }

    /// <summary>
    /// Content definition for Valenhold, the central regional metropolis of Act II.
    /// Manages 6 major city districts, services, guilds, guard HQ, and merchant hubs.
    /// </summary>
    public class ValenholdCityContent
    {
        private readonly Dictionary<string, CityDistrictDefinition> _districts = new(StringComparer.OrdinalIgnoreCase);

        public string CityId { get; } = "city_valenhold";
        public string DisplayName { get; } = "Valenhold Citadel";
        public string OwningFactionId { get; set; } = "faction_iron_vanguard";

        public ValenholdCityContent()
        {
            InitializeDistricts();
        }

        public void InitializeDistricts()
        {
            // 1. Government District
            RegisterDistrict(new CityDistrictDefinition
            {
                DistrictId = "district_valenhold_government",
                Name = "High Council Heights",
                Description = "The political heart of Valenhold housing High Council chambers, diplomatic emissaries, and faction heralds.",
                AvailableServices = new List<string> { "diplomacy_audience", "bounty_board", "reputation_herald" },
                KeyNpcIds = new List<string> { "npc_commander_harek", "npc_emissary_null" },
                MerchantShopIds = new List<string> { "shop_high_council_armory" },
                GuardPatrolActive = true
            });

            // 2. Market Quarter & Trade Harbor
            RegisterDistrict(new CityDistrictDefinition
            {
                DistrictId = "district_valenhold_market",
                Name = "Silver Bay Market & Harbor",
                Description = "Bustling merchant docks and trade bazaar filled with rare exotic goods from across Eternia.",
                AvailableServices = new List<string> { "caravan_travel", "item_appraisal", "bank_storage" },
                KeyNpcIds = new List<string> { "npc_elda_swamp_warden", "npc_merchant_silver" },
                MerchantShopIds = new List<string> { "shop_valenhold_general", "shop_exotic_imports" },
                GuardPatrolActive = true
            });

            // 3. Crafting Quarter
            RegisterDistrict(new CityDistrictDefinition
            {
                DistrictId = "district_valenhold_crafting",
                Name = "The Iron Foundry Quarter",
                Description = "Roaring war forges, alchemical laboratories, and master artisan workstations.",
                AvailableServices = new List<string> { "war_forge", "alchemy_station", "gear_repair" },
                KeyNpcIds = new List<string> { "npc_forgemaster_brynn" },
                MerchantShopIds = new List<string> { "shop_blacksmith_supplies", "shop_alchemist_herbs" },
                GuardPatrolActive = false
            });

            // 4. Guild Hall & Training Arena
            RegisterDistrict(new CityDistrictDefinition
            {
                DistrictId = "district_valenhold_guild",
                Name = "Champions' Guild Hall",
                Description = "Training arena where veteran gladiators spar and adventuring contracts are awarded.",
                AvailableServices = new List<string> { "combat_arena_sparring", "skill_respec", "contract_board" },
                KeyNpcIds = new List<string> { "npc_arena_master_kael" },
                MerchantShopIds = new List<string> { "shop_arena_rewards" },
                GuardPatrolActive = true
            });

            // 5. Temple Heights
            RegisterDistrict(new CityDistrictDefinition
            {
                DistrictId = "district_valenhold_temple",
                Name = "Sanctuary of the Astral Embers",
                Description = "A sacred temple complex devoted to healing, blessing heroic weapons, and ancient lore.",
                AvailableServices = new List<string> { "sanctuary_healing", "relic_blessing", "lore_library" },
                KeyNpcIds = new List<string> { "npc_high_priestess_lyra" },
                MerchantShopIds = new List<string> { "shop_blessed_potions" },
                GuardPatrolActive = false
            });

            // 6. Guard Headquarters & Barracks
            RegisterDistrict(new CityDistrictDefinition
            {
                DistrictId = "district_valenhold_guard_hq",
                Name = "Vanguard Fortress & Bastion",
                Description = "Impenetrable stone fortress coordinating city security, crime enforcement, and jail cells.",
                AvailableServices = new List<string> { "crime_bounty_pay", "guard_enlistment" },
                KeyNpcIds = new List<string> { "npc_captain_drael" },
                MerchantShopIds = new List<string> { "shop_vanguard_quartermaster" },
                GuardPatrolActive = true
            });
        }

        public void RegisterDistrict(CityDistrictDefinition district)
        {
            if (district != null && !string.IsNullOrEmpty(district.DistrictId))
            {
                _districts[district.DistrictId] = district;
            }
        }

        public CityDistrictDefinition? GetDistrict(string districtId)
        {
            return _districts.TryGetValue(districtId, out var district) ? district : null;
        }

        public List<CityDistrictDefinition> GetAllDistricts()
        {
            return new List<CityDistrictDefinition>(_districts.Values);
        }
    }
}
