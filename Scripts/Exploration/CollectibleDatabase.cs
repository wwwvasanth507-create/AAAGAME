using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Exploration
{
    public enum CollectibleCategory
    {
        AncientRelic,
        Book,
        Scroll,
        Map,
        Statue,
        Artifact,
        MusicRecord,
        Treasure,
        CreatureEntry,
        Cosmetic
    }

    public class CollectibleDefinition
    {
        public string CollectibleId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public CollectibleCategory Category { get; set; } = CollectibleCategory.Artifact;
        public string LoreText { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data-driven database and progress tracker for world collectibles and lore items.
    /// </summary>
    public class CollectibleDatabase
    {
        private readonly Dictionary<string, CollectibleDefinition> _collectibles = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _collectedIds = new(StringComparer.OrdinalIgnoreCase);

        public event Action<CollectibleDefinition>? OnCollectibleAcquired;

        public void RegisterCollectible(CollectibleDefinition item)
        {
            if (item != null && !string.IsNullOrEmpty(item.CollectibleId))
            {
                _collectibles[item.CollectibleId] = item;
            }
        }

        public bool CollectItem(string collectibleId)
        {
            if (_collectibles.TryGetValue(collectibleId, out var item) && _collectedIds.Add(collectibleId))
            {
                OnCollectibleAcquired?.Invoke(item);
                return true;
            }
            return false;
        }

        public bool IsCollected(string collectibleId)
        {
            return _collectedIds.Contains(collectibleId);
        }

        public int CollectedCount => _collectedIds.Count;
        public int TotalCount => _collectibles.Count;

        public void LoadCollectedItems(IEnumerable<string> items)
        {
            _collectedIds.Clear();
            if (items != null)
            {
                foreach (var id in items) _collectedIds.Add(id);
            }
        }

        public IEnumerable<string> CollectedIds => _collectedIds;
    }
}
