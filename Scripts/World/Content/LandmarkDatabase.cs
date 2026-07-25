using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HeroOfEternia.World.Content
{
    public enum LandmarkCategory
    {
        Major,
        Minor,
        NavigationMarker,
        ScenicViewpoint
    }

    public class LandmarkDefinition
    {
        public string LandmarkId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public LandmarkCategory Category { get; set; } = LandmarkCategory.Minor;
        public Vector3 WorldPosition { get; set; }
        public float DiscoveryRadius { get; set; } = 50f;
        public int ExplorationXpReward { get; set; } = 50;
        public float VisualUniquenessScore { get; set; } = 0.8f;
        public string DiscoveryAudioHook { get; set; } = "audio_landmark_discovered";
    }

    public class LandmarkDatabase
    {
        private readonly Dictionary<string, LandmarkDefinition> _landmarks = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterLandmark(LandmarkDefinition landmark)
        {
            if (landmark != null && !string.IsNullOrEmpty(landmark.LandmarkId))
            {
                _landmarks[landmark.LandmarkId] = landmark;
            }
        }

        public LandmarkDefinition? GetLandmark(string landmarkId)
        {
            return _landmarks.TryGetValue(landmarkId, out var lm) ? lm : null;
        }

        public List<LandmarkDefinition> GetAllLandmarks()
        {
            return _landmarks.Values.ToList();
        }

        public void RegisterDefaultLandmarks()
        {
            RegisterLandmark(new LandmarkDefinition
            {
                LandmarkId = "lm_titan_spire",
                DisplayName = "Titan Spire Peak",
                Category = LandmarkCategory.Major,
                WorldPosition = new Vector3(0, 120, 500),
                ExplorationXpReward = 200,
                VisualUniquenessScore = 1.0f
            });

            RegisterLandmark(new LandmarkDefinition
            {
                LandmarkId = "lm_whispering_falls",
                DisplayName = "Whispering Falls Viewpoint",
                Category = LandmarkCategory.ScenicViewpoint,
                WorldPosition = new Vector3(-250, 40, -100),
                ExplorationXpReward = 100,
                VisualUniquenessScore = 0.9f
            });
        }
    }
}
