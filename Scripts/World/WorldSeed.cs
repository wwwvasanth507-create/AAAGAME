using System;
using System.Text.RegularExpressions;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Utility class for procedural seeds.
    /// Supports 32-bit and 64-bit seed values, FNV-1a hashing of text inputs,
    /// validation, and sharing string formatting.
    /// </summary>
    public static class WorldSeed
    {
        private static readonly Random _rng = new();
        private static readonly Regex _seedRegex = new(@"^[a-zA-Z0-9_\-\s]+$");

        /// <summary>
        /// Generates a completely random 64-bit seed.
        /// </summary>
        public static ulong GenerateRandom()
        {
            byte[] bytes = new byte[8];
            _rng.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        /// <summary>
        /// Parses and validates a string seed. 
        /// If it's a numeric string, parses it directly.
        /// If it's alphanumeric text, hashes it deterministically using 64-bit FNV-1a.
        /// </summary>
        public static ulong Parse(string seedInput)
        {
            if (string.IsNullOrEmpty(seedInput))
            {
                return GenerateRandom();
            }

            string trimmed = seedInput.Trim();

            // Try direct numerical parse
            if (ulong.TryParse(trimmed, out ulong numericSeed))
            {
                return numericSeed;
            }

            // Fallback: Hash text using FNV-1a 64-bit
            return ComputeFnv1a64(trimmed);
        }

        /// <summary>
        /// Validates if seed input fits alphanumeric and basic spacer formats.
        /// </summary>
        public static bool Validate(string seedInput)
        {
            if (string.IsNullOrEmpty(seedInput)) return false;
            return _seedRegex.IsMatch(seedInput);
        }

        /// <summary>
        /// Format seed as shareable hex string.
        /// </summary>
        public static string ToShareString(ulong seed)
        {
            return seed.ToString("X16"); // 16-character uppercase Hex
        }

        /// <summary>
        /// Restores seed value from shareable hex format.
        /// </summary>
        public static bool TryParseShareString(string hexString, out ulong seed)
        {
            seed = 0;
            if (string.IsNullOrEmpty(hexString) || hexString.Length != 16) return false;
            return ulong.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out seed);
        }

        private static ulong ComputeFnv1a64(string text)
        {
            const ulong fnvPrime = 0x00000100000001B3;
            const ulong fnvOffsetBasis = 0xCBF29CE484222325;

            ulong hash = fnvOffsetBasis;
            foreach (char c in text)
            {
                hash ^= (byte)(c & 0xFF);
                hash *= fnvPrime;
                hash ^= (byte)((c >> 8) & 0xFF);
                hash *= fnvPrime;
            }
            return hash;
        }
    }
}
