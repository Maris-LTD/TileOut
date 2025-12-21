using GameModules.CameraSystem.Core;

namespace GameModules.CameraSystem.Behaviours
{
    using UnityEngine;

    public sealed class CameraOrbitBehaviour : CameraBehaviourBase
    {
        [SerializeField]
        private float horizontalSpeed = 90f;

        [SerializeField]
        private float verticalSpeed = 65f;

        [SerializeField]
        private bool requireMouseButton = true;

        [SerializeField]
        private int mouseButtonIndex = 1;

        [SerializeField]
        private bool scaleWithDeltaTime = true;

        protected override void OnActivated()
        {
        }

        protected override void OnContextUpdated()
        {
        }

        protected override void OnTick(float deltaTime)
        {
            if (Target == null)
            {
                return;
            }

            if (requireMouseButton && !CameraInputUtility.GetMouseButton(mouseButtonIndex))
            {
                return;
            }

            var mouseDelta = CameraInputUtility.GetMouseDelta();
            if (Mathf.Approximately(mouseDelta.x, 0f) && Mathf.Approximately(mouseDelta.y, 0f))
            {
                return;
            }

            var scale = scaleWithDeltaTime ? deltaTime : 1f;

            var cameraTransform = Context?.Brain?.OutputCamera != null
                ? Context.Brain.OutputCamera.transform
                : (Camera.main != null ? Camera.main.transform : null);

            if (cameraTransform == null)
            {
                return;
            }

            var horizontalAxis = cameraTransform.up;
            var verticalAxis = cameraTransform.right;

            var yawRotation = Quaternion.AngleAxis(mouseDelta.x * horizontalSpeed * scale, horizontalAxis);
            var pitchRotation = Quaternion.AngleAxis(-mouseDelta.y * verticalSpeed * scale, verticalAxis);

            Target.rotation = yawRotation * pitchRotation * Target.rotation;
        }
    }
}

