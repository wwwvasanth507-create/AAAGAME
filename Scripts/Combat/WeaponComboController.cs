using Godot;
using System;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    public enum ComboStep
    {
        Idle = 0,
        Hit1 = 1,
        Hit2 = 2,
        Hit3HeavyFinisher = 3
    }

    public partial class WeaponComboController : Node, IInitializable
    {
        private static WeaponComboController? _instance;
        public static WeaponComboController Instance => _instance ??= new WeaponComboController();

        public WeaponSlot ActiveSlot { get; private set; } = WeaponSlot.Primary;
        public ComboStep CurrentStep { get; private set; } = ComboStep.Idle;

        private float _comboWindowTimer = 0f;
        private const float COMBO_WINDOW_MAX = 1.2f;

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            CurrentStep = ComboStep.Idle;
            ActiveSlot = WeaponSlot.Primary;
            GD.Print("[WeaponComboController] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public override void _Process(double delta)
        {
            if (CurrentStep == ComboStep.Idle) return;

            _comboWindowTimer -= (float)delta;
            if (_comboWindowTimer <= 0f)
            {
                ResetCombo();
            }
        }

        /// <summary>
        /// Toggles between Primary and Secondary weapon slots.
        /// </summary>
        public void SwitchWeapon()
        {
            ActiveSlot = (ActiveSlot == WeaponSlot.Primary) ? WeaponSlot.Secondary : WeaponSlot.Primary;
            ResetCombo();
            EventBus.Publish(ActiveSlot);
            GD.Print($"[WeaponComboController] Switched to {ActiveSlot} weapon.");
        }

        /// <summary>
        /// Registers an attack input and advances the combo chain.
        /// </summary>
        public ComboStep RegisterAttackInput(out float damageMultiplier)
        {
            _comboWindowTimer = COMBO_WINDOW_MAX;

            CurrentStep = CurrentStep switch
            {
                ComboStep.Idle => ComboStep.Hit1,
                ComboStep.Hit1 => ComboStep.Hit2,
                ComboStep.Hit2 => ComboStep.Hit3HeavyFinisher,
                ComboStep.Hit3HeavyFinisher => ComboStep.Hit1,
                _ => ComboStep.Hit1
            };

            damageMultiplier = CurrentStep switch
            {
                ComboStep.Hit1 => 1.0f,
                ComboStep.Hit2 => 1.25f,
                ComboStep.Hit3HeavyFinisher => 1.8f,
                _ => 1.0f
            };

            EventBus.Publish(CurrentStep);

            if (CurrentStep == ComboStep.Hit3HeavyFinisher)
            {
                // Auto reset combo after heavy finisher
                _comboWindowTimer = 0.3f;
            }

            return CurrentStep;
        }

        public void ResetCombo()
        {
            CurrentStep = ComboStep.Idle;
            _comboWindowTimer = 0f;
        }
    }
}
