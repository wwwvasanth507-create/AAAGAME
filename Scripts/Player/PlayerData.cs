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

        public Stats.PlayerAttributeSet Attributes { get; } = new();

        // ---------------------------------------------------------------
        // VITALS
        // ---------------------------------------------------------------
        public float MaxHealth      { get => Attributes.GetValue(Stats.AttributeType.Health); set => Attributes.SetBaseValue(Stats.AttributeType.Health, value); }
        public float CurrentHealth  { get; set; } = 100f;
        public float MaxMana        { get => Attributes.GetValue(Stats.AttributeType.Mana); set => Attributes.SetBaseValue(Stats.AttributeType.Mana, value); }
        public float CurrentMana    { get; set; } = 50f;
        public float MaxStamina     { get => Attributes.GetValue(Stats.AttributeType.Stamina); set => Attributes.SetBaseValue(Stats.AttributeType.Stamina, value); }
        public float CurrentStamina { get; set; } = 100f;

        // Regeneration rates (per second)
        public float HealthRegen    { get; set; } = 0f;
        public float ManaRegen      { get; set; } = 1.5f;
        public float StaminaRegen   { get; set; } = 8f;

        // ---------------------------------------------------------------
        // PRIMARY STATS
        // ---------------------------------------------------------------
        public int Strength         { get => (int)Attributes.GetValue(Stats.AttributeType.Strength); set => Attributes.SetBaseValue(Stats.AttributeType.Strength, value); }
        public int Defense          { get => (int)Attributes.GetValue(Stats.AttributeType.Defense); set => Attributes.SetBaseValue(Stats.AttributeType.Defense, value); }
        public int Magic            { get => (int)Attributes.GetValue(Stats.AttributeType.Magic); set => Attributes.SetBaseValue(Stats.AttributeType.Magic, value); }
        public int Speed            { get => (int)Attributes.GetValue(Stats.AttributeType.Speed); set => Attributes.SetBaseValue(Stats.AttributeType.Speed, value); }
        public int Luck             { get => (int)Attributes.GetValue(Stats.AttributeType.Luck); set => Attributes.SetBaseValue(Stats.AttributeType.Luck, value); }
        public int Dexterity        { get => (int)Attributes.GetValue(Stats.AttributeType.Dexterity); set => Attributes.SetBaseValue(Stats.AttributeType.Dexterity, value); }
        public int Endurance        { get; set; } = 5;
        public int Intelligence     { get; set; } = 5;
        public int Charisma         { get; set; } = 5;

        // ---------------------------------------------------------------
        // MOVEMENT PARAMETERS (derived from stats but overrideable)
        // ---------------------------------------------------------------
        private float _baseWalkSpeed = 3.5f;
        public float WalkSpeed      { get => _baseWalkSpeed * (1.0f + (Attributes.GetValue(Stats.AttributeType.Speed) - 10f) * 0.02f); set => _baseWalkSpeed = value; }
        
        private float _baseRunSpeed = 6.0f;
        public float RunSpeed       { get => _baseRunSpeed * (1.0f + (Attributes.GetValue(Stats.AttributeType.Speed) - 10f) * 0.02f); set => _baseRunSpeed = value; }
        
        private float _baseSprintSpeed = 9.5f;
        public float SprintSpeed    { get => _baseSprintSpeed * (1.0f + (Attributes.GetValue(Stats.AttributeType.Speed) - 10f) * 0.02f); set => _baseSprintSpeed = value; }
        
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
