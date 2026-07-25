using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Initial biome types canonical to Eternia.
    /// </summary>
    public enum BiomeType
    {
        Forest,
        Grassland,
        Desert,
        Snow,
        Mountain,
        Swamp,
        Volcano,
        Beach,
        Ocean,
        Jungle,
        CrystalCaverns,
        AncientRuins,
        HauntedForest
    }

    /// <summary>
    /// Data-driven configuration container for a single Biome Type.
    /// Defines elevation boundaries, environmental parameters, and audio/weather profiles.
    /// </summary>
    public class BiomeDefinition
    {
        public BiomeType Type { get; set; }
        public string Name { get; set; } = "";
        
        public float Temperature { get; set; } = 0.5f; // 0.0 to 1.0 scale
        public float Humidity { get; set; } = 0.5f;    // 0.0 to 1.0 scale
        
        public float MinElevation { get; set; } = 0.0f; // Height range limits
        public float MaxElevation { get; set; } = 1.0f;
        
        public string TerrainType { get; set; } = ""; // "Flat", "Hilly", "Mountainous"
        public string SkyProfile { get; set; } = "";     // Res path to Sky Material
        public string AmbientSoundProfile { get; set; } = ""; // AudioManager profile keys
        public string WeatherProfile { get; set; } = "";
        public string LightingProfile { get; set; } = "";
        public string MusicProfile { get; set; } = "";

        // Placement rules configurations
        public List<string> SpawnRules { get; set; } = new();
        public List<string> ResourceRules { get; set; } = new();

        // DLC expansions properties map
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }
}
