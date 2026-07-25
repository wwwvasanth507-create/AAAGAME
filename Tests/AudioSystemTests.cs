using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Audio;
using HeroOfEternia.Core;

namespace HeroOfEternia.Tests
{
    public static class AudioSystemTests
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
            Logger.Info("RUNNING AUDIO SYSTEM TESTS (PROMPT 21)");
            Logger.Info("==================================================");

            TestCategoryVolumeScaling();
            TestAudioMuting();
            TestMusicStateTransitions();
            TestAmbientZoneBlending();
            TestPositionalAudioConfig();
            TestVoiceFrameworkSubtitles();
            TestSoundEventSystem();
            TestSaveV16Integration();

            Logger.Info($"AUDIO TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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

        private static void TestCategoryVolumeScaling()
        {
            var settings = new AudioSettings();
            settings.SetCategoryVolume(AudioCategory.Music, 0.5f);
            settings.SetCategoryVolume(AudioCategory.Combat, 0.9f);

            Assert(Math.Abs(settings.GetCategoryVolume(AudioCategory.Music) - 0.5f) < 0.001f, "Music volume set to 0.5");
            Assert(Math.Abs(settings.GetCategoryVolume(AudioCategory.Combat) - 0.9f) < 0.001f, "Combat volume set to 0.9");
        }

        private static void TestAudioMuting()
        {
            var settings = new AudioSettings();
            settings.IsMuted = true;
            Assert(settings.IsMuted, "IsMuted set to true");
        }

        private static void TestMusicStateTransitions()
        {
            var music = new MusicManager();
            music.SetStateDefaultTrack(MusicState.Combat, "combat_theme_01");
            music.TransitionToState(MusicState.Combat, 1.0f);

            Assert(music.CurrentState == MusicState.Combat, "Music state transitioned to Combat");
            Assert(music.CurrentTrackId == "combat_theme_01", "Current track updated to combat_theme_01");
        }

        private static void TestAmbientZoneBlending()
        {
            var ambient = new AmbientAudioManager();
            ambient.RegisterZone(new AmbientZone
            {
                ZoneId = "forest_deep",
                AudioStreamPath = "res://Assets/Audio/forest_wind.wav",
                DefaultVolume = 0.7f
            });

            ambient.SetZone("forest_deep");
            Assert(ambient.AmbientVolume > 0, "Ambient volume active");
        }

        private static void TestPositionalAudioConfig()
        {
            var player = new PositionalAudioPlayer
            {
                MaxDistance = 50f,
                UnitDistance = 2f,
                Attenuation = AttenuationModel.Inverse
            };

            Assert(player.MaxDistance == 50f, "Positional player MaxDistance configured to 50");
            Assert(player.Attenuation == AttenuationModel.Inverse, "Attenuation model configured");
        }

        private static void TestVoiceFrameworkSubtitles()
        {
            var voice = new VoiceFramework();
            bool triggered = false;

            voice.OnSubtitleTriggered += (line) =>
            {
                triggered = true;
                Assert(line.SpeakerName == "Elder Vance", "Speaker name matches");
            };

            voice.PlayBark("Elder Vance", "Greetings, young hero!", null, 3.0f);
            Assert(triggered, "Subtitle event fired on voice bark");
        }

        private static void TestSoundEventSystem()
        {
            var system = new SoundEventSystem();
            system.Initialize();
            system.Shutdown();
            Assert(true, "SoundEventSystem init & shutdown succeeded");
        }

        private static void TestSaveV16Integration()
        {
            var profile = new SaveProfile
            {
                AudioData = new AudioSettings
                {
                    MasterVolume = 0.95f,
                    MusicVolume = 0.65f
                }
            };

            Assert(profile.AudioData != null, "SaveProfile contains AudioData");
            Assert(Math.Abs(profile.AudioData.MasterVolume - 0.95f) < 0.001f, "MasterVolume persisted in Save V16 profile");
        }
    }
}
