using System;
using System.Collections.Generic;

namespace HeroOfEternia.Player.Abilities
{
    /// <summary>
    /// Defines the category of an ability for organizational and filtering purposes.
    /// Categories are extensible and data-driven.
    /// </summary>
    public enum AbilityCategory
    {
        Melee,
        Magic,
        Ranged,
        Movement,
        Support,
        Healing,
        Defensive,
        Summoning,
        Passive,
        Ultimate,
        Utility
    }

    /// <summary>
    /// Defines the resource type consumed by an ability.
    /// </summary>
    public enum ResourceType
    {
        Health,
        Mana,
        Stamina,
        Energy,
        Focus,
        Rage,
        Spirit,
        None
    }

    /// <summary>
    /// Defines the execution type of an ability.
    /// </summary>
    public enum AbilityExecutionType
    {
        Instant,
        Cast,
        Channeled,
        Toggle,
        Charge
    }

    /// <summary>
    /// Defines the ability type classification.
    /// </summary>
    public enum AbilityType
    {
        Active,
        Passive,
        Toggle,
        Triggered,
        Ultimate
    }

    /// <summary>
    /// Defines the upgrade path for an ability.
    /// </summary>
    public class AbilityUpgradePath
    {
        public int Level { get; set; } = 1;
        public string Description { get; set; } = string.Empty;
        public float DamageMultiplier { get; set; } = 1.0f;
        public float HealingMultiplier { get; set; } = 1.0f;
        public float CooldownReduction { get; set; } = 0f;
        public float RangeIncrease { get; set; } = 0f;
        public float ResourceCostReduction { get; set; } = 0f;
        public float CastTimeReduction { get; set; } = 0f;
        public float DurationIncrease { get; set; } = 0f;
        public List<string> NewEffects { get; set; } = new();
    }

    /// <summary>
    /// Category definition for extensible ability categories.
    /// Supports adding new categories at runtime without code changes.
    /// </summary>
    public class CategoryDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public bool IsUnlockedByDefault { get; set; } = true;
        public string UnlockCondition { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// Manages ability category definitions in a data-driven way.
    /// Supports adding new categories at runtime.
    /// </summary>
    public class CategoryManager
    {
        private readonly Dictionary<string, CategoryDefinition> _categories = new(StringComparer.OrdinalIgnoreCase);

        public event Action<string>? OnCategoryRegistered;

        public CategoryManager()
        {
            RegisterDefaultCategories();
        }

        private void RegisterDefaultCategories()
        {
            Register(new CategoryDefinition
            {
                Id = "Melee", DisplayName = "Melee", Description = "Close-range physical attacks",
                SortOrder = 1, Tags = new() { "physical", "close-range" }
            });
            Register(new CategoryDefinition
            {
                Id = "Magic", DisplayName = "Magic", Description = "Arcane and elemental spells",
                SortOrder = 2, Tags = new() { "magical", "ranged" }
            });
            Register(new CategoryDefinition
            {
                Id = "Ranged", DisplayName = "Ranged", Description = "Ranged physical attacks",
                SortOrder = 3, Tags = new() { "physical", "ranged" }
            });
            Register(new CategoryDefinition
            {
                Id = "Movement", DisplayName = "Movement", Description = "Mobility and positioning abilities",
                SortOrder = 4, Tags = new() { "mobility" }
            });
            Register(new CategoryDefinition
            {
                Id = "Support", DisplayName = "Support", Description = "Buff and utility abilities",
                SortOrder = 5, Tags = new() { "utility", "buff" }
            });
            Register(new CategoryDefinition
            {
                Id = "Healing", DisplayName = "Healing", Description = "Restorative abilities",
                SortOrder = 6, Tags = new() { "healing", "restoration" }
            });
            Register(new CategoryDefinition
            {
                Id = "Defensive", DisplayName = "Defensive", Description = "Protection and mitigation abilities",
                SortOrder = 7, Tags = new() { "defense", "mitigation" }
            });
            Register(new CategoryDefinition
            {
                Id = "Summoning", DisplayName = "Summoning", Description = "Summon allies or entities",
                SortOrder = 8, Tags = new() { "summon", "pet" }
            });
            Register(new CategoryDefinition
            {
                Id = "Passive", DisplayName = "Passive", Description = "Always-active bonuses",
                SortOrder = 9, Tags = new() { "passive", "bonus" }
            });
            Register(new CategoryDefinition
            {
                Id = "Ultimate", DisplayName = "Ultimate", Description = "Powerful ultimate abilities",
                SortOrder = 10, IsUnlockedByDefault = false, UnlockCondition = "level_50",
                Tags = new() { "ultimate", "powerful" }
            });
            Register(new CategoryDefinition
            {
                Id = "Utility", DisplayName = "Utility", Description = "Miscellaneous utility abilities",
                SortOrder = 11, Tags = new() { "utility", "misc" }
            });
        }

        public void Register(CategoryDefinition category)
        {
            if (string.IsNullOrWhiteSpace(category.Id))
                throw new ArgumentException("Category ID must not be empty.");
            _categories[category.Id] = category;
            OnCategoryRegistered?.Invoke(category.Id);
        }

        public CategoryDefinition? Get(string id)
        {
            _categories.TryGetValue(id, out var def);
            return def;
        }

        public bool Contains(string id) => _categories.ContainsKey(id);

        public IReadOnlyCollection<CategoryDefinition> GetAll() => _categories.Values;

        public List<CategoryDefinition> GetUnlocked(int playerLevel)
        {
            var list = new List<CategoryDefinition>();
            foreach (var cat in _categories.Values)
            {
                if (cat.IsUnlockedByDefault || (!string.IsNullOrEmpty(cat.UnlockCondition) && playerLevel >= 50))
                    list.Add(cat);
            }
            return list;
        }

        public int Count => _categories.Count;
    }

    /// <summary>
    /// Extended ability data with full category, resource, and upgrade support.
    /// </summary>
    public class ExtendedAbilityData
    {
        // Identity
        public string AbilityId { get; set; } = string.Empty;
        public string InternalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Categorization
        public AbilityCategory Category { get; set; } = AbilityCategory.Melee;
        public AbilityTargetType TargetType { get; set; } = AbilityTargetType.SingleEnemy;
        public AbilityDamageType DamageType { get; set; } = AbilityDamageType.Physical;
        public AbilityExecutionType ExecutionType { get; set; } = AbilityExecutionType.Instant;
        public ResourceType PrimaryResource { get; set; } = ResourceType.Mana;
        public AbilityElement Element { get; set; } = AbilityElement.None;
        
        // Costs
        public float CooldownSec { get; set; } = 3.0f;
        public float ResourceCost { get; set; } = 0f;
        public float SecondaryResourceCost { get; set; } = 0f;
        public int MaxCharges { get; set; } = 1;
        public float ChargeRechargeSec { get; set; } = 0f;
        
        // Effect
        public float BaseDamage { get; set; } = 0f;
        public float BaseHealing { get; set; } = 0f;
        public float ShieldAmount { get; set; } = 0f;
        public float AoeRadius { get; set; } = 0f;
        public float Duration { get; set; } = 0f;
        public float CastTime { get; set; } = 0f;
        public float Range { get; set; } = 15f;
        public float TickInterval { get; set; } = 0f;
        
        // Unlock & Progression
        public int LevelRequired { get; set; } = 1;
        public string UnlockQuestId { get; set; } = string.Empty;
        public string RequiredAbilityId { get; set; } = string.Empty;
        public int RequiredAbilityLevel { get; set; } = 0;
        public List<AbilityUpgradePath> UpgradePaths { get; set; } = new();
        
        // VFX / SFX hooks
        public string VfxCastKey { get; set; } = string.Empty;
        public string VfxHitKey { get; set; } = string.Empty;
        public string VfxChannelKey { get; set; } = string.Empty;
        public string SfxCastKey { get; set; } = string.Empty;
        public string SfxHitKey { get; set; } = string.Empty;
        public string SfxChannelKey { get; set; } = string.Empty;
        
        // Animation
        public string AnimationTrigger { get; set; } = string.Empty;
        public string AnimationLayer { get; set; } = string.Empty;
        
        // Metadata
        public string IconPath { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string LocalizationKey { get; set; } = string.Empty;
        public int Version { get; set; } = 1;
        
        // DLC compatibility
        public string DlcId { get; set; } = string.Empty;
    }
}