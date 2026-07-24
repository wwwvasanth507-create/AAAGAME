using System.Collections.Generic;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// All numeric and string stats for a player character.
    /// Extensible — add new stats here; serialisation via SaveProfile picks them up automatically.
    /// </summary>
    public class PlayerData
    {
        // ---------------------------------------------------------------
        // IDENTITY
        // ---------------------------------------------------------------
        public string Name          { get; set; } = "Hero";
        public int    Level         { get; set; } = 1;
        public int    CurrentXp     { get; set; } = 0;
        public int    XpToNextLevel { get; set; } = 100;

        // ---------------------------------------------------------------
        // VITALS
        // ---------------------------------------------------------------
        public float MaxHealth      { get; set; } = 100f;
        public float CurrentHealth  { get; set; } = 100f;
        public float MaxMana        { get; set; } = 50f;
        public float CurrentMana    { get; set; } = 50f;
        public float MaxStamina     { get; set; } = 100f;
        public float CurrentStamina { get; set; } = 100f;

        // Regeneration rates (per second)
        public float HealthRegen    { get; set; } = 0f;
        public float ManaRegen      { get; set; } = 1.5f;
        public float StaminaRegen   { get; set; } = 8f;

        // ---------------------------------------------------------------
        // PRIMARY STATS
        // ---------------------------------------------------------------
        public int Strength         { get; set; } = 10;
        public int Defense          { get; set; } = 5;
        public int Magic            { get; set; } = 5;
        public int Speed            { get; set; } = 10;
        public int Luck             { get; set; } = 5;
        public int Dexterity        { get; set; } = 5;
        public int Endurance        { get; set; } = 5;
        public int Intelligence     { get; set; } = 5;
        public int Charisma         { get; set; } = 5;

        // ---------------------------------------------------------------
        // MOVEMENT PARAMETERS (derived from stats but overrideable)
        // ---------------------------------------------------------------
        public float WalkSpeed      { get; set; } = 3.5f;
        public float RunSpeed       { get; set; } = 6.0f;
        public float SprintSpeed    { get; set; } = 9.5f;
        public float JumpVelocity   { get; set; } = 5.5f;
        public float CrouchSpeed    { get; set; } = 2.0f;
        public float RollSpeed      { get; set; } = 8.0f;

        // ---------------------------------------------------------------
        // STAMINA COSTS
        // ---------------------------------------------------------------
        public float SprintStaminaCost { get; set; } = 12f;  // per second
        public float JumpStaminaCost   { get; set; } = 8f;
        public float RollStaminaCost   { get; set; } = 20f;

        // ---------------------------------------------------------------
        // FUTURE EXTENSION (DLC / future phases)
        // ---------------------------------------------------------------
        public Dictionary<string, float> CustomStats { get; set; } = new();

        // ---------------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------------

        /// <summary>True when the character has stamina for the given cost.</summary>
        public bool HasStamina(float cost) => CurrentStamina >= cost;

        /// <summary>Drains stamina by the given amount, clamped to 0.</summary>
        public void DrainStamina(float amount)
        {
            CurrentStamina = System.Math.Max(0f, CurrentStamina - amount);
        }

        /// <summary>Regenerates all vitals by their per-second rates * deltaTime.</summary>
        public void RegenVitals(float deltaTime)
        {
            CurrentHealth  = System.Math.Min(MaxHealth,  CurrentHealth  + HealthRegen  * deltaTime);
            CurrentMana    = System.Math.Min(MaxMana,    CurrentMana    + ManaRegen    * deltaTime);
            CurrentStamina = System.Math.Min(MaxStamina, CurrentStamina + StaminaRegen * deltaTime);
        }

        /// <summary>Grants experience and returns true if level-up occurred.</summary>
        public bool AddXp(int amount)
        {
            CurrentXp += amount;
            if (CurrentXp >= XpToNextLevel)
            {
                CurrentXp    -= XpToNextLevel;
                Level         += 1;
                XpToNextLevel  = CalculateXpThreshold(Level);
                return true;
            }
            return false;
        }

        private static int CalculateXpThreshold(int level) =>
            100 + (level - 1) * 50 + (level * level * 5);
    }
}
