# AI Asset Pipeline & Manifest Report — Hero of Eternia (v0.5.0)

This report details the specifications, export paths, quality formats, and AI prompt templates canonical to all game assets generated via our AI-first production pipeline.

---

## 1. 3D Mesh & LOD Specifications

To ensure a solid 60 FPS on lower-end Android devices (2GB RAM targets), the following polygon budgets and formats are permanently enforced:

| Asset Category | LOD0 (Max Tris) | LOD1 (Max Tris) | LOD2 (Max Tris) | Format |
|---|---|---|---|---|
| **Player Hero (Base + Parts)** | 3,000 | 1,500 | 500 (No shadows) | `.glb` (glTF 2.0 binary) |
| **Enemies & Bosses** | 2,500 | 1,200 | 400 | `.glb` (glTF 2.0 binary) |
| **Dungeon Grids / Modules** | 800 | 400 | N/A | `.glb` (glTF 2.0 binary) |
| **Small Props / Interactive** | 300 | N/A | N/A | `.glb` (glTF 2.0 binary) |

### Collision Budgets
- **Static Obstacles / World:** Box colliders or simple convex decomposition shapes.
- **Entities / Characters:** Capsule (`CapsuleShape3D`) or sphere colliders only. No high-poly trimesh colliders.

---

## 2. Texturing & Image Compression

All 2D assets and 3D textures must follow these configurations:

- **Resolution Targets:**
  - UI Cards & Loading Screens: 2048x1024 or 2048x2048 px
  - 3D Character Models & Swappable Parts: 1024x1024 px
  - Dungeon Grids & Environmental Textures: 1024x1024 px
  - SFX Particle Sheets: 512x512 px or 256x256 px
  - Minimap / Map Tiles: 1024x1024 px
- **Texture Maps (PBR):** BaseColor, Normal, Metallic-Roughness (packed green/blue channels), Ambient Occlusion, Emission.
- **Godot VRAM Compression:** Targets ETC2 (Android Mobile) and ASTC (High quality mobile) formats in export presets.

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

### 5.1 Player Character & Swappable Parts (3D Model / Texture Concept)
- **Base Body Model:**
  - *Prompt:* `"Front-facing concept art of a stylized fantasy hero, athletic build, solid grey background, character reference sheet, stylized hand-painted textures"`
  - *Folder:* `Assets/Characters/Meshes/Player/`
- **Armor / Plates:**
  - *Prompt:* `"concept sheet of stylized leather and steel shoulder pauldrons, fantasy RPG, hand-painted texture, isolated grey background"`
  - *Folder:* `Assets/Characters/Armor/`
- **Hair Styles:**
  - *Prompt:* `"concept art of fantasy hair styles, spikes and braids, stylized RPG look, hand-painted albedo texture, isolated"`
  - *Folder:* `Assets/Characters/Hair/`

### 5.2 Environmental Brick (PBR Material)
- **Prompt:** *"Seamless dark stone brick texture, mossy crevices, high detail PBR, normal map, height map, roughness map, diffuse map, game texture, fantasy castle dungeon floor"*
- **Folder:** `Assets/Materials/`

### 5.3 Interface Card Frame (2D UI Glassmorphism)
- **Prompt:** *"Frosted glass panel, dark blue neon edge glow, RPG game inventory card frame, flat UI element, transparent background, vector graphic, 512x512"*
- **Folder:** `Assets/UI/`

### 5.4 Audio Sound Effect (Footsteps / Transitions)
- **Grass Footstep:**
  - *Prompt:* `"soft squish of boot stepping on damp green grass, dry transient, isolated, high fidelity wav format"`
  - *Folder:* `Assets/Audio/Player/Footsteps/Grass/`
- **Stone Footstep:**
  - *Prompt:* `"heavy boot step on rough granite stone block, sharp transient echo, isolated, high fidelity wav format"`
  - *Folder:* `Assets/Audio/Player/Footsteps/Stone/`
