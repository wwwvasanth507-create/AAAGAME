using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter9
{
    public class CommanderAbilityDefinition
    {
        public string AbilityId { get; set; } = "";
        public string Name { get; set; } = "";
        public float BaseDamage { get; set; } = 90f;
        public float CooldownSeconds { get; set; } = 8f;
        public string EffectVfxKey { get; set; } = "";
        public string AudioSfxKey { get; set; } = "";
    }

    /// <summary>
    /// Definition for General Vaelis the Unforgiving, the high commander boss of the Fortress of Obsidian Shadows.
    /// Features 3 combat phases (Vanguard Strike, Void Wall Stage, Desperate Reckoning) and 3,600 HP pool.
    /// </summary>
    public class FortressCommanderBossDefinition
    {
        public string BossId { get; } = "enemy_boss_general_vaelis";
        public string DisplayName { get; } = "General Vaelis the Unforgiving";
        public int RecommendedLevel { get; } = 36;
        public float MaxHealth { get; } = 3600f;
        public float ArmorMitigation { get; } = 0.38f;

        public List<CommanderAbilityDefinition> Abilities { get; private set; } = new();

        public FortressCommanderBossDefinition()
        {
            InitializeAbilities();
        }

        public void InitializeAbilities()
        {
            Abilities = new List<CommanderAbilityDefinition>
            {
                new CommanderAbilityDefinition
                {
                    AbilityId = "ability_vaelis_shadow_cleave",
                    Name = "Shadow Cleave",
                    BaseDamage = 110f,
                    CooldownSeconds = 6f,
                    EffectVfxKey = "vfx_shadow_cleave",
                    AudioSfxKey = "sfx_heavy_sword_swing"
                },
                new CommanderAbilityDefinition
                {
                    AbilityId = "ability_vaelis_void_shield_wall",
                    Name = "Void Shield Wall",
                    BaseDamage = 0f,
                    CooldownSeconds = 18f,
                    EffectVfxKey = "vfx_void_shield_wall",
                    AudioSfxKey = "sfx_shield_wall_deploy"
                },
                new CommanderAbilityDefinition
                {
                    AbilityId = "ability_vaelis_legion_rally",
                    Name = "Legion Rally Call",
                    BaseDamage = 45f,
                    CooldownSeconds = 22f,
                    EffectVfxKey = "vfx_legion_rally",
                    AudioSfxKey = "sfx_war_horn_blast"
                },
                new CommanderAbilityDefinition
                {
                    AbilityId = "ability_vaelis_cataclysmic_slam",
                    Name = "Cataclysmic Ground Slam",
                    BaseDamage = 175f,
                    CooldownSeconds = 28f,
                    EffectVfxKey = "vfx_ground_slam_waves",
                    AudioSfxKey = "sfx_cataclysmic_impact"
                }
            };
        }
    }
}
