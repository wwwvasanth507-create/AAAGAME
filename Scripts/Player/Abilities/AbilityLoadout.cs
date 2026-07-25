using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player.Abilities
{
    /// <summary>
    /// Represents a single ability loadout configuration.
    /// Players can have multiple loadouts for different situations.
    /// </summary>
    public class AbilityLoadout
    {
        public string LoadoutName { get; set; } = "Default";
        public string[] SlotAssignments { get; private set; } = new string[8]; // Primary 4 + Secondary 4
        public string[] PassiveSlots { get; set; } = new string[4];
        public string UltimateSlot { get; set; } = string.Empty;
        public string[] QuickSlots { get; set; } = new string[4];

        public const int PrimarySlotCount = 4;
        public const int SecondarySlotCount = 4;
        public const int PassiveSlotCount = 4;
        public const int QuickSlotCount = 4;

        public AbilityLoadout(string name)
        {
            LoadoutName = name;
            for (int i = 0; i < SlotAssignments.Length; i++) SlotAssignments[i] = string.Empty;
            for (int i = 0; i < PassiveSlots.Length; i++) PassiveSlots[i] = string.Empty;
            for (int i = 0; i < QuickSlots.Length; i++) QuickSlots[i] = string.Empty;
        }

        public void SetPrimarySlot(int index, string abilityId)
        {
            if (index >= 0 && index < PrimarySlotCount)
                SlotAssignments[index] = abilityId ?? string.Empty;
        }

        public void SetSecondarySlot(int index, string abilityId)
        {
            if (index >= 0 && index < SecondarySlotCount)
                SlotAssignments[PrimarySlotCount + index] = abilityId ?? string.Empty;
        }

        public string GetPrimarySlot(int index) =>
            (index >= 0 && index < PrimarySlotCount) ? SlotAssignments[index] : string.Empty;

        public string GetSecondarySlot(int index) =>
            (index >= 0 && index < SecondarySlotCount) ? SlotAssignments[PrimarySlotCount + index] : string.Empty;

        public LoadoutSaveData CreateSaveData()
        {
            return new LoadoutSaveData
            {
                LoadoutName = LoadoutName,
                SlotAssignments = SlotAssignments.ToList(),
                PassiveSlots = PassiveSlots.ToList(),
                UltimateSlot = UltimateSlot,
                QuickSlots = QuickSlots.ToList(),
                Version = 1
            };
        }

        public void LoadFromSaveData(LoadoutSaveData data)
        {
            if (data == null) return;
            LoadoutName = data.LoadoutName;
            if (data.SlotAssignments != null && data.SlotAssignments.Count == SlotAssignments.Length)
                SlotAssignments = data.SlotAssignments.ToArray();
            if (data.PassiveSlots != null && data.PassiveSlots.Count == PassiveSlots.Length)
                PassiveSlots = data.PassiveSlots.ToArray();
            UltimateSlot = data.UltimateSlot ?? string.Empty;
            if (data.QuickSlots != null && data.QuickSlots.Count == QuickSlots.Length)
                QuickSlots = data.QuickSlots.ToArray();
        }

        public override string ToString() =>
            $"[{LoadoutName}] Primary: {string.Join(",", SlotAssignments.Take(PrimarySlotCount))} | Ultimate: {UltimateSlot}";
    }

    /// <summary>
    /// Manages multiple ability loadouts for the player.
    /// Supports saving, loading, and switching between configurations.
    /// </summary>
    public class LoadoutManager
    {
        private readonly List<AbilityLoadout> _loadouts = new();
        private int _activeLoadoutIndex = 0;

        public event Action<int, AbilityLoadout>? OnLoadoutChanged;
        public event Action<int>? OnActiveLoadoutSwitched;

        public const int MaxLoadouts = 6;

        public LoadoutManager()
        {
            // Create default loadout
            _loadouts.Add(new AbilityLoadout("Adventure"));
            _loadouts.Add(new AbilityLoadout("Combat"));
            _loadouts.Add(new AbilityLoadout("Exploration"));
        }

        public AbilityLoadout ActiveLoadout => _loadouts[_activeLoadoutIndex];
        public int ActiveLoadoutIndex => _activeLoadoutIndex;
        public IReadOnlyList<AbilityLoadout> AllLoadouts => _loadouts;

        public bool SwitchLoadout(int index)
        {
            if (index < 0 || index >= _loadouts.Count) return false;
            _activeLoadoutIndex = index;
            OnActiveLoadoutSwitched?.Invoke(index);
            Logger.Info($"LoadoutManager: Switched to '{_loadouts[index].LoadoutName}' (index {index})");
            return true;
        }

        public AbilityLoadout CreateLoadout(string name)
        {
            if (_loadouts.Count >= MaxLoadouts)
            {
                Logger.Warning($"LoadoutManager: Maximum loadouts ({MaxLoadouts}) reached.");
                return _loadouts[0];
            }

            var loadout = new AbilityLoadout(name);
            _loadouts.Add(loadout);
            return loadout;
        }

        public bool DeleteLoadout(int index)
        {
            if (_loadouts.Count <= 1 || index < 0 || index >= _loadouts.Count)
                return false;

            _loadouts.RemoveAt(index);
            if (_activeLoadoutIndex >= _loadouts.Count)
                _activeLoadoutIndex = _loadouts.Count - 1;
            return true;
        }

        public bool RenameLoadout(int index, string newName)
        {
            if (index < 0 || index >= _loadouts.Count || string.IsNullOrWhiteSpace(newName))
                return false;
            _loadouts[index].LoadoutName = newName;
            return true;
        }

        public void AssignAbility(int slotIndex, string abilityId)
        {
            ActiveLoadout.SetPrimarySlot(slotIndex, abilityId);
            OnLoadoutChanged?.Invoke(_activeLoadoutIndex, ActiveLoadout);
        }

        public string GetActiveSlotAbility(int slotIndex)
        {
            return ActiveLoadout.GetPrimarySlot(slotIndex);
        }

        // Save/Load
        public LoadoutManagerSaveData CreateSaveData()
        {
            var data = new LoadoutManagerSaveData
            {
                ActiveIndex = _activeLoadoutIndex,
                Version = 1
            };
            foreach (var loadout in _loadouts)
                data.Loadouts.Add(loadout.CreateSaveData());
            return data;
        }

        public void LoadFromSaveData(LoadoutManagerSaveData data)
        {
            if (data == null) return;
            _loadouts.Clear();
            foreach (var ld in data.Loadouts)
            {
                var loadout = new AbilityLoadout(ld.LoadoutName);
                loadout.LoadFromSaveData(ld);
                _loadouts.Add(loadout);
            }
            _activeLoadoutIndex = Math.Clamp(data.ActiveIndex, 0, Math.Max(0, _loadouts.Count - 1));
            if (_loadouts.Count == 0)
                _loadouts.Add(new AbilityLoadout("Default"));
        }
    }

    // Save data classes
    public class LoadoutSaveData
    {
        public string LoadoutName { get; set; } = "Default";
        public List<string> SlotAssignments { get; set; } = new();
        public List<string> PassiveSlots { get; set; } = new();
        public string UltimateSlot { get; set; } = string.Empty;
        public List<string> QuickSlots { get; set; } = new();
        public int Version { get; set; } = 1;
    }

    public class LoadoutManagerSaveData
    {
        public int ActiveIndex { get; set; } = 0;
        public List<LoadoutSaveData> Loadouts { get; set; } = new();
        public int Version { get; set; } = 1;
    }
}