# Player Footstep Audio Specification

## AI Audio Generation Prompts

All files in this directory are loaded by `PlayerAudioController.cs`.
Generated via AI audio pipeline per the AI-first production rules.

---

## Required Files

### Stone Surface
| File | AI Prompt |
|---|---|
| `stone_01.wav` | *"Short sharp concrete footstep impact, hard sole on stone floor, dry resonance, 0.2s, 44100Hz mono"* |
| `stone_02.wav` | *"Slightly lighter concrete footstep variation, same character, subtle heel click"* |

### Grass Surface
| File | AI Prompt |
|---|---|
| `grass_01.wav` | *"Soft footstep on dense grass, slight rustle, organic thud, 0.25s, 44100Hz mono"* |
| `grass_02.wav` | *"Grass footstep variation, lighter stride, more leaf rustle"* |

### Wood Surface
| File | AI Prompt |
|---|---|
| `wood_01.wav` | *"Hollow wooden plank footstep, slight creak, warm resonance, 0.2s"* |
| `wood_02.wav` | *"Wood footstep variation, slightly more creak and resonance"* |

### Sand Surface
| File | AI Prompt |
|---|---|
| `sand_01.wav` | *"Crunching sand footstep, dry granular texture, 0.3s, soft attack"* |
| `sand_02.wav` | *"Lighter sand step, fewer grains displaced"* |

### Snow Surface
| File | AI Prompt |
|---|---|
| `snow_01.wav` | *"Compact snow crunch footstep, crisp high-frequency crystals, cold bite, 0.25s"* |
| `snow_02.wav` | *"Deeper powder snow footstep, softer crunch, slight squeak"* |

### Water Surface
| File | AI Prompt |
|---|---|
| `water_01.wav` | *"Shallow water splash footstep, wet slap, small ripple spray, 0.3s"* |
| `water_02.wav` | *"Lighter water footstep, less splash, more drip"* |

### Mud Surface
| File | AI Prompt |
|---|---|
| `mud_01.wav` | *"Thick wet mud footstep, suction squelch sound, heavy viscous pull, 0.35s"* |
| `mud_02.wav` | *"Lighter mud step, less suction, more splat"* |

---

## Action Sounds

| File | AI Prompt |
|---|---|
| `jump.wav` | *"Quick effort grunt + boot push-off on stone, sharp start, 0.2s"* |
| `land.wav` | *"Heavy landing impact thud, boot on stone, slight grunt, 0.3s"* |
| `roll.wav` | *"Quick body roll on ground, cloth and boot sounds, 0.4s"* |

---

## Technical Specifications
- **Format:** WAV, 44100 Hz, 16-bit mono
- **Loudness:** -18 LUFS (Godot handles bus mixing)
- **Loop:** No (one-shot)
- **Naming:** `{surface}_{01|02}.wav`, `{action}.wav`
- **Export:** OGG Vorbis (imported by Godot for Android compression)
- **Godot Import Setting:** `Loop=off`, `Compress=true`
