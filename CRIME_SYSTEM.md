# Crime System — Hero of Eternia

## Overview

The Crime System manages criminal behavior, witness detection, bounties, and crime expiration. It integrates with the Guard AI system for law enforcement reactions.

## Architecture

```
SocialManager
  └─ CrimeManager (ServiceKey: "CrimeManager")
       ├─ CrimeRecord (data model)
       ├─ CrimeType enum (7 types)
       ├─ CrimeSeverity enum (5 levels)
       ├─ Witness detection
       ├─ Bounty management
       └─ Crime expiration
```

## Crime Types

| Type | Severity | Base Bounty | Description |
|------|----------|-------------|-------------|
| Theft | Minor | 25 | Stealing items |
| Trespassing | Minor | 10 | Entering restricted areas |
| Assault | Serious | 100 | Physical attacks |
| Murder | Capital | 500 | Killing NPCs |
| PropertyDamage | Minor | 15 | Breaking objects |
| IllegalTrading | Moderate | 50 | Black market trade |
| RestrictedAreaEntry | Serious | 75 | Entering forbidden zones |

## Crime Severity Levels

| Severity | Bounty Multiplier | Expiration (hours) |
|----------|------------------|-------------------|
| Minor | 1× | 24 |
| Moderate | 2× | 72 |
| Serious | 5× | 168 |
| Severe | 10× | 720 |
| Capital | 20× | Never |

## Witness Detection

Witness detection uses probability-based checks:
- Detection chance varies by crime type (Murder: 90%, Trespassing: 20%)
- Distance scaling: closer = higher detection chance
- Hidden status bypasses detection entirely
- Detection uses seeded random for reproducibility

## Bounty System

- Perpetrator accumulates bounties per faction
- Bounty stacks across multiple crimes
- Bounty clearance on crime resolution
- Event-driven bounty updates

## Crime Expiration

- Minor crimes expire after 24 in-game hours
- Capital crimes never expire
- Expired crimes no longer affect reputation or guard behavior
- Bounties cleared when all associated crimes expire

## Save/Load

Full state serialization through CrimeSaveData container.