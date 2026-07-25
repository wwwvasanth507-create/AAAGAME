using System;
using System.Collections.Generic;

namespace HeroOfEternia.World.Content
{
    public class WorldContentSaveData
    {
        public int WorldSeed { get; set; } = 42;
        public List<string> DiscoveredLocations { get; set; } = new();
        public List<string> ClearedDungeons { get; set; } = new();
        public List<POISpawnInstance> SpawnedPois { get; set; } = new();
        public int SaveVersion { get; set; } = 19;
    }
}
