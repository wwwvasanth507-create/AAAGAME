using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI.HUD
{
    /// <summary>
    /// Modular HUD system with independently enabled/disabled widgets.
    /// Manages all in-game heads-up display elements via EventBus-driven updates.
    /// </summary>
    public partial class HUDController : CanvasLayer
    {
        // ---------------------------------------------------------------
        // Widget visibility flags
        // ---------------------------------------------------------------
        public bool ShowHealthBar { get; set; } = true;
        public bool ShowManaBar { get; set; } = true;
        public bool ShowStaminaBar { get; set; } = true;
        public bool ShowExperienceBar { get; set; } = true;
        public bool ShowCompass { get; set; } = true;
        public bool ShowMiniMap { get; set; } = true;
        public bool ShowQuestTracker { get; set; } = true;
        public bool ShowAbilityBar { get; set; } = true;
        public bool ShowInteractionPrompt { get; set; } = true;
        public bool ShowBuffDebuffIcons { get; set; } = true;
        public bool ShowStatusEffects { get; set; } = true;
        public bool ShowTargetInfo { get; set; } = true;
        public bool ShowBossHealth { get; set; } = true;
        public bool ShowFPSDebug { get; set; } = false;

        // ---------------------------------------------------------------
        // Widget references
        // ---------------------------------------------------------------
        private HUDWidget _healthWidget;
        private HUDWidget _manaWidget;
        private HUDWidget _staminaWidget;
        private HUDWidget _experienceWidget;
        private HUDWidget _compassWidget;
        private HUDWidget _miniMapWidget;
        private HUDWidget _questTrackerWidget;
        private HUDWidget _abilityBarWidget;
        private HUDWidget _interactionWidget;
        private HUDWidget _buffDebuffWidget;
        private HUDWidget _statusEffectWidget;
        private HUDWidget _targetInfoWidget;
        private HUDWidget _bossHealthWidget;
        private HUDWidget _fpsDebugWidget;

        private readonly List<HUDWidget> _allWidgets = new List<HUDWidget>();
        private UIPreferences _preferences;

        // ---------------------------------------------------------------
        // Lifecycle
        // ---------------------------------------------------------------
        public override void _Ready()
        {
            Layer = 20; // HUD layer
            Name = "HUDController";

            CreateWidgets();
            SubscribeEvents();
            ApplyPreferences();

            GD.Print("HUDController: Modular HUD initialized.");
        }

        public override void _ExitTree()
        {
            UnsubscribeEvents();
        }

        public override void _Process(double delta)
        {
            float dt = (float)delta;

            foreach (var widget in _allWidgets)
            {
                if (widget.Visible)
                    widget.OnUpdate(dt);
            }
        }

        // ---------------------------------------------------------------
        // Widget Creation
        // ---------------------------------------------------------------
        private void CreateWidgets()
        {
            // Top-left: Health, Mana, Stamina, Experience
            _healthWidget = CreateWidget<HealthWidget>("Health", new Vector2I(20, 20));
            _manaWidget = CreateWidget<ManaWidget>("Mana", new Vector2I(20, 90));
            _staminaWidget = CreateWidget<StaminaWidget>("Stamina", new Vector2I(20, 160));
            _experienceWidget = CreateWidget<ExperienceWidget>("Experience", new Vector2I(20, 230));

            // Top-center: Compass, Mini-map
            _compassWidget = CreateWidget<CompassWidget>("Compass", new Vector2I(860, 20));
            _miniMapWidget = CreateWidget<MiniMapWidget>("MiniMap", new Vector2I(1600, 20));

            // Top-right: Quest tracker
            _questTrackerWidget = CreateWidget<QuestTrackerWidget>("QuestTracker", new Vector2I(1400, 300));

            // Bottom-center: Ability bar
            _abilityBarWidget = CreateWidget<AbilityBarWidget>("AbilityBar", new Vector2I(360, 920));

            // Center: Interaction prompt
            _interactionWidget = CreateWidget<InteractionPromptWidget>("InteractionPrompt", new Vector2I(760, 500));

            // Top-right below minimap: Buffs/Debuffs
            _buffDebuffWidget = CreateWidget<BuffDebuffWidget>("BuffDebuff", new Vector2I(1600, 200));

            // Below health: Status effects
            _statusEffectWidget = CreateWidget<StatusEffectWidget>("StatusEffects", new Vector2I(20, 300));

            // Right side: Target info
            _targetInfoWidget = CreateWidget<TargetInfoWidget>("TargetInfo", new Vector2I(1400, 100));

            // Top-center (hidden by default): Boss health
            _bossHealthWidget = CreateWidget<BossHealthWidget>("BossHealth", new Vector2I(460, 50));

            // Bottom-right: FPS debug
            _fpsDebugWidget = CreateWidget<FPSDebugWidget>("FPSDebug", new Vector2I(1750, 20));

            // Apply initial visibility
            SyncWidgetVisibility();
        }

        private T CreateWidget<T>(string name, Vector2 position) where T : HUDWidget, new()
        {
            var widget = new T
            {
                Name = name,
                Position = position,
                Visible = true
            };
            AddChild(widget);
            _allWidgets.Add(widget);
            return widget;
        }

        // ---------------------------------------------------------------
        // Visibility Management
        // ---------------------------------------------------------------
        private void SyncWidgetVisibility()
        {
            SetWidgetVisible(_healthWidget, ShowHealthBar);
            SetWidgetVisible(_manaWidget, ShowManaBar);
            SetWidgetVisible(_staminaWidget, ShowStaminaBar);
            SetWidgetVisible(_experienceWidget, ShowExperienceBar);
            SetWidgetVisible(_compassWidget, ShowCompass);
            SetWidgetVisible(_miniMapWidget, ShowMiniMap);
            SetWidgetVisible(_questTrackerWidget, ShowQuestTracker);
            SetWidgetVisible(_abilityBarWidget, ShowAbilityBar);
            SetWidgetVisible(_interactionWidget, ShowInteractionPrompt);
            SetWidgetVisible(_buffDebuffWidget, ShowBuffDebuffIcons);
            SetWidgetVisible(_statusEffectWidget, ShowStatusEffects);
            SetWidgetVisible(_targetInfoWidget, ShowTargetInfo);
            SetWidgetVisible(_bossHealthWidget, ShowBossHealth);
            SetWidgetVisible(_fpsDebugWidget, ShowFPSDebug);
        }

        private void SetWidgetVisible(HUDWidget widget, bool visible)
        {
            if (widget != null)
                widget.Visible = visible;
        }

        public void ToggleWidget(string widgetName, bool visible)
        {
            switch (widgetName.ToLower())
            {
                case "health": ShowHealthBar = visible; break;
                case "mana": ShowManaBar = visible; break;
                case "stamina": ShowStaminaBar = visible; break;
                case "experience": ShowExperienceBar = visible; break;
                case "compass": ShowCompass = visible; break;
                case "minimap": ShowMiniMap = visible; break;
                case "questtracker": ShowQuestTracker = visible; break;
                case "abilitybar": ShowAbilityBar = visible; break;
                case "interaction": ShowInteractionPrompt = visible; break;
                case "buffs": ShowBuffDebuffIcons = visible; break;
                case "statuseffects": ShowStatusEffects = visible; break;
                case "targetinfo": ShowTargetInfo = visible; break;
                case "bosshealth": ShowBossHealth = visible; break;
                case "fps": ShowFPSDebug = visible; break;
            }
            SyncWidgetVisibility();
        }

        // ---------------------------------------------------------------
        // Event Subscriptions
        // ---------------------------------------------------------------
        private void SubscribeEvents()
        {
            EventBus.Subscribe<HudHealthChangedEvent>(OnHealthChanged);
            EventBus.Subscribe<HudManaChangedEvent>(OnManaChanged);
            EventBus.Subscribe<HudStaminaChangedEvent>(OnStaminaChanged);
            EventBus.Subscribe<HudExperienceChangedEvent>(OnExperienceChanged);
            EventBus.Subscribe<HudLevelUpEvent>(OnLevelUp);
            EventBus.Subscribe<HudInteractPromptEvent>(OnInteractPrompt);
            EventBus.Subscribe<HudBossSpawnedEvent>(OnBossSpawned);
            EventBus.Subscribe<HudBossHpChangedEvent>(OnBossHpChanged);
            EventBus.Subscribe<HudQuestUpdatedEvent>(OnQuestUpdated);
            EventBus.Subscribe<HudItemAcquiredEvent>(OnItemAcquired);
        }

        private void UnsubscribeEvents()
        {
            EventBus.Unsubscribe<HudHealthChangedEvent>(OnHealthChanged);
            EventBus.Unsubscribe<HudManaChangedEvent>(OnManaChanged);
            EventBus.Unsubscribe<HudStaminaChangedEvent>(OnStaminaChanged);
            EventBus.Unsubscribe<HudExperienceChangedEvent>(OnExperienceChanged);
            EventBus.Unsubscribe<HudLevelUpEvent>(OnLevelUp);
            EventBus.Unsubscribe<HudInteractPromptEvent>(OnInteractPrompt);
            EventBus.Unsubscribe<HudBossSpawnedEvent>(OnBossSpawned);
            EventBus.Unsubscribe<HudBossHpChangedEvent>(OnBossHpChanged);
            EventBus.Unsubscribe<HudQuestUpdatedEvent>(OnQuestUpdated);
            EventBus.Unsubscribe<HudItemAcquiredEvent>(OnItemAcquired);
        }

        // ---------------------------------------------------------------
        // Event Handlers — delegate to widgets
        // ---------------------------------------------------------------
        private void OnHealthChanged(HudHealthChangedEvent e)
        {
            if (_healthWidget is IHealthBar hb)
                hb.SetHealth(e.Current, e.Max);
        }

        private void OnManaChanged(HudManaChangedEvent e)
        {
            if (_manaWidget is IManaBar mb)
                mb.SetMana(e.Current, e.Max);
        }

        private void OnStaminaChanged(HudStaminaChangedEvent e)
        {
            if (_staminaWidget is IStaminaBar sb)
                sb.SetStamina(e.Current, e.Max);
        }

        private void OnExperienceChanged(HudExperienceChangedEvent e)
        {
            if (_experienceWidget is IExperienceBar xb)
                xb.SetExperience(e.Current, e.Max, e.Level);
        }

        private void OnLevelUp(HudLevelUpEvent e)
        {
            // Trigger notification
            var notif = ServiceLocator.Get<NotificationManager>();
            notif?.QueueNotification(new UINotification
            {
                Id = $"level_{e.NewLevel}",
                TitleKey = "notif_level_up_title",
                MessageKey = $"notif_level_up_msg_{e.NewLevel}",
                Priority = NotificationPriority.High,
                Duration = 4.0f,
                Icon = "level_up"
            });
        }

        private void OnInteractPrompt(HudInteractPromptEvent e)
        {
            if (_interactionWidget is IInteractionPrompt ip)
            {
                if (e.Show) ip.ShowPrompt(e.Text);
                else ip.HidePrompt();
            }
        }

        private void OnBossSpawned(HudBossSpawnedEvent e)
        {
            if (_bossHealthWidget is IBossHealthBar bh)
                bh.ShowBoss(e.BossName, e.MaxHp);
            ShowBossHealth = true;
            SyncWidgetVisibility();
        }

        private void OnBossHpChanged(HudBossHpChangedEvent e)
        {
            if (_bossHealthWidget is IBossHealthBar bh)
                bh.UpdateBossHp(e.Current);
        }

        private void OnQuestUpdated(HudQuestUpdatedEvent e)
        {
            if (_questTrackerWidget is IQuestTracker qt)
                qt.AddQuest(e.QuestName, e.Description);
        }

        private void OnItemAcquired(HudItemAcquiredEvent e)
        {
            var notif = ServiceLocator.Get<NotificationManager>();
            notif?.QueueNotification(new UINotification
            {
                Id = $"item_{e.ItemName}",
                TitleKey = "notif_item_acquired",
                MessageKey = e.ItemName,
                Priority = NotificationPriority.Normal,
                Duration = 2.5f,
                Icon = "item"
            });
        }

        // ---------------------------------------------------------------
        // Preferences
        // ---------------------------------------------------------------
        public void ApplyPreferences()
        {
            try
            {
                var uiManager = ServiceLocator.Get<UIManager>();
                _preferences = uiManager?.Preferences;
                if (_preferences == null) return;

                Scale = new Vector2(_preferences.UIScale, _preferences.UIScale);

                foreach (var widget in _allWidgets)
                {
                    if (widget is IAccessibleWidget accessible)
                    {
                        accessible.SetTextScale(_preferences.TextSize);
                        accessible.SetHighContrast(_preferences.HighContrast);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"HUDController: Failed to apply preferences: {ex.Message}");
            }
        }
    }

    // ---------------------------------------------------------------
    // Base HUD Widget
    // ---------------------------------------------------------------
    public abstract partial class HUDWidget : Control
    {
        public virtual void OnUpdate(float delta) { }

        public virtual void Refresh() { }
    }

    // ---------------------------------------------------------------
    // Widget Interfaces
    // ---------------------------------------------------------------
    public interface IHealthBar { void SetHealth(float current, float max); }
    public interface IManaBar { void SetMana(float current, float max); }
    public interface IStaminaBar { void SetStamina(float current, float max); }
    public interface IExperienceBar { void SetExperience(float current, float max, int level); }
    public interface IInteractionPrompt { void ShowPrompt(string text); void HidePrompt(); }
    public interface IBossHealthBar { void ShowBoss(string name, float maxHp); void UpdateBossHp(float current); void HideBoss(); }
    public interface IQuestTracker { void AddQuest(string name, string description); void RemoveQuest(string name); void Clear(); }
    public interface IAccessibleWidget { void SetTextScale(float scale); void SetHighContrast(bool enabled); }

    // ---------------------------------------------------------------
    // Health Widget
    // ---------------------------------------------------------------
    public partial class HealthWidget : HUDWidget, IHealthBar, IAccessibleWidget
    {
        private ProgressBar _bar;
        private Label _label;

        public override void _Ready()
        {
            Size = new Vector2I(250, 50);

            _bar = new ProgressBar
            {
                Position = new Vector2I(0, 0),
                Size = new Vector2I(250, 25),
                MinValue = 0,
                MaxValue = 100,
                Value = 100,
                ShowPercentage = false
            };
            AddChild(_bar);

            _label = new Label
            {
                Text = "HP",
                Position = new Vector2I(0, 28),
                Size = new Vector2I(250, 20)
            };
            AddChild(_label);
        }

        public void SetHealth(float current, float max)
        {
            if (_bar != null)
            {
                _bar.MaxValue = max;
                _bar.Value = current;
            }
            if (_label != null)
                _label.Text = $"HP: {(int)current}/{(int)max}";
        }

        public void SetTextScale(float scale)
        {
            if (_label != null)
                _label.Scale = new Vector2(scale, scale);
        }

        public void SetHighContrast(bool enabled)
        {
            if (_bar != null)
            {
                if (enabled)
                    _bar.Modulate = new Color(1, 0, 0);
                else
                    _bar.Modulate = Colors.White;
            }
        }
    }

    // ---------------------------------------------------------------
    // Mana Widget
    // ---------------------------------------------------------------
    public partial class ManaWidget : HUDWidget, IManaBar, IAccessibleWidget
    {
        private ProgressBar _bar;
        private Label _label;

        public override void _Ready()
        {
            Size = new Vector2I(250, 50);

            _bar = new ProgressBar
            {
                Position = new Vector2I(0, 0),
                Size = new Vector2I(250, 25),
                MinValue = 0,
                MaxValue = 100,
                Value = 100
            };
            AddChild(_bar);

            _label = new Label
            {
                Text = "MP",
                Position = new Vector2I(0, 28),
                Size = new Vector2I(250, 20)
            };
            AddChild(_label);
        }

        public void SetMana(float current, float max)
        {
            if (_bar != null)
            {
                _bar.MaxValue = max;
                _bar.Value = current;
            }
            if (_label != null)
                _label.Text = $"MP: {(int)current}/{(int)max}";
        }

        public void SetTextScale(float scale)
        {
            if (_label != null)
                _label.Scale = new Vector2(scale, scale);
        }

        public void SetHighContrast(bool enabled)
        {
            if (_bar != null)
            {
                if (enabled)
                    _bar.Modulate = new Color(0, 0.5f, 1);
                else
                    _bar.Modulate = Colors.White;
            }
        }
    }

    // ---------------------------------------------------------------
    // Stamina Widget
    // ---------------------------------------------------------------
    public partial class StaminaWidget : HUDWidget, IStaminaBar, IAccessibleWidget
    {
        private ProgressBar _bar;
        private Label _label;

        public override void _Ready()
        {
            Size = new Vector2I(250, 50);

            _bar = new ProgressBar
            {
                Position = new Vector2I(0, 0),
                Size = new Vector2I(250, 25),
                MinValue = 0,
                MaxValue = 100,
                Value = 100
            };
            AddChild(_bar);

            _label = new Label
            {
                Text = "Stamina",
                Position = new Vector2I(0, 28),
                Size = new Vector2I(250, 20)
            };
            AddChild(_label);
        }

        public void SetStamina(float current, float max)
        {
            if (_bar != null)
            {
                _bar.MaxValue = max;
                _bar.Value = current;
            }
            if (_label != null)
                _label.Text = $"Stamina: {(int)current}/{(int)max}";
        }

        public void SetTextScale(float scale)
        {
            if (_label != null)
                _label.Scale = new Vector2(scale, scale);
        }

        public void SetHighContrast(bool enabled)
        {
            if (_bar != null)
            {
                if (enabled)
                    _bar.Modulate = new Color(0, 1, 0);
                else
                    _bar.Modulate = Colors.White;
            }
        }
    }

    // ---------------------------------------------------------------
    // Experience Widget
    // ---------------------------------------------------------------
    public partial class ExperienceWidget : HUDWidget, IExperienceBar, IAccessibleWidget
    {
        private ProgressBar _bar;
        private Label _label;

        public override void _Ready()
        {
            Size = new Vector2I(250, 50);

            _bar = new ProgressBar
            {
                Position = new Vector2I(0, 0),
                Size = new Vector2I(250, 25),
                MinValue = 0,
                MaxValue = 100,
                Value = 0
            };
            AddChild(_bar);

            _label = new Label
            {
                Text = "XP",
                Position = new Vector2I(0, 28),
                Size = new Vector2I(250, 20)
            };
            AddChild(_label);
        }

        public void SetExperience(float current, float max, int level)
        {
            if (_bar != null)
            {
                _bar.MaxValue = max;
                _bar.Value = current;
            }
            if (_label != null)
                _label.Text = $"Lv.{level} XP: {(int)current}/{(int)max}";
        }

        public void SetTextScale(float scale)
        {
            if (_label != null)
                _label.Scale = new Vector2(scale, scale);
        }

        public void SetHighContrast(bool enabled)
        {
            if (_bar != null)
            {
                if (enabled)
                    _bar.Modulate = new Color(1, 0.84f, 0);
                else
                    _bar.Modulate = Colors.White;
            }
        }
    }

    // ---------------------------------------------------------------
    // Compass Widget
    // ---------------------------------------------------------------
    public partial class CompassWidget : HUDWidget, IAccessibleWidget
    {
        private Label _compassLabel;

        public override void _Ready()
        {
            Size = new Vector2I(200, 30);

            _compassLabel = new Label
            {
                Text = "N — — — — — — — S",
                HorizontalAlignment = HorizontalAlignment.Center,
                Size = new Vector2I(200, 30)
            };
            AddChild(_compassLabel);
        }

        public void SetDirection(float headingDegrees)
        {
            string dir = "N";
            if (headingDegrees > 22.5f && headingDegrees < 67.5f) dir = "NE";
            else if (headingDegrees >= 67.5f && headingDegrees < 112.5f) dir = "E";
            else if (headingDegrees >= 112.5f && headingDegrees < 157.5f) dir = "SE";
            else if (headingDegrees >= 157.5f && headingDegrees < 202.5f) dir = "S";
            else if (headingDegrees >= 202.5f && headingDegrees < 247.5f) dir = "SW";
            else if (headingDegrees >= 247.5f && headingDegrees < 292.5f) dir = "W";
            else if (headingDegrees >= 292.5f && headingDegrees < 337.5f) dir = "NW";

            if (_compassLabel != null)
                _compassLabel.Text = $"{dir} — {headingDegrees:F0}°";
        }

        public void SetTextScale(float scale)
        {
            if (_compassLabel != null)
                _compassLabel.Scale = new Vector2(scale, scale);
        }

        public void SetHighContrast(bool enabled)
        {
            if (_compassLabel != null)
                _compassLabel.Modulate = enabled ? new Color(1, 1, 0) : Colors.White;
        }
    }

    // ---------------------------------------------------------------
    // MiniMap Widget (Hook)
    // ---------------------------------------------------------------
    public partial class MiniMapWidget : HUDWidget
    {
        private ColorRect _mapRect;
        private Label _placeholder;

        public override void _Ready()
        {
            Size = new Vector2I(256, 256);

            _mapRect = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f),
                Size = new Vector2I(256, 256)
            };
            AddChild(_mapRect);

            _placeholder = new Label
            {
                Text = "MINI-MAP",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Size = new Vector2I(256, 256)
            };
            AddChild(_placeholder);
        }
    }

    // ---------------------------------------------------------------
    // Quest Tracker Widget
    // ---------------------------------------------------------------
    public partial class QuestTrackerWidget : HUDWidget, IQuestTracker, IAccessibleWidget
    {
        private VBoxContainer _questContainer;
        private Label _titleLabel;

        public override void _Ready()
        {
            Size = new Vector2I(300, 300);

            _titleLabel = new Label
            {
                Text = "Active Quests",
                Size = new Vector2I(300, 25)
            };
            AddChild(_titleLabel);

            _questContainer = new VBoxContainer
            {
                Position = new Vector2I(0, 30),
                Size = new Vector2I(300, 270)
            };
            AddChild(_questContainer);
        }

        public void AddQuest(string name, string description)
        {
            var questLabel = new Label
            {
                Text = $"{name}: {description}",
                Size = new Vector2I(300, 40),
                AutowrapMode = TextServer.AutowrapMode.Word
            };
            _questContainer.AddChild(questLabel);
        }

        public void RemoveQuest(string name)
        {
            foreach (Node child in _questContainer.GetChildren())
            {
                if (child is Label label && label.Text.StartsWith(name))
                {
                    _questContainer.RemoveChild(child);
                    child.QueueFree();
                    break;
                }
            }
        }

        public void Clear()
        {
            foreach (Node child in _questContainer.GetChildren())
                child.QueueFree();
        }

        public void SetTextScale(float scale)
        {
            Scale = new Vector2(scale, scale);
        }

        public void SetHighContrast(bool enabled)
        {
            Modulate = enabled ? new Color(1, 1, 0) : Colors.White;
        }
    }

    // ---------------------------------------------------------------
    // Ability Bar Widget
    // ---------------------------------------------------------------
    public partial class AbilityBarWidget : HUDWidget
    {
        private HBoxContainer _slotContainer;
        private readonly List<Panel> _slots = new List<Panel>();

        public override void _Ready()
        {
            Size = new Vector2I(600, 80);

            _slotContainer = new HBoxContainer
            {
                Size = new Vector2I(600, 80)
            };
            AddChild(_slotContainer);

            for (int i = 0; i < 6; i++)
            {
                var slot = new Panel
                {
                    Size = new Vector2I(80, 80),
                    Theme = CreateSlotTheme()
                };
                var number = new Label
                {
                    Text = (i + 1).ToString(),
                    Position = new Vector2I(65, 0),
                    Size = new Vector2I(15, 15)
                };
                slot.AddChild(number);
                _slotContainer.AddChild(slot);
                _slots.Add(slot);
            }
        }

        private Theme CreateSlotTheme()
        {
            var theme = new Theme();
            var style = new StyleBoxFlat { BgColor = new Color(0.1f, 0.1f, 0.15f, 0.8f), BorderWidthBottom = 2, BorderColor = new Color(0.3f, 0.3f, 0.4f) };
            theme.SetStylebox("panel", "Panel", style);
            return theme;
        }

        public void SetAbility(int slot, string abilityName)
        {
            if (slot < 0 || slot >= _slots.Count) return;

            var panel = _slots[slot];
            // Clear old label
            foreach (Node child in panel.GetChildren())
            {
                if (child is Label && child.Name != "number")
                {
                    panel.RemoveChild(child);
                    child.QueueFree();
                }
            }

            var label = new Label
            {
                Name = "ability",
                Text = abilityName,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Position = new Vector2I(0, 15),
                Size = new Vector2I(80, 50)
            };
            panel.AddChild(label);
        }
    }

    // ---------------------------------------------------------------
    // Interaction Prompt Widget
    // ---------------------------------------------------------------
    public partial class InteractionPromptWidget : HUDWidget, IInteractionPrompt, IAccessibleWidget
    {
        private Label _promptLabel;

        public override void _Ready()
        {
            Size = new Vector2I(400, 50);

            _promptLabel = new Label
            {
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                Size = new Vector2I(400, 50),
                Visible = false
            };
            AddChild(_promptLabel);
        }

        public void ShowPrompt(string text)
        {
            if (_promptLabel != null)
            {
                _promptLabel.Text = text;
                _promptLabel.Visible = true;
            }
        }

        public void HidePrompt()
        {
            if (_promptLabel != null)
                _promptLabel.Visible = false;
        }

        public void SetTextScale(float scale)
        {
            if (_promptLabel != null)
                _promptLabel.Scale = new Vector2(scale, scale);
        }

        public void SetHighContrast(bool enabled)
        {
            if (_promptLabel != null)
                _promptLabel.Modulate = enabled ? new Color(1, 1, 0) : Colors.White;
        }
    }

    // ---------------------------------------------------------------
    // Buff/Debuff Widget
    // ---------------------------------------------------------------
    public partial class BuffDebuffWidget : HUDWidget, IAccessibleWidget
    {
        private HBoxContainer _iconContainer;

        public override void _Ready()
        {
            Size = new Vector2I(300, 60);

            _iconContainer = new HBoxContainer
            {
                Size = new Vector2I(300, 60)
            };
            AddChild(_iconContainer);
        }

        public void AddBuff(string name, float duration)
        {
            var buff = new Panel
            {
                Size = new Vector2I(50, 50),
                Theme = CreateBuffTheme()
            };
            var label = new Label
            {
                Text = name.Substring(0, Math.Min(3, name.Length)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Size = new Vector2I(50, 50)
            };
            buff.AddChild(label);
            _iconContainer.AddChild(buff);
        }

        public void ClearBuffs()
        {
            foreach (Node child in _iconContainer.GetChildren())
                child.QueueFree();
        }

        private Theme CreateBuffTheme()
        {
            var theme = new Theme();
            var style = new StyleBoxFlat { BgColor = new Color(0, 0.5f, 0, 0.7f) };
            theme.SetStylebox("panel", "Panel", style);
            return theme;
        }

        public void SetTextScale(float scale) { }
        public void SetHighContrast(bool enabled) { }
    }

    // ---------------------------------------------------------------
    // Status Effect Widget
    // ---------------------------------------------------------------
    public partial class StatusEffectWidget : HUDWidget, IAccessibleWidget
    {
        private VBoxContainer _effectContainer;

        public override void _Ready()
        {
            Size = new Vector2I(250, 200);

            _effectContainer = new VBoxContainer
            {
                Size = new Vector2I(250, 200)
            };
            AddChild(_effectContainer);
        }

        public void AddEffect(string name, float remaining)
        {
            var effect = new Label
            {
                Text = $"{name} ({remaining:F1}s)",
                Size = new Vector2I(250, 25)
            };
            _effectContainer.AddChild(effect);
        }

        public void ClearEffects()
        {
            foreach (Node child in _effectContainer.GetChildren())
                child.QueueFree();
        }

        public void SetTextScale(float scale) { Scale = new Vector2(scale, scale); }
        public void SetHighContrast(bool enabled) { Modulate = enabled ? new Color(1, 1, 0) : Colors.White; }
    }

    // ---------------------------------------------------------------
    // Target Info Widget
    // ---------------------------------------------------------------
    public partial class TargetInfoWidget : HUDWidget, IAccessibleWidget
    {
        private Label _nameLabel;
        private ProgressBar _hpBar;
        private Label _levelLabel;

        public override void _Ready()
        {
            Size = new Vector2I(300, 100);

            _nameLabel = new Label
            {
                Text = "Target",
                Position = new Vector2I(0, 0),
                Size = new Vector2I(300, 25)
            };
            AddChild(_nameLabel);

            _hpBar = new ProgressBar
            {
                Position = new Vector2I(0, 30),
                Size = new Vector2I(300, 25),
                MinValue = 0,
                MaxValue = 100,
                Value = 100
            };
            AddChild(_hpBar);

            _levelLabel = new Label
            {
                Text = "Lv.1",
                Position = new Vector2I(0, 60),
                Size = new Vector2I(300, 25)
            };
            AddChild(_levelLabel);
        }

        public void SetTarget(string name, int level, float hp, float maxHp)
        {
            if (_nameLabel != null) _nameLabel.Text = name;
            if (_levelLabel != null) _levelLabel.Text = $"Lv.{level}";
            if (_hpBar != null) { _hpBar.MaxValue = maxHp; _hpBar.Value = hp; }
        }

        public void SetTextScale(float scale) { Scale = new Vector2(scale, scale); }
        public void SetHighContrast(bool enabled) { Modulate = enabled ? new Color(1, 0, 0) : Colors.White; }
    }

    // ---------------------------------------------------------------
    // Boss Health Widget
    // ---------------------------------------------------------------
    public partial class BossHealthWidget : HUDWidget, IBossHealthBar, IAccessibleWidget
    {
        private Label _bossNameLabel;
        private ProgressBar _bossHpBar;
        private Label _bossPercentLabel;

        public override void _Ready()
        {
            Size = new Vector2I(500, 80);
            Visible = false;

            _bossNameLabel = new Label
            {
                Text = "Boss",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 0),
                Size = new Vector2I(500, 25)
            };
            AddChild(_bossNameLabel);

            _bossHpBar = new ProgressBar
            {
                Position = new Vector2I(0, 30),
                Size = new Vector2I(500, 30),
                MinValue = 0,
                MaxValue = 100,
                Value = 100,
                ShowPercentage = false
            };
            AddChild(_bossHpBar);

            _bossPercentLabel = new Label
            {
                Text = "100%",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 65),
                Size = new Vector2I(500, 15)
            };
            AddChild(_bossPercentLabel);
        }

        public void ShowBoss(string name, float maxHp)
        {
            Visible = true;
            if (_bossNameLabel != null) _bossNameLabel.Text = name;
            if (_bossHpBar != null) { _bossHpBar.MaxValue = maxHp; _bossHpBar.Value = maxHp; }
            if (_bossPercentLabel != null) _bossPercentLabel.Text = "100%";
        }

        public void UpdateBossHp(float current)
        {
            if (_bossHpBar != null) _bossHpBar.Value = current;
            if (_bossPercentLabel != null && _bossHpBar != null)
            {
                float pct = (float)(_bossHpBar.MaxValue > 0 ? (current / _bossHpBar.MaxValue) * 100 : 0);
                _bossPercentLabel.Text = $"{pct:F0}%";
            }
        }

        public void HideBoss()
        {
            Visible = false;
        }

        public void SetTextScale(float scale) { Scale = new Vector2(scale, scale); }
        public void SetHighContrast(bool enabled) { }
    }

    // ---------------------------------------------------------------
    // FPS Debug Widget
    // ---------------------------------------------------------------
    public partial class FPSDebugWidget : HUDWidget
    {
        private Label _fpsLabel;
        private Label _memoryLabel;
        private float _updateTimer;

        public override void _Ready()
        {
            Size = new Vector2I(200, 60);
            Visible = false;

            _fpsLabel = new Label
            {
                Text = "FPS: --",
                Size = new Vector2I(200, 25)
            };
            AddChild(_fpsLabel);

            _memoryLabel = new Label
            {
                Text = "MEM: -- MB",
                Position = new Vector2I(0, 30),
                Size = new Vector2I(200, 25)
            };
            AddChild(_memoryLabel);
        }

        public override void OnUpdate(float delta)
        {
            _updateTimer += delta;
            if (_updateTimer < 0.5f) return; // Update twice per second
            _updateTimer = 0;

            if (_fpsLabel != null)
            {
                float fps = (float)Performance.GetMonitor(Performance.Monitor.TimeFps);
                _fpsLabel.Text = $"FPS: {fps:F0}";
            }

            if (_memoryLabel != null)
            {
                float memMB = (float)(Performance.GetMonitor(Performance.Monitor.MemoryStatic) / (1024.0 * 1024.0));
                _memoryLabel.Text = $"MEM: {memMB:F1} MB";
            }
        }
    }

    // ---------------------------------------------------------------
    // HUD Event Records
    // ---------------------------------------------------------------
    public record HudHealthChangedEvent(float Current, float Max);
    public record HudManaChangedEvent(float Current, float Max);
    public record HudStaminaChangedEvent(float Current, float Max);
    public record HudWeaponChangedEvent(string WeaponName);
    public record HudComboHitEvent();
    public record HudWaveChangedEvent(int Wave, int MaxWaves);
    public record HudExperienceChangedEvent(float Current, float Max, int Level);
    public record HudLevelUpEvent(int NewLevel);
    public record HudInteractPromptEvent(bool Show, string Text = "");
    public record HudBossSpawnedEvent(string BossName, float MaxHp);
    public record HudBossHpChangedEvent(float Current, float Max);
    public record HudQuestUpdatedEvent(string QuestName, string Description);
    public record HudItemAcquiredEvent(string ItemName, int Quantity);
}