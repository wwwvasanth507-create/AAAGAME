package com.antigravity.voidodyssey.db

import androidx.room.Entity
import androidx.room.PrimaryKey

/**
 * Entity representing the Player's Profile.
 * Stores current progression, resources, ship type, and coordinate in the galaxy.
 */
@Entity(tableName = "player_profile")
data class PlayerProfile(
    @PrimaryKey val slotId: Int,
    val playerName: String,
    val level: Int,
    val xp: Int,
    val credits: Long,
    val currentSystem: String,
    val shipType: String,
    val hull: Float,
    val shield: Float,
    val playTime: Long,
    val worldSeed: Long
)

/**
 * Entity representing an item in the player's inventory.
 * Items can be cargo, materials, ship modules, or weapons.
 */
@Entity(tableName = "inventory")
data class InventoryItem(
    @PrimaryKey(autoGenerate = true) val id: Long = 0,
    val slotId: Int, // Belongs to a save slot
    val itemId: String,
    val itemName: String,
    val quantity: Int,
    val type: String, // "WEAPON", "SHIELD", "MATERIAL", "PET", "UPGRADE"
    val isEquipped: Boolean,
    val attributesJson: String // Serialized stats/modifiers
)

/**
 * Entity representing Quest progression.
 */
@Entity(tableName = "quests")
data class QuestEntity(
    @PrimaryKey val questId: String,
    val slotId: Int,
    val title: String,
    val description: String,
    val status: String, // "NOT_STARTED", "ACTIVE", "COMPLETED"
    val progress: Int,
    val targetCount: Int
)

/**
 * Entity representing Skills in the skill tree.
 */
@Entity(tableName = "skills")
data class SkillEntity(
    @PrimaryKey val skillId: String,
    val slotId: Int,
    val name: String,
    val isUnlocked: Boolean,
    val level: Int,
    val maxLevel: Int
)

/**
 * Entity representing Achievements.
 */
@Entity(tableName = "achievements")
data class AchievementEntity(
    @PrimaryKey val achievementId: String,
    val slotId: Int,
    val title: String,
    val description: String,
    val progress: Int,
    val targetCount: Int,
    val isUnlocked: Boolean
)

/**
 * Entity representing Save Slot Metadata.
 */
@Entity(tableName = "save_slots")
data class SaveSlot(
    @PrimaryKey val slotId: Int,
    val label: String,
    val lastSaved: Long
)

/**
 * Entity representing general game settings (offline audio/graphics options).
 */
@Entity(tableName = "settings")
data class GameSetting(
    @PrimaryKey val key: String,
    val value: String
)

/**
 * Entity representing persistent World objects (outposts/buildings, generated asteroid fields, NPC merchants).
 */
@Entity(tableName = "world_objects")
data class WorldObjectEntity(
    @PrimaryKey val objectId: String,
    val slotId: Int,
    val systemName: String,
    val type: String, // "ASTEROID_FIELD", "OUTPOST", "STATION_MERCHANT"
    val x: Float,
    val y: Float,
    val z: Float,
    val health: Float,
    val faction: String,
    val customData: String // JSON metadata
)
