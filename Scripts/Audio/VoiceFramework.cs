using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Audio
{
    public enum VoiceType
    {
        CombatBark,
        QuestDialogue,
        AmbientChatter,
        PlayerGrunt,
        BossTaunt,
        NPCGreeting
    }

    public class SubtitleLine
    {
        public string SpeakerName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public float DurationSeconds { get; set; } = 3.0f;
        public Color SpeakerColor { get; set; } = Colors.White;
    }

    /// <summary>
    /// Voice framework for NPC/player dialogue hooks, combat barks,
    /// ambient chatter, subtitle synchronization, and localized audio streams.
    /// </summary>
    public partial class VoiceFramework : Node
    {
        private AudioStreamPlayer _voicePlayer;
        private readonly Queue<SubtitleLine> _subtitleQueue = new();

        public event Action<SubtitleLine>? OnSubtitleTriggered;
        public event Action? OnSubtitleCleared;

        public bool SubtitlesEnabled { get; set; } = true;
        public float VoiceVolume { get; set; } = 1.0f;

        public override void _Ready()
        {
            _voicePlayer = new AudioStreamPlayer { Name = "VoicePlayer" };
            AddChild(_voicePlayer);
        }

        public void PlayBark(string speakerName, string text, AudioStream stream = null, float duration = 2.5f)
        {
            var line = new SubtitleLine
            {
                SpeakerName = speakerName,
                Text = text,
                DurationSeconds = duration,
                SpeakerColor = GetSpeakerColor(speakerName)
            };

            if (SubtitlesEnabled)
            {
                OnSubtitleTriggered?.Invoke(line);
            }

            if (stream != null && _voicePlayer != null)
            {
                _voicePlayer.Stream = stream;
                _voicePlayer.VolumeDb = Mathf.LinearToDb(VoiceVolume);
                _voicePlayer.Play();
            }
        }

        public void ClearSubtitles()
        {
            _subtitleQueue.Clear();
            OnSubtitleCleared?.Invoke();
        }

        private Color GetSpeakerColor(string speakerName)
        {
            if (string.IsNullOrEmpty(speakerName)) return Colors.White;
            int hash = Math.Abs(speakerName.GetHashCode());
            float h = (hash % 360) / 360.0f;
            return Color.FromHsv(h, 0.6f, 0.95f);
        }
    }
}
