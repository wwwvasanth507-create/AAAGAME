using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player.Progression
{
    /// <summary>
    /// Represents the player's level, experience, and stat growth curves.
    /// All values are data-driven and support future prestige/seasonal systems.
    /// </summary>
    public class PlayerProgression
    {
        // ----------------------------------------------------------------
        // Events
        // ----------------------------------------------------------------
        public event Action<int, int>? OnLevelUp;           // (newLevel, previousLevel)
        public event Action<float, float>? OnExperienceGained;  // (currentXP, xpForNextLevel)
        public event Action<int>? OnPrestigeChanged;         // (prestigeLevel)

        // ----------------------------------------------------------------
        // Progression State
        // ----------------------------------------------------------------
        public int Level { get; private set; } = 1;
        public int PrestigeLevel { get; private set; } = 0;
        public float Experience { get; private set; } = 0f;
        public int TotalLevelsGained => Level + (PrestigeLevel * MaxLevel);

        // ----------------------------------------------------------------
        // Configuration
        // ----------------------------------------------------------------
        public const int MaxLevel = 100;
        public const int MaxPrestigeLevel = 10;
        public const float BaseXPRequirement = 100f;
        public const float XPGrowthFactor = 1.15f;  // 15% more XP per level

        // Stat growth multipliers per level
        public float HealthPerLevel { get; set; } = 20f;
        public float ManaPerLevel { get; set; } = 10f;
        public float StaminaPerLevel { get; set; } = 5f;
        public float AttackPerLevel { get; set; } = 3f;
        public float DefensePerLevel { get; set; } = 2f;
        public float MagicPerLevel { get; set; } = 3f;

        // Prestige bonuses (multiplied by prestige level)
        public float PrestigeDamageMultiplier { get; set; } = 0.05f;  // 5% per prestige
        public float PrestigeHealthMultiplier { get; set; } = 0.03f;  // 3% per prestige

        // ----------------------------------------------------------------
        // XP Calculation
        // ----------------------------------------------------------------
        public float GetXPRequiredForLevel(int level)
        {
            if (level <= 1) return 0f;
            return BaseXPRequirement * MathF.Pow(XPGrowthFactor, level - 1);
        }

        public float XPForNextLevel => GetXPRequiredForLevel(Level + 1);
        public float XPProgressPercent => XPForNextLevel > 0 
            ? MathF.Min(100f, (Experience / XPForNextLevel) * 100f) 
            : 0f;
        public float TotalXPRequired => GetXPRequiredForLevel(Math.Min(Level + 1, MaxLevel));

        // ----------------------------------------------------------------
        // Stat Calculation
        // ----------------------------------------------------------------
        public float GetBaseHealth() => 100f + (HealthPerLevel * (Level - 1)) * (1f + PrestigeLevel * PrestigeHealthMultiplier);
        public float GetBaseMana() => 50f + ManaPerLevel * (Level - 1);
        public float GetBaseStamina() => 50f + StaminaPerLevel * (Level - 1);
        public float GetBaseAttack() => 10f + AttackPerLevel * (Level - 1);
        public float GetBaseDefense() => 5f + DefensePerLevel * (Level - 1);
        public float GetBaseMagic() => 10f + MagicPerLevel * (Level - 1);
        public float GetDamageMultiplier() => 1f + PrestigeLevel * PrestigeDamageMultiplier;

        // ----------------------------------------------------------------
        // XP Management
        // ----------------------------------------------------------------
        public void AddExperience(float amount)
        {
            if (amount <= 0f) return;
            
            Experience += amount;
            OnExperienceGained?.Invoke(Experience, XPForNextLevel);

            // Check for level ups
            int levelsGained = 0;
            while (Experience >= XPForNextLevel && Level < MaxLevel)
            {
                Experience -= XPForNextLevel;
                Level++;
                levelsGained++;
            }

            if (levelsGained > 0)
            {
                OnLevelUp?.Invoke(Level, Level - levelsGained);
                Logger.Info($"PlayerProgression: Level up! Now level {Level} (+{levelsGained} levels)");
            }

            // Handle max level overflow
            if (Level >= MaxLevel)
            {
                Experience = MathF.Min(Experience, XPForNextLevel);
            }
        }

        public bool TryPrestige()
        {
            if (Level < MaxLevel)
            {
                Logger.Warning($"PlayerProgression: Cannot prestige until level {MaxLevel}. Current: {Level}");
                return false;
            }

            if (PrestigeLevel >= MaxPrestigeLevel)
            {
                Logger.Warning($"PlayerProgression: Maximum prestige level ({MaxPrestigeLevel}) reached.");
                return false;
            }

            PrestigeLevel++;
            Level = 1;
            Experience = 0f;
            OnPrestigeChanged?.Invoke(PrestigeLevel);
            Logger.Info($"PlayerProgression: Prestige! Now prestige level {PrestigeLevel}.");

            // Grant prestige rewards via event
            OnLevelUp?.Invoke(Level, 1);
            return true;
        }

        // ----------------------------------------------------------------
        // Save/Load
        // ----------------------------------------------------------------
        public ProgressionSaveData CreateSaveData()
        {
            return new ProgressionSaveData
            {
                Level = Level,
                PrestigeLevel = PrestigeLevel,
                Experience = Experience,
                Version = 1
            };
        }

        public void LoadFromSaveData(ProgressionSaveData data)
        {
            if (data == null) return;
            
            // Version migration support
            Level = Math.Clamp(data.Level, 1, MaxLevel);
            PrestigeLevel = Math.Clamp(data.PrestigeLevel, 0, MaxPrestigeLevel);
            Experience = MathF.Max(0, data.Experience);
        }

        public override string ToString() =>
            $"Lv.{Level} ({XPProgressPercent:F1}%) XP:{Experience:F0}/{XPForNextLevel:F0} Prestige:{PrestigeLevel}";
    }

    /// <summary>
    /// Save data for player progression (versioned for migration).
    /// </summary>
    public class ProgressionSaveData
    {
        public int Level { get; set; } = 1;
        public int PrestigeLevel { get; set; } = 0;
        public float Experience { get; set; } = 0f;
        public int Version { get; set; } = 1;
    }
}