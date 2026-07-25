using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HeroOfEternia.NPC;

namespace HeroOfEternia.Core
{
    // ==========================================================
    // LOCAL DATA STORAGE MODELS
    // ==========================================================

    public class PlayerStats
    {
        public string CharacterName { get; set; } = "Eternian Hero";
        public int Level { get; set; } = 1;
        public int CurrentXp { get; set; } = 0;
        public int Health { get; set; } = 100;
        public int Mana { get; set; } = 50;
        public int Stamina { get; set; } = 100;
    }

    public class InventoryData
    {
        public List<string> Items { get; set; } = new List<string>();
        public List<string> Equipment { get; set; } = new List<string>();
        public List<string> CraftedItems { get; set; } = new List<string>();
    }

    public class QuestData
    {
        public Dictionary<string, string> ActiveQuests { get; set; } = new Dictionary<string, string>();
        public List<string> CompletedQuests { get; set; } = new List<string>();
    }

    public class WorldState
    {
        public string WorldSeed { get; set; } = "EterniaSeed42";
        public float TimeOfDay { get; set; } = 12.0f;
        public List<string> MapDiscovery { get; set; } = new List<string>();
        public Dictionary<string, string> NpcStates { get; set; } = new Dictionary<string, string>();
    }

    public class Statistics
    {
        public double PlayTimeSeconds { get; set; } = 0.0;
        public int KillsCount { get; set; } = 0;
        public int SavesCount { get; set; } = 0;
    }

    /// <summary>
    /// Represents the full local save profile.
    /// Flexible JSON structure allows adding unlimited future fields without breaking older saves.
    /// </summary>
    public class SaveProfile
    {
        public int SaveVersion { get; set; } = 13;
        public string GameVersion { get; set; } = "1.0.0";
        public PlayerStats Stats { get; set; } = new PlayerStats();
        public InventoryData Inventory { get; set; } = new InventoryData();
        public QuestData Quests { get; set; } = new QuestData();
        public WorldState World { get; set; } = new WorldState();
        public Statistics StatsData { get; set; } = new Statistics();
        
        // Equipped visual parts: PartCategory to resource path
        public Dictionary<string, string> EquippedParts { get; set; } = new();

        // Base attribute values for stats
        public Dictionary<string, float> BaseAttributes { get; set; } = new();

        // Active effect types
        public List<string> ActiveEffects { get; set; } = new();

        // Inventory Systems
        public List<Inventory.InventorySlot> PlayerInventory { get; set; } = new();
        public Dictionary<string, Inventory.InventorySlot> EquippedSlots { get; set; } = new();
        public Dictionary<string, List<Inventory.InventorySlot>> StorageChests { get; set; } = new();

        // Procedural World Systems
        public ulong WorldSeed { get; set; } = 12345u;
        public HashSet<string> DiscoveredRegions { get; set; } = new();
        public Dictionary<string, List<string>> ModifiedChunkNodes { get; set; } = new();

        // Procedural Terrain & Decoration Systems
        public Dictionary<string, List<string>> ModifiedDecorations { get; set; } = new();
        public HashSet<string> DiscoveredNavRegions { get; set; } = new();
        public Dictionary<string, string> PopulatedLandmarks { get; set; } = new();

        // NPC Systems (Save V6)
        /// <summary>NPC runtime states keyed by NPC unique ID.</summary>
        public Dictionary<string, NpcSaveState> NpcStates { get; set; } = new();
        /// <summary>Flat reputation snapshot: "global", "reg:regionId", "fac:factionId", "ind:npcId".</summary>
        public Dictionary<string, int> ReputationSnapshot { get; set; } = new();
        /// <summary>Relationship snapshot keyed by "npcA_npcB" → float[4] (Friendship, Trust, Respect, Fear).</summary>
        public Dictionary<string, float[]> RelationshipSnapshot { get; set; } = new();

        // Combat Systems (Save V7)
        /// <summary>Combat style IDs unlocked by the player (framework hook).</summary>
        public List<string> UnlockedCombatStyles { get; set; } = new();
        /// <summary>Ability IDs the player has learned (framework hook — abilities not implemented yet).</summary>
        public List<string> LearnedAbilities { get; set; } = new();
        /// <summary>Temporary per-session combat modifiers (e.g. "damage_bonus" → 1.2f). Not persisted between sessions by default.</summary>
        public Dictionary<string, float> TemporaryCombatModifiers { get; set; } = new();
        /// <summary>Per-weapon durability remaining (weaponId → float 0.0–1.0).</summary>
        public Dictionary<string, float> WeaponDurability { get; set; } = new();

        // Gameplay Expansion Systems (Save V8)
        /// <summary>Ability IDs the player has unlocked via levelling.</summary>
        public List<string> UnlockedAbilityIds { get; set; } = new();
        /// <summary>Up to 4 equipped ability slot IDs (index = slot 0–3, empty string = empty slot).</summary>
        public string[] EquippedAbilitySlots { get; set; } = new string[4];
        /// <summary>Current player character level.</summary>
        public int PlayerLevel { get; set; } = 1;
        /// <summary>Current player XP within the current level.</summary>
        public int PlayerXp { get; set; } = 0;
        /// <summary>Total enemies killed across all sessions.</summary>
        public int EnemiesKilledTotal { get; set; } = 0;
        /// <summary>Total waves completed across all sessions.</summary>
        public int WavesCompleted { get; set; } = 0;

        // Boss & Encounter Systems (Save V9)
        public List<string> CompletedEncounters { get; set; } = new();
        public List<string> DefeatedBossIds { get; set; } = new();
        public List<string> EncounteredElites { get; set; } = new();
        public List<string> ClaimedRewards { get; set; } = new();

        // Ability System (Save V10)
        /// <summary>Ability levels keyed by ability ID.</summary>
        public Dictionary<string, int> AbilityLevels { get; set; } = new();
        /// <summary>Current loadout configuration (index 0 = active).</summary>
        public List<Player.Abilities.LoadoutSaveData> LoadoutData { get; set; } = new();
        /// <summary>Active loadout index.</summary>
        public int ActiveLoadoutIndex { get; set; } = 0;
        /// <summary>Ability manager runtime state (cooldowns, charges, etc.).</summary>
        public Player.Abilities.AbilityManagerSaveData? AbilityManagerState { get; set; }
        /// <summary>Progression data (level, XP, prestige).</summary>
        public Player.Progression.ProgressionSaveData? ProgressionData { get; set; }

        // Equipment Systems (Save V11)
        /// <summary>Complete equipment progression save data.</summary>
        public Equipment.Save.EquipmentSaveData? EquipmentData { get; set; }

        // Gathering, Profession & Crafting Systems (Save V12)
        /// <summary>Profession levels and experience.</summary>
        public List<Gathering.ProfessionSaveState> ProfessionStates { get; set; } = new();
        /// <summary>Resource node world states for respawn tracking.</summary>
        public List<Gathering.ResourceNodeState> ResourceNodeStates { get; set; } = new();
        /// <summary>Known/learned recipe IDs.</summary>
        public List<string> KnownRecipeIds { get; set; } = new();
        /// <summary>Active craft queue items.</summary>
        public List<Crafting.CraftQueueItem> CraftQueueItems { get; set; } = new();

        // Economy Systems (Save V13)
        /// <summary>Complete economy system save data.</summary>
        public Economy.EconomySaveData? EconomyData { get; set; }

        // Settlement Systems (Save V14)
        /// <summary>Complete settlement system save data.</summary>
        public Settlement.SettlementSaveData? SettlementData { get; set; }

        // Quest & Dialogue Systems (Save V15)
        /// <summary>Complete quest system save data.</summary>
        public Quest.QuestSaveData? QuestData { get; set; }
        /// <summary>Complete journal system save data.</summary>
        public Quest.JournalSaveData? JournalData { get; set; }
        /// <summary>Complete narrative system save data.</summary>
        public Quest.NarrativeSaveData? NarrativeData { get; set; }
        /// <summary>Dialogue manager runtime state.</summary>
        public Dialogue.DialogueManagerSaveData? DialogueData { get; set; }

        // Audio System (Save V16)
        /// <summary>Audio preferences and category settings.</summary>
        public Audio.AudioSettings? AudioData { get; set; }

        // Animation System (Save V17)
        /// <summary>Animation settings and IK preferences.</summary>
        public Animation.AnimationSaveData? AnimationData { get; set; }

        // Visual Presentation & Graphics System (Save V18)
        /// <summary>Graphics settings and post-processing preferences.</summary>
        public Graphics.GraphicsSaveData? GraphicsData { get; set; }

        // Procedural World Content System (Save V19)
        /// <summary>Exploration tracking, POI states, and dungeon completion.</summary>
        public World.Content.WorldContentSaveData? WorldContentData { get; set; }

        // Reusable Exploration Content Framework (Save V20)
        /// <summary>Activities, solved puzzles, discovered secrets, and collected items.</summary>
        public Exploration.ExplorationContentSaveData? ExplorationContentData { get; set; }

        // Reusable Story Progression Framework (Save V21)
        /// <summary>Campaign progression, active missions, world state flags, and lore codex.</summary>
        public Story.StoryProgressionSaveData? StoryProgressionData { get; set; }

        // Campaign Design & Narrative Blueprint (Save V22)
        /// <summary>Discovered region profiles, relationship levels, and defeated villains.</summary>
        public Story.Campaign.CampaignSaveData? CampaignData { get; set; }

        // Prologue, Starting Region & Chapter 1 Implementation (Save V23)
        /// <summary>Tutorial step progress, NPC interactions, and Chapter 1 states.</summary>
        public Content.Prologue.PrologueSaveData? PrologueData { get; set; }

        // Chapter 2, Second Region & First Major Story Arc (Save V24)
        /// <summary>Sylvanwood locations, Elderwood reputation, relic decision, and world phase.</summary>
        public Content.Chapter2.Chapter2SaveData? Chapter2Data { get; set; }

        // Chapter 3, First Major Dungeon & Act I Finale (Save V25)
        /// <summary>Dungeon room progress, checkpoints, boss phase, and Act I completion flag.</summary>
        public Content.Chapter3.Chapter3SaveData? Chapter3Data { get; set; }

        // Act II — Eastern Ridgeline & Mirkwood Swamps (Save V26)
        /// <summary>Act II region discoveries, companion join state, watchtower liberation, and recipe unlocks.</summary>
        public Content.Chapter4.Act2SaveData? Act2Data { get; set; }

        // Custom dictionary for future-proofing, plugins, or DLC variables
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new Dictionary<string, object>();
    }

    public class SaveMetadata
    {
        public int SlotId { get; set; }
        public string CharacterName { get; set; } = "";
        public int Level { get; set; }
        public double PlayTimeSeconds { get; set; }
        public string GameVersion { get; set; } = "";
        public long Timestamp { get; set; }
    }

    // ==========================================================
    // SAVE MANAGER SERVICE
    // ==========================================================

    public class SaveManager
    {
        private const int CurrentSaveVersion = 15;
        private const string GameVersionStr = "1.0.0";

        // Application-level salt — combined with device unique ID at runtime.
        // This means saves are device-bound and cannot be trivially copied between devices.
        private const string AppSalt = "HoE_AppSalt_v1_Eternia2026";

        private readonly string _saveDirectory;
        private readonly string _derivedKey;

        public SaveManager(string saveDirectory)
        {
            _saveDirectory = saveDirectory;
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }
            _derivedKey = BuildDerivedKey();
        }

        /// <summary>
        /// Derives an AES key from the application salt + device unique ID.
        /// Falls back to a test-safe constant when OS.GetUniqueId() is unavailable.
        /// </summary>
        private static string BuildDerivedKey()
        {
            try
            {
                string uniqueId = Godot.OS.GetUniqueId();
                return AppSalt + "::" + uniqueId;
            }
            catch
            {
                // Headless test environments do not expose a device unique ID.
                return AppSalt + "::TEST_DEVICE";
            }
        }

        // ----------------------------------------------------------------
        // Active session profile — held in memory between saves
        // ----------------------------------------------------------------
        private SaveProfile? _activeProfile;

        public SaveProfile GetOrCreateActiveProfile()
        {
            if (_activeProfile == null)
                _activeProfile = new SaveProfile { SaveVersion = CurrentSaveVersion };
            return _activeProfile;
        }

        /// <summary>
        /// Called by GameLoop to update in-memory profile stats before autosave.
        /// </summary>
        public void UpdateSessionStats(int playerLevel, int playerXp, int enemiesKilled, int wavesCompleted)
        {
            var p = GetOrCreateActiveProfile();
            p.PlayerLevel        = playerLevel;
            p.PlayerXp           = playerXp;
            p.EnemiesKilledTotal = enemiesKilled;
            p.WavesCompleted     = wavesCompleted;
            p.Stats.Level        = playerLevel;
            p.Stats.CurrentXp    = playerXp;
            p.StatsData.KillsCount = enemiesKilled;
        }

        /// <summary>
        /// Called by EncounterManager to sync boss battle states.
        /// </summary>
        public void UpdateEncounterStats(
            IEnumerable<string> completed,
            IEnumerable<string> defeated,
            IEnumerable<string> elites,
            IEnumerable<string> claimedRewards)
        {
            var p = GetOrCreateActiveProfile();
            p.CompletedEncounters = new List<string>(completed);
            p.DefeatedBossIds     = new List<string>(defeated);
            p.EncounteredElites    = new List<string>(elites);
            p.ClaimedRewards       = new List<string>(claimedRewards);
        }

        /// <summary>Saves the active in-memory profile to the given slot.</summary>
        public bool Save(int slotId)
        {
            var profile = GetOrCreateActiveProfile();
            return Save(slotId, profile);
        }

        public bool Save(int slotId, SaveProfile profile)
        {
            string savePath = GetSavePath(slotId);
            string backupPath = GetBackupPath(slotId);

            try
            {
                Logger.Info($"SaveManager: Initiating save sequence for slot {slotId}...");
                profile.StatsData.SavesCount++;
                profile.GameVersion = GameVersionStr;
                profile.SaveVersion = CurrentSaveVersion;

                // 1. Serialize profile to JSON
                string jsonString = JsonSerializer.Serialize(profile);
                byte[] rawBytes = Encoding.UTF8.GetBytes(jsonString);

                // 2. Encrypt bytes using AES-256
                byte[] encryptedBytes = Encrypt(rawBytes, _derivedKey);

                // 3. Generate SHA-256 Checksum for integrity validation
                byte[] checksum = GenerateChecksum(encryptedBytes);

                // 4. Create backup of the current save file first if it exists
                if (File.Exists(savePath))
                {
                    File.Copy(savePath, backupPath, true);
                    Logger.Info($"SaveManager: Created backup file for slot {slotId}.");
                }

                // 5. Write final encrypted file: [Encrypted Data] + [SHA-256 Checksum (32 bytes)]
                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(encryptedBytes, 0, encryptedBytes.Length);
                    fs.Write(checksum, 0, checksum.Length);
                }

                Logger.Info($"SaveManager: Save slot {slotId} complete.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"SaveManager: Save sequence failed for slot {slotId}: {ex.Message}");
                return false;
            }
        }

        public SaveProfile? Load(int slotId)
        {
            string savePath = GetSavePath(slotId);
            string backupPath = GetBackupPath(slotId);

            if (!File.Exists(savePath))
            {
                Logger.Warning($"SaveManager: Save slot {slotId} file not found. Trying backup...");
                if (File.Exists(backupPath))
                {
                    return LoadFromFile(backupPath);
                }
                return null;
            }

            var profile = LoadFromFile(savePath);
            if (profile == null && File.Exists(backupPath))
            {
                Logger.Critical($"SaveManager: Save slot {slotId} is corrupted! Attempting recovery from backup...");
                profile = LoadFromFile(backupPath);
                if (profile != null)
                {
                    Logger.Info($"SaveManager: Recovery successful! Restoring backup file to slot {slotId}...");
                    Save(slotId, profile); // Restore backup state
                }
            }

            return profile;
        }

        private SaveProfile? LoadFromFile(string path)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                int checksumSize = 32; // SHA-256 is 32 bytes

                if (fileBytes.Length <= checksumSize)
                {
                    Logger.Error($"SaveManager: Invalid file structure for path '{path}'.");
                    return null;
                }

                int dataLength = fileBytes.Length - checksumSize;
                byte[] encryptedBytes = new byte[dataLength];
                byte[] fileChecksum = new byte[checksumSize];

                Array.Copy(fileBytes, 0, encryptedBytes, 0, dataLength);
                Array.Copy(fileBytes, dataLength, fileChecksum, 0, checksumSize);

                // Verify integrity checksum
                byte[] calculatedChecksum = GenerateChecksum(encryptedBytes);
                if (!CompareChecksums(fileChecksum, calculatedChecksum))
                {
                    Logger.Error($"SaveManager: File integrity check failed for '{path}'. Tampering or corruption detected.");
                    return null;
                }

                // Decrypt data
                byte[] decryptedBytes = Decrypt(encryptedBytes, _derivedKey);
                string jsonString = Encoding.UTF8.GetString(decryptedBytes);

                // Deserialize JSON back to profile model
                var profile = JsonSerializer.Deserialize<SaveProfile>(jsonString);
                
                // Perform data schema migrations if needed
                if (profile != null && profile.SaveVersion < CurrentSaveVersion)
                {
                    MigrateProfile(profile);
                }

                return profile;
            }
            catch (Exception ex)
            {
                Logger.Error($"SaveManager: Load from file failed for '{path}': {ex.Message}");
                return null;
            }
        }

        public SaveMetadata? GetSlotPreview(int slotId)
        {
            var profile = Load(slotId);
            if (profile == null) return null;

            string savePath = GetSavePath(slotId);
            long fileTimestamp = File.GetLastWriteTimeUtc(savePath).ToFileTimeUtc();

            return new SaveMetadata
            {
                SlotId = slotId,
                CharacterName = profile.Stats.CharacterName,
                Level = profile.Stats.Level,
                PlayTimeSeconds = profile.StatsData.PlayTimeSeconds,
                GameVersion = profile.GameVersion,
                Timestamp = fileTimestamp
            };
        }

        private void MigrateProfile(SaveProfile profile)
        {
            Logger.Warning($"SaveManager: Migrating save profile from version {profile.SaveVersion} to {CurrentSaveVersion}...");
            if (profile.SaveVersion < 2)
            {
                profile.EquippedParts = new Dictionary<string, string>();
                profile.BaseAttributes = new Dictionary<string, float>();
                profile.ActiveEffects = new List<string>();
            }
            if (profile.SaveVersion < 3)
            {
                profile.PlayerInventory = new List<Inventory.InventorySlot>();
                profile.EquippedSlots = new Dictionary<string, Inventory.InventorySlot>();
                profile.StorageChests = new Dictionary<string, List<Inventory.InventorySlot>>();
            }
            if (profile.SaveVersion < 4)
            {
                profile.WorldSeed = 12345u;
                profile.DiscoveredRegions = new HashSet<string>();
                profile.ModifiedChunkNodes = new Dictionary<string, List<string>>();
            }
            if (profile.SaveVersion < 5)
            {
                profile.ModifiedDecorations = new Dictionary<string, List<string>>();
                profile.DiscoveredNavRegions = new HashSet<string>();
                profile.PopulatedLandmarks = new Dictionary<string, string>();
            }
            if (profile.SaveVersion < 6)
            {
                profile.NpcStates            = new Dictionary<string, NpcSaveState>();
                profile.ReputationSnapshot   = new Dictionary<string, int>();
                profile.RelationshipSnapshot = new Dictionary<string, float[]>();
            }
            if (profile.SaveVersion < 7)
            {
                profile.UnlockedCombatStyles     = new List<string>();
                profile.LearnedAbilities          = new List<string>();
                profile.TemporaryCombatModifiers  = new Dictionary<string, float>();
                profile.WeaponDurability          = new Dictionary<string, float>();
            }
            if (profile.SaveVersion < 8)
            {
                profile.UnlockedAbilityIds   = new List<string>();
                profile.EquippedAbilitySlots = new string[4];
                profile.PlayerLevel          = profile.Stats.Level;  // promote from Stats
                profile.PlayerXp             = profile.Stats.CurrentXp;
                profile.EnemiesKilledTotal   = profile.StatsData.KillsCount;
                profile.WavesCompleted       = 0;
            }
            if (profile.SaveVersion < 9)
            {
                profile.CompletedEncounters = new List<string>();
                profile.DefeatedBossIds     = new List<string>();
                profile.EncounteredElites    = new List<string>();
                profile.ClaimedRewards       = new List<string>();
            }
            if (profile.SaveVersion < 10)
            {
                profile.UnlockedAbilityIds = profile.LearnedAbilities ?? new List<string>();
                profile.AbilityLevels = new Dictionary<string, int>();
                profile.LoadoutData = new List<Player.Abilities.LoadoutSaveData>();
                profile.ActiveLoadoutIndex = 0;
                profile.AbilityManagerState = null;
                profile.ProgressionData = new Player.Progression.ProgressionSaveData
                {
                    Level = profile.PlayerLevel,
                    Experience = profile.PlayerXp,
                    PrestigeLevel = 0,
                    Version = 1
                };
            }
            if (profile.SaveVersion < 11)
            {
                profile.EquipmentData = new Equipment.Save.EquipmentSaveData();
            }
            if (profile.SaveVersion < 12)
            {
                profile.ProfessionStates = new List<Gathering.ProfessionSaveState>();
                profile.ResourceNodeStates = new List<Gathering.ResourceNodeState>();
                profile.KnownRecipeIds = new List<string>();
                profile.CraftQueueItems = new List<Crafting.CraftQueueItem>();
            }
            profile.SaveVersion = CurrentSaveVersion;
        }

        private string GetSavePath(int slotId)
        {
            return Path.Combine(_saveDirectory, $"slot_{slotId}.sav");
        }

        private string GetBackupPath(int slotId)
        {
            return Path.Combine(_saveDirectory, $"slot_{slotId}.bak");
        }

        // ==========================================================
        // CRYPTOGRAPHY UTILITIES
        // ==========================================================

        private byte[] Encrypt(byte[] rawData, string password)
        {
            byte[] salt = { 0x45, 0x74, 0x65, 0x72, 0x6e, 0x69, 0x61, 0x53 }; // "EterniaS"
            using (var keyDerivation = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA256))
            {
                byte[] key = keyDerivation.GetBytes(32); // 256 bits
                byte[] iv = keyDerivation.GetBytes(16);  // 128 bits

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(rawData, 0, rawData.Length);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
        }

        private byte[] Decrypt(byte[] encryptedData, string password)
        {
            byte[] salt = { 0x45, 0x74, 0x65, 0x72, 0x6e, 0x69, 0x61, 0x53 };
            using (var keyDerivation = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA256))
            {
                byte[] key = keyDerivation.GetBytes(32);
                byte[] iv = keyDerivation.GetBytes(16);

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedData, 0, encryptedData.Length);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
        }

        private byte[] GenerateChecksum(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        private bool CompareChecksums(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }
}
