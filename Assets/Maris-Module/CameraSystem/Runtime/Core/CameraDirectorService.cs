namespace GameModules.CameraSystem.Core
{
    using System;
    using System.Collections.Generic;
    using Authoring;
    using Events;
    using GameModules.Systems.Events;
    using UnityEngine;

    public interface ICameraRigRegistry
    {
        IReadOnlyCollection<CameraRigAuthoring> Rigs { get; }
        void Register(CameraRigAuthoring rig);
        void Unregister(CameraRigAuthoring rig);
        bool TryGetRig(string cameraId, out CameraRigAuthoring rig);
    }

    public interface ICameraDirectorService
    {
        CameraRigAuthoring ActiveRig { get; }
        bool SwitchCamera(string cameraId, CameraRuntimeContext context);
        void Tick(float deltaTime);
    }

    internal sealed class CameraRigRegistry : ICameraRigRegistry
    {
        private readonly Dictionary<string, CameraRigAuthoring> _rigs = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyCollection<CameraRigAuthoring> Rigs => _rigs.Values;

        public void Register(CameraRigAuthoring rig)
        {
            if (rig == null || string.IsNullOrWhiteSpace(rig.CameraId))
            {
                return;
            }

            _rigs[rig.CameraId] = rig;
        }

        public void Unregister(CameraRigAuthoring rig)
        {
            if (rig == null || string.IsNullOrWhiteSpace(rig.CameraId))
            {
                return;
            }

            if (_rigs.TryGetValue(rig.CameraId, out var existing) && existing == rig)
            {
                _rigs.Remove(rig.CameraId);
            }
        }

        public bool TryGetRig(string cameraId, out CameraRigAuthoring rig)
        {
            if (string.IsNullOrWhiteSpace(cameraId))
            {
                rig = null;
                return false;
            }

            return _rigs.TryGetValue(cameraId, out rig);
        }
    }

    internal sealed class CameraDirectorService : ICameraDirectorService
    {
        private readonly ICameraRigRegistry _registry;
        private readonly IEventBus _eventBus;

        private CameraRigAuthoring _activeRig;
        private CameraRuntimeContext _context;

        public CameraRigAuthoring ActiveRig => _activeRig;

        public CameraDirectorService(ICameraRigRegistry registry, IGlobalEventBus eventBus = null)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _eventBus = eventBus;
        }

        public bool SwitchCamera(string cameraId, CameraRuntimeContext context)
        {
            _eventBus?.Publish(new CameraSwitchRequested(cameraId, _activeRig?.CameraId, context?.Target));

            if (!_registry.TryGetRig(cameraId, out var nextRig))
            {
                _eventBus?.Publish(new CameraSwitchFailed(cameraId, _activeRig?.CameraId));
                return false;
            }

            if (_activeRig == nextRig)
            {
                _context = context;
                if (_activeRig != null) _activeRig.UpdateContext(context);
                return true;
            }

            _activeRig?.Deactivate();

            _context = context;
            nextRig.Activate(context);
            _activeRig = nextRig;

            _eventBus?.Publish(new CameraSwitchStarted(cameraId, _context?.Target));
            _eventBus?.Publish(new CameraSwitchCompleted(cameraId, Time.time));
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (_activeRig == null)
            {
                return;
            }

            _activeRig.Tick(deltaTime);
        }
    }
}

