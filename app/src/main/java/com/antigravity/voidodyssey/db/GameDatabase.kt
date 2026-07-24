package com.antigravity.voidodyssey.db

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase
import androidx.room.migration.Migration
import androidx.sqlite.db.SupportSQLiteDatabase

/**
 * Main database definition for the game.
 * Implements versioning and local save migrations.
 */
@Database(
    entities = [
        PlayerProfile::class,
        InventoryItem::class,
        QuestEntity::class,
        SkillEntity::class,
        AchievementEntity::class,
        SaveSlot::class,
        GameSetting::class,
        WorldObjectEntity::class
    ],
    version = 1,
    exportSchema = false
)
abstract class GameDatabase : RoomDatabase() {

    abstract fun gameDao(): GameDao

    companion object {
        @Volatile
        private var INSTANCE: GameDatabase? = null

        // Safe database migration pattern example (e.g., from v1 to v2 if we add new tables/columns later)
        val MIGRATION_1_2 = object : Migration(1, 2) {
            override fun migrate(db: SupportSQLiteDatabase) {
                // Future update migrations will go here
                // Example: db.execSQL("ALTER TABLE player_profile ADD COLUMN rank INTEGER NOT NULL DEFAULT 0")
            }
        }

        fun getDatabase(context: Context): GameDatabase {
            return INSTANCE ?: synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    GameDatabase::class.java,
                    "void_odyssey_db"
                )
                .addMigrations(MIGRATION_1_2)
                .fallbackToDestructiveMigration() // Dev helper: fallback if migration fails
                .build()
                INSTANCE = instance
                instance
            }
        }
    }
}
