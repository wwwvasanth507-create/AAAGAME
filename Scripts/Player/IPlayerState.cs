namespace HeroOfEternia.Player
{
    /// <summary>
    /// Enumerates all player states.
    /// Add new states here — the state machine requires no other changes.
    /// </summary>
    public enum PlayerStateId
    {
        Idle,
        Walking,
        Running,
        Sprinting,
        Jumping,
        Falling,
        Landing,
        Rolling,
        Crouching,
        Swimming,
        Climbing,
        TurnLeft,
        TurnRight,
        LookingAround,
        Pushing,
        Pulling,
        Interacting,
        Sleeping,
        Sitting,
        Celebrating,
        Dead,
        Respawn,
        Frozen,
        Disabled,

        // ── Combat States (Prompt 10) ─────────────────────────────────
        Attack,        // Light attack swing
        HeavyAttack,   // Charged / heavy attack swing
        Casting,       // Casting a spell / ranged attack
        Blocking,      // Active block (shield / parry window)
        Parrying,      // Precise parry input (brief window)
        HitReaction,   // Flinch from being hit
        Knockdown,     // Knocked to the ground
        Recovery,      // Rising from knockdown before Idle
    }

    /// <summary>
    /// All states implement this interface.
    /// Enter/Update/Exit are guaranteed to be called by the state machine.
    /// </summary>
    public interface IPlayerState
    {
        PlayerStateId Id { get; }

        /// <summary>Called once when this state becomes active.</summary>
        void Enter(PlayerRoot player);

        /// <summary>Called every physics frame while this state is active.</summary>
        /// <returns>The state to transition to, or null to stay in current state.</returns>
        PlayerStateId? Update(PlayerRoot player, double delta);

        /// <summary>Called once when leaving this state.</summary>
        void Exit(PlayerRoot player);
    }
}
