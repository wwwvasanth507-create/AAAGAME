using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public int SaveVersion { get; set; } = 1;
        public string GameVersion { get; set; } = "1.0.0";
        public PlayerStats Stats { get; set; } = new PlayerStats();
        public InventoryData Inventory { get; set; } = new InventoryData();
        public QuestData Quests { get; set; } = new QuestData();
        public WorldState World { get; set; } = new WorldState();
        public Statistics StatsData { get; set; } = new Statistics();
        
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
        private const int CurrentSaveVersion = 1;
        private const string GameVersionStr = "1.0.0";
        private const string DefaultPassword = "EterniaSuperSecretKey2026!";
        private readonly string _saveDirectory;

        public SaveManager(string saveDirectory)
        {
            _saveDirectory = saveDirectory;
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }
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
                byte[] encryptedBytes = Encrypt(rawBytes, DefaultPassword);

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
                byte[] decryptedBytes = Decrypt(encryptedBytes, DefaultPassword);
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
            // Add custom schema logic transitions here
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
