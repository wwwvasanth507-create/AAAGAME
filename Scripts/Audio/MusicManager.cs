using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Audio
{
    public enum MusicState
    {
        Exploration,
        Settlement,
        Combat,
        Boss,
        Dungeon,
        Victory,
        Defeat
    }

    /// <summary>
    /// Adaptive music manager for smooth crossfading, intensity scaling,
    /// dynamic state changes (Exploration, Combat, Boss, Night), and track pooling.
    /// </summary>
    public partial class MusicManager : Node
    {
        private AudioStreamPlayer _primaryPlayer;
        private AudioStreamPlayer _secondaryPlayer;
        private AudioStreamPlayer _activePlayer;
        private AudioStreamPlayer _inactivePlayer;

        private MusicState _currentState = MusicState.Exploration;
        private string _currentTrackId = string.Empty;
        private float _fadeDuration = 2.0f;
        private bool _isFading = false;
        private float _fadeTimer = 0f;

        private readonly Dictionary<string, AudioStream> _trackRegistry = new();
        private readonly Dictionary<MusicState, string> _stateDefaultTracks = new();

        public MusicState CurrentState => _currentState;
        public string CurrentTrackId => _currentTrackId;

        public float MusicVolume { get; set; } = 0.8f;

        public event Action<MusicState, string>? OnStateChanged;

        public override void _Ready()
        {
            _primaryPlayer = new AudioStreamPlayer { Name = "MusicPrimary" };
            _secondaryPlayer = new AudioStreamPlayer { Name = "MusicSecondary" };

            AddChild(_primaryPlayer);
            AddChild(_secondaryPlayer);

            _activePlayer = _primaryPlayer;
            _inactivePlayer = _secondaryPlayer;
        }

        public void RegisterTrack(string trackId, AudioStream stream)
        {
            if (string.IsNullOrEmpty(trackId) || stream == null) return;
            _trackRegistry[trackId] = stream;
        }

        public void SetStateDefaultTrack(MusicState state, string trackId)
        {
            _stateDefaultTracks[state] = trackId;
        }

        public void TransitionToState(MusicState state, float fadeTime = 2.0f)
        {
            if (_currentState == state) return;
            _currentState = state;

            if (_stateDefaultTracks.TryGetValue(state, out string trackId))
            {
                PlayTrack(trackId, fadeTime);
            }

            OnStateChanged?.Invoke(_currentState, _currentTrackId);
        }

        public void PlayTrack(string trackId, float fadeTime = 2.0f)
        {
            if (_currentTrackId == trackId && _activePlayer.Playing) return;

            _currentTrackId = trackId;
            _fadeDuration = MathF.Max(0.1f, fadeTime);

            if (_trackRegistry.TryGetValue(trackId, out var stream))
            {
                // Swap active and inactive players for crossfade
                var temp = _activePlayer;
                _activePlayer = _inactivePlayer;
                _inactivePlayer = temp;

                _activePlayer.Stream = stream;
                _activePlayer.VolumeDb = Mathf.LinearToDb(0.001f);
                _activePlayer.Play();

                _isFading = true;
                _fadeTimer = 0f;
            }
        }

        public override void _Process(double delta)
        {
            if (!_isFading) return;

            _fadeTimer += (float)delta;
            float progress = Math.Clamp(_fadeTimer / _fadeDuration, 0f, 1f);

            float activeVol = Math.Clamp(progress * MusicVolume, 0.001f, 1.0f);
            float inactiveVol = Math.Clamp((1.0f - progress) * MusicVolume, 0.001f, 1.0f);

            if (_activePlayer != null) _activePlayer.VolumeDb = Mathf.LinearToDb(activeVol);
            if (_inactivePlayer != null) _inactivePlayer.VolumeDb = Mathf.LinearToDb(inactiveVol);

            if (progress >= 1.0f)
            {
                _isFading = false;
                _inactivePlayer?.Stop();
            }
        }
    }
}
