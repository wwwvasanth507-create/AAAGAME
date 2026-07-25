using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI
{
    /// <summary>
    /// Central notification manager supporting priority levels, queueing,
    /// and hooks for quest updates, level up, item acquisition, achievements,
    /// crafting, system messages, warnings, errors, and future events.
    /// Registered in ServiceLocator as IInitializable.
    /// </summary>
    public class NotificationManager : IInitializable
    {
        // ---------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------
        private const int MaxVisibleNotifications = 5;
        private const int MaxQueueSize = 50;
        private const float DefaultNotificationDuration = 3.0f;
        private const float CriticalNotificationDuration = 6.0f;

        // ---------------------------------------------------------------
        // State
        // ---------------------------------------------------------------
        private readonly Queue<UINotification> _queue = new Queue<UINotification>();
        private readonly List<ActiveNotification> _active = new List<ActiveNotification>();
        private readonly List<UINotification> _history = new List<UINotification>();
        private readonly List<INotificationHandler> _handlers = new List<INotificationHandler>();

        private bool _initialized;
        private bool _enabled = true;
        private CanvasLayer _notificationLayer;
        private VBoxContainer _notificationContainer;
        private Window _gameWindow;

        // ---------------------------------------------------------------
        // Events
        // ---------------------------------------------------------------
        public event Action<UINotification> OnNotificationQueued;
        public event Action<UINotification> OnNotificationShown;
        public event Action<UINotification> OnNotificationDismissed;
        public event Action OnQueueCleared;

        // ---------------------------------------------------------------
        // Properties
        // ---------------------------------------------------------------
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public int QueueLength => _queue.Count;
        public int ActiveCount => _active.Count;
        public IReadOnlyList<UINotification> History => _history.AsReadOnly();

        // ---------------------------------------------------------------
        // Initialization
        // ---------------------------------------------------------------
        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("NotificationManager: Initializing...");

            _gameWindow = (Engine.GetMainLoop() as SceneTree)?.Root ?? new Window();

            _notificationLayer = new CanvasLayer
            {
                Layer = 60, // Notifications layer
                Name = "NotificationLayer"
            };
            _gameWindow.AddChild(_notificationLayer);

            _notificationContainer = new VBoxContainer
            {
                Position = new Vector2I(20, 20),
                Size = new Vector2I(400, 0),
                Alignment = BoxContainer.AlignmentMode.Begin
            };
            _notificationLayer.AddChild(_notificationContainer);

            Logger.Info("NotificationManager: Initialized successfully.");
        }

        public void Shutdown()
        {
            _queue.Clear();
            _active.Clear();
            _history.Clear();

            if (_notificationLayer != null && GodotObject.IsInstanceValid(_notificationLayer))
            {
                _notificationLayer.QueueFree();
            }

            _initialized = false;
            Logger.Info("NotificationManager: Shutdown complete.");
        }

        // ---------------------------------------------------------------
        // Queueing
        // ---------------------------------------------------------------
        public void QueueNotification(UINotification notification)
        {
            if (!_enabled)
            {
                Logger.Info($"NotificationManager: Notifications disabled, skipping '{notification.Id}'.");
                return;
            }

            if (_queue.Count >= MaxQueueSize)
            {
                Logger.Warning($"NotificationManager: Queue full ({MaxQueueSize}), dropping '{notification.Id}'.");
                return;
            }

            _queue.Enqueue(notification);
            _history.Add(notification);
            OnNotificationQueued?.Invoke(notification);

            Logger.Info($"NotificationManager: Queued '{notification.Id}' (priority: {notification.Priority}).");

            ProcessQueue();
        }

        public void QueueNotification(string id, string titleKey, string messageKey,
            NotificationPriority priority = NotificationPriority.Normal,
            float duration = -1, string icon = null)
        {
            var notification = new UINotification
            {
                Id = id,
                TitleKey = titleKey,
                MessageKey = messageKey,
                Priority = priority,
                Duration = duration > 0 ? duration : GetDefaultDuration(priority),
                Icon = icon ?? "default"
            };
            QueueNotification(notification);
        }

        // ---------------------------------------------------------------
        // Convenience Methods
        // ---------------------------------------------------------------
        public void QuestUpdated(string questName)
        {
            QueueNotification($"quest_{questName}", "notif_quest_updated", questName,
                NotificationPriority.Normal, 3.0f, "quest");
        }

        public void LevelUp(int newLevel)
        {
            QueueNotification($"level_{newLevel}", "notif_level_up_title",
                $"notif_level_up_msg_{newLevel}",
                NotificationPriority.High, 4.0f, "level_up");
        }

        public void ItemAcquired(string itemName, int quantity = 1)
        {
            string msg = quantity > 1 ? $"{itemName} x{quantity}" : itemName;
            QueueNotification($"item_{itemName}", "notif_item_acquired", msg,
                NotificationPriority.Normal, 2.5f, "item");
        }

        public void AchievementUnlocked(string achievementName)
        {
            QueueNotification($"achievement_{achievementName}", "notif_achievement",
                achievementName,
                NotificationPriority.High, 5.0f, "achievement");
        }

        public void CraftComplete(string itemName)
        {
            QueueNotification($"craft_{itemName}", "notif_craft_complete", itemName,
                NotificationPriority.Normal, 3.0f, "craft");
        }

        public void SystemMessage(string message)
        {
            QueueNotification($"sys_{Guid.NewGuid():N}", "notif_system", message,
                NotificationPriority.Low, 2.0f, "system");
        }

        public void Warning(string message)
        {
            QueueNotification($"warn_{Guid.NewGuid():N}", "notif_warning", message,
                NotificationPriority.High, 4.0f, "warning");
        }

        public void Error(string message)
        {
            QueueNotification($"err_{Guid.NewGuid():N}", "notif_error", message,
                NotificationPriority.Critical, 6.0f, "error");
        }

        // ---------------------------------------------------------------
        // Queue Processing
        // ---------------------------------------------------------------
        private void ProcessQueue()
        {
            while (_active.Count < MaxVisibleNotifications && _queue.Count > 0)
            {
                var notification = _queue.Dequeue();
                ShowNotification(notification);
            }
        }

        private void ShowNotification(UINotification notification)
        {
            var activeNotif = new ActiveNotification
            {
                Notification = notification,
                RemainingTime = notification.Duration
            };

            var panel = CreateNotificationPanel(notification);
            _notificationContainer.AddChild(panel);
            activeNotif.Panel = panel;

            _active.Add(activeNotif);
            OnNotificationShown?.Invoke(notification);

            // Notify handlers
            foreach (var handler in _handlers)
                handler.OnNotificationShown(notification);
        }

        private Panel CreateNotificationPanel(UINotification notification)
        {
            var panel = new Panel
            {
                Size = new Vector2I(380, 0),
                Theme = CreateNotificationTheme(notification.Priority)
            };

            var vbox = new VBoxContainer
            {
                Size = new Vector2I(360, 0),
                Position = new Vector2I(10, 5)
            };
            panel.AddChild(vbox);

            // Title
            var title = new Label
            {
                Text = GetLocalizedText(notification.TitleKey),
                Size = new Vector2I(360, 25),
                Theme = CreateTitleTheme()
            };
            vbox.AddChild(title);

            // Message
            var message = new Label
            {
                Text = GetLocalizedText(notification.MessageKey),
                Size = new Vector2I(360, 0),
                AutowrapMode = TextServer.AutowrapMode.Word,
                Theme = CreateMessageTheme()
            };
            vbox.AddChild(message);

            // Adjust panel height based on content
            panel.Size = new Vector2I(380, 30 + vbox.GetChildCount() * 25);

            return panel;
        }

        // ---------------------------------------------------------------
        // Update (call from game loop)
        // ---------------------------------------------------------------
        public void Update(float delta)
        {
            if (!_enabled || _active.Count == 0) return;

            var toRemove = new List<ActiveNotification>();

            foreach (var active in _active)
            {
                active.RemainingTime -= delta;

                if (active.RemainingTime <= 0 && !active.Notification.Persistent)
                {
                    toRemove.Add(active);
                }
            }

            foreach (var active in toRemove)
            {
                DismissNotification(active);
            }

            if (toRemove.Count > 0)
                ProcessQueue();
        }

        private void DismissNotification(ActiveNotification active)
        {
            _active.Remove(active);

            if (active.Panel != null && GodotObject.IsInstanceValid(active.Panel))
            {
                // Fade out animation
                var tween = active.Panel.CreateTween();
                tween.TweenProperty(active.Panel, "modulate", new Color(1, 1, 1, 0), 0.3f);
                tween.Finished += () =>
                {
                    if (active.Panel != null && GodotObject.IsInstanceValid(active.Panel))
                    {
                        _notificationContainer.RemoveChild(active.Panel);
                        active.Panel.QueueFree();
                    }
                };
            }

            OnNotificationDismissed?.Invoke(active.Notification);

            foreach (var handler in _handlers)
                handler.OnNotificationDismissed(active.Notification);
        }

        // ---------------------------------------------------------------
        // Management
        // ---------------------------------------------------------------
        public void ClearQueue()
        {
            _queue.Clear();
            OnQueueCleared?.Invoke();
            Logger.Info("NotificationManager: Queue cleared.");
        }

        public void DismissAll()
        {
            var toRemove = new List<ActiveNotification>(_active);
            foreach (var active in toRemove)
                DismissNotification(active);
        }

        public void ClearHistory()
        {
            _history.Clear();
            Logger.Info("NotificationManager: History cleared.");
        }

        // ---------------------------------------------------------------
        // Handlers
        // ---------------------------------------------------------------
        public void RegisterHandler(INotificationHandler handler)
        {
            if (!_handlers.Contains(handler))
            {
                _handlers.Add(handler);
                Logger.Info($"NotificationManager: Handler '{handler.GetType().Name}' registered.");
            }
        }

        public void UnregisterHandler(INotificationHandler handler)
        {
            _handlers.Remove(handler);
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------
        private float GetDefaultDuration(NotificationPriority priority)
        {
            return priority switch
            {
                NotificationPriority.Low => 2.0f,
                NotificationPriority.Normal => 3.0f,
                NotificationPriority.High => 4.0f,
                NotificationPriority.Critical => 6.0f,
                _ => DefaultNotificationDuration
            };
        }

        private string GetLocalizedText(string key)
        {
            try
            {
                var loc = ServiceLocator.Get<LocalizationManager>();
                return loc?.GetText(key) ?? key;
            }
            catch
            {
                return key;
            }
        }

        private Theme CreateNotificationTheme(NotificationPriority priority)
        {
            var theme = new Theme();
            var style = new StyleBoxFlat();

            style.BgColor = priority switch
            {
                NotificationPriority.Low => new Color(0.1f, 0.1f, 0.15f, 0.9f),
                NotificationPriority.Normal => new Color(0.1f, 0.12f, 0.2f, 0.9f),
                NotificationPriority.High => new Color(0.2f, 0.1f, 0.05f, 0.9f),
                NotificationPriority.Critical => new Color(0.3f, 0.05f, 0.05f, 0.95f),
                _ => new Color(0.1f, 0.1f, 0.15f, 0.9f)
            };

            style.BorderWidthBottom = 2;
            style.BorderColor = priority switch
            {
                NotificationPriority.Low => new Color(0.3f, 0.3f, 0.4f),
                NotificationPriority.Normal => new Color(0.3f, 0.4f, 0.6f),
                NotificationPriority.High => new Color(0.8f, 0.6f, 0.2f),
                NotificationPriority.Critical => new Color(1, 0.2f, 0.2f),
                _ => new Color(0.3f, 0.3f, 0.4f)
            };

            theme.SetStylebox("panel", "Panel", style);
            return theme;
        }

        private Theme CreateTitleTheme()
        {
            var theme = new Theme();
            theme.SetFont("font", "Label", ThemeDB.FallbackFont);
            theme.SetFontSize("font_size", "Label", 14);
            theme.SetColor("font_color", "Label", new Color(1, 0.84f, 0));
            return theme;
        }

        private Theme CreateMessageTheme()
        {
            var theme = new Theme();
            theme.SetFont("font", "Label", ThemeDB.FallbackFont);
            theme.SetFontSize("font_size", "Label", 12);
            theme.SetColor("font_color", "Label", Colors.White);
            return theme;
        }

        // ---------------------------------------------------------------
        // Internal classes
        // ---------------------------------------------------------------
        private class ActiveNotification
        {
            public UINotification Notification { get; set; }
            public float RemainingTime { get; set; }
            public Panel Panel { get; set; }
        }
    }

    // ---------------------------------------------------------------
    // Notification Handler Interface
    // ---------------------------------------------------------------
    public interface INotificationHandler
    {
        void OnNotificationShown(UINotification notification);
        void OnNotificationDismissed(UINotification notification);
    }
}