using GameModules.CameraSystem.Core;
using Unity.Cinemachine;

namespace GameModules.CameraSystem.Behaviours
{
    using UnityEngine;

    public sealed class CameraScrollZoomBehaviour : CameraBehaviourBase
    {
        [SerializeField]
        private float zoomSpeed = 5f;

        [SerializeField]
        private float minFieldOfView = 35f;

        [SerializeField]
        private float maxFieldOfView = 80f;

        [SerializeField]
        private float minOrthographicSize = 3f;

        [SerializeField]
        private float maxOrthographicSize = 20f;

        [SerializeField]
        private bool adjustFollowOffset = true;

        [SerializeField]
        private Vector3 minFollowOffset = new(0.5f, 0f, -5f);

        [SerializeField]
        private Vector3 maxFollowOffset = new(0.5f, 0f, -10f);

        private CinemachineFollow _followComponent;

        protected override void OnActivated()
        {
            CacheFollowComponent();
        }

        protected override void OnContextUpdated()
        {
            CacheFollowComponent();
        }

        protected override void OnTick(float deltaTime)
        {
            var scrollDelta = CameraInputUtility.GetScrollDelta();
            if (Mathf.Approximately(scrollDelta, 0f) || VirtualCamera == null)
            {
                return;
            }

            var lens = VirtualCamera.Lens;
            float normalizedZoom;

            if (Context.Brain.OutputCamera.orthographic)
            {
                lens.OrthographicSize = Mathf.Clamp(
                    lens.OrthographicSize - scrollDelta * zoomSpeed,
                    minOrthographicSize,
                    maxOrthographicSize);
                normalizedZoom = Mathf.InverseLerp(minOrthographicSize, maxOrthographicSize, lens.OrthographicSize);
            }
            else
            {
                lens.FieldOfView = Mathf.Clamp(
                    lens.FieldOfView - scrollDelta * zoomSpeed,
                    minFieldOfView,
                    maxFieldOfView);
                normalizedZoom = Mathf.InverseLerp(maxFieldOfView, minFieldOfView, lens.FieldOfView);
            }

            VirtualCamera.Lens = lens;

            if (adjustFollowOffset)
            {
                CacheFollowComponent();
                if (_followComponent != null)
                {
                    _followComponent.FollowOffset = Vector3.Lerp(maxFollowOffset, minFollowOffset, normalizedZoom);
                }
            }
        }

        private void CacheFollowComponent()
        {
            if (!adjustFollowOffset || _followComponent != null || VirtualCamera == null)
            {
                return;
            }

            _followComponent = VirtualCamera.GetComponent<CinemachineFollow>();
        }
    }
}

