# AI ASSET PRODUCTION — Prompt 10 Playtest Prototype

**Phase:** Prompt 10 / 150  
**Date:** 2026-07-25  
**Status:** Asset Specifications Ready for Generation

---

## Assets Introduced in This Phase

> [!NOTE]
> Phase 10 introduces combat systems that require corresponding visual and audio assets. All assets below are production-specified and ready for AI generation pipeline. No runtime assets are required for the headless test prototype, but they will be needed when visual prototype play begins in Prompt 11.

---

## 1. COMBAT VFX ASSETS

### 1.1 Sword Slash Effect

| Property | Value |
|----------|-------|
| **Asset Name** | vfx_slash |
| **Purpose** | Visual feedback for light melee attack swings |
| **Style** | Stylized fantasy, arc trail with bright edges |
| **Resolution** | 512×512 PNG (transparent) |
| **Art Direction** | Warm silver-white arc with slight blue energy |
| **VFX Hook Key** | `vfx_slash` |
| **Format** | PNG (RGBA) + spritesheet (8 frames) |
| **Folder** | `Assets/VFX/Combat/` |

**AI Generation Prompt:**
> A stylized fantasy sword slash VFX effect, single arc motion, white and light blue energy trail, semi-transparent alpha background, seen from a 3/4 top-down perspective, clean edges, suitable for a mobile RPG, 512×512 pixels, no shadow, flat illustration style.

**Negative Prompt:** realistic, photorealistic, dark, muddy, shadow overlay, cluttered

---

### 1.2 Heavy Slash Effect

| Property | Value |
|----------|-------|
| **Asset Name** | vfx_heavy_slash |
| **Purpose** | Heavy melee two-handed sweep — wider arc, power impact |
| **Style** | Larger arc, deeper red-orange energy |
| **Resolution** | 512×512 PNG (transparent) |
| **VFX Hook Key** | `vfx_heavy_slash` |
| **Format** | PNG (RGBA) + spritesheet (8 frames) |
| **Folder** | `Assets/VFX/Combat/` |

**AI Generation Prompt:**
> A large stylized fantasy heavy sword slash VFX, wide two-handed sweep arc, deep orange and red energy flame edge, alpha transparent background, mobile game art style, dramatic impact feel, 512×512 pixels.

---

### 1.3 Arrow / Projectile Trail

| Property | Value |
|----------|-------|
| **Asset Name** | vfx_arrow |
| **Purpose** | Bow projectile flight trail |
| **Style** | Thin golden trail, tapered behind arrow head |
| **Resolution** | 256×64 PNG (transparent) |
| **VFX Hook Key** | `vfx_arrow` |
| **Folder** | `Assets/VFX/Combat/` |

**AI Generation Prompt:**
> A stylized fantasy bow arrow flight trail VFX particle, golden yellow glow trail, narrow elongated shape, motion blur at tail, transparent background, suitable for a mobile action RPG, 256×64 pixels.

---

### 1.4 Fireball Projectile

| Property | Value |
|----------|-------|
| **Asset Name** | vfx_fireball |
| **Purpose** | Magic staff projectile orb |
| **Style** | Round glowing orange sphere, flame particles around edge |
| **Resolution** | 256×256 PNG (transparent) |
| **VFX Hook Key** | `vfx_fireball` |
| **Folder** | `Assets/VFX/Combat/` |

**AI Generation Prompt:**
> A stylized fantasy fireball magic projectile orb, glowing orange and red core, flame wisps around the edges, transparent alpha background, mobile RPG art style, soft glow aura, 256×256 pixels.

---

### 1.5 Pierce / Dagger Impact

| Property | Value |
|----------|-------|
| **Asset Name** | vfx_pierce |
| **Purpose** | Dagger stab impact flash |
| **Style** | Sharp cross-shaped flash, silver |
| **Resolution** | 256×256 PNG (transparent) |
| **Folder** | `Assets/VFX/Combat/` |

**AI Generation Prompt:**
> A stylized fantasy dagger stab impact VFX, sharp cross-spark flash, silver and white, fast motion effect, transparent background, 256×256 pixels, mobile RPG art style.

---

## 2. STATUS EFFECT VFX ASSETS

### 2.1 Burn Effect (Overlay)

**AI Generation Prompt:**
> A stylized fantasy Burn status VFX overlay, flickering orange and red flame particles around a character silhouette, transparent background, loopable sprite animation, mobile RPG art style, 512×512 pixels.

**VFX Hook Key:** `vfx_burn` | **Format:** Spritesheet (12 frames, 512×512)

---

### 2.2 Freeze Effect (Overlay)

**AI Generation Prompt:**
> A stylized fantasy Freeze status VFX overlay, icy blue crystalline frost covering a character silhouette, glittering particles, transparent background, 512×512 pixels, mobile RPG art style.

**VFX Hook Key:** `vfx_freeze` | **Format:** Spritesheet (8 frames, 512×512)

---

### 2.3 Poison Effect (Overlay)

**AI Generation Prompt:**
> A stylized fantasy Poison status VFX overlay, dripping green toxic droplets around a character, bubbling green mist effect, transparent background, 512×512 pixels, mobile RPG art style.

**VFX Hook Key:** `vfx_poison` | **Format:** Spritesheet (10 frames, 512×512)

---

### 2.4 Bleed Effect (Overlay)

**AI Generation Prompt:**
> A stylized fantasy Bleed status VFX overlay, small red blood droplets dripping downward from a character silhouette, crimson particles, transparent background, 512×512 pixels, mobile RPG art style.

**VFX Hook Key:** `vfx_bleed` | **Format:** Spritesheet (8 frames, 512×512)

---

## 3. WEAPON ICON ASSETS

### 3.1 Iron Sword Icon

| Property | Value |
|----------|-------|
| **Asset Name** | icon_wpn_sword |
| **Purpose** | Inventory icon for Iron Sword |
| **Style** | Fantasy RPG item icon, top-down angled view |
| **Resolution** | 256×256 PNG (transparent) |
| **Rarity Color** | `#9D9D9D` (Common grey border) |
| **Folder** | `Assets/UI/Icons/Weapons/` |

**AI Generation Prompt:**
> A stylized fantasy iron sword inventory icon for a mobile RPG game, simple grey iron blade with brown leather handle, slight golden crossguard, top-down angled isometric view, transparent background, clean crisp outline, 256×256 pixels.

---

### 3.2 Wooden Bow Icon

**AI Generation Prompt:**
> A stylized fantasy wooden bow inventory icon for a mobile RPG, curved brown wooden bow with simple string, top-down angled isometric view, transparent background, 256×256 pixels, warm earthy tones.

**Asset Name:** `icon_wpn_bow` | **Folder:** `Assets/UI/Icons/Weapons/`

---

### 3.3 Steel Dagger Icon

**AI Generation Prompt:**
> A stylized fantasy steel dagger inventory icon for a mobile RPG, slender silver blade with dark brown handle and small guard, top-down angled view, transparent background, clean crisp icon style, 256×256 pixels.

**Asset Name:** `icon_wpn_dagger` | **Folder:** `Assets/UI/Icons/Weapons/`

---

### 3.4 Wooden Staff Icon

**AI Generation Prompt:**
> A stylized fantasy wooden staff/wand inventory icon for a mobile RPG, gnarled wooden staff with a glowing blue crystal orb at the top, fantasy magic feel, top-down angled view, transparent background, 256×256 pixels.

**Asset Name:** `icon_wpn_staff` | **Folder:** `Assets/UI/Icons/Weapons/`

---

## 4. COMBAT AUDIO SPECIFICATIONS

### 4.1 Sword Swing SFX

| Property | Value |
|----------|-------|
| **Audio Hook Key** | `sfx_sword_swing` |
| **Duration** | 0.3–0.5 seconds |
| **Loop** | No |
| **Format** | WAV 44.1kHz 16-bit |
| **Export** | OGG (compressed) |
| **Folder** | `Assets/Audio/Combat/Melee/` |

**AI Music Prompt:**
> Sound effect: A realistic sword swing whoosh, sharp metallic air cut, medium weight, not too heavy, slightly resonant metallic hiss at the end. Duration 0.4 seconds. Mono audio.

---

### 4.2 Heavy Sword Swing SFX

**Audio Hook Key:** `sfx_greatsword`  
**AI Prompt:** Sound effect: A heavy two-handed greatsword swing whoosh, deep powerful air displacement, low rumble resonance, metallic scrape at end. Duration 0.6 seconds. Mono audio.

---

### 4.3 Bow Shot SFX

**Audio Hook Key:** `sfx_bowshot`  
**AI Prompt:** Sound effect: A wooden bow release twang followed by an arrow whoosh, light, quick, clean string pluck then air cut. Duration 0.5 seconds.

---

### 4.4 Magic Cast SFX

**Audio Hook Key:** `sfx_magic_cast`  
**AI Prompt:** Sound effect: A fantasy magic spell cast sound, energy charging build-up then a short release burst, crystalline and mystical, ethereal whoosh. Duration 0.6 seconds.

---

### 4.5 Dagger Hit SFX

**Audio Hook Key:** `sfx_dagger`  
**AI Prompt:** Sound effect: A short sharp metal stab impact sound, fast metallic puncture, light and precise. Duration 0.2 seconds.

---

## 5. FOLDER STRUCTURE

```
Assets/
├── VFX/
│   └── Combat/
│       ├── vfx_slash.png
│       ├── vfx_heavy_slash.png
│       ├── vfx_arrow.png
│       ├── vfx_fireball.png
│       ├── vfx_pierce.png
│       ├── vfx_burn_sheet.png
│       ├── vfx_freeze_sheet.png
│       ├── vfx_poison_sheet.png
│       └── vfx_bleed_sheet.png
├── Audio/
│   └── Combat/
│       ├── Melee/
│       │   ├── sfx_sword_swing.ogg
│       │   ├── sfx_greatsword.ogg
│       │   └── sfx_dagger.ogg
│       ├── Ranged/
│       │   └── sfx_bowshot.ogg
│       └── Magic/
│           └── sfx_magic_cast.ogg
└── UI/
    └── Icons/
        └── Weapons/
            ├── icon_wpn_sword.png
            ├── icon_wpn_greatsword.png
            ├── icon_wpn_dagger.png
            ├── icon_wpn_bow.png
            └── icon_wpn_staff.png
```

---

## 6. ASSET MANIFEST UPDATE

| Asset | Type | Resolution | Hook Key | Status |
|-------|------|------------|----------|--------|
| vfx_slash | VFX Spritesheet | 512×512 | vfx_slash | ⏳ Specified |
| vfx_heavy_slash | VFX Spritesheet | 512×512 | vfx_heavy_slash | ⏳ Specified |
| vfx_arrow | VFX Trail | 256×64 | vfx_arrow | ⏳ Specified |
| vfx_fireball | VFX Orb | 256×256 | vfx_fireball | ⏳ Specified |
| vfx_burn | Status Overlay | 512×512 | vfx_burn | ⏳ Specified |
| vfx_freeze | Status Overlay | 512×512 | vfx_freeze | ⏳ Specified |
| vfx_poison | Status Overlay | 512×512 | vfx_poison | ⏳ Specified |
| vfx_bleed | Status Overlay | 512×512 | vfx_bleed | ⏳ Specified |
| icon_wpn_sword | UI Icon | 256×256 | N/A | ⏳ Specified |
| icon_wpn_bow | UI Icon | 256×256 | N/A | ⏳ Specified |
| icon_wpn_dagger | UI Icon | 256×256 | N/A | ⏳ Specified |
| icon_wpn_staff | UI Icon | 256×256 | N/A | ⏳ Specified |
| sfx_sword_swing | Audio SFX | WAV/OGG | sfx_sword_swing | ⏳ Specified |
| sfx_greatsword | Audio SFX | WAV/OGG | sfx_greatsword | ⏳ Specified |
| sfx_bowshot | Audio SFX | WAV/OGG | sfx_bowshot | ⏳ Specified |
| sfx_magic_cast | Audio SFX | WAV/OGG | sfx_magic_cast | ⏳ Specified |
| sfx_dagger | Audio SFX | WAV/OGG | sfx_dagger | ⏳ Specified |

---

## 7. VALIDATION REPORT

| Validation Check | Result |
|-----------------|--------|
| Hook keys match CombatManager references | ✅ All 8 VFX hooks documented |
| Audio hook keys match WeaponDefinition | ✅ All 5 weapon audio hooks documented |
| Naming conventions consistent | ✅ snake_case prefix pattern |
| Folder paths established | ✅ Folder tree documented |
| Asset specifications complete | ✅ All prompts ready |
| Optimization targets defined | ✅ ETC2/ASTC specified |

---

## 8. OPTIMIZATION NOTES

| Asset Type | Optimization |
|------------|-------------|
| VFX sprites | Pack into atlases (2048×2048 max) to reduce draw calls |
| Weapon icons | Use texture arrays for batch rendering |
| Audio SFX | Convert WAV → OGG Vorbis (quality 6) for 5x compression |
| Status overlays | Implement as GPU Particles3D emitters for runtime performance |
