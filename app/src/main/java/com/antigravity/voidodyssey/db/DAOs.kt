package com.antigravity.voidodyssey.db

import androidx.room.*

@Dao
interface GameDao {

    // --- Save Slot Operations ---
    @Query("SELECT * FROM save_slots ORDER BY lastSaved DESC")
    fun getAllSaveSlots(): List<SaveSlot>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertSaveSlot(slot: SaveSlot)

    @Query("DELETE FROM save_slots WHERE slotId = :slotId")
    fun deleteSaveSlot(slotId: Int)

    // --- Player Profile Operations ---
    @Query("SELECT * FROM player_profile WHERE slotId = :slotId")
    fun getPlayerProfile(slotId: Int): PlayerProfile?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertPlayerProfile(profile: PlayerProfile)

    @Query("DELETE FROM player_profile WHERE slotId = :slotId")
    fun deletePlayerProfile(slotId: Int)

    // --- Inventory Operations ---
    @Query("SELECT * FROM inventory WHERE slotId = :slotId")
    fun getInventory(slotId: Int): List<InventoryItem>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertInventoryItem(item: InventoryItem)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertInventoryItems(items: List<InventoryItem>)

    @Query("DELETE FROM inventory WHERE id = :itemId")
    fun deleteInventoryItem(itemId: Long)

    @Query("DELETE FROM inventory WHERE slotId = :slotId")
    fun clearInventory(slotId: Int)

    // --- Quest Operations ---
    @Query("SELECT * FROM quests WHERE slotId = :slotId")
    fun getQuests(slotId: Int): List<QuestEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertQuests(quests: List<QuestEntity>)

    @Query("DELETE FROM quests WHERE slotId = :slotId")
    fun clearQuests(slotId: Int)

    // --- Skill Operations ---
    @Query("SELECT * FROM skills WHERE slotId = :slotId")
    fun getSkills(slotId: Int): List<SkillEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertSkills(skills: List<SkillEntity>)

    @Query("DELETE FROM skills WHERE slotId = :slotId")
    fun clearSkills(slotId: Int)

    // --- Achievement Operations ---
    @Query("SELECT * FROM achievements WHERE slotId = :slotId")
    fun getAchievements(slotId: Int): List<AchievementEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertAchievements(achievements: List<AchievementEntity>)

    @Query("DELETE FROM achievements WHERE slotId = :slotId")
    fun clearAchievements(slotId: Int)

    // --- World Objects Operations ---
    @Query("SELECT * FROM world_objects WHERE slotId = :slotId")
    fun getWorldObjects(slotId: Int): List<WorldObjectEntity>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertWorldObjects(objects: List<WorldObjectEntity>)

    @Query("DELETE FROM world_objects WHERE slotId = :slotId")
    fun clearWorldObjects(slotId: Int)

    // --- Settings Operations ---
    @Query("SELECT * FROM settings")
    fun getAllSettings(): List<GameSetting>

    @Query("SELECT value FROM settings WHERE `key` = :key")
    fun getSetting(key: String): String?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun insertSetting(setting: GameSetting)
}
