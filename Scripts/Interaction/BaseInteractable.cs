using Godot;
using System;

namespace HeroOfEternia.Interaction
{
    /// <summary>
    /// Base class for all interactable objects in the game world.
    /// Inherits from Area3D to support physical detection zones.
    /// Derived objects (Door, Chest, lever, button) override methods for custom logic.
    /// </summary>
    public partial class BaseInteractable : Area3D, IInteractable
    {
        [Export]
        public string PromptText { get; set; } = "Interact";

        [Export]
        public float MaxDistance { get; set; } = 3.5f;

        [Export]
        public InteractionType InterType { get; set; } = InteractionType.Tap;

        [Export]
        public float DurationToHold { get; set; } = 1.0f;

        // Visual highlighting references
        protected MeshInstance3D? VisualMesh;
        private ShaderMaterial? _highlightMaterial;
        private Material? _originalMaterialOverride;

        public virtual string InteractionPrompt => PromptText;
        public virtual float InteractionDistance => MaxDistance;
        public virtual InteractionType Type => InterType;
        public virtual float HoldDuration => DurationToHold;

        public override void _Ready()
        {
            // Try to auto-detect a MeshInstance3D child for highlighting
            VisualMesh = FindChildMesh(this);
            
            // Set up collision flags for the interaction zone
            CollisionLayer = 0;
            SetCollisionLayerValue(4, true); // Layer 4 = Interactables
            CollisionMask = 0;

            // Prepare highlight shader material
            _highlightMaterial = new ShaderMaterial();
            // A simple outline/glow shader
            var shader = new Shader();
            shader.Code = @"
                shader_type spatial;
                render_mode unshaded;
                void fragment() {
                    ALBEDO = vec3(0.0, 1.0, 1.0); // Neon Cyan glow
                }
            ";
            _highlightMaterial.Shader = shader;
        }

        public virtual void OnInteract(Player.PlayerRoot player)
        {
            Core.Logger.Info($"BaseInteractable: Interacted with {Name}!");
        }

        public virtual void OnInteractionStart(Player.PlayerRoot player)
        {
            Core.Logger.Info($"BaseInteractable: Interaction started with {Name}.");
        }

        public virtual void OnInteractionEnd(Player.PlayerRoot player, bool completed)
        {
            Core.Logger.Info($"BaseInteractable: Interaction ended with {Name}. Completed={completed}");
        }

        public virtual void SetHighlight(bool highlighted)
        {
            if (VisualMesh == null) return;

            if (highlighted)
            {
                if (VisualMesh.MaterialOverride != _highlightMaterial)
                {
                    _originalMaterialOverride = VisualMesh.MaterialOverride;
                    VisualMesh.MaterialOverride = _highlightMaterial;
                }
            }
            else
            {
                if (VisualMesh.MaterialOverride == _highlightMaterial)
                {
                    VisualMesh.MaterialOverride = _originalMaterialOverride;
                }
            }
        }

        public new Vector3 GetGlobalPosition()
        {
            return GlobalPosition;
        }

        private MeshInstance3D? FindChildMesh(Node node)
        {
            if (node is MeshInstance3D mesh) return mesh;

            foreach (var child in node.GetChildren())
            {
                var found = FindChildMesh(child);
                if (found != null) return found;
            }
            return null;
        }
    }
}
