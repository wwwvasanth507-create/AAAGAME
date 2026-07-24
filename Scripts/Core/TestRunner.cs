using System;
using System.IO;
using System.Linq;
using Godot;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// TestRunner is attached to the Boot scene.
    /// Checks for '--run-tests' in the command line args and executes unit tests.
    /// </summary>
    public partial class TestRunner : Control
    {
        public override void _Ready()
        {
            string[] args = OS.GetCmdlineArgs();
            if (args.Contains("--run-tests"))
            {
                GD.Print("TestRunner: Headless test mode triggered. Starting suite...");
                bool success = RunAllTests();
                if (success)
                {
                    GD.Print("TestRunner: ALL TESTS PASSED SUCCESSFULLY.");
                    GetTree().Quit(0);
                }
                else
                {
                    GD.Print("TestRunner: TEST SUITE FAILED.");
                    GetTree().Quit(1);
                }
            }
            else
            {
                GD.Print("TestRunner: Normal boot detected. Skipping automated test run.");
            }
        }

        private bool RunAllTests()
        {
            try
            {
                // Test 1: EventBus Test
                GD.Print("Running: EventBus Test...");
                bool eventFired = false;
                string testPayload = "";
                Action<string> listener = (payload) => {
                    eventFired = true;
                    testPayload = payload;
                };
                EventBus.Subscribe(listener);
                EventBus.Publish("EventBusPayload");
                EventBus.Unsubscribe(listener);
                
                if (!eventFired || testPayload != "EventBusPayload")
                {
                    GD.Print("FAIL: EventBus pub-sub check.");
                    return false;
                }

                // Test 2: Logger Test
                GD.Print("Running: Logger Test...");
                Logger.Info("Test Info Log");
                Logger.Warning("Test Warning Log");
                Logger.Error("Test Error Log");

                // Test 3: GameManager Test
                GD.Print("Running: GameManager Test...");
                var gm = new GameManager();
                gm.Initialize();
                if (gm.CurrentState != GameState.MainMenu)
                {
                    GD.Print($"FAIL: GameManager start state. Got {gm.CurrentState}");
                    return false;
                }
                gm.TransitionTo(GameState.Playing);
                if (gm.CurrentState != GameState.Playing)
                {
                    GD.Print("FAIL: GameManager state transition.");
                    return false;
                }

                // Test 4: SaveManager Test
                GD.Print("Running: SaveManager Test...");
                string tempSaveDir = Path.Combine(OS.GetUserDataDir(), "test_saves");
                var saveManager = new SaveManager(tempSaveDir);
                byte[] mockData = { 0xAA, 0xBB, 0xCC, 0xDD };
                
                // Write slot
                if (!saveManager.SaveSlot(99, mockData))
                {
                    GD.Print("FAIL: SaveManager writing slot.");
                    return false;
                }

                // Read slot
                byte[]? loaded = saveManager.LoadSlot(99);
                if (loaded == null || loaded.Length != 4 || loaded[0] != 0xAA)
                {
                    GD.Print("FAIL: SaveManager loading slot.");
                    return false;
                }

                // Tampering Check
                string savePath = Path.Combine(tempSaveDir, "slot_99.sav");
                byte[] saveBytes = File.ReadAllBytes(savePath);
                // Corrupt data (change last hash byte)
                saveBytes[saveBytes.Length - 1] ^= 0xFF;
                File.WriteAllBytes(savePath, saveBytes);
                
                byte[]? corruptLoaded = saveManager.LoadSlot(99);
                if (corruptLoaded != null)
                {
                    GD.Print("FAIL: SaveManager corrupt data detection (signature bypassed!).");
                    return false;
                }

                // Clean up test saves
                if (File.Exists(savePath)) File.Delete(savePath);
                if (Directory.Exists(tempSaveDir)) Directory.Delete(tempSaveDir);

                // Test 5: SettingsManager Test
                GD.Print("Running: SettingsManager Test...");
                var sm = new SettingsManager();
                sm.LoadSettings("", "", "", "");
                if (sm.QualityPreset != "HIGH" || sm.MasterVolume != 0.8f)
                {
                    GD.Print("FAIL: SettingsManager load values.");
                    return false;
                }
                sm.SetVolume(0.5f);
                if (sm.MasterVolume != 0.5f)
                {
                    GD.Print("FAIL: SettingsManager clamp volume adjustment.");
                    return false;
                }

                // Test 6: PerformanceManager Test
                GD.Print("Running: PerformanceManager Test...");
                var pm = new PerformanceManager();
                pm.Initialize(60.0f);
                if (pm.CurrentResolutionScale != 1.0f)
                {
                    GD.Print("FAIL: PerformanceManager scale start.");
                    return false;
                }
                // Simulate frame times at 10 FPS (long frame delta = 0.1s)
                for (int i = 0; i < 50; i++)
                {
                    pm.ReportFrameTime(0.1);
                }
                if (pm.CurrentResolutionScale >= 1.0f)
                {
                    GD.Print($"FAIL: PerformanceManager dynamic scale lowering. Got {pm.CurrentResolutionScale}");
                    return false;
                }

                // Test 7: InputManager Clamping Test
                GD.Print("Running: InputManager Test...");
                var im = new InputManager();
                im.ProcessJoystickTouch(1.5f, -2.0f);
                if (im.JoystickAxisX != 1.0f || im.JoystickAxisY != -1.0f)
                {
                    GD.Print($"FAIL: InputManager axis clamp checks. Got X={im.JoystickAxisX}, Y={im.JoystickAxisY}");
                    return false;
                }

                // Test 8: UIManager Stack Test
                GD.Print("Running: UIManager Test...");
                var ui = new UIManager();
                ui.PushScreen("MainMenu");
                ui.PushScreen("Settings");
                if (ui.CurrentScreen != "Settings")
                {
                    GD.Print("FAIL: UIManager stack push.");
                    return false;
                }
                ui.PopScreen();
                if (ui.CurrentScreen != "MainMenu")
                {
                    GD.Print("FAIL: UIManager stack pop.");
                    return false;
                }

                // Test 9: LocalizationManager Test
                GD.Print("Running: LocalizationManager Test...");
                var lm = new LocalizationManager();
                lm.Initialize("en");
                if (lm.GetText("MENU_START") != "Launch Operations")
                {
                    GD.Print($"FAIL: LocalizationManager translation key. Got {lm.GetText("MENU_START")}");
                    return false;
                }

                // Test 10: ResourceManager Test
                GD.Print("Running: ResourceManager Test...");
                var rm = new ResourceManager();
                rm.PreloadAsset("res://Test.tscn");
                if (rm.GetAsset("res://Test.tscn") == null)
                {
                    GD.Print("FAIL: ResourceManager load cache.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                GD.Print($"TEST RUN EXCEPTION: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
    }
}
