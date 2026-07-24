# AI Asset Pipeline Report — Hero of Eternia (v0.4.0)

This report details the specifications, formats, folders, and prompt templates for all game assets generated via the AI-first production pipeline.

---

## 1. 3D Model & Mesh Budgets

To ensure high framerates on lower-end Android devices (2GB RAM target), the following poly budgets and formats are permanently enforced:

| Asset Category | LOD0 (Max Tris) | LOD1 (Max Tris) | LOD2 (Max Tris) | Format |
|---|---|---|---|---|
| **Player Hero** | 3,000 | 1,500 | 500 | `.glb` (glTF 2.0 binary) |
| **Enemies & Bosses** | 2,500 | 1,200 | 400 | `.glb` (glTF 2.0 binary) |
| **Dungeon Grids / Modules** | 800 | 400 | N/A | `.glb` (glTF 2.0 binary) |
| **Small Props / Interactive** | 300 | N/A | N/A | `.glb` (glTF 2.0 binary) |

### Collision Budgets
- *Static Obstacles / World:* Simple convex hull collisions or box shape colliders.
- *Entities / Player:* Capsule colliders (`CapsuleShape3D`) or sphere colliders only. No high-poly mesh colliders.

---

## 2. Texturing & Image Compression

All 2D assets and 3D textures must follow these configurations:

- **Resolution Targets:**
  - UI Cards & Loading Screens: 2048x1024 or 2048x2048 px
  - 3D Character Models: 1024x1024 px
  - Dungeon Grids & Environmental Textures: 1024x1024 px
  - SFX Particle Sheets: 512x512 px or 256x256 px
  - Minimap / Map Tiles: 1024x1024 px
- **Texture Maps (PBR):** BaseColor, Normal, Metallic-Roughness (packed green/blue channels), Ambient Occlusion, Emission.
- **Godot VRAM Compression:** Set to target ETC2 (Android Mobile) and ASTC (High quality mobile) formats in project export configurations.

---

## 3. Audio & Music Specifications

- **Format:**
  - Sound Effects (Footsteps, UI clicks, hits): `.wav` format, 44100Hz, 16-bit Mono.
  - Background Music & Ambient Loops: `.ogg` format, 44100Hz, Stereo.
- **SFX Loudness:** Mixed targeting -18 LUFS.
- **BGM Loudness:** Mixed targeting -24 LUFS.

---

## 4. Folder Structure Standards

| Asset Type | Export / Source Directory |
|---|---|
| **3D Models & Rigs** | `Assets/Characters/`, `Assets/Enemies/`, `Assets/Environment/` |
| **PBR Textures** | `Assets/Materials/` |
| **UI Sprite Assets** | `Assets/UI/` |
| **Footstep WAV clips** | `Assets/Audio/Player/Footsteps/` |
| **BGM and Ambient loops** | `Assets/Audio/` |
| **Animation Libraries** | `Assets/Animations/` |
| **Custom Shaders** | `Shaders/` |

---

## 5. AI Prompt Templates Manifest

### 5.1 Player Character (3D Model Concept Art)
- **Prompt:** *"Front-facing concept art of a futuristic knight, Eternia crystal armor, glowing neon blue lines, high-tech chestplate, solid grey background, 2D flat, character reference sheet, artstation trending"*
- **Folder:** `Assets/Characters/`

### 5.2 Environmental Brick (PBR Material)
- **Prompt:** *"Seamless dark stone brick texture, mossy crevices, high detail PBR, normal map, height map, roughness map, diffuse map, game texture, fantasy castle dungeon floor"*
- **Folder:** `Assets/Materials/`

### 5.3 Interface Card Frame (2D UI Glassmorphism)
- **Prompt:** *"Frosted glass panel, dark blue neon edge glow, RPG game inventory card frame, flat UI element, transparent background, vector graphic, 512x512"*
- **Folder:** `Assets/UI/`

### 5.4 Audio Sound Effect (Double-tap Roll)
- **Prompt:** *"Fast swoosh slide sound effect, wind friction, fabric rustle, dodge roll action, dry transient, raw WAV format"*
- **Folder:** `Assets/Audio/Player/`
