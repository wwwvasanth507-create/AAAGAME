# CUSTOM STORY, DIALOGUE SYSTEM & WORLD OBJECT/THING MODELS

---

## 1. Executive Summary

This module introduces an extensible, data-driven framework for developing player narrative storylines ("Own Story & Dialogues"), interactive branching dialogue trees, and 3D specifications for interactive world objects ("Things" & Props).

---

## 2. Architecture & Data Structures

### Story & Dialogue Architecture
- **`CustomStoryDatabase`**: Stores custom storylines, narrative arcs (`CustomStoryArc`), and story nodes (`CustomStoryNode`). Pre-configured with original storylines including *The Astral Seal Saga* and *Whispers of the Eternal Crucible*.
- **`CustomDialogueController`**: Interactive branching dialogue manager supporting speaker types (`Npc`, `Player`, `Narrator`, `Item`), emotion hooks, voice audio triggers, conditional choices (world flags, reputation, items), decision recording, and quest/reward hooks.
- **`CustomStoryManager`**: Central service implementing `IInitializable` registered with `ServiceLocator`. Orchestrates active narrative sagas, tracks node completion, and updates `WorldStateManager`.

### World Object & "Things" Models
- **`WorldObjectDefinition`**: Comprehensive data structures for interactive props and world entities:
  - `ChestContainer`, `RelicAltar`, `ArcaneSwitch`, `LoreTablet`, `DestructibleContainer`, `WaystonePillar`, `TrapPedestal`, `ElementalDoorLock`.
- **`ObjectMeshSpecification`**: Technical 3D mesh specs including model paths, base geometric bounds, shader keys, highlight outline colors, LOD tiers (LOD0/1/2), particle attachment points, and audio clips.
- **`InteractablePropManager`**: Runtime manager registered with `ServiceLocator` handling prop spawning, state machine transitions (`Idle`, `Active`, `Opened`, `Locked`, `Destroyed`), distance interaction evaluations, and world state persistence.

---

## 3. API Reference & Usage Examples

### Starting a Custom Story Arc
```csharp
var storyMgr = ServiceLocator.Get<CustomStoryManager>();
storyMgr.StartStoryArc("arc_astral_seal");
storyMgr.AdvanceStoryNode("astral_node_01");
```

### Spawning & Interacting with a World Object
```csharp
var propMgr = ServiceLocator.Get<InteractablePropManager>();
var prop = propMgr.SpawnProp("obj_astral_altar_01", "inst_altar_01", new Vector3(10, 0, 10), Vector3.Zero);
propMgr.InteractWithProp("inst_altar_01");
```

---

## 4. AI Asset Production Report

> [!IMPORTANT]
> The following production-ready specifications, AI prompts, technical limits, and asset manifests govern all art, 3D model, audio, and dialogue generation for this module.

### A. 3D Model Generation Prompts
1. **Astral Altar of Wisdom (`obj_astral_altar_01`)**
   - **Asset Name**: `AstralAltar.glb`
   - **Purpose**: Ancient relic altar for story milestones.
   - **Style & Art Direction**: Dark fantasy, carved obsidian pedestal with glowing cyan runes and floating ethereal crystal core.
   - **Technical Specifications**: PBR textures (Albedo, Normal, Roughness, Metallic, Emission). 1,800 Polygons (LOD0), 900 Polygons (LOD1), 300 Polygons (LOD2). Bounding size: 1.5m x 2.2m x 1.5m.
   - **AI Prompt**: `"Game-ready 3D model of an ancient fantasy relic altar, carved obsidian stone pedestal with glowing glowing blue runic engravings, hovering magical crystal floating in center, clean UVs, PBR textures, dark fantasy aesthetic, isolated on black background"`
   - **Negative Prompt**: `"blurry, low quality, broken mesh, distorted geometry, overlapping UVs"`
   - **Folder Location**: `res://Assets/Models/Props/AstralAltar.glb`

2. **Ancient Waystone Monolith (`obj_waystone_pillar_01`)**
   - **Asset Name**: `WaystonePillar.glb`
   - **Purpose**: World fast-travel and milestone attunement pillar.
   - **Style & Art Direction**: Weathered granite pillar wrapped in gold leaf runic ribbons.
   - **Technical Specifications**: 1,500 Polygons (LOD0), 750 (LOD1), 250 (LOD2). Bounding size: 1.0m x 4.0m x 1.0m.
   - **AI Prompt**: `"3D asset game model of a towering ancient monolith stone pillar, golden glowing magic runes etched into granite, high detail PBR textures"`
   - **Negative Prompt**: `"modern materials, glossy plastic, smooth textures"`
   - **Folder Location**: `res://Assets/Models/Props/WaystonePillar.glb`

### B. Texture & Material Prompts
1. **Runic Glow Shader Material (`shader_astral_glow`)**
   - **Resolution**: 2048x2048 PNG (Albedo, Normal, Emission map).
   - **AI Prompt**: `"Tileable seamless PBR texture of dark obsidian stone with luminous neon cyan glowing magic rune patterns, high quality normal map"`
   - **Folder Location**: `res://Assets/Textures/Props/AstralGlow_Emission.png`

### C. Audio & Voice Generation Prompts
1. **Altar Activation Sound (`sfx_altar_hum`)**
   - **Specification**: 24-bit 48kHz WAV audio, 3.5 sec duration, seamlessly loopable tail.
   - **AI Audio Prompt**: `"Deep low-frequency magical swell blending crystalline harmonic chimes and ancient ethereal resonance"`
   - **Folder Location**: `res://Assets/Audio/SFX/sfx_altar_hum.wav`

2. **Keeper Orin Voice Clip (`vo_keeper_orin_01`)**
   - **Tone**: Wise, ancient, authoritative.
   - **Voice Prompt**: `"Elder male voice with deep resonant timbre and solemn tone, British accent, speaking about ancient seals"`
   - **Folder Location**: `res://Assets/Audio/Voice/vo_keeper_orin_01.wav`

---

## 5. Asset Manifest

| Asset ID | Type | Path | Status |
| :--- | :--- | :--- | :--- |
| `obj_astral_altar_01` | 3D Prop | `res://Assets/Models/Props/AstralAltar.glb` | Spec Complete |
| `obj_waystone_pillar_01` | 3D Prop | `res://Assets/Models/Props/WaystonePillar.glb` | Spec Complete |
| `obj_ancient_chest_tier2` | 3D Prop | `res://Assets/Models/Props/AncientChest.glb` | Spec Complete |
| `obj_arcane_switch_01` | 3D Prop | `res://Assets/Models/Props/ArcaneSwitch.glb` | Spec Complete |
| `shader_astral_glow` | Shader | `res://Shaders/Props/astral_glow.gdshader` | Spec Complete |

---

## 6. Verification & Performance Notes

- **Compilation**: Clean compilation with 0 errors across the solution.
- **Unit Tests**: Full test suite in `CustomStoryAndObjectTests.cs`.
- **Memory & Android Performance**: All prop LOD tiers strictly adhere to mobile polygon budgets (<2,000 polygons for LOD0).
