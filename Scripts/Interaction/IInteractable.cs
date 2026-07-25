using Godot;

namespace HeroOfEternia.Interaction
{
    /// <summary>
    /// Type of interaction trigger.
    /// </summary>
    public enum InteractionType
    {
        Tap,
        Hold,
        Auto
    }

    /// <summary>
    /// Standard interface implemented by any interactable object in the world.
    /// Supports distance limitations, tap/hold mechanics, and highlight overlays.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>The prompt displayed on the player's UI (e.g. "Open Chest", "Talk").</summary>
        string InteractionPrompt { get; }

        /// <summary>Maximum distance the player can be from the object to trigger interaction.</summary>
        float InteractionDistance { get; }

        /// <summary>Whether this object triggers immediately, requires a hold, or activates automatically.</summary>
        InteractionType Type { get; }

        /// <summary>Total seconds required to hold for triggering (only relevant if Type is Hold).</summary>
        float HoldDuration { get; }

        /// <summary>Triggered when the interaction starts or finishes successfully.</summary>
        void OnInteract(Player.PlayerRoot player);

        /// <summary>Called when the player begins holding the interaction key.</summary>
        void OnInteractionStart(Player.PlayerRoot player);

        /// <summary>Called when the player stops holding (either completed or aborted).</summary>
        void OnInteractionEnd(Player.PlayerRoot player, bool completed);

        /// <summary>Toggles a visual highlight or outline on the object.</summary>
        void SetHighlight(bool highlighted);

        /// <summary>Gets the global position of the interactable object for distance checks.</summary>
        Vector3 GetGlobalPosition();
    }
}
