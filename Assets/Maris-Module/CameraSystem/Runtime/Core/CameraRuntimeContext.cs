using Unity.Cinemachine;

namespace GameModules.CameraSystem.Core
{
    using UnityEngine;

    public class CameraRuntimeContext
    {
        public Transform Target { get; private set; }
        public Camera UnityCamera { get; private set; }
        public CinemachineBrain Brain { get; private set; }

        public Vector3 TargetPosition => Target != null ? Target.position : Vector3.zero;

        public CameraRuntimeContext(Transform target, Camera unityCamera, CinemachineBrain brain)
        {
            Target = target;
            UnityCamera = unityCamera;
            Brain = brain;
        }

        public void UpdateTarget(Transform target)
        {
            Target = target;
        }

        public void UpdateCamera(Camera camera)
        {
            if (camera == null)
            {
                return;
            }

            UnityCamera = camera;
        }

        public void UpdateBrain(CinemachineBrain brain)
        {
            if (brain == null)
            {
                return;
            }

            Brain = brain;
        }
    }
}

