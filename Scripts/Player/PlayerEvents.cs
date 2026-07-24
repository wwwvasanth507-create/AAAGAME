namespace HeroOfEternia.Player
{
    /// <summary>Published when the player character dies.</summary>
    public class PlayerDiedEvent
    {
        public PlayerRoot? Player { get; set; }
    }

    /// <summary>Published every time the player's state machine transitions.</summary>
    public class PlayerStateChangedEvent
    {
        public PlayerStateId NewState { get; set; }
    }

    /// <summary>Published when the player takes damage.</summary>
    public class PlayerDamagedEvent
    {
        public float Amount    { get; set; }
        public float Remaining { get; set; }
    }

    /// <summary>Published when the player levels up.</summary>
    public class PlayerLevelUpEvent
    {
        public int NewLevel { get; set; }
    }
}
