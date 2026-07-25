using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HeroOfEternia.Quest
{
    /// <summary>
    /// Central registry for all quest definitions.
    /// Data-driven: designers add quests via JSON, no code changes needed.
    /// Thread-safe for concurrent access.
    /// </summary>
    public class QuestDatabase
    {
        private static readonly object _lock = new();
        private static Dictionary<string, QuestDefinition> _quests = new();
        private static Dictionary<QuestCategory, List<string>> _categoryIndex = new();
        private static Dictionary<string, List<string>> _questGiverIndex = new();
        private static Dictionary<string, List<string>> _factionIndex = new();
        private static Dictionary<string, List<string>> _regionIndex = new();
        private static bool _initialized = false;

        // ==========================================================
        // INITIALIZATION
        // ==========================================================

        /// <summary>
        /// Loads quest definitions from a JSON file path.
        /// </summary>
        public static void LoadFromFile(string jsonPath)
        {
            lock (_lock)
            {
                try
                {
                    string json = File.ReadAllText(jsonPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    
                    var quests = JsonSerializer.Deserialize<List<QuestDefinition>>(json, options);
                    if (quests == null)
                    {
                        Godot.GD.PrintErr("[QuestDatabase] Failed to deserialize quests from " + jsonPath);
                        return;
                    }

                    RegisterQuests(quests);
                    Godot.GD.Print($"[QuestDatabase] Loaded {quests.Count} quests from {jsonPath}");
                }
                catch (Exception ex)
                {
                    Godot.GD.PrintErr($"[QuestDatabase] Error loading quests from {jsonPath}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Loads quest definitions from a JSON string (for testing).
        /// </summary>
        public static void LoadFromJsonString(string json)
        {
            lock (_lock)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    
                    var quests = JsonSerializer.Deserialize<List<QuestDefinition>>(json, options);
                    if (quests == null)
                    {
                        Godot.GD.PrintErr("[QuestDatabase] Failed to deserialize quests from JSON string");
                        return;
                    }

                    RegisterQuests(quests);
                    Godot.GD.Print($"[QuestDatabase] Loaded {quests.Count} quests from JSON string");
                }
                catch (Exception ex)
                {
                    Godot.GD.PrintErr($"[QuestDatabase] Error loading quests from JSON string: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Registers a quest definition at runtime (for testing or plugins).
        /// </summary>
        public static void RegisterQuest(QuestDefinition quest)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(quest.QuestId))
                {
                    Godot.GD.PrintErr("[QuestDatabase] Cannot register quest with empty QuestId");
                    return;
                }

                _quests[quest.QuestId] = quest;
                AddToIndexes(quest);
                _initialized = true;
            }
        }

        /// <summary>
        /// Registers multiple quests at once.
        /// </summary>
        public static void RegisterQuests(IEnumerable<QuestDefinition> quests)
        {
            lock (_lock)
            {
                foreach (var quest in quests)
                {
                    if (string.IsNullOrEmpty(quest.QuestId)) continue;
                    _quests[quest.QuestId] = quest;
                    AddToIndexes(quest);
                }
                _initialized = true;
            }
        }

        private static void AddToIndexes(QuestDefinition quest)
        {
            // Category index
            if (!_categoryIndex.ContainsKey(quest.Category))
                _categoryIndex[quest.Category] = new List<string>();
            _categoryIndex[quest.Category].Add(quest.QuestId);

            // Quest giver index
            if (!string.IsNullOrEmpty(quest.QuestGiverId))
            {
                if (!_questGiverIndex.ContainsKey(quest.QuestGiverId))
                    _questGiverIndex[quest.QuestGiverId] = new List<string>();
                _questGiverIndex[quest.QuestGiverId].Add(quest.QuestId);
            }

            // Faction index
            if (!string.IsNullOrEmpty(quest.RequiredFactionId))
            {
                if (!_factionIndex.ContainsKey(quest.RequiredFactionId))
                    _factionIndex[quest.RequiredFactionId] = new List<string>();
                _factionIndex[quest.RequiredFactionId].Add(quest.QuestId);
            }
        }

        // ==========================================================
        // LOOKUP METHODS
        // ==========================================================

        /// <summary>
        /// Gets a quest definition by ID. Returns null if not found.
        /// O(1) dictionary lookup.
        /// </summary>
        public static QuestDefinition? GetQuest(string questId)
        {
            lock (_lock)
            {
                _quests.TryGetValue(questId, out var quest);
                return quest;
            }
        }

        /// <summary>
        /// Gets all quest definitions.
        /// </summary>
        public static List<QuestDefinition> GetAllQuests()
        {
            lock (_lock)
            {
                return _quests.Values.ToList();
            }
        }

        /// <summary>
        /// Gets all enabled quest definitions.
        /// </summary>
        public static List<QuestDefinition> GetEnabledQuests()
        {
            lock (_lock)
            {
                return _quests.Values.Where(q => q.IsEnabled).ToList();
            }
        }

        /// <summary>
        /// Gets quests by category.
        /// O(1) indexed lookup.
        /// </summary>
        public static List<QuestDefinition> GetQuestsByCategory(QuestCategory category)
        {
            lock (_lock)
            {
                if (!_categoryIndex.TryGetValue(category, out var ids))
                    return new List<QuestDefinition>();

                return ids
                    .Select(id => _quests.TryGetValue(id, out var q) ? q : null)
                    .Where(q => q != null)
                    .Cast<QuestDefinition>()
                    .ToList();
            }
        }

        /// <summary>
        /// Gets quests by quest giver NPC ID.
        /// O(1) indexed lookup.
        /// </summary>
        public static List<QuestDefinition> GetQuestsByGiver(string npcId)
        {
            lock (_lock)
            {
                if (!_questGiverIndex.TryGetValue(npcId, out var ids))
                    return new List<QuestDefinition>();

                return ids
                    .Select(id => _quests.TryGetValue(id, out var q) ? q : null)
                    .Where(q => q != null)
                    .Cast<QuestDefinition>()
                    .ToList();
            }
        }

        /// <summary>
        /// Gets quests by required faction.
        /// O(1) indexed lookup.
        /// </summary>
        public static List<QuestDefinition> GetQuestsByFaction(string factionId)
        {
            lock (_lock)
            {
                if (!_factionIndex.TryGetValue(factionId, out var ids))
                    return new List<QuestDefinition>();

                return ids
                    .Select(id => _quests.TryGetValue(id, out var q) ? q : null)
                    .Where(q => q != null)
                    .Cast<QuestDefinition>()
                    .ToList();
            }
        }

        /// <summary>
        /// Searches quests by name or ID (case-insensitive contains).
        /// </summary>
        public static List<QuestDefinition> SearchQuests(string search)
        {
            lock (_lock)
            {
                string lower = search.ToLowerInvariant();
                return _quests.Values
                    .Where(q => q.QuestId.ToLowerInvariant().Contains(lower) ||
                                q.InternalName.ToLowerInvariant().Contains(lower) ||
                                q.DisplayName.ToLowerInvariant().Contains(lower))
                    .ToList();
            }
        }

        /// <summary>
        /// Gets all quests available at a given level range.
        /// </summary>
        public static List<QuestDefinition> GetQuestsByLevelRange(int minLevel, int maxLevel)
        {
            lock (_lock)
            {
                return _quests.Values
                    .Where(q => q.RecommendedLevel >= minLevel && q.RecommendedLevel <= maxLevel)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets quest count.
        /// </summary>
        public static int QuestCount
        {
            get { lock (_lock) { return _quests.Count; } }
        }

        /// <summary>
        /// Checks if a quest ID exists.
        /// </summary>
        public static bool HasQuest(string questId)
        {
            lock (_lock)
            {
                return _quests.ContainsKey(questId);
            }
        }

        /// <summary>
        /// Clears all quests (for testing).
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _quests.Clear();
                _categoryIndex.Clear();
                _questGiverIndex.Clear();
                _factionIndex.Clear();
                _regionIndex.Clear();
                _initialized = false;
            }
        }
    }
}