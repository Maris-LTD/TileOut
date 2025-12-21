using Unity.Cinemachine;

namespace GameModules.CameraSystem.Authoring
{
    using Behaviours;
    using Core;
    using GameModules.Core;
    using UnityEngine;
    using VContainer;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    [AutoInject(ModuleScope.Scene)]
    public sealed class CameraRigAuthoring : MonoBehaviour
    {
        [SerializeField]
        private string cameraId = "Default";

        [SerializeField]
        private CinemachineCamera virtualCamera;

        [SerializeField]
        private bool overrideFollowTarget = true;

        [SerializeField]
        private bool overrideLookAtTarget = true;

        [SerializeField]
        private int priorityBoost = 100;

        [SerializeField]
        private CameraBehaviourBase[] behaviours;

        private ICameraRigRegistry _registry;
        private CameraRuntimeContext _context;
        private Transform _currentTarget;
        private bool _isRegistered;
        private bool _isActive;
        private int _defaultPriority;

        public string CameraId => cameraId;
        public CinemachineCamera VirtualCamera => virtualCamera;
        public Transform CurrentTarget => _currentTarget;

        [Inject]
        public void Construct(ICameraRigRegistry registry)
        {
            _registry = registry;
            if (isActiveAndEnabled)
            {
                RegisterRig();
            }
        }

        private void Reset()
        {
            virtualCamera = GetComponent<CinemachineCamera>();
            behaviours = GetComponentsInChildren<CameraBehaviourBase>(true);
        }

        private void Awake()
        {
            if (virtualCamera == null)
            {
                virtualCamera = GetComponent<CinemachineCamera>();
            }

            _defaultPriority = virtualCamera != null ? virtualCamera.Priority : 0;
            InitializeBehaviours();
            RegisterRig();
        }

        private void OnDestroy()
        {
            UnregisterRig();
        }

        private void OnDisable()
        {
            if (_isActive)
            {
                Deactivate();
            }
        }

        private void InitializeBehaviours()
        {
            if (behaviours == null || behaviours.Length == 0)
            {
                behaviours = GetComponentsInChildren<CameraBehaviourBase>(true);
            }

            foreach (var behaviour in behaviours)
            {
                behaviour?.Initialize(this);
            }
        }

        private void RegisterRig()
        {
            if (_isRegistered)
            {
                return;
            }

            if (_registry == null)
            {
                return;
            }

            _registry.Register(this);
            _isRegistered = true;
        }

        private void UnregisterRig()
        {
            if (_registry == null || !_isRegistered)
            {
                return;
            }

            _registry.Unregister(this);
            _isRegistered = false;
        }

        internal void Activate(CameraRuntimeContext context)
        {
            _context = context;
            _currentTarget = context?.Target;
            _isActive = true;

            ApplyPriority(true);
            ApplyTargets();

            foreach (var behaviour in behaviours)
            {
                behaviour?.Activate(context);
            }
        }

        internal void UpdateContext(CameraRuntimeContext context)
        {
            _context = context;
            _currentTarget = context?.Target;

            ApplyTargets();

            foreach (var behaviour in behaviours)
            {
                behaviour?.UpdateContext(context);
            }
        }

        internal void Deactivate()
        {
            _isActive = false;

            foreach (var behaviour in behaviours)
            {
                behaviour?.Deactivate();
            }

            ApplyPriority(false);
        }

        internal void Tick(float deltaTime)
        {
            if (!_isActive)
            {
                return;
            }
            
            foreach (var behaviour in behaviours)
            {
                behaviour?.Tick(deltaTime);
            }
        }

        private void ApplyPriority(bool active)
        {
            if (virtualCamera == null)
            {
                return;
            }

            virtualCamera.Priority = active ? _defaultPriority + priorityBoost : _defaultPriority;
        }

        private void ApplyTargets()
        {
            if (_currentTarget == null || virtualCamera == null)
            {
                return;
            }

            if (overrideFollowTarget)
            {
                virtualCamera.Follow = _currentTarget;
            }

            if (overrideLookAtTarget)
            {
                virtualCamera.LookAt = _currentTarget;
            }
        }
    }
}

