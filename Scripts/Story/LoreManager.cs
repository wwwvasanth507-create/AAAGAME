using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Story
{
    public enum LoreCategory
    {
        Book,
        Letter,
        StoneTablet,
        AncientRecord,
        AudioLog,
        MemoryFragment,
        TimelineEntry,
        HistoricalEvent
    }

    public class LoreDefinition
    {
        public string LoreId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public LoreCategory Category { get; set; } = LoreCategory.Book;
        public string ContentText { get; set; } = string.Empty;
        public string AudioLogStreamPath { get; set; } = string.Empty;
        public string TimelineEra { get; set; } = "First Age";
        public bool IsDiscovered { get; set; } = false;
    }

    /// <summary>
    /// Historical lore manager and codex database tracking discovered books, letters,
    /// ancient tablets, timeline entries, and memory fragments.
    /// </summary>
    public class LoreManager
    {
        private readonly Dictionary<string, LoreDefinition> _loreEntries = new(StringComparer.OrdinalIgnoreCase);

        public event Action<LoreDefinition>? OnLoreDiscovered;

        public void RegisterLore(LoreDefinition lore)
        {
            if (lore != null && !string.IsNullOrEmpty(lore.LoreId))
            {
                _loreEntries[lore.LoreId] = lore;
            }
        }

        public bool DiscoverLore(string loreId)
        {
            if (_loreEntries.TryGetValue(loreId, out var lore) && !lore.IsDiscovered)
            {
                lore.IsDiscovered = true;
                OnLoreDiscovered?.Invoke(lore);
                return true;
            }
            return false;
        }

        public LoreDefinition? GetLore(string loreId)
        {
            return _loreEntries.TryGetValue(loreId, out var lore) ? lore : null;
        }

        public List<LoreDefinition> GetDiscoveredLore()
        {
            return _loreEntries.Values.Where(l => l.IsDiscovered).ToList();
        }
    }
}
