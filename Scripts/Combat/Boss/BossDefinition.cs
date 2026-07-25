using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroOfEternia.Combat
{
    public enum BossClass
    {
        Guardian,
        Behemoth,
        Mage,
        Summoner,
        Stalker
    }

    public enum SpecialAttackType
    {
        MeleeCombo,
        AreaOfEffect,
        ProjectilePattern,
        SummonHook,
        MovementCharge,
        BeamAttack,
        GroundHazard
    }

    public record SpecialAttackData
    {
        public string AttackId { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public SpecialAttackType AttackType { get; init; } = SpecialAttackType.MeleeCombo;
        public float BaseDamage { get; init; } = 20f;
        public float Range { get; init; } = 5f;
        public float Cooldown { get; init; } = 5f;
        public float CastTime { get; init; } = 1.5f;
        public float AoeRadius { get; init; } = 0f;
        public int ProjectileCount { get; init; } = 0;
        public string VfxCastKey { get; init; } = string.Empty;
        public string SfxCastKey { get; init; } = string.Empty;
    }

    public record BossData
    {
        public string BossId { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public BossClass Class { get; init; } = BossClass.Guardian;
        public string ArenaId { get; init; } = string.Empty;
        public string Biome { get; init; } = string.Empty;
        public int MinLevel { get; init; } = 10;
        public int MaxLevel { get; init; } = 50;
        public float MaxHp { get; init; } = 500f;
        public float MaxShield { get; init; } = 100f;
        public float Armor { get; init; } = 30f;
        public float MoveSpeed { get; init; } = 5.0f;
        public string Element { get; init; } = string.Empty;
        public List<string> Weaknesses { get; init; } = new();
        public List<string> Resistances { get; init; } = new();
        public string LootTableId { get; init; } = string.Empty;
        public string MusicProfileId { get; init; } = string.Empty;
        public string VfxProfileId { get; init; } = string.Empty;
        public string VoiceProfileId { get; init; } = string.Empty;
        public string CameraProfileId { get; init; } = string.Empty;
        public string RewardProfileId { get; init; } = string.Empty;
        public List<SpecialAttackData> SpecialAttacks { get; init; } = new();
        public List<BossPhaseData> Phases { get; init; } = new();

        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; init; } = new();
    }

    public record BossPhaseData
    {
        public int PhaseIndex { get; init; } = 1;
        public float HpThresholdPct { get; init; } = 1.0f; // e.g. 0.5f triggers at 50% HP
        public string BehaviorModifier { get; init; } = string.Empty;
        public float SpeedMultiplier { get; init; } = 1.0f;
        public float DamageMultiplier { get; init; } = 1.0f;
        public string VfxTriggerKey { get; init; } = string.Empty;
        public string SfxTriggerKey { get; init; } = string.Empty;
        public List<string> PhaseSpecialAttackIds { get; init; } = new();
    }

    public class BossDefinition
    {
        public BossData Data { get; }

        public BossDefinition(BossData data)
        {
            if (string.IsNullOrWhiteSpace(data.BossId))
                throw new ArgumentException("BossId cannot be empty.");
            if (data.MaxHp <= 0f)
                throw new ArgumentOutOfRangeException(nameof(data.MaxHp), "MaxHp must be positive.");
            
            Data = data;
        }

        public float GetDamageMultiplierForElement(string element)
        {
            if (Data.Weaknesses.Contains(element)) return 1.5f;
            if (Data.Resistances.Contains(element)) return 0.5f;
            return 1.0f;
        }
    }
}
