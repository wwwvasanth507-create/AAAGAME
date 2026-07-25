using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI.Screens
{
    /// <summary>
    /// Central registry for all game screens. Handles registration, lazy loading,
    /// and provides factory methods for creating screen instances.
    /// </summary>
    public static class ScreenRegistry
    {
        // ---------------------------------------------------------------
        // Screen Identifiers
        // ---------------------------------------------------------------
        public const string MainMenu = "main_menu";
        public const string PauseMenu = "pause_menu";
        public const string Settings = "settings";
        public const string Inventory = "inventory";
        public const string Equipment = "equipment";
        public const string Character = "character";
        public const string Abilities = "abilities";
        public const string QuestJournal = "quest_journal";
        public const string Map = "map";
        public const string Crafting = "crafting";
        public const string Trading = "trading";
        public const string Dialogue = "dialogue";
        public const string Notifications = "notifications";
        public const string Loading = "loading";
        public const string GameOver = "game_over";
        public const string SaveLoad = "save_load";
        public const string Bestiary = "bestiary";
        public const string Codex = "codex";
        public const string Achievements = "achievements";
        public const string DLCPlaceholder = "dlc_placeholder";

        // ---------------------------------------------------------------
        // Registration
        // ---------------------------------------------------------------
        public static void RegisterAll(UIManager manager)
        {
            if (manager == null)
            {
                Logger.Error("ScreenRegistry: UIManager is null, cannot register screens.");
                return;
            }

            Logger.Info("ScreenRegistry: Registering all screens...");

            // Core screens
            RegisterScreen<MainMenuScreen>(manager, MainMenu, true);
            RegisterScreen<PauseMenuScreen>(manager, PauseMenu, true);
            RegisterScreen<SettingsScreen>(manager, Settings, true);
            RegisterScreen<LoadingScreen>(manager, Loading, true);
            RegisterScreen<GameOverScreen>(manager, GameOver, true);
            RegisterScreen<SaveLoadScreen>(manager, SaveLoad, true);

            // Gameplay screens
            RegisterScreen<InventoryScreen>(manager, Inventory, false);
            RegisterScreen<EquipmentScreen>(manager, Equipment, false);
            RegisterScreen<CharacterScreen>(manager, Character, false);
            RegisterScreen<AbilitiesScreen>(manager, Abilities, false);
            RegisterScreen<QuestJournalScreen>(manager, QuestJournal, false);
            RegisterScreen<MapScreen>(manager, Map, false);
            RegisterScreen<CraftingScreen>(manager, Crafting, false);
            RegisterScreen<TradingScreen>(manager, Trading, false);
            RegisterScreen<DialogueScreen>(manager, Dialogue, false);
            RegisterScreen<NotificationHistoryScreen>(manager, Notifications, false);

            // Content screens
            RegisterScreen<BestiaryScreen>(manager, Bestiary, false);
            RegisterScreen<CodexScreen>(manager, Codex, false);
            RegisterScreen<AchievementsScreen>(manager, Achievements, false);

            // DLC placeholder
            RegisterScreen<DLCPlaceholderScreen>(manager, DLCPlaceholder, false);

            Logger.Info($"ScreenRegistry: {manager.ScreenStackDepth} screens registered.");
        }

        private static void RegisterScreen<T>(UIManager manager, string screenId, bool lazyLoad) where T : UIScreen, new()
        {
            var screen = new T
            {
                Name = screenId,
                LazyLoad = lazyLoad,
                Visible = false
            };
            manager.RegisterScreen(screenId, screen);
        }
    }

    // ---------------------------------------------------------------
    // Main Menu Screen
    // ---------------------------------------------------------------
    public partial class MainMenuScreen : UIScreen
    {
        private Button _newGameButton;
        private Button _continueButton;
        private Button _settingsButton;
        private Button _quitButton;
        private Label _titleLabel;
        private Label _versionLabel;

        protected override void OnLazyLoad()
        {
            // Create main menu layout
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            // Background panel
            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            // Title
            _titleLabel = new Label
            {
                Text = "HERO OF ETERNIA",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Position = new Vector2I(0, 100),
                Size = new Vector2I(1920, 100),
                Theme = CreateTitleTheme()
            };
            AddChild(_titleLabel);

            // Version
            _versionLabel = new Label
            {
                Text = "v0.20.0 — UI Framework",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 200),
                Size = new Vector2I(1920, 40),
                Theme = CreateVersionTheme()
            };
            AddChild(_versionLabel);

            // Menu buttons
            int buttonY = 350;
            int buttonSpacing = 70;
            _newGameButton = CreateMenuButton("New Adventure", new Vector2I(760, buttonY));
            _continueButton = CreateMenuButton("Continue Journey", new Vector2I(760, buttonY + buttonSpacing));
            _settingsButton = CreateMenuButton("Settings", new Vector2I(760, buttonY + buttonSpacing * 2));
            _quitButton = CreateMenuButton("Exit", new Vector2I(760, buttonY + buttonSpacing * 3));

            _newGameButton.Pressed += OnNewGamePressed;
            _continueButton.Pressed += OnContinuePressed;
            _settingsButton.Pressed += OnSettingsPressed;
            _quitButton.Pressed += OnQuitPressed;
        }

        private void OnNewGamePressed()
        {
            var ui = ServiceLocator.Get<UIManager>();
            ui?.OpenScreen(ScreenRegistry.Loading);
        }

        private void OnContinuePressed()
        {
            var ui = ServiceLocator.Get<UIManager>();
            ui?.OpenScreen(ScreenRegistry.SaveLoad);
        }

        private void OnSettingsPressed()
        {
            var ui = ServiceLocator.Get<UIManager>();
            ui?.OpenScreen(ScreenRegistry.Settings);
        }

        private void OnQuitPressed()
        {
            GetTree().Quit();
        }

        private Button CreateMenuButton(string text, Vector2 position)
        {
            var btn = new Button
            {
                Text = text,
                Position = position,
                Size = new Vector2I(400, 50),
                Theme = CreateButtonTheme()
            };
            AddChild(btn);
            return btn;
        }

        private Theme CreateTitleTheme()
        {
            var theme = new Theme();
            var font = ThemeDB.FallbackFont;
            theme.SetFont("font", "Label", font);
            theme.SetFontSize("font_size", "Label", 48);
            theme.SetColor("font_color", "Label", new Color(1, 0.84f, 0));
            return theme;
        }

        private Theme CreateVersionTheme()
        {
            var theme = new Theme();
            theme.SetFont("font", "Label", ThemeDB.FallbackFont);
            theme.SetFontSize("font_size", "Label", 18);
            theme.SetColor("font_color", "Label", new Color(0.6f, 0.6f, 0.6f));
            return theme;
        }

        private Theme CreateButtonTheme()
        {
            var theme = new Theme();
            theme.SetFont("font", "Button", ThemeDB.FallbackFont);
            theme.SetFontSize("font_size", "Button", 22);
            theme.SetColor("font_color", "Button", Colors.White);
            theme.SetColor("font_hover_color", "Button", new Color(1, 0.84f, 0));
            theme.SetColor("font_pressed_color", "Button", new Color(0.8f, 0.7f, 0));
            theme.SetStyleBox("normal", "Button", new StyleBoxEmpty());
            theme.SetStyleBox("hover", "Button", new StyleBoxEmpty());
            theme.SetStyleBox("pressed", "Button", new StyleBoxEmpty());
            return theme;
        }
    }

    // ---------------------------------------------------------------
    // Pause Menu Screen
    // ---------------------------------------------------------------
    public partial class PauseMenuScreen : UIScreen
    {
        private Button _resumeButton;
        private Button _settingsButton;
        private Button _saveButton;
        private Button _loadButton;
        private Button _quitButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            // Dim background
            var dim = new ColorRect
            {
                Color = new Color(0, 0, 0, 0.6f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(dim);

            // Pause panel
            var panel = new Panel
            {
                Position = new Vector2I(760, 200),
                Size = new Vector2I(400, 500)
            };
            AddChild(panel);

            var title = new Label
            {
                Text = "PAUSED",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 20),
                Size = new Vector2I(400, 50)
            };
            panel.AddChild(title);

            int buttonY = 100;
            int spacing = 60;
            _resumeButton = new Button { Text = "Resume", Position = new Vector2I(50, buttonY), Size = new Vector2I(300, 45) };
            _settingsButton = new Button { Text = "Settings", Position = new Vector2I(50, buttonY + spacing), Size = new Vector2I(300, 45) };
            _saveButton = new Button { Text = "Save Game", Position = new Vector2I(50, buttonY + spacing * 2), Size = new Vector2I(300, 45) };
            _loadButton = new Button { Text = "Load Game", Position = new Vector2I(50, buttonY + spacing * 3), Size = new Vector2I(300, 45) };
            _quitButton = new Button { Text = "Quit to Menu", Position = new Vector2I(50, buttonY + spacing * 4), Size = new Vector2I(300, 45) };

            panel.AddChild(_resumeButton);
            panel.AddChild(_settingsButton);
            panel.AddChild(_saveButton);
            panel.AddChild(_loadButton);
            panel.AddChild(_quitButton);

            _resumeButton.Pressed += OnResumePressed;
            _settingsButton.Pressed += OnSettingsPressed;
            _saveButton.Pressed += OnSavePressed;
            _loadButton.Pressed += OnLoadPressed;
            _quitButton.Pressed += OnQuitPressed;
        }

        private void OnResumePressed()
        {
            var ui = ServiceLocator.Get<UIManager>();
            ui?.CloseScreen();
        }

        private void OnSettingsPressed()
        {
            var ui = ServiceLocator.Get<UIManager>();
            ui?.OpenScreen(ScreenRegistry.Settings);
        }

        private void OnSavePressed()
        {
            var ui = ServiceLocator.Get<UIManager>();
            ui?.OpenScreen(ScreenRegistry.SaveLoad);
        }

        private void OnLoadPressed()
        {
            var ui = ServiceLocator.Get<UIManager>();
            ui?.OpenScreen(ScreenRegistry.SaveLoad);
        }

        private void OnQuitPressed()
        {
            GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
        }
    }

    // ---------------------------------------------------------------
    // Settings Screen
    // ---------------------------------------------------------------
    public partial class SettingsScreen : UIScreen
    {
        private TabContainer _tabs;
        private VBoxContainer _audioTab;
        private VBoxContainer _graphicsTab;
        private VBoxContainer _controlsTab;
        private VBoxContainer _accessibilityTab;
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            _tabs = new TabContainer
            {
                Position = new Vector2I(200, 80),
                Size = new Vector2I(1520, 800)
            };
            AddChild(_tabs);

            _audioTab = CreateSettingsTab("Audio");
            _graphicsTab = CreateSettingsTab("Graphics");
            _controlsTab = CreateSettingsTab("Controls");
            _accessibilityTab = CreateSettingsTab("Accessibility");

            _tabs.AddChild(_audioTab);
            _tabs.AddChild(_graphicsTab);
            _tabs.AddChild(_controlsTab);
            _tabs.AddChild(_accessibilityTab);

            // Audio settings
            AddSliderSetting(_audioTab, "Master Volume", 0, 100, 80);
            AddSliderSetting(_audioTab, "Music Volume", 0, 100, 70);
            AddSliderSetting(_audioTab, "SFX Volume", 0, 100, 80);
            AddSliderSetting(_audioTab, "Voice Volume", 0, 100, 90);

            // Graphics settings
            AddDropdownSetting(_graphicsTab, "Quality Preset", new[] { "Low", "Medium", "High", "Ultra" }, 1);
            AddDropdownSetting(_graphicsTab, "Resolution Scale", new[] { "50%", "75%", "100%", "150%", "200%" }, 2);
            AddCheckboxSetting(_graphicsTab, "VSync", true);
            AddCheckboxSetting(_graphicsTab, "Shadows", true);
            AddCheckboxSetting(_graphicsTab, "Post Processing", true);

            // Controls settings
            AddSliderSetting(_controlsTab, "Look Sensitivity", 1, 100, 50);
            AddSliderSetting(_controlsTab, "Touch Deadzone", 0, 50, 15);
            AddCheckboxSetting(_controlsTab, "Invert Y-Axis", false);
            AddCheckboxSetting(_controlsTab, "Vibration", true);

            // Accessibility settings
            AddSliderSetting(_accessibilityTab, "UI Scale", 50, 200, 100);
            AddSliderSetting(_accessibilityTab, "Text Size", 50, 200, 100);
            AddCheckboxSetting(_accessibilityTab, "High Contrast Mode", false);
            AddCheckboxSetting(_accessibilityTab, "Reduced Motion", false);
            AddDropdownSetting(_accessibilityTab, "Color Blind Mode", new[] { "None", "Protanopia", "Deuteranopia", "Tritanopia" }, 0);
            AddCheckboxSetting(_accessibilityTab, "Subtitles", true);
            AddCheckboxSetting(_accessibilityTab, "Haptic Feedback", true);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(200, 900),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }

        private VBoxContainer CreateSettingsTab(string title)
        {
            var tab = new VBoxContainer
            {
                Name = title,
                Size = new Vector2I(1500, 750)
            };
            return tab;
        }

        private void AddSliderSetting(VBoxContainer parent, string label, int min, int max, int defaultValue)
        {
            var hbox = new HBoxContainer();
            var lbl = new Label { Text = label, Size = new Vector2I(300, 30) };
            var slider = new HSlider
            {
                MinValue = min,
                MaxValue = max,
                Value = defaultValue,
                Size = new Vector2I(400, 30)
            };
            var valueLabel = new Label { Text = defaultValue.ToString(), Size = new Vector2I(60, 30) };
            slider.ValueChanged += (value) => valueLabel.Text = ((int)value).ToString();
            hbox.AddChild(lbl);
            hbox.AddChild(slider);
            hbox.AddChild(valueLabel);
            parent.AddChild(hbox);
        }

        private void AddDropdownSetting(VBoxContainer parent, string label, string[] options, int defaultIndex)
        {
            var hbox = new HBoxContainer();
            var lbl = new Label { Text = label, Size = new Vector2I(300, 30) };
            var dropdown = new OptionButton { Size = new Vector2I(400, 30) };
            foreach (var opt in options)
                dropdown.AddItem(opt);
            dropdown.Select(defaultIndex);
            hbox.AddChild(lbl);
            hbox.AddChild(dropdown);
            parent.AddChild(hbox);
        }

        private void AddCheckboxSetting(VBoxContainer parent, string label, bool defaultValue)
        {
            var hbox = new HBoxContainer();
            var checkbox = new CheckBox { Text = label, ButtonPressed = defaultValue, Size = new Vector2I(400, 30) };
            hbox.AddChild(checkbox);
            parent.AddChild(hbox);
        }
    }

    // ---------------------------------------------------------------
    // Loading Screen
    // ---------------------------------------------------------------
    public partial class LoadingScreen : UIScreen
    {
        private ProgressBar _progressBar;
        private Label _statusLabel;
        private Label _tipLabel;
        private string[] _tips = {
            "Tip: Explore every corner of the world for hidden treasures.",
            "Tip: Upgrade your gear at workstations to improve stats.",
            "Tip: Complete faction quests to unlock unique rewards.",
            "Tip: Manage your resources wisely when crafting.",
            "Tip: Different enemies have different weaknesses."
        };

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.02f, 0.02f, 0.05f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "LOADING",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 350),
                Size = new Vector2I(1920, 60)
            };
            AddChild(title);

            _progressBar = new ProgressBar
            {
                Position = new Vector2I(460, 450),
                Size = new Vector2I(1000, 30),
                MinValue = 0,
                MaxValue = 100,
                Value = 0
            };
            AddChild(_progressBar);

            _statusLabel = new Label
            {
                Text = "Initializing...",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 500),
                Size = new Vector2I(1920, 30)
            };
            AddChild(_statusLabel);

            _tipLabel = new Label
            {
                Text = _tips[new Random().Next(_tips.Length)],
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 600),
                Size = new Vector2I(1920, 30)
            };
            AddChild(_tipLabel);
        }

        public void SetProgress(float value, string status)
        {
            if (_progressBar != null)
                _progressBar.Value = value;
            if (_statusLabel != null)
                _statusLabel.Text = status;
        }
    }

    // ---------------------------------------------------------------
    // Game Over Screen
    // ---------------------------------------------------------------
    public partial class GameOverScreen : UIScreen
    {
        private Button _retryButton;
        private Button _loadButton;
        private Button _menuButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.1f, 0.02f, 0.02f, 0.9f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "GAME OVER",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 300),
                Size = new Vector2I(1920, 80)
            };
            AddChild(title);

            int buttonY = 450;
            int spacing = 70;
            _retryButton = new Button { Text = "Retry", Position = new Vector2I(760, buttonY), Size = new Vector2I(400, 50) };
            _loadButton = new Button { Text = "Load Save", Position = new Vector2I(760, buttonY + spacing), Size = new Vector2I(400, 50) };
            _menuButton = new Button { Text = "Main Menu", Position = new Vector2I(760, buttonY + spacing * 2), Size = new Vector2I(400, 50) };

            AddChild(_retryButton);
            AddChild(_loadButton);
            AddChild(_menuButton);

            _retryButton.Pressed += () => GetTree().ReloadCurrentScene();
            _loadButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.OpenScreen(ScreenRegistry.SaveLoad);
            };
            _menuButton.Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
        }
    }

    // ---------------------------------------------------------------
    // Save/Load Screen
    // ---------------------------------------------------------------
    public partial class SaveLoadScreen : UIScreen
    {
        private VBoxContainer _slotList;
        private Button _backButton;
        private bool _saveMode = true;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Save / Load",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 50),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            _slotList = new VBoxContainer
            {
                Position = new Vector2I(460, 150),
                Size = new Vector2I(1000, 700)
            };
            AddChild(_slotList);

            for (int i = 1; i <= 10; i++)
            {
                var slotBtn = new Button
                {
                    Text = $"Slot {i} — Empty",
                    Size = new Vector2I(1000, 55)
                };
                int slot = i;
                slotBtn.Pressed += () => OnSlotSelected(slot);
                _slotList.AddChild(slotBtn);
            }

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(460, 900),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }

        private void OnSlotSelected(int slot)
        {
            Logger.Info($"SaveLoadScreen: Slot {slot} selected (mode: {(_saveMode ? "Save" : "Load")})");
            // TODO: Integrate with SaveManager
        }
    }

    // ---------------------------------------------------------------
    // Inventory Screen
    // ---------------------------------------------------------------
    public partial class InventoryScreen : UIScreen
    {
        private GridContainer _itemGrid;
        private Label _infoLabel;
        private Button _sortButton;
        private Button _filterButton;
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Inventory",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            _itemGrid = new GridContainer
            {
                Columns = 6,
                Position = new Vector2I(100, 100),
                Size = new Vector2I(1200, 800)
            };
            AddChild(_itemGrid);

            // Populate with placeholder slots
            for (int i = 0; i < 30; i++)
            {
                var slot = new Panel
                {
                    Size = new Vector2I(180, 180),
                    Theme = CreateSlotTheme()
                };
                _itemGrid.AddChild(slot);
            }

            _infoLabel = new Label
            {
                Text = "Select an item to view details",
                Position = new Vector2I(1400, 100),
                Size = new Vector2I(400, 200),
                AutowrapMode = TextServer.AutowrapMode.Word
            };
            AddChild(_infoLabel);

            _sortButton = new Button { Text = "Sort", Position = new Vector2I(1400, 350), Size = new Vector2I(150, 40) };
            _filterButton = new Button { Text = "Filter", Position = new Vector2I(1570, 350), Size = new Vector2I(150, 40) };
            _backButton = new Button { Text = "Back", Position = new Vector2I(1400, 900), Size = new Vector2I(200, 50) };

            AddChild(_sortButton);
            AddChild(_filterButton);
            AddChild(_backButton);

            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
        }

        private Theme CreateSlotTheme()
        {
            var theme = new Theme();
            var style = new StyleBoxFlat { BgColor = new Color(0.1f, 0.1f, 0.15f), BorderWidthBottom = 1, BorderColor = new Color(0.3f, 0.3f, 0.4f) };
            theme.SetStyleBox("panel", "Panel", style);
            return theme;
        }
    }

    // ---------------------------------------------------------------
    // Equipment Screen
    // ---------------------------------------------------------------
    public partial class EquipmentScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Equipment",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            // Equipment slots
            string[] slots = { "Helmet", "Chest", "Legs", "Boots", "Weapon", "Shield", "Ring 1", "Ring 2", "Amulet", "Gloves" };
            int y = 120;
            foreach (var slot in slots)
            {
                var panel = new Panel
                {
                    Position = new Vector2I(760, y),
                    Size = new Vector2I(400, 60)
                };
                var label = new Label
                {
                    Text = $"[{slot}] — Empty",
                    Position = new Vector2I(10, 15),
                    Size = new Vector2I(380, 30)
                };
                panel.AddChild(label);
                AddChild(panel);
                y += 75;
            }

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(760, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Character Screen
    // ---------------------------------------------------------------
    public partial class CharacterScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Character",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            // Stats display
            string[] stats = { "Level: 1", "HP: 100/100", "MP: 50/50", "Stamina: 100/100", "Strength: 10", "Agility: 10", "Intelligence: 10", "Vitality: 10" };
            int y = 120;
            foreach (var stat in stats)
            {
                var label = new Label
                {
                    Text = stat,
                    Position = new Vector2I(760, y),
                    Size = new Vector2I(400, 35)
                };
                AddChild(label);
                y += 45;
            }

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(760, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Abilities Screen
    // ---------------------------------------------------------------
    public partial class AbilitiesScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Abilities",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            // Ability slots
            string[] abilities = { "Slash", "Fireball", "Heal", "Dash", "Shield", "Arrow" };
            int y = 120;
            foreach (var ability in abilities)
            {
                var panel = new Panel
                {
                    Position = new Vector2I(660, y),
                    Size = new Vector2I(600, 60)
                };
                var label = new Label
                {
                    Text = ability,
                    Position = new Vector2I(10, 15),
                    Size = new Vector2I(400, 30)
                };
                var levelLabel = new Label
                {
                    Text = "Lv.1",
                    Position = new Vector2I(500, 15),
                    Size = new Vector2I(80, 30),
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                panel.AddChild(label);
                panel.AddChild(levelLabel);
                AddChild(panel);
                y += 75;
            }

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(760, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Quest Journal Screen
    // ---------------------------------------------------------------
    public partial class QuestJournalScreen : UIScreen
    {
        private ItemList _questList;
        private RichTextLabel _questDetail;
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Quest Journal",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            _questList = new ItemList
            {
                Position = new Vector2I(100, 100),
                Size = new Vector2I(500, 800)
            };
            _questList.AddItem("Main Quest: The Awakening");
            _questList.AddItem("Side: Lost Artifacts");
            _questList.AddItem("Faction: Guild Introduction");
            _questList.AddItem("Daily: Resource Collection");
            AddChild(_questList);

            _questDetail = new RichTextLayout
            {
                Position = new Vector2I(650, 100),
                Size = new Vector2I(600, 800)
            };
            _questDetail.Text = "Select a quest to view details.";
            AddChild(_questDetail);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(1400, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Map Screen
    // ---------------------------------------------------------------
    public partial class MapScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Map",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            // Map placeholder
            var mapPanel = new Panel
            {
                Position = new Vector2I(200, 100),
                Size = new Vector2I(1200, 800)
            };
            var mapLabel = new Label
            {
                Text = "World Map",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Position = new Vector2I(0, 0),
                Size = new Vector2I(1200, 800)
            };
            mapPanel.AddChild(mapLabel);
            AddChild(mapPanel);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(200, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Crafting Screen
    // ---------------------------------------------------------------
    public partial class CraftingScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Crafting",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            // Recipe list
            var recipeList = new ItemList
            {
                Position = new Vector2I(100, 100),
                Size = new Vector2I(500, 700)
            };
            recipeList.AddItem("Iron Sword (Lv.1 Blacksmith)");
            recipeList.AddItem("Health Potion (Lv.1 Alchemy)");
            recipeList.AddItem("Leather Armor (Lv.2 Tailoring)");
            AddChild(recipeList);

            // Craft button
            var craftButton = new Button
            {
                Text = "Craft",
                Position = new Vector2I(100, 850),
                Size = new Vector2I(200, 50)
            };
            AddChild(craftButton);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(1400, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Trading Screen
    // ---------------------------------------------------------------
    public partial class TradingScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Trade",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            // Merchant inventory
            var merchantLabel = new Label
            {
                Text = "Merchant's Wares",
                Position = new Vector2I(200, 100),
                Size = new Vector2I(400, 30)
            };
            AddChild(merchantLabel);

            var merchantItems = new ItemList
            {
                Position = new Vector2I(200, 140),
                Size = new Vector2I(400, 400)
            };
            merchantItems.AddItem("Iron Ore x10 — 50g");
            merchantItems.AddItem("Health Potion — 25g");
            merchantItems.AddItem("Leather — 30g");
            AddChild(merchantItems);

            // Player inventory
            var playerLabel = new Label
            {
                Text = "Your Items",
                Position = new Vector2I(800, 100),
                Size = new Vector2I(400, 30)
            };
            AddChild(playerLabel);

            var playerItems = new ItemList
            {
                Position = new Vector2I(800, 140),
                Size = new Vector2I(400, 400)
            };
            playerItems.AddItem("Gold: 500g");
            playerItems.AddItem("Wood x20");
            playerItems.AddItem("Stone x15");
            AddChild(playerItems);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(800, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Dialogue Screen
    // ---------------------------------------------------------------
    public partial class DialogueScreen : UIScreen
    {
        private RichTextLabel _dialogueText;
        private VBoxContainer _choiceContainer;
        private Label _speakerLabel;
        private Button _continueButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            // Dim background
            var dim = new ColorRect
            {
                Color = new Color(0, 0, 0, 0.4f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(dim);

            // Dialogue panel at bottom
            var panel = new Panel
            {
                Position = new Vector2I(100, 650),
                Size = new Vector2I(1720, 400)
            };
            AddChild(panel);

            _speakerLabel = new Label
            {
                Text = "NPC",
                Position = new Vector2I(20, 15),
                Size = new Vector2I(400, 30)
            };
            panel.AddChild(_speakerLabel);

            _dialogueText = new RichTextLabel
            {
                Text = "Hello, adventurer!",
                Position = new Vector2I(20, 50),
                Size = new Vector2I(1680, 150),
                AutowrapMode = TextServer.AutowrapMode.Word
            };
            panel.AddChild(_dialogueText);

            _choiceContainer = new VBoxContainer
            {
                Position = new Vector2I(20, 220),
                Size = new Vector2I(1680, 150)
            };
            panel.AddChild(_choiceContainer);

            _continueButton = new Button
            {
                Text = "Continue",
                Position = new Vector2I(1480, 340),
                Size = new Vector2I(200, 40)
            };
            panel.AddChild(_continueButton);
        }

        public void SetDialogue(string speaker, string text, string[] choices)
        {
            if (_speakerLabel != null)
                _speakerLabel.Text = speaker;
            if (_dialogueText != null)
                _dialogueText.Text = text;

            // Clear old choices
            if (_choiceContainer != null)
            {
                foreach (Node child in _choiceContainer.GetChildren())
                    child.QueueFree();

                foreach (var choice in choices)
                {
                    var btn = new Button
                    {
                        Text = choice,
                        Size = new Vector2I(1680, 35)
                    };
                    _choiceContainer.AddChild(btn);
                }
            }
        }
    }

    // ---------------------------------------------------------------
    // Notification History Screen
    // ---------------------------------------------------------------
    public partial class NotificationHistoryScreen : UIScreen
    {
        private ItemList _notificationList;
        private Button _clearButton;
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Notifications",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            _notificationList = new ItemList
            {
                Position = new Vector2I(200, 100),
                Size = new Vector2I(1520, 750)
            };
            _notificationList.AddItem("Quest Updated: The Awakening");
            _notificationList.AddItem("Level Up! You are now Level 2.");
            _notificationList.AddItem("Item Acquired: Iron Sword");
            _notificationList.AddItem("Craft Complete: Health Potion");
            AddChild(_notificationList);

            _clearButton = new Button
            {
                Text = "Clear All",
                Position = new Vector2I(200, 900),
                Size = new Vector2I(200, 50)
            };
            _clearButton.Pressed += () => _notificationList?.Clear();
            AddChild(_clearButton);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(1520, 900),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Bestiary Screen
    // ---------------------------------------------------------------
    public partial class BestiaryScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Bestiary",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            var list = new ItemList
            {
                Position = new Vector2I(200, 100),
                Size = new Vector2I(500, 800)
            };
            list.AddItem("Goblin — Common");
            list.AddItem("Skeleton — Common");
            list.AddItem("Wolf — Common");
            list.AddItem("Dark Knight — Elite");
            list.AddItem("Dragon — Boss");
            AddChild(list);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(200, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Codex Screen
    // ---------------------------------------------------------------
    public partial class CodexScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Codex",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            var list = new ItemList
            {
                Position = new Vector2I(200, 100),
                Size = new Vector2I(500, 800)
            };
            list.AddItem("History of Eternia");
            list.AddItem("The Ancient War");
            list.AddItem("Factions of the Realm");
            list.AddItem("Bestiary: Creatures");
            list.AddItem("Herbology Guide");
            AddChild(list);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(200, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // Achievements Screen
    // ---------------------------------------------------------------
    public partial class AchievementsScreen : UIScreen
    {
        private Button _backButton;

        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "Achievements",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 30),
                Size = new Vector2I(1920, 50)
            };
            AddChild(title);

            var list = new ItemList
            {
                Position = new Vector2I(200, 100),
                Size = new Vector2I(800, 800)
            };
            list.AddItem("First Steps — Complete the tutorial");
            list.AddItem("Goblin Slayer — Defeat 10 goblins");
            list.AddItem("Rich Adventurer — Collect 1000 gold");
            list.AddItem("Crafting Novice — Craft your first item");
            list.AddItem("Explorer — Discover 5 locations");
            AddChild(list);

            _backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(200, 950),
                Size = new Vector2I(200, 50)
            };
            _backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(_backButton);
        }
    }

    // ---------------------------------------------------------------
    // DLC Placeholder Screen
    // ---------------------------------------------------------------
    public partial class DLCPlaceholderScreen : UIScreen
    {
        protected override void OnLazyLoad()
        {
            Size = new Vector2I(1920, 1080);
            AnchorRight = 1;
            AnchorBottom = 1;

            var bg = new ColorRect
            {
                Color = new Color(0.05f, 0.05f, 0.1f, 0.95f),
                Size = Size,
                AnchorRight = 1,
                AnchorBottom = 1
            };
            AddChild(bg);

            var title = new Label
            {
                Text = "DLC Content",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 300),
                Size = new Vector2I(1920, 60)
            };
            AddChild(title);

            var placeholder = new Label
            {
                Text = "Additional content will be available in future updates.",
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new Vector2I(0, 400),
                Size = new Vector2I(1920, 40)
            };
            AddChild(placeholder);

            var backButton = new Button
            {
                Text = "Back",
                Position = new Vector2I(860, 500),
                Size = new Vector2I(200, 50)
            };
            backButton.Pressed += () =>
            {
                var ui = ServiceLocator.Get<UIManager>();
                ui?.CloseScreen();
            };
            AddChild(backButton);
        }
    }
}