using GameModules.CameraSystem.Core;

namespace GameModules.CameraSystem.Behaviours
{
    using UnityEngine;

    public sealed class CameraOrbitYawLimitedBehaviour : CameraBehaviourBase
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

        [SerializeField]
        private float minPitch = -30f;

        [SerializeField]
        private float maxPitch = 60f;

        private float _currentYaw;
        private float _currentPitch;

        protected override void OnActivated()
        {
            if (Target == null)
            {
                return;
            }

            var euler = Target.rotation.eulerAngles;
            _currentYaw = NormalizeAngle(euler.y);
            _currentPitch = NormalizeAngle(euler.x);
        }

        protected override void OnContextUpdated()
        {
            if (Target == null)
            {
                return;
            }

            var euler = Target.rotation.eulerAngles;
            _currentYaw = NormalizeAngle(euler.y);
            _currentPitch = NormalizeAngle(euler.x);
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

            _currentYaw += mouseDelta.x * horizontalSpeed * scale;
            _currentPitch -= mouseDelta.y * verticalSpeed * scale;

            _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);

            Target.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f)
            {
                angle -= 360f;
            }

            if (angle < -180f)
            {
                angle += 360f;
            }

            return angle;
        }
    }
}


