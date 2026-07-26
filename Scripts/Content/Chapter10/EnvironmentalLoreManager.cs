using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter10
{
    public class EnvironmentalLoreRecord
    {
        public string LoreId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "Mural";
        public string TextContent { get; set; } = "";
        public string LocationChamberId { get; set; } = "";
        public bool IsDiscovered { get; set; } = false;
        public string UnlocksCodexCategory { get; set; } = "History";
    }

    /// <summary>
    /// Environmental Lore Manager for Chapter 10 & Act III Finale.
    /// Manages ancient murals, historical echoes, inscriptions, broken relics, and codex entries.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class EnvironmentalLoreManager : IInitializable
    {
        private readonly Dictionary<string, EnvironmentalLoreRecord> _loreRecords = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<EnvironmentalLoreRecord>? OnLoreDiscovered;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultLoreRecords();

            // Register with ServiceLocator
            ServiceLocator.Register<EnvironmentalLoreManager>(this);

            IsInitialized = true;
            Logger.Info("EnvironmentalLoreManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _loreRecords.Clear();

            ServiceLocator.Unregister<EnvironmentalLoreManager>();
            IsInitialized = false;
            Logger.Info("EnvironmentalLoreManager: Shutdown completed.");
        }

        private void RegisterDefaultLoreRecords()
        {
            // 1. Sun Carving Mural
            RegisterLoreRecord(new EnvironmentalLoreRecord
            {
                LoreId = "lore_sun_carving_mural",
                Title = "Mural of the First Dawn",
                Category = "Mural",
                TextContent = "Before Malakor's shadow, the sun kings forged three celestial seals to bind the Void Gate.",
                LocationChamberId = "chamber_temple_entrance",
                UnlocksCodexCategory = "Ancient_History"
            });

            // 2. Astral War Inscription
            RegisterLoreRecord(new EnvironmentalLoreRecord
            {
                LoreId = "lore_astral_war_inscription",
                Title = "The Astral War Tablet",
                Category = "Inscription",
                TextContent = "Malakor was once the High Warden of the Sun Spire before taking the Void Core.",
                LocationChamberId = "chamber_sun_court",
                UnlocksCodexCategory = "Antagonist_Origin"
            });

            // 3. Codex Revelation Plate
            RegisterLoreRecord(new EnvironmentalLoreRecord
            {
                LoreId = "lore_codex_malakor_truth",
                Title = "Plate of the Shattered Crown",
                Category = "Golden Codex",
                TextContent = "The Crown Seal was broken from within the High Council, not by siege engines.",
                LocationChamberId = "chamber_subterranean_sanctum",
                UnlocksCodexCategory = "Act_III_Climax"
            });
        }

        public void RegisterLoreRecord(EnvironmentalLoreRecord record)
        {
            if (record != null && !string.IsNullOrEmpty(record.LoreId))
            {
                _loreRecords[record.LoreId] = record;
            }
        }

        public bool DiscoverLore(string loreId)
        {
            if (!_loreRecords.TryGetValue(loreId, out var record))
            {
                Logger.Warning($"EnvironmentalLoreManager: Lore record '{loreId}' not found.");
                return false;
            }

            if (record.IsDiscovered) return true;

            record.IsDiscovered = true;
            OnLoreDiscovered?.Invoke(record);

            Logger.Info($"EnvironmentalLoreManager: Player discovered lore '{record.Title}' ({loreId}). Unlocked codex category: {record.UnlocksCodexCategory}.");
            return true;
        }

        public EnvironmentalLoreRecord? GetLoreRecord(string loreId)
        {
            return _loreRecords.TryGetValue(loreId, out var r) ? r : null;
        }

        public List<EnvironmentalLoreRecord> GetAllLoreRecords()
        {
            return new List<EnvironmentalLoreRecord>(_loreRecords.Values);
        }
    }
}
