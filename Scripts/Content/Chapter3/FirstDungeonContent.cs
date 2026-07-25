using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Content.Chapter3
{
    public enum DungeonFloor
    {
        Entrance,
        FloorOne,
        FloorTwo,
        MiniBossArena,
        FloorThree,
        BossAntechamber,
        BossArena
    }

    public class DungeonRoom
    {
        public string RoomId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DungeonFloor Floor { get; set; } = DungeonFloor.FloorOne;
        public Vector3 Position { get; set; }
        public bool IsSecret { get; set; } = false;
        public bool IsCheckpoint { get; set; } = false;
        public string CheckpointId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Layout builder for the Citadel of Void Shadows — first major dungeon,
    /// spanning seven distinct floors with puzzles, mini-boss, and final boss arena.
    /// </summary>
    public class FirstDungeonContent
    {
        private readonly Dictionary<string, DungeonRoom> _rooms = new(StringComparer.OrdinalIgnoreCase);

        public void InitializeDungeon()
        {
            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_citadel_entrance",
                DisplayName = "Shattered Gates of Aethelgard",
                Floor = DungeonFloor.Entrance,
                Position = new Vector3(500, 0, 600),
                IsCheckpoint = true,
                CheckpointId = "cp_citadel_entrance"
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_floor1_shadow_corridor",
                DisplayName = "Shadow Corridor",
                Floor = DungeonFloor.FloorOne,
                Position = new Vector3(520, -5, 620),
                IsCheckpoint = true,
                CheckpointId = "cp_floor1"
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_floor1_puzzle_rune",
                DisplayName = "Rune Pressure Chamber",
                Floor = DungeonFloor.FloorOne,
                Position = new Vector3(560, -5, 640)
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_floor2_hazard_hall",
                DisplayName = "Void Spike Gauntlet",
                Floor = DungeonFloor.FloorTwo,
                Position = new Vector3(580, -12, 660)
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_floor2_secret_vault",
                DisplayName = "Ancient Etherian Vault",
                Floor = DungeonFloor.FloorTwo,
                Position = new Vector3(610, -12, 650),
                IsSecret = true
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_miniboss_arena",
                DisplayName = "Shadow Knight's Crucible",
                Floor = DungeonFloor.MiniBossArena,
                Position = new Vector3(620, -15, 680),
                IsCheckpoint = true,
                CheckpointId = "cp_miniboss"
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_floor3_lore_chamber",
                DisplayName = "Codex Hall of the Void",
                Floor = DungeonFloor.FloorThree,
                Position = new Vector3(640, -20, 700)
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_boss_antechamber",
                DisplayName = "Sanctum of the Void Gate",
                Floor = DungeonFloor.BossAntechamber,
                Position = new Vector3(660, -22, 720),
                IsCheckpoint = true,
                CheckpointId = "cp_boss_antechamber"
            });

            RegisterRoom(new DungeonRoom
            {
                RoomId = "room_boss_arena",
                DisplayName = "Throne of the Void — Malakor's Gate",
                Floor = DungeonFloor.BossArena,
                Position = new Vector3(680, -25, 750)
            });
        }

        public void RegisterRoom(DungeonRoom room)
        {
            if (room != null && !string.IsNullOrEmpty(room.RoomId))
                _rooms[room.RoomId] = room;
        }

        public DungeonRoom? GetRoom(string roomId)
            => _rooms.TryGetValue(roomId, out var r) ? r : null;

        public IReadOnlyCollection<DungeonRoom> AllRooms => _rooms.Values;

        public IEnumerable<DungeonRoom> GetCheckpoints()
        {
            foreach (var r in _rooms.Values)
                if (r.IsCheckpoint) yield return r;
        }
    }
}
