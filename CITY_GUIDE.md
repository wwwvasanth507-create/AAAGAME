# HERO OF ETERNIA — CITY GUIDE: VALENHOLD METROPOLIS

---

## 1. Overview & District Breakdown

Valenhold Citadel is the primary regional hub of Act II. It comprises 6 distinct districts:

1. **High Council Heights (`district_valenhold_government`)**: Government district housing Council chambers, diplomatic emissaries, and bounty heralds.
2. **Silver Bay Market & Harbor (`district_valenhold_market`)**: Trading docks, caravan travel hub, and exotic import shops.
3. **The Iron Foundry Quarter (`district_valenhold_crafting`)**: War forges, alchemical cauldrons, and blacksmith merchants.
4. **Champions' Guild Hall (`district_valenhold_guild`)**: Training arena, skill respec trainers, and mercenary contract boards.
5. **Sanctuary of the Astral Embers (`district_valenhold_temple`)**: Healing temple, relic blessing altars, and ancient lore libraries.
6. **Vanguard Fortress & Bastion (`district_valenhold_guard_hq`)**: Guard headquarters, crime bounty clearance, and jail cells.

---

## 2. AI Asset Production Report

### A. 3D Props & UI Icons
1. **Valenhold City Map UI Icon (`icon_city_valenhold`)**
   - **Resolution**: 512x512 PNG transparent.
   - **AI Prompt**: `"Game UI icon of a majestic medieval walled city citadel with high spires and golden banners, isolated transparent background, fantasy art style"`
   - **Folder Location**: `res://Assets/UI/Icons/icon_city_valenhold.png`

---

## 3. Integration & Code References

District structures are managed by `ValenholdCityContent.cs` and exposed through `Act2Manager.ValenholdCity`. Verified by `Act2SystemTests`.
