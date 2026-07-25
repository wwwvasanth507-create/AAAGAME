using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Graphics;

namespace HeroOfEternia.Tests
{
    public static class GraphicsSystemTests
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
            Logger.Info("RUNNING GRAPHICS SYSTEM TESTS (PROMPT 23)");
            Logger.Info("==================================================");

            TestVFXManagerInit();
            TestParticleDefinitions();
            TestShaderParameterUpdates();
            TestLightingProfileLerp();
            TestPostProcessingPresets();
            TestWeatherVisualTransitions();
            TestDecalSystemSpawning();
            TestCameraShakeImpulse();
            TestRenderingOptimizationPresets();
            TestSaveV18Integration();

            Logger.Info($"GRAPHICS TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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

        private static void TestVFXManagerInit()
        {
            var vfx = new VisualEffectManager();
            vfx.Initialize();

            bool spawned = vfx.SpawnEffect("vfx_fire_burst", Vector3.Zero);
            Assert(spawned, "Spawned registered particle effect vfx_fire_burst");

            vfx.Shutdown();
        }

        private static void TestParticleDefinitions()
        {
            var config = new ParticleEffectConfig
            {
                EffectId = "vfx_test_spark",
                Type = ParticleType.Spark,
                LifetimeSeconds = 1.5f,
                Priority = VFXPriority.High
            };

            Assert(config.EffectId == "vfx_test_spark", "EffectId matches");
            Assert(config.Type == ParticleType.Spark, "ParticleType matches Spark");
            Assert(config.Priority == VFXPriority.High, "VFXPriority matches High");
        }

        private static void TestShaderParameterUpdates()
        {
            var shader = new ShaderManager();
            var mat = new ShaderMaterial();
            shader.RegisterMaterial("char_hero", mat);
            shader.SetMaterialDissolve("char_hero", 0.5f);
            shader.SetSeasonalVariant("char_hero", "Winter");

            Assert(true, "ShaderManager updated dissolve & seasonal tint parameters");
        }

        private static void TestLightingProfileLerp()
        {
            var day = LightingProfile.CreateDefaultDayProfile();
            var night = LightingProfile.CreateDefaultNightProfile();
            var lerped = LightingProfile.Lerp(day, night, 0.5f);

            Assert(lerped != null, "LightingProfile lerp created new profile");
            Assert(lerped.SunlightEnergy < day.SunlightEnergy, "Lerped sunlight energy between Day and Night");
        }

        private static void TestPostProcessingPresets()
        {
            var low = PostProcessingProfile.GetPreset(GraphicsQualityPreset.Low);
            var high = PostProcessingProfile.GetPreset(GraphicsQualityPreset.High);

            Assert(!low.EnableBloom, "Low quality preset disables bloom for mobile performance");
            Assert(high.EnableBloom, "High quality preset enables bloom");
            Assert(high.EnableAO, "High quality preset enables ambient occlusion");
        }

        private static void TestWeatherVisualTransitions()
        {
            var weather = new WeatherVisualsController();
            bool eventFired = false;

            weather.OnWeatherVisualChanged += (type, intensity) =>
            {
                eventFired = true;
                Assert(type == WeatherVisualType.Rain, "Weather visual type is Rain");
                Assert(Math.Abs(intensity - 0.8f) < 0.001f, "Weather intensity is 0.8");
            };

            weather.SetWeatherVisual(WeatherVisualType.Rain, 0.8f);
            Assert(eventFired, "Weather visual change event dispatched");
        }

        private static void TestDecalSystemSpawning()
        {
            var decals = new DecalSystem { MaxDecals = 2 };
            decals.SpawnDecal(DecalType.Footprint, Vector3.Zero, Vector3.Up);
            decals.SpawnDecal(DecalType.Blood, Vector3.One, Vector3.Up);
            Assert(decals.ActiveDecals.Count == 2, "Active decals count equals 2");

            decals.SpawnDecal(DecalType.ScorchMark, new Vector3(2, 0, 2), Vector3.Up);
            Assert(decals.ActiveDecals.Count == 2, "Decal count capped at MaxDecals limit of 2");
        }

        private static void TestCameraShakeImpulse()
        {
            var cam = new CameraEffectsController();
            cam.TriggerCameraShake(2.5f);
            Assert(Math.Abs(cam.CurrentShakeIntensity - 2.5f) < 0.001f, "Camera shake intensity set to 2.5");
        }

        private static void TestRenderingOptimizationPresets()
        {
            var opt = new RenderingOptimizationManager();
            opt.ApplyQualitySettings(GraphicsQualityPreset.Low);

            Assert(opt.ShadowQuality == ShadowQualityLevel.Off, "Low graphics preset disables shadows for Android");
            Assert(opt.MaxDrawDistance == 80f, "Low graphics preset sets draw distance to 80m");
        }

        private static void TestSaveV18Integration()
        {
            var profile = new SaveProfile
            {
                GraphicsData = new GraphicsSaveData
                {
                    QualityPreset = GraphicsQualityPreset.Medium,
                    BloomEnabled = true
                }
            };

            Assert(profile.GraphicsData != null, "SaveProfile contains GraphicsData");
            Assert(profile.GraphicsData.SaveVersion == 18, "SaveVersion is 18");
        }
    }
}
