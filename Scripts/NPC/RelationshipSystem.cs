using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.NPC
{
    /// <summary>
    /// Encapsulates all relational dimensions between two NPCs (or player↔NPC).
    /// All values are clamped to –100..+100.
    /// </summary>
    public class NpcRelationship
    {
        public float Friendship { get; private set; }
        public float Trust { get; private set; }
        public float Respect { get; private set; }
        public float Fear { get; private set; }
        public string FactionStanding { get; set; } = "neutral";
        public bool IsFamilyLink { get; set; } = false;
        public bool IsRival { get; set; } = false;

        public void AdjustFriendship(float delta) => Friendship = Math.Clamp(Friendship + delta, -100f, 100f);
        public void AdjustTrust(float delta)      => Trust      = Math.Clamp(Trust + delta,      -100f, 100f);
        public void AdjustRespect(float delta)    => Respect    = Math.Clamp(Respect + delta,    -100f, 100f);
        public void AdjustFear(float delta)       => Fear       = Math.Clamp(Fear + delta,       -100f, 100f);

        /// <summary>Returns a simple –100..+100 aggregate score.</summary>
        public float AggregateScore() => (Friendship + Trust + Respect - Fear) / 4f;
    }

    /// <summary>
    /// Central service managing all NPC–NPC and Player–NPC relationship records.
    /// Keyed by a canonical pair string: "{idLower}_{idHigher}" (alphabetical order).
    /// </summary>
    public class RelationshipSystem
    {
        private readonly Dictionary<string, NpcRelationship> _relationships = new();

        private static string MakeKey(string a, string b)
        {
            return string.Compare(a, b, StringComparison.Ordinal) <= 0
                ? $"{a}_{b}"
                : $"{b}_{a}";
        }

        /// <summary>
        /// Returns existing relationship or creates a neutral one.
        /// </summary>
        public NpcRelationship GetOrCreate(string idA, string idB)
        {
            string key = MakeKey(idA, idB);
            if (!_relationships.TryGetValue(key, out var rel))
            {
                rel = new NpcRelationship();
                _relationships[key] = rel;
            }
            return rel;
        }

        public NpcRelationship? Get(string idA, string idB)
        {
            string key = MakeKey(idA, idB);
            return _relationships.TryGetValue(key, out var rel) ? rel : null;
        }

        public void AdjustFriendship(string idA, string idB, float delta) =>
            GetOrCreate(idA, idB).AdjustFriendship(delta);

        public void AdjustTrust(string idA, string idB, float delta) =>
            GetOrCreate(idA, idB).AdjustTrust(delta);

        public void AdjustRespect(string idA, string idB, float delta) =>
            GetOrCreate(idA, idB).AdjustRespect(delta);

        public void AdjustFear(string idA, string idB, float delta) =>
            GetOrCreate(idA, idB).AdjustFear(delta);

        public void SetFamilyLink(string idA, string idB, bool value) =>
            GetOrCreate(idA, idB).IsFamilyLink = value;

        public void SetRivalry(string idA, string idB, bool value) =>
            GetOrCreate(idA, idB).IsRival = value;

        /// <summary>
        /// Returns a float[] snapshot [Friendship, Trust, Respect, Fear] for Save V6.
        /// </summary>
        public float[] GetSnapshot(string idA, string idB)
        {
            var rel = Get(idA, idB);
            if (rel == null) return new float[] { 0f, 0f, 0f, 0f };
            return new float[] { rel.Friendship, rel.Trust, rel.Respect, rel.Fear };
        }

        /// <summary>
        /// Restores a saved float[] snapshot.
        /// </summary>
        public void RestoreSnapshot(string idA, string idB, float[] snapshot)
        {
            if (snapshot == null || snapshot.Length < 4) return;
            var rel = GetOrCreate(idA, idB);
            rel.AdjustFriendship(snapshot[0] - rel.Friendship);
            rel.AdjustTrust(snapshot[1] - rel.Trust);
            rel.AdjustRespect(snapshot[2] - rel.Respect);
            rel.AdjustFear(snapshot[3] - rel.Fear);
        }

        public int Count => _relationships.Count;
    }
}
