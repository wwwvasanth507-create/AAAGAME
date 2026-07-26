using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World
{
    public enum ObjectCategory
    {
        ChestContainer,
        RelicAltar,
        ArcaneSwitch,
        LoreTablet,
        DestructibleContainer,
        WaystonePillar,
        TrapPedestal,
        ElementalDoorLock,
        CraftingNode
    }

    public enum ObjectState
    {
        Idle,
        Active,
        Opened,
        Locked,
        Destroyed,
        Disabled
    }

    public enum MeshShapeType
    {
        Box,
        Cylinder,
        Sphere,
        CustomMesh
    }

    /// <summary>
    /// 3D Technical mesh and rendering specifications for an interactive world object / prop model.
    /// </summary>
    public class ObjectMeshSpecification
    {
        public string ModelPath { get; set; } = "";
        public MeshShapeType BaseShape { get; set; } = MeshShapeType.Box;
        public Vector3 BoundingSize { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
        public string ShaderMaterialKey { get; set; } = "default_prop_shader";
        public Color HighlightColor { get; set; } = new Color(0.0f, 1.0f, 1.0f, 1.0f);
        public int Lod0PolyCount { get; set; } = 1200;
        public int Lod1PolyCount { get; set; } = 600;
        public int Lod2PolyCount { get; set; } = 200;
        public string ParticleEffectKey { get; set; } = "";
        public string InteractionAudioKey { get; set; } = "";
    }

    /// <summary>
    /// Interaction definition for how a player or world entity interacts with an object.
    /// </summary>
    public class ObjectInteractionDefinition
    {
        public string PromptText { get; set; } = "Interact";
        public float MaxInteractionDistance { get; set; } = 3.5f;
        public float HoldDurationSeconds { get; set; } = 0.0f;
        public string RequiredKeyItemId { get; set; } = "";
        public int RequiredLevel { get; set; } = 1;
        public string AssociatedDialogueId { get; set; } = "";
        public string AssociatedQuestHookId { get; set; } = "";
        public string LootTableId { get; set; } = "";
        public string SetWorldFlagOnInteract { get; set; } = "";
        public bool OneTimeUse { get; set; } = false;
    }

    /// <summary>
    /// Complete data model definition for an interactive world object ("Thing" / Prop).
    /// </summary>
    public class WorldObjectDefinition
    {
        public string ObjectId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public ObjectCategory Category { get; set; } = ObjectCategory.RelicAltar;
        public ObjectState DefaultState { get; set; } = ObjectState.Idle;
        public ObjectMeshSpecification MeshSpec { get; set; } = new();
        public ObjectInteractionDefinition InteractionSpec { get; set; } = new();
        public bool IsPersistent { get; set; } = true;
        public int RespawnTimeSeconds { get; set; } = 0;
    }

    /// <summary>
    /// Database holding pre-configured interactive object and thing models.
    /// </summary>
    public class ThingModelDatabase
    {
        private readonly Dictionary<string, WorldObjectDefinition> _objects = new(StringComparer.OrdinalIgnoreCase);

        public ThingModelDatabase()
        {
            RegisterDefaultThingModels();
        }

        public void RegisterDefaultThingModels()
        {
            // 1. Astral Altar of Wisdom (Relic Altar)
            RegisterObject(new WorldObjectDefinition
            {
                ObjectId = "obj_astral_altar_01",
                DisplayName = "Astral Altar of Wisdom",
                Description = "An ancient runic altar emitting a soft, ethereal azure radiance.",
                Category = ObjectCategory.RelicAltar,
                DefaultState = ObjectState.Idle,
                MeshSpec = new ObjectMeshSpecification
                {
                    ModelPath = "res://Assets/Models/Props/AstralAltar.glb",
                    BaseShape = MeshShapeType.Cylinder,
                    BoundingSize = new Vector3(1.5f, 2.2f, 1.5f),
                    ShaderMaterialKey = "shader_astral_glow",
                    HighlightColor = new Color(0.2f, 0.8f, 1.0f),
                    Lod0PolyCount = 1800,
                    Lod1PolyCount = 900,
                    Lod2PolyCount = 300,
                    ParticleEffectKey = "vfx_astral_particles",
                    InteractionAudioKey = "sfx_altar_hum"
                },
                InteractionSpec = new ObjectInteractionDefinition
                {
                    PromptText = "Inspect Relic Altar",
                    MaxInteractionDistance = 4.0f,
                    AssociatedDialogueId = "dlg_astral_relic_inspect",
                    SetWorldFlagOnInteract = "astral_altar_inspected",
                    OneTimeUse = false
                }
            });

            // 2. Ancient Waystone Pillar (Waystone Pillar)
            RegisterObject(new WorldObjectDefinition
            {
                ObjectId = "obj_waystone_pillar_01",
                DisplayName = "Ancient Waystone Pillar",
                Description = "A towering monolith carved with protective runes that light up upon touch.",
                Category = ObjectCategory.WaystonePillar,
                DefaultState = ObjectState.Idle,
                MeshSpec = new ObjectMeshSpecification
                {
                    ModelPath = "res://Assets/Models/Props/WaystonePillar.glb",
                    BaseShape = MeshShapeType.Cylinder,
                    BoundingSize = new Vector3(1.0f, 4.0f, 1.0f),
                    ShaderMaterialKey = "shader_runic_monolith",
                    HighlightColor = new Color(1.0f, 0.84f, 0.0f),
                    Lod0PolyCount = 1500,
                    Lod1PolyCount = 750,
                    Lod2PolyCount = 250,
                    ParticleEffectKey = "vfx_waystone_sparks",
                    InteractionAudioKey = "sfx_waystone_activate"
                },
                InteractionSpec = new ObjectInteractionDefinition
                {
                    PromptText = "Attune Waystone",
                    MaxInteractionDistance = 3.5f,
                    HoldDurationSeconds = 1.5f,
                    SetWorldFlagOnInteract = "waystone_vale_attuned",
                    OneTimeUse = true
                }
            });

            // 3. Ancient Treasure Chest (Chest Container)
            RegisterObject(new WorldObjectDefinition
            {
                ObjectId = "obj_ancient_chest_tier2",
                DisplayName = "Ancient Reinforced Coffer",
                Description = "A heavy iron-bound chest engraved with ancient Eternian heraldry.",
                Category = ObjectCategory.ChestContainer,
                DefaultState = ObjectState.Idle,
                MeshSpec = new ObjectMeshSpecification
                {
                    ModelPath = "res://Assets/Models/Props/AncientChest.glb",
                    BaseShape = MeshShapeType.Box,
                    BoundingSize = new Vector3(1.2f, 1.0f, 0.8f),
                    ShaderMaterialKey = "shader_metallic_gold",
                    HighlightColor = new Color(1.0f, 0.9f, 0.3f),
                    Lod0PolyCount = 1200,
                    Lod1PolyCount = 600,
                    Lod2PolyCount = 200,
                    InteractionAudioKey = "sfx_chest_open"
                },
                InteractionSpec = new ObjectInteractionDefinition
                {
                    PromptText = "Open Coffer",
                    MaxInteractionDistance = 3.0f,
                    LootTableId = "loot_table_ancient_vault",
                    RequiredKeyItemId = "key_ancient_iron",
                    OneTimeUse = true
                }
            });

            // 4. Arcane Crystal Switch (Arcane Switch)
            RegisterObject(new WorldObjectDefinition
            {
                ObjectId = "obj_arcane_switch_01",
                DisplayName = "Arcane Crystal Switch",
                Description = "A levitating crystal cluster that channels energy to unlock nearby barriers.",
                Category = ObjectCategory.ArcaneSwitch,
                DefaultState = ObjectState.Idle,
                MeshSpec = new ObjectMeshSpecification
                {
                    ModelPath = "res://Assets/Models/Props/ArcaneSwitch.glb",
                    BaseShape = MeshShapeType.Sphere,
                    BoundingSize = new Vector3(1.0f, 1.0f, 1.0f),
                    ShaderMaterialKey = "shader_crystal_refract",
                    HighlightColor = new Color(0.9f, 0.1f, 0.9f),
                    Lod0PolyCount = 950,
                    Lod1PolyCount = 450,
                    Lod2PolyCount = 150,
                    ParticleEffectKey = "vfx_crystal_pulse",
                    InteractionAudioKey = "sfx_switch_toggle"
                },
                InteractionSpec = new ObjectInteractionDefinition
                {
                    PromptText = "Toggle Switch",
                    MaxInteractionDistance = 3.5f,
                    SetWorldFlagOnInteract = "arcane_switch_activated",
                    OneTimeUse = false
                }
            });
        }

        public void RegisterObject(WorldObjectDefinition obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ObjectId))
            {
                _objects[obj.ObjectId] = obj;
            }
        }

        public WorldObjectDefinition? GetObjectDefinition(string objectId)
        {
            return _objects.TryGetValue(objectId, out var def) ? def : null;
        }

        public List<WorldObjectDefinition> GetAllDefinitions()
        {
            return new List<WorldObjectDefinition>(_objects.Values);
        }
    }
}
