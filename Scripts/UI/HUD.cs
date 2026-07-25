using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Combat;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI
{
    /// <summary>
    /// HUD manages all in-game heads-up display elements:
    /// health bar, stamina bar, weapon label, interact prompt,
    /// combo counter, and optional boss health bar.
    ///
    /// Listens to EventBus events to update automatically without
    /// coupling to the Player or CombatManager directly.
    /// </summary>
    public partial class HUD : CanvasLayer
    {
        // ----------------------------------------------------------------
        // Node references (set via Export or found in _Ready)
        // ----------------------------------------------------------------
        [Export] public ProgressBar? HealthBar       { get; set; }
        [Export] public ProgressBar? StaminaBar      { get; set; }
        [Export] public Label?       HealthLabel     { get; set; }
        [Export] public Label?       StaminaLabel    { get; set; }
        [Export] public Label?       WeaponLabel     { get; set; }
        [Export] public Label?       InteractPrompt  { get; set; }
        [Export] public Label?       ComboLabel      { get; set; }
        [Export] public Control?     BossHpPanel     { get; set; }
        [Export] public ProgressBar? BossHealthBar   { get; set; }
        [Export] public Label?       BossNameLabel   { get; set; }
        [Export] public Label?       WaveLabel       { get; set; }

        // ----------------------------------------------------------------
        // Runtime state
        // ----------------------------------------------------------------
        private float _currentHp    = 100f;
        private float _maxHp        = 100f;
        private float _currentStam  = 100f;
        private float _maxStam      = 100f;
        private int   _combo        = 0;
        private float _comboResetTimer = 0f;
        private const float ComboResetDelay = 3.0f;

        // ----------------------------------------------------------------
        // Lifecycle
        // ----------------------------------------------------------------
        public override void _Ready()
        {
            // Subscribe to events
            EventBus.Subscribe<HudHealthChangedEvent>(OnHealthChanged);
            EventBus.Subscribe<HudStaminaChangedEvent>(OnStaminaChanged);
            EventBus.Subscribe<HudWeaponChangedEvent>(OnWeaponChanged);
            EventBus.Subscribe<HudInteractPromptEvent>(OnInteractPrompt);
            EventBus.Subscribe<HudComboHitEvent>(OnComboHit);
            EventBus.Subscribe<HudBossSpawnedEvent>(OnBossSpawned);
            EventBus.Subscribe<HudBossHpChangedEvent>(OnBossHpChanged);
            EventBus.Subscribe<HudWaveChangedEvent>(OnWaveChanged);

            // Initial state
            SetHealth(_currentHp, _maxHp);
            SetStamina(_currentStam, _maxStam);
            HideInteractPrompt();
            HideBossPanel();

            GD.Print("HUD: Initialised successfully.");
        }

        public override void _ExitTree()
        {
            EventBus.Unsubscribe<HudHealthChangedEvent>(OnHealthChanged);
            EventBus.Unsubscribe<HudStaminaChangedEvent>(OnStaminaChanged);
            EventBus.Unsubscribe<HudWeaponChangedEvent>(OnWeaponChanged);
            EventBus.Unsubscribe<HudInteractPromptEvent>(OnInteractPrompt);
            EventBus.Unsubscribe<HudComboHitEvent>(OnComboHit);
            EventBus.Unsubscribe<HudBossSpawnedEvent>(OnBossSpawned);
            EventBus.Unsubscribe<HudBossHpChangedEvent>(OnBossHpChanged);
            EventBus.Unsubscribe<HudWaveChangedEvent>(OnWaveChanged);
        }

        public override void _Process(double delta)
        {
            // Combo timer reset
            if (_combo > 0)
            {
                _comboResetTimer -= (float)delta;
                if (_comboResetTimer <= 0f)
                    ResetCombo();
            }
        }

        // ----------------------------------------------------------------
        // Public API — called directly by Player or GameLoop when events
        // are not available (e.g. initialisation)
        // ----------------------------------------------------------------
        public void SetHealth(float current, float max)
        {
            _currentHp = current;
            _maxHp     = max;
            if (HealthBar  != null) { HealthBar.MaxValue = max; HealthBar.Value = current; }
            if (HealthLabel != null) HealthLabel.Text = $"{(int)current}/{(int)max}";
        }

        public void SetStamina(float current, float max)
        {
            _currentStam = current;
            _maxStam     = max;
            if (StaminaBar   != null) { StaminaBar.MaxValue = max; StaminaBar.Value = current; }
            if (StaminaLabel != null) StaminaLabel.Text = $"{(int)current}/{(int)max}";
        }

        public void ShowInteractPrompt(string text = "[E] Interact")
        {
            if (InteractPrompt == null) return;
            InteractPrompt.Text    = text;
            InteractPrompt.Visible = true;
        }

        public void HideInteractPrompt()
        {
            if (InteractPrompt != null) InteractPrompt.Visible = false;
        }

        public void SetWeaponName(string name)
        {
            if (WeaponLabel != null) WeaponLabel.Text = name;
        }

        public void AddComboHit()
        {
            _combo++;
            _comboResetTimer = ComboResetDelay;
            if (ComboLabel == null) return;
            ComboLabel.Visible = _combo > 1;
            ComboLabel.Text    = $"x{_combo} Combo!";
        }

        public void ResetCombo()
        {
            _combo = 0;
            _comboResetTimer = 0f;
            if (ComboLabel != null) ComboLabel.Visible = false;
        }

        public void ShowBossPanel(string bossName, float maxHp)
        {
            if (BossHpPanel  != null) BossHpPanel.Visible = true;
            if (BossNameLabel != null) BossNameLabel.Text = bossName;
            if (BossHealthBar != null) { BossHealthBar.MaxValue = maxHp; BossHealthBar.Value = maxHp; }
        }

        public void UpdateBossHp(float current)
        {
            if (BossHealthBar != null) BossHealthBar.Value = current;
        }

        public void HideBossPanel()
        {
            if (BossHpPanel != null) BossHpPanel.Visible = false;
        }

        public void SetWaveLabel(int wave, int maxWaves)
        {
            if (WaveLabel != null) WaveLabel.Text = $"Wave {wave} / {maxWaves}";
        }

        // ----------------------------------------------------------------
        // EventBus handlers
        // ----------------------------------------------------------------
        private void OnHealthChanged(HudHealthChangedEvent e)  => SetHealth(e.Current, e.Max);
        private void OnStaminaChanged(HudStaminaChangedEvent e) => SetStamina(e.Current, e.Max);
        private void OnWeaponChanged(HudWeaponChangedEvent e)   => SetWeaponName(e.WeaponName);
        private void OnComboHit(HudComboHitEvent _)             => AddComboHit();
        private void OnBossSpawned(HudBossSpawnedEvent e)       => ShowBossPanel(e.BossName, e.MaxHp);
        private void OnBossHpChanged(HudBossHpChangedEvent e)   => UpdateBossHp(e.Current);
        private void OnWaveChanged(HudWaveChangedEvent e)       => SetWaveLabel(e.Wave, e.MaxWaves);

        private void OnInteractPrompt(HudInteractPromptEvent e)
        {
            if (e.Show) ShowInteractPrompt(e.Text);
            else        HideInteractPrompt();
        }
    }

    // ----------------------------------------------------------------
    // HUD Event Records — fired on EventBus
    // ----------------------------------------------------------------
    public record HudHealthChangedEvent(float Current, float Max);
    public record HudStaminaChangedEvent(float Current, float Max);
    public record HudWeaponChangedEvent(string WeaponName);
    public record HudInteractPromptEvent(bool Show, string Text = "[E] Interact");
    public record HudComboHitEvent();
    public record HudBossSpawnedEvent(string BossName, float MaxHp);
    public record HudBossHpChangedEvent(float Current, float Max);
    public record HudWaveChangedEvent(int Wave, int MaxWaves);
}
