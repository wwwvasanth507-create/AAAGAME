using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Player;
using HeroOfEternia.Player.Abilities;
using HeroOfEternia.Player.Resources;
using HeroOfEternia.Player.Progression;
using PlayerResourceManager = HeroOfEternia.Player.Resources.ResourceManager;

namespace HeroOfEternia.Tests
{
    /// <summary>
    /// Comprehensive test suite for the Ability System (Phase 13).
    /// Tests: ability activation, cooldowns, resource consumption, target validation,
    /// save/load, progression, loadouts, and stress testing.
    /// </summary>
    public static class AbilitySystemTests
    {
        private static int _passed = 0;
        private static int _failed = 0;
        private static readonly List<string> _failures = new();

        public static bool RunAll()
        {
            _passed = 0;
            _failed = 0;
            _failures.Clear();

            Logger.Info("========================================");
            Logger.Info("ABILITY SYSTEM TESTS - PHASE 13");
            Logger.Info("========================================");

            TestAbilityDefinition();
            TestAbilityCategory();
            TestAbilityState();
            TestAbilityActivation();
            TestCooldowns();
            TestResourceConsumption();
            TestTargetValidation();
            TestCancellation();
            TestInterruption();
            TestCharges();
            TestGlobalCooldown();
            TestProgression();
            TestLoadout();
            TestSaveLoad();
            TestEffectsManager();
            TestStress();

            Logger.Info("========================================");
            Logger.Info($"RESULTS: {_passed} passed, {_failed} failed");
            Logger.Info("========================================");

            if (_failures.Count > 0)
            {
                Logger.Warning("FAILURES:");
                foreach (var f in _failures)
                    Logger.Warning($"  - {f}");
            }

            return _failed == 0;
        }

        private static void Assert(bool condition, string testName)
        {
            if (condition)
            {
                _passed++;
                Logger.Info($"  ✓ {testName}");
            }
            else
            {
                _failed++;
                _failures.Add(testName);
                Logger.Error($"  ✗ {testName} - FAILED");
            }
        }

        // ----------------------------------------------------------------
        // Test: Ability Definition
        // ----------------------------------------------------------------
        private static void TestAbilityDefinition()
        {
            Logger.Info("\n--- Ability Definition Tests ---");

            // Valid ability
            var data = new AbilityData
            {
                AbilityId = "test_ability",
                DisplayName = "Test Ability",
                CooldownSec = 5f,
                BaseDamage = 50f,
                Range = 10f
            };
            var def = new AbilityDefinition(data);
            Assert(def.Data.AbilityId == "test_ability", "AbilityDefinition: ID matches");
            Assert(def.DoesDamage, "AbilityDefinition: DoesDamage true");
            Assert(!def.DoesHeal, "AbilityDefinition: DoesHeal false");
            Assert(!def.HasShield, "AbilityDefinition: HasShield false");
            Assert(def.IsInstant, "AbilityDefinition: IsInstant true");
            Assert(!def.HasCastTime, "AbilityDefinition: HasCastTime false");
            Assert(def.IsUnlocked(1), "AbilityDefinition: IsUnlocked at level 1");
            Assert(!def.IsUnlocked(0), "AbilityDefinition: Not unlocked at level 0");

            // Ability with cast time
            var castData = new AbilityData
            {
                AbilityId = "cast_ability",
                CastTime = 2f,
                CooldownSec = 10f
            };
            var castDef = new AbilityDefinition(castData);
            Assert(castDef.HasCastTime, "AbilityDefinition: HasCastTime true");
            Assert(!castDef.IsInstant, "AbilityDefinition: Not instant with cast time");

            // Ability with healing
            var healData = new AbilityData
            {
                AbilityId = "heal_ability",
                BaseHealing = 100f,
                CooldownSec = 8f
            };
            var healDef = new AbilityDefinition(healData);
            Assert(healDef.DoesHeal, "AbilityDefinition: DoesHeal true");

            // Ability with shield
            var shieldData = new AbilityData
            {
                AbilityId = "shield_ability",
                ShieldAmount = 50f,
                CooldownSec = 15f
            };
            var shieldDef = new AbilityDefinition(shieldData);
            Assert(shieldDef.HasShield, "AbilityDefinition: HasShield true");

            // Ability with charges
            var chargeData = new AbilityData
            {
                AbilityId = "charge_ability",
                MaxCharges = 3,
                ChargeRechargeSec = 5f,
                CooldownSec = 0f
            };
            var chargeDef = new AbilityDefinition(chargeData);
            Assert(chargeDef.HasCharges, "AbilityDefinition: HasCharges true");

            // Resource cost query
            var resourceData = new AbilityData
            {
                AbilityId = "resource_ability",
                ManaCost = 30f,
                StaminaCost = 20f,
                EnergyCost = 15f,
                CooldownSec = 5f
            };
            var resourceDef = new AbilityDefinition(resourceData);
            Assert(resourceDef.GetResourceCost("mana") == 30f, "AbilityDefinition: Mana cost");
            Assert(resourceDef.GetResourceCost("stamina") == 20f, "AbilityDefinition: Stamina cost");
            Assert(resourceDef.GetResourceCost("energy") == 15f, "AbilityDefinition: Energy cost");
            Assert(resourceDef.GetResourceCost("unknown") == 0f, "AbilityDefinition: Unknown cost returns 0");

            // Invalid ability
            try
            {
                var invalid = new AbilityDefinition(new AbilityData { AbilityId = "" });
                Assert(false, "AbilityDefinition: Should throw on empty ID");
            }
            catch (ArgumentException)
            {
                Assert(true, "AbilityDefinition: Throws on empty ID");
            }

            // Negative cooldown
            try
            {
                var invalid = new AbilityDefinition(new AbilityData { AbilityId = "bad", CooldownSec = -1f });
                Assert(false, "AbilityDefinition: Should throw on negative cooldown");
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert(true, "AbilityDefinition: Throws on negative cooldown");
            }
        }

        // ----------------------------------------------------------------
        // Test: Ability Category
        // ----------------------------------------------------------------
        private static void TestAbilityCategory()
        {
            Logger.Info("\n--- Ability Category Tests ---");

            var manager = new CategoryManager();
            Assert(manager.Count == 11, "CategoryManager: 11 default categories");
            Assert(manager.Contains("Melee"), "CategoryManager: Contains Melee");
            Assert(manager.Contains("Magic"), "CategoryManager: Contains Magic");
            Assert(manager.Contains("Ultimate"), "CategoryManager: Contains Ultimate");
            Assert(!manager.Contains("Nonexistent"), "CategoryManager: Does not contain nonexistent");

            var melee = manager.Get("Melee");
            Assert(melee != null && melee.DisplayName == "Melee", "CategoryManager: Get Melee");

            // Add custom category
            manager.Register(new CategoryDefinition
            {
                Id = "Custom",
                DisplayName = "Custom Category",
                SortOrder = 99
            });
            Assert(manager.Count == 12, "CategoryManager: 12 after adding custom");
            Assert(manager.Contains("Custom"), "CategoryManager: Contains custom category");

            // Unlocked categories
            var unlocked = manager.GetUnlocked(1);
            Assert(unlocked.Count > 0, "CategoryManager: Has unlocked at level 1");

            // Ultimate not unlocked by default
            var ultimate = manager.Get("Ultimate");
            Assert(ultimate != null && !ultimate.IsUnlockedByDefault, "CategoryManager: Ultimate not default unlocked");
        }

        // ----------------------------------------------------------------
        // Test: Ability State
        // ----------------------------------------------------------------
        private static void TestAbilityState()
        {
            Logger.Info("\n--- Ability State Tests ---");

            var state = new AbilityState("test_ability", 2, 5f);
            Assert(state.AbilityId == "test_ability", "AbilityState: ID matches");
            Assert(state.CurrentCharges == 2, "AbilityState: Initial charges = 2");
            Assert(state.MaxCharges == 2, "AbilityState: Max charges = 2");
            Assert(state.IsReady, "AbilityState: IsReady initially");
            Assert(!state.IsOnCooldown, "AbilityState: Not on cooldown initially");
            Assert(!state.IsCasting, "AbilityState: Not casting initially");

            // Start cooldown
            state.StartCooldown(5f);
            Assert(state.IsOnCooldown, "AbilityState: On cooldown after start");
            Assert(!state.IsReady, "AbilityState: Not ready on cooldown");

            // Tick cooldown
            state.TickCooldown(3f);
            Assert(state.CooldownRemaining == 2f, "AbilityState: Cooldown ticked correctly");

            state.TickCooldown(2f);
            Assert(!state.IsOnCooldown, "AbilityState: Cooldown expired");

            // Start cast
            state.StartCast(2f);
            Assert(state.IsCasting, "AbilityState: Is casting");
            Assert(!state.IsReady, "AbilityState: Not ready while casting");

            state.TickCast(1f);
            Assert(state.CastTimeRemaining == 1f, "AbilityState: Cast ticked correctly");

            state.TickCast(1f);
            Assert(!state.IsCasting, "AbilityState: Cast completed");

            // Interrupt
            state.StartCast(3f);
            state.Interrupt();
            Assert(!state.IsCasting, "AbilityState: Interrupted");
            Assert(state.CastTimeRemaining == 0f, "AbilityState: Cast time reset after interrupt");

            // Consume charge
            Assert(state.ConsumeCharge(), "AbilityState: Consume charge succeeds");
            Assert(state.CurrentCharges == 1, "AbilityState: Charges reduced to 1");
            Assert(state.ConsumeCharge(), "AbilityState: Consume second charge");
            Assert(state.CurrentCharges == 0, "AbilityState: Charges reduced to 0");
            Assert(!state.ConsumeCharge(), "AbilityState: Cannot consume when 0 charges");
        }

        // ----------------------------------------------------------------
        // Test: Ability Activation
        // ----------------------------------------------------------------
        private static void TestAbilityActivation()
        {
            Logger.Info("\n--- Ability Activation Tests ---");

            // Setup
            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            abilityManager.RegisterAllFromDatabase();
            Assert(abilityManager.RegisteredAbilityCount > 0, "AbilityManager: Registered abilities from database");

            // Activate power_strike
            var result = abilityManager.ActivateAbility("power_strike");
            Assert(result.Success, "AbilityManager: Activate power_strike succeeds");
            Assert(result.AbilityId == "power_strike", "AbilityManager: Result has correct ability ID");
            Assert(result.DamageDealt == 40f, "AbilityManager: Damage dealt matches");

            // Activate unknown ability
            var unknownResult = abilityManager.ActivateAbility("nonexistent");
            Assert(!unknownResult.Success, "AbilityManager: Unknown ability fails");

            // Activate on cooldown
            var cdResult = abilityManager.ActivateAbility("power_strike");
            Assert(!cdResult.Success, "AbilityManager: Fails on cooldown");

            // Activate from slot
            abilityManager.BindSlot(0, "power_strike");
            var slotResult = abilityManager.ActivateSlot(0);
            Assert(!slotResult.Success, "AbilityManager: Slot on cooldown fails");

            // Empty slot
            abilityManager.UnbindSlot(1);
            var emptyResult = abilityManager.ActivateSlot(1);
            Assert(!emptyResult.Success, "AbilityManager: Empty slot fails");
        }

        // ----------------------------------------------------------------
        // Test: Cooldowns
        // ----------------------------------------------------------------
        private static void TestCooldowns()
        {
            Logger.Info("\n--- Cooldown Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            abilityManager.RegisterAbility("power_strike");
            abilityManager.BindSlot(0, "power_strike");

            // Activate
            abilityManager.ActivateSlot(0);
            Assert(abilityManager.GetSlotCooldownRemaining(0) > 0f, "Cooldown: Slot on cooldown after activation");
            Assert(abilityManager.GetCooldownRemaining("power_strike") > 0f, "Cooldown: Ability on cooldown");

            // Tick cooldown
            abilityManager.Tick(3f);
            Assert(abilityManager.GetCooldownRemaining("power_strike") <= 3f, "Cooldown: Ticked correctly");

            // Full tick to complete
            abilityManager.Tick(10f);
            Assert(abilityManager.GetCooldownRemaining("power_strike") == 0f, "Cooldown: Completed after full tick");
            Assert(abilityManager.IsSlotReady(0), "Cooldown: Slot ready after cooldown");
        }

        // ----------------------------------------------------------------
        // Test: Resource Consumption
        // ----------------------------------------------------------------
        private static void TestResourceConsumption()
        {
            Logger.Info("\n--- Resource Consumption Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            abilityManager.RegisterAbility("fireball");
            abilityManager.BindSlot(0, "fireball");

            // Check initial mana
            var manaPool = resourceManager.GetPool(ResourceType.Mana);
            Assert(manaPool != null, "Resource: Mana pool exists");
            Assert(manaPool!.Current == 100f, "Resource: Initial mana = 100");

            // Activate fireball (costs 30 mana)
            var result = abilityManager.ActivateSlot(0);
            Assert(result.Success, "Resource: Fireball activation succeeds");
            Assert(manaPool.Current == 70f, "Resource: Mana reduced to 70 after fireball");

            // Not enough mana
            manaPool.Spend(70f); // Now 0 mana
            var noManaResult = abilityManager.ActivateSlot(0);
            Assert(!noManaResult.Success, "Resource: Fails with no mana");

            // Restore and try again
            manaPool.Restore(100f);
            var restoredResult = abilityManager.ActivateSlot(0);
            Assert(restoredResult.Success, "Resource: Succeeds after mana restore");
        }

        // ----------------------------------------------------------------
        // Test: Target Validation
        // ----------------------------------------------------------------
        private static void TestTargetValidation()
        {
            Logger.Info("\n--- Target Validation Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            abilityManager.RegisterAbility("barrier");
            abilityManager.RegisterAbility("power_strike");

            // Self-target (barrier) - no target needed
            var selfResult = abilityManager.ActivateAbility("barrier");
            Assert(selfResult.Success, "Target: Self-target succeeds without target");

            // Single enemy with target
            var targetResult = abilityManager.ActivateAbility("power_strike", new object());
            Assert(targetResult.Success, "Target: Single enemy with target succeeds");
        }

        // ----------------------------------------------------------------
        // Test: Cancellation
        // ----------------------------------------------------------------
        private static void TestCancellation()
        {
            Logger.Info("\n--- Cancellation Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            // Fireball has cast time, so we can cancel it
            abilityManager.RegisterAbility("fireball");
            abilityManager.BindSlot(0, "fireball");

            // Activate (starts cast)
            var result = abilityManager.ActivateSlot(0);
            Assert(result.Success, "Cancel: Fireball activation succeeds");

            // Cancel
            bool cancelled = abilityManager.CancelAbility("fireball");
            Assert(cancelled, "Cancel: Fireball cancelled successfully");

            // Try to cancel again (should fail, already cancelled)
            bool cancelAgain = abilityManager.CancelAbility("fireball");
            Assert(!cancelAgain, "Cancel: Cannot cancel already cancelled ability");

            // Cancel non-existent
            bool cancelNonexistent = abilityManager.CancelAbility("nonexistent");
            Assert(!cancelNonexistent, "Cancel: Cannot cancel non-existent ability");
        }

        // ----------------------------------------------------------------
        // Test: Interruption
        // ----------------------------------------------------------------
        private static void TestInterruption()
        {
            Logger.Info("\n--- Interruption Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            abilityManager.RegisterAbility("fireball");
            abilityManager.BindSlot(0, "fireball");

            // Activate
            abilityManager.ActivateSlot(0);

            // Interrupt
            bool interrupted = abilityManager.InterruptAbility("fireball", "Stunned");
            Assert(interrupted, "Interrupt: Fireball interrupted");

            // Interrupt all
            abilityManager.ActivateSlot(0);
            abilityManager.InterruptAll("Boss Stomp");
            var state = abilityManager.GetAbilityState("fireball");
            Assert(state != null && !state.IsCasting, "Interrupt: All abilities interrupted");
        }

        // ----------------------------------------------------------------
        // Test: Charges
        // ----------------------------------------------------------------
        private static void TestCharges()
        {
            Logger.Info("\n--- Charge Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            // Dodge roll has 2 charges
            abilityManager.RegisterAbility("dodge_roll");
            abilityManager.BindSlot(0, "dodge_roll");

            Assert(abilityManager.GetCharges("dodge_roll") == 2, "Charges: Initial = 2");

            // Use first charge
            abilityManager.ActivateSlot(0);
            Assert(abilityManager.GetCharges("dodge_roll") == 1, "Charges: After first use = 1");

            // Use second charge
            abilityManager.ActivateSlot(0);
            Assert(abilityManager.GetCharges("dodge_roll") == 0, "Charges: After second use = 0");

            // No charges left
            var noChargeResult = abilityManager.ActivateSlot(0);
            Assert(!noChargeResult.Success, "Charges: Fails with no charges");
        }

        // ----------------------------------------------------------------
        // Test: Global Cooldown
        // ----------------------------------------------------------------
        private static void TestGlobalCooldown()
        {
            Logger.Info("\n--- Global Cooldown Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            abilityManager.RegisterAbility("power_strike");
            abilityManager.RegisterAbility("dodge_roll");
            abilityManager.BindSlot(0, "power_strike");
            abilityManager.BindSlot(1, "dodge_roll");

            // Activate first ability
            abilityManager.ActivateSlot(0);
            Assert(abilityManager.GlobalCooldownRemaining > 0f, "GCD: Active after ability use");

            // Second ability should fail due to GCD
            var gcdResult = abilityManager.ActivateSlot(1);
            Assert(!gcdResult.Success, "GCD: Second ability fails during GCD");

            // Tick past GCD
            abilityManager.Tick(1f);
            Assert(abilityManager.GlobalCooldownRemaining == 0f, "GCD: Expired after tick");
        }

        // ----------------------------------------------------------------
        // Test: Progression
        // ----------------------------------------------------------------
        private static void TestProgression()
        {
            Logger.Info("\n--- Progression Tests ---");

            var progression = new PlayerProgression();
            Assert(progression.Level == 1, "Progression: Initial level = 1");
            Assert(progression.Experience == 0f, "Progression: Initial XP = 0");
            Assert(progression.XPForNextLevel > 0f, "Progression: XP for next level > 0");

            // Add XP
            progression.AddExperience(50f);
            Assert(progression.Experience == 50f, "Progression: XP added correctly");
            Assert(progression.Level == 1, "Progression: Still level 1");

            // Level up
            float xpNeeded = progression.XPForNextLevel;
            progression.AddExperience(xpNeeded);
            Assert(progression.Level >= 2, "Progression: Leveled up");

            // Stat growth
            float baseHealth = progression.GetBaseHealth();
            Assert(baseHealth > 100f, "Progression: Base health scales with level");

            // Prestige
            // Set to max level
            while (progression.Level < PlayerProgression.MaxLevel)
                progression.AddExperience(progression.XPForNextLevel + 1);

            Assert(progression.Level == PlayerProgression.MaxLevel, "Progression: Reached max level");
            Assert(progression.TryPrestige(), "Progression: Prestige succeeds at max level");
            Assert(progression.PrestigeLevel == 1, "Progression: Prestige level = 1");
            Assert(progression.Level == 1, "Progression: Level reset to 1 after prestige");

            // Save/load
            var saveData = progression.CreateSaveData();
            Assert(saveData.Level == 1, "Progression: Save data level = 1");
            Assert(saveData.PrestigeLevel == 1, "Progression: Save data prestige = 1");

            var newProgression = new PlayerProgression();
            newProgression.LoadFromSaveData(saveData);
            Assert(newProgression.Level == 1, "Progression: Loaded level = 1");
            Assert(newProgression.PrestigeLevel == 1, "Progression: Loaded prestige = 1");
        }

        // ----------------------------------------------------------------
        // Test: Loadout
        // ----------------------------------------------------------------
        private static void TestLoadout()
        {
            Logger.Info("\n--- Loadout Tests ---");

            var loadoutManager = new LoadoutManager();
            Assert(loadoutManager.AllLoadouts.Count == 3, "Loadout: 3 default loadouts");
            Assert(loadoutManager.ActiveLoadout != null, "Loadout: Active loadout exists");

            // Create new loadout
            var newLoadout = loadoutManager.CreateLoadout("Test Loadout");
            Assert(newLoadout != null, "Loadout: Created new loadout");
            Assert(loadoutManager.AllLoadouts.Count == 4, "Loadout: 4 loadouts after creation");

            // Assign ability
            loadoutManager.AssignAbility(0, "power_strike");
            Assert(loadoutManager.GetActiveSlotAbility(0) == "power_strike", "Loadout: Slot 0 has power_strike");

            // Switch loadout
            Assert(loadoutManager.SwitchLoadout(0), "Loadout: Switched to index 0");
            Assert(loadoutManager.ActiveLoadoutIndex == 0, "Loadout: Active index = 0");

            // Rename
            Assert(loadoutManager.RenameLoadout(0, "Adventure+"), "Loadout: Renamed");
            Assert(loadoutManager.AllLoadouts[0].LoadoutName == "Adventure+", "Loadout: Name updated");

            // Delete
            Assert(loadoutManager.DeleteLoadout(3), "Loadout: Deleted index 3");
            Assert(loadoutManager.AllLoadouts.Count == 3, "Loadout: 3 loadouts after deletion");

            // Save/load
            var saveData = loadoutManager.CreateSaveData();
            Assert(saveData.Loadouts.Count == 3, "Loadout: Save data has 3 loadouts");

            var newManager = new LoadoutManager();
            newManager.LoadFromSaveData(saveData);
            Assert(newManager.AllLoadouts.Count == 3, "Loadout: Loaded 3 loadouts");
        }

        // ----------------------------------------------------------------
        // Test: Save/Load
        // ----------------------------------------------------------------
        private static void TestSaveLoad()
        {
            Logger.Info("\n--- Save/Load Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            abilityManager.RegisterAllFromDatabase();
            abilityManager.BindSlot(0, "power_strike");
            abilityManager.BindSlot(1, "fireball");

            // Activate some abilities to create state
            abilityManager.ActivateSlot(0);
            abilityManager.Tick(2f);

            // Create save data
            var saveData = abilityManager.CreateSaveData();
            Assert(saveData != null, "Save/Load: Save data created");
            Assert(saveData.SlotBindings.Count == 2, "Save/Load: 2 slot bindings saved");
            Assert(saveData.AbilityStates.Count > 0, "Save/Load: Ability states saved");

            // Create new manager and load
            var newManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);
            newManager.RegisterAllFromDatabase();
            newManager.LoadFromSaveData(saveData);

            Assert(newManager.GetSlotAbility(0) == "power_strike", "Save/Load: Slot 0 restored");
            Assert(newManager.GetSlotAbility(1) == "fireball", "Save/Load: Slot 1 restored");
            Assert(newManager.GetCooldownRemaining("power_strike") > 0f, "Save/Load: Cooldown restored");
        }

        // ----------------------------------------------------------------
        // Test: Effects Manager
        // ----------------------------------------------------------------
        private static void TestEffectsManager()
        {
            Logger.Info("\n--- Effects Manager Tests ---");

            var effectsManager = new EffectsManager();

            // Register effect
            var effect = new AbilityEffect
            {
                EffectId = "burn",
                DisplayName = "Burn",
                Type = EffectType.Damage,
                BaseValue = 10f,
                Duration = 5f,
                TickInterval = 1f,
                MaxStacks = 3
            };
            effectsManager.RegisterEffect(effect);
            Assert(effectsManager.GetDefinition("burn") != null, "Effects: Effect registered");

            // Apply effect
            effectsManager.ApplyEffect("enemy_1", "burn", "fireball");
            Assert(effectsManager.HasEffect("enemy_1", "burn"), "Effects: Effect applied");
            Assert(effectsManager.GetEffectStacks("enemy_1", "burn") == 1, "Effects: 1 stack");

            // Stack effect
            effectsManager.ApplyEffect("enemy_1", "burn", "fireball");
            Assert(effectsManager.GetEffectStacks("enemy_1", "burn") == 2, "Effects: 2 stacks");

            // Max stacks
            effectsManager.ApplyEffect("enemy_1", "burn", "fireball");
            effectsManager.ApplyEffect("enemy_1", "burn", "fireball");
            Assert(effectsManager.GetEffectStacks("enemy_1", "burn") == 3, "Effects: Max 3 stacks");

            // Tick
            effectsManager.Tick("enemy_1", 1f);
            Assert(effectsManager.HasEffect("enemy_1", "burn"), "Effects: Still active after tick");

            // Remove
            effectsManager.RemoveEffect("enemy_1", "burn");
            Assert(!effectsManager.HasEffect("enemy_1", "burn"), "Effects: Removed");

            // Remove all
            effectsManager.ApplyEffect("enemy_1", "burn", "fireball");
            effectsManager.RemoveAllEffects("enemy_1");
            Assert(!effectsManager.HasEffect("enemy_1", "burn"), "Effects: All removed");
        }

        // ----------------------------------------------------------------
        // Test: Stress
        // ----------------------------------------------------------------
        private static void TestStress()
        {
            Logger.Info("\n--- Stress Tests ---");

            var db = new AbilityDatabase();
            db.Load("Settings");
            var resourceManager = new PlayerResourceManager();
            var progression = new PlayerProgression();
            var effectsManager = new EffectsManager();
            var loadoutManager = new LoadoutManager();
            var abilityManager = new AbilityManager(db, resourceManager, progression, effectsManager, loadoutManager);

            // Register all abilities
            abilityManager.RegisterAllFromDatabase();
            int count = abilityManager.RegisteredAbilityCount;
            Assert(count >= 5, $"Stress: Registered {count} abilities (expected >= 5)");

            // Rapid activation
            for (int i = 0; i < 20; i++)
            {
                abilityManager.Tick(0.5f);
                // Try to activate various abilities
                abilityManager.ActivateAbility("power_strike");
                abilityManager.ActivateAbility("dodge_roll");
            }

            // Verify no crashes
            Assert(true, "Stress: No crashes during rapid activation");

            // Many simultaneous abilities
            for (int i = 0; i < 5; i++)
            {
                abilityManager.BindSlot(i, "power_strike");
            }

            // Tick many times
            for (int i = 0; i < 100; i++)
            {
                abilityManager.Tick(0.1f);
            }

            Assert(true, "Stress: No crashes during extended tick");

            // Debug summary
            string summary = abilityManager.DebugSummary();
            Assert(!string.IsNullOrEmpty(summary), "Stress: Debug summary not empty");

            Logger.Info($"  Stress Summary: {summary}");
        }
    }
}