using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using HeroOfEternia.Core;
using HeroOfEternia.Gathering;

namespace HeroOfEternia.Crafting
{
    /// <summary>
    /// Crafting recipe definition.
    /// Each recipe specifies profession, level, ingredients, result, and conditions.
    /// </summary>
    public class RecipeDefinition
    {
        /// <summary>Unique recipe identifier (e.g. "craft_iron_sword").</summary>
        public string RecipeId { get; set; } = string.Empty;
        
        /// <summary>Display name of the recipe.</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>Description of what the recipe creates.</summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>Profession required to craft this recipe.</summary>
        public string Profession { get; set; } = string.Empty;
        
        /// <summary>Minimum profession level required.</summary>
        public int RequiredLevel { get; set; } = 1;
        
        /// <summary>Required ingredients (item ID -> quantity).</summary>
        public Dictionary<string, int> Ingredients { get; set; } = new();
        
        /// <summary>Result item ID.</summary>
        public string ResultItemId { get; set; } = string.Empty;
        
        /// <summary>Quantity of result item produced.</summary>
        public int Quantity { get; set; } = 1;
        
        /// <summary>Base craft time in seconds.</summary>
        public float CraftTime { get; set; } = 3.0f;
        
        /// <summary>Base success chance (0.0 - 1.0).</summary>
        public float SuccessChance { get; set; } = 1.0f;
        
        /// <summary>Experience awarded per craft.</summary>
        public int ExperienceReward { get; set; } = 50;
        
        /// <summary>Workstation type required (empty = any).</summary>
        public string RequiredWorkstation { get; set; } = string.Empty;
        
        /// <summary>Is this recipe unlocked by default?</summary>
        public bool IsDefaultUnlock { get; set; } = true;
        
        /// <summary>Category for recipe grouping.</summary>
        public string Category { get; set; } = string.Empty;
        
        /// <summary>Schema version.</summary>
        public int Version { get; set; } = 1;
        
        /// <summary>Future quality modifier hooks.</summary>
        public Dictionary<string, float> QualityModifiers { get; set; } = new();
        
        /// <summary>Future specialization bonus hooks.</summary>
        public Dictionary<string, float> SpecializationBonuses { get; set; } = new();
        
        /// <summary>Future DLC / extension catch-all.</summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }

    /// <summary>
    /// Data-driven recipe database.
    /// Loads from Settings/crafting_recipes.json.
    /// Supports unlimited recipes without code changes.
    /// </summary>
    public class RecipeDatabase : IInitializable
    {
        private static RecipeDatabase? _instance;
        public static RecipeDatabase Instance => _instance ??= new RecipeDatabase();

        private Dictionary<string, RecipeDefinition> _recipes = new();
        private Dictionary<string, List<string>> _professionIndex = new();
        private Dictionary<string, List<string>> _categoryIndex = new();
        private Dictionary<string, List<string>> _workstationIndex = new();
        private bool _isInitialized;
        private bool _isLoaded;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            GD.Print("[RecipeDatabase] Initialized. Call LoadDatabase() to load recipes.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _isLoaded = false;
            _recipes.Clear();
            _professionIndex.Clear();
            _categoryIndex.Clear();
            _workstationIndex.Clear();
        }

        /// <summary>
        /// Loads recipe definitions from a JSON file.
        /// </summary>
        public bool LoadDatabase(string filePath = "res://Settings/crafting_recipes.json")
        {
            try
            {
                var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
                if (file == null)
                {
                    Logger.Error($"[RecipeDatabase] Failed to open file: {filePath}");
                    return false;
                }

                string json = file.GetAsText();
                file.Close();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var wrapper = JsonSerializer.Deserialize<RecipeDatabaseWrapper>(json, options);
                if (wrapper?.Recipes == null)
                {
                    Logger.Error("[RecipeDatabase] Invalid database format.");
                    return false;
                }

                _recipes.Clear();
                _professionIndex.Clear();
                _categoryIndex.Clear();
                _workstationIndex.Clear();

                foreach (var recipe in wrapper.Recipes)
                {
                    if (string.IsNullOrEmpty(recipe.RecipeId))
                    {
                        Logger.Warning("[RecipeDatabase] Skipping recipe with empty RecipeId.");
                        continue;
                    }

                    _recipes[recipe.RecipeId] = recipe;

                    // Build profession index
                    string prof = string.IsNullOrEmpty(recipe.Profession) ? "Any" : recipe.Profession;
                    if (!_professionIndex.ContainsKey(prof))
                        _professionIndex[prof] = new List<string>();
                    _professionIndex[prof].Add(recipe.RecipeId);

                    // Build category index
                    string cat = string.IsNullOrEmpty(recipe.Category) ? "Uncategorized" : recipe.Category;
                    if (!_categoryIndex.ContainsKey(cat))
                        _categoryIndex[cat] = new List<string>();
                    _categoryIndex[cat].Add(recipe.RecipeId);

                    // Build workstation index
                    string ws = string.IsNullOrEmpty(recipe.RequiredWorkstation) ? "Any" : recipe.RequiredWorkstation;
                    if (!_workstationIndex.ContainsKey(ws))
                        _workstationIndex[ws] = new List<string>();
                    _workstationIndex[ws].Add(recipe.RecipeId);
                }

                _isLoaded = true;
                GD.Print($"[RecipeDatabase] Loaded {_recipes.Count} recipes across {_professionIndex.Count} professions.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"[RecipeDatabase] Load failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>Gets a recipe by ID. Returns null if not found.</summary>
        public RecipeDefinition? GetRecipe(string recipeId)
        {
            return _recipes.TryGetValue(recipeId, out var r) ? r : null;
        }

        /// <summary>Returns all loaded recipes.</summary>
        public IEnumerable<RecipeDefinition> GetAllRecipes() => _recipes.Values;

        /// <summary>Returns total recipe count.</summary>
        public int RecipeCount => _recipes.Count;

        /// <summary>Returns recipes for a specific profession.</summary>
        public List<string> GetRecipesByProfession(string profession)
        {
            var results = new List<string>();
            if (_professionIndex.TryGetValue(profession, out var list))
                results.AddRange(list);
            if (profession != "Any" && _professionIndex.TryGetValue("Any", out var anyList))
                results.AddRange(anyList);
            return results;
        }

        /// <summary>Returns recipes by category.</summary>
        public List<string> GetRecipesByCategory(string category)
        {
            return _categoryIndex.TryGetValue(category, out var list) 
                ? new List<string>(list) 
                : new List<string>();
        }

        /// <summary>Returns recipes requiring a specific workstation.</summary>
        public List<string> GetRecipesByWorkstation(string workstation)
        {
            return _workstationIndex.TryGetValue(workstation, out var list) 
                ? new List<string>(list) 
                : new List<string>();
        }

        /// <summary>Fast lookup check.</summary>
        public bool HasRecipe(string recipeId) => _recipes.ContainsKey(recipeId);

        public bool IsInitialized => _isInitialized;
        public bool IsLoaded => _isLoaded;
    }

    /// <summary>
    /// JSON wrapper for recipe database deserialization.
    /// </summary>
    internal class RecipeDatabaseWrapper
    {
        public string Version { get; set; } = "1.0";
        public List<RecipeDefinition> Recipes { get; set; } = new();
    }
}