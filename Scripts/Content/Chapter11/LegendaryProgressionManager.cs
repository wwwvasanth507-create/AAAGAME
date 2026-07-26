using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter11
{
    public class LegendaryRecipeRecord
    {
        public string RecipeId { get; set; } = "";
        public string TargetItemName { get; set; } = "";
        public string EquipmentSlot { get; set; } = "Weapon";
        public Dictionary<string, int> RequiredMaterials { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public bool IsUnlocked { get; set; } = false;
        public int TierLevel { get; set; } = 5;
    }

    /// <summary>
    /// Legendary Progression Manager for Act IV & Endgame Content.
    /// Manages Tier 5 Legendary Crafting Recipes, Astral Essences, Item Traits, and Legendary Ability Enhancements.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class LegendaryProgressionManager : IInitializable
    {
        private readonly Dictionary<string, LegendaryRecipeRecord> _recipes = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, int> _legendaryMaterials = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<LegendaryRecipeRecord>? OnRecipeUnlocked;
        public event Action<string, int>? OnMaterialQuantityChanged;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultRecipes();

            // Register with ServiceLocator
            ServiceLocator.Register<LegendaryProgressionManager>(this);

            IsInitialized = true;
            Logger.Info("LegendaryProgressionManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _recipes.Clear();
            _legendaryMaterials.Clear();

            ServiceLocator.Unregister<LegendaryProgressionManager>();
            IsInitialized = false;
            Logger.Info("LegendaryProgressionManager: Shutdown completed.");
        }

        private void RegisterDefaultRecipes()
        {
            // 1. Blade of Sol Supreme
            RegisterRecipe(new LegendaryRecipeRecord
            {
                RecipeId = "recipe_legendary_sol_blade",
                TargetItemName = "Astral Sunblade of Sol",
                EquipmentSlot = "Weapon",
                RequiredMaterials = new Dictionary<string, int>
                {
                    { "material_astral_essence", 5 },
                    { "resource_sol_ore", 12 }
                },
                TierLevel = 5
            });

            // 2. Crown of Celestial Light
            RegisterRecipe(new LegendaryRecipeRecord
            {
                RecipeId = "recipe_legendary_celestial_crown",
                TargetItemName = "Diadem of Astral Light",
                EquipmentSlot = "Head",
                RequiredMaterials = new Dictionary<string, int>
                {
                    { "material_sun_core_fragment", 3 },
                    { "resource_caelum_dust", 15 }
                },
                TierLevel = 5
            });
        }

        public void RegisterRecipe(LegendaryRecipeRecord recipe)
        {
            if (recipe != null && !string.IsNullOrEmpty(recipe.RecipeId))
            {
                _recipes[recipe.RecipeId] = recipe;
            }
        }

        public bool UnlockRecipe(string recipeId)
        {
            if (!_recipes.TryGetValue(recipeId, out var r)) return false;
            if (r.IsUnlocked) return true;

            r.IsUnlocked = true;
            OnRecipeUnlocked?.Invoke(r);
            Logger.Info($"LegendaryProgressionManager: Unlocked Tier {r.TierLevel} Legendary Recipe '{r.TargetItemName}' ({recipeId}).");
            return true;
        }

        public void AddMaterial(string materialId, int amount)
        {
            if (string.IsNullOrEmpty(materialId) || amount <= 0) return;

            _legendaryMaterials.TryGetValue(materialId, out int current);
            int updated = current + amount;
            _legendaryMaterials[materialId] = updated;

            OnMaterialQuantityChanged?.Invoke(materialId, updated);
            Logger.Info($"LegendaryProgressionManager: Added {amount}x '{materialId}'. Total: {updated}.");
        }

        public int GetMaterialQuantity(string materialId)
        {
            return _legendaryMaterials.TryGetValue(materialId, out int qty) ? qty : 0;
        }

        public LegendaryRecipeRecord? GetRecipe(string recipeId)
        {
            return _recipes.TryGetValue(recipeId, out var r) ? r : null;
        }

        public List<LegendaryRecipeRecord> GetAllRecipes()
        {
            return new List<LegendaryRecipeRecord>(_recipes.Values);
        }
    }
}
