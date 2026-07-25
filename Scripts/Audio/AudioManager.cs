using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Audio
{
    /// <summary>
    /// Central audio engine manager implementing <see cref="IInitializable"/>.
    /// Handles channel pooling, dynamic bus routing, master/category volumes,
    /// dynamic range profiles, adaptive music, spatial audio, and save data integration.
    /// </summary>
    public partial class AudioManager : Node, IInitializable
    {
        private bool _initialized = false;

        public AudioSettings Settings { get; private set; } = new();
        public MusicManager MusicManager { get; private set; }
        public AmbientAudioManager AmbientManager { get; private set; }
        public VoiceFramework VoiceFramework { get; private set; }
        public SoundEventSystem EventSystem { get; private set; } = new();

        private readonly List<PositionalAudioPlayer> _3dPlayerPool = new();
        private readonly Queue<AudioStreamPlayer> _2dPlayerPool = new();
        private readonly Dictionary<string, AudioStream> _soundCache = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized => _initialized;

        public override void _Ready()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("AudioManager: Initializing audio framework...");

            // Create sub-systems
            MusicManager = new MusicManager { Name = "MusicManager" };
            AddChild(MusicManager);

            AmbientManager = new AmbientAudioManager { Name = "AmbientManager" };
            AddChild(AmbientManager);

            VoiceFramework = new VoiceFramework { Name = "VoiceFramework" };
            AddChild(VoiceFramework);

            // Warm 2D player pool (32 concurrent channels)
            for (int i = 0; i < 32; i++)
            {
                var player = new AudioStreamPlayer { Name = $"Audio2D_Pool_{i}" };
                AddChild(player);
                _2dPlayerPool.Enqueue(player);
            }

            // Warm 3D positional pool (16 concurrent spatial sources)
            for (int i = 0; i < 16; i++)
            {
                var spatialPlayer = new PositionalAudioPlayer { Name = $"Positional_Pool_{i}" };
                AddChild(spatialPlayer);
                _3dPlayerPool.Add(spatialPlayer);
            }

            // Register EventBus listeners
            EventSystem.Initialize();

            // Register with ServiceLocator
            ServiceLocator.Register(this);

            Logger.Info("AudioManager: Audio framework initialized successfully.");
        }

        public void Shutdown()
        {
            if (!_initialized) return;

            Logger.Info("AudioManager: Shutting down audio engine...");
            EventSystem.Shutdown();

            MusicManager?.QueueFree();
            AmbientManager?.QueueFree();
            VoiceFramework?.QueueFree();

            _2dPlayerPool.Clear();
            _3dPlayerPool.Clear();
            _soundCache.Clear();

            ServiceLocator.Unregister<AudioManager>();
            _initialized = false;
        }

        public void PlaySound(string soundId, AudioCategory category = AudioCategory.Combat, Vector3 position = default, float volume = 1.0f, float pitch = 1.0f)
        {
            if (Settings.IsMuted) return;

            float catVol = Settings.GetCategoryVolume(category);
            float finalVol = Settings.MasterVolume * catVol * volume;
            if (finalVol <= 0.001f) return;

            if (!_soundCache.TryGetValue(soundId, out var stream))
            {
                string path = $"res://Assets/Audio/{soundId}.wav";
                if (ResourceLoader.Exists(path))
                {
                    stream = GD.Load<AudioStream>(path);
                    _soundCache[soundId] = stream;
                }
            }

            if (stream == null) return;

            if (position != default)
            {
                // Play via 3D spatial pool
                var player3D = GetAvailable3DPlayer();
                player3D?.PlaySound(stream, position, finalVol, pitch);
            }
            else
            {
                // Play via 2D pool
                if (_2dPlayerPool.Count > 0)
                {
                    var player2D = _2dPlayerPool.Dequeue();
                    player2D.Stream = stream;
                    player2D.VolumeDb = Mathf.LinearToDb(finalVol);
                    player2D.PitchScale = Math.Clamp(pitch, 0.5f, 2.0f);
                    player2D.Play();
                    _2dPlayerPool.Enqueue(player2D);
                }
            }
        }

        public void SetCategoryVolume(AudioCategory category, float volume)
        {
            Settings.SetCategoryVolume(category, volume);
            if (category == AudioCategory.Music && MusicManager != null)
            {
                MusicManager.MusicVolume = volume;
            }
            else if (category == AudioCategory.Ambient && AmbientManager != null)
            {
                AmbientManager.AmbientVolume = volume;
            }
        }

        public void MuteAll(bool mute)
        {
            Settings.IsMuted = mute;
        }

        private PositionalAudioPlayer GetAvailable3DPlayer()
        {
            foreach (var p in _3dPlayerPool)
            {
                if (!p.IsActive) return p;
            }
            return _3dPlayerPool.Count > 0 ? _3dPlayerPool[0] : null;
        }
    }
}
