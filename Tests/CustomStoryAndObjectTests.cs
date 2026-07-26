using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Dialogue;
using HeroOfEternia.Interaction;
using HeroOfEternia.Story;
using HeroOfEternia.World;

namespace HeroOfEternia.Tests
{
    public static class CustomStoryAndObjectTests
    {
        private static int _passed = 0;
        private static int _failed = 0;
        private static readonly List<string> _failures = new();

        public static void RunAll()
        {
            _passed = 0;
            _failed = 0;
            _failures.Clear();

            Logger.Info("==================================================");
            Logger.Info("RUNNING CUSTOM STORY AND OBJECT MODEL TESTS");
            Logger.Info("==================================================");

            TestCustomStoryDatabase();
            TestCustomStoryManagerLifecycle();
            TestCustomStoryNodeAdvancement();
            TestCustomDialogueControllerTree();
            TestThingModelDatabase();
            TestInteractablePropManagerSpawning();
            TestInteractablePropStateTransitions();

            Logger.Info($"CUSTOM STORY & OBJECT TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
            if (_failed > 0)
            {
                foreach (var fail in _failures)
                {
                    Logger.Error($"  [FAIL] {fail}");
                }
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (condition)
            {
                _passed++;
            }
            else
            {
                _failed++;
                _failures.Add(message);
                Logger.Error($"  ASSERT FAILED: {message}");
            }
        }

        private static void TestCustomStoryDatabase()
        {
            var db = new CustomStoryDatabase();
            var arc = db.GetArc("arc_astral_seal");
            Assert(arc != null, "Found story arc 'arc_astral_seal' in CustomStoryDatabase");
            Assert(arc?.ChapterIds.Count == 3, "Astral Seal saga has 3 chapters/nodes");

            var node = db.GetNode("astral_node_01");
            Assert(node != null, "Found story node 'astral_node_01'");
            Assert(node?.AssociatedObjectId == "obj_astral_altar_01", "Node 01 associated object is Astral Altar");
        }

        private static void TestCustomStoryManagerLifecycle()
        {
            var mgr = new CustomStoryManager();
            mgr.Initialize();

            Assert(mgr.IsInitialized, "CustomStoryManager initialized successfully");
            CustomStoryManager? resolved = null;
            try { resolved = ServiceLocator.Get<CustomStoryManager>(); } catch { }
            Assert(resolved != null, "CustomStoryManager registered with ServiceLocator");

            bool started = mgr.StartStoryArc("arc_astral_seal");
            Assert(started, "Started story arc 'arc_astral_seal'");
            Assert(mgr.GetActiveNodeForArc("arc_astral_seal") == "astral_node_01", "Active node for arc is 'astral_node_01'");

            mgr.Shutdown();
            Assert(!mgr.IsInitialized, "CustomStoryManager shutdown successfully");
            
            resolved = null;
            try { resolved = ServiceLocator.Get<CustomStoryManager>(); } catch { }
            Assert(resolved == null, "CustomStoryManager unregistered from ServiceLocator");
        }

        private static void TestCustomStoryNodeAdvancement()
        {
            var mgr = new CustomStoryManager();
            mgr.Initialize();

            mgr.StartStoryArc("arc_astral_seal");
            mgr.AdvanceStoryNode("astral_node_01");
            Assert(mgr.IsStoryNodeCompleted("astral_node_01"), "Node 01 completed");
            Assert(mgr.GetActiveNodeForArc("arc_astral_seal") == "astral_node_02", "Arc advanced to node 02");

            mgr.AdvanceStoryNode("astral_node_02");
            mgr.AdvanceStoryNode("astral_node_03");

            Assert(mgr.IsStoryArcCompleted("arc_astral_seal"), "Arc 'arc_astral_seal' marked as completed after final node");

            mgr.Shutdown();
        }

        private static void TestCustomDialogueControllerTree()
        {
            // Register a sample conversation into DialogueDatabase
            var sampleConv = new ConversationDefinition
            {
                ConversationId = "conv_test_custom_intro",
                NpcId = "npc_keeper_orin",
                StartingDialogueId = "dlg_test_01",
                Dialogues = new List<DialogueEntry>
                {
                    new DialogueEntry
                    {
                        DialogueId = "dlg_test_01",
                        SpeakerId = "npc_keeper_orin",
                        SpeakerType = DialogueSpeakerType.Npc,
                        TextKey = "Welcome, traveler.",
                        Choices = new List<DialogueChoice>
                        {
                            new DialogueChoice
                            {
                                ChoiceId = "choice_01",
                                TextKey = "Tell me about the seal.",
                                NextDialogueId = "END",
                                SetFlag = "flag_learned_seal"
                            }
                        }
                    }
                }
            };
            DialogueDatabase.RegisterConversation(sampleConv);

            var controller = new CustomDialogueController();
            bool started = controller.StartConversation("conv_test_custom_intro");
            Assert(started, "Dialogue controller started 'conv_test_custom_intro'");
            Assert(controller.IsInConversation, "Controller is actively in conversation");
            Assert(controller.CurrentDialogue != null, "Current dialogue entry is not null");

            var choices = controller.GetAvailableChoices();
            Assert(choices.Count > 0, "Available choices count > 0 for intro conversation");

            var result = controller.SelectChoice(0);
            Assert(result.Success, "Selection of choice 0 succeeded");

            controller.EndConversation();
            Assert(!controller.IsInConversation, "Conversation ended cleanly");
        }

        private static void TestThingModelDatabase()
        {
            var db = new ThingModelDatabase();
            var altar = db.GetObjectDefinition("obj_astral_altar_01");

            Assert(altar != null, "Found 'obj_astral_altar_01' in ThingModelDatabase");
            Assert(altar?.Category == ObjectCategory.RelicAltar, "Altar category is RelicAltar");
            Assert(altar?.MeshSpec.BaseShape == MeshShapeType.Cylinder, "Altar base mesh shape is Cylinder");
            Assert(altar?.MeshSpec.Lod0PolyCount == 1800, "Altar LOD0 poly count is 1800");

            var chest = db.GetObjectDefinition("obj_ancient_chest_tier2");
            Assert(chest != null, "Found 'obj_ancient_chest_tier2' in ThingModelDatabase");
            Assert(chest?.Category == ObjectCategory.ChestContainer, "Chest category is ChestContainer");
        }

        private static void TestInteractablePropManagerSpawning()
        {
            var propMgr = new InteractablePropManager();
            propMgr.Initialize();

            var inst = propMgr.SpawnProp("obj_astral_altar_01", "inst_altar_01", new Vector3(10, 0, 10), Vector3.Zero);
            Assert(inst != null, "Spawned prop instance 'inst_altar_01'");
            Assert(inst?.CurrentState == ObjectState.Idle, "Initial prop state is Idle");
            Assert(propMgr.GetActiveInstances().Count == 1, "Active prop count is 1");

            propMgr.Shutdown();
        }

        private static void TestInteractablePropStateTransitions()
        {
            var propMgr = new InteractablePropManager();
            propMgr.Initialize();

            propMgr.SpawnProp("obj_arcane_switch_01", "inst_switch_01", Vector3.Zero, Vector3.Zero);
            bool interacted = propMgr.InteractWithProp("inst_switch_01");

            Assert(interacted, "Interacted with Arcane Switch");
            var inst = propMgr.GetInstance("inst_switch_01");
            Assert(inst?.CurrentState == ObjectState.Active, "Switch state toggled to Active upon interaction");

            propMgr.Shutdown();
        }
    }
}
