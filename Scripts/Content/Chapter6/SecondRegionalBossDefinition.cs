using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter6
{
    public class BossAbilityDefinition
    {
        public string AbilityId { get; set; } = "";
        public string Name { get; set; } = "";
        public float BaseDamage { get; set; } = 50f;
        public float CooldownSeconds { get; set; } = 8f;
        public string EffectVfxKey { get; set; } = "";
        public string AudioSfxKey { get; set; } = "";
    }

    /// <summary>
    /// Definition for High Inquisitor Vesper, the second major regional boss encounter of Act II.
    /// Features 3 combat phases (Barrier Stage, Void Flame Barrage, Desperate Cataclysm) and 2,800 HP pool.
    /// </summary>
    public class SecondRegionalBossDefinition
    {
        public string BossId { get; } = "enemy_boss_high_inquisitor_vesper";
        public string DisplayName { get; } = "High Inquisitor Vesper";
        public int RecommendedLevel { get; } = 27;
        public float MaxHealth { get; } = 2800f;
        public float ArmorMitigation { get; } = 0.35f;

        public List<BossAbilityDefinition> Abilities { get; private set; } = new();

        public SecondRegionalBossDefinition()
        {
            InitializeAbilities();
        }

        public void InitializeAbilities()
        {
            Abilities = new List<BossAbilityDefinition>
            {
                new BossAbilityDefinition
                {
                    AbilityId = "ability_vesper_void_nova",
                    Name = "Void Flame Nova",
                    BaseDamage = 85f,
                    CooldownSeconds = 6f,
                    EffectVfxKey = "vfx_void_nova",
                    AudioSfxKey = "sfx_void_nova_cast"
                },
                new BossAbilityDefinition
                {
                    AbilityId = "ability_vesper_barrier",
                    Name = "Arcane Barrier Shield",
                    BaseDamage = 0f,
                    CooldownSeconds = 15f,
                    EffectVfxKey = "vfx_arcane_shield",
                    AudioSfxKey = "sfx_shield_activate"
                },
                new BossAbilityDefinition
                {
                    AbilityId = "ability_vesper_summons",
                    Name = "Shadow Construct Summon",
                    BaseDamage = 40f,
                    CooldownSeconds = 20f,
                    EffectVfxKey = "vfx_shadow_summon",
                    AudioSfxKey = "sfx_summon_roar"
                },
                new BossAbilityDefinition
                {
                    AbilityId = "ability_vesper_cataclysm",
                    Name = "Void Cataclysm",
                    BaseDamage = 140f,
                    CooldownSeconds = 25f,
                    EffectVfxKey = "vfx_cataclysm_pulse",
                    AudioSfxKey = "sfx_cataclysm_explosion"
                }
            };
        }
    }
}
