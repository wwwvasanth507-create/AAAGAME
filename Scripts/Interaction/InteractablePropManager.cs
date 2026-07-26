using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.World;

namespace HeroOfEternia.Interaction
{
    public class RuntimePropInstance
    {
        public string InstanceId { get; set; } = "";
        public WorldObjectDefinition Definition { get; set; } = new();
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public ObjectState CurrentState { get; set; } = ObjectState.Idle;
        public bool IsHighlighted { get; set; } = false;
        public BaseInteractable? InteractableNode { get; set; }
    }

    /// <summary>
    /// Runtime manager responsible for spawning, updating, state management, and interaction dispatching for world object & thing models.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class InteractablePropManager : IInitializable
    {
        private ThingModelDatabase _database = new();
        private readonly Dictionary<string, RuntimePropInstance> _activeInstances = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }
        public ThingModelDatabase Database => _database;

        public event Action<RuntimePropInstance>? OnPropSpawned;
        public event Action<RuntimePropInstance, ObjectState>? OnPropStateChanged;
        public event Action<RuntimePropInstance>? OnPropInteracted;

        public void Initialize()
        {
            if (IsInitialized) return;

            _database = new ThingModelDatabase();

            // Register with ServiceLocator
            ServiceLocator.Register<InteractablePropManager>(this);

            IsInitialized = true;
            Logger.Info("InteractablePropManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _activeInstances.Clear();

            ServiceLocator.Unregister<InteractablePropManager>();
            IsInitialized = false;
            Logger.Info("InteractablePropManager: Shutdown completed.");
        }

        public RuntimePropInstance? SpawnProp(string definitionId, string instanceId, Vector3 position, Vector3 rotation)
        {
            var def = _database.GetObjectDefinition(definitionId);
            if (def == null)
            {
                Logger.Warning($"InteractablePropManager: Definition '{definitionId}' not found.");
                return null;
            }

            var instance = new RuntimePropInstance
            {
                InstanceId = instanceId,
                Definition = def,
                Position = position,
                Rotation = rotation,
                CurrentState = def.DefaultState
            };

            _activeInstances[instanceId] = instance;

            OnPropSpawned?.Invoke(instance);
            Logger.Info($"InteractablePropManager: Spawned prop '{def.DisplayName}' ({instanceId}) at {position}.");
            return instance;
        }

        public bool InteractWithProp(string instanceId, Player.PlayerRoot? player = null)
        {
            if (!_activeInstances.TryGetValue(instanceId, out var instance))
            {
                Logger.Warning($"InteractablePropManager: Prop instance '{instanceId}' not found.");
                return false;
            }

            if (instance.CurrentState == ObjectState.Disabled || instance.CurrentState == ObjectState.Destroyed)
            {
                Logger.Warning($"InteractablePropManager: Prop instance '{instanceId}' cannot be interacted with in state '{instance.CurrentState}'.");
                return false;
            }

            // Execute interaction behavior based on category
            switch (instance.Definition.Category)
            {
                case ObjectCategory.ChestContainer:
                    SetPropState(instanceId, ObjectState.Opened);
                    break;

                case ObjectCategory.ArcaneSwitch:
                    var newState = instance.CurrentState == ObjectState.Active ? ObjectState.Idle : ObjectState.Active;
                    SetPropState(instanceId, newState);
                    break;

                case ObjectCategory.WaystonePillar:
                case ObjectCategory.RelicAltar:
                    SetPropState(instanceId, ObjectState.Active);
                    break;

                case ObjectCategory.DestructibleContainer:
                    SetPropState(instanceId, ObjectState.Destroyed);
                    break;
            }

            // Update world state flag if specified
            if (!string.IsNullOrEmpty(instance.Definition.InteractionSpec.SetWorldFlagOnInteract))
            {
                try
                {
                    var worldState = ServiceLocator.Get<Story.WorldStateManager>();
                    worldState?.SetFlag(instance.Definition.InteractionSpec.SetWorldFlagOnInteract, "true");
                }
                catch
                {
                    // WorldStateManager not registered in lightweight unit test
                }
            }

            OnPropInteracted?.Invoke(instance);
            Logger.Info($"InteractablePropManager: Interacted with prop '{instance.Definition.DisplayName}' ({instanceId}). New State: {instance.CurrentState}.");
            return true;
        }

        public bool SetPropState(string instanceId, ObjectState newState)
        {
            if (!_activeInstances.TryGetValue(instanceId, out var instance)) return false;

            if (instance.CurrentState != newState)
            {
                instance.CurrentState = newState;
                OnPropStateChanged?.Invoke(instance, newState);
                Logger.Info($"InteractablePropManager: Instance '{instanceId}' state set to '{newState}'.");
            }
            return true;
        }

        public RuntimePropInstance? GetInstance(string instanceId)
        {
            return _activeInstances.TryGetValue(instanceId, out var inst) ? inst : null;
        }

        public List<RuntimePropInstance> GetActiveInstances()
        {
            return new List<RuntimePropInstance>(_activeInstances.Values);
        }
    }
}
