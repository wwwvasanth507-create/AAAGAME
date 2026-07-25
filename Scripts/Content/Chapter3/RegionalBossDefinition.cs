using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter3
{
    public enum BossPhase
    {
        Intro,
        PhaseOne,
        PhaseTwo,
        PhaseThree,
        Defeated
    }

    public class BossAbility
    {
        public string AbilityId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int BaseDamage { get; set; }
        public float CooldownSeconds { get; set; }
        public bool IsTelegraphed { get; set; } = true;
        public BossPhase UnlocksAtPhase { get; set; } = BossPhase.PhaseOne;
    }

    /// <summary>
    /// Data-driven boss profile and phase controller for Commander Vareth the Void Knight
    /// — Act I regional boss encounter with three distinct combat phases.
    /// </summary>
    public class RegionalBossDefinition
    {
        public string BossId { get; set; } = "boss_commander_vareth";
        public string DisplayName { get; set; } = "Commander Vareth — Void Knight";
        public int Level { get; set; } = 18;
        public int MaxHp { get; set; } = 1800;
        public BossPhase CurrentPhase { get; private set; } = BossPhase.Intro;

        // Phase HP thresholds (as percentage)
        public float Phase2HpThreshold { get; set; } = 0.65f;
        public float Phase3HpThreshold { get; set; } = 0.30f;

        private readonly List<BossAbility> _abilities = new();

        public void RegisterAbilities()
        {
            _abilities.Add(new BossAbility
            {
                AbilityId = "ability_void_slash",
                DisplayName = "Void Slash",
                BaseDamage = 55,
                CooldownSeconds = 3.5f,
                UnlocksAtPhase = BossPhase.PhaseOne
            });

            _abilities.Add(new BossAbility
            {
                AbilityId = "ability_shadow_bolt",
                DisplayName = "Shadow Bolt Barrage",
                BaseDamage = 35,
                CooldownSeconds = 6.0f,
                UnlocksAtPhase = BossPhase.PhaseOne
            });

            _abilities.Add(new BossAbility
            {
                AbilityId = "ability_void_surge",
                DisplayName = "Void Surge Wave",
                BaseDamage = 80,
                CooldownSeconds = 12.0f,
                UnlocksAtPhase = BossPhase.PhaseTwo
            });

            _abilities.Add(new BossAbility
            {
                AbilityId = "ability_void_gate_summon",
                DisplayName = "Void Gate Summon",
                BaseDamage = 0,
                CooldownSeconds = 20.0f,
                UnlocksAtPhase = BossPhase.PhaseThree
            });

            _abilities.Add(new BossAbility
            {
                AbilityId = "ability_desperate_voidstrike",
                DisplayName = "Desperate Voidstrike",
                BaseDamage = 120,
                CooldownSeconds = 25.0f,
                UnlocksAtPhase = BossPhase.PhaseThree
            });
        }

        public void AdvancePhase(BossPhase phase)
        {
            if (phase > CurrentPhase)
            {
                CurrentPhase = phase;
                Logger.Info($"RegionalBossDefinition: Boss advanced to phase '{phase}'");
            }
        }

        public IReadOnlyList<BossAbility> AllAbilities => _abilities;
    }
}
