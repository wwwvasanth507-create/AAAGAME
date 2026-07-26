# HERO OF ETERNIA — BRANCHING STORY SYSTEM DOCUMENTATION

---

## 1. Overview & Narrative Paths

Chapter 5 introduces non-linear choice mechanics allowing player decisions to determine the active narrative branch:

1. **Iron Vanguard Path (`IronVanguardAlliance`)**: Direct military action resulting in Vanguard dominance and heavy road security.
2. **Silver Syndicate Path (`SilverSyndicateAlliance`)**: Underground trade maneuvering resulting in contraband merchant unlocks and market expansion.
3. **Sylvan Circle Path (`SylvanCircleNeutrality`)**: Environmental preservation resulting in swamp purification and rare herbal gathering node spawns.

---

## 2. Decision Log Tracking

Decisions are recorded via `ChoiceDecisionRecord` instances in `BranchingStoryFramework.cs` and saved in `Chapter5SaveData.cs` (`RecordedChoiceIds`).

---

## 3. AI Asset Production Report

### A. UI Choice Icons
1. **Branch Choice UI Frame (`icon_choice_branch_frame`)**
   - **Resolution**: 512x512 PNG transparent.
   - **AI Prompt**: `"Game UI ornamental decision choice button frame with gold runic border and crimson banner background, transparent PNG"`
   - **Folder Location**: `res://Assets/UI/Icons/icon_choice_branch_frame.png`
