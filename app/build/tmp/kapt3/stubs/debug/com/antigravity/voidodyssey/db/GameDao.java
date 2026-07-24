package com.antigravity.voidodyssey.db;

@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000^\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0000\n\u0002\u0010\u0002\n\u0000\n\u0002\u0010\b\n\u0002\b\u0006\n\u0002\u0010\t\n\u0002\b\u0003\n\u0002\u0010 \n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0010\u000e\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0013\bg\u0018\u00002\u00020\u0001J\u0010\u0010\u0002\u001a\u00020\u00032\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0010\u0010\u0006\u001a\u00020\u00032\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0010\u0010\u0007\u001a\u00020\u00032\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0010\u0010\b\u001a\u00020\u00032\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0010\u0010\t\u001a\u00020\u00032\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0010\u0010\n\u001a\u00020\u00032\u0006\u0010\u000b\u001a\u00020\fH\'J\u0010\u0010\r\u001a\u00020\u00032\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0010\u0010\u000e\u001a\u00020\u00032\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0016\u0010\u000f\u001a\b\u0012\u0004\u0012\u00020\u00110\u00102\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u000e\u0010\u0012\u001a\b\u0012\u0004\u0012\u00020\u00130\u0010H\'J\u000e\u0010\u0014\u001a\b\u0012\u0004\u0012\u00020\u00150\u0010H\'J\u0016\u0010\u0016\u001a\b\u0012\u0004\u0012\u00020\u00170\u00102\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0012\u0010\u0018\u001a\u0004\u0018\u00010\u00192\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0016\u0010\u001a\u001a\b\u0012\u0004\u0012\u00020\u001b0\u00102\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0012\u0010\u001c\u001a\u0004\u0018\u00010\u001d2\u0006\u0010\u001e\u001a\u00020\u001dH\'J\u0016\u0010\u001f\u001a\b\u0012\u0004\u0012\u00020 0\u00102\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0016\u0010!\u001a\b\u0012\u0004\u0012\u00020\"0\u00102\u0006\u0010\u0004\u001a\u00020\u0005H\'J\u0016\u0010#\u001a\u00020\u00032\f\u0010$\u001a\b\u0012\u0004\u0012\u00020\u00110\u0010H\'J\u0010\u0010%\u001a\u00020\u00032\u0006\u0010&\u001a\u00020\u0017H\'J\u0016\u0010\'\u001a\u00020\u00032\f\u0010(\u001a\b\u0012\u0004\u0012\u00020\u00170\u0010H\'J\u0010\u0010)\u001a\u00020\u00032\u0006\u0010*\u001a\u00020\u0019H\'J\u0016\u0010+\u001a\u00020\u00032\f\u0010,\u001a\b\u0012\u0004\u0012\u00020\u001b0\u0010H\'J\u0010\u0010-\u001a\u00020\u00032\u0006\u0010.\u001a\u00020\u0013H\'J\u0010\u0010/\u001a\u00020\u00032\u0006\u00100\u001a\u00020\u0015H\'J\u0016\u00101\u001a\u00020\u00032\f\u00102\u001a\b\u0012\u0004\u0012\u00020 0\u0010H\'J\u0016\u00103\u001a\u00020\u00032\f\u00104\u001a\b\u0012\u0004\u0012\u00020\"0\u0010H\'\u00a8\u00065"}, d2 = {"Lcom/antigravity/voidodyssey/db/GameDao;", "", "clearAchievements", "", "slotId", "", "clearInventory", "clearQuests", "clearSkills", "clearWorldObjects", "deleteInventoryItem", "itemId", "", "deletePlayerProfile", "deleteSaveSlot", "getAchievements", "", "Lcom/antigravity/voidodyssey/db/AchievementEntity;", "getAllSaveSlots", "Lcom/antigravity/voidodyssey/db/SaveSlot;", "getAllSettings", "Lcom/antigravity/voidodyssey/db/GameSetting;", "getInventory", "Lcom/antigravity/voidodyssey/db/InventoryItem;", "getPlayerProfile", "Lcom/antigravity/voidodyssey/db/PlayerProfile;", "getQuests", "Lcom/antigravity/voidodyssey/db/QuestEntity;", "getSetting", "", "key", "getSkills", "Lcom/antigravity/voidodyssey/db/SkillEntity;", "getWorldObjects", "Lcom/antigravity/voidodyssey/db/WorldObjectEntity;", "insertAchievements", "achievements", "insertInventoryItem", "item", "insertInventoryItems", "items", "insertPlayerProfile", "profile", "insertQuests", "quests", "insertSaveSlot", "slot", "insertSetting", "setting", "insertSkills", "skills", "insertWorldObjects", "objects", "app_debug"})
@androidx.room.Dao()
public abstract interface GameDao {
    
    @androidx.room.Query(value = "SELECT * FROM save_slots ORDER BY lastSaved DESC")
    @org.jetbrains.annotations.NotNull()
    public abstract java.util.List<com.antigravity.voidodyssey.db.SaveSlot> getAllSaveSlots();
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertSaveSlot(@org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.db.SaveSlot slot);
    
    @androidx.room.Query(value = "DELETE FROM save_slots WHERE slotId = :slotId")
    public abstract void deleteSaveSlot(int slotId);
    
    @androidx.room.Query(value = "SELECT * FROM player_profile WHERE slotId = :slotId")
    @org.jetbrains.annotations.Nullable()
    public abstract com.antigravity.voidodyssey.db.PlayerProfile getPlayerProfile(int slotId);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertPlayerProfile(@org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.db.PlayerProfile profile);
    
    @androidx.room.Query(value = "DELETE FROM player_profile WHERE slotId = :slotId")
    public abstract void deletePlayerProfile(int slotId);
    
    @androidx.room.Query(value = "SELECT * FROM inventory WHERE slotId = :slotId")
    @org.jetbrains.annotations.NotNull()
    public abstract java.util.List<com.antigravity.voidodyssey.db.InventoryItem> getInventory(int slotId);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertInventoryItem(@org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.db.InventoryItem item);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertInventoryItems(@org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.db.InventoryItem> items);
    
    @androidx.room.Query(value = "DELETE FROM inventory WHERE id = :itemId")
    public abstract void deleteInventoryItem(long itemId);
    
    @androidx.room.Query(value = "DELETE FROM inventory WHERE slotId = :slotId")
    public abstract void clearInventory(int slotId);
    
    @androidx.room.Query(value = "SELECT * FROM quests WHERE slotId = :slotId")
    @org.jetbrains.annotations.NotNull()
    public abstract java.util.List<com.antigravity.voidodyssey.db.QuestEntity> getQuests(int slotId);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertQuests(@org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.db.QuestEntity> quests);
    
    @androidx.room.Query(value = "DELETE FROM quests WHERE slotId = :slotId")
    public abstract void clearQuests(int slotId);
    
    @androidx.room.Query(value = "SELECT * FROM skills WHERE slotId = :slotId")
    @org.jetbrains.annotations.NotNull()
    public abstract java.util.List<com.antigravity.voidodyssey.db.SkillEntity> getSkills(int slotId);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertSkills(@org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.db.SkillEntity> skills);
    
    @androidx.room.Query(value = "DELETE FROM skills WHERE slotId = :slotId")
    public abstract void clearSkills(int slotId);
    
    @androidx.room.Query(value = "SELECT * FROM achievements WHERE slotId = :slotId")
    @org.jetbrains.annotations.NotNull()
    public abstract java.util.List<com.antigravity.voidodyssey.db.AchievementEntity> getAchievements(int slotId);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertAchievements(@org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.db.AchievementEntity> achievements);
    
    @androidx.room.Query(value = "DELETE FROM achievements WHERE slotId = :slotId")
    public abstract void clearAchievements(int slotId);
    
    @androidx.room.Query(value = "SELECT * FROM world_objects WHERE slotId = :slotId")
    @org.jetbrains.annotations.NotNull()
    public abstract java.util.List<com.antigravity.voidodyssey.db.WorldObjectEntity> getWorldObjects(int slotId);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertWorldObjects(@org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.db.WorldObjectEntity> objects);
    
    @androidx.room.Query(value = "DELETE FROM world_objects WHERE slotId = :slotId")
    public abstract void clearWorldObjects(int slotId);
    
    @androidx.room.Query(value = "SELECT * FROM settings")
    @org.jetbrains.annotations.NotNull()
    public abstract java.util.List<com.antigravity.voidodyssey.db.GameSetting> getAllSettings();
    
    @androidx.room.Query(value = "SELECT value FROM settings WHERE `key` = :key")
    @org.jetbrains.annotations.Nullable()
    public abstract java.lang.String getSetting(@org.jetbrains.annotations.NotNull()
    java.lang.String key);
    
    @androidx.room.Insert(onConflict = 1)
    public abstract void insertSetting(@org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.db.GameSetting setting);
}