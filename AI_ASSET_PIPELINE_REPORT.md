# AI Asset Pipeline Report — Hero of Eternia (v0.2.0)

This report details the specifications, prompts formats, and pipelines configurations for all game assets.

---

## 1. 3D Model Specifications
All 3D assets (characters, props, environmental grids) will follow these target budgets to run smoothly within budget Android GPU limitations:
*   **Hero Model:** LOD0 < 3,000 tris. LOD1 < 1,500 tris. LOD2 < 500 tris.
*   **Enemies & Bosses:** LOD0 < 2,500 tris. LOD1 < 1,200 tris.
*   **Dungeon Grids & Props:** LOD0 < 800 tris.
*   **Colliders:** Box/Sphere collision hulls only.
*   **Format:** glTF 2.0 binary (.glb).

---

## 2. Texturing & Compression
*   **Resolutions:** UI/Loading (2048x2048), Environmental/Models (1024x1024), SFX particles (512x512).
*   **Formats:** PBR Maps (Metallic, Roughness, Normal, Ambient Occlusion).
*   **VRAM Compression:** Pre-configured in Godot project settings to target ETC2/ASTC compression.

---

## 3. AI Prompts Manifest Templates
*   **Loading Screen Concept Prompt:**
    `Realistic dark fantasy landscape, mystical ruined castle stone entrance, neon blue glowing runic patterns, starry nebula sky, volumetric fog, Unreal Engine 5 render, cinematic lighting, ultra-detailed --ar 16:9`
*   **SFX Audio Generator Prompt:**
    `Heavy iron sword slash hit metal impact, medieval battle sound effect, raw transient, low ambient noise, WAV format`
*   **Export Locations:**
    *   Images/Textures: `Assets/Textures/`
    *   3D Models: `Assets/Characters/` or `Assets/Environment/`
    *   Audio SFX: `Assets/Audio/SFX/`
