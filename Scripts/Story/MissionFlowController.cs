using System;
using Godot;

namespace HeroOfEternia.Story
{
    public enum MissionState
    {
        Inactive,
        InProgress,
        Failed,
        Completed
    }

    public class MissionStatusData
    {
        public string MissionId { get; set; } = string.Empty;
        public MissionState State { get; set; } = MissionState.Inactive;
        public int ActiveCheckpointIndex { get; set; } = 0;
        public Vector3 LastCheckpointPosition { get; set; }
    }

    /// <summary>
    /// Mission lifecycle controller managing mission start, checkpoints, updates,
    /// failures, retries, and reward dispatches.
    /// </summary>
    public class MissionFlowController
    {
        private MissionStatusData _currentMission = new();

        public event Action<MissionStatusData>? OnMissionStateChanged;
        public event Action<int, Vector3>? OnCheckpointReached;

        public void StartMission(string missionId, Vector3 initialPosition)
        {
            _currentMission = new MissionStatusData
            {
                MissionId = missionId,
                State = MissionState.InProgress,
                ActiveCheckpointIndex = 0,
                LastCheckpointPosition = initialPosition
            };

            OnMissionStateChanged?.Invoke(_currentMission);
        }

        public void SetCheckpoint(int checkpointIndex, Vector3 checkpointPosition)
        {
            if (_currentMission.State == MissionState.InProgress)
            {
                _currentMission.ActiveCheckpointIndex = checkpointIndex;
                _currentMission.LastCheckpointPosition = checkpointPosition;
                OnCheckpointReached?.Invoke(checkpointIndex, checkpointPosition);
            }
        }

        public void FailMission()
        {
            if (_currentMission.State == MissionState.InProgress)
            {
                _currentMission.State = MissionState.Failed;
                OnMissionStateChanged?.Invoke(_currentMission);
            }
        }

        public void CompleteMission()
        {
            if (_currentMission.State == MissionState.InProgress)
            {
                _currentMission.State = MissionState.Completed;
                OnMissionStateChanged?.Invoke(_currentMission);
            }
        }

        public MissionStatusData CurrentMission => _currentMission;
    }
}
