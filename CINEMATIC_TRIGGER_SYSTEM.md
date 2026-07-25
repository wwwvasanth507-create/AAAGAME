# CINEMATIC TRIGGER SYSTEM SPECIFICATION — HERO OF ETERNIA (PROMPT 26)

## System Overview
`CinematicTriggerFramework.cs` manages spatial and event-based triggers for story cutscenes, sequence playback, and narrative transitions.

## Trigger Condition Types
* **EnterArea**: Spatial proximity trigger when player enters radius.
* **ExitArea**: Spatial trigger when exiting a designated boundary.
* **QuestCompletion / DialogueCompletion**: Triggered when narrative milestones finish.
* **BossDefeat**: Triggered on major boss death events.
* **Manual**: Triggered directly by script or event sequence.
