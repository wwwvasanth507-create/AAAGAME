# Journal System — Hero of Eternia

> Last Updated: 2026-07-25 (Phase 19)

## Architecture

The JournalManager provides a comprehensive journal system tracking quests, lore, dialogue, discoveries, and more. All data-driven with future bestiary/codex support.

### Components

- **Quest Journal**: Active, completed, and failed quests with full progress tracking
- **Lore Entries**: Unlockable lore with categories (world, faction, character, item, bestiary)
- **Dialogue Log**: Record of all conversations with NPCs
- **Discovery Log**: Locations, landmarks, settlements, dungeons discovered
- **Future**: Bestiary, Codex

### Save Integration

Journal data persisted via SaveProfile V15 with JournalSaveData.