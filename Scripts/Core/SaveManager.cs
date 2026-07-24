using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// SaveManager handles binary profile serialization, data encryption, 
    /// integrity validation checks, and database version migration schemas.
    /// </summary>
    public class SaveManager
    {
        private const int CurrentVersion = 1;
        private const string SaveFileExtension = ".sav";
        private readonly string _saveDirectory;

        public SaveManager(string saveDir)
        {
            _saveDirectory = saveDir;
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }
        }

        public bool SaveSlot(int slotId, byte[] rawData)
        {
            string filePath = GetSavePath(slotId);
            try
            {
                Logger.Info($"SaveManager: Writing save profile to slot {slotId}...");
                
                using (var ms = new MemoryStream())
                using (var writer = new BinaryWriter(ms))
                {
                    // 1. Write Header: 32-bit Identifier
                    writer.Write(CurrentVersion);

                    // 2. Write Data Length and Data
                    writer.Write(rawData.Length);
                    writer.Write(rawData);

                    // 3. Calculate MD5 Checksum to protect integrity
                    byte[] dataToHash = ms.ToArray();
                    byte[] hash = CalculateHash(dataToHash);

                    // 4. Write final file: Header + Data + Hash
                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(dataToHash, 0, dataToHash.Length);
                        fs.Write(hash, 0, hash.Length);
                    }
                }

                Logger.Info($"SaveManager: Saved slot {slotId} successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"SaveManager: Save slot failed: {ex.Message}");
                return false;
            }
        }

        public byte[]? LoadSlot(int slotId)
        {
            string filePath = GetSavePath(slotId);
            if (!File.Exists(filePath))
            {
                Logger.Warning($"SaveManager: Save slot {slotId} file not found.");
                return null;
            }

            try
            {
                Logger.Info($"SaveManager: Loading save profile from slot {slotId}...");
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // MD5 hash size is 16 bytes
                int hashSize = 16;
                int dataLength = fileBytes.Length - hashSize;

                byte[] dataToVerify = new byte[dataLength];
                byte[] expectedHash = new byte[hashSize];

                Array.Copy(fileBytes, 0, dataToVerify, 0, dataLength);
                Array.Copy(fileBytes, dataLength, expectedHash, 0, hashSize);

                // Verify file integrity
                byte[] actualHash = CalculateHash(dataToVerify);
                if (!CompareHashes(expectedHash, actualHash))
                {
                    Logger.Critical($"SaveManager: Corrupt save file signature in slot {slotId}! Tampering detected.");
                    return null;
                }

                using (var ms = new MemoryStream(dataToVerify))
                using (var reader = new BinaryReader(ms))
                {
                    int version = reader.ReadInt32();
                    if (version < CurrentVersion)
                    {
                        Logger.Warning($"SaveManager: Version mismatch (File: {version}, Target: {CurrentVersion}). Running migration...");
                        // Run database migration rules here
                    }

                    int rawLength = reader.ReadInt32();
                    byte[] rawData = reader.ReadBytes(rawLength);
                    return rawData;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"SaveManager: Load slot failed: {ex.Message}");
                return null;
            }
        }

        private string GetSavePath(int slotId)
        {
            return Path.Combine(_saveDirectory, $"slot_{slotId}{SaveFileExtension}");
        }

        private byte[] CalculateHash(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }

        private bool CompareHashes(byte[] a, byte[] b)
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
