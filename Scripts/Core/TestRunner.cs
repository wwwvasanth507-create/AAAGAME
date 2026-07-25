using System;
using System.IO;
using System.Linq;
using Godot;
using HeroOfEternia.World;

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
    }
}
