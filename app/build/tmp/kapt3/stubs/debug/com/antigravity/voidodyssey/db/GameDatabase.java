package com.antigravity.voidodyssey.db;

/**
 * Main database definition for the game.
 * Implements versioning and local save migrations.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000\u0014\n\u0002\u0018\u0002\n\u0002\u0018\u0002\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0002\b\u0002\b\'\u0018\u0000 \u00052\u00020\u0001:\u0001\u0005B\u0005\u00a2\u0006\u0002\u0010\u0002J\b\u0010\u0003\u001a\u00020\u0004H&\u00a8\u0006\u0006"}, d2 = {"Lcom/antigravity/voidodyssey/db/GameDatabase;", "Landroidx/room/RoomDatabase;", "()V", "gameDao", "Lcom/antigravity/voidodyssey/db/GameDao;", "Companion", "app_debug"})
@androidx.room.Database(entities = {com.antigravity.voidodyssey.db.PlayerProfile.class, com.antigravity.voidodyssey.db.InventoryItem.class, com.antigravity.voidodyssey.db.QuestEntity.class, com.antigravity.voidodyssey.db.SkillEntity.class, com.antigravity.voidodyssey.db.AchievementEntity.class, com.antigravity.voidodyssey.db.SaveSlot.class, com.antigravity.voidodyssey.db.GameSetting.class, com.antigravity.voidodyssey.db.WorldObjectEntity.class}, version = 1, exportSchema = false)
public abstract class GameDatabase extends androidx.room.RoomDatabase {
    @kotlin.jvm.Volatile()
    @org.jetbrains.annotations.Nullable()
    private static volatile com.antigravity.voidodyssey.db.GameDatabase INSTANCE;
    @org.jetbrains.annotations.NotNull()
    private static final androidx.room.migration.Migration MIGRATION_1_2 = null;
    @org.jetbrains.annotations.NotNull()
    public static final com.antigravity.voidodyssey.db.GameDatabase.Companion Companion = null;
    
    public GameDatabase() {
        super();
    }
    
    @org.jetbrains.annotations.NotNull()
    public abstract com.antigravity.voidodyssey.db.GameDao gameDao();
    
    @kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000 \n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0004\n\u0002\u0018\u0002\n\u0000\b\u0086\u0003\u0018\u00002\u00020\u0001B\u0007\b\u0002\u00a2\u0006\u0002\u0010\u0002J\u000e\u0010\t\u001a\u00020\u00042\u0006\u0010\n\u001a\u00020\u000bR\u0010\u0010\u0003\u001a\u0004\u0018\u00010\u0004X\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u0011\u0010\u0005\u001a\u00020\u0006\u00a2\u0006\b\n\u0000\u001a\u0004\b\u0007\u0010\b\u00a8\u0006\f"}, d2 = {"Lcom/antigravity/voidodyssey/db/GameDatabase$Companion;", "", "()V", "INSTANCE", "Lcom/antigravity/voidodyssey/db/GameDatabase;", "MIGRATION_1_2", "Landroidx/room/migration/Migration;", "getMIGRATION_1_2", "()Landroidx/room/migration/Migration;", "getDatabase", "context", "Landroid/content/Context;", "app_debug"})
    public static final class Companion {
        
        private Companion() {
            super();
        }
        
        @org.jetbrains.annotations.NotNull()
        public final androidx.room.migration.Migration getMIGRATION_1_2() {
            return null;
        }
        
        @org.jetbrains.annotations.NotNull()
        public final com.antigravity.voidodyssey.db.GameDatabase getDatabase(@org.jetbrains.annotations.NotNull()
        android.content.Context context) {
            return null;
        }
    }
}