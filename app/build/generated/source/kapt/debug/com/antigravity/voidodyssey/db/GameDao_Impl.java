package com.antigravity.voidodyssey.db;

import android.database.Cursor;
import androidx.annotation.NonNull;
import androidx.room.EntityInsertionAdapter;
import androidx.room.RoomDatabase;
import androidx.room.RoomSQLiteQuery;
import androidx.room.SharedSQLiteStatement;
import androidx.room.util.CursorUtil;
import androidx.room.util.DBUtil;
import androidx.sqlite.db.SupportSQLiteStatement;
import java.lang.Class;
import java.lang.Override;
import java.lang.String;
import java.lang.SuppressWarnings;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import javax.annotation.processing.Generated;

@Generated("androidx.room.RoomProcessor")
@SuppressWarnings({"unchecked", "deprecation"})
public final class GameDao_Impl implements GameDao {
  private final RoomDatabase __db;

  private final EntityInsertionAdapter<SaveSlot> __insertionAdapterOfSaveSlot;

  private final EntityInsertionAdapter<PlayerProfile> __insertionAdapterOfPlayerProfile;

  private final EntityInsertionAdapter<InventoryItem> __insertionAdapterOfInventoryItem;

  private final EntityInsertionAdapter<QuestEntity> __insertionAdapterOfQuestEntity;

  private final EntityInsertionAdapter<SkillEntity> __insertionAdapterOfSkillEntity;

  private final EntityInsertionAdapter<AchievementEntity> __insertionAdapterOfAchievementEntity;

  private final EntityInsertionAdapter<WorldObjectEntity> __insertionAdapterOfWorldObjectEntity;

  private final EntityInsertionAdapter<GameSetting> __insertionAdapterOfGameSetting;

  private final SharedSQLiteStatement __preparedStmtOfDeleteSaveSlot;

  private final SharedSQLiteStatement __preparedStmtOfDeletePlayerProfile;

  private final SharedSQLiteStatement __preparedStmtOfDeleteInventoryItem;

  private final SharedSQLiteStatement __preparedStmtOfClearInventory;

  private final SharedSQLiteStatement __preparedStmtOfClearQuests;

  private final SharedSQLiteStatement __preparedStmtOfClearSkills;

  private final SharedSQLiteStatement __preparedStmtOfClearAchievements;

  private final SharedSQLiteStatement __preparedStmtOfClearWorldObjects;

  public GameDao_Impl(@NonNull final RoomDatabase __db) {
    this.__db = __db;
    this.__insertionAdapterOfSaveSlot = new EntityInsertionAdapter<SaveSlot>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `save_slots` (`slotId`,`label`,`lastSaved`) VALUES (?,?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final SaveSlot entity) {
        statement.bindLong(1, entity.getSlotId());
        if (entity.getLabel() == null) {
          statement.bindNull(2);
        } else {
          statement.bindString(2, entity.getLabel());
        }
        statement.bindLong(3, entity.getLastSaved());
      }
    };
    this.__insertionAdapterOfPlayerProfile = new EntityInsertionAdapter<PlayerProfile>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `player_profile` (`slotId`,`playerName`,`level`,`xp`,`credits`,`currentSystem`,`shipType`,`hull`,`shield`,`playTime`,`worldSeed`) VALUES (?,?,?,?,?,?,?,?,?,?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final PlayerProfile entity) {
        statement.bindLong(1, entity.getSlotId());
        if (entity.getPlayerName() == null) {
          statement.bindNull(2);
        } else {
          statement.bindString(2, entity.getPlayerName());
        }
        statement.bindLong(3, entity.getLevel());
        statement.bindLong(4, entity.getXp());
        statement.bindLong(5, entity.getCredits());
        if (entity.getCurrentSystem() == null) {
          statement.bindNull(6);
        } else {
          statement.bindString(6, entity.getCurrentSystem());
        }
        if (entity.getShipType() == null) {
          statement.bindNull(7);
        } else {
          statement.bindString(7, entity.getShipType());
        }
        statement.bindDouble(8, entity.getHull());
        statement.bindDouble(9, entity.getShield());
        statement.bindLong(10, entity.getPlayTime());
        statement.bindLong(11, entity.getWorldSeed());
      }
    };
    this.__insertionAdapterOfInventoryItem = new EntityInsertionAdapter<InventoryItem>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `inventory` (`id`,`slotId`,`itemId`,`itemName`,`quantity`,`type`,`isEquipped`,`attributesJson`) VALUES (nullif(?, 0),?,?,?,?,?,?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final InventoryItem entity) {
        statement.bindLong(1, entity.getId());
        statement.bindLong(2, entity.getSlotId());
        if (entity.getItemId() == null) {
          statement.bindNull(3);
        } else {
          statement.bindString(3, entity.getItemId());
        }
        if (entity.getItemName() == null) {
          statement.bindNull(4);
        } else {
          statement.bindString(4, entity.getItemName());
        }
        statement.bindLong(5, entity.getQuantity());
        if (entity.getType() == null) {
          statement.bindNull(6);
        } else {
          statement.bindString(6, entity.getType());
        }
        final int _tmp = entity.isEquipped() ? 1 : 0;
        statement.bindLong(7, _tmp);
        if (entity.getAttributesJson() == null) {
          statement.bindNull(8);
        } else {
          statement.bindString(8, entity.getAttributesJson());
        }
      }
    };
    this.__insertionAdapterOfQuestEntity = new EntityInsertionAdapter<QuestEntity>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `quests` (`questId`,`slotId`,`title`,`description`,`status`,`progress`,`targetCount`) VALUES (?,?,?,?,?,?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final QuestEntity entity) {
        if (entity.getQuestId() == null) {
          statement.bindNull(1);
        } else {
          statement.bindString(1, entity.getQuestId());
        }
        statement.bindLong(2, entity.getSlotId());
        if (entity.getTitle() == null) {
          statement.bindNull(3);
        } else {
          statement.bindString(3, entity.getTitle());
        }
        if (entity.getDescription() == null) {
          statement.bindNull(4);
        } else {
          statement.bindString(4, entity.getDescription());
        }
        if (entity.getStatus() == null) {
          statement.bindNull(5);
        } else {
          statement.bindString(5, entity.getStatus());
        }
        statement.bindLong(6, entity.getProgress());
        statement.bindLong(7, entity.getTargetCount());
      }
    };
    this.__insertionAdapterOfSkillEntity = new EntityInsertionAdapter<SkillEntity>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `skills` (`skillId`,`slotId`,`name`,`isUnlocked`,`level`,`maxLevel`) VALUES (?,?,?,?,?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final SkillEntity entity) {
        if (entity.getSkillId() == null) {
          statement.bindNull(1);
        } else {
          statement.bindString(1, entity.getSkillId());
        }
        statement.bindLong(2, entity.getSlotId());
        if (entity.getName() == null) {
          statement.bindNull(3);
        } else {
          statement.bindString(3, entity.getName());
        }
        final int _tmp = entity.isUnlocked() ? 1 : 0;
        statement.bindLong(4, _tmp);
        statement.bindLong(5, entity.getLevel());
        statement.bindLong(6, entity.getMaxLevel());
      }
    };
    this.__insertionAdapterOfAchievementEntity = new EntityInsertionAdapter<AchievementEntity>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `achievements` (`achievementId`,`slotId`,`title`,`description`,`progress`,`targetCount`,`isUnlocked`) VALUES (?,?,?,?,?,?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final AchievementEntity entity) {
        if (entity.getAchievementId() == null) {
          statement.bindNull(1);
        } else {
          statement.bindString(1, entity.getAchievementId());
        }
        statement.bindLong(2, entity.getSlotId());
        if (entity.getTitle() == null) {
          statement.bindNull(3);
        } else {
          statement.bindString(3, entity.getTitle());
        }
        if (entity.getDescription() == null) {
          statement.bindNull(4);
        } else {
          statement.bindString(4, entity.getDescription());
        }
        statement.bindLong(5, entity.getProgress());
        statement.bindLong(6, entity.getTargetCount());
        final int _tmp = entity.isUnlocked() ? 1 : 0;
        statement.bindLong(7, _tmp);
      }
    };
    this.__insertionAdapterOfWorldObjectEntity = new EntityInsertionAdapter<WorldObjectEntity>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `world_objects` (`objectId`,`slotId`,`systemName`,`type`,`x`,`y`,`z`,`health`,`faction`,`customData`) VALUES (?,?,?,?,?,?,?,?,?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final WorldObjectEntity entity) {
        if (entity.getObjectId() == null) {
          statement.bindNull(1);
        } else {
          statement.bindString(1, entity.getObjectId());
        }
        statement.bindLong(2, entity.getSlotId());
        if (entity.getSystemName() == null) {
          statement.bindNull(3);
        } else {
          statement.bindString(3, entity.getSystemName());
        }
        if (entity.getType() == null) {
          statement.bindNull(4);
        } else {
          statement.bindString(4, entity.getType());
        }
        statement.bindDouble(5, entity.getX());
        statement.bindDouble(6, entity.getY());
        statement.bindDouble(7, entity.getZ());
        statement.bindDouble(8, entity.getHealth());
        if (entity.getFaction() == null) {
          statement.bindNull(9);
        } else {
          statement.bindString(9, entity.getFaction());
        }
        if (entity.getCustomData() == null) {
          statement.bindNull(10);
        } else {
          statement.bindString(10, entity.getCustomData());
        }
      }
    };
    this.__insertionAdapterOfGameSetting = new EntityInsertionAdapter<GameSetting>(__db) {
      @Override
      @NonNull
      protected String createQuery() {
        return "INSERT OR REPLACE INTO `settings` (`key`,`value`) VALUES (?,?)";
      }

      @Override
      protected void bind(@NonNull final SupportSQLiteStatement statement,
          @NonNull final GameSetting entity) {
        if (entity.getKey() == null) {
          statement.bindNull(1);
        } else {
          statement.bindString(1, entity.getKey());
        }
        if (entity.getValue() == null) {
          statement.bindNull(2);
        } else {
          statement.bindString(2, entity.getValue());
        }
      }
    };
    this.__preparedStmtOfDeleteSaveSlot = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM save_slots WHERE slotId = ?";
        return _query;
      }
    };
    this.__preparedStmtOfDeletePlayerProfile = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM player_profile WHERE slotId = ?";
        return _query;
      }
    };
    this.__preparedStmtOfDeleteInventoryItem = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM inventory WHERE id = ?";
        return _query;
      }
    };
    this.__preparedStmtOfClearInventory = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM inventory WHERE slotId = ?";
        return _query;
      }
    };
    this.__preparedStmtOfClearQuests = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM quests WHERE slotId = ?";
        return _query;
      }
    };
    this.__preparedStmtOfClearSkills = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM skills WHERE slotId = ?";
        return _query;
      }
    };
    this.__preparedStmtOfClearAchievements = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM achievements WHERE slotId = ?";
        return _query;
      }
    };
    this.__preparedStmtOfClearWorldObjects = new SharedSQLiteStatement(__db) {
      @Override
      @NonNull
      public String createQuery() {
        final String _query = "DELETE FROM world_objects WHERE slotId = ?";
        return _query;
      }
    };
  }

  @Override
  public void insertSaveSlot(final SaveSlot slot) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfSaveSlot.insert(slot);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertPlayerProfile(final PlayerProfile profile) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfPlayerProfile.insert(profile);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertInventoryItem(final InventoryItem item) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfInventoryItem.insert(item);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertInventoryItems(final List<InventoryItem> items) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfInventoryItem.insert(items);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertQuests(final List<QuestEntity> quests) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfQuestEntity.insert(quests);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertSkills(final List<SkillEntity> skills) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfSkillEntity.insert(skills);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertAchievements(final List<AchievementEntity> achievements) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfAchievementEntity.insert(achievements);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertWorldObjects(final List<WorldObjectEntity> objects) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfWorldObjectEntity.insert(objects);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void insertSetting(final GameSetting setting) {
    __db.assertNotSuspendingTransaction();
    __db.beginTransaction();
    try {
      __insertionAdapterOfGameSetting.insert(setting);
      __db.setTransactionSuccessful();
    } finally {
      __db.endTransaction();
    }
  }

  @Override
  public void deleteSaveSlot(final int slotId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfDeleteSaveSlot.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, slotId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfDeleteSaveSlot.release(_stmt);
    }
  }

  @Override
  public void deletePlayerProfile(final int slotId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfDeletePlayerProfile.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, slotId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfDeletePlayerProfile.release(_stmt);
    }
  }

  @Override
  public void deleteInventoryItem(final long itemId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfDeleteInventoryItem.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, itemId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfDeleteInventoryItem.release(_stmt);
    }
  }

  @Override
  public void clearInventory(final int slotId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfClearInventory.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, slotId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfClearInventory.release(_stmt);
    }
  }

  @Override
  public void clearQuests(final int slotId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfClearQuests.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, slotId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfClearQuests.release(_stmt);
    }
  }

  @Override
  public void clearSkills(final int slotId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfClearSkills.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, slotId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfClearSkills.release(_stmt);
    }
  }

  @Override
  public void clearAchievements(final int slotId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfClearAchievements.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, slotId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfClearAchievements.release(_stmt);
    }
  }

  @Override
  public void clearWorldObjects(final int slotId) {
    __db.assertNotSuspendingTransaction();
    final SupportSQLiteStatement _stmt = __preparedStmtOfClearWorldObjects.acquire();
    int _argIndex = 1;
    _stmt.bindLong(_argIndex, slotId);
    try {
      __db.beginTransaction();
      try {
        _stmt.executeUpdateDelete();
        __db.setTransactionSuccessful();
      } finally {
        __db.endTransaction();
      }
    } finally {
      __preparedStmtOfClearWorldObjects.release(_stmt);
    }
  }

  @Override
  public List<SaveSlot> getAllSaveSlots() {
    final String _sql = "SELECT * FROM save_slots ORDER BY lastSaved DESC";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 0);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfSlotId = CursorUtil.getColumnIndexOrThrow(_cursor, "slotId");
      final int _cursorIndexOfLabel = CursorUtil.getColumnIndexOrThrow(_cursor, "label");
      final int _cursorIndexOfLastSaved = CursorUtil.getColumnIndexOrThrow(_cursor, "lastSaved");
      final List<SaveSlot> _result = new ArrayList<SaveSlot>(_cursor.getCount());
      while (_cursor.moveToNext()) {
        final SaveSlot _item;
        final int _tmpSlotId;
        _tmpSlotId = _cursor.getInt(_cursorIndexOfSlotId);
        final String _tmpLabel;
        if (_cursor.isNull(_cursorIndexOfLabel)) {
          _tmpLabel = null;
        } else {
          _tmpLabel = _cursor.getString(_cursorIndexOfLabel);
        }
        final long _tmpLastSaved;
        _tmpLastSaved = _cursor.getLong(_cursorIndexOfLastSaved);
        _item = new SaveSlot(_tmpSlotId,_tmpLabel,_tmpLastSaved);
        _result.add(_item);
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public PlayerProfile getPlayerProfile(final int slotId) {
    final String _sql = "SELECT * FROM player_profile WHERE slotId = ?";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 1);
    int _argIndex = 1;
    _statement.bindLong(_argIndex, slotId);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfSlotId = CursorUtil.getColumnIndexOrThrow(_cursor, "slotId");
      final int _cursorIndexOfPlayerName = CursorUtil.getColumnIndexOrThrow(_cursor, "playerName");
      final int _cursorIndexOfLevel = CursorUtil.getColumnIndexOrThrow(_cursor, "level");
      final int _cursorIndexOfXp = CursorUtil.getColumnIndexOrThrow(_cursor, "xp");
      final int _cursorIndexOfCredits = CursorUtil.getColumnIndexOrThrow(_cursor, "credits");
      final int _cursorIndexOfCurrentSystem = CursorUtil.getColumnIndexOrThrow(_cursor, "currentSystem");
      final int _cursorIndexOfShipType = CursorUtil.getColumnIndexOrThrow(_cursor, "shipType");
      final int _cursorIndexOfHull = CursorUtil.getColumnIndexOrThrow(_cursor, "hull");
      final int _cursorIndexOfShield = CursorUtil.getColumnIndexOrThrow(_cursor, "shield");
      final int _cursorIndexOfPlayTime = CursorUtil.getColumnIndexOrThrow(_cursor, "playTime");
      final int _cursorIndexOfWorldSeed = CursorUtil.getColumnIndexOrThrow(_cursor, "worldSeed");
      final PlayerProfile _result;
      if (_cursor.moveToFirst()) {
        final int _tmpSlotId;
        _tmpSlotId = _cursor.getInt(_cursorIndexOfSlotId);
        final String _tmpPlayerName;
        if (_cursor.isNull(_cursorIndexOfPlayerName)) {
          _tmpPlayerName = null;
        } else {
          _tmpPlayerName = _cursor.getString(_cursorIndexOfPlayerName);
        }
        final int _tmpLevel;
        _tmpLevel = _cursor.getInt(_cursorIndexOfLevel);
        final int _tmpXp;
        _tmpXp = _cursor.getInt(_cursorIndexOfXp);
        final long _tmpCredits;
        _tmpCredits = _cursor.getLong(_cursorIndexOfCredits);
        final String _tmpCurrentSystem;
        if (_cursor.isNull(_cursorIndexOfCurrentSystem)) {
          _tmpCurrentSystem = null;
        } else {
          _tmpCurrentSystem = _cursor.getString(_cursorIndexOfCurrentSystem);
        }
        final String _tmpShipType;
        if (_cursor.isNull(_cursorIndexOfShipType)) {
          _tmpShipType = null;
        } else {
          _tmpShipType = _cursor.getString(_cursorIndexOfShipType);
        }
        final float _tmpHull;
        _tmpHull = _cursor.getFloat(_cursorIndexOfHull);
        final float _tmpShield;
        _tmpShield = _cursor.getFloat(_cursorIndexOfShield);
        final long _tmpPlayTime;
        _tmpPlayTime = _cursor.getLong(_cursorIndexOfPlayTime);
        final long _tmpWorldSeed;
        _tmpWorldSeed = _cursor.getLong(_cursorIndexOfWorldSeed);
        _result = new PlayerProfile(_tmpSlotId,_tmpPlayerName,_tmpLevel,_tmpXp,_tmpCredits,_tmpCurrentSystem,_tmpShipType,_tmpHull,_tmpShield,_tmpPlayTime,_tmpWorldSeed);
      } else {
        _result = null;
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public List<InventoryItem> getInventory(final int slotId) {
    final String _sql = "SELECT * FROM inventory WHERE slotId = ?";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 1);
    int _argIndex = 1;
    _statement.bindLong(_argIndex, slotId);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfId = CursorUtil.getColumnIndexOrThrow(_cursor, "id");
      final int _cursorIndexOfSlotId = CursorUtil.getColumnIndexOrThrow(_cursor, "slotId");
      final int _cursorIndexOfItemId = CursorUtil.getColumnIndexOrThrow(_cursor, "itemId");
      final int _cursorIndexOfItemName = CursorUtil.getColumnIndexOrThrow(_cursor, "itemName");
      final int _cursorIndexOfQuantity = CursorUtil.getColumnIndexOrThrow(_cursor, "quantity");
      final int _cursorIndexOfType = CursorUtil.getColumnIndexOrThrow(_cursor, "type");
      final int _cursorIndexOfIsEquipped = CursorUtil.getColumnIndexOrThrow(_cursor, "isEquipped");
      final int _cursorIndexOfAttributesJson = CursorUtil.getColumnIndexOrThrow(_cursor, "attributesJson");
      final List<InventoryItem> _result = new ArrayList<InventoryItem>(_cursor.getCount());
      while (_cursor.moveToNext()) {
        final InventoryItem _item;
        final long _tmpId;
        _tmpId = _cursor.getLong(_cursorIndexOfId);
        final int _tmpSlotId;
        _tmpSlotId = _cursor.getInt(_cursorIndexOfSlotId);
        final String _tmpItemId;
        if (_cursor.isNull(_cursorIndexOfItemId)) {
          _tmpItemId = null;
        } else {
          _tmpItemId = _cursor.getString(_cursorIndexOfItemId);
        }
        final String _tmpItemName;
        if (_cursor.isNull(_cursorIndexOfItemName)) {
          _tmpItemName = null;
        } else {
          _tmpItemName = _cursor.getString(_cursorIndexOfItemName);
        }
        final int _tmpQuantity;
        _tmpQuantity = _cursor.getInt(_cursorIndexOfQuantity);
        final String _tmpType;
        if (_cursor.isNull(_cursorIndexOfType)) {
          _tmpType = null;
        } else {
          _tmpType = _cursor.getString(_cursorIndexOfType);
        }
        final boolean _tmpIsEquipped;
        final int _tmp;
        _tmp = _cursor.getInt(_cursorIndexOfIsEquipped);
        _tmpIsEquipped = _tmp != 0;
        final String _tmpAttributesJson;
        if (_cursor.isNull(_cursorIndexOfAttributesJson)) {
          _tmpAttributesJson = null;
        } else {
          _tmpAttributesJson = _cursor.getString(_cursorIndexOfAttributesJson);
        }
        _item = new InventoryItem(_tmpId,_tmpSlotId,_tmpItemId,_tmpItemName,_tmpQuantity,_tmpType,_tmpIsEquipped,_tmpAttributesJson);
        _result.add(_item);
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public List<QuestEntity> getQuests(final int slotId) {
    final String _sql = "SELECT * FROM quests WHERE slotId = ?";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 1);
    int _argIndex = 1;
    _statement.bindLong(_argIndex, slotId);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfQuestId = CursorUtil.getColumnIndexOrThrow(_cursor, "questId");
      final int _cursorIndexOfSlotId = CursorUtil.getColumnIndexOrThrow(_cursor, "slotId");
      final int _cursorIndexOfTitle = CursorUtil.getColumnIndexOrThrow(_cursor, "title");
      final int _cursorIndexOfDescription = CursorUtil.getColumnIndexOrThrow(_cursor, "description");
      final int _cursorIndexOfStatus = CursorUtil.getColumnIndexOrThrow(_cursor, "status");
      final int _cursorIndexOfProgress = CursorUtil.getColumnIndexOrThrow(_cursor, "progress");
      final int _cursorIndexOfTargetCount = CursorUtil.getColumnIndexOrThrow(_cursor, "targetCount");
      final List<QuestEntity> _result = new ArrayList<QuestEntity>(_cursor.getCount());
      while (_cursor.moveToNext()) {
        final QuestEntity _item;
        final String _tmpQuestId;
        if (_cursor.isNull(_cursorIndexOfQuestId)) {
          _tmpQuestId = null;
        } else {
          _tmpQuestId = _cursor.getString(_cursorIndexOfQuestId);
        }
        final int _tmpSlotId;
        _tmpSlotId = _cursor.getInt(_cursorIndexOfSlotId);
        final String _tmpTitle;
        if (_cursor.isNull(_cursorIndexOfTitle)) {
          _tmpTitle = null;
        } else {
          _tmpTitle = _cursor.getString(_cursorIndexOfTitle);
        }
        final String _tmpDescription;
        if (_cursor.isNull(_cursorIndexOfDescription)) {
          _tmpDescription = null;
        } else {
          _tmpDescription = _cursor.getString(_cursorIndexOfDescription);
        }
        final String _tmpStatus;
        if (_cursor.isNull(_cursorIndexOfStatus)) {
          _tmpStatus = null;
        } else {
          _tmpStatus = _cursor.getString(_cursorIndexOfStatus);
        }
        final int _tmpProgress;
        _tmpProgress = _cursor.getInt(_cursorIndexOfProgress);
        final int _tmpTargetCount;
        _tmpTargetCount = _cursor.getInt(_cursorIndexOfTargetCount);
        _item = new QuestEntity(_tmpQuestId,_tmpSlotId,_tmpTitle,_tmpDescription,_tmpStatus,_tmpProgress,_tmpTargetCount);
        _result.add(_item);
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public List<SkillEntity> getSkills(final int slotId) {
    final String _sql = "SELECT * FROM skills WHERE slotId = ?";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 1);
    int _argIndex = 1;
    _statement.bindLong(_argIndex, slotId);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfSkillId = CursorUtil.getColumnIndexOrThrow(_cursor, "skillId");
      final int _cursorIndexOfSlotId = CursorUtil.getColumnIndexOrThrow(_cursor, "slotId");
      final int _cursorIndexOfName = CursorUtil.getColumnIndexOrThrow(_cursor, "name");
      final int _cursorIndexOfIsUnlocked = CursorUtil.getColumnIndexOrThrow(_cursor, "isUnlocked");
      final int _cursorIndexOfLevel = CursorUtil.getColumnIndexOrThrow(_cursor, "level");
      final int _cursorIndexOfMaxLevel = CursorUtil.getColumnIndexOrThrow(_cursor, "maxLevel");
      final List<SkillEntity> _result = new ArrayList<SkillEntity>(_cursor.getCount());
      while (_cursor.moveToNext()) {
        final SkillEntity _item;
        final String _tmpSkillId;
        if (_cursor.isNull(_cursorIndexOfSkillId)) {
          _tmpSkillId = null;
        } else {
          _tmpSkillId = _cursor.getString(_cursorIndexOfSkillId);
        }
        final int _tmpSlotId;
        _tmpSlotId = _cursor.getInt(_cursorIndexOfSlotId);
        final String _tmpName;
        if (_cursor.isNull(_cursorIndexOfName)) {
          _tmpName = null;
        } else {
          _tmpName = _cursor.getString(_cursorIndexOfName);
        }
        final boolean _tmpIsUnlocked;
        final int _tmp;
        _tmp = _cursor.getInt(_cursorIndexOfIsUnlocked);
        _tmpIsUnlocked = _tmp != 0;
        final int _tmpLevel;
        _tmpLevel = _cursor.getInt(_cursorIndexOfLevel);
        final int _tmpMaxLevel;
        _tmpMaxLevel = _cursor.getInt(_cursorIndexOfMaxLevel);
        _item = new SkillEntity(_tmpSkillId,_tmpSlotId,_tmpName,_tmpIsUnlocked,_tmpLevel,_tmpMaxLevel);
        _result.add(_item);
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public List<AchievementEntity> getAchievements(final int slotId) {
    final String _sql = "SELECT * FROM achievements WHERE slotId = ?";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 1);
    int _argIndex = 1;
    _statement.bindLong(_argIndex, slotId);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfAchievementId = CursorUtil.getColumnIndexOrThrow(_cursor, "achievementId");
      final int _cursorIndexOfSlotId = CursorUtil.getColumnIndexOrThrow(_cursor, "slotId");
      final int _cursorIndexOfTitle = CursorUtil.getColumnIndexOrThrow(_cursor, "title");
      final int _cursorIndexOfDescription = CursorUtil.getColumnIndexOrThrow(_cursor, "description");
      final int _cursorIndexOfProgress = CursorUtil.getColumnIndexOrThrow(_cursor, "progress");
      final int _cursorIndexOfTargetCount = CursorUtil.getColumnIndexOrThrow(_cursor, "targetCount");
      final int _cursorIndexOfIsUnlocked = CursorUtil.getColumnIndexOrThrow(_cursor, "isUnlocked");
      final List<AchievementEntity> _result = new ArrayList<AchievementEntity>(_cursor.getCount());
      while (_cursor.moveToNext()) {
        final AchievementEntity _item;
        final String _tmpAchievementId;
        if (_cursor.isNull(_cursorIndexOfAchievementId)) {
          _tmpAchievementId = null;
        } else {
          _tmpAchievementId = _cursor.getString(_cursorIndexOfAchievementId);
        }
        final int _tmpSlotId;
        _tmpSlotId = _cursor.getInt(_cursorIndexOfSlotId);
        final String _tmpTitle;
        if (_cursor.isNull(_cursorIndexOfTitle)) {
          _tmpTitle = null;
        } else {
          _tmpTitle = _cursor.getString(_cursorIndexOfTitle);
        }
        final String _tmpDescription;
        if (_cursor.isNull(_cursorIndexOfDescription)) {
          _tmpDescription = null;
        } else {
          _tmpDescription = _cursor.getString(_cursorIndexOfDescription);
        }
        final int _tmpProgress;
        _tmpProgress = _cursor.getInt(_cursorIndexOfProgress);
        final int _tmpTargetCount;
        _tmpTargetCount = _cursor.getInt(_cursorIndexOfTargetCount);
        final boolean _tmpIsUnlocked;
        final int _tmp;
        _tmp = _cursor.getInt(_cursorIndexOfIsUnlocked);
        _tmpIsUnlocked = _tmp != 0;
        _item = new AchievementEntity(_tmpAchievementId,_tmpSlotId,_tmpTitle,_tmpDescription,_tmpProgress,_tmpTargetCount,_tmpIsUnlocked);
        _result.add(_item);
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public List<WorldObjectEntity> getWorldObjects(final int slotId) {
    final String _sql = "SELECT * FROM world_objects WHERE slotId = ?";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 1);
    int _argIndex = 1;
    _statement.bindLong(_argIndex, slotId);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfObjectId = CursorUtil.getColumnIndexOrThrow(_cursor, "objectId");
      final int _cursorIndexOfSlotId = CursorUtil.getColumnIndexOrThrow(_cursor, "slotId");
      final int _cursorIndexOfSystemName = CursorUtil.getColumnIndexOrThrow(_cursor, "systemName");
      final int _cursorIndexOfType = CursorUtil.getColumnIndexOrThrow(_cursor, "type");
      final int _cursorIndexOfX = CursorUtil.getColumnIndexOrThrow(_cursor, "x");
      final int _cursorIndexOfY = CursorUtil.getColumnIndexOrThrow(_cursor, "y");
      final int _cursorIndexOfZ = CursorUtil.getColumnIndexOrThrow(_cursor, "z");
      final int _cursorIndexOfHealth = CursorUtil.getColumnIndexOrThrow(_cursor, "health");
      final int _cursorIndexOfFaction = CursorUtil.getColumnIndexOrThrow(_cursor, "faction");
      final int _cursorIndexOfCustomData = CursorUtil.getColumnIndexOrThrow(_cursor, "customData");
      final List<WorldObjectEntity> _result = new ArrayList<WorldObjectEntity>(_cursor.getCount());
      while (_cursor.moveToNext()) {
        final WorldObjectEntity _item;
        final String _tmpObjectId;
        if (_cursor.isNull(_cursorIndexOfObjectId)) {
          _tmpObjectId = null;
        } else {
          _tmpObjectId = _cursor.getString(_cursorIndexOfObjectId);
        }
        final int _tmpSlotId;
        _tmpSlotId = _cursor.getInt(_cursorIndexOfSlotId);
        final String _tmpSystemName;
        if (_cursor.isNull(_cursorIndexOfSystemName)) {
          _tmpSystemName = null;
        } else {
          _tmpSystemName = _cursor.getString(_cursorIndexOfSystemName);
        }
        final String _tmpType;
        if (_cursor.isNull(_cursorIndexOfType)) {
          _tmpType = null;
        } else {
          _tmpType = _cursor.getString(_cursorIndexOfType);
        }
        final float _tmpX;
        _tmpX = _cursor.getFloat(_cursorIndexOfX);
        final float _tmpY;
        _tmpY = _cursor.getFloat(_cursorIndexOfY);
        final float _tmpZ;
        _tmpZ = _cursor.getFloat(_cursorIndexOfZ);
        final float _tmpHealth;
        _tmpHealth = _cursor.getFloat(_cursorIndexOfHealth);
        final String _tmpFaction;
        if (_cursor.isNull(_cursorIndexOfFaction)) {
          _tmpFaction = null;
        } else {
          _tmpFaction = _cursor.getString(_cursorIndexOfFaction);
        }
        final String _tmpCustomData;
        if (_cursor.isNull(_cursorIndexOfCustomData)) {
          _tmpCustomData = null;
        } else {
          _tmpCustomData = _cursor.getString(_cursorIndexOfCustomData);
        }
        _item = new WorldObjectEntity(_tmpObjectId,_tmpSlotId,_tmpSystemName,_tmpType,_tmpX,_tmpY,_tmpZ,_tmpHealth,_tmpFaction,_tmpCustomData);
        _result.add(_item);
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public List<GameSetting> getAllSettings() {
    final String _sql = "SELECT * FROM settings";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 0);
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final int _cursorIndexOfKey = CursorUtil.getColumnIndexOrThrow(_cursor, "key");
      final int _cursorIndexOfValue = CursorUtil.getColumnIndexOrThrow(_cursor, "value");
      final List<GameSetting> _result = new ArrayList<GameSetting>(_cursor.getCount());
      while (_cursor.moveToNext()) {
        final GameSetting _item;
        final String _tmpKey;
        if (_cursor.isNull(_cursorIndexOfKey)) {
          _tmpKey = null;
        } else {
          _tmpKey = _cursor.getString(_cursorIndexOfKey);
        }
        final String _tmpValue;
        if (_cursor.isNull(_cursorIndexOfValue)) {
          _tmpValue = null;
        } else {
          _tmpValue = _cursor.getString(_cursorIndexOfValue);
        }
        _item = new GameSetting(_tmpKey,_tmpValue);
        _result.add(_item);
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @Override
  public String getSetting(final String key) {
    final String _sql = "SELECT value FROM settings WHERE `key` = ?";
    final RoomSQLiteQuery _statement = RoomSQLiteQuery.acquire(_sql, 1);
    int _argIndex = 1;
    if (key == null) {
      _statement.bindNull(_argIndex);
    } else {
      _statement.bindString(_argIndex, key);
    }
    __db.assertNotSuspendingTransaction();
    final Cursor _cursor = DBUtil.query(__db, _statement, false, null);
    try {
      final String _result;
      if (_cursor.moveToFirst()) {
        if (_cursor.isNull(0)) {
          _result = null;
        } else {
          _result = _cursor.getString(0);
        }
      } else {
        _result = null;
      }
      return _result;
    } finally {
      _cursor.close();
      _statement.release();
    }
  }

  @NonNull
  public static List<Class<?>> getRequiredConverters() {
    return Collections.emptyList();
  }
}
