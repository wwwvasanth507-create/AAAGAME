using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World.Content
{
    public enum DungeonType
    {
        Crypt,
        Mine,
        Cavern,
        Ruins,
        Fortress,
        Rift
    }

    public class DungeonRoomNode
    {
        public string RoomId { get; set; } = Guid.NewGuid().ToString();
        public string RoomType { get; set; } = "Standard";
        public Vector3 RelativePosition { get; set; }
        public List<string> ConnectedRoomIds { get; set; } = new();
        public bool IsBossRoom { get; set; } = false;
        public string EnemySpawnGroup { get; set; } = string.Empty;
        public string LootTableId { get; set; } = string.Empty;
    }

    public class DungeonDefinition
    {
        public string DungeonId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DungeonType Type { get; set; } = DungeonType.Crypt;
        public string Biome { get; set; } = "Plains";
        public Vector3 EntranceWorldPosition { get; set; }
        public int DifficultyRating { get; set; } = 1;
        public List<DungeonRoomNode> RoomGraph { get; set; } = new();
        public bool IsCleared { get; set; } = false;
    }

    /// <summary>
    /// Reusable dungeon architecture framework managing room graphs, enemy spawn hooks,
    /// puzzle triggers, and boss chamber anchors.
    /// </summary>
    public class DungeonFramework
    {
        private readonly Dictionary<string, DungeonDefinition> _dungeons = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterDungeon(DungeonDefinition dungeon)
        {
            if (dungeon != null && !string.IsNullOrEmpty(dungeon.DungeonId))
            {
                _dungeons[dungeon.DungeonId] = dungeon;
            }
        }

        public DungeonDefinition? GetDungeon(string dungeonId)
        {
            return _dungeons.TryGetValue(dungeonId, out var d) ? d : null;
        }

        public void MarkDungeonCleared(string dungeonId)
        {
            if (_dungeons.TryGetValue(dungeonId, out var d))
            {
                d.IsCleared = true;
            }
        }
    }
}
