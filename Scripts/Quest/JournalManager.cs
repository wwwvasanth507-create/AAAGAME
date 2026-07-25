using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Quest
{
    /// <summary>
    /// Journal system managing quest tracking, history, lore entries,
    /// dialogue logs, and discovery records.
    /// Data-driven with no hardcoded story content.
    /// </summary>
    public class JournalManager
    {
        // ==========================================================
        // EVENTS
        // ==========================================================
        public event Action<string>? QuestAddedToJournal;      // questId
        public event Action<string>? QuestRemovedFromJournal;   // questId
        public event Action<LoreEntry>? LoreEntryUnlocked;
        public event Action<DialogueLogEntry>? DialogueLogged;
        public event Action<DiscoveryEntry>? DiscoveryMade;
        public event Action? JournalUpdated;

        // ==========================================================
        // JOURNAL STATE
        // ==========================================================
        private readonly Dictionary<string, JournalQuestEntry> _activeJournalQuests = new();
        private readonly Dictionary<string, JournalQuestEntry> _completedJournalQuests = new();
        private readonly Dictionary<string, JournalQuestEntry> _failedJournalQuests = new();
        private readonly List<LoreEntry> _loreEntries = new();
        private readonly List<DialogueLogEntry> _dialogueLog = new();
        private readonly List<DiscoveryEntry> _discoveries = new();

        // ==========================================================
        // QUEST JOURNAL
        // ==========================================================

        /// <summary>
        /// Adds a quest to the active journal.
        /// </summary>
        public void AddQuestToJournal(QuestInstance instance)
        {
            var definition = instance.GetDefinition();
            if (definition == null) return;

            var entry = new JournalQuestEntry
            {
                QuestId = instance.QuestId,
                QuestName = definition.DisplayName,
                Category = definition.Category,
                TitleKey = definition.TitleKey,
                DescriptionKey = definition.DescriptionKey,
                ProgressKey = definition.ProgressKey,
                CompletionKey = definition.CompletionKey,
                FailureKey = definition.FailureKey,
                AbandonKey = definition.AbandonKey,
                RecommendedLevel = definition.RecommendedLevel,
                QuestGiverId = definition.QuestGiverId,
                AcceptedTime = instance.AcceptedTime,
                State = QuestState.Active,
                Objectives = new List<ObjectiveProgressInfo>()
            };

            _activeJournalQuests[instance.QuestId] = entry;
            QuestAddedToJournal?.Invoke(instance.QuestId);
            JournalUpdated?.Invoke();
        }

        /// <summary>
        /// Updates a quest's progress in the journal.
        /// </summary>
        public void UpdateQuestProgress(QuestInstance instance, List<ObjectiveProgressInfo> progress)
        {
            if (_activeJournalQuests.TryGetValue(instance.QuestId, out var entry))
            {
                entry.Objectives = progress;
                entry.LastUpdated = DateTime.UtcNow;
                JournalUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Moves a quest from active to completed journal.
        /// </summary>
        public void CompleteQuestInJournal(QuestInstance instance)
        {
            if (_activeJournalQuests.Remove(instance.QuestId, out var entry))
            {
                entry.State = QuestState.Completed;
                entry.CompletedTime = instance.CompletedTime;
                _completedJournalQuests[instance.QuestId] = entry;
                QuestRemovedFromJournal?.Invoke(instance.QuestId);
                JournalUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Moves a quest from active to failed journal.
        /// </summary>
        public void FailQuestInJournal(QuestInstance instance)
        {
            if (_activeJournalQuests.Remove(instance.QuestId, out var entry))
            {
                entry.State = QuestState.Failed;
                _failedJournalQuests[instance.QuestId] = entry;
                QuestRemovedFromJournal?.Invoke(instance.QuestId);
                JournalUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Removes a quest from the active journal (abandonment).
        /// </summary>
        public void RemoveQuestFromJournal(string questId)
        {
            if (_activeJournalQuests.Remove(questId))
            {
                QuestRemovedFromJournal?.Invoke(questId);
                JournalUpdated?.Invoke();
            }
        }

        /// <summary>
        /// Gets all active journal quests.
        /// </summary>
        public List<JournalQuestEntry> GetActiveJournalQuests()
        {
            return _activeJournalQuests.Values.OrderBy(q => q.AcceptedTime).ToList();
        }

        /// <summary>
        /// Gets all completed journal quests.
        /// </summary>
        public List<JournalQuestEntry> GetCompletedJournalQuests()
        {
            return _completedJournalQuests.Values
                .OrderByDescending(q => q.CompletedTime)
                .ToList();
        }

        /// <summary>
        /// Gets all failed journal quests.
        /// </summary>
        public List<JournalQuestEntry> GetFailedJournalQuests()
        {
            return _failedJournalQuests.Values
                .OrderByDescending(q => q.LastUpdated)
                .ToList();
        }

        /// <summary>
        /// Gets a journal entry by quest ID.
        /// </summary>
        public JournalQuestEntry? GetJournalEntry(string questId)
        {
            if (_activeJournalQuests.TryGetValue(questId, out var entry)) return entry;
            if (_completedJournalQuests.TryGetValue(questId, out entry)) return entry;
            if (_failedJournalQuests.TryGetValue(questId, out entry)) return entry;
            return null;
        }

        /// <summary>
        /// Gets quest history (all entries across all states).
        /// </summary>
        public List<JournalQuestEntry> GetQuestHistory()
        {
            var all = new List<JournalQuestEntry>();
            all.AddRange(_activeJournalQuests.Values);
            all.AddRange(_completedJournalQuests.Values);
            all.AddRange(_failedJournalQuests.Values);
            return all.OrderByDescending(q => q.LastUpdated).ToList();
        }

        // ==========================================================
        // LORE ENTRIES
        // ==========================================================

        /// <summary>
        /// Unlocks a lore entry in the journal.
        /// </summary>
        public void UnlockLoreEntry(string loreId, string titleKey, string bodyKey, string category = "")
        {
            // Don't add duplicates
            if (_loreEntries.Any(e => e.LoreId == loreId)) return;

            var entry = new LoreEntry
            {
                LoreId = loreId,
                TitleKey = titleKey,
                BodyKey = bodyKey,
                Category = category,
                UnlockedTime = DateTime.UtcNow
            };

            _loreEntries.Add(entry);
            LoreEntryUnlocked?.Invoke(entry);
            JournalUpdated?.Invoke();
        }

        /// <summary>
        /// Gets all unlocked lore entries.
        /// </summary>
        public List<LoreEntry> GetLoreEntries(string category = "")
        {
            if (string.IsNullOrEmpty(category))
                return _loreEntries.OrderBy(e => e.UnlockedTime).ToList();

            return _loreEntries
                .Where(e => e.Category == category)
                .OrderBy(e => e.UnlockedTime)
                .ToList();
        }

        /// <summary>
        /// Checks if a lore entry has been unlocked.
        /// </summary>
        public bool HasLoreEntry(string loreId)
        {
            return _loreEntries.Any(e => e.LoreId == loreId);
        }

        // ==========================================================
        // DIALOGUE LOG
        // ==========================================================

        /// <summary>
        /// Logs a dialogue entry to the journal.
        /// </summary>
        public void LogDialogue(string npcId, string npcName, string dialogueKey, string playerChoice = "")
        {
            var entry = new DialogueLogEntry
            {
                NpcId = npcId,
                NpcName = npcName,
                DialogueKey = dialogueKey,
                PlayerChoice = playerChoice,
                Timestamp = DateTime.UtcNow
            };

            _dialogueLog.Add(entry);
            DialogueLogged?.Invoke(entry);
            JournalUpdated?.Invoke();
        }

        /// <summary>
        /// Gets the dialogue log, optionally filtered.
        /// </summary>
        public List<DialogueLogEntry> GetDialogueLog(int maxEntries = 100, string? npcId = null)
        {
            var query = _dialogueLog.AsEnumerable();

            if (!string.IsNullOrEmpty(npcId))
                query = query.Where(e => e.NpcId == npcId);

            return query
                .OrderByDescending(e => e.Timestamp)
                .Take(maxEntries)
                .ToList();
        }

        /// <summary>
        /// Clears the dialogue log.
        /// </summary>
        public void ClearDialogueLog()
        {
            _dialogueLog.Clear();
            JournalUpdated?.Invoke();
        }

        // ==========================================================
        // DISCOVERY LOG
        // ==========================================================

        /// <summary>
        /// Records a discovery in the journal.
        /// </summary>
        public void RecordDiscovery(string locationId, string locationName, string descriptionKey, DiscoveryType type)
        {
            // Don't add duplicates
            if (_discoveries.Any(d => d.LocationId == locationId)) return;

            var entry = new DiscoveryEntry
            {
                LocationId = locationId,
                LocationName = locationName,
                DescriptionKey = descriptionKey,
                Type = type,
                DiscoveredTime = DateTime.UtcNow
            };

            _discoveries.Add(entry);
            DiscoveryMade?.Invoke(entry);
            JournalUpdated?.Invoke();
        }

        /// <summary>
        /// Gets all discoveries.
        /// </summary>
        public List<DiscoveryEntry> GetDiscoveries(DiscoveryType? type = null)
        {
            if (!type.HasValue)
                return _discoveries.OrderBy(d => d.DiscoveredTime).ToList();

            return _discoveries
                .Where(d => d.Type == type.Value)
                .OrderBy(d => d.DiscoveredTime)
                .ToList();
        }

        /// <summary>
        /// Gets the count of discoveries.
        /// </summary>
        public int DiscoveryCount => _discoveries.Count;

        // ==========================================================
        // SAVE / LOAD
        // ==========================================================

        /// <summary>
        /// Saves journal state to a serializable object.
        /// </summary>
        public JournalSaveData GetSaveData()
        {
            return new JournalSaveData
            {
                ActiveEntries = _activeJournalQuests.Values.ToList(),
                CompletedEntries = _completedJournalQuests.Values.ToList(),
                FailedEntries = _failedJournalQuests.Values.ToList(),
                LoreEntries = _loreEntries.ToList(),
                DialogueLog = _dialogueLog.ToList(),
                Discoveries = _discoveries.ToList()
            };
        }

        /// <summary>
        /// Restores journal state from saved data.
        /// </summary>
        public void LoadSaveData(JournalSaveData saveData)
        {
            if (saveData == null) return;

            _activeJournalQuests.Clear();
            _completedJournalQuests.Clear();
            _failedJournalQuests.Clear();
            _loreEntries.Clear();
            _dialogueLog.Clear();
            _discoveries.Clear();

            foreach (var entry in saveData.ActiveEntries)
                _activeJournalQuests[entry.QuestId] = entry;

            foreach (var entry in saveData.CompletedEntries)
                _completedJournalQuests[entry.QuestId] = entry;

            foreach (var entry in saveData.FailedEntries)
                _failedJournalQuests[entry.QuestId] = entry;

            _loreEntries.AddRange(saveData.LoreEntries);
            _dialogueLog.AddRange(saveData.DialogueLog);
            _discoveries.AddRange(saveData.Discoveries);
        }

        /// <summary>
        /// Clears all journal data (for testing).
        /// </summary>
        public void Clear()
        {
            _activeJournalQuests.Clear();
            _completedJournalQuests.Clear();
            _failedJournalQuests.Clear();
            _loreEntries.Clear();
            _dialogueLog.Clear();
            _discoveries.Clear();
        }
    }

    // ==========================================================
    // JOURNAL DATA MODELS
    // ==========================================================

    public class JournalQuestEntry
    {
        public string QuestId { get; set; } = "";
        public string QuestName { get; set; } = "";
        public QuestCategory Category { get; set; }
        public string TitleKey { get; set; } = "";
        public string DescriptionKey { get; set; } = "";
        public string ProgressKey { get; set; } = "";
        public string CompletionKey { get; set; } = "";
        public string FailureKey { get; set; } = "";
        public string AbandonKey { get; set; } = "";
        public int RecommendedLevel { get; set; } = 1;
        public string QuestGiverId { get; set; } = "";
        public QuestState State { get; set; }
        public DateTime AcceptedTime { get; set; }
        public DateTime CompletedTime { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public List<ObjectiveProgressInfo> Objectives { get; set; } = new();
    }

    public class LoreEntry
    {
        public string LoreId { get; set; } = "";
        public string TitleKey { get; set; } = "";
        public string BodyKey { get; set; } = "";
        public string Category { get; set; } = ""; // "world", "faction", "character", "item", "bestiary", etc.
        public DateTime UnlockedTime { get; set; }
    }

    public class DialogueLogEntry
    {
        public string NpcId { get; set; } = "";
        public string NpcName { get; set; } = "";
        public string DialogueKey { get; set; } = "";
        public string PlayerChoice { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }

    public enum DiscoveryType
    {
        Location,
        Landmark,
        Settlement,
        Dungeon,
        Boss,
        HiddenArea,
        Lore,
        Resource
    }

    public class DiscoveryEntry
    {
        public string LocationId { get; set; } = "";
        public string LocationName { get; set; } = "";
        public string DescriptionKey { get; set; } = "";
        public DiscoveryType Type { get; set; }
        public DateTime DiscoveredTime { get; set; }
    }

    public class JournalSaveData
    {
        public List<JournalQuestEntry> ActiveEntries { get; set; } = new();
        public List<JournalQuestEntry> CompletedEntries { get; set; } = new();
        public List<JournalQuestEntry> FailedEntries { get; set; } = new();
        public List<LoreEntry> LoreEntries { get; set; } = new();
        public List<DialogueLogEntry> DialogueLog { get; set; } = new();
        public List<DiscoveryEntry> Discoveries { get; set; } = new();

        // Future expansion hooks
        public List<object> BestiaryEntries { get; set; } = new();
        public List<object> CodexEntries { get; set; } = new();
    }
}