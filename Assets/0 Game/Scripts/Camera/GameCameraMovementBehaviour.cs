using GameModules.CameraSystem.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Camera
{
    public sealed class GameCameraMovementBehaviour : CameraBehaviourBase
    {
        [SerializeField]
        private float movementSpeed = 1f;

        [SerializeField]
        private bool requireMouseButton = true;

        [SerializeField]
        private int mouseButtonIndex;

        private Vector2 _lastMousePosition;
        private bool _isDragging;
        private UnityEngine.Camera _camera;

        protected override void OnActivated()
        {
            _camera = Context?.UnityCamera;
            _isDragging = false;
        }

        protected override void OnContextUpdated()
        {
            _camera = Context?.UnityCamera;
        }

        protected override void OnTick(float deltaTime)
        {
            if (Context == null || _camera == null || VirtualCamera == null)
            {
                return;
            }

            HandleInput();
        }

        private void OnEnable()
        {
            _isDragging = false;
        }

        private void HandleInput()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

#if ENABLE_INPUT_SYSTEM
            HandleInputSystem();
#else
            HandleInputManager();
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private void HandleInputSystem()
        {
            var touch = UnityEngine.InputSystem.Touchscreen.current;
            if (touch is { touches: { Count: > 0 } })
            {
                HandleTouchInput(touch.touches[0]);
                return;
            }

            if (requireMouseButton)
            {
                HandleMouseInput();
            }
        }

        private void HandleTouchInput(UnityEngine.InputSystem.Controls.TouchControl touch)
        {
            var currentPosition = touch.position.ReadValue();

            if (touch.press.wasPressedThisFrame)
            {
                _lastMousePosition = currentPosition;
                _isDragging = true;
            }
            else if (touch.press.isPressed && _isDragging)
            {
                var delta = _lastMousePosition - currentPosition;
                MoveCamera(delta);
                _lastMousePosition = currentPosition;
            }
            else if (touch.press.wasReleasedThisFrame)
            {
                _isDragging = false;
            }
        }

        private void HandleMouseInput()
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse == null)
            {
                return;
            }

            bool isPressed = mouseButtonIndex switch
            {
                0 => mouse.leftButton.isPressed,
                1 => mouse.rightButton.isPressed,
                2 => mouse.middleButton.isPressed,
                _ => false
            };

            if (isPressed)
            {
                if (!_isDragging)
                {
                    _isDragging = true;
                }

                var mouseDelta = mouse.delta.ReadValue();
                if (mouseDelta.magnitude > 0.1f)
                {
                    MoveCamera(-mouseDelta);
                }
            }
            else if (_isDragging)
            {
                _isDragging = false;
            }
        }
#else
        private void HandleInputManager()
        {
            if (Input.GetMouseButtonDown(mouseButtonIndex))
            {
                _lastMousePosition = Input.mousePosition;
                _isDragging = true;
            }
            else if (Input.GetMouseButton(mouseButtonIndex) && _isDragging)
            {
                var delta = _lastMousePosition - (Vector3)Input.mousePosition;
                MoveCamera(delta);
                _lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(mouseButtonIndex))
            {
                _isDragging = false;
            }
        }
#endif

        private void MoveCamera(Vector3 screenDelta)
        {
            if (!_camera.orthographic)
            {
                return;
            }

            var worldUnitsPerPixel = (_camera.orthographicSize * 2f) / Screen.height;
            var worldDelta = new Vector3(
                screenDelta.x * worldUnitsPerPixel * movementSpeed,
                screenDelta.y * worldUnitsPerPixel * movementSpeed,
                0f
            );

            if (worldDelta.magnitude < 0.001f)
            {
                return;
            }

            var virtualCameraTransform = VirtualCamera.transform;
            var newPosition = virtualCameraTransform.position + worldDelta;
            virtualCameraTransform.position = newPosition;

            var outputCamera = Context.Brain?.OutputCamera ?? Context.UnityCamera;
            if (outputCamera != null)
            {
                var outputTransform = outputCamera.transform;
                outputTransform.position = new Vector3(newPosition.x, newPosition.y, outputTransform.position.z);
            }
        }
    }
}

