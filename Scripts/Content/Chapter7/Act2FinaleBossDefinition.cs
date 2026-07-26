using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter7
{
    public class FinaleBossAbilityDefinition
    {
        public string AbilityId { get; set; } = "";
        public string Name { get; set; } = "";
        public float BaseDamage { get; set; } = 80f;
        public float CooldownSeconds { get; set; } = 10f;
        public string EffectVfxKey { get; set; } = "";
        public string AudioSfxKey { get; set; } = "";
    }

    /// <summary>
    /// Definition for Shadow Lord Malakor Emissary, the epic finale boss encounter concluding Act II.
    /// Features 3 combat phases (Rift Phase, Shadow Army Phase, Devastation Cataclysm) and 4,200 HP pool.
    /// </summary>
    public class Act2FinaleBossDefinition
    {
        public string BossId { get; } = "enemy_boss_malakor_emissary";
        public string DisplayName { get; } = "Malakor's Harbinger: Shadow Lord Emissary";
        public int RecommendedLevel { get; } = 30;
        public float MaxHealth { get; } = 4200f;
        public float ArmorMitigation { get; } = 0.40f;

        public List<FinaleBossAbilityDefinition> Abilities { get; private set; } = new();

        public Act2FinaleBossDefinition()
        {
            InitializeAbilities();
        }

        public void InitializeAbilities()
        {
            Abilities = new List<FinaleBossAbilityDefinition>
            {
                new FinaleBossAbilityDefinition
                {
                    AbilityId = "ability_malakor_shadow_devastation",
                    Name = "Shadow Devastation Nova",
                    BaseDamage = 120f,
                    CooldownSeconds = 8f,
                    EffectVfxKey = "vfx_shadow_devastation",
                    AudioSfxKey = "sfx_shadow_devastation_blast"
                },
                new FinaleBossAbilityDefinition
                {
                    AbilityId = "ability_malakor_void_rift",
                    Name = "Void Rift Tear",
                    BaseDamage = 95f,
                    CooldownSeconds = 12f,
                    EffectVfxKey = "vfx_void_rift_tear",
                    AudioSfxKey = "sfx_rift_tear_open"
                },
                new FinaleBossAbilityDefinition
                {
                    AbilityId = "ability_malakor_shadow_army",
                    Name = "Army of Shadows Summon",
                    BaseDamage = 50f,
                    CooldownSeconds = 25f,
                    EffectVfxKey = "vfx_army_shadows_summon",
                    AudioSfxKey = "sfx_army_summon_roar"
                },
                new FinaleBossAbilityDefinition
                {
                    AbilityId = "ability_malakor_oblivion_slam",
                    Name = "Cataclysmic Oblivion Slam",
                    BaseDamage = 200f,
                    CooldownSeconds = 30f,
                    EffectVfxKey = "vfx_oblivion_slam_pulse",
                    AudioSfxKey = "sfx_oblivion_impact"
                }
            };
        }
    }
}
