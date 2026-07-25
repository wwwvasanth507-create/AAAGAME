using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter2
{
    public class AdvancedRecipe
    {
        public string RecipeId { get; set; } = string.Empty;
        public string ResultItemId { get; set; } = string.Empty;
        public Dictionary<string, int> Ingredients { get; set; } = new();
        public int CraftingTimeSeconds { get; set; } = 5;
    }

    public class Chapter2ContentAdditions
    {
        private readonly List<AdvancedRecipe> _recipes = new();

        public void RegisterChapter2Recipes()
        {
            var steelSword = new AdvancedRecipe
            {
                RecipeId = "recipe_steel_sword",
                ResultItemId = "item_weapon_steel_sword",
                CraftingTimeSeconds = 6
            };
            steelSword.Ingredients["item_mat_iron_ore"] = 4;
            steelSword.Ingredients["item_mat_coal"] = 2;
            _recipes.Add(steelSword);

            var sylvanArmor = new AdvancedRecipe
            {
                RecipeId = "recipe_sylvan_armor",
                ResultItemId = "item_armor_sylvan_tunic",
                CraftingTimeSeconds = 8
            };
            sylvanArmor.Ingredients["item_mat_heavy_leather"] = 5;
            sylvanArmor.Ingredients["item_mat_moonflower"] = 3;
            _recipes.Add(sylvanArmor);
        }

        public IReadOnlyList<AdvancedRecipe> AllRecipes => _recipes;
    }
}
