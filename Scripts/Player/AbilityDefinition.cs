using System;
using System.Collections.Generic;

namespace HeroOfEternia.Player
{
    // ----------------------------------------------------------------
    // Ability target modes
    // ----------------------------------------------------------------
    public enum AbilityTargetType
    {
        Self,          // Applied to the caster (e.g. Barrier)
        SingleEnemy,   // Requires a locked target (e.g. Power Strike)
        AoE,           // Area around player (e.g. Arrow Rain)
        Projectile,    // Fires a projectile (e.g. Fireball)
        Directional    // In facing direction (e.g. Dodge Roll)
    }

    // ----------------------------------------------------------------
    // Ability damage type
    // ----------------------------------------------------------------
    public enum AbilityDamageType
    {
        None,
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Holy,
        Shadow
    }

    // ----------------------------------------------------------------
    // Ability element type
    // ----------------------------------------------------------------
    public enum AbilityElement
    {
        None,
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Holy,
        Shadow,
        Arcane,
        Nature,
        Wind,
        Earth,
        Water
    }

    // ----------------------------------------------------------------
    // AbilityData — pure data record loaded from JSON
    // ----------------------------------------------------------------
    public record AbilityData
    {
        // Identity
        public string AbilityId      { get; init; } = string.Empty;
        public string InternalName   { get; init; } = string.Empty;
        public string DisplayName    { get; init; } = string.Empty;
        public string Description    { get; init; } = string.Empty;

        // Categorization
        public string Category       { get; init; } = "Melee";
        public string AbilityType    { get; init; } = "Instant";
        public AbilityTargetType  TargetType  { get; init; } = AbilityTargetType.SingleEnemy;
        public AbilityDamageType  DamageType  { get; init; } = AbilityDamageType.Physical;
        public AbilityElement     Element     { get; init; } = AbilityElement.None;

        // Animation / Audio / VFX references
        public string AnimationReference { get; init; } = string.Empty;
        public string AudioReference     { get; init; } = string.Empty;
        public string VisualEffectReference { get; init; } = string.Empty;

        // Costs
        public float CooldownSec   { get; init; } = 3.0f;
        public float CastTime      { get; init; } = 0f;
        public float ManaCost      { get; init; } = 0f;
        public float StaminaCost   { get; init; } = 0f;
        public float EnergyCost    { get; init; } = 0f;
        public float FocusCost     { get; init; } = 0f;
        public float RageCost      { get; init; } = 0f;
        public float SpiritCost    { get; init; } = 0f;
        public float HealthCost    { get; init; } = 0f;

        // Effect
        public float BaseDamage    { get; init; } = 0f;
        public float BaseHealing   { get; init; } = 0f;
        public float ShieldAmount  { get; init; } = 0f;
        public float AoeRadius     { get; init; } = 0f;   // 0 = not AoE
        public float Duration      { get; init; } = 0f;   // 0 = instant
        public float Range         { get; init; } = 15f;
        public int   MaxCharges    { get; init; } = 1;
        public float ChargeRechargeSec { get; init; } = 0f;

        // Unlock gate
        public int   LevelRequired { get; init; } = 1;
        public string UnlockRequirement { get; init; } = string.Empty;
        public string UpgradePathHook   { get; init; } = string.Empty;

        // VFX / SFX hooks
        public string VfxCastKey   { get; init; } = string.Empty;
        public string VfxHitKey    { get; init; } = string.Empty;
        public string VfxChannelKey { get; init; } = string.Empty;
        public string SfxCastKey   { get; init; } = string.Empty;
        public string SfxHitKey    { get; init; } = string.Empty;
        public string SfxChannelKey { get; init; } = string.Empty;

        // Metadata
        public string IconPath     { get; init; } = string.Empty;
        public string LocalizationKey { get; init; } = string.Empty;
        public int    Version      { get; init; } = 1;
        public string DlcId        { get; init; } = string.Empty;
        public List<string> Tags   { get; init; } = new();
    }

    // ----------------------------------------------------------------
    // AbilityDefinition — wraps AbilityData with runtime helpers
    // ----------------------------------------------------------------
    public class AbilityDefinition
    {
        public AbilityData Data { get; }

        public AbilityDefinition(AbilityData data)
        {
            if (string.IsNullOrWhiteSpace(data.AbilityId))
                throw new ArgumentException("AbilityData.AbilityId must not be empty.");
            if (data.CooldownSec < 0)
                throw new ArgumentOutOfRangeException(nameof(data.CooldownSec), "Cooldown must be >= 0.");
            Data = data;
        }

        public bool IsInstant     => Data.Duration <= 0f && Data.CastTime <= 0f;
        public bool IsAoE         => Data.TargetType == AbilityTargetType.AoE;
        public bool IsProjectile  => Data.TargetType == AbilityTargetType.Projectile;
        public bool IsSelfCast    => Data.TargetType == AbilityTargetType.Self;
        public bool DoesDamage    => Data.BaseDamage > 0f;
        public bool DoesHeal      => Data.BaseHealing > 0f;
        public bool HasShield     => Data.ShieldAmount > 0f;
        public bool HasCharges    => Data.MaxCharges > 1;
        public bool HasCastTime   => Data.CastTime > 0f;

        /// <summary>True when player meets level requirement for this ability.</summary>
        public bool IsUnlocked(int playerLevel) => playerLevel >= Data.LevelRequired;

        /// <summary>Gets the total resource cost for a given resource type.</summary>
        public float GetResourceCost(string resourceType)
        {
            return resourceType.ToLowerInvariant() switch
            {
                "mana" => Data.ManaCost,
                "stamina" => Data.StaminaCost,
                "energy" => Data.EnergyCost,
                "focus" => Data.FocusCost,
                "rage" => Data.RageCost,
                "spirit" => Data.SpiritCost,
                "health" => Data.HealthCost,
                _ => 0f
            };
        }

        public override string ToString() =>
            $"[Ability:{Data.AbilityId}] CD={Data.CooldownSec}s Cast={Data.CastTime}s Dmg={Data.BaseDamage} Heal={Data.BaseHealing}";
    }
}