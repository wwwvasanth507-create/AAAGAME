using System;
using System.Collections.Generic;

namespace HeroOfEternia.Enemies
{
    // ----------------------------------------------------------------
    // Enemy AI behaviour profile
    // ----------------------------------------------------------------
    public enum EnemyBehaviour
    {
        Passive,      // Only attacks when attacked first
        Aggressive,   // Attacks on sight within aggro range
        Patrol,       // Patrols waypoints, attacks on sight
        Guard,        // Stays at position, attacks if player enters range
        Ambush        // Hidden until player is very close, then bursts
    }

    // ----------------------------------------------------------------
    // Enemy elemental profile
    // ----------------------------------------------------------------
    public enum EnemyElement
    {
        None,
        Fire,
        Ice,
        Lightning,
        Poison,
        Shadow,
        Holy
    }

    // ----------------------------------------------------------------
    // EnemyData — data-only record loaded from enemy_database.json
    // ----------------------------------------------------------------
    public record EnemyData
    {
        // Identity
        public string EnemyId      { get; init; } = string.Empty;
        public string DisplayName  { get; init; } = string.Empty;
        public string Species      { get; init; } = string.Empty;
        public string Description  { get; init; } = string.Empty;

        // Base stats
        public float  MaxHp           { get; init; } = 50f;
        public float  MoveSpeed       { get; init; } = 3.5f;
        public float  AttackDamage    { get; init; } = 8f;
        public float  AttackRange     { get; init; } = 2.0f;
        public float  AggroRange      { get; init; } = 10.0f;
        public float  AttackCooldown  { get; init; } = 1.5f;
        public float  Defense         { get; init; } = 0f;

        // Experience & loot
        public int    XpReward        { get; init; } = 10;
        public string LootTableId     { get; init; } = string.Empty;

        // Behaviour
        public EnemyBehaviour Behaviour { get; init; } = EnemyBehaviour.Aggressive;

        // Element
        public EnemyElement Element    { get; init; } = EnemyElement.None;

        // Resistances & weaknesses — element name → multiplier (0.5 = resist, 2.0 = weak)
        public Dictionary<string, float> Resistances  { get; init; } = new();
        public Dictionary<string, float> Weaknesses   { get; init; } = new();

        // VFX / audio hooks
        public string VfxHitKey        { get; init; } = string.Empty;
        public string VfxDeathKey      { get; init; } = string.Empty;
        public string SfxAggroKey      { get; init; } = string.Empty;
        public string SfxAttackKey     { get; init; } = string.Empty;
        public string SfxDeathKey      { get; init; } = string.Empty;

        // Asset
        public string ModelPath        { get; init; } = string.Empty;
        public int    PolyBudget       { get; init; } = 2000;

        // Scaling — applied at runtime per difficulty
        public float HpScaleFactor     { get; init; } = 1.0f;
        public float DamageScaleFactor { get; init; } = 1.0f;
    }

    // ----------------------------------------------------------------
    // EnemyDefinition — wraps EnemyData with runtime helpers
    // ----------------------------------------------------------------
    public class EnemyDefinition
    {
        public EnemyData Data { get; }

        public EnemyDefinition(EnemyData data)
        {
            if (string.IsNullOrWhiteSpace(data.EnemyId))
                throw new ArgumentException("EnemyData.EnemyId must not be empty.");
            if (data.MaxHp <= 0)
                throw new ArgumentOutOfRangeException(nameof(data.MaxHp), "MaxHp must be > 0.");
            if (data.AttackRange <= 0)
                throw new ArgumentOutOfRangeException(nameof(data.AttackRange), "AttackRange must be > 0.");

            Data = data;
        }

        /// <summary>Returns true when this enemy can aggro players from afar.</summary>
        public bool IsAggressive => Data.Behaviour == EnemyBehaviour.Aggressive
                                 || Data.Behaviour == EnemyBehaviour.Ambush;

        /// <summary>Applies wave difficulty scaling — returns a modified EnemyData.</summary>
        public EnemyData GetScaledData(int waveIndex)
        {
            float scale = 1f + (waveIndex - 1) * 0.15f;  // +15% per wave
            return Data with
            {
                MaxHp        = MathF.Round(Data.MaxHp        * Data.HpScaleFactor     * scale, 1),
                AttackDamage = MathF.Round(Data.AttackDamage * Data.DamageScaleFactor * scale, 1)
            };
        }

        /// <summary>Returns the damage multiplier for a given attacker element.</summary>
        public float GetDamageMultiplier(string attackerElement)
        {
            string key = attackerElement.ToLowerInvariant();
            if (Data.Resistances.TryGetValue(key, out float resist)) return resist;
            if (Data.Weaknesses.TryGetValue(key,  out float weak))   return weak;
            return 1.0f;
        }

        public override string ToString() =>
            $"[Enemy:{Data.EnemyId}] HP={Data.MaxHp} SPD={Data.MoveSpeed} DMG={Data.AttackDamage}";
    }
}
