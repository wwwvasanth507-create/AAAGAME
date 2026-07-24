package com.antigravity.voidodyssey.db;

import androidx.annotation.NonNull;
import androidx.room.DatabaseConfiguration;
import androidx.room.InvalidationTracker;
import androidx.room.RoomDatabase;
import androidx.room.RoomOpenHelper;
import androidx.room.migration.AutoMigrationSpec;
import androidx.room.migration.Migration;
import androidx.room.util.DBUtil;
import androidx.room.util.TableInfo;
import androidx.sqlite.db.SupportSQLiteDatabase;
import androidx.sqlite.db.SupportSQLiteOpenHelper;
import java.lang.Class;
import java.lang.Override;
import java.lang.String;
import java.lang.SuppressWarnings;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import javax.annotation.processing.Generated;

@Generated("androidx.room.RoomProcessor")
@SuppressWarnings({"unchecked", "deprecation"})
public final class GameDatabase_Impl extends GameDatabase {
  private volatile GameDao _gameDao;

  @Override
  @NonNull
  protected SupportSQLiteOpenHelper createOpenHelper(@NonNull final DatabaseConfiguration config) {
    final SupportSQLiteOpenHelper.Callback _openCallback = new RoomOpenHelper(config, new RoomOpenHelper.Delegate(1) {
      @Override
      public void createAllTables(@NonNull final SupportSQLiteDatabase db) {
        db.execSQL("CREATE TABLE IF NOT EXISTS `player_profile` (`slotId` INTEGER NOT NULL, `playerName` TEXT NOT NULL, `level` INTEGER NOT NULL, `xp` INTEGER NOT NULL, `credits` INTEGER NOT NULL, `currentSystem` TEXT NOT NULL, `shipType` TEXT NOT NULL, `hull` REAL NOT NULL, `shield` REAL NOT NULL, `playTime` INTEGER NOT NULL, `worldSeed` INTEGER NOT NULL, PRIMARY KEY(`slotId`))");
        db.execSQL("CREATE TABLE IF NOT EXISTS `inventory` (`id` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, `slotId` INTEGER NOT NULL, `itemId` TEXT NOT NULL, `itemName` TEXT NOT NULL, `quantity` INTEGER NOT NULL, `type` TEXT NOT NULL, `isEquipped` INTEGER NOT NULL, `attributesJson` TEXT NOT NULL)");
        db.execSQL("CREATE TABLE IF NOT EXISTS `quests` (`questId` TEXT NOT NULL, `slotId` INTEGER NOT NULL, `title` TEXT NOT NULL, `description` TEXT NOT NULL, `status` TEXT NOT NULL, `progress` INTEGER NOT NULL, `targetCount` INTEGER NOT NULL, PRIMARY KEY(`questId`))");
        db.execSQL("CREATE TABLE IF NOT EXISTS `skills` (`skillId` TEXT NOT NULL, `slotId` INTEGER NOT NULL, `name` TEXT NOT NULL, `isUnlocked` INTEGER NOT NULL, `level` INTEGER NOT NULL, `maxLevel` INTEGER NOT NULL, PRIMARY KEY(`skillId`))");
        db.execSQL("CREATE TABLE IF NOT EXISTS `achievements` (`achievementId` TEXT NOT NULL, `slotId` INTEGER NOT NULL, `title` TEXT NOT NULL, `description` TEXT NOT NULL, `progress` INTEGER NOT NULL, `targetCount` INTEGER NOT NULL, `isUnlocked` INTEGER NOT NULL, PRIMARY KEY(`achievementId`))");
        db.execSQL("CREATE TABLE IF NOT EXISTS `save_slots` (`slotId` INTEGER NOT NULL, `label` TEXT NOT NULL, `lastSaved` INTEGER NOT NULL, PRIMARY KEY(`slotId`))");
        db.execSQL("CREATE TABLE IF NOT EXISTS `settings` (`key` TEXT NOT NULL, `value` TEXT NOT NULL, PRIMARY KEY(`key`))");
        db.execSQL("CREATE TABLE IF NOT EXISTS `world_objects` (`objectId` TEXT NOT NULL, `slotId` INTEGER NOT NULL, `systemName` TEXT NOT NULL, `type` TEXT NOT NULL, `x` REAL NOT NULL, `y` REAL NOT NULL, `z` REAL NOT NULL, `health` REAL NOT NULL, `faction` TEXT NOT NULL, `customData` TEXT NOT NULL, PRIMARY KEY(`objectId`))");
        db.execSQL("CREATE TABLE IF NOT EXISTS room_master_table (id INTEGER PRIMARY KEY,identity_hash TEXT)");
        db.execSQL("INSERT OR REPLACE INTO room_master_table (id,identity_hash) VALUES(42, 'c8e5c45f0d2d0f39bff61e896adcd4d7')");
      }

      @Override
      public void dropAllTables(@NonNull final SupportSQLiteDatabase db) {
        db.execSQL("DROP TABLE IF EXISTS `player_profile`");
        db.execSQL("DROP TABLE IF EXISTS `inventory`");
        db.execSQL("DROP TABLE IF EXISTS `quests`");
        db.execSQL("DROP TABLE IF EXISTS `skills`");
        db.execSQL("DROP TABLE IF EXISTS `achievements`");
        db.execSQL("DROP TABLE IF EXISTS `save_slots`");
        db.execSQL("DROP TABLE IF EXISTS `settings`");
        db.execSQL("DROP TABLE IF EXISTS `world_objects`");
        final List<? extends RoomDatabase.Callback> _callbacks = mCallbacks;
        if (_callbacks != null) {
          for (RoomDatabase.Callback _callback : _callbacks) {
            _callback.onDestructiveMigration(db);
          }
        }
      }

      @Override
      public void onCreate(@NonNull final SupportSQLiteDatabase db) {
        final List<? extends RoomDatabase.Callback> _callbacks = mCallbacks;
        if (_callbacks != null) {
          for (RoomDatabase.Callback _callback : _callbacks) {
            _callback.onCreate(db);
          }
        }
      }

      @Override
      public void onOpen(@NonNull final SupportSQLiteDatabase db) {
        mDatabase = db;
        internalInitInvalidationTracker(db);
        final List<? extends RoomDatabase.Callback> _callbacks = mCallbacks;
        if (_callbacks != null) {
          for (RoomDatabase.Callback _callback : _callbacks) {
            _callback.onOpen(db);
          }
        }
      }

      @Override
      public void onPreMigrate(@NonNull final SupportSQLiteDatabase db) {
        DBUtil.dropFtsSyncTriggers(db);
      }

      @Override
      public void onPostMigrate(@NonNull final SupportSQLiteDatabase db) {
      }

      @Override
      @NonNull
      public RoomOpenHelper.ValidationResult onValidateSchema(
          @NonNull final SupportSQLiteDatabase db) {
        final HashMap<String, TableInfo.Column> _columnsPlayerProfile = new HashMap<String, TableInfo.Column>(11);
        _columnsPlayerProfile.put("slotId", new TableInfo.Column("slotId", "INTEGER", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("playerName", new TableInfo.Column("playerName", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("level", new TableInfo.Column("level", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("xp", new TableInfo.Column("xp", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("credits", new TableInfo.Column("credits", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("currentSystem", new TableInfo.Column("currentSystem", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("shipType", new TableInfo.Column("shipType", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("hull", new TableInfo.Column("hull", "REAL", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("shield", new TableInfo.Column("shield", "REAL", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("playTime", new TableInfo.Column("playTime", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsPlayerProfile.put("worldSeed", new TableInfo.Column("worldSeed", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysPlayerProfile = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesPlayerProfile = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoPlayerProfile = new TableInfo("player_profile", _columnsPlayerProfile, _foreignKeysPlayerProfile, _indicesPlayerProfile);
        final TableInfo _existingPlayerProfile = TableInfo.read(db, "player_profile");
        if (!_infoPlayerProfile.equals(_existingPlayerProfile)) {
          return new RoomOpenHelper.ValidationResult(false, "player_profile(com.antigravity.voidodyssey.db.PlayerProfile).\n"
                  + " Expected:\n" + _infoPlayerProfile + "\n"
                  + " Found:\n" + _existingPlayerProfile);
        }
        final HashMap<String, TableInfo.Column> _columnsInventory = new HashMap<String, TableInfo.Column>(8);
        _columnsInventory.put("id", new TableInfo.Column("id", "INTEGER", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsInventory.put("slotId", new TableInfo.Column("slotId", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsInventory.put("itemId", new TableInfo.Column("itemId", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsInventory.put("itemName", new TableInfo.Column("itemName", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsInventory.put("quantity", new TableInfo.Column("quantity", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsInventory.put("type", new TableInfo.Column("type", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsInventory.put("isEquipped", new TableInfo.Column("isEquipped", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsInventory.put("attributesJson", new TableInfo.Column("attributesJson", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysInventory = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesInventory = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoInventory = new TableInfo("inventory", _columnsInventory, _foreignKeysInventory, _indicesInventory);
        final TableInfo _existingInventory = TableInfo.read(db, "inventory");
        if (!_infoInventory.equals(_existingInventory)) {
          return new RoomOpenHelper.ValidationResult(false, "inventory(com.antigravity.voidodyssey.db.InventoryItem).\n"
                  + " Expected:\n" + _infoInventory + "\n"
                  + " Found:\n" + _existingInventory);
        }
        final HashMap<String, TableInfo.Column> _columnsQuests = new HashMap<String, TableInfo.Column>(7);
        _columnsQuests.put("questId", new TableInfo.Column("questId", "TEXT", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsQuests.put("slotId", new TableInfo.Column("slotId", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsQuests.put("title", new TableInfo.Column("title", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsQuests.put("description", new TableInfo.Column("description", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsQuests.put("status", new TableInfo.Column("status", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsQuests.put("progress", new TableInfo.Column("progress", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsQuests.put("targetCount", new TableInfo.Column("targetCount", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysQuests = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesQuests = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoQuests = new TableInfo("quests", _columnsQuests, _foreignKeysQuests, _indicesQuests);
        final TableInfo _existingQuests = TableInfo.read(db, "quests");
        if (!_infoQuests.equals(_existingQuests)) {
          return new RoomOpenHelper.ValidationResult(false, "quests(com.antigravity.voidodyssey.db.QuestEntity).\n"
                  + " Expected:\n" + _infoQuests + "\n"
                  + " Found:\n" + _existingQuests);
        }
        final HashMap<String, TableInfo.Column> _columnsSkills = new HashMap<String, TableInfo.Column>(6);
        _columnsSkills.put("skillId", new TableInfo.Column("skillId", "TEXT", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSkills.put("slotId", new TableInfo.Column("slotId", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSkills.put("name", new TableInfo.Column("name", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSkills.put("isUnlocked", new TableInfo.Column("isUnlocked", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSkills.put("level", new TableInfo.Column("level", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSkills.put("maxLevel", new TableInfo.Column("maxLevel", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysSkills = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesSkills = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoSkills = new TableInfo("skills", _columnsSkills, _foreignKeysSkills, _indicesSkills);
        final TableInfo _existingSkills = TableInfo.read(db, "skills");
        if (!_infoSkills.equals(_existingSkills)) {
          return new RoomOpenHelper.ValidationResult(false, "skills(com.antigravity.voidodyssey.db.SkillEntity).\n"
                  + " Expected:\n" + _infoSkills + "\n"
                  + " Found:\n" + _existingSkills);
        }
        final HashMap<String, TableInfo.Column> _columnsAchievements = new HashMap<String, TableInfo.Column>(7);
        _columnsAchievements.put("achievementId", new TableInfo.Column("achievementId", "TEXT", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsAchievements.put("slotId", new TableInfo.Column("slotId", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsAchievements.put("title", new TableInfo.Column("title", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsAchievements.put("description", new TableInfo.Column("description", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsAchievements.put("progress", new TableInfo.Column("progress", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsAchievements.put("targetCount", new TableInfo.Column("targetCount", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsAchievements.put("isUnlocked", new TableInfo.Column("isUnlocked", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysAchievements = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesAchievements = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoAchievements = new TableInfo("achievements", _columnsAchievements, _foreignKeysAchievements, _indicesAchievements);
        final TableInfo _existingAchievements = TableInfo.read(db, "achievements");
        if (!_infoAchievements.equals(_existingAchievements)) {
          return new RoomOpenHelper.ValidationResult(false, "achievements(com.antigravity.voidodyssey.db.AchievementEntity).\n"
                  + " Expected:\n" + _infoAchievements + "\n"
                  + " Found:\n" + _existingAchievements);
        }
        final HashMap<String, TableInfo.Column> _columnsSaveSlots = new HashMap<String, TableInfo.Column>(3);
        _columnsSaveSlots.put("slotId", new TableInfo.Column("slotId", "INTEGER", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSaveSlots.put("label", new TableInfo.Column("label", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSaveSlots.put("lastSaved", new TableInfo.Column("lastSaved", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysSaveSlots = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesSaveSlots = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoSaveSlots = new TableInfo("save_slots", _columnsSaveSlots, _foreignKeysSaveSlots, _indicesSaveSlots);
        final TableInfo _existingSaveSlots = TableInfo.read(db, "save_slots");
        if (!_infoSaveSlots.equals(_existingSaveSlots)) {
          return new RoomOpenHelper.ValidationResult(false, "save_slots(com.antigravity.voidodyssey.db.SaveSlot).\n"
                  + " Expected:\n" + _infoSaveSlots + "\n"
                  + " Found:\n" + _existingSaveSlots);
        }
        final HashMap<String, TableInfo.Column> _columnsSettings = new HashMap<String, TableInfo.Column>(2);
        _columnsSettings.put("key", new TableInfo.Column("key", "TEXT", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsSettings.put("value", new TableInfo.Column("value", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysSettings = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesSettings = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoSettings = new TableInfo("settings", _columnsSettings, _foreignKeysSettings, _indicesSettings);
        final TableInfo _existingSettings = TableInfo.read(db, "settings");
        if (!_infoSettings.equals(_existingSettings)) {
          return new RoomOpenHelper.ValidationResult(false, "settings(com.antigravity.voidodyssey.db.GameSetting).\n"
                  + " Expected:\n" + _infoSettings + "\n"
                  + " Found:\n" + _existingSettings);
        }
        final HashMap<String, TableInfo.Column> _columnsWorldObjects = new HashMap<String, TableInfo.Column>(10);
        _columnsWorldObjects.put("objectId", new TableInfo.Column("objectId", "TEXT", true, 1, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("slotId", new TableInfo.Column("slotId", "INTEGER", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("systemName", new TableInfo.Column("systemName", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("type", new TableInfo.Column("type", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("x", new TableInfo.Column("x", "REAL", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("y", new TableInfo.Column("y", "REAL", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("z", new TableInfo.Column("z", "REAL", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("health", new TableInfo.Column("health", "REAL", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("faction", new TableInfo.Column("faction", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        _columnsWorldObjects.put("customData", new TableInfo.Column("customData", "TEXT", true, 0, null, TableInfo.CREATED_FROM_ENTITY));
        final HashSet<TableInfo.ForeignKey> _foreignKeysWorldObjects = new HashSet<TableInfo.ForeignKey>(0);
        final HashSet<TableInfo.Index> _indicesWorldObjects = new HashSet<TableInfo.Index>(0);
        final TableInfo _infoWorldObjects = new TableInfo("world_objects", _columnsWorldObjects, _foreignKeysWorldObjects, _indicesWorldObjects);
        final TableInfo _existingWorldObjects = TableInfo.read(db, "world_objects");
        if (!_infoWorldObjects.equals(_existingWorldObjects)) {
          return new RoomOpenHelper.ValidationResult(false, "world_objects(com.antigravity.voidodyssey.db.WorldObjectEntity).\n"
                  + " Expected:\n" + _infoWorldObjects + "\n"
                  + " Found:\n" + _existingWorldObjects);
        }
        return new RoomOpenHelper.ValidationResult(true, null);
      }
    }, "c8e5c45f0d2d0f39bff61e896adcd4d7", "dae3dc50c5ff2bf6de777c71f67d83b2");
    final SupportSQLiteOpenHelper.Configuration _sqliteConfig = SupportSQLiteOpenHelper.Configuration.builder(config.context).name(config.name).callback(_openCallback).build();
    final SupportSQLiteOpenHelper _helper = config.sqliteOpenHelperFactory.create(_sqliteConfig);
    return _helper;
  }

  @Override
  @NonNull
  protected InvalidationTracker createInvalidationTracker() {
    final HashMap<String, String> _shadowTablesMap = new HashMap<String, String>(0);
    final HashMap<String, Set<String>> _viewTables = new HashMap<String, Set<String>>(0);
    return new InvalidationTracker(this, _shadowTablesMap, _viewTables, "player_profile","inventory","quests","skills","achievements","save_slots","settings","world_objects");
  }

  @Override
  public void clearAllTables() {
    super.assertNotMainThread();
    final SupportSQLiteDatabase _db = super.getOpenHelper().getWritableDatabase();
    try {
      super.beginTransaction();
      _db.execSQL("DELETE FROM `player_profile`");
      _db.execSQL("DELETE FROM `inventory`");
      _db.execSQL("DELETE FROM `quests`");
      _db.execSQL("DELETE FROM `skills`");
      _db.execSQL("DELETE FROM `achievements`");
      _db.execSQL("DELETE FROM `save_slots`");
      _db.execSQL("DELETE FROM `settings`");
      _db.execSQL("DELETE FROM `world_objects`");
      super.setTransactionSuccessful();
    } finally {
      super.endTransaction();
      _db.query("PRAGMA wal_checkpoint(FULL)").close();
      if (!_db.inTransaction()) {
        _db.execSQL("VACUUM");
      }
    }
  }

  @Override
  @NonNull
  protected Map<Class<?>, List<Class<?>>> getRequiredTypeConverters() {
    final HashMap<Class<?>, List<Class<?>>> _typeConvertersMap = new HashMap<Class<?>, List<Class<?>>>();
    _typeConvertersMap.put(GameDao.class, GameDao_Impl.getRequiredConverters());
    return _typeConvertersMap;
  }

  @Override
  @NonNull
  public Set<Class<? extends AutoMigrationSpec>> getRequiredAutoMigrationSpecs() {
    final HashSet<Class<? extends AutoMigrationSpec>> _autoMigrationSpecsSet = new HashSet<Class<? extends AutoMigrationSpec>>();
    return _autoMigrationSpecsSet;
  }

  @Override
  @NonNull
  public List<Migration> getAutoMigrations(
      @NonNull final Map<Class<? extends AutoMigrationSpec>, AutoMigrationSpec> autoMigrationSpecs) {
    final List<Migration> _autoMigrations = new ArrayList<Migration>();
    return _autoMigrations;
  }

  @Override
  public GameDao gameDao() {
    if (_gameDao != null) {
      return _gameDao;
    } else {
      synchronized(this) {
        if(_gameDao == null) {
          _gameDao = new GameDao_Impl(this);
        }
        return _gameDao;
      }
    }
  }
}
