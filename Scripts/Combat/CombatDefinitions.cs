using System;
using System.Collections.Generic;

namespace HeroOfEternia.Combat
{
    // ==========================================================
    // CORE ENUMERATIONS
    // ==========================================================

    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Holy,
        Shadow,
        True,       // Bypasses all resistances
        Healing     // Negative damage — restores health
    }

    public enum WeaponType
    {
        Unarmed,
        Sword,
        GreatSword,
        Axe,
        Hammer,
        Dagger,
        Spear,
        Bow,
        Crossbow,
        Staff,
        Wand,
        Shield,
        DualWield,    // Future
        Firearm       // Future
    }

    public enum TargetMode
    {
        Free,
        SoftLock,
        HardLock,
        Nearest,
        Manual
    }

    public enum HitVolumeShape
    {
        AABB,
        Sphere
    }

    public enum StatusEffectType
    {
        Burn,
        Freeze,
        Shock,
        Poison,
        Bleed,
        Slow,
        Stun,
        Silence,
        Knockback,
        Regeneration,
        Custom        // Data-driven extension point
    }

    public enum CombatEventType
    {
        AttackStarted,
        HitLanded,
        HitMissed,
        DamageDealt,
        DamageReceived,
        StatusApplied,
        StatusExpired,
        TargetChanged,
        TargetLost,
        ProjectileFired,
        ProjectileImpact,
        EntityDied,
        EntityRevived,
        BlockSucceeded,
        ParrySucceeded,
        CriticalHit
    }

    // ==========================================================
    // CORE VALUE RECORDS
    // ==========================================================

    /// <summary>
    /// Immutable description of a single damage application.
    /// Created at attack time and passed through the pipeline.
    /// </summary>
    public class DamageInstance
    {
        public string AttackerId { get; set; } = "";
        public string TargetId   { get; set; } = "";
        public float  BaseDamage { get; set; } = 0f;
        public DamageType Type   { get; set; } = DamageType.Physical;
        public float  CritChance { get; set; } = 0.05f;    // 5% default
        public float  CritMultiplier { get; set; } = 1.5f;
        public bool   IsCritical { get; set; } = false;    // Set by DamageSystem
        public string SourceWeaponId { get; set; } = "";
        public string SourceAbilityId { get; set; } = "";
        public List<StatusEffectType> AppliedEffects { get; set; } = new();
    }

    /// <summary>
    /// Result of a single hit detection check.
    /// </summary>
    public class HitResult
    {
        public bool   DidHit    { get; set; } = false;
        public string TargetId  { get; set; } = "";
        public float  Distance  { get; set; } = 0f;
        public float  HitPointX { get; set; } = 0f;
        public float  HitPointY { get; set; } = 0f;
        public float  HitPointZ { get; set; } = 0f;
    }

    /// <summary>
    /// Event record fired through the EventBus after any combat action.
    /// </summary>
    public class CombatEvent
    {
        public CombatEventType Type      { get; set; }
        public string          ActorId   { get; set; } = "";
        public string          TargetId  { get; set; } = "";
        public float           Value     { get; set; } = 0f;     // Damage, heal, etc.
        public string          MetaTag   { get; set; } = "";     // e.g. weapon id, effect type
        public double          Timestamp { get; set; } = 0.0;
    }
}
