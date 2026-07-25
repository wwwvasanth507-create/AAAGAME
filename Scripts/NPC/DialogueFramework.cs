using System;
using System.Collections.Generic;
using HeroOfEternia.World;

namespace HeroOfEternia.NPC
{
    public enum DialogueCategory
    {
        Greeting,
        Farewell,
        IdleComment,
        WeatherComment,
        TimeOfDayComment,
        RelationshipVariant
    }

    /// <summary>
    /// A single dialogue entry. Content is represented by a localization key only —
    /// no story text is embedded in code. Voice lines are referenced by VoiceClipKey.
    /// </summary>
    public class DialogueLine
    {
        public string LocalizationKey { get; set; } = "";
        public DialogueCategory Category { get; set; } = DialogueCategory.IdleComment;
        public string ConditionTag { get; set; } = "";       // e.g. "time_morning", "weather_rain", "rel_friend"
        public float RelationshipThreshold { get; set; } = 0f; // minimum aggregate score required
        public string VoiceClipKey { get; set; } = "";        // future voice playback
        public string LocaleOverride { get; set; } = "";      // e.g. "fr", "de" — empty = default
    }

    /// <summary>
    /// Dialogue resolver. Given an NPC's type, current relationship score, time-of-day,
    /// and weather, selects the best-matching dialogue line from the NPC's registry.
    /// No branching story content — only localization key resolution.
    /// </summary>
    public class DialogueFramework
    {
        private readonly Dictionary<string, List<DialogueLine>> _npcDialogues = new();

        // ─────────────────────── Registration ───────────────────────

        public void RegisterLine(string npcId, DialogueLine line)
        {
            if (!_npcDialogues.TryGetValue(npcId, out var list))
            {
                list = new List<DialogueLine>();
                _npcDialogues[npcId] = list;
            }
            list.Add(line);
        }

        public void RegisterLines(string npcId, IEnumerable<DialogueLine> lines)
        {
            foreach (var l in lines) RegisterLine(npcId, l);
        }

        // ─────────────────────── Resolution ───────────────────────

        /// <summary>
        /// Resolves the best matching DialogueLine for a category given context.
        /// </summary>
        public DialogueLine? Resolve(
            string npcId,
            DialogueCategory category,
            float relationshipScore,
            double timeOfDay,
            string weatherTag = "",
            string locale = "")
        {
            if (!_npcDialogues.TryGetValue(npcId, out var lines)) return null;

            string timeTag = GetTimeTag(timeOfDay);
            DialogueLine? best = null;
            int bestScore = -1;

            foreach (var line in lines)
            {
                if (line.Category != category) continue;
                if (relationshipScore < line.RelationshipThreshold) continue;
                if (!string.IsNullOrEmpty(line.LocaleOverride) && line.LocaleOverride != locale) continue;

                int score = 0;
                if (!string.IsNullOrEmpty(line.ConditionTag))
                {
                    if (line.ConditionTag == timeTag)    score += 2;
                    if (line.ConditionTag == weatherTag) score += 2;
                    if (line.ConditionTag.StartsWith("rel_") && MatchesRelationship(line.ConditionTag, relationshipScore)) score += 3;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    best = line;
                }
            }
            return best;
        }

        // ─────────────────────── Default line set ───────────────────────

        /// <summary>
        /// Generates a minimal default set of localization-keyed lines for a new NPC.
        /// </summary>
        public static List<DialogueLine> BuildDefaultLines(NpcTypeEnum npcType)
        {
            string prefix = npcType.ToString().ToLowerInvariant();
            return new List<DialogueLine>
            {
                new DialogueLine { Category = DialogueCategory.Greeting,        LocalizationKey = $"npc.{prefix}.greeting.default",          ConditionTag = "" },
                new DialogueLine { Category = DialogueCategory.Greeting,        LocalizationKey = $"npc.{prefix}.greeting.morning",          ConditionTag = "time_morning" },
                new DialogueLine { Category = DialogueCategory.Greeting,        LocalizationKey = $"npc.{prefix}.greeting.friend",           ConditionTag = "rel_friend", RelationshipThreshold = 40f },
                new DialogueLine { Category = DialogueCategory.Farewell,        LocalizationKey = $"npc.{prefix}.farewell.default",          ConditionTag = "" },
                new DialogueLine { Category = DialogueCategory.IdleComment,     LocalizationKey = $"npc.{prefix}.idle.default",             ConditionTag = "" },
                new DialogueLine { Category = DialogueCategory.WeatherComment,  LocalizationKey = $"npc.{prefix}.weather.rain",             ConditionTag = "weather_rain" },
                new DialogueLine { Category = DialogueCategory.WeatherComment,  LocalizationKey = $"npc.{prefix}.weather.sunny",            ConditionTag = "weather_sunny" },
                new DialogueLine { Category = DialogueCategory.TimeOfDayComment,LocalizationKey = $"npc.{prefix}.time.night",              ConditionTag = "time_night" },
                new DialogueLine { Category = DialogueCategory.RelationshipVariant, LocalizationKey = $"npc.{prefix}.rel.rival",           ConditionTag = "rel_rival", RelationshipThreshold = -60f },
            };
        }

        // ─────────────────────── Helpers ───────────────────────

        private static string GetTimeTag(double timeOfDay)
        {
            return timeOfDay switch
            {
                < 0.20 => "time_night",
                < 0.45 => "time_morning",
                < 0.65 => "time_afternoon",
                < 0.80 => "time_evening",
                _      => "time_night"
            };
        }

        private static bool MatchesRelationship(string conditionTag, float score)
        {
            return conditionTag switch
            {
                "rel_rival"  => score <= -60f,
                "rel_neutral" => score is > -40f and < 40f,
                "rel_friend" => score >= 40f,
                "rel_bestfriend" => score >= 75f,
                _ => false
            };
        }
    }
}
