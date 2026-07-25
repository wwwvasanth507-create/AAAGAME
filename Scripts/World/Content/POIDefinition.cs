using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World.Content
{
    /// <summary>
    /// Data-driven definition model for Points of Interest (POIs).
    /// </summary>
    public class POIDefinition
    {
        public string PoiId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public POIType Type { get; set; } = POIType.AncientRuins;
        public List<string> BiomeRestrictions { get; set; } = new();
        public float MinDistanceToSameType { get; set; } = 300f;
        public float MinDistanceToSettlement { get; set; } = 150f;
        public float SpawnWeight { get; set; } = 1.0f;
        public int DifficultyRating { get; set; } = 1;
        public POISize Size { get; set; } = POISize.Medium;
        public List<string> RequiredTags { get; set; } = new();
        public List<string> ForbiddenTags { get; set; } = new();
        public string MusicHookTrack { get; set; } = string.Empty;
        public string AmbientAudioZone { get; set; } = string.Empty;
        public string VisualTheme { get; set; } = "default";
        public string LootTableId { get; set; } = string.Empty;
        public string StoryHookId { get; set; } = string.Empty;
        public string DlcModuleId { get; set; } = string.Empty;
    }

    public class POISpawnInstance
    {
        public string InstanceId { get; set; } = Guid.NewGuid().ToString();
        public string PoiId { get; set; } = string.Empty;
        public Vector3 WorldPosition { get; set; }
        public float RotationDegreesY { get; set; } = 0.0f;
        public bool IsDiscovered { get; set; } = false;
        public double DiscoveredTimeSeconds { get; set; } = 0.0;
    }
}
