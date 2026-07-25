using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HeroOfEternia.Dialogue
{
    // ==========================================================
    // DIALOGUE DATA MODELS
    // ==========================================================

    public enum DialogueSpeakerType
    {
        Npc,
        Player,
        Narrator,
        System,
        Item
    }

    public class DialogueCondition
    {
        public string Type { get; set; } = "";     // "flag", "quest", "reputation", "faction", "skill", "variable", "custom"
        public string Parameter { get; set; } = ""; // condition parameter (e.g., "flag:has_met_king")
        public string ExpectedValue { get; set; } = ""; // expected value for comparison
        public bool Negate { get; set; } = false;   // invert condition
    }

    public class DialogueChoice
    {
        public string ChoiceId { get; set; } = "";
        public string TextKey { get; set; } = "";          // localization key
        public List<DialogueCondition> Conditions { get; set; } = new();
        public string NextDialogueId { get; set; } = "";   // go to this dialogue on selection
        public string SetFlag { get; set; } = "";           // flag to set on selection
        public string SetFlagValue { get; set; } = "true";
        public string RecordDecision { get; set; } = "";    // decision ID to record
        public string RecordChoice { get; set; } = "";      // choice value to record
        public string SetVariable { get; set; } = "";       // variable name to set
        public string SetVariableValue { get; set; } = "";  // variable value to set
        public string QuestHookId { get; set; } = "";       // quest ID to accept/advance/complete
        public string QuestHookAction { get; set; } = "";   // "accept", "advance", "complete", "fail"
        public List<Quest.QuestReward> Rewards { get; set; } = new(); // immediate rewards on choice
        
        // Merchant/Service hooks
        public string MerchantHookId { get; set; } = "";    // open merchant shop
        public string ServiceHookId { get; set; } = "";     // use service (heal, repair, etc.)
        public string ServiceAction { get; set; } = "";     // "open_merchant", "open_service", "open_crafting"
        
        // Future cinematic hooks
        public string CinematicHookId { get; set; } = "";   // trigger cutscene
        public string CameraHookId { get; set; } = "";      // camera animation
    }

    public class DialogueEntry
    {
        public string DialogueId { get; set; } = "";
        public string SpeakerId { get; set; } = "";      // NPC ID or "narrator" or "player"
        public DialogueSpeakerType SpeakerType { get; set; } = DialogueSpeakerType.Npc;
        public string TargetId { get; set; } = "";       // who is being addressed (optional)
        public string TextKey { get; set; } = "";         // localization key for dialogue text
        public string AudioKey { get; set; } = "";        // voice clip key
        public string EmotionHook { get; set; } = "";     // emotion to play (e.g., "angry", "happy", "sad")
        public string AnimationHook { get; set; } = "";   // animation to play on speaker
        public string CameraHook { get; set; } = "";      // camera shot type
        public string VfxHook { get; set; } = "";         // visual effect to trigger
        
        // Conditions to display this dialogue
        public List<DialogueCondition> Conditions { get; set; } = new();
        
        // Player choices (empty = auto-advance dialogue)
        public List<DialogueChoice> Choices { get; set; } = new();
        
        // Quest hooks
        public string QuestHookId { get; set; } = "";     // quest ID to hook into
        public string QuestHookAction { get; set; } = "";  // "advance_objective", "complete_objective", "set_flag"
        public string QuestObjectiveId { get; set; } = ""; // objective to advance on display
        
        // Nested conversation context
        public string NextDialogueId { get; set; } = "";   // auto-advance to next dialogue if no choices
        public bool IsEndOfConversation { get; set; } = false; // ends conversation
        
        // Nested conversation support
        public string ParentDialogueId { get; set; } = ""; // for nested conversations
        public int Depth { get; set; } = 0;                // nesting depth (prevents infinite loops)
    }

    public class ConversationDefinition
    {
        public string ConversationId { get; set; } = "";
        public string NpcId { get; set; } = "";           // primary NPC
        public string StartingDialogueId { get; set; } = "";
        public List<DialogueEntry> Dialogues { get; set; } = new();
        public List<DialogueCondition> EntryConditions { get; set; } = new();
        public string OnStartFlag { get; set; } = "";     // flag set when conversation starts
        public string OnEndFlag { get; set; } = "";       // flag set when conversation ends
        public int MaxDepth { get; set; } = 10;           // prevent infinite loops
    }

    // ==========================================================
    // DIALOGUE DATABASE
    // ==========================================================

    /// <summary>
    /// Central registry for all dialogue entries and conversations.
    /// Fully data-driven - designers add dialogue via JSON.
    /// Thread-safe with O(1) lookups.
    /// </summary>
    public class DialogueDatabase
    {
        private static readonly object _lock = new();
        private static Dictionary<string, DialogueEntry> _dialogues = new();
        private static Dictionary<string, ConversationDefinition> _conversations = new();
        private static Dictionary<string, List<string>> _npcConversations = new(); // npcId -> conversationIds
        private static bool _initialized = false;

        // ==========================================================
        // INITIALIZATION
        // ==========================================================

        /// <summary>
        /// Loads dialogue definitions from a JSON file.
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

                    var conversations = JsonSerializer.Deserialize<List<ConversationDefinition>>(json, options);
                    if (conversations == null)
                    {
                        Godot.GD.PrintErr("[DialogueDatabase] Failed to deserialize dialogues from " + jsonPath);
                        return;
                    }

                    RegisterConversations(conversations);
                    Godot.GD.Print($"[DialogueDatabase] Loaded {conversations.Count} conversations from {jsonPath}");
                }
                catch (Exception ex)
                {
                    Godot.GD.PrintErr($"[DialogueDatabase] Error loading dialogues from {jsonPath}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Loads dialogues from a JSON string (for testing).
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

                    var conversations = JsonSerializer.Deserialize<List<ConversationDefinition>>(json, options);
                    if (conversations == null)
                    {
                        Godot.GD.PrintErr("[DialogueDatabase] Failed to deserialize dialogues from JSON string");
                        return;
                    }

                    RegisterConversations(conversations);
                    Godot.GD.Print($"[DialogueDatabase] Loaded {conversations.Count} conversations from JSON string");
                }
                catch (Exception ex)
                {
                    Godot.GD.PrintErr($"[DialogueDatabase] Error loading dialogues from JSON string: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Registers a conversation at runtime.
        /// </summary>
        public static void RegisterConversation(ConversationDefinition conversation)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(conversation.ConversationId)) return;

                _conversations[conversation.ConversationId] = conversation;

                // Register all dialogue entries
                foreach (var dialogue in conversation.Dialogues)
                {
                    if (!string.IsNullOrEmpty(dialogue.DialogueId))
                        _dialogues[dialogue.DialogueId] = dialogue;
                }

                // NPC index
                if (!string.IsNullOrEmpty(conversation.NpcId))
                {
                    if (!_npcConversations.ContainsKey(conversation.NpcId))
                        _npcConversations[conversation.NpcId] = new List<string>();
                    if (!_npcConversations[conversation.NpcId].Contains(conversation.ConversationId))
                        _npcConversations[conversation.NpcId].Add(conversation.ConversationId);
                }

                _initialized = true;
            }
        }

        /// <summary>
        /// Registers multiple conversations at once.
        /// </summary>
        public static void RegisterConversations(IEnumerable<ConversationDefinition> conversations)
        {
            lock (_lock)
            {
                foreach (var conv in conversations)
                    RegisterConversation(conv);
            }
        }

        // ==========================================================
        // LOOKUP METHODS
        // ==========================================================

        /// <summary>
        /// Gets a dialogue entry by ID. O(1).
        /// </summary>
        public static DialogueEntry? GetDialogue(string dialogueId)
        {
            lock (_lock)
            {
                _dialogues.TryGetValue(dialogueId, out var entry);
                return entry;
            }
        }

        /// <summary>
        /// Gets a conversation definition by ID. O(1).
        /// </summary>
        public static ConversationDefinition? GetConversation(string conversationId)
        {
            lock (_lock)
            {
                _conversations.TryGetValue(conversationId, out var conv);
                return conv;
            }
        }

        /// <summary>
        /// Gets all conversations for a specific NPC.
        /// </summary>
        public static List<ConversationDefinition> GetConversationsForNpc(string npcId)
        {
            lock (_lock)
            {
                if (!_npcConversations.TryGetValue(npcId, out var convIds))
                    return new List<ConversationDefinition>();

                return convIds
                    .Select(id => _conversations.TryGetValue(id, out var c) ? c : null)
                    .Where(c => c != null)
                    .Cast<ConversationDefinition>()
                    .ToList();
            }
        }

        /// <summary>
        /// Gets all conversations.
        /// </summary>
        public static List<ConversationDefinition> GetAllConversations()
        {
            lock (_lock)
            {
                return _conversations.Values.ToList();
            }
        }

        /// <summary>
        /// Gets the starting dialogue for a conversation.
        /// </summary>
        public static DialogueEntry? GetStartingDialogue(string conversationId)
        {
            lock (_lock)
            {
                if (!_conversations.TryGetValue(conversationId, out var conv))
                    return null;

                if (string.IsNullOrEmpty(conv.StartingDialogueId))
                    return conv.Dialogues.FirstOrDefault();

                _dialogues.TryGetValue(conv.StartingDialogueId, out var entry);
                return entry;
            }
        }

        /// <summary>
        /// Checks if a dialogue entry exists.
        /// </summary>
        public static bool HasDialogue(string dialogueId)
        {
            lock (_lock)
            {
                return _dialogues.ContainsKey(dialogueId);
            }
        }

        /// <summary>
        /// Gets total dialogue count.
        /// </summary>
        public static int DialogueCount
        {
            get { lock (_lock) { return _dialogues.Count; } }
        }

        /// <summary>
        /// Gets total conversation count.
        /// </summary>
        public static int ConversationCount
        {
            get { lock (_lock) { return _conversations.Count; } }
        }

        /// <summary>
        /// Clears all data (for testing).
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _dialogues.Clear();
                _conversations.Clear();
                _npcConversations.Clear();
                _initialized = false;
            }
        }
    }
}