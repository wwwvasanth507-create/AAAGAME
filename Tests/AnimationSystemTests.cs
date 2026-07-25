using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Animation;
using HeroOfEternia.Core;

namespace HeroOfEternia.Tests
{
    public static class AnimationSystemTests
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
            Logger.Info("RUNNING ANIMATION SYSTEM TESTS (PROMPT 22)");
            Logger.Info("==================================================");

            TestAnimationStatePlayback();
            TestPriorityPreemption();
            TestLayerBlending();
            TestIKSystemToggles();
            TestProceduralLookAt();
            TestAnimationEventRouting();
            TestCharacterProfileMapping();
            TestRootMotionDeltas();
            TestSaveV17Integration();

            Logger.Info($"ANIMATION TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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

        private static void TestAnimationStatePlayback()
        {
            var anim = new AnimationManager();
            anim.Initialize();

            bool played = anim.PlayState(AnimationState.Walk);
            Assert(played, "Played Walk state successfully");
            Assert(anim.CurrentState == AnimationState.Walk, "CurrentState updated to Walk");

            anim.Shutdown();
        }

        private static void TestPriorityPreemption()
        {
            var anim = new AnimationManager();
            anim.Initialize();

            anim.PlayState(AnimationState.Walk, AnimationLayerType.FullBody, AnimationPriority.Normal);
            bool lowPrioResult = anim.PlayState(AnimationState.Idle, AnimationLayerType.FullBody, AnimationPriority.Low);
            Assert(!lowPrioResult, "Low priority state preempted by Normal priority state");

            bool highPrioResult = anim.PlayState(AnimationState.Attack, AnimationLayerType.FullBody, AnimationPriority.High);
            Assert(highPrioResult, "High priority Attack state preempts Normal priority state");

            anim.Shutdown();
        }

        private static void TestLayerBlending()
        {
            var layer = new AnimationLayer(AnimationLayerType.UpperBody, 0.8f, true, "Spine1");
            Assert(layer.LayerType == AnimationLayerType.UpperBody, "LayerType matches UpperBody");
            Assert(layer.IsAdditive, "IsAdditive set to true");
            Assert(Math.Abs(layer.Weight - 0.8f) < 0.001f, "Layer Weight set to 0.8");
        }

        private static void TestIKSystemToggles()
        {
            var ik = new IKSystem();
            ik.SetAllIKEnabled(false);
            Assert(!ik.IsEnabled, "IKSystem globally disabled");
            Assert(!ik.LeftFoot.Enabled, "Left foot IK disabled");

            ik.SetAllIKEnabled(true);
            Assert(ik.IsEnabled, "IKSystem re-enabled");
        }

        private static void TestProceduralLookAt()
        {
            var proc = new ProceduralAnimationEngine();
            proc.SetLookAtTarget(new Vector3(10, 2, 5), 0.75f);

            Assert(proc.LookAtTarget == new Vector3(10, 2, 5), "LookAtTarget position updated");
            Assert(Math.Abs(proc.LookAtWeight - 0.75f) < 0.001f, "LookAtWeight set to 0.75");
        }

        private static void TestAnimationEventRouting()
        {
            var events = new AnimationEventSystem();
            bool eventDispatched = false;

            events.OnAnimationEvent += (e) =>
            {
                eventDispatched = true;
                Assert(e.EventType == AnimationEventType.Footstep, "EventType is Footstep");
            };

            events.DispatchEvent(new AnimationEventData { EventType = AnimationEventType.Footstep, EventName = "step_left" });
            Assert(eventDispatched, "Animation event dispatched to listener");
        }

        private static void TestCharacterProfileMapping()
        {
            var playerProf = CharacterAnimationProfile.CreateDefaultPlayerProfile();
            var bossProf = CharacterAnimationProfile.CreateDefaultBossProfile();

            Assert(playerProf.Archetype == CharacterArchetype.Player, "Player profile archetype matches");
            Assert(bossProf.Archetype == CharacterArchetype.Boss, "Boss profile archetype matches");
            Assert(bossProf.GetConfig(AnimationState.Attack)?.ClipName == "boss_titan_smash", "Boss attack clip mapped");
        }

        private static void TestRootMotionDeltas()
        {
            var root = new RootMotionController { RootMotionEnabled = true };
            bool extracted = false;

            root.OnRootMotionExtracted += (pos, rot) =>
            {
                extracted = true;
                Assert(pos.Z == 2.0f, "Delta position Z matches 2.0");
            };

            root.ProcessRootMotion(new Vector3(0, 0, 2.0f), Quaternion.Identity);
            Assert(extracted, "Root motion event fired with delta position");
        }

        private static void TestSaveV17Integration()
        {
            var profile = new SaveProfile
            {
                AnimationData = new AnimationSaveData
                {
                    FootIKEnabled = true,
                    GlobalIKWeight = 0.9f
                }
            };

            Assert(profile.AnimationData != null, "SaveProfile contains AnimationData");
            Assert(profile.AnimationData.SaveVersion == 17, "SaveVersion is 17");
        }
    }
}
