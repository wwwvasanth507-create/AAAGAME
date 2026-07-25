using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Quest;
using HeroOfEternia.Dialogue;

namespace HeroOfEternia.Tests
{
    public static class QuestSystemTests
    {
        private static int _passed = 0;
        private static int _failed = 0;
        private static readonly List<string> _failures = new();

        // ==========================================================
        // TEST RUNNER
        // ==========================================================

        public static int RunAll()
        {
            _passed = 0;
            _failed = 0;
            _failures.Clear();

            Godot.GD.Print("========================================");
            Godot.GD.Print("  QUEST & DIALOGUE SYSTEM TESTS");
            Godot.GD.Print("========================================");

            // Quest Database Tests
            Run("QuestDB_EmptyInit", TestQuestDatabaseEmptyInit);
            Run("QuestDB_RegisterSingle", TestQuestDatabaseRegisterSingle);
            Run("QuestDB_RegisterMultiple", TestQuestDatabaseRegisterMultiple);
            Run("QuestDB_GetByCategory", TestQuestDatabaseGetByCategory);
            Run("QuestDB_GetByGiver", TestQuestDatabaseGetByGiver);
            Run("QuestDB_Search", TestQuestDatabaseSearch);
            Run("QuestDB_Clear", TestQuestDatabaseClear);
            Run("QuestDB_StressLookup", TestQuestDatabaseStressLookup);

            // Quest Manager Tests
            Run("QuestMgr_AcceptQuest", TestQuestManagerAcceptQuest);
            Run("QuestMgr_CompleteQuest", TestQuestManagerCompleteQuest);
            Run("QuestMgr_FailQuest", TestQuestManagerFailQuest);
            Run("QuestMgr_AbandonQuest", TestQuestManagerAbandonQuest);
            Run("QuestMgr_RetryQuest", TestQuestManagerRetryQuest);
            Run("QuestMgr_ActiveQuests", TestQuestManagerActiveQuests);
            Run("QuestMgr_HistoryTracking", TestQuestManagerHistoryTracking);
            Run("QuestMgr_SaveLoad", TestQuestManagerSaveLoad);

            // Objective Manager Tests
            Run("ObjMgr_InitObjectives", TestObjectiveManagerInit);
            Run("ObjMgr_AdvanceObjective", TestObjectiveManagerAdvance);
            Run("ObjMgr_CompleteObjective", TestObjectiveManagerComplete);
            Run("ObjMgr_FailObjective", TestObjectiveManagerFail);
            Run("ObjMgr_Branching", TestObjectiveManagerBranching);
            Run("ObjMgr_OptionalObjectives", TestObjectiveManagerOptional);
            Run("ObjMgr_PrerequisiteChain", TestObjectiveManagerPrerequisiteChain);

            // Narrative Manager Tests
            Run("NarrMgr_GlobalFlags", TestNarrativeManagerGlobalFlags);
            Run("NarrMgr_RegionalFlags", TestNarrativeManagerRegionalFlags);
            Run("NarrMgr_WorldVariables", TestNarrativeManagerWorldVariables);
            Run("NarrMgr_NpcVariables", TestNarrativeManagerNpcVariables);
            Run("NarrMgr_PlayerDecisions", TestNarrativeManagerPlayerDecisions);
            Run("NarrMgr_ConditionEval", TestNarrativeManagerConditionEval);
            Run("NarrMgr_StoryChapters", TestNarrativeManagerStoryChapters);
            Run("NarrMgr_SaveLoad", TestNarrativeManagerSaveLoad);

            // Journal Manager Tests
            Run("JourMgr_AddQuest", TestJournalManagerAddQuest);
            Run("JourMgr_CompleteQuest", TestJournalManagerCompleteQuest);
            Run("JourMgr_LoreEntries", TestJournalManagerLoreEntries);
            Run("JourMgr_DialogueLog", TestJournalManagerDialogueLog);
            Run("JourMgr_Discoveries", TestJournalManagerDiscoveries);
            Run("JourMgr_SaveLoad", TestJournalManagerSaveLoad);

            // Dialogue Database Tests
            Run("DlgDB_Register", TestDialogueDatabaseRegister);
            Run("DlgDB_GetByNpc", TestDialogueDatabaseGetByNpc);
            Run("DlgDB_StartingDialogue", TestDialogueDatabaseStartingDialogue);
            Run("DlgDB_StressTest", TestDialogueDatabaseStress);

            // Dialogue Manager Tests
            Run("DlgMgr_StartConversation", TestDialogueManagerStartConversation);
            Run("DlgMgr_ChoiceSelection", TestDialogueManagerChoiceSelection);
            Run("DlgMgr_ConditionalChoices", TestDialogueManagerConditionalChoices);
            Run("DlgMgr_EndConversation", TestDialogueManagerEndConversation);
            Run("DlgMgr_LoopPrevention", TestDialogueManagerLoopPrevention);

            // Stress Tests
            Run("Stress_ThousandQuests", TestStressThousandQuests);
            Run("Stress_ThousandDialogues", TestStressThousandDialogues);
            Run("Stress_ConcurrentOperations", TestStressConcurrentOperations);
            Run("Stress_Memory", TestStressMemory);

            // Edge Cases
            Run("Edge_EmptyQuestDb", TestEdgeEmptyQuestDb);
            Run("Edge_DuplicateRegistration", TestEdgeDuplicateRegistration);
            Run("Edge_InvalidQuestAccept", TestEdgeInvalidQuestAccept);
            Run("Edge_MaxDepthDialogue", TestEdgeMaxDepthDialogue);
            Run("Edge_SerializationIntegrity", TestEdgeSerializationIntegrity);

            Godot.GD.Print("========================================");
            Godot.GD.Print($"  RESULTS: {_passed} passed, {_failed} failed");
            if (_failures.Count > 0)
            {
                Godot.GD.PrintErr("  FAILURES:");
                foreach (var f in _failures)
                    Godot.GD.PrintErr($"    - {f}");
            }
            Godot.GD.Print("========================================");

            return _failed == 0 ? 0 : 1;
        }

        private static void Run(string name, Func<bool> test)
        {
            try
            {
                if (test())
                {
                    _passed++;
                    Godot.GD.Print($"  ✓ {name}");
                }
                else
                {
                    _failed++;
                    _failures.Add(name);
                    Godot.GD.PrintErr($"  ✗ {name}");
                }
            }
            catch (Exception ex)
            {
                _failed++;
                _failures.Add($"{name}: {ex.Message}");
                Godot.GD.PrintErr($"  ✗ {name}: {ex.Message}");
            }
        }

        // ==========================================================
        // QUEST DATABASE TESTS
        // ==========================================================

        private static bool TestQuestDatabaseEmptyInit()
        {
            QuestDatabase.Clear();
            return QuestDatabase.QuestCount == 0;
        }

        private static bool TestQuestDatabaseRegisterSingle()
        {
            QuestDatabase.Clear();
            var quest = new QuestDefinition
            {
                QuestId = "test_quest_1",
                InternalName = "Test Quest 1",
                DisplayName = "Test Quest One",
                Category = QuestCategory.Side,
                RecommendedLevel = 5
            };
            QuestDatabase.RegisterQuest(quest);
            var retrieved = QuestDatabase.GetQuest("test_quest_1");
            return retrieved != null && retrieved.InternalName == "Test Quest 1" && QuestDatabase.QuestCount == 1;
        }

        private static bool TestQuestDatabaseRegisterMultiple()
        {
            QuestDatabase.Clear();
            var quests = new List<QuestDefinition>
            {
                new() { QuestId = "q1", InternalName = "Q1", Category = QuestCategory.Main },
                new() { QuestId = "q2", InternalName = "Q2", Category = QuestCategory.Side },
                new() { QuestId = "q3", InternalName = "Q3", Category = QuestCategory.Daily },
                new() { QuestId = "q4", InternalName = "Q4", Category = QuestCategory.Main },
                new() { QuestId = "q5", InternalName = "Q5", Category = QuestCategory.Faction }
            };
            QuestDatabase.RegisterQuests(quests);
            return QuestDatabase.QuestCount == 5;
        }

        private static bool TestQuestDatabaseGetByCategory()
        {
            QuestDatabase.Clear();
            var quests = new List<QuestDefinition>
            {
                new() { QuestId = "q1", InternalName = "Q1", Category = QuestCategory.Main },
                new() { QuestId = "q2", InternalName = "Q2", Category = QuestCategory.Side },
                new() { QuestId = "q3", InternalName = "Q3", Category = QuestCategory.Main },
                new() { QuestId = "q4", InternalName = "Q4", Category = QuestCategory.Main },
                new() { QuestId = "q5", InternalName = "Q5", Category = QuestCategory.Side }
            };
            QuestDatabase.RegisterQuests(quests);
            var mains = QuestDatabase.GetQuestsByCategory(QuestCategory.Main);
            var sides = QuestDatabase.GetQuestsByCategory(QuestCategory.Side);
            return mains.Count == 3 && sides.Count == 2;
        }

        private static bool TestQuestDatabaseGetByGiver()
        {
            QuestDatabase.Clear();
            var quests = new List<QuestDefinition>
            {
                new() { QuestId = "q1", QuestGiverId = "npc_1" },
                new() { QuestId = "q2", QuestGiverId = "npc_1" },
                new() { QuestId = "q3", QuestGiverId = "npc_2" }
            };
            QuestDatabase.RegisterQuests(quests);
            var npc1Quests = QuestDatabase.GetQuestsByGiver("npc_1");
            return npc1Quests.Count == 2;
        }

        private static bool TestQuestDatabaseSearch()
        {
            QuestDatabase.Clear();
            var quests = new List<QuestDefinition>
            {
                new() { QuestId = "quest_herbs", InternalName = "Herb Gathering", DisplayName = "Collect Herbs" },
                new() { QuestId = "quest_monster", InternalName = "Monster Hunt", DisplayName = "Hunt Monsters" },
                new() { QuestId = "quest_potion", InternalName = "Potion Making", DisplayName = "Brew Potion" }
            };
            QuestDatabase.RegisterQuests(quests);
            var huntResults = QuestDatabase.SearchQuests("monster");
            var herbResults = QuestDatabase.SearchQuests("herb");
            return huntResults.Count == 1 && herbResults.Count == 2;
        }

        private static bool TestQuestDatabaseClear()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "q1" });
            QuestDatabase.Clear();
            return QuestDatabase.QuestCount == 0;
        }

        private static bool TestQuestDatabaseStressLookup()
        {
            QuestDatabase.Clear();
            var quests = new List<QuestDefinition>();
            for (int i = 0; i < 1000; i++)
            {
                quests.Add(new QuestDefinition
                {
                    QuestId = $"stress_q_{i}",
                    InternalName = $"Stress Quest {i}",
                    Category = i % 3 == 0 ? QuestCategory.Main : i % 3 == 1 ? QuestCategory.Side : QuestCategory.Daily
                });
            }
            QuestDatabase.RegisterQuests(quests);

            // Test O(1) lookups
            var q = QuestDatabase.GetQuest("stress_q_500");
            if (q == null || q.InternalName != "Stress Quest 500") return false;

            var mains = QuestDatabase.GetQuestsByCategory(QuestCategory.Main);
            return mains.Count > 0 && QuestDatabase.QuestCount == 1000;
        }

        // ==========================================================
        // QUEST MANAGER TESTS
        // ==========================================================

        private static bool TestQuestManagerAcceptQuest()
        {
            QuestDatabase.Clear();
            var quest = new QuestDefinition
            {
                QuestId = "accept_test",
                InternalName = "Accept Test",
                IsEnabled = true,
                Branches = { new QuestBranch { BranchId = "b1", Objectives = { new ObjectiveDefinition { ObjectiveId = "obj1", Type = ObjectiveType.TalkToNpc } } } }
            };
            QuestDatabase.RegisterQuest(quest);

            var manager = new QuestManager();
            var instance = manager.AcceptQuest("accept_test");

            return instance != null && instance.State == QuestState.Active && manager.ActiveQuestCount == 1;
        }

        private static bool TestQuestManagerCompleteQuest()
        {
            QuestDatabase.Clear();
            var quest = new QuestDefinition
            {
                QuestId = "complete_test",
                InternalName = "Complete Test",
                IsEnabled = true,
                Branches = { new QuestBranch { BranchId = "b1", Objectives = { new ObjectiveDefinition { ObjectiveId = "obj1", Type = ObjectiveType.TalkToNpc, RequiredCount = 1 } } } },
                CompletionRewards = { new QuestReward { Type = RewardType.Experience, FloatValue = 100 } }
            };
            QuestDatabase.RegisterQuest(quest);

            var manager = new QuestManager();
            var instance = manager.AcceptQuest("complete_test");
            if (instance == null) return false;

            manager.CompleteQuest(instance.InstanceId);
            return manager.IsQuestCompleted("complete_test") && manager.ActiveQuestCount == 0;
        }

        private static bool TestQuestManagerFailQuest()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "fail_test", IsEnabled = true, Branches = { new QuestBranch { BranchId = "b1" } } });

            var manager = new QuestManager();
            var instance = manager.AcceptQuest("fail_test");
            if (instance == null) return false;

            manager.FailQuest(instance.InstanceId);
            return manager.IsQuestFailed("fail_test");
        }

        private static bool TestQuestManagerAbandonQuest()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "abandon_test", IsEnabled = true, Branches = { new QuestBranch { BranchId = "b1" } } });

            var manager = new QuestManager();
            var instance = manager.AcceptQuest("abandon_test");
            if (instance == null) return false;

            manager.AbandonQuest(instance.InstanceId);
            return manager.GetAbandonedQuestIds().Contains("abandon_test");
        }

        private static bool TestQuestManagerRetryQuest()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "retry_test", IsEnabled = true, Repeatable = true, MaxRepeatCount = 3, Branches = { new QuestBranch { BranchId = "b1" } } });

            var manager = new QuestManager();
            var instance1 = manager.AcceptQuest("retry_test");
            if (instance1 == null) return false;

            manager.FailQuest(instance1.InstanceId);
            var instance2 = manager.RetryQuest("retry_test");

            return instance2 != null && instance2.State == QuestState.RetryReady;
        }

        private static bool TestQuestManagerActiveQuests()
        {
            QuestDatabase.Clear();
            for (int i = 0; i < 5; i++)
            {
                QuestDatabase.RegisterQuest(new QuestDefinition
                {
                    QuestId = $"active_{i}",
                    IsEnabled = true,
                    Branches = { new QuestBranch { BranchId = "b1" } }
                });
            }

            var manager = new QuestManager();
            for (int i = 0; i < 5; i++)
                manager.AcceptQuest($"active_{i}");

            return manager.ActiveQuestCount == 5 && manager.GetActiveQuests().Count == 5;
        }

        private static bool TestQuestManagerHistoryTracking()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "hist_test", IsEnabled = true, DisplayName = "History Test", Branches = { new QuestBranch { BranchId = "b1" } } });

            var manager = new QuestManager();
            var instance = manager.AcceptQuest("hist_test");
            if (instance == null) return false;

            manager.CompleteQuest(instance.InstanceId);
            var history = manager.GetQuestHistory();
            return history.Count == 1 && history[0].QuestId == "hist_test";
        }

        private static bool TestQuestManagerSaveLoad()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "save_test", IsEnabled = true, Branches = { new QuestBranch { BranchId = "b1" } } });

            var manager = new QuestManager();
            var instance = manager.AcceptQuest("save_test");
            if (instance == null) return false;

            // Save
            var saveData = manager.GetSaveData();

            // Create new manager and load
            var manager2 = new QuestManager();
            manager2.LoadSaveData(saveData);

            return manager2.ActiveQuestCount == 1;
        }

        // ==========================================================
        // OBJECTIVE MANAGER TESTS
        // ==========================================================

        private static bool TestObjectiveManagerInit()
        {
            var def = new QuestDefinition
            {
                QuestId = "obj_init",
                Branches = { new QuestBranch { BranchId = "b1", Objectives = { new ObjectiveDefinition { ObjectiveId = "obj1", Type = ObjectiveType.TalkToNpc } } } }
            };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(def);

            var instance = new QuestInstance { QuestId = "obj_init", ActiveBranchId = "b1" };
            var objMgr = new ObjectiveManager();
            objMgr.InitializeObjectives(instance);

            return instance.ObjectiveStates.Count == 1 && instance.ObjectiveStates[0].State == ObjectiveState.Active;
        }

        private static bool TestObjectiveManagerAdvance()
        {
            var def = new QuestDefinition
            {
                QuestId = "obj_advance",
                Branches = { new QuestBranch { BranchId = "b1", Objectives = { new ObjectiveDefinition { ObjectiveId = "obj1", Type = ObjectiveType.DefeatEnemy, RequiredCount = 5 } } } }
            };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(def);

            var instance = new QuestInstance { QuestId = "obj_advance", ActiveBranchId = "b1" };
            var objMgr = new ObjectiveManager();
            objMgr.InitializeObjectives(instance);

            // Advance 3 times
            objMgr.AdvanceObjective(instance, "obj1");
            objMgr.AdvanceObjective(instance, "obj1");
            objMgr.AdvanceObjective(instance, "obj1");

            var state = instance.ObjectiveStates[0];
            return state.CurrentCount == 3 && !state.IsCompleted;
        }

        private static bool TestObjectiveManagerComplete()
        {
            var def = new QuestDefinition
            {
                QuestId = "obj_complete",
                Branches = { new QuestBranch { BranchId = "b1", Objectives = { new ObjectiveDefinition { ObjectiveId = "obj1", Type = ObjectiveType.DefeatEnemy, RequiredCount = 3 } } } }
            };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(def);

            var instance = new QuestInstance { QuestId = "obj_complete", ActiveBranchId = "b1" };
            var objMgr = new ObjectiveManager();
            objMgr.InitializeObjectives(instance);

            objMgr.AdvanceObjective(instance, "obj1");
            objMgr.AdvanceObjective(instance, "obj1");
            objMgr.AdvanceObjective(instance, "obj1");

            return instance.ObjectiveStates[0].IsCompleted;
        }

        private static bool TestObjectiveManagerFail()
        {
            var def = new QuestDefinition
            {
                QuestId = "obj_fail",
                Branches = { new QuestBranch { BranchId = "b1", Objectives = { new ObjectiveDefinition { ObjectiveId = "obj1", Type = ObjectiveType.Custom } } } }
            };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(def);

            var instance = new QuestInstance { QuestId = "obj_fail", ActiveBranchId = "b1" };
            var objMgr = new ObjectiveManager();
            objMgr.InitializeObjectives(instance);

            objMgr.FailObjective(instance, "obj1");
            return instance.ObjectiveStates[0].IsFailed;
        }

        private static bool TestObjectiveManagerBranching()
        {
            var def = new QuestDefinition
            {
                QuestId = "obj_branch",
                Branches =
                {
                    new QuestBranch
                    {
                        BranchId = "b1",
                        Objectives =
                        {
                            new ObjectiveDefinition { ObjectiveId = "obj1", Type = ObjectiveType.TalkToNpc, RequiredCount = 1, OnCompleteBranchId = "b2" }
                        }
                    },
                    new QuestBranch
                    {
                        BranchId = "b2",
                        Objectives =
                        {
                            new ObjectiveDefinition { ObjectiveId = "obj2", Type = ObjectiveType.DefeatEnemy, RequiredCount = 3 }
                        }
                    }
                }
            };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(def);

            var instance = new QuestInstance { QuestId = "obj_branch", ActiveBranchId = "b1" };
            var objMgr = new ObjectiveManager();
            objMgr.InitializeObjectives(instance);

            // Complete b1 objective
            objMgr.CompleteObjective(instance, "obj1");

            return instance.ActiveBranchId == "b2" && instance.ObjectiveStates.Count == 1 && instance.ObjectiveStates[0].ObjectiveId == "obj2";
        }

        private static bool TestObjectiveManagerOptional()
        {
            var def = new QuestDefinition
            {
                QuestId = "obj_optional",
                Branches = { new QuestBranch { BranchId = "b1", Objectives = {
                    new ObjectiveDefinition { ObjectiveId = "main", Type = ObjectiveType.TalkToNpc, RequiredCount = 1, IsOptional = false },
                    new ObjectiveDefinition { ObjectiveId = "optional", Type = ObjectiveType.CollectItem, RequiredCount = 5, IsOptional = true }
                } } }
            };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(def);

            var instance = new QuestInstance { QuestId = "obj_optional", ActiveBranchId = "b1" };
            var objMgr = new ObjectiveManager();
            objMgr.InitializeObjectives(instance);

            var optional = instance.ObjectiveStates.FirstOrDefault(os => os.ObjectiveId == "optional");
            return optional != null && optional.State == ObjectiveState.Optional;
        }

        private static bool TestObjectiveManagerPrerequisiteChain()
        {
            var def = new QuestDefinition
            {
                QuestId = "obj_chain",
                Branches = { new QuestBranch { BranchId = "b1", Objectives = {
                    new ObjectiveDefinition { ObjectiveId = "step1", Type = ObjectiveType.TalkToNpc, RequiredCount = 1 },
                    new ObjectiveDefinition { ObjectiveId = "step2", Type = ObjectiveType.CollectItem, RequiredCount = 3, PrerequisiteObjectiveIds = { "step1" } },
                    new ObjectiveDefinition { ObjectiveId = "step3", Type = ObjectiveType.DeliverItem, RequiredCount = 1, PrerequisiteObjectiveIds = { "step2" } }
                } } }
            };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(def);

            var instance = new QuestInstance { QuestId = "obj_chain", ActiveBranchId = "b1" };
            var objMgr = new ObjectiveManager();
            objMgr.InitializeObjectives(instance);

            // step1 should be active, step2 and step3 should be locked
            var step1 = instance.ObjectiveStates[0];
            var step2 = instance.ObjectiveStates[1];
            var step3 = instance.ObjectiveStates[2];

            return step1.State == ObjectiveState.Active && step2.State == ObjectiveState.Locked && step3.State == ObjectiveState.Locked;
        }

        // ==========================================================
        // NARRATIVE MANAGER TESTS
        // ==========================================================

        private static bool TestNarrativeManagerGlobalFlags()
        {
            var nm = new NarrativeManager();
            nm.SetGlobalFlag("has_met_king", "true");
            nm.SetGlobalFlag("world_saved", "yes");
            return nm.HasGlobalFlag("has_met_king") && nm.GetGlobalFlag("world_saved") == "yes" && !nm.HasGlobalFlag("nonexistent");
        }

        private static bool TestNarrativeManagerRegionalFlags()
        {
            var nm = new NarrativeManager();
            nm.SetRegionalFlag("region_forest", "cleared", "true");
            nm.SetRegionalFlag("region_desert", "explored", "yes");
            return nm.HasRegionalFlag("region_forest", "cleared") && nm.GetRegionalFlag("region_desert", "explored") == "yes";
        }

        private static bool TestNarrativeManagerWorldVariables()
        {
            var nm = new NarrativeManager();
            nm.SetWorldVariable("day", 5);
            nm.SetWorldVariable("season", "summer");
            return nm.GetWorldVariableFloat("day") == 5f && nm.GetWorldVariableString("season") == "summer";
        }

        private static bool TestNarrativeManagerNpcVariables()
        {
            var nm = new NarrativeManager();
            nm.SetNpcVariable("npc_blacksmith", "has_met", true);
            nm.SetNpcVariable("npc_blacksmith", "friendship", 50);
            return nm.GetNpcVariableFloat("npc_blacksmith", "friendship") == 50f;
        }

        private static bool TestNarrativeManagerPlayerDecisions()
        {
            var nm = new NarrativeManager();
            nm.RecordDecision("decision_help_village", "helped", "quest_01");
            return nm.DidPlayerChoose("decision_help_village", "helped") && !nm.DidPlayerChoose("decision_help_village", "refused");
        }

        private static bool TestNarrativeManagerConditionEval()
        {
            var nm = new NarrativeManager();
            nm.SetGlobalFlag("flag1", "true");
            nm.SetWorldVariable("level", 10);

            // Test conditions
            bool cond1 = nm.EvaluateCondition("flag:flag1");
            bool cond2 = nm.EvaluateCondition("!flag:flag1"); // negated
            bool cond3 = nm.EvaluateCondition("var:level=10");
            bool cond4 = nm.EvaluateCondition("var:level>5");

            return cond1 && !cond2 && cond3 && cond4;
        }

        private static bool TestNarrativeManagerStoryChapters()
        {
            var nm = new NarrativeManager();
            nm.UnlockStoryChapter("chapter_01_introduction");
            nm.UnlockStoryChapter("chapter_02_awakening");
            return nm.IsStoryChapterUnlocked("chapter_01_introduction") && nm.GetUnlockedStoryChapters().Count == 2;
        }

        private static bool TestNarrativeManagerSaveLoad()
        {
            var nm = new NarrativeManager();
            nm.SetGlobalFlag("flag_test", "saved");
            nm.RecordDecision("dec_test", "choice_a");

            var data = nm.GetSaveData();

            var nm2 = new NarrativeManager();
            nm2.LoadSaveData(data);

            return nm2.HasGlobalFlag("flag_test") && nm2.DidPlayerChoose("dec_test", "choice_a");
        }

        // ==========================================================
        // JOURNAL MANAGER TESTS
        // ==========================================================

        private static bool TestJournalManagerAddQuest()
        {
            var jm = new JournalManager();
            var instance = new QuestInstance { QuestId = "j_test" };
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "j_test", DisplayName = "Journal Test", Category = QuestCategory.Side });
            jm.AddQuestToJournal(instance);

            return jm.GetActiveJournalQuests().Count == 1;
        }

        private static bool TestJournalManagerCompleteQuest()
        {
            var jm = new JournalManager();
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "j_comp", DisplayName = "Complete Test", Category = QuestCategory.Side });

            var instance = new QuestInstance { QuestId = "j_comp", CompletedTime = DateTime.UtcNow };
            jm.AddQuestToJournal(instance);
            jm.CompleteQuestInJournal(instance);

            return jm.GetActiveJournalQuests().Count == 0 && jm.GetCompletedJournalQuests().Count == 1;
        }

        private static bool TestJournalManagerLoreEntries()
        {
            var jm = new JournalManager();
            jm.UnlockLoreEntry("lore_creation", "The Creation", "How the world began...", "world");
            jm.UnlockLoreEntry("lore_war", "The Great War", "Tale of the ancient war", "history");

            return jm.GetLoreEntries().Count == 2 && jm.GetLoreEntries("world").Count == 1;
        }

        private static bool TestJournalManagerDialogueLog()
        {
            var jm = new JournalManager();
            jm.LogDialogue("npc_elder", "Elder", "dialogue.elder.greeting", "Asked about quest");
            jm.LogDialogue("npc_blacksmith", "Blacksmith", "dialogue.smith.greeting");

            return jm.GetDialogueLog(100).Count == 2;
        }

        private static bool TestJournalManagerDiscoveries()
        {
            var jm = new JournalManager();
            jm.RecordDiscovery("loc_village", "Eternal Village", "discovery.village.desc", DiscoveryType.Settlement);
            jm.RecordDiscovery("loc_forest", "Whispering Woods", "discovery.forest.desc", DiscoveryType.Location);

            return jm.DiscoveryCount == 2;
        }

        private static bool TestJournalManagerSaveLoad()
        {
            var jm = new JournalManager();
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "j_save", DisplayName = "Save Test", Category = QuestCategory.Side });

            var instance = new QuestInstance { QuestId = "j_save" };
            jm.AddQuestToJournal(instance);
            jm.UnlockLoreEntry("lore_test", "Test", "Test body");

            var data = jm.GetSaveData();

            var jm2 = new JournalManager();
            jm2.LoadSaveData(data);

            return jm2.GetActiveJournalQuests().Count == 1 && jm2.GetLoreEntries().Count == 1;
        }

        // ==========================================================
        // DIALOGUE DATABASE TESTS
        // ==========================================================

        private static bool TestDialogueDatabaseRegister()
        {
            DialogueDatabase.Clear();
            var conv = new ConversationDefinition
            {
                ConversationId = "conv_test",
                NpcId = "npc_test",
                StartingDialogueId = "dlg_start",
                Dialogues =
                {
                    new DialogueEntry { DialogueId = "dlg_start", SpeakerId = "npc_test", TextKey = "test.greeting" },
                    new DialogueEntry { DialogueId = "dlg_followup", SpeakerId = "npc_test", TextKey = "test.followup" }
                }
            };
            DialogueDatabase.RegisterConversation(conv);

            return DialogueDatabase.ConversationCount == 1 && DialogueDatabase.DialogueCount == 2;
        }

        private static bool TestDialogueDatabaseGetByNpc()
        {
            DialogueDatabase.Clear();
            DialogueDatabase.RegisterConversation(new ConversationDefinition
            {
                ConversationId = "conv_npc1", NpcId = "npc_test1",
                Dialogues = { new DialogueEntry { DialogueId = "d1", SpeakerId = "npc_test1" } }
            });
            DialogueDatabase.RegisterConversation(new ConversationDefinition
            {
                ConversationId = "conv_npc2", NpcId = "npc_test2",
                Dialogues = { new DialogueEntry { DialogueId = "d2", SpeakerId = "npc_test2" } }
            });

            var npc1Convs = DialogueDatabase.GetConversationsForNpc("npc_test1");
            return npc1Convs.Count == 1;
        }

        private static bool TestDialogueDatabaseStartingDialogue()
        {
            DialogueDatabase.Clear();
            DialogueDatabase.RegisterConversation(new ConversationDefinition
            {
                ConversationId = "conv_start",
                StartingDialogueId = "dlg_start",
                Dialogues = { new DialogueEntry { DialogueId = "dlg_start", SpeakerId = "npc", TextKey = "start.text" } }
            });

            var start = DialogueDatabase.GetStartingDialogue("conv_start");
            return start != null && start.DialogueId == "dlg_start";
        }

        private static bool TestDialogueDatabaseStress()
        {
            DialogueDatabase.Clear();
            var convs = new List<ConversationDefinition>();
            for (int i = 0; i < 100; i++)
            {
                var conv = new ConversationDefinition
                {
                    ConversationId = $"stress_conv_{i}",
                    NpcId = $"npc_{i % 10}",
                    Dialogues = new List<DialogueEntry>()
                };
                for (int j = 0; j < 10; j++)
                {
                    conv.Dialogues.Add(new DialogueEntry { DialogueId = $"stress_dlg_{i}_{j}", SpeakerId = $"npc_{i % 10}", TextKey = $"stress.{i}.{j}" });
                }
                convs.Add(conv);
            }
            DialogueDatabase.RegisterConversations(convs);

            return DialogueDatabase.ConversationCount == 100 && DialogueDatabase.DialogueCount == 1000;
        }

        // ==========================================================
        // DIALOGUE MANAGER TESTS
        // ==========================================================

        private static bool TestDialogueManagerStartConversation()
        {
            DialogueDatabase.Clear();
            DialogueDatabase.RegisterConversation(new ConversationDefinition
            {
                ConversationId = "conv_start_test",
                NpcId = "npc_test",
                StartingDialogueId = "dlg_start",
                Dialogues = { new DialogueEntry { DialogueId = "dlg_start", SpeakerId = "npc_test", TextKey = "start.text", Choices = { new DialogueChoice { ChoiceId = "c1", TextKey = "choice.text" } } } }
            });

            var dm = new DialogueManager();
            var dlg = dm.StartConversation("conv_start_test");

            return dlg != null && dm.IsInConversation && dm.ActiveConversationId == "conv_start_test";
        }

        private static bool TestDialogueManagerChoiceSelection()
        {
            DialogueDatabase.Clear();
            var conv = new ConversationDefinition
            {
                ConversationId = "conv_choice_test",
                NpcId = "npc_test",
                StartingDialogueId = "dlg_choice",
                Dialogues =
                {
                    new DialogueEntry
                    {
                        DialogueId = "dlg_choice", SpeakerId = "npc_test", TextKey = "choice.dlg",
                        Choices =
                        {
                            new DialogueChoice { ChoiceId = "c_accept", TextKey = "accept.text", NextDialogueId = "dlg_accepted" },
                            new DialogueChoice { ChoiceId = "c_decline", TextKey = "decline.text", NextDialogueId = "dlg_declined" }
                        }
                    },
                    new DialogueEntry { DialogueId = "dlg_accepted", SpeakerId = "npc_test", TextKey = "accepted.text", IsEndOfConversation = true },
                    new DialogueEntry { DialogueId = "dlg_declined", SpeakerId = "npc_test", TextKey = "declined.text", IsEndOfConversation = true }
                }
            };
            DialogueDatabase.RegisterConversation(conv);

            var dm = new DialogueManager();
            dm.StartConversation("conv_choice_test");

            // Select accept choice
            var next = dm.SelectChoice("dlg_choice", "c_accept");

            return next != null && next.DialogueId == "dlg_accepted";
        }

        private static bool TestDialogueManagerConditionalChoices()
        {
            DialogueDatabase.Clear();
            var nm = new NarrativeManager();
            
            var conv = new ConversationDefinition
            {
                ConversationId = "conv_cond",
                NpcId = "npc_test",
                StartingDialogueId = "dlg_cond",
                Dialogues =
                {
                    new DialogueEntry
                    {
                        DialogueId = "dlg_cond", SpeakerId = "npc_test", TextKey = "cond.dlg",
                        Choices =
                        {
                            new DialogueChoice { ChoiceId = "c_special", TextKey = "special.text", NextDialogueId = "dlg_special",
                                Conditions = { new DialogueCondition { Type = "flag", Parameter = "is_special", ExpectedValue = "true" } } },
                            new DialogueChoice { ChoiceId = "c_normal", TextKey = "normal.text", NextDialogueId = "dlg_normal" }
                        }
                    },
                    new DialogueEntry { DialogueId = "dlg_special", SpeakerId = "npc_test", TextKey = "special.text", IsEndOfConversation = true },
                    new DialogueEntry { DialogueId = "dlg_normal", SpeakerId = "npc_test", TextKey = "normal.text", IsEndOfConversation = true }
                }
            };
            DialogueDatabase.RegisterConversation(conv);

            var dm = new DialogueManager();
            dm.SetNarrativeManager(nm);
            dm.StartConversation("conv_cond");

            // Both choices should be available
            var choices = dm.GetCurrentChoices();
            if (choices.Count != 2) return false;

            // Set the flag
            nm.SetGlobalFlag("is_special", "true");
            dm.StartConversation("conv_cond");

            // Now only special should be shown (normal has no condition, so both still visible)
            // Test: select special choice
            var next = dm.SelectChoice("dlg_cond", "c_special");
            return next != null && next.DialogueId == "dlg_special";
        }

        private static bool TestDialogueManagerEndConversation()
        {
            DialogueDatabase.Clear();
            DialogueDatabase.RegisterConversation(new ConversationDefinition
            {
                ConversationId = "conv_end",
                NpcId = "npc_test",
                StartingDialogueId = "dlg_end",
                Dialogues = { new DialogueEntry { DialogueId = "dlg_end", SpeakerId = "npc_test", TextKey = "end.text", IsEndOfConversation = true } }
            });

            var dm = new DialogueManager();
            dm.StartConversation("conv_end");

            // Should auto-end since IsEndOfConversation = true
            return !dm.IsInConversation;
        }

        private static bool TestDialogueManagerLoopPrevention()
        {
            DialogueDatabase.Clear();
            var conv = new ConversationDefinition
            {
                ConversationId = "conv_loop",
                NpcId = "npc_test",
                MaxDepth = 3,
                StartingDialogueId = "dlg_loop",
                Dialogues =
                {
                    new DialogueEntry { DialogueId = "dlg_loop", SpeakerId = "npc_test", TextKey = "loop.1", NextDialogueId = "dlg_loop" } // self-loop
                }
            };
            DialogueDatabase.RegisterConversation(conv);

            var dm = new DialogueManager();
            dm.StartConversation("conv_loop");

            // Since dlg_loop has no choices but has NextDialogueId pointing to itself,
            // AdvanceDialogue should be called and eventually detect the loop
            var result = dm.AdvanceDialogue("dlg_loop");
            return result == null; // should return null (loop prevented)
        }

        // ==========================================================
        // STRESS TESTS
        // ==========================================================

        private static bool TestStressThousandQuests()
        {
            QuestDatabase.Clear();
            var quests = new List<QuestDefinition>();
            for (int i = 0; i < 1000; i++)
            {
                quests.Add(new QuestDefinition
                {
                    QuestId = $"stress_q_{i}",
                    InternalName = $"Stress {i}",
                    Category = (QuestCategory)(i % 18),
                    IsEnabled = true,
                    Branches = { new QuestBranch { BranchId = "b1", Objectives = { new ObjectiveDefinition { ObjectiveId = $"obj_{i}", Type = ObjectiveType.TalkToNpc } } } }
                });
            }
            QuestDatabase.RegisterQuests(quests);

            var manager = new QuestManager();
            int accepted = 0;
            for (int i = 0; i < 1000; i++)
            {
                var inst = manager.AcceptQuest($"stress_q_{i}");
                if (inst != null) accepted++;
            }

            return accepted == 1000 && manager.ActiveQuestCount == 1000;
        }

        private static bool TestStressThousandDialogues()
        {
            DialogueDatabase.Clear();
            var convs = new List<ConversationDefinition>();
            for (int i = 0; i < 100; i++)
            {
                var conv = new ConversationDefinition
                {
                    ConversationId = $"stress_conv_{i}",
                    NpcId = $"npc_{i}",
                    Dialogues = new List<DialogueEntry>()
                };
                for (int j = 0; j < 10; j++)
                {
                    conv.Dialogues.Add(new DialogueEntry
                    {
                        DialogueId = $"stress_dlg_{i}_{j}",
                        SpeakerId = $"npc_{i}",
                        TextKey = $"stress.dlg.{i}.{j}"
                    });
                }
                convs.Add(conv);
            }
            DialogueDatabase.RegisterConversations(convs);

            var dm = new DialogueManager();
            int started = 0;
            for (int i = 0; i < 100; i++)
            {
                // Set starting dialogue ID for each
                var conv = DialogueDatabase.GetConversation($"stress_conv_{i}");
                if (conv != null)
                {
                    conv.StartingDialogueId = $"stress_dlg_{i}_0";
                }
                var dlg = dm.StartConversation($"stress_conv_{i}");
                if (dlg != null) started++;
                dm.EndConversation();
            }

            return started == 100;
        }

        private static bool TestStressConcurrentOperations()
        {
            // Test rapid operations
            QuestDatabase.Clear();
            var manager = new QuestManager();

            for (int i = 0; i < 50; i++)
            {
                QuestDatabase.RegisterQuest(new QuestDefinition
                {
                    QuestId = $"cq_{i}",
                    IsEnabled = true,
                    Branches = { new QuestBranch { BranchId = "b1" } }
                });
            }

            // Accept and complete in rapid succession
            for (int i = 0; i < 50; i++)
            {
                var inst = manager.AcceptQuest($"cq_{i}");
                if (inst != null)
                {
                    manager.CompleteQuest(inst.InstanceId);
                }
            }

            return manager.GetCompletedQuestIds().Count == 50;
        }

        private static bool TestStressMemory()
        {
            QuestDatabase.Clear();
            // Create many quests with branches and objectives
            var quests = new List<QuestDefinition>();
            for (int i = 0; i < 500; i++)
            {
                var q = new QuestDefinition
                {
                    QuestId = $"mem_q_{i}",
                    IsEnabled = true,
                    Branches = new List<QuestBranch>()
                };
                for (int b = 0; b < 3; b++)
                {
                    var branch = new QuestBranch { BranchId = $"mem_q_{i}_b{b}" };
                    for (int o = 0; o < 5; o++)
                    {
                        branch.Objectives.Add(new ObjectiveDefinition
                        {
                            ObjectiveId = $"mem_q_{i}_obj_{o}",
                            Type = (ObjectiveType)(o % 16)
                        });
                    }
                    q.Branches.Add(branch);
                }
                quests.Add(q);
            }
            QuestDatabase.RegisterQuests(quests);

            return QuestDatabase.QuestCount == 500;
        }

        // ==========================================================
        // EDGE CASE TESTS
        // ==========================================================

        private static bool TestEdgeEmptyQuestDb()
        {
            QuestDatabase.Clear();
            var q = QuestDatabase.GetQuest("nonexistent");
            return q == null && QuestDatabase.QuestCount == 0;
        }

        private static bool TestEdgeDuplicateRegistration()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "dup" });
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "dup", InternalName = "Override" });

            var q = QuestDatabase.GetQuest("dup");
            return q != null && q.InternalName == "Override"; // last registration wins
        }

        private static bool TestEdgeInvalidQuestAccept()
        {
            QuestDatabase.Clear();
            var manager = new QuestManager();

            // Accepting non-existent quest should return null
            var instance = manager.AcceptQuest("nonexistent");
            if (instance != null) return false;

            // Accept disabled quest should fail
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "disabled", IsEnabled = false, Branches = { new QuestBranch { BranchId = "b1" } } });
            instance = manager.AcceptQuest("disabled");
            if (instance != null) return false;

            return true;
        }

        private static bool TestEdgeMaxDepthDialogue()
        {
            DialogueDatabase.Clear();
            var conv = new ConversationDefinition
            {
                ConversationId = "conv_depth",
                MaxDepth = 1,
                StartingDialogueId = "dlg_1",
                Dialogues = new List<DialogueEntry>
                {
                    new() { DialogueId = "dlg_1", SpeakerId = "npc", TextKey = "depth.1", NextDialogueId = "dlg_2" },
                    new() { DialogueId = "dlg_2", SpeakerId = "npc", TextKey = "depth.2", IsEndOfConversation = false, Choices = { new DialogueChoice { ChoiceId = "c_end", TextKey = "end" } } }
                }
            };
            DialogueDatabase.RegisterConversation(conv);

            var dm = new DialogueManager();
            dm.SetMaxDepth(1);
            dm.StartConversation("conv_depth");

            // Advance - should hit depth limit and end
            var result = dm.AdvanceDialogue("dlg_1");
            return !dm.IsInConversation;
        }

        private static bool TestEdgeSerializationIntegrity()
        {
            QuestDatabase.Clear();
            QuestDatabase.RegisterQuest(new QuestDefinition { QuestId = "serial_test", IsEnabled = true, Branches = { new QuestBranch { BranchId = "b1" } } });

            var manager = new QuestManager();
            var instance = manager.AcceptQuest("serial_test");
            if (instance == null) return false;

            var saveData = manager.GetSaveData();

            // Verify save data integrity
            if (saveData.ActiveQuests.Count != 1) return false;
            if (saveData.ActiveQuests[0].QuestId != "serial_test") return false;
            if (saveData.ActiveQuests[0].State != QuestState.Active) return false;

            // Load into new manager
            var manager2 = new QuestManager();
            manager2.LoadSaveData(saveData);

            // Verify restore
            return manager2.ActiveQuestCount == 1;
        }
    }
}