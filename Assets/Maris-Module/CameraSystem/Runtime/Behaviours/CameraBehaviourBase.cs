using Unity.Cinemachine;

namespace GameModules.CameraSystem.Behaviours
{
    using GameModules.CameraSystem.Authoring;
    using GameModules.CameraSystem.Core;
    using UnityEngine;

    public abstract class CameraBehaviourBase : MonoBehaviour
    {
        protected CameraRigAuthoring Rig { get; private set; }
        protected CameraRuntimeContext Context { get; private set; }
        protected CinemachineCamera VirtualCamera => Rig?.VirtualCamera;
        protected Transform Target => Context?.Target;

        internal void Initialize(CameraRigAuthoring rig)
        {
            Rig = rig;
            OnInitialized();
        }

        internal void Activate(CameraRuntimeContext context)
        {
            Context = context;
            OnActivated();
        }

        internal void UpdateContext(CameraRuntimeContext context)
        {
            Context = context;
            OnContextUpdated();
        }

        internal void Deactivate()
        {
            OnDeactivated();
            Context = null;
        }

        internal void Tick(float deltaTime)
        {
            if (Context == null)
            {
                return;
            }

            OnTick(deltaTime);
        }

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnActivated()
        {
        }

        protected virtual void OnContextUpdated()
        {
        }

        protected virtual void OnDeactivated()
        {
        }

        protected virtual void OnTick(float deltaTime)
        {
        }
    }
}

