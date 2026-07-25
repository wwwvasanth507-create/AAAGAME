using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Story
{
    /// <summary>
    /// Campaign orchestrator managing active chapter/mission tracking, unlocking content,
    /// and evaluating story progression rules.
    /// </summary>
    public class StoryProgressionManager
    {
        public ChapterFramework Chapters { get; } = new();
        public StoryDatabase StoryDatabase { get; } = new();

        public string ActiveChapterId { get; private set; } = "chapter_prologue";
        public string ActiveMissionId { get; private set; } = "story_prologue_01";

        private readonly HashSet<string> _completedChapterIds = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _completedMissionIds = new(StringComparer.OrdinalIgnoreCase);

        public event Action<string>? OnChapterStarted;
        public event Action<string>? OnChapterCompleted;

        public bool CompleteChapter(string chapterId)
        {
            if (_completedChapterIds.Add(chapterId))
            {
                OnChapterCompleted?.Invoke(chapterId);
                return true;
            }
            return false;
        }

        public bool IsChapterCompleted(string chapterId)
        {
            return _completedChapterIds.Contains(chapterId);
        }

        public void SetActiveChapter(string chapterId)
        {
            ActiveChapterId = chapterId;
            OnChapterStarted?.Invoke(chapterId);
        }

        public IEnumerable<string> CompletedChapterIds => _completedChapterIds;
        public IEnumerable<string> CompletedMissionIds => _completedMissionIds;
    }
}
