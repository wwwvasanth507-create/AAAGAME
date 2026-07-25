using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Exploration
{
    public enum SecretType
    {
        HiddenRoom,
        HiddenPassage,
        BreakableWall,
        IllusionaryWall,
        UndergroundEntrance,
        InvisibleTrigger,
        RareDiscovery,
        SecretAchievement
    }

    public class SecretDefinition
    {
        public string SecretId { get; set; } = string.Empty;
        public SecretType Type { get; set; } = SecretType.HiddenRoom;
        public Vector3 Position { get; set; }
        public float DetectionRadius { get; set; } = 5.0f;
        public bool IsDiscovered { get; set; } = false;
        public int RewardXp { get; set; } = 100;
        public string AudioHook { get; set; } = "audio_secret_revealed";
    }

    /// <summary>
    /// Secret discovery manager handling breakable walls, illusionary passages,
    /// hidden underground entrances, and invisible trigger reveals.
    /// </summary>
    public class SecretManager
    {
        private readonly Dictionary<string, SecretDefinition> _secrets = new(StringComparer.OrdinalIgnoreCase);

        public event Action<SecretDefinition>? OnSecretDiscovered;

        public void RegisterSecret(SecretDefinition secret)
        {
            if (secret != null && !string.IsNullOrEmpty(secret.SecretId))
            {
                _secrets[secret.SecretId] = secret;
            }
        }

        public bool DiscoverSecret(string secretId)
        {
            if (_secrets.TryGetValue(secretId, out var secret) && !secret.IsDiscovered)
            {
                secret.IsDiscovered = true;
                OnSecretDiscovered?.Invoke(secret);
                return true;
            }
            return false;
        }

        public bool IsDiscovered(string secretId)
        {
            return _secrets.TryGetValue(secretId, out var s) && s.IsDiscovered;
        }
    }
}
