namespace GameModules.CameraSystem.Events
{
    using UnityEngine;

    public readonly struct CameraSwitchRequested
    {
        public string RequestedId { get; }
        public string CurrentId { get; }
        public Transform Target { get; }

        public CameraSwitchRequested(string requestedId, string currentId, Transform target)
        {
            RequestedId = requestedId;
            CurrentId = currentId;
            Target = target;
        }
    }

    public readonly struct CameraSwitchStarted
    {
        public string CameraId { get; }
        public Transform Target { get; }

        public CameraSwitchStarted(string cameraId, Transform target)
        {
            CameraId = cameraId;
            Target = target;
        }
    }

    public readonly struct CameraSwitchCompleted
    {
        public string CameraId { get; }
        public float Timestamp { get; }

        public CameraSwitchCompleted(string cameraId, float timestamp)
        {
            CameraId = cameraId;
            Timestamp = timestamp;
        }
    }

    public readonly struct CameraSwitchFailed
    {
        public string RequestedId { get; }
        public string PreviousId { get; }

        public CameraSwitchFailed(string requestedId, string previousId)
        {
            RequestedId = requestedId;
            PreviousId = previousId;
        }
    }
}

