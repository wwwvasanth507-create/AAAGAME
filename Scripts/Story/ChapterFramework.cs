using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Story
{
    public enum ChapterType
    {
        Prologue,
        Act,
        Chapter,
        Mission,
        Interlude,
        FinaleHook,
        PostGameHook,
        ExpansionHook
    }

    public class ChapterDefinition
    {
        public string ChapterId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ChapterType Type { get; set; } = ChapterType.Chapter;
        public List<string> Prerequisites { get; set; } = new();
        public List<string> MissionIds { get; set; } = new();
        public bool IsCompleted { get; set; } = false;
        public bool AllowReplay { get; set; } = true;
    }

    /// <summary>
    /// Chapter architecture framework managing campaign structure, prerequisites,
    /// interludes, finale hooks, post-game content, and expansion modules.
    /// </summary>
    public class ChapterFramework
    {
        private readonly Dictionary<string, ChapterDefinition> _chapters = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterChapter(ChapterDefinition chapter)
        {
            if (chapter != null && !string.IsNullOrEmpty(chapter.ChapterId))
            {
                _chapters[chapter.ChapterId] = chapter;
            }
        }

        public ChapterDefinition? GetChapter(string chapterId)
        {
            return _chapters.TryGetValue(chapterId, out var c) ? c : null;
        }

        public bool IsChapterUnlocked(string chapterId, IEnumerable<string> completedChapterIds)
        {
            if (!_chapters.TryGetValue(chapterId, out var ch)) return false;
            if (ch.Prerequisites.Count == 0) return true;

            var completedSet = new HashSet<string>(completedChapterIds, StringComparer.OrdinalIgnoreCase);
            return ch.Prerequisites.All(prereq => completedSet.Contains(prereq));
        }

        public void RegisterDefaultChapters()
        {
            RegisterChapter(new ChapterDefinition
            {
                ChapterId = "chapter_prologue",
                DisplayName = "Prologue: First Light",
                Type = ChapterType.Prologue,
                MissionIds = new List<string> { "story_prologue_01" }
            });

            RegisterChapter(new ChapterDefinition
            {
                ChapterId = "chapter_act1_ch1",
                DisplayName = "Act I: Shadows over Eternia",
                Type = ChapterType.Act,
                Prerequisites = new List<string> { "chapter_prologue" }
            });
        }
    }
}
