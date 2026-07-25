using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Gathering;
using HeroOfEternia.Inventory;

namespace HeroOfEternia.Crafting
{
    /// <summary>
    /// Result of a craft attempt.
    /// </summary>
    public class CraftResult
    {
        public bool Success { get; set; }
        public string FailureReason { get; set; } = string.Empty;
        public string RecipeId { get; set; } = string.Empty;
        public string ResultItemId { get; set; } = string.Empty;
        public int QuantityProduced { get; set; }
        public int ExperienceGained { get; set; }
        public double CraftTimeSeconds { get; set; }
        public CraftQueueItem? QueueItem { get; set; }
    }

    /// <summary>
    /// A queued craft operation with progress tracking.
    /// </summary>
    public class CraftQueueItem
    {
        public string QueueId { get; set; } = string.Empty;
        public string RecipeId { get; set; } = string.Empty;
        public int BatchCount { get; set; } = 1;
        public int CompletedCount { get; set; }
        public float RemainingTime { get; set; }
        public float TimePerCraft { get; set; }
        public bool IsPaused { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsComplete { get; set; }
        public string StartedAt { get; set; } = string.Empty;
        
        public float Progress => BatchCount > 0 ? (float)CompletedCount / BatchCount : 0f;
    }

    /// <summary>
    /// Crafting event published on EventBus.
    /// </summary>
    public class CraftEvent
    {
        public string RecipeId { get; set; } = string.Empty;
        public string ResultItemId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool Success { get; set; }
        public string Profession { get; set; } = string.Empty;
    }

    /// <summary>
    /// Central crafting manager service.
    /// Handles recipe validation, ingredient validation, craft queue, 
    /// instant/timed/batch crafting, and cancellation.
    /// Full data-driven design — no hardcoded recipes.
    /// </summary>
    public class CraftingManager : IInitializable
    {
        private static CraftingManager? _instance;
        public static CraftingManager Instance => _instance ??= new CraftingManager();

        private RecipeDatabase _recipeDb = null!;
        private ProfessionManager _professionManager = null!;
        private bool _isInitialized;

        /// <summary>Active craft queue.</summary>
        private List<CraftQueueItem> _craftQueue = new();

        /// <summary>Known recipe unlocks (recipe ID -> is unlocked).</summary>
        private Dictionary<string, bool> _knownRecipes = new();

        private int _nextQueueId = 1;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _recipeDb = RecipeDatabase.Instance;
            _professionManager = ProfessionManager.Instance;

            if (!_recipeDb.IsInitialized)
                _recipeDb.Initialize();
            if (!_professionManager.IsInitialized)
                _professionManager.Initialize();

            GD.Print("[CraftingManager] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _craftQueue.Clear();
            _knownRecipes.Clear();
        }

        /// <summary>
        /// Validates a crafting recipe against player state and inventory.
        /// </summary>
        public CraftResult ValidateCraft(string recipeId, InventoryContainer inventory, 
            string workstationType = "", ProfessionType? professionOverride = null)
        {
            var recipe = _recipeDb.GetRecipe(recipeId);
            if (recipe == null)
                return new CraftResult { Success = false, FailureReason = "Recipe not found." };

            // Check if recipe is known
            if (_knownRecipes.TryGetValue(recipeId, out bool unlocked) && !unlocked)
                return new CraftResult { Success = false, FailureReason = "Recipe not learned." };

            // Check profession requirement
            if (!string.IsNullOrEmpty(recipe.Profession))
            {
                ProfessionType profType;
                if (professionOverride.HasValue)
                    profType = professionOverride.Value;
                else if (!Enum.TryParse<ProfessionType>(recipe.Profession, true, out profType))
                    return new CraftResult { Success = false, FailureReason = $"Unknown profession: {recipe.Profession}" };

                if (!_professionManager.MeetsRequirement(profType, recipe.RequiredLevel))
                {
                    var prof = _professionManager.GetProfession(profType);
                    int currentLevel = prof?.Level ?? 0;
                    return new CraftResult 
                    { 
                        Success = false, 
                        FailureReason = $"Requires {recipe.Profession} level {recipe.RequiredLevel} (current: {currentLevel})."
                    };
                }
            }

            // Check workstation requirement
            if (!string.IsNullOrEmpty(recipe.RequiredWorkstation) && 
                !string.IsNullOrEmpty(workstationType))
            {
                if (!recipe.RequiredWorkstation.Equals(workstationType, StringComparison.OrdinalIgnoreCase))
                    return new CraftResult { Success = false, FailureReason = $"Requires workstation: {recipe.RequiredWorkstation}" };
            }

            // Check ingredients
            foreach (var (ingredientId, requiredCount) in recipe.Ingredients)
            {
                int available = 0;
                foreach (var slot in inventory.Slots)
                {
                    if (!slot.IsEmpty && slot.ItemId == ingredientId)
                        available += slot.Quantity;
                }

                if (available < requiredCount)
                    return new CraftResult 
                    { 
                        Success = false, 
                        FailureReason = $"Missing ingredient: {ingredientId} ({requiredCount} needed, {available} available)."
                    };
            }

            return new CraftResult { Success = true };
        }

        /// <summary>
        /// Performs an instant craft (no queue). Consumes ingredients and produces results.
        /// </summary>
        public CraftResult CraftInstant(string recipeId, InventoryContainer inventory,
            string workstationType = "", ProfessionType? professionOverride = null)
        {
            var validation = ValidateCraft(recipeId, inventory, workstationType, professionOverride);
            if (!validation.Success)
                return validation;

            var recipe = _recipeDb.GetRecipe(recipeId)!;

            // Process success chance
            var rng = new Random();
            bool success = rng.NextDouble() <= recipe.SuccessChance;
            
            if (!success)
            {
                // Consume ingredients anyway
                ConsumeIngredients(recipe, inventory);
                
                return new CraftResult
                {
                    Success = false,
                    FailureReason = "Craft failed. Ingredients consumed.",
                    RecipeId = recipeId,
                    ResultItemId = recipe.ResultItemId,
                    QuantityProduced = 0
                };
            }

            // Consume ingredients
            ConsumeIngredients(recipe, inventory);

            // Add result to inventory
            int totalQuantity = recipe.Quantity;
            bool added = inventory.AddItem(recipe.ResultItemId, totalQuantity);

            // Award profession experience
            int xpReward = recipe.ExperienceReward;
            if (!string.IsNullOrEmpty(recipe.Profession) && 
                Enum.TryParse<ProfessionType>(recipe.Profession, true, out var profType))
            {
                _professionManager.AddExperience(profType, xpReward);
            }

            var craftEvent = new CraftEvent
            {
                RecipeId = recipeId,
                ResultItemId = recipe.ResultItemId,
                Quantity = totalQuantity,
                Success = true,
                Profession = recipe.Profession
            };
            EventBus.Publish(craftEvent);

            return new CraftResult
            {
                Success = true,
                RecipeId = recipeId,
                ResultItemId = recipe.ResultItemId,
                QuantityProduced = totalQuantity,
                ExperienceGained = xpReward,
                CraftTimeSeconds = recipe.CraftTime
            };
        }

        /// <summary>
        /// Queues a timed craft operation.
        /// </summary>
        public CraftResult QueueCraft(string recipeId, InventoryContainer inventory, int batchCount = 1,
            string workstationType = "", ProfessionType? professionOverride = null)
        {
            var validation = ValidateCraft(recipeId, inventory, workstationType, professionOverride);
            if (!validation.Success)
                return validation;

            var recipe = _recipeDb.GetRecipe(recipeId)!;

            // Check if we have enough ingredients for the full batch
            foreach (var (ingredientId, requiredCount) in recipe.Ingredients)
            {
                int available = 0;
                foreach (var slot in inventory.Slots)
                {
                    if (!slot.IsEmpty && slot.ItemId == ingredientId)
                        available += slot.Quantity;
                }

                int totalRequired = requiredCount * batchCount;
                if (available < totalRequired)
                {
                    int maxBatch = available / requiredCount;
                    return new CraftResult
                    {
                        Success = false,
                        FailureReason = $"Insufficient ingredients for {batchCount} crafts. Can craft at most {maxBatch}."
                    };
                }
            }

            // Consume all ingredients upfront for the batch
            for (int i = 0; i < batchCount; i++)
            {
                ConsumeIngredients(recipe, inventory);
            }

            var queueItem = new CraftQueueItem
            {
                QueueId = $"cq_{_nextQueueId++}",
                RecipeId = recipeId,
                BatchCount = batchCount,
                CompletedCount = 0,
                RemainingTime = recipe.CraftTime,
                TimePerCraft = recipe.CraftTime,
                IsPaused = false,
                IsCancelled = false,
                IsComplete = false,
                StartedAt = DateTime.UtcNow.ToString("o")
            };

            _craftQueue.Add(queueItem);

            return new CraftResult
            {
                Success = true,
                RecipeId = recipeId,
                ResultItemId = recipe.ResultItemId,
                QuantityProduced = 0,
                CraftTimeSeconds = recipe.CraftTime,
                QueueItem = queueItem
            };
        }

        /// <summary>
        /// Updates all active craft queue items. Call from game loop tick.
        /// </summary>
        public void UpdateQueue(float deltaTime)
        {
            var completedItems = new List<CraftQueueItem>();

            foreach (var item in _craftQueue)
            {
                if (item.IsPaused || item.IsCancelled || item.IsComplete) continue;

                item.RemainingTime -= deltaTime;

                if (item.RemainingTime <= 0)
                {
                    CompleteCraft(item);
                    item.CompletedCount++;
                    item.RemainingTime = item.TimePerCraft;

                    if (item.CompletedCount >= item.BatchCount)
                    {
                        item.IsComplete = true;
                        completedItems.Add(item);
                    }
                }
            }

            foreach (var item in completedItems)
            {
                _craftQueue.Remove(item);
            }
        }

        /// <summary>
        /// Completes a single craft in the queue.
        /// </summary>
        private void CompleteCraft(CraftQueueItem item)
        {
            var recipe = _recipeDb.GetRecipe(item.RecipeId);
            if (recipe == null) return;

            // Success check
            var rng = new Random();
            bool success = rng.NextDouble() <= recipe.SuccessChance;

            if (success)
            {
                var craftEvent = new CraftEvent
                {
                    RecipeId = recipe.RecipeId,
                    ResultItemId = recipe.ResultItemId,
                    Quantity = recipe.Quantity,
                    Success = true,
                    Profession = recipe.Profession
                };
                EventBus.Publish(craftEvent);

                // Award profession experience
                if (!string.IsNullOrEmpty(recipe.Profession) &&
                    Enum.TryParse<ProfessionType>(recipe.Profession, true, out var profType))
                {
                    _professionManager.AddExperience(profType, recipe.ExperienceReward);
                }

                GD.Print($"[CraftingManager] Queued craft complete: {recipe.ResultItemId} x{recipe.Quantity}");
            }
        }

        /// <summary>
        /// Cancels a queued craft.
        /// </summary>
        public bool CancelCraft(string queueId)
        {
            var item = _craftQueue.FirstOrDefault(q => q.QueueId == queueId);
            if (item == null) return false;

            item.IsCancelled = true;
            _craftQueue.Remove(item);
            return true;
        }

        /// <summary>
        /// Pauses or resumes a queued craft.
        /// </summary>
        public bool SetPause(string queueId, bool pause)
        {
            var item = _craftQueue.FirstOrDefault(q => q.QueueId == queueId);
            if (item == null) return false;

            item.IsPaused = pause;
            return true;
        }

        /// <summary>
        /// Consumes ingredients from inventory for a recipe.
        /// </summary>
        private void ConsumeIngredients(RecipeDefinition recipe, InventoryContainer inventory)
        {
            foreach (var (ingredientId, count) in recipe.Ingredients)
            {
                inventory.RemoveItem(ingredientId, count);
            }
        }

        /// <summary>Returns all active craft queue items.</summary>
        public List<CraftQueueItem> GetActiveQueue() => new(_craftQueue);

        /// <summary>Returns total queue count.</summary>
        public int QueueCount => _craftQueue.Count;

        /// <summary>Learns a recipe.</summary>
        public void LearnRecipe(string recipeId)
        {
            _knownRecipes[recipeId] = true;
        }

        /// <summary>Checks if a recipe is known.</summary>
        public bool IsRecipeKnown(string recipeId)
        {
            var recipe = _recipeDb.GetRecipe(recipeId);
            if (recipe == null) return false;
            return recipe.IsDefaultUnlock || (_knownRecipes.TryGetValue(recipeId, out bool known) && known);
        }

        /// <summary>Exports known recipes for save.</summary>
        public List<string> ExportKnownRecipes()
        {
            return _knownRecipes.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        }

        /// <summary>Restores known recipes from save.</summary>
        public void RestoreKnownRecipes(List<string> recipeIds)
        {
            if (recipeIds == null) return;
            foreach (var id in recipeIds)
            {
                _knownRecipes[id] = true;
            }
        }

        /// <summary>Exports active queue for save (for future resume).</summary>
        public List<CraftQueueItem> ExportQueue() => new(_craftQueue);

        /// <summary>Restores queue from save.</summary>
        public void RestoreQueue(List<CraftQueueItem> queue)
        {
            if (queue == null) return;
            _craftQueue = new List<CraftQueueItem>(queue);
            // Recalculate next queue ID
            foreach (var item in _craftQueue)
            {
                if (int.TryParse(item.QueueId.Replace("cq_", ""), out int id) && id >= _nextQueueId)
                    _nextQueueId = id + 1;
            }
        }

        public bool IsInitialized => _isInitialized;
    }
}