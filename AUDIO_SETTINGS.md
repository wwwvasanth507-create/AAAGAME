# AUDIO SETTINGS SPECIFICATION — HERO OF ETERNIA (PROMPT 21)

## System Overview
`AudioSettings.cs` encapsulates category volume gains, mute flags, dynamic range profiles, and subtitle configurations. Integrated directly into `SaveProfile` schema (Save V16).

## Data Schema

```json
{
  "MasterVolume": 1.0,
  "MusicVolume": 0.8,
  "AmbientVolume": 0.8,
  "EnvironmentVolume": 0.8,
  "CombatVolume": 0.9,
  "UIVolume": 0.8,
  "DialogueVolume": 1.0,
  "NPCVolume": 0.9,
  "CreaturesVolume": 0.9,
  "WeatherVolume": 0.8,
  "FootstepsVolume": 0.7,
  "AbilitiesVolume": 0.9,
  "VoiceOverVolume": 1.0,
  "IsMuted": false,
  "MuteInBackground": true,
  "DynamicRange": "Mobile",
  "Subtitles": {
    "Enabled": true,
    "FontSize": 18,
    "ShowSpeakerName": true,
    "ColorCodeSpeakers": true,
    "ShowBackground": true,
    "BackgroundOpacity": 0.6
  }
}
```
