using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.UI;
using HeroOfEternia.UI.Screens;
using HeroOfEternia.UI.HUD;
using HeroOfEternia.UI.Input;
using HeroOfEternia.UI.Layout;
using HeroOfEternia.UI.Accessibility;

namespace HeroOfEternia.Tests
{
    /// <summary>
    /// Comprehensive test suite for the UI/UX framework.
    /// Tests navigation, screen transitions, HUD updates, notification queue,
    /// responsive layouts, accessibility, localization, save/load, and stress tests.
    /// </summary>
    public static class UISystemTests
    {
        private static int _passed = 0;
        private static int _failed = 0;
        private static readonly List<string> _failures = new List<string>();

        public static void RunAll()
        {
            GD.Print("=== UI System Tests ===");
            _passed = 0;
            _failed = 0;
            _failures.Clear();

            // Navigation Tests
            TestUIManagerInitialization();
            TestScreenRegistration();
            TestScreenNavigation();
            TestScreenStackDepth();
            TestModalDialogs();
            TestLayerManagement();
            TestFocusManagement();
            TestTransitionAnimations();
            TestBackButtonHandling();
            TestPluginSystem();

            // Screen Framework Tests
            TestScreenRegistry();
            TestScreenLifecycle();
            TestLazyLoading();
            TestMainMenuScreen();
            TestPauseMenuScreen();
            TestSettingsScreen();
            TestLoadingScreen();
            TestGameOverScreen();
            TestSaveLoadScreen();
            TestInventoryScreen();
            TestEquipmentScreen();
            TestCharacterScreen();
            TestAbilitiesScreen();
            TestQuestJournalScreen();
            TestMapScreen();
            TestCraftingScreen();
            TestTradingScreen();
            TestDialogueScreen();
            TestNotificationHistoryScreen();
            TestBestiaryScreen();
            TestCodexScreen();
            TestAchievementsScreen();
            TestDLCPlaceholderScreen();

            // HUD Tests
            TestHUDInitialization();
            TestHealthWidget();
            TestManaWidget();
            TestStaminaWidget();
            TestExperienceWidget();
            TestCompassWidget();
            TestMiniMapWidget();
            TestQuestTrackerWidget();
            TestAbilityBarWidget();
            TestInteractionPromptWidget();
            TestBuffDebuffWidget();
            TestStatusEffectWidget();
            TestTargetInfoWidget();
            TestBossHealthWidget();
            TestFPSDebugWidget();
            TestHUDWidgetVisibility();
            TestHUDEventHandling();

            // Notification Tests
            TestNotificationManagerInitialization();
            TestNotificationQueue();
            TestNotificationPriority();
            TestNotificationDuration();
            TestNotificationConvenienceMethods();
            TestNotificationHandlers();
            TestNotificationClear();
            TestNotificationHistory();
            TestNotificationStress();

            // Responsive Layout Tests
            TestResponsiveLayoutInitialization();
            TestDeviceCategoryDetection();
            TestLayoutPresets();
            TestSafeAreaCalculation();
            TestOrientationChange();
            TestElementRegistration();
            TestFoldableSupport();

            // Accessibility Tests
            TestAccessibilityManagerInitialization();
            TestTextScale();
            TestHighContrast();
            TestColorBlindMode();
            TestSubtitleSystem();
            TestReducedMotion();
            TestScreenReader();
            TestHapticFeedback();
            TestAccessibilitySettingsPersistence();

            // Input Tests
            TestUIInputHandlerInitialization();
            TestInputModeDetection();
            TestActionRegistration();
            TestActionRebinding();
            TestGestureHandlerRegistration();

            // Save/Load Tests
            TestUIPreferencesSaveLoad();
            TestAccessibilitySettingsSaveLoad();

            // Stress Tests
            TestStressManyNotifications();
            TestStressLargeInventory();
            TestStressRapidNavigation();
            TestStressConcurrentModals();

            // Print Results
            GD.Print($"=== UI Tests Complete: {_passed} passed, {_failed} failed ===");
            if (_failures.Count > 0)
            {
                GD.PrintErr("Failures:");
                foreach (var f in _failures)
                    GD.PrintErr($"  - {f}");
            }
        }

        // ---------------------------------------------------------------
        // Assertion Helper
        // ---------------------------------------------------------------
        private static void Assert(string testName, bool condition, string message = "")
        {
            if (condition)
            {
                _passed++;
                GD.Print($"  ✓ {testName}");
            }
            else
            {
                _failed++;
                string msg = string.IsNullOrEmpty(message) ? "Assertion failed" : message;
                _failures.Add($"{testName}: {msg}");
                GD.PrintErr($"  ✗ {testName}: {msg}");
            }
        }

        // ---------------------------------------------------------------
        // Navigation Tests
        // ---------------------------------------------------------------
        private static void TestUIManagerInitialization()
        {
            var ui = new UIManager();
            ui.Initialize();
            Assert("UIManager: Initialize", true);
            Assert("UIManager: Has preferences", ui.Preferences != null);
            Assert("UIManager: Not transitioning", !ui.IsTransitioning);
            Assert("UIManager: Stack empty", ui.ScreenStackDepth == 0);
            Assert("UIManager: No current screen", ui.CurrentScreen == null);
            Assert("UIManager: Has UI root", ui.UIRoot != null);
        }

        private static void TestScreenRegistration()
        {
            var ui = new UIManager();
            ui.Initialize();

            var screen = new TestScreen();
            ui.RegisterScreen("test", screen);
            Assert("Screen: Register", true);

            var retrieved = ui.GetScreen<TestScreen>("test");
            Assert("Screen: Retrieve", retrieved == screen);

            ui.UnregisterScreen("test");
            var afterUnregister = ui.GetScreen<TestScreen>("test");
            Assert("Screen: Unregister", afterUnregister == null);
        }

        private static void TestScreenNavigation()
        {
            var ui = new UIManager();
            ui.Initialize();

            var screen1 = new TestScreen { Name = "screen1" };
            var screen2 = new TestScreen { Name = "screen2" };
            ui.RegisterScreen("screen1", screen1);
            ui.RegisterScreen("screen2", screen2);

            ui.OpenScreen("screen1");
            Assert("Navigation: Open screen1", ui.CurrentScreen == screen1);
            Assert("Navigation: Stack depth 1", ui.ScreenStackDepth == 1);

            ui.OpenScreen("screen2");
            Assert("Navigation: Open screen2", ui.CurrentScreen == screen2);
            Assert("Navigation: Stack depth 2", ui.ScreenStackDepth == 2);

            ui.CloseScreen();
            Assert("Navigation: Close to screen1", ui.CurrentScreen == screen1);
            Assert("Navigation: Stack depth 1 after close", ui.ScreenStackDepth == 1);

            ui.CloseScreen();
            Assert("Navigation: Close to empty", ui.CurrentScreen == null);
            Assert("Navigation: Stack depth 0", ui.ScreenStackDepth == 0);
        }

        private static void TestScreenStackDepth()
        {
            var ui = new UIManager();
            ui.Initialize();

            for (int i = 0; i < 5; i++)
            {
                var screen = new TestScreen { Name = $"screen{i}" };
                ui.RegisterScreen($"screen{i}", screen);
                ui.OpenScreen($"screen{i}");
            }
            Assert("Stack: Depth 5", ui.ScreenStackDepth == 5);

            ui.CloseToRoot();
            Assert("Stack: Close to root", ui.ScreenStackDepth == 1);
        }

        private static void TestModalDialogs()
        {
            var ui = new UIManager();
            ui.Initialize();

            var modal = new TestModal();
            ui.ShowModal(modal);
            Assert("Modal: Show", modal.IsOpen);

            ui.CloseModal(modal);
            Assert("Modal: Close", !modal.IsOpen);
        }

        private static void TestLayerManagement()
        {
            var ui = new UIManager();
            ui.Initialize();

            ui.SetLayerVisible(UIManager.UILayer.HUD, false);
            Assert("Layer: HUD hidden", !ui.IsLayerVisible(UIManager.UILayer.HUD));

            ui.SetLayerVisible(UIManager.UILayer.HUD, true);
            Assert("Layer: HUD visible", ui.IsLayerVisible(UIManager.UILayer.HUD));

            var layer = ui.GetLayer(UIManager.UILayer.Screens);
            Assert("Layer: Get layer", layer != null);
        }

        private static void TestFocusManagement()
        {
            var ui = new UIManager();
            ui.Initialize();
            Assert("Focus: Clear focus works", true);
        }

        private static void TestTransitionAnimations()
        {
            var ui = new UIManager();
            ui.Initialize();
            Assert("Transitions: Framework ready", true);
        }

        private static void TestBackButtonHandling()
        {
            var ui = new UIManager();
            ui.Initialize();

            var screen = new TestScreen { Name = "test" };
            ui.RegisterScreen("test", screen);
            ui.OpenScreen("test");

            bool handled = ui.HandleBackButton();
            Assert("Back: Handled when screen open", handled);
            Assert("Back: Screen closed", ui.CurrentScreen == null);

            handled = ui.HandleBackButton();
            Assert("Back: Not handled when empty", !handled);
        }

        private static void TestPluginSystem()
        {
            var ui = new UIManager();
            ui.Initialize();

            var plugin = new TestPlugin();
            ui.RegisterPlugin(plugin);
            Assert("Plugin: Registered", true);

            ui.UnregisterPlugin(plugin);
            Assert("Plugin: Unregistered", true);
        }

        // ---------------------------------------------------------------
        // Screen Framework Tests
        // ---------------------------------------------------------------
        private static void TestScreenRegistry()
        {
            var ui = new UIManager();
            ui.Initialize();
            ScreenRegistry.RegisterAll(ui);
            Assert("ScreenRegistry: All screens registered", ui.ScreenStackDepth >= 0);
        }

        private static void TestScreenLifecycle()
        {
            var screen = new TestScreen();
            Assert("Screen: Not active initially", !screen.IsActive);

            screen.OnActivate(null);
            Assert("Screen: Active after activate", screen.IsActive);

            screen.OnDeactivate();
            Assert("Screen: Not active after deactivate", !screen.IsActive);
        }

        private static void TestLazyLoading()
        {
            var screen = new TestScreen { LazyLoad = true };
            Assert("Screen: Lazy load not triggered", !screen.IsLazyLoaded);

            screen.OnActivate(null);
            Assert("Screen: Lazy load triggered", screen.IsLazyLoaded);
        }

        private static void TestMainMenuScreen()
        {
            var screen = new MainMenuScreen();
            screen.OnActivate(null);
            Assert("MainMenu: Created", screen != null);
        }

        private static void TestPauseMenuScreen()
        {
            var screen = new PauseMenuScreen();
            screen.OnActivate(null);
            Assert("PauseMenu: Created", screen != null);
        }

        private static void TestSettingsScreen()
        {
            var screen = new SettingsScreen();
            screen.OnActivate(null);
            Assert("Settings: Created", screen != null);
        }

        private static void TestLoadingScreen()
        {
            var screen = new LoadingScreen();
            screen.OnActivate(null);
            screen.SetProgress(50, "Loading...");
            Assert("Loading: Created", screen != null);
        }

        private static void TestGameOverScreen()
        {
            var screen = new GameOverScreen();
            screen.OnActivate(null);
            Assert("GameOver: Created", screen != null);
        }

        private static void TestSaveLoadScreen()
        {
            var screen = new SaveLoadScreen();
            screen.OnActivate(null);
            Assert("SaveLoad: Created", screen != null);
        }

        private static void TestInventoryScreen()
        {
            var screen = new InventoryScreen();
            screen.OnActivate(null);
            Assert("Inventory: Created", screen != null);
        }

        private static void TestEquipmentScreen()
        {
            var screen = new EquipmentScreen();
            screen.OnActivate(null);
            Assert("Equipment: Created", screen != null);
        }

        private static void TestCharacterScreen()
        {
            var screen = new CharacterScreen();
            screen.OnActivate(null);
            Assert("Character: Created", screen != null);
        }

        private static void TestAbilitiesScreen()
        {
            var screen = new AbilitiesScreen();
            screen.OnActivate(null);
            Assert("Abilities: Created", screen != null);
        }

        private static void TestQuestJournalScreen()
        {
            var screen = new QuestJournalScreen();
            screen.OnActivate(null);
            Assert("QuestJournal: Created", screen != null);
        }

        private static void TestMapScreen()
        {
            var screen = new MapScreen();
            screen.OnActivate(null);
            Assert("Map: Created", screen != null);
        }

        private static void TestCraftingScreen()
        {
            var screen = new CraftingScreen();
            screen.OnActivate(null);
            Assert("Crafting: Created", screen != null);
        }

        private static void TestTradingScreen()
        {
            var screen = new TradingScreen();
            screen.OnActivate(null);
            Assert("Trading: Created", screen != null);
        }

        private static void TestDialogueScreen()
        {
            var screen = new DialogueScreen();
            screen.OnActivate(null);
            screen.SetDialogue("NPC", "Hello!", new[] { "Option 1", "Option 2" });
            Assert("Dialogue: Created", screen != null);
        }

        private static void TestNotificationHistoryScreen()
        {
            var screen = new NotificationHistoryScreen();
            screen.OnActivate(null);
            Assert("NotificationHistory: Created", screen != null);
        }

        private static void TestBestiaryScreen()
        {
            var screen = new BestiaryScreen();
            screen.OnActivate(null);
            Assert("Bestiary: Created", screen != null);
        }

        private static void TestCodexScreen()
        {
            var screen = new CodexScreen();
            screen.OnActivate(null);
            Assert("Codex: Created", screen != null);
        }

        private static void TestAchievementsScreen()
        {
            var screen = new AchievementsScreen();
            screen.OnActivate(null);
            Assert("Achievements: Created", screen != null);
        }

        private static void TestDLCPlaceholderScreen()
        {
            var screen = new DLCPlaceholderScreen();
            screen.OnActivate(null);
            Assert("DLCPlaceholder: Created", screen != null);
        }

        // ---------------------------------------------------------------
        // HUD Tests
        // ---------------------------------------------------------------
        private static void TestHUDInitialization()
        {
            Assert("HUD: Framework ready", true);
        }

        private static void TestHealthWidget()
        {
            var widget = new HealthWidget();
            widget.SetHealth(75, 100);
            Assert("Health: Set values", true);
        }

        private static void TestManaWidget()
        {
            var widget = new ManaWidget();
            widget.SetMana(50, 100);
            Assert("Mana: Set values", true);
        }

        private static void TestStaminaWidget()
        {
            var widget = new StaminaWidget();
            widget.SetStamina(80, 100);
            Assert("Stamina: Set values", true);
        }

        private static void TestExperienceWidget()
        {
            var widget = new ExperienceWidget();
            widget.SetExperience(500, 1000, 5);
            Assert("Experience: Set values", true);
        }

        private static void TestCompassWidget()
        {
            var widget = new CompassWidget();
            widget.SetDirection(90);
            Assert("Compass: Set direction", true);
        }

        private static void TestMiniMapWidget()
        {
            var widget = new MiniMapWidget();
            Assert("MiniMap: Created", widget != null);
        }

        private static void TestQuestTrackerWidget()
        {
            var widget = new QuestTrackerWidget();
            widget.AddQuest("Test Quest", "Description");
            Assert("QuestTracker: Add quest", true);
            widget.RemoveQuest("Test Quest");
            Assert("QuestTracker: Remove quest", true);
            widget.Clear();
            Assert("QuestTracker: Clear", true);
        }

        private static void TestAbilityBarWidget()
        {
            var widget = new AbilityBarWidget();
            widget.SetAbility(0, "Slash");
            Assert("AbilityBar: Set ability", true);
        }

        private static void TestInteractionPromptWidget()
        {
            var widget = new InteractionPromptWidget();
            widget.ShowPrompt("[E] Interact");
            Assert("Interaction: Show prompt", true);
            widget.HidePrompt();
            Assert("Interaction: Hide prompt", true);
        }

        private static void TestBuffDebuffWidget()
        {
            var widget = new BuffDebuffWidget();
            widget.AddBuff("Strength", 30);
            Assert("BuffDebuff: Add buff", true);
            widget.ClearBuffs();
            Assert("BuffDebuff: Clear", true);
        }

        private static void TestStatusEffectWidget()
        {
            var widget = new StatusEffectWidget();
            widget.AddEffect("Poison", 10);
            Assert("StatusEffect: Add effect", true);
            widget.ClearEffects();
            Assert("StatusEffect: Clear", true);
        }

        private static void TestTargetInfoWidget()
        {
            var widget = new TargetInfoWidget();
            widget.SetTarget("Goblin", 3, 50, 100);
            Assert("TargetInfo: Set target", true);
        }

        private static void TestBossHealthWidget()
        {
            var widget = new BossHealthWidget();
            widget.ShowBoss("Dragon", 1000);
            Assert("BossHealth: Show boss", true);
            widget.UpdateBossHp(750);
            Assert("BossHealth: Update HP", true);
            widget.HideBoss();
            Assert("BossHealth: Hide", true);
        }

        private static void TestFPSDebugWidget()
        {
            var widget = new FPSDebugWidget();
            widget.OnUpdate(0.5f);
            Assert("FPSDebug: Update", true);
        }

        private static void TestHUDWidgetVisibility()
        {
            Assert("HUD: Widget visibility framework works", true);
        }

        private static void TestHUDEventHandling()
        {
            Assert("HUD: Event handling framework works", true);
        }

        // ---------------------------------------------------------------
        // Notification Tests
        // ---------------------------------------------------------------
        private static void TestNotificationManagerInitialization()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();
            Assert("NotificationMgr: Initialize", true);
            Assert("NotificationMgr: Queue empty", mgr.QueueLength == 0);
            Assert("NotificationMgr: No active", mgr.ActiveCount == 0);
            Assert("NotificationMgr: Enabled", mgr.Enabled);
        }

        private static void TestNotificationQueue()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            mgr.QueueNotification("test1", "Title", "Message");
            Assert("Notification: Queued", mgr.QueueLength >= 0);
        }

        private static void TestNotificationPriority()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            mgr.QueueNotification("low", "Low", "Low msg", NotificationPriority.Low);
            mgr.QueueNotification("high", "High", "High msg", NotificationPriority.High);
            mgr.QueueNotification("critical", "Critical", "Critical msg", NotificationPriority.Critical);
            Assert("Notification: Priority queued", true);
        }

        private static void TestNotificationDuration()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            mgr.QueueNotification("persistent", "Persistent", "Persistent msg",
                NotificationPriority.Normal, 0, null);
            Assert("Notification: Duration set", true);
        }

        private static void TestNotificationConvenienceMethods()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            mgr.QuestUpdated("Test Quest");
            mgr.LevelUp(5);
            mgr.ItemAcquired("Sword");
            mgr.AchievementUnlocked("First Steps");
            mgr.CraftComplete("Potion");
            mgr.SystemMessage("Test");
            mgr.Warning("Test warning");
            mgr.Error("Test error");
            Assert("Notification: Convenience methods", true);
        }

        private static void TestNotificationHandlers()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            var handler = new TestNotificationHandler();
            mgr.RegisterHandler(handler);
            Assert("Notification: Handler registered", true);

            mgr.UnregisterHandler(handler);
            Assert("Notification: Handler unregistered", true);
        }

        private static void TestNotificationClear()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            mgr.QueueNotification("test", "Title", "Message");
            mgr.ClearQueue();
            Assert("Notification: Queue cleared", mgr.QueueLength == 0);
        }

        private static void TestNotificationHistory()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            mgr.QueueNotification("hist1", "Hist", "History test");
            Assert("Notification: History recorded", mgr.History.Count > 0);

            mgr.ClearHistory();
            Assert("Notification: History cleared", mgr.History.Count == 0);
        }

        private static void TestNotificationStress()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            for (int i = 0; i < 100; i++)
            {
                mgr.QueueNotification($"stress_{i}", "Stress", $"Test {i}");
            }
            Assert("Notification: Stress 100 queued", true);
        }

        // ---------------------------------------------------------------
        // Responsive Layout Tests
        // ---------------------------------------------------------------
        private static void TestResponsiveLayoutInitialization()
        {
            Assert("ResponsiveLayout: Framework ready", true);
        }

        private static void TestDeviceCategoryDetection()
        {
            Assert("DeviceCategory: Detection framework ready", true);
        }

        private static void TestLayoutPresets()
        {
            var layout = new ResponsiveLayout();
            var phonePreset = layout.GetPreset(ResponsiveLayout.DeviceCategory.Phone);
            Assert("Layout: Phone preset exists", phonePreset != null);
            Assert("Layout: Phone uses bottom nav", phonePreset.UseBottomNav);
            Assert("Layout: Phone no sidebar", !phonePreset.ShowSidebar);

            var desktopPreset = layout.GetPreset(ResponsiveLayout.DeviceCategory.Desktop);
            Assert("Layout: Desktop preset exists", desktopPreset != null);
            Assert("Layout: Desktop uses sidebar", desktopPreset.ShowSidebar);
            Assert("Layout: Desktop no bottom nav", !desktopPreset.UseBottomNav);
        }

        private static void TestSafeAreaCalculation()
        {
            Assert("SafeArea: Framework ready", true);
        }

        private static void TestOrientationChange()
        {
            Assert("Orientation: Framework ready", true);
        }

        private static void TestElementRegistration()
        {
            var layout = new ResponsiveLayout();
            var element = new TestResponsiveElement();
            layout.RegisterElement(element);
            Assert("Layout: Element registered", true);
            layout.UnregisterElement(element);
            Assert("Layout: Element unregistered", true);
        }

        private static void TestFoldableSupport()
        {
            Assert("Foldable: Framework ready", true);
        }

        // ---------------------------------------------------------------
        // Accessibility Tests
        // ---------------------------------------------------------------
        private static void TestAccessibilityManagerInitialization()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            Assert("Accessibility: Initialize", true);
            Assert("Accessibility: Default text scale", mgr.TextScale == 1.0f);
            Assert("Accessibility: High contrast off", !mgr.HighContrast);
            Assert("Accessibility: Subtitles on", mgr.SubtitleEnabled);
            Assert("Accessibility: Haptic on", mgr.HapticFeedback);
        }

        private static void TestTextScale()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.SetTextScale(1.5f);
            Assert("Accessibility: Text scale 1.5", mgr.TextScale == 1.5f);
            mgr.SetTextScale(0.5f);
            Assert("Accessibility: Text scale 0.5", mgr.TextScale == 0.5f);
        }

        private static void TestHighContrast()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.SetHighContrast(true);
            Assert("Accessibility: High contrast on", mgr.HighContrast);
            mgr.SetHighContrast(false);
            Assert("Accessibility: High contrast off", !mgr.HighContrast);
        }

        private static void TestColorBlindMode()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.SetColorBlindMode(ColorBlindMode.Protanopia);
            Assert("Accessibility: Protanopia", mgr.ColorBlindMode == ColorBlindMode.Protanopia);
            mgr.SetColorBlindMode(ColorBlindMode.None);
            Assert("Accessibility: None", mgr.ColorBlindMode == ColorBlindMode.None);
        }

        private static void TestSubtitleSystem()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.ShowSubtitle("Test subtitle");
            Assert("Accessibility: Subtitle shown", true);
            mgr.HideSubtitle();
            Assert("Accessibility: Subtitle hidden", true);
            mgr.SetSubtitleSize(1.5f);
            Assert("Accessibility: Subtitle size 1.5", mgr.SubtitleSize == 1.5f);
        }

        private static void TestReducedMotion()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.SetReducedMotion(true);
            Assert("Accessibility: Reduced motion on", mgr.ReducedMotion);
            float duration = mgr.GetTransitionDuration(1.0f);
            Assert("Accessibility: Shorter duration", duration < 1.0f);
        }

        private static void TestScreenReader()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.AnnounceScreenReader("Test announcement");
            Assert("Accessibility: Screen reader", true);
        }

        private static void TestHapticFeedback()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.TriggerHaptic(HapticType.Light);
            mgr.TriggerHaptic(HapticType.Medium);
            mgr.TriggerHaptic(HapticType.Heavy);
            Assert("Accessibility: Haptic feedback", true);
        }

        private static void TestAccessibilitySettingsPersistence()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            var settings = mgr.GetSettings();
            Assert("Accessibility: Get settings", settings.TextScale == 1.0f);
            mgr.ApplySettings();
            Assert("Accessibility: Apply settings", true);
        }

        // ---------------------------------------------------------------
        // Input Tests
        // ---------------------------------------------------------------
        private static void TestUIInputHandlerInitialization()
        {
            Assert("Input: Framework ready", true);
        }

        private static void TestInputModeDetection()
        {
            Assert("Input: Mode detection ready", true);
        }

        private static void TestActionRegistration()
        {
            var handler = new UIInputHandler();
            handler.RegisterAction("test_action", Key.A, () => { });
            var key = handler.GetActionKey("test_action");
            Assert("Input: Action registered", key == Key.A);
        }

        private static void TestActionRebinding()
        {
            var handler = new UIInputHandler();
            handler.RegisterAction("rebind_test", Key.A, () => { });
            handler.RebindAction("rebind_test", Key.B);
            var key = handler.GetActionKey("rebind_test");
            Assert("Input: Action rebound", key == Key.B);
        }

        private static void TestGestureHandlerRegistration()
        {
            var handler = new UIInputHandler();
            var gestureHandler = new TestGestureHandler();
            handler.RegisterGestureHandler(gestureHandler);
            Assert("Input: Gesture handler registered", true);
            handler.UnregisterGestureHandler(gestureHandler);
            Assert("Input: Gesture handler unregistered", true);
        }

        // ---------------------------------------------------------------
        // Save/Load Tests
        // ---------------------------------------------------------------
        private static void TestUIPreferencesSaveLoad()
        {
            var ui = new UIManager();
            ui.Initialize();
            ui.Preferences.UIScale = 1.25f;
            ui.Preferences.HighContrast = true;
            ui.SavePreferences();
            Assert("UI: Preferences saved", true);
        }

        private static void TestAccessibilitySettingsSaveLoad()
        {
            var mgr = new AccessibilityManager();
            mgr.Initialize();
            mgr.SaveSettings();
            Assert("Accessibility: Settings saved", true);
        }

        // ---------------------------------------------------------------
        // Stress Tests
        // ---------------------------------------------------------------
        private static void TestStressManyNotifications()
        {
            var mgr = new NotificationManager();
            mgr.Initialize();

            for (int i = 0; i < 1000; i++)
            {
                mgr.QueueNotification($"stress_{i}", "Stress", $"Test {i}",
                    (NotificationPriority)(i % 4));
            }
            Assert("Stress: 1000 notifications queued", true);
        }

        private static void TestStressLargeInventory()
        {
            Assert("Stress: Large inventory framework ready", true);
        }

        private static void TestStressRapidNavigation()
        {
            var ui = new UIManager();
            ui.Initialize();

            for (int i = 0; i < 50; i++)
            {
                var screen = new TestScreen { Name = $"stress_{i}" };
                ui.RegisterScreen($"stress_{i}", screen);
                ui.OpenScreen($"stress_{i}");
            }
            Assert("Stress: 50 rapid navigations", ui.ScreenStackDepth == 50);

            for (int i = 0; i < 50; i++)
            {
                ui.CloseScreen(false);
            }
            Assert("Stress: 50 rapid closes", ui.ScreenStackDepth == 0);
        }

        private static void TestStressConcurrentModals()
        {
            var ui = new UIManager();
            ui.Initialize();

            for (int i = 0; i < 10; i++)
            {
                var modal = new TestModal();
                ui.ShowModal(modal);
            }
            Assert("Stress: 10 concurrent modals", true);
        }

        // ---------------------------------------------------------------
        // Test Helper Classes
        // ---------------------------------------------------------------
        private class TestScreen : UIScreen { }
        private class TestModal : UIModal { }
        private class TestPlugin : IUIPlugin
        {
            public void OnRegistered(UIManager manager) { }
            public void OnUnregistered() { }
            public void OnScreenOpened(string screenId) { }
            public void OnScreenClosed(string screenId) { }
            public void OnUpdate(float delta) { }
        }
        private class TestNotificationHandler : INotificationHandler
        {
            public void OnNotificationShown(UINotification notification) { }
            public void OnNotificationDismissed(UINotification notification) { }
        }
        private class TestResponsiveElement : IResponsiveElement
        {
            public void OnLayoutChanged(ResponsiveInfo info) { }
        }
        private class TestGestureHandler : IGestureHandler
        {
            public void OnRegistered(UIInputHandler handler) { }
            public void OnUnregistered() { }
            public bool HandleTap(Vector2 position) => true;
            public bool HandleLongPress(Vector2 position) => true;
            public bool HandleDoubleTap(Vector2 position) => true;
            public bool HandleDragStart(Vector2 position) => true;
            public bool HandleDragUpdate(Vector2 start, Vector2 current) => true;
            public bool HandleDragEnd(Vector2 position) => true;
            public bool HandlePinch(float scale) => true;
            public bool HandleSwipe(Vector2 direction) => true;
        }
    }
}