using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter5
{
    public class DungeonRoomDefinition
    {
        public string RoomId { get; set; } = "";
        public string Name { get; set; } = "";
        public int FloorNumber { get; set; } = 1;
        public string RouteType { get; set; } = "Main"; // VanguardAssault, SyndicateTunnels, SylvanSewer
        public bool IsCheckpoint { get; set; } = false;
        public bool IsSecretRoom { get; set; } = false;
        public string HazardType { get; set; } = ""; // FireTrap, PoisonGas, RunicSpikes
        public List<string> EnemySpawnIds { get; set; } = new();
    }

    /// <summary>
    /// Multi-level Faction Dungeon builder for Chapter 5: Stronghold of Iron & Shadow.
    /// Supports 6 floors, alternative entrance routes, checkpoints, puzzles, and boss encounters.
    /// </summary>
    public class FactionDungeonContent
    {
        private readonly Dictionary<string, DungeonRoomDefinition> _rooms = new(StringComparer.OrdinalIgnoreCase);

        public string DungeonId { get; } = "dungeon_faction_stronghold";
        public string DisplayName { get; } = "Stronghold of Iron & Shadow";
        public int TotalFloors { get; } = 6;

        public FactionDungeonContent()
        {
            InitializeRooms();
        }

        public void InitializeRooms()
        {
            // Floor 1: Alternative Entrances
            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_vanguard_gate",
                Name = "Iron Vanguard Assault Gate",
                FloorNumber = 1,
                RouteType = "VanguardAssault",
                IsCheckpoint = true,
                EnemySpawnIds = new List<string> { "enemy_vanguard_captain", "enemy_elite_soldier" }
            });

            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_syndicate_tunnels",
                Name = "Silver Syndicate Smugglers' Tunnel",
                FloorNumber = 1,
                RouteType = "SyndicateTunnels",
                IsCheckpoint = true,
                EnemySpawnIds = new List<string> { "enemy_veteran_bandit", "enemy_shadow_lurker" }
            });

            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_sylvan_sewer",
                Name = "Sylvan Waterway Secret Passage",
                FloorNumber = 1,
                RouteType = "SylvanSewer",
                IsCheckpoint = true,
                HazardType = "PoisonGas",
                EnemySpawnIds = new List<string> { "enemy_bog_witch", "enemy_corrupted_beast" }
            });

            // Floor 2: Central Courtyard & Armory
            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_courtyard",
                Name = "Central Armory Courtyard",
                FloorNumber = 2,
                RouteType = "Main",
                EnemySpawnIds = new List<string> { "enemy_heavy_defender", "enemy_spellcaster" }
            });

            // Floor 3: Crucible & Puzzles
            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_puzzle_crucible",
                Name = "Crucible of Elemental Relics",
                FloorNumber = 3,
                RouteType = "Main",
                HazardType = "FireTrap",
                EnemySpawnIds = new List<string> { "enemy_elemental_golem" }
            });

            // Floor 4: Inquisitor Hall & Checkpoint
            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_inquisitor_hall",
                Name = "Inquisitor's Tribunal",
                FloorNumber = 4,
                RouteType = "Main",
                IsCheckpoint = true,
                EnemySpawnIds = new List<string> { "enemy_spellcaster", "enemy_elite_soldier" }
            });

            // Floor 5: Secret Vault (Optional)
            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_secret_vault",
                Name = "Vault of the Forgotten Crown",
                FloorNumber = 5,
                RouteType = "Main",
                IsSecretRoom = true,
                HazardType = "RunicSpikes",
                EnemySpawnIds = new List<string> { "enemy_regional_champion" }
            });

            // Floor 6: Boss Arena
            RegisterRoom(new DungeonRoomDefinition
            {
                RoomId = "room_stronghold_boss_arena",
                Name = "Grand Marshal's Sanctuary",
                FloorNumber = 6,
                RouteType = "Main",
                IsCheckpoint = true,
                EnemySpawnIds = new List<string> { "enemy_boss_grand_marshal_kaelen" }
            });
        }

        public void RegisterRoom(DungeonRoomDefinition room)
        {
            if (room != null && !string.IsNullOrEmpty(room.RoomId))
            {
                _rooms[room.RoomId] = room;
            }
        }

        public DungeonRoomDefinition? GetRoom(string roomId)
        {
            return _rooms.TryGetValue(roomId, out var r) ? r : null;
        }

        public List<DungeonRoomDefinition> GetRoomsForFloor(int floor)
        {
            var list = new List<DungeonRoomDefinition>();
            foreach (var r in _rooms.Values)
            {
                if (r.FloorNumber == floor) list.Add(r);
            }
            return list;
        }
    }
}
