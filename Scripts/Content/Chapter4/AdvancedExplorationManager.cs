using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public class ExplorationVaultDefinition
    {
        public string VaultId { get; set; } = "";
        public string Name { get; set; } = "";
        public string RegionId { get; set; } = "";
        public int PuzzleStages { get; set; } = 3;
        public string KeyItemId { get; set; } = "";
        public string RewardLootTableId { get; set; } = "";
        public bool IsCleared { get; set; } = false;
    }

    /// <summary>
    /// Advanced Exploration & Traversal Manager for Act II.
    /// Controls hidden vaults, multi-stage puzzles, traversal challenges, and optional elite encounters.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class AdvancedExplorationManager : IInitializable
    {
        private readonly Dictionary<string, ExplorationVaultDefinition> _vaults = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<ExplorationVaultDefinition>? OnVaultCleared;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultVaults();

            // Register with ServiceLocator
            ServiceLocator.Register<AdvancedExplorationManager>(this);

            IsInitialized = true;
            Logger.Info("AdvancedExplorationManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _vaults.Clear();

            ServiceLocator.Unregister<AdvancedExplorationManager>();
            IsInitialized = false;
            Logger.Info("AdvancedExplorationManager: Shutdown completed.");
        }

        private void RegisterDefaultVaults()
        {
            // 1. Vault of the Whispering Ridgeline
            RegisterVault(new ExplorationVaultDefinition
            {
                VaultId = "vault_ridgeline_01",
                Name = "Vault of the Whispering Ridgeline",
                RegionId = "region_eastern_ridgeline",
                PuzzleStages = 3,
                KeyItemId = "key_ridgeline_emblem",
                RewardLootTableId = "loot_table_vault_ridgeline"
            });

            // 2. Sunken Grotto Vault
            RegisterVault(new ExplorationVaultDefinition
            {
                VaultId = "vault_mirkwood_01",
                Name = "Sunken Grotto Vault",
                RegionId = "region_mirkwood_swamps",
                PuzzleStages = 4,
                KeyItemId = "key_bog_jewel",
                RewardLootTableId = "loot_table_vault_mirkwood"
            });
        }

        public void RegisterVault(ExplorationVaultDefinition vault)
        {
            if (vault != null && !string.IsNullOrEmpty(vault.VaultId))
            {
                _vaults[vault.VaultId] = vault;
            }
        }

        public bool ClearVault(string vaultId)
        {
            if (!_vaults.TryGetValue(vaultId, out var vault))
            {
                Logger.Warning($"AdvancedExplorationManager: Vault '{vaultId}' not found.");
                return false;
            }

            if (vault.IsCleared) return true;

            vault.IsCleared = true;
            OnVaultCleared?.Invoke(vault);

            Logger.Info($"AdvancedExplorationManager: Vault '{vault.Name}' ({vaultId}) successfully cleared!");
            return true;
        }

        public ExplorationVaultDefinition? GetVault(string vaultId)
        {
            return _vaults.TryGetValue(vaultId, out var vault) ? vault : null;
        }

        public List<ExplorationVaultDefinition> GetAllVaults()
        {
            return new List<ExplorationVaultDefinition>(_vaults.Values);
        }
    }
}
