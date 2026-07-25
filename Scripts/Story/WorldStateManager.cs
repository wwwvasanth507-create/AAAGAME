using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Story
{
    public class StateChangeEvent
    {
        public string Key { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Reversible world state engine managing story flags, regional flags, global flags,
    /// settlement states, NPC availability, enemy variants, and weather overrides.
    /// </summary>
    public class WorldStateManager
    {
        private readonly Dictionary<string, string> _flags = new(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<StateChangeEvent> _history = new();

        public event Action<StateChangeEvent>? OnStateChanged;

        public void SetFlag(string key, string value, bool trackHistory = true)
        {
            if (string.IsNullOrEmpty(key)) return;

            string oldValue = _flags.TryGetValue(key, out var val) ? val : string.Empty;
            _flags[key] = value;

            var evt = new StateChangeEvent
            {
                Key = key,
                OldValue = oldValue,
                NewValue = value
            };

            if (trackHistory)
            {
                _history.Push(evt);
            }

            OnStateChanged?.Invoke(evt);
        }

        public string GetFlag(string key, string defaultValue = "")
        {
            return _flags.TryGetValue(key, out var val) ? val : defaultValue;
        }

        public bool HasFlag(string key)
        {
            return _flags.ContainsKey(key);
        }

        public bool RevertLastStateChange()
        {
            if (_history.Count > 0)
            {
                var last = _history.Pop();
                _flags[last.Key] = last.OldValue;
                OnStateChanged?.Invoke(new StateChangeEvent { Key = last.Key, OldValue = last.NewValue, NewValue = last.OldValue });
                return true;
            }
            return false;
        }

        public IReadOnlyDictionary<string, string> ActiveFlags => _flags;

        public void LoadFlags(IDictionary<string, string> flags)
        {
            _flags.Clear();
            _history.Clear();
            if (flags != null)
            {
                foreach (var kvp in flags) _flags[kvp.Key] = kvp.Value;
            }
        }
    }
}
