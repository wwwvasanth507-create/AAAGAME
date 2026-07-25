using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Coordinates audio bus routing, pools audio players, and plays background music and SFX.
    /// </summary>
    public partial class AudioManager : Node, IInitializable
    {
        private AudioStreamPlayer _bgmPlayer = null!;
        private readonly List<AudioStreamPlayer> _sfxPlayersPool = new();
        private readonly List<AudioStreamPlayer3D> _sfxPlayers3DPool = new();
        private const int InitialPoolSize = 8;

        public void Initialize()
        {
            // Attach to the root scene tree dynamically so we can handle game-wide audio
            var tree = Engine.GetMainLoop() as SceneTree;
            if (tree != null)
            {
                tree.Root.CallDeferred(Node.MethodName.AddChild, this);
            }

            // Create global background music player
            _bgmPlayer = new AudioStreamPlayer { Name = "BgmPlayer" };
            AddChild(_bgmPlayer);

            // Populate initial 2D and 3D audio pools
            for (int i = 0; i < InitialPoolSize; i++)
            {
                CreateNewSfxPlayer();
                CreateNewSfxPlayer3D();
            }

            // Load volume presets from SettingsManager
            SyncVolumesWithSettings();
        }

        private void SyncVolumesWithSettings()
        {
            try
            {
                var settings = ServiceLocator.Get<SettingsManager>();
                SetBusVolume("Master", settings.MasterVolume);
                SetBusVolume("Music", settings.MusicVolume);
                SetBusVolume("SFX", settings.SfxVolume);
                Logger.Info("AudioManager: Volumes synchronized with SettingsManager.");
            }
            catch (Exception)
            {
                Logger.Warning("AudioManager: SettingsManager not registered. Defaulting to standard volumes.");
                SetBusVolume("Master", 0.8f);
                SetBusVolume("Music", 0.7f);
                SetBusVolume("SFX", 0.9f);
            }
        }

        /// <summary>
        /// Sets volume for a specific audio bus (e.g. Master, Music, SFX).
        /// </summary>
        public void SetBusVolume(string busName, float volume)
        {
            int busIdx = AudioServer.GetBusIndex(busName);
            if (busIdx != -1)
            {
                float db = Mathf.LinearToDb(volume);
                AudioServer.SetBusVolumeDb(busIdx, db);
                Logger.Info($"AudioManager: Bus '{busName}' volume set to {volume:P} ({db:F1} dB)");
            }
            else
            {
                Logger.Warning($"AudioManager: Audio bus '{busName}' not found.");
            }
        }

        public void PlayMusic(string trackPath, bool loop = true)
        {
            Logger.Info($"AudioManager: Playing music track: {trackPath} (Loop={loop})");
            var stream = GD.Load<AudioStream>(trackPath);
            if (stream == null)
            {
                Logger.Error($"AudioManager: Failed to load music track from path '{trackPath}'.");
                return;
            }

            _bgmPlayer.Stream = stream;
            _bgmPlayer.VolumeDb = 0f; // Reset fade

            // Attempt to loop stream based on runtime type
            if (stream is AudioStreamMP3 mp3) mp3.Loop = loop;
            else if (stream is AudioStreamOggVorbis ogg) ogg.Loop = loop;

            _bgmPlayer.Play();
        }

        public void PlaySfx(string sfxPath, float volumeScale = 1.0f)
        {
            var stream = GD.Load<AudioStream>(sfxPath);
            if (stream == null)
            {
                Logger.Error($"AudioManager: Failed to load SFX track from path '{sfxPath}'.");
                return;
            }

            AudioStreamPlayer? player = null;
            foreach (var p in _sfxPlayersPool)
            {
                if (!p.Playing)
                {
                    player = p;
                    break;
                }
            }

            if (player == null)
            {
                player = CreateNewSfxPlayer();
            }

            player.Stream = stream;
            player.VolumeDb = Mathf.LinearToDb(volumeScale);
            player.Play();
        }

        /// <summary>
        /// Plays 3D positional SFX at a specific coordinate.
        /// </summary>
        public void PlaySfx3D(string sfxPath, Vector3 position, float volumeScale = 1.0f)
        {
            var stream = GD.Load<AudioStream>(sfxPath);
            if (stream == null)
            {
                Logger.Error($"AudioManager: Failed to load 3D SFX track from path '{sfxPath}'.");
                return;
            }

            AudioStreamPlayer3D? player = null;
            foreach (var p in _sfxPlayers3DPool)
            {
                if (!p.Playing)
                {
                    player = p;
                    break;
                }
            }

            if (player == null)
            {
                player = CreateNewSfxPlayer3D();
            }

            player.GlobalPosition = position;
            player.Stream = stream;
            player.VolumeDb = Mathf.LinearToDb(volumeScale);
            player.Play();
        }

        public void StopMusic()
        {
            if (_bgmPlayer.Playing)
            {
                Logger.Info("AudioManager: Stopping music playback with short fade-out.");
                var tween = CreateTween();
                tween.TweenProperty(_bgmPlayer, "volume_db", -80f, 0.4f);
                tween.TweenCallback(Callable.From(() => _bgmPlayer.Stop()));
            }
        }

        private AudioStreamPlayer CreateNewSfxPlayer()
        {
            var p = new AudioStreamPlayer { Name = $"SfxPlayer_{_sfxPlayersPool.Count}" };
            AddChild(p);
            _sfxPlayersPool.Add(p);
            return p;
        }

        private AudioStreamPlayer3D CreateNewSfxPlayer3D()
        {
            var p = new AudioStreamPlayer3D { Name = $"SfxPlayer3D_{_sfxPlayers3DPool.Count}" };
            AddChild(p);
            _sfxPlayers3DPool.Add(p);
            return p;
        }
    }
}
