# Diplomacy System — Hero of Eternia

## Overview

The Diplomacy System manages inter-faction relationships including alliances, trade agreements, war, peace, and ceasefire. It integrates with the Faction Database and Reputation System for consistent world simulation.

## Architecture

```
SocialManager
  └─ DiplomacyManager (ServiceKey: "DiplomacyManager")
       ├─ DiplomaticRelation enum (7 states)
       ├─ Relation key: "factionA_factionB" (alphabetically sorted)
       ├─ Default initialization from FactionDatabase
       └─ Event-driven change notifications
```

## Diplomatic Relations

| Relation | Description | Reputation Modifier |
|----------|-------------|-------------------|
| Alliance | Mutual defense pact | +30 |
| TradeAgreement | Economic partnership | +15 |
| Peace | Non-aggression | +5 |
| Neutral | Default state | 0 |
| Ceasefire | Temporary truce | 0 |
| Conflict | Active disagreement | -20 |
| War | Open hostilities | -50 |

## Key Features

- **Symmetric Relations**: Relations are stored with alphabetically sorted keys, ensuring `GetRelation(A, B) == GetRelation(B, A)`
- **Default Initialization**: Automatically sets relations based on faction database friendly/hostile/neutral lists
- **Event-Driven**: Diplomatic changes fire events for UI updates, quest triggers, and faction database synchronization
- **Reputation Integration**: Diplomatic relations affect NPC reactions through the NpcReactionSystem

## API

| Method | Description |
|--------|-------------|
| GetRelation(a, b) | Get current relation between two factions |
| SetRelation(a, b, relation) | Set relation between two factions |
| DeclareAlliance(a, b) | Declare alliance |
| DeclareWar(a, b) | Declare war |
| DeclarePeace(a, b) | Declare peace |
| EstablishTradeAgreement(a, b) | Establish trade agreement |
| DeclareCeasefire(a, b) | Declare ceasefire |
| AreAllied(a, b) | Check if allied |
| AreAtWar(a, b) | Check if at war |
| GetAllies(factionId) | Get all allied factions |
| GetEnemies(factionId) | Get all enemy factions |
| GetDiplomaticReputationModifier(observer, target) | Get reputation modifier based on relations |

## Save/Load

Full state serialization through DiplomaticSaveData container.