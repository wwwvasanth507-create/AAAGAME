using Godot;
using System;
using System.Collections.Generic;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// Identifies the swappable visual slots on the player character.
    /// </summary>
    public enum PartCategory
    {
        Hair,
        Face,
        Eyes,
        Body,
        Hands,
        Feet,
        Armor,
        Helmet,
        Cape,
        Weapon,
        Accessory
    }

    /// <summary>
    /// Defines character gender configuration.
    /// </summary>
    public enum CharacterGender
    {
        Male,
        Female
    }

    /// <summary>
    /// PlayerModelController manages swappable body parts, weapons, armor pieces, and accessories.
    /// Supports Level of Detail (LOD), material overrides, and customization tints.
    /// Generates fallback primitive shapes if asset resources are not found, ensuring headless test safety.
    /// </summary>
    public partial class PlayerModelController : Node3D
    {
        private readonly Dictionary<PartCategory, Node3D> _activeParts = new();
        private readonly Dictionary<PartCategory, string> _partPaths = new();
        private readonly Dictionary<PartCategory, Material> _materialOverrides = new();
        private readonly Dictionary<PartCategory, Color> _colorTints = new();

        [Export]
        public CharacterGender Gender { get; private set; } = CharacterGender.Male;

        [Export]
        public int CurrentLOD { get; private set; } = 0; // 0 = High (LOD0), 1 = Medium (LOD1), 2 = Low (LOD2)

        /// <summary>
        /// Swaps out a mesh or node configuration in a specific slot.
        /// </summary>
        public void SwapPart(PartCategory category, string partResourcePath)
        {
            RemovePart(category);

            _partPaths[category] = partResourcePath;
            Node3D partNode;

            if (string.IsNullOrEmpty(partResourcePath) || !ResourceLoader.Exists(partResourcePath))
            {
                // Fallback to generating a simple primitive shape
                partNode = CreateFallbackMesh(category);
                Core.Logger.Warning($"PlayerModelController: Resource path '{partResourcePath}' not found for slot '{category}'. Fallback primitive created.");
            }
            else
            {
                try
                {
                    var resource = ResourceLoader.Load(partResourcePath);
                    if (resource is PackedScene scene)
                    {
                        var instance = scene.Instantiate();
                        partNode = instance as Node3D ?? new Node3D();
                        if (instance != partNode)
                        {
                            instance.QueueFree();
                        }
                    }
                    else if (resource is Mesh mesh)
                    {
                        var meshInstance = new MeshInstance3D { Mesh = mesh };
                        partNode = meshInstance;
                    }
                    else
                    {
                        partNode = CreateFallbackMesh(category);
                        Core.Logger.Warning($"PlayerModelController: Unsupported resource type for '{partResourcePath}'. Fallback primitive created.");
                    }
                }
                catch (Exception ex)
                {
                    partNode = CreateFallbackMesh(category);
                    Core.Logger.Error($"PlayerModelController: Failed to load part '{partResourcePath}': {ex.Message}. Fallback primitive created.");
                }
            }

            partNode.Name = $"Part_{category}";
            AddChild(partNode);
            _activeParts[category] = partNode;

            // Apply existing material overrides
            if (_materialOverrides.TryGetValue(category, out var mat))
            {
                ApplyMaterialToNode(partNode, mat);
            }
            else if (_colorTints.TryGetValue(category, out var color))
            {
                ApplyColorTintToNode(partNode, color);
            }

            // Apply current LOD rules to the newly loaded part
            ApplyLODToPart(category, partNode, CurrentLOD);
        }

        /// <summary>
        /// Removes the mesh/accessory in the designated slot.
        /// </summary>
        public void RemovePart(PartCategory category)
        {
            if (_activeParts.TryGetValue(category, out var partNode))
            {
                partNode.QueueFree();
                _activeParts.Remove(category);
            }
            _partPaths.Remove(category);
        }

        /// <summary>
        /// Sets the base rig gender and forces reloading of base body parts if necessary.
        /// </summary>
        public void SetGender(CharacterGender gender)
        {
            if (Gender == gender) return;
            Gender = gender;
            Core.Logger.Info($"PlayerModelController: Gender set to {gender}.");
            // In a production asset flow, this would trigger reloading the base skeleton and skin meshes.
        }

        /// <summary>
        /// Sets the Level of Detail (LOD) level: 0 (High), 1 (Medium), 2 (Low).
        /// </summary>
        public void SetLOD(int lodLevel)
        {
            CurrentLOD = Mathf.Clamp(lodLevel, 0, 2);
            Core.Logger.Info($"PlayerModelController: Setting LOD to {CurrentLOD}.");

            foreach (var kvp in _activeParts)
            {
                ApplyLODToPart(kvp.Key, kvp.Value, CurrentLOD);
            }
        }

        /// <summary>
        /// Applies a custom material override to a specific part slot.
        /// </summary>
        public void SetPartMaterial(PartCategory category, Material material)
        {
            _materialOverrides[category] = material;
            if (_activeParts.TryGetValue(category, out var partNode))
            {
                ApplyMaterialToNode(partNode, material);
            }
        }

        /// <summary>
        /// Colorizes a specific part slot (e.g. hair tinting, armor dye).
        /// </summary>
        public void SetPartColor(PartCategory category, Color color)
        {
            _colorTints[category] = color;
            if (_activeParts.TryGetValue(category, out var partNode))
            {
                ApplyColorTintToNode(partNode, color);
            }
        }

        /// <summary>
        /// Returns the loaded resource path for a given part slot, or empty string.
        /// </summary>
        public string GetPartPath(PartCategory category)
        {
            return _partPaths.TryGetValue(category, out var path) ? path : string.Empty;
        }

        /// <summary>
        /// Clears all slots.
        /// </summary>
        public void ClearAllParts()
        {
            foreach (var category in Enum.GetValues<PartCategory>())
            {
                RemovePart(category);
            }
            _materialOverrides.Clear();
            _colorTints.Clear();
        }

        // ---------------------------------------------------------------
        // PRIVATE HELPERS
        // ---------------------------------------------------------------

        private Node3D CreateFallbackMesh(PartCategory category)
        {
            var instance = new MeshInstance3D();
            
            // Set up placeholder shapes depending on category
            switch (category)
            {
                case PartCategory.Hair:
                    instance.Mesh = new SphereMesh { Radius = 0.25f, Height = 0.5f };
                    break;
                case PartCategory.Face:
                    instance.Mesh = new BoxMesh { Size = new Vector3(0.2f, 0.2f, 0.1f) };
                    break;
                case PartCategory.Eyes:
                    instance.Mesh = new BoxMesh { Size = new Vector3(0.15f, 0.05f, 0.05f) };
                    break;
                case PartCategory.Body:
                    instance.Mesh = new CapsuleMesh { Radius = 0.4f, Height = 1.2f };
                    break;
                case PartCategory.Hands:
                    instance.Mesh = new BoxMesh { Size = new Vector3(0.1f, 0.1f, 0.2f) };
                    break;
                case PartCategory.Feet:
                    instance.Mesh = new BoxMesh { Size = new Vector3(0.15f, 0.1f, 0.3f) };
                    break;
                case PartCategory.Weapon:
                    instance.Mesh = new BoxMesh { Size = new Vector3(0.05f, 1.2f, 0.05f) }; // sword placeholder
                    break;
                case PartCategory.Helmet:
                    instance.Mesh = new CylinderMesh { TopRadius = 0.25f, BottomRadius = 0.25f, Height = 0.3f };
                    break;
                default:
                    instance.Mesh = new BoxMesh { Size = new Vector3(0.2f, 0.2f, 0.2f) };
                    break;
            }

            // Create a default color-coded material so placeholders look distinct
            var mat = new StandardMaterial3D();
            mat.AlbedoColor = GetCategoryColor(category);
            instance.MaterialOverride = mat;

            return instance;
        }

        private static Color GetCategoryColor(PartCategory category) => category switch
        {
            PartCategory.Hair => new Color(0.8f, 0.5f, 0.2f),       // Brown
            PartCategory.Face => new Color(0.9f, 0.7f, 0.6f),       // Skin
            PartCategory.Eyes => new Color(0.2f, 0.6f, 0.9f),       // Blue
            PartCategory.Body => new Color(0.5f, 0.5f, 0.5f),       // Grey
            PartCategory.Hands => new Color(0.9f, 0.7f, 0.6f),
            PartCategory.Feet => new Color(0.3f, 0.2f, 0.1f),       // Leather brown
            PartCategory.Armor => new Color(0.7f, 0.7f, 0.8f),      // Steel
            PartCategory.Helmet => new Color(0.8f, 0.8f, 0.0f),     // Gold
            PartCategory.Cape => new Color(0.8f, 0.1f, 0.1f),       // Red
            PartCategory.Weapon => new Color(0.4f, 0.9f, 0.4f),     // Green Glow
            _ => new Color(1f, 1f, 1f)
        };

        private void ApplyMaterialToNode(Node node, Material material)
        {
            if (node is MeshInstance3D meshInstance)
            {
                meshInstance.MaterialOverride = material;
            }

            foreach (var child in node.GetChildren())
            {
                ApplyMaterialToNode(child, material);
            }
        }

        private void ApplyColorTintToNode(Node node, Color color)
        {
            if (node is MeshInstance3D meshInstance)
            {
                var mat = meshInstance.MaterialOverride as StandardMaterial3D ?? new StandardMaterial3D();
                mat.AlbedoColor = color;
                meshInstance.MaterialOverride = mat;
            }

            foreach (var child in node.GetChildren())
            {
                ApplyColorTintToNode(child, color);
            }
        }

        private void ApplyLODToPart(PartCategory category, Node3D partNode, int lod)
        {
            // Adjust visual quality or toggle components based on LOD level.
            // In a production mesh pipeline, LOD nodes are grouped under a DistanceLOD node or renamed submeshes.
            // For this framework, we simulate LOD by toggling optional aesthetic elements (e.g. Cape or Hair details)
            // or reducing shadow casting flags.
            
            bool castShadows = lod switch
            {
                0 => true,  // Full shadows
                1 => true,  // Full shadows
                _ => false  // Disable shadows at low LOD (LOD2) to boost mobile performance
            };

            SetShadowCastingRecursive(partNode, castShadows);

            // LOD2 (very low) can hide fine detail features like Cape, Eyes, Hair extensions if we want optimization.
            if (lod >= 2 && (category == PartCategory.Eyes || category == PartCategory.Accessory))
            {
                partNode.Visible = false;
            }
            else
            {
                partNode.Visible = true;
            }
        }

        private void SetShadowCastingRecursive(Node node, bool enabled)
        {
            if (node is GeometryInstance3D geom)
            {
                geom.CastShadow = enabled ? GeometryInstance3D.ShadowCastingSetting.On : GeometryInstance3D.ShadowCastingSetting.Off;
            }

            foreach (var child in node.GetChildren())
            {
                SetShadowCastingRecursive(child, enabled);
            }
        }
    }
}
