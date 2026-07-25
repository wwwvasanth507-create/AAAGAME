# Settlement System - Hero of Eternia

> Complete settlement simulation framework for living, breathing communities.

## Architecture Overview

The settlement system provides a data-driven framework for creating and managing settlements as living communities. NPCs follow daily routines, occupy meaningful locations, and provide reusable services.

### Core Components

```
SettlementManager (Orchestrator)
├── SettlementDatabase (Settlement+Type definitions)
├── BuildingDatabase (Building definitions)
├── NpcScheduleExpanded (Per-profession schedules)
└── WorldEventFramework (Dynamic world events)
     └── Save V14 Integration
```

### Settlement Types (15 total)

| Type | Min Pop | Max Pop | Default Pop | Key Features |
|------|---------|---------|-------------|--------------|
| Camp | 5 | 50 | 20 | Minimal facilities, temporary |
| Hamlet | 20 | 150 | 60 | Basic services, rural |
| Village | 50 | 500 | 150 | Farms, trade, blacksmith |
| Town | 200 | 2000 | 600 | Walls, diverse services |
| City | 1000 | 10000 | 3000 | All services, strong |
| Capital | 5000 | 50000 | 10000 | Maximum population |
| Fort | 50 | 500 | 150 | Military focus |
| Castle | 200 | 2000 | 500 | Noble seat |
| Port | 100 | 3000 | 500 | Coastal trade |
| MiningCamp | 20 | 200 | 60 | Resource extraction |
| ForestOutpost | 10 | 80 | 30 | Remote wilderness |
| Temple | 30 | 300 | 80 | Religious center |
| NomadCamp | 15 | 100 | 40 | Mobile camp |
| FloatingSettlement | - | - | - | Future DLC |
| UndergroundSettlement | - | - | - | Future DLC |

### Settlement Data Fields

- SettlementId, DisplayName, LocalizationKey
- Type (SettlementType enum)
- Region, Biome
- Population, MaxPopulation
- Prosperity (ProsperityLevel)
- Security (SecurityRating)
- Government (GovernmentType - future)
- PrimaryIndustries, PrimaryExports, PrimaryImports
- Faction
- ClimateModifier
- SettlementServices (List<ServiceType>)
- BuildingIds (List<string>)
- SpawnRules (SettlementSpawnRules)
- MusicProfile
- WorldPositionX, WorldPositionZ
- ExtensionData (future DLC)

### Building System

**Categories:** Residential, Commercial, Industrial, Agricultural, Civic, Military, Religious, Services, Storage, Transportation, Entertainment, Educational, Medical, Custom

**Default Buildings (25+):**
- 3 Houses (Small/Medium/Large)
- 2 Inns/Taverns
- 2 Blacksmiths
- 2 Merchant Shops
- 2 Farms
- Mill, Bakery
- Town Hall
- 2 Guard Barracks
- 2 Temples/Cathedrals
- Hospital
- Warehouse
- 2 Markets
- Stables
- 2 Docks
- Workshop
- Library
- Training Grounds

Each building supports: Interior/Exterior hooks, NPC capacity, operating hours, services, ownership, upgrade hooks (up to level 3).

### NPC Scheduling

**Supported Activities:** Wake, Sleep, Breakfast, TravelToWork, Work, Lunch, Shopping, Socialize, Patrol, Relax, GoHome, Emergency, Festival, Idle, Train, Worship, Study, Guard, Hunt, Fish, Gather, Craft, Trade, Travel

**Per-Profession Schedules:**
- Farmer (4:48 AM - 9:36 PM)
- Merchant (5:16 AM - 9:36 PM)
- Blacksmith (4:48 AM - 9:07 PM)
- Guard (4:48 AM - 8:53 PM)
- Civilian (Default)
- Priest (4:19 AM - 8:24 PM)

**Schedule Adaptation:**
- Weather overrides (storms → stay indoors)
- Festival overrides (celebrate in square)
- Emergency overrides (guards to gate)

### World Events

**Supported Events:** MarketDay, Festival, Harvest, StormPreparation, MonsterAlert, MerchantArrival, TravelingCaravan, ResourceShortage

**Event Lifecycle:** Pending → Active → Resolving → Completed

**Features:**
- Cooldown enforcement per settlement
- Prosperity bounds checking
- Settlement type filtering
- Severity scaling (Minor/Moderate/Major/Critical)
- Random trigger with weighted selection
- Save/restore support

### Services

**Supported Services (20):** Trading, Crafting, EquipmentRepair, Healing, InnRest, Storage, Training, Travel, Banking, Guild, Housing, Stables, Blacksmith, Enchanting, Alchemy, Library, Temple, Market, Dock, TownHall

### Save Integration (V14)

Persists: Settlement prosperity, population changes, building states, NPC schedules, world events.

### Performance Optimizations

- Dictionary-based O(1) lookups for settlements and buildings
- Settlement streaming (load/unload distant settlements)
- Configurable max NPCs per settlement
- Cooldown tracking with efficient cleanup
- Lazy initialization with fallback data

### Android Optimization

- Configurable spawn limits for mobile hardware
- Efficient schedule resolution (O(n) with n < 15 blocks)
- No runtime allocations during update loops
- Bulky data stored in config JSON, not code