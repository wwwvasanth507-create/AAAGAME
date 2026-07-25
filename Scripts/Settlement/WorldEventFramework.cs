using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Settlement
{
    /// <summary>
    /// Reusable world event framework for settlements.
    /// Manages event templates, triggers, lifecycle, and effects.
    /// Supports Market Day, Festival, Harvest, Storm Preparation,
    /// Monster Alert, Merchant Arrival, Traveling Caravan, Resource Shortage,
    /// and future events.
    /// </summary>
    public class WorldEventFramework
    {
        private readonly Dictionary<WorldEventType, WorldEventTemplate> _templates = new();
        private readonly Dictionary<string, WorldEventInstance> _activeEvents = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, float> _cooldowns = new(StringComparer.OrdinalIgnoreCase); // settlementId -> remaining days
        private readonly Random _rng = new();
        private int _eventIdCounter = 0;
        private bool _isLoaded = false;

        public bool IsLoaded => _isLoaded;
        public int ActiveEventCount => _activeEvents.Count;

        /// <summary>Load default event templates.</summary>
        public void Load()
        {
            _templates.Clear();
            _activeEvents.Clear();
            _cooldowns.Clear();
            LoadDefaultTemplates();
            _isLoaded = true;
            Logger.Info($"WorldEventFramework: Loaded {_templates.Count} event templates.");
        }

        /// <summary>Register a custom event template.</summary>
        public void RegisterTemplate(WorldEventTemplate template)
        {
            if (template == null) return;
            _templates[template.Type] = template;
        }

        /// <summary>Get a template by event type.</summary>
        public WorldEventTemplate? GetTemplate(WorldEventType type)
        {
            return _templates.TryGetValue(type, out var template) ? template : null;
        }

        /// <summary>Get all templates.</summary>
        public List<WorldEventTemplate> GetAllTemplates()
        {
            return new List<WorldEventTemplate>(_templates.Values);
        }

        /// <summary>Trigger an event in a settlement.</summary>
        public WorldEventInstance? TriggerEvent(WorldEventType type, string settlementId, SettlementData settlement,
            EventSeverity? overrideSeverity = null)
        {
            if (!_templates.TryGetValue(type, out var template)) return null;
            if (settlement == null) return null;

            // Check cooldown
            string cooldownKey = $"{settlementId}_{type}";
            if (_cooldowns.TryGetValue(cooldownKey, out var remaining) && remaining > 0f)
                return null;

            // Check prosperity bounds
            float prosperityValue = (float)settlement.Prosperity;
            if (prosperityValue < template.MinProsperity || prosperityValue > template.MaxProsperity)
                return null;

            // Check settlement type
            if (template.AllowedSettlementTypes.Count > 0 && 
                !template.AllowedSettlementTypes.Contains(settlement.Type))
                return null;

            var instance = new WorldEventInstance
            {
                EventId = $"evt_{settlementId}_{type}_{++_eventIdCounter}",
                Type = type,
                Severity = overrideSeverity ?? template.DefaultSeverity,
                Phase = EventPhase.Active,
                SettlementId = settlementId,
                DisplayName = template.DisplayName,
                Description = template.Description,
                DurationDays = template.BaseDurationDays * (1f + (float)(_rng.NextDouble() * 0.4f - 0.2f)),
                ElapsedDays = 0f,
                TriggerDay = 0,
                Effects = new Dictionary<string, float>(template.DefaultEffects),
                IsRecurring = template.IsRecurring,
                RecurIntervalDays = template.CooldownDays,
                DialogueHook = template.DialogueHook
            };

            // Apply severity scaling
            ApplySeverityScaling(instance);

            _activeEvents[instance.EventId] = instance;

            // Set cooldown
            _cooldowns[cooldownKey] = template.CooldownDays;

            // Publish event
            var eventBus = ServiceLocator.Get<EventBus>();
            eventBus?.Publish("WorldEventTriggered", new WorldEventTriggeredEvent
            {
                EventId = instance.EventId,
                Type = instance.Type,
                Severity = instance.Severity,
                SettlementId = instance.SettlementId,
                Description = instance.Description
            });

            Logger.Info($"WorldEventFramework: Triggered '{instance.DisplayName}' ({instance.Severity}) in '{settlementId}' for {instance.DurationDays:F1} days.");
            return instance;
        }

        /// <summary>Update all active events for a day tick.</summary>
        public void DailyUpdate()
        {
            var completed = new List<string>();

            foreach (var (eventId, instance) in _activeEvents)
            {
                instance.ElapsedDays += 1f;

                if (instance.ElapsedDays >= instance.DurationDays)
                {
                    instance.Phase = EventPhase.Resolving;
                    ResolveEvent(instance);
                    instance.Phase = EventPhase.Completed;
                    completed.Add(eventId);
                }
            }

            // Remove completed events
            foreach (var eventId in completed)
            {
                _activeEvents.Remove(eventId);
            }

            // Decrement cooldowns
            var keys = new List<string>(_cooldowns.Keys);
            foreach (var key in keys)
            {
                _cooldowns[key] = Math.Max(0f, _cooldowns[key] - 1f);
            }
        }

        /// <summary>Try to trigger a random event for a settlement based on weights.</summary>
        public WorldEventInstance? TryTriggerRandomEvent(string settlementId, SettlementData settlement)
        {
            if (settlement == null) return null;

            // Calculate total weight
            float totalWeight = 0f;
            var eligible = new List<WorldEventTemplate>();

            foreach (var (type, template) in _templates)
            {
                if (type == WorldEventType.None) continue;

                // Check cooldown
                string cooldownKey = $"{settlementId}_{type}";
                if (_cooldowns.TryGetValue(cooldownKey, out var remaining) && remaining > 0f)
                    continue;

                // Check prosperity bounds
                float prosperityValue = (float)settlement.Prosperity;
                if (prosperityValue < template.MinProsperity || prosperityValue > template.MaxProsperity)
                    continue;

                // Check settlement type
                if (template.AllowedSettlementTypes.Count > 0 &&
                    !template.AllowedSettlementTypes.Contains(settlement.Type))
                    continue;

                totalWeight += template.TriggerWeight;
                eligible.Add(template);
            }

            if (eligible.Count == 0 || totalWeight <= 0f) return null;

            // Roll
            float roll = (float)_rng.NextDouble() * totalWeight;
            float cumulative = 0f;

            foreach (var template in eligible)
            {
                cumulative += template.TriggerWeight;
                if (roll <= cumulative)
                {
                    return TriggerEvent(template.Type, settlementId, settlement);
                }
            }

            return null;
        }

        /// <summary>Get active events for a settlement.</summary>
        public List<WorldEventInstance> GetActiveEventsForSettlement(string settlementId)
        {
            return _activeEvents.Values
                .Where(e => e.SettlementId.Equals(settlementId, StringComparison.OrdinalIgnoreCase) &&
                            e.Phase == EventPhase.Active)
                .ToList();
        }

        /// <summary>Get all active events.</summary>
        public List<WorldEventInstance> GetAllActiveEvents()
        {
            return new List<WorldEventInstance>(_activeEvents.Values);
        }

        /// <summary>Check if a settlement has an active event of a specific type.</summary>
        public bool HasActiveEvent(string settlementId, WorldEventType type)
        {
            return _activeEvents.Values.Any(e =>
                e.SettlementId.Equals(settlementId, StringComparison.OrdinalIgnoreCase) &&
                e.Type == type &&
                e.Phase == EventPhase.Active);
        }

        /// <summary>Cancel an active event.</summary>
        public void CancelEvent(string eventId)
        {
            if (_activeEvents.TryGetValue(eventId, out var instance))
            {
                instance.Phase = EventPhase.Completed;
                _activeEvents.Remove(eventId);
                Logger.Info($"WorldEventFramework: Cancelled event '{eventId}'.");
            }
        }

        /// <summary>Get events for save state.</summary>
        public List<WorldEventSaveState> GetSaveState()
        {
            var states = new List<WorldEventSaveState>();
            foreach (var (eventId, instance) in _activeEvents)
            {
                states.Add(new WorldEventSaveState
                {
                    EventId = eventId,
                    Type = instance.Type,
                    Severity = instance.Severity,
                    Phase = instance.Phase,
                    ElapsedDays = instance.ElapsedDays,
                    Version = 1
                });
            }
            return states;
        }

        /// <summary>Restore events from save state.</summary>
        public void RestoreSaveState(List<WorldEventSaveState> states)
        {
            _activeEvents.Clear();
            foreach (var state in states)
            {
                // Reconstruct from template if available
                if (_templates.TryGetValue(state.Type, out var template))
                {
                    var instance = new WorldEventInstance
                    {
                        EventId = state.EventId,
                        Type = state.Type,
                        Severity = state.Severity,
                        Phase = state.Phase,
                        SettlementId = "", // Will need to be resolved
                        DisplayName = template.DisplayName,
                        Description = template.Description,
                        DurationDays = template.BaseDurationDays,
                        ElapsedDays = state.ElapsedDays,
                        Effects = new Dictionary<string, float>(template.DefaultEffects)
                    };
                    _activeEvents[state.EventId] = instance;
                }
            }
            Logger.Info($"WorldEventFramework: Restored {states.Count} event states.");
        }

        private void ApplySeverityScaling(WorldEventInstance instance)
        {
            float multiplier = instance.Severity switch
            {
                EventSeverity.Minor => 0.5f,
                EventSeverity.Moderate => 1.0f,
                EventSeverity.Major => 1.5f,
                EventSeverity.Critical => 2.0f,
                _ => 1.0f
            };

            instance.DurationDays *= multiplier;

            var scaledEffects = new Dictionary<string, float>();
            foreach (var (key, value) in instance.Effects)
            {
                scaledEffects[key] = value * multiplier;
            }
            instance.Effects = scaledEffects;
        }

        private void ResolveEvent(WorldEventInstance instance)
        {
            Logger.Info($"WorldEventFramework: Resolving event '{instance.DisplayName}' in '{instance.SettlementId}'.");

            // Event resolution effects
            switch (instance.Type)
            {
                case WorldEventType.MarketDay:
                    Logger.Info($"WorldEventFramework: Market day bonus applied to '{instance.SettlementId}'.");
                    break;
                case WorldEventType.Festival:
                    Logger.Info($"WorldEventFramework: Festival happiness bonus applied to '{instance.SettlementId}'.");
                    break;
                case WorldEventType.Harvest:
                    Logger.Info($"WorldEventFramework: Harvest resources added to '{instance.SettlementId}'.");
                    break;
                case WorldEventType.StormPreparation:
                    Logger.Info($"WorldEventFramework: Storm damage mitigated for '{instance.SettlementId}'.");
                    break;
                case WorldEventType.MonsterAlert:
                    Logger.Info($"WorldEventFramework: Monster threat neutralized for '{instance.SettlementId}'.");
                    break;
                case WorldEventType.MerchantArrival:
                    Logger.Info($"WorldEventFramework: New merchant goods available in '{instance.SettlementId}'.");
                    break;
                case WorldEventType.TravelingCaravan:
                    Logger.Info($"WorldEventFramework: Caravan trade completed for '{instance.SettlementId}'.");
                    break;
                case WorldEventType.ResourceShortage:
                    Logger.Info($"WorldEventFramework: Resource shortage easing for '{instance.SettlementId}'.");
                    break;
            }
        }

        private void LoadDefaultTemplates()
        {
            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.MarketDay,
                DisplayName = "Market Day",
                Description = "Traveling merchants set up stalls. Prices reduced and rare items available.",
                DefaultSeverity = EventSeverity.Minor,
                BaseDurationDays = 1f,
                MinProsperity = 0f,
                MaxProsperity = 1f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Village, SettlementType.Town, SettlementType.City, SettlementType.Capital, SettlementType.Port },
                TriggerWeight = 3.0f,
                CooldownDays = 7f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "price_modifier", 0.8f },
                    { "merchant_activity", 1.5f },
                    { "trade_volume", 2.0f }
                }
            });

            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.Festival,
                DisplayName = "Festival",
                Description = "A joyous festival! NPCs celebrate, happiness rises, and special goods appear.",
                DefaultSeverity = EventSeverity.Minor,
                BaseDurationDays = 2f,
                MinProsperity = 0.2f,
                MaxProsperity = 1f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Village, SettlementType.Town, SettlementType.City, SettlementType.Capital, SettlementType.Port, SettlementType.Castle },
                TriggerWeight = 1.5f,
                CooldownDays = 21f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "happiness", 0.3f },
                    { "merchant_activity", 1.3f },
                    { "food_consumption", 1.5f }
                }
            });

            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.Harvest,
                DisplayName = "Harvest Season",
                Description = "The fields are abundant! Food supplies increase significantly.",
                DefaultSeverity = EventSeverity.Minor,
                BaseDurationDays = 3f,
                MinProsperity = 0f,
                MaxProsperity = 0.7f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Hamlet, SettlementType.Village, SettlementType.Town },
                TriggerWeight = 2.0f,
                CooldownDays = 30f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "food_supply", 1.5f },
                    { "prosperity", 0.1f }
                }
            });

            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.StormPreparation,
                DisplayName = "Storm Preparation",
                Description = "A storm is approaching! NPCs prepare by stocking supplies and reinforcing buildings.",
                DefaultSeverity = EventSeverity.Moderate,
                BaseDurationDays = 1f,
                MinProsperity = 0f,
                MaxProsperity = 1f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Hamlet, SettlementType.Village, SettlementType.Town, SettlementType.City, SettlementType.Capital, SettlementType.Port },
                TriggerWeight = 1.0f,
                CooldownDays = 14f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "building_damage_mitigation", 0.5f },
                    { "supply_consumption", 1.3f }
                }
            });

            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.MonsterAlert,
                DisplayName = "Monster Alert",
                Description = "Monsters have been spotted nearby! Guards are on high alert.",
                DefaultSeverity = EventSeverity.Moderate,
                BaseDurationDays = 2f,
                MinProsperity = 0f,
                MaxProsperity = 1f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Camp, SettlementType.Hamlet, SettlementType.Village, SettlementType.Town, SettlementType.MiningCamp, SettlementType.ForestOutpost },
                TriggerWeight = 1.2f,
                CooldownDays = 10f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "security", -0.2f },
                    { "guard_readiness", 1.5f },
                    { "population_morale", -0.1f }
                }
            });

            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.MerchantArrival,
                DisplayName = "Merchant Arrival",
                Description = "A wealthy merchant has arrived with exotic goods from distant lands!",
                DefaultSeverity = EventSeverity.Minor,
                BaseDurationDays = 1f,
                MinProsperity = 0.1f,
                MaxProsperity = 1f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Village, SettlementType.Town, SettlementType.City, SettlementType.Capital, SettlementType.Port, SettlementType.Castle },
                TriggerWeight = 1.8f,
                CooldownDays = 14f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "trade_volume", 2.5f },
                    { "rare_goods_chance", 0.3f },
                    { "price_modifier", 1.2f }
                }
            });

            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.TravelingCaravan,
                DisplayName = "Traveling Caravan",
                Description = "A caravan has arrived! Bulk goods available at competitive prices.",
                DefaultSeverity = EventSeverity.Minor,
                BaseDurationDays = 1f,
                MinProsperity = 0f,
                MaxProsperity = 1f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Camp, SettlementType.Hamlet, SettlementType.Village, SettlementType.Town, SettlementType.Port },
                TriggerWeight = 2.0f,
                CooldownDays = 10f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "supply", 1.3f },
                    { "price_modifier", 0.9f },
                    { "trade_volume", 1.5f }
                }
            });

            RegisterTemplate(new WorldEventTemplate
            {
                Type = WorldEventType.ResourceShortage,
                DisplayName = "Resource Shortage",
                Description = "A local resource has become scarce. Prices for affected goods have increased.",
                DefaultSeverity = EventSeverity.Moderate,
                BaseDurationDays = 4f,
                MinProsperity = 0f,
                MaxProsperity = 0.8f,
                AllowedSettlementTypes = new List<SettlementType> { SettlementType.Hamlet, SettlementType.Village, SettlementType.Town, SettlementType.MiningCamp, SettlementType.ForestOutpost },
                TriggerWeight = 1.0f,
                CooldownDays = 20f,
                DefaultEffects = new Dictionary<string, float>
                {
                    { "supply", 0.5f },
                    { "price_modifier", 1.5f },
                    { "prosperity", -0.1f }
                }
            });
        }
    }
}