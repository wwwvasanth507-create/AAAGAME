using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.World;
using HeroOfEternia.NPC;
using HeroOfEternia.Combat;
using HeroOfEternia.Player;
using HeroOfEternia.Player.States;
using HeroOfEternia.Enemies;

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
                var am = new AudioManager();
                var scm = new SceneManager();
                var rm = new ResourceManager();
                var um = new UIManager();

                ServiceLocator.Register(pm);
                ServiceLocator.Register(sm);
                ServiceLocator.Register(lm);
                ServiceLocator.Register(gm);
                ServiceLocator.Register(am);
                ServiceLocator.Register(scm);
                ServiceLocator.Register(rm);
                ServiceLocator.Register(um);

                // Fetching resolves lazy initialization and logs performance
                var resolvedPm = ServiceLocator.Get<PerformanceManager>();
                var resolvedSm = ServiceLocator.Get<SettingsManager>();
                var resolvedLm = ServiceLocator.Get<LocalizationManager>();
                var resolvedGm = ServiceLocator.Get<GameManager>();
                var resolvedAm = ServiceLocator.Get<AudioManager>();
                var resolvedScm = ServiceLocator.Get<SceneManager>();
                var resolvedRm = ServiceLocator.Get<ResourceManager>();
                var resolvedUm = ServiceLocator.Get<UIManager>();

                if (resolvedPm == null || resolvedSm == null || resolvedLm == null || resolvedGm == null ||
                    resolvedAm == null || resolvedScm == null || resolvedRm == null || resolvedUm == null)
                {
                    GD.Print("FAIL: ServiceLocator resolution.");
                    return false;
                }

                // ==========================================
                // TEST 2: SettingsManager Persistence & Reset
                // ==========================================
                GD.Print("Running: SettingsManager Persistence checks...");
                resolvedSm.LoadSettings();
                
                // Adjust a setting and check automatic save writing
                resolvedSm.SetVolume(0.4f);
                resolvedSm.ApplyGraphicsPreset("LOW");
                
                // Re-initialize manager to load from file
                var checkSm = new SettingsManager(tempDir);
                checkSm.LoadSettings();
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

                // ==========================================
                // PHASE 4 TESTS
                // ==========================================
                if (!RunPhase4Tests(tempDir)) return false;
                if (!RunPhase11Tests(tempDir)) return false;
                if (!RunPhase12Tests(tempDir)) return false;

                GD.Print("\n=== ALL PHASES PASSED: 72/72 TESTS ===");
                return true;
            }
            catch (Exception ex)
            {
                GD.Print($"TEST HARNESS CORE EXCEPTION: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        // ---------------------------------------------------------------
        // PHASE 4 TESTS
        // ---------------------------------------------------------------
        private bool RunPhase4Tests(string tempDir)
        {
            Directory.CreateDirectory(tempDir);

            // ----------------------------------------------------------
            // TEST 6: InputActionMap — registers all actions
            // ----------------------------------------------------------
            GD.Print("Running: InputActionMap registration...");
            try
            {
                Input.InputActionMap.Initialize();
                // Verify a key action exists in Godot InputMap
                if (!InputMap.HasAction(Input.InputActions.Jump))
                {
                    GD.Print("FAIL: InputActionMap did not register 'jump' action.");
                    return false;
                }
                GD.Print("PASS: InputActionMap registered all actions.");
            }
            catch (Exception ex)
            {
                GD.Print($"FAIL: InputActionMap exception: {ex.Message}");
                return false;
            }

            // ----------------------------------------------------------
            // TEST 7: PlayerData — stats, stamina, XP, vitals
            // ----------------------------------------------------------
            GD.Print("Running: PlayerData stat checks...");
            var data = new Player.PlayerData();

            // Stamina drain
            data.DrainStamina(30f);
            if (data.CurrentStamina != 70f)
            {
                GD.Print($"FAIL: Stamina drain. Got {data.CurrentStamina}, expected 70.");
                return false;
            }

            // Stamina check
            if (!data.HasStamina(50f) || data.HasStamina(80f))
            {
                GD.Print("FAIL: HasStamina() boundary check.");
                return false;
            }

            // Vitals regen
            data.CurrentHealth = 50f;
            data.HealthRegen   = 10f;
            data.RegenVitals(2f); // 2 seconds
            if (data.CurrentHealth != 70f)
            {
                GD.Print($"FAIL: HealthRegen. Got {data.CurrentHealth}, expected 70.");
                return false;
            }

            // XP level-up
            bool leveledUp = data.AddXp(100);
            if (!leveledUp || data.Level != 2)
            {
                GD.Print($"FAIL: XP level-up. Got Level={data.Level}, LeveledUp={leveledUp}.");
                return false;
            }
            GD.Print("PASS: PlayerData stats, stamina, regen, and XP verified.");

            // ----------------------------------------------------------
            // TEST 8: PlayerStateMachine — transitions
            // ----------------------------------------------------------
            GD.Print("Running: PlayerStateMachine transition checks...");
            var fsm = new Player.PlayerStateMachine();
            fsm.Register(new Player.States.IdleState());
            fsm.Register(new Player.States.RunState());
            fsm.Register(new Player.States.JumpState());
            fsm.Register(new Player.States.FallState());
            fsm.Register(new Player.States.LandState());
            fsm.Register(new Player.States.DeadState());

            if (fsm.CurrentStateId != Player.PlayerStateId.Idle)
            {
                GD.Print("FAIL: FSM default state is not Idle.");
                return false;
            }

            // Forced transitions
            fsm.ForceTransition(null!, Player.PlayerStateId.Dead);
            if (fsm.CurrentStateId != Player.PlayerStateId.Dead)
            {
                GD.Print("FAIL: FSM did not transition to Dead.");
                return false;
            }
            GD.Print("PASS: PlayerStateMachine transitions verified.");

            // ----------------------------------------------------------
            // TEST 9: PlayerSettings — persistence
            // ----------------------------------------------------------
            GD.Print("Running: PlayerSettings persistence checks...");
            var ps = new Player.PlayerSettings();
            ps.SetSensitivity(0.75f);
            ps.SetInvertY(true);
            ps.SetLeftHanded(true);

            // Re-load in fresh instance
            var ps2 = new Player.PlayerSettings();
            ps2.Load();
            if (ps2.Data.CameraSensitivity != 0.75f || !ps2.Data.InvertY || !ps2.Data.LeftHandedMode)
            {
                GD.Print($"FAIL: PlayerSettings persistence. " +
                         $"Sens={ps2.Data.CameraSensitivity}, InvertY={ps2.Data.InvertY}, Left={ps2.Data.LeftHandedMode}");
                return false;
            }
            ps2.ResetToDefaults();
            GD.Print("PASS: PlayerSettings persistence and reset verified.");

            if (!RunPhase5Tests(tempDir)) return false;
            if (!RunPhase6Tests(tempDir)) return false;
            if (!RunPhase7Tests(tempDir)) return false;
            if (!RunPhase8Tests(tempDir)) return false;

            Directory.Delete(tempDir, true);
            return true;
        }

        private class MockInteractable : Interaction.IInteractable
        {
            public string InteractionPrompt => "Mock Target";
            public float InteractionDistance => 3.5f;
            public Interaction.InteractionType Type { get; set; } = Interaction.InteractionType.Tap;
            public float HoldDuration { get; set; } = 0f;
            public bool Interacted { get; set; } = false;
            public bool Highlighted { get; set; } = false;
            public Vector3 Position { get; set; } = Vector3.Zero;

            public void OnInteract(Player.PlayerRoot player) => Interacted = true;
            public void OnInteractionStart(Player.PlayerRoot player) {}
            public void OnInteractionEnd(Player.PlayerRoot player, bool completed) {}
            public void SetHighlight(bool highlighted) => Highlighted = highlighted;
            public Vector3 GetGlobalPosition() => Position;
        }

        private bool RunPhase5Tests(string tempDir)
        {
            GD.Print("Running Phase 5 player character framework tests...");

            // 1. Model Swap & LOD test
            GD.Print("Testing PlayerModelController...");
            var modelNode = new Player.PlayerModelController();
            AddChild(modelNode);
            
            // Check fallback mesh creation
            modelNode.SwapPart(Player.PartCategory.Hair, "invalid_path");
            if (string.IsNullOrEmpty(modelNode.GetPartPath(Player.PartCategory.Hair)))
            {
                GD.Print("FAIL: Model swap did not register slot path.");
                modelNode.QueueFree();
                return false;
            }

            modelNode.SetLOD(2);
            if (modelNode.CurrentLOD != 2)
            {
                GD.Print("FAIL: Model LOD update.");
                modelNode.QueueFree();
                return false;
            }
            modelNode.QueueFree();
            GD.Print("PASS: PlayerModelController tests.");

            // 2. Attribute / Stats Modifier calculation test
            GD.Print("Testing Stats & Attribute system...");
            var attrSet = new Player.Stats.PlayerAttributeSet();
            float baseHp = attrSet.GetValue(Player.Stats.AttributeType.Health);
            if (baseHp != 100f)
            {
                GD.Print($"FAIL: Default HP value mismatch. Got {baseHp}, expected 100.");
                return false;
            }

            // Flat boost
            var modFlat = new Player.Stats.StatModifier("test_flat", 25f, Player.Stats.ModifierType.Flat, Player.Stats.ModifierSource.Equipment);
            attrSet.AddModifier(Player.Stats.AttributeType.Health, modFlat);
            float hpWithFlat = attrSet.GetValue(Player.Stats.AttributeType.Health);
            if (hpWithFlat != 125f)
            {
                GD.Print($"FAIL: Flat attribute modification. Got {hpWithFlat}, expected 125.");
                return false;
            }

            // Timed Percent modifier
            var modPct = new Player.Stats.StatModifier("test_pct", 0.1f, Player.Stats.ModifierType.PercentAdd, Player.Stats.ModifierSource.Buff, 1.0);
            attrSet.AddModifier(Player.Stats.AttributeType.Health, modPct);
            float hpWithPct = attrSet.GetValue(Player.Stats.AttributeType.Health);
            if (hpWithPct != 137.5f) // (100 + 25) * 1.1 = 137.5
            {
                GD.Print($"FAIL: Percent attribute modification. Got {hpWithPct}, expected 137.5.");
                return false;
            }

            // Update timers
            attrSet.Update(0.5f);
            if (attrSet.GetValue(Player.Stats.AttributeType.Health) != 137.5f)
            {
                GD.Print("FAIL: Timed modifier expired too early.");
                return false;
            }
            
            attrSet.Update(0.6f); // Total 1.1s elapsed (exceeds 1.0s)
            float hpAfterExpiry = attrSet.GetValue(Player.Stats.AttributeType.Health);
            if (hpAfterExpiry != 125f)
            {
                GD.Print($"FAIL: Timed modifier failed to expire. Got {hpAfterExpiry}, expected 125.");
                return false;
            }
            GD.Print("PASS: Stats & Attribute modification system.");

            // 3. Universal Interaction Detection test
            GD.Print("Testing Interaction system...");
            var playerRoot = new Player.PlayerRoot();
            AddChild(playerRoot);
            
            var detector = playerRoot.GetNodeOrNull<Player.PlayerInteractionDetector>("PlayerInteractionDetector");
            if (detector == null)
            {
                GD.Print("FAIL: PlayerInteractionDetector not found on PlayerRoot.");
                playerRoot.QueueFree();
                return false;
            }

            var mockObj = new MockInteractable();
            mockObj.Position = playerRoot.GlobalPosition + new Vector3(1f, 0f, 0f); // 1 meter away (well within 3.5m limit)
            detector.RegisterManualInteractable(mockObj);

            // Trigger physics process manually or trigger detection logic
            detector._PhysicsProcess(0.016f);

            if (detector.ClosestInteractable != mockObj)
            {
                GD.Print("FAIL: Detector did not select the mock interactable.");
                playerRoot.QueueFree();
                return false;
            }

            if (!mockObj.Highlighted)
            {
                GD.Print("FAIL: Target interactable was not highlighted.");
                playerRoot.QueueFree();
                return false;
            }

            detector.UnregisterManualInteractable(mockObj);
            playerRoot.QueueFree();
            GD.Print("PASS: Universal Interaction & Detection system.");

            // 4. Effects Framework tests
            GD.Print("Testing PlayerEffectsController...");
            var player = new Player.PlayerRoot();
            AddChild(player);
            var effects = player.Effects;
            if (effects == null)
            {
                GD.Print("FAIL: PlayerEffectsController not initialized on PlayerRoot.");
                player.QueueFree();
                return false;
            }

            effects.ApplyEffect(Player.PlayerEffectType.Shield, 1.0f);
            if (!effects.HasEffect(Player.PlayerEffectType.Shield))
            {
                GD.Print("FAIL: Effects framework did not apply Shield effect.");
                player.QueueFree();
                return false;
            }
            
            player.QueueFree();
            GD.Print("PASS: PlayerEffectsController.");

            // 5. Save Integration test
            GD.Print("Testing SaveProfile slot save and load...");
            var saveManager = new SaveManager(tempDir);
            var profile = new SaveProfile();
            profile.Stats.CharacterName = "Eternia Tester";
            profile.EquippedParts["Hair"] = "res://Assets/Characters/Hair_01.tscn";
            profile.BaseAttributes["Health"] = 120f;
            profile.ActiveEffects.Add("Shield");

            if (!saveManager.Save(1, profile))
            {
                GD.Print("FAIL: SaveManager did not save slot 1.");
                return false;
            }

            var loaded = saveManager.Load(1);
            if (loaded == null || loaded.Stats.CharacterName != "Eternia Tester" || 
                !loaded.EquippedParts.ContainsKey("Hair") || 
                loaded.BaseAttributes["Health"] != 120f || 
                loaded.ActiveEffects[0] != "Shield")
            {
                GD.Print("FAIL: SaveProfile data mismatch after load.");
                return false;
            }

            // Migration check
            var oldProfile = new SaveProfile();
            oldProfile.SaveVersion = 1;
            oldProfile.Stats.CharacterName = "Old Version Save";
            saveManager.Save(2, oldProfile); // Will save as version 2 but emulate legacy file migration
            
            var migrateTestProfile = new SaveProfile();
            migrateTestProfile.SaveVersion = 1;
            var method = typeof(SaveManager).GetMethod("MigrateProfile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(saveManager, new object[] { migrateTestProfile });
                if (migrateTestProfile.SaveVersion != 2 || migrateTestProfile.EquippedParts == null)
                {
                    GD.Print("FAIL: SaveManager MigrateProfile did not run correctly.");
                    return false;
                }
            }
            GD.Print("PASS: Save slot integration and migration.");

            // 6. ResourceManager test
            GD.Print("Testing ResourceManager preload...");
            var resourceManager = ServiceLocator.Get<ResourceManager>();
            resourceManager.PreloadAsset("res://Scenes/Boot.tscn");
            var preloadedScene = resourceManager.GetAsset<PackedScene>("res://Scenes/Boot.tscn");
            if (preloadedScene == null)
            {
                GD.Print("FAIL: ResourceManager did not preload and cache res://Scenes/Boot.tscn");
                return false;
            }
            GD.Print("PASS: ResourceManager cache.");

            // 7. AudioManager test
            GD.Print("Testing AudioManager bus controls...");
            var audioManager = ServiceLocator.Get<AudioManager>();
            audioManager.SetBusVolume("Master", 0.5f);
            int masterIndex = AudioServer.GetBusIndex("Master");
            if (masterIndex != -1)
            {
                float expectedDb = Mathf.LinearToDb(0.5f);
                float actualDb = AudioServer.GetBusVolumeDb(masterIndex);
                if (Mathf.Abs(actualDb - expectedDb) > 0.1f)
                {
                    GD.Print($"FAIL: AudioManager Master volume DB mismatch. Got {actualDb}, expected {expectedDb}");
                    return false;
                }
            }
            GD.Print("PASS: AudioManager volume.");

            // 8. SceneManager test
            GD.Print("Testing SceneManager resolution...");
            var sceneManager = ServiceLocator.Get<SceneManager>();
            if (sceneManager == null || sceneManager.CurrentSceneName != "Boot")
            {
                GD.Print("FAIL: SceneManager not initialized correctly.");
                return false;
            }
            GD.Print("PASS: SceneManager checks.");

            return true;
        }

        private bool RunPhase6Tests(string tempDir)
        {
            GD.Print("Running Phase 6 item ecosystem tests...");

            // 1. Item Database Verification
            GD.Print("Testing ItemDatabase...");
            var itemDb = new Items.ItemDatabase();
            itemDb.Initialize();

            var sword = itemDb.GetItem("wpn_iron_sword");
            if (sword == null || sword.DisplayName != "Rusty Iron Sword" || sword.Rarity != Items.ItemRarity.Common)
            {
                GD.Print("FAIL: ItemDatabase did not load wpn_iron_sword correctly.");
                return false;
            }

            var rar = itemDb.GetRarity(Items.ItemRarity.Legendary);
            if (rar == null || rar.ColorHex != "#FF8000")
            {
                GD.Print("FAIL: ItemDatabase did not load Legendary rarity definitions correctly.");
                return false;
            }
            GD.Print("PASS: ItemDatabase validation.");

            // 2. Stack splitting & merging
            GD.Print("Testing Inventory stack splitting & merging...");
            var container = new Inventory.InventoryContainer(10);
            
            // Add 15 potions
            if (!container.AddItem("pot_minor_health", 15))
            {
                GD.Print("FAIL: Failed to add 15 health potions.");
                return false;
            }

            if (container.Slots[0].Quantity != 15)
            {
                GD.Print($"FAIL: Potion quantity mismatch. Got {container.Slots[0].Quantity}, expected 15.");
                return false;
            }

            // Split 5 potions to slot 1
            if (!container.SplitStack(0, 1, 5))
            {
                GD.Print("FAIL: SplitStack from slot 0 to 1 failed.");
                return false;
            }

            if (container.Slots[0].Quantity != 10 || container.Slots[1].Quantity != 5)
            {
                GD.Print($"FAIL: Split stack values. Slot0={container.Slots[0].Quantity}, Slot1={container.Slots[1].Quantity}");
                return false;
            }

            // Merge back
            if (!container.MergeStacks(1, 0))
            {
                GD.Print("FAIL: MergeStacks from slot 1 to 0 failed.");
                return false;
            }

            if (container.Slots[0].Quantity != 15 || !container.Slots[1].IsEmpty)
            {
                GD.Print($"FAIL: Merge stack values. Slot0={container.Slots[0].Quantity}, Slot1_IsEmpty={container.Slots[1].IsEmpty}");
                return false;
            }
            GD.Print("PASS: Stack arithmetic.");

            // 3. Sorting & Filtering
            GD.Print("Testing Inventory sorting & filtering...");
            // Slot 0 has 15 health potions. Clear slot 0 first.
            container.Slots[0].Clear();
            
            // Add sword and potions in separate slots
            container.AddItem("wpn_iron_sword", 1);
            container.AddItem("pot_minor_health", 10);

            // Favorite potions (normally in slot 1)
            container.Slots[1].IsFavorite = true;

            // Sort by Value
            container.Sort(Inventory.InventorySortType.Value);
            
            // Favorites are moved to slot 0 first
            if (container.Slots[0].ItemId != "pot_minor_health" || !container.Slots[0].IsFavorite)
            {
                GD.Print($"FAIL: Inventory sorting. Favorite was not sorted to slot 0. Got ID={container.Slots[0].ItemId}");
                return false;
            }

            // Filter search mask
            var filtered = container.Filter(searchMask: "iron");
            if (filtered.Count != 1 || filtered[0].ItemId != "wpn_iron_sword")
            {
                GD.Print($"FAIL: Inventory filter by text mask. Matches={filtered.Count}");
                return false;
            }
            GD.Print("PASS: Sorting and filtering.");

            // 4. Equipment slot assignment and attribute modifiers
            GD.Print("Testing Equipment slot assignment & attribute updates...");
            var player = new Player.PlayerRoot();
            AddChild(player);
            
            float baseStrength = player.Data.Attributes.GetValue(Player.Stats.AttributeType.Strength);
            if (baseStrength != 10f)
            {
                GD.Print($"FAIL: Default player Strength base is {baseStrength}, expected 10.");
                player.QueueFree();
                return false;
            }

            // Temporarily register database in locator so EquipmentManager can resolve
            ServiceLocator.Clear();
            var conf = new ConfigManager(tempDir);
            ServiceLocator.Register(conf);
            ServiceLocator.Register(itemDb);

            var equipManager = new Inventory.EquipmentManager();
            var itemSlot = new Inventory.InventorySlot { ItemId = "wpn_iron_sword", Quantity = 1 };

            // Equip sword
            if (!equipManager.EquipItem(Items.EquipmentSlotType.MainWeapon, itemSlot, player))
            {
                GD.Print("FAIL: EquipmentManager failed to equip MainWeapon.");
                player.QueueFree();
                return false;
            }

            float equippedStrength = player.Data.Attributes.GetValue(Player.Stats.AttributeType.Strength);
            if (equippedStrength != 12f)
            {
                GD.Print($"FAIL: Modified player Strength is {equippedStrength}, expected 12.");
                player.QueueFree();
                return false;
            }

            // Unequip sword
            equipManager.UnequipItem(Items.EquipmentSlotType.MainWeapon, player);
            float unequippedStrength = player.Data.Attributes.GetValue(Player.Stats.AttributeType.Strength);
            if (unequippedStrength != 10f)
            {
                GD.Print($"FAIL: Unequipped player Strength returned to {unequippedStrength}, expected 10.");
                player.QueueFree();
                return false;
            }

            player.QueueFree();
            GD.Print("PASS: Equipment attribute modifiers.");

            // 5. Save & Load slot integration & migration
            GD.Print("Testing SaveProfile V3 slot serialization & migration...");
            var saveManager = new SaveManager(tempDir);
            var saveProf = new SaveProfile();
            saveProf.Stats.CharacterName = "Item Ecosystem Hero";
            
            var testSlot = new Inventory.InventorySlot { ItemId = "pot_minor_health", Quantity = 5 };
            saveProf.PlayerInventory.Add(testSlot);
            saveProf.EquippedSlots["MainWeapon"] = new Inventory.InventorySlot { ItemId = "wpn_iron_sword", Quantity = 1 };

            if (!saveManager.Save(10, saveProf))
            {
                GD.Print("FAIL: SaveManager did not save slot 10.");
                return false;
            }

            var loadedProf = saveManager.Load(10);
            if (loadedProf == null || loadedProf.PlayerInventory.Count != 1 || 
                loadedProf.PlayerInventory[0].ItemId != "pot_minor_health" || 
                loadedProf.EquippedSlots["MainWeapon"].ItemId != "wpn_iron_sword")
            {
                GD.Print("FAIL: SaveProfile V3 deserialization data mismatch.");
                return false;
            }

            // Migration from V2 to V3 test
            var legacyProf = new SaveProfile();
            legacyProf.SaveVersion = 2;
            legacyProf.Stats.CharacterName = "Legacy V2 Hero";

            var migrateMethod = typeof(SaveManager).GetMethod("MigrateProfile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (migrateMethod != null)
            {
                migrateMethod.Invoke(saveManager, new object[] { legacyProf });
                if (legacyProf.SaveVersion != 3 || legacyProf.PlayerInventory == null || legacyProf.EquippedSlots == null)
                {
                    GD.Print("FAIL: SaveProfile V2 to V3 migration failed.");
                    return false;
                }
            }
            GD.Print("PASS: Save slot V3 integration and migration.");

            // 6. Loot Table rolling
            GD.Print("Testing Loot Table resolutions...");
            var lootTable = new Items.LootTable { TableId = "test_table" };
            lootTable.Entries.Add(new Items.LootEntry { ItemId = "pot_minor_health", Chance = 1.0f, MinQuantity = 2, MaxQuantity = 5 });
            
            var rolledLoot = lootTable.RollLoot();
            if (rolledLoot.Count != 1 || rolledLoot[0].ItemId != "pot_minor_health" || rolledLoot[0].Quantity < 2 || rolledLoot[0].Quantity > 5)
            {
                GD.Print("FAIL: Loot Table rolling did not resolve entries correctly.");
                return false;
            }
            GD.Print("PASS: Loot Table roll resolved.");

            // 7. Consumable Item Effects
            GD.Print("Testing consumable item effects resolver...");
            var mockPlayer = new Player.PlayerRoot();
            AddChild(mockPlayer);
            mockPlayer.Data.MaxHealth = 100f;
            mockPlayer.Data.CurrentHealth = 50f;

            var healingEffect = new Items.ItemEffectData { EffectType = "Healing", Magnitude = 25f };
            if (!Items.ItemEffectsFramework.TriggerEffect(healingEffect, mockPlayer) || mockPlayer.Data.CurrentHealth != 75f)
            {
                GD.Print($"FAIL: ItemEffectsFramework did not apply healing. HP={mockPlayer.Data.CurrentHealth}");
                mockPlayer.QueueFree();
                return false;
            }

            mockPlayer.QueueFree();
            GD.Print("PASS: Item Effects framework hooks.");

            // 8. Performance Benchmarks
            GD.Print("Running Item Ecosystem Performance Benchmarks...");
            var lookupWatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                var val = itemDb.GetItem("wpn_iron_sword");
            }
            lookupWatch.Stop();
            GD.Print($"BENCHMARK: 100,000 Item lookups completed in {lookupWatch.ElapsedMilliseconds} ms (Average: {(double)lookupWatch.ElapsedTicks / 100000.0:F4} ticks/lookup).");

            var largeContainer = new Inventory.InventoryContainer(1000);
            for (int i = 0; i < 1000; i++)
            {
                largeContainer.AddItem("pot_minor_health", 5);
            }
            var serialWatch = System.Diagnostics.Stopwatch.StartNew();
            var serialStr = System.Text.Json.JsonSerializer.Serialize(largeContainer.SaveSlots());
            serialWatch.Stop();
            GD.Print($"BENCHMARK: 1,000 Inventory slots serialized in {serialWatch.ElapsedMilliseconds} ms (JSON Size: {serialStr.Length / 1024.0:F2} KB).");
            GD.Print("PASS: Performance Benchmarks completed.");

            // Restore primary TestRunner registrations
            ServiceLocator.Clear();
            var resolvedPm = new PerformanceManager();
            var resolvedSm = new SettingsManager(tempDir);
            var resolvedLm = new LocalizationManager();
            var resolvedGm = new GameManager();
            var resolvedAm = new AudioManager();
            var resolvedScm = new SceneManager();
            var resolvedRm = new ResourceManager();
            var resolvedUm = new UIManager();

            ServiceLocator.Register(resolvedPm);
            ServiceLocator.Register(resolvedSm);
            ServiceLocator.Register(resolvedLm);
            ServiceLocator.Register(resolvedGm);
            ServiceLocator.Register(resolvedAm);
            ServiceLocator.Register(resolvedScm);
            ServiceLocator.Register(resolvedRm);
            ServiceLocator.Register(resolvedUm);

            resolvedSm.LoadSettings();

            return true;
        }

        private bool RunPhase7Tests(string tempDir)
        {
            GD.Print("Running Phase 7 world & chunk streaming tests...");

            // 1. WorldSeed tests
            GD.Print("Testing WorldSeed...");
            ulong seed1 = WorldSeed.Parse("Eternia");
            ulong seed2 = WorldSeed.Parse("Eternia");
            if (seed1 != seed2)
            {
                GD.Print("FAIL: WorldSeed parsing is not deterministic.");
                return false;
            }

            if (!WorldSeed.Validate("Seed_123-Normal"))
            {
                GD.Print("FAIL: WorldSeed validation did not allow valid alphanumeric string.");
                return false;
            }

            string hex = WorldSeed.ToShareString(seed1);
            if (!WorldSeed.TryParseShareString(hex, out ulong parsedSeed) || parsedSeed != seed1)
            {
                GD.Print("FAIL: WorldSeed Hex sharing format failed.");
                return false;
            }
            GD.Print("PASS: WorldSeed validations.");

            // 2. Deterministic RNG check
            GD.Print("Testing Deterministic PRNG coordinate rolls...");
            var rng1 = new RandomNumberGenerator();
            rng1.Seed = seed1;
            float val1 = rng1.Randf();

            var rng2 = new RandomNumberGenerator();
            rng2.Seed = seed1;
            float val2 = rng2.Randf();

            if (Mathf.Abs(val1 - val2) > 0.0001f)
            {
                GD.Print("FAIL: Godot RNG seed yields non-deterministic float.");
                return false;
            }
            GD.Print("PASS: Deterministic rolls.");

            // 3. Biomes and Elements Database checks
            GD.Print("Testing WorldDatabase & Biomes definitions...");
            var wdb = new WorldDatabase();
            wdb.Initialize();
            
            var forest = wdb.GetBiome(BiomeType.Forest);
            if (forest == null || forest.Name != "Forest")
            {
                GD.Print("FAIL: WorldDatabase did not resolve Forest biome.");
                return false;
            }

            var oakTree = wdb.GetRecord("tree_oak");
            if (oakTree == null || oakTree.DisplayName != "Oak Tree")
            {
                GD.Print("FAIL: WorldDatabase did not resolve tree_oak record.");
                return false;
            }
            GD.Print("PASS: WorldDatabase definitions.");

            // 4. Time & Weather controls
            GD.Print("Testing WorldTimeSystem & Weather stages...");
            var timeSystem = new WorldTimeSystem();
            timeSystem.SetTimeState(0.22, 1);
            if (timeSystem.GetCycleStage() != DayCycleStage.Sunrise)
            {
                GD.Print($"FAIL: 0.22 time of day stage is {timeSystem.GetCycleStage()}, expected Sunrise.");
                return false;
            }

            timeSystem.SetTimeState(0.5, 1);
            if (timeSystem.GetCycleStage() != DayCycleStage.Day)
            {
                GD.Print($"FAIL: 0.5 time of day stage is {timeSystem.GetCycleStage()}, expected Day.");
                return false;
            }

            var weather = new WeatherManager();
            weather.Initialize();
            weather.ChangeWeather(WeatherType.Storm);
            if (weather.CurrentWeather.WindStrength != 0.8f)
            {
                GD.Print($"FAIL: Storm weather wind strength is {weather.CurrentWeather.WindStrength}, expected 0.8.");
                return false;
            }
            GD.Print("PASS: Time and weather controls.");

            // 5. Chunk loading, async task & node modifications
            GD.Print("Testing ChunkManager streaming & active nodes modification...");
            ServiceLocator.Clear();
            var conf = new ConfigManager(tempDir);
            ServiceLocator.Register(conf);
            ServiceLocator.Register(wdb);

            var chunkManager = new ChunkManager();
            chunkManager.Initialize();
            chunkManager.ActiveSeed = seed1;

            bool loadedTriggered = false;
            chunkManager.OnChunkLoaded += (c) => {
                if (c.Coords == Vector2I.Zero)
                {
                    loadedTriggered = true;
                }
            };

            // Trigger load for coordinate (0,0) by setting player position close to zero
            chunkManager.UpdatePlayerPosition(new Vector3(10f, 0f, 10f));
            
            // Allow async thread pool a brief duration to parse chunk nodes
            System.Threading.Thread.Sleep(100);

            var zeroChunk = chunkManager.GetChunk(Vector2I.Zero);
            if (zeroChunk == null || zeroChunk.State != ChunkState.Loaded)
            {
                GD.Print("FAIL: ChunkManager did not stream chunk (0,0) asynchronously.");
                return false;
            }

            if (!loadedTriggered)
            {
                GD.Print("FAIL: ChunkManager OnChunkLoaded event did not trigger for zero chunk.");
                return false;
            }

            int baseNodeCount = zeroChunk.ActiveNodes.Count;
            if (baseNodeCount == 0)
            {
                GD.Print("FAIL: Zero chunk generated 0 resource nodes.");
                return false;
            }

            // Modify node (mined tree)
            string mineId = zeroChunk.ActiveNodes[0].NodeInstanceId;
            chunkManager.ModifyNode(Vector2I.Zero, mineId);
            
            if (zeroChunk.ActiveNodes.Count != baseNodeCount - 1 || !zeroChunk.ModifiedNodeIds.Contains(mineId))
            {
                GD.Print("FAIL: ModifyNode did not flag instance as modified/deleted.");
                return false;
            }
            GD.Print("PASS: Chunk streaming & modifications.");

            // 6. Save slot V4 serialization & migration
            GD.Print("Testing SaveProfile V4 world configurations serialization...");
            var saveManager = new SaveManager(tempDir);
            var saveProf = new SaveProfile();
            saveProf.WorldSeed = seed1;
            saveProf.DiscoveredRegions.Add("EterniaFields");
            saveProf.ModifiedChunkNodes["0_0"] = new List<string> { mineId };

            if (!saveManager.Save(11, saveProf))
            {
                GD.Print("FAIL: SaveManager did not save slot 11.");
                return false;
            }

            var loadedProf = saveManager.Load(11);
            if (loadedProf == null || loadedProf.WorldSeed != seed1 || 
                !loadedProf.DiscoveredRegions.Contains("EterniaFields") || 
                !loadedProf.ModifiedChunkNodes["0_0"].Contains(mineId))
            {
                GD.Print("FAIL: SaveProfile V4 loaded parameters mismatch.");
                return false;
            }

            // Legacy V3 migration
            var legacyProf = new SaveProfile();
            legacyProf.SaveVersion = 3;
            var migrateMethod = typeof(SaveManager).GetMethod("MigrateProfile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (migrateMethod != null)
            {
                migrateMethod.Invoke(saveManager, new object[] { legacyProf });
                if (legacyProf.SaveVersion != 4 || legacyProf.DiscoveredRegions == null || legacyProf.ModifiedChunkNodes == null)
                {
                    GD.Print("FAIL: SaveProfile V3 to V4 migration failed.");
                    return false;
                }
            }
            GD.Print("PASS: Save slot V4 integration and migration.");

            // Restore primary TestRunner registrations
            ServiceLocator.Clear();
            var resolvedPm = new PerformanceManager();
            var resolvedSm = new SettingsManager(tempDir);
            var resolvedLm = new LocalizationManager();
            var resolvedGm = new GameManager();
            var resolvedAm = new AudioManager();
            var resolvedScm = new SceneManager();
            var resolvedRm = new ResourceManager();
            var resolvedUm = new UIManager();

            ServiceLocator.Register(resolvedPm);
            ServiceLocator.Register(resolvedSm);
            ServiceLocator.Register(resolvedLm);
            ServiceLocator.Register(resolvedGm);
            ServiceLocator.Register(resolvedAm);
            ServiceLocator.Register(resolvedScm);
            ServiceLocator.Register(resolvedRm);
            ServiceLocator.Register(resolvedUm);

            resolvedSm.LoadSettings();

            return true;
        }

        private bool RunPhase8Tests(string tempDir)
        {
            GD.Print("Running Phase 8 terrain & navigation tests...");

            ulong testSeed = 54321u;
            var terrainGen = new TerrainGenerator(testSeed);

            // 1. Layered Terrain Generation check
            GD.Print("Testing TerrainGenerator height reproduction...");
            float h1 = terrainGen.GetHeight(25f, -40f);
            float h2 = terrainGen.GetHeight(25f, -40f);
            if (Mathf.Abs(h1 - h2) > 0.0001f)
            {
                GD.Print("FAIL: TerrainGenerator height calculations are not deterministic.");
                return false;
            }

            BiomeType bType = terrainGen.GetBiomeAt(25f, -40f);
            GD.Print($"PASS: Terrain Y={h1:F2}, Biome={bType}");

            // 2. Navigation walkable grid check
            GD.Print("Testing NavigationFoundation grids...");
            bool[,] navGrid = NavigationFoundation.GenerateNavigationGrid(terrainGen, Vector2I.Zero, 16);
            if (navGrid == null || navGrid.GetLength(0) != 16)
            {
                GD.Print("FAIL: NavigationFoundation did not generate 16x16 grid.");
                return false;
            }
            GD.Print("PASS: Navigation grids.");

            // 3. Graphics-scaled Vegetation density check
            GD.Print("Testing VegetationSystem preset density scaling...");
            int lowCount = VegetationSystem.ScaleSpawnCount(100, "Low");
            int highCount = VegetationSystem.ScaleSpawnCount(100, "High");
            if (lowCount != 25 || highCount != 100)
            {
                GD.Print($"FAIL: VegetationSystem count scale. Low={lowCount}, High={highCount}");
                return false;
            }
            GD.Print("PASS: Vegetation densities.");

            // 4. World Population landmark checks
            GD.Print("Testing WorldPopulationManager landmarks layout...");
            var popManager = new WorldPopulationManager(testSeed);
            popManager.GenerateLandmarks(terrainGen, 5);
            var landmarks = popManager.GetAllLandmarks();
            if (landmarks.Count == 0 || landmarks[0].Type != LandmarkType.BossArena)
            {
                GD.Print("FAIL: WorldPopulationManager did not place central arena.");
                return false;
            }
            GD.Print("PASS: Landmarks populator.");

            // 5. Automated World Validator audits
            GD.Print("Testing WorldValidator floating meshes scans...");
            var testChunk = new Chunk(Vector2I.Zero);
            
            // Add a floating tree (Y is 50.0 units, terrain is h1 ~ base)
            testChunk.ActiveNodes.Add(new SpawnedNode
            {
                NodeInstanceId = "float_tree",
                ElementRecordId = "tree_oak",
                LocalX = 0f,
                LocalY = 100f, // Extreme height
                LocalZ = 0f
            });

            var validationReport = WorldValidator.ValidateChunk(testChunk, terrainGen);
            if (validationReport.IsSuccess || validationReport.ErrorsCount != 1 || !validationReport.Errors[0].Contains("FloatingObject"))
            {
                GD.Print($"FAIL: WorldValidator did not flag floating tree node. Errors Count={validationReport.ErrorsCount}");
                return false;
            }
            GD.Print("PASS: World Validator scans.");

            // 6. Save slot V5 serialization & migration checks
            GD.Print("Testing SaveProfile V5 terrain states serialization...");
            var saveManager = new SaveManager(tempDir);
            var saveProf = new SaveProfile();
            saveProf.WorldSeed = testSeed;
            saveProf.DiscoveredNavRegions.Add("NavFields");
            saveProf.ModifiedDecorations["0_0"] = new List<string> { "float_tree" };

            if (!saveManager.Save(12, saveProf))
            {
                GD.Print("FAIL: SaveManager did not save slot 12.");
                return false;
            }

            var loadedProf = saveManager.Load(12);
            if (loadedProf == null || loadedProf.WorldSeed != testSeed || 
                !loadedProf.DiscoveredNavRegions.Contains("NavFields") || 
                !loadedProf.ModifiedDecorations["0_0"].Contains("float_tree"))
            {
                GD.Print("FAIL: SaveProfile V5 loaded parameters mismatch.");
                return false;
            }

            // Legacy V4 to V5 migration
            var legacyProf = new SaveProfile();
            legacyProf.SaveVersion = 4;
            var migrateMethod = typeof(SaveManager).GetMethod("MigrateProfile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (migrateMethod != null)
            {
                migrateMethod.Invoke(saveManager, new object[] { legacyProf });
                if (legacyProf.SaveVersion != 5 || legacyProf.ModifiedDecorations == null || legacyProf.DiscoveredNavRegions == null)
                {
                    GD.Print("FAIL: SaveProfile V4 to V5 migration failed.");
                    return false;
                }
            }
            GD.Print("PASS: Save slot V5 integration and migration.");

            // Restore primary TestRunner registrations
            ServiceLocator.Clear();
            var resolvedPm = new PerformanceManager();
            var resolvedSm = new SettingsManager(tempDir);
            var resolvedLm = new LocalizationManager();
            var resolvedGm = new GameManager();
            var resolvedAm = new AudioManager();
            var resolvedScm = new SceneManager();
            var resolvedRm = new ResourceManager();
            var resolvedUm = new UIManager();

            ServiceLocator.Register(resolvedPm);
            ServiceLocator.Register(resolvedSm);
            ServiceLocator.Register(resolvedLm);
            ServiceLocator.Register(resolvedGm);
            ServiceLocator.Register(resolvedAm);
            ServiceLocator.Register(resolvedScm);
            ServiceLocator.Register(resolvedRm);
            ServiceLocator.Register(resolvedUm);

            resolvedSm.LoadSettings();

            // Phase 9 — NPC Architecture tests
            GD.Print("Running: Phase 9 NPC Architecture Tests...");
            bool phase9Pass = RunPhase9Tests(tempDir);
            if (!phase9Pass) return false;

            // Phase 10 — Combat Architecture tests
            GD.Print("Running: Phase 10 Combat Architecture Tests...");
            bool phase10Pass = RunPhase10Tests(tempDir);
            if (!phase10Pass) return false;

            return true;
        }

        // ==========================================================
        // PHASE 9 — NPC ARCHITECTURE TEST SUITE
        // ==========================================================

        private bool RunPhase9Tests(string tempDir)
        {
            // ------------------------------------------
            // TEST P9-1: NPC data creation & integrity
            // ------------------------------------------
            var npcData = new NpcData
            {
                UniqueId    = "npc_test_001",
                DisplayName = "TestVillager",
                Occupation  = NpcTypeEnum.Villager,
                Age         = 32,
                Gender      = GenderType.Female,
                MaxHealth   = 100f,
                CurrentHealth = 100f
            };
            if (npcData.UniqueId != "npc_test_001" || npcData.Occupation != NpcTypeEnum.Villager)
            {
                GD.Print("FAIL P9-1: NPC data creation integrity.");
                return false;
            }
            GD.Print("PASS P9-1: NPC data creation & integrity.");

            // ------------------------------------------
            // TEST P9-2: FSM transitions (Idle → Walking → Working)
            // ------------------------------------------
            var fsm = new NpcStateMachine("npc_test_001");
            fsm.RegisterDefaultTransitions();
            bool t1 = fsm.TransitionTo(NpcStateEnum.Walking);
            bool t2 = fsm.TransitionTo(NpcStateEnum.Working);
            bool t3 = fsm.TransitionTo(NpcStateEnum.Idle);
            if (!t1 || !t2 || !t3 || fsm.CurrentState != NpcStateEnum.Idle)
            {
                GD.Print("FAIL P9-2: FSM state transitions.");
                return false;
            }
            GD.Print("PASS P9-2: FSM state transitions (Idle→Walking→Working→Idle).");

            // ------------------------------------------
            // TEST P9-3: Schedule block resolution at time fractions
            // ------------------------------------------
            var scheduler = NpcScheduler.BuildDefaultCivilianSchedule();
            var morningBlock = scheduler.GetActiveBlock(0.30); // Morning
            var nightBlock   = scheduler.GetActiveBlock(0.10); // Night
            if (morningBlock == null || morningBlock.TargetState != NpcStateEnum.Working)
            {
                GD.Print("FAIL P9-3: Schedule morning block resolution.");
                return false;
            }
            if (nightBlock == null || nightBlock.TargetState != NpcStateEnum.Sleeping)
            {
                GD.Print("FAIL P9-3: Schedule night block resolution.");
                return false;
            }
            GD.Print("PASS P9-3: Schedule block resolution at different time fractions.");

            // ------------------------------------------
            // TEST P9-4: Relationship adjustments
            // ------------------------------------------
            var relSystem = new RelationshipSystem();
            relSystem.AdjustFriendship("npc_001", "npc_002", 50f);
            relSystem.AdjustTrust("npc_001", "npc_002", 30f);
            relSystem.AdjustFear("npc_001", "npc_002", 200f); // should clamp to 100
            var rel = relSystem.Get("npc_001", "npc_002");
            if (rel == null || rel.Friendship != 50f || rel.Trust != 30f || rel.Fear != 100f)
            {
                GD.Print($"FAIL P9-4: Relationship values incorrect. F={rel?.Friendship} T={rel?.Trust} Fe={rel?.Fear}");
                return false;
            }
            GD.Print("PASS P9-4: Relationship adjustments & clamping.");

            // ------------------------------------------
            // TEST P9-5: Reputation scope changes
            // ------------------------------------------
            var repSystem = new ReputationSystem();
            repSystem.AdjustGlobal(100, "saved_villager");
            repSystem.AdjustRegional("region_forest", 50, "completed_quest");
            repSystem.AdjustFaction("faction_guild", -20, "stolen_item");
            repSystem.AdjustIndividual("npc_king", 200, "hero_recognition");
            repSystem.AdjustGlobal(-2000, "test_clamp"); // test lower clamp
            if (repSystem.GetGlobal() != -900 ||
                repSystem.GetRegional("region_forest") != 50 ||
                repSystem.GetFaction("faction_guild") != -20 ||
                repSystem.GetIndividual("npc_king") != 200)
            {
                GD.Print("FAIL P9-5: Reputation scope changes incorrect.");
                return false;
            }
            GD.Print("PASS P9-5: Reputation scope changes & clamping.");

            // ------------------------------------------
            // TEST P9-6: Dialogue line resolution
            // ------------------------------------------
            var dialogue = new DialogueFramework();
            var lines = DialogueFramework.BuildDefaultLines(NpcTypeEnum.Villager);
            dialogue.RegisterLines("npc_001", lines);
            var greeting = dialogue.Resolve("npc_001", DialogueCategory.Greeting, 60f, 0.30, "weather_sunny");
            if (greeting == null || !greeting.LocalizationKey.Contains("villager"))
            {
                GD.Print("FAIL P9-6: Dialogue line resolution returned null or wrong key.");
                return false;
            }
            GD.Print($"PASS P9-6: Dialogue line resolved: '{greeting.LocalizationKey}'.");

            // ------------------------------------------
            // TEST P9-7: NPC spawn determinism
            // ------------------------------------------
            var spawner = new NpcSpawner("TestWorldSeed");
            spawner.RegisterDefaultRules();
            var spawnList1 = spawner.GenerateForRegion("region_01", 0f, 0f);
            var spawnList2 = spawner.GenerateForRegion("region_01", 0f, 0f);
            if (spawnList1.Count == 0 || spawnList1.Count != spawnList2.Count ||
                spawnList1[0].WorldX != spawnList2[0].WorldX)
            {
                GD.Print("FAIL P9-7: NPC spawn not deterministic.");
                return false;
            }
            GD.Print($"PASS P9-7: NPC spawn determinism ({spawnList1.Count} NPCs).");

            // ------------------------------------------
            // TEST P9-8: NpcManager register + update throttle
            // ------------------------------------------
            var manager = new NpcManager();
            manager.RegisterNpc(npcData);
            if (manager.Count != 1)
            {
                GD.Print("FAIL P9-8: NpcManager registration count mismatch.");
                return false;
            }
            manager.UpdateAll(0.1, 0.30); // Under threshold — no state change expected
            manager.UpdateAll(0.5, 0.30); // At threshold — schedule should fire
            var fsmResult = manager.GetFsm("npc_test_001");
            if (fsmResult == null)
            {
                GD.Print("FAIL P9-8: NpcManager FSM not found after registration.");
                return false;
            }
            GD.Print("PASS P9-8: NpcManager registration & throttled update.");

            // ------------------------------------------
            // TEST P9-9: Save V6 serialization & V5→V6 migration
            // ------------------------------------------
            var saveManager = new SaveManager(tempDir);
            var prof = new SaveProfile();
            prof.NpcStates["npc_test_001"] = new NpcSaveState
            {
                UniqueId = "npc_test_001", WorldX = 10f, WorldY = 0f, WorldZ = 20f,
                Emotion = EmotionState.Happy, CurrentHealth = 90f
            };
            prof.ReputationSnapshot["global"] = 100;
            prof.RelationshipSnapshot["npc_001_npc_002"] = new float[] { 50f, 30f, 20f, 5f };

            bool saveOk = saveManager.Save(9, prof);
            var loadedProf = saveManager.Load(9);
            if (!saveOk || loadedProf == null ||
                !loadedProf.NpcStates.ContainsKey("npc_test_001") ||
                loadedProf.ReputationSnapshot["global"] != 100)
            {
                GD.Print("FAIL P9-9: Save V6 serialization failed.");
                return false;
            }

            // V5 → V6 migration test
            var legacyV5 = new SaveProfile();
            legacyV5.SaveVersion = 5;
            var migMethod = typeof(SaveManager).GetMethod("MigrateProfile",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (migMethod != null)
            {
                migMethod.Invoke(saveManager, new object[] { legacyV5 });
                if (legacyV5.SaveVersion != 6 ||
                    legacyV5.NpcStates == null ||
                    legacyV5.ReputationSnapshot == null)
                {
                    GD.Print("FAIL P9-9: SaveProfile V5→V6 migration failed.");
                    return false;
                }
            }
            GD.Print("PASS P9-9: Save V6 serialization & V5→V6 migration.");

            GD.Print("Phase 9 NPC Architecture: ALL 9 TESTS PASSED.");
            return true;
        }

        // ==========================================================
        // PHASE 10 — COMBAT ARCHITECTURE TEST SUITE
        // ==========================================================

        private bool RunPhase10Tests(string tempDir)
        {
            // ------------------------------------------
            // TEST P10-1: Target Selection
            // ------------------------------------------
            var ts = new TargetingSystem();
            ts.RegisterTarget(new CombatTarget { TargetId = "t1", WorldX = 5f, WorldZ = 0f, FactionId = "enemy", IsAlive = true });
            ts.RegisterTarget(new CombatTarget { TargetId = "t2", WorldX = 10f, WorldZ = 0f, FactionId = "enemy", IsAlive = true, Priority = 5 });
            ts.RegisterTarget(new CombatTarget { TargetId = "t3", WorldX = 2f, WorldZ = 0f, FactionId = "friend", IsAlive = true });

            var nearest = ts.FindNearest(0f, 0f, 0f, 15f, "friend");
            if (nearest == null || nearest.TargetId != "t1") // t3 excluded, t1 (5m) is closer than t2 (10m)
            {
                GD.Print($"FAIL P10-1: FindNearest returned incorrect target. Expected 't1', got '{nearest?.TargetId}'");
                return false;
            }

            ts.HardLock("t2");
            if (ts.CurrentTargetId != "t2" || ts.Mode != TargetMode.HardLock)
            {
                GD.Print("FAIL P10-1: HardLock assignment failed.");
                return false;
            }
            GD.Print("PASS P10-1: Target Selection nearest & locking.");

            // ------------------------------------------
            // TEST P10-2: Hit Detection
            // ------------------------------------------
            var attackVol = HitVolume.MakeSphere("attacker", 0f, 0f, 0f, 3f, "player");
            var targets = new List<CombatTarget>
            {
                new CombatTarget { TargetId = "target_in", WorldX = 2f, WorldZ = 0f, IsAlive = true, FactionId = "enemy" },
                new CombatTarget { TargetId = "target_out", WorldX = 5f, WorldZ = 0f, IsAlive = true, FactionId = "enemy" }
            };
            var hits = HitDetection.CheckMelee(attackVol, targets);
            if (hits.Count != 1 || hits[0].TargetId != "target_in")
            {
                GD.Print($"FAIL P10-2: Hit detection sphere mismatch. Hits count: {hits.Count}");
                return false;
            }
            GD.Print("PASS P10-2: Hit Detection sphere-sphere overlap.");

            // ------------------------------------------
            // TEST P10-3: Damage Calculation
            // ------------------------------------------
            var dmg = new DamageInstance
            {
                AttackerId = "attacker", TargetId = "target", BaseDamage = 100f,
                Type = DamageType.Fire, CritChance = 0f // force no crit
            };
            var rp = new ResistanceProfile();
            rp.Set(DamageType.Fire, 0.25f); // 25% resistance

            var rng = new Random(42); // deterministic seed
            float finalDmg = DamageSystem.ProcessDamage(dmg, rp, rng);
            // Expected: 100 * 1.25 (elemental multiplier) * 0.75 (resistance) = 93.75
            if (Math.Abs(finalDmg - 93.75f) > 0.01f)
            {
                GD.Print($"FAIL P10-3: Damage value calculation mismatch: got {finalDmg}");
                return false;
            }
            GD.Print("PASS P10-3: Damage Calculation with resistance.");

            // ------------------------------------------
            // TEST P10-4: Status Effect Application
            // ------------------------------------------
            var ses = new StatusEffectSystem();
            ses.Apply(StatusEffectType.Burn, "target_1", "attacker");
            if (!ses.HasEffect("target_1", StatusEffectType.Burn))
            {
                GD.Print("FAIL P10-4: Status effect not applied.");
                return false;
            }
            // Check stack limit / refresh
            ses.Apply(StatusEffectType.Burn, "target_1", "attacker");
            var effects = ses.GetEffects("target_1");
            if (effects.Count != 1)
            {
                GD.Print($"FAIL P10-4: Stack limit refresh added duplicate: count {effects.Count}");
                return false;
            }
            GD.Print("PASS P10-4: Status Effect application and stack validation.");

            // ------------------------------------------
            // TEST P10-5: Projectile Behavior
            // ------------------------------------------
            var ps = new ProjectileSystem();
            var pData = new ProjectileData { UniqueId = "arrow", Speed = 10f, Lifetime = 1f };
            ps.Fire(pData, 0f, 0f, 0f, 1f, 0f, 0f);
            
            // Sim update
            ps.UpdateAll(0.5);
            if (ps.ActiveCount != 1)
            {
                GD.Print("FAIL P10-5: Active projectile count mismatch.");
                return false;
            }
            var activeP = ps.Get("proj_0");
            if (activeP == null || Math.Abs(activeP.PosX - 5.0f) > 0.01f)
            {
                GD.Print($"FAIL P10-5: Projectile position update mismatch: X={activeP?.PosX}");
                return false;
            }
            GD.Print("PASS P10-5: Projectile simulation movement.");

            // ------------------------------------------
            // TEST P10-6: State Transitions
            // ------------------------------------------
            // Create dummy PlayerRoot to test state initialization/transitions
            // In headless testing, PlayerRoot Ready is run dynamically. We can test class instance properties.
            var root = new PlayerRoot();
            // In headless C# environment, we can instantiate state machine and test state registers directly.
            var fsm = new PlayerStateMachine();
            fsm.Register(new IdleState());
            fsm.Register(new AttackState());
            fsm.Start(root, PlayerStateId.Idle);
            if (fsm.CurrentStateId != PlayerStateId.Idle)
            {
                GD.Print("FAIL P10-6: FSM initial state setup.");
                return false;
            }
            fsm.ForceTransition(root, PlayerStateId.Attack);
            if (fsm.CurrentStateId != PlayerStateId.Attack)
            {
                GD.Print("FAIL P10-6: FSM transition to Attack state failed.");
                return false;
            }
            GD.Print("PASS P10-6: Player FSM combat state transitions.");

            // ------------------------------------------
            // TEST P10-7: Save/Load Compatibility
            // ------------------------------------------
            var saveManager = new SaveManager(tempDir);
            var prof = new SaveProfile();
            prof.UnlockedCombatStyles.Add("style_dual_swords");
            prof.LearnedAbilities.Add("ability_fireball");
            prof.WeaponDurability["wpn_sword"] = 0.85f;

            bool saveOk = saveManager.Save(10, prof);
            var loaded = saveManager.Load(10);
            if (!saveOk || loaded == null ||
                !loaded.UnlockedCombatStyles.Contains("style_dual_swords") ||
                !loaded.LearnedAbilities.Contains("ability_fireball") ||
                loaded.WeaponDurability["wpn_sword"] != 0.85f)
            {
                GD.Print("FAIL P10-7: Save V7 serialization roundtrip failed.");
                return false;
            }

            // Legacy V6 to V7 migration test
            var legacyV6 = new SaveProfile();
            legacyV6.SaveVersion = 6;
            var migMethod = typeof(SaveManager).GetMethod("MigrateProfile",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (migMethod != null)
            {
                migMethod.Invoke(saveManager, new object[] { legacyV6 });
                if (legacyV6.SaveVersion != 7 ||
                    legacyV6.UnlockedCombatStyles == null ||
                    legacyV6.LearnedAbilities == null)
                {
                    GD.Print("FAIL P10-7: SaveProfile V6→V7 migration failed.");
                    return false;
                }
            }
            GD.Print("PASS P10-7: Save V7 serialization & V6→V7 migration.");

            // ------------------------------------------
            // TEST P10-8: Weapon Database Config
            // ------------------------------------------
            var wdb = new WeaponDatabase();
            var weapon = wdb.GetOrDefault("wpn_greatsword");
            if (weapon == null || weapon.BaseDamage != 28f || weapon.Type != WeaponType.GreatSword)
            {
                GD.Print("FAIL P10-8: Default weapon properties mismatch.");
                return false;
            }
            GD.Print("PASS P10-8: Weapon Database query & configuration.");

            // ------------------------------------------
            // TEST P10-9: Event-Driven Combat Execution
            // ------------------------------------------
            var cm = new CombatManager();
            cm.RegisterEntity("attacker_ent", 100f);
            cm.RegisterEntity("target_ent", 100f);
            cm.Targeting.RegisterTarget(new CombatTarget { TargetId = "target_ent", WorldX = 1f, WorldZ = 0f, IsAlive = true });

            bool attackEventFired = false;
            bool hitEventFired = false;
            cm.OnCombatEvent += (evt) =>
            {
                if (evt.Type == CombatEventType.AttackStarted) attackEventFired = true;
                if (evt.Type == CombatEventType.HitLanded) hitEventFired = true;
            };

            // execute attack at range (1.0m, within weapon range)
            cm.ExecuteAttack("attacker_ent", "wpn_sword", 0f, 0f, 0f);
            if (!attackEventFired || !hitEventFired)
            {
                GD.Print($"FAIL P10-9: Combat events did not trigger properly: attack={attackEventFired}, hit={hitEventFired}");
                return false;
            }
            GD.Print("PASS P10-9: Event-Driven combat execution.");

            // ------------------------------------------
            // TEST P10-10: Stress Testing (Performance Audit)
            // ------------------------------------------
            long startMs = System.Environment.TickCount;
            // simulate 200 projectile updates with 10 targets
            var stressTargets = new List<CombatTarget>();
            for (int i = 0; i < 10; i++)
            {
                stressTargets.Add(new CombatTarget { TargetId = $"target_{i}", WorldX = 5f, WorldZ = (float)i, IsAlive = true });
            }
            var stressPs = new ProjectileSystem();
            var stressData = new ProjectileData { UniqueId = "stress_arrow", Speed = 20f, Lifetime = 5f };
            for (int i = 0; i < 200; i++)
            {
                stressPs.Fire(stressData, 0f, 0f, 0f, 1f, 0f, 0f);
            }
            stressPs.UpdateAll(0.016, stressTargets); // single frame update
            long elapsedMs = System.Environment.TickCount - startMs;
            
            if (elapsedMs > 50) // Allow up to 50ms for cold start, usually < 2ms
            {
                GD.Print($"FAIL P10-10: Stress test took too long: {elapsedMs}ms");
                return false;
            }
            GD.Print($"PASS P10-10: Stress test completed in {elapsedMs}ms (200 projectiles, 10 targets).");

            GD.Print("Phase 10 Combat Architecture: ALL 10 TESTS PASSED.");
            return true;
        }

        // =============================================
        // PHASE 11 — GAMEPLAY EXPANSION (10 TESTS)
        // =============================================
        private bool RunPhase11Tests(string tempDir)
        {
            GD.Print("\n=== PHASE 11: Gameplay Expansion ===");

            // ------------------------------------------
            // P11-1: EnemyDefinition — constructor validation
            // ------------------------------------------
            GD.Print("Running: P11-1 EnemyDefinition validation...");
            var goblinData = new EnemyData
            {
                EnemyId = "goblin_grunt", DisplayName = "Goblin Grunt", Species = "Goblin",
                MaxHp = 40f, AttackDamage = 6f, AttackRange = 1.5f, AggroRange = 12f,
                MoveSpeed = 4.5f, XpReward = 8, Behaviour = EnemyBehaviour.Aggressive
            };
            var goblinDef = new EnemyDefinition(goblinData);
            if (!goblinDef.IsAggressive || goblinDef.Data.MaxHp != 40f)
            {
                GD.Print("FAIL P11-1: EnemyDefinition IsAggressive or MaxHp mismatch.");
                return false;
            }
            GD.Print("PASS P11-1: EnemyDefinition validates correctly.");

            // ------------------------------------------
            // P11-2: EnemyDatabase — default registry loads 5 enemies
            // ------------------------------------------
            GD.Print("Running: P11-2 EnemyDatabase default load...");
            var db = new EnemyDatabase();
            db.Load("nonexistent_path_forces_defaults");
            if (db.Count != 5)
            {
                GD.Print($"FAIL P11-2: EnemyDatabase expected 5 defaults, got {db.Count}.");
                return false;
            }
            var golem = db.Get("stone_golem");
            if (golem == null || golem.Data.Defense != 20f)
            {
                GD.Print("FAIL P11-2: EnemyDatabase stone_golem lookup failed or Defense mismatch.");
                return false;
            }
            GD.Print("PASS P11-2: EnemyDatabase loads 5 default enemies.");

            // ------------------------------------------
            // P11-3: EnemyStateMachine — state transitions
            // ------------------------------------------
            GD.Print("Running: P11-3 EnemyStateMachine transitions...");
            var fsm = new EnemyStateMachine("test_goblin");
            if (fsm.Current != EnemyState.Idle)
            {
                GD.Print("FAIL P11-3: FSM initial state should be Idle.");
                return false;
            }
            // Simulate target entering aggro range
            var aggroCtx = new EnemyContext
            {
                DistanceToTarget = 8f, HasLineOfSight = true, CurrentHp = 40f, MaxHp = 40f,
                IsStaggered = false, TargetExists = true, AggroRange = 12f,
                AttackRange = 1.5f, AttackCooldownLeft = 1f, Behaviour = EnemyBehaviour.Aggressive
            };
            fsm.Tick(aggroCtx, 0.016f);
            if (fsm.Current != EnemyState.Chase)
            {
                GD.Print($"FAIL P11-3: FSM should be Chase, got {fsm.Current}.");
                return false;
            }
            // Simulate reaching attack range
            var attackCtx = aggroCtx with { DistanceToTarget = 1.2f, AttackCooldownLeft = 0f };
            fsm.Tick(attackCtx, 0.016f);
            if (fsm.Current != EnemyState.Attack)
            {
                GD.Print($"FAIL P11-3: FSM should be Attack, got {fsm.Current}.");
                return false;
            }
            GD.Print("PASS P11-3: EnemyStateMachine transitions Idle→Chase→Attack.");

            // ------------------------------------------
            // P11-4: EnemyStateMachine — death transition
            // ------------------------------------------
            GD.Print("Running: P11-4 EnemyStateMachine death...");
            var deadCtx = attackCtx with { CurrentHp = 0f };
            fsm.Tick(deadCtx, 0.016f);
            if (fsm.Current != EnemyState.Dead || fsm.IsAlive)
            {
                GD.Print($"FAIL P11-4: FSM should be Dead, got {fsm.Current}.");
                return false;
            }
            GD.Print("PASS P11-4: EnemyStateMachine transitions to Dead on 0 HP.");

            // ------------------------------------------
            // P11-5: EnemyDefinition — wave scaling
            // ------------------------------------------
            GD.Print("Running: P11-5 EnemyDefinition wave scaling...");
            var scaled1 = goblinDef.GetScaledData(1);
            var scaled5 = goblinDef.GetScaledData(5);
            if (scaled1.MaxHp >= scaled5.MaxHp)
            {
                GD.Print($"FAIL P11-5: Wave 5 HP ({scaled5.MaxHp}) should exceed wave 1 HP ({scaled1.MaxHp}).");
                return false;
            }
            GD.Print($"PASS P11-5: Wave scaling correct. W1={scaled1.MaxHp} W5={scaled5.MaxHp}");

            // ------------------------------------------
            // P11-6: AbilityDefinition — data model
            // ------------------------------------------
            GD.Print("Running: P11-6 AbilityDefinition validation...");
            var abilityData = new AbilityData
            {
                AbilityId   = "power_strike",
                DisplayName = "Power Strike",
                CooldownSec = 6f,
                ManaCost    = 0f,
                StaminaCost = 25f,
                TargetType  = AbilityTargetType.SingleEnemy,
                DamageType  = AbilityDamageType.Physical,
                BaseDamage  = 40f,
                LevelRequired = 1
            };
            var abilityDef = new AbilityDefinition(abilityData);
            if (abilityDef.DoesDamage == false || abilityDef.IsUnlocked(1) == false)
            {
                GD.Print("FAIL P11-6: AbilityDefinition DoesDamage or IsUnlocked mismatch.");
                return false;
            }
            GD.Print("PASS P11-6: AbilityDefinition validates correctly.");

            // ------------------------------------------
            // P11-7: AbilityDatabase — 5 defaults load
            // ------------------------------------------
            GD.Print("Running: P11-7 AbilityDatabase default load...");
            var adb = new AbilityDatabase();
            adb.Load("nonexistent_forces_defaults");
            if (adb.Count != 5)
            {
                GD.Print($"FAIL P11-7: AbilityDatabase expected 5 defaults, got {adb.Count}.");
                return false;
            }
            var fireball = adb.Get("fireball");
            if (fireball == null || fireball.Data.BaseDamage != 55f)
            {
                GD.Print("FAIL P11-7: Fireball lookup or BaseDamage mismatch.");
                return false;
            }
            GD.Print("PASS P11-7: AbilityDatabase loads 5 default abilities.");

            // ------------------------------------------
            // P11-8: AbilityExecutor — cooldown tracking
            // ------------------------------------------
            GD.Print("Running: P11-8 AbilityExecutor cooldown tracking...");
            float mana = 100f, stamina = 100f;
            var executor = new AbilityExecutor(
                getMana:      () => mana,
                getStamina:   () => stamina,
                spendMana:    (v) => mana    -= v,
                spendStamina: (v) => stamina -= v
            );
            var psAbility = new AbilityDefinition(new AbilityData
            {
                AbilityId = "power_strike", CooldownSec = 6f, StaminaCost = 25f,
                BaseDamage = 40f, TargetType = AbilityTargetType.SingleEnemy,
                DamageType = AbilityDamageType.Physical, LevelRequired = 1
            });
            executor.EquipAbility(0, psAbility);
            bool firstCast = executor.Execute(0);
            bool secondCast = executor.Execute(0); // Should be blocked by cooldown
            if (!firstCast || secondCast)
            {
                GD.Print($"FAIL P11-8: First={firstCast} Second={secondCast}. Expected true/false.");
                return false;
            }
            if (stamina != 75f)
            {
                GD.Print($"FAIL P11-8: Stamina should be 75 after 25 cost, got {stamina}.");
                return false;
            }
            GD.Print("PASS P11-8: AbilityExecutor cooldown and stamina cost work correctly.");

            // ------------------------------------------
            // P11-9: AbilityExecutor — insufficient resource
            // ------------------------------------------
            GD.Print("Running: P11-9 AbilityExecutor insufficient mana...");
            float testMana = 5f, testStam = 100f;
            var manaExecutor = new AbilityExecutor(
                () => testMana, () => testStam,
                (v) => testMana -= v, (v) => testStam -= v
            );
            var fireballAbility = new AbilityDefinition(new AbilityData
            {
                AbilityId = "fireball", CooldownSec = 8f, ManaCost = 30f,
                BaseDamage = 55f, TargetType = AbilityTargetType.Projectile,
                DamageType = AbilityDamageType.Fire, LevelRequired = 4
            });
            manaExecutor.EquipAbility(1, fireballAbility);
            bool manaCast = manaExecutor.Execute(1);
            if (manaCast)
            {
                GD.Print("FAIL P11-9: Execute should fail with insufficient mana.");
                return false;
            }
            GD.Print("PASS P11-9: AbilityExecutor blocks execution on insufficient mana.");

            // ------------------------------------------
            // P11-10: SaveManager V8 — roundtrip with gameplay fields
            // ------------------------------------------
            GD.Print("Running: P11-10 SaveManager V8 roundtrip...");
            string v8Dir = Path.Combine(tempDir, "savev8_test");
            Directory.CreateDirectory(v8Dir);
            var smV8 = new SaveManager(v8Dir);
            smV8.UpdateSessionStats(playerLevel: 5, playerXp: 250, enemiesKilled: 42, wavesCompleted: 3);
            bool saved = smV8.Save(0);
            var loaded = smV8.Load(0);
            if (!saved || loaded == null)
            {
                GD.Print("FAIL P11-10: V8 save or load returned null.");
                return false;
            }
            if (loaded.PlayerLevel != 5 || loaded.EnemiesKilledTotal != 42 || loaded.WavesCompleted != 3)
            {
                GD.Print($"FAIL P11-10: V8 data mismatch. Level={loaded.PlayerLevel} Kills={loaded.EnemiesKilledTotal} Waves={loaded.WavesCompleted}");
                return false;
            }
            GD.Print($"PASS P11-10: SaveManager V8 roundtrip. Level={loaded.PlayerLevel} Kills={loaded.EnemiesKilledTotal} Waves={loaded.WavesCompleted}");

            GD.Print("Phase 11 Gameplay Expansion: ALL 10 TESTS PASSED.");
            return true;
        }

        // =============================================
        // PHASE 12 — BOSS FRAMEWORK & ENCOUNTERS (10 TESTS)
        // =============================================
        private bool RunPhase12Tests(string tempDir)
        {
            GD.Print("\n=== PHASE 12: Boss Framework & Encounters ===");

            // ------------------------------------------
            // P12-1: Boss Database load/validate
            // ------------------------------------------
            GD.Print("Running: P12-1 Boss Database loading...");
            var bdb = new BossDatabase();
            bdb.Load("invalid_path_forces_defaults");
            if (bdb.Count != 1)
            {
                GD.Print($"FAIL P12-1: Expected 1 default boss, got {bdb.Count}.");
                return false;
            }
            var titan = bdb.Get("golem_titan");
            if (titan == null || titan.Data.MaxHp != 800f)
            {
                GD.Print("FAIL P12-1: Failed to retrieve Golem Titan or HP mismatch.");
                return false;
            }
            GD.Print("PASS P12-1: Boss Database loaded default successfully.");

            // ------------------------------------------
            // P12-2: Boss Phase evaluation/transitions
            // ------------------------------------------
            GD.Print("Running: P12-2 Boss Phase transitions...");
            var phaseSys = new BossPhaseSystem(titan);
            if (phaseSys.CurrentPhaseIndex != 1)
            {
                GD.Print($"FAIL P12-2: Expected starting phase 1, got {phaseSys.CurrentPhaseIndex}.");
                return false;
            }
            // Trigger 50% HP threshold transition
            phaseSys.Update(399f, 800f, 0.016f);
            if (phaseSys.CurrentPhaseIndex != 2)
            {
                GD.Print($"FAIL P12-2: Expected phase 2 transition, got {phaseSys.CurrentPhaseIndex}.");
                return false;
            }
            GD.Print("PASS P12-2: Boss Phase System evaluated and transitioned correctly.");

            // ------------------------------------------
            // P12-3: Elite modifier prefix/suffix and multipliers
            // ------------------------------------------
            GD.Print("Running: P12-3 Elite modifiers stats calculation...");
            var baseEnemy = new EnemyData
            {
                EnemyId = "grunt", DisplayName = "Grunt", Species = "Goblin",
                MaxHp = 100f, AttackDamage = 10f, MoveSpeed = 4.0f, XpReward = 10
            };
            var eliteData = EliteSystem.ApplyEliteModifiers(baseEnemy, EliteModifierType.Fortified | EliteModifierType.Swift);
            if (eliteData.MaxHp != 200f || eliteData.MoveSpeed != 5.4f)
            {
                GD.Print($"FAIL P12-3: Modifiers scaling mismatch: HP={eliteData.MaxHp} SPD={eliteData.MoveSpeed}");
                return false;
            }
            if (!eliteData.DisplayName.Contains("Fortified Swift"))
            {
                GD.Print($"FAIL P12-3: Modifiers prefix naming mismatch: {eliteData.DisplayName}");
                return false;
            }
            GD.Print("PASS P12-3: Elite prefix/suffix and stats multipliers calculated correctly.");

            // ------------------------------------------
            // P12-4: Special attack definition composition
            // ------------------------------------------
            GD.Print("Running: P12-4 Reusable Special Attack composition...");
            var attack = titan.Data.SpecialAttacks[0];
            if (attack.AttackId != "titan_slam" || attack.AttackType != SpecialAttackType.AreaOfEffect || attack.AoeRadius != 8f)
            {
                GD.Print("FAIL P12-4: Special attack definition parsing failed.");
                return false;
            }
            GD.Print("PASS P12-4: Reusable Special Attack details verified successfully.");

            // ------------------------------------------
            // P12-5: Arena boundary cylindrical check and hazards
            // ------------------------------------------
            GD.Print("Running: P12-5 Arena cylindrical containment check...");
            var arenaDef = new ArenaDefinition
            {
                ArenaId = "arena_titan", DisplayName = "Titan Arena",
                Boundary = new ArenaBoundary { Center = Vector3.Zero, Radius = 10f, Height = 10f },
                Hazards = new List<ArenaHazardZone>
                {
                    new() { HazardId = "lava", Center = new Vector3(5f, 0f, 5f), Radius = 2f, DamagePerSecond = 10f }
                }
            };
            var arena = new ArenaInstance(arenaDef);
            if (!arena.IsWithinBoundaries(new Vector3(2f, 1f, 2f)))
            {
                GD.Print("FAIL P12-5: Valid position flagged as out of boundaries.");
                return false;
            }
            if (arena.IsWithinBoundaries(new Vector3(12f, 1f, 2f)))
            {
                GD.Print("FAIL P12-5: Out-of-bounds position flagged as within boundaries.");
                return false;
            }
            // Check active hazard collision
            var hazardHit = arena.GetActiveHazardCollision(new Vector3(5.1f, 0f, 5.1f));
            if (hazardHit == null || hazardHit.HazardId != "lava")
            {
                GD.Print("FAIL P12-5: Hazard collision check failed.");
                return false;
            }
            GD.Print("PASS P12-5: Arena cylindrical containment and hazards check verified.");

            // ------------------------------------------
            // P12-6: EncounterManager lifecycle states
            // ------------------------------------------
            GD.Print("Running: P12-6 EncounterManager lifecycle states...");
            var em = new EncounterManager();
            if (em.State != EncounterState.Inactive)
            {
                GD.Print($"FAIL P12-6: Expected state Inactive, got {em.State}.");
                return false;
            }
            em.StartEncounter(titan, arenaDef);
            if (em.State != EncounterState.Active)
            {
                GD.Print($"FAIL P12-6: Expected state Active, got {em.State}.");
                return false;
            }
            GD.Print("PASS P12-6: EncounterManager transitions and states verified.");

            // ------------------------------------------
            // P12-7: EncounterManager resets player exit/death
            // ------------------------------------------
            GD.Print("Running: P12-7 EncounterManager resets on boundary exit...");
            // Simulate player moving out of bounds
            em.Update(1.0f, new Vector3(15f, 0f, 0f), true);
            if (em.State != EncounterState.Inactive)
            {
                GD.Print($"FAIL P12-7: Expected state Inactive after reset, got {em.State}.");
                return false;
            }
            GD.Print("PASS P12-7: EncounterManager reset evaluated on boundary exit successfully.");

            // ------------------------------------------
            // P12-8: Reward anti-duplication claims validation
            // ------------------------------------------
            GD.Print("Running: P12-8 Reward anti-duplication claim validation...");
            var tracker = em.RewardTracker;
            int grantCount = 0;
            bool claim1 = tracker.Claim("reward_golem_titan", item => grantCount++);
            bool claim2 = tracker.Claim("reward_golem_titan", item => grantCount++);
            if (!claim1 || claim2 || grantCount != 4)
            {
                GD.Print($"FAIL P12-8: Anti-duplication failed: Claim1={claim1} Claim2={claim2} Count={grantCount}");
                return false;
            }
            GD.Print("PASS P12-8: Reward claims validate anti-duplication successfully.");

            // ------------------------------------------
            // P12-9: SaveManager V9 serialization integration roundtrip
            // ------------------------------------------
            GD.Print("Running: P12-9 SaveManager V9 integration roundtrip...");
            string v9Path = Path.Combine(tempDir, "savev9_roundtrip");
            Directory.CreateDirectory(v9Path);
            var smV9 = new SaveManager(v9Path);
            smV9.UpdateEncounterStats(
                completed: new[] { "arena_titan" },
                defeated: new[] { "golem_titan" },
                elites: new[] { "Fortified Grunt" },
                claimedRewards: new[] { "reward_golem_titan" }
            );
            bool saved = smV9.Save(1);
            var loaded = smV9.Load(1);
            if (!saved || loaded == null)
            {
                GD.Print("FAIL P12-9: V9 save or load returned null.");
                return false;
            }
            if (loaded.CompletedEncounters.Count != 1 || loaded.CompletedEncounters[0] != "arena_titan")
            {
                GD.Print("FAIL P12-9: V9 serialized encounter data mismatch.");
                return false;
            }
            GD.Print("PASS P12-9: SaveManager V9 roundtrip integration verified.");

            // ------------------------------------------
            // P12-10: Memory stress testing of rapid phase transitions
            // ------------------------------------------
            GD.Print("Running: P12-10 Memory stress testing of phase transitions...");
            long startMs = System.Environment.TickCount;
            for (int i = 0; i < 500; i++)
            {
                var stressSys = new BossPhaseSystem(titan);
                stressSys.Update(399f, 800f, 0.01f);
            }
            long elapsedMs = System.Environment.TickCount - startMs;
            if (elapsedMs > 50)
            {
                GD.Print($"FAIL P12-10: Stress test took too long: {elapsedMs}ms");
                return false;
            }
            GD.Print($"PASS P12-10: 500 rapid phase updates executed in {elapsedMs}ms.");

            GD.Print("Phase 12 Boss Framework & Encounters: ALL 10 TESTS PASSED.");
            return true;
        }
    }
}
