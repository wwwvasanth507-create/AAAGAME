using System;
using System.IO;
using System.Linq;
using Godot;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// TestRunner handles automated suite checks for all Phase 3 framework components.
    /// Runs headlessly when '--run-tests' parameter matches.
    /// </summary>
    public partial class TestRunner : Control
    {
        public override void _Ready()
        {
            string[] args = OS.GetCmdlineArgs();
            if (args.Contains("--run-tests"))
            {
                GD.Print("TestRunner: Headless test mode triggered. Starting Phase 3 validation suite...");
                bool success = RunAllTests();
                if (success)
                {
                    GD.Print("TestRunner: ALL FRAMEWORK TESTS PASSED.");
                    GetTree().Quit(0);
                }
                else
                {
                    GD.Print("TestRunner: VALIDATION SUITE ENCOUNTERED FAILURES.");
                    GetTree().Quit(1);
                }
            }
            else
            {
                GD.Print("TestRunner: Boot scene ready. Automated tests skipped.");
            }
        }

        private bool RunAllTests()
        {
            try
            {
                string tempDir = Path.Combine(OS.GetUserDataDir(), "test_sandbox");
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
                Directory.CreateDirectory(tempDir);

                // Initialize ErrorSystem and diagnostics logs
                ErrorSystem.Initialize(tempDir);

                // ==========================================
                // TEST 1: ServiceLocator Registration & Boot Order
                // ==========================================
                GD.Print("Running: ServiceLocator DI & Startup Logging tests...");
                ServiceLocator.Clear();
                
                var pm = new PerformanceManager();
                var sm = new SettingsManager(tempDir);
                var lm = new LocalizationManager();
                var gm = new GameManager();

                ServiceLocator.Register(pm);
                ServiceLocator.Register(sm);
                ServiceLocator.Register(lm);
                ServiceLocator.Register(gm);

                // Fetching resolves lazy initialization and logs performance
                var resolvedPm = ServiceLocator.Get<PerformanceManager>();
                var resolvedSm = ServiceLocator.Get<SettingsManager>();
                var resolvedLm = ServiceLocator.Get<LocalizationManager>();
                var resolvedGm = ServiceLocator.Get<GameManager>();

                if (resolvedPm == null || resolvedSm == null || resolvedLm == null || resolvedGm == null)
                {
                    GD.Print("FAIL: ServiceLocator resolution.");
                    return false;
                }

                // ==========================================
                // TEST 2: SettingsManager Persistence & Reset
                // ==========================================
                GD.Print("Running: SettingsManager Persistence checks...");
                resolvedSm.LoadSettings("", "", "", "");
                
                // Adjust a setting and check automatic save writing
                resolvedSm.SetVolume(0.4f);
                resolvedSm.ApplyGraphicsPreset("LOW");
                
                // Re-initialize manager to load from file
                var checkSm = new SettingsManager(tempDir);
                checkSm.LoadSettings("", "", "", "");
                if (checkSm.MasterVolume != 0.4f || checkSm.QualityPreset != "LOW")
                {
                    GD.Print($"FAIL: Settings persistence values. Got Volume={checkSm.MasterVolume}, Quality={checkSm.QualityPreset}");
                    return false;
                }

                // Reset defaults
                checkSm.ResetToDefaults();
                if (checkSm.MasterVolume != 0.8f || checkSm.QualityPreset != "HIGH")
                {
                    GD.Print("FAIL: Settings reset defaults.");
                    return false;
                }

                // ==========================================
                // TEST 3: ConfigManager & Template Generation
                // ==========================================
                GD.Print("Running: ConfigManager Hot-Reload & templates checks...");
                var configManager = new ConfigManager(tempDir);
                
                // Querying non-existent triggers template creations
                string physicsJson = configManager.GetConfigJson("physics");
                if (string.IsNullOrEmpty(physicsJson) || !physicsJson.Contains("gravity"))
                {
                    GD.Print("FAIL: ConfigManager template creations.");
                    return false;
                }

                // Hot reload test
                configManager.HotReloadAll();

                // ==========================================
                // TEST 4: DeviceDetector Speeds & Recommendations
                // ==========================================
                GD.Print("Running: DeviceDetector checks...");
                var detector = new DeviceDetector();
                detector.DetectDevice();
                string recPreset = detector.GetRecommendedPreset();
                if (string.IsNullOrEmpty(recPreset))
                {
                    GD.Print("FAIL: DeviceDetector preset recommended checks.");
                    return false;
                }

                // ==========================================
                // TEST 5: SaveManager AES-256 and Backup Checksum tests
                // ==========================================
                GD.Print("Running: SaveManager Encryption, Checksum, and Backup validations...");
                var saveManager = new SaveManager(tempDir);
                
                var profile = new SaveProfile();
                profile.Stats.CharacterName = "Vasanth E.";
                profile.Stats.Level = 45;
                profile.StatsData.PlayTimeSeconds = 12500.5;

                // Save slot 0
                if (!saveManager.Save(0, profile))
                {
                    GD.Print("FAIL: SaveManager save profile slot write.");
                    return false;
                }

                // Check file exists
                string mainFile = Path.Combine(tempDir, "slot_0.sav");
                if (!File.Exists(mainFile))
                {
                    GD.Print("FAIL: Save file not created on disk.");
                    return false;
                }

                // Load slot 0 and check values
                var loaded = saveManager.Load(0);
                if (loaded == null || loaded.Stats.CharacterName != "Vasanth E." || loaded.Stats.Level != 45)
                {
                    GD.Print("FAIL: SaveManager load slot values mismatch.");
                    return false;
                }

                // Check Slot Preview Metadata cache
                var preview = saveManager.GetSlotPreview(0);
                if (preview == null || preview.CharacterName != "Vasanth E." || preview.Level != 45 || preview.PlayTimeSeconds != 12500.5)
                {
                    GD.Print("FAIL: SaveManager slot preview cache retrieval.");
                    return false;
                }

                // Check backup file creation
                profile.Stats.Level = 46;
                saveManager.Save(0, profile); // Triggers backup copy of level 45 file to slot_0.bak
                string backupFile = Path.Combine(tempDir, "slot_0.bak");
                if (!File.Exists(backupFile))
                {
                    GD.Print("FAIL: SaveManager backup file .bak not generated.");
                    return false;
                }

                // Corrupt main file (write random bytes) and load to verify backup recovery triggers
                File.WriteAllBytes(mainFile, new byte[] { 0x01, 0x02, 0x03, 0x04 });
                var recovered = saveManager.Load(0);
                if (recovered == null || recovered.Stats.Level != 45)
                {
                    GD.Print($"FAIL: SaveManager backup recovery. Got Level={(recovered != null ? recovered.Stats.Level.ToString() : "Null")}, expected 45");
                    return false;
                }

                // Clean up directory
                Directory.Delete(tempDir, true);
                return true;
            }
            catch (Exception ex)
            {
                GD.Print($"TEST HARNESS CORE EXCEPTION: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
    }
}
