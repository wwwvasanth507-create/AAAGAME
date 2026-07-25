using Godot;
using System;
using System.Collections.Generic;
using HeroOfEternia.Core;
using HeroOfEternia.Inventory;

namespace HeroOfEternia.Crafting
{
    public class CraftingRecipe
    {
        public string RecipeId { get; set; } = string.Empty;
        public string OutputItemId { get; set; } = string.Empty;
        public int OutputQuantity { get; set; } = 1;
        public int GoldCost { get; set; } = 100;
        public Dictionary<string, int> RequiredMaterials { set; get; } = new();
    }

    public class CraftingSystem : IInitializable
    {
        private static CraftingSystem? _instance;
        public static CraftingSystem Instance => _instance ??= new CraftingSystem();

        private readonly Dictionary<string, CraftingRecipe> _recipes = new();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            LoadDefaultRecipes();
            GD.Print("[CraftingSystem] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _recipes.Clear();
        }

        private void LoadDefaultRecipes()
        {
            _recipes.Clear();

            _recipes["craft_iron_sword"] = new CraftingRecipe
            {
                RecipeId = "craft_iron_sword",
                OutputItemId = "weapon_iron_sword",
                GoldCost = 150,
                RequiredMaterials = new Dictionary<string, int> { { "mat_iron_ingot", 3 }, { "mat_wood", 1 } }
            };

            _recipes["craft_health_potion"] = new CraftingRecipe
            {
                RecipeId = "craft_health_potion",
                OutputItemId = "consumable_health_potion",
                OutputQuantity = 2,
                GoldCost = 50,
                RequiredMaterials = new Dictionary<string, int> { { "mat_herb_red", 2 }, { "mat_vial", 2 } }
            };
        }

        public bool CraftItem(string recipeId, InventoryContainer inventory)
        {
            if (!_recipes.TryGetValue(recipeId, out var recipe)) return false;

            // Verify materials
            foreach (var (matId, count) in recipe.RequiredMaterials)
            {
                int totalAvailable = 0;
                foreach (var slot in inventory.Slots)
                {
                    if (slot.ItemId == matId) totalAvailable += slot.Quantity;
                }

                if (totalAvailable < count)
                {
                    GD.Print($"[CraftingSystem] Missing material {matId} ({count} required, found {totalAvailable})");
                    return false;
                }
            }

            // Deduct materials
            foreach (var (matId, count) in recipe.RequiredMaterials)
            {
                int remainingToRemove = count;
                foreach (var slot in inventory.Slots)
                {
                    if (slot.ItemId == matId)
                    {
                        int removeAmount = Math.Min(remainingToRemove, slot.Quantity);
                        slot.Quantity -= removeAmount;
                        remainingToRemove -= removeAmount;
                        if (slot.Quantity <= 0) slot.Clear();
                        if (remainingToRemove <= 0) break;
                    }
                }
            }

            EventBus.Publish(recipe.OutputItemId);
            GD.Print($"[CraftingSystem] Successfully crafted {recipe.OutputItemId} x{recipe.OutputQuantity}");
            return true;
        }

        public CraftingRecipe? GetRecipe(string recipeId)
        {
            return _recipes.TryGetValue(recipeId, out var r) ? r : null;
        }
    }
}
