using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter6
{
    public enum GuildRank
    {
        None,
        Recruit,
        Journeyman,
        Master,
        Grandmaster
    }

    public class GuildDefinitionRecord
    {
        public string GuildId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string GuildmasterNpcId { get; set; } = "";
        public GuildRank CurrentRank { get; set; } = GuildRank.None;
        public int ReputationPoints { get; set; } = 0;
        public bool IsJoined { get; set; } = false;
        public List<string> AvailableMissions { get; set; } = new();
    }

    /// <summary>
    /// Multi-guild progression manager for Hero of Eternia.
    /// Manages guild memberships, ranks, reputation meters, guild missions, and special vendor unlocks.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class GuildSystemManager : IInitializable
    {
        private readonly Dictionary<string, GuildDefinitionRecord> _guilds = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<string, GuildRank>? OnGuildRankChanged;
        public event Action<string, int>? OnGuildReputationChanged;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultGuilds();

            // Register with ServiceLocator
            ServiceLocator.Register<GuildSystemManager>(this);

            IsInitialized = true;
            Logger.Info("GuildSystemManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _guilds.Clear();

            ServiceLocator.Unregister<GuildSystemManager>();
            IsInitialized = false;
            Logger.Info("GuildSystemManager: Shutdown completed.");
        }

        private void RegisterDefaultGuilds()
        {
            // 1. Adventurers' Guild
            RegisterGuild(new GuildDefinitionRecord
            {
                GuildId = "guild_adventurers",
                Name = "The Crown Adventurers' Guild",
                Description = "A heroic guild dedicated to beast hunting, ruin exploration, and bounty hunting.",
                GuildmasterNpcId = "npc_guildmaster_vane",
                AvailableMissions = new List<string> { "mission_bounty_drael", "mission_swamp_monster_hunt" }
            });

            // 2. Arcane Circle Guild
            RegisterGuild(new GuildDefinitionRecord
            {
                GuildId = "guild_arcane_circle",
                Name = "The Arcane Circle of Mages",
                Description = "Venerable order of scholars and sorcerers devoted to spellcraft and ancient relics.",
                GuildmasterNpcId = "npc_archmage_serena",
                AvailableMissions = new List<string> { "mission_astral_crystal_research", "mission_void_seal_study" }
            });

            // 3. Iron Artisans Guild
            RegisterGuild(new GuildDefinitionRecord
            {
                GuildId = "guild_iron_artisans",
                Name = "Consortium of Iron Artisans",
                Description = "Master blacksmiths and alchemists advancing craftmanship and war smithing.",
                GuildmasterNpcId = "npc_grand_smith_thorin",
                AvailableMissions = new List<string> { "mission_mithril_ore_delivery", "mission_alchemical_catalyst" }
            });
        }

        public void RegisterGuild(GuildDefinitionRecord record)
        {
            if (record != null && !string.IsNullOrEmpty(record.GuildId))
            {
                _guilds[record.GuildId] = record;
            }
        }

        public bool JoinGuild(string guildId)
        {
            if (!_guilds.TryGetValue(guildId, out var guild))
            {
                Logger.Warning($"GuildSystemManager: Guild '{guildId}' not found.");
                return false;
            }

            if (guild.IsJoined) return true;

            guild.IsJoined = true;
            guild.CurrentRank = GuildRank.Recruit;

            OnGuildRankChanged?.Invoke(guildId, GuildRank.Recruit);
            Logger.Info($"GuildSystemManager: Player joined guild '{guild.Name}' as Recruit.");
            return true;
        }

        public bool AddReputation(string guildId, int points)
        {
            if (!_guilds.TryGetValue(guildId, out var guild)) return false;

            guild.ReputationPoints += points;
            OnGuildReputationChanged?.Invoke(guildId, guild.ReputationPoints);

            // Auto promotion check
            EvaluateRankPromotion(guild);

            Logger.Info($"GuildSystemManager: Guild '{guild.Name}' reputation increased to {guild.ReputationPoints}.");
            return true;
        }

        private void EvaluateRankPromotion(GuildDefinitionRecord guild)
        {
            GuildRank newRank = guild.CurrentRank;

            if (guild.ReputationPoints >= 1000 && guild.CurrentRank < GuildRank.Grandmaster)
                newRank = GuildRank.Grandmaster;
            else if (guild.ReputationPoints >= 500 && guild.CurrentRank < GuildRank.Master)
                newRank = GuildRank.Master;
            else if (guild.ReputationPoints >= 200 && guild.CurrentRank < GuildRank.Journeyman)
                newRank = GuildRank.Journeyman;

            if (newRank != guild.CurrentRank)
            {
                guild.CurrentRank = newRank;
                OnGuildRankChanged?.Invoke(guild.GuildId, newRank);
                Logger.Info($"GuildSystemManager: Guild '{guild.Name}' rank promoted to {newRank}!");
            }
        }

        public GuildDefinitionRecord? GetGuild(string guildId)
        {
            return _guilds.TryGetValue(guildId, out var record) ? record : null;
        }

        public List<GuildDefinitionRecord> GetAllGuilds()
        {
            return new List<GuildDefinitionRecord>(_guilds.Values);
        }
    }
}
