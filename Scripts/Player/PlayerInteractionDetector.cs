using Godot;
using System;
using System.Collections.Generic;
using HeroOfEternia.Interaction;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// PlayerInteractionDetector attached to PlayerRoot.
    /// Uses Area3D overlapping and distance checks to find and highlight the closest IInteractable object.
    /// Handles Tap, Hold, and Auto interact trigger inputs.
    /// </summary>
    public partial class PlayerInteractionDetector : Area3D
    {
        private PlayerRoot _player = null!;
        private IInteractable? _closestInteractable;
        private readonly List<IInteractable> _manualInteractables = new(); // For testing/headless support
        
        private float _holdTimer = 0f;
        private bool _isHolding = false;
        private bool _lastInteractInput = false;

        [Export]
        public float DetectionRadius { get; set; } = 4.0f;

        public IInteractable? ClosestInteractable => _closestInteractable;
        public float CurrentHoldProgress => (_closestInteractable != null && _closestInteractable.HoldDuration > 0f) 
            ? Mathf.Clamp(_holdTimer / _closestInteractable.HoldDuration, 0f, 1f) 
            : 0f;

        public override void _Ready()
        {
            _player = GetParent<PlayerRoot>();

            // Setup collision shape programmatically
            CollisionLayer = 0;
            CollisionMask = 0;
            SetCollisionMaskValue(4, true); // Scan Layer 4 (Interactables)

            var shape = new CollisionShape3D();
            shape.Shape = new SphereShape3D { Radius = DetectionRadius };
            AddChild(shape);

            Core.Logger.Info("PlayerInteractionDetector: Ready.");
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_player == null) return;

            // Find closest interactable
            IInteractable? bestTarget = FindBestTarget();

            // Handle target changes and highlighting
            if (bestTarget != _closestInteractable)
            {
                if (_isHolding && _closestInteractable != null)
                {
                    CancelHold();
                }

                _closestInteractable?.SetHighlight(false);
                _closestInteractable = bestTarget;
                _closestInteractable?.SetHighlight(true);

                if (_closestInteractable != null)
                {
                    Core.Logger.Info($"PlayerInteractionDetector: Target changed to {_closestInteractable.InteractionPrompt}");
                    if (_closestInteractable.Type == InteractionType.Auto)
                    {
                        TriggerInteraction();
                    }
                }
            }

            // Handle Input triggers
            if (_closestInteractable != null && _closestInteractable.Type != InteractionType.Auto)
            {
                bool interactPressed = _player.Input.Current.Interact;

                if (_closestInteractable.Type == InteractionType.Tap)
                {
                    // Trigger on rising edge
                    if (interactPressed && !_lastInteractInput)
                    {
                        TriggerInteraction();
                    }
                }
                else if (_closestInteractable.Type == InteractionType.Hold)
                {
                    if (interactPressed)
                    {
                        if (!_isHolding)
                        {
                            StartHold();
                        }
                        else
                        {
                            UpdateHold((float)delta);
                        }
                    }
                    else if (_isHolding)
                    {
                        CancelHold();
                    }
                }

                _lastInteractInput = interactPressed;
            }
            else
            {
                _lastInteractInput = false;
            }
        }

        /// <summary>
        /// Manually registers an interactable (primarily for testing or manual triggers).
        /// </summary>
        public void RegisterManualInteractable(IInteractable interactable)
        {
            if (!_manualInteractables.Contains(interactable))
            {
                _manualInteractables.Add(interactable);
            }
        }

        /// <summary>
        /// Manually unregisters an interactable.
        /// </summary>
        public void UnregisterManualInteractable(IInteractable interactable)
        {
            _manualInteractables.Remove(interactable);
        }

        // ---------------------------------------------------------------
        // PRIVATE HELPERS
        // ---------------------------------------------------------------

        private IInteractable? FindBestTarget()
        {
            IInteractable? bestTarget = null;
            float minDistance = float.MaxValue;
            Vector3 playerPos = _player.GlobalPosition;

            // 1. Check physics overlapping areas
            var overlappingAreas = GetOverlappingAreas();
            foreach (var area in overlappingAreas)
            {
                if (area is IInteractable interactable)
                {
                    float dist = playerPos.DistanceTo(interactable.GetGlobalPosition());
                    if (dist <= interactable.InteractionDistance && dist < minDistance)
                    {
                        minDistance = dist;
                        bestTarget = interactable;
                    }
                }
            }

            // 2. Check manually registered interactables (for headless unit test runner)
            foreach (var interactable in _manualInteractables)
            {
                float dist = playerPos.DistanceTo(interactable.GetGlobalPosition());
                if (dist <= interactable.InteractionDistance && dist < minDistance)
                {
                    minDistance = dist;
                    bestTarget = interactable;
                }
            }

            return bestTarget;
        }

        private void TriggerInteraction()
        {
            if (_closestInteractable == null) return;
            
            // Set player to Interacting state
            _player.FSM.ForceTransition(_player, PlayerStateId.Interacting);
            
            Core.Logger.Info($"PlayerInteractionDetector: Triggered interaction prompt: {_closestInteractable.InteractionPrompt}");
            _closestInteractable.OnInteract(_player);
        }

        private void StartHold()
        {
            if (_closestInteractable == null) return;
            _isHolding = true;
            _holdTimer = 0f;
            _closestInteractable.OnInteractionStart(_player);
            Core.Logger.Info($"PlayerInteractionDetector: Started holding for {_closestInteractable.InteractionPrompt}");
        }

        private void UpdateHold(float delta)
        {
            if (_closestInteractable == null || !_isHolding) return;

            _holdTimer += delta;
            if (_holdTimer >= _closestInteractable.HoldDuration)
            {
                _isHolding = false;
                _holdTimer = 0f;
                _closestInteractable.OnInteractionEnd(_player, true);
                TriggerInteraction();
            }
        }

        private void CancelHold()
        {
            if (_closestInteractable == null || !_isHolding) return;
            _isHolding = false;
            _holdTimer = 0f;
            _closestInteractable.OnInteractionEnd(_player, false);
            Core.Logger.Info("PlayerInteractionDetector: Hold interaction cancelled.");
        }
    }
}
