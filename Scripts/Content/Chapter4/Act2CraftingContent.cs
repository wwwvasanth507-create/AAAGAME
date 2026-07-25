using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public class AdvancedCraftingRecipe
    {
        public string RecipeId { get; set; } = string.Empty;
        public string OutputItemId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int CraftingTier { get; set; } = 2;
        public Dictionary<string, int> Ingredients { get; set; } = new();
        public int RequiredLevel { get; set; }
    }

    public class AdvancedCraftingStation
    {
        public string StationId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int UnlockedAtLevel { get; set; }
    }

    /// <summary>
    /// Act II crafting expansion — registers advanced Tier 2/3 recipes available
    /// after defeating the Watchtower Captain and unlocking Forge-Master Brynn.
    /// </summary>
    public class Act2CraftingContent
    {
        private readonly List<AdvancedCraftingRecipe> _recipes = new();
        private readonly List<AdvancedCraftingStation> _stations = new();

        public void RegisterCraftingContent()
        {
            // Advanced Crafting Stations
            _stations.Add(new AdvancedCraftingStation
            {
                StationId = "station_ridgeline_forge",
                DisplayName = "Ridgeline War Forge",
                Location = "poi_ridgeline_watchtower",
                UnlockedAtLevel = 20
            });

            _stations.Add(new AdvancedCraftingStation
            {
                StationId = "station_mirkwood_alchemy",
                DisplayName = "Swamp Alchemy Cauldron",
                Location = "region_mirkwood_swamps",
                UnlockedAtLevel = 22
            });

            // Tier 2 Weapon Recipes
            _recipes.Add(new AdvancedCraftingRecipe
            {
                RecipeId = "recipe_stormforged_sword",
                OutputItemId = "item_weapon_stormforged_sword",
                DisplayName = "Stormforged Sword",
                CraftingTier = 2,
                RequiredLevel = 20,
                Ingredients = new Dictionary<string, int>
                {
                    { "item_resource_void_crystal", 2 },
                    { "item_material_storm_iron", 4 },
                    { "item_material_ridgeline_flint", 2 }
                }
            });

            // Tier 2 Armor Recipes
            _recipes.Add(new AdvancedCraftingRecipe
            {
                RecipeId = "recipe_ridgeline_warplate",
                OutputItemId = "item_armor_ridgeline_warplate",
                DisplayName = "Ridgeline War Plate",
                CraftingTier = 2,
                RequiredLevel = 21,
                Ingredients = new Dictionary<string, int>
                {
                    { "item_material_storm_iron", 6 },
                    { "item_resource_void_crystal", 1 },
                    { "item_material_harpy_feather", 3 }
                }
            });

            // Tier 2 Consumable Recipes
            _recipes.Add(new AdvancedCraftingRecipe
            {
                RecipeId = "recipe_swamp_antidote",
                OutputItemId = "item_consumable_swamp_antidote",
                DisplayName = "Swamp Antidote",
                CraftingTier = 2,
                RequiredLevel = 21,
                Ingredients = new Dictionary<string, int>
                {
                    { "item_herb_bog_lotus", 3 },
                    { "item_herb_swamp_moss", 2 }
                }
            });

            // Tier 3 Unlock Preview
            _recipes.Add(new AdvancedCraftingRecipe
            {
                RecipeId = "recipe_voidweave_cloak",
                OutputItemId = "item_armor_voidweave_cloak",
                DisplayName = "Voidweave Cloak",
                CraftingTier = 3,
                RequiredLevel = 24,
                Ingredients = new Dictionary<string, int>
                {
                    { "item_resource_void_crystal", 4 },
                    { "item_material_shadow_silk", 5 },
                    { "item_gem_moonstone", 1 }
                }
            });

            Logger.Info($"Act2CraftingContent: {_stations.Count} stations, {_recipes.Count} recipes registered.");
        }

        public IReadOnlyList<AdvancedCraftingRecipe> AllRecipes => _recipes;
        public IReadOnlyList<AdvancedCraftingStation> AllStations => _stations;
    }
}
