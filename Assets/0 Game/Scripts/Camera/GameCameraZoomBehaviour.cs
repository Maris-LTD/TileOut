using GameModules.CameraSystem.Behaviours;
using GameModules.CameraSystem.Core;
using UnityEngine;

namespace Game.Camera
{
    public sealed class GameCameraZoomBehaviour : CameraBehaviourBase
    {
        [SerializeField]
        private float zoomSpeed = 5f;

        [SerializeField]
        private float zoomRange = 3f;

        private float _baseOrthographicSize;
        private float _minOrthographicSize;
        private float _maxOrthographicSize;

        protected override void OnActivated()
        {
            if (Context?.UnityCamera == null)
            {
                return;
            }

            _baseOrthographicSize = Context.UnityCamera.orthographicSize;
            _minOrthographicSize = _baseOrthographicSize - zoomRange;
            _maxOrthographicSize = _baseOrthographicSize + zoomRange;
        }

        protected override void OnContextUpdated()
        {
            if (Context?.UnityCamera == null)
            {
                return;
            }

            _baseOrthographicSize = Context.UnityCamera.orthographicSize;
            _minOrthographicSize = _baseOrthographicSize - zoomRange;
            _maxOrthographicSize = _baseOrthographicSize + zoomRange;
        }

        protected override void OnTick(float deltaTime)
        {
            if (Context?.UnityCamera == null || VirtualCamera == null)
            {
                return;
            }

            var scrollDelta = CameraInputUtility.GetScrollDelta();
            if (Mathf.Approximately(scrollDelta, 0f))
            {
                return;
            }

            var lens = VirtualCamera.Lens;
            lens.OrthographicSize = Mathf.Clamp(
                lens.OrthographicSize - scrollDelta * zoomSpeed * deltaTime,
                _minOrthographicSize,
                _maxOrthographicSize);

            VirtualCamera.Lens = lens;
            Context.UnityCamera.orthographicSize = lens.OrthographicSize;
        }
    }
}

